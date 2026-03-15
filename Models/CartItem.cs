using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookShop.Models;

public class CartItem
{
    [Key]
    public int CartId { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [Required]
    public int BookId { get; set; }

    [ForeignKey(nameof(BookId))]
    public Book? Book { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; }
}

