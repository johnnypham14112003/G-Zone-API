namespace GZone.Service.BusinessModels.Response
{
    public class ProductVariantResponse
    {
        public Guid VariantId { get; set; }
        public string Sku { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal AdditionalPrice { get; set; }
        public int StockQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public decimal Weight { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
