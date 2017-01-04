using System;
using UPSTimeInTransit;

namespace ConsoleSampleProject
{
    class Program
    {
        static void Main(string[] args)
        {      
            TimeInTransitHelper tntHelper = new TimeInTransitHelper("LicenseNo", "UserID", "Password");
            var availableServices = tntHelper.GetAvailableShippingService("22310", new DateTime(2017, 1, 7));

            if (availableServices.Count > 0)
            {
                foreach (var item in availableServices)
                {
                    Console.WriteLine(item.ServiceName + "\t" + item.ExpectedDeliverDate + "\t" + (item.IsSaturday ? "IsSaturday" : "")
                        + "\t" + "Total Days: " + item.TotalDays.ToString() + "\t" + item.DayOfWeek);
                }
            }
            else
            {
                Console.WriteLine(tntHelper.GetError);
            }

            Console.ReadKey();
        }
    }
}
