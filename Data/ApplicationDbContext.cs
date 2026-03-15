using Microsoft.EntityFrameworkCore;
using OnlineBookShop.Models;

namespace OnlineBookShop.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<CartItem>()
            .HasIndex(c => new { c.UserId, c.BookId })
            .IsUnique();

        // Seed admin user (password: Admin@123)
        modelBuilder.Entity<User>().HasData(new User
        {
            UserId = 1,
            Name = "Admin",
            Email = "admin@bookshop.local",
            PasswordHash = "E86F78A8A3CAF0B60D8E74E5942AA6D86DC150CD3C03338AEF25B7D2D7E3ACC7",
            Role = "Admin",
            Address = "Head Office",
            Phone = "000-000-0000"
        });

        modelBuilder.Entity<Book>().HasData(
            new Book
            {
                BookId = 1,
                Title = "Clean Code",
                Author = "Robert C. Martin",
                Category = "Programming",
                Price = 350.00m,
                Stock = 50,
                ImageUrl = "/images/books/clean-code.jpg",
                AverageRating = 4.8
            },
            new Book
            {
                BookId = 2,
                Title = "The Pragmatic Programmer",
                Author = "Andrew Hunt, David Thomas",
                Category = "Programming",
                Price = 450.00m,
                Stock = 40,
                ImageUrl = "/images/books/pp.jpg",
                AverageRating = 4.7
            },
            new Book
            {
                BookId = 3,
                Title = "Design Patterns",
                Author = "Erich Gamma et al.",
                Category = "Architecture",
                Price = 700.00m,
                Stock = 30,
                ImageUrl = "/images/books/design-patterns.jpg",
                AverageRating = 4.6
            },
            new Book
            {
                BookId = 4,
                Title = "Domain-Driven Design",
                Author = "Eric Evans",
                Category = "Architecture",
                Price = 2000.00m,
                Stock = 20,
                ImageUrl = "/images/books/ddd.jpg",
                AverageRating = 4.5
            },
            new Book
            {
                BookId = 5,
                Title = "Refactoring",
                Author = "Martin Fowler",
                Category = "Programming",
                Price = 3299.00m,
                Stock = 35,
                ImageUrl = "/images/books/rr.jpeg",
                AverageRating = 4.6
            }
        );
    }
}

