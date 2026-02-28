namespace GZone.Service.BusinessModels.Request.Notification
{
    public class NotificationRequest
    {
        public string NotificationType { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public Guid OrderId { get; set; }
        public Guid CustomizationId { get; set; }
        public Guid WarrantyClaimId { get; set; }
    }
}
