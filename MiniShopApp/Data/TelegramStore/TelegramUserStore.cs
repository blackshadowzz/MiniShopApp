using System.Collections.Concurrent;

namespace MiniShopApp.Data.TelegramStore
{
    public class TelegramUserStore
    {
            public long Id { get; set; }
            public bool IsBot { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Username { get; set; }
            public string? LanguageCode { get; set; }
            public bool IsPremium { get; set; }

    }
}
