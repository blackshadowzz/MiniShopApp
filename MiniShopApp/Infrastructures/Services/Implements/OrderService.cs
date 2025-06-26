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
        protected async Task<string> GetWebURLAsync()
        {
            await using var context = await contextFactory.CreateDbContextAsync();
            var reult = await context.TbTelegramBotTokens.FirstOrDefaultAsync();
            if (reult != null)
            {
                return reult.WebAppUrl!;
            }
            return "";
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
                    userState.UserId = customerId;
                    string url = await GetWebURLAsync();
                    var webURL = url + $"/index?userid={userState.UserId}";// Set the user ID in the state
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
                            InlineKeyboardButton.WithWebApp("Open App",webURL),

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

        public async Task<Result<IEnumerable<ViewTbOrders>>> GetOrderSummaryAsync(string? filter = "")
        {
            try
            {
                await using var context = await contextFactory.CreateDbContextAsync();
                var query = context.TbOrders
                    .Include(o => o.TbOrderDetails)
                    .AsNoTracking()
                    .Join(context.TbUserCustomers,
                          o => o.CustomerId,
                          u => u.CustomerId,
                          (o, u) => new { Order = o, User = u });

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    if (long.TryParse(filter, out var customerId))
                    {
                        query = query.Where(x => x.Order.CustomerId == customerId);
                    }
                    else
                    {
                        query = query.Where(x =>
                            (x.User.FirstName + " " + x.User.LastName).Contains(filter));
                    }
                }

                var orders = await query
                    .Select(x => new ViewTbOrders
                    {
                        Id = x.Order.Id,
                        CustomerId = x.Order.CustomerId,
                        TableNumber = x.Order.TableNumber,
                        ItemCount = x.Order.ItemCount,
                        SubPrice = x.Order.SubPrice,
                        DiscountPrice = x.Order.DiscountPrice,
                        TotalPrice = x.Order.TotalPrice,
                        Notes = x.Order.Notes,
                        CreatedDT=x.Order.CreatedDT,
                        ModifiedDT=x.Order.ModifiedDT,
                        IsActive=x.Order.IsActive,
                        TbOrderDetails = x.Order.TbOrderDetails != null
                            ? x.Order.TbOrderDetails.Select(d => new ViewTbOrderDetails
                            {
                                Id = d.Id,
                                OrderId = d.OrderId,
                                ItemId = d.ItemId,
                                ItemName = d.ItemName,
                                Price = d.Price,
                                Quantity = d.Quantity,
                                TotalPrice = d.TotalPrice
                            }).ToList()
                            : new List<ViewTbOrderDetails>()
                    })
                    .ToListAsync();

                return await Result.SuccessAsync<IEnumerable<ViewTbOrders>>(orders);
            }
            catch (Exception ex)
            {
                logger.LogError("Error in GetOrderSummaryAsync: " + ex.Message, ex);
                return Result.Failure<IEnumerable<ViewTbOrders>>(ErrorResponse.ServerError(ex.Message));
            }
        }

        public Task<Result<IEnumerable<TbOrderDetails>>> GetOrderDetailsAsync(long? orderId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<TbOrderDetails>>> GetOrdersByUserAsync(long? customerId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<UserCustomer>> GetUserAsync(long? filter)
        {
            throw new NotImplementedException();
        }
        // ViewTbOrders projection version for internal or custom use
        public async Task<Result<IEnumerable<ViewTbOrders>>> GetOrderByUserAsync(long? customerId)
        {
            try
            {
                if (customerId == null || customerId <= 0)
                {
                    return await Result.FailureAsync<IEnumerable<ViewTbOrders>>(new ErrorResponse("Invalid customer ID."));
                }
                await using var context = await contextFactory.CreateDbContextAsync();
                var orders = await context.TbOrders
                    .Where(o => o.CustomerId == customerId)
                    .Include(o => o.TbOrderDetails)
                    .AsNoTracking()
                    .Select(o => new ViewTbOrders
                    {
                        Id = o.Id,
                        CustomerId = o.CustomerId,
                        TableNumber = o.TableNumber,
                        ItemCount = o.ItemCount,
                        SubPrice = o.SubPrice,
                        DiscountPrice = o.DiscountPrice,
                        TotalPrice = o.TotalPrice,
                        Notes = o.Notes,
                        CreatedDT = o.CreatedDT,
                        ModifiedDT = o.ModifiedDT,
                        IsActive = o.IsActive,
                        TbOrderDetails = o.TbOrderDetails != null ? o.TbOrderDetails.Select(d => new ViewTbOrderDetails
                        {
                            Id = d.Id,
                            OrderId = d.OrderId,
                            ItemId = d.ItemId,
                            ItemName = d.ItemName,
                            Price = d.Price,
                            Quantity = d.Quantity,
                            TotalPrice = d.TotalPrice
                        }).ToList() : new List<ViewTbOrderDetails>()
                    })
                    .ToListAsync();
                return await Result.SuccessAsync<IEnumerable<ViewTbOrders>>(orders);
            }
            catch (Exception ex)
            {
                logger.LogError("Error Get order by User "+ex.Message, ex);
                return Result.Failure<IEnumerable<ViewTbOrders>>(ErrorResponse.ServerError(ex.Message));
            }
        }
    }
}
