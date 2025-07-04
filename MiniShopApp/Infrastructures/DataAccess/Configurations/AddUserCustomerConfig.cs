using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniShopApp.Models.Customers;

namespace MiniShopApp.Infrastructures.DataAccess.Configurations
{
    public class AddUserCustomerConfig : IEntityTypeConfiguration<UserCustomer>
    {
        public void Configure(EntityTypeBuilder<UserCustomer> builder)
        {
            builder.ToTable("TbUserCustomers");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.CustomerId).IsRequired(true);
            builder.Property(x => x.CustomerType).HasColumnType("nvarchar(100)").IsRequired(false);
            builder.Property(x => x.FirstName).HasColumnType("nvarchar(50)").IsRequired(false);
            builder.Property(x => x.LastName).HasColumnType("nvarchar(50)").IsRequired(false);
            builder.Property(x => x.UserName).HasColumnType("nvarchar(100)").IsRequired(false);
            builder.Property(x => x.phoneNumber).HasColumnType("nvarchar(20)").IsRequired(false);
            builder.Property(x => x.loginDateTime).HasColumnType("datetime");
            builder.Property(x => x.LastLoginDT).HasColumnType("datetime");

            //Ad-on fields
            builder.Property(x => x.CreatedDT).HasColumnType("datetime").HasDefaultValueSql("getdate()");
            builder.Property(x => x.ModifiedDT).HasColumnType("datetime").HasDefaultValueSql("getdate()");
            builder.Property(x => x.CreatedBy).HasColumnType("nvarchar(100)").IsRequired(false);
            builder.Property(x => x.ModifiedBy).HasColumnType("nvarchar(100)").IsRequired(false);
            builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired(false);
            builder.Property(x => x.EditSeq).HasDefaultValue(0).IsRequired(false);
            builder.Property(x => x.IsLocked).HasDefaultValue(false).IsRequired(false);
            builder.Property(x => x.IsPremium).HasDefaultValue(false).IsRequired(false);
            builder.Property(x => x.EmailAddress).HasColumnType("nvarchar(100)").IsRequired(false);
            builder.Property(x => x.Address).IsRequired(false);


        }
    }
}
