using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using VFS;
using UFInterfaces.Service;
#endif
using System.Collections.ObjectModel;

namespace DocumentManager.ComponentService
{
    public interface IDocumentManager
    {
#if !WINDOWS_UWP && !NET_STANDARD
        Uri CreateNewDocument(Uri relative, IDocument parent, bool encryptFile = false);

        void Edit(Uri uri, IDocument parent);
        void Delete(Uri uri, IDocument parent);
        void Copy(Uri uri, String newPath, bool bCopy, IDocument parent, bool bUploading);
        void Rename(Uri uri, String oldName, String newName, IDocument parent);
        void CleanCoreFiles(String projectPath);
#endif
        void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context);
        void PreTerminate(Uri uri, IDocument parent);
        void Terminate(Uri uri, IDocument parent);
#if !WINDOWS_UWP && !NET_STANDARD
        void SaveAllChild(IDocument parent);
        bool CloseAllChild(IDocument parent, bool bParentClosing = false);
        bool IsAnyChildNeedsSave(IDocument parent);
        bool SaveDocument(IDocument parent, Uri uri, bool encryptFile = false);
#endif
        IDocument GetDocument(Uri uri);
        IDocument GetChildDocument(Uri uri);
#if !WINDOWS_UWP && !NET_STANDARD
        bool IsResourceExpandable(IDocument document = null);
        ObservableCollection<IDocumentManager> GetChildDocumentManagers();
        ObservableCollection<IDocumentManager> GetChildDocumentManagers(Uri uri, IDocument parent);

        IServiceControl GetServiceControl(IDocument parent);
        IToolbar GetToolbar();
        IList<System.Windows.Input.ICommand> GetAlwaysAvailableCommand();
        IDictionary<string, string> GetOptionsLicenseRequired(IDocument parent);
#endif

        Type DocumentType { get; }
        String TypeTitle { get; }
        String TypeLabel { get; }
#if !WINDOWS_UWP && !NET_STANDARD
        BitmapImage TypeIcon { get; }
        BitmapImage TypeIconLarge { get; }
        System.Windows.Controls.Primitives.Popup TypeContextMenu { get; }
#endif
        String TypeScheme { get; }
        String FileType { get; }
        String FileName { get; }
        String[] SaveAsFileExtensions { get; }
        bool isMultipleResource { get; }
        bool isServiceResource { get; }
#if !WINDOWS_UWP && !NET_STANDARD
        bool RegisterFileType { get; }
        bool CanBeDragged { get; }
        Object DragContent { get; }

        Object BrowsableContent { get; }
#endif
        bool IsStartupControllerAware { get; }
    }
}
