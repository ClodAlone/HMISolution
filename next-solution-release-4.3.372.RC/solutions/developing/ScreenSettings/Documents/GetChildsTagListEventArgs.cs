using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace ScreenSettings
{
    public class GetChildsTagListEventArgs : EventArgs
        {
            public IDictionary<String, IList<string>> map { get; set; }
        }
}