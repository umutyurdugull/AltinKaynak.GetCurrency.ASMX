using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace altinkaynak.Migrations
{
    /// <inheritdoc />
    public partial class GuncellemeZamani : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "GuncellemeZamani",
                table: "Kurlar",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuncellemeZamani",
                table: "Kurlar");
        }
    }
}
