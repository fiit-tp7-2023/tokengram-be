namespace Tokengram.Models.DTOS.Shared.Responses
{
    public class UserChatProfileResponseDTO
    {
        public IEnumerable<ChatResponseDTO> Chats { get; set; } = new List<ChatResponseDTO>();

        public IEnumerable<ReceivedChatInvitationResponseDTO> ReceivedChatInvitations { get; set; } =
            new List<ReceivedChatInvitationResponseDTO>();
    }
}
