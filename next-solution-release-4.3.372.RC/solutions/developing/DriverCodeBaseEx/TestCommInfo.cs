using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriverCodeBaseEx
{
    public class TestCommInfo
    {
        public bool Connected {  get; set; }
        public string Info { get; set; }

        public TestCommInfo()
        {
            Connected = false;
            Info = string.Empty;
        }

        public TestCommInfo(bool connected, string info)
        {
            Connected = connected;
            Info = info;
        }
    }
}
