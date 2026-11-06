using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KutubxonaAt.Migrations
{
    /// <inheritdoc />
    public partial class FixUserForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Beruvlar_Foydalanuvchi_FoydalanuvchiId1",
                table: "Beruvlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bildirishnomalar_AspNetUsers_ApplicationUserId",
                table: "Bildirishnomalar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bildirishnomalar_Foydalanuvchi_FoydalanuvchiId",
                table: "Bildirishnomalar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bronlar_Foydalanuvchi_FoydalanuvchiId1",
                table: "Bronlar");

            migrationBuilder.DropForeignKey(
                name: "FK_FaoliyatLoglar_AspNetUsers_ApplicationUserId",
                table: "FaoliyatLoglar");

            migrationBuilder.DropForeignKey(
                name: "FK_FaoliyatLoglar_Foydalanuvchi_FoydalanuvchiId",
                table: "FaoliyatLoglar");

            migrationBuilder.DropForeignKey(
                name: "FK_Jarimalar_Foydalanuvchi_FoydalanuvchiId1",
                table: "Jarimalar");

            migrationBuilder.DropForeignKey(
                name: "FK_Reytinglar_AspNetUsers_ApplicationUserId",
                table: "Reytinglar");

            migrationBuilder.DropForeignKey(
                name: "FK_Reytinglar_Foydalanuvchi_FoydalanuvchiId",
                table: "Reytinglar");

            migrationBuilder.DropTable(
                name: "Foydalanuvchi");

            migrationBuilder.DropIndex(
                name: "IX_Reytinglar_ApplicationUserId",
                table: "Reytinglar");

            migrationBuilder.DropIndex(
                name: "IX_Jarimalar_FoydalanuvchiId1",
                table: "Jarimalar");

            migrationBuilder.DropIndex(
                name: "IX_FaoliyatLoglar_ApplicationUserId",
                table: "FaoliyatLoglar");

            migrationBuilder.DropIndex(
                name: "IX_Bronlar_FoydalanuvchiId1",
                table: "Bronlar");

            migrationBuilder.DropIndex(
                name: "IX_Bildirishnomalar_ApplicationUserId",
                table: "Bildirishnomalar");

            migrationBuilder.DropIndex(
                name: "IX_Beruvlar_FoydalanuvchiId1",
                table: "Beruvlar");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Reytinglar");

            migrationBuilder.DropColumn(
                name: "FoydalanuvchiId1",
                table: "Jarimalar");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "FaoliyatLoglar");

            migrationBuilder.DropColumn(
                name: "FoydalanuvchiId1",
                table: "Bronlar");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Bildirishnomalar");

            migrationBuilder.DropColumn(
                name: "FoydalanuvchiId1",
                table: "Beruvlar");

            migrationBuilder.AlterColumn<string>(
                name: "FoydalanuvchiId",
                table: "Reytinglar",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "FoydalanuvchiId",
                table: "FaoliyatLoglar",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FoydalanuvchiId",
                table: "Bildirishnomalar",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Bildirishnomalar_AspNetUsers_FoydalanuvchiId",
                table: "Bildirishnomalar",
                column: "FoydalanuvchiId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaoliyatLoglar_AspNetUsers_FoydalanuvchiId",
                table: "FaoliyatLoglar",
                column: "FoydalanuvchiId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Reytinglar_AspNetUsers_FoydalanuvchiId",
                table: "Reytinglar",
                column: "FoydalanuvchiId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bildirishnomalar_AspNetUsers_FoydalanuvchiId",
                table: "Bildirishnomalar");

            migrationBuilder.DropForeignKey(
                name: "FK_FaoliyatLoglar_AspNetUsers_FoydalanuvchiId",
                table: "FaoliyatLoglar");

            migrationBuilder.DropForeignKey(
                name: "FK_Reytinglar_AspNetUsers_FoydalanuvchiId",
                table: "Reytinglar");

            migrationBuilder.AlterColumn<int>(
                name: "FoydalanuvchiId",
                table: "Reytinglar",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Reytinglar",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FoydalanuvchiId1",
                table: "Jarimalar",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FoydalanuvchiId",
                table: "FaoliyatLoglar",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "FaoliyatLoglar",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FoydalanuvchiId1",
                table: "Bronlar",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FoydalanuvchiId",
                table: "Bildirishnomalar",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Bildirishnomalar",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FoydalanuvchiId1",
                table: "Beruvlar",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Foydalanuvchi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AmalMuddati = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Fakultet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Familiya = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Faol = table.Column<bool>(type: "bit", nullable: false),
                    Guruh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ism = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Izoh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Manzil = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OxirgiKirish = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Parol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rasm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoyxatSanasi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sharif = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TalabaId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TugilganSana = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Turi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foydalanuvchi", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reytinglar_ApplicationUserId",
                table: "Reytinglar",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Jarimalar_FoydalanuvchiId1",
                table: "Jarimalar",
                column: "FoydalanuvchiId1");

            migrationBuilder.CreateIndex(
                name: "IX_FaoliyatLoglar_ApplicationUserId",
                table: "FaoliyatLoglar",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bronlar_FoydalanuvchiId1",
                table: "Bronlar",
                column: "FoydalanuvchiId1");

            migrationBuilder.CreateIndex(
                name: "IX_Bildirishnomalar_ApplicationUserId",
                table: "Bildirishnomalar",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Beruvlar_FoydalanuvchiId1",
                table: "Beruvlar",
                column: "FoydalanuvchiId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Beruvlar_Foydalanuvchi_FoydalanuvchiId1",
                table: "Beruvlar",
                column: "FoydalanuvchiId1",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bildirishnomalar_AspNetUsers_ApplicationUserId",
                table: "Bildirishnomalar",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bildirishnomalar_Foydalanuvchi_FoydalanuvchiId",
                table: "Bildirishnomalar",
                column: "FoydalanuvchiId",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bronlar_Foydalanuvchi_FoydalanuvchiId1",
                table: "Bronlar",
                column: "FoydalanuvchiId1",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FaoliyatLoglar_AspNetUsers_ApplicationUserId",
                table: "FaoliyatLoglar",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FaoliyatLoglar_Foydalanuvchi_FoydalanuvchiId",
                table: "FaoliyatLoglar",
                column: "FoydalanuvchiId",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jarimalar_Foydalanuvchi_FoydalanuvchiId1",
                table: "Jarimalar",
                column: "FoydalanuvchiId1",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reytinglar_AspNetUsers_ApplicationUserId",
                table: "Reytinglar",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reytinglar_Foydalanuvchi_FoydalanuvchiId",
                table: "Reytinglar",
                column: "FoydalanuvchiId",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
