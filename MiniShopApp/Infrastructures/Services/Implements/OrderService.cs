using Helpers.Responses;
using Microsoft.EntityFrameworkCore;
using MiniShopApp.Data;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models;
using MiniShopApp.Models.Orders;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
            await using var context = await contextFactory.CreateDbContextAsync();
            try
            {
                context.Database.BeginTransaction(); // Start transaction

                logger.LogInformation("Creating order for customer {CustomerId}", customerId);
                if(customerId <= 0)
                {
                    return await Result.FailureAsync<string>(new ErrorResponse("Invalid customer ID."));
                }
                context.TbOrders.Add(model);

                await context.SaveChangesAsync();

                string detailsText = "";
                    detailsText = string.Join("\n", model.TbOrderDetails!.Select(d =>
                        $"- {d.ItemName} {d.Quantity} x {d.Price?.ToString("c2")} =\t{d.TotalPrice?.ToString("c2")}"
                    ));
              
                await _botClient.SendMessage(
                        chatId: customerId, // Replace with your chat ID

                        text: $"\nOrder details:\n{detailsText}" +
                        $"\n\nOrder summary:" +
                        $"\nTable:\t {model.TableNumber}" +
                        $"\nItem count:\t {model.ItemCount}" +
                        $"\nTotal price:\t {model.TotalPrice?.ToString("c2")}" +
                        $"\nNotes:\t {model.Notes}" +
                        $"\n" +
                        
                        $"\nThank you for ordering! \nplease enjoy your foods." ,
                        parseMode: ParseMode.Html,
                         replyMarkup: new InlineKeyboardButton[]
                            {
                            InlineKeyboardButton.WithWebApp("Open App","https://minishopapp.runasp.net/index"),

                            }
                    // You can specify entities if needed

                    );
                context.Database.CommitTransaction(); // Ensure transaction is committed

                return await Result.SuccessAsync<string>("Order created successfully.");
            }
            catch(Exception ex)
            {
                context.Database.RollbackTransaction(); // Rollback transaction on error
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
