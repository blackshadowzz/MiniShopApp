using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using MiniShopApp.Data;
using MiniShopApp.Data.TelegramStore;
using MiniShopApp.Infrastructures.Services.Implements;
using MiniShopApp.Models;
using MiniShopApp.Models.Customers;
using MiniShopApp.Models.Settings;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
        protected async Task<string> GetWebURLAsync()
        {
            await using var context = await dbContext.CreateDbContextAsync();
            var reult= await context.TbTelegramBotTokens.FirstOrDefaultAsync();
            if (reult != null)
            {
                return reult.WebAppUrl!;
            }
            return "";
        }
        private async Task OnMessage(ITelegramBotClient telegramBot, Update update, CancellationToken cancellationToken)
        {
            try
            {
                string url = await GetWebURLAsync();
                var webURL=url+ $"/index?userid={update.Message!.Chat.Id}";
                string webappUrl = $"https://minishopapp.runasp.net/index?userid={update.Message!.Chat.Id}";
                //var groupId=update.Message.MediaGroupId;
                //userState.UserId = update.Message!.Chat.Id;
                if (update.Message!.Text == "/start")
                {

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
                           
                            
                           
                            }
                           
                        


                            );
                }
                else if (update.Message.Text == "/help")
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
                else if (update.Message.Text == "/about")
                {
                    await _botClient.SendMessage(update.Message.Chat.Id, "This is Mini App\n\n" +
                        "We are developing a web application integrated with the Telegram Mini App platform, " +
                        "designed to streamline online ordering for restaurants and food vendors. While currently in testing, the app already supports essential features like menu browsing and placing orders. " +
                        "We’re actively refining performance and fixing minor issues to ensure a smooth and engaging experience for users within Telegram.\r\n\n" +
                        "Our App:\n" +
                        "Bot URL: t.me/Miniorder_bot\n" +
                        "Version: Testing\n" +
                        "Release: Under developing\n\n" +
                        "Thank you!!!",
                        replyMarkup: new InlineKeyboardButton[]
                            {
                            InlineKeyboardButton.WithWebApp("Open App",webURL),

                            }


                            );
                }
                else if (update.Message.Text == "/feedback")
                {
                    await _botClient.SendMessage(update.Message.Chat.Id, "Please send your feedback to our support team.",
                        replyMarkup: new InlineKeyboardButton[]
                            {
                                InlineKeyboardButton.WithUrl("Send us feedback", "https://t.me/Mangry_off"),
                            }
                            );
                }
                else if (update.Message.Text == "/setgroupid")
                {
                    //this command working on group only
                    if (update.Message.Chat.Type == ChatType.Group || update.Message.Chat.Type == ChatType.Supergroup)
                    {
                        long groupId = update.Message.Chat.Id;
                        await using var context = await dbContext.CreateDbContextAsync();
                        var result= await context.TbTelegramGroups.AsNoTracking().FirstOrDefaultAsync(x=>x.GroupId==groupId);
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
                            TelegramUserId=update.Message.From?.Id,
                            Description= $"This group's ID is: {groupId} Group Name: {update.Message.Chat.Title}",
                            IsActive=true
                        };
                        context.TbTelegramGroups.Add(data);
                        var row= await context.SaveChangesAsync();
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
                }else if (update.Message.Text == "/paid")
                {
                    //delete all user data in bot and delete bot after user use command /paid
                    //var chatId = update.Message.Chat.Id;
                    //var messageId = update.Message.MessageId;
                    //await _botClient.DeleteMessage(chatId,messageId);
                }
                else
                {
                    await _botClient.SendMessage(update.Message.Chat.Id, $"You said: {update.Message.Text}");
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

        //public override async Task StopAsync(CancellationToken cancellationToken)
        //{
        //    _logger.LogInformation("Stopping Telegram Bot service...");
        //    await base.StopAsync(cancellationToken);
        //    _logger.LogInformation("Telegram Bot service stopped.");
        //}
        private static readonly object _userLogLock = new object();
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

                    // Create a new user customer if it doesn't exist
                    context.TbUserCustomers.Add(new UserCustomer
                    {

                        CustomerId = update.Message.Chat.Id,
                        CustomerType = "Telegram",
                        FirstName = update.Message.Chat.FirstName,
                        LastName = update.Message.Chat.LastName,
                        UserName = update.Message.Chat.Username,
                        phoneNumber = update.Message.Contact?.PhoneNumber,
                        loginDateTime = DateTime.Now,
                        LastLoginDT = DateTime.Now,
                    });
                }
                await context.SaveChangesAsync();
                // Write new user info to a plain text file
                var userInfo = $"" +
                    $"UserId: {update.Message.Chat.Id}, " +
                    $"FirstName: {update.Message.Chat.FirstName}, " +
                    $"LastName: {update.Message.Chat.LastName}, " +
                    $"Username: {update.Message.Chat.Username}, " +
                    $"Phone: {update.Message.Contact?.PhoneNumber}, " +
                    $"Registered: {update.Message.Date.ToLocalTime()}\n";
                // Ensure the wwwroot directory exists
                var wwwrootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
                if (!Directory.Exists(wwwrootPath))
                {
                    Directory.CreateDirectory(wwwrootPath);
                }
                var logFilePath = Path.Combine(wwwrootPath, "UserLog.txt");
                lock (_userLogLock)
                {
                    File.AppendAllText(logFilePath, userInfo);
                }
                _logger.LogInformation("\n\nNew user registered successful: {UserId}\n\n", update.Message.Chat.Id);


            }
            catch (Exception ex)
            {

                var logMessage = $"Message log: {ex.Message}\n";
                    
                // Ensure the wwwroot directory exists
                var wwwrootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
                if (!Directory.Exists(wwwrootPath))
                {
                    Directory.CreateDirectory(wwwrootPath);
                }
                var logFilePath = Path.Combine(wwwrootPath, "UserLog.txt");
                lock (_userLogLock)
                {
                    File.AppendAllText(logFilePath, logMessage);
                }
                _logger.LogError(ex, "Error creating user customer with ID: {UserId}", update.Message.Chat.Id);
            }
        }
    }
}
