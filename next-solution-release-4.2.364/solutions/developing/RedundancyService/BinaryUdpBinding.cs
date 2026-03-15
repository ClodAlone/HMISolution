using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace RedundancyService
{
    internal class BinaryUdpBinding : CustomBinding
    {
        #region Declarations
        readonly UdpTransportBindingElement udpTransport;
        #endregion

        #region Constructors
        public BinaryUdpBinding()
        {
            var bynaryEncoding = new BinaryMessageEncodingBindingElement();
            bynaryEncoding.MessageVersion = MessageVersion.Default;
            bynaryEncoding.CompressionFormat = CompressionFormat.None;
            Elements.Add(bynaryEncoding);

            udpTransport = new UdpTransportBindingElement();
            Elements.Add(udpTransport);
        }
        #endregion

        #region Properties
        public int MaxRetransmitCount
        {
            get
            {
                return udpTransport.RetransmissionSettings.MaxMulticastRetransmitCount;
            }
            set
            {
                if (udpTransport.RetransmissionSettings.MaxMulticastRetransmitCount == value)
                    return;

                udpTransport.RetransmissionSettings.MaxMulticastRetransmitCount = value;
            }
        }
        #endregion
    }
}
