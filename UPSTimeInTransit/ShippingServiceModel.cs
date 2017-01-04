using System;

namespace UPSTimeInTransit
{
    public class ShippingService
    {
        public string ServiceName { get; set; }
        public DateTime ExpectedDeliverDate { get; set; }
        public string DayOfWeek { get; set; }
        public int TotalDays { get; set; }
        public bool Guaranteed { get; set; }
        public bool IsSaturday { get; set; }
    }
}
