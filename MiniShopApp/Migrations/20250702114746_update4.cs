using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniShopApp.Migrations
{
    /// <inheritdoc />
    public partial class update4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrderCode",
                table: "TTbOrders",
                type: "nvarchar(100)",
                nullable: true,
                computedColumnSql: "'OR'+ FORMAT(GETDATE(), 'ddMMyy')+ CAST([Id] AS nvarchar)",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldNullable: true,
                oldComputedColumnSql: "FORMAT(GETDATE(), 'ddMMyy')+ CAST([Id] AS nvarchar)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrderCode",
                table: "TTbOrders",
                type: "nvarchar(100)",
                nullable: true,
                computedColumnSql: "FORMAT(GETDATE(), 'ddMMyy')+ CAST([Id] AS nvarchar)",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldNullable: true,
                oldComputedColumnSql: "'OR'+ FORMAT(GETDATE(), 'ddMMyy')+ CAST([Id] AS nvarchar)");
        }
    }
}
