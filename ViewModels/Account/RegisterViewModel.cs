using System.ComponentModel.DataAnnotations;

namespace OnlineBookShop.ViewModels.Account;

public class RegisterViewModel
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Address { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }
}

