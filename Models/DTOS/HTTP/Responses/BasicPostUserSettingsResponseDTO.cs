namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class BasicPostUserSettingsResponseDTO
    {
        public long Id { get; set; }

        public string UserAddress { get; set; } = null!;

        public bool IsVisible { get; set; } = false;

        public string? Description { get; set; }
    }
}
