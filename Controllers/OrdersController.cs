using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBookShop.Data;
using OnlineBookShop.Models;

namespace OnlineBookShop.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var userId = GetUserId();
        var items = await _context.CartItems
            .Include(c => c.Book)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!items.Any())
        {
            TempData["Toast"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        ViewBag.Total = items.Sum(i => i.Book!.Price * i.Quantity);
        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //public async Task<IActionResult> PlaceOrder(string cardHolder, string cardNumber, string expiry, string cvv)
    //{
    //    var userId = GetUserId();
    //    var items = await _context.CartItems
    //        .Include(c => c.Book)
    //        .Where(c => c.UserId == userId)
    //        .ToListAsync();

    //    if (!items.Any())
    //    {
    //        TempData["Toast"] = "Your cart is empty.";
    //        return RedirectToAction("Index", "Cart");
    //    }

    //    // Simulated payment: simple validation
    //    if (string.IsNullOrWhiteSpace(cardHolder) || string.IsNullOrWhiteSpace(cardNumber))
    //    {
    //        TempData["Toast"] = "Payment failed. Please fill in all required fields.";
    //        return RedirectToAction(nameof(Checkout));
    //    }

    //    var total = items.Sum(i => i.Book!.Price * i.Quantity);

    //    var order = new Order
    //    {
    //        UserId = userId,
    //        OrderDate = DateTime.UtcNow,
    //        TotalAmount = total,
    //        Status = "Paid",
    //        OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{userId}"
    //    };

    //    foreach (var item in items)
    //    {
    //        order.Items.Add(new OrderItem
    //        {
    //            BookId = item.BookId,
    //            Quantity = item.Quantity,
    //            UnitPrice = item.Book!.Price
    //        });

    //        if (item.Book!.Stock >= item.Quantity)
    //        {
    //            item.Book.Stock -= item.Quantity;
    //        }
    //    }

    //    _context.Orders.Add(order);
    //    _context.CartItems.RemoveRange(items);
    //    await _context.SaveChangesAsync();

    //    return RedirectToAction(nameof(OrderSuccess), new { id = order.OrderId });
    //}

   
    public async Task<IActionResult> PlaceOrder(string cardHolder, string cardNumber, string expiry, string cvv)
    {
        var userId = GetUserId();

        var items = await _context.CartItems
            .Include(c => c.Book)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!items.Any())
        {
            TempData["Toast"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        if (string.IsNullOrWhiteSpace(cardHolder) ||
            string.IsNullOrWhiteSpace(cardNumber) ||
            string.IsNullOrWhiteSpace(cvv))
        {
            TempData["Toast"] = "Invalid payment details.";
            return RedirectToAction(nameof(Checkout));
        }

        var total = items.Sum(i => i.Book!.Price * i.Quantity);

        using var transaction = await _context.Database.BeginTransactionAsync();

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            TotalAmount = total,
            Status = "Paid",
            OrderNumber = $"ORD-{Guid.NewGuid().ToString()[..8].ToUpper()}"
        };

        foreach (var item in items)
        {
            if (item.Book!.Stock < item.Quantity)
            {
                TempData["Toast"] = $"{item.Book.Title} is out of stock.";
                return RedirectToAction("Index", "Cart");
            }

            item.Book.Stock -= item.Quantity;

            order.Items.Add(new OrderItem
            {
                BookId = item.BookId,
                Quantity = item.Quantity,
                UnitPrice = item.Book.Price
            });
        }

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(items);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return RedirectToAction(nameof(OrderSuccess), new { id = order.OrderId });
    }

    public async Task<IActionResult> OrderSuccess(int id)
    {
        var userId = GetUserId();
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Book)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

        if (order == null) return NotFound();
        return View(order);
    }

    private int GetUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(idClaim!);
    }
}

