using System.Diagnostics;
using ElamIgugu.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElamIgugu.Web.Controllers;

public class HomeController : Controller
{
    // Landing page: public, shows Sign in / Sign up
    public IActionResult Index() => View();

    // Sends a signed-in user to the right page for their role
    [Authorize]
    public IActionResult Dashboard() =>
        User.IsInRole(AppRoles.Admin)
            ? RedirectToAction("Index", "Admin")
            : RedirectToAction("Index", "Donor");

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorInfo { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
