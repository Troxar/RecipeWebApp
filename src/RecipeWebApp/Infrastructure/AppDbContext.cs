using Microsoft.EntityFrameworkCore;
using RecipeWebApp.Entities;

namespace RecipeWebApp.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
