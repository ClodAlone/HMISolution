using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPenHelpers
{
    public class ModelChangedEventArgs : EventArgs
    {
        #region Constructors
        public ModelChangedEventArgs(string humanReadable, NodeIdViewModel newModel)
        {
            this.HumanReadable = humanReadable;
            this.Model = newModel;
        }
        #endregion

        #region Properties
        public string HumanReadable { get; private set; }

        public NodeIdViewModel Model { get; private set; }
        #endregion
    }
}
