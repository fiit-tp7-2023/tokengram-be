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
        private readonly TokengramDbContext _tokengramDbContext;
        private readonly IUserService _userService;

        public CommentService(TokengramDbContext tokengramDbContext, IUserService userService)
        {
            _tokengramDbContext = tokengramDbContext;
            _userService = userService;
        }

        public async Task<IEnumerable<CommentWithUserContext>> GetComments(
            PaginationRequestDTO request,
            Post post,
            string userAddress
        )
        {
            IEnumerable<CommentWithUserContext> comments = await _tokengramDbContext.Comments
                .Include(x => x.Commenter)
                .Where(x => x.PostNFTAddress == post.NFTAddress)
                .Select(
                    x =>
                        new CommentWithUserContext
                        {
                            Comment = x,
                            CommentReplyCount = x.CommentReplies.Count,
                            LikeCount = x.Likes.Count,
                            IsLiked = x.Likes.Any(x => x.LikerAddress == userAddress)
                        }
                )
                .OrderByDescending(x => x.LikeCount)
                .ThenBy(x => x.Comment.CreatedAt)
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync();

            return comments;
        }

        public async Task<IEnumerable<CommentWithUserContext>> GetCommentReplies(
            PaginationRequestDTO request,
            Comment comment,
            string userAddress
        )
        {
            IEnumerable<CommentWithUserContext> comments = await _tokengramDbContext.Comments
                .Include(x => x.Commenter)
                .Where(x => x.ParentCommentId == comment.Id)
                .OrderBy(x => x.CreatedAt)
                .Paginate(request.PageNumber, request.PageSize)
                .Select(
                    x =>
                        new CommentWithUserContext
                        {
                            Comment = x,
                            CommentReplyCount = x.CommentReplies.Count,
                            LikeCount = x.Likes.Count,
                            IsLiked = x.Likes.Any(x => x.LikerAddress == userAddress)
                        }
                )
                .ToListAsync();

            return comments;
        }

        public async Task<Comment> CreateComment(CommentRequestDTO request, Post post, string userAddress)
        {
            if (request.ParentCommentId != null)
            {
                Comment parrentComment =
                    await _tokengramDbContext.Comments.FirstOrDefaultAsync(
                        x => x.Id == request.ParentCommentId && x.PostNFTAddress == post.NFTAddress
                    ) ?? throw new NotFoundException(Constants.ErrorMessages.COMMENT_NOT_FOUND);
                if (parrentComment.ParentCommentId != null)
                    throw new BadRequestException(Constants.ErrorMessages.COMMENT_REPLY_PARENT_COMMENT_NOT_BASE);
            }

            Comment comment =
                new()
                {
                    Content = request.Content,
                    Post = post,
                    CommenterAddress = userAddress,
                    ParentCommentId = request.ParentCommentId
                };
            await _tokengramDbContext.Comments.AddAsync(comment);
            await _tokengramDbContext.SaveChangesAsync();

            User user = await _userService.GetUser(userAddress);
            await _userService.UpdateUserVectorByComment(user, post);

            return comment;
        }

        public async Task<CommentWithUserContext> UpdateComment(
            CommentUpdateRequestDTO request,
            Comment comment,
            string userAddress
        )
        {
            if (comment.CommenterAddress != userAddress)
                throw new ForbiddenException(Constants.ErrorMessages.COMMENT_NOT_COMMENTER);

            comment.Content = request.Content;
            await _tokengramDbContext.SaveChangesAsync();

            CommentWithUserContext commentWithUserContext = await _tokengramDbContext.Comments
                .Include(x => x.Commenter)
                .Select(
                    x =>
                        new CommentWithUserContext
                        {
                            Comment = x,
                            CommentReplyCount = x.CommentReplies.Count,
                            LikeCount = x.Likes.Count,
                            IsLiked = x.Likes.Any(x => x.LikerAddress == userAddress)
                        }
                )
                .FirstAsync(x => x.Comment.Id == comment.Id);

            return commentWithUserContext;
        }

        public async Task DeleteComment(Comment comment, string userAddress)
        {
            comment = await _tokengramDbContext.Comments
                .Include(x => x.Post)
                .Include(x => x.ParentComment)
                .FirstAsync(x => x.Id == comment.Id);
            if (comment.CommenterAddress != userAddress)
                throw new ForbiddenException(Constants.ErrorMessages.COMMENT_NOT_COMMENTER);

            _tokengramDbContext.Comments.Remove(comment);
            await _tokengramDbContext.SaveChangesAsync();

            User user = await _userService.GetUser(userAddress);
            await _userService.UpdateUserVectorByUncomment(user, comment.Post);
        }
    }
}
