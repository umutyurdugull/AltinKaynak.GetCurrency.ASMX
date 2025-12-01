using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace altinkaynak.Migrations
{
    /// <inheritdoc />
    public partial class InitalCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Altinlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alis = table.Column<double>(type: "float", nullable: false),
                    Satis = table.Column<double>(type: "float", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AltinTipi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Altinlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kurlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alis = table.Column<double>(type: "float", nullable: false),
                    Satis = table.Column<double>(type: "float", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KurTipi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KurKodu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuncellemeZamani = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kurlar", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Altinlar");

            migrationBuilder.DropTable(
                name: "Kurlar");
        }
    }
}
