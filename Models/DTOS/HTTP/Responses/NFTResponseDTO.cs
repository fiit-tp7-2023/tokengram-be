using Tokengram.Database.Neo4j.Nodes;

namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class NFTResponseDTO : NFTNode
    {
        public IEnumerable<TagRelationResponseDTO> Tags { get; set; } = null!;
    }
}
