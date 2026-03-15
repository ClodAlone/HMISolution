using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OPCUAViewModel.MonitoredItemViewModel;

namespace WPFPenHelpers
{
    public class ValueChangedEventArgs : EventArgs
    {
        #region Constructors
        public ValueChangedEventArgs(object newValue)
        {
            this.NewValue = newValue;
        }
        #endregion

        #region Properties
        public object NewValue { get; private set; }
        #endregion
    }
}
