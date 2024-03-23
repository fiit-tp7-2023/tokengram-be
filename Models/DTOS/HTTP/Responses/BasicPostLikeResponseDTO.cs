namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class BasicPostLikeResponseDTO
    {
        public long Id { get; set; }

        public string LikerAddress { get; set; } = null!;

        public string PostNFTAddress { get; set; } = null!;
    }
}
