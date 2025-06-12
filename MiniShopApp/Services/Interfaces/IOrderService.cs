using Helpers.Responses;
using MiniShopApp.Models;
using MiniShopApp.Models.Orders;

namespace MiniShopApp.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Result<string>> CreateAsync(long customerId, TbOrder model);
        Task<Result<TbOrder>> GetOrderSummaryAsync(long customerId, TbOrder model);
        Task<Result<IEnumerable<TbOrderDetails>>> GetOrderDetailsAsync(long customerId, TbOrderDetails model);
        Task<Result<UserCustomer>> GetUserAsync(long? filter);
    }
}
