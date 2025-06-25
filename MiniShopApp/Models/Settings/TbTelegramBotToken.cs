using System.ComponentModel.DataAnnotations;

namespace MiniShopApp.Models.Settings
{
    public sealed class TbTelegramBotToken :BaseHelpEntity
    {
        //use to store back end Token and Web app url
        public int Id { get; set; }
        public long? UserId { get; set; }
        public string? UserType { get; set; }
        public string? BotToken { get; set; } = string.Empty;
        public string? BotTokenSecret { get; set;} = string.Empty;
        public string? WebAppUrl { get; set;} = string.Empty;
    }
    public class TbTelegramBotTokenDto
    {
        [Required]
        public string? BotToken { get; set; } = string.Empty;
        [Required]

        public string? WebAppUrl { get; set; } = string.Empty;
    }
}
