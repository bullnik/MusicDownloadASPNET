using System.Collections.ObjectModel;

namespace MusicDownloadASPNET.Rabbit
{
    public interface IRabbitMqService
    {
        void SendMusicDownloadRequest(string link);
        ReadOnlyCollection<string> GetCompletedDownloadsList();
    }
}
