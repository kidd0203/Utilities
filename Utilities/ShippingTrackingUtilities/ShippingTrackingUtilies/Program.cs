using System;

namespace ShippingTrackingUtilities
{
    class Program
    {
        static void Main(string[] args)
        {
        //    bool toContinue = true;

        //    do
        //    {
        //        Console.WriteLine("Please enter the tracking #:");

        //        string trackingNo = Console.ReadLine();

        //        TrackingUtilities utils = new TrackingUtilities();
        //        ShippingResult shippingResult = utils.GetTrackingResult(trackingNo);

        //        Console.WriteLine("Tracking Result: ");

        //        string result = ComposeOutputResult(shippingResult);

        //        Console.WriteLine(result);

        //        Console.WriteLine("Press any key except X to continue.");

        //        string enteredKey = Console.ReadLine();

        //        if (enteredKey.Trim().ToUpper() == "X")
        //            toContinue = false;

        //    } while (toContinue);
            
        }

        //private static string ComposeOutputResult(ShippingResult shippingResult)
        //{
        //    string result = "Delivered: " + shippingResult.Delivered;
        //    result += "\nStatus: " + shippingResult.Status;
        //    result += "\nSummary: " + shippingResult.StatusSummary;

        //    if (shippingResult.Delivered)
        //        result += "\nDelivered On: " + shippingResult.DeliveredDateTime;
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(shippingResult.Message))
        //            result += "\nMessage: " + shippingResult.Message;
        //        if (!string.IsNullOrEmpty(shippingResult.ScheduledDeliveryDate))
        //            result += "\nScheduled Delivery Date: " + shippingResult.ScheduledDeliveryDate;
        //    }

        //    if (shippingResult.TrackingDetails.Count > 0)
        //    {
        //        result += "\nTracking Details: ";

        //        foreach (var detail in shippingResult.TrackingDetails)
        //        {
        //            result += "\nEventDateTime: " + detail.EventDateTime;
        //            result += "\tEvent: " + detail.Event;
        //            result += "\nEvent Address: " + detail.EventAddress;
        //        }
        //    }
        //    return result;
        //}
    }
}
