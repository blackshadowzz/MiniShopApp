using MiniShopApp.Infrastructures.Services.Interfaces;

namespace MiniShopApp.Pages.Lists.TbTables
{
    public partial class CreateTable
    {
        private readonly ITableListService tableService;
        public CreateTable(ITableListService tableService)
        {
            this.tableService = tableService;
        }
        public async Task SaveTable()
        {
            
        }
    }
}