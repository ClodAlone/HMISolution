using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFInterfaces.Constants
{
    public class TransportOrder
    {
        public readonly static string[] transportOrderByRelevance = new string[]
		{
#if !NET_STANDARD
            Opc.Ua.Utils.UriSchemeNetPipe,
            Opc.Ua.Utils.UriSchemeNetTcp,
#endif
            Opc.Ua.Utils.UriSchemeOpcTcp,
            Opc.Ua.Utils.UriSchemeHttps,
#if !NET_STANDARD
            Opc.Ua.Utils.UriSchemeHttp,
            Opc.Ua.Utils.UriSchemeNoSecurityHttp
#endif
		};
    }
}
