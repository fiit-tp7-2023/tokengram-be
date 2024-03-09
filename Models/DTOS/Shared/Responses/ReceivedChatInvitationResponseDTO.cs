namespace Tokengram.Models.DTOS.Shared.Responses
{
    public class ReceivedChatInvitationResponseDTO
    {
        public BasicChatResponseDTO Chat { get; set; } = null!;

        public UserResponseDTO Sender { get; set; } = null!;
    }
}
