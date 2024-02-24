using Newtonsoft.Json;

namespace Tokengram.Database.Neo4j.Nodes
{
    public class AddressNode
    {
        [JsonProperty("address")]
        public string Address { get; set; } = null!;

        [JsonProperty("createdAtBlock")]
        public ulong CreatedAtBlock { get; set; }
    }
}
