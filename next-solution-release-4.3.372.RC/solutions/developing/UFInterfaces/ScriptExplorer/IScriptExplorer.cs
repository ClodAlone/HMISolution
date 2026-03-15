using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
using UFInterfaces.Scriptable;
using System.Windows.Controls;

namespace ScriptExplorer.ComponentService
{
    public interface IScriptExplorer : IUFInterfaceBase
    {
        IDictionary<IScriptable, Grid> CreateEditControls(IList<IScriptable> list);
        void CheckForChanges(IList<IScriptable> list);
        void DestroyEditControls(IList<IScriptable> list);
    }
}
