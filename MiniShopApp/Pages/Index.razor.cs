using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MiniShopApp.Models.Items;

namespace MiniShopApp.Pages
{
    public partial class Index
    {
        [Inject] public ProtectedSessionStorage SessionStorage { get; set; } = default!;
        private readonly ProtectedLocalStorage localStorage;
        public Index(ProtectedLocalStorage localStorage)
        {
            this.localStorage = localStorage;
        }
        long? userId = null;
        protected List<Product> products = new List<Product>();
       

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (userId == null)
                {
                    // Simulate fetching user ID from a service or storage
                    userId = userState.UserId; // Replace with actual user ID retrieval logic
                    await SessionStorage.SetAsync("customerId", userId.ToString()!);
                }
                //products = (await productService.GetAllAsync()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            await base.OnInitializedAsync();
        }
    }
}