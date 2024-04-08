using Tokengram.Database.Tokengram.Entities;

namespace Tokengram.Models.CustomEntities
{
    public class PostWithCosineSimilarity : Post
    {
        public Dictionary<string, int> NFTVector { get; set; } = new Dictionary<string, int>();
        public double CosineSimilarity { get; set; }
    }
}
