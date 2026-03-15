using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFShortcutEditor.UndoRedo
{
    internal enum UndoRedoAction
    {
        LayoutState,
        Added,
        Changed,
        Removed
    }
}
