using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniShopApp.Models.Orders
{
    public class TbOrder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long CustomerId { get; set; }
        [Column(TypeName = "nvarchar(30)")]
        public string? TableNumber { get; set; }
        public int? ItemCount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public double? SubPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public double? DiscountPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public double? TotalPrice { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedDT { get; set; }
        public ICollection<TbOrderDetails>? TbOrderDetails { get; set; }
    }
}
