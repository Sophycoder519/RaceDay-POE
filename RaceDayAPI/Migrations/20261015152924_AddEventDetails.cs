using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaceDayAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddEventDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Distance",
                table: "Events",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Distance",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "Events");
        }
    }
}
