using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace LacbusPC
{
    class VortexDevManager
    {
        #region Constructors
        public VortexDevManager()
        {
            _ErrorDescription = "";
        }
        #endregion

        #region Data Members
        public unsafe void* hContext = null;
        public unsafe void* hConnection = null;
        #endregion

        #region Properties

        /// <summary>
        /// This property stores the last error description.
        /// </summary>
        private string _ErrorDescription;
        public string ErrorDescription
        {
            get
            {
                return _ErrorDescription;
            }
        }
        #endregion

        #region Specific methods

        public void ResetVortexConnection()
        {
            try
            {
                unsafe
                {
                    if (hConnection != null)
                    {
                        hConnection = null;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        ///<exclude/>
        public int vortex_init_ctx()
        {
            try
            {
                unsafe
                {
                    if (hContext == null)
                    {
                        // Create the context for the Vortex library
                        hContext = VortexAPI._vortex_ctx_new();
                        if (hContext == null)
                        {
                            return (-1);
                        }
                    }

                    // Initialize the Vortex library
                    if (VortexAPI._vortex_init_ctx(hContext) == 0)
                    {
                        VortexAPI._vortex_ctx_free(hContext);
                        return (-2);
                    }
                }
                return (0);
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return(-99);
            }
        }

        public int vortex_exit_ctx()
        {
            try
            {
                unsafe
                {
                    if (hContext != null)
                    {
                        // Exit from Vortex library
                        VortexAPI._vortex_exit_ctx(hContext, 1);
                        hContext = null;
                    }
                }
                return (0);
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return (-99);
            }
        }

        public int vortex_profiles_register(string profileUri,
                                            IntPtr start,
                                            IntPtr close,
                                            IntPtr received)
        {
            try
            {
                unsafe
                {
                    // Check the context for the Vortex library
                    if (hContext == null)
                    {
                        return (-1);
                    }

                    // Register the profile
                    if(VortexAPI._vortex_profiles_register(hContext, profileUri, start, null, close, null, received, null) == 0)
                    {
                        return (-2);
                    }
                    return(0);
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return (-99);
            }
        }

        public int vortex_open_connection(string hostName, UInt32 hostPort, UInt32 connectionTimeout)
        {
            try
            {
                unsafe
                {
                    // Check the context for the Vortex library
                    if (hContext == null)
                    {
                        return (-1);
                    }

                    // Connection already established?
                    if(hConnection != null)
                    {
                        return (0);
                    }

                    // Set the connection timeout (synchronous connection)
                    Int32 connTimeoutInMicroseconds = (Int32)(connectionTimeout * 1000);
                    VortexAPI._vortex_connection_connect_timeout(hContext, connTimeoutInMicroseconds);
                    Int32 currentConnTimeout = VortexAPI._vortex_connection_get_connect_timeout(hContext);
                    VortexAPI._vortex_connection_timeout(hContext, connTimeoutInMicroseconds);
                    Int32 currentTimeout = VortexAPI._vortex_connection_get_timeout(hContext);
                    //System.Diagnostics.Debug.WriteLine("SwitchServerDEBUG - vortex_open_connection {0} - hostName = {1} ConnTimeout = {2} Timeout = {3}", DateTime.Now.ToString("HH:mm:ss.fff"), hostName, currentConnTimeout, currentTimeout);

                    // Open synchronously a TCP/IP session with the server
                    string hostPortString = hostPort.ToString();
                    hConnection = VortexAPI._vortex_connection_new(hContext, hostName, hostPortString, null, null);

                    // Verify the connection
                    if(VortexAPI._vortex_connection_is_ok(hConnection, 0) == 0)
                    {
                        VortexAPI._vortex_connection_close(hConnection);
                        hConnection = null;
                        return (-3);
                    }
                }

                return (0);
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return (-99);
            }
        }

        public int vortex_close_connection()
        {
            try
            {
                unsafe
                {
                    if(hConnection != null)
                    {
                        VortexAPI._vortex_connection_shutdown(hConnection);
                        VortexAPI._vortex_connection_close(hConnection);
                        hConnection = null;
                    }
                }
                return (0);
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return (-99);
            }
        }

        public int vortex_connection_set_on_close(IntPtr onClose)
        {
            try
            {
                unsafe
                {
                    if (hConnection != null)
                    {
                        VortexAPI._vortex_connection_set_on_close(hConnection, onClose);
                    }
                }
                return (0);
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return (-99);
            }
        }

        public unsafe int vortex_channel_new(string profile, ref void* channel)
        {
            try
            {
                unsafe
                {
                    channel = null;
                    // Check the handles of the context and of the connection 
                    if ((hContext == null) || (hConnection == null))
                    {
                        return (-1);
                    }

                    // Check if the profile is supported
                    if(VortexAPI._vortex_connection_is_profile_supported(hConnection, profile) == 0)
                    {
                        return (-2);
                    }

                    // Open the BEEP channel
                    channel = VortexAPI._vortex_channel_new(hConnection, 0, profile, (IntPtr)null, null, (IntPtr)null, null, (IntPtr)null, null);
                    if (channel == null)
                    {
                        return (-3);
                    }

                    // Success
                    return (0);
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return (-99);
            }
        }

        public unsafe int vortex_connection_is_profile_supported(string profile)
        {
            try
            {
                // Check the handles of the context and of the connection 
                if ((hContext == null) || (hConnection == null))
                {
                    return (-1);
                }

                // Check if the profile is supported
                if (VortexAPI._vortex_connection_is_profile_supported(hConnection, profile) == 0)
                {
                    return (-2);
                }

                // Success
                return (0);
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return (-99);
            }
        }

        // Structure for setting the keep-alive parameters
        [
            System.Runtime.InteropServices.StructLayout
            (
                System.Runtime.InteropServices.LayoutKind.Explicit
            )
        ]
        unsafe struct TcpKeepAlive
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            [
                System.Runtime.InteropServices.MarshalAs
                (
                    System.Runtime.InteropServices.UnmanagedType.ByValArray,
                    SizeConst = 12
                )
            ]
            public fixed byte Bytes[12];

            [System.Runtime.InteropServices.FieldOffset(0)]
            public uint OnOff;

            [System.Runtime.InteropServices.FieldOffset(4)]
            public uint KeepAliveTime;

            [System.Runtime.InteropServices.FieldOffset(8)]
            public uint KeepAliveInterval;
        }

        public unsafe int SetKeepAlive()
        {
            IntPtr pnt = Marshal.AllocHGlobal(12);
            IntPtr pntNum = Marshal.AllocHGlobal(4);
            try
            {
                unsafe
                {
                    // Check the handles of the context and of the connection 
                    if ((hContext == null) || (hConnection == null))
                    {
                        Marshal.FreeHGlobal(pnt);
                        Marshal.FreeHGlobal(pntNum);
                        return (-1);
                    }

                    // Get the socket handle of the current connection
                    Int32 socket = VortexAPI._vortex_connection_get_socket(hConnection);
                    if (socket == -1)
                    {
                        Marshal.FreeHGlobal(pnt);
                        Marshal.FreeHGlobal(pntNum);
                        return (-2);
                    }

                    // Keep-alive configuration
                    // keep-alive parameters
                    // Send a keep-alive message after 15 seconds inactivity
                    // Repeat every 5 seconds
                    TcpKeepAlive keepAliveParameters = new TcpKeepAlive();
                    keepAliveParameters.OnOff = 1;
                    keepAliveParameters.KeepAliveTime = 15000;
                    keepAliveParameters.KeepAliveInterval = 5000;
                    byte[] keepAliveParamArray = new byte[12];
                    for(int i=0; i< 12; i++)
                    {
                        keepAliveParamArray[i] = keepAliveParameters.Bytes[i];
                    }
                    Marshal.Copy(keepAliveParamArray, 0, pnt, 12);
                    uint sio_keepalive_vals = 0x98000004;
                    int returnValue = VortexAPI._ws2_WSAIoctl(socket, sio_keepalive_vals, (void*)pnt, 12, null, 0, (void*)pntNum, null, null);
                    Marshal.FreeHGlobal(pnt);
                    Marshal.FreeHGlobal(pntNum);

                    if(returnValue != 0)
                    {
                        _ErrorDescription = String.Format("Error {0} setting keep alive parameters", VortexAPI._ws2_WSAGetLastError());
                        System.Diagnostics.Debug.WriteLine(_ErrorDescription);
                    }
                    return (returnValue);
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
               Marshal.FreeHGlobal(pnt);
               Marshal.FreeHGlobal(pntNum);
               return (-99);
            }
        }

        public unsafe int vortex_channel_new(string profile, ref void* channel, IntPtr close, IntPtr receive, IntPtr created)
        {
            try
            {
                unsafe
                {
                    channel = null;
                    // Check the handles of the context and of the connection 
                    if ((hContext == null) || (hConnection == null))
                    {
                        return (-1);
                    }

                    // Check if the profile is supported
                    if (VortexAPI._vortex_connection_is_profile_supported(hConnection, profile) == 0)
                    {
                        return (-2);
                    }

                    // Open the BEEP channel
                    channel = VortexAPI._vortex_channel_new(hConnection, 0, profile, close, null, receive, null, created, null);
                    if (channel == null)
                    {
                        return (-3);
                    }

                    // Check the reply to the channel creation request
                    if (VortexAPI._vortex_channel_have_piggyback(channel) != 0)
                    {
                        void* frame = VortexAPI._vortex_channel_get_piggyback(channel);
                        Int32 messageDataSize = VortexAPI._vortex_frame_get_payload_size(frame);
                        if (messageDataSize > 0)
                        {
                            byte[] framePayload = new byte[messageDataSize];
                            byte* framePayloadPointer = (byte*)VortexAPI._vortex_frame_get_payload(frame);
                            Marshal.Copy((IntPtr)framePayloadPointer, framePayload, 0, messageDataSize);
                            Encoding enc8 = Encoding.UTF8;
                            string replyString = enc8.GetString(framePayload);
                            System.Diagnostics.Debug.WriteLine("VortexDevManager.vortex_channel_new: received reply: {0}", framePayload);
                            if (IsErrorReply(replyString) == true)
                            {
                                VortexAPI._vortex_channel_unref(channel);
                                channel = null;
                                return (-4);
                            }
                        }
                    }

                    // Success
                    return (0);
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return (-99);
            }
        }

        private bool IsErrorReply(string reply)
        {
            bool errorReply = false;
            if(String.IsNullOrWhiteSpace(reply) == false)
            {
                int errorLabelIndex = reply.IndexOf("<error");
                // Error reply?
                if (errorLabelIndex >= 0)
                {
                    errorReply = true;
                    _ErrorDescription = String.Empty;
                    // Get the error code
                    int errorCodeInitString = reply.IndexOf("'");
                    if(errorCodeInitString >= 0)
                    {
                        errorCodeInitString++;
                        int errorCodeEndString = reply.LastIndexOf("'");
                        if (errorCodeEndString > errorCodeInitString)
                        {
                            _ErrorDescription = reply.Substring(errorCodeInitString, errorCodeEndString - errorCodeInitString);
                        }
                    }
                    // Get the error description
                    int errorDescriptionInitString = reply.IndexOf(">");
                    if(errorDescriptionInitString >= 0)
                    {
                        errorDescriptionInitString++;
                        int errorDescriptionEndString = reply.LastIndexOf("<");
                        if (errorDescriptionEndString > errorDescriptionInitString)
                        {
                            _ErrorDescription += " ";
                            _ErrorDescription += reply.Substring(errorDescriptionInitString, errorDescriptionEndString - errorDescriptionInitString);
                        }
                    }
                }
            }

            return (errorReply);
        }

        public unsafe int vortex_channel_unref(ref void* channel)
        {
            try
            {
                if(channel != null)
                {
                    VortexAPI._vortex_channel_unref(channel);
                    channel = null;
                }

                return (0);
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return (-99);
            }
        }

        public unsafe int vortex_channel_new_full(string profile, string profile_content, Int32 profile_content_size, ref void* channel)
        {
            try
            {
                unsafe
                {
                    channel = null;
                    // Check the handles of the context and of the connection 
                    if ((hContext == null) || (hConnection == null))
                    {
                        return (-1);
                    }

                    // Check if the profile is supported
                    if (VortexAPI._vortex_connection_is_profile_supported(hConnection, profile) == 0)
                    {
                        return (-2);
                    }

                    // Open the BEEP channel
                    channel = VortexAPI._vortex_channel_new_full(hConnection, 0, null, profile, 1, profile_content, profile_content_size, (IntPtr)null, null, (IntPtr)null, null, (IntPtr)null, null);
                    if (channel == null)
                    {
                        return (-3);
                    }

                    // Check the reply to the channel creation request
                    if (VortexAPI._vortex_channel_have_piggyback(channel) != 0)
                    {
                        void* frame = VortexAPI._vortex_channel_get_piggyback(channel);
                        Int32 messageDataSize = VortexAPI._vortex_frame_get_payload_size(frame);
                        if (messageDataSize > 0)
                        {
                            byte[] framePayload = new byte[messageDataSize];
                            byte* framePayloadPointer = (byte*)VortexAPI._vortex_frame_get_payload(frame);
                            Marshal.Copy((IntPtr)framePayloadPointer, framePayload, 0, messageDataSize);
                            Encoding enc8 = Encoding.UTF8;
                            string replyString = enc8.GetString(framePayload);
                            System.Diagnostics.Debug.WriteLine("VortexDevManager.vortex_channel_new_full: received reply: {0}", framePayload);
                            if(IsErrorReply(replyString) == true)
                            {
                                VortexAPI._vortex_channel_unref(channel);
                                channel = null;
                                return (-4);
                            }
                        }
                    }

                    // Success
                    return (0);
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return (-99);
            }
        }

        public unsafe int vortex_channel_close(void* channel)
        {
            try
            {
                unsafe
                {
                    if(channel != null)
                    {
                        if(VortexAPI._vortex_channel_close(channel, (IntPtr)null) == 0)
                        {
                            return (-1);
                        }
                    }

                    // Success
                    return (0);
                }
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return (-99);
            }
        }

        public unsafe int vortex_frame_get_msgno(void* frame, ref int messageNumber)
        {
            try
            {
                messageNumber = VortexAPI._vortex_frame_get_msgno(frame);
                return (0);
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return (-99);
            }
        }

        public unsafe int vortex_channel_send_rpy(void* channel, string message, uint message_size, int msg_no_rpy)
        {
            try
            {
                if(VortexAPI._vortex_channel_send_rpy(channel, message, message_size, msg_no_rpy) == 0)
                {
                    return (-1);
                }

                // Success
                return (0);
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                return (-99);
            }
        }

        public unsafe int vortex_channel_send_msg(void* channel, ref byte[] message, int message_size, ref int msg_no)
        {
            // Allocate a pointer (unmanaged memory) for the message
            IntPtr pnt = Marshal.AllocHGlobal(message_size);
            // Allocate a pointer (unmanaged memory) for the message number
            IntPtr pntNum = Marshal.AllocHGlobal(4);

            try
            {
                unsafe
                {
                    // Copy the array to unmanaged memory
                    Marshal.Copy(message, 0, pnt, message_size);
                    int* messageNumber = (int*)pntNum.ToPointer(); 

                    // Send the message
                    if (VortexAPI._vortex_channel_send_msg(channel, (void*)pnt, (uint)message_size, messageNumber) == 0)
                    {
                        Marshal.FreeHGlobal(pnt);
                        Marshal.FreeHGlobal(pntNum);
                        return (-1);
                    }

                    // Set the message number
                    byte[] managedArray = new byte[4];
                    Marshal.Copy(pntNum, managedArray, 0, 4);
                    msg_no = BitConverter.ToInt32(managedArray, 0);

                    System.Diagnostics.Debug.WriteLine("vortex_channel_send_msg: size {0} byte[0] = [{1}], byte[size-1] = [{2}], mes num = {3}",
                        message_size, message[0], message[message_size - 1], msg_no);
                }

                Marshal.FreeHGlobal(pnt);
                Marshal.FreeHGlobal(pntNum);

                // Success
                return (0);
            }
            catch (Exception ex)
            {
                _ErrorDescription = ex.Message.ToString();
                Marshal.FreeHGlobal(pnt);
                Marshal.FreeHGlobal(pntNum);
                return (-99);
            }
        }
        #endregion
    }
}
