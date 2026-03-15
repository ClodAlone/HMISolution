using System;
using System.Collections.Generic;
using System.Linq;
#if WINDOWS_UWP
using Windows.UI.Xaml.Controls;
#else
#if !NET_STANDARD
using System.Windows.Controls;
using VFS;
#endif
#endif

namespace DocumentManager.ComponentService
{
    public enum SpecialFolders
    {
        Images,
        Documents,
        RuntimeData
    };

    public interface IDocument
    {
#if !NET_STANDARD
        UserControl ActiveView { get; }
        UserControl View { get; }
#endif
        IDocument Parent { get; }

        IList<IDocument> Childs { get; }

#if !WINDOWS_UWP && !NET_STANDARD
        FileSystemProviderBase fileSystemProviderBase { get; }
#endif
        String rootBase { get; }
        String rootBaseDB { get; }
        String Title { get; }
        String FilePath { get; }
        String Theme { get; }
        String ProjectType { get; }

        bool Protected { get; }
        bool IsEmpty { get; }
        bool IsRoot { get; }
        Guid Id { get; }

        Uri MakeAbosoluteUri(Uri relative);
        IDocument UpdateParentFromUri(Uri relative);
        Uri MakeRelativeUri(Uri relative);
        Uri GetSpecialFolder(SpecialFolders specialFolder);

        Object GetService(Type type);

        event EventHandler Disposing;
    }
}
