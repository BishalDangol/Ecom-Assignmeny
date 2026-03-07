using System;

namespace Saja.Entities
{
    /// <summary>
    /// Represents a shipping or billing address for a member.
    /// </summary>
    public class MemberAddress
    {
        public int AddressId { get; set; }
        public int MemberId { get; set; }
        public string RecipientName { get; set; }
        public string Phone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public int DistrictId { get; set; }
        public string PostalCode { get; set; }
        public bool IsDefault { get; set; }

        // Navigation Property
        public District District { get; set; }

        public MemberAddress() { }

        public MemberAddress(int addressId, int memberId, string addressLine1, int districtId)
        {
            AddressId = addressId;
            MemberId = memberId;
            AddressLine1 = addressLine1;
            DistrictId = districtId;
        }

        public override string ToString()
        {
            return $"{RecipientName}, {AddressLine1}, {AddressLine2} (District ID: {DistrictId})";
        }
    }
}
