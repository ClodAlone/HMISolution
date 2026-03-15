using log4net;
using log4net.Config;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace ipldtestwindows
{
    class Program
    {
        static bool bExit;
        static readonly ILog log = LogManager.GetLogger("License");

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

#if DEBUG
            Console.WriteLine("if you would like to attach a debugger now is the right moment !");
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
#endif

            Console.CancelKeyPress += (s, e) =>
            {
                bExit = true;
            };

            try
            {
                int counter = 0;
                while (!bExit)
                {
                    var ipldVersion = ipldGetVersion();
                    Console.WriteLine("IPLD Library Version: {0}.{1}", (byte)(ipldVersion >> 8), (byte)(ipldVersion & 0x00FF));

                    uint lResultOut;
                    string json_bufferOut;
                    var license = ReadDaemon(out lResultOut, out json_bufferOut);
                    if (!String.IsNullOrEmpty(license))
                    {
                        var message = String.Format("License Valid!\nLicenseinfo: Result: {0}, JSON: {1}", lResultOut, json_bufferOut);
                        log.Info(message);
                        Console.WriteLine(message);
                    }
                    else
                    {
                        var message = String.Format("License Invalid!\nLicenseinfo: Result: {0}, JSON: {1}", lResultOut, json_bufferOut);
                        log.Error(message);
                        Console.WriteLine(message);
                    }

                    Console.WriteLine("Executed #{0}, Press Ctrl-C to exit...\n", ++counter);

                    if (Properties.Settings.Default.MaxCycleCount > 0 && counter >= Properties.Settings.Default.MaxCycleCount)
                        break;
                    
                    System.Threading.Thread.Sleep(Properties.Settings.Default.SleepCycleTime);
                }
            }
            catch (Exception ex)
            {
                log.Debug("Exception occurred", ex);
                Console.WriteLine("Exception occurred\n{0}\nPress enter to exit...", ex.Message);
                Console.ReadLine();
            }
        }

        static string ReadDaemon(out uint lResultOut, out string json_bufferOut)
        {
            bool bSuccess = false;
            uint jsonBufferSize = IPLD_DEFAULT_JSON_BUFFSIZE;
            uint lResult = 0;
            var json_buffer = new StringBuilder((int)jsonBufferSize);

            try
            {
                bSuccess = ipldGetLicStatus(Properties.Settings.Default.FileFormatVersion, json_buffer, ref jsonBufferSize, ref lResult);
                if (!bSuccess && lResult == (UInt32)ipldResult.IPLD_ERROR_NOT_ENOUGH_MEMORY)
                {
                    log.WarnFormat("IPLD_ERROR_NOT_ENOUGH_MEMORY with buffer size {0}, will retry with new buffer size {1}", IPLD_DEFAULT_JSON_BUFFSIZE, jsonBufferSize);
                    json_buffer = new StringBuilder((int)jsonBufferSize);
                    bSuccess = ipldGetLicStatus(Properties.Settings.Default.FileFormatVersion, json_buffer, ref jsonBufferSize, ref lResult);
                }

                if (bSuccess && lResult == (UInt32)ipldResult.IPLD_NO_ERROR)
                {
                    var licenseInfo = JObject.Parse(json_buffer.ToString());
                    var skuType = licenseInfo["sku1"];
                    var doc = System.Xml.Linq.XDocument.Parse(Properties.Settings.Default.LicenseSKUFile);
                    var expandoObject = new System.Dynamic.ExpandoObject();
                    var element = doc.Root.XPathSelectElement(String.Format("//sku{0}", skuType));
                    if (element != null)
                    {
                        XmlDocument xDoc = new XmlDocument();
                        XmlDeclaration xDecl = xDoc.CreateXmlDeclaration("1.0", System.Text.Encoding.UTF8.WebName, null);
                        xDoc.AppendChild(xDecl);

                        XmlElement xRoot = xDoc.CreateElement("Modules");
                        xDoc.AppendChild(xRoot);

                        //SiteCode
                        //XmlElement xElemL0 = xDoc.CreateElement(string.Format("{0}", "SiteCode"));
                        //xElemL0.InnerText = licenseInfo["mac"].Value<String>();
                        //xRoot.AppendChild(xElemL0);
                        foreach (var child in element.Descendants())
                        {
                            var xElem = xDoc.CreateElement(child.Name.LocalName);
                            xElem.InnerText = child.Value;
                            xRoot.AppendChild(xElem);
                        }

                        return xDoc.InnerXml;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Debug("Exception", ex);
            }
            finally
            {
                lResultOut = lResult;
                json_bufferOut = json_buffer.ToString();
            }

            return string.Empty;
        }

#region Daemon-Emerson API
        internal enum ipldResult : UInt32
        {
            IPLD_NO_ERROR,                      // Success (function returned TRUE)
            IPLD_ERROR_BAD_ARGUMENTS,           // Argument validation failed
            IPLD_ERROR_NOT_ENOUGH_MEMORY,       // Size of json_buffer passed in too small, on return bufsize holds the required size
            IPLD_ERROR_PRODUCT_VERSION,         // Mismatch of Progea version passed in and version in license file
            IPLD_ERROR_FILE_NOT_FOUND,          // License file or signature file not found
            IPLD_ERROR_PUBLIC_KEY,              // Error related to public key, e.g. public key for verification not found
            IPLD_ERROR_VERIFICATION,            // Signature verification error
            IPLD_ERROR_LICVALIDURL,             // Linux specific: License validation URL not found (http://emerson-nginx/emerson/pacedge/license/licensevalid.json)
            IPLD_ERROR_LICURL,                  // Linux specific: License URL not found (http://emerson-nginx/emerson/pacedge/license/license.json)
            IPLD_ERROR_PARSE,                   // Error parsing license JOSON data
            IPLD_ERROR_PLATFORM,                // The license check detected a HW platform not accepted
            IPLD_ERROR_HW_LIC_MISMATCH,         // The license file was created for another HW unit
            IPLD_ERROR_FILE,                    // Unspecified error related to file handling (read/write/open/close)
            IPLD_ERROR_MEMALLOC,                // Allocation error for dynamically allocated memory for internal usage
            IPLD_ERROR_UNSPECIFIED              // Unspecified error
        }

        internal const UInt32 IPLD_DEFAULT_JSON_BUFFSIZE = 1024;

        [DllImport("libipld.dll")]
        internal static extern bool ipldGetLicStatus(
                                    [In] String progea_vsn_string,
                                    [Out] StringBuilder json_buffer,
                                    [In, Out] ref uint bufsize,
                                    [In, Out] ref uint result);

        [DllImport("libipld.dll")]
        internal static extern ushort ipldGetVersion();

        [DllImport("libipld.dll")]
        internal static extern bool ipldSetDebug([In] int val);

        [DllImport("libipld.dll")]
        internal static extern bool ipldSetLicenseUrl([In] String url);

        [DllImport("libipld.dll")]
        internal static extern bool ipldSetLicenseValidUrl([In] String url);
#endregion
    }
}
