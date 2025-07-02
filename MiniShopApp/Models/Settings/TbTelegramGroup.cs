namespace MiniShopApp.Models.Settings
{
    public class TbTelegramGroup
    {
        public int Id { get; set; }
        public long? GroupId { get; set; }
        public string? GroupName { get; set; }
        public string? BotToken { get; set; }
        public long? TelegramUserId { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
