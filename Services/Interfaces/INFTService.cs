using Tokengram.Database.Indexer.Entities;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.QueryResults;

namespace Tokengram.Services.Interfaces
{
    public interface INFTService
    {
        Task<NFTOwner?> GetNFTOwner(string nftAddress);

        Task<IEnumerable<NFTOwner>> GetNFTOwners(IEnumerable<string> nftAddresses);

        Task<bool> IsNFTOwner(string nftAddress, string userAddress);

        Task<IEnumerable<string>> GetOwnedNFTs(PaginationRequestDTO request, string userAddress);

        Task<NFTQueryResult> GetNFT(string nftAddress);

        Task<IEnumerable<NFTQueryResult>> GetNFTs(IEnumerable<string> nftAddresses);

        Task<IEnumerable<string>> FilterOwnedNFTs(IEnumerable<string> nftAddresses, string userAddress);

        Task<IEnumerable<NFTWithVectorQueryResult>> FillNFTsWithVector(IEnumerable<string> nftAddresses);
    }
}
