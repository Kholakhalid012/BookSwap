using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Register(string username, string password, string confirmPassword)
    {
        return RedirectToAction("Index", "Home");
    }
    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        return RedirectToAction("Index", "Home");
    }

}
