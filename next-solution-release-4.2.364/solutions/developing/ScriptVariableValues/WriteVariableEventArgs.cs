using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptVariableValues
{
    public class WriteVariableEventArgs : EventArgs
    {
        public WriteVariableEventArgs(String n, Object v)
        {
            Name = n;
            Value = v;
        }

        public String Name { get; private set; }
        public Object Value { get; private set; }
    }
}
