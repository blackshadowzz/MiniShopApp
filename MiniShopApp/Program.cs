
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MiniShopApp;
using MiniShopApp.Components;
using MiniShopApp.Data;
using MiniShopApp.Infrastructures;
using Radzen;
using Telegram.Bot;


var builder = WebApplication.CreateBuilder(args);
var token = builder.Configuration["BotToken"];
var webappUrl = builder.Configuration["BotWebAppUrl"];

//insert token to botService
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token!));
//Register background server when App Start
builder.Services.AddHostedService<botService>();

builder.Services.AddSingleton<UserState>();


builder.Services.AddRadzenComponents();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


//Add Service connection string by Constructor
var connectionString = builder.Configuration.GetConnectionString("MyServerConnection");
builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    options.ConfigureWarnings(w => w.Ignore(RelationalEventId.MigrationsUserTransactionWarning));

    options.UseSqlServer(connectionString);
});

/// Add any services in AddInfraServices class 
builder.Services.AddInfraServices(builder.Configuration);

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
