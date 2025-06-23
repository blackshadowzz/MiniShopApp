using Microsoft.AspNetCore.Components;
using MiniShopApp.Data.TelegramStore;

namespace MiniShopApp.Pages.Orders.OrderHistory
{
    public partial class HistoryIndex
    {
        //[Inject]
        //protected UserState userState { get; set; } = default!;
        [Parameter] public long? userId { get; set; }
        protected override async Task OnInitializedAsync()
        {

            //userState.UserId = userId;

            await base.OnInitializedAsync();
        }
    }
}
