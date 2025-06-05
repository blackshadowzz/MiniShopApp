using MiniShopApp.Models.Items;
using System.Diagnostics;

namespace MiniShopApp.Services.Interfaces
{
    public interface IProductService
    {
        public Task<IEnumerable<Product>> GetAllAsync(string? filter=null);
    }
}
