using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniShopApp.Models.Settings;

namespace MiniShopApp.Infrastructures.DataAccess.Configurations
{
    public class AddTbTelegramBotTokenConfig : IEntityTypeConfiguration<TbTelegramBotToken>
    {
        public void Configure(EntityTypeBuilder<TbTelegramBotToken> builder)
        {
            builder.ToTable("TbTelegramBotToken");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.CreatedDT).HasDefaultValueSql("getdate()");
            builder.Property(x => x.ModifiedDT).HasDefaultValueSql("getdate()");
        }
    }
}
