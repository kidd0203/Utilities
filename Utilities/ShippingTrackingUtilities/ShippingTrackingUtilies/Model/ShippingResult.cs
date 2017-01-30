using System.Collections.Generic;

namespace ShippingTrackingUtilities
{
    public class ShippingResult
    {
        public bool Delivered { get; set; }
        public string ServiceType { get; set; }
        public string PickupDate { get; set; }
        public string ScheduledDeliveryDate { get; set; }
        public string StatusCode { get; set; }
        public string Status { get; set; }
        public string StatusSummary { get; set; }  
        //USPS e.g. Your item was delivered to an individual at the address at 12:33 pm on July 28, 2016 in MURRAY, KY 42071.
        //UPS e.g. FRONT DOOR
        public string Message { get; set; }   //UPS e.g. On Time

        public string DeliveredDateTime { get; set; }

        public string SignatureName { get; set; }
        public ShippingResult()
        {
            Delivered = false;
            ServiceType = string.Empty;
            PickupDate = string.Empty;
            ScheduledDeliveryDate = string.Empty;
            StatusCode = string.Empty;
            Status = string.Empty;
            StatusSummary = string.Empty;
            Message = string.Empty;
            DeliveredDateTime = string.Empty;
            // TrackingDetails = new List<USPSTrackingResult.TrackResponseTrackInfoTrackDetail>();
            TrackingDetails = new List<ShippingResultEventDetail>();
            SignatureName = string.Empty;
        }

        public List<ShippingResultEventDetail> TrackingDetails { get; set; }
        //public List<USPSTrackingResult.TrackResponseTrackInfoTrackDetail> TrackingDetails { get; set; }
    }
}
