using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KutubxonaAt.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbonementTurlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nomi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tavsif = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Narxi = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DavomiylikKun = table.Column<int>(type: "int", nullable: false),
                    BepulKitoblarSoni = table.Column<int>(type: "int", nullable: false),
                    ChegirimFoizi = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    BepulYetkazish = table.Column<bool>(type: "bit", nullable: false),
                    MaksimalKitoblar = table.Column<int>(type: "int", nullable: false),
                    Faol = table.Column<bool>(type: "bit", nullable: false),
                    YaratilganSana = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbonementTurlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KutishRoyxati",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoydalanuvchiId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KitobId = table.Column<int>(type: "int", nullable: false),
                    QoshilganSana = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Holati = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BildirilganSana = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Tartib = table.Column<int>(type: "int", nullable: false),
                    Izoh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KutishRoyxati", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KutishRoyxati_AspNetUsers_FoydalanuvchiId",
                        column: x => x.FoydalanuvchiId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KutishRoyxati_Kitoblar_KitobId",
                        column: x => x.KitobId,
                        principalTable: "Kitoblar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YetkazibBerishlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoydalanuvchiId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BeruvId = table.Column<int>(type: "int", nullable: true),
                    BronId = table.Column<int>(type: "int", nullable: true),
                    Manzil = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Shahar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TelefонRaqam = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    YetkazishNarxi = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Holati = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuyurtmaSana = table.Column<DateTime>(type: "datetime2", nullable: false),
                    YetkazilishKutilganSana = table.Column<DateTime>(type: "datetime2", nullable: true),
                    YetkazilganSana = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HaydovchiMaʼlumoti = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    KuzatuvRaqami = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Izoh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TolovQilinganmi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YetkazibBerishlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YetkazibBerishlar_AspNetUsers_FoydalanuvchiId",
                        column: x => x.FoydalanuvchiId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YetkazibBerishlar_Beruvlar_BeruvId",
                        column: x => x.BeruvId,
                        principalTable: "Beruvlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_YetkazibBerishlar_Bronlar_BronId",
                        column: x => x.BronId,
                        principalTable: "Bronlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Abonementlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoydalanuvchiId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AbonementTuriId = table.Column<int>(type: "int", nullable: false),
                    BoshlanishSana = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TugashSana = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TolanganNarx = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Holati = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TolovUsuli = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TolovId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IshlatilganBepulKitoblar = table.Column<int>(type: "int", nullable: false),
                    Eslatma3KunYuborildi = table.Column<bool>(type: "bit", nullable: false),
                    EslatmaTugaganYuborildi = table.Column<bool>(type: "bit", nullable: false),
                    YaratilganSana = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Izoh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abonementlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Abonementlar_AbonementTurlari_AbonementTuriId",
                        column: x => x.AbonementTuriId,
                        principalTable: "AbonementTurlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Abonementlar_AspNetUsers_FoydalanuvchiId",
                        column: x => x.FoydalanuvchiId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Abonementlar_AbonementTuriId",
                table: "Abonementlar",
                column: "AbonementTuriId");

            migrationBuilder.CreateIndex(
                name: "IX_Abonementlar_FoydalanuvchiId",
                table: "Abonementlar",
                column: "FoydalanuvchiId");

            migrationBuilder.CreateIndex(
                name: "IX_KutishRoyxati_FoydalanuvchiId",
                table: "KutishRoyxati",
                column: "FoydalanuvchiId");

            migrationBuilder.CreateIndex(
                name: "IX_KutishRoyxati_KitobId",
                table: "KutishRoyxati",
                column: "KitobId");

            migrationBuilder.CreateIndex(
                name: "IX_YetkazibBerishlar_BeruvId",
                table: "YetkazibBerishlar",
                column: "BeruvId");

            migrationBuilder.CreateIndex(
                name: "IX_YetkazibBerishlar_BronId",
                table: "YetkazibBerishlar",
                column: "BronId");

            migrationBuilder.CreateIndex(
                name: "IX_YetkazibBerishlar_FoydalanuvchiId",
                table: "YetkazibBerishlar",
                column: "FoydalanuvchiId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Abonementlar");

            migrationBuilder.DropTable(
                name: "KutishRoyxati");

            migrationBuilder.DropTable(
                name: "YetkazibBerishlar");

            migrationBuilder.DropTable(
                name: "AbonementTurlari");
        }
    }
}
