using Microsoft.EntityFrameworkCore;
using Tokengram.Database.Postgres.Models;

namespace Tokengram.Database.Postgres
{
    public class TokengramDbContext : DbContext
    {
        public TokengramDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
