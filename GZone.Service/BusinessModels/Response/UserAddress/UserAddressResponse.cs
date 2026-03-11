namespace GZone.Service.BusinessModels.Response.UserAddress
{
    public class UserAddressResponse
    {
        public Guid AddressId { get; set; }

        public string AddressLabel { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }

        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Foreign keys
        public Guid AccountId { get; set; }
    }
}
