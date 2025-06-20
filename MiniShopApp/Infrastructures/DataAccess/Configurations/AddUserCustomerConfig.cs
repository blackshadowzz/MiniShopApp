using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniShopApp.Models;

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
            builder.Property(x => x.FirstName).HasColumnType("nvarchar(50)").IsRequired(false);
            builder.Property(x => x.LastName).HasColumnType("nvarchar(50)").IsRequired(false);
            builder.Property(x => x.UserName).HasColumnType("nvarchar(100)").IsRequired(false);
            builder.Property(x => x.phoneNumber).HasColumnType("nvarchar(20)").IsRequired(false);
            builder.Property(x => x.loginDateTime).HasColumnType("datetime");
            builder.Property(x => x.LastLoginDT).HasColumnType("datetime");

        }
    }
}
