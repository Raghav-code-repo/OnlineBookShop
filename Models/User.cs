using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookShop.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Address { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [Required, StringLength(20)]
    public string Role { get; set; } = "Customer"; // Customer or Admin

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}

