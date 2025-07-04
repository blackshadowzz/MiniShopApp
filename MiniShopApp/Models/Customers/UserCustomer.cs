using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniShopApp.Models.Customers
{
    public class UserCustomer :BaseEntity
    {
        public int Id { get; set; }
        public long CustomerId { get; set; }
        public string? CustomerType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? phoneNumber { get; set; }
        public DateTime? loginDateTime { get; set; }
        public DateTime? LastLoginDT { get; set; }

        //Ad-on fields
        public string? EmailAddress { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool? IsLocked { get; set; }
        public bool? IsPremium { get; set; }
    }
    public class ViewUserCustomers :BaseEntity
    {
        public int Id { get; set; }
        public long CustomerId { get; set; }
        public string? CustomerType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? phoneNumber { get; set; }
        public DateTime? loginDateTime { get; set; }
        public DateTime? LastLoginDT { get; set; }

        //Ad-on fields
        public string? EmailAddress { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool? IsLocked { get; set; } //For telegram user
        public bool? IsPremium { get; set; }//For telegram user
    }
    public class CustomerAlertMessege
    {
        public int Id { get; set; }
        public long? CustomerId { get; set; }
        public string? CustomerType { get; set; }

        public string? FirstName { get; set; }
        public string? AlertMessege { get; set; }
    }
}
