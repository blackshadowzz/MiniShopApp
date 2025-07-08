using Domain.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MiniShopApp.Pages.Account

{
    public class LogInHandleModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LogInHandleModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync(string email, string password, bool rememberMe, string returnUrl = "/")
        {
            var user = await _userManager.FindByEmailAsync(email) ?? await _userManager.FindByNameAsync(email);
            if (user == null)
                return Redirect($"/account/login?error=Invalid credentials");

            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, false);
            if (result.Succeeded)
                return LocalRedirect(returnUrl);

            return Redirect($"/account/login?error=Invalid credentials");
        }
    }
}
