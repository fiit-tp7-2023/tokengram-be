using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EF = Microsoft.EntityFrameworkCore;

namespace Tokengram.Database.Postgres.Models
{
    [Table("users")]
    [EF.Index(nameof(PublicAddress), IsUnique = true)]
    [EF.Index(nameof(Nonce), IsUnique = true)]
    public class User
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("public_address")]
        public string PublicAddress { get; set; } = null!;

        [Column("nonce")]
        public Guid Nonce { get; set; }

        [Column("refresh_token")]
        public string? RefreshToken { get; set; }

        [Column("refresh_token_expiry_time")]
        public DateTime RefreshTokenExpiryTime { get; set; }

        public string GetNonceMessage()
        {
            return $"Tokengram authentication\n\nNonce - {Nonce}\n\nSign this message to authenticate your account. This nonce is unique to this authentication request and helps ensure the security of your account.";
        }
    }
}
