using Microsoft.AspNetCore.Components;
using MiniShopApp.Data.TelegramStore;

namespace MiniShopApp.Pages.Settings
{
    public partial class AboutIndex
    {
        //[Inject] public UserState userState { get; set; } = default!;
        [Parameter] public long? userId { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
    }
}