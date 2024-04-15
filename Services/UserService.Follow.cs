using Microsoft.EntityFrameworkCore;
using Tokengram.Constants;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Extensions;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.Exceptions;
using Tokengram.Services.Interfaces;

namespace Tokengram.Services
{
    public partial class UserService : IUserService
    {
        public async Task<IEnumerable<UserFollow>> GetUserFollowers(User user, PaginationRequestDTO request)
        {
            return await _tokengramDbContext.UserFollows
                .Include(x => x.Follower)
                .Where(x => x.FollowedUserAddress == user.Address)
                .OrderByDescending(x => x.CreatedAt)
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserFollow>> GetUserFollowings(User user, PaginationRequestDTO request)
        {
            return await _tokengramDbContext.UserFollows
                .Include(x => x.FollowedUser)
                .Where(x => x.FollowerAddress == user.Address)
                .OrderByDescending(x => x.CreatedAt)
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync();
        }

        public async Task<UserFollow> FollowUser(string userAddress, User targetUser)
        {
            var user =
                await _tokengramDbContext.Users.FirstOrDefaultAsync(x => x.Address == userAddress)
                ?? throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            if (user.Address == targetUser.Address)
                throw new BadRequestException(ErrorMessages.CANNOT_FOLLOW_SELF);

            var alreadyFollows = await _tokengramDbContext.UserFollows.AnyAsync(
                x => x.FollowerAddress == userAddress && x.FollowedUserAddress == targetUser.Address
            );
            if (alreadyFollows)
                throw new BadRequestException(ErrorMessages.ALREADY_FOLLOWING);

            var follow = new UserFollow { Follower = user, FollowedUser = targetUser };

            await _tokengramDbContext.UserFollows.AddAsync(follow);
            await _tokengramDbContext.SaveChangesAsync();

            return follow;
        }

        public async Task UnfollowUser(string userAddress, User targetUser)
        {
            var user =
                await _tokengramDbContext.Users.FirstOrDefaultAsync(x => x.Address == userAddress)
                ?? throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            if (user.Address == targetUser.Address)
                throw new BadRequestException(ErrorMessages.CANNOT_UNFOLLOW_SELF);

            var follow =
                await _tokengramDbContext.UserFollows.FirstOrDefaultAsync(
                    x => x.FollowerAddress == userAddress && x.FollowedUserAddress == targetUser.Address
                ) ?? throw new BadRequestException(ErrorMessages.NOT_FOLLOWING);

            _tokengramDbContext.UserFollows.Remove(follow);
            await _tokengramDbContext.SaveChangesAsync();
        }

        public async Task UnfollowUser(string userAddress, string targetUserAddress)
        {
            var targetUser =
                await _tokengramDbContext.Users.FirstOrDefaultAsync(x => x.Address == targetUserAddress)
                ?? throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            await UnfollowUser(userAddress, targetUser);
        }
    }
}
