using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2Shelf_Life.Migrations
{
    /// <inheritdoc />
    public partial class OneToOneMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Listings_ListingID",
                table: "Rating");

            migrationBuilder.DropIndex(
                name: "IX_Rating_ListingID",
                table: "Rating");

            migrationBuilder.AddColumn<int>(
                name: "RatingID",
                table: "Listings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Listings_RatingID",
                table: "Listings",
                column: "RatingID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Rating_RatingID",
                table: "Listings",
                column: "RatingID",
                principalTable: "Rating",
                principalColumn: "RatingID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Rating_RatingID",
                table: "Listings");

            migrationBuilder.DropIndex(
                name: "IX_Listings_RatingID",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "RatingID",
                table: "Listings");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_ListingID",
                table: "Rating",
                column: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Listings_ListingID",
                table: "Rating",
                column: "ListingID",
                principalTable: "Listings",
                principalColumn: "ListingID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
