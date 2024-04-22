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
            return await _indexerDbContext.NFTOwners.FirstOrDefaultAsync(x => x.NFTId == nftAddress);
        }

        public async Task<IEnumerable<NFTOwner>> GetNFTOwners(IEnumerable<string> nftAddresses)
        {
            return await _indexerDbContext.NFTOwners
                .Where(x => x.NFTId.In(nftAddresses) && x.OwnerId != null)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> FilterOwnedNFTs(IEnumerable<string> nftAddresses, string userAddress)
        {
            return await _indexerDbContext.NFTOwners
                .Where(x => x.NFTId.In(nftAddresses) && x.OwnerId == userAddress)
                .Select(x => x.NFTId)
                .ToListAsync();
        }

        public async Task<bool> IsNFTOwner(string nftAddress, string userAddress)
        {
            return await _indexerDbContext.NFTOwners.AnyAsync(x => x.NFTId == nftAddress && x.OwnerId == userAddress);
        }

        public async Task<IEnumerable<string>> GetOwnedNFTs(PaginationRequestDTO request, string userAddress)
        {
            return await _indexerDbContext.NFTOwners
                .Where(x => x.OwnerId == userAddress)
                .OrderByDescending(x => x.AcquiredAt)
                .Select(x => x.NFTId)
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

        public async Task<IEnumerable<NFTWithVectorQueryResult>> FillNFTsWithVector(IEnumerable<string> nftAddresses)
        {
            return await _graphClient.Cypher
                .Match($"(nft:{NodeNames.NFT})")
                .Where<NFTNode>((nft) => nft.Address.In(nftAddresses))
                .Return(
                    (nft) =>
                        new NFTWithVectorQueryResult
                        {
                            Address = nft.As<NFTNode>().Address,
                            Vector = nft.As<NFTNode>().NFTVector
                        }
                )
                .ResultsAsync;
        }
    }
}
