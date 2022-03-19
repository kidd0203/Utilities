using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace ShippingTrackingUtilities
{
    public class FedExTracking : ITrackingFacility
    {
        string trackingNumber;

        public FedExTracking(string trackingNumber)
        {
            this.trackingNumber = trackingNumber;
        }

        public ShippingResult GetTrackingResult()
        {
            ShippingResult shippingResult = new ShippingResult();

            string shippingResultInString = GetTrackingInfoFedExInString();

            XmlSerializer serializer = new XmlSerializer(typeof(FedExTrackingResult.TrackReply));
            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(shippingResultInString));

            FedExTrackingResult.TrackReply resultingMessage = new FedExTrackingResult.TrackReply();

            if (memStream != null)
                resultingMessage = (FedExTrackingResult.TrackReply)serializer.Deserialize(memStream);

            shippingResult = FedExTrackingResultWrap(resultingMessage);

            return shippingResult;
        }

        private ShippingResult FedExTrackingResultWrap(FedExTrackingResult.TrackReply resultMessage)
        {
            ShippingResult shippingResult = new ShippingResult();

            if (resultMessage.HighestSeverity == "ERROR")
            {
                shippingResult.StatusCode = "ERROR";
                shippingResult.Status = resultMessage.Notifications[0].Message;
                shippingResult.Message = resultMessage.Notifications[0].Message;

            }
            else if (resultMessage.HighestSeverity == "SUCCESS")
            {
                if (resultMessage.TrackDetails[0].StatusCode.Trim().ToUpper() == "DL")
                    shippingResult.Delivered = true;

                shippingResult.StatusCode = resultMessage.TrackDetails[0].StatusCode;
                shippingResult.Status = resultMessage.TrackDetails[0].StatusDescription;
                shippingResult.StatusSummary = resultMessage.TrackDetails[0].StatusDescription;
                shippingResult.ServiceType = resultMessage.TrackDetails[0].ServiceType;

                if (shippingResult.Delivered)
                {
                    shippingResult.DeliveredDateTime = !string.IsNullOrEmpty(resultMessage.TrackDetails[0].ActualDeliveryTimestamp) ? resultMessage.TrackDetails[0].ActualDeliveryTimestamp : "";
                    // by CJ on Oct-05-2016 to include Signature.
                    shippingResult.SignatureName = !string.IsNullOrEmpty(resultMessage.TrackDetails[0].DeliverySignatureName) ? resultMessage.TrackDetails[0].DeliverySignatureName : "";
                }
                else
                    shippingResult.ScheduledDeliveryDate = !string.IsNullOrEmpty(resultMessage.TrackDetails[0].EstimatedDeliveryTimestamp) ? resultMessage.TrackDetails[0].EstimatedDeliveryTimestamp : "";

                if (resultMessage.TrackDetails[0].Events != null)
                {
                    if(resultMessage.TrackDetails[0].Events.Length>0)
                    {
                        foreach (var detail in resultMessage.TrackDetails[0].Events)
                        {
                            ShippingResultEventDetail eventDetail = new ShippingResultEventDetail();
                            eventDetail.Event = detail.EventType + " " + detail.EventDescription; ;
                            eventDetail.EventDateTime = detail.Timestamp;
                            if (detail.Address !=null)
                                eventDetail.EventAddress = detail.Address[0].City + " " + detail.Address[0].StateOrProvinceCode + " " + detail.Address[0].PostalCode;

                            shippingResult.TrackingDetails.Add(eventDetail);
                        }
                    }
                }
            }

            return shippingResult;
        }

        private string GetTrackingInfoFedExInString()
        {
            string apiUrl = "https://gateway.fedex.com:443/xml"; 

            string xml = @"<TrackRequest xmlns='http://fedex.com/ws/track/v3'>"
                + @"<WebAuthenticationDetail>"
                + @"<UserCredential>"
                + @"<Key>%ACC_KEY%</Key>"
                + @"<Password>%ACC_PASSWORD%</Password>"
                + @"</UserCredential>"
                + @"</WebAuthenticationDetail>"
                + @"<ClientDetail>"
                + @"<AccountNumber>%ACC_NUMBER%</AccountNumber>"
                + @"<MeterNumber>%ACC_METER_NUMBER%</MeterNumber>"
                + @"</ClientDetail>"
                + @"<TransactionDetail><CustomerTransactionId>ActiveShipping</CustomerTransactionId></TransactionDetail>"
                + @"<Version><ServiceId>trck</ServiceId><Major>3</Major><Intermediate>0</Intermediate><Minor>0</Minor></Version>"
                + @"<PackageIdentifier><Value>%TRACKINGNO%</Value><Type>TRACKING_NUMBER_OR_DOORTAG</Type></PackageIdentifier>"
                + @"<IncludeDetailedScans>1</IncludeDetailedScans>"
                + @"</TrackRequest>";

            string raw_response = UPSRequest(apiUrl,
                xml.Replace("%ACC_KEY%", ConnectionString.FEDEX_USER_KEY).
                    Replace("%ACC_PASSWORD%", ConnectionString.FEDEX_USER_PASSWORD).
                    Replace("%TRACKINGNO%", trackingNumber).
                    Replace("%ACC_NUMBER%",ConnectionString.FEDEX_ACCOUNTNUMBER).
                    Replace("%ACC_METER_NUMBER%", ConnectionString.FEDEX_METERNUMBER));

            return raw_response;
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
    }
}
