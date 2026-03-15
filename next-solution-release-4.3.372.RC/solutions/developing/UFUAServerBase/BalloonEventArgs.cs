using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAServerBase
{
    public class BalloonEventArgs : EventArgs
    {
        public String Message = String.Empty;
        public System.Windows.Forms.ToolTipIcon Icon =  System.Windows.Forms.ToolTipIcon.None;
    }
}
