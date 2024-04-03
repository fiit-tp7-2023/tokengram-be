using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nethereum.Signer;
using Nethereum.Util;
using Tokengram.Models.Config;
using Tokengram.Database.Tokengram;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.DTOS.HTTP.Responses;
using Tokengram.Services.Interfaces;
using Tokengram.Models.Exceptions;
using Microsoft.Extensions.Options;
using Tokengram.Constants;
using System.IdentityModel.Tokens.Jwt;
using Tokengram.Models.QueryResults;

namespace Tokengram.Services
{
    public class AuthService : IAuthService
    {
        private readonly TokengramDbContext _dbContext;
        private readonly EthereumMessageSigner _signer;
        private readonly JWTOptions _jwtOptions;

        private readonly INFTService _nftService;

        private readonly IUserService _userService;

        public AuthService(
            TokengramDbContext dbContext,
            IOptions<JWTOptions> jWTOptions,
            INFTService nftService,
            IUserService userService
        )
        {
            _dbContext = dbContext;
            _signer = new EthereumMessageSigner();
            _userService = userService;
            _nftService = nftService;
            _jwtOptions = jWTOptions.Value;
        }

        public async Task<NonceMessageResponseDTO> GenerateNonceMessage(NonceRequestDTO request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Address == request.Address);

            if (user != null)
                return new NonceMessageResponseDTO { Message = user.GetNonceMessage() };

            user = new User { Address = request.Address, Nonce = Guid.NewGuid() };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var allNfts = await _nftService.GetOwnedNFTs(
                new PaginationRequestDTO { PageNumber = 1, PageSize = 100 },
                request.Address
            );

            var newNfts = allNfts.Where(nft => !_dbContext.PostUserSettings.Any(pus => pus.PostNFTAddress == nft));

            foreach (var nft in newNfts)
            {
                var nftQueryResults = await FetchNFTQueryResults(nft);
                await _userService.UpdateUserVectorByNewNFT(user, nftQueryResults.NFT);
            }

            return new NonceMessageResponseDTO { Message = user.GetNonceMessage() };
        }

        private async Task<NFTQueryResult> FetchNFTQueryResults(string nftAddress)
        {
            return await _nftService.GetNFT(nftAddress);
        }

        public async Task<TokensResponseDTO> Login(LoginRequestDTO request)
        {
            var user =
                await _dbContext.Users.FirstOrDefaultAsync(x => x.Address == request.Address)
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

            if (!AddressUtil.Current.AreAddressesTheSame(user.Address, signerAddress))
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
                    UserAddress = user.Address,
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

            Claim[] claims = new Claim[] { new(ClaimTypes.NameIdentifier, user.Address), };

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
