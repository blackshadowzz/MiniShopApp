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
        private readonly IDbContextFactory<AppDbContext> _context;
        private readonly ILogger<ProductService> logger;

        public ProductService(IDbContextFactory<AppDbContext> context, ILogger<ProductService> logger)
        {
            _context = context;
            this.logger = logger;
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
                
                await using var dbContext = _context.CreateDbContext();
                dbContext.TbProducts.Add(model);
                
                await dbContext.SaveChangesAsync();
                
                return "Product created successfully";

            } catch (Exception ex)
            {
                throw new Exception("Error creating product", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync(string? filter = null)
        {
            await using var dbContext = _context.CreateDbContext();
            if (string.IsNullOrEmpty(filter))
            {
                
                return await dbContext.TbProducts.ToListAsync();
            }
            else
            {
                return await dbContext.TbProducts
                    .Where(p => p.ProductName.Contains(filter) || p.ProductCode.Contains(filter))
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        
    }
}
