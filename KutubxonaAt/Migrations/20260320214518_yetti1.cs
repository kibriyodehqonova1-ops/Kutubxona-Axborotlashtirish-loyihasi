using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KutubxonaAt.Migrations
{
    /// <inheritdoc />
    public partial class yetti1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FaylFormati",
                table: "KitobNusxalari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FaylHajmi",
                table: "KitobNusxalari",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaylManzili",
                table: "KitobNusxalari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "YuklabOlishMumkin",
                table: "KitobNusxalari",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MediaTuri",
                table: "Kitoblar",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaylFormati",
                table: "KitobNusxalari");

            migrationBuilder.DropColumn(
                name: "FaylHajmi",
                table: "KitobNusxalari");

            migrationBuilder.DropColumn(
                name: "FaylManzili",
                table: "KitobNusxalari");

            migrationBuilder.DropColumn(
                name: "YuklabOlishMumkin",
                table: "KitobNusxalari");

            migrationBuilder.DropColumn(
                name: "MediaTuri",
                table: "Kitoblar");
        }
    }
}
