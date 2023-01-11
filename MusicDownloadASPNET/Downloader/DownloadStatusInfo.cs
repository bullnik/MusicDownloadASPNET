using Newtonsoft.Json;

namespace MusicDownloadASPNET.Downloader
{
    public class DownloadStatusInfo
    {
        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("miniolink")]
        public string MinioLink { get; set; }

        [JsonProperty("downloaded")]
        public bool Downloaded { get; set; }

        [JsonProperty("error")]
        public bool Error { get; set; }

        public DownloadStatusInfo(string link, string minioLink = "", bool error = true)
        {
            Link = link;
            MinioLink = minioLink;
            Downloaded = MinioLink != "";
            Error = error;
            if (Downloaded)
            {
                Error = false;
            }
        }
    }
}
