using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Items;
using MudBlazor;

namespace MiniShopApp.Pages.Lists.TbTables
{
    public partial class CreateTable
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }
        private readonly ITableListService tableService;
        protected TbTable model = new TbTable();
        public CreateTable(ITableListService tableService)
        {
            this.tableService = tableService;
        }
        

        private void Submit() => MudDialog.Close(DialogResult.Ok(true));

        private void Cancel() => MudDialog.Cancel();
        public async Task SaveTable()
        {
            try
            {
                var result = await tableService.CreateAsync(model);
                if (string.IsNullOrEmpty(result))
                {
                    throw new Exception("creation failed.");
                }
                model = new TbTable();
                Submit();
            }
            catch(Exception ex)
            {
                throw new Exception("Error creating", ex);
            }
        }
    }
}