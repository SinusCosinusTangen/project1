using Microsoft.EntityFrameworkCore;

namespace project1.Models
{
    public class UserContext(DbContextOptions<UserContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; } = null!;
    }
}