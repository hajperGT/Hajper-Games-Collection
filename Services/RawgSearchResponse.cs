using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HGC.Services
{
    internal class RawgSearchResponse
    {
        [JsonPropertyName("results")]
        public List<RawgGameResult> Results { get; set; } = new();
    }
}