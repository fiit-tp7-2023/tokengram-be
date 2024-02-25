using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EF = Microsoft.EntityFrameworkCore;

namespace Tokengram.Database.Postgres.Models
{
    [Table("refresh_tokens")]
    [EF.Index(nameof(Token), IsUnique = true)]
    public class RefreshToken
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("token")]
        public string Token { get; set; } = null!;

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Column("blacklisted_at")]
        public DateTime? BlackListedAt { get; set; }

        public User User { get; set; } = null!;
    }
}
