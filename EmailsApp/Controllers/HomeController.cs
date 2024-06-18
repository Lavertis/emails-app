using Microsoft.AspNetCore.Mvc;

namespace EmailsApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "Person");
}