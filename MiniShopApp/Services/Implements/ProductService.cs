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
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly ITelegramBotClient _botClient;
     

        public ProductService(AppDbContext context,ITelegramBotClient botClient )
        {
            _context = context;
            _botClient = botClient;
        }

        public async Task<string> CreateAsync(long userId, Product model)
        {
            try
            {

                //await _botClient.SendMessage(
                //        chatId:  userId, // Replace with your chat ID
                        
                //        text: 
                //        $"Product Code: {model.ProductCode}" +
                //        $"\nProduct Name: {model.ProductName}" +
                //        $"\n Price: {model.Price}" +
                //        $"\n Description: {model.Decription}",
                //        parseMode:ParseMode.Html
                //    );
                

                _context.TbProducts.Add(model);
                
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
                return await _context.TbProducts.ToListAsync();
            }
            else
            {
                return await _context.TbProducts
                    .Where(p => p.ProductName.Contains(filter) || p.ProductCode.Contains(filter))
                    .ToListAsync();
            }
        }

        
    }
}
