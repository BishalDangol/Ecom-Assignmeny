using System;

namespace Saja.Entities
{
    /// <summary>
    /// Represents a vendor/seller on the platform.
    /// </summary>
    public class Vendor
    {
        public int VendorId { get; set; }
        public string BusinessName { get; set; }
        public string OwnerName { get; set; }
        public int DistrictId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Story { get; set; }
        public string ImageUrl { get; set; }
        public bool Verified { get; set; }
        public decimal CommissionRate { get; set; }
        public DateTime CreatedAt { get; set; }

        public Vendor() { }

        public Vendor(int vendorId, string businessName, bool verified)
        {
            VendorId = vendorId;
            BusinessName = businessName;
            Verified = verified;
        }

        public override string ToString()
        {
            return $"Vendor: {BusinessName} (Verified: {Verified})";
        }
    }
}
