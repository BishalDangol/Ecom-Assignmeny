using System;

namespace Saja.Entities
{
    /// <summary>
    /// Represents the snapshot of the shipping address at the time of order.
    /// </summary>
    public class OrderAddress
    {
        public int AddressId { get; set; }
        public int OrderId { get; set; }
        public string RecipientName { get; set; }
        public string Phone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public int DistrictId { get; set; }
        public string PostalCode { get; set; }

        public OrderAddress() { }

        public override string ToString()
        {
            return $"{RecipientName}, {AddressLine1} (Order ID: {OrderId})";
        }
    }
}
