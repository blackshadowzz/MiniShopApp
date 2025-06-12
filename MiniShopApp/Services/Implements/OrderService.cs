using Helpers.Responses;
using Microsoft.EntityFrameworkCore;
using MiniShopApp.Data;
using MiniShopApp.Models;
using MiniShopApp.Models.Orders;
using MiniShopApp.Services.Interfaces;
using Telegram.Bot;

namespace MiniShopApp.Services.Implements
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> logger;
        private readonly IDbContextFactory<AppDbContext> contextFactory;
        private readonly ITelegramBotClient botClient;

        public OrderService(ILogger<OrderService> logger, 
            IDbContextFactory<AppDbContext> contextFactory,
            ITelegramBotClient botClient)
        {
            this.logger = logger;
            this.contextFactory = contextFactory;
            this.botClient = botClient;
        }
        public Task<Result<string>> CreateAsync(long customerId, TbOrder model)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<TbOrderDetails>>> GetOrderDetailsAsync(long customerId, TbOrderDetails model)
        {
            throw new NotImplementedException();
        }

        public Task<Result<TbOrder>> GetOrderSummaryAsync(long customerId, TbOrder model)
        {
            throw new NotImplementedException();
        }

        public Task<Result<UserCustomer>> GetUserAsync(long? filter)
        {
            throw new NotImplementedException();
        }
    }
}
