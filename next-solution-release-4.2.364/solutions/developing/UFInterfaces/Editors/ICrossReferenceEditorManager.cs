using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using DocumentManager.ComponentService;
using UFInterfaces;

namespace UFCrossReferenceEditor.ComponentService
{
    public interface ICrossReferenceEditorManager : IUFInterfaceBase
    {
        UserControl GetRuntimeControl(IDocument parent);
    }
}
