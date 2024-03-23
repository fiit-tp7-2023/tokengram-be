namespace Tokengram.Database.Indexer.Entities
{
    public class NFTOwner
    {
        public string Id { get; set; } = null!;

        public string? OwnerId { get; set; }

        public string? NFTId { get; set; }

        public long Amount { get; set; }
    }
}
