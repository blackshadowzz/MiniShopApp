using Microsoft.EntityFrameworkCore;
using MiniShopApp.Data;
using MiniShopApp.Models.Items;
using MiniShopApp.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MiniShopApp.Services.Implements
{
    public class ProductService : BackgroundService, IProductService
    {
        private readonly AppDbContext _context;
        private readonly ITelegramBotClient _botClient;

        public ProductService(AppDbContext context,ITelegramBotClient botClient)
        {
            _context = context;
            _botClient = botClient;
        }

        public async Task<string> CreateAsync(long userId, Product model)
        {
            try
            {
                // Placeholder for the update object, if needed
                //if (update?.Message?.Chat != null)
                //{
                    await _botClient.SendMessage(
                        chatId: userId, // Replace with your chat ID
                        text: $"Creating product: {model.ProductName}"
                    );
                //}
                //else
                //{
                //    throw new ArgumentNullException(nameof(update.Message.Chat), "Chat information is missing in the update.");
                //}
                _context.Products.Add(model);
                
                await _context.SaveChangesAsync();
                
                return "Product created successfully";

            } catch (Exception ex)
            {
                throw new Exception("Error creating product", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync(string? filter = null)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return await _context.Products.ToListAsync();
            }
            else
            {
                return await _context.Products
                    .Where(p => p.ProductName.Contains(filter) || p.ProductCode.Contains(filter))
                    .ToListAsync();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
                cancellationToken: stoppingToken
                );
            //_logger.LogInformation("Telegram Bot is now listening for updates.");

            // Keep the hosted service alive until cancellation is requested
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            await _botClient.LogOut();
        }
        private async Task OnMessage(ITelegramBotClient telegramBot, Update update, CancellationToken cancellationToken)
        {
           
            if (update.Message!.Text=="/help")
            {
                await _botClient.SendMessage(update.Message.Id, "Welcome to our Mini App Online");
            }
        }

        private async Task ErrorMsg(ITelegramBotClient telegramBot, Exception exp, CancellationToken cancellationToken)
        {
            if (exp is ApiRequestException requestException) await _botClient.SendMessage("", exp.Message.ToString());
        }
    }
}
