using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace UFUACommonControls
{
    public class TransportList : ObservableCollection<string>
    {
        public TransportList()
            : base()
        {
        }

        public void InitList(IList<string> appBaseAddresses)
        {
            if (appBaseAddresses.Count > 0)
            {
                foreach (var address in appBaseAddresses)
                {
                    var uri = Opc.Ua.Utils.ParseUri(address);
                    if (uri != null)
                    {
                        switch (uri.Scheme)
                        {
                            case Opc.Ua.Utils.UriSchemeNetPipe:
                                if (!Contains(Opc.Ua.Utils.UriSchemeNetPipe))
                                    Add(Opc.Ua.Utils.UriSchemeNetPipe);
                                break;
                            case Opc.Ua.Utils.UriSchemeNetTcp:
                                if (!Contains(Opc.Ua.Utils.UriSchemeNetTcp))
                                    Add(Opc.Ua.Utils.UriSchemeNetTcp);
                                break;
                            case Opc.Ua.Utils.UriSchemeOpcTcp:
                                if (!Contains(Opc.Ua.Utils.UriSchemeOpcTcp))
                                    Add(Opc.Ua.Utils.UriSchemeOpcTcp);
                                break;
                            case Opc.Ua.Utils.UriSchemeHttp:
                                if (!Contains(Opc.Ua.Utils.UriSchemeHttp))
                                    Add(Opc.Ua.Utils.UriSchemeHttp);
                                break;
                            case Opc.Ua.Utils.UriSchemeHttps:
                                if (!Contains(Opc.Ua.Utils.UriSchemeHttps))
                                    Add(Opc.Ua.Utils.UriSchemeHttps);
                                break;
                            case Opc.Ua.Utils.UriSchemeNoSecurityHttp:
                                if (!Contains(Opc.Ua.Utils.UriSchemeNoSecurityHttp))
                                    Add(Opc.Ua.Utils.UriSchemeNoSecurityHttp);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
