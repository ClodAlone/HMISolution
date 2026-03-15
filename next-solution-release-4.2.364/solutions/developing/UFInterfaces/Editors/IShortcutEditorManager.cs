using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
using DocumentManager.ComponentService;
using System.Windows.Input;
#if WINDOWS_UWP
using Windows.UI.Core;
#endif
namespace UFShortcutEditor.ComponentService
{
    public interface IShortcutEditorManager : IUFInterfaceBase
    {
        IList<String> Activate(IDocument parent, Uri uri, String sessionName, bool bLookForDefault);
        void Terminate(IDocument parent);
        bool ExecuteCommand(IDocument parent, String Command);

        bool ExecuteGestureOnDown(IDocument parent, KeyEventArgs e);
        bool ExecuteGestureOnUp(IDocument parent, KeyEventArgs e);
    }
}
