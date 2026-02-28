namespace GZone.Service.BusinessModels.Response.Notification
{
    public class NotificationResponse
    {
        public Guid NotificationId { get; set; }
        public string NotificationType { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public Guid OrderId { get; set; }
        public Guid CustomizationId { get; set; }
        public Guid WarrantyClaimId { get; set; }
    }
}
