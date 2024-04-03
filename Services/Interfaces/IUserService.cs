using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Database.Neo4j.Nodes;

namespace Tokengram.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> UpdateUser(string userAddress, UserUpdateRequest request);

        Task<User> GetUser(string userAddress);

        Task<User> UpdateUserVectorByLike(User user, Post post);

        Task<User> UpdateUserVectorByUnlike(User user, Post post);

        Task<User> UpdateUserVectorByComment(User user, Post post);

        Task<User> UpdateUserVectorByUncomment(User user, Post post);

        Task<User> UpdateUserVectorByNewNFT(User user, NFTNode nft);
    }
}
