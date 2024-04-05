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
        public async Task<PostLike> LikePost(Post post, string userAddress)
        {
            PostLike? postLike = await _dbContext.PostLikes.FirstOrDefaultAsync(
                x => x.PostNFTAddress == post.NFTAddress && x.LikerAddress == userAddress
            );

            if (postLike != null)
                throw new BadRequestException(Constants.ErrorMessages.POST_ALREADY_LIKED);

            postLike = new() { Post = post, LikerAddress = userAddress };
            await _dbContext.PostLikes.AddAsync(postLike);
            post.LikeCount++;
            await _dbContext.SaveChangesAsync();

            return postLike;
        }

        public async Task UnlikePost(Post post, string userAddress)
        {
            PostLike postLike =
                await _dbContext.PostLikes.FirstOrDefaultAsync(
                    x => x.PostNFTAddress == post.NFTAddress && x.LikerAddress == userAddress
                ) ?? throw new BadRequestException(Constants.ErrorMessages.POST_NOT_LIKED);

            _dbContext.PostLikes.Remove(postLike);
            post.LikeCount--;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<PostLike>> GetPostLikes(PaginationRequestDTO request, Post post)
        {
            IEnumerable<PostLike> postLikes = await _dbContext.PostLikes
                .Include(x => x.Liker)
                .Where(x => x.PostNFTAddress == post.NFTAddress)
                .OrderBy(x => x.CreatedAt)
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync();

            return postLikes;
        }
    }
}
