using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChurchWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBulletinPdfStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                schema: "churchweb",
                table: "Bulletins",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                schema: "churchweb",
                table: "Bulletins",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                schema: "churchweb",
                table: "Bulletins",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<byte[]>(
                name: "PdfData",
                schema: "churchweb",
                table: "Bulletins",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                schema: "churchweb",
                table: "Bulletins");

            migrationBuilder.DropColumn(
                name: "FileName",
                schema: "churchweb",
                table: "Bulletins");

            migrationBuilder.DropColumn(
                name: "FileSize",
                schema: "churchweb",
                table: "Bulletins");

            migrationBuilder.DropColumn(
                name: "PdfData",
                schema: "churchweb",
                table: "Bulletins");
        }
    }
}
