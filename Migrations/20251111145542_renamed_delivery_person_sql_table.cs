using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShelfLife.Migrations
{
    /// <inheritdoc />
    public partial class renamed_delivery_person_sql_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_DeliveryPeople_DeliveryPersonID",
                table: "Deliveries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryPeople",
                table: "DeliveryPeople");

            migrationBuilder.RenameTable(
                name: "DeliveryPeople",
                newName: "DeliveryPerson");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryPerson",
                table: "DeliveryPerson",
                column: "DeliveryPersonID");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_DeliveryPerson_DeliveryPersonID",
                table: "Deliveries",
                column: "DeliveryPersonID",
                principalTable: "DeliveryPerson",
                principalColumn: "DeliveryPersonID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_DeliveryPerson_DeliveryPersonID",
                table: "Deliveries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryPerson",
                table: "DeliveryPerson");

            migrationBuilder.RenameTable(
                name: "DeliveryPerson",
                newName: "DeliveryPeople");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryPeople",
                table: "DeliveryPeople",
                column: "DeliveryPersonID");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_DeliveryPeople_DeliveryPersonID",
                table: "Deliveries",
                column: "DeliveryPersonID",
                principalTable: "DeliveryPeople",
                principalColumn: "DeliveryPersonID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
