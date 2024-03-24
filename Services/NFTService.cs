using Microsoft.EntityFrameworkCore;
using Neo4jClient;
using Tokengram.Database.Indexer;
using Tokengram.Database.Neo4j.Nodes;
using Tokengram.Services.Interfaces;
using Tokengram.Constants;
using Tokengram.Models.QueryResults;
using Tokengram.Database.Neo4j.Relationships;
using Neo4jClient.Extensions;
using Tokengram.Extensions;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Database.Indexer.Entities;

namespace Tokengram.Services
{
    public class NFTService : INFTService
    {
        private readonly IndexerDbContext _indexerDbContext;
        private readonly IGraphClient _graphClient;

        public NFTService(IndexerDbContext indexerDbContext, IGraphClient graphClient)
        {
            _indexerDbContext = indexerDbContext;
            _graphClient = graphClient;
        }

        public async Task<NFTOwner?> GetNFTOwner(string nftAddress)
        {
            return await _indexerDbContext.NFTOwners.FirstOrDefaultAsync(
                x => x.NFTId == nftAddress && x.OwnerId != null
            );
        }

        public async Task<bool> IsNFTOwner(string nftAddress, string userAddress)
        {
            return await _indexerDbContext.NFTOwners.AnyAsync(x => x.NFTId == nftAddress && x.OwnerId == userAddress);
        }

        public async Task<IEnumerable<string>> GetOwnedNFTs(PaginationRequestDTO request, string userAddress)
        {
            return await _indexerDbContext.NFTOwners
                .Where(x => x.OwnerId == userAddress && x.NFTId != null)
                .Select(x => x.NFTId!)
                .OrderBy(x => x)
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync();
        }

        public async Task<NFTQueryResult> GetNFT(string nftAddress)
        {
            NFTQueryResult queryResult = await _graphClient.Cypher
                .Match($"(nft:{NodeNames.NFT})")
                .Where<NFTNode>((nft) => nft.Address == nftAddress)
                .OptionalMatch($"(nft)-[rel:{RelationshipNames.TAGGED}]->(tag:{NodeNames.TAG})")
                .ReturnDistinct(
                    (nft, rel, tag) =>
                        new NFTQueryResult
                        {
                            NFT = nft.As<NFTNode>(),
                            Tags = tag.CollectAsDistinct<TagNode>(),
                            TagRelations = rel.CollectAsDistinct<TaggedRelationship>()
                        }
                )
                .FirstAsync();

            return queryResult;
        }

        public async Task<IEnumerable<NFTQueryResult>> GetNFTs(IEnumerable<string> nftAddresses)
        {
            IEnumerable<NFTQueryResult> queryResult = await _graphClient.Cypher
                .Match($"(nft:{NodeNames.NFT})")
                .Where<NFTNode>((nft) => nft.Address.In(nftAddresses))
                .OptionalMatch($"(nft)-[rel:{RelationshipNames.TAGGED}]->(tag:{NodeNames.TAG})")
                .ReturnDistinct(
                    (nft, rel, tag) =>
                        new NFTQueryResult
                        {
                            NFT = nft.As<NFTNode>(),
                            Tags = tag.CollectAsDistinct<TagNode>(),
                            TagRelations = rel.CollectAsDistinct<TaggedRelationship>()
                        }
                )
                .ResultsAsync;

            return queryResult;
        }
    }
}
