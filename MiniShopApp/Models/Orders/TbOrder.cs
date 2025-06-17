using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniShopApp.Models.Orders
{
    public class TbOrder
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string? TableNumber { get; set; }
        public int? ItemCount { get; set; }
        public double? SubPrice { get; set; }
        public double? DiscountPrice { get; set; }
        public double? TotalPrice { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedDT { get; set; }
        public ICollection<TbOrderDetails>? TbOrderDetails { get; set; }
    }
    public class ViewTbOrders
    {
        public long Id { get; set; }
        public string? CategoryName { get; set; }
        public string? ItemName { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public double? TotalPrice { get; set; }
        public DateTime? CreatedDT { get; set; }
    }
    public class OrderCreateModel
    {
        public long CustomerId { get; set; }
        [Required( ErrorMessage = "Table number is required!")]
        public string? TableNumber { get; set; }
        public int? ItemCount { get; set; }
        public double? SubPrice { get; set; }
        public double? DiscountPrice { get; set; }
        public double? TotalPrice { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedDT { get; set; }
        public ICollection<TbOrderDetails>? TbOrderDetails { get; set; }
    }
}