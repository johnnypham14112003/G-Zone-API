namespace GZone.Service.BusinessModels.Request.Notification
{
    public class NotificationQuery
    {
        public string? Keyword { get; set; }
        public string? NotificationType { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? CustomizationId { get; set; }
        public Guid? WarrantyClaimId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SortBy { get; set; } // newest | oldest
    }
}
