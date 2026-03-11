namespace GZone.Service.BusinessModels.Request.Order
{
    public class OrderPatchRequest
    {
        // Status update
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public string? CancellationReason { get; set; }

        // Shipping update
        public string? TrackingNumber { get; set; }
        public DateTime? EstimatedDelivery { get; set; }

        // Staff assignment
        public Guid? ManagedByStaffId { get; set; }
    }
}
