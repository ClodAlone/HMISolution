using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Controls;
#endif
using DocumentManager.ComponentService;
using UFInterfaces;

namespace StringManager.ComponentService
{
    public interface IStringEditorManager : IUFInterfaceBase
    {
        IEnumerable<String> GetListAvailableCultures(IDocument parent);
        IDictionary<String, String> GetListStringForCulture(IDocument parent, String culture);
        IEnumerable<String> GetListStringIDs(IDocument parent, bool inExecution = false);
#if !NET_STANDARD
        bool AddListStringId(IDocument parent, IList<String> list);
        bool RemoveListStringId(IDocument parent, IList<String> list);
#endif
        String GetActiveCulture(IDocument parent, bool bAlwaysReturnCulture = true);
        void SetActiveCulture(IDocument parent, String Culture, bool bDesign = false);
        void ClearActiveCulture(IDocument parent);
        event EventHandler CultureChanged;
        event EventHandler LocalesChanged;

        String GetConnectionStringFromFile(String file);
#if !WINDOWS_UWP && !NET_STANDARD
        UserControl GetStringEditor(IDocument parent, bool bAllowMultiSelection = false);
#endif
    }
}
