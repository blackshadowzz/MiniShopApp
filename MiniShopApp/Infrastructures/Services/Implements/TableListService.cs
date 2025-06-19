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
    }
}
