using Microsoft.EntityFrameworkCore;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Extensions;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.Exceptions;
using Tokengram.Services.Interfaces;

namespace Tokengram.Services
{
    public partial class PostService : IPostService
    {
        public async Task<PostLike> LikePost(string postNFTAddress, string userAddress)
        {
            Post post =
                await _dbContext.Posts.FirstOrDefaultAsync(x => x.NFTAddress == postNFTAddress)
                ?? throw new NotFoundException(Constants.ErrorMessages.POST_NOT_FOUND);
            PostLike? postLike = await _dbContext.PostLikes.FirstOrDefaultAsync(
                x => x.PostNFTAddress == postNFTAddress && x.LikerAddress == userAddress
            );

            User user = await _userService.GetUser(userAddress);
            await _userService.UpdateUserVectorByLike(user, post);

            if (postLike != null)
                throw new BadRequestException(Constants.ErrorMessages.POST_ALREADY_LIKED);

            postLike = new() { Post = post, LikerAddress = userAddress };
            await _dbContext.PostLikes.AddAsync(postLike);
            post.LikeCount++;
            await _dbContext.SaveChangesAsync();

            return postLike;
        }

        public async Task UnlikePost(string postNFTAddress, string userAddress)
        {
            Post post =
                await _dbContext.Posts.FirstOrDefaultAsync(x => x.NFTAddress == postNFTAddress)
                ?? throw new NotFoundException(Constants.ErrorMessages.POST_NOT_FOUND);
            PostLike postLike =
                await _dbContext.PostLikes.FirstOrDefaultAsync(
                    x => x.PostNFTAddress == postNFTAddress && x.LikerAddress == userAddress
                ) ?? throw new BadRequestException(Constants.ErrorMessages.POST_NOT_LIKED);

            User user = await _userService.GetUser(userAddress);
            await _userService.UpdateUserVectorByUnlike(user, post);

            _dbContext.PostLikes.Remove(postLike);
            post.LikeCount--;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<PostLike>> GetPostLikes(PaginationRequestDTO request, string postNFTAddress)
        {
            Post post =
                await _dbContext.Posts.FirstOrDefaultAsync(x => x.NFTAddress == postNFTAddress)
                ?? throw new NotFoundException(Constants.ErrorMessages.POST_NOT_FOUND);
            IEnumerable<PostLike> postLikes = await _dbContext.PostLikes
                .Include(x => x.Liker)
                .Where(x => x.PostNFTAddress == postNFTAddress)
                .OrderBy(x => x.CreatedAt)
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync();

            return postLikes;
        }
    }
}
