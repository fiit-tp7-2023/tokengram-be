using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.CustomEntities;
using Tokengram.Models.DTOS.HTTP.Requests;

namespace Tokengram.Services.Interfaces
{
    public interface IPostService
    {
        Task<PostUserSettings> UpdatePostUserSettings(
            PostUserSettingsRequestDTO request,
            string postNFTAddress,
            string userAddress
        );

        Task<IEnumerable<UserPost>> GetUserPosts(PaginationRequestDTO request, string userAddress);

        Task<PostLike> LikePost(Post post, string userAddress);

        Task UnlikePost(Post post, string userAddress);

        Task<IEnumerable<PostLike>> GetPostLikes(PaginationRequestDTO request, Post post);
    }
}
