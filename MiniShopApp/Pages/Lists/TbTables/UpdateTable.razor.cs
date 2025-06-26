using Microsoft.AspNetCore.Components;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Items;
using MudBlazor;

namespace MiniShopApp.Pages.Lists.TbTables
{
    public partial class UpdateTable
    {
        [CascadingParameter] IMudDialogInstance Mudialog { get; set; }
        [Parameter] public int? iTemid { get; set; }
        private TbTable? model { get; set; } = new();
        private readonly ITableListService _context;
        private void Submit() => Mudialog.Close(DialogResult.Ok(true));
        private void Cancel() => Mudialog.Close();
        public UpdateTable(ITableListService tableListService)
        {
            _context = tableListService;
        }
        protected async override Task OnInitializedAsync()
        {
            var ds = await _context.GetOneTable(iTemid);
            model = ds;
            Console.WriteLine("id",iTemid);
            await base.OnInitializedAsync();
        }
        private async Task SaveUpdate()
        {
            if(model != null)
            {
                var data = new TbTable();
                data = model;
                var result = await _context.UpdateAsync(data, iTemid);
                if (result != false)
                {
                    Submit();
                }
                Cancel();
            }
        }
        
    }
}