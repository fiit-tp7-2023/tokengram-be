namespace Tokengram.Models.DTOS.WS.Requests
{
    public class ChatRequestDTO
    {
        public List<string> UserAddresses { get; set; } = new List<string>();

        public string? Name { get; set; }
    }
}
