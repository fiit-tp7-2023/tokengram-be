using System.ComponentModel.DataAnnotations.Schema;
using Tokengram.Models.QueryResults;

namespace Tokengram.Database.Tokengram.Entities
{
    public class Post : BaseEntity
    {
        public string NFTAddress { get; set; } = null!;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();

        public PostUserSettings PostUserSettings { get; set; } = null!;

        [NotMapped]
        public NFTQueryResult? NFTQueryResult { get; set; }
    }
}
