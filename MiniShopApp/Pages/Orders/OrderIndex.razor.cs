using MiniShopApp.Models.Items;
using MiniShopApp.Services.Interfaces;

namespace MiniShopApp.Pages.Orders
{
    public partial class OrderIndex
    {
        private readonly IProductService productService;

        public OrderIndex(IProductService productService)
        {
            this.productService = productService;
        }
        private List<Product> _products = new List<Product>();
        private string? _filter = null;

        protected override async Task OnInitializedAsync()
        {
            await FilterProducts();
            await base.OnInitializedAsync();
        }
        protected async Task FilterProducts(string? filter = "")
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
    }
}