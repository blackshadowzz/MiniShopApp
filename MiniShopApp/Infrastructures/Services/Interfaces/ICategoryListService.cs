using Helpers.Responses;
using MiniShopApp.Models.Items;

namespace MiniShopApp.Infrastructures.Services.Interfaces
{
    public interface ICategoryListService
    {
        public Task<IEnumerable<Category>> GetAllAsync(string? filter = null);
        public Task<Category> GetOneTable(int? id);
        public Task<Result<string>> CreateAsync(Category model);
        public Task<bool> UpdateAsync(Category model, int? id);
        public Task<bool> DeleteAsync(int id);
    }
}
