using Microsoft.AspNetCore.Mvc;

namespace TaskFlow.Controllers
{
    public class LandingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
