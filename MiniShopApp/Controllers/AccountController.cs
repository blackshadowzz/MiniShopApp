using Domain.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MiniShopApp.Controllers
{
    [Route("account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("login/form")]
        public async Task<IActionResult> Login([FromBody] UserRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(model.Email);
            }
            if (user is null)
            {
                return BadRequest($"Error:{model.Email} is not register yet.");
            }

            if (user.IsActive == false)
            {

                return BadRequest($"Error:User is already deactive....");
            }

            var checkedPassword = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!checkedPassword)
            {
                return BadRequest($"Error:Invalid Password...!");
            }
            if (user == null)
                return Unauthorized("User not found");

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
                return Redirect("/"); // ✅ redirect home or dashboard after login

            return Unauthorized("Login failed");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }

    
}
