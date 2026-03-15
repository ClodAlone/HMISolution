using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace RedundancyService
{
    internal class BinaryNetTcpBinding : CustomBinding
    {
        #region Declarations
        readonly TcpTransportBindingElement tcpTransport;
        #endregion

        #region Constructors
        public BinaryNetTcpBinding(SecurityMode securityMode)
        {
            var bynaryEncoding = new BinaryMessageEncodingBindingElement();
            bynaryEncoding.MessageVersion = MessageVersion.Default;
            bynaryEncoding.CompressionFormat = CompressionFormat.GZip;
            Elements.Add(bynaryEncoding);

            tcpTransport = new TcpTransportBindingElement();
            Elements.Add(tcpTransport);
        }
        #endregion

        #region Properties
        public long MaxReceivedMessageSize
        {
            get
            {
                return tcpTransport.MaxReceivedMessageSize;
            }
            set
            {
                if (tcpTransport.MaxReceivedMessageSize == value)
                    return;

                tcpTransport.MaxReceivedMessageSize = value;
            }
        }
        #endregion
    }
}
