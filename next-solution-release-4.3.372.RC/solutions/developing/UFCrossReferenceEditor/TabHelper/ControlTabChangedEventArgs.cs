using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFCrossReferenceEditor.TabHelper
{
    public class ControlTabChangedEventArgs : EventArgs
    {
        public ControlTabEnum OldTabId { get; set; }
        public ControlTabEnum NewTabId { get; set; }
    }
}
