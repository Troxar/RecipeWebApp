using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Pages.Recipes
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        public CreateRecipeCommand Input { get; set; }

        public void OnGet()
        {
            Input = new CreateRecipeCommand();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                return Redirect("/");
            }

            return Page();
        }
    }
}
