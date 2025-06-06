using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniShopApp;
using MiniShopApp.Components;
using MiniShopApp.Data;
using MiniShopApp.Infrastructures;
using MiniShopApp.Services.Implements;
using MiniShopApp.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var builder = WebApplication.CreateBuilder(args);
var token = builder.Configuration["7823822574:AAFm2SzyMoepbVa6kMTElqlsbgr8JSUkkM4"];
var webappUrl = builder.Configuration["BotWebAppUrl"];



// Add services to the container.

builder.Services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient(httpClient => new TelegramBotClient("7823822574:AAFm2SzyMoepbVa6kMTElqlsbgr8JSUkkM4", httpClient));
//insert token to botService
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient("7823822574:AAFm2SzyMoepbVa6kMTElqlsbgr8JSUkkM4"));
//Register background server when App Start
builder.Services.AddHostedService<botService>();
builder.Services.AddScoped<UserState>();

//builder.Services.AddHostedService<ProductService>();


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

/// Add any services in AddInfraServices class 
builder.Services.AddInfraServices(builder.Configuration);

///Add Service connection string by Constructor
var connectionString = builder.Configuration.GetConnectionString("MyConection");
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
