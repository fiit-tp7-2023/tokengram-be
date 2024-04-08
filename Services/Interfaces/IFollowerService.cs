using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.HTTP.Requests;

namespace Tokengram.Services.Interfaces
{
    public interface IFollowerService
    {
        public Task<IEnumerable<UserFollow>> GetUserFollowers(User user, PaginationRequestDTO request);
        public Task<IEnumerable<UserFollow>> GetUserFollowings(User user, PaginationRequestDTO request);
        public Task<UserFollow> FollowUser(string userAddress, User targetUser);
        public Task UnfollowUser(string userAddress, User targetUser);
        public Task UnfollowUser(string userAddress, string targetUser);
    }
}
