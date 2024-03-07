namespace Tokengram.Models.DTOS.WS.Requests
{
    public class ChatPromoteToAdminRequestDTO
    {
        public long ChatId { get; set; }

        public string AdminAddress { get; set; } = null!;
    }
}
