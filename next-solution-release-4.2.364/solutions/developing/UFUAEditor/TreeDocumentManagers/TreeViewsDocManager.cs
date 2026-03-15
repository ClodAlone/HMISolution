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
    class TreeViewsDocManager : TreeChangedDocument
    {
        #region Declarations
        UFUAModel.UFUAView View;
        readonly IDocumentManager Parent;
        #endregion

        #region Constructors
        internal TreeViewsDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc) 
            : base(c, doc)
        { }

        TreeViewsDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc,
            UFUAModel.UFUAView view, IDocumentManager parent) 
            : base(c, doc)
        {
            View = view;
            Parent = parent;
        }
        #endregion

        #region Overrides
        protected override void FillChilds(bool bRefresh, bool bReload)
        {
            if (bReload)
            {
                if (View != null)
                    View = Document.GetView(View.Name);
            }

            if (list == null || bRefresh)
            {
                if (list == null)
                    list = new ObservableUpdateCollection<IDocumentManager>(bCheckDisposableElement: true);
                else
                    list.Clear();

                if (View == null)
                {
                    foreach (var name in Document.GetViewsList())
                    {
                        list.Add(new TreeDocumentManagers.TreeViewsDocManager(editorManagerComponent,
                                                    Document, name, this));
                    }
                }
            }
        }

        protected override bool IsRootTreeDocManager
        {
            get
            {
                return View == null;
            }
        }

        internal override bool OnChangedDocument(Object sender, ChangedType type, object changedObject)
        {
            var view = changedObject as UFUAModel.UFUAView;

            switch (type)
            {
                case ChangedType.changed:
                    if (view != null)
                    {
                        if (view == View)
                        {
                            OnPropertyChanged();
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeViewsDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    break;

                case ChangedType.added:
                    if (view != null)
                    {
                        if (list != null)
                            list.Add(new TreeDocumentManagers.TreeViewsDocManager(editorManagerComponent,
                                                        Document, view, this));
                        return true;
                    }
                    break;

                case ChangedType.removed:
                    if (view != null)
                    {
                        if (list != null)
                        {
                            var found = (from c in list.OfType<TreeViewsDocManager>() where c.View == view select c).ToList();
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
            editorManagerComponent.GetViewFromUri(uri).tabViews.IsSelected = true;
        }

        public override string TypeLabel
        {
            get 
            {
                if (View != null)
                    return View.Name;
                else
                    return Properties.Resources.Views;
            }
        }

        public override BitmapImage TypeIcon
        {
            get 
            {
                return UFUAEditorManagerComponent.GetBitmapImage("UFUASViewsSmall"); 
            }
        }

        public override BitmapImage TypeIconLarge
        {
            get 
            {
                return UFUAEditorManagerComponent.GetBitmapImage("UFUASView"); 
            }
        }

        public override string TypeScheme
        {
            get
            {
                return Properties.Settings.Default.ViewsTypeScheme;
            }
        }

        public override bool IsResourceExpandable(IDocument document = null)
        {
            if (IsRootTreeDocManager)
                CreateDocumentIfDisposed();
            if (Document == null || Document.IsDisposed)
                return IsRootTreeDocManager;

            if (View != null)
                return false;
            else
            {
                var list = Document.GetViewsList();
                return list.Count > 0;
            }
        }

        public override Object BrowsableContent
        {
            get
            {
                if (Document == null || Document.IsDisposed)
                    return null;

                if (View != null)
                    return View;

                return null;
            }
        }
        #endregion
    }
}
