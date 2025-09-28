using System.ComponentModel.DataAnnotations;

namespace StockTrackerAPI.Models
{
    public class StockHolding
    {
        public string Symbol { get; set; } = string.Empty;
        public decimal TotalQuantity { get; set; }
        public decimal TotalCost { get; set; }
    }

    public class AddStockRequest
    {
        [Required]
        public string Symbol { get; set; } = string.Empty;
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
    }

    public class UpdateQuantityRequest
    {
        [Required]
        public decimal Quantity { get; set; }
    }

    public class StockSummary
    {
        public string Symbol { get; set; } = string.Empty;
        public decimal TotalQuantity { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TotalInvested { get; set; }
        public decimal CurrentValue { get; set; }
    }
}
