namespace GZone.Service.BusinessModels.Response.Category
{
    public class CategoryResponse
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public Guid? ParentCategoryId { get; set; }
    }
}
