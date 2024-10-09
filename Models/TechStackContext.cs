using Microsoft.EntityFrameworkCore;

namespace project1.Models
{
    public class TechStackContext : DbContext
    {
        public TechStackContext(DbContextOptions<TechStackContext> options) : base(options)
        {}

        public DbSet<TechStack> techStacks { get; set; } = null!;
    }
}