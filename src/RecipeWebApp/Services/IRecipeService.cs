using RecipeWebApp.Entities;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Services
{
    public interface IRecipeService
    {
        Task<RecipeDetailViewModel> CreateRecipe(CreateRecipeCommand cmd, ApplicationUser user);
        Task<IEnumerable<RecipeSummaryViewModel>> GetRecipes();
        Task<RecipeDetailViewModel?> GetRecipe(int id);
        Task<UpdateRecipeCommand?> GetRecipeForUpdate(int id);
        Task UpdateRecipe(UpdateRecipeCommand cmd);
        Task DeleteRecipe(int id);
        Task<IEnumerable<RecipeSummaryViewModel>> GetUserRecipes(string userId, int count);
    }
}
