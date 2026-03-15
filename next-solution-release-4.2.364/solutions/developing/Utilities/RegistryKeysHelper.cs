using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class RegistryKeysHelper
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static byte[] registryKeysHelperValue { get; private set; } // bytes value for 'MSZ.dll'
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static byte[] registryKeysHelperValue2 { get; private set; } // bytes value for 'libipld.dll' on Windows OS and 'libipld.so' on Linux OS

        #region Static Constructors
#if !DEBUG
        static RegistryKeysHelper()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var assemblyPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(assembly.Location),
                    WPFUtilities.CryptString.CryptString.DecryptString("u2MUIZoiwsHg0ARAi4/J/w==")); // "MSZ.dll"
                registryKeysHelperValue = System.IO.File.ReadAllBytes(assemblyPath);
            }
            catch
            { }

            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            //var isOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (isWindows)
            {
                try
                {
                    var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    var assemblyPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(assembly.Location),
                        WPFUtilities.CryptString.CryptString.DecryptString("gJiMRm7TQyYYsidOA1MkUw==")); // "libipld.dll"
                    registryKeysHelperValue2 = System.IO.File.ReadAllBytes(assemblyPath);
                }
                catch
                {
                    try
                    {
                        var assemblyPath = System.IO.Path.Combine(Environment.SystemDirectory,
                            WPFUtilities.CryptString.CryptString.DecryptString("gJiMRm7TQyYYsidOA1MkUw==")); // "libipld.dll"
                        registryKeysHelperValue2 = System.IO.File.ReadAllBytes(assemblyPath);
                    }
                    catch
                    { }
                }
            }
            else if (isLinux)
            {
                try
                {
                    var assemblyPath = System.IO.Path.Combine(@"/usr/lib",
                        WPFUtilities.CryptString.CryptString.DecryptString("Ubs2SKddQzTv+Brf/YY+iw==")); // "libipld.so"
                    registryKeysHelperValue2 = System.IO.File.ReadAllBytes(assemblyPath);
                }
                catch
                { }
            }
        }
#endif
        #endregion

        #region Static Properties
        public static string SoftwareProductKey
        {
            get
            {
                var product = !String.IsNullOrWhiteSpace(AssemblyInfo.Product) ? AssemblyInfo.Product : AssemblyInfo.FileName;
                if (!string.IsNullOrWhiteSpace(AssemblyInfo.Company))
                    return string.Format(@"SOFTWARE\{0}\{1} {2}", AssemblyInfo.Company, product, AssemblyInfo.FileFormatMainVersion);
                else
                    return string.Format(@"SOFTWARE\{0} {1}", product, AssemblyInfo.FileFormatMainVersion);
            }
        }
        public static string PressAndHoldKey
        {
            get
            {
                if (Environment.OSVersion.Version < new Version(6, 2))
                    return @"Software\Microsoft\Wisp\Pen\SysEventParameters";
                else
                    return @"Software\Microsoft\Wisp\Touch";
            }
        }
        public static string PressAndHoldValue
        {
            get
            {
                if (Environment.OSVersion.Version < new Version(6, 2))
                    return "HoldMode";
                else
                    return "TouchMode_hold";
            }
        }
        #endregion

        #region Static Methods
        public static void ReadLogException(ref bool bLogExceptions)
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey(RegistryKeysHelper.SoftwareProductKey);
                if (key != null)
                {
                    Object o = key.GetValue("AllowSendInfo");
                    if (o != null)
                    {
                        int allow = (int)o;
                        bLogExceptions = allow > 0;
                    }
                }
            }
            catch (Exception ex)  //just for demonstration...it's always best to handle specific exceptions
            {
                //react appropriately
            }
        }

        public static bool PressAndHoldEnabled()
        {
            uint defVal = 1;
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(PressAndHoldKey);
                if (key != null)
                {
                    Object val = key.GetValue(PressAndHoldValue, defVal);
                    return Convert.ToUInt32(val ?? defVal) == 1;
                }
                return defVal == 1;
            }
            catch (Exception ex){
                return defVal == 1;
            }
        }
        #endregion
    }
}
