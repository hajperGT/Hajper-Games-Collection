using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace HGC.Services
{
    public class RawgGameResult
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("released")]
        public string Released { get; set; } = "";

        [JsonPropertyName("background_image")]
        public string BackgroundImage { get; set; } = "";

        [JsonPropertyName("platforms")]
        public List<RawgPlatformWrapper> Platforms { get; set; } = new();

        public string PlatformsText =>
            Platforms == null || Platforms.Count == 0
                ? ""
                : string.Join(", ", Platforms
                    .Where(x => x.Platform != null)
                    .Select(x => x.Platform.Name));

        public override string ToString()
        {
            return Name;
        }
    }

    public class RawgPlatformWrapper
    {
        [JsonPropertyName("platform")]
        public RawgPlatform Platform { get; set; }
    }

    public class RawgPlatform
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }
}