using Helpers.Responses;
using MiniShopApp.Models;
using MiniShopApp.Models.Orders;

namespace MiniShopApp.Infrastructures.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Result<string>> CreateAsync(long customerId, TbOrder model);
        Task<Result<IEnumerable<ViewTbOrders>>> GetOrderSummaryAsync(string? filter="");
        Task<Result<IEnumerable<TbOrderDetails>>> GetOrderDetailsAsync(long? orderId);
        Task<Result<IEnumerable<TbOrderDetails>>> GetOrdersByUserAsync(long? customerId);
        Task<Result<UserCustomer>> GetUserAsync(long? filter);
        Task<Result<IEnumerable<ViewTbOrders>>> GetOrderByUserAsync(long? customerId);
    }
}
