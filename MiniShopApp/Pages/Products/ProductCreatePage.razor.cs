using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MiniShopApp.Data;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Items;

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
        string? alert = null;

        string? userId = null;
        protected Product model = new Product();
        protected override async Task OnInitializedAsync()
        {
            //if(userState.UserId == null)
            //{
            //    message = "System cannot getting your informations. Please use /start command to refresh bot.";
            //    return;
            //}
            //userId =userState.UserId?.ToString();

            //userId =userState.OnUserIdChanged.ToString();
            //await CreateProduct();
            await base.OnInitializedAsync();
        }

        protected async Task CreateProduct()
        {
            try
            {
                alert = null;
         
                var result = await productService.CreateAsync(model);
                if (string.IsNullOrEmpty(result))
                {
                    throw new Exception("Product creation failed.");
                }
                alert = result+ ": " + model.ProductName;
                model = new Product(); // Reset the model after successful creation
                // Handle success, e.g., show a message or redirect
            }
            catch (Exception ex)
            {
               throw new Exception("Error creating product", ex);
            }
        }
    }
}