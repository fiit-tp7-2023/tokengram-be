using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.CustomEntities;
using Tokengram.Models.DTOS.HTTP.Requests;

namespace Tokengram.Services.Interfaces
{
    public interface IPostService
    {
        Task<PostWithUserContext> UpdatePostUserSettings(
            PostUserSettingsRequestDTO request,
            string postNFTAddress,
            string userAddress
        );

        Task<IEnumerable<PostWithUserContext>> GetUserPosts(
            PaginationRequestDTO request,
            string userAddress,
            bool? isVisible
        );

        Task<PostLike> LikePost(Post post, string userAddress);

        Task UnlikePost(Post post, string userAddress);

        Task<IEnumerable<PostLike>> GetPostLikes(PaginationRequestDTO request, Post post);

        Task<IEnumerable<PostWithUserContext>> GetHotPosts(User? user, PaginationRequestDTO request);
    }
}
