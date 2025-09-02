using Microsoft.AspNetCore.Mvc;

namespace ShopTARge24.Controllers
{
    public class SpaceshipsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
