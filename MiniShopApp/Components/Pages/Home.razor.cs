using MiniShopApp.Models.Items;
using MiniShopApp.Services.Interfaces;

namespace MiniShopApp.Components.Pages
{
    public partial class Home
    {
        private readonly IProductService productService;

        public Home(IProductService productService)
        {
            this.productService = productService;
        }
        protected List<Product> products = new List<Product>();
        protected override async Task OnInitializedAsync()
        {
            try
            {
                products = (await productService.GetAllAsync()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching products: {ex.Message}");
            }
            await base.OnInitializedAsync();
        }
    }
}