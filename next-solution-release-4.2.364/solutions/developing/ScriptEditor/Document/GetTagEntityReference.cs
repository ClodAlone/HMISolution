using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPCUAViewModel;

namespace ScriptManager
{
    public class GetTagEntityReference : EventArgs
    {
        public GetTagEntityReference(String n, String i)
        {
            Name = n;
            Instance = i;
        }

        public String Instance { get; private set; }
        public String Name { get; private set; }
        public OPCUAEntityReference entityReference { get; set; }
    }
}
