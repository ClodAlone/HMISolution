using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;

namespace HilscherCifXmultiProtocol
{
    public class CifXDevMngr
    {
        #region Constructors
        public CifXDevMngr()
        {
            _ErrorDescription = "";
            _BoardName = "";
            _BoardAlias = "";
            _sFileName = "";
            _FwVersion = "";
            _RcvPktCount = 0;
            _SndPktCount = 0;
            _SndCommand = 0;
            _SndDestination = 0;
            _SndDestinationID = 0;
            _SndExtension = 0;
            _SndID = 0;
            _SndLength = 0;
            _SndRoute = 0;
            _SndSource = 0;
            _SndSourceID = 0;
            _SndState = 0;
            _RcvCommand = 0;
            _RcvDestination = 0;
            _RcvDestinationID = 0;
            _RcvExtension = 0;
            _RcvID = 0;
            _RcvLength = 0;
            _RcvRoute = 0;
            _RcvSource = 0;
            _RcvSourceID = 0;
            _RcvState = 0;
            _DpmPhysicalSize = 0;
            _DpmSizeFromFw = 0;
            _SystemError = 0;
            _DeviceNumber = 0;
            _SerialNumber = 0;
            _MBXSize = 0;
        }
        #endregion

        #region Data Members

        private UInt32 lReturn = 0;
        private UInt32 _OpenCount = 0;

        private byte[,] abInfoBlock = new byte[CIFX_MAX_NUMBER_OF_CHANNEL_DEFINITION, CIFX_SYSTEM_CHANNEL_DEFAULT_INFO_BLOCK_SIZE];
        private byte[] _ChannelNumber = new byte[CIFX_MAX_NUMBER_OF_CHANNEL_DEFINITION];
        private string[] _ChannelType = new string[CIFX_MAX_NUMBER_OF_CHANNEL_DEFINITION];
        private byte[] _NumberOfBlocks = new byte[CIFX_MAX_NUMBER_OF_CHANNEL_DEFINITION];
        private int[] _SizeOfChannel = new int[CIFX_MAX_NUMBER_OF_CHANNEL_DEFINITION];
        private int[] _MbxStartOffset = new int[CIFX_MAX_NUMBER_OF_CHANNEL_DEFINITION];
        private int[] _MbxSize = new int[CIFX_MAX_NUMBER_OF_CHANNEL_DEFINITION];
        private byte[] _HskWidth = new byte[CIFX_MAX_NUMBER_OF_CHANNEL_DEFINITION];
        private byte[] _HskPos = new byte[CIFX_MAX_NUMBER_OF_CHANNEL_DEFINITION];


        private static CifXAPIStruct.DRIVER_INFORMATIONtag _DriverInformation;
        private static CifXAPIStruct.CIFX_DIRECTORYENTRYtag _CifXDirectoryEntry;
        private static CifXAPIStruct.SYSTEM_CHANNEL_INFORMATIONtag _SystemChannelInformation;   //Command: CIFX_INFO_CMD_SYSTEM_INFORMATION
        private static CifXAPIStruct.SYSTEM_CHANNEL_INFO_BLOCKtag _SystemChannelInfoBlock;   //Command: CIFX_INFO_CMD_SYSTEM_INFO_BLOCK
        private static CifXAPIStruct.SYSTEM_CHANNEL_SYSTEM_CONTROL_BLOCKtag _SystemChannelSystemControlBlock;   //Command: CIFX_INFO_CMD_SYSTEM_CONTROL_BLOCK
        private static CifXAPIStruct.SYSTEM_CHANNEL_SYSTEM_STATUS_BLOCKtag _SystemChannelSystemStatusBlock;   //Command: CIFX_INFO_CMD_SYSTEM_STATUS_BLOCK
        private static CifXAPIStruct.BOARD_INFORMATIONtag _BoardInformation;
        private static CifXAPIStruct.CHANNEL_INFORMATIONtag _ChannelInformation;
        private static CifXAPIStruct.CHANNEL_IO_INFORMATIONtag _ChannelIOInformation;

        IntPtr hDriverIntPointer;
        public unsafe void* hDriver = null;
        IntPtr hChannelIntPointer;
        public unsafe void* hChannel = null;
        #endregion

