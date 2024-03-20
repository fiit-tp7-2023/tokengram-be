using System;
using System.Data.Entity;
using Tokengram.Constants;
using Tokengram.Database.Tokengram;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.DTOS.HTTP.Responses;
using Tokengram.Models.DTOS.Shared.Responses;
using Tokengram.Models.Exceptions;
using Tokengram.Services.Interfaces;

namespace Tokengram.Services
{
    public class ProfileService : IProfileService
    {
        private static Random random = new Random();
        private readonly TokengramDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public ProfileService(TokengramDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<User> ChangeProfileInfo(string userAddress, ChangeProfileInfoRequestDTO request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Address == userAddress);
            if (user == null)
            {
                throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);
            }

            if (request.Username != null)
            {
                bool alreadyUsed = await _dbContext.Users.AnyAsync(x => x.Username == request.Username);
                if (alreadyUsed)
                {
                    throw new ForbiddenException(ErrorMessages.USERNAME_ALREADY_TAKEN);
                }

                user.Username = request.Username;
            }

            string? oldProfilePicture = null;
            if (request.ProfilePictureFile != null)
            {
                string basePath = _configuration["PublicUploads:FileSystemPath"] ?? "/var/tokengram/storage/uploads/public/";
                string userFolder = Path.Combine(basePath, userAddress);

                Directory.CreateDirectory(userFolder);

                string fileName = GenerateUniqueFileName(userFolder, Path.GetExtension(request.ProfilePictureFile.FileName));
                string fileRelativePath = Path.Combine(userAddress, fileName);

                if (user.ProfilePicturePath != null)
                {
                    oldProfilePicture = Path.Combine(basePath, user.ProfilePicturePath);
                }

                using (var stream = new FileStream(Path.Combine(basePath, fileRelativePath), FileMode.Create))
                {
                    request.ProfilePictureFile.CopyTo(stream);
                }

                user.ProfilePicturePath = fileRelativePath;
            }

            await _dbContext.SaveChangesAsync();

            if (oldProfilePicture != null)
            {
                try
                {
                    File.Delete(oldProfilePicture);
                }
                catch { }
            }

            return user;
        }

        protected static string GenerateUniqueFileName(string directory, string extension, int length = 45)
        {
            string? fileName;
            do
            {
                fileName = GenerateRandomString(length) + extension;
            } while (File.Exists(Path.Combine(directory, fileName)));
            return fileName;
        }

        protected static string GenerateRandomString(int length = 45)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
            char[] fileName = new char[length];

            for (int i = 0; i < length; i++)
            {
                fileName[i] = chars[random.Next(chars.Length)];
            }

            return new string(fileName);
        }
    }
}
