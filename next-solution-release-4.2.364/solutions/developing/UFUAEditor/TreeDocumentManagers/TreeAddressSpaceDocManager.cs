using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentManager.ComponentService;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using UFUAEditor.ComponentService;
using System.Windows.Controls;
using UFUAEditor.Document;
using Utilities;
using System.Windows.Input;
using System.ComponentModel;
using WPFUtilities;

namespace UFUAEditor.TreeDocumentManagers
{
    class TreeAddressSpaceDocManager : TreeChangedDocument
    {
        #region Declarations
        UFUAModel.UFUATag Tag;
        UFUAModel.UFUAFolder Folder;
        readonly IDocumentManager Parent;
        readonly String Title;

        bool bRefreshing;

        static readonly int maxItems = Properties.Settings.Default.MaxItemsInTree;
        #endregion

        #region Constructors
        internal TreeAddressSpaceDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc) 
            : base(c, doc)
        { }

        internal TreeAddressSpaceDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc,
            String title, IDocumentManager parent) 
            : base(c, doc)
        {
            Title = title;
            Parent = parent;
        }

        internal TreeAddressSpaceDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc,
            UFUAModel.UFUATag tag, IDocumentManager parent) 
            : base(c, doc)
        {
            Tag = tag;
            Parent = parent;

            (Tag as INotifyPropertyChanged).PropertyChanged += Tag_PropertyChanged;
        }

        internal TreeAddressSpaceDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc,
            UFUAModel.UFUAFolder folder, IDocumentManager parent) 
            : base(c, doc)
        {
            Folder = folder;
            Parent = parent;
        }
        #endregion

        #region Methods

        internal UFUAModel.UFUATag GetTag() { return Tag; }
        internal UFUAModel.UFUAFolder GetFolder() { return Folder; }

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
            if (bReload)
            {
                if (Tag != null)
                {
                    var prototypeReference = Tag.PrototypeReference;
                    if (prototypeReference != null && prototypeReference.UFUATagOwner != null)
                        Tag = Document.FindSubPrototypeMemberByNodeId(Tag.NodeId, prototypeReference.NodeId, prototypeReference.UFUATagOwner.NodeId);
                    else if (prototypeReference != null)
                        Tag = Document.FindTagByNodeId(Tag.NodeId, prototypeReference.NodeId);
                    else
                        Tag = Document.FindTagByNodeId(Tag.NodeId);
                }
                else if (Folder != null)
                {
                    var prototypeReference = Folder.PrototypeReference;
                    if (prototypeReference != null && prototypeReference.UFUATagOwner != null)
                        Folder = Document.FindSubPrototypeFolderByNodeId(Folder.NodeId, prototypeReference.NodeId, prototypeReference.UFUATagOwner.NodeId);
                    else if (prototypeReference != null)
                        Folder = Document.FindFolderByNodeId(Folder.NodeId, prototypeReference.NodeId);
                    else
                        Folder = Document.FindFolderByNodeId(Folder.NodeId);
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
                        if (Tag.ModelType == UFUAModel.ModelType.ObjectType && !String.IsNullOrEmpty(Tag.PrototypeModel))
                        {
                            var prototype = Document.CreateSubPrototype(Tag);
                            if (prototype != null)
                            {
                                prototype.EnsureUniqueMembersOrderId();

                                int i = 0;
                                var members = (from c in prototype.Members orderby c.Name ascending select c);
                                foreach (var name in members)
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

                                i = 0;
                                var folders = (from c in prototype.Folders orderby c.Name ascending select c);
                                foreach (var name in folders)
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
                    else
                    {
                        int i = 0;
                        var tagList = Document.GetTagCollection(Folder, sortByName: true);
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
                        var folderCollection = Document.GetFolderCollection(Folder, sortByName: true);
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
            var tag = changedObject as UFUAModel.UFUATag;
            var folder = changedObject as UFUAModel.UFUAFolder;

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
                    if (tag != null && tag.UFUATagPrototype == null)
                    {
                        if (tag.UFUAFolder == null || tag.UFUAFolder == Folder)
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
                    else if (folder != null && folder.UFUATagPrototype == null)
                    {
                        if (folder.UFUAFolderAss == null || folder.UFUAFolderAss == Folder)
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
                    if (tag != null && tag.UFUATagPrototype == null)
                    {
                        if (tag.UFUAFolder == null || tag.UFUAFolder == Folder)
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
                    else if (folder != null && folder.UFUATagPrototype == null)
                    {
                        if (folder.UFUAFolderAss == null || folder.UFUAFolderAss == Folder)
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
                editorManagerComponent.GetViewFromUri(uri).tabAddressSpace.IsSelected = true;
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
                    return Properties.Resources.AddressSpace; 
            }
        }

        public override BitmapImage TypeIcon
        {
            get 
            {
                if (Tag != null)
                {
                    return UFUAEditorManagerComponent.GetTagBitmapImage(Tag);
                }
                else if (Folder != null)
                {
                    return UFUAEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);
                }
                else
                    return UFUAEditorManagerComponent.GetBitmapImage("UFUASAddressSpace"); 
            }

        }

        public override BitmapImage TypeIconLarge
        {
            get 
            {
                if (Tag != null)
                {
                    var bmpImage = UFUAEditorManagerComponent.GetBitmapImage("UFUASVariable");
                    switch (Tag.ModelType)
                    {
                        case UFUAModel.ModelType.Method:
                            bmpImage = UFUAEditorManagerComponent.GetBitmapImage("UFUASMethod");
                            break;
                        case UFUAModel.ModelType.ObjectType:
                            bmpImage = UFUAEditorManagerComponent.GetBitmapImage("UFUASVariableType");
                            break;
                    }

                    return bmpImage;
                }
                else if (Folder != null)
                {
                    return UFUAEditorManagerComponent.GetBitmapImage("CloseFolder", true);
                }
                else
                    return UFUAEditorManagerComponent.GetBitmapImage("UFUASAddressSpace"); 
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
                if (Tag.ModelType == UFUAModel.ModelType.ObjectType && !String.IsNullOrEmpty(Tag.PrototypeModel))
                {
                    var prototypeName = Tag.PrototypeName;
                    var prototypeFound = (from prototype in Document.GetPrototypes()/*.AsParallel()*/
                                            where prototype.Name == prototypeName
                                            select prototype).ToList();

                    if (prototypeFound.Count > 0)
                        return prototypeFound[0].Folders != null && prototypeFound[0].Folders.Count > 0 ||
                            prototypeFound[0].Members != null && prototypeFound[0].Members.Count > 0;

                    return false;
                }
            }
            else
            {
                if (Properties.Settings.Default.FolderAlwaysExpandible)
                    return true;

                var list = Document.GetFolderCollection(Folder);
                if (list != null && list.Count > 0)
                    return true;
                var list1 = Document.GetTagCollection(Folder);

                return list1 != null && list1.Count > 0;
            }

            return false;
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
                    var list = new List<UFUAModel.UFUATag>();
                    var parent = Parent as TreeAddressSpaceDocManager;
                    while (parent != null)
                    {
                        if (parent.Tag != null && !list.Contains(parent.Tag))
                            list.Add(parent.Tag);
                        parent = parent.Parent as TreeAddressSpaceDocManager;
                    }

                    return Document.GetTagOPCUAEntityReference(Tag, list, refresh: true);
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
