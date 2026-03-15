using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentManager.ComponentService;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using ScreenSettings;
using ScreenManager.ComponentService;
using System.Windows;
using Utilities;

namespace ScreenManager.TreeDocumentManagers
{
    class TreeElementDocManager : IDocumentManager
    {
        #region Declarations
        readonly ScreenDocument screenDocument;
        readonly ScreenManagerComponent screenManagerComponent;
        readonly FrameworkElement element;
        #endregion

        public TreeElementDocManager(ScreenManagerComponent c, ScreenDocument doc, FrameworkElement e)
        {
            screenManagerComponent = c;
            screenDocument = doc;
            element = e;
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
            screenManagerComponent.Edit(uri, parent);
            screenManagerComponent.GetActiveView(uri).SelectElement(element.Name);
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
            using (new WaitCursor())
            {
                var list = new ObservableCollection<IDocumentManager>();
                foreach (object sub in LogicalTreeHelper.GetChildren(element))
                {
                    if (!(sub is FrameworkElement))
                        continue;

                    list.Add(new TreeDocumentManagers.TreeElementDocManager(screenManagerComponent,
                                                screenDocument, sub as FrameworkElement));
                }
                return list;
            }
        }

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers(Uri uri, IDocument parent)
        {
            return null;
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
                return !(String.IsNullOrEmpty(element.Name)) ? element.Name : element.DependencyObjectType.Name; ; 
            }
        }

        public BitmapImage TypeIcon
        {
            get { return ScreenManagerComponent.GetBitmapImage("SMElementsSmall"); }
        }

        public BitmapImage TypeIconLarge
        {
            get { return ScreenManagerComponent.GetBitmapImage("SMElements"); }
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
                return Properties.Resources.TreeElements.Replace(" ", ""); 
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
            foreach (object sub in LogicalTreeHelper.GetChildren(element))
            {
                if (!(sub is FrameworkElement))
                    continue;

                return true;
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
                return null;// element;
            }
        }
    }
}
