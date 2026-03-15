using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentManager.ComponentService;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Utilities;
using System.Windows.Input;
using UFEventEditor.ComponentService;
using UFEventEditor.Document;

namespace UFEventEditor.TreeDocumentManagers
{
    class EventListDocumentManager : IDocumentManager
    {
        #region Declarations
        readonly EventEditorManagerComponent DocumentEditorManagerComponent;
        EventEditorDocument Document;
        UFEventModel.UFEventObject Tag;
        UFEventModel.UFEventFolder Folder;
        readonly IDocumentManager Parent;
        readonly String Title;
        bool bIsMaxItemsReached = false;
        #endregion

        public EventListDocumentManager(EventEditorManagerComponent c, EventEditorDocument doc)
        {
            DocumentEditorManagerComponent = c;
            Document = doc;

            Tag = null;
            Folder = null;
            Parent = null;
        }

        internal EventListDocumentManager(EventEditorManagerComponent c, EventEditorDocument doc, IDocumentManager parent)
            : this(c, doc)
        {
            bIsMaxItemsReached = true;
            Title = Properties.Resources.MaxItemCountVisibleReached;
            Parent = parent;
        }

        internal EventListDocumentManager(EventEditorManagerComponent c, EventEditorDocument doc,
            UFEventModel.UFEventObject tag, IDocumentManager parent) 
            : this(c, doc)
        {
            Tag = tag;
            Parent = parent;
        }

        internal EventListDocumentManager(EventEditorManagerComponent c, EventEditorDocument doc,
            UFEventModel.UFEventFolder folder, IDocumentManager parent)
            : this(c, doc)
        {
            DocumentEditorManagerComponent = c;
            Document = doc;
            Folder = folder;
            Parent = parent;
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
        bool IsRootTreeDocManager
        {
            get
            {
                return Tag == null && Folder == null;
            }
        }
        public void Edit(Uri uri, IDocument parent)
        {
            if (bIsMaxItemsReached)
                DocumentEditorManagerComponent.GetChilds(uri, parent, false);
            else
                DocumentEditorManagerComponent.Edit(uri, parent);
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

        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, object Context)
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

        static readonly int maxItems = Properties.Settings.Default.MaxItemsInTree;

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                if (Document == null || Document.IsDisposed)
                {
                    Document = DocumentEditorManagerComponent.GetOrCreateDocument(parent, bRefresh: true);
                    if (Document == null)
                        return null;

                    if (Folder != null)
                        Folder = Document.FindFolderByNodeId(Folder.NodeId);
                    else if (Tag != null)
                        Tag = Document.FindEventByNodeId(Tag.NodeId);
                }

                bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

                var list = new ObservableCollection<IDocumentManager>();

                int i = 0;
                foreach (var name in Document.GetFolderCollection(Folder))
                {
                    list.Add(new TreeDocumentManagers.EventListDocumentManager(DocumentEditorManagerComponent,
                                                Document, name, this));
                    if (!bShiftDown && ++i >= maxItems)
                    {
                        list.Add(new TreeDocumentManagers.EventListDocumentManager(DocumentEditorManagerComponent, Document, this));
                        break;
                    }
                }
                i = 0;
                foreach (var name in Document.GetEventsCollection(Folder))
                {
                    list.Add(new TreeDocumentManagers.EventListDocumentManager(DocumentEditorManagerComponent,
                                                Document, name, this));
                    if (!bShiftDown && ++i >= maxItems)
                    {
                        list.Add(new TreeDocumentManagers.EventListDocumentManager(DocumentEditorManagerComponent, Document, this));
                        break;
                    }
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
                if (Tag != null)
                    return Tag.Name;
                else if (Folder != null)
                    return Folder.Name;
                else if (!String.IsNullOrEmpty(Title))
                    return Title;
                else
                    return Properties.Resources.EventManager;
            }
        }

        public BitmapImage TypeIcon
        {
            get
            {
                if (Tag != null)
                {
                    return EventEditorManagerComponent.GetBitmapImage("EVMEvent");
                }
                else if (Folder != null)
                {
                    return EventEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);
                }
                else
                    return EventEditorManagerComponent.GetBitmapImage("EVMEditorSmall");
            }
        }

        public BitmapImage TypeIconOpen
        {
            get
            {
                if (Folder != null)
                {
                    return EventEditorManagerComponent.GetBitmapImage("OpenFolderSmall", true);
                }
                else
                    return TypeIcon;
            }
        }

        public BitmapImage TypeIconLarge
        {
            get
            {
                if (Tag != null)
                {
                    return EventEditorManagerComponent.GetBitmapImage("EVMEvent");
                }
                else if (Folder != null)
                {
                    return EventEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);
                }
                else
                    return EventEditorManagerComponent.GetBitmapImage("EVMEditorSmall");
            }
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
                return Properties.Resources.TypeTitle.Replace(" ", ""); 
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
            if (bIsMaxItemsReached)
                return false;

            if (Document == null || Document.IsDisposed)
                return IsRootTreeDocManager;

            if (Tag != null)
            {
                return false;
            }
            else if (Document != null)
            {

                if (Properties.Settings.Default.FolderAlwaysExpandible)
                    return true;

                var list = Document.GetFolderCollection(Folder);
                if (list.Count > 0)
                    return true;
                var list1 = Document.GetEventsCollection(Folder);
                return list1.Count > 0;
            } 
                
            return false;
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
                return null;
            }
        }
    }
}
