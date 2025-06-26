using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniShopApp.Models.Orders;

namespace MiniShopApp.Infrastructures.DataAccess.Configurations
{
    public class AddOrderConfig : IEntityTypeConfiguration<TbOrder>
    {
        public void Configure(EntityTypeBuilder<TbOrder> builder)
        {
            builder.ToTable("TbOrders");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.CustomerId).IsRequired(true);
            builder.Property(x => x.TableNumber).HasColumnType("nvarchar(30)").IsRequired(false);
            builder.Property(x => x.ItemCount).IsRequired(false);
            builder.Property(x => x.SubPrice).HasColumnType("decimal(18,2)").IsRequired(false);
            builder.Property(x => x.DiscountPrice).HasColumnType("decimal(18,2)").IsRequired(false);
            builder.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)").IsRequired(false);
            builder.Property(x => x.Notes).HasColumnType("nvarchar(max)").IsRequired(false);
            builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired(false);
            builder.Property(x => x.EditSeq).HasDefaultValue(0).IsRequired(false);

            builder.Property(x => x.CreatedDT).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            builder.Property(x => x.ModifiedDT).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");

            builder.HasMany(x => x.TbOrderDetails)
                   .WithOne(x=>x.TbOrder)
                   .HasForeignKey(od => od.OrderId);
                  
        }
    }
    
    
}
