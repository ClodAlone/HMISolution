using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace ScreenSettings
{
    public class VariableChangedEventArgs : EventArgs
    {
        public VariableChangedEventArgs(String name, DataValue value)
        {
            Name = name;
            Value = value;
        }

        public DataValue Value { get; private set; }
        public String Name { get; private set; }
    }
}
