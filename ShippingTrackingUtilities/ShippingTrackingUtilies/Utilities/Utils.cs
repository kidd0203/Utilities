using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShippingTrackingUtilities.Utilities
{
    public static class Utils
    {
        public static StringBuilder sbErrMsgs = new StringBuilder();

        public static void LogException(Exception ex)
        {        
            try
            {
                StreamWriter sw = new StreamWriter(@"ZuckersReport_ErrorLog.txt", true);
                sw.WriteLine("");
                sw.WriteLine("");
                sw.WriteLine(DateTime.UtcNow.ToString());
                sw.WriteLine("Error Message: " + ex.Message);
                sw.WriteLine("STrace: " + ex.StackTrace);
                if (ex.InnerException != null)
                    sw.WriteLine("InnerException: " + ex.InnerException.ToString());
                sw.WriteLine(new string('-', 200));
                sw.Close();

                //put the current exeption message into the Message Variable to send email...
                Utils.sbErrMsgs.Append(DateTime.UtcNow.ToString() + ex.Message + "\n");
                Utils.sbErrMsgs.Append(DateTime.UtcNow.ToString() + ex.StackTrace + "\n");
            }
            catch { }
        }
    }
}
