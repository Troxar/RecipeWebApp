using RecipeWebApp.Entities;

namespace RecipeWebApp.ViewModels
{
    public class RecipeSummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TimeToCook { get; set; }

        public static RecipeSummaryViewModel FromRecipe(Recipe recipe)
        {
            return new RecipeSummaryViewModel
            {
                Id = recipe.RecipeId,
                Name = recipe.Name,
                TimeToCook = $"{recipe.TimeToCook.TotalMinutes} mins"
            };
        }
    }
}
