using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace ScriptManager
{
    public class GetPrototypeListEventArgs : EventArgs
            {
                public IDictionary<String, IList<String>> mapDefinitions { get; set; }
                public IDictionary<String, String> mapPrototypes { get; set; }
            }
}
