using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Settings;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MiniShopApp.Pages.Settings.Telegrams
{
    public partial class TelegramConfigs(ITelegramBotServices telegramBotServices)
    {
        protected TbTelegramBotToken? model = new();
        protected TbTelegramBotTokenDto? modelDto = new();
        protected List<TbTelegramGroup>? groups = [];
        bool IsLoading=false;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var results = await telegramBotServices.GetTokenAsync();
                if (results.IsSuccess && results.Data!.Any())
                {
                    model = results.Data?.FirstOrDefault();
                    modelDto.BotToken = model.BotToken;
                    modelDto.WebAppUrl = model.WebAppUrl;
                }
                else
                {
                    model = new();
                    errors = new[] { "Failed to retrieve token." };

                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error UI "+ ex.Message);
            }
            
            await base.OnInitializedAsync();
        }
        async Task ModifyGroup(TbTelegramGroup model)
        {
            try
            {
                var active =(bool)model.IsActive?false:true;
                model.IsActive = active;
                _loading = true;
                var results = await telegramBotServices.UpdateGroupAsync(model);
                if (results.IsSuccess)
                {
                    await GetGroups();
                    SnackbarService.Add(results.Data,MudBlazor.Severity.Success);
                }
                _loading = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        async Task GetGroups(string? filter="")
        {
            try
            {
                _loading = true;
                var results = await telegramBotServices.GetGroupAsync(filter);
                if (results.IsSuccess)
                {
                    groups = results.Data?.ToList();
                }
                _loading = false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        async Task SetBotAsync()
        {
            await form.Validate();
            if (form.IsValid)
            {
                IsLoading = true;
                // Validate the token before showing confirmation dialog
                if (!await ValidateTelegramBotTokenAsync(model?.BotToken))
                {
                    errors = new[] { "Invalid Telegram Bot Token. Please check and try again." };
                    SnackbarService.Add("Invalid Telegram Bot Token. Please check and try again.",MudBlazor.Severity.Warning);
                    IsLoading = false;
                    return;
                }
                if (!await ValidateWebAppUrlAsync(model?.WebAppUrl))
                {
                    errors = new[] { "Invalid or unreachable Web App URL. Please check and try again." };
                    SnackbarService.Add("Invalid or unreachable Web App URL. Please check and try again.", MudBlazor.Severity.Warning);
                    IsLoading=false;
                    return;
                }
                var result = await DialogService.ShowMessageBox(
                   "Confirmation",
                   $"please Double check your Token & Web URL is correct, avoiding system errors. \n" +
                   $"All is correct?",
                   "Yes", "No");
                if (result == true)
                {
                    model!.WebAppUrl = model.WebAppUrl?.TrimEnd('/');
                    var results = await telegramBotServices.SetBotTokenAsync(model!);
                    if (results.IsSuccess)
                    {
                        SnackbarService.Add(results.Data!, MudBlazor.Severity.Success);
                        errors = new[] { results.Data };
                        await OnInitializedAsync();
                    }
                    else
                        errors = new[] { results.ErrMessage };
                    IsLoading = false;
                }
                
                IsLoading = false;
                StateHasChanged();
            } 
        }
        private async Task<bool> ValidateTelegramBotTokenAsync(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            try
            {
                var client = new Telegram.Bot.TelegramBotClient(token);
                var me = await client.GetMe();
                return me != null && !string.IsNullOrEmpty(me.Username);
            }
            catch
            {
                return false;
            }
        }
        private async Task<bool> ValidateWebAppUrlAsync(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            // Check if the URL is well-formed and absolute
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) ||
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                return false;
            }

            // Optional: Check if the URL is reachable
            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(3);
                var response = await httpClient.GetAsync(uriResult);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        async Task ModifyBotAsync()
        {
            await form.Validate();
            if (form.IsValid)
            {

                var results = await telegramBotServices.UpdateBotTokenAsync(model!);
                if (results.IsSuccess)
                {
                    SnackbarService.Add(results.Data!, MudBlazor.Severity.Success);
                }
                else
                    errors = new[] { results.ErrMessage };
            }
            
        }
    }
}