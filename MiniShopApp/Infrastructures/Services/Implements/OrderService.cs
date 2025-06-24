using Helpers.Responses;
using Microsoft.EntityFrameworkCore;
using MiniShopApp.Data;
using MiniShopApp.Data.TelegramStore;
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
        private readonly UserState userState;

        public OrderService(ILogger<OrderService> logger, 
            IDbContextFactory<AppDbContext> contextFactory,
            AppDbContext context,
            ITelegramBotClient botClient,
            UserState userState)
        {
            this.logger = logger;
            this.contextFactory = contextFactory;
            this.context = context;
            _botClient = botClient;
            this.userState = userState;
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

                var row= await context.SaveChangesAsync();
                if (row > 0)
                {
                    
                    string detailsText = "";
                    detailsText = string.Join("\n", model.TbOrderDetails!.Select(d =>
                        $"- {d.ItemName} {d.Quantity} x {d.Price?.ToString("c2")} =\t{d.TotalPrice?.ToString("c2")}"
                    ));
                    userState.UserId = customerId; // Set the user ID in the state
                    await _botClient.SendMessage(
                            chatId: customerId, // Replace with your chat ID

                            text: $"Your ordering created successful!\n" +
                            $"Here details and summary of your ordered\n" +
                            $"\nOrder details:\n{detailsText}" +
                            $"\n\nOrder summary:\n" +
                            $"\nTable No:\t {model.TableNumber}\n" +
                            $"\nItem count:\t {model.ItemCount}\n" +
                            $"\nTotal price:\t {model.TotalPrice?.ToString("c2")}\n" +
                            $"\nNotes:\t {model.Notes}" +
                            $"\n" +

                            $"\nThank you for ordering! please enjoy.",
                            parseMode: ParseMode.Html,
                             replyMarkup: new InlineKeyboardButton[]
                                {
                            InlineKeyboardButton.WithWebApp("Open App",$"https://minishopapp.runasp.net/index?userid={userState.UserId}"),

                                }
                        // You can specify entities if needed

                        );
                    var user = await context.TbUserCustomers
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.CustomerId == customerId);
                    var groups= await context.TbTelegramGroups
                        .Where(x=>x.IsActive==true)
                        .AsNoTracking()
                        .ToListAsync();
                    if (groups.Count > 0)
                    {
                        groups.ForEach(x =>
                        {
                            long groupId = -1002895453976;
                            _botClient.SendMessage(
                                    chatId: x.GroupId??groupId, // Replace with your chat ID

                                    text: $"({user?.FirstName}) created order successful!\n" +
                                    $"Here details and summary of ordered\n" +
                                    $"\nOrder details:\n{detailsText}" +
                                    $"\n\nOrder summary:\n" +
                                    $"\nTable No:\t {model.TableNumber}\n" +
                                    $"\nItem count:\t {model.ItemCount} \n" +
                                    $"\nTotal price:\t {model.TotalPrice?.ToString("c2")} \n" +
                                    $"\nNotes:\t {model.Notes}" +
                                    $"\n"
                                // You can specify entities if needed

                                );
                        });

                        
                    }
                    
                }
                
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

        public Task<Result<TbOrder>> GetOrderByUserAsync(long? customerId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<TbOrderDetails>>> GetOrderDetailsAsync(long customerId, TbOrderDetails model)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<TbOrderDetails>>> GetOrdersByUserAsync(long customerId)
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
