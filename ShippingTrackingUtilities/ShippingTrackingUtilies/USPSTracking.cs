using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;


namespace ShippingTrackingUtilities
{
    public class USPSTracking : ITrackingFacility
    {
        readonly string trackingNumber;

        public USPSTracking(string trackingNumber)
        {
            this.trackingNumber = trackingNumber;
        }

        public ShippingResult GetTrackingResult()
        {
            ShippingResult shippingResult;

            var shippingResultInString = GetTrackingInfoUSPSInString();

            using (MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(shippingResultInString)))
            {
                if (shippingResultInString.Contains("<Error>") && !shippingResultInString.Contains("<TrackResponse>"))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(USPSTrackingResultError.Error));
                    var error = (USPSTrackingResultError.Error)serializer.Deserialize(memStream);
                    shippingResult = USPSTrackingResultErrorWrap(error);
                } 
                else
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(USPSTrackingResult.TrackResponse));
                    var resultingMessage = (USPSTrackingResult.TrackResponse)serializer.Deserialize(memStream);
                    shippingResult = USPSTrackingResultWrap(resultingMessage);
                }
            }

            return shippingResult;
        }

        private string GetTrackingInfoUSPSInString()
        {
            string USPS_USERID = ConnectionString.USPS_USERID;

            string BASEURL = "http://production.shippingapis.com/ShippingAPI.dll";

            string response = string.Empty;

            try
            {
                var usps = new StringBuilder(BASEURL).AppendFormat(
                    "?API=TrackV2&XML=<TrackFieldRequest USERID=\"{0}\">", USPS_USERID);
                usps.Append("<Revision>1</Revision>");
                // TODO: Don't use 127.0.0.1
                usps.Append("<ClientIp>" + "127.0.0.1" + "</ClientIp>");
                usps.Append("<SourceId>" + "SV" + "</SourceId>");
                usps.Append("<TrackID ID=\"" + trackingNumber + "\"></TrackID>");
                usps.Append("</TrackFieldRequest>");


                byte[] responseData;
                using (WebClient wsClient = new WebClient())
                {
                    responseData = wsClient.DownloadData(usps.ToString());
                }

                foreach (byte item in responseData)
                {
                    response += (char)item;
                }

                if (string.IsNullOrEmpty(response))
                    return null;
            }
            catch (Exception)
            {
                return null;
            }

            return response;

        }

        private ShippingResult USPSTrackingResultErrorWrap(USPSTrackingResultError.Error resultingMessage)
        {
            ShippingResult shippingResult = new ShippingResult();

            shippingResult.StatusCode = "ERROR";
            shippingResult.Status = resultingMessage.Description;
            shippingResult.Message = resultingMessage.Description;

            return shippingResult;
        }

        private ShippingResult USPSTrackingResultWrap(USPSTrackingResult.TrackResponse resultingMessage)
        {
            ShippingResult shippingResult = new ShippingResult();

            shippingResult.ServiceType = resultingMessage.Items[0].Class;
            shippingResult.StatusCode = resultingMessage.Items[0].StatusCategory;
            shippingResult.Status = resultingMessage.Items[0].Status;
            shippingResult.StatusSummary = resultingMessage.Items[0].StatusSummary;

            if (resultingMessage.Items[0].Error != null && !string.IsNullOrEmpty(resultingMessage.Items[0].Error.Number.ToString()))
            {
                shippingResult.Delivered = false;
                shippingResult.StatusCode = "Error";
                shippingResult.Status = resultingMessage.Items[0].Error.Description;
                shippingResult.StatusSummary = resultingMessage.Items[0].Error.Description;
                shippingResult.Message = resultingMessage.Items[0].Error.Description;
            }
            
            if (!string.IsNullOrEmpty(shippingResult.StatusCode) && shippingResult.StatusCode.ToUpper().Trim() == "DELIVERED")
            {
                shippingResult.Delivered = true;
                foreach (var item in resultingMessage.Items[0].TrackSummary)
                {
                    if (item.Event.ToUpper().Contains("DELIVERED"))
                    {
                        shippingResult.DeliveredDateTime = item.EventDate + " " + item.EventTime;

                        //by CJ on Oct-05-2016, to have the signatureName.
                        shippingResult.SignatureName = string.IsNullOrEmpty(item.Name) ? "" : item.Name;

                        break;
                    }
                }
            }

            if (resultingMessage.Items[0].TrackDetail != null && resultingMessage.Items[0].TrackDetail.Length > 0)
            {
                foreach (var detail in resultingMessage.Items[0].TrackDetail)
                {
                    ShippingResultEventDetail eventDetail = new ShippingResultEventDetail();
                    eventDetail.Event = detail.Event;
                    eventDetail.EventDateTime = detail.EventDate + " " + detail.EventTime;
                    eventDetail.EventAddress = detail.EventCity + " " + detail.EventState + " " + detail.EventZIPCode;
                    shippingResult.TrackingDetails.Add(eventDetail);
                }      
            }

            return shippingResult;
        }
    }
}
