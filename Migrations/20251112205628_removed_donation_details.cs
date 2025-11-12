using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShelfLife.Migrations
{
    /// <inheritdoc />
    public partial class removed_donation_details : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDonatable",
                table: "BookListings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDonatable",
                table: "BookListings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
