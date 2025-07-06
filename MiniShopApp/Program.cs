using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Domain.IdentityModel;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using MiniShopApp;
using MiniShopApp.Components;
using MiniShopApp.Components.Account;
using MiniShopApp.Data;
using MiniShopApp.Data.TelegramStore;
using MiniShopApp.Infrastructures;
using MiniShopApp.Infrastructures.Services;
using MiniShopApp.Models.Settings;
using MudBlazor.Services;
using Telegram.Bot;


var builder = WebApplication.CreateBuilder(args);
var token = "";
var defaultedToken = builder.Configuration["BotTokenTest"];

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    //options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    //options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
})
    .AddIdentityCookies();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddRoleManager<RoleManager<ApplicationRole>>()
    .AddSignInManager()
    .AddDefaultTokenProviders();
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
builder.Services.AddHostedService<botService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddRazorPages();
builder.Services.AddMudServices();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeders>();
    seeder.DatabaseSeederAsync().GetAwaiter().GetResult();
}
if (!app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapControllers();
// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();
app.Run();
