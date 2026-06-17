using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfvalMonitoring.Migrations.DataDb
{
    /// <inheritdoc />
    public partial class AddUserIdAndAllToVoorspelling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "All",
                table: "Voorspellingen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Voorspellingen",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Voorspellingen_UserId",
                table: "Voorspellingen",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Voorspellingen_AppUsers_UserId",
                table: "Voorspellingen",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voorspellingen_AppUsers_UserId",
                table: "Voorspellingen");

            migrationBuilder.DropIndex(
                name: "IX_Voorspellingen_UserId",
                table: "Voorspellingen");

            migrationBuilder.DropColumn(
                name: "All",
                table: "Voorspellingen");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Voorspellingen");
        }
    }
}
