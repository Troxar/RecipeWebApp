using RecipeWebApp.Entities;

namespace RecipeWebApp.ViewModels
{
    public class RecipeDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Method { get; set; }
        public DateTime LastModified { get; set; }
        public string CreatedById { get; set; }
        public IEnumerable<IngredientItem> Ingredients { get; set; }

        public static RecipeDetailViewModel FromRecipe(Recipe recipe)
        {
            return new RecipeDetailViewModel
            {
                Id = recipe.RecipeId,
                Name = recipe.Name,
                Method = recipe.Method,
                LastModified = recipe.LastModified,
                CreatedById = recipe.CreatedById,
                Ingredients = recipe.Ingredients
                    .Select(i => IngredientItem.FromIngredient(i))
            };
        }

        public class IngredientItem
        {
            public string Name { get; set; }
            public string Quantity { get; set; }

            public static IngredientItem FromIngredient(Ingredient ingredient)
            {
                return new IngredientItem
                {
                    Name = ingredient.Name,
                    Quantity = $"{ingredient.Quantity} {ingredient.Unit}"
                };
            }
        }
    }
}
