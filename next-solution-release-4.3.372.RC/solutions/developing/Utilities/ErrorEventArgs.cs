using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class ErrorEventArgs : EventArgs
    {
        public string ErrorMessage { get; set; }
    }
}
