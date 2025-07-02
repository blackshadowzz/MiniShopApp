using Microsoft.AspNetCore.Components;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Customers;
using MiniShopApp.Models.Items;
using MiniShopApp.Models.Orders;

namespace MiniShopApp.Pages.Orders.WebOrders
{
    public partial class OrderPage(IOrderService orderService,
        IProductService productService,
        ICustomerTypeService customerTypeService,
        ITableListService tableListService
        )
    {

        protected List<ViewProductOrders> _products = [];
        protected List<ViewProductOrders> _productsStore = [];
        protected List<TbOrderDetails> orderDetails= [];
        protected List<ViewCustomerType> viewCustomerTypes = [];
        protected OrderCreateModel Order = new OrderCreateModel();
        private string? _filter = null;
        [Parameter] public long? userId { get; set; } = null;
        string? customerId = null;
        protected bool IsLoading = false;
        protected bool LoadingProducts = false;
        protected override async Task OnInitializedAsync()
        {
            LoadingProducts = true;
            await FilterProducts(_filter);
            _products = _productsStore;
            await GetCustomers();
            await GetTables();
            var type=viewCustomerTypes.FirstOrDefault();
            if(type is not null)
            {
                Order.CustomerId = type!.Id;

            }
            LoadingProducts = false;

            await base.OnInitializedAsync();
        }
        protected async Task GetTables()
        {
            try
            {
                var results = await tableListService.GetAllAsync();
                if (results.Any())
                {
                    tables=results.ToList();
                }
            }
            catch (Exception ex)
            {
                SnackbarService.Add("Error fetching products: " + ex.Message, MudBlazor.Severity.Error);

                throw new Exception($"Error filtering products: {ex.Message}");
            }
        }
        protected async Task GetCustomers()
        {
            try
            {
                var results = await customerTypeService.GetAllAsync();
                if (results.IsSuccess)
                {
                    viewCustomerTypes = results.Data!.ToList();
                }
                else
                {
                    SnackbarService.Add("Error fetching products: " + results.Errors.ErrMessage, MudBlazor.Severity.Error);

                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
                SnackbarService.Add("Error fetching products: " + ex.Message, MudBlazor.Severity.Error);

                throw new Exception($"Error filtering products: {ex.Message}");
            }
        }
        protected async Task FilterProducts(string? filter = null)
        {

            try
            {
                _filter = filter;
                var products = await productService.GetOrderAllAsync(_filter);
                if (products.IsSuccess)
                {
                    _productsStore = products.Data!.OrderByDescending(x => x.CategoryName).ToList();

                }
                else
                {
                    SnackbarService.Add("Error fetching products: " + products.Errors.ErrMessage, MudBlazor.Severity.Error);

                    Console.WriteLine($"Error fetching products: {products.Errors}");
                }
            }
            catch (Exception ex)
            {
                SnackbarService.Add("Error fetching products: " + ex.Message, MudBlazor.Severity.Error);

                throw new Exception($"Error filtering products: {ex.Message}");
            }
            finally
            {
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
                        if (existingProduct.QTYIncrease <= 0)
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
        protected async Task OnSearch(ChangeEventArgs e)
        {
            try
            {
                _filter = e.Value?.ToString();
                var products = await productService.GetOrderAllAsync(_filter);
                if (products.IsSuccess)
                {
                    _products = products.Data!.OrderByDescending(x => x.CategoryName).ToList();
                }
                StateHasChanged();
                // Simulate async operation
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during search: {ex.Message}");
            }
        }
        protected async void OnGetSearchRefresh()
        {
            _products = _productsStore;
            StateHasChanged();
            IsLoading = false;
        }
    }
}