using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Services
{
    public interface IRecipeService
    {
        Task<int> CreateRecipe(CreateRecipeCommand cmd);
        Task<IEnumerable<RecipeSummaryViewModel>> GetRecipes();
        Task<RecipeDetailViewModel?> GetRecipe(int id);
        Task<UpdateRecipeCommand?> GetRecipeForUpdate(int id);
        Task UpdateRecipe(UpdateRecipeCommand cmd);
        Task DeleteRecipe(int id);
    }
}
