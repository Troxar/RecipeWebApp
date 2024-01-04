using Microsoft.EntityFrameworkCore;
using RecipeWebApp.Entities;

namespace RecipeWebApp.Infrastructure
{
    public interface IAppDbContext
    {
        DbSet<Recipe> Recipes { get; set; }
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
