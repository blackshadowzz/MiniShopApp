using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniShopApp.Models.Items
{
    public class Product
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public double? Price { get; set; }
        public string? Decription { get; set; }
    }
}
