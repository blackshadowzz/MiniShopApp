using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Orders;
using MiniShopApp.Shared;
using MudBlazor;

namespace MiniShopApp.Pages.Orders
{
    public partial class OrderCreatePage
    {
        [Inject]
        protected ProtectedLocalStorage localStorage { get; set; } = default!;
        private readonly IOrderService orderService;

        public OrderCreatePage(IOrderService orderService)
        {
            this.orderService = orderService;

            // Constructor logic if needed
        }
        public OrderCreateModel Order = new OrderCreateModel();
        public List<TbOrderDetails>? orderDetails = [];

        protected override async Task OnInitializedAsync()
        {
           GetDate(); 
            await base.OnInitializedAsync();
        }

        protected async void GetDate()
        {
            try
            {
                var result = await localStorage.GetAsync<OrderCreateModel>("orderToCreate");
                if (result.Success && result.Value != null)
                {
                    Order = result.Value;
                    orderDetails = Order.TbOrderDetails!.ToList() ?? new List<TbOrderDetails>();
                }
                else
                {
                    Order = new OrderCreateModel();
                }
            }
            catch (Exception ex)
            {
                // Log the error or notify the user as needed
                Console.WriteLine($"Error accessing local storage: {ex.Message}");
                Order = new OrderCreateModel();
            }
            StateHasChanged(); // Notify the component to re-render with the loaded data
        }
        protected async Task HandleValidSubmit()
        {
            try
            {
                IsLoading= true;
                if (Order.TableNumber == null)
                {
                    SnackbarService.Add("Please enter a valid table number.", MudBlazor.Severity.Warning);
                    IsLoading = false;

                    // NotificationService.Notify(Radzen.NotificationSeverity.Error, "Invalid Table Number", "Please enter a valid table number.");
                    return;
                }
                if (orderDetails == null || !orderDetails.Any())
                {
                    IsLoading = false;

                    SnackbarService.Add("Please back to add at least one item to the order.", MudBlazor.Severity.Warning);
                    return;
                }
                var result = await DialogService.ShowMessageBox(
                    "Confirmation",
                    $"Are you sure you want to submit this order with total price: {Order.TotalPrice?.ToString("c2")}?", 
                    "Yes", "No");
                if (result==true)
                {
                    
                    TbOrder order = new TbOrder
                    {
                        CustomerId = Order.CustomerId,
                        TableNumber = Order.TableNumber,
                        ItemCount = Order.ItemCount,
                        SubPrice = Order.SubPrice,
                        DiscountPrice = Order.DiscountPrice,
                        TotalPrice = Order.TotalPrice,
                        Notes = Order.Notes,
                        CreatedDT = DateTime.Now,
                        TbOrderDetails = orderDetails,
                    };
                    var message = await orderService.CreateAsync(Order.CustomerId, order);

                    if (message.IsSuccess)
                    {
                        // Clear the order details after successful submission
                        orderDetails!.Clear();
                        Order = new OrderCreateModel(); // Reset the order model
                        await localStorage.SetAsync("orderToCreate", Order);
                        SnackbarService.Add("Your order has been successfully created.", MudBlazor.Severity.Success);
                        IsLoading = false;

                        NavigationManager.NavigateTo("/orders");
                        await Task.Delay(500).ContinueWith(_ => 
                        {
                            IsLoading = false;
                            StateHasChanged();
                        });
                    }
                    else
                    {
                        IsLoading = false;

                        Console.WriteLine($"Error creating order: {message.Errors}");
                    }
                }
                
            }
            catch (Exception ex)
            {
                IsLoading = false;

                // Handle any exceptions that occur during form submission
                Console.WriteLine($"Error during form submission: {ex.Message}");
                SnackbarService.Add("An error occurred while creating the order. Please try again." + ex.InnerException!.Message, MudBlazor.Severity.Error);
                return;
            }
           
        }
    }
}