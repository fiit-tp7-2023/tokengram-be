using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.CustomEntities;
using Tokengram.Models.DTOS.HTTP.Requests;

namespace Tokengram.Services.Interfaces
{
    public interface IRecommendationService
    {
        // get hot posts
        Task<IEnumerable<PostWithCosineSimilarity>> GetHotPosts(User user, PaginationRequestDTO request);
    }
}
