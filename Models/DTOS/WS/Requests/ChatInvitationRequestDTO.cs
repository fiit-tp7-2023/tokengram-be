namespace Tokengram.Models.DTOS.WS.Requests
{
    public class ChatInvitationRequestDTO
    {
        public long ChatId { get; set; }

        public string UserAddress { get; set; } = null!;
    }
}
