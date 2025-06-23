namespace MiniShopApp.Models.Settings
{
    public class TbTelegramBotToken :BaseHelpEntity
    {
        //use to store back end Token and fix userId
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? BotToken { get; set; } = string.Empty;
        public string? BotTokenSecret { get; set;} = string.Empty;
    }
}
