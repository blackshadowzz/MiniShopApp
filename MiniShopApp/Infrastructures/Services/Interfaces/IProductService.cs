using MiniShopApp.Models.Items;
using System.Diagnostics;

namespace MiniShopApp.Infrastructures.Services.Interfaces
{
    public interface IProductService
    {
        public Task<IEnumerable<Product>> GetAllAsync(string? filter=null);
        public Task<string> CreateAsync(long userId,Product model);
    }
}
