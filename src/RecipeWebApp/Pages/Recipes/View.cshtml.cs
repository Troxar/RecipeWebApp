using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeWebApp.Services;
using RecipeWebApp.Services.Exceptions;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Pages.Recipes
{
    public class ViewModel : PageModel
    {
        private readonly IRecipeService _service;
        private readonly IAuthorizationService _authService;
        private readonly ILogger<ViewModel> _logger;

        public RecipeDetailViewModel Recipe { get; set; }
        public bool CanEditRecipe { get; set; }

        public ViewModel(IRecipeService service,
            IAuthorizationService authService,
            ILogger<ViewModel> logger)
        {
            _service = service;
            _authService = authService;
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

            var authResult = await _authService.AuthorizeAsync(User, Recipe, "CanManageRecipe");
            CanEditRecipe = authResult.Succeeded;

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var recipeDetail = await _service.GetRecipe(id);
            if (recipeDetail is null)
            {
                _logger.LogWarning("Recipe not found: {id}", id);
                return NotFound();
            }

            var authResult = await _authService.AuthorizeAsync(User, recipeDetail, "CanManageRecipe");
            if (!authResult.Succeeded)
            {
                return new ForbidResult();
            }

            try
            {
                await _service.DeleteRecipe(id);
            }
            catch (RecipeException ex)
            {
                _logger.LogWarning(ex, "Failed to get recipe for deleting: {id}", id);
                return RedirectToPage("/Error");
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
