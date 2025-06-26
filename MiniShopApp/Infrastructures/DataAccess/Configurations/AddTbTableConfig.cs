using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniShopApp.Models.Items;

namespace MiniShopApp.Infrastructures.DataAccess.Configurations
{
    public class AddTbTableConfig : IEntityTypeConfiguration<TbTable>
    {
        public void Configure(EntityTypeBuilder<TbTable> builder)
        {
            builder.ToTable("TbTables");
            builder.HasKey(x => x.TableId);
            builder.Property(x => x.TableId).ValueGeneratedOnAdd();
            builder.Property(x => x.TableNumber).HasColumnType("nvarchar(30)").IsRequired(false);
            builder.Property(x => x.Description).HasColumnType("nvarchar(100)").IsRequired(false);
            builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired(false);
            builder.Property(x => x.CreatedDT).HasDefaultValueSql("getdate()").IsRequired(false);
            builder.Property(x => x.ModifiedDT).HasDefaultValueSql("getdate()").IsRequired(false);
        }
    }
   
}
