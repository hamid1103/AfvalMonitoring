using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfvalMonitoring.Migrations.DataDb
{
    /// <inheritdoc />
    public partial class RemoveUserNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voorspellingen_AppUsers_UserId",
                table: "Voorspellingen");

            migrationBuilder.DropIndex(
                name: "IX_Voorspellingen_UserId",
                table: "Voorspellingen");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
