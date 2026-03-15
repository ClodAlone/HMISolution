using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScreenManager
{
    public class OpenUriEventArgs : EventArgs
    {
        public Uri uri { get; set; }
    }
}
