using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
namespace ShippingTrackingUtilities
{
    public class UPSTracking :ITrackingFacility
    {
        string trackingNumber;

        public UPSTracking(string trackingNumber)
        {
            this.trackingNumber = trackingNumber;
	    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        }

        public ShippingResult GetTrackingResult()
        {
            ShippingResult shippingResult = new ShippingResult();
            string shippingResultInString = GetTrackingInfoUPSInString();
            XmlSerializer serializer = new XmlSerializer(typeof(UPSTrackingResult.TrackResponse));
            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(shippingResultInString));

            UPSTrackingResult.TrackResponse resultingMessage = new UPSTrackingResult.TrackResponse();

            if (memStream != null)
                resultingMessage = (UPSTrackingResult.TrackResponse)serializer.Deserialize(memStream);

            shippingResult = UPSTrackingResultWrap(resultingMessage);

            return shippingResult;

        }

        private string UPSRequest(string url, string requestText)
        {   // try request upto 3 times 
            string result = "";
            ASCIIEncoding encodedData = new ASCIIEncoding();
            byte[] byteArray = encodedData.GetBytes(requestText);


            int call_tries = 0;
            int call_max = 3;
            bool call_succeeded = false;

            while (call_tries < call_max && call_succeeded == false)
            {  // try three times before giving up
                call_tries++;
                try
                {
                    HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
                    wr.Method = "POST";
                    wr.KeepAlive = false;
                    wr.UserAgent = "Benz";
                    wr.ContentType = "application/x-www-form-urlencoded";
                    wr.ContentLength = byteArray.Length;

                    // send xml data
                    Stream SendStream = wr.GetRequestStream();
                    SendStream.Write(byteArray, 0, byteArray.Length);
                    SendStream.Close();

                    // get da response
                    HttpWebResponse WebResp = (HttpWebResponse)wr.GetResponse();
                    using (StreamReader sr = new StreamReader(WebResp.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                        sr.Close();
                    }

                    WebResp.Close();

                    call_succeeded = true;
                }
                catch (Exception e)
                {
                    System.Threading.Thread.Sleep(200);
                }
            }

            return result;
        }

        private string GetTrackingInfoUPSInString()
        {
            string apiUrl = "https://onlinetools.ups.com/ups.app/xml/Track"; 
            //string apiUrl = "https://www.ups.com/ups.app/xml/Track"; // "https://wwwcie.ups.com/ups.app/xml/Track";

            string xml1 = "<?xml version=\"1.0\"?>" +
            "<AccessRequest xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
            @"	<Password>%PASSWORD%</Password>
	            <UserId>%USERID%</UserId>
            <AccessLicenseNumber>%ACCESSLICENSENUMBER%</AccessLicenseNumber>
            </AccessRequest>";



            string xml2 = @"<?xml version='1.0'?>
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
            string raw_response = UPSRequest(
                apiUrl,
                xml1.Replace("%ACCESSLICENSENUMBER%", ConnectionString.UPS_ACCESS_LICENSE_NO)
                + xml2.Replace("%TN%", trackingNumber));

            return raw_response;
        }

        private ShippingResult UPSTrackingResultWrap(UPSTrackingResult.TrackResponse resultingMessage)
        {
            ShippingResult shippingResult = new ShippingResult();

            UPSTrackingResult.TrackResponseResponse response = (UPSTrackingResult.TrackResponseResponse)resultingMessage.Items[0];
            if (response.ResponseStatusCode == "1")
            {
                UPSTrackingResult.TrackResponseShipment shipment = (UPSTrackingResult.TrackResponseShipment)resultingMessage.Items[1];
                shippingResult.ServiceType = shipment.Service[0].Description;
                shippingResult.StatusCode = shipment.Package[0].Activity[0].Status[0].StatusType[0].Code;
                shippingResult.Status = shipment.Package[0].Activity[0].Status[0].StatusType[0].Description;

                if (!string.IsNullOrEmpty(shippingResult.StatusCode))
                    if (shippingResult.StatusCode == "D")
                    {
                        shippingResult.Delivered = true;
                        shippingResult.StatusSummary = shipment.Package[0].Activity[0].ActivityLocation[0].Description;
                        shippingResult.DeliveredDateTime = shipment.Package[0].Activity[0].Date + " " + shipment.Package[0].Activity[0].Time;
                        shippingResult.SignatureName = string.IsNullOrEmpty(shipment.Package[0].Activity[0].ActivityLocation[0].SignedForByName) ? "" : shipment.Package[0].Activity[0].ActivityLocation[0].SignedForByName;
                    }


                if (!string.IsNullOrEmpty(shippingResult.StatusCode))
                    if (shippingResult.StatusCode != "D")
                    {
                        shippingResult.PickupDate = shipment.PickupDate;
                        shippingResult.ScheduledDeliveryDate = shipment.ScheduledDeliveryDate;

                        if (shipment.Package[0].Message != null)
                            shippingResult.Message = shipment.Package[0].Message[0].Description;
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
