using Microsoft.AspNetCore.Components;
using MiniShopApp.Models.Items;
using MudBlazor;

namespace MiniShopApp.Pages.Lists.Categories
{
    public partial class CreateCategory
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }
        private void Submit() => MudDialog.Close(DialogResult.Ok(true));
        private void Cancel() => MudDialog.Cancel();
        private Category model = new Category();
        private async Task SaveData()
        {

        }
    }
}