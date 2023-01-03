using Newtonsoft.Json;

namespace MusicDownloadASPNET.Downloader
{
    public class DownloadStatusInfo
    {
        [JsonProperty("downloaded")]
        public bool Downloaded { get; set; }
        [JsonProperty("progress")]
        public int Progress { get; set; }
        [JsonProperty("miniolink")]
        public string MinioLink { get; set; }
        [JsonProperty("error")]
        public bool Error { get; set; }

        public DownloadStatusInfo(bool downloaded, int progress, string minioLink, bool error)
        {
            Downloaded = downloaded;
            Progress = progress;
            MinioLink = minioLink;
            Error = error;
        }
    }
}
