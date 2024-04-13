using Microsoft.EntityFrameworkCore;
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
        private readonly TokengramDbContext _tokengramDbContext;

        private readonly INFTService _nftService;

        private readonly IUserService _userService;

        public PostService(TokengramDbContext tokengramDbContext, INFTService nftService, IUserService userService)
        {
            _tokengramDbContext = tokengramDbContext;
            _nftService = nftService;
            _userService = userService;
        }

        public async Task<PostWithUserContext> UpdatePostUserSettings(
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

            PostWithUserContext? post = await _tokengramDbContext.Posts
                .Include(x => x.PostUserSettings)
                .Select(
                    x =>
                        new PostWithUserContext
                        {
                            OwnerAddress = userAddress,
                            Post = x,
                            LikeCount = x.Likes.Count,
                            CommentCount = x.Comments.Count,
                            IsLiked = x.Likes.Any(x => x.LikerAddress == userAddress)
                        }
                )
                .FirstOrDefaultAsync(x => x.Post.NFTAddress == postNFTAddress);

            if (post == null)
            {
                Post newPost = new() { NFTAddress = postNFTAddress };
                await _tokengramDbContext.Posts.AddAsync(newPost);
                post = new PostWithUserContext { Post = newPost, OwnerAddress = userAddress };
            }

            if (post.Post.PostUserSettings == null || post.Post.PostUserSettings.UserAddress != userAddress)
            {
                if (post.Post.PostUserSettings != null && post.Post.PostUserSettings.UserAddress != userAddress)
                    _tokengramDbContext.PostUserSettings.Remove(post.Post.PostUserSettings);

                PostUserSettings postUserSettings =
                    new()
                    {
                        Post = post.Post,
                        UserAddress = userAddress,
                        IsVisible = request.IsVisible,
                        Description = request.Description
                    };
                await _tokengramDbContext.PostUserSettings.AddAsync(postUserSettings);
                post.Post.PostUserSettings = postUserSettings;
            }
            else
            {
                post.Post.PostUserSettings.IsVisible = request.IsVisible;
                post.Post.PostUserSettings.Description = request.Description;
            }

            await _tokengramDbContext.SaveChangesAsync();

            post = await FillPostWithNFT(post);

            return post;
        }

        public async Task<IEnumerable<PostWithUserContext>> GetUserPosts(
            PaginationRequestDTO request,
            string userAddress,
            bool? isVisible
        )
        {
            User user = await _tokengramDbContext.Users.FirstAsync(x => x.Address == userAddress);
            IEnumerable<string> ownedNFTs = await _nftService.GetOwnedNFTs(request, userAddress);
            List<PostWithUserContext> posts = new();

            if (isVisible == null || isVisible.Value)
            {
                posts = await _tokengramDbContext.Posts
                    .Include(x => x.PostUserSettings)
                    .Where(x => ownedNFTs.Contains(x.NFTAddress))
                    .WhereIf(isVisible != null, x => x.PostUserSettings.IsVisible == isVisible)
                    .OrderByDescending(x => x.PostUserSettings.CreatedAt)
                    .Select(
                        x =>
                            new PostWithUserContext
                            {
                                Post = x,
                                CommentCount = x.Comments.Count,
                                LikeCount = x.Likes.Count,
                                IsLiked = x.Likes.Any(x => x.LikerAddress == userAddress),
                                OwnerAddress = userAddress
                            }
                    )
                    .ToListAsync();
            }

            if (isVisible == null || !isVisible.Value)
            {
                List<PostWithUserContext> dummyPosts = ownedNFTs
                    .Except(posts.Select(x => x.Post.NFTAddress))
                    .Select(
                        nftAddress =>
                            new PostWithUserContext
                            {
                                Post = new Post { NFTAddress = nftAddress },
                                OwnerAddress = userAddress
                            }
                    )
                    .ToList();
                posts.AddRange(dummyPosts);
            }

            posts = (await FillPostsWithNFTs(posts)).ToList();

            // if only visible posts are returned, they will be ordered by the time of visibility setting
            if (isVisible != null && isVisible.Value)
                return posts;

            Dictionary<string, PostWithUserContext> postDictionary = posts.ToDictionary(
                post => post.Post.NFTAddress,
                post => post
            );

            // if all posts are returned, they will be ordered by the time of acquisition
            return ownedNFTs.Where(postDictionary.ContainsKey).Select(nftAddress => postDictionary[nftAddress]);
        }

        private async Task<PostWithUserContext> FillPostWithNFT(PostWithUserContext post)
        {
            NFTQueryResult nftQueryResult = await _nftService.GetNFT(post.Post.NFTAddress);
            if (nftQueryResult != null)
                post.Post.NFTQueryResult = nftQueryResult;

            return post;
        }

        private async Task<IEnumerable<PostWithUserContext>> FillPostsWithNFTs(IEnumerable<PostWithUserContext> posts)
        {
            IEnumerable<NFTQueryResult> nftQueryResults = await _nftService.GetNFTs(
                posts.Select(x => x.Post.NFTAddress)
            );
            List<PostWithUserContext> postsWithNFTs = new();

            foreach (PostWithUserContext post in posts)
            {
                NFTQueryResult? nftQueryResult = nftQueryResults.FirstOrDefault(
                    x => x.NFT.Address == post.Post.NFTAddress
                );

                if (nftQueryResult != null)
                {
                    post.Post.NFTQueryResult = nftQueryResult;
                    postsWithNFTs.Add(post);
                }
            }

            return postsWithNFTs;
        }
    }
}
