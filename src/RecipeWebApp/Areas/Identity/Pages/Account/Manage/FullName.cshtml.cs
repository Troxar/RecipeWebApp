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
                _logger.LogError("Unable to load user {id}", _userManager.GetUserId(User));
                return NotFound("Unable to load user info");
            }

            var claims = await _userManager.GetClaimsAsync(appUser);
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
            if (ModelState.IsValid)
            {
                try
                {
                    await ChangeFullNameClaim();
                    StatusMessage = "Your full name has been changed";
                }
                catch (ApplicationException ex)
                {
                    _logger.LogWarning(ex, "Error changing FullName");
                    StatusMessage = "Error changing FullName";
                }
            }

            return Page();
        }

        private async Task ChangeFullNameClaim()
        {
            if (Input.NewFullName == FullName)
            {
                return;
            }

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser is null)
            {
                throw new ApplicationException($"Unable to load user {_userManager.GetUserId(User)}");
            }

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var claim = identity?.FindFirst("FullName");
            if (claim is null)
            {
                throw new ApplicationException($"FullName claim of user {appUser.Id} not found");
            }

            var result = await _userManager.ReplaceClaimAsync(appUser, claim, new Claim("FullName", Input.NewFullName));
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unable to change FullName claim of user {appUser.Id}");
            }

            await _signInManager.RefreshSignInAsync(appUser);
        }
    }
}
