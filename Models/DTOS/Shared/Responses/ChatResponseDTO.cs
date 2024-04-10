using Tokengram.Enums;

namespace Tokengram.Models.DTOS.Shared.Responses
{
    public class ChatResponseDTO
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public ChatMessageResponseDTO? LastChatMessage { get; set; }

        public BasicUserResponseDTO Admin { get; set; } = null!;

        public ChatTypeEnum Type { get; set; }

        public IEnumerable<BasicUserResponseDTO> Users { get; set; } = new List<BasicUserResponseDTO>();

        public IEnumerable<ChatInvitationResponseDTO> ChatInvitations { get; set; } =
            new List<ChatInvitationResponseDTO>();
    }
}
