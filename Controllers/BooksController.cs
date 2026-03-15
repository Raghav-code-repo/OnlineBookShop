using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBookShop.Data;
using OnlineBookShop.Models;

namespace OnlineBookShop.Controllers;

public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private const int PageSize = 9;

    public BooksController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index(string? search, string? category, int page = 1)
    {
        var query = _context.Books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(b => b.Category == category);
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

        var books = await query
            .OrderBy(b => b.Title)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.Category = category;
        ViewBag.Page = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.Categories = await _context.Books
            .Select(b => b.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        return View(books);
    }

    public async Task<IActionResult> Details(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();
        return View(book);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminList()
    {
        var books = await _context.Books
            .OrderBy(b => b.Title)
            .ToListAsync();
        return View(books);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book model, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (imageFile != null && imageFile.Length > 0)
        {
            model.ImageUrl = await SaveBookImageAsync(imageFile);
        }

        _context.Books.Add(model);
        await _context.SaveChangesAsync();
        TempData["Toast"] = "Book added successfully.";
        return RedirectToAction(nameof(AdminList));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();
        return View(book);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Book model, IFormFile? imageFile)
    {
        if (id != model.BookId) return BadRequest();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();

        book.Title = model.Title;
        book.Author = model.Author;
        book.Category = model.Category;
        book.Price = model.Price;
        book.Stock = model.Stock;

        if (imageFile != null && imageFile.Length > 0)
        {
            book.ImageUrl = await SaveBookImageAsync(imageFile);
        }

        await _context.SaveChangesAsync();
        TempData["Toast"] = "Book updated successfully.";
        return RedirectToAction(nameof(AdminList));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        TempData["Toast"] = "Book deleted.";
        return RedirectToAction(nameof(AdminList));
    }

    private async Task<string> SaveBookImageAsync(IFormFile imageFile)
    {
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "books");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);
        await using var stream = System.IO.File.Create(filePath);
        await imageFile.CopyToAsync(stream);

        return $"/images/books/{fileName}";
    }
}

