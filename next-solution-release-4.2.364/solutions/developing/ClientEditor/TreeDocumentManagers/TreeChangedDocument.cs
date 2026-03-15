using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using DocumentManager.ComponentService;
using ClientEditor.ComponentService;
using ClientEditor.Document;
using Utilities;
using WPFUtilities;
using System.Linq;

namespace ClientEditor.TreeDocumentManagers
{
    abstract class TreeChangedDocument : IDocumentManager, INotifyPropertyChanged, IDisposable
    {
        #region Declarations
        protected readonly ClientEditorManagerComponent editorManagerComponent;
        protected ClientDocument Document;

        bool bNeedToRefreshChilds;
        bool bNeedToReload;

        protected ObservableUpdateCollection<IDocumentManager> list;

        #endregion

        #region Constructors
        public TreeChangedDocument(ClientEditorManagerComponent c, ClientDocument doc)
        {
            editorManagerComponent = c;
            Document = doc;

            if (Document != null)
                Document.Disposing += Document_Disposing;
        }
        #endregion

        #region Methods
        void Document_Disposing(object sender, EventArgs e)
        {
            Document.Disposing -= Document_Disposing;

            if (list != null)
                list.Clear();
        }

        protected void CreateDocumentIfDisposed()
        {
            if (Document == null || !Document.IsDisposed)
                return;
            
            Document = editorManagerComponent.GetOrCreateDocument(Document.Parent, bRefresh: true);

            if (Document != null)
            {
                bNeedToReload = true;
                Document.Disposing += Document_Disposing;
            }
        }

        protected void ClearChilds()
        {
            if (list != null)
            {
                bNeedToRefreshChilds = true;
                list.Clear();
            }
        }

        List<ObservableUpdateCollection<IDocumentManager>> BeginUpdate()
        {
            var ret = new List<ObservableUpdateCollection<IDocumentManager>>();
            if (list != null)
            {
                list.BeginUpdate();
                ret.Add(list);
                foreach (var item in list.OfType<TreeDocumentManagers.TreeChangedDocument>())
                    ret.AddRange(item.BeginUpdate());
            }
            return ret;
        }

