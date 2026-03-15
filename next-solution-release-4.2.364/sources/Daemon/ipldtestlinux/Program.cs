using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;


namespace ipldtestlinux
{
    class Program
    {
        static bool bExit;
        static void Main(string[] args)
        {
            // Register the import resolver before calling the imported function.
            // Only one import resolver can be set for a given assembly.
            //NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);

            try
            {
                Console.WriteLine("if you would like to attach a debugger now is the right moment !");
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();

                Console.CancelKeyPress += (s, e) =>
                {
                    bExit = true;
                };

                int counter = 0;
                while (!bExit)
                {
                    bool bSuccess = false;
                    uint lResult = 0;
                    uint jsonBufferSize = IPLD_DEFAULT_JSON_BUFFSIZE;
                    var json_buffer = new StringBuilder((int)jsonBufferSize);
                    String progea_vsn_string = "4.1.322";

                    var ipldVersion = ipldGetVersion();
                    Console.WriteLine("IPLD Library Version: {0}.{1}", (byte)(ipldVersion >> 8), (byte)(ipldVersion & 0x00FF));
                    //bSuccess = ipldSetLicenseUrl(@"http://emerson-nginx/emerson/pacedge/license/license.json");
                    //Console.WriteLine("ipldSetLicenseUrl, Result: {0}", bSuccess);
                    //bSuccess = ipldSetLicenseValidUrl(@"http://emerson-nginx/emerson/pacedge/license/licensevalid.json");
                    //Console.WriteLine("ipldSetLicenseValidUrl, Result: {0}", bSuccess);
                    bSuccess = ipldSetDebug(1);
                    Console.WriteLine("ipldSetDebug, Result: {0}", bSuccess);
                    bSuccess = ipldGetLicStatus(progea_vsn_string, json_buffer, ref jsonBufferSize, ref lResult);
                    if (!bSuccess && lResult == (UInt32)ipldResult.IPLD_ERROR_NOT_ENOUGH_MEMORY)
                    {
                        json_buffer = new StringBuilder((int)jsonBufferSize);
                        bSuccess = ipldGetLicStatus(progea_vsn_string, json_buffer, ref jsonBufferSize, ref lResult);
                    }

                    if (bSuccess && lResult == (UInt32)ipldResult.IPLD_NO_ERROR)
                    {
                        Console.WriteLine("License Valid!\nLicenseinfo:\n{0}\n", json_buffer.ToString());
                    }
                    else
                    {
                        Console.WriteLine("License Invalid!\nResult: {0}\nLicenseinfo:\n{1}\n", json_buffer.ToString());
                    }

                    Console.WriteLine("Executed #{0}, Press Ctrl-C to exit...", ++counter);
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        ////static IntPtr libCurl;
        //private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        //{
        //    if (libraryName == "libipld" && libCurl == IntPtr.Zero)
        //    {
        //        //NativeLibrary.TryLoad("/usr/lib/x86_64-linux-gnu/libcurl.so.4.6.0", out libCurl);
        //        return NativeLibrary.Load("libipld", assembly, searchPath);
        //    }

        //    // Otherwise, fallback to default import resolver.
        //    return IntPtr.Zero;
        //}

        #region Daemon-Emerson API
        internal enum ipldResult : UInt32
        {
            IPLD_NO_ERROR,
            IPLD_ERROR_BAD_ARGUMENTS,
            IPLD_ERROR_NOT_ENOUGH_MEMORY,
            IPLD_ERROR_PRODUCT_VERSION,
            IPLD_ERROR_FILE_NOT_FOUND,
            IPLD_ERROR_PUBLIC_KEY,
            IPLD_ERROR_VERIFICATION,
            IPLD_ERROR_LICVALIDURL,
            IPLD_ERROR_LICURL,
            IPLD_ERROR_PARSE
        }

        internal const UInt32 IPLD_DEFAULT_JSON_BUFFSIZE = 1024;

        [DllImport("libipld")]
        internal static extern bool ipldGetLicStatus(
                                    [In] String progea_vsn_string,
                                    [Out] StringBuilder json_buffer,
                                    [In, Out] ref uint bufsize,
                                    [In, Out] ref uint result);

        [DllImport("libipld")]
        internal static extern ushort ipldGetVersion();

        [DllImport("libipld")]
        internal static extern bool ipldSetDebug([In] int val);

        [DllImport("libipld")]
        internal static extern bool ipldSetLicenseUrl([In] String url);

        [DllImport("libipld")]
        internal static extern bool ipldSetLicenseValidUrl([In] String url);
        #endregion
    }
}
