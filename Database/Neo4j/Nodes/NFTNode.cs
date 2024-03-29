using SYSTEM_JSON = System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Tokengram.Database.Neo4j.Nodes
{
    public class NFTNode
    {
        [JsonProperty("address")]
        public string Address { get; set; } = null!;

        [JsonProperty("createdAtBlock")]
        public ulong CreatedAtBlock { get; set; }

        [JsonProperty("tokenId")]
        public ulong TokenId { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("uri")]
        public string? Uri { get; set; }

        [JsonProperty("raw")]
        public string? Raw { get; set; }

        [JsonProperty("image")]
        public string? Image { get; set; }

        [JsonProperty("externalUrl")]
        public string? ExternalUrl { get; set; }

        [JsonProperty("animationUrl")]
        public string? AnimationUrl { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [SYSTEM_JSON.JsonIgnore]
        [JsonProperty("attributes")]
        public string? AttributesString { get; set; }

        public IEnumerable<Attribute> Attributes
        {
            get
            {
                if (AttributesString == null)
                {
                    return new List<Attribute>();
                }

                return JsonConvert.DeserializeObject<List<Attribute>>(AttributesString) ?? new List<Attribute>();
            }
        }

        public class Attribute
        {
            [JsonProperty("trait_type")]
            public string TraitType { get; set; } = null!;

            [JsonProperty("value")]
            public string Value { get; set; } = null!;
        }
    }
}
