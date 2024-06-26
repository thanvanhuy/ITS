using Microsoft.AspNetCore.Mvc;

namespace VVA.ITS.WebApp.Controllers
{
    public class ViewDetailController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
