using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MemoApp.Resources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace MemoApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager, IStringLocalizer<SharedResource> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _localizer = localizer;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "The Email field is required.")]
            [EmailAddress(ErrorMessage = "The Email field is not a valid e - mail address.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "The Password field is required.")]
            [StringLength(100, ErrorMessage = "The Password must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                if (!_userManager.Users.Any())
                {
                    bool existAdminRole = await _roleManager.RoleExistsAsync("Admin");
                    if (!existAdminRole)
                    {
                        //creating role for administrator
                        var adminRole = new IdentityRole
                        {
                            Name = "Admin"
                        };
                        await _roleManager.CreateAsync(adminRole);
                    }
                    var admin = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                    var result = await _userManager.CreateAsync(admin, Input.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(admin, "Admin");
                        await _signInManager.SignInAsync(admin, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    var existUserRole = await _roleManager.RoleExistsAsync("User");
                    if (!existUserRole)
                    {
                        //creating role for user
                        var userRole = new IdentityRole
                        {
                            Name = "User"
                        };
                        await _roleManager.CreateAsync(userRole);
                    }
                    var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
