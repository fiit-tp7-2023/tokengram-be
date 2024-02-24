using Newtonsoft.Json;

namespace Tokengram.Database.Neo4j.Relationships
{
    public class TaggedRelationship
    {
        [JsonProperty("value")]
        public int Value { get; set; } = 1;
    }
}
