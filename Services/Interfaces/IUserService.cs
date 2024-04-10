using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Database.Neo4j.Nodes;
using Tokengram.Models.CustomEntities;

namespace Tokengram.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserWithUserContext> GetUserProfile(User user, string requestingUserAddress);

        Task<User> UpdateUser(string userAddress, UserUpdateRequest request);

        Task<User> GetUser(string userAddress);

        Task<User> UpdateUserVectorByLike(User user, Post post);

        Task<User> UpdateUserVectorByUnlike(User user, Post post);

        Task<User> UpdateUserVectorByComment(User user, Post post);

        Task<User> UpdateUserVectorByUncomment(User user, Post post);

        Task<User> UpdateUserVectorByNewNFT(User user, NFTNode nft);

        public Task<IEnumerable<UserFollow>> GetUserFollowers(User user, PaginationRequestDTO request);
        public Task<IEnumerable<UserFollow>> GetUserFollowings(User user, PaginationRequestDTO request);
        public Task<UserFollow> FollowUser(string userAddress, User targetUser);
        public Task UnfollowUser(string userAddress, User targetUser);
        public Task UnfollowUser(string userAddress, string targetUser);
    }
}
