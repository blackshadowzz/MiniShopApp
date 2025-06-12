using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniShopApp.Models.Orders;

namespace MiniShopApp.Infrastructures.DataAccess
{
    public class AddOrderConfig : IEntityTypeConfiguration<TbOrder>
    {
        public void Configure(EntityTypeBuilder<TbOrder> builder)
        {
            builder.ToTable("TbOrders");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDT).HasDefaultValue(DateTime.Now);
        }
    }
    
    
}
