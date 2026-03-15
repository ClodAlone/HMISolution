using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentManager.ComponentService;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using UFUAEditor.ComponentService;
using System.Windows.Controls;
using Utilities;
using UFUAEditor.Document;
using System.ComponentModel;
using WPFUtilities;

namespace UFUAEditor.TreeDocumentManagers
{
    class TreePrototypeDocManager : TreeChangedDocument
    {
        #region Declarations
        UFUAModel.UFUATagPrototype PrototypeTag;
        readonly IDocumentManager Parent;
        #endregion

        #region Constructors
        internal TreePrototypeDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc) 
            : base(c, doc)
        { }

        TreePrototypeDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc,
            UFUAModel.UFUATagPrototype prototype, IDocumentManager parent) 
            : base(c, doc)
        {
            PrototypeTag = prototype;
            Parent = parent;
        }
        #endregion

        #region Overrides
        protected override void FillChilds(bool bRefresh, bool bReload)
        {
            if (bReload)
            {
                if (PrototypeTag != null)
                    PrototypeTag = Document.FindPrototypeByNodeId(PrototypeTag.NodeId);
            }

            if (list == null || bRefresh)
            {
                if (list == null)
                    list = new ObservableUpdateCollection<IDocumentManager>(bCheckDisposableElement: true);
                else
                    list.Clear();

                if (PrototypeTag != null)
                {
                    var folders = (from c in PrototypeTag.Folders orderby c.Name ascending select c);
                    foreach (var name in folders)
                    {
                        list.Add(new TreeDocumentManagers.TreeAddressSpaceDocManager(editorManagerComponent,
                                                    Document, name, this));
                    }
                    var members = (from c in PrototypeTag.Members orderby c.Name ascending select c);
                    foreach (var name in members)
                    {
                        list.Add(new TreeDocumentManagers.TreeAddressSpaceDocManager(editorManagerComponent,
                                                    Document, name, this));
                    }
                }
                else
                {
                    var prototypes = (from c in Document.GetPrototypes() orderby c.Name ascending select c);
                    foreach (var name in prototypes)
                    {
                        list.Add(new TreeDocumentManagers.TreePrototypeDocManager(editorManagerComponent,
                                                    Document, name, this));
                    }
                }
            }
        }

        protected override bool IsRootTreeDocManager
        {
            get
            {
                return PrototypeTag == null;
            }
        }

        internal override bool OnChangedDocument(Object sender, ChangedType type, object changedObject)
        {
            var tag = changedObject as UFUAModel.UFUATag;
            var folder = changedObject as UFUAModel.UFUAFolder;
            var prototype = changedObject as UFUAModel.UFUATagPrototype;

            if (tag != null && !tag.IsPrototypeMember || folder != null && !folder.IsPrototypeMember)
                return false;

            switch (type)
            {
                case ChangedType.changed:
                    if (prototype != null && prototype == PrototypeTag)
                    {
                        OnPropertyChanged();
                        return true;
                    }
                    else if (tag != null && tag.PrototypeReference == PrototypeTag)
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
                    else if (folder != null && folder.PrototypeReference == PrototypeTag)
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
                    else
                    {
                        if (list != null)
                        {
                            foreach (var item in list.OfType<TreeDocumentManagers.TreePrototypeDocManager>())
                            {
                                if (item.OnChangedDocument(sender, type, changedObject))
                                    return true;
                            }
                        }
                    }
                    break;

                case ChangedType.added:
                    if (prototype != null)
                    {
                        if (list != null)
                            list.Add(new TreeDocumentManagers.TreePrototypeDocManager(editorManagerComponent,
                                                        Document, prototype, this));
                        return true;
                    }
                    else if (tag != null)
                    {
                        if (tag.UFUATagPrototype != null)
                        {
                            if (tag.UFUATagPrototype == PrototypeTag)
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
                                    foreach (var item in list.OfType<TreeDocumentManagers.TreePrototypeDocManager>())
                                    {
                                        if (item.OnChangedDocument(sender, type, changedObject))
                                            return true;
                                    }
                                }
                            }
                        }
                        else if (tag.PrototypeReference == PrototypeTag)
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
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreePrototypeDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    else if (folder != null)
                    {
                        if (folder.UFUATagPrototype != null)
                        {
                            if (folder.UFUATagPrototype == PrototypeTag)
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
                                    foreach (var item in list.OfType<TreeDocumentManagers.TreePrototypeDocManager>())
                                    {
                                        if (item.OnChangedDocument(sender, type, changedObject))
                                            return true;
                                    }
                                }
                            }
                        }
                        else if (folder.PrototypeReference == PrototypeTag)
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
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreePrototypeDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    break;

                case ChangedType.removed:
                    if (prototype != null)
                    {
                        if (list != null)
                        {
                            var found = (from c in list.OfType<TreePrototypeDocManager>() where c.PrototypeTag == prototype select c).ToList();
                            found.ForEach(item => list.Remove(item));
                        }
                        return true;
                    }
                    else if (tag != null)
                    {
                        if (tag.UFUATagPrototype != null)
                        {
                            if (tag.UFUATagPrototype == PrototypeTag)
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
                                    foreach (var item in list.OfType<TreeDocumentManagers.TreePrototypeDocManager>())
                                    {
                                        if (item.OnChangedDocument(sender, type, changedObject))
                                            return true;
                                    }
                                }
                            }
                        }
                        else if (tag.PrototypeReference == PrototypeTag)
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
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreePrototypeDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    else if (folder != null)
                    {
                        if (folder.UFUATagPrototype != null)
                        {
                            if (folder.UFUATagPrototype == PrototypeTag)
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
                                    foreach (var item in list.OfType<TreeDocumentManagers.TreePrototypeDocManager>())
                                    {
                                        if (item.OnChangedDocument(sender, type, changedObject))
                                            return true;
                                    }
                                }
                            }
                        }
                        else if (folder.PrototypeReference == PrototypeTag)
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
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreePrototypeDocManager>())
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

        public override void Edit(Uri uri, IDocument parent)
        {
            editorManagerComponent.Edit(uri, parent);
            editorManagerComponent.GetViewFromUri(uri).tabPrototypes.IsSelected = true;
        }

        public override string TypeLabel
        {
            get 
            {
                if (PrototypeTag != null)
                    return PrototypeTag.Name;
                else
                    return Properties.Resources.Prototypes; 
            }
        }

        public override BitmapImage TypeIcon
        {
            get 
            {
                return UFUAEditorManagerComponent.GetBitmapImage("UFUASPrototypesSmall"); 
            }
        }

        public override BitmapImage TypeIconLarge
        {
            get 
            {

                return UFUAEditorManagerComponent.GetBitmapImage("UFUASPrototypes"); 
            }
        }

        public override string TypeScheme
        {
            get
            {
                return Properties.Settings.Default.TreePrototypeTypeScheme;
            }
        }

        public override bool IsResourceExpandable(IDocument document = null)
        {
            if (IsRootTreeDocManager)
                CreateDocumentIfDisposed();
            if (Document == null || Document.IsDisposed)
                return IsRootTreeDocManager;

            if (PrototypeTag != null)
                return PrototypeTag.Folders.Count > 0 || PrototypeTag.Members.Count > 0;
            else
            {
                if (Properties.Settings.Default.FolderAlwaysExpandible)
                    return true;

                var list = Document.GetPrototypes();
                return list.Count > 0;
            }
        }

        public override Object BrowsableContent
        {
            get
            {
                if (Document == null || Document.IsDisposed)
                    return null;

                if (PrototypeTag != null)
                    return PrototypeTag;

                return null;
            }
        }
        #endregion
    }
}
