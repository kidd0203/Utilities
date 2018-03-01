using System;

namespace ShippingTrackingUtilities
{
    public class ShippingTrackingException : Exception
    {
        public string ResponseXml { get; }

        public ShippingTrackingException(string message, string responseXml) : base(message)
        {
            ResponseXml = responseXml;
        }

        public ShippingTrackingException(string message) : base(message)
        {
        }
    }
}