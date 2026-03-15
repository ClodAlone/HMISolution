using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Utilities;

namespace MSZ
{
    enum TransReqType : int
    {
        Hardware = 0,
        Software = 1,
        Net = 2
    }

    public class MSZUtils
    {
        #region Public Static Methods
        public static string GetPrevious()
        {
            return WPFUtilities.CryptString.CryptString.EncryptString(MSZControls.GetComponent());
        }
        #endregion

        #region Internal Static Methods
        internal static string GetLast(string data)
        {
            var prev = WPFUtilities.CryptString.CryptString.DecryptString(data);
            try
            {
                //var data1 = ConvertFromHex((string)(prev.ToUpper()).Substring(0,6));
                //var data2 = ConvertFromHex((string)(prev.ToUpper()).Substring(6, 3));
                //var data3 = ConvertFromHex((string)(prev.ToUpper()).Substring(9, 3));

                var data1 = Int32.Parse((string)(prev.ToUpper()).Substring(0, 6), System.Globalization.NumberStyles.HexNumber);
                var data2 = Int32.Parse((string)(prev.ToUpper()).Substring(6, 3), System.Globalization.NumberStyles.HexNumber);
                var data3 = Int32.Parse((string)(prev.ToUpper()).Substring(9, 3), System.Globalization.NumberStyles.HexNumber);

                var rData1 = data1 / 3 * 2;
                var rData2 = data2 / 7 * 3;
                var rData3 = data3 / 2 * 4;

                return WPFUtilities.CryptString.CryptString.EncryptString(string.Format("{0}{1}{2}", Convert.ToString(rData3), Convert.ToString(rData1), Convert.ToString(rData2)));
            }
            catch (FormatException e)
            {
                //Console.WriteLine("Input string is not a sequence of digits.");
                return string.Empty;
            }
            catch (OverflowException e)
            {
                //Console.WriteLine("The number cannot fit in an Int32.");
                return string.Empty;
            }
        }

        internal static float ConvertFromHex(string asciiString)
        {
            uint num = uint.Parse(asciiString, System.Globalization.NumberStyles.AllowHexSpecifier);

            byte[] floatVals = BitConverter.GetBytes(num);
            float f = BitConverter.ToSingle(floatVals, 0);
            return f;
        }

        internal static bool CheckLast(string data, string todata)
        {
            var result = GetLast(data); 
            return (bool)result.Equals(todata);
        }

        internal static bool CheckPrevious(string todata)
        {
            try
            {
                var result = MSZControls.GetComponent();
                //return (bool)result.Equals(todata);
                if (String.IsNullOrEmpty(result) || !result.Equals(todata))
                {
                    return MSZControls.GetComponent(todata);
                }
                else return true;
            }
            catch
            {
                return false;
            }
        }

#if !NET_STANDARD
        static readonly string[] GoGlobalDlls = { "cpls.dll", "cs4s3.dll", "Components.dll", "redirector.dll", "VBM_UserMode.dll" };
        const string GoGlobalCompanyName = "GraphOn Corporation";
        static bool? isGoGlobalServerSession;
        internal static bool IsGoGlobalServerSession
        {
            get
            {
                if (isGoGlobalServerSession.HasValue)
                    return isGoGlobalServerSession.Value;

                isGoGlobalServerSession = false;
                foreach (var dllName in GoGlobalDlls)
                {
                    try
                    {
                        IntPtr handle = NativeMethods.GetModuleHandle(dllName);
                        if (handle != IntPtr.Zero)
                        {
                            StringBuilder moduleFilePath = new StringBuilder(NativeMethods.MAX_PATH);
                            var result = NativeMethods.GetModuleFileName(handle, moduleFilePath, (uint)(moduleFilePath.Capacity));
                            if (result != 0 && moduleFilePath.Length > 0)
                            {
                                var info = System.Diagnostics.FileVersionInfo.GetVersionInfo(moduleFilePath.ToString());
                                if (info.CompanyName.Contains(GoGlobalCompanyName) || info.LegalCopyright.Contains(GoGlobalCompanyName))
                                {
                                    isGoGlobalServerSession = true;
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    { }
                }
                
                return isGoGlobalServerSession.Value;
            }
        }
#endif
        #endregion
    }
}
