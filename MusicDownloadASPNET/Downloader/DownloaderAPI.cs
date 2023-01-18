using MusicDownloadASPNET.Rabbit;
using Newtonsoft.Json;
using System.Web;
using static System.Net.WebRequestMethods;

namespace MusicDownloadASPNET.Downloader
{
    public class DownloaderAPI : IDownloaderAPI
    {
        private readonly IRabbitMqService _rabbit;
        private readonly string _requestsQueueName;
        private readonly string _resultsQueueName;
        private readonly IList<string> _sendedRequests;
        private readonly IDictionary<string, DownloadStatusInfo> _results;

        public DownloaderAPI(IRabbitMqService rabbitMqService) 
        {
            _requestsQueueName = "DownloadRequests";
            _resultsQueueName = "DownloadResults";
            _rabbit = rabbitMqService;
            _sendedRequests = new List<string>();
            _results = new Dictionary<string, DownloadStatusInfo>();
        }

        public DownloadStatusInfo Download(string link)
        {
            DownloadStatusInfo errorInfo = new(link);
            if (!TryValidateLink(link, out string shortenedLink))
            {
                return errorInfo;
            }
            link = shortenedLink;

            if (!_sendedRequests.Contains(link))
            {
                _sendedRequests.Add(link);
                _rabbit.SendMessage(_requestsQueueName, link);
            }

            if (_rabbit.TryReceiveMessage(_resultsQueueName, out string resultText))
            {
                DownloadStatusInfo? result = TryDeserialize(resultText);
                if (result is not null)
                {
                    lock (_results)
                    {
                        _results.Add(result.Link, result);
                    }
                }
            }

            if (_sendedRequests.Contains(link) && _results.ContainsKey(link))
            {
                DownloadStatusInfo info = _results[link];
                lock(_sendedRequests)
                {
                    _sendedRequests.Remove(link);
                }
                lock(_results)
                {
                    _results.Remove(link);
                }
                return info;
            }

            DownloadStatusInfo defaultInfo = new(link, error: false);
            return defaultInfo;
        }

        private static DownloadStatusInfo? TryDeserialize(string text) 
        {
            try
            {
                return JsonConvert.DeserializeObject<DownloadStatusInfo>(text);
            }
            catch { }
            return null;
        }

        private static bool TryValidateLink(string rawLink, out string validatedLink)
        {
            validatedLink = "";
            if (rawLink is null || rawLink.Split("v=").Length < 2)
            {
                return false;
            }
            validatedLink = rawLink.Split("v=")[1].Split('&')[0];
            return true;
        }
    }
}
