using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChurchWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOutreachCardFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OutreachMapLink",
                schema: "churchweb",
                table: "ChurchInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OutreachShortUrl",
                schema: "churchweb",
                table: "ChurchInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OutreachWelcomeMessage",
                schema: "churchweb",
                table: "ChurchInfos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutreachMapLink",
                schema: "churchweb",
                table: "ChurchInfos");

            migrationBuilder.DropColumn(
                name: "OutreachShortUrl",
                schema: "churchweb",
                table: "ChurchInfos");

            migrationBuilder.DropColumn(
                name: "OutreachWelcomeMessage",
                schema: "churchweb",
                table: "ChurchInfos");
        }
    }
}
