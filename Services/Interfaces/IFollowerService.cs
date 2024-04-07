using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.HTTP.Requests;

namespace Tokengram.Services.Interfaces
{
    public interface IFollowerService
    {
        public Task<IEnumerable<UserFollow>> GetUserFollowers(string userAddress, PaginationRequestDTO request);
        public Task<IEnumerable<UserFollow>> GetUserFollowings(string userAddress, PaginationRequestDTO request);
        public Task<UserFollow> FollowUser(string userAddress, User targetUser);
        public Task UnfollowUser(string userAddress, User targetUser);
        public Task UnfollowUser(string userAddress, string targetUser);
    }
}
