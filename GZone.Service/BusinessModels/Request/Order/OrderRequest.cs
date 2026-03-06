namespace GZone.Service.BusinessModels.Request.Order
{
    public class OrderRequest
    {
        // Shipping Info
        public string? ShippingAddress { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingDistrict { get; set; }
        public string? ShippingWard { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverPhone { get; set; }

        // Payment
        public string? PaymentMethod { get; set; }
        public bool WholeSale { get; set; }

        // Note
        public string? Note { get; set; }

        // Order Details (for creating order with items)
        public List<OrderDetailItemRequest>? OrderDetails { get; set; }
    }

    public class OrderDetailItemRequest
    {
        public Guid? ProductVariantId { get; set; }
        public Guid? CustomizationId { get; set; }
        public string? ProductName { get; set; }
        public string? VariantInfo { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool IsCustomDesign { get; set; }
        public string? CustomDesignNote { get; set; }
        public string? CustomDesignImage { get; set; }
        public int WarrantyPeriodMonths { get; set; }
    }
}
