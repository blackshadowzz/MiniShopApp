using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Microsoft.SqlServer.Server;
using MiniShopApp.Data.TelegramStore;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Items;
using MiniShopApp.Models.Orders;
using Telegram.Bot.Types;

namespace MiniShopApp.Pages.Orders
{
    public partial class OrderIndex
    {
        [Inject] NavigationManager navigation { get; set; } = default!;
        private readonly IProductService productService;
        [Inject]
        protected ProtectedLocalStorage localStorage { get; set; } = default!;
        //[Inject]
        //protected ProtectedSessionStorage sessionStorage { get; set; } = default!;
        //[Inject] IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        protected UserState userState { get; set; } = default!;
        public OrderIndex(IProductService productService)
        {
            this.productService = productService;
           
        }
        protected List<ViewProductOrders> _products = [];
        protected List<TbOrderDetails> orderDetails = [];
        protected OrderCreateModel order = new OrderCreateModel();
        private string? _filter = null;
        [Parameter] public long? userId { get; set; } = null;
        string? customerId = null;
        protected bool IsLoading = false;

        protected override async Task OnInitializedAsync()
        {
            userState.UserId=userId;
            customerId =userId?.ToString();
            
            await FilterProducts();
            //GetCustomerData();

            await base.OnInitializedAsync();
        }
        
        protected async void OnGetSearchRefresh()
        {
            await OnInitializedAsync();
        }
        protected async Task FilterProducts(string? filter = null)
        {
            IsLoading = true;
            try
            {
                
                _filter = filter;
                var products = await productService.GetOrderAllAsync(_filter);
                if(products.IsSuccess) {
                    _products = products.Data!.OrderByDescending(x=>x.CategoryName).ToList();
                    IsLoading = false;
                }
                else
                {
                    SnackbarService.Add("Error fetching products: " + products.Errors.ErrMessage, MudBlazor.Severity.Error);
                    
                    Console.WriteLine($"Error fetching products: {products.Errors}");
                    IsLoading = false;
                }
                
            }
            catch (Exception ex)
            {
                IsLoading = false;
                SnackbarService.Add("Error fetching products: " + ex.Message, MudBlazor.Severity.Error);

                throw new Exception($"Error filtering products: {ex.Message}");
            }
            IsLoading = false;
            StateHasChanged();
        }
        protected async Task OnSearch(ChangeEventArgs e)
        {
            try
            {
                _filter = e.Value?.ToString(); 
                await FilterProducts(_filter);
                StateHasChanged();
                // Simulate async operation
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during search: {ex.Message}");
            }
        }
        //protected async void GetCustomerData()
        //{
        //    try
        //    {
        //        var result = await sessionStorage.GetAsync<string>("userId");
        //        if (result.Success && result.Value!=null)
        //        {
        //            customerId = result.Value;
        //            StateHasChanged();// Convert to readable data (already a string in this case)
        //        }
        //        else
        //        {
        //            customerId = string.Empty;
        //            SnackbarService.Add("Getting user not found! please refresh page or bot /start again.", MudBlazor.Severity.Error);
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //SnackbarService.Add("Error 1: "+ex.Message, MudBlazor.Severity.Error);
        //        throw new Exception($"Get local data: {ex.Message}");
                
        //    }
        //}
        protected void DescreasProduct(int productId)
        {
            try
            {
                var product = _products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    if (_products.Any(_products => _products.Id == product.Id))
                    {
                        var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
                        if(existingProduct.QTYIncrease<= 0)
                        {
                            existingProduct.QTYIncrease = 0; // Decrease the quantity of the product in the list

                        }
                        else
                        {
                            existingProduct.QTYIncrease -= 1; // Decrease the quantity of the product in the list

                        }
                        StateHasChanged();

                    }

                    if (orderDetails.Any(od => od.ItemId == product.Id))
                    {
                        // If the product already exists in the order, descrease the quantity
                        var existingOrderDetail = orderDetails.First(od => od.ItemId == product.Id);
                        if (existingOrderDetail.Quantity == 1)
                        {
                            orderDetails.Remove(existingOrderDetail);
                        }
                        else
                        {
                            existingOrderDetail.Quantity -= 1;
                            existingOrderDetail.TotalPrice = existingOrderDetail.Price * existingOrderDetail.Quantity;
                        }

                        
                        StateHasChanged();
                    }
                   

                }
                else
                {
                    Console.WriteLine("Product not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product: {ex.Message}");
            }
        }
        protected void AddProduct(int productId)
        {
            try
            {
                var product = _products.FirstOrDefault(p => p.Id == productId);
                if (_products.Any(_products => _products.Id == product!.Id))
                {
                    var existingProduct = _products.FirstOrDefault(p => p.Id == product!.Id);
               
                    existingProduct!.QTYIncrease += 1; // Increase the quantity of the product in the list
                    StateHasChanged();

                }
                if (product != null)
                {
                    

                    if (orderDetails.Any(od => od.ItemId == product.Id))
                    {
                        // If the product already exists in the order, increase the quantity
                        var existingOrderDetail = orderDetails.First(od => od.ItemId == product.Id);
    
                        existingOrderDetail.Quantity += 1;
                        existingOrderDetail.TotalPrice = existingOrderDetail.Price * existingOrderDetail.Quantity;
                        StateHasChanged();
                    }
                    else
                    {
                        // If the product does not exist in the order, add it
                        orderDetails.Add(new TbOrderDetails
                        {
                            ItemId = product.Id,
                            ItemName = product.ProductName,
                            Price = product.Price,
                            Quantity = 1, // Default quantity to 1, can be adjusted later
                            TotalPrice = product.Price // Initial total price based on quantity of 1
                        });
                    }
                    
                }
                else
                {
                    Console.WriteLine("Product not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product: {ex.Message}");
            }
        }
        protected void GetViewOrder()
        {

        }
        protected async Task PlaceOrderAsync()
        {
            

            IsLoading = true;

            try
            {
                if(userId == null)
                {
                    customerId = userState.UserId.ToString();
                }
                
                if ( orderDetails.Count <= 0)
                {
                    IsLoading = false;
                    SnackbarService.Add("Please add products to the order before placing it.", MudBlazor.Severity.Warning);
                   
                    return;
                }
                //GetCustomerData();

                //customerId = userState.UserId.ToString();

                if (!string.IsNullOrEmpty(customerId))
                {
                    Console.WriteLine($"\n\n Customer ID: {customerId} \n\n");
                    order.CustomerId = long.Parse(customerId);
                    order.ItemCount = orderDetails.Count;
                    order.SubPrice = orderDetails.Sum(od => od.TotalPrice);
                    order.DiscountPrice = 0;
                    order.TotalPrice = order.SubPrice - order.DiscountPrice;
                    
                    order.CreatedDT = DateTime.Now;
                    order.TbOrderDetails = orderDetails;
                    // Save order to local storage
                    await localStorage.SetAsync("orderToCreate", order);
                    //OrderCreatePage orderCreatePage = new OrderCreatePage(order);
                    IsLoading= false;
                    navigation.NavigateTo($"/orders/create/{userId}");
                }
                else
                {
                    SnackbarService.Add("User invalid! please refresh page or bot by use command /start again.", MudBlazor.Severity.Error);
                    
                    IsLoading = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;// Handle the error, e.g., log it or show a notification
                Console.WriteLine($"Error creating order: {ex.Message}");
                SnackbarService.Add("Error creating order: " + ex.Message, MudBlazor.Severity.Error);
                
            }
        }
    }
}