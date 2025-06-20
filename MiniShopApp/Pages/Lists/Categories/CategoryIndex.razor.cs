using MiniShopApp.Models.Items;
using MudBlazor;

namespace MiniShopApp.Pages.Lists.Categories
{
    public partial class CategoryIndex
    {
        private List<Category> model = new List<Category>();
        private readonly DialogOptions _backdropClick = new()
        {
            BackdropClick = false,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
        };
        private async Task CreateNew()
        {
            var optionss = _backdropClick;
            await Dialog.ShowAsync<CreateCategory>("Create new category", optionss);
        }
    }
}