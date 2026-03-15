using System;
using System.Collections.Generic;
using DocumentManager.ComponentService;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using ClientEditor.Document;
using Utilities;
using ClientEditor.ComponentService;

namespace ClientEditor.TreeDocumentManagers
{
    class TreeGeneralSettingsDocManager : IDocumentManager
    {
        #region Declarations
        readonly ClientEditorManagerComponent editorManagerComponent;
        ClientDocument Document;
        #endregion

        #region Constructors
        internal TreeGeneralSettingsDocManager(ClientEditorManagerComponent c, ClientDocument doc)
        {
            editorManagerComponent = c;
            Document = doc;
        }
        #endregion

        #region IDocumentManager
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
            editorManagerComponent.Edit(uri, parent);
            editorManagerComponent.GetViewFromUri(uri).tabGeneralSettings.IsSelected = true;
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

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                if (Document == null || Document.IsDisposed)
                {
                    Document = editorManagerComponent.GetOrCreateDocument(parent, bRefresh: true);
                    if (Document == null)
                        return null;
                }

                var list = new ObservableCollection<IDocumentManager>();
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
                return Properties.Resources.GeneralSettings; 
            }
        }

        public BitmapImage TypeIcon
        {
            get 
            {
                return ClientEditorManagerComponent.GetBitmapImage("CEGeneralSettingsSmall"); 
            }
        }

        public BitmapImage TypeIconLarge
        {
            get 
            {
                return ClientEditorManagerComponent.GetBitmapImage("CEGeneralSettings"); 
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
                return Properties.Settings.Default.GeneralSettingsTypeScheme;
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
        #endregion
    }
}
