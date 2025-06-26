using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniShopApp.Models.Items;

namespace MiniShopApp.Infrastructures.DataAccess.Configurations
{
    public class AddCategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("TbCategories");
            builder.HasKey(x=>x.CategoryId);
            builder.Property(x => x.CategoryId).ValueGeneratedOnAdd();
            builder.Property(x => x.CategoryName).IsRequired(true);
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.EditSeq).HasDefaultValue(0);
            builder.Property(x => x.CreatedBy).HasDefaultValueSql("getdate()");
            builder.Property(x => x.ModifiedBy).HasDefaultValueSql("getdate()");
        }
    }
}
