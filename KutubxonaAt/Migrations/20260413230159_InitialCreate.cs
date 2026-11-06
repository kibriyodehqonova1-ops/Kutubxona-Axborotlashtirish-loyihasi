using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KutubxonaAt.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bildirishnomalar_Foydalanuvchi_FoydalanuvchiId",
                table: "Bildirishnomalar");

            migrationBuilder.DropForeignKey(
                name: "FK_FaoliyatLoglar_Foydalanuvchi_FoydalanuvchiId",
                table: "FaoliyatLoglar");

            migrationBuilder.DropForeignKey(
                name: "FK_Reytinglar_Foydalanuvchi_FoydalanuvchiId",
                table: "Reytinglar");

            migrationBuilder.DropForeignKey(
                name: "FK_Reytinglar_Kitoblar_KitobId",
                table: "Reytinglar");

            migrationBuilder.DropIndex(
                name: "IX_Mualliflar_Familiya_Ism",
                table: "Mualliflar");

            migrationBuilder.DropIndex(
                name: "IX_KitobNusxalari_BarKod",
                table: "KitobNusxalari");

            migrationBuilder.DropIndex(
                name: "IX_KitobNusxalari_InventarRaqami",
                table: "KitobNusxalari");

            migrationBuilder.DropIndex(
                name: "IX_Kitoblar_ISBN",
                table: "Kitoblar");

            migrationBuilder.DropIndex(
                name: "IX_Jarimalar_Holati",
                table: "Jarimalar");

            migrationBuilder.DropIndex(
                name: "IX_Bronlar_Holati",
                table: "Bronlar");

            migrationBuilder.DropIndex(
                name: "IX_Beruvlar_Holati",
                table: "Beruvlar");

            migrationBuilder.DropIndex(
                name: "IX_Beruvlar_QaytarishMuddati",
                table: "Beruvlar");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TalabaId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "QoshilganSana",
                table: "Kitoblar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Holati",
                table: "Jarimalar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BerilganSana",
                table: "Jarimalar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Sana",
                table: "FaoliyatLoglar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Holati",
                table: "Bronlar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BronSana",
                table: "Bronlar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "YuborilganSana",
                table: "Bildirishnomalar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Holati",
                table: "Beruvlar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BerilganSana",
                table: "Beruvlar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "TalabaId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RoyxatSanasi",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoydalanuvchiId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Sana = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JamiSumma = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Holati = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TolovUsuli = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Manzil = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Izoh = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_FoydalanuvchiId",
                        column: x => x.FoydalanuvchiId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    KitobId = table.Column<int>(type: "int", nullable: false),
                    KitobNusxaId = table.Column<int>(type: "int", nullable: false),
                    Narxi = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_KitobNusxalari_KitobNusxaId",
                        column: x => x.KitobNusxaId,
                        principalTable: "KitobNusxalari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Kitoblar_KitobId",
                        column: x => x.KitobId,
                        principalTable: "Kitoblar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_KitobId",
                table: "OrderItems",
                column: "KitobId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_KitobNusxaId",
                table: "OrderItems",
                column: "KitobNusxaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_FoydalanuvchiId",
                table: "Orders",
                column: "FoydalanuvchiId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bildirishnomalar_Foydalanuvchi_FoydalanuvchiId",
                table: "Bildirishnomalar",
                column: "FoydalanuvchiId",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaoliyatLoglar_Foydalanuvchi_FoydalanuvchiId",
                table: "FaoliyatLoglar",
                column: "FoydalanuvchiId",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reytinglar_Foydalanuvchi_FoydalanuvchiId",
                table: "Reytinglar",
                column: "FoydalanuvchiId",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reytinglar_Kitoblar_KitobId",
                table: "Reytinglar",
                column: "KitobId",
                principalTable: "Kitoblar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bildirishnomalar_Foydalanuvchi_FoydalanuvchiId",
                table: "Bildirishnomalar");

            migrationBuilder.DropForeignKey(
                name: "FK_FaoliyatLoglar_Foydalanuvchi_FoydalanuvchiId",
                table: "FaoliyatLoglar");

            migrationBuilder.DropForeignKey(
                name: "FK_Reytinglar_Foydalanuvchi_FoydalanuvchiId",
                table: "Reytinglar");

            migrationBuilder.DropForeignKey(
                name: "FK_Reytinglar_Kitoblar_KitobId",
                table: "Reytinglar");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.AlterColumn<DateTime>(
                name: "QoshilganSana",
                table: "Kitoblar",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Holati",
                table: "Jarimalar",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BerilganSana",
                table: "Jarimalar",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Sana",
                table: "FaoliyatLoglar",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Holati",
                table: "Bronlar",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BronSana",
                table: "Bronlar",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "YuborilganSana",
                table: "Bildirishnomalar",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Holati",
                table: "Beruvlar",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BerilganSana",
                table: "Beruvlar",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "TalabaId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RoyxatSanasi",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_Mualliflar_Familiya_Ism",
                table: "Mualliflar",
                columns: new[] { "Familiya", "Ism" });

            migrationBuilder.CreateIndex(
                name: "IX_KitobNusxalari_BarKod",
                table: "KitobNusxalari",
                column: "BarKod",
                unique: true,
                filter: "[BarKod] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_KitobNusxalari_InventarRaqami",
                table: "KitobNusxalari",
                column: "InventarRaqami",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kitoblar_ISBN",
                table: "Kitoblar",
                column: "ISBN",
                unique: true,
                filter: "[ISBN] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Jarimalar_Holati",
                table: "Jarimalar",
                column: "Holati");

            migrationBuilder.CreateIndex(
                name: "IX_Bronlar_Holati",
                table: "Bronlar",
                column: "Holati");

            migrationBuilder.CreateIndex(
                name: "IX_Beruvlar_Holati",
                table: "Beruvlar",
                column: "Holati");

            migrationBuilder.CreateIndex(
                name: "IX_Beruvlar_QaytarishMuddati",
                table: "Beruvlar",
                column: "QaytarishMuddati");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TalabaId",
                table: "AspNetUsers",
                column: "TalabaId",
                unique: true,
                filter: "[TalabaId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Bildirishnomalar_Foydalanuvchi_FoydalanuvchiId",
                table: "Bildirishnomalar",
                column: "FoydalanuvchiId",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaoliyatLoglar_Foydalanuvchi_FoydalanuvchiId",
                table: "FaoliyatLoglar",
                column: "FoydalanuvchiId",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Reytinglar_Foydalanuvchi_FoydalanuvchiId",
                table: "Reytinglar",
                column: "FoydalanuvchiId",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reytinglar_Kitoblar_KitobId",
                table: "Reytinglar",
                column: "KitobId",
                principalTable: "Kitoblar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
