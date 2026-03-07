using System;

namespace Saja.Entities
{
    /// <summary>
    /// Represents a log entry for order status changes or actions.
    /// </summary>
    public class OrderLog
    {
        public int LogId { get; set; }
        public int OrderId { get; set; }
        public int? AdminId { get; set; }
        public string Action { get; set; }
        public string StatusFrom { get; set; }
        public string StatusTo { get; set; }
        public string Notes { get; set; }
        public DateTime LogDate { get; set; }

        public OrderLog() { }

        public override string ToString()
        {
            return $"OrderLog: Order {OrderId}, Action: {Action}, Date: {LogDate}";
        }
    }
}
