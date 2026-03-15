using System;
using System.Linq;
using DocumentManager.ComponentService;
using System.Windows.Media.Imaging;
using TempVariablesManager;
using ClientEditor.Document;
using System.Windows.Input;
using System.ComponentModel;
using WPFUtilities;
using ClientEditor.ComponentService;

namespace ClientEditor.TreeDocumentManagers
{
    class TreeAddressSpaceDocManager : TreeChangedDocument
    {
        #region Declarations
        TempVariablesModel.Variable Tag;
        TempVariablesModel.Folder Folder;
        readonly IDocumentManager Parent;
        readonly String Title;

        bool bRefreshing;

        static readonly int maxItems = Properties.Settings.Default.MaxItemsInTree;
        #endregion

        #region Constructors
        internal TreeAddressSpaceDocManager(ClientEditorManagerComponent c, ClientDocument doc) 
            : base(c, doc)
        { }

        internal TreeAddressSpaceDocManager(ClientEditorManagerComponent c, ClientDocument doc,
            String title, IDocumentManager parent) 
            : base(c, doc)
        {
            Title = title;
            Parent = parent;
        }

        internal TreeAddressSpaceDocManager(ClientEditorManagerComponent c, ClientDocument doc,
            TempVariablesModel.Variable tag, IDocumentManager parent)
            : base(c, doc)
        {
            Tag = tag;
            Parent = parent;

            (Tag as INotifyPropertyChanged).PropertyChanged += Tag_PropertyChanged;
        }

        internal TreeAddressSpaceDocManager(ClientEditorManagerComponent c, ClientDocument doc,
            TempVariablesModel.Folder folder, IDocumentManager parent)
            : base(c, doc)
        {
            Folder = folder;
            Parent = parent;
        }
        #endregion

        #region Methods

        internal TempVariablesModel.Variable GetTag() { return Tag; }
        internal TempVariablesModel.Folder GetFolder() { return Folder; }

        void Tag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PrototypeModel" && !bRefreshing)
            {
                ClearChilds();
            }
        }

        #endregion

        #region Overrides
        protected override void FillChilds(bool bRefresh, bool bReload)
        {
            FillChilds(bRefresh, bReload, maxItems);
        }

