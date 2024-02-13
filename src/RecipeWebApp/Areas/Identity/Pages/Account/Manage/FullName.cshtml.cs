using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeWebApp.Entities;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RecipeWebApp.Areas.Identity.Pages.Account.Manage
{
    public partial class FullNameModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<FullNameModel> _logger;

        public FullNameModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<FullNameModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public string Username { get; set; }

        public string FullName { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(100)]
            [Display(Name = "New full name")]
            public string NewFullName { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser is null)
            {
                var userId = _userManager.GetUserId(User);
                _logger.LogError("Unable to load user: {UserId}", userId);
                return NotFound("Unable to load user info");
            }

            _logger.LogInformation("User loaded: {UserId}", appUser.Id);

            var claims = await _userManager.GetClaimsAsync(appUser);
            _logger.LogInformation("User claims loaded: {UserId}", appUser.Id);

            var claim = claims.FirstOrDefault(c => c.Type == "FullName");
            FullName = claim is null ? string.Empty : claim.Value;

            Input = new InputModel
            {
                NewFullName = FullName
            };

            return Page();
        }

        public async Task<IActionResult> OnPostChangeAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser is null)
            {
                var userId = _userManager.GetUserId(User);
                _logger.LogError("Unable to load user: {UserId}", userId);
                StatusMessage = "Error changing FullName";
                return Page();
            }

            try
            {
                await ChangeFullNameClaim(appUser);

                _logger.LogInformation("User's full name changed: {UserId}", appUser.Id);
                StatusMessage = "Your full name has been changed";
                return Page();
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Failed to change user's full name: {UserId}", appUser.Id);
                StatusMessage = "Error changing FullName";
                return Page();
            }
        }

        private async Task ChangeFullNameClaim(ApplicationUser appUser)
        {
            if (Input.NewFullName == FullName)
            {
                return;
            }

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var claim = identity?.FindFirst("FullName");
            if (claim is null)
            {
                throw new ApplicationException($"User's FullName claim not found. User: {appUser.Id}");
            }

            var result = await _userManager.ReplaceClaimAsync(appUser, claim, new Claim("FullName", Input.NewFullName));
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ApplicationException($"Errors occurred while replacing FullName claim. User: {appUser.Id}. Errors: {errors}");
            }

            await _signInManager.RefreshSignInAsync(appUser);
        }
    }
}
