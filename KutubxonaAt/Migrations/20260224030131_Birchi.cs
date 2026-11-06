using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KutubxonaAt.Migrations
{
    /// <inheritdoc />
    public partial class Birchi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ism = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Familiya = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sharif = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Manzil = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TugilganSana = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Turi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fakultet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Guruh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TalabaId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoyxatSanasi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    OxirgiKirish = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AmalMuddati = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Faol = table.Column<bool>(type: "bit", nullable: false),
                    Rasm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Izoh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Foydalanuvchi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ism = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Familiya = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sharif = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Parol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Manzil = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TugilganSana = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Turi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fakultet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Guruh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TalabaId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoyxatSanasi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OxirgiKirish = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AmalMuddati = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Faol = table.Column<bool>(type: "bit", nullable: false),
                    Rasm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Izoh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foydalanuvchi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kategoriyalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nomi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tavsif = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OtaKategoriyaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriyalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kategoriyalar_Kategoriyalar_OtaKategoriyaId",
                        column: x => x.OtaKategoriyaId,
                        principalTable: "Kategoriyalar",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Kitoblar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nomi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AslNomi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ISBN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Tavsif = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Annotatsiya = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NashrYili = table.Column<int>(type: "int", nullable: true),
                    SahifalarSoni = table.Column<int>(type: "int", nullable: true),
                    Nashriyot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shahar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Til = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UDK = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BBK = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MuallifBelgisi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Narxi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Rasm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QoshilganSana = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    TahrirlanganSana = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NusxalarSoni = table.Column<int>(type: "int", nullable: false),
                    MavjudNusxalar = table.Column<int>(type: "int", nullable: false),
                    Elektron = table.Column<bool>(type: "bit", nullable: false),
                    ElektronManzil = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ElektronFayl = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    FaylFormati = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KorishlarSoni = table.Column<int>(type: "int", nullable: false),
                    OrtachaBaho = table.Column<double>(type: "float", nullable: false),
                    BaholarSoni = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kitoblar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mualliflar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ism = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Familiya = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Sharif = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Taxallus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TugilganSana = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VafotSana = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Biografiya = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Davlat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rasm = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mualliflar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bildirishnomalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoydalanuvchiId = table.Column<int>(type: "int", nullable: false),
                    Sarlavha = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Matn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    YuborilganSana = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Oqilgan = table.Column<bool>(type: "bit", nullable: false),
                    OqilganSana = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Tur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Havola = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bildirishnomalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bildirishnomalar_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bildirishnomalar_Foydalanuvchi_FoydalanuvchiId",
                        column: x => x.FoydalanuvchiId,
                        principalTable: "Foydalanuvchi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FaoliyatLoglar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoydalanuvchiId = table.Column<int>(type: "int", nullable: true),
                    AmalTuri = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Tavsif = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IP = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    Qurilma = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Sana = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaoliyatLoglar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaoliyatLoglar_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FaoliyatLoglar_Foydalanuvchi_FoydalanuvchiId",
                        column: x => x.FoydalanuvchiId,
                        principalTable: "Foydalanuvchi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "KitobKategoriyalari",
                columns: table => new
                {
                    KitobId = table.Column<int>(type: "int", nullable: false),
                    KategoriyaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitobKategoriyalari", x => new { x.KitobId, x.KategoriyaId });
                    table.ForeignKey(
                        name: "FK_KitobKategoriyalari_Kategoriyalar_KategoriyaId",
                        column: x => x.KategoriyaId,
                        principalTable: "Kategoriyalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KitobKategoriyalari_Kitoblar_KitobId",
                        column: x => x.KitobId,
                        principalTable: "Kitoblar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KitobNusxalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KitobId = table.Column<int>(type: "int", nullable: false),
                    InventarRaqami = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BarKod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RFID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QabulQilinganSana = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KelibChiqishi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Narxi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Holati = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JismoniyHolati = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Joylashuv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OquvZalidanChiqmaydi = table.Column<bool>(type: "bit", nullable: false),
                    Cheklangan = table.Column<bool>(type: "bit", nullable: false),
                    OxirgiInventarizatsiya = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Izoh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitobNusxalari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KitobNusxalari_Kitoblar_KitobId",
                        column: x => x.KitobId,
                        principalTable: "Kitoblar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reytinglar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoydalanuvchiId = table.Column<int>(type: "int", nullable: false),
                    KitobId = table.Column<int>(type: "int", nullable: false),
                    Baho = table.Column<int>(type: "int", nullable: false),
                    Sharh = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    YaratilganSana = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TahrirlanganSana = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reytinglar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reytinglar_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reytinglar_Foydalanuvchi_FoydalanuvchiId",
                        column: x => x.FoydalanuvchiId,
                        principalTable: "Foydalanuvchi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reytinglar_Kitoblar_KitobId",
                        column: x => x.KitobId,
                        principalTable: "Kitoblar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KitobMualliflari",
                columns: table => new
                {
                    KitobId = table.Column<int>(type: "int", nullable: false),
                    MuallifId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitobMualliflari", x => new { x.KitobId, x.MuallifId });
                    table.ForeignKey(
                        name: "FK_KitobMualliflari_Kitoblar_KitobId",
                        column: x => x.KitobId,
                        principalTable: "Kitoblar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KitobMualliflari_Mualliflar_MuallifId",
                        column: x => x.MuallifId,
                        principalTable: "Mualliflar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Beruvlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoydalanuvchiId = table.Column<int>(type: "int", nullable: false),
                    KitobNusxaId = table.Column<int>(type: "int", nullable: false),
                    BeruvRaqami = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BerilganSana = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    QaytarishMuddati = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QaytarilganSana = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Holati = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UzaytirishlarSoni = table.Column<int>(type: "int", nullable: false),
                    UzaytirilganMuddat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BerganXodim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QabulQilganXodim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Izoh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beruvlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beruvlar_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Beruvlar_Foydalanuvchi_FoydalanuvchiId",
                        column: x => x.FoydalanuvchiId,
                        principalTable: "Foydalanuvchi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Beruvlar_KitobNusxalari_KitobNusxaId",
                        column: x => x.KitobNusxaId,
                        principalTable: "KitobNusxalari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bronlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoydalanuvchiId = table.Column<int>(type: "int", nullable: false),
                    KitobId = table.Column<int>(type: "int", nullable: false),
                    BronSana = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    AmalMuddati = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Holati = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KitobNusxaId = table.Column<int>(type: "int", nullable: true),
                    Izoh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bronlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bronlar_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bronlar_Foydalanuvchi_FoydalanuvchiId",
                        column: x => x.FoydalanuvchiId,
                        principalTable: "Foydalanuvchi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bronlar_KitobNusxalari_KitobNusxaId",
                        column: x => x.KitobNusxaId,
                        principalTable: "KitobNusxalari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Bronlar_Kitoblar_KitobId",
                        column: x => x.KitobId,
                        principalTable: "Kitoblar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Jarimalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoydalanuvchiId = table.Column<int>(type: "int", nullable: false),
                    BeruvId = table.Column<int>(type: "int", nullable: true),
                    Miqdor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Valyuta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JarimaTuri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sabab = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BerilganSana = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    TolanganSana = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Holati = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TolovUsuli = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Izoh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jarimalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jarimalar_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Jarimalar_Beruvlar_BeruvId",
                        column: x => x.BeruvId,
                        principalTable: "Beruvlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jarimalar_Foydalanuvchi_FoydalanuvchiId",
                        column: x => x.FoydalanuvchiId,
                        principalTable: "Foydalanuvchi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

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

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Beruvlar_ApplicationUserId",
                table: "Beruvlar",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Beruvlar_FoydalanuvchiId",
                table: "Beruvlar",
                column: "FoydalanuvchiId");

            migrationBuilder.CreateIndex(
                name: "IX_Beruvlar_Holati",
                table: "Beruvlar",
                column: "Holati");

            migrationBuilder.CreateIndex(
                name: "IX_Beruvlar_KitobNusxaId",
                table: "Beruvlar",
                column: "KitobNusxaId");

            migrationBuilder.CreateIndex(
                name: "IX_Beruvlar_QaytarishMuddati",
                table: "Beruvlar",
                column: "QaytarishMuddati");

            migrationBuilder.CreateIndex(
                name: "IX_Bildirishnomalar_ApplicationUserId",
                table: "Bildirishnomalar",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bildirishnomalar_FoydalanuvchiId",
                table: "Bildirishnomalar",
                column: "FoydalanuvchiId");

            migrationBuilder.CreateIndex(
                name: "IX_Bronlar_ApplicationUserId",
                table: "Bronlar",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bronlar_FoydalanuvchiId",
                table: "Bronlar",
                column: "FoydalanuvchiId");

            migrationBuilder.CreateIndex(
                name: "IX_Bronlar_Holati",
                table: "Bronlar",
                column: "Holati");

            migrationBuilder.CreateIndex(
                name: "IX_Bronlar_KitobId",
                table: "Bronlar",
                column: "KitobId");

            migrationBuilder.CreateIndex(
                name: "IX_Bronlar_KitobNusxaId",
                table: "Bronlar",
                column: "KitobNusxaId");

            migrationBuilder.CreateIndex(
                name: "IX_FaoliyatLoglar_ApplicationUserId",
                table: "FaoliyatLoglar",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FaoliyatLoglar_FoydalanuvchiId",
                table: "FaoliyatLoglar",
                column: "FoydalanuvchiId");

            migrationBuilder.CreateIndex(
                name: "IX_Jarimalar_ApplicationUserId",
                table: "Jarimalar",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Jarimalar_BeruvId",
                table: "Jarimalar",
                column: "BeruvId");

            migrationBuilder.CreateIndex(
                name: "IX_Jarimalar_FoydalanuvchiId",
                table: "Jarimalar",
                column: "FoydalanuvchiId");

            migrationBuilder.CreateIndex(
                name: "IX_Jarimalar_Holati",
                table: "Jarimalar",
                column: "Holati");

            migrationBuilder.CreateIndex(
                name: "IX_Kategoriyalar_OtaKategoriyaId",
                table: "Kategoriyalar",
                column: "OtaKategoriyaId");

            migrationBuilder.CreateIndex(
                name: "IX_KitobKategoriyalari_KategoriyaId",
                table: "KitobKategoriyalari",
                column: "KategoriyaId");

            migrationBuilder.CreateIndex(
                name: "IX_Kitoblar_ISBN",
                table: "Kitoblar",
                column: "ISBN",
                unique: true,
                filter: "[ISBN] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_KitobMualliflari_MuallifId",
                table: "KitobMualliflari",
                column: "MuallifId");

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
                name: "IX_KitobNusxalari_KitobId",
                table: "KitobNusxalari",
                column: "KitobId");

            migrationBuilder.CreateIndex(
                name: "IX_Mualliflar_Familiya_Ism",
                table: "Mualliflar",
                columns: new[] { "Familiya", "Ism" });

            migrationBuilder.CreateIndex(
                name: "IX_Reytinglar_ApplicationUserId",
                table: "Reytinglar",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reytinglar_FoydalanuvchiId",
                table: "Reytinglar",
                column: "FoydalanuvchiId");

            migrationBuilder.CreateIndex(
                name: "IX_Reytinglar_KitobId",
                table: "Reytinglar",
                column: "KitobId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Bildirishnomalar");

            migrationBuilder.DropTable(
                name: "Bronlar");

            migrationBuilder.DropTable(
                name: "FaoliyatLoglar");

            migrationBuilder.DropTable(
                name: "Jarimalar");

            migrationBuilder.DropTable(
                name: "KitobKategoriyalari");

            migrationBuilder.DropTable(
                name: "KitobMualliflari");

            migrationBuilder.DropTable(
                name: "Reytinglar");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Beruvlar");

            migrationBuilder.DropTable(
                name: "Kategoriyalar");

            migrationBuilder.DropTable(
                name: "Mualliflar");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Foydalanuvchi");

            migrationBuilder.DropTable(
                name: "KitobNusxalari");

            migrationBuilder.DropTable(
                name: "Kitoblar");
        }
    }
}
