using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Items;
using MiniShopApp.Pages.Lists.TbTables;
using MudBlazor;

namespace MiniShopApp.Pages.Lists.Categories
{
    public partial class CategoryIndex
    {
        private readonly ICategoryListService _context;
        public CategoryIndex(ICategoryListService categoryListService)
        {
            _context = categoryListService;
        }
        protected override async Task OnInitializedAsync()
        {
            var getcatlist = await _context.GetAllAsync(_filter);
            model = getcatlist.ToList();
            await base.OnInitializedAsync();
        }
        private string? _filter = string.Empty;
        private List<Category> model = new List<Category>();
        private string searchString = "";
        private readonly DialogOptions _backdropClick = new()
        {
            BackdropClick = false,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            Position = DialogPosition.Center,
        };
        private bool FilterFunc1(Category element) => FilterFunc(element, searchString);

        private bool FilterFunc(Category element, string searchStrings)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.CategoryName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
        private async Task CreateNew()
        {
            var optionss = _backdropClick;
            var dialog = await Dialog.ShowAsync<CreateCategory>("Create new Category", optionss);
            var result = await dialog.Result;

            if (!result!.Canceled)
            {
                // Refresh the table list after successful creation
                var tables = await _context.GetAllAsync(_filter);
                model = tables.ToList();
                StateHasChanged();
            }
        }
        public async Task Update(int? id)
        {
            var optionss = _backdropClick;
            var dialog = await Dialog.ShowAsync<UpdateCategory>("Update category", new DialogParameters
            {
                ["iTemid"] = id
            }
            , optionss);
            var result = await dialog.Result;
            if (!result!.Canceled)
            {
                var table = await _context.GetAllAsync(_filter);
                model = table.ToList();
                StateHasChanged();
            }


        }
        public async Task remove(int? id)
        {
            //Dialog.
            var parameters = new DialogParameters { ["Message"] = $"Are you sure you want to delete?" };
            var options = new DialogOptions { CloseOnEscapeKey = true };

            var dialog = Dialog.Show<DeleteDialog>("Confirm Delete", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await DeleteItem(id);
            }

        }
        public async Task DeleteItem(int? id)
        {
            try
            {
                if (id.HasValue)
                {
                    var result = await _context.DeleteAsync(id.Value);
                    if (result)
                    {
                        var tables = await _context.GetAllAsync(_filter);
                        model = tables.ToList();
                        SnackbarService.Add("Category deleted successfully.", Severity.Success);
                        StateHasChanged();
                    }
                    else
                    {
                        SnackbarService.Add("Failed to delete the category.", Severity.Error);
                        throw new Exception("Failed to delete the category.");
                    }
                }
            }
            catch (Exception ex)
            {
                SnackbarService.Add("Error deleting category: " + ex.Message, Severity.Error);
                throw new Exception($"Error deleting category: {ex.Message}");
            }

        }
    }
}