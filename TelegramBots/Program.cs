using System.Xml.Linq;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


internal class Program
{
    private static readonly string Token = "7823822574:AAFm2SzyMoepbVa6kMTElqlsbgr8JSUkkM4";
    private static TelegramBotClient botClient;
    private static void Main(string[] args)
    {
        TelegramBotClient botClient;



        botClient = new TelegramBotClient(Token);
        var result = botClient.GetMe().Result;

        botClient.StartReceiving(UpdateHandler, ErrorHandler);

}
private static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken token)
        {

            if (update.Type != UpdateType.Message)
                return;
            if (update.Message!.Type != MessageType.Text)
                return;
            switch (update.Message.Text.ToLower())
            {
                case "/start":
                    var m = await
                    botClient.SendMessage(
                        chatId: update.Message.Chat,
                        text: "Welcome to the bot! Type /help to see available commands."
                    );
                
                    //txtMessage.Text = m.Chat.Username;
                    //txtName.Text = m.Chat.FirstName + " " + m.Chat.LastName;
                    break;

                case "/help":
                    await
                    botClient.SendMessage(
                        chatId: update.Message.Chat,
                        text: "/start - Start the bot\n/help - Get help"
                    );
                    break;

                default:
                    await
                    botClient.SendMessage(
                        chatId: update.Message.Chat,
                        text: "You said:\n" + update.Message.Text
                    );
                    break;
            }
        }
        private static Task ErrorHandler(ITelegramBotClient bot, Exception exception, CancellationToken token)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Error : \n {apiRequestException.ErrorCode} \n {apiRequestException.Message}",
                _ => exception.ToString()
            };

            return Task.CompletedTask;

        }
    }
