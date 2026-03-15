using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookShop.Models;

public class OrderItem
{
    [Key]
    public int OrderItemId { get; set; }

    [Required]
    public int OrderId { get; set; }

    [ForeignKey(nameof(OrderId))]
    public Order? Order { get; set; }

    [Required]
    public int BookId { get; set; }

    [ForeignKey(nameof(BookId))]
    public Book? Book { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; }

    [Range(0, 100000)]
    public decimal UnitPrice { get; set; }

    [NotMapped]
    public decimal LineTotal => UnitPrice * Quantity;
}

