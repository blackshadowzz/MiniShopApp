using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniShopApp.Migrations
{
    /// <inheritdoc />
    public partial class updateUserCust : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "TbUserCustomers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TbUserCustomers",
                type: "nvarchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDT",
                table: "TbUserCustomers",
                type: "datetime",
                nullable: true,
                defaultValueSql: "getdate()");

            migrationBuilder.AddColumn<int>(
                name: "EditSeq",
                table: "TbUserCustomers",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "TbUserCustomers",
                type: "nvarchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TbUserCustomers",
                type: "bit",
                nullable: true,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "TbUserCustomers",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPremium",
                table: "TbUserCustomers",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "TbUserCustomers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "TbUserCustomers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "TbUserCustomers",
                type: "nvarchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDT",
                table: "TbUserCustomers",
                type: "datetime",
                nullable: true,
                defaultValueSql: "getdate()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "TbUserCustomers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TbUserCustomers");

            migrationBuilder.DropColumn(
                name: "CreatedDT",
                table: "TbUserCustomers");

            migrationBuilder.DropColumn(
                name: "EditSeq",
                table: "TbUserCustomers");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "TbUserCustomers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TbUserCustomers");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "TbUserCustomers");

            migrationBuilder.DropColumn(
                name: "IsPremium",
                table: "TbUserCustomers");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "TbUserCustomers");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "TbUserCustomers");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "TbUserCustomers");

            migrationBuilder.DropColumn(
                name: "ModifiedDT",
                table: "TbUserCustomers");
        }
    }
}
