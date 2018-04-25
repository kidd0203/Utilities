using System;

namespace ShippingTrackingUtilities
{
    public class ShippingTrackingException : Exception
    {
        public string ResponseXml { get; }

        public ShippingTrackingException(string message, string responseXml) : base(message)
        {
            ResponseXml = responseXml;
            Data.Add(nameof(ResponseXml), responseXml);
        }

        public ShippingTrackingException(string message) : base(message)
        {
        }

        public override string ToString()
        {
            return $"{nameof(ResponseXml)}: {ResponseXml}\r\n{base.ToString()}";
        }
    }
}