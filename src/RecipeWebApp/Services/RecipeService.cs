using Microsoft.EntityFrameworkCore;
using RecipeWebApp.Infrastructure;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Services
{
    public class RecipeService : IRecipeService
    {
        readonly IAppDbContext _context;

        public RecipeService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateRecipe(CreateRecipeCommand cmd)
        {
            var recipe = cmd.ToRecipe();
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return recipe.RecipeId;
        }

        public async Task<RecipeDetailViewModel?> GetRecipe(int id)
        {
            return await _context.Recipes
                .Where(r => r.RecipeId == id && !r.IsDeleted)
                .Include(r => r.Ingredients)
                .Select(r => RecipeDetailViewModel.FromRecipe(r))
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<RecipeSummaryViewModel>> GetRecipes()
        {
            return await _context.Recipes
                .Where(r => !r.IsDeleted)
                .Select(r => RecipeSummaryViewModel.FromRecipe(r))
                .ToListAsync();
        }
    }
}
