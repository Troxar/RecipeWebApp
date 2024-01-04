using RecipeWebApp.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RecipeWebApp.ViewModels
{
    public class CreateRecipeCommand
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [Range(0, 24), DisplayName("Time to cook (hrs)")]
        public int TimeToCookHrs { get; set; }

        [Range(0, 59), DisplayName("Time to cook (mins)")]
        public int TimeToCookMins { get; set; }

        public string Method { get; set; }

        [DisplayName("Vegetarian?")]
        public bool IsVegetarian { get; set; }

        [DisplayName("Vegan?")]
        public bool IsVegan { get; set; }

        public IList<CreateIngredientCommand> Ingredients { get; set; } = new List<CreateIngredientCommand>();

        public Recipe ToRecipe()
        {
            return new Recipe
            {
                Name = Name,
                TimeToCook = new TimeSpan(TimeToCookHrs, TimeToCookMins, 0),
                Method = Method,
                IsVegetarian = IsVegetarian,
                IsVegan = IsVegan,
                Ingredients = Ingredients
                    .Select(i => i.ToIngredient())
                    .ToList()
            };
        }
    }
}
