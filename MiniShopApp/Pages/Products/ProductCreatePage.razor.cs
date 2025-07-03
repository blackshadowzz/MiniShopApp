using DocumentFormat.OpenXml.Office2021.Excel.RichDataWebImage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.IdentityModel.Tokens;
using MiniShopApp.Data;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Items;
using MudBlazor;

namespace MiniShopApp.Pages.Products
{
    public partial class ProductCreatePage
    {
        private readonly IProductService productService;
        private readonly ICategoryListService categoryListService;



        public ProductCreatePage(IProductService productService,ICategoryListService categoryListService)
        {
            this.productService = productService;
            this.categoryListService = categoryListService;

            // Constructor logic can be added here if needed
        }
        string? message = null;
        string? alert = null;

        string? userId = null;
        protected Product model = new Product();
        protected List<Category> categories = new List<Category>();
        [Inject] ISnackbar Snackbar { get; set; } = default!;
        MudForm form;
        
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
            await GetCategory();

            await base.OnInitializedAsync();
        }
        private async Task GetCategory()
        {
            var result = await categoryListService.GetAllAsync();
            categories = result.ToList();
        }
        protected async Task CreateProduct()
        {
            try
            {
                alert = null;
                var insertimg = await HandleValidSubmit();
                if(insertimg!=false&&model.ImageUrl!=null)
                {
                    var result = await productService.CreateAsync(model);
                    if (string.IsNullOrEmpty(result))
                    {
                        throw new Exception("Product creation failed.");
                    }
                    alert = result + ": " + model.ProductName;
                    model = new Product(); // Reset the model after successful creation
                                           // Handle success, e.g., show a message or redirect
                    return;
                }
                Snackbar.Add("Please upload an image", Severity.Error);

            }
            catch (Exception ex)
            {
               throw new Exception("Error creating product", ex);
            }
        }
    }
}