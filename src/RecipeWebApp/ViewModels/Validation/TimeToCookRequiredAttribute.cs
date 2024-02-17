using System.ComponentModel.DataAnnotations;

namespace RecipeWebApp.ViewModels.Validation
{
    public class TimeToCookRequiredAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is EditRecipeBase editRecipe
                && editRecipe.TimeToCookHrs + editRecipe.TimeToCookMins > 0)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Time to cook should be filled");
            }
        }
    }
}
