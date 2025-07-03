using Helpers.Responses;
using MiniShopApp.Models.Customers;

namespace MiniShopApp.Infrastructures.Services.Interfaces
{
    public interface IUserCustomerService
    {
        Task<Result<IEnumerable<ViewUserCustomers>>> GetAllAsync(string? filter = null);
        Task<Result<string>> CreateUserAlertAsync(List<CustomerAlertMessege> alertMesseges);
        Task<Result<string>> CreateUserAlertAsync(CustomerAlertMessege alertMessege);
    }
}
