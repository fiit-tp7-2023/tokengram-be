using Microsoft.EntityFrameworkCore;
using Tokengram.Constants;
using Tokengram.Database.Tokengram;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Extensions;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.Exceptions;
using Tokengram.Services.Interfaces;

namespace Tokengram.Services
{
    public class FollowerService : IFollowerService
    {
        private readonly TokengramDbContext _dbContext;

        public FollowerService(TokengramDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<UserFollow>> GetUserFollowers(string userAddress, PaginationRequestDTO request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Address == userAddress)
                ?? throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            return await _dbContext.UserFollows
                .Include(x => x.User)
                .Include(x => x.FollowedUser)
                .Where(x => x.FollowedUserAddress == userAddress)
                .OrderByDescending(x => x.CreatedAt)
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserFollow>> GetUserFollowings(string userAddress, PaginationRequestDTO request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Address == userAddress)
                ?? throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            return await _dbContext.UserFollows
                .Include(x => x.User)
                .Include(x => x.FollowedUser)
                .Where(x => x.UserAddress == userAddress)
                .OrderByDescending(x => x.CreatedAt)
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync();
        }

        public async Task<UserFollow> FollowUser(string userAddress, User targetUser)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Address == userAddress)
                ?? throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            if (user.Address == targetUser.Address)
            {
                throw new BadRequestException(ErrorMessages.CANNOT_FOLLOW_SELF);
            }

            var alreadyFollows = await _dbContext.UserFollows.AnyAsync(
                x => x.UserAddress == userAddress && x.FollowedUserAddress == targetUser.Address);
            if (alreadyFollows)
            {
                throw new BadRequestException(ErrorMessages.ALREADY_FOLLOWING);
            }

            var follow = new UserFollow { UserAddress = userAddress, FollowedUserAddress = targetUser.Address };

            await _dbContext.UserFollows.AddAsync(follow);
            await _dbContext.SaveChangesAsync();

            return follow;
        }

        public async Task UnfollowUser(string userAddress, User targetUser)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Address == userAddress)
                ?? throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            if (user.Address == targetUser.Address)
            {
                throw new BadRequestException(ErrorMessages.CANNOT_UNFOLLOW_SELF);
            }

            var follow = await _dbContext.UserFollows.FirstOrDefaultAsync(
                x => x.UserAddress == userAddress && x.FollowedUserAddress == targetUser.Address)
                ?? throw new BadRequestException(ErrorMessages.NOT_FOLLOWING);

            _dbContext.UserFollows.Remove(follow);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UnfollowUser(string userAddress, string targetUser)
        {
            var targetUserObj = await _dbContext.Users.FirstOrDefaultAsync(x => x.Address == targetUser)
                ?? throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            await this.UnfollowUser(userAddress, targetUserObj);
        }
    }
}
