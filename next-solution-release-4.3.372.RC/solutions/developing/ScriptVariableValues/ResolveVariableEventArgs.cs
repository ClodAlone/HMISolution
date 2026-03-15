using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptVariableValues
{
    public class ResolveVariableEventArgs : EventArgs
    {
        public ResolveVariableEventArgs(String n)
        {
            Name = n;
        }

        public String Name { get; private set; }
    }
}
