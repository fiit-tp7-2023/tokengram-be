using Microsoft.EntityFrameworkCore;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Extensions;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.Exceptions;
using Tokengram.Services.Interfaces;

namespace Tokengram.Services
{
    public partial class CommentService : ICommentService
    {
        public async Task<CommentLike> LikeComment(Comment comment, string userAddress)
        {
            CommentLike? commentLike = await _dbContext.CommentLikes.FirstOrDefaultAsync(
                x => x.CommentId == comment.Id && x.LikerAddress == userAddress
            );

            if (commentLike != null)
                throw new BadRequestException(Constants.ErrorMessages.COMMENT_ALREADY_LIKED);

            commentLike = new() { Comment = comment, LikerAddress = userAddress };
            await _dbContext.CommentLikes.AddAsync(commentLike);
            comment.LikeCount++;
            await _dbContext.SaveChangesAsync();

            return commentLike;
        }

        public async Task UnlikeComment(Comment comment, string userAddress)
        {
            CommentLike commentLike =
                await _dbContext.CommentLikes.FirstOrDefaultAsync(
                    x => x.CommentId == comment.Id && x.LikerAddress == userAddress
                ) ?? throw new BadRequestException(Constants.ErrorMessages.COMMENT_NOT_LIKED);

            _dbContext.CommentLikes.Remove(commentLike);
            comment.LikeCount--;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<CommentLike>> GetCommentLikes(PaginationRequestDTO request, Comment comment)
        {
            IEnumerable<CommentLike> commentLikes = await _dbContext.CommentLikes
                .Include(x => x.Liker)
                .Where(x => x.CommentId == comment.Id)
                .OrderBy(x => x.CreatedAt)
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync();

            return commentLikes;
        }
    }
}
