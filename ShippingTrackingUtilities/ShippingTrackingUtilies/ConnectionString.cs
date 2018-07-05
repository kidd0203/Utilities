namespace ShippingTrackingUtilities
{
    public static class ConnectionString
    {
        internal static string USPS_USERID { get; private set; } = string.Empty;

        internal static string UPS_ACCESS_LICENSE_NO { get; private set; } = string.Empty;

        internal static string FEDEX_USER_KEY { get; private set; } = string.Empty;

        internal static string FEDEX_USER_PASSWORD { get; private set; } = string.Empty;

        internal static readonly bool ToShowDetails = false;

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
