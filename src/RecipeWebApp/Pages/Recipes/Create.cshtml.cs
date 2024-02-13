using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeWebApp.Entities;
using RecipeWebApp.Services;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Pages.Recipes
{
    [Authorize]
    public class CreateModel : PageModel
    {
        readonly IRecipeService _service;
        readonly UserManager<ApplicationUser> _userManager;
        readonly ILogger<CreateModel> _logger;

        [BindProperty]
        public CreateRecipeCommand Input { get; set; }

        public CreateModel(IRecipeService service,
            UserManager<ApplicationUser> userManager,
            ILogger<CreateModel> logger)
        {
            _service = service;
            _userManager = userManager;
            _logger = logger;
        }

        public void OnGet()
        {
            Input = new CreateRecipeCommand();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser is null)
            {
                _logger.LogWarning("Unable to load user: {UserId}", _userManager.GetUserId(User));
                return NotFound("Unable to load user info");
            }

            try
            {
                var recipe = await _service.CreateRecipe(Input, appUser);
                _logger.LogInformation("Recipe created: {RecipeId}", recipe.Id);
                return Redirect("/");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Failed to save");
                _logger.LogError(ex, "Failed to save recipe");
                return Page();
            }
        }
    }
}
