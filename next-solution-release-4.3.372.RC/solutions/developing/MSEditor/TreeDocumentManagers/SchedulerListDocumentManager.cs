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
using MSEditor.ComponentService;
using MSSchedulerSettings.Document;

namespace MSEditor.TreeDocumentManagers
{
    class SchedulerListDocumentManager : IDocumentManager
    {
        #region Declarations
        readonly SchedulerEditorManagerComponent DocumentEditorManagerComponent;
        SchedulerEditorDocument Document;
        MSModel.MSScheduledAction Tag;
        MSModel.MSFolder Folder;
        readonly IDocumentManager Parent;
        readonly String Title;
        bool bIsMaxItemsReached = false;
        #endregion

        public SchedulerListDocumentManager(SchedulerEditorManagerComponent c, SchedulerEditorDocument doc)
        {
            DocumentEditorManagerComponent = c;
            Document = doc;

            Tag = null;
            Folder = null;
            Parent = null;
        }

        internal SchedulerListDocumentManager(SchedulerEditorManagerComponent c, SchedulerEditorDocument doc, IDocumentManager parent)
            : this(c, doc)
        {
            bIsMaxItemsReached = true;
            Title = Properties.Resources.MaxItemCountVisibleReached;
            Parent = parent;
        }

        internal SchedulerListDocumentManager(SchedulerEditorManagerComponent c, SchedulerEditorDocument doc,
            MSModel.MSScheduledAction tag, IDocumentManager parent) 
            : this(c, doc)
        {
            Tag = tag;
            Parent = parent;
        }

        internal SchedulerListDocumentManager(SchedulerEditorManagerComponent c, SchedulerEditorDocument doc,
            MSModel.MSFolder folder, IDocumentManager parent)
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
            {
                DocumentEditorManagerComponent.Edit(uri, parent);
                DocumentEditorManagerComponent.GetViewFromUri(uri).tabEventList.IsSelected = true;
            }
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
                        Tag = Document.FindActionByNodeId(Tag.NodeId);
                }

                bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

                var list = new ObservableCollection<IDocumentManager>();

                int i = 0;
                foreach (var name in Document.GetFolderCollection(Folder))
                {
                    list.Add(new TreeDocumentManagers.SchedulerListDocumentManager(DocumentEditorManagerComponent,
                                                Document, name, this));
                    if (!bShiftDown && ++i >= maxItems)
                    {
                        list.Add(new TreeDocumentManagers.SchedulerListDocumentManager(DocumentEditorManagerComponent,
                                                    Document, this));
                        break;
                    }
                }
                i = 0;
                foreach (var name in Document.GetEventsCollection(Folder))
                {
                    list.Add(new TreeDocumentManagers.SchedulerListDocumentManager(DocumentEditorManagerComponent,
                                                Document, name, this));
                    if (!bShiftDown && ++i >= maxItems)
                    {
                        list.Add(new TreeDocumentManagers.SchedulerListDocumentManager(DocumentEditorManagerComponent,
                                                    Document, this));
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
                    return Properties.Resources.tabEventList;
            }
        }

        public BitmapImage TypeIcon
        {
            get
            {
                if (Tag != null)
                {
                    return SchedulerEditorManagerComponent.GetBitmapImage("SSAction");
                }
                else if (Folder != null)
                {
                    return SchedulerEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);
                }
                else
                    return SchedulerEditorManagerComponent.GetBitmapImage("SSEditorSmall");
            }
        }

        public BitmapImage TypeIconOpen
        {
            get
            {
                if (Folder != null)
                {
                    return SchedulerEditorManagerComponent.GetBitmapImage("OpenFolderSmall", true);
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
                    return SchedulerEditorManagerComponent.GetBitmapImage("SSAction");
                }
                else if (Folder != null)
                {
                    return SchedulerEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);
                }
                else
                    return SchedulerEditorManagerComponent.GetBitmapImage("SSEditorSmall");
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
