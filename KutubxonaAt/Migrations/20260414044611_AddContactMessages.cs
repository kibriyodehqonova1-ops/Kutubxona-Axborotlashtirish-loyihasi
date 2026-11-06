using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KutubxonaAt.Migrations
{
    /// <inheritdoc />
    public partial class AddContactMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ism = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Familiya = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Xabar = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    YuborilganSana = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Oqilgan = table.Column<bool>(type: "bit", nullable: false),
                    OqilganSana = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdminJavobi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JavobYozilganSana = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactMessages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_UserId",
                table: "ContactMessages",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactMessages");
        }
    }
}
