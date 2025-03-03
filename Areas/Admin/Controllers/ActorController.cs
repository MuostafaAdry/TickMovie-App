using Microsoft.AspNetCore.Mvc;

namespace MoviePoint.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
