using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class CommentLikeResponseDTO
    {
        public BasicUserResponseDTO Liker { get; set; } = null!;

        public long CommentId { get; set; }
    }
}
