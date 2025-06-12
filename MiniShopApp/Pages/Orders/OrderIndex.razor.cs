using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MiniShopApp.Models.Items;
using MiniShopApp.Services.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot.Types;
namespace MiniShopApp.Pages.Orders
{
    public partial class OrderIndex
    {
        private readonly IProductService productService;
        [Inject]
        protected ProtectedLocalStorage localStorage { get; set; } = default!;
        
        public OrderIndex(IProductService productService)
        {
            this.productService = productService;
           
        }
        protected IEnumerable<Product> _products = [];
        private string? _filter = null;
        string? customerId = null;
        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (firstRender)
        //    {
                
        //    }
        //    await base.OnAfterRenderAsync(firstRender);
        //}
        protected override async Task OnInitializedAsync()
        {
            

            await FilterProducts();
            await base.OnInitializedAsync();
        }
        protected async Task FilterProducts(string? filter = null)
        {
            try
            {
                
                _filter = filter;
                var products = await productService.GetAllAsync(_filter);
                _products = products.ToList();
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
                var result = await localStorage.GetAsync<string?>("customerId");
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
        protected async Task CreateOrderAsync(Product product)
        {
            try
            {
                if (product != null)
                {
                    long? userId = (await localStorage.GetAsync<long>("customerId")).Value;
                    if (userId.HasValue)
                    {
                        var result = await productService.CreateAsync(userId.Value, product);
                        Console.WriteLine($"Order created: {result}");
                    }
                    else
                    {
                        Console.WriteLine("User ID is not available.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order: {ex.Message}");
            }
        }
    }
}