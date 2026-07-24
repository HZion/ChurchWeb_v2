using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChurchWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMapCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                schema: "churchweb",
                table: "ChurchInfos",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                schema: "churchweb",
                table: "ChurchInfos",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                schema: "churchweb",
                table: "ChurchInfos");

            migrationBuilder.DropColumn(
                name: "Longitude",
                schema: "churchweb",
                table: "ChurchInfos");
        }
    }
}
