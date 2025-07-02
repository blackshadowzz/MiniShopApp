using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniShopApp.Migrations
{
    /// <inheritdoc />
    public partial class updateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TbOrderDetails_TbOrders_OrderId",
                table: "TbOrderDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbOrders",
                table: "TbOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbOrderDetails",
                table: "TbOrderDetails");

            migrationBuilder.RenameTable(
                name: "TbOrders",
                newName: "TTbOrders");

            migrationBuilder.RenameTable(
                name: "TbOrderDetails",
                newName: "TTbOrderDetails");

            migrationBuilder.RenameIndex(
                name: "IX_TbOrderDetails_OrderId",
                table: "TTbOrderDetails",
                newName: "IX_TTbOrderDetails_OrderId");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TbTables",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDT",
                table: "TbTables",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "getdate()");

            migrationBuilder.AddColumn<int>(
                name: "EditSeq",
                table: "TbTables",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TbTables",
                type: "bit",
                nullable: true,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "TbTables",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDT",
                table: "TbTables",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "getdate()");

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "TbProducts",
                type: "nvarchar(100)",
                nullable: true,
                collation: "Khmer_100_CI_AI_SC_UTF8",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductCode",
                table: "TbProducts",
                type: "nvarchar(50)",
                nullable: true,
                collation: "Khmer_100_CI_AI_SC_UTF8",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "TbProducts",
                type: "bit",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TbProducts",
                type: "nvarchar(max)",
                nullable: true,
                collation: "Khmer_100_CI_AI_SC_UTF8",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TbProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDT",
                table: "TbProducts",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "getdate()");

            migrationBuilder.AddColumn<int>(
                name: "EditSeq",
                table: "TbProducts",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "TbProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDT",
                table: "TbProducts",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "getdate()");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "TbCategories",
                type: "bit",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "TbCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TbCategories",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "getdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDT",
                table: "TbCategories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EditSeq",
                table: "TbCategories",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "TbCategories",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "getdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDT",
                table: "TbCategories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TTbOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EditSeq",
                table: "TTbOrders",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TTbOrders",
                type: "bit",
                nullable: true,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "TTbOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDT",
                table: "TTbOrders",
                type: "datetime",
                nullable: true,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TTbOrders",
                table: "TTbOrders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TTbOrderDetails",
                table: "TTbOrderDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TTbOrderDetails_TTbOrders_OrderId",
                table: "TTbOrderDetails",
                column: "OrderId",
                principalTable: "TTbOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TTbOrderDetails_TTbOrders_OrderId",
                table: "TTbOrderDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TTbOrders",
                table: "TTbOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TTbOrderDetails",
                table: "TTbOrderDetails");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TbTables");

            migrationBuilder.DropColumn(
                name: "CreatedDT",
                table: "TbTables");

            migrationBuilder.DropColumn(
                name: "EditSeq",
                table: "TbTables");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TbTables");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "TbTables");

            migrationBuilder.DropColumn(
                name: "ModifiedDT",
                table: "TbTables");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TbProducts");

            migrationBuilder.DropColumn(
                name: "CreatedDT",
                table: "TbProducts");

            migrationBuilder.DropColumn(
                name: "EditSeq",
                table: "TbProducts");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "TbProducts");

            migrationBuilder.DropColumn(
                name: "ModifiedDT",
                table: "TbProducts");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TbCategories");

            migrationBuilder.DropColumn(
                name: "CreatedDT",
                table: "TbCategories");

            migrationBuilder.DropColumn(
                name: "EditSeq",
                table: "TbCategories");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "TbCategories");

            migrationBuilder.DropColumn(
                name: "ModifiedDT",
                table: "TbCategories");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TTbOrders");

            migrationBuilder.DropColumn(
                name: "EditSeq",
                table: "TTbOrders");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TTbOrders");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "TTbOrders");

            migrationBuilder.DropColumn(
                name: "ModifiedDT",
                table: "TTbOrders");

            migrationBuilder.RenameTable(
                name: "TTbOrders",
                newName: "TbOrders");

            migrationBuilder.RenameTable(
                name: "TTbOrderDetails",
                newName: "TbOrderDetails");

            migrationBuilder.RenameIndex(
                name: "IX_TTbOrderDetails_OrderId",
                table: "TbOrderDetails",
                newName: "IX_TbOrderDetails_OrderId");

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "TbProducts",
                type: "nvarchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldNullable: true,
                oldCollation: "Khmer_100_CI_AI_SC_UTF8");

            migrationBuilder.AlterColumn<string>(
                name: "ProductCode",
                table: "TbProducts",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true,
                oldCollation: "Khmer_100_CI_AI_SC_UTF8");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "TbProducts",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TbProducts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldCollation: "Khmer_100_CI_AI_SC_UTF8");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "TbCategories",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "TbCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbOrders",
                table: "TbOrders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbOrderDetails",
                table: "TbOrderDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TbOrderDetails_TbOrders_OrderId",
                table: "TbOrderDetails",
                column: "OrderId",
                principalTable: "TbOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
