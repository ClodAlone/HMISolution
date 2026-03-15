using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace HilscherCifXmultiProtocol
{
    class CifXAPIStruct
    {
        #region Structure definitions
        /// <exclude/>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DRIVER_INFORMATIONtag
        {
            /// <exclude/>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] abDriverVersion;                              /*!< Driver version                 */
            /// <exclude/>
            public UInt32 ulBoardCnt;                                   /*!< Number of available Boards     */
        }
        /// <exclude/>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SYSTEM_CHANNEL_INFORMATIONtag                     /*! System Channel Information structure*/
        {
            /// <exclude/>
            public UInt32 ulSystemError;                                /*!< Global system error            */
            /// <exclude/>
            public UInt32 ulDpmTotalSize;                               /*!< Total size dual-port memory in bytes */
            /// <exclude/>
            public UInt32 ulMBXSize;                                    /*!< System mailbox data size [Byte]*/
            /// <exclude/>
            public UInt32 ulDeviceNumber;                               /*!< Global device number           */
            /// <exclude/>
            public UInt32 ulSerialNumber;                               /*!< Global serial number           */
            /// <exclude/>
            public UInt32 ulOpenCnt;                                    /*!< Channel open counter           */
        }
        /// <exclude/>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CIFX_DIRECTORYENTRYtag                    /*! Directory Information structure for enumerating directories */
        {
            /// <exclude/>
            public UInt32 hList;                                /*!< Handle from Enumeration function, do not touch */
            /// <exclude/>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CifXDevMngr.CIFx_MAX_INFO_NAME_LENTH)]
            public byte[] szFilename;                           /*!< Returned file name. */
            /// <exclude/>
            public byte bFiletype;                            /*!< Returned file type. */
            /// <exclude/>
            public UInt32 ulFilesize;                           /*!< Returned file size. */
        }
        /// <exclude/>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SYSTEM_CHANNEL_INFO_BLOCKtag                         /* System Channel: System Information Block */
        {
            /// <exclude/>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] abCookie;                                      /*!< 0x00 "netX" cookie */
            /// <exclude/>
            public UInt32 ulDpmTotalSize;                                /*!< 0x04 Total Size of the whole dual-port memory in bytes */
            /// <exclude/>
            public UInt32 ulDeviceNumber;                                /*!< 0x08 Device number */
            /// <exclude/>
            public UInt32 ulSerialNumber;                                /*!< 0x0C Serial number */
            /// <exclude/>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public UInt16[] ausHwOptions;                                  /*!< 0x10 Hardware options, xC port 0..3 */
            /// <exclude/>
            public UInt16 usManufacturer;                                /*!< 0x18 Manufacturer Location */
            /// <exclude/>
            public UInt16 usProductionDate;                              /*!< 0x1A Date of production */
            /// <exclude/>
            public UInt32 ulLicenseFlags1;                               /*!< 0x1C License code flags 1 */
            /// <exclude/>
            public UInt32 ulLicenseFlags2;                               /*!< 0x20 License code flags 2 */
            /// <exclude/>
            public UInt16 usNetxLicenseID;                               /*!< 0x24 netX license identification */
            /// <exclude/>
            public UInt16 usNetxLicenseFlags;                            /*!< 0x26 netX license flags */
            /// <exclude/>
            public UInt16 usDeviceClass;                                 /*!< 0x28 netX device class */
            /// <exclude/>
            public byte bHwRevision;                                   /*!< 0x2A Hardware revision index */
            /// <exclude/>
            public byte bHwCompatibility;                              /*!< 0x2B Hardware compatibility index */
            /// <exclude/>
            public byte bDevIdNumber;                                  /*!< Device Identification number (rotary switch) */
            /// <exclude/>
            public byte bReserved;                                     /*!< unused/reserved */
            /// <exclude/>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public UInt16[] ausReserved;                                   /*!< 0x2C:0x2F Reserved */
        }

        /// <exclude/>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SYSTEM_CHANNEL_SYSTEM_CONTROL_BLOCKtag             /* System Channel: System Control Block */
        {
            /// <exclude/>
            public UInt32 ulSystemCommandCOS;                            /*!< 0x00 System channel change of state command */
            /// <exclude/>
            public UInt32 ulReserved;                                    /*!< 0x04 Reserved */
        }
        /// <exclude/>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SYSTEM_CHANNEL_SYSTEM_STATUS_BLOCKtag              /* System Channel: System Status Block */
        {
            /// <exclude/>
            public UInt32 ulSystemCOS;                                   /*!< 0x00 System channel change of state acknowledge */
            /// <exclude/>
            public UInt32 ulSystemStatus;                                /*!< 0x04 Actual system state */
            /// <exclude/>
            public UInt32 ulSystemError;                                 /*!< 0x08 Actual system error */
            /// <exclude/>
            public UInt32 ulBootError;                                   /*!< 0x0C Bootup error code (only valid if Cookie="BOOT") */
            /// <exclude/>
            public UInt32 ulTimeSinceStart;                              /*!< 0x10 time since start in seconds */
            /// <exclude/>
            public UInt16 usCpuLoad;                                     /*!< 0x14 cpu load in 0,01% units (10000 => 100%) */
            /// <exclude/>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 42)]
            public byte[] abReserved;                                    /*!< 0x16:3F Reserved */
        }
        /// <exclude/>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BOARD_INFORMATIONtag                            /*! Board Information structure                                              */
        {
            /// <exclude/>
            public UInt32 lBoardError;                                /*!< Global Board error. Set when device specific data must not be used */
            /// <exclude/>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CifXDevMngr.CIFx_MAX_INFO_NAME_LENTH)]
            public byte[] abBoardName;                                /*!< Global board name              */
            /// <exclude/>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CifXDevMngr.CIFx_MAX_INFO_NAME_LENTH)]
            public byte[] abBoardAlias;                               /*!< Global board alias name        */
            /// <exclude/>
            public UInt32 ulBoardID;                                  /*!< Unique board ID, driver created*/
            /// <exclude/>
            public UInt32 ulSystemError;                              /*!< System error                   */
            /// <exclude/>
            public UInt32 ulPhysicalAddress;                          /*!< Physical memory address        */
            /// <exclude/>
            public UInt32 ulIrqNumber;                                /*!< Hardware interrupt number      */
            /// <exclude/>
            public byte bIrqEnabled;                                /*!< Hardware interrupt enable flag */
            /// <exclude/>
            public UInt32 ulChannelCnt;                               /*!< Number of available channels   */
            /// <exclude/>
            public UInt32 ulDpmTotalSize;                             /*!< Dual-Port memory size in bytes */
            /// <exclude/>
            public SYSTEM_CHANNEL_INFO_BLOCKtag tSystemInfo;          /*!< System information             */
        }
        /// <exclude/>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CHANNEL_INFORMATIONtag                            /*! Channel Information structure                                            */
        {
            /// <exclude/>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CifXDevMngr.CIFx_MAX_INFO_NAME_LENTH)]
            public byte[] abBoardName;                                  /*!< Global board name              */
            /// <exclude/>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CifXDevMngr.CIFx_MAX_INFO_NAME_LENTH)]
            public byte[] abBoardAlias;                                 /*!< Global board alias name        */
            /// <exclude/>
            public UInt32 ulDeviceNumber;                               /*!< Global board device number     */
            /// <exclude/>
            public UInt32 ulSerialNumber;                               /*!< Global board serial number     */
            /// <exclude/>
            public UInt16 usFWMajor;                                    /*!< Major Version of Channel Firmware  */
            /// <exclude/>
            public UInt16 usFWMinor;                                    /*!< Minor Version of Channel Firmware  */
            /// <exclude/>
            public UInt16 usFWBuild;                                    /*!< Build number of Channel Firmware   */
            /// <exclude/>
            public UInt16 usFWRevision;                                 /*!< Revision of Channel Firmware       */
            /// <exclude/>
            public byte bFWNameLength;                                /*!< Length  of FW Name                 */
            /// <exclude/>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 63)]
            public byte[] abFWName;                                     /*!< Firmware Name                      */
            /// <exclude/>
            public UInt16 usFWYear;                                     /*!< Build Year of Firmware             */
            /// <exclude/>
            public byte bFWMonth;                                     /*!< Build Month of Firmware (1..12)    */
            /// <exclude/>
            public byte bFWDay;                                       /*!< Build Day of Firmware (1..31)      */
            /// <exclude/>
            public UInt32 ulChannelError;                               /*!< Channel error                  */
            /// <exclude/>
            public UInt32 ulOpenCnt;                                    /*!< Channel open counter           */
            /// <exclude/>
            public UInt32 ulPutPacketCnt;                               /*!< Number of put packet commands  */
            /// <exclude/>
            public UInt32 ulGetPacketCnt;                               /*!< Number of get packet commands  */
            /// <exclude/>
            public UInt32 ulMailboxSize;                                /*!< Mailbox packet size in bytes   */
            /// <exclude/>
            public UInt32 ulIOInAreaCnt;                                /*!< Number of IO IN areas          */
            /// <exclude/>
            public UInt32 ulIOOutAreaCnt;                               /*!< Number of IO OUT areas         */
            /// <exclude/>
            public UInt32 ulHskSize;                                    /*!< Size of the handshake cells    */
            /// <exclude/>
            public UInt32 ulNetxFlags;                                  /*!< Actual netX state flags        */
            /// <exclude/>
            public UInt32 ulHostFlags;                                  /*!< Actual Host flags              */
            /// <exclude/>
            public UInt32 ulHostCOSFlags;                               /*!< Actual Host COS flags          */
            /// <exclude/>
            public UInt32 ulDeviceCOSFlags;                             /*!< Actual Device COS flags        */
        }
        /// <exclude/>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CHANNEL_IO_INFORMATIONtag         /*! IO Area Information structure                                            */
        {
            /// <exclude/>
            public UInt32 ulTotalSize;                  /*!< Total IO area size in byte */
            /// <exclude/>
            public UInt32 ulUsedSize;                   /*!< Used IO area size in byte */
            /// <exclude/>
            public UInt32 ulIOMode;                     /*!< Exchange mode */
        }
        /// <exclude/>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct MEMORY_INFORMATION         /*! Memory Information structure                                             */
        {
            /// <exclude/>
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)]
            public UInt32 pvMemoryID;                  /*!< Identification of the memory area      */
            /// <exclude/>
            public UInt32* ppvMemoryPtr;                /*!< Memory pointer                         */
            /// <exclude/>
            public UInt32* pulMemorySize;               /*!< Complete size of the mapped memory     */
            /// <exclude/>
            public UInt32 ulChannel;                   /*!< Requested channel number               */
            /// <exclude/>
            public UInt32* pulChannelStartOffset;       /*!< Start offset of the requested channel  */
            /// <exclude/>
            public UInt32* pulChannelSize;              /*!< Memory size of the requested channel   */
        }
        /// <exclude/>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct PLC_MEMORY_INFORMATIONtag          /*! PLC Memory Information structure */
        {
            /// <exclude/>
            void* pvMemoryID;           /*!< Identification of the memory area */
            /// <exclude/>
            void** ppvMemoryPtr;         /*!< Memory pointer                   */
            /// <exclude/>
            UInt32 ulAreaDefinition;     /*!< Input/output area                  */
            /// <exclude/>
            UInt32 ulAreaNumber;         /*!< Area number                        */
            /// <exclude/>
            UInt32* pulIOAreaStartOffset; /*!< Start offset                       */
            /// <exclude/>
            UInt32* pulIOAreaSize;        /*!< Memory size                        */
        }

        /// <exclude/>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CIFX_PACKET_HEADERtag     /*! Packet header     */
        {
            /// <exclude/>
            public UInt32 ulDest;   /*!< destination of packet, process queue */
            /// <exclude/>
            public UInt32 ulSrc;    /*!< source of packet, process queue */
            /// <exclude/>
            public UInt32 ulDestId; /*!< destination reference of packet */
            /// <exclude/>
            public UInt32 ulSrcId;  /*!< source reference of packet */
            /// <exclude/>
            public UInt32 ulLen;    /*!< length of packet data without header */
            /// <exclude/>
            public UInt32 ulId;     /*!< identification handle of sender */
            /// <exclude/>
            public UInt32 ulState;  /*!< status code of operation */
            /// <exclude/>
            public UInt32 ulCmd;    /*!< packet command defined in TLR_Commands.h */
            /// <exclude/>
            public UInt32 ulExt;    /*!< extension */
            /// <exclude/>
            public UInt32 ulRout;   /*!< router */
        }
        /// <exclude/>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CIFX_PACKETtag            /*! Definition of the rcX Packet                                             */
        {
            /// <exclude/>
            public CIFX_PACKET_HEADERtag tHeader;                   /**! */
            /// <exclude/>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CifXDevMngr.CIFX_MAX_DATA_SIZE)]
            public byte[] abData;
        }
        /// <exclude/>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PFN_PROGRESS_CALLBACK
        {
            /// <exclude/>
            UInt32 ulStep;
            /// <exclude/>
            UInt32 ulMaxStep;
            /// <exclude/>
            UInt32 pvUser;
            /// <exclude/>
            char bFinished;
            /// <exclude/>
            long lError;
        }
        /// <exclude/>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PFN_RECV_PKT_CALLBACK
        {
            /// <exclude/>
            [MarshalAs(UnmanagedType.U4)]
            CIFX_PACKETtag ptRecvPkt;
            /// <exclude/>
            UInt32 pvUser;
        }
        //typedef void(*PFN_PROGRESS_CALLBACK)(unsigned long ulStep, unsigned long ulMaxStep, void* pvUser, char bFinished, long lError);
        //typedef void(*PFN_RECV_PKT_CALLBACK)(CIFX_PACKET* ptRecvPkt, void* pvUser);

        #endregion

    }
}
