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

        public UpdateRecipeCommand(int recipeId, EditRecipeBase editBase)
        {
            RecipeId = recipeId;
            Name = editBase.Name;
            TimeToCookHrs = editBase.TimeToCookHrs;
            TimeToCookMins = editBase.TimeToCookMins;
            Method = editBase.Method;
            IsVegetarian = editBase.IsVegetarian;
            IsVegan = editBase.IsVegan;
        }
    }
}
