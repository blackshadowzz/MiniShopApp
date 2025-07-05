using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MiniShopApp.Infrastructures.Services.Implements;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Items;
using MudBlazor;
using System.Runtime.InteropServices;

namespace MiniShopApp.Pages.Products
{
    public partial class ProductUpdatePage
    {
        private readonly IProductService productService;
        private readonly ICategoryListService categoryListService;
        public ProductUpdatePage(IProductService productService, ICategoryListService categoryListService)
        {
            this.productService = productService;
            this.categoryListService = categoryListService;
        }
        [Parameter] public int Id { get; set; }
        protected Product model = new Product();
        protected List<Category> categories = new List<Category>();
        MudForm form;
        protected override async Task OnInitializedAsync()
        {
           await LoadProductAsync();
            await GetUpdate(Id);
            //await HandleFileSelected(model.ImageUrl)
            await GetCategory();
            await base.OnInitializedAsync();
        }
        private async Task LoadProductAsync()
        {

            await GetUpdate(Id);
            //await HandleFileSelected(model.ImageUrl)
            await GetCategory();
            StateHasChanged(); // Ensure UI updates
        }

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (firstRender)
        //    {
        //        await GetUpdate(Id);
        //        //await HandleFileSelected(model.ImageUrl)
        //        await GetCategory();
        //        StateHasChanged();
        //    }
        //}

        private IBrowserFile selectedFile;
        private string imagePreviewUrl;
        bool isLoading = false;
        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {


            try
            {
                const int maxWidth = 1024;
                const int maxHeight = 768;
                await using var stream = e.File.OpenReadStream();
                using var image = await SixLabors.ImageSharp.Image.LoadAsync(stream);

                if (image.Width > maxWidth || image.Height > maxHeight)
                {
                    Console.WriteLine("Image resolution too large.");
                    // Show error or prevent saving
                    SnackbarService.Add("Image resolution exceeds maximum allowed size.", Severity.Error);
                    return;
                }
                isLoading = true;

                selectedFile = e.File;
                // Preview: Convert to Base64 data URL
                var buffer = new byte[selectedFile.Size];
                await selectedFile.OpenReadStream().ReadAsync(buffer);

                var ext = Path.GetExtension(selectedFile.Name).ToLowerInvariant();
                var mimeType = ext switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream"
                };

                imagePreviewUrl = $"data:{mimeType};base64,{Convert.ToBase64String(buffer)}";
                isLoading = false;
            }
            catch (Exception ex)
            {
                isLoading = false;
                SnackbarService.Add($"Error: {ex.Message}", Severity.Error);
                return;
            }
            finally
            {
                StateHasChanged();
                isLoading = false;
            }

        }
        private async Task<bool> HandleValidSubmit()
        {
            if (selectedFile is not null)
            {
                var uploadsFolder = Path.Combine("wwwroot", "images", "products");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(selectedFile.Name)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                await using var stream = File.Create(filePath);
                await selectedFile.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024).CopyToAsync(stream);

                // Set relative URL for DB
                model.ImageUrl = $"images/products/{fileName}";
                return true;
            }
            SnackbarService.Add("Please select a file.", Severity.Error);
            return false;

            // Insert newProduct into the database here (e.g., via EF Core)
        }
        private async Task GetCategory()
        {
            var result = await categoryListService.GetAllAsync();
            categories = result.ToList();
        }
        private async Task GetUpdate(int id)
        {
            try
            {
                if(id != null)
                {
                    var result = await productService.GetProductById(id);
                    model = result;
                    return;
                }
                SnackbarService.Add("No id include.", Severity.Warning);
            }
            catch (Exception ex)
            {
                SnackbarService.Add($"Error fetching product: {ex.Message}", Severity.Error);
                throw new Exception($"Error fetching product with ID {id}: {ex.Message}");
            }
        }
        private async Task SaveUpdate()
        {
            try
            {
                var checkediSeq = await productService.GetProductById(model.Id);
                if (checkediSeq.EditSeq==model.EditSeq)
                {
                    
                    if (imagePreviewUrl != null)
                    {
                        
                        if (await HandleValidSubmit())
                        {
                            model.EditSeq++;
                            var result1 = await productService.UpdateAsync(model);
                            SnackbarService.Add(result1, Severity.Success);
                            return;
                        }
                        else
                        {
                            SnackbarService.Add("File upload failed.", Severity.Error);
                            return;
                        }
                    }
                    else
                    {
                        model.EditSeq++;
                        var result = await productService.UpdateAsync(model);
                        SnackbarService.Add(result, Severity.Info);
                        return;
                    }


                }
                else
                {
                    SnackbarService.Add("try to refresh page before update.", Severity.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                SnackbarService.Add($"Error updating product: {ex.Message}", Severity.Error);
                throw new Exception($"Error updating product with ID {model.Id}: {ex.Message}");
            }
        }
    }
}