using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Orders;

namespace MiniShopApp.Pages.Orders
{
    public partial class OrderViewToday(IOrderService orderService)
    {
        List<ViewTbOrders> orders = new List<ViewTbOrders>();
        protected DateTime today= DateTime.Now.Date;
        string? filter = string.Empty;
        protected override async Task OnInitializedAsync()
        {
            await GetOrderToday();
            await base.OnInitializedAsync();
        }
        async Task GetOrderToday()
        {
            try
            {
                var results = await orderService.GetOrderByDateAsync(filter, today, 0);
                if (results.IsSuccess)
                {
                    orders=results.Data!.OrderByDescending(x=>x.Id).ToList();
                }
            }
            catch (Exception ex)
            {


            }
        }
    }
}