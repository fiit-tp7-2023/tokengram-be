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
            PostLike? postLike = await _tokengramDbContext.PostLikes.FirstOrDefaultAsync(
                x => x.PostNFTAddress == post.NFTAddress && x.LikerAddress == userAddress
            );

            if (postLike != null)
                throw new BadRequestException(Constants.ErrorMessages.POST_ALREADY_LIKED);

            User user = await _userService.GetUser(userAddress);
            await _userService.UpdateUserVectorByLike(user, post);

            postLike = new() { Post = post, Liker = user };
            await _tokengramDbContext.PostLikes.AddAsync(postLike);
            await _tokengramDbContext.SaveChangesAsync();

            return postLike;
        }

        public async Task UnlikePost(Post post, string userAddress)
        {
            PostLike postLike =
                await _tokengramDbContext.PostLikes.FirstOrDefaultAsync(
                    x => x.PostNFTAddress == post.NFTAddress && x.LikerAddress == userAddress
                ) ?? throw new BadRequestException(Constants.ErrorMessages.POST_NOT_LIKED);

            User user = await _userService.GetUser(userAddress);
            await _userService.UpdateUserVectorByUnlike(user, post);

            _tokengramDbContext.PostLikes.Remove(postLike);
            await _tokengramDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<PostLike>> GetPostLikes(PaginationRequestDTO request, Post post)
        {
            IEnumerable<PostLike> postLikes = await _tokengramDbContext.PostLikes
                .Include(x => x.Liker)
                .Where(x => x.PostNFTAddress == post.NFTAddress)
                .OrderBy(x => x.CreatedAt)
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync();

            return postLikes;
        }
    }
}
