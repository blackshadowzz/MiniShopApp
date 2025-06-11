using MiniShopApp.Models.Items;

namespace MiniShopApp.Pages
{
    public partial class Index
    {
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