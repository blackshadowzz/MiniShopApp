using Helpers.Responses;
using Microsoft.EntityFrameworkCore;
using MiniShopApp.Data;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Items;

namespace MiniShopApp.Infrastructures.Services.Implements
{
    public class CategoryListService : ICategoryListService
    {
        private readonly IDbContextFactory<AppDbContext> _context;
        private readonly ILogger<ICategoryListService> logger;
        public CategoryListService(IDbContextFactory<AppDbContext> context, ILogger<ICategoryListService> logger)
        {
            _context = context;
            this.logger = logger;
        }
        public async Task<Result<string>> CreateAsync(Category model)
        {
            try
            {
                await using var dbcontext = _context.CreateDbContext();
                dbcontext.TbCategories.Add(model);
                await dbcontext.SaveChangesAsync();
                return Result.Success<string>("create successfully");
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var dbContext = _context.CreateDbContext();
            var existing = await dbContext.TbCategories.FindAsync(id);
            if (existing == null)
            {
                return false;
            }
            try
            {
                dbContext.TbCategories.Remove(existing);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Category>> GetAllAsync(string? filter = null)
        {
            await using var dbContext = _context.CreateDbContext();
            if (string.IsNullOrEmpty(filter))
            {

                return await dbContext.TbCategories.ToListAsync();
            }
            else
            {
                return await dbContext.TbCategories
                    .Where(p => p.CategoryName!.Contains(filter))
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task<Category> GetOneTable(int? id)
        {
            await using var dbContext = _context.CreateDbContext();
            var geton = await dbContext.TbCategories.FirstOrDefaultAsync(x => x.CategoryId == id);
            return geton ?? default!;
        }

        public async Task<bool> UpdateAsync(Category model, int? id)
        {
            await using var dbContext = _context.CreateDbContext();
            var existing = await dbContext.TbCategories.FindAsync(id);
            if (existing == null)
            {
                return false;
            }
            try
            {
                dbContext.Entry(existing).CurrentValues.SetValues(model);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception as needed
                return false;
            }
        }
    }
}
