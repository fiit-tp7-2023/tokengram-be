namespace Tokengram.Models.CustomEntities
{
    public class PostRecommendationView
    {
        public string NFTVector { get; set; } = string.Empty;
        public string NFTAddress { get; set; } = null!;
        public string OwnerAddress { get; set; } = null!;
        public double CosineSimilarity { get; set; } = 0;
    }
}
