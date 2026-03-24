namespace GZone.Service.BusinessModels.Request.ProductVariant
{
    public class ProductVariantCreateRequest
    {
        public string Sku { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal AdditionalPrice { get; set; }
        public int StockQuantity { get; set; }
        public decimal Weight { get; set; }
        public Guid ProductId { get; set; }
    }
}
