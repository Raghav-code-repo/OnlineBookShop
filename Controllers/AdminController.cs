using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBookShop.Data;

namespace OnlineBookShop.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard()
    {
        ViewBag.TotalUsers = await _context.Users.CountAsync();
        ViewBag.TotalBooks = await _context.Books.CountAsync();
        ViewBag.TotalOrders = await _context.Orders.CountAsync();
        ViewBag.TotalRevenue = await _context.Orders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

        var latestOrders = await _context.Orders
            .OrderByDescending(o => o.OrderDate)
            .Take(5)
            .Include(o => o.User)
            .ToListAsync();

        return View(latestOrders);
    }

    public async Task<IActionResult> Users()
    {
        var users = await _context.Users
            .OrderBy(u => u.Name)
            .ToListAsync();
        return View(users);
    }

    public async Task<IActionResult> Orders()
    {
        var orders = await _context.Orders
            .Include(o => o.User)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        return View(orders);
    }
}

