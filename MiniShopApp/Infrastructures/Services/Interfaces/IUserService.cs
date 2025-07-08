using Domain.IdentityModel;

namespace MiniShopApp.Infrastructures.Services.Interfaces
{
    public interface IUserService
    {
        Task<(bool success, string? error)> LoginAsync(UserRequest input);
        Task LogoutAsync();
    }
}
