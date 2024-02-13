using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeWebApp.Entities;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EditModel> _logger;

        [BindProperty]
        public UpdateRecipeCommand Input { get; set; }

        public EditModel(IRecipeService service,
            IAuthorizationService authService,
            UserManager<ApplicationUser> userManager,
            ILogger<EditModel> logger)
        {
            _service = service;
            _authService = authService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var recipeDetail = await _service.GetRecipe(id);
            if (recipeDetail is null)
            {
                _logger.LogWarning("Recipe not found: {RecipeId}", id);
                return NotFound();
            }

            _logger.LogInformation("Recipe loaded: {RecipeId}", id);

            var authResult = await _authService.AuthorizeAsync(User, recipeDetail, "CanManageRecipe");
            if (!authResult.Succeeded)
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                _logger.LogWarning("User {UserId} cannot manage the recipe: {RecipeId}", userId, id);
                return new ForbidResult();
            }

            try
            {
                Input = await _service.GetRecipeForUpdate(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get recipe for update: {RecipeId}", id);
                return RedirectToPage("/Error");
            }

            if (Input is null)
            {
                _logger.LogWarning("Recipe for update not found: {RecipeId}", id);
                return NotFound();
            }

            _logger.LogInformation("Recipe for update loaded: {RecipeId}", id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Input.RecipeId = id;

            try
            {
                await _service.UpdateRecipe(Input);
                _logger.LogInformation("Recipe updated: {RecipeId}", id);
                return RedirectToPage("View", new { id });
            }
            catch (RecipeException ex)
            {
                ModelState.AddModelError(string.Empty, "Failed to update recipe");
                _logger.LogWarning(ex, "Failed to update recipe: {RecipeId}", id);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update recipe: {RecipeId}", id);
                return RedirectToPage("/Error");
            }
        }
    }
}
