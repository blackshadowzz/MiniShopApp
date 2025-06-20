using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniShopApp.Migrations
{
    /// <inheritdoc />
    public partial class M2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDT",
                table: "TbUserCustomers",
                type: "datetime",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLoginDT",
                table: "TbUserCustomers");
        }
    }
}
