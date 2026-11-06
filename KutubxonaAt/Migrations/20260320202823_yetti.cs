using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KutubxonaAt.Migrations
{
    /// <inheritdoc />
    public partial class yetti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Beruvlar_AspNetUsers_ApplicationUserId",
                table: "Beruvlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Beruvlar_Foydalanuvchi_FoydalanuvchiId",
                table: "Beruvlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bronlar_AspNetUsers_ApplicationUserId",
                table: "Bronlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bronlar_Foydalanuvchi_FoydalanuvchiId",
                table: "Bronlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Jarimalar_AspNetUsers_ApplicationUserId",
                table: "Jarimalar");

            migrationBuilder.DropForeignKey(
                name: "FK_Jarimalar_Foydalanuvchi_FoydalanuvchiId",
                table: "Jarimalar");

            migrationBuilder.DropIndex(
                name: "IX_Jarimalar_ApplicationUserId",
                table: "Jarimalar");

            migrationBuilder.DropIndex(
                name: "IX_Bronlar_ApplicationUserId",
                table: "Bronlar");

            migrationBuilder.DropIndex(
                name: "IX_Beruvlar_ApplicationUserId",
                table: "Beruvlar");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Jarimalar");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Bronlar");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Beruvlar");

            migrationBuilder.AlterColumn<string>(
                name: "FoydalanuvchiId",
                table: "Jarimalar",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "FoydalanuvchiId1",
                table: "Jarimalar",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FoydalanuvchiId",
                table: "Bronlar",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "FoydalanuvchiId1",
                table: "Bronlar",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FoydalanuvchiId",
                table: "Beruvlar",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "FoydalanuvchiId1",
                table: "Beruvlar",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jarimalar_FoydalanuvchiId1",
                table: "Jarimalar",
                column: "FoydalanuvchiId1");

            migrationBuilder.CreateIndex(
                name: "IX_Bronlar_FoydalanuvchiId1",
                table: "Bronlar",
                column: "FoydalanuvchiId1");

            migrationBuilder.CreateIndex(
                name: "IX_Beruvlar_FoydalanuvchiId1",
                table: "Beruvlar",
                column: "FoydalanuvchiId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Beruvlar_AspNetUsers_FoydalanuvchiId",
                table: "Beruvlar",
                column: "FoydalanuvchiId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Beruvlar_Foydalanuvchi_FoydalanuvchiId1",
                table: "Beruvlar",
                column: "FoydalanuvchiId1",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bronlar_AspNetUsers_FoydalanuvchiId",
                table: "Bronlar",
                column: "FoydalanuvchiId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bronlar_Foydalanuvchi_FoydalanuvchiId1",
                table: "Bronlar",
                column: "FoydalanuvchiId1",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jarimalar_AspNetUsers_FoydalanuvchiId",
                table: "Jarimalar",
                column: "FoydalanuvchiId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jarimalar_Foydalanuvchi_FoydalanuvchiId1",
                table: "Jarimalar",
                column: "FoydalanuvchiId1",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Beruvlar_AspNetUsers_FoydalanuvchiId",
                table: "Beruvlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Beruvlar_Foydalanuvchi_FoydalanuvchiId1",
                table: "Beruvlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bronlar_AspNetUsers_FoydalanuvchiId",
                table: "Bronlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bronlar_Foydalanuvchi_FoydalanuvchiId1",
                table: "Bronlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Jarimalar_AspNetUsers_FoydalanuvchiId",
                table: "Jarimalar");

            migrationBuilder.DropForeignKey(
                name: "FK_Jarimalar_Foydalanuvchi_FoydalanuvchiId1",
                table: "Jarimalar");

            migrationBuilder.DropIndex(
                name: "IX_Jarimalar_FoydalanuvchiId1",
                table: "Jarimalar");

            migrationBuilder.DropIndex(
                name: "IX_Bronlar_FoydalanuvchiId1",
                table: "Bronlar");

            migrationBuilder.DropIndex(
                name: "IX_Beruvlar_FoydalanuvchiId1",
                table: "Beruvlar");

            migrationBuilder.DropColumn(
                name: "FoydalanuvchiId1",
                table: "Jarimalar");

            migrationBuilder.DropColumn(
                name: "FoydalanuvchiId1",
                table: "Bronlar");

            migrationBuilder.DropColumn(
                name: "FoydalanuvchiId1",
                table: "Beruvlar");

            migrationBuilder.AlterColumn<int>(
                name: "FoydalanuvchiId",
                table: "Jarimalar",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Jarimalar",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FoydalanuvchiId",
                table: "Bronlar",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Bronlar",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FoydalanuvchiId",
                table: "Beruvlar",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Beruvlar",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jarimalar_ApplicationUserId",
                table: "Jarimalar",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bronlar_ApplicationUserId",
                table: "Bronlar",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Beruvlar_ApplicationUserId",
                table: "Beruvlar",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Beruvlar_AspNetUsers_ApplicationUserId",
                table: "Beruvlar",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Beruvlar_Foydalanuvchi_FoydalanuvchiId",
                table: "Beruvlar",
                column: "FoydalanuvchiId",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bronlar_AspNetUsers_ApplicationUserId",
                table: "Bronlar",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bronlar_Foydalanuvchi_FoydalanuvchiId",
                table: "Bronlar",
                column: "FoydalanuvchiId",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jarimalar_AspNetUsers_ApplicationUserId",
                table: "Jarimalar",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jarimalar_Foydalanuvchi_FoydalanuvchiId",
                table: "Jarimalar",
                column: "FoydalanuvchiId",
                principalTable: "Foydalanuvchi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
