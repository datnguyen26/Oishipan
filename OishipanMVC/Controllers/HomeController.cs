using Microsoft.AspNetCore.Mvc;

namespace OishipanMVC.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("trang-chu")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("chinh-sach")]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("tin-tuc")]
        public IActionResult News()
        {
            return View();
        }

        [HttpGet("ve-oishipan")]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet("lien-he")]
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("loi")] 
        public IActionResult Error()
        {
            return View();
        }
    }
}
