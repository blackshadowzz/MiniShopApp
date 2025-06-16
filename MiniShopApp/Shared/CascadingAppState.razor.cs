using Microsoft.AspNetCore.Components;

namespace MiniShopApp.Shared
{
    public partial class CascadingAppState: ComponentBase
    {
        [Parameter] public RenderFragment ChildContent { get; set; } = default!;

    }
}