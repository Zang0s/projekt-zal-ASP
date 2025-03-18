using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.CustomIdentity;

public class AccountController : Controller
{
    private readonly SignInManager<CustomUser> _signInManager;
    private readonly UserManager<CustomUser> _userManager;

    public AccountController(SignInManager<CustomUser> signInManager, UserManager<CustomUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user != null && password == user.PasswordHash)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Invalid username or password.";
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}