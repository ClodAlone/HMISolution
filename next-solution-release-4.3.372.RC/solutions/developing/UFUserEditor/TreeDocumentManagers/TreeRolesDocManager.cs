using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentManager.ComponentService;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Utilities;
using UFUserEditor.Document;
using UFUserEditor.ComponentService;

namespace UFUserEditor.TreeDocumentManagers
{
    class TreeRolesDocManager : IDocumentManager
    {
        #region Declarations
        readonly UFUserDocument userDocument;
        readonly UFUserEditorManagerComponent userManagerComponent;
        readonly UFUserModel.UFRole Role;
        #endregion

        public TreeRolesDocManager(UFUserEditorManagerComponent c, UFUserDocument doc, UFUserModel.UFRole role)
        {
            userManagerComponent = c;
            userDocument = doc;
            Role = role;
        }

        public Uri CreateNewDocument(Uri relative, IDocument parent, bool encryptFile = false)
        {
            throw new NotImplementedException();
        }

        public Type DocumentType
        {
            get
            {
                return null;
            }
        }

        public void Edit(Uri uri, IDocument parent)
        {
            userManagerComponent.Edit(uri, parent);
        }

        public void Delete(Uri uri, IDocument parent)
        {
            throw new NotImplementedException();
        }

        public void Copy(Uri uri, string newPath, bool bCopy, IDocument parent, bool bUploading)
        {
            throw new NotImplementedException();
        }

        public void Rename(Uri uri, string oldName, string newName, IDocument parent)
        {
            throw new NotImplementedException();
        }

        public void PreTerminate(Uri uri, IDocument parent)
        { }

        public void Terminate(Uri uri, IDocument parent)
        {
            
        }

        public void SaveAllChild(IDocument parent)
        {
            
        }

        public bool CloseAllChild(IDocument parent, bool bParentClosing = false)
        {
            return true;
        }

        public bool IsAnyChildNeedsSave(IDocument parent)
        {
            return false;
        }

        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, object Context)
        {
            throw new NotImplementedException();
        }

        public bool SaveDocument(IDocument parent, Uri uri, bool encryptFile = false)
        {
            return false;
        }

        public void CleanCoreFiles(String projectPath)
        { }

        public IDocument GetDocument(Uri uri)
        {
            throw new NotImplementedException();
        }

        public IDocument GetChildDocument(Uri uri)
        {
            return null;
        }

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers()
        {
            return null;
        }

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                var document = userDocument;
                if (document == null || document.IsDisposed)
                    document = userManagerComponent.GetOrCreateDocument(parent);
                if (document == null)
                    return null;

                var list = new ObservableCollection<IDocumentManager>();
                foreach (var user in document.GetUserCollection(Role))
                {
                    list.Add(new TreeDocumentManagers.TreeUsersDocManager(userManagerComponent,
                                                document, user));
                }
                return list;
            }
        }

        public UFInterfaces.Service.IServiceControl GetServiceControl(IDocument parent)
        {
            return null;
        }

        public IDictionary<string, string> GetOptionsLicenseRequired(IDocument parent)
        {
            return null;
        }

        public IToolbar GetToolbar()
        {
            return null;
        }

        public IList<System.Windows.Input.ICommand> GetAlwaysAvailableCommand()
        {
            return null;
        }

        public String TypeTitle
        {
            get
            {
                return TypeLabel;
            }
        }

        public string TypeLabel
        {
            get 
            {
                return Role.Name; 
            }
        }

        public BitmapImage TypeIcon
        {
            get { return UFUserEditorManagerComponent.GetBitmapImage("UFUSRGroupSmall"); }
        }

        public BitmapImage TypeIconOpen
        {
            get { return TypeIcon; }
        }

        public BitmapImage TypeIconLarge
        {
            get { return UFUserEditorManagerComponent.GetBitmapImage("UFUSRGroup"); }
        }

        public System.Windows.Controls.Primitives.Popup TypeContextMenu
        {
            get
            {
                return null;
            }
        }

        public string TypeScheme
        {
            get 
            {
                return userManagerComponent.TypeScheme; 
            }
        }

        public string FileType
        {
            get { return String.Empty; }
        }

        public string FileName
        {
            get { return String.Empty; }
        }

        public String[] SaveAsFileExtensions
        {
            get { return null; }
        }

        public bool isMultipleResource
        {
            get { return false; }
        }

        public bool isServiceResource
        {
            get { return false; }
        }

        public bool IsResourceExpandable(IDocument document = null)
        {
            if (userDocument == null || userDocument.IsDisposed)
                return false;

            if (Properties.Settings.Default.FolderAlwaysExpandible)
                return true;

            var list = userDocument.GetUserCollection(Role);
            return list != null && list.Count > 0; 
        }

        public bool IsStartupControllerAware
        {
            get
            {
                return false;
            }
        }

        public bool RegisterFileType
        {
            get { return false; }
        }

        public bool CanBeDragged
        {
            get
            {
                return false;
            }
        }
        public Object DragContent
        {
            get
            {
                return null;
            }
        }

        public Object BrowsableContent
        {
            get
            {
                if (userDocument == null || userDocument.IsDisposed)
                    return null;

                return null; // userDocument.GetNestedObject(Role);
            }
        }
    }
}
