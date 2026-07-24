using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChurchWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChurchInfoPhase1Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnnualSlogan",
                schema: "churchweb",
                table: "ChurchInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OutreachCardImageUrl",
                schema: "churchweb",
                table: "ChurchInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OutreachCardPdfUrl",
                schema: "churchweb",
                table: "ChurchInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PracticesJson",
                schema: "churchweb",
                table: "ChurchInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PromoVideoUrl",
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
                name: "AnnualSlogan",
                schema: "churchweb",
                table: "ChurchInfos");

            migrationBuilder.DropColumn(
                name: "OutreachCardImageUrl",
                schema: "churchweb",
                table: "ChurchInfos");

            migrationBuilder.DropColumn(
                name: "OutreachCardPdfUrl",
                schema: "churchweb",
                table: "ChurchInfos");

            migrationBuilder.DropColumn(
                name: "PracticesJson",
                schema: "churchweb",
                table: "ChurchInfos");

            migrationBuilder.DropColumn(
                name: "PromoVideoUrl",
                schema: "churchweb",
                table: "ChurchInfos");
        }
    }
}
