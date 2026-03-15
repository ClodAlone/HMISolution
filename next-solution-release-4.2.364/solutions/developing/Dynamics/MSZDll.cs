using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MSZ
{
    internal sealed class MSZDll
    {
        #region SG-Lock API
        internal const int SGL_SUCCESS = 0x00;
        internal const int SGL_DGL_NOT_FOUND = 0x01;
        internal const int SGL_LPT_BUSY = 0x02;
        internal const int SGL_LPT_OPEN_ERROR = 0x03;
        internal const int SGL_NO_LPT_PORT_FOUND = 0x04;
        internal const int SGL_AUTHENTICATION_REQUIRED = 0x05;
        internal const int SGL_AUTHENTICATION_FAILED = 0x06;
        internal const int SGL_FUNCTION_NOT_SUPPORTED = 0x07;
        internal const int SGL_PARAMETER_INVALID = 0x08;
        internal const int SGL_SIGNATURE_INVALID = 0x09;
        internal const int SGL_USB_BUSY = 0x0A;
        internal const uint TEA_KEY_NUM = 1;

        internal const int REGISTER_MAX_COUNT = 47;
        internal const int REGISTER_MAX_BYTECOUNT = 188;

        #region SglAuthentA
        internal static UInt32 SglAuthentA(UInt32[] AuhtentCode, UInt32[] AppRandNum, UInt32[] LibRandNum)
        {
#if NET_STANDARD
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return SglAuthentALinux(AuhtentCode, AppRandNum, LibRandNum);
            else 
#endif
            if (System.Environment.Is64BitProcess)
                return SglAuthentAWin64Bit(AuhtentCode, AppRandNum, LibRandNum);
            else
                return SglAuthentAWin32Bit(AuhtentCode, AppRandNum, LibRandNum);
        }
        [DllImport("sglw32.dll", EntryPoint = "SglAuthentA")]
        static extern UInt32 SglAuthentAWin32Bit([Out] UInt32[] AuhtentCode, [In, Out] UInt32[] AppRandNum, [In] UInt32[] LibRandNum);
        [DllImport("sglw64.dll", EntryPoint = "SglAuthentA")]
        static extern UInt32 SglAuthentAWin64Bit([Out] UInt32[] AuhtentCode, [In, Out] UInt32[] AppRandNum, [In] UInt32[] LibRandNum);
#if NET_STANDARD
        [DllImport("libsgllnx", EntryPoint = "SglAuthentA")]
        static extern UInt32 SglAuthentALinux([Out] UInt32[] AuhtentCode, [In, Out] UInt32[] AppRandNum, [In] UInt32[] LibRandNum);
#endif
        #endregion

        #region SglAuthentB
        internal static UInt32 SglAuthentB(UInt32[] LibRandNum)
        {
#if NET_STANDARD
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return SglAuthentBLinux(LibRandNum);
            else 
#endif
            if (System.Environment.Is64BitProcess)
                return SglAuthentBWin64Bit(LibRandNum);
            else
                return SglAuthentBWin32Bit(LibRandNum);
        }
        [DllImport("sglw32.dll", EntryPoint = "SglAuthentB")]
        static extern UInt32 SglAuthentBWin32Bit([Out] UInt32[] LibRandNum);
        [DllImport("sglw64.dll", EntryPoint = "SglAuthentB")]
        static extern UInt32 SglAuthentBWin64Bit([Out] UInt32[] LibRandNum);
#if NET_STANDARD
        [DllImport("libsgllnx", EntryPoint = "SglAuthentB")]
        static extern UInt32 SglAuthentBLinux([Out] UInt32[] LibRandNum);
#endif
        #endregion

        #region SglSearchLock
        internal static uint SglSearchLock(uint ProductId)
        {
#if NET_STANDARD
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return SglSearchLockLinux(ProductId);
            else 
#endif
            if (System.Environment.Is64BitProcess)
                return SglSearchLockWin64Bit(ProductId);
            else
                return SglSearchLockWin32Bit(ProductId);
        }
        [DllImport("sglw32.dll", EntryPoint = "SglSearchLock")]
        static extern uint SglSearchLockWin32Bit([Out] uint ProductId);
        [DllImport("sglw64.dll", EntryPoint = "SglSearchLock")]
        static extern uint SglSearchLockWin64Bit([Out] uint ProductId);
#if NET_STANDARD
        [DllImport("libsgllnx", EntryPoint = "SglSearchLock")]
        static extern uint SglSearchLockLinux([Out] uint ProductId);
#endif
        #endregion

        #region SglReadSerialNumber
        internal static uint SglReadSerialNumber(uint ProductId, uint[] SerialNumber)
        {
#if NET_STANDARD
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return SglReadSerialNumberLinux(ProductId, SerialNumber);
            else 
#endif
            if (System.Environment.Is64BitProcess)
                return SglReadSerialNumberWin64Bit(ProductId, SerialNumber);
            else
                return SglReadSerialNumberWin32Bit(ProductId, SerialNumber);
        }
        [DllImport("sglw32.dll", EntryPoint = "SglReadSerialNumber")]
        static extern uint SglReadSerialNumberWin32Bit([Out] uint ProductId, [In] uint[] SerialNumber); // pointer to 1 DWORD, read only!
        [DllImport("sglw64.dll", EntryPoint = "SglReadSerialNumber")]
        static extern uint SglReadSerialNumberWin64Bit([Out] uint ProductId, [In] uint[] SerialNumber); // pointer to 1 DWORD, read only!
#if NET_STANDARD
        [DllImport("libsgllnx", EntryPoint = "SglReadSerialNumber")]
        static extern uint SglReadSerialNumberLinux([Out] uint ProductId, [In] uint[] SerialNumber); // pointer to 1 DWORD, read only!
#endif
        #endregion

        #region SglReadProductId
        // administration functions
        internal static uint SglReadProductId(uint[] ProductIdPtr)
        {
#if NET_STANDARD
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return SglReadProductIdLinux(ProductIdPtr);
            else 
#endif
            if (System.Environment.Is64BitProcess)
                return SglReadProductIdWin64Bit(ProductIdPtr);
            else
                return SglReadProductIdWin32Bit(ProductIdPtr);
        }
        [DllImport("sglw32.dll", EntryPoint = "SglReadProductId")]
        static extern uint SglReadProductIdWin32Bit([In] uint[] ProductIdPtr); // pointer to 1 DWORD
        [DllImport("sglw64.dll", EntryPoint = "SglReadProductId")]
        static extern uint SglReadProductIdWin64Bit([In] uint[] ProductIdPtr); // pointer to 1 DWORD
#if NET_STANDARD
        [DllImport("libsgllnx", EntryPoint = "SglReadProductId")]
        static extern uint SglReadProductIdLinux([In] uint[] ProductIdPtr); // pointer to 1 DWORD
#endif
        #endregion

        #region SglWriteProductId
        // administration functions
        internal static uint SglWriteProductId(uint OldProductId, uint NewProductId)
        {
#if NET_STANDARD
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return SglWriteProductIdLinux(OldProductId, NewProductId);
            else 
#endif
            if (System.Environment.Is64BitProcess)
                return SglWriteProductIdWin64Bit(OldProductId, NewProductId);
            else
                return SglWriteProductIdWin32Bit(OldProductId, NewProductId);
        }
        [DllImport("sglw32.dll", EntryPoint = "SglWriteProductId")]
        static extern uint SglWriteProductIdWin32Bit([Out] uint OldProductId, [Out] uint NewProductId);
        [DllImport("sglw64.dll", EntryPoint = "SglWriteProductId")]
        static extern uint SglWriteProductIdWin64Bit([Out] uint OldProductId, [Out] uint NewProductId);
#if NET_STANDARD
        [DllImport("libsgllnx", EntryPoint = "SglWriteProductId")]
        static extern uint SglWriteProductIdLinux([Out] uint OldProductId, [Out] uint NewProductId);
#endif
        #endregion

        #region SglReadConfig
        // administration functions
        internal static uint SglReadConfig(uint ProductId, uint Category, uint[] Data)
        {
#if NET_STANDARD
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return SglReadConfigLinux(ProductId, Category, Data);
            else
#endif
            if (System.Environment.Is64BitProcess)
                return SglReadConfigWin64Bit(ProductId, Category, Data);
            else
                return SglReadConfigWin32Bit(ProductId, Category, Data);
        }
        [DllImport("sglw32.dll", EntryPoint = "SglReadConfig")]
        static extern uint SglReadConfigWin32Bit([Out] uint ProductId, [Out] uint Category, [In] uint[] Data); // has to be an array of 8 DWord/32bit values 
        [DllImport("sglw64.dll", EntryPoint = "SglReadConfig")]
        static extern uint SglReadConfigWin64Bit([Out] uint ProductId, [Out] uint Category, [In] uint[] Data); // has to be an array of 8 DWord/32bit values 
#if NET_STANDARD
        [DllImport("libsgllnx", EntryPoint = "SglReadConfig")]
        static extern uint SglReadConfigLinux([Out] uint ProductId, [Out] uint Category, [In]  uint[] Data); // has to be an array of 8 DWord/32bit values 
#endif
        #endregion

        // possible parameters for 'Category'
        internal const int SGL_READ_CONFIG_LOCK_INFO = 0x0000;    // copy protection hardware info

        // possible returns in 'Data' array    
        // Data[0] = ModelId ( see below )
        internal const int SGL_CONFIG_LOCK_SERIES_2 = 0x0001;
        internal const int SGL_CONFIG_LOCK_SERIES_3 = 0x0002;
        internal const int SGL_CONFIG_LOCK_SERIES_4 = 0x0003;

        // Data[1] = Interface ( see below )  
        internal const int SGL_CONFIG_INTERFACE_USB = 0x0000;
        internal const int SGL_CONFIG_INTERFACE_LPT = 0x0001;

        // Data[2] = Software Version of SG-Lock ( high word = major version, low word = minor version )
        // Data[3] = Hardware Version of SG-Lock ( high word = major version, low word = minor version ) 
        // Data[4] = Serial Number
        // Data[5] = Memory Size ( size of memory in DWords !!)
        // Data[6] = Counter Count
        // Data[7] = 128 bit Key Count

        /*******************************************************************/
        /*                                                                 */
        /*  SG-Lock API extended functions (supported only by L3,L4,U3,U4) */
        /*                                                                 */
        /*******************************************************************/

        #region SglReadData
        // Memory functions
        internal static uint SglReadData(uint ProductId, uint Address, uint Count, uint[] Data)
        {
#if NET_STANDARD
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return SglReadDataLinux(ProductId, Address, Count, Data);
            else 
#endif
            if (System.Environment.Is64BitProcess)
                return SglReadDataWin64Bit(ProductId, Address, Count, Data);
            else
                return SglReadDataWin32Bit(ProductId, Address, Count, Data);
        }
        [DllImport("sglw32.dll", EntryPoint = "SglReadData")]
        static extern uint SglReadDataWin32Bit(
                            [Out] uint ProductId,
                            [Out] uint Address,         // L3,U3: 0-63; L4,U4: 0-255
                            [Out] uint Count,           // L3,U3: 1-63; L4,U4: 1-255
                            [In] uint[] Data);          // data array with minimum size 

        [DllImport("sglw64.dll", EntryPoint = "SglReadData")]
        static extern uint SglReadDataWin64Bit(
                            [Out] uint ProductId,
                            [Out] uint Address,         // L3,U3: 0-63; L4,U4: 0-255
                            [Out] uint Count,           // L3,U3: 1-63; L4,U4: 1-255
                            [In]  uint[] Data);         // data array with minimum size 
#if NET_STANDARD
        [DllImport("libsgllnx", EntryPoint = "SglReadData")]
        static extern uint SglReadDataLinux(
                            [Out] uint ProductId,
                            [Out] uint Address,         // L3,U3: 0-63; L4,U4: 0-255
                            [Out] uint Count,           // L3,U3: 1-63; L4,U4: 1-255
                            [In] uint[] Data);          // data array with minimum size 
#endif
        #endregion

        #region SglWriteData
        // Memory functions
        internal static uint SglWriteData(uint ProductId, uint Address, uint Count, uint[] Data)
        {
#if NET_STANDARD
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return SglWriteDataLinux(ProductId, Address, Count, Data);
            else 
#endif
            if (System.Environment.Is64BitProcess)
                return SglWriteDataWin64Bit(ProductId, Address, Count, Data);
            else
                return SglWriteDataWin32Bit(ProductId, Address, Count, Data);
        }
        [DllImport("sglw32.dll", EntryPoint = "SglWriteData")]
        static extern uint SglWriteDataWin32Bit(
                            [Out] uint ProductId,
                            [Out] uint Address,         // L3,U3: 0-63; L4,U4: 0-255
                            [Out] uint Count,           // L3,U3: 1-63; L4,U4: 1-255
                            [In] uint[] Data);          // data array with minimum size 

        [DllImport("sglw64.dll", EntryPoint = "SglWriteData")]
        static extern uint SglWriteDataWin64Bit(
                            [Out] uint ProductId,
                            [Out] uint Address,         // L3,U3: 0-63; L4,U4: 0-255
                            [Out] uint Count,           // L3,U3: 1-63; L4,U4: 1-255
                            [In] uint[] Data);          // data array with minimum size 
#if NET_STANDARD
        [DllImport("libsgllnx", EntryPoint = "SglWriteData")]
        static extern uint SglWriteDataLinux(
                            [Out] uint ProductId,
                            [Out] uint Address,         // L3,U3: 0-63; L4,U4: 0-255
                            [Out] uint Count,           // L3,U3: 1-63; L4,U4: 1-255
                            [In] uint[] Data);          // data array with minimum size 
#endif
        #endregion

        #region SglReadCounter
        // Counter functions
        internal static uint SglReadCounter(uint ProductId, uint CntNum, uint[] Data)
        {
#if NET_STANDARD
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return SglReadCounterLinux(ProductId, CntNum, Data);
            else
#endif
            if (System.Environment.Is64BitProcess)
                return SglReadCounterWin64Bit(ProductId, CntNum, Data);
            else
                return SglReadCounterWin32Bit(ProductId, CntNum, Data);
        }
        [DllImport("sglw32.dll", EntryPoint = "SglReadCounter")]
        static extern uint SglReadCounterWin32Bit(
                            [Out] uint ProductId,
                            [Out] uint CntNum,          // number of counter: L3,U3: 0-15
                            [In] uint[] Data);          // L4,U4: 0-63, *Data is a pointer to 1 DWORD
        [DllImport("sglw64.dll", EntryPoint = "SglReadCounter")]
        static extern uint SglReadCounterWin64Bit(
                            [Out] uint ProductId,
                            [Out] uint CntNum,          // number of counter: L3,U3: 0-15
                            [In] uint[] Data);          // L4,U4: 0-63, *Data is a pointer to 1 DWORD
#if NET_STANDARD
        [DllImport("libsgllnx", EntryPoint = "SglReadCounter")]
        static extern uint SglReadCounterLinux(
                            [Out] uint ProductId,
                            [Out] uint CntNum,          // number of counter: L3,U3: 0-15
                            [In]  uint[] Data);         // L4,U4: 0-63, *Data is a pointer to 1 DWORD
#endif
        #endregion

        #region SglReadCounter
        // Counter functions
        internal static uint SglWriteCounter(uint ProductId, uint CntNum, uint Data)
        {
#if NET_STANDARD
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return SglWriteCounterLinux(ProductId, CntNum, Data);
            else
#endif
            if (System.Environment.Is64BitProcess)
                return SglWriteCounterWin64Bit(ProductId, CntNum, Data);
            else
                return SglWriteCounterWin32Bit(ProductId, CntNum, Data);
        }
        [DllImport("sglw32.dll", EntryPoint = "SglWriteCounter")]
        static extern uint SglWriteCounterWin32Bit(
                            [Out] uint ProductId,
                            [Out] uint CntNum,          // number of counter: L3,U3: 0-15
                            [Out] uint Data);           // L4,U4: 0-63, Data is the counter value
        [DllImport("sglw64.dll", EntryPoint = "SglWriteCounter")]
        static extern uint SglWriteCounterWin64Bit(
                            [Out] uint ProductId,
                            [Out] uint CntNum,          // number of counter: L3,U3: 0-15
                            [Out] uint Data);           // L4,U4: 0-63, Data is the counter value
#if NET_STANDARD
        [DllImport("libsgllnx", EntryPoint = "SglWriteCounter")]
        static extern uint SglWriteCounterLinux(
                            [Out] uint ProductId,
                            [Out] uint CntNum,          // number of counter: L3,U3: 0-15
                            [Out] uint Data);           // L4,U4: 0-63, Data is the counter value
#endif
        #endregion

        #region SglCryptLock
        // Cryptographic and signing functions
        internal static uint SglCryptLock(uint ProductId, uint KeyNum, uint CryptMode, uint BlockCnt, uint[] Data)
        {
#if NET_STANDARD
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return SglCryptLockLinux(ProductId, KeyNum, CryptMode, BlockCnt, Data);
            else
#endif
            if (System.Environment.Is64BitProcess)
                return SglCryptLockWin64Bit(ProductId, KeyNum, CryptMode, BlockCnt, Data);
            else
                return SglCryptLockWin32Bit(ProductId, KeyNum, CryptMode, BlockCnt, Data);
        }
        [DllImport("sglw32.dll", EntryPoint = "SglCryptLock")]
        static extern uint SglCryptLockWin32Bit(
                            [Out] uint ProductId,
                            [Out] uint KeyNum,          // number of key: L2, U2: 0 ; L3,U3: 0-1 ; L4,U4: 0-15
                            [Out] uint CryptMode,       // 0=encrypt, 1=decrypt
                            [Out] uint BlockCnt,        // number of 8-Bytes blocks to proccess
                            [In] uint[] Data);          // data to process, size must be BlockCnt * 8 Bytes
        [DllImport("sglw64.dll", EntryPoint = "SglCryptLock")]
        static extern uint SglCryptLockWin64Bit(
                            [Out] uint ProductId,
                            [Out] uint KeyNum,          // number of key: L2, U2: 0 ; L3,U3: 0-1 ; L4,U4: 0-15
                            [Out] uint CryptMode,       // 0=encrypt, 1=decrypt
                            [Out] uint BlockCnt,        // number of 8-Bytes blocks to proccess
                            [In]  uint[] Data);         // data to process, size must be BlockCnt * 8 Bytes
#if NET_STANDARD
        [DllImport("libsgllnx", EntryPoint = "SglCryptLock")]
        static extern uint SglCryptLockLinux(
                            [Out] uint ProductId,
                            [Out] uint KeyNum,          // number of key: L2, U2: 0 ; L3,U3: 0-1 ; L4,U4: 0-15
                            [Out] uint CryptMode,       // 0=encrypt, 1=decrypt
                            [Out] uint BlockCnt,        // number of 8-Bytes blocks to proccess
                            [In]  uint[] Data);         // data to process, size must be BlockCnt * 8 Bytes
#endif
        #endregion

        #region SglWriteKey
        // Administrative functions
        internal static uint SglWriteKey(uint ProductId, uint KeyNum, uint[] Key)
        {
#if NET_STANDARD
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return SglWriteKeyLinux(ProductId, KeyNum, Key);
            else
#endif
            if (System.Environment.Is64BitProcess)
                return SglWriteKeyWin64Bit(ProductId, KeyNum, Key);
            else
                return SglWriteKeyWin32Bit(ProductId, KeyNum, Key);
        }
        [DllImport("sglw32.dll", EntryPoint = "SglWriteKey")]
        static extern uint SglWriteKeyWin32Bit(
                            [Out] uint ProductId,       // L2,U2: Key fixed - that means not writeable
                            [Out] uint KeyNum,          // number of key: L3,U3: 0-1 ; L4,U4: 0-15 
                            [In]  uint[] Key);          // 128-Bit (16 Bytes) Key, write only!!! 

        [DllImport("sglw64.dll", EntryPoint = "SglWriteKey")]
        static extern uint SglWriteKeyWin64Bit(
                            [Out] uint ProductId,       // L2,U2: Key fixed - that means not writeable
                            [Out] uint KeyNum,          // number of key: L3,U3: 0-1 ; L4,U4: 0-15 
                            [In] uint[] Key);           // 128-Bit (16 Bytes) Key, write only!!! 
#if NET_STANDARD
        [DllImport("libsgllnx", EntryPoint = "SglWriteKey")]
        static extern uint SglWriteKeyLinux(
                            [Out] uint ProductId,       // L2,U2: Key fixed - that means not writeable
                            [Out] uint KeyNum,          // number of key: L3,U3: 0-1 ; L4,U4: 0-15 
                            [In]  uint[] Key);          // 128-Bit (16 Bytes) Key, write only!!! 
#endif
        #endregion
        #endregion

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

#if !NET_STANDARD
        [DllImport("libipld.dll")]
#else
        [DllImport("libipld")]
#endif
        internal static extern bool ipldGetLicStatus(
                                    [In] String progea_vsn_string,
                                    [Out] StringBuilder json_buffer,
                                    [In, Out] ref uint bufsize,
                                    [In, Out] ref uint result);

#if !NET_STANDARD
        [DllImport("libipld.dll")]
#else
        [DllImport("libipld")]
#endif
        internal static extern ushort ipldGetVersion();

#if !NET_STANDARD
        [DllImport("libipld.dll")]
#else
        [DllImport("libipld")]
#endif
        internal static extern bool ipldSetDebug([In] int val);

#if !NET_STANDARD
        [DllImport("libipld.dll")]
#else
        [DllImport("libipld")]
#endif
        internal static extern bool ipldSetLicenseUrl([In] String url);

#if !NET_STANDARD
        [DllImport("libipld.dll")]
#else
        [DllImport("libipld")]
#endif
        internal static extern bool ipldSetLicenseValidUrl([In] String url);
        #endregion
    }

#if !NET_STANDARD
    #region Native methods
    [ComVisibleAttribute(false),
     System.Security.SuppressUnmanagedCodeSecurity()]
    internal class NativeMethods
    {
        public const int MAX_PATH = 260;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint GetModuleFileName(IntPtr hModule, [Out] StringBuilder lpBaseName, [In][MarshalAs(UnmanagedType.U4)] uint nSize);
    }
    #endregion
#endif
}