        #region Exported Properties
        /// <summary>
        /// Stores the name of the active board. You have to set this value, each time you want to change the active board<br></br>
        /// you want to communicate with.
        /// </summary>
        private static string _ActivBoard = "";
        public string ActivBoard
        {
            get
            {
                return _ActivBoard;
            }
            set
            {
                _ActivBoard = value;
            }
        }
        /// <summary>
        /// The name of the file for up- and download functions.
        /// </summary>
        private string _sFileName;
        public string FileName
        {
            get
            {
                return _sFileName;
            }
            set
            {
                _sFileName = value;
            }
        }

        /// <summary>
        /// This property stores the error description after the function <see cref="xDriverGetErrorDescription"/> is called.
        /// </summary>
        /// <seealso cref="xDriverGetErrorDescription"/>
        private string _ErrorDescription;
        public string ErrorDescription
        {
            get
            {
                return _ErrorDescription;
            }
        }
        /// <summary>
        /// This property stores the boardname of the active board. This value will be updated each time you call a new board.
        /// </summary>
        /// <seealso cref="xDriverEnumChannels"/>
        /// <seealso cref="xChannelInfo"/>
        private string _BoardName;
        public string BoardName
        {
            get
            {
                return _BoardName;
            }
        }
        /// <summary>
        /// This property stores the boardalias of the active board. This value will be updated each time you call a new board.
        /// </summary>
        /// <seealso cref="xDriverEnumChannels"/>
        /// <seealso cref="xChannelInfo"/>
        private string _BoardAlias;
        public string BoardAlias
        {
            get
            {
                return _BoardAlias;
            }
        }
        /// <summary>
        /// Readonly property with the name and version of the driver.
        /// </summary>
        /// <seealso cref="xDriverGetInformation"/>
        private static string _DriverVersion = "";
        public string DriverVersion
        {
            get
            {
                return _DriverVersion;
            }
        }
        /// <summary>
        /// Readonly property stores the DPM cookie.
        /// </summary>
        /// <seealso cref="xDriverEnumBoards"/>
        //private string _Cookie;
        //string Cookie
        //{
        //    get
        //    {
        //        return _Cookie;
        //    }
        //}
        /// <summary>
        /// String containing the Firmware version.
        /// </summary>
        /// <seealso cref="cifXDevMngr.xDriverEnumChannels"/>
        /// <seealso cref="cifXDevMngr.xChannelInfo"/>
        private string _FwVersion;
        public string FwVersion
        {
            get
            {
                return _FwVersion;
            }
        }

        /// <summary>
        /// After calling one of these functions xSysdeviceGetMBXState or xChannelGetMBXState<br></br>
        /// the total amount of packets to receive is stored in this property.
        /// </summary>
        /// <seealso cref="xSysdeviceGetMBXState"/>
        /// <seealso cref="xChannelGetMBXState"/>
        private UInt32 _RcvPktCount;
        public int RcvPktCount
        {
            get
            {
                return (int)_RcvPktCount;
            }
        }
        /// <summary>
        /// After calling one of these functions xSysdeviceGetMBXState or xChannelGetMBXState<br></br>
        /// the total amount of packets which could be send to the card is stored in this property.
        /// </summary>
        /// <seealso cref="xSysdeviceGetMBXState"/>
        /// <seealso cref="xChannelGetMBXState"/>
        private UInt32 _SndPktCount = 0;
        public int SndPktCount
        {
            get
            {
                return (int)_SndPktCount;
            }
        }
        /// <summary>
        /// Mailbox size in Bytes
        /// </summary>
        /// <seealso cref="xChannelInfo"/>
        /// <seealso cref="xDriverEnumChannels"/>
        private uint _MBXSize;
        public int MBXSize
        {
            get
            {
                return (int)_MBXSize;
            }
        }
        /// <summary>
        /// Number of calls to xChannelOpen for this channel (<see cref="xChannelInfo"/>).<br></br> 
        /// Number of times this device is open (<see cref="xDriverEnumChannels"/>)
        /// </summary>
        /// <seealso cref="xDriverEnumChannels"/>
        /// <seealso cref="xChannelInfo"/>
        //int OpenCnt { get; }
        /// <summary>
        /// This property stores the command of a received packet by <see cref="xSysdeviceGetPacket"/> or <see cref="xChannelGetPacket"/>.
        /// </summary>
        /// <seealso cref="xSysdeviceGetPacket"/>
        /// <seealso cref="xChannelGetPacket"/>
        private UInt32 _RcvCommand;
        public int RcvCommand
        {
            get
            {
                return (int)_RcvCommand;
            }
        }

