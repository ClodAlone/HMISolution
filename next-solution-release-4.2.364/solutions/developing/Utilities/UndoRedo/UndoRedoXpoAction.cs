using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Xpo.UndoRedo
{
    public enum UndoRedoAction
    {
        None,
        Added,
        Changed,
        Replaced,
        Removed
    }
}
