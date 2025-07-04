
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using MiniShopApp;
using MiniShopApp.Components;
using MiniShopApp.Data;
using MiniShopApp.Data.TelegramStore;
using MiniShopApp.Infrastructures;
using MiniShopApp.Models.Settings;
using MudBlazor.Services;
using Telegram.Bot;


var builder = WebApplication.CreateBuilder(args);
var token = "";
var defaultedToken = builder.Configuration["BotTokenTest"];

// Add any services in AddInfraServices class 
builder.Services.AddInfraServices(builder.Configuration);

// Build a temporary provider to get the token from the database
var dbContextFactory = builder.Services.BuildServiceProvider().GetRequiredService<IDbContextFactory<AppDbContext>>();
using (var context = dbContextFactory.CreateDbContext())
{
    try
    {
        var tokenEntity = context.TbTelegramBotTokens.AsNoTracking().OrderByDescending(x=>x.Id).FirstOrDefault();
        if (tokenEntity != null)
        {
            if (string.IsNullOrEmpty(tokenEntity.BotToken))
            {
                token=defaultedToken;
            }
            else
            {
                token = tokenEntity.BotToken;
                Console.WriteLine("Bot Token set!");
            }
               
        }
        else
        {
            token = defaultedToken;
            Console.WriteLine("Get Bot Token not found!");
        }
    }
    catch (Exception ex)
    {
        token = defaultedToken;

        throw new Exception("Program :"+ex.Message, ex);
    }
    
}
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token!));
//Register background server when App Start
builder.Services.AddHostedService<botService>();

builder.Services.AddScoped<UserState>();

builder.Services.AddMudServices();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

//app.Use(async (context, next) =>
//{
//    if (context.Request.Path.StartsWithSegments("/UserLog.txt"))
//    {
//        context.Response.StatusCode = StatusCodes.Status403Forbidden;
//        await context.Response.WriteAsync("Access Denied.");
//        return;
//    }
//    await next();
//});
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