        /// <summary>
        /// This property stores the destination of a received packet by <see cref="xSysdeviceGetPacket"/> or ^<see cref="xChannelGetPacket"/>.
        /// </summary>
        /// <seealso cref="xSysdeviceGetPacket"/>
        /// <seealso cref="xChannelGetPacket"/>
        private UInt32 _RcvDestination;
        public int RcvDestination
        {
            get
            {
                return (int)_RcvDestination;
            }
        }

        /// <summary>
        /// This property stores the destination id of a received packet by <see cref="xSysdeviceGetPacket"/> or <see cref="xChannelGetPacket"/>.
        /// </summary>
        /// <seealso cref="xSysdeviceGetPacket"/>
        /// <seealso cref="xChannelGetPacket"/>
        private UInt32 _RcvDestinationID;
        public int RcvDestinationID
        {
            get
            {
                return (int)_RcvDestinationID;
            }
        }

        /// <summary>
        /// This property stores the extension of a received packet by <see cref="xSysdeviceGetPacket"/> or <see cref="xChannelGetPacket"/>.
        /// </summary>
        /// <seealso cref="xSysdeviceGetPacket"/>
        /// <seealso cref="xChannelGetPacket"/>
        private UInt32 _RcvExtension;
        public int RcvExtension
        {
            get
            {
                return (int)_RcvExtension;
            }
        }

        /// <summary>
        /// This property stores the ID of a received packet by <see cref="xSysdeviceGetPacket"/> or <see cref="xChannelGetPacket"/>.
        /// </summary>
        /// <seealso cref="xSysdeviceGetPacket"/>
        /// <seealso cref="xChannelGetPacket"/>
        private UInt32 _RcvID;
        public int RcvID
        {
            get
            {
                return (int)_RcvID;
            }
        }

        /// <summary>
        /// This property stores the length of the data received in a packet by <see cref="xSysdeviceGetPacket"/> or <see cref="xChannelGetPacket"/>.
        /// </summary>
        /// <seealso cref="xSysdeviceGetPacket"/>
        /// <seealso cref="xChannelGetPacket"/>
        private UInt32 _RcvLength;
        public int RcvLength
        {
            get
            {
                return (int)_RcvLength;
            }
        }

        /// <summary>
        /// This property stores the route of a received packet by <see cref="xSysdeviceGetPacket"/> or <see cref="xChannelGetPacket"/>.
        /// </summary>
        /// <seealso cref="xSysdeviceGetPacket"/>
        /// <seealso cref="xChannelGetPacket"/>
        private UInt32 _RcvRoute;
        public int RcvRoute
        {
            get
            {
                return (int)_RcvRoute;
            }
        }

