namespace MiniShopApp.Models.Settings
{
    public sealed class TbTelegramBotToken :BaseHelpEntity
    {
        //use to store back end Token and fix userId
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? UserType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? BotToken { get; set; } = string.Empty;
        public string? BotTokenSecret { get; set;} = string.Empty;
    }
}
