using Microsoft.AspNetCore.Mvc;

namespace FarmUp.Controllers
{
    public class BlankController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
