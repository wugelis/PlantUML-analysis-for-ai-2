using Microsoft.AspNetCore.Mvc;
using RentalCarSystem.WebUI.Models.ViewModels;

namespace RentalCarSystem.WebUI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var model = new HomeViewModel
        {
            IsLoggedIn = !string.IsNullOrEmpty(HttpContext.Session.GetString("CustomerId")),
            CustomerName = HttpContext.Session.GetString("CustomerName")
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}