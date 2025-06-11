using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniShopApp.Models
{
    public class Category
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "bit"),DefaultValue(true)]
        public bool? IsActive { get; set; }
    }
}
