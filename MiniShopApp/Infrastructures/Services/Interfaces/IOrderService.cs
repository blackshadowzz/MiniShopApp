using Helpers.Responses;
using MiniShopApp.Models.Customers;
using MiniShopApp.Models.Orders;

namespace MiniShopApp.Infrastructures.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Result<string>> CreateAsync(long customerId, TbOrder model);
        Task<Result<string>> CreateAsync(TbOrder model,CancellationToken cancellationToken=default);
        Task<Result<string>> ModifiedAsync(OrderUpdateModel model,CancellationToken cancellationToken=default);
        Task<Result<string>> ModifiedStatusAsync(long Id, Statuses statuses, CancellationToken cancellationToken=default);
        Task<Result<string>> ModifiedStatusAsync(List<long> Id, Statuses statuses, CancellationToken cancellationToken=default);
        Task<Result<IEnumerable<ViewTbOrders>>> GetOrderSummaryAsync(string? filter="");
        Task<Result<IEnumerable<ViewTbOrderDetails>>> GetOrderDetailsAsync(long? orderId);
        Task<Result<IEnumerable<TbOrderDetails>>> GetOrdersByUserAsync(long? customerId);
        Task<Result<IEnumerable<ViewTbOrders>>> GetOrderByUserAsync(long? customerId);
        Task<Result<IEnumerable<ViewTbOrders>>> GetOrderByDateAsync(string? filter="", DateTime? dateTime=null, long? customerId=0);
    }
}
