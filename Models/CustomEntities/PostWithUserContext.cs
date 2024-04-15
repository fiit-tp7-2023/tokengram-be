using Tokengram.Database.Tokengram.Entities;

namespace Tokengram.Models.CustomEntities
{
    public class PostWithUserContext
    {
        public string OwnerAddress { get; set; } = null!;

        public Post Post { get; set; } = null!;

        public long LikeCount { get; set; } = 0;

        public long CommentCount { get; set; } = 0;

        public bool IsLiked { get; set; } = false;
    }
}
