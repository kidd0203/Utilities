namespace ShippingTrackingUtilities
{
    public static class ConnectionString
    {
        internal static string USPS_USERID = "";

        internal static string UPS_ACCESS_LICENSE_NO = "";

        internal static string FEDEX_USER_KEY = "";
        internal static string FEDEX_USER_PASSWORD = "";

        internal static bool ToShowDetails = false;

        public static void SetupUSPSCredential(string uspsUserID)
        {
            USPS_USERID = uspsUserID;
        }
        public static void SetupUPSCredential(string licenseNO)
        {
            UPS_ACCESS_LICENSE_NO = licenseNO;
        }

        public static void SetupFedExCredential(string userKey, string password)
        {
            FEDEX_USER_KEY = userKey;
            FEDEX_USER_PASSWORD = password;
        }
    }
}
