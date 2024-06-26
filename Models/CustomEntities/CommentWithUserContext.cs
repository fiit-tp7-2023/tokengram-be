using Tokengram.Database.Tokengram.Entities;

namespace Tokengram.Models.CustomEntities
{
    public class CommentWithUserContext
    {
        public Comment Comment { get; set; } = null!;
        public long LikeCount { get; set; } = 0;
        public long CommentReplyCount { get; set; } = 0;
        public bool IsLiked { get; set; } = false;
    }
}
