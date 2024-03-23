using Tokengram.Database.Neo4j.Nodes;
using Tokengram.Database.Neo4j.Relationships;

namespace Tokengram.Models.QueryResults
{
    public class NFTQueryResult
    {
        public NFTNode NFT { get; set; } = null!;
        public IEnumerable<TagNode> Tags { get; set; } = null!;
        public IEnumerable<TaggedRelationship> TagRelations { get; set; } = null!;
    }
}
