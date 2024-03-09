namespace Tokengram.Models.DTOS.HTTP.Requests
{
    public class LoginRequestDTO
    {
        public string Signature { get; set; } = null!;

        public string Address { get; set; } = null!;
    }
}
