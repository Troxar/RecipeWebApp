using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeWebApp.Services;
using RecipeWebApp.Services.Exceptions;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Pages.Recipes
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IRecipeService _service;
        private readonly IAuthorizationService _authService;
        private readonly ILogger<EditModel> _logger;


        [BindProperty]
        public UpdateRecipeCommand Input { get; set; }

        public EditModel(IRecipeService service,
            IAuthorizationService authService,
            ILogger<EditModel> logger)
        {
            _service = service;
            _authService = authService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id)
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
                Input = await _service.GetRecipeForUpdate(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get recipe for update: {id}", id);
                return RedirectToPage("/Error");
            }

            if (Input is null)
            {
                _logger.LogWarning("Recipe for update not found: {id}", id);
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
            catch (RecipeException ex)
            {
                _logger.LogWarning(ex, "Failed to get recipe for updating: {id}", id);
                return RedirectToPage("/Error");
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
