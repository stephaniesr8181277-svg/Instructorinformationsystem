using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstructorInformationSystem.Models;

namespace InstructorInformationSystem.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<UserAccount> _passwordHasher;

    public AccountController(
        ApplicationDbContext context,
        IPasswordHasher<UserAccount> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Dashboard));
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedEmail = model.Email.Trim().ToLowerInvariant();
        var emailAlreadyExists = await _context.UserAccounts
            .AnyAsync(account => account.Email.ToLower() == normalizedEmail);

        if (emailAlreadyExists)
        {
            ModelState.AddModelError(nameof(model.Email), "An account with this e-mail address already exists.");
            return View(model);
        }

        var account = new UserAccount
        {
            FullName = model.FullName.Trim(),
            Email = normalizedEmail
        };
        account.PasswordHash = _passwordHasher.HashPassword(account, model.Password);

        _context.UserAccounts.Add(account);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Registration successful. You can now log in.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Dashboard));
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedEmail = model.Email.Trim().ToLowerInvariant();
        var account = await _context.UserAccounts
            .FirstOrDefaultAsync(item => item.Email.ToLower() == normalizedEmail);

        if (account is null ||
            _passwordHasher.VerifyHashedPassword(account, account.PasswordHash, model.Password)
                == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Invalid e-mail address or password.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.UserAccountId.ToString()),
            new(ClaimTypes.Name, account.FullName),
            new(ClaimTypes.Email, account.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var properties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            AllowRefresh = true
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        ViewBag.InstructorCount = await _context.Instructors.CountAsync();
        ViewBag.AccountEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        return View();
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
