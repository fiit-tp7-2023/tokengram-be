namespace Tokengram.Models.DTOS.HTTP.Requests
{
    public class ChangeProfileInfoRequestDTO
    {
        public string? Username { get; set; }
        public IFormFile? ProfilePictureFile { get; set; }
    }
}
