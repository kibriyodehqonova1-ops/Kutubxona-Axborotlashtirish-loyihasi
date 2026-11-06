using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KutubxonaAt.Migrations
{
    /// <inheritdoc />
    public partial class sotuv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SotuvgaQoyilgan",
                table: "KitobNusxalari",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Sotuvlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoydalanuvchiId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KitobId = table.Column<int>(type: "int", nullable: false),
                    SotibOlinganSana = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TolanganNarx = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TolovUsuli = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sotuvlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sotuvlar_AspNetUsers_FoydalanuvchiId",
                        column: x => x.FoydalanuvchiId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sotuvlar_Kitoblar_KitobId",
                        column: x => x.KitobId,
                        principalTable: "Kitoblar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sotuvlar_FoydalanuvchiId",
                table: "Sotuvlar",
                column: "FoydalanuvchiId");

            migrationBuilder.CreateIndex(
                name: "IX_Sotuvlar_KitobId",
                table: "Sotuvlar",
                column: "KitobId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sotuvlar");

            migrationBuilder.DropColumn(
                name: "SotuvgaQoyilgan",
                table: "KitobNusxalari");
        }
    }
}
