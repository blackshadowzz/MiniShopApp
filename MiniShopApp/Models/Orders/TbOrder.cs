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
}
