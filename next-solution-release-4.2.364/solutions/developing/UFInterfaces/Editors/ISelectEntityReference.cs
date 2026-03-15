using System.Collections.Generic;
#if WINDOWS_UWP
using Windows.UI.Xaml.Controls;
#else
#if !NET_STANDARD
using System.Windows.Controls;
using VFS;
#endif
#endif

namespace UFInterfaces.Editors
{
    public interface ISelectEntityReference
    {
        object SelectedReference { get; set; }
        List<object> SelectedReferences { get; set; }
        void BringIntoView(object selectedReference);
    }
}
