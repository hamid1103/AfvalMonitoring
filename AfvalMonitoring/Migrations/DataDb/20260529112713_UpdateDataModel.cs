using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfvalMonitoring.Migrations.DataDb
{
    /// <inheritdoc />
    public partial class UpdateDataModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataObjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Confidence = table.Column<float>(type: "real", nullable: false),
                    LocatieCoordinaten = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    LocatieAdres = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tijd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CameraID = table.Column<int>(type: "int", nullable: false),
                    BoundingBoxRB = table.Column<float>(type: "real", nullable: false),
                    BoundingBoxLB = table.Column<float>(type: "real", nullable: false),
                    BoundingBoxCenterX = table.Column<float>(type: "real", nullable: false),
                    BoundingBoxCenterY = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataObjects", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataObjects");
        }
    }
}
