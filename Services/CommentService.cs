using Microsoft.EntityFrameworkCore;
using Tokengram.Database.Tokengram;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Extensions;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.Exceptions;
using Tokengram.Services.Interfaces;

namespace Tokengram.Services
{
    public partial class CommentService : ICommentService
    {
        private readonly TokengramDbContext _dbContext;

        public CommentService(TokengramDbContext dbContext, INFTService nftService)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Comment>> GetComments(PaginationRequestDTO request, string postNFTAddress)
        {
            Post post =
                await _dbContext.Posts.FirstOrDefaultAsync(x => x.NFTAddress == postNFTAddress)
                ?? throw new NotFoundException(Constants.ErrorMessages.POST_NOT_FOUND);
            IEnumerable<Comment> comments = await _dbContext.Comments
                .Include(x => x.Commenter)
                .Where(x => x.PostNFTAddress == postNFTAddress)
                .OrderByDescending(x => x.LikeCount)
                .ThenBy(x => x.CreatedAt)
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync();

            return comments;
        }

        public async Task<IEnumerable<Comment>> GetCommentReplies(PaginationRequestDTO request, long commentId)
        {
            Comment comment =
                await _dbContext.Comments.FirstOrDefaultAsync(x => x.Id == commentId)
                ?? throw new NotFoundException(Constants.ErrorMessages.COMMENT_NOT_FOUND);
            IEnumerable<Comment> comments = await _dbContext.Comments
                .Include(x => x.Commenter)
                .Where(x => x.ParentCommentId == commentId)
                .OrderBy(x => x.CreatedAt)
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync();

            return comments;
        }

        public async Task<Comment> CreateComment(CommentRequestDTO request, string postNFTAddress, string userAddress)
        {
            Post post =
                await _dbContext.Posts.FirstOrDefaultAsync(x => x.NFTAddress == postNFTAddress)
                ?? throw new NotFoundException(Constants.ErrorMessages.POST_NOT_FOUND);

            if (request.ParentCommentId != null)
            {
                Comment parrentComment =
                    await _dbContext.Comments.FirstOrDefaultAsync(
                        x => x.Id == request.ParentCommentId && x.PostNFTAddress == postNFTAddress
                    ) ?? throw new NotFoundException(Constants.ErrorMessages.COMMENT_NOT_FOUND);
                if (parrentComment.ParentCommentId != null)
                    throw new BadRequestException(Constants.ErrorMessages.COMMENT_REPLY_PARENT_COMMENT_NOT_BASE);
                parrentComment.CommentReplyCount++;
            }

            Comment comment =
                new()
                {
                    Content = request.Content,
                    Post = post,
                    CommenterAddress = userAddress,
                    ParentCommentId = request.ParentCommentId
                };
            await _dbContext.Comments.AddAsync(comment);
            post.CommentCount++;
            await _dbContext.SaveChangesAsync();

            return comment;
        }

        public async Task<Comment> UpdateComment(CommentUpdateRequestDTO request, long commentId, string userAddress)
        {
            Comment comment =
                await _dbContext.Comments.FirstOrDefaultAsync(x => x.Id == commentId)
                ?? throw new NotFoundException(Constants.ErrorMessages.COMMENT_NOT_FOUND);
            if (comment.CommenterAddress != userAddress)
                throw new ForbiddenException(Constants.ErrorMessages.COMMENT_NOT_COMMENTER);

            comment.Content = request.Content;
            await _dbContext.SaveChangesAsync();

            return comment;
        }

        public async Task DeleteComment(long commentId, string userAddress)
        {
            Comment comment =
                await _dbContext.Comments
                    .Include(x => x.Post)
                    .Include(x => x.ParentComment)
                    .FirstOrDefaultAsync(x => x.Id == commentId)
                ?? throw new NotFoundException(Constants.ErrorMessages.COMMENT_NOT_FOUND);
            if (comment.CommenterAddress != userAddress)
                throw new ForbiddenException(Constants.ErrorMessages.COMMENT_NOT_COMMENTER);

            _dbContext.Comments.Remove(comment);
            comment.Post.CommentCount = comment.Post.CommentCount - (1 + comment.CommentReplyCount);
            if (comment.ParentComment != null)
                comment.ParentComment.CommentReplyCount--;
            await _dbContext.SaveChangesAsync();
        }
    }
}
