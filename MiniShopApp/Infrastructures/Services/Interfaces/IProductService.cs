using Helpers.Responses;
using MiniShopApp.Models.Items;
using System.Diagnostics;

namespace MiniShopApp.Infrastructures.Services.Interfaces
{
    public interface IProductService
    {
        public Task<IEnumerable<Product>> GetAllAsync(string? filter=null);
        public Task<Product> GetProductById(int id);
        public Task<Result<IEnumerable<ViewProductOrders>>> GetOrderAllAsync(string? filter=null);
        public Task<string> CreateAsync(Product model);
        public Task<string> UpdateAsync(Product model);
        public Task<string> DeleteAsync(int id);
    }
}
