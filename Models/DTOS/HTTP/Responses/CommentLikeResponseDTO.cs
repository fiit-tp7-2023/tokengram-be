using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class CommentLikeResponseDTO
    {
        public long Id { get; set; }

        public UserResponseDTO Liker { get; set; } = null!;

        public long CommentId { get; set; }
    }
}
