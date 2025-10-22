using System.Text.Json.Serialization;

namespace Bloxstrap.Models.APIs.GitHub
{
    public class GithubReleaseAsset
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("browser_download_url")]
        public string BrowserDownloadUrl { get; set; } = null!;

        [JsonPropertyName("size")]
        public long Size { get; set; }
    }
}