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
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
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
                        Snackbar.Add("Update failed", Severity.Error);
                        throw new Exception("Update failed.");
                    }
                    Snackbar.Add("Category updated successfully", Severity.Success);
                    Submit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating", ex);
            }

        }
    }
}