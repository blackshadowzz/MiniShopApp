using Microsoft.EntityFrameworkCore;
using MiniShopApp.Data;
using MiniShopApp.Models.Items;
using MiniShopApp.Services.Interfaces;

namespace MiniShopApp.Services.Implements
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        public ProductService(AppDbContext context)
        {
            _context = context;
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
    
    
    }
}
