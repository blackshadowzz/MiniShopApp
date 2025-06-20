using Helpers.Responses;
using Microsoft.EntityFrameworkCore;
using MiniShopApp.Data;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models;

namespace MiniShopApp.Infrastructures.Services.Implements
{
    public class UserCustomerService : IUserCustomerService
    {
        private readonly IDbContextFactory<AppDbContext> contextFactory;
        private readonly ILogger<UserCustomerService> logger;

        public UserCustomerService(IDbContextFactory<AppDbContext> contextFactory,ILogger<UserCustomerService> logger)
        {
            this.contextFactory = contextFactory;
            this.logger = logger;
        }
        public async Task<Result<IEnumerable<ViewUserCustomers>>> GetAllAsync(string? filter = null)
        {
            try
            {
                logger.LogInformation("Retrieving user customers with filter: {Filter}", filter);
                await using var context = contextFactory.CreateDbContext();
                var results = context.TbUserCustomers
                    .Where(c => string.IsNullOrEmpty(filter)
                    || c.CustomerId.ToString().Contains(filter)
                    || c.LastName.Contains(filter)
                    || c.FirstName.Contains(filter))
                    .Select(c=> new ViewUserCustomers
                    {
                        Id = c.Id,
                        CustomerId = c.CustomerId,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        UserName = c.UserName,
                        phoneNumber = c.phoneNumber,
                        loginDateTime = c.loginDateTime,
                        LastLoginDT = c.LastLoginDT
                    });
                var userCustomers = await results.ToListAsync();
                logger.LogInformation("Retrieved {Count} user customers", userCustomers.Count);
                return Result.Success<IEnumerable<ViewUserCustomers>>(userCustomers);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error retrieving user customers with filter: {Filter}", filter);
                return Result.Failure<IEnumerable<ViewUserCustomers>>(ErrorResponse.ServerError(ex.Message));
            }
        }
    }
}
