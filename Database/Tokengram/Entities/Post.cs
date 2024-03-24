using System.ComponentModel.DataAnnotations.Schema;
using Tokengram.Models.QueryResults;

namespace Tokengram.Database.Tokengram.Entities
{
    public class Post : BaseEntity
    {
        public string NFTAddress { get; set; } = null!;

        public int CommentCount { get; set; } = 0;

        public int LikeCount { get; set; } = 0;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();

        public ICollection<PostUserSettings> PostUserSettings = new List<PostUserSettings>();

        [NotMapped]
        public NFTQueryResult? NFTQueryResult { get; set; }
    }
}
