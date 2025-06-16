using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniShopApp.Models
{
    public class UserCustomer
    {
        public int Id { get; set; }
        public long CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? phoneNumber { get; set; }
        public DateTime? loginDateTime { get; set; }
    }
}
