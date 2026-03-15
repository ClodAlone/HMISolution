using System;
using System.Collections.Generic;
using System.Linq;

namespace ScreenSettings
{
    public class GetTagListEventArgs : EventArgs
        {
            public IEnumerable<String> list { get; set; }
        }
}