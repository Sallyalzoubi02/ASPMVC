using Microsoft.AspNetCore.Mvc;

namespace Master.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Shop()
        {
            return View();
        }

        public IActionResult ProductDetails()
        {
            return View();
        }
    }
}
