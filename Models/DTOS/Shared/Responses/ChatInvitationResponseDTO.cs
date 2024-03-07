namespace Tokengram.Models.DTOS.Shared.Responses
{
    public class ChatInvitationResponseDTO
    {
        public UserResponseDTO User { get; set; } = null!;

        public UserResponseDTO Sender { get; set; } = null!;
    }
}
