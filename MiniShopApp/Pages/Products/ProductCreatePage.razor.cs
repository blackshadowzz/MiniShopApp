using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MiniShopApp.Data;
using MiniShopApp.Models.Items;
using MiniShopApp.Services.Interfaces;

namespace MiniShopApp.Pages.Products
{
    public partial class ProductCreatePage
    {
        private readonly IProductService productService;



        public ProductCreatePage(IProductService productService)
        {
            this.productService = productService;

            // Constructor logic can be added here if needed
        }
        string? message = null;

        string? userId = null;
        protected override async Task OnInitializedAsync()
        {
            if(userState.UserId == null)
            {
                message = "System cannot getting your informations. Please use /start command to refresh bot.";
                return;
            }
            userId =userState.UserId?.ToString();
            //userId =userState.OnUserIdChanged.ToString();
            //await CreateProduct();
            await base.OnInitializedAsync();
        }

        protected async Task CreateProduct()
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    await OnInitializedAsync();
                }
                var model = new Product
                {
                    ProductName = "Sample Product",
                    ProductCode = "SP001",
                    Price = 19.99,
                    Decription = "This is a sample product."
                };
                var result = await productService.CreateAsync(long.Parse(userId!), model);
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