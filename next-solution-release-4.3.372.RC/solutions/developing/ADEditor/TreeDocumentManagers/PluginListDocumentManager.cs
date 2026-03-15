using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentManager.ComponentService;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Utilities;
using ADEditor.Document;
using ADEditor.ComponentService;
using System.Windows.Input;

namespace ADEditor.TreeDocumentManagers
{
    class PluginListDocumentManager : IDocumentManager
    {
        #region Declarations
        readonly ADEditorManagerComponent DocumentEditorManagerComponent;
        ADEditorDocument Document;
        ADModel.ADPlugin Tag;
        readonly IDocumentManager Parent;
        readonly String Title;
        #endregion

        public PluginListDocumentManager(ADEditorManagerComponent c, ADEditorDocument doc)
        {
            DocumentEditorManagerComponent = c;
            Document = doc;

            Tag = null;
            Parent = null;
        }

        internal PluginListDocumentManager(ADEditorManagerComponent c, ADEditorDocument doc,
            String title, IDocumentManager parent)
            : this(c, doc)
        {
            Title = title;
            Parent = parent;
        }

        internal PluginListDocumentManager(ADEditorManagerComponent c, ADEditorDocument doc,
            ADModel.ADPlugin tag, IDocumentManager parent)
            : this(c, doc)
        {
            Tag = tag;
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
                return Tag == null;
            }
        }
        public void Edit(Uri uri, IDocument parent)
        {
            DocumentEditorManagerComponent.Edit(uri, parent);
            DocumentEditorManagerComponent.GetViewFromUri(uri).tabPluginList.IsSelected = true;
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

                    if (Tag != null)
                        Tag = Document.FindPluginByNodeId(Tag.NodeId);
                }

                bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

                var list = new ObservableCollection<IDocumentManager>();
                if(Tag == null)
                {
                    int i = 0;
                    foreach (var name in Document.GetPluginCollection())
                    {
                        list.Add(new TreeDocumentManagers.PluginListDocumentManager(DocumentEditorManagerComponent,
                                                    Document, name, this));
                        if (!bShiftDown && ++i >= maxItems)
                        {
                            list.Add(new TreeDocumentManagers.PluginListDocumentManager(DocumentEditorManagerComponent,
                                                        Document, Properties.Resources.MaxItemCountVisibleReached, this));
                            break;
                        }
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
                else if (!String.IsNullOrEmpty(Title))
                    return Title;
                else
                    return Properties.Resources.tabPluginList;
            }
        }

        public BitmapImage TypeIcon
        {
            get 
            {
                return ADEditorManagerComponent.GetBitmapImage("ADPluginsSmall");
            }
        }

        public BitmapImage TypeIconOpen
        {
            get
            {
                return TypeIcon;
            }
        }

        public BitmapImage TypeIconLarge
        {
            get
            {
                return ADEditorManagerComponent.GetBitmapImage("ADPluginsSmall");
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
            if (Document == null || Document.IsDisposed)
                return IsRootTreeDocManager;

            if (Document != null)
            {
                if (Properties.Settings.Default.FolderAlwaysExpandible)
                    return true;

                var list1 = Document.GetPluginCollection();
                return list1.Count > 0 && Tag == null;
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
