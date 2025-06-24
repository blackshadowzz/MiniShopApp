using Helpers.Responses;
using MiniShopApp.Models.Items;

namespace MiniShopApp.Infrastructures.Services.Interfaces
{
    public interface ITableListService
    {
        public Task<IEnumerable<TbTable>> GetAllAsync(string? filter = null);
        //public Task<Result<IEnumerable<ViewProductOrders>>> GetOrderAllAsync(string? filter = null);
        public Task<string> CreateAsync(TbTable model);
        public Task<string> UpdateAsync(TbTable model,int id);
        public Task<bool> DeleteAsync(int id);
    }
}
