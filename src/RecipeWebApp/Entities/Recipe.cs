﻿namespace RecipeWebApp.Entities
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public TimeSpan TimeToCook { get; set; }
        public bool IsDeleted { get; set; }
        public string Method { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; }

    }
}
