using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.HTTP.Requests;

namespace Tokengram.Services.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<Comment>> GetComments(PaginationRequestDTO request, string postNFTAddress);

        Task<IEnumerable<Comment>> GetCommentReplies(PaginationRequestDTO request, long commentId);

        Task<Comment> CreateComment(CommentRequestDTO request, string postNFTAddress, string userAddress);

        Task<Comment> UpdateComment(CommentUpdateRequestDTO request, long commentId, string userAddress);

        Task DeleteComment(long commentId, string userAddress);

        Task<CommentLike> LikeComment(long commentId, string userAddress);

        Task UnlikeComment(long commentId, string userAddress);

        Task<IEnumerable<CommentLike>> GetCommentLikes(PaginationRequestDTO request, long commentId);
    }
}
