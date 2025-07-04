using DocumentFormat.OpenXml.Spreadsheet;
using Helpers.InformationLogs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MiniShopApp.Data.TelegramStore;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Customers;
using MiniShopApp.Models.Items;

namespace MiniShopApp.Pages
{
    public partial class Index
    {
        [Inject] public ProtectedSessionStorage SessionStorage { get; set; } = default!;
        [Inject] public ProtectedLocalStorage localStorage { get; set; } = default!;
        //[Inject] public UserState userState { get; set; } = default!;
        //private readonly botService botService;
        bool isLoading = false;
        protected ViewUserCustomers? userCustomer = new();
        private readonly IUserCustomerService customerService;
        public Index(IUserCustomerService customerService)
        {
            this.customerService = customerService;
        }
        protected long? userId = null;
       
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
                    //await SessionStorage.SetAsync("userId", userId.ToString()!);
                    //await localStorage.SetAsync("customerId", userId.ToString()!);
                }
               await GetUserInfo(userId);
            }
            catch (Exception ex)
            {
                var logMessage = $"Message log(While Start App): {ex.Message}\n";
                SystemLogs.UserLogPlainText(logMessage);
                throw new Exception(logMessage, ex);
            }
            await base.OnInitializedAsync();
        }

        private async Task GetUserInfo(long? Id)
        {
            try
            {
                var user = await customerService.GetUserByIdAsync(userId);
                if (user.IsSuccess)
                {
                    userCustomer = user.Data;
                    // Write new user info to a plain text file
                    var userInfo = $"while Start App: " +
                        $"UserId: {user.Data?.Id}, " +
                        $"FirstName: {user.Data?.FirstName}, " +
                        $"LastName: {user.Data?.LastName}, " +
                        $"Username: {user.Data?.UserName}, " +
                        $"Phone: {user.Data?.phoneNumber}, " +
                        $"Start App Dated: {DateTime.Now}\n";
                   SystemLogs.UserLogPlainText(userInfo);
                }else
                    SystemLogs.UserLogPlainText($"Error Get User: {user.ErrMessage}\n");

            }
            catch (Exception ex)
            {
                var logMessage = $"Message log(While Start App): {ex.Message}\n";
                SystemLogs.UserLogPlainText(logMessage);
                throw;
            }
        }
        ////Get device location if enable
        //private async Task RequestLocation()
        //{
        //    await jS.InvokeVoidAsync("getLocation");
        //}

        //[JSInvokable("ReceiveLocation")]
        //public static Task ReceiveLocation(double latitude, double longitude)
        //{
        //    //// Save to your database here
        //    //using var db = new YourDbContext();
        //    //var userLocation = new LocationRecord
        //    //{
        //    //    Latitude = latitude,
        //    //    Longitude = longitude,
        //    //    Timestamp = DateTime.UtcNow
        //    //};
        //    //db.LocationRecords.Add(userLocation);
        //    //db.SaveChanges();

        //    return Task.CompletedTask;
        //}

    }
}