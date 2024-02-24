using Newtonsoft.Json;

namespace Tokengram.Database.Neo4j.Nodes
{
    public class TagNode
    {
        [JsonProperty("type")]
        public string Type { get; set; } = null!;
    }
}
