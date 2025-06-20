using Microsoft.JSInterop;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Items;
using MudBlazor;
using System.Runtime;
using System.Text.Json;

namespace MiniShopApp.Pages.Lists.TbTables
{
    public partial class TableIndex
    {
        private readonly ITableListService _context;
        private readonly DialogOptions _backdropClick = new() 
        { 
            BackdropClick = false,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
        };
        public TableIndex(ITableListService tableListService)
        {
            _context = tableListService;
        }
        
        //void ShowNotification(NotificationMessage message)
        //{
        //    NotificationService.Notify(message);
        //}
        private List<TbTable> model = new List<TbTable>();
        private string? _filter = string.Empty;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await _context.GetAllAsync(_filter);
                model = result.ToList();
            }
            catch(Exception ex)
            {
                throw new Exception($"Error initializing List : {ex.Message}");
            }
            await base.OnInitializedAsync();
        }
        public async Task Createtable()
        {
            var optionss = _backdropClick;
            await Dialog.ShowAsync<CreateTable>("Create new Table", optionss);
        }

    }
}