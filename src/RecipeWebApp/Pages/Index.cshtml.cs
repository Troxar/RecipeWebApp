using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Pages
{
    public class IndexModel : PageModel
    {
        public IEnumerable<RecipeSummaryViewModel> Recipes { get; private set; }

        public void OnGet()
        {
            Recipes = Array.Empty<RecipeSummaryViewModel>();
        }
    }
}
