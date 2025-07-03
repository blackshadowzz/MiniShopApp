using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniShopApp.Models.Orders
{
    public class TbOrderDetails
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public int? ItemId { get; set; }
        public string? ItemName { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public double? TotalPrice { get; set; }
        public TbOrder? TbOrder { get; set; }
    }
    public class ViewTbOrderDetails
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public int? ItemId { get; set; }
        public string? ItemName { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public double? TotalPrice { get; set; }
    }
}
