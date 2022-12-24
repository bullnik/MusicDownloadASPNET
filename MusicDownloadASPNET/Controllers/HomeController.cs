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

            _rabbit.SendMusicDownloadRequest(link);

            link = link.Replace("/", "a");

            string path = "/app/music/" + link + ".mp3";
            Stopwatch timer = new Stopwatch();
            timer.Start();
            while (timer.ElapsedMilliseconds < 30000 
                && !System.IO.File.Exists(path))
            {
                Console.WriteLine("WAIT");
                Thread.Sleep(1000);
            }

            if (System.IO.File.Exists(path))
            {
                Console.WriteLine("FILE EXISTS!!");
                Thread.Sleep(1000);
                return File(System.IO.File.OpenRead(path), "application/octet-stream", "video.mp3");
            }

            Console.WriteLine("FILE NOT EXISTS");
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