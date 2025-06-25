using Helpers.Responses;
using Microsoft.EntityFrameworkCore;
using MiniShopApp.Data;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Settings;
using System.Collections.Generic;

namespace MiniShopApp.Infrastructures.Services.Implements
{
    public class TelegramBotServices : ITelegramBotServices
    {
        private readonly IDbContextFactory<AppDbContext> dbContext;
        private readonly ILogger<TelegramBotServices> logger;

        public TelegramBotServices(IDbContextFactory<AppDbContext> dbContext
            ,ILogger<TelegramBotServices> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }
        public async Task<Result<string>> DeleteGroupAsync(int id)
        {
            try
            {
                logger.LogInformation("Deleting group");
                await using var context= await dbContext.CreateDbContextAsync();
                var resul = await context.TbTelegramGroups.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id);
                if (resul != null)
                {
                    context.TbTelegramGroups.Remove(resul);
                    await context.SaveChangesAsync();
                    return Result.Success<string>("Group deleted succeed!");
                }
                return Result.Failure<string>(ErrorResponse.ServerError("Group not found!"));

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return Result.Failure<string>(ErrorResponse.ServerError(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<TbTelegramGroup>>> GetGroupAsync(string? filter = "")
        {
            try
            {
                logger.LogInformation("Getting group");
                await using var context = await dbContext.CreateDbContextAsync();
                var results = await context.TbTelegramGroups.AsNoTracking().ToListAsync();
                if (filter != null)
                {
                    results = results.Where(x => x.GroupName!.ToLower().Contains(filter.ToLower())).ToList();
                }
                return Result.Success <IEnumerable < TbTelegramGroup >> (results);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return Result.Failure<IEnumerable<TbTelegramGroup>>(ErrorResponse.ServerError(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<TbTelegramBotToken>>> GetTokenAsync(string? filter = "")
        {
            try
            {
                logger.LogInformation("Getting token");
                await using var context = await dbContext.CreateDbContextAsync();
                var results = await context.TbTelegramBotTokens.ToListAsync();
                
                return Result.Success<IEnumerable<TbTelegramBotToken>>(results);

            }
            catch (Exception ex)
            {
                logger.LogError("Error Get token " +ex.Message);
                return Result.Failure<IEnumerable<TbTelegramBotToken>>(ErrorResponse.ServerError(ex.Message));
            }
        }

        public async Task<Result<string>> SetBotTokenAsync(TbTelegramBotToken tbTelegram)
        {
            try
            {
                logger.LogInformation("Setting bot token");
                await using var context = await dbContext.CreateDbContextAsync();
                var result = await context.TbTelegramBotTokens.AsNoTracking().ToListAsync();
                if (result.Any())
                {
                    context.TbTelegramBotTokens.RemoveRange(result);

                }
                    context.TbTelegramBotTokens.Add(tbTelegram);

                await context.SaveChangesAsync();

                return Result.Success<string>("Bot Token set succeed!");
            }
            catch (Exception ex)
            {
                logger.LogError("Errors Server "+ ex.Message);
                return Result.Failure<string>(ErrorResponse.ServerError(ex.Message));
            }
        }

        public async Task<Result<string>> UpdateBotTokenAsync(TbTelegramBotToken tbTelegram)
        {
            try
            {
                logger.LogInformation("updating bot token");
                await using var context = await dbContext.CreateDbContextAsync();
                var result = await context.TbTelegramBotTokens.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==tbTelegram.Id);
                if (result!=null)
                {
                    context.TbTelegramBotTokens.Update(tbTelegram);
                    await context.SaveChangesAsync();
                    return Result.Success<string>("Bot Token updated succeed!");
                }
                return Result.Failure<string>(ErrorResponse.ServerError("Bot token not found!"));

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return Result.Failure<string>(ErrorResponse.ServerError(ex.Message));
            }
        }

        public async Task<Result<string>> UpdateGroupAsync(TbTelegramGroup tbTelegramGroup)
        {
            try
            {
                logger.LogInformation("updating group");
                await using var context = await dbContext.CreateDbContextAsync();
                var result = await context.TbTelegramGroups.AsNoTracking().FirstOrDefaultAsync(x => x.Id == tbTelegramGroup.Id);
                if (result != null)
                {
                    context.TbTelegramGroups.Update(tbTelegramGroup);
                    await context.SaveChangesAsync();
                    return Result.Success<string>("Group updated succeed!");
                }
                return Result.Failure<string>(ErrorResponse.ServerError("Group not found!"));

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return Result.Failure<string>(ErrorResponse.ServerError(ex.Message));
            }
        }
    }
}
