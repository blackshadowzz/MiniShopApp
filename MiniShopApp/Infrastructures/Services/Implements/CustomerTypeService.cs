using Helpers.Responses;
using Microsoft.EntityFrameworkCore;
using MiniShopApp.Data;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Customers;

namespace MiniShopApp.Infrastructures.Services.Implements
{
    public class CustomerTypeService(IDbContextFactory<AppDbContext> contextFactory, ILogger<CustomerTypeService> logger) : ICustomerTypeService
    {
        public async Task<Result<string>> CreateAsync(CustomerTypeDtoCreate dto)
        {
            try
            {
                await using var db = await contextFactory.CreateDbContextAsync();
                var entity = CustomerType.FromDtoCreate(dto);
                db.TbCustomerTypes.Add(entity);
                await db.SaveChangesAsync();
                return Result<string>.Success($"Customer type '{entity.TypeName}' created successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating customer type");
                return Result<string>.Failure<string>(ErrorResponse.ServerError($"Failed to create customer type: {ex.Message}"));
            }
        }

        public async Task<Result<string>> DeleteAsync(int id)
        {
            try
            {
                await using var db = await contextFactory.CreateDbContextAsync();
                var entity = await db.TbCustomerTypes.FindAsync(id);
                if (entity == null)
                    return Result<string>.Failure<string>(ErrorResponse.NotFound($"Customer type with id {id} not found."));
                db.TbCustomerTypes.Remove(entity);
                await db.SaveChangesAsync();
                return Result<string>.Success($"Customer type '{entity.TypeName}' deleted successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting customer type");
                return Result<string>.Failure<string>(ErrorResponse.ServerError($"Failed to delete customer type: {ex.Message}"));
            }
        }

        public async Task<Result<IEnumerable<ViewCustomerType>>> GetAllAsync(string? filter = "")
        {
            try
            {
                await using var db = await contextFactory.CreateDbContextAsync();
                var query = db.TbCustomerTypes.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    query = query.Where(x => x.TypeName != null && x.TypeName.Contains(filter));
                }
                var result = await query.Select(x => x.ToViewCustomerType()).AsNoTracking().ToListAsync();
                return Result<IEnumerable<ViewCustomerType>>.Success(result.AsEnumerable());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting all customer types");
                return Result<IEnumerable<ViewCustomerType>>.Failure<IEnumerable<ViewCustomerType>>(ErrorResponse.ServerError($"Failed to get customer types: {ex.Message}"));
            }
        }

        public async Task<Result<ViewCustomerType>> GetByIdAsync(int id)
        {
            try
            {
                await using var db = await contextFactory.CreateDbContextAsync();
                var entity = await db.TbCustomerTypes.FindAsync(id);
                if (entity == null)
                    return Result<ViewCustomerType>.Failure<ViewCustomerType>(ErrorResponse.NotFound($"Customer type with id {id} not found."));
                return Result<ViewCustomerType>.Success(entity.ToViewCustomerType());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting customer type by id");
                return Result<ViewCustomerType>.Failure<ViewCustomerType>(ErrorResponse.ServerError($"Failed to get customer type: {ex.Message}"));
            }
        }

        public async Task<Result<string>> UpdateAsync(CustomerTypeDtoUpdate dto)
        {
            try
            {
                await using var db = await contextFactory.CreateDbContextAsync();
                var entity = await db.TbCustomerTypes.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==dto.Id);
                if (entity == null)
                    return Result<string>.Failure<string>(ErrorResponse.NotFound($"Customer type with id {dto.Id} not found."));
                var model = CustomerType.FromDtoUpdate(dto);
                db.TbCustomerTypes.Update(model);
                await db.SaveChangesAsync();
                return Result<string>.Success($"Customer type '{entity.TypeName}' updated successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating customer type");
                return Result<string>.Failure<string>(ErrorResponse.ServerError($"Failed to update customer type: {ex.Message}"));
            }
        }
    }
}
