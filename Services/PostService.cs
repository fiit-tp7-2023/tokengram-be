using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Neo4jClient.Extensions;
using Tokengram.Database.Indexer.Entities;
using Tokengram.Database.Tokengram;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Extensions;
using Tokengram.Models.CustomEntities;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.Exceptions;
using Tokengram.Models.QueryResults;
using Tokengram.Services.Interfaces;

namespace Tokengram.Services
{
    public partial class PostService : IPostService
    {
        private readonly TokengramDbContext _dbContext;

        private readonly IMapper _mapper;

        private readonly INFTService _nftService;

        public PostService(TokengramDbContext dbContext, IMapper mapper, INFTService nftService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _nftService = nftService;
        }

        public async Task<PostUserSettings> UpdatePostUserSettings(
            PostUserSettingsRequestDTO request,
            string postNFTAddress,
            string userAddress
        )
        {
            NFTOwner nftOwner =
                await _nftService.GetNFTOwner(postNFTAddress)
                ?? throw new NotFoundException(Constants.ErrorMessages.POST_NOT_FOUND);

            if (nftOwner.OwnerId != userAddress)
                throw new ForbiddenException(Constants.ErrorMessages.POST_NOT_OWNER);

            Post? post = await _dbContext.Posts
                .Include(x => x.PostUserSettings)
                .FirstOrDefaultAsync(x => x.NFTAddress == postNFTAddress);

            if (post == null)
            {
                post = new() { NFTAddress = postNFTAddress };
                await _dbContext.Posts.AddAsync(post);
            }

            PostUserSettings? postUserSettings = post.PostUserSettings.FirstOrDefault(
                x => x.UserAddress == userAddress
            );

            if (postUserSettings == null)
            {
                postUserSettings = new()
                {
                    Post = post,
                    UserAddress = userAddress,
                    IsVisible = request.IsVisible,
                    Description = request.Description
                };
                await _dbContext.PostUserSettings.AddAsync(postUserSettings);
            }
            else
            {
                postUserSettings.IsVisible = request.IsVisible;
                postUserSettings.Description = request.Description;
            }

            await _dbContext.SaveChangesAsync();

            return postUserSettings;
        }

        public async Task<IEnumerable<UserPost>> GetUserPosts(PaginationRequestDTO request, string userAddress)
        {
            User user = await _dbContext.Users.FirstAsync(x => x.Address == userAddress);
            IEnumerable<string> ownedNFTs = await _nftService.GetOwnedNFTs(request, userAddress);

            IEnumerable<Post> realPosts = await _dbContext.Posts
                .Include(x => x.PostUserSettings.Where(x => x.UserAddress == userAddress))
                .Where(x => x.NFTAddress.In(ownedNFTs))
                .ToListAsync();
            IEnumerable<Post> dummyPosts = ownedNFTs
                .Except(realPosts.Select(x => x.NFTAddress))
                .Select(nftAddress => new Post { NFTAddress = nftAddress });
            IEnumerable<Post> posts = realPosts.Concat(dummyPosts);
            posts = await FillPostsWithNFTs(posts);

            IEnumerable<string> likedPosts = await _dbContext.PostLikes
                .Where(x => x.LikerAddress == userAddress && posts.Select(x => x.NFTAddress).Contains(x.PostNFTAddress))
                .Select(x => x.PostNFTAddress)
                .ToListAsync();

            return posts.Select(x =>
            {
                PostUserSettings? postUserSettings = x.PostUserSettings.FirstOrDefault();
                UserPost userPost = _mapper.Map<UserPost>(x);
                userPost.IsLiked = likedPosts.Contains(x.NFTAddress);
                userPost.IsVisible = postUserSettings != null && postUserSettings.IsVisible;
                userPost.Description = postUserSettings?.Description;

                return userPost;
            });
        }

        private async Task<IEnumerable<Post>> FillPostsWithNFTs(IEnumerable<Post> posts)
        {
            IEnumerable<NFTQueryResult> nftQueryResults = await _nftService.GetNFTs(posts.Select(x => x.NFTAddress));
            List<Post> postsWithNFTs = new();

            foreach (Post post in posts)
            {
                NFTQueryResult? nftQueryResult = nftQueryResults.FirstOrDefault(x => x.NFT.Address == post.NFTAddress);

                if (nftQueryResult != null)
                {
                    post.NFTQueryResult = nftQueryResult;
                    postsWithNFTs.Add(post);
                }
            }

            return postsWithNFTs;
        }
    }
}
