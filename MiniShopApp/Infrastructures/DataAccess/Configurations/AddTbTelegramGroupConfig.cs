using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniShopApp.Models.Orders;
using MiniShopApp.Models.Settings;

namespace MiniShopApp.Infrastructures.DataAccess.Configurations
{
    public class AddTbTelegramGroupConfig : IEntityTypeConfiguration<TbTelegramGroup>
    {
        public void Configure(EntityTypeBuilder<TbTelegramGroup> builder)
        {
            builder.ToTable("TbTelegramGroup");
            builder.HasKey(x=>x.Id);
            builder.Property(x=>x.Id).ValueGeneratedOnAdd();
            builder.Property(x=>x.IsActive).HasDefaultValue(true);
        }
    }
}
