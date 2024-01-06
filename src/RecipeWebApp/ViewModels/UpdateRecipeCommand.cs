using RecipeWebApp.Entities;

namespace RecipeWebApp.ViewModels
{
    public class UpdateRecipeCommand : EditRecipeBase
    {
        public int RecipeId { get; set; }

        public UpdateRecipeCommand() { }

        public UpdateRecipeCommand(Recipe recipe)
        {
            RecipeId = recipe.RecipeId;
            Name = recipe.Name;
            TimeToCookHrs = recipe.TimeToCook.Hours;
            TimeToCookMins = recipe.TimeToCook.Minutes;
            Method = recipe.Method;
            IsVegetarian = recipe.IsVegetarian;
            IsVegan = recipe.IsVegan;
        }
    }
}
