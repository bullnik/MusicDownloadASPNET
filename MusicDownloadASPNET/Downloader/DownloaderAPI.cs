using Newtonsoft.Json;
using System.Web;
using static System.Net.WebRequestMethods;

namespace MusicDownloadASPNET.Downloader
{
    public class DownloaderAPI : IDownloaderAPI
    {
        private readonly HttpClient _http = new();
        private readonly string _apiEndpoint;

        public DownloaderAPI() 
        {
            string? apiEndpoint = Environment.GetEnvironmentVariable("API_ENDPOINT");
            if (apiEndpoint is null )
            {
                throw new Exception("Environment not specified");
            }
            _apiEndpoint = apiEndpoint;
        }

        public DownloadStatusInfo Download(string link)
        {
            var builder = new UriBuilder($"http://{_apiEndpoint}/Home/Download");
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["link"] = link;
            builder.Query = query.ToString();
            string httpRequestString = builder.ToString();
            string text = GetResponseTextWithHttp(httpRequestString, TimeSpan.FromSeconds(10));
            DownloadStatusInfo? result = TryDeserialize(text);
            return result is null ? new DownloadStatusInfo(false, 0, "", true) : result;
        }

        private DownloadStatusInfo? TryDeserialize(string text) 
        {
            try
            {
                return JsonConvert.DeserializeObject<DownloadStatusInfo>(text);
            }
            catch { }
            return null;
        }

        private string GetResponseTextWithHttp(string request, TimeSpan timeout)
        {
            try
            {
                Task<HttpResponseMessage> httpRequest;
                httpRequest = _http.GetAsync(request);
                if (!httpRequest.Wait(timeout)
                || httpRequest.Result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return "";
                }
                var httpResponse = httpRequest.Result;
                return httpResponse.Content.ReadAsStringAsync().Result;
            }
            catch
            {
                return "";
            }
        }
    }
}
