using Microsoft.EntityFrameworkCore;
using Tokengram.Database.Indexer.Entities;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.CustomEntities;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.QueryResults;
using Tokengram.Services.Interfaces;
using Tokengram.Utils;

namespace Tokengram.Services
{
    public partial class PostService : IPostService
    {
        private const int RANDOM_POSTS_SUBSET_COUNT = 1000;

        public async Task<IEnumerable<PostWithUserContext>> GetHotPosts(User? user, PaginationRequestDTO request)
        {
            // random subset of visible posts (according to last owner)
            List<PostRecommendationView> posts = user == null ?
                (await GetNewestPostsSubset()).ToList() : (await GetRandomPostsSubset()).ToList();

            // nfts with vectors
            IEnumerable<NFTWithVectorQueryResult> nftsWithVectors = await _nftService.FillNFTsWithVector(
                posts.Select(x => x.NFTAddress)
            );

            // concat the results
            foreach (var post in posts)
            {
                string? nftVector = nftsWithVectors.FirstOrDefault(x => x.Address == post.NFTAddress)?.Vector;
                post.NFTVector = nftVector ?? string.Empty;
            }

            if (user != null)
            {
                // calculate similarity and order
                posts = CalculateCosineSimilarity(posts, user.UserVector)
                    .OrderByDescending(x => x.CosineSimilarity)
                    .ToList();
            }

            // fetch current owners
            IEnumerable<NFTOwner> nftOwners = await _nftService.GetNFTOwners(posts.Select(x => x.NFTAddress));

            // remove any posts that no longer belong to the owner we currently have in database
            foreach (var nftOwner in nftOwners)
            {
                PostRecommendationView post = posts.First(x => x.NFTAddress == nftOwner.NFTId);

                if (post.OwnerAddress != nftOwner.OwnerId)
                    posts.Remove(post);
            }

            // take user requested subset
            posts = posts.Take(request.PageSize).ToList();

            // fetch all the data needed
            IEnumerable<PostWithUserContext> fullPosts = await _tokengramDbContext.Posts
                .Include(x => x.PostUserSettings)
                .Where(x => posts.Select(x => x.NFTAddress).Contains(x.NFTAddress))
                .Select(
                    x =>
                        new PostWithUserContext
                        {
                            Post = x,
                            CommentCount = x.Comments.Count,
                            LikeCount = x.Likes.Count,
                            IsLiked = user == null ? false : x.Likes.Any(x => x.LikerAddress == user.Address),
                            OwnerAddress = x.PostUserSettings.UserAddress
                        }
                )
                .ToListAsync();

            fullPosts = await FillPostsWithNFTs(fullPosts);

            return fullPosts;
        }

        private async Task<IEnumerable<PostRecommendationView>> GetRandomPostsSubset()
        {
            return await _tokengramDbContext.Posts
                .Include(x => x.PostUserSettings)
                .Where(x => x.PostUserSettings.IsVisible)
                .OrderBy(x => Guid.NewGuid())
                .Take(RANDOM_POSTS_SUBSET_COUNT)
                .Select(
                    x =>
                        new PostRecommendationView
                        {
                            NFTAddress = x.NFTAddress,
                            OwnerAddress = x.PostUserSettings.UserAddress
                        }
                )
                .ToListAsync();
        }

        private async Task<IEnumerable<PostRecommendationView>> GetNewestPostsSubset()
        {
            return await _tokengramDbContext.Posts
                .Include(x => x.PostUserSettings)
                .Where(x => x.PostUserSettings.IsVisible)
                .OrderByDescending(x => x.PostUserSettings.CreatedAt)
                .Take(RANDOM_POSTS_SUBSET_COUNT)
                .Select(
                    x =>
                        new PostRecommendationView
                        {
                            NFTAddress = x.NFTAddress,
                            OwnerAddress = x.PostUserSettings.UserAddress
                        }
                )
                .ToListAsync();
        }

        private static IEnumerable<PostRecommendationView> CalculateCosineSimilarity(
            IEnumerable<PostRecommendationView> postRecommendationViews,
            string userVector
        )
        {
            foreach (var post in postRecommendationViews)
            {
                var fixedNFTVector = CosineSimilarityUtil.ParseVectorString(post.NFTVector);
                var fixedUserVector = CosineSimilarityUtil.ParseVectorString(userVector);
                var similarity = CosineSimilarityUtil.GetCosineSimilarity(fixedUserVector, fixedNFTVector);

                post.CosineSimilarity = similarity;
            }

            return postRecommendationViews;
        }
    }
}
