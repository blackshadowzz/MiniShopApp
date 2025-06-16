using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniShopApp.Models.Items
{
    public class TbTable
    {

        public int TableId { get; set; }
        public string? TableNumber { get; set; }
        public string? Description { get; set; }
      
    }
}
