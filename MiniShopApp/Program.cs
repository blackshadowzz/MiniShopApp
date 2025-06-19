
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using MiniShopApp;
using MiniShopApp.Components;
using MiniShopApp.Data.TelegramStore;
using MiniShopApp.Infrastructures;
using Radzen;
using Telegram.Bot;


var builder = WebApplication.CreateBuilder(args);
var token = builder.Configuration["BotTokenTest"];
var webappUrl = builder.Configuration["BotWebAppUrl"];

//insert token to botService
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token!));
//Register background server when App Start
builder.Services.AddHostedService<botService>();

builder.Services.AddSingleton<UserState>();
builder.Services.AddRadzenComponents();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

/// Add any services in AddInfraServices class 
builder.Services.AddInfraServices(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/UserLog.txt"))
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Access Denied.");
        return;
    }
    await next();
});
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
