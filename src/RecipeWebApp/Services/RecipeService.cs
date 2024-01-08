using Microsoft.EntityFrameworkCore;
using RecipeWebApp.Entities;
using RecipeWebApp.Infrastructure;
using RecipeWebApp.Services.Exceptions;
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

        public async Task<UpdateRecipeCommand?> GetRecipeForUpdate(int id)
        {
            return await _context.Recipes
                .Where(r => r.RecipeId == id && !r.IsDeleted)
                .Select(r => new UpdateRecipeCommand(r))
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<RecipeSummaryViewModel>> GetRecipes()
        {
            return await _context.Recipes
                .Where(r => !r.IsDeleted)
                .Select(r => RecipeSummaryViewModel.FromRecipe(r))
                .ToListAsync();
        }

        public async Task UpdateRecipe(UpdateRecipeCommand cmd)
        {
            var recipe = await _context.Recipes.FindAsync(cmd.RecipeId);
            if (recipe is null)
            {
                throw new RecipeNotFoundException($"Unable to find the recipe: {cmd.RecipeId}");
            }

            if (recipe.IsDeleted)
            {
                throw new RecipeIsDeletedException($"Unable to update a deleted recipe: {cmd.RecipeId}");
            }

            UpdateRecipe(recipe, cmd);
            await _context.SaveChangesAsync();
        }

        static void UpdateRecipe(Recipe recipe, UpdateRecipeCommand cmd)
        {
            recipe.RecipeId = cmd.RecipeId;
            recipe.Name = cmd.Name;
            recipe.TimeToCook = new TimeSpan(cmd.TimeToCookHrs, cmd.TimeToCookMins, 0);
            recipe.Method = cmd.Method;
            recipe.IsVegetarian = cmd.IsVegetarian;
            recipe.IsVegan = cmd.IsVegan;
        }

        public async Task DeleteRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe is null)
            {
                throw new RecipeNotFoundException($"Unable to find the recipe: {id}");
            }

            recipe.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }
}
