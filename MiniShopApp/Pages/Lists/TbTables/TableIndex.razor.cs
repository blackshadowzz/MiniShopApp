using Microsoft.JSInterop;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Items;
using MudBlazor;
using System.Runtime;
using System.Text.Json;
using static MudBlazor.CategoryTypes;

namespace MiniShopApp.Pages.Lists.TbTables
{
    public partial class TableIndex
    {
        private readonly ITableListService _context;
        private readonly DialogOptions _backdropClick = new()
        {
            BackdropClick = false,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            Position = DialogPosition.Center,
        };
        public TableIndex(ITableListService tableListService)
        {
            _context = tableListService;
        }
        private List<TbTable> model = new List<TbTable>();
        private string searchString = "";

        private string? _filter = string.Empty;
        bool _expanded = true;

        private bool FilterFunc1(TbTable element) => FilterFunc(element, searchString);

        private bool FilterFunc(TbTable element, string searchStrings)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.TableNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
        private void OnExpandCollapseClick()
        {
            _expanded = !_expanded;
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await _context.GetAllAsync(_filter);
                model = result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error initializing List : {ex.Message}");
            }
            await base.OnInitializedAsync();
        }
        public async Task Createtable()
        {
            var optionss = _backdropClick;
            var dialog = await Dialog.ShowAsync<CreateTable>("Create new Table", optionss);
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
            var dialog = await Dialog.ShowAsync<UpdateTable>("Update table", new DialogParameters
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
                        SnackbarService.Add("Table deleted successfully.", Severity.Success);
                        StateHasChanged();
                    }
                    else
                    {
                        SnackbarService.Add("Failed to delete the table.", Severity.Error);
                        throw new Exception("Failed to delete the table.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting table: {ex.Message}");
            }

        }
    }
}