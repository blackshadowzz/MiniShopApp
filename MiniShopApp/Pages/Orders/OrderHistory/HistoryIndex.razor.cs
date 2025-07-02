using Microsoft.AspNetCore.Components;
using MiniShopApp.Data.TelegramStore;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Orders;
using Helpers.Responses;

namespace MiniShopApp.Pages.Orders.OrderHistory
{
    public partial class HistoryIndex(IOrderService OrderService)
    {
        [Parameter] public long? userId { get; set; }
        protected List<ViewTbOrders> Orders { get; set; } = new();
        protected bool IsLoading { get; set; } = false;
        protected string? ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (userId == null)
                userId = 1274162242;
            await GetOrders();
            await base.OnInitializedAsync();
        }

        protected async Task GetOrders()
        {
            IsLoading = true;
            ErrorMessage = null;
            try
            {
                if (userId.HasValue)
                {
                    var result = await OrderService.GetOrderByUserAsync(userId);
                    if (result.IsSuccess && result.Data != null)
                    {
                        Orders = result.Data.ToList();
                    }
                    else
                    {
                        ErrorMessage = result.Errors?.ErrMessage ?? "Failed to load orders.";
                    }
                }
                else
                {
                    ErrorMessage = "User ID is not specified.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                SnackbarService.Add(ErrorMessage, MudBlazor.Severity.Info);
                IsLoading = false;
            }
            
        }
    }
}
