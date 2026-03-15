using System.ComponentModel.DataAnnotations;

namespace OnlineBookShop.Models;

public class Book
{
    [Key]
    public int BookId { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string Author { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [Range(0, 100000)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public double AverageRating { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}

