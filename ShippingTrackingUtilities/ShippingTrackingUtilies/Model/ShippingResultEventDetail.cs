namespace ShippingTrackingUtilities
{
    public class ShippingResultEventDetail
    {
        public string EventDateTime{ get; set; }
        public string Event{ get; set; }
        public string EventAddress{ get; set; }

        public ShippingResultEventDetail()
	    {
            EventDateTime = string.Empty;
            Event = string.Empty;
            EventAddress = string.Empty;
	    }
    }
    
}
