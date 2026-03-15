using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBookShop.Data;
using OnlineBookShop.Models;

namespace OnlineBookShop.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        var items = await _context.CartItems
            .Include(c => c.Book)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        var total = items.Sum(i => i.Book!.Price * i.Quantity);
        ViewBag.Total = total;
        return View(items);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int bookId, int quantity = 1)
    {
        var userId = GetUserId();

        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

        if (item == null)
        {
            item = new CartItem
            {
                UserId = userId,
                BookId = bookId,
                Quantity = quantity
            };
            _context.CartItems.Add(item);
        }
        else
        {
            item.Quantity += quantity;
        }

        await _context.SaveChangesAsync();

        var count = await _context.CartItems
            .Where(c => c.UserId == userId)
            .SumAsync(c => c.Quantity);

        return Json(new { success = true, count });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int cartId, int quantity)
    {
        var userId = GetUserId();
        var item = await _context.CartItems
            .Include(c => c.Book)
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.UserId == userId);
        if (item == null) return NotFound();

        if (quantity <= 0)
        {
            _context.CartItems.Remove(item);
        }
        else
        {
            item.Quantity = quantity;
        }

        await _context.SaveChangesAsync();

        var items = await _context.CartItems
            .Include(c => c.Book)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        var total = items.Sum(i => i.Book!.Price * i.Quantity);
        var count = items.Sum(i => i.Quantity);
        return Json(new { success = true, total, count });
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int cartId)
    {
        var userId = GetUserId();
        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.UserId == userId);
        if (item == null) return NotFound();

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();

        var count = await _context.CartItems
            .Where(c => c.UserId == userId)
            .SumAsync(c => c.Quantity);

        return Json(new { success = true, count });
    }

    private int GetUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(idClaim!);
    }
}

