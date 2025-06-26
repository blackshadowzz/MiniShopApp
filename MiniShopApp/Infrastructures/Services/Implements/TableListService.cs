using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MiniShopApp.Data;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Items;

namespace MiniShopApp.Infrastructures.Services.Implements
{
    public class TableListService : ITableListService
    {
        private readonly IDbContextFactory<AppDbContext> _context;
        private readonly ILogger<ITableListService> logger;
        public TableListService(IDbContextFactory<AppDbContext> context, ILogger<ITableListService> logger)
        {
            _context = context;
            this.logger = logger;
        }

        public async Task<string> CreateAsync(TbTable model)
        {
            try
            {
                await using var dbcontext = _context.CreateDbContext();
                dbcontext.TbTables.Add(model);
                await dbcontext.SaveChangesAsync();
                return "Tabel Code create successfully";
            }
            catch(Exception ex)
            {
                throw new Exception("Error creating product", ex);
            }

        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var dbContext = _context.CreateDbContext();
            var existing = await dbContext.TbTables.FindAsync(id);
            if (existing == null)
            {
                return false;
            }
            try
            {
                dbContext.TbTables.Remove(existing);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<IEnumerable<TbTable>> GetAllAsync(string? filter = null)
        {
            await using var dbContext = _context.CreateDbContext();
            if (string.IsNullOrEmpty(filter))
            {

                return await dbContext.TbTables.ToListAsync();
            }
            else
            {
                return await dbContext.TbTables
                    .Where(p => p.TableNumber!.Contains(filter))
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task<TbTable> GetOneTable(int? id)
        {
            await using var dbContext = _context.CreateDbContext();
            var geton = await dbContext.TbTables.FirstOrDefaultAsync(x=>x.TableId == id);
            return geton ?? default!;
        }

        public async Task<bool> UpdateAsync(TbTable model, int? id)
        {
            await using var dbContext = _context.CreateDbContext();
            var existing = await dbContext.TbTables.FindAsync(id);
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
