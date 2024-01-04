using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeWebApp.Services;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Pages.Recipes
{
    public class CreateModel : PageModel
    {
        readonly IRecipeService _service;
        readonly ILogger<CreateModel> _logger;

        [BindProperty]
        public CreateRecipeCommand Input { get; set; }

        public CreateModel(IRecipeService service, ILogger<CreateModel> logger)
        {
            _service = service;
            _logger = logger;
        }

        public void OnGet()
        {
            Input = new CreateRecipeCommand();
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var recipe = await _service.CreateRecipe(Input);
                    return Redirect("/");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Failed to save");
                _logger.LogError(ex, "Failed to save recipe");
            }

            return Page();
        }
    }
}
