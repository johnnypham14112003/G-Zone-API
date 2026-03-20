namespace GZone.Service.BusinessModels.Request.ProductVariant
{
    public class ProductVariantQuery
    {
        public string? Keyword { get; set; }
        public Guid? ProductId { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; }
    }
}
