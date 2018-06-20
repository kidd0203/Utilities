using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using ShippingTrackingUtilities.UPSTrackingResult;

#pragma warning disable S1075 // URIs should not be hardcoded
namespace ShippingTrackingUtilities
{
    public class UPSTracking : ITrackingFacility
    {
        private readonly string trackingNumber;

        public UPSTracking(string trackingNumber)
        {
            this.trackingNumber = trackingNumber;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType) 3072;
        }

        public ShippingResult GetTrackingResult()
        {
            var shippingResult = new ShippingResult();
            var shippingResultInString = GetTrackingInfoUPSInString();

            if (shippingResultInString.Contains("<ResponseStatusDescription>Failure</ResponseStatusDescription>")
                && shippingResultInString.Contains("<Error><ErrorSeverity>Hard</ErrorSeverity>") &&
                !shippingResultInString.Contains("No tracking information available"))
            {
                throw new ShippingTrackingException("UPS API ERROR", shippingResultInString);
            }

            var serializer = new XmlSerializer(typeof(TrackResponse));
            TrackResponse resultingMessage;
            using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(shippingResultInString)))
            {
                resultingMessage = (TrackResponse) serializer.Deserialize(memStream);
            }

            shippingResult = UPSTrackingResultWrap(resultingMessage);

            return shippingResult;
        }

        private string UPSRequest(string url, string requestText)
        {
            // try request upto 3 times 
            var result = "";
            var encodedData = new ASCIIEncoding();
            var byteArray = encodedData.GetBytes(requestText);


            var call_tries = 0;
            var call_max = 3;
            var call_succeeded = false;

            while (call_tries < call_max && call_succeeded == false)
            {
                // try three times before giving up
                call_tries++;
                try
                {
                    var wr = (HttpWebRequest) WebRequest.Create(url);
                    wr.Method = "POST";
                    wr.KeepAlive = false;
                    wr.UserAgent = "Benz";
                    wr.ContentType = "application/x-www-form-urlencoded";
                    wr.ContentLength = byteArray.Length;

                    // send xml data
                    var SendStream = wr.GetRequestStream();
                    SendStream.Write(byteArray, 0, byteArray.Length);
                    SendStream.Close();

                    // get da response
                    var WebResp = (HttpWebResponse) wr.GetResponse();
                    using (var sr = new StreamReader(WebResp.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                        sr.Close();
                    }

                    WebResp.Close();

                    call_succeeded = true;
                }
                catch (Exception)
                {
                    Thread.Sleep(200);
                }
            }

            return result;
        }

        private string GetTrackingInfoUPSInString()
        {
            var apiUrl = "https://www.ups.com/ups.app/xml/Track"; // "https://wwwcie.ups.com/ups.app/xml/Track";

            var xml1 = "<?xml version=\"1.0\"?>" +
                       "<AccessRequest xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                       @"	<Password>%PASSWORD%</Password>
                <UserId>%USERID%</UserId>
            <AccessLicenseNumber>%ACCESSLICENSENUMBER%</AccessLicenseNumber>
            </AccessRequest>";


            var xml2 = @"<?xml version='1.0'?>
            <TrackRequest>
                <Request>
                    <TransactionReference>
            
                    </TransactionReference>
                    <RequestAction>Track</RequestAction>
                    <RequestOption>0</RequestOption>
                </Request>
                <TrackingNumber>%TN%</TrackingNumber>
            </TrackRequest>";

            //string raw_response = UPSRequest(
            //    apiUrl, 
            //    xml1.Replace("%PASSWORD%", ConnectionString.UPS_PASSWORD).Replace("%USERID%",ConnectionString.UPS_USER_ID).Replace("%ACCESSLICENSENUMBER%",ConnectionString.UPS_ACCESS_LICENSE_NO)
            //    + xml2.Replace("%TN%", trackingNumber));
            var raw_response = UPSRequest(
                apiUrl,
                xml1.Replace("%ACCESSLICENSENUMBER%", ConnectionString.UPS_ACCESS_LICENSE_NO)
                + xml2.Replace("%TN%", trackingNumber));

            return raw_response;
        }

        private ShippingResult UPSTrackingResultWrap(TrackResponse resultingMessage)
        {
            var shippingResult = new ShippingResult();

            var response = (TrackResponseResponse) resultingMessage.Items[0];
            if (response.ResponseStatusCode == "1")
            {
                var shipment = (TrackResponseShipment) resultingMessage.Items[1];
                shippingResult.ServiceType = shipment.Service[0].Description;
                shippingResult.StatusCode = shipment.Package[0].Activity[0].Status[0].StatusType[0].Code;
                shippingResult.Status = shipment.Package[0].Activity[0].Status[0].StatusType[0].Description;

                if (!string.IsNullOrEmpty(shippingResult.StatusCode) && shippingResult.StatusCode == "D")
                {
                    shippingResult.Delivered = true;
                    shippingResult.StatusSummary = shipment.Package[0].Activity[0].ActivityLocation[0].Description;
                    shippingResult.DeliveredDateTime =
                        shipment.Package[0].Activity[0].Date + " " + shipment.Package[0].Activity[0].Time;
                    shippingResult.SignatureName =
                        string.IsNullOrEmpty(shipment.Package[0].Activity[0].ActivityLocation[0].SignedForByName)
                            ? ""
                            : shipment.Package[0].Activity[0].ActivityLocation[0].SignedForByName;
                }
                
                if (!string.IsNullOrEmpty(shippingResult.StatusCode) && shippingResult.StatusCode != "D")
                {
                    shippingResult.PickupDate = shipment.PickupDate;
                    shippingResult.ScheduledDeliveryDate = shipment.ScheduledDeliveryDate;

                    if (shipment.Package[0].Message != null)
                    {
                        shippingResult.Message = shipment.Package[0].Message[0].Description;
                    }
                }
            }
            else
            {
                shippingResult.StatusCode = "Error";
                shippingResult.Status = response.Error.ErrorDescription;
                shippingResult.Message = response.Error.ErrorDescription;
            }

            return shippingResult;
        }
    }
}