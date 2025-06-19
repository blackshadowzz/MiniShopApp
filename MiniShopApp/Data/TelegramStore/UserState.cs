using Microsoft.EntityFrameworkCore;
using MiniShopApp.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MiniShopApp.Data.TelegramStore
{
    public class UserState
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<UserState> logger;

        public UserState(IDbContextFactory<AppDbContext> dbContextFactory,ITelegramBotClient botClient,ILogger<UserState> logger)
        {
            _dbContextFactory = dbContextFactory;
            _botClient = botClient;
            this.logger = logger;
        }
        public long? UserId { get; set; }
        public void Initialize(long chatId)
        {
            UserId = chatId;
        }
        public async Task<UserCustomer?> GetUserCustomerAsync()
        {
            if (UserId == null)
                return null;

            await using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.TbUserCustomers.AsNoTracking().FirstOrDefaultAsync(u => u.CustomerId == UserId);
        }
        public async Task<long> GetBotIdAsync(long chatId)
        {
            try
            {
                //var data =await _botClient.get();
                var me = await _botClient.GetMe();
                return me.Id;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting bot ID");
                return 0;
            }
        }
    }
}
