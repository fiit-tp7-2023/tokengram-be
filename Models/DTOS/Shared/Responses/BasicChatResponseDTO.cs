using Tokengram.Enums;

namespace Tokengram.Models.DTOS.Shared.Responses
{
    public class BasicChatResponseDTO
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public UserResponseDTO Admin { get; set; } = null!;

        public ChatTypeEnum Type { get; set; }

        public IEnumerable<UserResponseDTO> Users { get; set; } = new List<UserResponseDTO>();
    }
}
