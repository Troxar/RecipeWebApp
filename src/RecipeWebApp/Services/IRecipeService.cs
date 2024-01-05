using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Services
{
    public interface IRecipeService
    {
        Task<int> CreateRecipe(CreateRecipeCommand cmd);
        Task<IEnumerable<RecipeSummaryViewModel>> GetRecipes();
    }
}
