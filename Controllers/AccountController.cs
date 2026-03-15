using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBookShop.Data;
using OnlineBookShop.Models;
using OnlineBookShop.ViewModels.Account;

namespace OnlineBookShop.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existing = await _context.Users.AnyAsync(u => u.Email == model.Email);
        if (existing)
        {
            ModelState.AddModelError(nameof(model.Email), "Email is already registered.");
            return View(model);
        }

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            PasswordHash = HashPassword(model.Password),
            Address = model.Address,
            Phone = model.Phone,
            Role = "Customer"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await SignInUser(user);
        return RedirectToAction("Index", "Books");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null || user.PasswordHash != HashPassword(model.Password))
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        await SignInUser(user);

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Books");
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Books");
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return RedirectToAction("Login");
        return View(user);
    }

    [Authorize]
    public async Task<IActionResult> MyOrders()
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return RedirectToAction("Login");

        var orders = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Book)
            .Where(o => o.UserId == user.UserId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    private async Task SignInUser(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    private async Task<User?> GetCurrentUserAsync()
    {
        if (!User.Identity?.IsAuthenticated ?? true) return null;
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idClaim, out var id)) return null;
        return await _context.Users.FindAsync(id);
    }
}

