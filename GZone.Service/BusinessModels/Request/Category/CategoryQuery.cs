namespace GZone.Service.BusinessModels.Request.Category
{
    public class CategoryQuery
    {
        public string? Keyword { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; } // name_asc | name_desc | order_asc | order_desc | newest | oldest
    }
}
