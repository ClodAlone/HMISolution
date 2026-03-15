using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.UndoRedo
{
    public enum UndoRedoAction
    {
        Snapshot,
        Added,
        Changed,
        Removed
    }
}
