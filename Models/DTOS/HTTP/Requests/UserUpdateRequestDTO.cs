namespace Tokengram.Models.DTOS.HTTP.Requests
{
    public class UserUpdateRequest
    {
        public string? Username { get; set; }

        public IFormFile? ProfilePicture { get; set; }
    }
}
