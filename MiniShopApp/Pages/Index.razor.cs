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
                        //NotificationService.Notify(Radzen.NotificationSeverity.Warning, "User Id By UserState: ", userState.UserId.ToString());
                        //NotificationService.Notify(Radzen.NotificationSeverity.Warning, "User Id By URL: ", userCustId.ToString());
                    }
                    // Simulate fetching user ID from a service or storage

                    await localStorage.SetAsync("customerId", userId.ToString()!);
                }
                //products = (await productService.GetAllAsync()).ToList();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(Radzen.NotificationSeverity.Error, "Loading Error...", ex.Message);
                Console.WriteLine($"Error: {ex.Message}");
            }
            await base.OnInitializedAsync();
        }
        async Task onCheckOrder()
        {

            //NotificationService.Notify(Radzen.NotificationSeverity.Warning, "User Id By UserState: ", userState.UserId.ToString());
            //NotificationService.Notify(Radzen.NotificationSeverity.Warning, "User Id By URL: ", userId.ToString());
            NavigationManager.NavigateTo($"/orders");
        }
    }
}