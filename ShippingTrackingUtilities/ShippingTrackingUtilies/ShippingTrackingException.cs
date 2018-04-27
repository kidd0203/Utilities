using System;
using System.Runtime.Serialization;

namespace ShippingTrackingUtilities
{
    [Serializable]
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

        protected ShippingTrackingException (SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}