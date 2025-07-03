using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniShopApp.Models.Customers;

namespace MiniShopApp.Infrastructures.DataAccess.Configurations
{
    public class AddCustomerTypeConfig : IEntityTypeConfiguration<CustomerType>
    {
        public void Configure(EntityTypeBuilder<CustomerType> builder)
        {
            builder.ToTable("TbCustomerType");
            builder.HasKey(x => x.Id);
            builder.Property(x=>x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.TypeName)
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)")
                .IsRequired(false);

            builder.Property(x => x.DiscountRate)
                .HasColumnType("decimal(18,2)")
                .IsRequired(false);

            builder.Property(x => x.Description)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            // BaseEntity properties
            builder.Property(x => x.EditSeq)
                .IsRequired(false)
                .HasDefaultValue(0);

            builder.Property(x => x.IsActive)
                .IsRequired(false)
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedDT)
                .IsRequired(false)
                .HasDefaultValueSql("getdate()");

            builder.Property(x => x.ModifiedDT)
                .IsRequired(false).HasDefaultValueSql("getdate()");

            builder.Property(x => x.CreatedBy)
                .HasColumnType("nvarchar(100)")
                .IsRequired(false);

            builder.Property(x => x.ModifiedBy)
                .HasColumnType("nvarchar(100)")
                .IsRequired(false);
        }
    }
}
