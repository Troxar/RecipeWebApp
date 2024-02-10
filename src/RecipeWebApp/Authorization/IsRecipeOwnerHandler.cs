using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using RecipeWebApp.Entities;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Authorization
{
    public class IsRecipeOwnerHandler
        : AuthorizationHandler<IsRecipeOwnerRequirement, RecipeDetailViewModel>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IsRecipeOwnerHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IsRecipeOwnerRequirement requirement,
            RecipeDetailViewModel resource)
        {
            var appUser = await _userManager.GetUserAsync(context.User);
            if (appUser is not null && resource.CreatedById == appUser.Id)
            {
                context.Succeed(requirement);
            }
        }
    }
}