        void EndUpdate(List<ObservableUpdateCollection<IDocumentManager>> list)
        {
            list.ForEach(item => item.EndUpdate());
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged()
        {
            var e = PropertyChanged;
            if (e != null)
                e(this, new PropertyChangedEventArgs("changed"));
        }
        #endregion

        #region IDocumentManager
        public virtual Uri CreateNewDocument(Uri relative, IDocument parent, bool encryptFile = false)
        {
            throw new NotImplementedException();
        }

        public virtual Type DocumentType
        {
            get
            {
                return null;
            }
        }

        public virtual void Edit(Uri uri, IDocument parent)
        {
            editorManagerComponent.Edit(uri, parent);
        }

        public virtual void Delete(Uri uri, IDocument parent)
        {
            throw new NotImplementedException();
        }

        public virtual void Copy(Uri uri, string newPath, bool bCopy, IDocument parent, bool bUploading)
        {
            throw new NotImplementedException();
        }

        public virtual void Rename(Uri uri, string oldName, string newName, IDocument parent)
        {
            throw new NotImplementedException();
        }

        public virtual void Execute(Uri uri, IDocument parent, ExecutionMode mode, object Context)
        {
            throw new NotImplementedException();
        }

        public virtual void SaveAllChild(IDocument parent)
        {

        }

        public virtual bool CloseAllChild(IDocument parent, bool bParentClosing = false)
        {
            return true;
        }

        public virtual bool IsAnyChildNeedsSave(IDocument parent)
        {
            return false;
        }

        public virtual void PreTerminate(Uri uri, IDocument parent)
        { }

        public virtual void Terminate(Uri uri, IDocument parent)
        {

        }

        public virtual bool SaveDocument(IDocument parent, Uri uri, bool encryptFile = false)
        {
            return false;
        }

        public void CleanCoreFiles(String projectPath)
        { }

        public virtual IDocument GetDocument(Uri uri)
        {
            throw new NotImplementedException();
        }

        public virtual IDocument GetChildDocument(Uri uri)
        {
            if (IsRootTreeDocManager)
                return Document;
            return null;
        }

        public virtual ObservableCollection<IDocumentManager> GetChildDocumentManagers()
        {
            bNeedToRefreshChilds = true;
            return GetChilds();
        }

        public virtual ObservableCollection<IDocumentManager> GetChildDocumentManagers(Uri uri, IDocument parent)
        {
            bNeedToRefreshChilds = true;
            return GetChilds(parent);
        }

        ObservableCollection<IDocumentManager> GetChilds(IDocument parent = null)
        {
            using (new WaitCursor())
            {
                bool bRefresh = bNeedToRefreshChilds || bNeedToReload;
                bNeedToRefreshChilds = false;
                bool bReload = bNeedToReload;
                bNeedToReload = false;
                if (Document == null || Document.IsDisposed)
                {
                    bRefresh = bReload = true;
                    if (parent == null)
                        return null;

                    Document = editorManagerComponent.GetOrCreateDocument(parent, bRefresh: true);
                    if (Document == null)
                        return null;
                    Document.Disposing += Document_Disposing;
                }

                FillChilds(bRefresh, bReload);

                return list;
            }
        }

        public virtual UFInterfaces.Service.IServiceControl GetServiceControl(IDocument parent)
        {
            return null;
        }
        public virtual IDictionary<string, string> GetOptionsLicenseRequired(IDocument parent)
        {
            return null;
        }

        public virtual IToolbar GetToolbar()
        {
            return null;
        }

        public IList<System.Windows.Input.ICommand> GetAlwaysAvailableCommand()
        {
            return null;
        }

        public virtual String TypeTitle
        {
            get
            {
                return TypeLabel;
            }
        }

        public virtual string TypeLabel
        {
            get
            {
                return String.Empty;
            }
        }

        public virtual BitmapImage TypeIcon
        {
            get
            {
                return ClientEditorManagerComponent.GetBitmapImage("CEEditorSmall");
            }
        }

        public virtual BitmapImage TypeIconLarge
        {
            get
            {
                return ClientEditorManagerComponent.GetBitmapImage("CEEditor");
            }
        }

        public virtual System.Windows.Controls.Primitives.Popup TypeContextMenu
        {
            get
            {
                return null;
            }
        }

        public virtual string TypeScheme
        {
            get
            {
                return null;
            }
        }

        public virtual string FileType
        {
            get
            {
                return String.Empty;
            }
        }

        public virtual string FileName
        {
            get
            {
                return String.Empty;
            }
        }

        public String[] SaveAsFileExtensions
        {
            get { return null; }
        }

        public virtual bool isMultipleResource
        {
            get
            {
                return false;
            }
        }

        public bool isServiceResource
        {
            get { return false; }
        }

        public virtual bool IsResourceExpandable(IDocument document = null)
        {
            if (IsRootTreeDocManager)
                CreateDocumentIfDisposed();
            if (Document == null || Document.IsDisposed)
                return IsRootTreeDocManager;

            return false;
        }

        public virtual bool IsStartupControllerAware
        {
            get
            {
                return false;
            }
        }

        public virtual bool RegisterFileType
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanBeDragged
        {
            get
            {
                return false;
            }
        }
        public virtual Object DragContent
        {
            get
            {
                return null;
            }
        }

        public virtual Object BrowsableContent
        {
            get
            {
                return null;
            }
        }
        #endregion

        #region Abstract Methods/Properties
        protected abstract void FillChilds(bool bRefresh, bool bReload);

        protected abstract bool IsRootTreeDocManager { get; }

        internal abstract bool OnChangedDocument(Object sender, ChangedType type, object changedObject);
        #endregion

        #region Virtual Methods
        internal void OnChangedDocument(Object sender, ChangedType type, System.Collections.ICollection changedObjects)
        {
            if (changedObjects.Count > 0)
            {
                var ret = BeginUpdate();
                try
                {
                    foreach (var changedObject in changedObjects)
                        OnChangedDocument(sender, type, changedObject);
                }
                finally
                {
                    EndUpdate(ret);
                }
            }
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return TypeLabel;
        }
        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            OnDispose();
        }

        protected virtual void OnDispose()
        {
            if (Document != null)
                Document.Disposing -= Document_Disposing;
        }

        #endregion
    }
}
