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
        public string? UserName { get; set; }
        public string? phoneNumber { get; set; }
        public DateTime? loginDateTime { get; set; }
        public DateTime? LastLoginDT { get; set; }
    }
    public class ViewUserCustomers
    {
        public int Id { get; set; }
        public long CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? phoneNumber { get; set; }
        public DateTime? loginDateTime { get; set; }
        public DateTime? LastLoginDT { get; set; }
    }
    public class CustomerAlertMessege
    {
        public int Id { get; set; }
        public long? CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? AlertMessege { get; set; }
    }
}
