namespace Tokengram.DTOS.Requests
{
    public class LoginRequestDTO
    {
        public string Signature { get; set; } = null!;

        public string PublicAddress { get; set; } = null!;
    }
}
