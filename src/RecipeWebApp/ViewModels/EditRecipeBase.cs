﻿using RecipeWebApp.ViewModels.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RecipeWebApp.ViewModels
{
    [TimeToCookRequired]
    public class EditRecipeBase
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [Range(0, 24), DisplayName("Time to cook (hrs)")]
        public int TimeToCookHrs { get; set; }

        [Range(0, 59), DisplayName("Time to cook (mins)")]
        public int TimeToCookMins { get; set; }

        public string Method { get; set; } = string.Empty;

        [DisplayName("Vegetarian?")]
        public bool IsVegetarian { get; set; }

        [DisplayName("Vegan?")]
        public bool IsVegan { get; set; }
    }
}
