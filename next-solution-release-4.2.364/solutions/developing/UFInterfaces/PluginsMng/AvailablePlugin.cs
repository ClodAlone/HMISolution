using System;
using System.ComponentModel;

namespace UFInterfaces
{
    namespace Types
    {
        /// <summary>
        /// Data Class for Available Plugin.  Holds and instance of the loaded Plugin, as well as the Plugin's Assembly Path
        /// </summary>
        public class AvailablePlugin
        {
            public IUFInterfaceBase Instance { get; set; }
            public String AssemblyPath  { get; set; }
        }
    }
}
