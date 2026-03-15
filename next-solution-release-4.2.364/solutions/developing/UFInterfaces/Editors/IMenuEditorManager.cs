using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
using DocumentManager.ComponentService;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UFMenuEditor.ComponentService
{
    public interface IMenuEditorManager : IUFInterfaceBase
    {
        MenuBase GetMenu(IDocument parent, Uri uri, String sessionName, bool bLookForDefault, bool bContextMenu, bool bTest);
        void TerminateMenu(MenuBase menu);
    }
}