        /// <summary>
        /// This property stores the source of a received packet by <see cref="xSysdeviceGetPacket"/> or <see cref="xChannelGetPacket"/>.
        /// </summary>
        /// <seealso cref="xSysdeviceGetPacket"/>
        /// <seealso cref="xChannelGetPacket"/>
        private UInt32 _RcvSource;
        public int RcvSource
        {
            get
            {
                return (int)_RcvSource;
            }
        }
        /// <summary>
        /// This property stores the source id of a received packet by <see cref="xSysdeviceGetPacket"/> or <see cref="xChannelGetPacket"/>.
        /// </summary>
        /// <seealso cref="xSysdeviceGetPacket"/>
        /// <seealso cref="xChannelGetPacket"/>
        private UInt32 _RcvSourceID;
        public int RcvSourceID
        {
            get
            {
                return (int)_RcvSourceID;
            }
        }
        /// <summary>
        /// This property stores the status of a received packet by <see cref="xSysdeviceGetPacket"/> or <see cref="xChannelGetPacket"/>.
        /// </summary>
        /// <seealso cref="xSysdeviceGetPacket"/>
        /// <seealso cref="xChannelGetPacket"/>
        private UInt32 _RcvState;
        public int RcvState
        {
            get
            {
                return (int)_RcvState;
            }
        }
        /// <summary>
        /// Amount of installed boards in the PC.
        /// </summary>
        /// <seealso cref="xDriverGetInformation"/>
        private static UInt32 _BoardCount = 0;
        int BoardCount
        {
            get
            {
                return (int)_BoardCount;
            } 
        }
        /// <summary>
        /// Boot-up/System error, when trying to handle device
        /// </summary>
        /// <seealso cref="xDriverEnumBoards"/>
        /// <seealso cref="xSysdeviceInfo"/>
        private UInt32 _SystemError;
        int SystemError
        {
            get
            {
                return (int)_SystemError;
            }
        }
        /// <summary>
        /// Total size of the physical dual port memory in bytes.
        /// </summary>
        /// <seealso cref="xDriverEnumBoards"/>
        private UInt32 _DpmPhysicalSize;
        public int DpmPhysicalSize
        {
            get
            {
                return (int)_DpmPhysicalSize;
            }
        }
        /// <summary>
        /// Total size of the DPM provided by the firmware.
        /// </summary>
        /// <seealso cref="xDriverEnumBoards"/>
        private UInt32 _DpmSizeFromFw;
        public int DpmSizeFromFw
        {
            get
            {
                return (int)_DpmSizeFromFw;
            }
        }
        /// <summary>
        /// Device number (as found on the barcode).
        /// </summary>
        /// <seealso cref="xDriverEnumBoards"/>
        private UInt32 _DeviceNumber;
        public int DeviceNumber
        {
            get
            {
                return (int)_DeviceNumber;
            }
        }
        /// <summary>
        /// Serial number (as found on the barcode).
        /// </summary>
        /// <seealso cref="xDriverEnumBoards"/>
        private UInt32 _SerialNumber;
        public int SerialNumber
        {
            get
            {
                return (int)_SerialNumber;
            }
        }
        /// <summary>
        /// Number of I/O Input areas
        /// </summary>
        /// <seealso cref="xDriverEnumChannels"/>
        /// <seealso cref="xChannelInfo"/>
        private UInt32 _IOInAreaCount;
        public int IOInAreaCount
        {
            get
            {
                return (int)_IOInAreaCount;
            }
        }
        /// <summary>
        /// Number of I/O output areas
        /// </summary>
        /// <seealso cref="xDriverEnumChannels"/>
        /// <seealso cref="xChannelInfo"/>
        private UInt32 _IOOutAreaCount;
        public int IOOutAreaCount
        {
            get
            {
                return (int)_IOOutAreaCount;
            }
        }
        /// <summary>
        /// Size of Input/Output area
        /// </summary>
        /// <seealso cref="xDriverEnumChannels"/>
        /// <seealso cref="xChannelIOInfo"/>
        private UInt32 _IOAreaSize;
        public int IOAreaSize
        {
            get
            {
                return (int)_IOAreaSize;
            }
        }
        /// <summary>
        /// Number of used bytes in Input/Output area
        /// </summary>
        /// <seealso cref="xDriverEnumChannels"/>
        /// <seealso cref="xChannelIOInfo"/>
        private UInt32 _IOAreaUsedSize;
        public int IOAreaUsedSize
        {
            get
            {
                return (int)_IOAreaUsedSize;
            }
        }
        /// <summary>
        /// This property stores the command of a send packet by xSysdevicePutPacket or xChannelPutPacket.
        /// </summary>
        /// <seealso cref="xSysdevicePutPacket"/>
        /// <seealso cref="xChannelPutPacket"/>
        private UInt32 _SndCommand;
        public int SndCommand
        {
            set
            {
                _SndCommand = (UInt32)value;
            }
        }
        /// <summary>
        /// This property stores the destination of a send packet by xSysdevicePutPacket or xChannelPutPacket.
        /// </summary>
        /// <seealso cref="xSysdevicePutPacket"/>
        /// <seealso cref="xChannelPutPacket"/>
        private UInt32 _SndDestination;
        int SndDestination
        {
            set
            {
                _SndDestination = (UInt32)value;
            }
        }
        /// <summary>
        /// This property stores the destination id of a send packet by xSysdevicePutPacket or xChannelPutPacket.
        /// </summary>
        /// <seealso cref="xSysdevicePutPacket"/>
        /// <seealso cref="xChannelPutPacket"/>
        private UInt32 _SndDestinationID;
        int SndDestinationID
        {
            set
            {
                _SndDestinationID = (UInt32)value;
            }
        }
        /// <summary>
        /// This property stores the extension of a send packet by xSysdevicePutPacket or xChannelPutPacket.
        /// </summary>
        /// <seealso cref="xSysdevicePutPacket"/>
        /// <seealso cref="xChannelPutPacket"/>
        private UInt32 _SndExtension;
        public int SndExtension
        {
            set
            {
                _SndExtension = (UInt32)value;
            }
        }
        /// <summary>
        /// This property stores the id of a send packet by xSysdevicePutPacket or xChannelPutPacket.
        /// </summary>
        /// <seealso cref="xSysdevicePutPacket"/>
        /// <seealso cref="xChannelPutPacket"/>
        private UInt32 _SndID;
        public int SndID
        {
            set
            {
                _SndID = (UInt32)value;
            }
        }
        /// <summary>
        /// This property stores the length of a send packet by xSysdevicePutPacket or xChannelPutPacket.
        /// </summary>
        /// <seealso cref="xSysdevicePutPacket"/>
        /// <seealso cref="xChannelPutPacket"/>
        private UInt32 _SndLength;
        public int SndLength
        {
            set
            {
                _SndLength = (UInt32)value;
            }
        }
        /// <summary>
        /// This property stores the route of a send packet by xSysdevicePutPacket or xChannelPutPacket.
        /// </summary>
        /// <seealso cref="xSysdevicePutPacket"/>
        /// <seealso cref="xChannelPutPacket"/>
        private UInt32 _SndRoute;
        public int SndRoute
        {
            set
            {
                _SndRoute = (UInt32)value;
            }
        }
        /// <summary>
        /// This property stores the source of a send packet by xSysdevicePutPacket or xChannelPutPacket.
        /// </summary>
        /// <seealso cref="xSysdevicePutPacket"/>
        /// <seealso cref="xChannelPutPacket"/>
        private UInt32 _SndSource;
        public int SndSource
        {
            set
            {
                _SndSource = (UInt32)value;
            }
        }
        /// <summary>
        /// This property stores the source id of a send packet by xSysdevicePutPacket or xChannelPutPacket.
        /// </summary>
        /// <seealso cref="xSysdevicePutPacket"/>
        /// <seealso cref="xChannelPutPacket"/>
        private UInt32 _SndSourceID;
        public int SndSourceID
        {
            set
            {
                _SndSourceID = (UInt32)value;
            }
        }
        /// <summary>
        /// This property stores the state of a send packet by xSysdevicePutPacket or xChannelPutPacket.
        /// </summary>
        /// <seealso cref="xSysdevicePutPacket"/>
        /// <seealso cref="xChannelPutPacket"/>
        private UInt32 _SndState;
        public int SndState
        {
            set
            {
                _SndState = (UInt32)value;
            }
        }

