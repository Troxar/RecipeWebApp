using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeWebApp.Services;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Pages
{
    public class IndexModel : PageModel
    {
        readonly IRecipeService _service;

        public IEnumerable<RecipeSummaryViewModel> Recipes { get; private set; }

        public IndexModel(IRecipeService service)
        {
            _service = service;
        }

        public async Task OnGet()
        {
            Recipes = await _service.GetRecipes();
        }
    }
}
