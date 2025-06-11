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
            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "TbProducts");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "TbProducts",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CatgoryId",
                table: "TbProducts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "TbProducts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TbProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "SubPrice",
                table: "TbProducts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbProducts",
                table: "TbProducts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TbCategories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbCategories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "TbOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    TableNumber = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    ItemCount = table.Column<int>(type: "int", nullable: true),
                    SubPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDT = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbTables",
                columns: table => new
                {
                    TableId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableNumber = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbTables", x => x.TableId);
                });

            migrationBuilder.CreateTable(
                name: "TbUserCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    loginDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbUserCustomers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbOrderDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbOrderDetails_TbOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "TbOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TbOrderDetails_OrderId",
                table: "TbOrderDetails",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TbCategories");

            migrationBuilder.DropTable(
                name: "TbOrderDetails");

            migrationBuilder.DropTable(
                name: "TbTables");

            migrationBuilder.DropTable(
                name: "TbUserCustomers");

            migrationBuilder.DropTable(
                name: "TbOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbProducts",
                table: "TbProducts");

            migrationBuilder.DropColumn(
                name: "CatgoryId",
                table: "TbProducts");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "TbProducts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TbProducts");

            migrationBuilder.DropColumn(
                name: "SubPrice",
                table: "TbProducts");

            migrationBuilder.RenameTable(
                name: "TbProducts",
                newName: "Products");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Products",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");
        }
    }
}
