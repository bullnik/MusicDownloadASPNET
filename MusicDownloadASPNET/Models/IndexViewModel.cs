using System.IO;

namespace MusicDownloadASPNET.Models
{
    public class IndexViewModel
    {
        public string RootFolder { get; private set; } = Environment.CurrentDirectory;

        public IndexViewModel() 
        {

        }
    }
}
