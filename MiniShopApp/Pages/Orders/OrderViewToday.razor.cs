using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Orders;

namespace MiniShopApp.Pages.Orders
{
    public partial class OrderViewToday(IOrderService orderService)
    {
        List<ViewTbOrders> orders = new List<ViewTbOrders>();
        protected DateTime today = DateTime.Now.Date;
        string? filter = string.Empty;

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

        async Task GetOrderToday()
        {
            try
            {
                var results = await orderService.GetOrderByDateAsync(filter, today, 0);
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
                var results = await orderService.GetOrderByDateAsync(filter, dt, 0);
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
                    var results = await orderService.GetOrderByDateAsync(filter, date, 0);
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
                    var results = await orderService.GetOrderByDateAsync(filter, date, 0);
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
    }
}