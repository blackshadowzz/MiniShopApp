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
            var messageId = update.CallbackQuery?.Message?.MessageId;
            var chatType = message?.Chat?.Type;
            var chatId = message?.Chat.Id;
            var fromUserId = update.CallbackQuery?.From?.Id;
            var fromUserName = update.CallbackQuery?.From?.FirstName;
            SystemLogs.UserLogPlainText($"\nUser: {fromUserId} Name: {fromUserName} used: {command} in {chatId} Name: {message?.Chat.Title} Dated: {DateTime.Now.ToLocalTime()}\n");


            if (chatType == ChatType.Group || chatType == ChatType.Supergroup)
            {
                if (chatId != null)
                {
                    // Check if user is admin
                    var chatMember = await _botClient.GetChatMember(chatId, fromUserId!.Value);
                    if (chatMember.Status != ChatMemberStatus.Administrator &&
                        chatMember.Status != ChatMemberStatus.Creator)
                    {
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "🚫 You must be a group admin to perform this action.");
                        return;
                    }
                }
            }

            // Local helper to reduce code duplication
            async Task ExecuteOrNotifyAsync(string? id, Func<string, long, Task> action, string actionText)
            {
                if (chatId != null)
                {
                    if (!string.IsNullOrWhiteSpace(id) && messageId!=null)
                    {
                        await action(id, (long)chatId);
                        // 🧼 Changed text inline buttons after successful action
                        var updatedButtons = new List<List<InlineKeyboardButton>>
                        {
                            new()
                            {
                                command == "/cookconfirm"
                                    ? InlineKeyboardButton.WithCallbackData("✅ Confirmed", "disabled")
                                    : InlineKeyboardButton.WithCallbackData("Confirm Cook ✅", $"/cookconfirm:{modelId}"),

                                command == "/cancelorder"
                                    ? InlineKeyboardButton.WithCallbackData("❌ Cancelled", "disabled")
                                    : InlineKeyboardButton.WithCallbackData("Cancel Order ❌", $"/cancelorder:{modelId}")
                            },
                            new()
                            {
                                command == "/paidconfirm"
                                    ? InlineKeyboardButton.WithCallbackData("💵 Paid", "disabled")
                                    : InlineKeyboardButton.WithCallbackData("Confirm Paid 💵", $"/paidconfirm:{modelId}")
                            },
                            new()
                            {
                                command == "/printreceipt"
                                    ? InlineKeyboardButton.WithCallbackData("📄 Printed", "disabled")
                                    : InlineKeyboardButton.WithCallbackData("Print Receipt(PDF)📄", $"/printreceipt:{modelId}")
                            }
                        };

                        var replyMarkup = new InlineKeyboardMarkup(updatedButtons);
                        await _botClient.EditMessageReplyMarkup(
                            chatId: chatId,
                            messageId: messageId.Value,
                            replyMarkup: replyMarkup
                        );
                        if(update.CallbackQuery?.Id!=null)
                            await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, "✅ Action completed");

                    }
                    else
                    {
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: $"⚠️ Model ID is missing. Can't {actionText}.");
                    }
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
            else
            {
                if(chatId != null)
                    await _botClient.SendMessage(
                            chatId: chatId,
                            text: "❓ Unknown command.");
            }
        }
        private async Task HandleUserCommandAsync(Update update)
        {
            var message = update.Message;
            var chatId = message?.Chat.Id;

            if (message is null || chatId is null || update.CallbackQuery != null)
                return;

            switch (message.Text)
            {
                case "/start":
                    string? url = await GetWebURLAsync();
                    string webURL = $"{url}/index?userid={chatId}";

                    await _botClient.SetChatMenuButton(chatId.Value, new MenuButtonWebApp
                    {
                        Text = "Open App",
                        WebApp = new WebAppInfo { Url = webURL }
                    });

                    await _botClient.SendMessage(chatId.Value,
                        $"Welcome to our Mini App Online! {message.Chat.FirstName}\n\n" +
                        $"Our Mini App is still in development. If you find any issues, please send feedback using /feedback.\n\nThanks for testing! (_._)",
                        replyMarkup: InlineKeyboardButton.WithWebApp("Open App", webURL));

                    break;

                case "/help":
                    await _botClient.SendMessage(chatId.Value,
                        "📖 Help Menu\n\n" +
                        "Available Commands:\n" +
                        "/start - Start or refresh the bot.\n" +
                        "/help - Get help.\n" +
                        "/feedback - Send us feedback.\n" +
                        "/about - About the project.\n",
                        replyMarkup: InlineKeyboardButton.WithUrl("Contact us", "https://t.me/Mangry_off"));
                    break;

                case "/about":
                    await _botClient.SendMessage(chatId.Value,
                        "ℹ️ About Mini App\n\n" +
                        "We’re developing a web-based Telegram Mini App for restaurants and food vendors. " +
                        "Currently in testing, it supports menu browsing, placing orders, and more. " +
                        "Bot URL: t.me/Miniorder_bot\nVersion: Testing\nRelease: Under development\n\nThank you!");
                    break;

                case "/feedback":
                    await _botClient.SendMessage(chatId.Value,
                        "📝 Please send your feedback to our support team.",
                        replyMarkup: InlineKeyboardButton.WithUrl("Send us feedback", "https://t.me/Mangry_off"));
                    break;

                case "/setgroupid":
                    if (message.Chat.Type is ChatType.Group or ChatType.Supergroup)
                    {
                        await HandleSetGroupIdAsync(chatId.Value, message);
                    }
                    else
                    {
                        await _botClient.SendMessage(chatId.Value, "⚠️ This command only works in groups.");
                    }
                    break;

                default:
                    //if (message.Chat.Type is not ChatType.Group or ChatType.Supergroup)
                    //{
                    //    await _botClient.SendMessage(chatId.Value, "❓ Unknown command. Use /help to see available options.");
                    //}
                    break;
            }
        }
        private async Task HandleSetGroupIdAsync(long groupId, Message message)
        {
            await using var context = await dbContext.CreateDbContextAsync();
            try
            {
                var existing = await context.TbTelegramGroups.AsNoTracking().FirstOrDefaultAsync(x => x.GroupId == groupId);
                if (existing != null)
                {
                    await _botClient.SendMessage(groupId, "⚠️ This group is already registered.");
                    return;
                }

                var groupData = new TbTelegramGroup
                {
                    GroupId = groupId,
                    GroupName = message.Chat.Title,
                    TelegramUserId = message.From?.Id,
                    Description = $"Group ID: {groupId}, Name: {message.Chat.Title}",
                    IsActive = true
                };

                context.TbTelegramGroups.Add(groupData);
                var saved = await context.SaveChangesAsync();

                if (saved > 0)
                {
                    await _botClient.SendMessage(groupId, "✅ Group successfully registered!");
                }
            }
            catch (Exception ex)
            {
                SystemLogs.UserLogPlainText($"Error While set Group: {ex.Message}");
                throw new Exception(ex.Message);
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
                await HandleUserCommandAsync(update);
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
