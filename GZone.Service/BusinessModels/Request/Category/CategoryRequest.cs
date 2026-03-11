namespace GZone.Service.BusinessModels.Request.Category
{
    public class CategoryRequest
    {
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? ParentCategoryId { get; set; }
    }
}
