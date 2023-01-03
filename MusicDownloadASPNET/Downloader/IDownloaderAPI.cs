namespace MusicDownloadASPNET.Downloader
{
    public interface IDownloaderAPI
    {
        DownloadStatusInfo Download(string url);
    }
}
