using Tokengram.Database.Tokengram.Entities;

namespace Tokengram.Models.CustomEntities
{
    public class UserPost : Post
    {
        public bool IsLiked { get; set; } = false;

        public bool IsVisible { get; set; } = false;

        public string? Description { get; set; }
    }
}
