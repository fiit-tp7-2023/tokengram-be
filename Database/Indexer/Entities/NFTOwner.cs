namespace Tokengram.Database.Indexer.Entities
{
    public class NFTOwner
    {
        public string Id { get; set; } = null!;

        public string OwnerId { get; set; } = null!;

        public string NFTId { get; set; } = null!;

        public long Amount { get; set; }

        public int AcquiredAt { get; set; }

        public int AcquiredAtBlock { get; set; }
    }
}
