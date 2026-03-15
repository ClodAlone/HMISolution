using System;
using DriverCodeBaseEx;
using IpDriverCodeBaseEx;

namespace SaiaDataMode
{
    public class UDPManagerSaiaDataMode : UDPManager
    {
        #region Constructors
        public UDPManagerSaiaDataMode(Channel channel, UdpChannelSettings settings)
            :base(channel, settings)
        { 

        }
        #endregion

        #region Override Methods
        public override bool IsDeviceOpen()
        {
            bool returnValue;
            lock (lockStream)
            {
                returnValue = Client != null;
            }
            // evaluate Channel State into Channel.IsDeviceOpen()
            //refChannel.SetStateCommandVariableBit(!returnValue, (UInt16)ChannelVariableBits.ChannelUnconnected);
            return (returnValue);
        }
        #endregion       
    }
}
