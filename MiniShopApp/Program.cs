using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniShopApp;
using MiniShopApp.Components;
using MiniShopApp.Data;
using MiniShopApp.Infrastructures;
using MiniShopApp.Services.Implements;
using MiniShopApp.Services.Interfaces;
using Radzen;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var builder = WebApplication.CreateBuilder(args);
var token = builder.Configuration["BotTokenTest"];
var webappUrl = builder.Configuration["BotWebAppUrl"];

//insert token to botService
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token!));
//Register background server when App Start
builder.Services.AddHostedService<botService>();

builder.Services.AddSingleton<UserState>();



builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRadzenComponents();

/// Add any services in AddInfraServices class 
builder.Services.AddInfraServices(builder.Configuration);

//Add Service connection string by Constructor
//var connectionString = builder.Configuration.GetConnectionString("MyConnection");
//builder.Services.AddDbContext<AppDbContext>(option =>
//{
//    option.UseSqlServer(connectionString);
//});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
