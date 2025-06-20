using Microsoft.IdentityModel.Tokens;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Items;

namespace MiniShopApp.Pages.Lists.TbTables
{
    public partial class CreateTable
    {
        private readonly ITableListService tableService;
        protected TbTable model = new TbTable();
        public CreateTable(ITableListService tableService)
        {
            this.tableService = tableService;
        }
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
            }
            catch(Exception ex)
            {
                throw new Exception("Error creating", ex);
            }
        }
    }
}