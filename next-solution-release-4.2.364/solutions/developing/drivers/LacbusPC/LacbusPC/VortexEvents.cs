using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace LacbusPC
{
    public class VortexEvents : Object
    {
        #region Constructors
        public VortexEvents(LacbusPCChannel channel)
        {
            correspondingChannel = channel;
        }
        #endregion

        #region Data members
        LacbusPCChannel correspondingChannel;
        #endregion

        #region Methods
        public unsafe void FrameReceivedFromRemotePeerCallbackFunc(void* channel, void* connection, void* frame, void* user_data)
        {
            int messageNumber = 0;
            try
            {
                messageNumber = VortexAPI._vortex_frame_get_msgno(frame);
                System.Diagnostics.Debug.WriteLine("VortexEvents.FrameReceivedFromRemotePeerCallbackFunc: received frame n. {0}", messageNumber);
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine("VortexEvents.FrameReceivedFromRemotePeerCallbackFunc: exception {0} getting message number", ex.Message);
                return;
            }

            try
            {
                unsafe
                {

                    Int32 messageDataSize = VortexAPI._vortex_frame_get_payload_size(frame);
                    System.Diagnostics.Debug.WriteLine("VortexEvents.FrameReceivedFromRemotePeerCallbackFunc: message length =  {0}", messageDataSize);
                    if (messageDataSize > 0)
                    {
                        byte[] framePayload = new byte[messageDataSize];
                        byte* framePayloadPointer = (byte*)VortexAPI._vortex_frame_get_payload(frame);
                        Marshal.Copy((IntPtr)framePayloadPointer, framePayload, 0, messageDataSize);
                        System.Diagnostics.Debug.WriteLine("VortexEvents.FrameReceivedFromRemotePeerCallbackFunc: copy of the message {0} succesfully done: [{1}]", messageNumber, framePayload[0]);
                        if (correspondingChannel != null)
                        {
                            correspondingChannel.AddMessageFromRemote(framePayload);
                        }
                    }

                    string replyMessage = "<ok />";
                    if (VortexAPI._vortex_channel_send_rpy(channel, replyMessage, (uint)replyMessage.Length, messageNumber) == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("VortexEvents.FrameReceivedFromRemotePeerCallbackFunc: failure sending reply to message number {0}", messageNumber);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("VortexEvents.FrameReceivedFromRemotePeerCallbackFunc: reply to message number {0} succesfully sent", messageNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("VortexEvents.FrameReceivedFromRemotePeerCallbackFunc: exception {0} sending reply to message number {1}", ex.Message, messageNumber);
                return;
            }

            return;
        }

        public unsafe void ReplyReceivedFromRemotePeerCallbackFunc(void* channel, void* connection, void* frame, void* user_data)
        {
            int messageNumber = 0;
            try
            {
                messageNumber = VortexAPI._vortex_frame_get_msgno(frame);
                System.Diagnostics.Debug.WriteLine("VortexEvents.ReplyReceivedFromRemotePeerCallbackFunc: received frame n. {0}", messageNumber);
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine("VortexEvents.ReplyReceivedFromRemotePeerCallbackFunc: exception {0} getting the message number", ex.Message);
                return;
            }

            try
            {
                unsafe
                {
                    Int32 messageDataSize = VortexAPI._vortex_frame_get_payload_size(frame);
                    System.Diagnostics.Debug.WriteLine("VortexEvents.ReplyReceivedFromRemotePeerCallbackFunc: message length =  {0}", messageDataSize);
                    if (messageDataSize > 0)
                    {
                        byte[] framePayload = new byte[messageDataSize];
                        byte* framePayloadPointer = (byte*)VortexAPI._vortex_frame_get_payload(frame);
                        Marshal.Copy((IntPtr)framePayloadPointer, framePayload, 0, messageDataSize);
                        System.Diagnostics.Debug.WriteLine("VortexEvents.ReplyReceivedFromRemotePeerCallbackFunc: copy of the message succesfully done: [{0}]", framePayload[0]);
                        if (correspondingChannel != null)
                        {
                            correspondingChannel.AddRepliesFromRemote(messageNumber, framePayload);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("VortexEvents.ReplyReceivedFromRemotePeerCallbackFunc: exception {0} getting the reply to message number {1}", ex.Message, messageNumber);
                return;
            }

            return;
        }

        public unsafe void ConnectionCloseCallbackFunc(void* connection)
        {
            System.Diagnostics.Debug.WriteLine("VortexEvents.ConnectionCloseCallbackFunc as been called at {0}", DateTime.Now);
            if (correspondingChannel != null)
            {
                correspondingChannel.SetConnectionBroken();
            }
        }
        #endregion
    }
}
