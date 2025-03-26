using Microsoft.AspNetCore.Mvc;

namespace SnipEx.Web.Areas.User.Controllers
{
    [Area("User")]
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
