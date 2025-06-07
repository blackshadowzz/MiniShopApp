
using MiniShopApp.Models.Items;
using MiniShopApp.Services.Interfaces;

namespace MiniShopApp.Pages.Products
{
    public partial class ProductIndexPage
    {
        private readonly IProductService productService;

        public ProductIndexPage(IProductService productService)
        {
            this.productService = productService;
        }
        private List<Product> _products = new List<Product>();
        private string? _filter = null;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var products = await productService.GetAllAsync(_filter);
                _products = products.ToList();
            }
            catch (Exception ex)
            {
               throw new Exception($"Error initializing ProductIndexPage: {ex.Message}");
            }
            await base.OnInitializedAsync();
        }
    }
}