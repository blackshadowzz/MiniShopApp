using Domain.IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MiniShopApp.Components.Account;
using MiniShopApp.Infrastructures.Services.Interfaces;
using System.Security.Claims;

namespace MiniShopApp.Infrastructures.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly AuthenticationStateProvider _authProvider;

        public UserService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            AuthenticationStateProvider authProvider,
            ILogger<AuthenticationService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _authProvider = authProvider;
            _logger = logger;
        }
        public async Task<(bool success, string? error)> LoginAsync(UserRequest input)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return (false, "No HTTP context.");

            var user = await _userManager.FindByEmailAsync(input.Email)
                       ?? await _userManager.FindByNameAsync(input.Email);
            if (user == null) return (false, "User not found.");

            if (!(bool)user.IsActive!)
                return (false, "User is deactivated.");

            var isValid = await _userManager.CheckPasswordAsync(user, input.Password);
            if (!isValid)
                return (false, "Invalid password.");

            //var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.Surname, user.TelegramUserId.ToString() ?? ""),
        };
            //claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var principal = new ClaimsPrincipal(identity);

            await context.SignInAsync(IdentityConstants.ApplicationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = input.RememberMe
            });

            if (_authProvider is CookieAuthStateProvider cookieAuth)
                cookieAuth.NotifyAuthStateChanged();

            _logger.LogInformation("User {User} logged in successfully.", user.UserName);
            return (true, null);
        }

        public async Task LogoutAsync()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                await context.SignOutAsync(IdentityConstants.ApplicationScheme);
                if (_authProvider is CookieAuthStateProvider cookieAuth)
                    cookieAuth.NotifyAuthStateChanged();
            }
        }
    }
}
