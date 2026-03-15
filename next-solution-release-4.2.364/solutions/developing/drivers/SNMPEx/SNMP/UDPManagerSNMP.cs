using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBaseEx;
using IpDriverCodeBaseEx;

namespace SNMP
{
    class UDPManagerSNMP : UDPManager
    {
        #region Constructors
        public UDPManagerSNMP(Channel channel, UdpChannelSettings settings)
            : base(channel, settings)
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
