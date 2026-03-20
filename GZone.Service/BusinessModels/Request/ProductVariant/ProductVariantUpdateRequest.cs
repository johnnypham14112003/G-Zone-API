namespace GZone.Service.BusinessModels.Request.ProductVariant
{
    public class ProductVariantUpdateRequest
    {
        public string Sku { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal AdditionalPrice { get; set; }
        public int StockQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public decimal Weight { get; set; }
        public bool IsActive { get; set; }
    }
}
