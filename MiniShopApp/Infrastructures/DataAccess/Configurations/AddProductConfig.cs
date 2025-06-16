using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniShopApp.Models.Items;

namespace MiniShopApp.Infrastructures.DataAccess.Configurations
{
    public class AddProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("TbProducts");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.ProductName).HasColumnType("nvarchar(100)").IsRequired(false);
            builder.Property(x => x.ProductCode).HasColumnType("nvarchar(50)").IsRequired(false);
            builder.Property(x => x.Price).HasColumnType("decimal(18,2)").IsRequired(false);
            builder.Property(x => x.SubPrice).HasColumnType("decimal(18,2)").IsRequired(false);
            builder.Property(x => x.Description).HasColumnType("nvarchar(max)").IsRequired(false);
            builder.Property(x => x.ImageUrl).HasColumnType("text").IsRequired(false);
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.CategoryId).IsRequired(false);
        }
    }
}
