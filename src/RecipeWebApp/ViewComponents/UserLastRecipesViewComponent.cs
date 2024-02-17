using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeWebApp.Entities;
using RecipeWebApp.Services;

namespace RecipeWebApp.ViewComponents
{
    public class UserLastRecipesViewComponent : ViewComponent
    {
        private readonly IRecipeService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserLastRecipesViewComponent(IRecipeService service,
            UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(int numberOfRecipes)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View("Unauthenticated");
            }

            var userId = _userManager.GetUserId(HttpContext.User);
            var recipes = await _service.GetUserRecipes(userId, numberOfRecipes);

            return View(recipes);
        }
    }
}
