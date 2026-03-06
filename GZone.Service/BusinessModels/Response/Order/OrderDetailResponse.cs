namespace GZone.Service.BusinessModels.Response.Order
{
    public class OrderDetailResponse
    {
        public Guid OrderDetailId { get; set; }
        public string ProductName { get; set; }
        public string VariantInfo { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCustomDesign { get; set; }
        public string CustomDesignNote { get; set; }
        public string CustomDesignImage { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int WarrantyPeriodMonths { get; set; }

        // Foreign keys
        public Guid OrderId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public Guid? CustomizationId { get; set; }
    }
}
