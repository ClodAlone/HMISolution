using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace ScriptManager
{
    public class GetTagListEventArgs : EventArgs
        {
            public IEnumerable<String> list { get; set; }
        }
}