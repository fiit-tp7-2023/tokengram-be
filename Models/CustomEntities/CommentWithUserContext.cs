using Tokengram.Database.Tokengram.Entities;

namespace Tokengram.Models.CustomEntities
{
    public class CommentWithUserContext : Comment
    {
        public bool IsLiked { get; set; } = false;
    }
}
