using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniShopApp.Models.Orders
{
    public class TbOrderDetails
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long OrderId { get; set; }
        public int? ItemId { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string? ItemName { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public double? TotalPrice { get; set; }
        public TbOrder? TbOrder { get; set; }
    }
}
