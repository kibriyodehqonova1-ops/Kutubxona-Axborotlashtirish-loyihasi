using Microsoft.AspNetCore.Mvc;

namespace KutubxonaAt.Areas.User.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
