using MiniShopApp.Data;
using MiniShopApp.Models.Items;
using MiniShopApp.Services.Interfaces;

namespace MiniShopApp.Components.Pages.Products
{
    public partial class ProductCreatePage
    {
        private readonly IProductService productService;
        private readonly UserState userState;

        public ProductCreatePage(IProductService productService, UserState userState)
        {
            this.productService = productService;
            this.userState = userState;
            // Constructor logic can be added here if needed
        }
        string? message = null;
        string? message2 = null;
        protected override async Task OnInitializedAsync()
        {
            //await CreateProduct();
            await base.OnInitializedAsync();
        }
        protected async Task CreateProduct()
        {
            try
            {
                message2="Creating product...";
                
                var model = new Product
                {
                    ProductName = "Sample Product",
                    ProductCode = "SP001",
                    Price = 19.99,
                    Decription = "This is a sample product."
                };
                var result = await productService.CreateAsync(userState.UserId, model);
                if (string.IsNullOrEmpty(result))
                {
                    throw new Exception("Product creation failed.");
                }
                string message = result;
                // Handle success, e.g., show a message or redirect
            }
            catch (Exception ex)
            {
               throw new Exception("Error creating product", ex);
            }
        }
    }
}