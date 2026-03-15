using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace LacbusPC
{
    public unsafe delegate void ConnectionCallbackFunc(void* connection, void* user_data);
    public unsafe delegate void OnCloseConnectionCallbackFunc(void* connection);
    public unsafe delegate Int32 OnStartChannelCallbackFunc(Int32 channel_num, void* connection, void* user_data);
    public unsafe delegate Int32 OnCloseChannelCallbackFunc(Int32 channel_num, void* connection, void* user_data);
    public unsafe delegate void OnFrameReceivedCallbackFunc(void* channel, void* connection, void* frame, void* user_data);
    public unsafe delegate void OnCreatedChannelCallbackFunc(Int32 channel_num, void* channel, void* connection, void* user_data);
    public unsafe delegate void OnClosedNotificationCallbackFunc(Int32 channel_num, Int32 was_closed, string code, string msg);

    class VortexAPI
    {
        #region API Calls

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_init_ctx")]
        public static extern unsafe Int32 _vortex_init_ctx(void* ctx);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_exit_ctx")]
        public static extern unsafe void _vortex_exit_ctx(void* ctx, Int32 free_ctx);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_ctx_new")]
        public static extern unsafe void* _vortex_ctx_new();

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_ctx_free")]
        public static unsafe extern void _vortex_ctx_free(void* ctx);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_connection_new")]
        public static extern unsafe void* _vortex_connection_new(void* ctx, string host, string port, ConnectionCallbackFunc on_connected, void* user_data);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_connection_is_ok")]
        public static extern unsafe Int32 _vortex_connection_is_ok(void* connection, int free_on_fail);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_connection_close")]
        public static extern unsafe Int32 _vortex_connection_close(void* connection);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_connection_connect_timeout")]
        public static extern unsafe void _vortex_connection_connect_timeout(void* ctx, Int32 microseconds_to_wait);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_connection_timeout")]
        public static extern unsafe void _vortex_connection_timeout(void* ctx, Int32 microseconds_to_wait);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_connection_get_timeout")]
        public static extern unsafe Int32 _vortex_connection_get_timeout(void* ctx);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_connection_get_connect_timeout")]
        public static extern unsafe Int32 _vortex_connection_get_connect_timeout(void* ctx);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_connection_shutdown")]
        public static extern unsafe void _vortex_connection_shutdown(void* connection);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_profiles_register")]
        public static extern unsafe Int32 _vortex_profiles_register(void* ctx, string uri, IntPtr start, void* start_user_data,
                                                                    IntPtr close, void* close_user_data,
                                                                    IntPtr received, void* received_user_data);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_connection_is_profile_supported")]
        public static extern unsafe Int32 _vortex_connection_is_profile_supported(void* connection, string uri);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_channel_new")]
        public static extern unsafe void* _vortex_channel_new(void* connection, Int32 channel_num, string profile, IntPtr close,
                                                              void* close_user_data, IntPtr received, void* received_user_data,
                                                              IntPtr on_channel_created, void* user_data);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_channel_new_full")]
        public static extern unsafe void* _vortex_channel_new_full(void* connection, Int32 channel_num, string serverName, string profile, Int32 encoding,
                                                                   string profile_content, Int32 profile_content_size, IntPtr close,
                                                                   void* close_user_data, IntPtr received, void* received_user_data,
                                                                   IntPtr on_channel_created, void* user_data);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_channel_close")]
        public static extern unsafe Int32 _vortex_channel_close(void* channel, IntPtr on_closed);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_frame_get_msgno")]
        public static extern unsafe Int32 _vortex_frame_get_msgno(void* frame);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_channel_send_rpy")]
        public static extern unsafe Int32 _vortex_channel_send_rpy(void* channel, string message, UInt32 message_size, Int32 msg_no_rpy);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_connection_get_channel")]
        public static extern unsafe void* _vortex_connection_get_channel(void* connection, Int32 channel_num);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_frame_get_payload_size")]
        public static extern unsafe Int32 _vortex_frame_get_payload_size(void* frame);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_frame_get_payload")]
        public static extern unsafe void* _vortex_frame_get_payload(void* frame);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_channel_send_msg")]
        public static extern unsafe Int32 _vortex_channel_send_msg(void* channel, void* message, UInt32 message_size, int* msg_no);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_connection_set_on_close")]
        public static extern unsafe void _vortex_connection_set_on_close(void* connection, IntPtr on_close_handler);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_channel_have_piggyback")]
        public static extern unsafe Int32 _vortex_channel_have_piggyback(void* channel);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_channel_get_piggyback")]
        public static extern unsafe void* _vortex_channel_get_piggyback(void* channel);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_channel_unref")]
        public static extern unsafe void _vortex_channel_unref(void* channel);

        [DllImport("libvortex-1.1.dll", EntryPoint = "vortex_connection_get_socket")]
        public static extern unsafe Int32 _vortex_connection_get_socket(void* connection);

        [DllImport("ws2_32.dll", EntryPoint = "WSAIoctl")]
        public static extern unsafe int _ws2_WSAIoctl(int s, uint dwIoControlCode, void* lpvInBuffer, uint cbInBuffer, void* lpvOutBuffer, uint cbOutBuffer, void* lpcbBytesReturned, void* lpOverlapped, void* lpCompletionRoutine);

        [DllImport("ws2_32.dll", EntryPoint = "WSAGetLastError")]
        public static extern unsafe int _ws2_WSAGetLastError();

        #endregion
    }
}
