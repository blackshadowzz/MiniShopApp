using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniShopApp.Models.Items
{
    public class TbTable
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TableId { get; set; }
        [Column(TypeName = "nvarchar(30)")]
        public string? TableNumber { get; set; }
        [Column(TypeName = "nvarchar(100)")]

        public string? Description { get; set; }
      
    }
}
