using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeWebApp.Services;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Pages.Recipes
{
    public class EditModel : PageModel
    {
        private readonly IRecipeService _service;
        private readonly ILogger<EditModel> _logger;

        [BindProperty]
        public UpdateRecipeCommand Input { get; set; }

        public EditModel(IRecipeService service, ILogger<EditModel> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Input = await _service.GetRecipeForUpdate(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get recipe: {id}", id);
                return NotFound();
            }

            if (Input is null)
            {
                _logger.LogWarning("Recipe not found: {id}", id);
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Input.RecipeId = id;

            try
            {
                if (ModelState.IsValid)
                {
                    await _service.UpdateRecipe(Input);
                    return RedirectToPage("View", new { id = Input.RecipeId });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Failed to update recipe");
                _logger.LogError(ex, "Failed to update recipe: {id}", Input.RecipeId);
            }

            return Page();
        }
    }
}
