using Radzen;

namespace MiniShopApp.Pages.Lists.TbTables
{
    public partial class TableIndex
    {
        void ShowNotification(NotificationMessage message)
        {
            NotificationService.Notify(message);
        }

    }
}