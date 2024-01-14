using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeWebApp.Entities;

namespace RecipeWebApp.Infrastructure
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>, IAppDbContext
    {
        public DbSet<Recipe> Recipes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
