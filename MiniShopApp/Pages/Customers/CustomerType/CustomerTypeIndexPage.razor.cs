using Microsoft.AspNetCore.Components;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Customers;

namespace MiniShopApp.Pages.Customers.CustomerType
{
    public partial class CustomerTypeIndexPage(ICustomerTypeService customerTypeService)
    {

        private List<ViewCustomerType> customerTypes = new();
        protected CustomerTypeDtoCreate dtoCreate = new();
        protected CustomerTypeDtoUpdate dtoUpdate = new();
        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            await GetCustomerType();
            await base.OnInitializedAsync();
        }
        async Task GetCustomerType()
        {
            isLoading = true;
            var result = await customerTypeService.GetAllAsync();
            if (result != null && result.IsSuccess && result.Data != null)
            {
                customerTypes = result.Data.ToList();
            }
            isLoading = false;
        }
    }
}