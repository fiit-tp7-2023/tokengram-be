using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.HTTP.Requests;

namespace Tokengram.Services.Interfaces
{
    public interface IPostService
    {
        Task<Post> UpdatePostUserSettings(
            PostUserSettingsRequestDTO request,
            string postNFTAddress,
            string userAddress
        );

        Task<IEnumerable<Post>> GetOwnedPosts(PaginationRequestDTO request, string userAddress);

        Task<PostLike> LikePost(string postNFTAddress, string userAddress);

        Task UnlikePost(string postNFTAddress, string userAddress);

        Task<IEnumerable<PostLike>> GetPostLikes(PaginationRequestDTO request, string postNFTAddress);
    }
}
