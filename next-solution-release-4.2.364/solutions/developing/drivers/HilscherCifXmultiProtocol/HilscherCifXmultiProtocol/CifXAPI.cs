using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
//using System.Windows.Forms;

namespace HilscherCifXmultiProtocol
{
    class CifXAPI
    {
        #region API Calls

        [DllImport("cifx32dll.dll", EntryPoint = "xDriverOpen")]
        public static extern unsafe UInt32 _xDriverOpen(ref void* cifXHandle);

        [DllImport("cifx32dll.dll", EntryPoint = "xDriverClose")]
        public static extern unsafe UInt32 _xDriverClose(void* cifXHandle);

        [DllImport("cifx32dll.dll", EntryPoint = "xDriverGetErrorDescription")]
        public static extern UInt32 _xDriverGetErrorDescription(UInt32 lError,
                                                                [Out, MarshalAs(UnmanagedType.LPArray)] byte[] szBuffer,
                                                                UInt32 ulBufferLen);

        [DllImport("cifx32dll.dll", EntryPoint = "xDriverEnumBoards")]
        public static extern unsafe UInt32 _xDriverEnumBoards(void* cifXHandle,
                                                              UInt32 ulBoard,
                                                              UInt32 ulSize,
                                                              ref CifXAPIStruct.BOARD_INFORMATIONtag pvBoardInfo);

        [DllImport("cifx32dll.dll", EntryPoint = "xDriverEnumChannels")]
        public static extern UInt32 _xDriverEnumChannels(UInt32 CIFXHANDLE,
                                                            UInt32 ulBoard,
                                                            UInt32 ulChannel,
                                                            UInt32 ulSize,
                                                            ref CifXAPIStruct.CHANNEL_INFORMATIONtag pvChannelInfo);

        /* Channel depending functions */
        [DllImport("cifx32dll.dll", EntryPoint = "xChannelOpen")]
        public static extern unsafe UInt32 _xChannelOpen(void* cifXHandle,
                                                         string szBoard,
                                                         UInt32 ulChannel,
                                                         ref void* channelHandle);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelClose")]
        public static extern unsafe UInt32 _xChannelClose(void* hChannel);


        [DllImport("cifx32dll.dll", EntryPoint = "xChannelHostState")]
        public static extern unsafe UInt32 _xChannelHostState(void* hChannel,
                                                              UInt32 ulCmd,
                                                              ref UInt32 pulState,
                                                              UInt32 ulTimeout);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelBusState")]
        public static extern unsafe UInt32 _xChannelBusState(void* hChannel,
                                                             UInt32 ulCmd,
                                                             ref UInt32 pulState,
                                                             UInt32 ulTimeout);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelIOInfo")]
        public static extern unsafe UInt32 _xChannelIOInfo(void* hChannel,
                                                           UInt32 ulCmd,
                                                           UInt32 ulAreaNumber,
                                                           UInt32 ulSize,
                                                           ref CifXAPIStruct.CHANNEL_IO_INFORMATIONtag pvData);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelIORead")]
        public static extern unsafe UInt32 _xChannelIORead(void* hChannel,
                                                           UInt32 ulAreaNumber,
                                                           UInt32 ulOffset,
                                                           UInt32 ulDataLen,
                                                           [In, MarshalAs(UnmanagedType.LPArray)] byte[] pvData,
                                                           UInt32 ulTimeout);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelIOWrite")]
        public static extern unsafe UInt32 _xChannelIOWrite(void* hChannel,
                                                            UInt32 ulAreaNumber,
                                                            UInt32 ulOffset,
                                                            UInt32 ulDataLen,
                                                            [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pvData,
                                                            UInt32 ulTimeout);

        [DllImport("cifx32dll.dll", EntryPoint = "xChannelIOReadSendData")]
        public static extern unsafe UInt32 _xChannelIOReadSendData(void* hChannel,
                                                                   UInt32 ulAreaNumber,
                                                                   UInt32 ulOffset,
                                                                   UInt32 ulDataLen,
                                                                   [In, MarshalAs(UnmanagedType.LPArray)] byte[] pvData);

        #endregion

    }
}
