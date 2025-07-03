using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Orders;
using MiniShopApp.Shared.AdditionalServices;
using System.Buffers.Text;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace MiniShopApp.Pages.Orders
{
    public partial class OrderViewToday(IOrderService orderService)
    {
        [Inject] IJSRuntime JS { get; set; } = default!;
        List<ViewTbOrders> orders = new List<ViewTbOrders>();
        protected DateTime today = DateTime.Now.Date;
        //string? filter = string.Empty;

        // Month/Year selection for Months tab
        protected int selectedMonth = DateTime.Now.Month;
        protected int selectedYear = DateTime.Now.Year;
        protected List<int> months = Enumerable.Range(1, 12).ToList();
        protected List<int> years = new();

        // Sum of total price for orders (for table footer)
        protected decimal TotalPriceSum => orders?.Sum(o => (decimal)(o.TotalPrice ?? 0)) ?? 0;

        protected override async Task OnInitializedAsync()
        {
            await LoadYearsFromOrders();
            await GetOrderToday();
            await base.OnInitializedAsync();
        }

        private async Task LoadYearsFromOrders()
        {
            var result = await orderService.GetOrderSummaryAsync();
            if (result.IsSuccess && result.Data != null && result.Data.Any())
            {
                var minYear = result.Data.Min(o => o.CreatedDT?.Year ?? DateTime.Now.Year);
                var maxYear = result.Data.Max(o => o.CreatedDT?.Year ?? DateTime.Now.Year);
                years = Enumerable.Range(minYear, maxYear - minYear + 1).ToList();
                if (!years.Contains(selectedYear))
                    selectedYear = years.Last();
            }
            else
            {
                years = new List<int> { DateTime.Now.Year };
            }
        }
        async Task ConfirmPayment(long Id)
        {
            try
            {
                var confirm = await DialogService.ShowMessageBox(
                    "Confirmation",
                    $"Modified confirm payment?",
                    "Yes", "No");
                if (confirm == true)
                {
                    var result = await orderService.ModifiedStatusAsync(Id, Statuses.Paid, default);
                    if (result.IsSuccess)
                    {
                        SnackbarService.Add(result.Data!, MudBlazor.Severity.Success);
                        await GetOrderToday();
                        StateHasChanged();
                        return;
                    }
                    SnackbarService.Add(result.ErrMessage, MudBlazor.Severity.Error);
                }
                
            }
            catch (Exception ex)
            {
                
                SnackbarService.Add(ex.Message, MudBlazor.Severity.Error);
                throw new Exception(ex.InnerException!.Message, ex);
            }
        }
        async Task ConfirmPaidBulk(List<ViewTbOrders> viewTbOrders)
        {
            try
            {
                var confirm = await DialogService.ShowMessageBox(
                    "Confirmation",
                    $"Modified confirm payment?",
                    "Yes", "No");
                if (confirm == true)
                {
                    var result = await orderService.ModifiedStatusAsync(viewTbOrders.Select(o => o.Id).ToList(), Statuses.Paid, default);
                    if (result.IsSuccess)
                    {
                        SnackbarService.Add(result.Data!, MudBlazor.Severity.Success);
                        await GetOrderToday();
                        clearData();
                        StateHasChanged();
                        return;
                    }
                    SnackbarService.Add(result.ErrMessage, MudBlazor.Severity.Error);
                }
                
            }
            catch (Exception ex)
            {
                
                SnackbarService.Add(ex.Message, MudBlazor.Severity.Error);
                throw new Exception(ex.InnerException!.Message, ex);
            }
        }
        async Task ConfirmCancelBulk(List<ViewTbOrders> viewTbOrders)
        {
            try
            {
                var confirm = await DialogService.ShowMessageBox(
                    "Confirmation",
                    $"Modified confirm payment?",
                    "Yes", "No");
                if (confirm == true)
                {
                    var result = await orderService.ModifiedStatusAsync(viewTbOrders.Select(o => o.Id).ToList(), Statuses.Canceled, default);
                    if (result.IsSuccess)
                    {
                        SnackbarService.Add(result.Data!, MudBlazor.Severity.Success);
                        await GetOrderToday();
                        clearData();

                        StateHasChanged();
                        return;
                    }
                    SnackbarService.Add(result.ErrMessage, MudBlazor.Severity.Error);
                }

            }
            catch (Exception ex)
            {

                SnackbarService.Add(ex.Message, MudBlazor.Severity.Error);
                throw new Exception(ex.InnerException!.Message, ex);
            }
        }
        async Task ConfirmPaymentCancel (long Id)
        {
            try
            {
                var confirm = await DialogService.ShowMessageBox(
                   "Confirmation",
                   $"Modified confirm cancel order?",
                   "Yes", "No");
                if (confirm == true)
                {
                    var result = await orderService.ModifiedStatusAsync(Id, Statuses.Canceled, default);
                    if (result.IsSuccess)
                    {
                        SnackbarService.Add(result.Data!, MudBlazor.Severity.Success);
                        await GetOrderToday();
                        StateHasChanged();
                        return;
                    }
                    SnackbarService.Add(result.ErrMessage, MudBlazor.Severity.Error);
                }
            }
            catch (Exception ex)
            {

                SnackbarService.Add(ex.Message, MudBlazor.Severity.Error);
                throw new Exception(ex.InnerException!.Message, ex);
            }
        }
        async Task GetOrderToday()
        {
            try
            {
                var results = await orderService.GetOrderByDateAsync(searchString, today, 0);
                if (results.IsSuccess)
                {
                    orders = results.Data!.OrderByDescending(x => x.Id).ToList();
                }
            }
            catch (Exception ex)
            {


            }
        }
        async Task GetOrderYesterday()
        {
            try
            {

                var dt = today.AddDays(-1);
                var results = await orderService.GetOrderByDateAsync(searchString, dt, 0);
                if (results.IsSuccess)
                {
                    orders = results.Data!.OrderByDescending(x => x.Id).ToList();
                }
            }
            catch (Exception ex)
            {


            }
        }
        async Task GetOrderWeek()
        {
            try
            {
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
                var endOfWeek = startOfWeek.AddDays(6);

                var weekOrders = new List<ViewTbOrders>();
                for (var date = startOfWeek; date <= endOfWeek; date = date.AddDays(1))
                {
                    var results = await orderService.GetOrderByDateAsync(searchString, date, 0);
                    if (results.IsSuccess && results.Data != null)
                    {
                        weekOrders.AddRange(results.Data);
                    }
                }
                orders = weekOrders.OrderByDescending(x => x.Id).ToList();
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }
        async Task GetOrderMonth()
        {
            try
            {
                var firstDayOfMonth = new DateTime(selectedYear, selectedMonth, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                var monthOrders = new List<ViewTbOrders>();
                for (var date = firstDayOfMonth; date <= lastDayOfMonth; date = date.AddDays(1))
                {
                    var results = await orderService.GetOrderByDateAsync(searchString, date, 0);
                    if (results.IsSuccess && results.Data != null)
                    {
                        monthOrders.AddRange(results.Data);
                    }
                }
                orders = monthOrders.OrderByDescending(x => x.Id).ToList();
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }

        private async Task GenerateReceiptAsync(IEnumerable<ViewTbOrders> orders)
        {
            if(!orders.Any())
                return;
            foreach (var order in orders)
            {

                var pdfBytes = pdfService.CreateOrderReceiptPdf(order);
                var base64 = Convert.ToBase64String(pdfBytes);
                await JS.InvokeVoidAsync("DownloadReceipt", $"receipt_{order.Id}.pdf", "application/pdf", base64);
            }
        }

        private string BuildReceiptHtml(IEnumerable<ViewTbOrders> orders)
        {
            var html = new System.Text.StringBuilder();
            html.AppendLine("<html><head><meta charset='UTF-8'><title>Receipt</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; }");
            html.AppendLine("table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }");
            html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            html.AppendLine("th { background-color: #f2f2f2; }");
            html.AppendLine("</style></head><body>");
            html.AppendLine("<h2>Order Receipt</h2>");
            foreach (var order in orders)
            {
                html.AppendLine($"<h3>Order #{order.Id} - {order.FirstName} {order.LastName}</h3>");
                html.AppendLine($"<p>Date: {order.CreatedDT?.ToString("dd/MMM/yy hh:mm tt")}</p>");
                html.AppendLine($"<p>Table: {order.TableNumber}</p>");
                html.AppendLine("<table>");
                html.AppendLine("<tr><th>Item</th><th>Qty</th><th>Price</th><th>Total</th></tr>");
                if (order.TbOrderDetails != null)
                {
                    foreach (var detail in order.TbOrderDetails)
                    {
                        html.AppendLine($"<tr><td>{detail.ItemName}</td><td>{detail.Quantity}</td><td>{detail.Price?.ToString("c2")}</td><td>{detail.TotalPrice?.ToString("c2")}</td></tr>");
                    }
                }
                html.AppendLine("</table>");
                html.AppendLine($"<p><b>Total: {order.TotalPrice?.ToString("c2")}</b></p>");
                html.AppendLine("<hr/>");
            }
            html.AppendLine("</body></html>");
            return html.ToString();
        }
        private string? receiptHtml;

        async Task print(IEnumerable<ViewTbOrders> orders)
        {
            receiptHtml = BuildReceiptHtml(orders);
            await Task.Yield(); // ensures UI updates
        }

        private async Task TriggerPrint()
        {
            await JS.InvokeVoidAsync("window.print");
        }

        private MarkupString GetReceiptMarkup()
        {
            return new MarkupString(receiptHtml); // Trusts the HTML as-is
        }

    }
}