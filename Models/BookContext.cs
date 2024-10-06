using Microsoft.EntityFrameworkCore;

namespace project1.Models;

public class BookContext : DbContext
{
    public BookContext(DbContextOptions<BookContext> options) : base(options)
    {}

    public DbSet<Book> Books { get; set; } = null!;
}