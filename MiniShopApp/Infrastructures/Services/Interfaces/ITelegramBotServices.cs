using Helpers.Responses;
using MiniShopApp.Models.Settings;

namespace MiniShopApp.Infrastructures.Services.Interfaces
{
    public interface ITelegramBotServices
    {
        Task<Result<string>> SetBotTokenAsync(TbTelegramBotToken tbTelegram);
        Task<Result<string>> UpdateBotTokenAsync(TbTelegramBotToken tbTelegram);
        Task<Result<string>> UpdateGroupAsync(TbTelegramGroup tbTelegramGroup);
        Task<Result<string>> DeleteGroupAsync(int id);
        Task<Result<IEnumerable<TbTelegramBotToken>>> GetTokenAsync(string? filter="");
        Task<Result<IEnumerable<TbTelegramGroup>>> GetGroupAsync(string? filter="");
    }
}
