using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniShopApp.Migrations
{
    /// <inheritdoc />
    public partial class updateOrder3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrderCode",
                table: "TTbOrders",
                type: "nvarchar(100)",
                nullable: true,
                computedColumnSql: "FORMAT(GETDATE(), 'ddMMyy')+ CAST([Id] AS nvarchar)",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrderCode",
                table: "TTbOrders",
                type: "nvarchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldNullable: true,
                oldComputedColumnSql: "FORMAT(GETDATE(), 'ddMMyy')+ CAST([Id] AS nvarchar)");
        }
    }
}
