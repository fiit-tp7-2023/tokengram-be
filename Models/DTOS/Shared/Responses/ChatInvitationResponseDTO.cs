namespace Tokengram.Models.DTOS.Shared.Responses
{
    public class ChatInvitationResponseDTO
    {
        public BasicUserResponseDTO User { get; set; } = null!;

        public BasicUserResponseDTO Sender { get; set; } = null!;
    }
}
