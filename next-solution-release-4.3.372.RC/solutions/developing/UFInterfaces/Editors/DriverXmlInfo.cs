using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UFInterfaces.Editors
{
    public class DriverXmlInfo
    {
        public String Factory { get; set; }
        public String FriendlyName { get; set; }
        public String Help { get; set; }
        public String AssemblyName { get; set; }
        public String PackageType { get; set; }
        public bool Win32 { get; set; }
        public bool Win64 { get; set; }
        public bool Linux { get; set; }
    }
}
