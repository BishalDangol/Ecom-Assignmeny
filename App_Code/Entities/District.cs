using System;

namespace Saja.Entities
{
    /// <summary>
    /// Represents a district in Nepal for delivery and vendor location.
    /// </summary>
    public class District
    {
        public int DistrictId { get; set; }
        public string Name { get; set; }
        public string Zone { get; set; }
        public string Province { get; set; }
        public decimal DeliveryCharge { get; set; }
        public int DeliveryDays { get; set; }
        public bool IsActive { get; set; }

        public District() { }

        public District(int districtId, string name, decimal deliveryCharge)
        {
            DistrictId = districtId;
            Name = name;
            DeliveryCharge = deliveryCharge;
        }

        public override string ToString()
        {
            return $"District: {Name} (Charge: NPR {DeliveryCharge})";
        }
    }
}
