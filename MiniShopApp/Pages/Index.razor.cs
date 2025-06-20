using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MiniShopApp.Data.TelegramStore;
using MiniShopApp.Models.Items;

namespace MiniShopApp.Pages
{
    public partial class Index
    {
        [Inject] public ProtectedSessionStorage SessionStorage { get; set; } = default!;
        [Inject] public ProtectedLocalStorage localStorage { get; set; } = default!;
        [Inject] public UserState userState { get; set; } = default!;
        //private readonly botService botService;

        public Index()
        {

        }
        long? userId = null;
        protected List<Product> products = new List<Product>();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (userId == null)
                {
                    //userId=userState.UserId;
                    var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
                    if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("userid", out var userIdStr) && long.TryParse(userIdStr, out var userCustId))
                    {
                        userState.UserId = userCustId;
                        userId = userCustId;
                        
                    }
                    // Simulate fetching user ID from a service or storage

                    await localStorage.SetAsync("customerId", userId.ToString()!);
                }
                
            }
            catch (Exception ex)
            {
                //NotificationService.Notify(Radzen.NotificationSeverity.Error, "Loading Error...", ex.Message);
                Console.WriteLine($"Error: {ex.Message}");
            }
            await base.OnInitializedAsync();
            StateHasChanged();
        }
        async Task onCheckOrder()
        {

            
            NavigationManager.NavigateTo($"/orders");
        }
    }
}