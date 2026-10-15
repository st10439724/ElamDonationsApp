using ElamIgugu.Web.Models;
using ElamIgugu.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ElamIgugu.Web.Controllers;

/// <summary>Sign up, sign in, sign out. Public pages, so no [Authorize] on the class.</summary>
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    // ---------- Register ----------

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Dashboard", "Home");
        return View(new RegisterViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,          // the email is the sign-in name
            Email = model.Email,
            PhoneNumber = model.ContactNumber.Trim(),
            FullName = model.FullName.Trim(),
            CreatedDate = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        // Self-registration ALWAYS creates a Donor. Admins are created by the team (see DbSeeder).
        await _userManager.AddToRoleAsync(user, AppRoles.Donor);
        _logger.LogInformation("New donor registered: {Email}", model.Email);

        TempData["Success"] = "Your account has been created. Please sign in.";
        return RedirectToAction(nameof(Login));
    }

    // ---------- Login ----------

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Dashboard", "Home");

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        // lockoutOnFailure: true counts wrong passwords and locks the account temporarily
        var result = await _signInManager.PasswordSignInAsync(
            model.Email, model.Password, isPersistent: false, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            // Only follow a return address that belongs to this site (stops open-redirect attacks)
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Dashboard", "Home");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Too many failed attempts. Please try again in a few minutes.");
        }
        else if (result.RequiresTwoFactor)
        {
            // Not reachable yet: nobody has two-factor switched on. The authenticator-app
            // step will be added here later (see README, "Activating the authenticator app").
            ModelState.AddModelError(string.Empty, "Two-factor sign-in is not available yet.");
        }
        else
        {
            // Same message for "no such email" and "wrong password" so attackers learn nothing
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
        }

        return View(model);
    }

    // ---------- Logout ----------

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // ---------- Shown when a donor tries to open an admin page ----------

    [HttpGet]
    public IActionResult AccessDenied() => View();
}
