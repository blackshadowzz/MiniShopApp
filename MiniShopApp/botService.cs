using DocumentFormat.OpenXml.Spreadsheet;
using Helpers.InformationLogs;
using Helpers.Responses;
using Mapster;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MiniShopApp.Data;
using MiniShopApp.Data.TelegramStore;
using MiniShopApp.Infrastructures.Services.Implements;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models;
using MiniShopApp.Models.Customers;
using MiniShopApp.Models.Orders;
using MiniShopApp.Models.Settings;
using MiniShopApp.Shared.AdditionalServices;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Update = Telegram.Bot.Types.Update;

namespace MiniShopApp
{
    public class botService : BackgroundService
    {
        private readonly ILogger<botService> _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly IDbContextFactory<AppDbContext> dbContext;
        private readonly IServiceProvider serviceProvider;

        //private readonly UserState userState;

        public botService(ILogger<botService> logger,
            ITelegramBotClient botClient,
            IDbContextFactory<AppDbContext> dbContext,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _botClient = botClient;
            this.dbContext = dbContext;
            this.serviceProvider = serviceProvider;
            //this.userState = userState;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Start receiving updates from Telegram with long polling
            var _reciverOptions = new ReceiverOptions
            {
                //AllowedUpdates = new UpdateType[] { UpdateType.Message }
                AllowedUpdates = Array.Empty<UpdateType>(), // receive all
                DropPendingUpdates = false // don't skip old messages
            };
            _botClient.StartReceiving(
                updateHandler: OnMessage,
                errorHandler: ErrorMsg,
                receiverOptions: _reciverOptions,
                cancellationToken: stoppingToken
                );
            _logger.LogInformation("Telegram Bot is now listening for updates.");

            // Keep the hosted service alive until cancellation is requested
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            await _botClient.LogOut();

        }
        protected async Task<string?> GetWebURLAsync()
        {
            await using var context = await dbContext.CreateDbContextAsync();
            var reult= await context.TbTelegramBotTokens.FirstOrDefaultAsync();
            if (reult != null)
            {
                return reult.WebAppUrl;
            }
            return "";
        }
        private async Task ExecutePrintReceiptAsync(string modelId, long chatId)
        {

            try
            {
                await using var context = await dbContext.CreateDbContextAsync();
                var model= await context.TbOrders.Include(x=>x.TbOrderDetails).FirstOrDefaultAsync(x=>x.Id==long.Parse(modelId));
                if (model == null)
                {
                    await _botClient.SendMessage(chatId, $"📄 Order receipt not found!");
                    return;
                }
                //get customer info 
                var user = await context.TbUserCustomers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.CustomerId == model!.CustomerId);
                using var scope = serviceProvider.CreateScope();
                var pdfService = scope.ServiceProvider.GetRequiredService<PdfService>();

                //Create receipt file send to bot
                var mapModel = model.Adapt<ViewTbOrders>();
                mapModel.FirstName = user?.FirstName;
                mapModel.LastName = user?.LastName;

                var pdfReceipt = pdfService.CreateOrderReceiptPdf(mapModel);

                // Send to customer
                await using (var stream = new MemoryStream(pdfReceipt))
                {
                    var inputFile = new InputFileStream(stream, $"Ordered_Receipt_{mapModel.OrderCode}.pdf");
                    await _botClient.SendDocument(
                    chatId: chatId,
                    document: inputFile,
                    caption: $"📄 Order receipt for #:{model.OrderCode} N0: {model.TableNumber}"
                    );
                }
                
            }
            catch (Exception ex)
            {
                SystemLogs.UserLogPlainText($"Error Print Receipt from Group: {ex.Message}\n");
                throw new Exception(ex.ToString() );
            } 
            
        }
        private async Task ExecuteConfirmAsync(string modelId, long chatId)
        {
            try
            {
                await using var context = await dbContext.CreateDbContextAsync();
                var result = await context.TbOrders.Where(o => o.Id == long.Parse(modelId)).FirstOrDefaultAsync();
                if (result==null)
                {
                    await _botClient.SendMessage(chatId, $"📄 Order not found!");
                    return;
                }
                result!.OrderStatus = Statuses.Cooked.ToString();
                result.EditSeq += 1;
                result.ModifiedDT = DateTime.Now;
                context.TbOrders.Update(result!);
                await context.SaveChangesAsync();
                await _botClient.SendMessage(chatId, $"🍳 Order: {result.OrderCode} N0: {result.TableNumber} confirmed.");
                if (result.CustomerType == "Telegram")
                {
                    await _botClient.SendMessage(result.CustomerId, $"🍳 Order: {result.OrderCode} N0: {result.TableNumber} confirmed.");
                }
            }
            catch (Exception ex)
            {
                SystemLogs.UserLogPlainText($"Error confirm Order from Group: {ex.Message}\n");
                throw new Exception(ex.ToString());
            }
            
        }

