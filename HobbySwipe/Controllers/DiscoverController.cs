using Microsoft.AspNetCore.Mvc;

namespace HobbySwipe.Controllers
{
    [Route("[controller]")]
    public class DiscoverController : Controller
    {
        private readonly ILogger<DiscoverController> _logger;

        public DiscoverController(ILogger<DiscoverController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Discover()
        {
            return View();
        }
    }
}
