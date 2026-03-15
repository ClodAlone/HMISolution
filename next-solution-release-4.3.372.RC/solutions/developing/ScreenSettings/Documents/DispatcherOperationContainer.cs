using Opc.Ua;
using System;
using System.Windows.Threading;

namespace ScreenSettings.Documents
{
    internal class DispatcherOperationContainer
    {
        public DispatcherOperation dispacherOperation;
        public DispatcherOperation DispacherOperation
        {
            get
            {
                return dispacherOperation;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("new value cannot be null.");

                dispacherOperation = value;
            }
        }

        DataValue value;
        public DataValue Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }
    }
}
