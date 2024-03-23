namespace Tokengram.Database.Tokengram.Entities
{
    public class Comment : BaseEntity
    {
        public long Id { get; set; }

        public string Content { get; set; } = null!;

        public string CommenterAddress { get; set; } = null!;

        public string PostNFTAddress { get; set; } = null!;

        public long? ParentCommentId { get; set; }

        public int CommentReplyCount { get; set; } = 0;

        public int LikeCount { get; set; } = 0;

        public User Commenter { get; set; } = null!;

        public Post Post { get; set; } = null!;

        public Comment? ParentComment { get; set; }

        public ICollection<CommentLike> Likes { get; set; } = new List<CommentLike>();

        public ICollection<Comment> CommentReplies { get; set; } = new List<Comment>();
    }
}
