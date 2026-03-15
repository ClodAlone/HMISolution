using System;
using System.Collections.Generic;
using System.Linq;

namespace ScreenSettings
{
    public class GetPrototypeListEventArgs : EventArgs
                {
                    public IDictionary<String, IList<String>> mapDefinitions { get; set; }
                    public IDictionary<String, String> mapPrototypes { get; set; }
                }
}
