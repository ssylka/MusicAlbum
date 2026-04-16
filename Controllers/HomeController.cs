using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicAlbums.Models;
using MusicAlbums.Services;
using System.Diagnostics;

namespace MusicAlbums.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SongGenerator _songGenerator = new SongGenerator();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpGet("cover")]
        public IActionResult GetCover(string title, string artist, int seed)
        {
            var generator = new CoverGenerator();

            var imageBytes = generator.Generate(title, artist, seed);

            return File(imageBytes, "image/png");
        }
        [HttpGet("")]
        public IActionResult Index(int page = 1, int seed = 1, string mode = "table", string location = "en_US", double avgLikes = 5.0)
        {
            var generator = new SongGenerator();

            var songs = generator.Generate(page, seed, avgLikes, location);

            ViewBag.Page = page;
            ViewBag.Seed = seed;
            ViewBag.Mode = mode;
            ViewBag.Location = location;
            ViewBag.AvgLikes = avgLikes;

            return View(songs);
        }
        [HttpGet("/load-more")]
        public IActionResult LoadMore(int page, int seed, string mode, string location, double avgLikes)
        {
            var generator = new SongGenerator();
            var songs = generator.Generate(page, seed, avgLikes, location);

            return PartialView("_SongsPartial", songs);
        }
        [HttpGet("/reload")]
        public IActionResult Reload(int page, int seed, string mode, string location, double avgLikes)
        {
            var songs = _songGenerator.Generate(page, seed, avgLikes, location);

            ViewBag.Page = page;
            ViewBag.Seed = seed;
            ViewBag.Mode = mode;
            ViewBag.Location = location;
            ViewBag.AvgLikes = avgLikes;
            return View("index", songs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
