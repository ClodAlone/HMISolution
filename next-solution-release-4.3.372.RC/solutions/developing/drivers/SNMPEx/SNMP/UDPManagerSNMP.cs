using System;
using System.Linq;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
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
            lock (lockStream)
            {
                bool returnValue = false;
                lock (lockStream)
                    returnValue = Client != null;                
                refChannel.SetStateCommandVariableBit((!returnValue || InErrorState()), (UInt16)ChannelVariableBits.ChannelUnconnected);
                return (returnValue);
            }

        }
        #endregion

        #region Methods
        private bool InErrorState()
        {
            // set channel in error only if all station are in error
            return (((SNMPChannel)refChannel).GetStations().Count(s => !s.InErrorState) == 0);
        }
        #endregion
    }
}
