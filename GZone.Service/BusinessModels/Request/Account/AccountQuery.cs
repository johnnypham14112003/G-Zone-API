namespace GZone.Service.BusinessModels.Request.Account
{
    public class AccountQuery
    {
        public string? SearchTerm { get; set; } // Tìm tương đối theo Username, Email, Name, Phone
        public string? Role { get; set; }       // Lọc chính xác
        public string? Status { get; set; }     // Lọc chính xác
        public bool? IsActive { get; set; }     // Lọc theo trạng thái
        public DateTime? FromDate { get; set; } // Lọc theo ngày tạo
        public DateTime? ToDate { get; set; }
    }
}
