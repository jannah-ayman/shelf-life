namespace ShelfLife.DTOs
{
    public class PaymentBreakdownDTO
    {
        // Order info
        public int OrderID { get; set; }
        public Models.OrderType OrderType { get; set; }
        public Models.UserType SellerUserType { get; set; }

        // Item pricing
        public decimal ItemPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }

        // Fees
        public decimal PlatformFeePercentage { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal DeliveryFee { get; set; }

        // For swaps - split delivery fee
        public decimal? BuyerDeliveryShare { get; set; }
        public decimal? SellerDeliveryShare { get; set; }

        // Totals
        public decimal TotalAmount { get; set; }
        public decimal BuyerPays { get; set; }

        // Distribution
        public decimal SellerReceives { get; set; }
        public decimal DeliveryPersonReceives { get; set; }
        public decimal PlatformReceives { get; set; }

        // Breakdown explanation
        public string PaymentSummary { get; set; } = string.Empty;
    }
}
