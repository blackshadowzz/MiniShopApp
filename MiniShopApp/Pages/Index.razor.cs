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

                    //userId=userState.UserId;
                    var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
                    if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("userid", out var userIdStr) && long.TryParse(userIdStr, out var userCustId))
                    {
                       

                        userState.UserId = userCustId;
                        userId = userCustId;
                    await SessionStorage.SetAsync("userId", userId.ToString()!);
                    await localStorage.SetAsync("customerId", userId.ToString()!);
                }
            }
            catch (Exception ex)
            {
                //NotificationService.Notify(Radzen.NotificationSeverity.Error, "Loading Error...", ex.Message);
                Console.WriteLine($"Error: {ex.Message}");
            }
            await base.OnInitializedAsync();
        }
        bool isLoading = false;
        async void onCheckOrder()
        {
            //isLoading = true;
            //await localStorage.SetAsync("customerId", userState.UserId.ToString()!);
            userState.UserId=userId;
            NavigationManager.NavigateTo($"/order/ordering/{userId}");
            //await Task.Delay(500).ContinueWith(_ => 
            //{
            //    isLoading = false;
            //    StateHasChanged();
            //});
            isLoading = false;
        }
    }
}