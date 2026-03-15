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
    public class MinMaxRangeChangedEventArgs : EventArgs
    {
        #region Constructors
        public MinMaxRangeChangedEventArgs(double minValue, double maxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }
        #endregion

        #region Properties
        public double MinValue { get; private set; }

        public double MaxValue { get; private set; }
        #endregion
    }
}
