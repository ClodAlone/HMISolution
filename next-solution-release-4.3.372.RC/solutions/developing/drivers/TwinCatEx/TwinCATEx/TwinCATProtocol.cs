////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	IEC60870_5_104Protocol.cs
//
// summary:	Implements the driver IEC60870_5_104 protocol class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using System.Runtime.InteropServices;
using TwinCAT.Ads;

namespace TwinCAT
{
    /// <summary>   Communication protocol of IEC60870_5_104 driver. </summary>
    public class TwinCATProtocol
    {
        #region const
        public const int MAX_DATA_BYTES = 2048;
        public const int DEFAULT_STRING_SIZE = 80;

        public const uint MAX_REQUEST_NUMBER = 500;
        public const uint DEFAULT_REQUEST_NUMBER = 50;
        public const string TEST_COMM_DYNAMIC_SETTINGS = "TwinCAT.Station={0}|LinkType=1|SA=%MW0|DL=0";
        #endregion

        #region methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return maximum memory size of TwinCAT objects basing on Movicon data type.</summary>
        ///
        /// <returns>   The maximum job size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static uint GetMaxJobSize(UFUAModel.DataType varType)
        {
            if (varType == UFUAModel.DataType.Boolean)
                return MAX_DATA_BYTES * 8;
            else
                return MAX_DATA_BYTES;
        }

        public static bool IsValidStringSize(uint size)
        {
            return (size<=0 || MAX_DATA_BYTES > size);
        }

        #endregion


        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string strLibraryName);

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        static extern Int32 FreeLibrary(IntPtr hModule);


        public static bool CheckDll()
        {
            bool result = false;

            IntPtr hModuleDLL = LoadLibrary("TcAdsDll");

            if (hModuleDLL != IntPtr.Zero)
            {
                result = true;

                TcAdsClient tcAds = null;
                try
                {
                    tcAds = new TcAdsClient();
                } catch (Exception ex)
                {
                    result = false;
                }
                finally
                {
                    if (tcAds != null)
                    {
                        tcAds.Dispose();
                        tcAds = null;
                    }
                }

                FreeLibrary(hModuleDLL);
            }

            return result;
        }

        public static bool IsTimeOutError(AdsErrorCode error)
        {
            return (error == AdsErrorCode.SyncTimeOut || error == AdsErrorCode.DeviceTimeOut|| error == AdsErrorCode.ClientSyncTimeOut || error == AdsErrorCode.ClientTimeoutInvalid);
        }
    }
}
