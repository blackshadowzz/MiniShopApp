using Helpers.Responses;
using MiniShopApp.Models;

namespace MiniShopApp.Infrastructures.Services.Interfaces
{
    public interface IUserCustomerService
    {
        Task<Result<IEnumerable<ViewUserCustomers>>> GetAllAsync(string? filter = null);
    }
}
