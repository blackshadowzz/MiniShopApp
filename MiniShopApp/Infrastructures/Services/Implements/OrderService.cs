using Helpers.Responses;
using Microsoft.EntityFrameworkCore;
using MiniShopApp.Data;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models;
using MiniShopApp.Models.Orders;
using Telegram.Bot;

namespace MiniShopApp.Infrastructures.Services.Implements
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> logger;
        private readonly IDbContextFactory<AppDbContext> contextFactory;
        private readonly AppDbContext context;
        private readonly ITelegramBotClient _botClient;

        public OrderService(ILogger<OrderService> logger, 
            IDbContextFactory<AppDbContext> contextFactory,
            AppDbContext context,
            ITelegramBotClient botClient)
        {
            this.logger = logger;
            this.contextFactory = contextFactory;
            this.context = context;
            _botClient = botClient;
        }
        public async Task<Result<string>> CreateAsync(long customerId, TbOrder model)
        {
            try
            {
                logger.LogInformation("Creating order for customer {CustomerId}", customerId);
                await using var context = await contextFactory.CreateDbContextAsync();
                if(customerId <= 0)
                {
                    return await Result.FailureAsync<string>(new ErrorResponse("Invalid customer ID."));
                }
                context.TbOrders.Add(model);
                //context.TbOrderDetails.AddRange(model.TbOrderDetails);
                var affectedRows = await context.SaveChangesAsync();
                await _botClient.SendMessage(
                        chatId: customerId, // Replace with your chat ID

                        text:
                        $"Product Count: {model.ItemCount}" 
                       
                    );
                return affectedRows > 0 
                    ? await Result.SuccessAsync("Ordering was created successful!") 
                    : await Result.FailureAsync<string>(new ErrorResponse("Failed to create order."));
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error creating order for customer {CustomerId}", customerId);
                return await Result.FailureAsync<string>(new ErrorResponse(ex.Message));
            }
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