        private async Task ExecuteCancelAsync(string modelId, long chatId)
        {
            try
            {
                await using var context = await dbContext.CreateDbContextAsync();
                var result = await context.TbOrders.Where(o => o.Id == long.Parse(modelId)).FirstOrDefaultAsync();
                if (result == null)
                {
                    await _botClient.SendMessage(chatId, $"📄 Order not found!");
                    return;
                }
                result!.OrderStatus = Statuses.Canceled.ToString();
                result.EditSeq += 1;
                result.ModifiedDT = DateTime.Now;
                context.TbOrders.Update(result!);
                await context.SaveChangesAsync();
                await _botClient.SendMessage(chatId, $"🛑 Order {result.OrderCode} canceled.");
                if (result.CustomerType == "Telegram")
                {
                    await _botClient.SendMessage(result.CustomerId, $"🛑 Order {result.OrderCode} canceled.");

                }
            }
            catch (Exception ex)
            {
                SystemLogs.UserLogPlainText($"Error Cancel Order from Group: {ex.Message}\n");
                throw new Exception(ex.ToString());
            }
            
        }

        private async Task ExecutePaidAsync(string modelId, long chatId)
        {
            try
            {
                await using var context = await dbContext.CreateDbContextAsync();
                var result = await context.TbOrders.Where(o => o.Id == long.Parse(modelId)).FirstOrDefaultAsync();
                if (result == null)
                {
                    await _botClient.SendMessage(chatId, $"📄 Order not found!");
                    return;
                }
                result!.OrderStatus = Statuses.Paid.ToString();
                result.EditSeq += 1;
                result.ModifiedDT = DateTime.Now;
                context.TbOrders.Update(result!);
                await context.SaveChangesAsync();
                await _botClient.SendMessage(chatId, $"💰 Payment received for order {result.OrderCode}.");
                if (result.CustomerType == "Telegram")
                {
                    await _botClient.SendMessage(result.CustomerId, $"💰 Payment received for order {result.OrderCode}.");


                }
            }
            catch (Exception ex)
            {
                SystemLogs.UserLogPlainText($"Error Paid Order from Group: {ex.Message}\n");
                throw new Exception(ex.ToString());
            }
            
        }
        private async Task HandleCallbackQueryAsync(Update update)
        {
            var parts = update.CallbackQuery?.Data?.Split(':');
            var command = parts?.FirstOrDefault();
            var modelId = parts?.Length > 1 ? parts[1] : null;
            var message = update.CallbackQuery?.Message;
            var chatType = message?.Chat?.Type;
            var groupId = message?.Chat?.Id;

            if (groupId == null || (chatType != ChatType.Group && chatType != ChatType.Supergroup))
                return;

            async Task ExecuteOrNotifyAsync(string? id, Func<string, long, Task> action, string errorText)
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    await action(id, groupId.Value);
                }
                else
                {
                    await _botClient.SendMessage(
                        chatId: groupId.Value,
                        text: $"⚠️ Model ID is missing. Can't {errorText}.");
                }
            }

            var actions = new Dictionary<string, (Func<string, long, Task> action, string errorText)>
            {
                ["/cookconfirm"] = (ExecuteConfirmAsync, "confirm"),
                ["/cancelorder"] = (ExecuteCancelAsync, "cancel"),
                ["/paidconfirm"] = (ExecutePaidAsync, "paid"),
                ["/printreceipt"] = (ExecutePrintReceiptAsync, "print receipt")
            };

            if (command != null && actions.TryGetValue(command, out var entry))
            {
                await ExecuteOrNotifyAsync(modelId, entry.action, entry.errorText);
            }
        }

        private async Task OnMessage(ITelegramBotClient telegramBot, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.CallbackQuery != null)
                {
                    await HandleCallbackQueryAsync(update);
                    return;
                }


                if (update is { Message: { Text: "/start" }, CallbackQuery: null })
                {
                    string? url = await GetWebURLAsync();
                    string? webURL = url + $"/index?userid={update.Message!.Chat.Id}";
                    await _botClient.SetChatMenuButton(
                             chatId: update.Message.Chat.Id,
                             menuButton: new MenuButtonWebApp
                             {
                                 Text = "Open App",
                                 WebApp = new WebAppInfo { Url = webURL }
                             }
                         );
                    await _botClient.SendMessage(
                        update.Message.Chat.Id,
                        $"Welcome to our Mini App Online! {update.Message.Chat.FirstName}\n\n" +
                        $"Our Mini App still developing while you testing if has some errors, please feedback to us by type or click command /feedback. \n" +
                        $"\nThanks for testing! (_._) Click Open App",
                        replyMarkup: new InlineKeyboardButton[]
                            {
                            InlineKeyboardButton.WithWebApp("Open App",webURL),
                            });
                }
                else if (update.Message?.Text == "/help")
                {
                    await _botClient.SendMessage(update.Message.Chat.Id, "Please contact us for more informations.\n\n" +
                        "Available  command: \n" +
                        "/start - start bot or refresh bot.\n" +
                        "/help - get help.\n" +
                        "/feedback - send us feedback.\n" +
                        "/about - about us.\n\n",
                        replyMarkup: new InlineKeyboardButton[]
                            {
                            InlineKeyboardButton.WithUrl("Contact us","https://t.me/Mangry_off"),

                            }


                            );
                }
                else if (update.Message?.Text == "/about")
                {
                    await _botClient.SendMessage(update.Message.Chat.Id, "This is Mini App\n\n" +
                        "We are developing a web application integrated with the Telegram Mini App platform, " +
                        "designed to streamline online ordering for restaurants and food vendors. While currently in testing, the app already supports essential features like menu browsing and placing orders. " +
                        "We’re actively refining performance and fixing minor issues to ensure a smooth and engaging experience for users within Telegram.\r\n\n" +
                        "Our App:\n" +
                        "Bot URL: t.me/Miniorder_bot\n" +
                        "Version: Testing\n" +
                        "Release: Under developing\n\n" +
                        "Thank you!!!"
                       
                            );
                }
                else if (update.Message?.Text == "/feedback")
                {
                    await _botClient.SendMessage(update.Message.Chat.Id, "Please send your feedback to our support team.",
                        replyMarkup: new InlineKeyboardButton[]
                            {
                                InlineKeyboardButton.WithUrl("Send us feedback", "https://t.me/Mangry_off"),
                            }
                            );
                }
                else if (update.Message?.Text == "/setgroupid")
                {
                    //this command working on group only
                    if (update.Message.Chat.Type == ChatType.Group || update.Message.Chat.Type == ChatType.Supergroup)
                    {
                        long groupId = update.Message.Chat.Id;
                        await using var context = await dbContext.CreateDbContextAsync();
                        var result = await context.TbTelegramGroups.AsNoTracking().FirstOrDefaultAsync(x => x.GroupId == groupId);
                        if (result != null)
                        {
                            await _botClient.SendMessage(
                                chatId: groupId,
                                text: $"This group already set!!!"
                            );
                            return;
                        }
                        var data = new TbTelegramGroup
                        {
                            GroupId = groupId,
                            GroupName = update.Message.Chat.Title,
                            TelegramUserId = update.Message.From?.Id,
                            Description = $"This group's ID is: {groupId} Group Name: {update.Message.Chat.Title}",
                            IsActive = true
                        };
                        context.TbTelegramGroups.Add(data);
                        var row = await context.SaveChangesAsync();
                        if (row > 0)
                        {
                            await _botClient.SendMessage(
                                chatId: groupId,
                                text: $"This group was set to system succeed!!!"
                            );
                        }
                    }
                    else
                    {
                        await _botClient.SendMessage(
                            chatId: update.Message.Chat.Id,
                            text: "This command only works in groups."
                        );
                    }
                } else if (update.Message?.Text == "/paid")
                {
                    //delete all user data in bot and delete bot after user use command /paid
                    //var chatId = update.Message.Chat.Id;
                    //var messageId = update.Message.MessageId;
                    //await _botClient.DeleteMessage(chatId,messageId);
                }
                else
                {
                    var chatId = update.Message?.Chat.Id;
                    if (chatId != null)
                    {
                        await _botClient.SendMessage(chatId, $"Use command /help to see available command!");
                    }
                }
                
                await UserCustomerCreateAsync(update);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing update: {Update}", update);
                return;
            }

        }
        private async Task ErrorMsg(ITelegramBotClient telegramBot, Exception exp, CancellationToken cancellationToken)
        {
            if (exp is ApiRequestException requestException) await _botClient.SendMessage("", exp.Message.ToString());
        }
        private async Task UserCustomerCreateAsync(Update update)
        {
            if (update.Message?.Chat == null) return;
            
            
            _logger.LogInformation("New customer created with ID: {UserId}", update.Message.Chat.Id);
            try
            {
                
                await using var context = await dbContext.CreateDbContextAsync();
                var existingUser = await context.TbUserCustomers.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.CustomerId == update.Message.Chat.Id);
                if(existingUser != null)
                {
                    existingUser.LastLoginDT = DateTime.Now;
                    existingUser.CustomerType = "Telegram";

                    context.Update(existingUser);

                }
                else
                {
                    
                    bool? isPremium = update.Message.From?.IsPremium??false;
                    var location = update.Message?.Location;
                    string? address = location != null ? location.ToString() : "Unknown";
                    double? latitude = location?.Latitude;
                    double? longitude = location?.Longitude;

                    var chatId = update.Message?.Chat.Id;
                    // Create a new user customer if it doesn't exist
                    context.TbUserCustomers.Add(new UserCustomer
                    {

                        CustomerId = chatId??default,
                        CustomerType = "Telegram",
                        FirstName = update.Message?.Chat.FirstName,
                        LastName = update.Message?.Chat.LastName,
                        UserName = update.Message?.Chat.Username,
                        phoneNumber = update.Message?.Contact?.PhoneNumber,
                        loginDateTime = DateTime.Now,
                        LastLoginDT = DateTime.Now,
                        IsPremium = isPremium,
                        Address = address,
                        Latitude = latitude,
                        Longitude = longitude,
                    });
                    _logger.LogInformation("\n\nNew user registered successful: {UserId}\n\n", update.Message?.Chat.Id);
                }
                await context.SaveChangesAsync();
                // Write new user info to a plain text file
                var userInfo = $"Start Bot Log:" +
                    $"UserId: {update.Message?.Chat.Id}, " +
                    $"FirstName: {update.Message?.Chat.FirstName}, " +
                    $"LastName: {update.Message?.Chat.LastName}, " +
                    $"Username: {update.Message?.Chat.Username}, " +
                    $"Phone: {update.Message?.Contact?.PhoneNumber}, " +
                    $"Registered: {update.Message?.Date.ToLocalTime()}\n";
                SystemLogs.UserLogPlainText(userInfo);
                _logger.LogInformation($"\n\nUser start bot successful: {update.Message?.Chat.Id}\n\n");


            }
            catch (Exception ex)
            {
                var logMessage = $"Message log(start bot): {ex.Message}\n";
                SystemLogs.UserLogPlainText(logMessage);
                _logger.LogError(ex, "Error creating user customer with ID: {UserId}", update.Message.Chat.Id);
            }
        }
    }
}
