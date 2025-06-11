using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniShopApp.Models.Items
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public double? Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public double? SubPrice { get; set; }
        public string? Decription { get; set; }
        [Column(TypeName = "text")]
        public string? ImageUrl { get; set; }
        [Column(TypeName = ("bit")),DefaultValue(true)]
        public bool IsActive { get; set; }
        public int? CatgoryId { get; set; }
    }
}
