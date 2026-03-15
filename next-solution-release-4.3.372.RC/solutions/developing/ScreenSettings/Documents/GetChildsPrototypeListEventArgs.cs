using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace ScreenSettings
{
    public class GetChildsPrototypeListEventArgs : EventArgs
            {
                public IDictionary<String, IDictionary<String, IList<String>>> mapChildsDefinitions { get; set; }
                public IDictionary<String, IDictionary<String, String>> mapChildsPrototypes { get; set; }
            }
}
