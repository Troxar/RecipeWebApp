using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeWebApp.Services;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Pages.Recipes
{
    public class ViewModel : PageModel
    {
        private readonly IRecipeService _service;
        private readonly ILogger<ViewModel> _logger;

        public RecipeDetailViewModel Recipe { get; set; }

        public ViewModel(IRecipeService service, ILogger<ViewModel> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Recipe = await _service.GetRecipe(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get recipe: {id}", id);
                return NotFound();
            }

            if (Recipe is null)
            {
                _logger.LogWarning("Recipe not found: {id}", id);
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await _service.DeleteRecipe(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete recipe: {id}", id);
                return RedirectToPage("/Error");
            }

            return RedirectToPage("/Index");
        }
    }
}
