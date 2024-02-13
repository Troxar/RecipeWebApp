using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeWebApp.Services;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IRecipeService _service;
        private readonly ILogger<IndexModel> _logger;

        public IEnumerable<RecipeSummaryViewModel> Recipes { get; private set; }

        public IndexModel(IRecipeService service, ILogger<IndexModel> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Recipes = await _service.GetRecipes();
                _logger.LogInformation("Recipes loaded: {RecipeCount}", Recipes.Count());
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get recipes");
                return RedirectToPage("/Error");
            }
        }
    }
}
