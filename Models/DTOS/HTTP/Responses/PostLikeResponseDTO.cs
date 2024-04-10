using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class PostLikeResponseDTO
    {
        public BasicUserResponseDTO Liker { get; set; } = null!;

        public string PostNFTAddress { get; set; } = null!;
    }
}