        #endregion

        #region private constants

        ///<exclude/>
        public const int CIFX_NO_ERROR = 0;
        ///<exclude/>
        public const int CIFX_MAX_NUMBER_OF_CHANNEL_DEFINITION = 8;
        ///<exclude/>
        public const int CIFx_MAX_INFO_NAME_LENTH = 16;
        ///<exclude/>
        public const int CIFX_SYSTEM_CHANNEL_DEFAULT_INFO_BLOCK_SIZE = 16;
        ///<exclude/>
        public const int CIFX_PACKET_HEADER_SIZE = 40;                                               /*!< Maximum size of the RCX packet header in bytes */
        ///<exclude/>
        public const int CIFX_MAX_DATA_SIZE = CIFX_MAX_PACKET_SIZE - CIFX_PACKET_HEADER_SIZE;   /*!< Maximum RCX packet data size */
        ///<exclude/>
        public const int CIFX_MAX_PACKET_SIZE = 1600;
        ///<exclude/>
        public const int CIFX_MAX_NUMBER_OF_CHANNELS = 6;
        ///<exclude/>
        public const UInt32 CIFX_NO_CHANNEL = 0xFFFFFFFF;
        ///<exclude/>
        public const int CIFX_INFO_CMD_SYSTEM_INFORMATION = 1;
        ///<exclude/>
        public const int CIFX_INFO_CMD_SYSTEM_INFO_BLOCK = 2;
        ///<exclude/>
        public const int CIFX_INFO_CMD_SYSTEM_CHANNEL_BLOCK = 3;
        ///<exclude/>
        public const int CIFX_INFO_CMD_SYSTEM_CONTROL_BLOCK = 4;
        ///<exclude/>
        public const int CIFX_INFO_CMD_SYSTEM_STATUS_BLOCK = 5;

        ///<exclude/>
        public const int CIFX_HOST_STATE_NOT_READY = 0;
        ///<exclude/>
        public const int CIFX_HOST_STATE_READY = 1;
        ///<exclude/>
        public const int CIFX_HOST_STATE_READ = 2;

