using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MiniShopApp.Data;
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
        private readonly UserState userState;

        public botService(ILogger<botService> logger,
            ITelegramBotClient botClient,
            UserState userState)
        {
            _logger = logger;
            _botClient = botClient;
            this.userState = userState;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Subscribe to message event

            //_botClient.OnMessage += BotClient_OnMessage;

            // Start receiving updates from Telegram with long polling
            var _reciverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[] { UpdateType.Message }
            };
            _botClient.StartReceiving(
                updateHandler: OnMessage,
                errorHandler: ErrorMsg,
                receiverOptions: _reciverOptions,
                cancellationToken:stoppingToken
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
            {

               
                //if (update.Message is Message message) await _botClient.SendTextMessageAsync(message.Chat.Id, "I am kunpeng");
                userState.UserId = update.Message!.Chat.Id;

                //if (update.Message is Message message)
                //{
                //    userState.UserId = update.Message!.Chat.Id;
                //}
                if (update.Message!.Text == "/start")
                {

                    string webappUrl = "https://google.com/";
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
                    await _botClient.SendMessage(update.Message.Chat.Id, "Please contact us for more informations");
                }
                else if (update.Message.Text == "/about")
                {
                    await _botClient.SendMessage(update.Message.Chat.Id, "This is a mini app for learning Blazor Server and Telegram Bot integration.");
                }
                else
                {
                    await _botClient.SendMessage(update.Message.Chat.Id, $"You said: {update.Message.Text}");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error processing update: {Update}", update);
                return;
            }
            
        }
        private async Task ErrorMsg(ITelegramBotClient telegramBot, Exception exp, CancellationToken cancellationToken)
        {
            if (exp is ApiRequestException requestException) await _botClient.SendMessage("", exp.Message.ToString());
        }



        //private async void BotClient_OnMessage(object sender, Message e)
        //{
        //    if (e.Text != null)
        //    {
        //        _logger.LogInformation($"Received message from chat {e.MessageId}: {e.Text}");

        //        // For example, respond to a "/start" command or any text message
        //        var response = e.Text.ToLower() == "/start"
        //            ? "Welcome to your Blazor Server Telegram Bot!"
        //            : $"You said: {e.Text}";

        //        await _botClient.SendMessage(
        //            chatId: e.MessageId,
        //            text: response
        //        );
        //    }

        //}
    }
}
