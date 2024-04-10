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
            CommentLike? commentLike = await _tokengramDbContext.CommentLikes.FirstOrDefaultAsync(
                x => x.CommentId == comment.Id && x.LikerAddress == userAddress
            );

            if (commentLike != null)
                throw new BadRequestException(Constants.ErrorMessages.COMMENT_ALREADY_LIKED);

            User liker = await _tokengramDbContext.Users.FirstAsync(x => x.Address == userAddress);

            commentLike = new() { Comment = comment, Liker = liker };
            await _tokengramDbContext.CommentLikes.AddAsync(commentLike);
            await _tokengramDbContext.SaveChangesAsync();

            return commentLike;
        }

        public async Task UnlikeComment(Comment comment, string userAddress)
        {
            CommentLike commentLike =
                await _tokengramDbContext.CommentLikes.FirstOrDefaultAsync(
                    x => x.CommentId == comment.Id && x.LikerAddress == userAddress
                ) ?? throw new BadRequestException(Constants.ErrorMessages.COMMENT_NOT_LIKED);

            _tokengramDbContext.CommentLikes.Remove(commentLike);
            await _tokengramDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<CommentLike>> GetCommentLikes(PaginationRequestDTO request, Comment comment)
        {
            IEnumerable<CommentLike> commentLikes = await _tokengramDbContext.CommentLikes
                .Include(x => x.Liker)
                .Where(x => x.CommentId == comment.Id)
                .OrderBy(x => x.CreatedAt)
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync();

            return commentLikes;
        }
    }
}
