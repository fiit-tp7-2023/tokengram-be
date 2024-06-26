using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.CustomEntities;
using Tokengram.Models.DTOS.HTTP.Requests;

namespace Tokengram.Services.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentWithUserContext>> GetComments(
            PaginationRequestDTO request,
            Post post,
            string userAddress
        );

        Task<IEnumerable<CommentWithUserContext>> GetCommentReplies(
            PaginationRequestDTO request,
            Comment comment,
            string userAddress
        );

        Task<Comment> CreateComment(CommentRequestDTO request, Post post, string userAddress);

        Task<CommentWithUserContext> UpdateComment(
            CommentUpdateRequestDTO request,
            Comment comment,
            string userAddress
        );

        Task DeleteComment(Comment comment, string userAddress);

        Task<CommentLike> LikeComment(Comment comment, string userAddress);

        Task UnlikeComment(Comment comment, string userAddress);

        Task<IEnumerable<CommentLike>> GetCommentLikes(PaginationRequestDTO request, Comment comment);
    }
}
