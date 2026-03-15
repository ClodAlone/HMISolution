using System;

namespace ActiproUtilities
{
    public static class ActiproUtilities
    {
        #region Declarations
        static readonly object staticLock = new object();

        static string licensee = "Progea srl";
        static string licenseKey = "WPF211-PP54X-K03E4-6WQ54-0KCG";
        static bool isLicenseRegistered;
        #endregion

        #region Public Static Methods
        public static void RegisterLicense()
        {
            lock (staticLock)
            {
                if (isLicenseRegistered)
                    return;
                isLicenseRegistered = true;

                ActiproSoftware.Products.ActiproLicenseManager.RegisterLicense(licensee, licenseKey);
            }
        }
        #endregion
    }
}
