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
using System.ComponentModel;
using WPFUtilities;

namespace UFUAEditor.TreeDocumentManagers
{
    class TreeHistoricalPrototypesDocManager : TreeChangedDocument
    {
        #region Declarations
        UFUAModel.UFUAHistorianSettings Historical;
        readonly IDocumentManager Parent;
        #endregion

        #region Constructors
        internal TreeHistoricalPrototypesDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc) 
            : base(c, doc)
        { }

        TreeHistoricalPrototypesDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc,
            UFUAModel.UFUAHistorianSettings historical, IDocumentManager parent) 
            : base(c, doc)
        {
            Historical = historical;
            Parent = parent;
        }
        #endregion

        #region Overrides
        protected override void FillChilds(bool bRefresh, bool bReload)
        {
            if (bReload && Historical != null)
                Historical = Document.GetHistoricalSettings(Historical.Name);


            if (list == null || bRefresh)
            {
                if (list == null)
                    list = new ObservableUpdateCollection<IDocumentManager>(bCheckDisposableElement: true);
                else
                    list.Clear();

                if (Historical == null)
                {
                    foreach (var name in Document.GetHistoricalSettings())
                    {
                        list.Add(new TreeDocumentManagers.TreeHistoricalPrototypesDocManager(editorManagerComponent,
                                                    Document, name, this));
                    }
                }
            }
        }

        protected override bool IsRootTreeDocManager
        {
            get
            {
                return Historical == null;
            }
        }

        internal override bool OnChangedDocument(Object sender, ChangedType type, object changedObject)
        {
            var historical = changedObject as UFUAModel.UFUAHistorianSettings;

            switch (type)
            {
                case ChangedType.changed:
                    if (historical != null)
                    {
                        if (historical == Historical)
                        {
                            OnPropertyChanged();
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeHistoricalPrototypesDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    break;

                case ChangedType.added:
                    if (historical != null)
                    {
                        if (list != null)
                            list.Add(new TreeDocumentManagers.TreeHistoricalPrototypesDocManager(editorManagerComponent,
                                                        Document, historical, this));
                        return true;
                    }
                    break;

                case ChangedType.removed:
                    if (historical != null)
                    {
                        if (list != null)
                        {
                            var found = (from c in list.OfType<TreeHistoricalPrototypesDocManager>() where c.Historical == historical select c).ToList();
                            found.ForEach(item => list.Remove(item));
                        }
                        return true;
                    }
                    break;
            }

            return false;
        }

        public override void Edit(Uri uri, IDocument parent)
        {
            editorManagerComponent.Edit(uri, parent);
            editorManagerComponent.GetViewFromUri(uri).tabHistoricalPrototypes.IsSelected = true;
        }

        public override string TypeLabel
        {
            get 
            {
                if (Historical != null)
                    return Historical.Name;
                else
                    return Properties.Resources.HistoricalRibbonTitle; 
            }
        }

        public override BitmapImage TypeIcon
        {
            get 
            { 
                return UFUAEditorManagerComponent.GetBitmapImage("UFUASHistoricalPrototypesSmall"); 
            }
        }

        public override BitmapImage TypeIconLarge
        {
            get 
            {
                return UFUAEditorManagerComponent.GetBitmapImage("UFUASHistoricalPrototypes"); 
            }
        }

        public override string TypeScheme
        {
            get
            {
                return Properties.Settings.Default.HistoricalTypeScheme;
            }
        }

        public override bool IsResourceExpandable(IDocument document = null)
        {
            if (IsRootTreeDocManager)
                CreateDocumentIfDisposed();
            if (Document == null || Document.IsDisposed)
                return IsRootTreeDocManager;

            if (Historical != null)
                return false;
            else
            {
                if (Properties.Settings.Default.FolderAlwaysExpandible)
                    return true;

                var list = Document.GetHistoricalSettings();
                return list.Count > 0;
            }
        }

        public override Object BrowsableContent
        {
            get
            {
                if (Document == null || Document.IsDisposed)
                    return null;

                if (Historical != null)
                    return Historical;

                return null;
            }
        }
        #endregion
    }
}
