using Helpers.Responses;
using Microsoft.EntityFrameworkCore;
using MiniShopApp.Data;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Items;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MiniShopApp.Infrastructures.Services.Implements
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

        public async Task<string> CreateAsync(Product model)
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
        public async Task<string> UpdateAsync(Product model)
        {
            try
            {
                await using var dbContext = _context.CreateDbContext();
                dbContext.TbProducts.Update(model);
                await dbContext.SaveChangesAsync();
                return "Product updated successfully";  
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating product", ex);
            }
        }

        public Task<string> DeleteAsync(int id)
        {
            throw new NotImplementedException();
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

        public async Task<Result<IEnumerable<ViewProductOrders>>> GetOrderAllAsync(string? filter = null)
        {
            try
            {
                

                logger.LogInformation("Fetching product orders with filter: {Filter}", filter);
                await using var dbContext = _context.CreateDbContext();
                IQueryable<ViewProductOrders> results = (from pro in dbContext.TbProducts
                               join cat in dbContext.TbCategories
                               on pro.CategoryId equals cat.CategoryId into catg
                               from cat in catg.DefaultIfEmpty()
                               select new ViewProductOrders
                               {
                                   Id = pro.Id,
                                   ProductCode = pro.ProductCode,
                                   ProductName = pro.ProductName,
                                   Price = pro.Price,
                                   SubPrice = pro.SubPrice,
                                   Description = pro.Description,
                                   ImageUrl = pro.ImageUrl,
                                   IsActive = pro.IsActive,
                                   CategoryId = pro.CategoryId,
                                   CategoryName = cat.CategoryName
                               });
                var filteredResults = string.IsNullOrEmpty(filter)
                    ? results
                    : results.Where(x =>
                EF.Functions.Like(x.CategoryName, $"%{filter}%") ||
                EF.Functions.Like(x.ProductCode, $"%{filter}%") ||
                EF.Functions.Like(x.ProductName, $"%{filter}%") ||
                EF.Functions.Like(x.Description, $"%{filter}%"));
                var productOrders = await filteredResults.AsNoTracking().ToListAsync();
                return Result.Success<IEnumerable<ViewProductOrders>>(productOrders);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error fetching product orders");
                return Result.Failure<IEnumerable<ViewProductOrders>>(ErrorResponse.ServerError(ex.Message));
            }
        }

        public async Task<Product> GetProductById(int id)
        {
            using var context = _context.CreateDbContext();
            return await context.TbProducts.FindAsync(id) ?? default!;
        }

        
    }
}
