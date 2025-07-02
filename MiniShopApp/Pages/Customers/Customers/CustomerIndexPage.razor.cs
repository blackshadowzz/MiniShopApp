using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Customers;

namespace MiniShopApp.Pages.Customers.Customers
{
    public partial class CustomerIndexPage(IUserCustomerService customerService)
    {
        private List<ViewUserCustomers> customers = new();
        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            var result = await customerService.GetAllAsync(null);
            if (result.IsSuccess && result.Data is not null)
                customers = result.Data.OrderByDescending(x=>x.Id).ToList();
            isLoading = false;
            await base.OnInitializedAsync();
        }
    }
}