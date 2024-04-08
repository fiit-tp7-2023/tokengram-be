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

        private readonly IUserService _userService;

        public CommentService(TokengramDbContext dbContext, IMapper mapper, IUserService userService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<IEnumerable<CommentWithUserContext>> GetCommentsWithUserContext(
            PaginationRequestDTO request,
            Post post,
            string userAddress
        )
        {
            IEnumerable<Comment> comments = await _dbContext.Comments
                .Include(x => x.Commenter)
                .Where(x => x.PostNFTAddress == post.NFTAddress)
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
            Comment comment,
            string userAddress
        )
        {
            IEnumerable<Comment> comments = await _dbContext.Comments
                .Include(x => x.Commenter)
                .Where(x => x.ParentCommentId == comment.Id)
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

        public async Task<Comment> CreateComment(CommentRequestDTO request, Post post, string userAddress)
        {
            if (request.ParentCommentId != null)
            {
                Comment parrentComment =
                    await _dbContext.Comments.FirstOrDefaultAsync(
                        x => x.Id == request.ParentCommentId && x.PostNFTAddress == post.NFTAddress
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

            User user = await _userService.GetUser(userAddress);
            await _userService.UpdateUserVectorByComment(user, post);

            return comment;
        }

        public async Task<Comment> UpdateComment(CommentUpdateRequestDTO request, Comment comment, string userAddress)
        {
            if (comment.CommenterAddress != userAddress)
                throw new ForbiddenException(Constants.ErrorMessages.COMMENT_NOT_COMMENTER);

            comment.Content = request.Content;
            await _dbContext.SaveChangesAsync();

            return comment;
        }

        public async Task DeleteComment(Comment comment, string userAddress)
        {
            comment = await _dbContext.Comments
                .Include(x => x.Post)
                .Include(x => x.ParentComment)
                .FirstAsync(x => x.Id == comment.Id);
            if (comment.CommenterAddress != userAddress)
                throw new ForbiddenException(Constants.ErrorMessages.COMMENT_NOT_COMMENTER);

            _dbContext.Comments.Remove(comment);
            comment.Post.CommentCount = comment.Post.CommentCount - (1 + comment.CommentReplyCount);
            if (comment.ParentComment != null)
                comment.ParentComment.CommentReplyCount--;
            await _dbContext.SaveChangesAsync();

            User user = await _userService.GetUser(userAddress);
            await _userService.UpdateUserVectorByUncomment(user, comment.Post);
        }
    }
}
