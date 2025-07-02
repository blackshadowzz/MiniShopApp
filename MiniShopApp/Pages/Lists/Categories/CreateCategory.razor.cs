using Microsoft.AspNetCore.Components;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Items;
using MudBlazor;

namespace MiniShopApp.Pages.Lists.Categories
{
    public partial class CreateCategory
    {
        private readonly ICategoryListService _categoryService;
        public CreateCategory(ICategoryListService categoryService)
        {
            _categoryService = categoryService;
        }
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }
        private void Submit() => MudDialog.Close(DialogResult.Ok(true));
        private void Cancel() => MudDialog.Cancel();
        private Category model = new Category();
        private async Task SaveData()
        {
            try
            {
                var result = await _categoryService.CreateAsync(model);
                if (result.IsSuccess!=true)
                {
                    Snackbar.Add("Creation failed", Severity.Error);
                    throw new Exception("creation failed.");
                }
                model = new Category();
                Snackbar.Add("Category created successfully", Severity.Success);
                Submit();
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating", ex);
            }
        }
    }
}