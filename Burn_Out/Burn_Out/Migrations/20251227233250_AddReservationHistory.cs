using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Burn_Out.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "HallReservations",
                type: "nvarchar(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "WhenReserved",
                table: "HallReservations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "HallReservations");

            migrationBuilder.DropColumn(
                name: "WhenReserved",
                table: "HallReservations");
        }
    }
}