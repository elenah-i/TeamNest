using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TeamNest.Models;

namespace TeamNest.Controllers;

public class HomeController : Controller
{
    // GET: /  (public landing page)
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // Shown for 404, 403 and other status codes (see UseStatusCodePagesWithReExecute)
    [Route("Home/Status/{code:int}")]
    public IActionResult Status(int code)
    {
        return View("StatusCode", code);
    }
}
