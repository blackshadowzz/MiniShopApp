using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniShopApp.Models.Orders
{
    public class TbOrder : BaseEntity
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string? TableNumber { get; set; }
        public int? ItemCount { get; set; }
        public double? SubPrice { get; set; }
        public double? DiscountPrice { get; set; }
        public double? TotalPrice { get; set; }
        public string? Notes { get; set; }
        //Ad-on fields
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? OrderCode { get; set; }
        public double? TaxRate { get; set; }
        public string? OrderStatus { get; set; }
        public string? CustomerType { get; set; }
        public ICollection<TbOrderDetails>? TbOrderDetails { get; set; }
    }
    public class ViewTbOrders : BaseEntity
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? TableNumber { get; set; }
        public int? ItemCount { get; set; }
        public double? SubPrice { get; set; }
        public double? DiscountPrice { get; set; }
        public double? TotalPrice { get; set; }
        public string? Notes { get; set; }

        //Ad-on fields
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? OrderCode { get; set; }
        public double? TaxRate { get; set; }
        public string? OrderStatus { get; set; }
        public string? CustomerType { get; set; }

        public List<ViewTbOrderDetails>? TbOrderDetails { get; set; }=new List<ViewTbOrderDetails>();

    }
    public class OrderCreateModel : BaseEntity
    {
        
        public long CustomerId { get; set; }
        [Required( ErrorMessage = "Table number is required!")]
        public string? TableNumber { get; set; }
        public int? ItemCount { get; set; }
        public double? SubPrice { get; set; }
        public double? DiscountPrice { get; set; }
        public double? TotalPrice { get; set; }
        public string? Notes { get; set; }
        //Ad-on fields
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? OrderCode { get; set; }
        public double? TaxRate { get; set; }
        public string? OrderStatus { get; set; }=Statuses.Pending.ToString();
        public string? CustomerType { get; set; }

        public ICollection<TbOrderDetails>? TbOrderDetails { get; set; }
    }
    public class OrderUpdateModel : BaseEntity
    {

        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string? TableNumber { get; set; }
        public int? ItemCount { get; set; }
        public double? SubPrice { get; set; }
        public double? DiscountPrice { get; set; }
        public double? TotalPrice { get; set; }
        public string? Notes { get; set; }
        //Ad-on fields
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? OrderCode { get; set; }
        public double? TaxRate { get; set; }
        public string? OrderStatus { get; set; }
        public string? CustomerType { get; set; }
        public ICollection<TbOrderDetails>? TbOrderDetails { get; set; }
    }
    public enum Statuses
    {
        None = 0,
        Pending,
        Canceled,
        Paid

    }
    public class OrderStatusDto
    {
        public long Id { get; set; }
        public bool? IsActive { get; set; }
        public int? EditSeq {  get; set; }
        public Statuses? Statuses { get; set; }
    }

}