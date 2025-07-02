using System.Collections.Generic;
using System.Threading.Tasks;
using MiniShopApp.Models.Customers;
using Helpers.Responses;

namespace MiniShopApp.Infrastructures.Services.Interfaces
{
    public interface ICustomerTypeService
    {
        Task<Result<IEnumerable<ViewCustomerType>>> GetAllAsync(string? filter="");
        Task<Result<ViewCustomerType>> GetByIdAsync(int id);
        Task<Result<string>> CreateAsync(CustomerTypeDtoCreate dto);
        Task<Result<string>> UpdateAsync(CustomerTypeDtoUpdate dto);
        Task<Result<string>> DeleteAsync(int id);
    }
}
