using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfvalMonitoring.Migrations.DataDb
{
    /// <inheritdoc />
    public partial class CreateVoorspellingen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Voorspellingen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StreetName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Biodegradable = table.Column<int>(type: "int", nullable: false),
                    Paper = table.Column<int>(type: "int", nullable: false),
                    Metal = table.Column<int>(type: "int", nullable: false),
                    Plastic = table.Column<int>(type: "int", nullable: false),
                    Glass = table.Column<int>(type: "int", nullable: false),
                    Cloth = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voorspellingen", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Voorspellingen");
        }
    }
}
