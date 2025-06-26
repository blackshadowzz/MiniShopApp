using Helpers.Responses;
using MiniShopApp.Models.Items;

namespace MiniShopApp.Infrastructures.Services.Interfaces
{
    public interface ITableListService
    {
        public Task<IEnumerable<TbTable>> GetAllAsync(string? filter = null);
        public Task<TbTable> GetOneTable(int? id);
        public Task<string> CreateAsync(TbTable model);
        public Task<bool> UpdateAsync(TbTable model,int? id);
        public Task<bool> DeleteAsync(int id);
    }
}
