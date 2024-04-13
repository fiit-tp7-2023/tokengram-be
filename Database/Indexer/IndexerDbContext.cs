using Microsoft.EntityFrameworkCore;
using Tokengram.Database.Indexer.Entities;

namespace Tokengram.Database.Indexer
{
    public class IndexerDbContext : DbContext
    {
        public IndexerDbContext(DbContextOptions<IndexerDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NFTOwner>(e =>
            {
                e.ToTable("nft_owner_entity");

                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.Amount).HasColumnName("amount");
                e.Property(x => x.NFTId).HasColumnName("nft_id");
                e.Property(x => x.OwnerId).HasColumnName("owner_id");
                e.Property(x => x.AcquiredAt).HasColumnName("acquired_at");

                e.HasKey(x => x.Id);
            });
        }

        public DbSet<NFTOwner> NFTOwners { get; set; }
    }
}
