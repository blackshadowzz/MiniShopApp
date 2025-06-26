using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models;

namespace MiniShopApp.Pages.Settings.UserManages
{
    public partial class UserCustomerLogIndex(IUserCustomerService userCustomer)
    {
        protected IEnumerable<ViewUserCustomers>? userCustomers = [];
        protected string? filter = null;
        protected bool IsLoading = false;
        public string? UserLogContent { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            await base.OnInitializedAsync();
        }

        protected async Task LoadDataAsync()
        {
            IsLoading = true;
            try
            {
                var result = await userCustomer.GetAllAsync(filter);
                if (result.IsSuccess)
                {
                    userCustomers = result.Data?.OrderByDescending(x => x.Id);
                    IsLoading = false;
                }
                else
                {
                    IsLoading = false;
                    // Handle the error, e.g., show a notification or log it
                    Console.WriteLine($"Error loading user customers: {result.ErrMessage}");
                    ////NotificationService.Notify(Radzen.NotificationSeverity.Error, "Error", result.ErrMessage);
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
                // Handle exceptions, e.g., log them or show a notification
                //NotificationService.Notify(Radzen.NotificationSeverity.Error, "Error", ex.Message);
                Console.WriteLine($"Exception occurred while loading user customers: {ex.Message}");
            }
        }
        protected async Task ViewDetails(long? id)
        {
            await Task.CompletedTask;
        }
        protected  async Task ReadUserLog()
        {
            var wwwrootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
            var logFilePath = Path.Combine(wwwrootPath, "UserLog.txt");

            if (File.Exists(logFilePath))
            {
                UserLogContent = File.ReadAllText(logFilePath);
            }
            else
            {
                UserLogContent = "No log file found.";
            }
        }
        protected async Task CreateAlert()
        {
            try
            {
                var data = userCustomers!.Select(x => new CustomerAlertMessege
                {
                    Id = x.Id,
                    CustomerId = x.CustomerId,
                    AlertMessege = messegeText,
                    FirstName = x.FirstName,
                }).Where(x => x.CustomerId.HasValue && x.CustomerId.Value > 0);

                alertMessege = data.ToList();
                var result = await userCustomer.CreateUserAlertAsync(alertMessege);
                if (result.IsSuccess)
                {
                    SnackbarService.Add(result.Data!, MudBlazor.Severity.Success);

                    IsLoading = false;
                }
                else
                {
                    IsLoading = false;
                    // Handle the error, e.g., show a notification or log it
                    Console.WriteLine($"Error loading user customers: {result.ErrMessage}");
                    SnackbarService.Add("Error Creating: "+result.ErrMessage, MudBlazor.Severity.Error);

                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
                // Handle exceptions, e.g., log them or show a notification
                SnackbarService.Add("Error at UI " + ex.Message, MudBlazor.Severity.Error);
                Console.WriteLine($"Exception occurred while loading user customers: {ex.Message}");
            }
        }
    }
}