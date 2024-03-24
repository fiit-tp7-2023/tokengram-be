namespace Tokengram.Models.DTOS.HTTP.Requests
{
    public class PostUserSettingsRequestDTO
    {
        public bool IsVisible { get; set; }

        public string? Description { get; set; }
    }
}