        protected void FillChilds(bool bRefresh, bool bReload, int maxItems)
        {
            var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables") as TempVariables;
            if (bReload)
            {
                if (Tag != null)
                {
                }
                else if (Folder != null)
                {
                    Folder = dsInterface.FindFolderByNodeId(Document, Folder.NodeId);
                }
            }

            bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (list == null || bShiftDown || bRefresh)
            {
                try
                {
                    bRefreshing = true;
                    if (list == null)
                        list = new ObservableUpdateCollection<IDocumentManager>(bCheckDisposableElement: true);
                    else
                        list.Clear();

                    if (Tag != null)
                    {
                    }
                    else
                    {
                        int i = 0;
                        var tagList = dsInterface.GetTagCollection(Document, Folder);
                        if (tagList != null)
                        {
                            foreach (var name in tagList)
                            {
                                list.Add(new TreeDocumentManagers.TreeAddressSpaceDocManager(editorManagerComponent,
                                                            Document, name, this));
                                if (!bShiftDown && ++i >= maxItems)
                                {
                                    list.Add(new TreeDocumentManagers.TreeAddressSpaceDocManager(editorManagerComponent,
                                                                Document, Properties.Resources.MaxItemCountVisibleReached, this));
                                    break;
                                }
                            }
                        }

                        i = 0;
                        var folderCollection = dsInterface.GetFolderCollection(Document, Folder);
                        if (folderCollection != null)
                        {
                            foreach (var name in folderCollection)
                            {
                                list.Add(new TreeDocumentManagers.TreeAddressSpaceDocManager(editorManagerComponent,
                                                            Document, name, this));
                                if (!bShiftDown && ++i >= maxItems)
                                {
                                    list.Add(new TreeDocumentManagers.TreeAddressSpaceDocManager(editorManagerComponent,
                                                                Document, Properties.Resources.MaxItemCountVisibleReached, this));
                                    break;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    bRefreshing = false;
                }
            }
        }

        internal override bool OnChangedDocument(Object sender, ChangedType type, object changedObject)
        {
            var tag = changedObject as TempVariablesModel.Variable;
            var folder = changedObject as TempVariablesModel.Folder;

            switch (type)
            {
                case ChangedType.changed:
                    if (tag != null)
                    {
                        if (tag == Tag)
                        {
                            OnPropertyChanged();
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAddressSpaceDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    else if (folder != null)
                    {
                        if (folder == Folder)
                        {
                            OnPropertyChanged();
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAddressSpaceDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    break;

                case ChangedType.added:
                    if (tag != null)
                    {
                        if (tag.Folder == null || tag.Folder == Folder)
                        {
                            if (list != null)
                                list.Add(new TreeDocumentManagers.TreeAddressSpaceDocManager(editorManagerComponent,
                                                            Document, tag, this));
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAddressSpaceDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    else if (folder != null)
                    {
                        if (folder.FolderAss == null || folder.FolderAss == Folder)
                        {
                            if (list != null)
                                list.Add(new TreeDocumentManagers.TreeAddressSpaceDocManager(editorManagerComponent,
                                                            Document, folder, this));
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAddressSpaceDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    break;

                case ChangedType.removed:
                    if (tag != null)
                    {
                        if (tag.Folder == null || tag.Folder == Folder)
                        {
                            if (list != null)
                            {
                                var found = (from c in list.OfType<TreeAddressSpaceDocManager>() where c.GetTag() == tag select c).ToList();
                                found.ForEach(item => list.Remove(item));
                            }
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAddressSpaceDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    else if (folder != null)
                    {
                        if (folder.FolderAss == null || folder.FolderAss == Folder)
                        {
                            if (list != null)
                            {
                                var found = (from c in list.OfType<TreeAddressSpaceDocManager>() where c.GetFolder() == folder select c).ToList();
                                found.ForEach(item => list.Remove(item));
                            }
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAddressSpaceDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    break;
            }

            return false;
        }

        protected override bool IsRootTreeDocManager
        {
            get
            {
                return Tag == null && Folder == null;
            }
        }

        public override void Edit(Uri uri, IDocument parent)
        {
            if (editorManagerComponent.PropertyControl != null && BrowsableContent != null)
            {
                editorManagerComponent.PropertyControl.Activate();
            }
            else if (!String.IsNullOrEmpty(Title) && Parent is TreeAddressSpaceDocManager)
            {
                (Parent as TreeAddressSpaceDocManager).FillChilds(true, false, int.MaxValue);
            }
            else
            {
                editorManagerComponent.Edit(uri, parent);
                editorManagerComponent.GetViewFromUri(uri).tabTempVariables.IsSelected = true;
            }
        }

        public override string TypeLabel
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
                    return Properties.Resources.TagList; 
            }
        }

        public override BitmapImage TypeIcon
        {
            get 
            {
                if (Tag != null)
                {
                    var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables") as TempVariables;
                    return dsInterface.GetTagBitmapImage(Tag.DataType);
                }
                else if (Folder != null)
                {
                    return ClientEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);
                }
                else
                    return ClientEditorManagerComponent.GetBitmapImage("CEAddressSpace"); 
            }

        }

        public override BitmapImage TypeIconOpen
        {
            get
            {
                if (Folder != null)
                {
                    return ClientEditorManagerComponent.GetBitmapImage("OpenFolderSmall", true);
                }
                else
                    return TypeIcon;
            }

        }

        public override BitmapImage TypeIconLarge
        {
            get 
            {
                if (Tag != null)
                {
                    var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables") as TempVariables;
                    return dsInterface.GetTagBitmapImage(Tag.DataType);
                }
                else if (Folder != null)
                {
                    return ClientEditorManagerComponent.GetBitmapImage("CloseFolder", true);
                }
                else
                    return ClientEditorManagerComponent.GetBitmapImage("CEAddressSpace"); 
            }
        }

        public override string TypeScheme
        {
            get 
            {
                return Properties.Settings.Default.AddressSpaceTypeScheme;
            }
        }

        public override bool IsResourceExpandable(IDocument document = null)
        {
            if (!String.IsNullOrEmpty(Title))
                return false;

            if (IsRootTreeDocManager)
                CreateDocumentIfDisposed();
            if (Document == null || Document.IsDisposed)
                return IsRootTreeDocManager;

            if (Tag != null)
            {
                return false;
            }
            else
            {
                if (Properties.Settings.Default.FolderAlwaysExpandible)
                    return true;
                var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables") as TempVariables;

                var list = dsInterface.GetFolderCollection(Document, Folder);
                if (list != null && list.Count > 0)
                    return true;
                var list1 = dsInterface.GetTagCollection(Document, Folder);

                return list1 != null && list1.Count > 0;
            }
        }

        public override bool CanBeDragged
        {
            get
            {
                return Document != null && !Document.IsDisposed && Tag != null;
            }
        }

        public override Object DragContent
        {
            get
            {
                if (Document == null || Document.IsDisposed)
                    return null;

                if (Tag != null)
                {
                    var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables") as TempVariables;
                    return dsInterface.GetReference(Tag);
                }
                else
                    return null;
            }
        }

        public override Object BrowsableContent
        {
            get
            {
                if (Document == null || Document.IsDisposed)
                    return null;

                if (Tag != null)
                    return Tag;
                else if (Folder != null)
                    return Folder;

                return null;
            }
        }

        protected override void OnDispose()
        {
            if (Tag != null)
                (Tag as INotifyPropertyChanged).PropertyChanged -= Tag_PropertyChanged;

            base.OnDispose();
        }
        #endregion
    }
}
