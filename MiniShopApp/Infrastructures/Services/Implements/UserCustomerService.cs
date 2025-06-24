using Helpers.Responses;
using Microsoft.EntityFrameworkCore;
using MiniShopApp.Data;
using MiniShopApp.Data.TelegramStore;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace MiniShopApp.Infrastructures.Services.Implements
{
    public class UserCustomerService : IUserCustomerService
    {
        private readonly IDbContextFactory<AppDbContext> contextFactory;
        private readonly ILogger<UserCustomerService> logger;
        private readonly ITelegramBotClient botClient;

        public UserCustomerService(IDbContextFactory<AppDbContext> contextFactory,
            ILogger<UserCustomerService> logger,
            ITelegramBotClient botClient)
        {
            this.contextFactory = contextFactory;
            this.logger = logger;
            this.botClient = botClient;
        }

        public async Task<Result<string>> CreateUserAlertAsync(List<CustomerAlertMessege> alertMesseges)
        {
            try
            {
                if (alertMesseges!.Count > 0)
                {
                    foreach (var user in alertMesseges)
                    {
                        await botClient.SendMessage(user.CustomerId!, $"Hi {user.FirstName}! we missing u :)\n\n" +
                        $"{user.AlertMessege}\r\n\n" +
                        "Our App:\n" +
                        "Bot URL: t.me/Miniorder_bot\n" +
                        "Version: Testing\n" +
                        "Release: Under developing\n\n" +
                        "Thank you!!!",
                        replyMarkup: new InlineKeyboardButton[]
                            {
                            InlineKeyboardButton.WithWebApp("Open App",$"https://minishopapp.runasp.net/index?userid={user.CustomerId}"),

                            }


                            );
                    }
                    return Result.Success<string>("Alerting to users was succeed!");
                }
                return Result.Failure<string>(ErrorResponse.Failure("Alerting failed! users not found"));

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving user customers with filter: {Filter}", alertMesseges);
                return Result.Failure<string>(ErrorResponse.ServerError(ex.Message));
            }
        }

        public async Task<Result<string>> CreateUserAlertAsync(CustomerAlertMessege alertMessege)
        {
            try
            {
                if (alertMessege != null)
                {
                        await botClient.SendMessage(alertMessege.CustomerId!, $"Hi {alertMessege.FirstName}! we missing u :)\n\n" +
                        $"{alertMessege.AlertMessege}\r\n\n" +
                        "Our App:\n" +
                        "Bot URL: t.me/Miniorder_bot\n" +
                        "Version: Testing\n" +
                        "Release: Under developing\n\n" +
                        "Thank you!!!",
                        replyMarkup: new InlineKeyboardButton[]
                            {
                            InlineKeyboardButton.WithWebApp("Open App",$"https://minishopapp.runasp.net/index?userid={alertMessege.CustomerId}"),

                            }


                            );
                    return Result.Success<string>("Alerting to users was succeed!");
                }
                return Result.Failure<string>(ErrorResponse.Failure("Alerting failed! users not found"));

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving user customers with filter: {Filter}", alertMessege);
                return Result.Failure<string>(ErrorResponse.ServerError(ex.Message));
            }
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
