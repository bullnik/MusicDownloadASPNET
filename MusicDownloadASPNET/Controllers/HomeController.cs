using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MusicDownloadASPNET.Downloader;
using MusicDownloadASPNET.Models;
using MusicDownloadASPNET.Rabbit;
using System.Diagnostics;
using System.Web;
using static System.Net.WebRequestMethods;

namespace MusicDownloadASPNET.Controllers
{
    public class HomeController : Controller
    {
        readonly IDownloaderAPI _downloaderAPI;

        public HomeController(IDownloaderAPI downloaderAPI)
        {
            _downloaderAPI = downloaderAPI;
        }

        [HttpGet]
        public IActionResult Download(string link)
        {
            DownloadStatusInfo downloadStatusInfo = _downloaderAPI.Download(link);
            return PartialView("ApiResult", downloadStatusInfo);
        }

        public IActionResult Index()
        {
            return View(new IndexViewModel());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}