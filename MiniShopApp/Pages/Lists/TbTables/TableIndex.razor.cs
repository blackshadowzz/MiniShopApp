using Microsoft.JSInterop;
using Radzen;
using System.Runtime;
using System.Text.Json;

namespace MiniShopApp.Pages.Lists.TbTables
{
    public partial class TableIndex
    {
        
        void ShowNotification(NotificationMessage message)
        {
            NotificationService.Notify(message);
        }
        public async Task Createtable()
        {
            await DialogService.OpenAsync<CreateTable>("Create Table");
        }

    }
}