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
    class TreeEngineeringUnitPrototypesDocManager : TreeChangedDocument
    {
        #region Declarations
        UFUAModel.UFUAEngineeringUnit EngineeringUnit;
        readonly IDocumentManager Parent;
        #endregion

        #region Constructors
        internal TreeEngineeringUnitPrototypesDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc) 
            : base(c, doc)
        { }

        TreeEngineeringUnitPrototypesDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc,
            UFUAModel.UFUAEngineeringUnit engineeringunit, IDocumentManager parent) 
            : base(c, doc)
        {
            EngineeringUnit = engineeringunit;
            Parent = parent;
        }
        #endregion

        #region Overrides
        protected override void FillChilds(bool bRefresh, bool bReload)
        {
            if (bReload)
            {
                if (EngineeringUnit != null)
                    EngineeringUnit = Document.GetEngineeringUnits(EngineeringUnit.Name);
            }

            if (list == null || bRefresh)
            {
                if (list == null)
                    list = new ObservableUpdateCollection<IDocumentManager>(bCheckDisposableElement: true);
                else
                    list.Clear();

                if (EngineeringUnit == null)
                {
                    foreach (var name in Document.GetEngineeringUnits())
                    {
                        list.Add(new TreeDocumentManagers.TreeEngineeringUnitPrototypesDocManager(editorManagerComponent,
                                                    Document, name, this));
                    }
                }
            }
        }

        protected override bool IsRootTreeDocManager
        {
            get
            {
                return EngineeringUnit == null;
            }
        }

        internal override bool OnChangedDocument(Object sender, ChangedType type, object changedObject)
        {
            var engineeringUnit = changedObject as UFUAModel.UFUAEngineeringUnit;

            switch (type)
            {
                case ChangedType.changed:
                    if (engineeringUnit != null)
                    {
                        if (engineeringUnit == EngineeringUnit)
                        {
                            OnPropertyChanged();
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeEngineeringUnitPrototypesDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    break;

                case ChangedType.added:
                    if (engineeringUnit != null)
                    {
                        if (list != null)
                            list.Add(new TreeDocumentManagers.TreeEngineeringUnitPrototypesDocManager(editorManagerComponent,
                                                        Document, engineeringUnit, this));
                        return true;
                    }
                    break;

                case ChangedType.removed:
                    if (engineeringUnit != null)
                    {
                        if (list != null)
                        {
                            var found = (from c in list.OfType<TreeEngineeringUnitPrototypesDocManager>() where c.EngineeringUnit == engineeringUnit select c).ToList();
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
            editorManagerComponent.GetViewFromUri(uri).tabEngineeringUnitPrototypes.IsSelected = true;
        }

        public override string TypeLabel
        {
            get 
            {
                if (EngineeringUnit != null)
                    return EngineeringUnit.Name;
                else
                    return Properties.Resources.EngineeringUnitPrototypes; 
            }
        }

        public override BitmapImage TypeIcon
        {
            get 
            {
                return UFUAEditorManagerComponent.GetBitmapImage("UFUASEngineeringUnitSmall"); 
            }
        }

        public override BitmapImage TypeIconLarge
        {
            get 
            {
                return UFUAEditorManagerComponent.GetBitmapImage("UFUASEngineeringUnit"); 
            }
        }

        public override string TypeScheme
        {
            get
            {
                return Properties.Settings.Default.EngineeringUnitsTypeScheme;
            }
        }

        public override bool IsResourceExpandable(IDocument document = null)
        {
            if (IsRootTreeDocManager)
                CreateDocumentIfDisposed();
            if (Document == null || Document.IsDisposed)
                return IsRootTreeDocManager;

            if (EngineeringUnit != null)
                return false;
            else
            {
                var list = Document.GetEngineeringUnits();
                return list.Count > 0;
            }
        }

        public override Object BrowsableContent
        {
            get
            {
                if (Document == null || Document.IsDisposed)
                    return null;

                if (EngineeringUnit != null)
                    return EngineeringUnit;

                return null;
            }
        }
        #endregion
    }
}
