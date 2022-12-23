using Microsoft.AspNetCore.Mvc;
using MusicDownloadASPNET.Models;
using MusicDownloadASPNET.Rabbit;
using System.Diagnostics;

namespace MusicDownloadASPNET.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRabbitMqService _rabbit;

        public HomeController(ILogger<HomeController> logger, IRabbitMqService mqService)
        {
            _logger = logger;
            _rabbit = mqService;
        }

        [HttpGet]
        public async Task<IActionResult> Download(string link)
        {
            if (link is null)
            {
                return NotFound();
            }

            //string path = Environment.CurrentDirectory + "\\" + link;
            //if (System.IO.File.Exists(path))
            //{
            //    return File(System.IO.File.OpenRead(path), "application/octet-stream", Path.GetFileName(path));
            //}

            _rabbit.SendMusicDownloadRequest(link);
            return NotFound();
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