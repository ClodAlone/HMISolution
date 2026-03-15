using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpoHelpers
{
    public class CleaningUnitOfWorkArgs : EventArgs
    {
        public CachedUnitOfWork Task { get; set; }
    }
}
