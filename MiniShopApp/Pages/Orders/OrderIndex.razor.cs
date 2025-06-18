using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Items;
using MiniShopApp.Models.Orders;
using System.Threading.Tasks;
using Telegram.Bot.Types;
namespace MiniShopApp.Pages.Orders
{
    public partial class OrderIndex
    {
        [Inject] NavigationManager navigation { get; set; } = default!;
        [Inject] ProtectedSessionStorage sessionStorage { get; set; } = default!;
        private readonly IProductService productService;
        [Inject]
        protected ProtectedLocalStorage localStorage { get; set; } = default!;
        
        public OrderIndex(IProductService productService)
        {
            this.productService = productService;
           
        }
        protected List<ViewProductOrders> _products = [];
        protected List<TbOrderDetails> orderDetails = [];
        protected OrderCreateModel order = new OrderCreateModel();
        private string? _filter = null;
        string? customerId = null;

        protected override async Task OnInitializedAsync()
        {
            

            await FilterProducts();
            await base.OnInitializedAsync();
        }
        protected async Task OnGetSearchRefresh()
        {
            await OnInitializedAsync();
        }
        protected async Task FilterProducts(string? filter = null)
        {
            try
            {
                
                _filter = filter;
                var products = await productService.GetOrderAllAsync(_filter);
                if(products.IsSuccess) {
                    _products = products.Data!.OrderByDescending(x=>x.CategoryName).ToList();
                }
                else
                {
                    //NotificationService.Error("Error fetching products", products.ErrorMessage);
                    Console.WriteLine($"Error fetching products: {products.Errors}");
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Error filtering products: {ex.Message}");
            }
        }
        protected async Task OnSearch(ChangeEventArgs e)
        {
            try
            {
                _filter = e.Value?.ToString();
                _products= _products.Where(p =>
                p.ProductName?.Contains(_filter!, StringComparison.InvariantCultureIgnoreCase)==true).ToList();
                await Task.CompletedTask; // Simulate async operation
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during search: {ex.Message}");
            }
        }
        protected async Task GetCustomerData()
        {
            try
            {
                var result = await sessionStorage.GetAsync<string?>("customerId");
                if (result.Success && result.Value != null)
                {
                    customerId = result.Value; // Convert to readable data (already a string in this case)
                }
                else
                {
                    customerId = "No data found or key does not exist.";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Get local data: {ex.Message}");
            }
        }
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
                    //else
                    //{
                    //    // If the product does not exist in the order, add it
                    //    orderDetails.Add(new TbOrderDetails
                    //    {
                    //        ItemId = product.Id,
                    //        ItemName = product.ProductName,
                    //        Price = product.Price,
                    //        Quantity = 1, // Default quantity to 1, can be adjusted later
                    //        TotalPrice = product.Price // Initial total price based on quantity of 1
                    //    });
                    //}

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
                if (_products.Any(_products => _products.Id == product.Id))
                {
                    var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
               
                    existingProduct.QTYIncrease += 1; // Increase the quantity of the product in the list
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
            try
            {
                await GetCustomerData();
                if (!string.IsNullOrEmpty(customerId))
                {
                    Console.WriteLine($"\n\n Customer ID: {customerId} \n\n");
                    order.CustomerId = long.Parse(customerId);
                    order.ItemCount = orderDetails.Count;
                    order.SubPrice = orderDetails.Sum(od => od.TotalPrice);
                    order.DiscountPrice = 0; // Set discount price to 0 for now, can be adjusted later
                    order.TotalPrice = order.SubPrice - order.DiscountPrice;
                    //order.TableNumber = "Table 1"; // Set a default table number, can be adjusted later
                    //order.Notes = "bla bla"; // Set a default table number, can be adjusted later
                    order.CreatedDT = DateTime.Now;
                    order.TbOrderDetails = orderDetails;
                    // Save order to local storage
                    await localStorage.SetAsync("orderToCreate", order);
                    //OrderCreatePage orderCreatePage = new OrderCreatePage(order);
                    navigation.NavigateTo("/orders/create");
                }
                else
                {
                    NotificationService.Notify(Radzen.NotificationSeverity.Warning, "Empty User", "User ID is not available.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order: {ex.Message}");
            }
        }
    }
}