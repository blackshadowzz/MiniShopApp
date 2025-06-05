using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MiniShopApp
{
    public class botService : BackgroundService
    {
        private readonly ILogger<botService> _logger;
        private readonly ITelegramBotClient _botClient;
        public botService(ILogger<botService> logger, ITelegramBotClient botClient)
        {
            _logger = logger;
            _botClient = botClient;
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
            //if (update.Message is Message message) await _botClient.SendTextMessageAsync(message.Chat.Id, "I am kunpeng");
            if (update.Message is Message message) await _botClient.SendMessage(message.Chat.Id, "I am kunpeng");
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
