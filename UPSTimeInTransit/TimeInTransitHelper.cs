using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Serialization;

namespace UPSTimeInTransit
{
    public class TimeInTransitHelper
    {
        private const string defaultShipFromZip = "11220";
        
        private string licenseNo;
        private string username;
        private string password;
        private string shipFromZip;
        private string error;


        public string GetError { get { return error; } }
        public TimeInTransitHelper(string license, string username,string password)
            : this(license, username, password,defaultShipFromZip)
        {}

        public TimeInTransitHelper(string license, string username, string password,string shipFromZip)
        {
            this.licenseNo = license;
            this.username = username;
            this.password = password;
            this.shipFromZip = shipFromZip;
            error = "";
        }

        public List<ShippingService> GetAvailableShippingService(string shipToZip, DateTime expectedDeliveryDate)
        {
            List<ShippingService> availableService = new List<ShippingService>();

            try
            {
                TimeInTransitResponse result = GetUPSEstimatedDeliveryDate(shipToZip);

                if (result != null)
                {
                    if (!string.IsNullOrEmpty(result.Response.ResponseStatusCode))
                    {
                        if (result.Response.ResponseStatusCode == "1")
                        {
                            if (result.Items.Length > 0)
                            {
                                TransitResponseType response = (TransitResponseType)result.Items[0];

                                if (response.ServiceSummary.Length > 0)
                                {
                                    var services = response.ServiceSummary
                                        .Where(s => Convert.ToDateTime(s.EstimatedArrival.Date) <= expectedDeliveryDate)
                                        .OrderByDescending(s => s.Service.Code).ToList();

                                    if (services.Count > 0)
                                    {
                                        for (int i = 0; i < services.Count; i++)
                                        {
                                            ShippingService service = new ShippingService();
                                            service.ExpectedDeliverDate = Convert.ToDateTime(services[i].EstimatedArrival.Date);
                                            service.ServiceName = services[i].Service.Description;
                                            service.DayOfWeek = services[i].EstimatedArrival.DayOfWeek;
                                            service.TotalDays = Convert.ToInt16(services[i].EstimatedArrival.BusinessTransitDays);
                                            if (!string.IsNullOrEmpty(services[i].SaturdayDelivery))
                                                if (services[i].SaturdayDelivery == "1")
                                                    service.IsSaturday = true;

                                            if (services[i].Guaranteed.Length > 0)
                                                if (!string.IsNullOrEmpty(services[i].Guaranteed[0].Code))
                                                    if (services[i].Guaranteed[0].Code == "Y")
                                                        service.Guaranteed = true;

                                            availableService.Add(service);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                error += "No available service returned.\n";
                            }
                        }
                        else
                        {
                            error += !string.IsNullOrEmpty(result.Response.Error[0].ErrorDescription) ? result.Response.Error[0].ErrorDescription +"\n": "Unknown Error.\n";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error += ex.Message + "\n";
            }
            
            

            return availableService;
        }

        private TimeInTransitResponse GetUPSEstimatedDeliveryDate(string shipToZip)
        {
            TimeInTransitResponse result = new TimeInTransitResponse();

            Uri url = new Uri("https://onlinetools.ups.com/ups.app/xml/TimeInTransit");
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/xml; charset=utf-8";
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/xml; charset=utf-8";

            string requestXMLInString = InitializeAccess();
            requestXMLInString += ComposeRequest(shipToZip);

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                requestXMLInString = requestXMLInString.Replace("\r\n", "");
                streamWriter.Write(requestXMLInString);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TimeInTransitResponse));
                result = (TimeInTransitResponse)serializer.Deserialize(streamReader);
            }

            return result;
        }

        private string InitializeAccess()
        {
            AccessRequest accessRequest = new AccessRequest()
            {
                AccessLicenseNumber = licenseNo,
                UserId = username,
                Password = password
            };

            XmlSerializer xsSubmitAccessRequest = new XmlSerializer(typeof(AccessRequest));
            string xml = "";
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmitAccessRequest.Serialize(writer, accessRequest);
                    xml = sww.ToString(); // Your XML
                }
            }
            return xml;
        }

        private string ComposeRequest(string shipToZip)
        {
            TimeInTransitRequest request = new TimeInTransitRequest();

            request.Request = new Request()
            {
                TransactionReference = new TransactionReference(),
                RequestAction = "TimeInTransit"
            };

            request.TransitFrom = new TransitFromType()
            {
                AddressArtifactFormat = new AddressArtifactFormatType()
                {
                    PostcodePrimaryLow = shipFromZip,
                    CountryCode = "US"
                }
            };
            request.TransitTo = new TransitToType()
            {
                AddressArtifactFormat = new TransitToAddressArtifactFormatType()
                {
                    PostcodePrimaryLow = shipToZip,
                    CountryCode = "US"
                }
            };

            request.PickupDate = DateTime.Now.ToString("yyyyMMdd");


            XmlSerializer xsSubmitAccessRequest = new XmlSerializer(typeof(TimeInTransitRequest));
            string xml = "";
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmitAccessRequest.Serialize(writer, request);
                    xml = sww.ToString(); // Your XML
                }
            }

            return xml;
        }
    }
}
