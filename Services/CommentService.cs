using Microsoft.EntityFrameworkCore;
using Tokengram.Database.Tokengram;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Extensions;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.Exceptions;
using Tokengram.Models.CustomEntities;
using Tokengram.Services.Interfaces;
using AutoMapper;

namespace Tokengram.Services
{
    public partial class CommentService : ICommentService
    {
        private readonly TokengramDbContext _dbContext;
        private readonly IMapper _mapper;

        public CommentService(TokengramDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CommentWithUserContext>> GetCommentsWithUserContext(
            PaginationRequestDTO request,
            string postNFTAddress,
            string userAddress
        )
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
            IEnumerable<long> likedComments = await _dbContext.CommentLikes
                .Where(x => x.LikerAddress == userAddress && comments.Select(x => x.Id).Contains(x.CommentId))
                .Select(x => x.CommentId)
                .ToListAsync();

            return comments.Select(x =>
            {
                CommentWithUserContext commentWithUserContext = _mapper.Map<CommentWithUserContext>(x);
                commentWithUserContext.IsLiked = likedComments.Contains(x.Id);

                return commentWithUserContext;
            });
        }

        public async Task<IEnumerable<CommentWithUserContext>> GetCommentRepliesWithUserContext(
            PaginationRequestDTO request,
            long commentId,
            string userAddress
        )
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
            IEnumerable<long> likedComments = await _dbContext.CommentLikes
                .Where(x => x.LikerAddress == userAddress && comments.Select(x => x.Id).Contains(x.CommentId))
                .Select(x => x.CommentId)
                .ToListAsync();

            return comments.Select(x =>
            {
                CommentWithUserContext commentWithUserContext = _mapper.Map<CommentWithUserContext>(x);
                commentWithUserContext.IsLiked = likedComments.Contains(x.Id);

                return commentWithUserContext;
            });
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
