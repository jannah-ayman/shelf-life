using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShelfLife.Migrations
{
    /// <inheritdoc />
    public partial class renamed_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookListings_Categories_CategoryID",
                table: "BookListings");

            migrationBuilder.DropForeignKey(
                name: "FK_BookListings_Users_UserID",
                table: "BookListings");

            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_DeliveryPerson_DeliveryPersonID",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Orders_OrderID",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_Negotiations_BookListings_OfferedListingID",
                table: "Negotiations");

            migrationBuilder.DropForeignKey(
                name: "FK_Negotiations_Orders_OrderID",
                table: "Negotiations");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UserID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_BookListings_ListingID",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_BuyerID",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_OrderID",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Orders_OrderID",
                table: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Negotiations",
                table: "Negotiations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deliveries",
                table: "Deliveries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookListings",
                table: "BookListings");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "Ratings",
                newName: "Rating");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payment");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Order");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notification");

            migrationBuilder.RenameTable(
                name: "Negotiations",
                newName: "Negotiation");

            migrationBuilder.RenameTable(
                name: "Deliveries",
                newName: "Delivery");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.RenameTable(
                name: "BookListings",
                newName: "BookListing");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_OrderID",
                table: "Rating",
                newName: "IX_Rating_OrderID");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_OrderID",
                table: "Payment",
                newName: "IX_Payment_OrderID");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_ListingID",
                table: "Order",
                newName: "IX_Order_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_BuyerID",
                table: "Order",
                newName: "IX_Order_BuyerID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_UserID",
                table: "Notification",
                newName: "IX_Notification_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Negotiations_OrderID",
                table: "Negotiation",
                newName: "IX_Negotiation_OrderID");

            migrationBuilder.RenameIndex(
                name: "IX_Negotiations_OfferedListingID",
                table: "Negotiation",
                newName: "IX_Negotiation_OfferedListingID");

            migrationBuilder.RenameIndex(
                name: "IX_Deliveries_OrderID",
                table: "Delivery",
                newName: "IX_Delivery_OrderID");

            migrationBuilder.RenameIndex(
                name: "IX_Deliveries_DeliveryPersonID",
                table: "Delivery",
                newName: "IX_Delivery_DeliveryPersonID");

            migrationBuilder.RenameIndex(
                name: "IX_BookListings_UserID",
                table: "BookListing",
                newName: "IX_BookListing_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_BookListings_CategoryID",
                table: "BookListing",
                newName: "IX_BookListing_CategoryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rating",
                table: "Rating",
                column: "RatingID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "PaymentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Order",
                table: "Order",
                column: "OrderID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "NotificationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Negotiation",
                table: "Negotiation",
                column: "NegotiationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Delivery",
                table: "Delivery",
                column: "DeliveryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "CategoryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookListing",
                table: "BookListing",
                column: "BookListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_BookListing_Category_CategoryID",
                table: "BookListing",
                column: "CategoryID",
                principalTable: "Category",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_BookListing_User_UserID",
                table: "BookListing",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_DeliveryPerson_DeliveryPersonID",
                table: "Delivery",
                column: "DeliveryPersonID",
                principalTable: "DeliveryPerson",
                principalColumn: "DeliveryPersonID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Order_OrderID",
                table: "Delivery",
                column: "OrderID",
                principalTable: "Order",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Negotiation_BookListing_OfferedListingID",
                table: "Negotiation",
                column: "OfferedListingID",
                principalTable: "BookListing",
                principalColumn: "BookListingID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Negotiation_Order_OrderID",
                table: "Negotiation",
                column: "OrderID",
                principalTable: "Order",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_User_UserID",
                table: "Notification",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_BookListing_ListingID",
                table: "Order",
                column: "ListingID",
                principalTable: "BookListing",
                principalColumn: "BookListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_User_BuyerID",
                table: "Order",
                column: "BuyerID",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Order_OrderID",
                table: "Payment",
                column: "OrderID",
                principalTable: "Order",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Order_OrderID",
                table: "Rating",
                column: "OrderID",
                principalTable: "Order",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookListing_Category_CategoryID",
                table: "BookListing");

            migrationBuilder.DropForeignKey(
                name: "FK_BookListing_User_UserID",
                table: "BookListing");

            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_DeliveryPerson_DeliveryPersonID",
                table: "Delivery");

            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Order_OrderID",
                table: "Delivery");

            migrationBuilder.DropForeignKey(
                name: "FK_Negotiation_BookListing_OfferedListingID",
                table: "Negotiation");

            migrationBuilder.DropForeignKey(
                name: "FK_Negotiation_Order_OrderID",
                table: "Negotiation");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_User_UserID",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_BookListing_ListingID",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_User_BuyerID",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Order_OrderID",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Order_OrderID",
                table: "Rating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rating",
                table: "Rating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Order",
                table: "Order");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Negotiation",
                table: "Negotiation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Delivery",
                table: "Delivery");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookListing",
                table: "BookListing");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Rating",
                newName: "Ratings");

            migrationBuilder.RenameTable(
                name: "Payment",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "Order",
                newName: "Orders");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "Negotiation",
                newName: "Negotiations");

            migrationBuilder.RenameTable(
                name: "Delivery",
                newName: "Deliveries");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.RenameTable(
                name: "BookListing",
                newName: "BookListings");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_OrderID",
                table: "Ratings",
                newName: "IX_Ratings_OrderID");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_OrderID",
                table: "Payments",
                newName: "IX_Payments_OrderID");

            migrationBuilder.RenameIndex(
                name: "IX_Order_ListingID",
                table: "Orders",
                newName: "IX_Orders_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_Order_BuyerID",
                table: "Orders",
                newName: "IX_Orders_BuyerID");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_UserID",
                table: "Notifications",
                newName: "IX_Notifications_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Negotiation_OrderID",
                table: "Negotiations",
                newName: "IX_Negotiations_OrderID");

            migrationBuilder.RenameIndex(
                name: "IX_Negotiation_OfferedListingID",
                table: "Negotiations",
                newName: "IX_Negotiations_OfferedListingID");

            migrationBuilder.RenameIndex(
                name: "IX_Delivery_OrderID",
                table: "Deliveries",
                newName: "IX_Deliveries_OrderID");

            migrationBuilder.RenameIndex(
                name: "IX_Delivery_DeliveryPersonID",
                table: "Deliveries",
                newName: "IX_Deliveries_DeliveryPersonID");

            migrationBuilder.RenameIndex(
                name: "IX_BookListing_UserID",
                table: "BookListings",
                newName: "IX_BookListings_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_BookListing_CategoryID",
                table: "BookListings",
                newName: "IX_BookListings_CategoryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings",
                column: "RatingID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "PaymentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "OrderID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "NotificationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Negotiations",
                table: "Negotiations",
                column: "NegotiationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deliveries",
                table: "Deliveries",
                column: "DeliveryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "CategoryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookListings",
                table: "BookListings",
                column: "BookListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_BookListings_Categories_CategoryID",
                table: "BookListings",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_BookListings_Users_UserID",
                table: "BookListings",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_DeliveryPerson_DeliveryPersonID",
                table: "Deliveries",
                column: "DeliveryPersonID",
                principalTable: "DeliveryPerson",
                principalColumn: "DeliveryPersonID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Orders_OrderID",
                table: "Deliveries",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Negotiations_BookListings_OfferedListingID",
                table: "Negotiations",
                column: "OfferedListingID",
                principalTable: "BookListings",
                principalColumn: "BookListingID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Negotiations_Orders_OrderID",
                table: "Negotiations",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserID",
                table: "Notifications",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_BookListings_ListingID",
                table: "Orders",
                column: "ListingID",
                principalTable: "BookListings",
                principalColumn: "BookListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_BuyerID",
                table: "Orders",
                column: "BuyerID",
                principalTable: "Users",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrderID",
                table: "Payments",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Orders_OrderID",
                table: "Ratings",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