        ///<exclude/>
        public const int CIFX_BUS_STATE_OFF = 0;
        ///<exclude/>
        public const int CIFX_BUS_STATE_ON = 1;
        ///<exclude/>
        public const int CIFX_BUS_STATE_GETSTATE = 2;

        ///<exclude/>
        public const int CIFX_IO_INPUT_AREA = 1;
        ///<exclude/>
        public const int CIFX_IO_OUTPUT_AREA = 2;

        #endregion

        #region internal definitions
        ///<exclude/>
        private UInt32 CIFX_CHANNELINIT = 2;
        #endregion

        #region driver specific functions
        ///<exclude/>
        public Int32 xDriverOpen()
        {
            try
            {
                unsafe
                {
                    if(hDriver == null)
                    {
                        hDriverIntPointer = Marshal.AllocHGlobal(4);
                        hDriver = (void*)hDriverIntPointer.ToPointer();
                    }
                    lReturn = CifXAPI._xDriverOpen(ref hDriver);
                    return (Int32)lReturn;
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return 99;
            }
        }

        ///<exclude/>
        public int xDriverEnumBoards(int BoardNumber)
        {
            try
            {
                unsafe
                {
                    lReturn = CifXAPI._xDriverEnumBoards(hDriver,
                                                         (UInt32)BoardNumber,
                                                         (UInt32)Marshal.SizeOf(_BoardInformation),
                                                         ref _BoardInformation);

                    _BoardName = ByteArrayToString(ref _BoardInformation.abBoardName);
                    _BoardAlias = ByteArrayToString(ref _BoardInformation.abBoardAlias);
                    _SystemError = _BoardInformation.ulSystemError;
                    _DpmPhysicalSize = _BoardInformation.ulDpmTotalSize;
                    _DeviceNumber = _BoardInformation.tSystemInfo.ulDeviceNumber;
                    _SerialNumber = _BoardInformation.tSystemInfo.ulSerialNumber;

                    return (int)lReturn;
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return 99;
            }
        }

        ///<exclude/>
        public int xDriverClose()
        {
            try
            {
                unsafe
                {
                    lReturn = 0;
                    if (hDriver != null)
                    {
                        lReturn = CifXAPI._xDriverClose(hDriver);
                        hDriver = null;
                        Marshal.FreeHGlobal(hDriverIntPointer);
                    }
                    return (int)lReturn;
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return 99;
            }
        }

        #endregion

        #region Sysdevice specific functions

        ///<exclude/>        
        private string GetChannelType(byte bCHType)
        {
            switch (bCHType)
            {
                case 0x00:
                    return "UNDEFINED";
                case 0x01:
                    return "NOT AVAILABLE";
                case 0x02:
                    return "RESERVED";
                case 0x03:
                    return "SYSTEM";
                case 0x04:
                    return "HANDSHAKE";
                case 0x05:
                    return "COMMUNICATION";
                case 0x06:
                    return "APPLICATION";
            }
            return "RESERVED";
        }

        #endregion

        #region channel specific functions

        ///<exclude/>
        public int xChannelOpen(string BoardName, int ChannelNumber)
        {
            try
            {
                unsafe
                {
                    if (hChannel == null)
                    {
                        hChannelIntPointer = Marshal.AllocHGlobal(4);
                        hChannel = (void*)hChannelIntPointer.ToPointer();
                    }
                    lReturn = CifXAPI._xChannelOpen(hDriver,
                                                    BoardName,
                                                    (UInt32)ChannelNumber,
                                                    ref hChannel);
                    return (int)lReturn;
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return 99;
            }
        }

        public int xChannelClose()
        {
            try
            {
                unsafe
                {
                    lReturn = 0;
                    if (hChannel != null)
                    {
                        lReturn = CifXAPI._xChannelClose(hChannel);
                        hChannel = null;
                        Marshal.FreeHGlobal(hChannelIntPointer);
                    }
                    return (int)lReturn;
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return 99;
            }
        }
        ///<exclude/>
        private UInt32 WriteFile(byte[] abFileData, string FilePath)
        {
            try
            {
                FileStream filestream = new FileStream(FilePath, FileMode.Create);
                filestream.Write(abFileData, 0, abFileData.Length);
                filestream.Close();
                return CIFX_NO_ERROR;
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return 99;
            }
        }
        ///<exclude/>
        private byte[] ReadFile(string filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }
        ///<exclude/>
        public int xChannelHostState(int ulCmd, ref UInt32 pulState, int ulTimeout)
        {
            try
            {
                unsafe
                {
                    lReturn = CifXAPI._xChannelHostState(hChannel, (UInt32)ulCmd, ref pulState, (UInt32)ulTimeout);
                    return (int)lReturn;
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return 99;
            }
        }

        ///<exclude/>
        public int xChannelBusState(int ulCmd, ref UInt32 pulState, UInt32 ulTimeout)
        {
            try
            {
                unsafe
                {
                    lReturn = CifXAPI._xChannelBusState(hChannel, (UInt32)ulCmd, ref pulState, ulTimeout);
                    return (int)lReturn;
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return 99;
            }
        }

        ///<exclude/>
        public int xChannelIORead(int ulAreaNumber, int ulOffset, int ulDataLen, ref byte[] pvData)
        {
            try
            {
                unsafe
                {
                    lReturn = CifXAPI._xChannelIORead(hChannel, (UInt32)ulAreaNumber, (UInt32)ulOffset, (UInt32)ulDataLen, pvData, 0);
                    return (int)lReturn;
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return 99;
            }
        }

        ///<exclude/>
        public int xChannelIOReadSendData(int ulAreaNumber, int ulOffset, int ulDataLen, ref byte[] pvData)
        {
            try
            {
                unsafe
                {
                    lReturn = CifXAPI._xChannelIOReadSendData(hChannel, (UInt32)ulAreaNumber, (UInt32)ulOffset, (UInt32)ulDataLen, pvData);
                    return (int)lReturn;
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return 99;
            }
        }

        ///<exclude/>
        public int xChannelIOWrite(int ulAreaNumber, int ulOffset, int ulDataLen, ref byte[] pvData)
        {
            try
            {
                unsafe
                {
                    lReturn = CifXAPI._xChannelIOWrite(hChannel, (UInt32)ulAreaNumber, (UInt32)ulOffset, (UInt32)ulDataLen, pvData, 0);
                    return (int)lReturn;
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return 99;
            }
        }

        ///<exclude/>
        public int xChannelIOInfo(int ulCmd, int ulAreaNumber)
        {
            try
            {
                unsafe
                {
                    lReturn = CifXAPI._xChannelIOInfo(hChannel, (UInt32)ulCmd, (UInt32)ulAreaNumber, (UInt32)Marshal.SizeOf(_ChannelIOInformation), ref _ChannelIOInformation);
                    _IOAreaSize = _ChannelIOInformation.ulTotalSize;
                    _IOAreaUsedSize = _ChannelIOInformation.ulUsedSize;
                    return (int)lReturn;
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return 99;
            }
        }

        #endregion

        #region specific methods

        public string ByteArrayToString(ref byte[] ArrayOfByte)
        {
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                return enc.GetString(ArrayOfByte);
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return _ErrorDescription;
            }
        }
        ///<exclude/>
        public byte[] CreateOutputData(string StringToSplit, bool AutoIncrement)
        {
            //delete all existing blanks
            StringToSplit = StringToSplit.Replace(" ", "");

            int iLen = StringToSplit.Length;
            if (iLen > 0)
            {
                //insert new blanks
                for (int i = 2; i <= iLen; i += 3)
                {
                    StringToSplit = StringToSplit.Insert(i, " ");
                    iLen++;
                }

                //split the string into an array of string
                string[] arTemp = StringToSplit.Split(new Char[] { ' ' });
                //create a new array for the byte data
                byte[] pvData = new byte[arTemp.Length - 1];

                //convert each value of the textfield to a corresponding hexadecimal value
                int iIndex = 0;
                foreach (string s in arTemp)
                {
                    pvData[iIndex] = Convert.ToByte(int.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier));
                    if (AutoIncrement == true)
                        pvData[iIndex]++;
                    iIndex++;
                    if (iIndex == pvData.Length)
                        break;
                }
                StringToSplit = "";

                return pvData;
            }
            byte[] pvNullData = new byte[0];
            return pvNullData;
        }
        ///<exclude/>
        public int StringToInt(string sTemp)
        {
            UInt32 uiTmp = Convert.ToUInt32(sTemp, 16);
            int iReturn = (int)uiTmp;
            return iReturn;
        }
        ///<exclude/>
        public string IntToString(int iTemp)
        {
            string sTemp = string.Format("0x{0:X8}", iTemp);
            return sTemp;
        }

        #endregion
    }
}
