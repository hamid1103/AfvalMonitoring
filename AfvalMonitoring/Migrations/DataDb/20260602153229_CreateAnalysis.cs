using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfvalMonitoring.Migrations.DataDb
{
    /// <inheritdoc />
    public partial class CreateAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Analyses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotaleAfval = table.Column<int>(type: "int", nullable: false),
                    Biodegradable = table.Column<int>(type: "int", nullable: false),
                    Paper = table.Column<int>(type: "int", nullable: false),
                    Metal = table.Column<int>(type: "int", nullable: false),
                    Plastic = table.Column<int>(type: "int", nullable: false),
                    Glass = table.Column<int>(type: "int", nullable: false),
                    Cloth = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analyses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Analyses");
        }
    }
}
