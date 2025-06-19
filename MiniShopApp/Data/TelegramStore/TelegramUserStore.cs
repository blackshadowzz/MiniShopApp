using System.Collections.Concurrent;

namespace MiniShopApp.Data.TelegramStore
{
    public class TelegramUserStore
    {
        // Key: Telegram ChatId, Value: (optional) App UserId or other info
        public static ConcurrentDictionary<long, long> TelegramUsers { get; } = new();
    }
}
