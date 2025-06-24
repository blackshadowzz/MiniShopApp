using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniShopApp.Migrations
{
    /// <inheritdoc />
    public partial class updateTelegramTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TbTelegramGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<long>(type: "bigint", nullable: true),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BotToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TelegramUserId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbTelegramGroup", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TbTelegramGroup");
        }
    }
}
