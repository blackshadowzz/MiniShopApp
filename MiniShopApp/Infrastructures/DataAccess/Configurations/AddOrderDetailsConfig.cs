using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniShopApp.Models.Orders;

namespace MiniShopApp.Infrastructures.DataAccess.Configurations
{
    public class AddOrderDetailsConfig : IEntityTypeConfiguration<TbOrderDetails>
    {
        public void Configure(EntityTypeBuilder<TbOrderDetails> builder)
        {
            builder.ToTable("TTbOrderDetails");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.OrderId);
            builder.Property(x => x.ItemId);
            builder.Property(x => x.ItemName).HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.Quantity).HasColumnType("int").IsRequired(false);
            builder.Property(x => x.Price).HasColumnType("decimal(18,2)").IsRequired(false);
            builder.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)").IsRequired(false);
        }
    }
    
    
}
