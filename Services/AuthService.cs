using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nethereum.Signer;
using Nethereum.Util;
using Tokengram.Models.Config;
using Tokengram.Database.Postgres;
using Tokengram.Database.Postgres.Models;
using Tokengram.DTOS.Requests;
using Tokengram.DTOS.Responses;
using Tokengram.Services.Interfaces;
using Tokengram.Models.Exceptions;
using Microsoft.Extensions.Options;
using Tokengram.Constants;

namespace Tokengram.Services
{
    public class AuthService : IAuthService
    {
        private readonly TokengramDbContext _dbContext;
        private readonly EthereumMessageSigner _signer;
        private readonly JWTOptions _jwtOptions;

        public AuthService(TokengramDbContext dbContext, IOptions<JWTOptions> jWTOptions)
        {
            _dbContext = dbContext;
            _signer = new EthereumMessageSigner();
            _jwtOptions = jWTOptions.Value;
        }

        public async Task<string> GenerateNonceMessage(NonceRequestDTO request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.PublicAddress == request.PublicAddress);

            if (user != null)
                return user.GetNonceMessage();

            user = new User { PublicAddress = request.PublicAddress, Nonce = Guid.NewGuid() };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return user.GetNonceMessage();
        }

        public async Task<TokensResponseDTO> Login(LoginRequestDTO request)
        {
            var user =
                await _dbContext.Users.FirstOrDefaultAsync(x => x.PublicAddress == request.PublicAddress)
                ?? throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);
            var signerAddress = "";

            try
            {
                signerAddress = _signer.EncodeUTF8AndEcRecover(user.GetNonceMessage(), request.Signature);
            }
            catch
            {
                throw new BadRequestException(ErrorMessages.SIGNATURE_INVALID);
            }

            if (!AddressUtil.Current.AreAddressesTheSame(user.PublicAddress, signerAddress))
                throw new BadRequestException(ErrorMessages.SIGNATURE_INVALID);

            var x = user.Nonce = Guid.NewGuid();
            await _dbContext.SaveChangesAsync();

            return GenerateTokens(user);
        }

        public async Task<TokensResponseDTO> RefreshToken(RefreshTokenRequestDTO request)
        {
            var refreshToken =
                await _dbContext.RefreshTokens
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Token == request.RefreshToken)
                ?? throw new BadRequestException(ErrorMessages.REFRESH_TOKEN_INVALID);

            if (refreshToken.ExpiresAt < DateTime.Now)
                throw new BadRequestException(ErrorMessages.REFRESH_TOKEN_EXPIRED);

            if (refreshToken.BlackListedAt != null)
                throw new BadRequestException(ErrorMessages.REFRESH_TOKEN_BLACKLISTED);

            refreshToken.BlackListedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return GenerateTokens(refreshToken.User);
        }

        private TokensResponseDTO GenerateTokens(User user)
        {
            return new TokensResponseDTO
            {
                AccessToken = GenerateAccessToken(user),
                RefreshToken = GenerateRefreshToken(user)
            };
        }

        private string GenerateRefreshToken(User user)
        {
            RefreshToken refreshToken =
                new()
                {
                    UserId = user.Id,
                    Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                    ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenValidityInDays)
                };
            _dbContext.Add(refreshToken);
            _dbContext.SaveChanges();

            return refreshToken.Token;
        }

        private string GenerateAccessToken(User user)
        {
            byte[] key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);

            Claim[] claims = new Claim[]
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.Actor, user.PublicAddress),
            };

            SecurityTokenDescriptor tokenDescriptor =
                new()
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.TokenValidityInMinutes),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    ),
                    Issuer = _jwtOptions.ValidIssuer,
                    IssuedAt = DateTime.UtcNow
                };

            JwtSecurityTokenHandler tokenHandler = new();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }
    }
}
