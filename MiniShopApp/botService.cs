using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using MiniShopApp.Data;
using MiniShopApp.Data.TelegramStore;
using MiniShopApp.Infrastructures.Services.Implements;
using MiniShopApp.Models;
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
        private readonly UserState userState;

        public botService(ILogger<botService> logger,
            ITelegramBotClient botClient,
            IDbContextFactory<AppDbContext> dbContext,
            IServiceProvider serviceProvider,
            UserState userState)
        {
            _logger = logger;
            _botClient = botClient;
            this.dbContext = dbContext;
            this.serviceProvider = serviceProvider;
            this.userState = userState;
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
        
        private async Task OnMessage(ITelegramBotClient telegramBot, Update update, CancellationToken cancellationToken)
        {
            try
            {//string webappUrl = "https://minishopapp.runasp.net/index";
                string webappUrl = $"https://minishopapp.runasp.net/index?userid={update.Message!.Chat.Id}";
                //if (update.Message is Message message)
                //{
                userState.UserId = update.Message!.Chat.Id;
                    if (update.Message!.Text == "/start")
                    {

                        
                        await _botClient.SendMessage(
                            update.Message.Chat.Id,
                            $"Welcome to our Mini App Online! {update.Message.Chat.FirstName}",
                            replyMarkup: new InlineKeyboardButton[]
                                {
                            InlineKeyboardButton.WithWebApp("Open App",webappUrl),

                                }


                                );
                    }
                    else if (update.Message.Text == "/help")
                    {
                        await _botClient.SendMessage(update.Message.Chat.Id, "Please contact us for more informations",
                            replyMarkup: new InlineKeyboardButton[]
                                {
                            InlineKeyboardButton.WithWebApp("Open App",webappUrl),

                                }


                                );
                    }
                    else if (update.Message.Text == "/about")
                    {
                        await _botClient.SendMessage(update.Message.Chat.Id, "This is a mini app for learning Blazor Server and Telegram Bot integration.",
                            replyMarkup: new InlineKeyboardButton[]
                                {
                            InlineKeyboardButton.WithWebApp("Open App",webappUrl),

                                }


                                );
                }
                    else
                    {
                        await _botClient.SendMessage(update.Message.Chat.Id, $"You said: {update.Message.Text}");
                    }
                    await UserCustomerCreateAsync(update);
                //}

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
                    existingUser.LastLoginDT = update.Message.Date.ToLocalTime();
                    context.Update(existingUser);

                }
                else
                {
                   
                    // Create a new user customer if it doesn't exist
                    context.TbUserCustomers.Add(new UserCustomer
                    {
                        
                        CustomerId = update.Message.Chat.Id,
                        FirstName = update.Message.Chat.FirstName,
                        LastName = update.Message.Chat.LastName,
                        UserName = update.Message.Chat.Username,
                        phoneNumber = update.Message.Contact?.PhoneNumber,
                        loginDateTime = update.Message.Date.ToLocalTime(),
                        LastLoginDT = update.Message.Date.ToLocalTime()
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
                _logger.LogError(ex, "Error creating user customer with ID: {UserId}", update.Message.Chat.Id);
            }
        }
    }
}
