using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookShop.Models;

public class Order
{
    [Key]
    public int OrderId { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Range(0, 1000000)]
    public decimal TotalAmount { get; set; }

    [Required, StringLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Paid, Shipped, Completed

    [Required, StringLength(100)]
    public string OrderNumber { get; set; } = string.Empty;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

