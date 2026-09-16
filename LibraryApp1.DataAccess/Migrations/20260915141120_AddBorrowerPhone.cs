using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Class_Library.Migrations
{
    /// <inheritdoc />
    public partial class AddBorrowerPhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BorrowerPhone",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorrowerPhone",
                table: "Reservations");
        }
    }
}
