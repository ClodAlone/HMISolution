using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptVariableValues
{
    public class WriteVariableEventArgs : EventArgs
    {
        public WriteVariableEventArgs(String n, Object v, VariableValues varValues, int timeout = -1, bool sync = false)
        {
            Name = n;
            Value = v;
            Timeout = timeout;
            VarValues = varValues;
            Sync = sync;
        }

        public String Name { get; private set; }
        public Object Value { get; private set; }
        public int Timeout { get; private set; }
        public VariableValues VarValues { get; private set; }
        public bool Sync { get; private set; }
    }
}
