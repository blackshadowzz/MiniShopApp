using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniShopApp.Models.Items
{
    public class Product
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public double? Price { get; set; }
        public double? SubPrice { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public int? CategoryId { get; set; }
    }
    public class ViewProducts
    {
        public int Id { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public double? Price { get; set; }
        public double? SubPrice { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public int? CategoryId { get; set; }
        public int? CategoryName { get; set; }
    }
    public class ProductCreateModel
    {
        [Required(ErrorMessage = "Product Code is required.")]
        public string? ProductCode { get; set; }
        [Required(ErrorMessage = "Product Name is required.")]
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public double Price { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public int? CategoryId { get; set; }
    }
    public class ProductUpdateModel : ProductCreateModel
    {
        [Required(ErrorMessage = "Product ID is required.")]
        public int Id { get; set; }
    }
}