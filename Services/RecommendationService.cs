using Microsoft.EntityFrameworkCore;
using Tokengram.Database.Tokengram;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.CustomEntities;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Services.Interfaces;
using Tokengram.Database.Neo4j.Nodes;
using Neo4jClient;
using Tokengram.Constants;
using Tokengram.Models.QueryResults;
using Tokengram.Utils;

namespace Tokengram.Services
{
    public partial class RecommendationService : IRecommendationService
    {
        private readonly TokengramDbContext _tokengramDbContext;

        private readonly IGraphClient _graphClient;

        public RecommendationService(TokengramDbContext tokengramDbContext, IGraphClient graphClient)
        {
            _tokengramDbContext = tokengramDbContext;
            _graphClient = graphClient;
        }

        public async Task<IEnumerable<PostWithCosineSimilarity>> GetHotPosts(User user, PaginationRequestDTO request)
        {
            var newestPosts = await FetchNewestPosts(request);

            var nftQueryResults = await FetchNFTQueryResults(newestPosts);
            var userPreferences = user.UserVector;

            var postsWithSimilarity = CalculateCosineSimilarity(newestPosts, nftQueryResults, userPreferences);

            return postsWithSimilarity.OrderByDescending(x => x.CosineSimilarity);
        }

        private async Task<List<Post>> FetchNewestPosts(PaginationRequestDTO request)
        {
            var random = new Random();
            var skipAmount = (request.PageNumber - 1) * request.PageSize;

            return await _tokengramDbContext.Posts
                .OrderBy(x => random.Next())
                .Include(p => p.PostUserSettings)
                .Where(x => x.PostUserSettings.Any(pus => pus.IsVisible))
                .Skip(skipAmount)
                .Take(request.PageSize)
                .ToListAsync();
        }

        private async Task<List<NFTQueryResult>> FetchNFTQueryResults(List<Post> newestPosts)
        {
            var nftAddresses = newestPosts.Select(x => x.NFTAddress).ToList();
            return (List<NFTQueryResult>)
                await _graphClient.Cypher
                    .Match($"(NFT:{NodeNames.NFT})")
                    .Where<NFTNode>(nft => nftAddresses.Contains(nft.Address))
                    .OptionalMatch($"(NFT)-[rel:{RelationshipNames.TAGGED}]->(tag:{NodeNames.TAG})")
                    .Return(
                        (nft, rel, tag) =>
                            new NFTQueryResult { NFT = nft.As<NFTNode>(), Tags = tag.CollectAsDistinct<TagNode>() }
                    )
                    .ResultsAsync;
        }

        private List<PostWithCosineSimilarity> CalculateCosineSimilarity(
            List<Post> newestPosts,
            List<NFTQueryResult> nftQueryResults,
            string userPreferences
        )
        {
            var postsWithSimilarity = new List<PostWithCosineSimilarity>();

            foreach (var post in newestPosts)
            {
                var nftQueryResult = nftQueryResults.FirstOrDefault(x => x.NFT.Address == post.NFTAddress);
                if (nftQueryResult == null)
                {
                    continue;
                }

                var postVector = nftQueryResult.NFT.NFTVector;
                if (postVector != null)
                {
                    var fixedpostVector = CosineSimilarityUtil.ParseVectorString(postVector);
                    var fixeduserVector = CosineSimilarityUtil.ParseVectorString(userPreferences);
                    var similarity = CosineSimilarityUtil.GetCosineSimilarity(fixeduserVector, fixedpostVector);
                    postsWithSimilarity.Add(
                        new PostWithCosineSimilarity
                        {
                            NFTVector = fixedpostVector,
                            CosineSimilarity = similarity,
                            NFTAddress = post.NFTAddress,
                            CreatedAt = post.CreatedAt,
                            LikeCount = post.LikeCount,
                            PostUserSettings = post.PostUserSettings,
                            NFTQueryResult = nftQueryResult
                        }
                    );
                }
            }

            return postsWithSimilarity;
        }
    }
}
