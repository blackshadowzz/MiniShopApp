using Microsoft.AspNetCore.Components;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Items;
using MudBlazor;

namespace MiniShopApp.Pages.Lists.Categories
{
    public partial class UpdateCategory
    {
        private readonly ICategoryListService _categoryService;
        public UpdateCategory(ICategoryListService categoryService)
        {
            _categoryService = categoryService;
        }
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }
        private void Submit() => MudDialog.Close(DialogResult.Ok(true));
        private void Cancel() => MudDialog.Cancel();
        private Category model = new Category();
        [Parameter] public int? iTemid { get; set; }
        protected async override Task OnInitializedAsync()
        {
            await Getupdate(iTemid);
            await base.OnInitializedAsync();
        }
        private async Task Getupdate(int? id)
        {
            var ds = await _categoryService.GetOneTable(id);
            model = ds;
        }
        private async Task SaveUpdate()
        {
            try
            {
                if (model != null)
                {
                    var result = await _categoryService.UpdateAsync(model, iTemid);
                    if (result != true)
                    {
                        SnackbarService.Add("Update failed", Severity.Error);
                        throw new Exception("Update failed.");
                    }
                    SnackbarService.Add("Category updated successfully", Severity.Success);
                    Submit();
                }
            }
            catch (Exception ex)
            {
                SnackbarService.Add("Error updating", Severity.Error);
                throw new Exception("Error updating", ex);
            }

        }
    }
}