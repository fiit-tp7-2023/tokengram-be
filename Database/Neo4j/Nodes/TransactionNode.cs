using Newtonsoft.Json;

namespace Tokengram.Database.Neo4j.Nodes
{
    public class TransactionNode
    {
        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        [JsonProperty("amount")]
        public int Amount { get; set; }
    }
}
