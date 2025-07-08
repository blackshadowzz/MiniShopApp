using Domain.IdentityModel;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MiniShopApp.Components.Account
{
    public class SessionAuthProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public SessionAuthProvider(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var context = _httpContextAccessor.HttpContext;

            if (context?.Session.TryGetValue("userId", out var userIdBytes) == true)
            {
                var userId = System.Text.Encoding.UTF8.GetString(userIdBytes);
                var user = await _userManager.FindByIdAsync(userId);

                if (user is not null)
                {
                    
                    var roles = await _userManager.GetRolesAsync(user);
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.Surname, user.TelegramUserId.ToString() ?? ""),
                };
                    foreach (var role in roles)
                        claims.Add(new Claim(ClaimTypes.Role, role));

                    var identity = new ClaimsIdentity(claims, "SessionAuth");
                    return new AuthenticationState(new ClaimsPrincipal(identity));
                }
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public void SignOut()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void Refresh()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }

}
