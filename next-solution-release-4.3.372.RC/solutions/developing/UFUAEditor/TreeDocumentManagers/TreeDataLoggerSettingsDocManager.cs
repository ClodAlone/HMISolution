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
    class TreeDataLoggerSettingsDocManager : TreeChangedDocument
    {
        #region Declarations
        DataLoggerModel.DataLoggerSettings DataLogger;
        DataLoggerModel.DataLoggerColumn Column;
        readonly IDocumentManager Parent;
        #endregion

        #region Constructors
        internal TreeDataLoggerSettingsDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc) 
            : base(c, doc)
        { }

        TreeDataLoggerSettingsDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc,
            DataLoggerModel.DataLoggerSettings datalogger, IDocumentManager parent) 
            : base(c, doc)
        {
            DataLogger = datalogger;
            Parent = parent;
        }

        TreeDataLoggerSettingsDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc,
            DataLoggerModel.DataLoggerColumn column, IDocumentManager parent) 
            : base(c, doc)
        {
            Column = column;
            Parent = parent;
        }
        #endregion

        #region Overrides
        protected override void FillChilds(bool bRefresh, bool bReload)
        {
            if (bReload)
            {
                if (DataLogger != null)
                    DataLogger = Document.GetDataLogger(DataLogger.Name);
                else if (Column != null)
                {
                    var datalogger = Document.GetDataLogger(Column.DataLoggerReference.Name);
                    Column = (from c in datalogger.Columns
                              where c.Name == Column.Name
                              select c).FirstOrDefault();
                }
            }

            if (list == null || bRefresh)
            {
                if (list == null)
                    list = new ObservableUpdateCollection<IDocumentManager>(bCheckDisposableElement: true);
                else
                    list.Clear();

                if (DataLogger != null)
                {
                    var columns = (from c in DataLogger.Columns orderby c.ColumnName ascending select c).ToList();
                    foreach (var name in columns)
                    {
                        list.Add(new TreeDocumentManagers.TreeDataLoggerSettingsDocManager(editorManagerComponent,
                                                    Document, name, this));
                    }
                }
                else if (Column == null)
                {
                    foreach (var name in Document.GetDataLoggerSettings())
                    {
                        list.Add(new TreeDocumentManagers.TreeDataLoggerSettingsDocManager(editorManagerComponent,
                                                    Document, name, this));
                    }
                }
            }
        }

        protected override bool IsRootTreeDocManager
        {
            get
            {
                return DataLogger == null && Column == null;
            }
        }

        public override string TypeScheme
        {
            get
            {
                return Properties.Settings.Default.DataLoggerTypeScheme;
            }
        }

        internal override bool OnChangedDocument(Object sender, ChangedType type, object changedObject)
        {
            var datalogger = changedObject as DataLoggerModel.DataLoggerSettings;
            var column = changedObject as DataLoggerModel.DataLoggerColumn;

            switch (type)
            {
                case ChangedType.changed:
                    if (datalogger != null)
                    {
                        if (datalogger == DataLogger)
                        {
                            OnPropertyChanged();
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeDataLoggerSettingsDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    else if (column != null)
                    {
                        if (column == Column)
                        {
                            OnPropertyChanged();
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeDataLoggerSettingsDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    break;

                case ChangedType.added:
                    if (datalogger != null)
                    {
                        if (list != null)
                            list.Add(new TreeDocumentManagers.TreeDataLoggerSettingsDocManager(editorManagerComponent,
                                                        Document, datalogger, this));
                        return true;
                    }
                    else if (column != null)
                    {
                        if (column.DataLoggerReference == null || column.DataLoggerReference == DataLogger)
                        {
                            if (list != null)
                                list.Add(new TreeDocumentManagers.TreeDataLoggerSettingsDocManager(editorManagerComponent,
                                                            Document, column, this));
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeDataLoggerSettingsDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    break;

                case ChangedType.removed:
                    if (datalogger != null)
                    {
                        if (list != null)
                        {
                            var found = (from c in list.OfType<TreeDataLoggerSettingsDocManager>() where c.DataLogger == datalogger select c).ToList();
                            found.ForEach(item => list.Remove(item));
                        }
                        return true;
                    }
                    else if (column != null)
                    {
                        if (column.DataLoggerReference == null || column.DataLoggerReference == DataLogger)
                        {
                            if (list != null)
                            {
                                var found = (from c in list.OfType<TreeDataLoggerSettingsDocManager>() where c.Column == column select c).ToList();
                                found.ForEach(item => list.Remove(item));
                            }
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeDataLoggerSettingsDocManager>())
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
            editorManagerComponent.GetViewFromUri(uri).tabDataLoggerSettings.IsSelected = true;
        }

        public override string TypeLabel
        {
            get 
            {
                if (DataLogger != null)
                    return DataLogger.Name;
                else if (Column != null)
                    return Column.Name;
                else
                    return Properties.Resources.DataLoggerSettings; 
            }
        }

        public override BitmapImage TypeIcon
        {
            get 
            { 
                if (DataLogger != null)
                    return UFUAEditorManagerComponent.GetBitmapImage("UFUASDataLoggerSettingsSmall");
                else if (Column != null)
                    return UFUAEditorManagerComponent.GetBitmapImage("UFUASDataLoggerColumnSmall");
                else
                    return UFUAEditorManagerComponent.GetBitmapImage("UFUASDataLoggerSettingsSmall"); 
            }
        }

        public override BitmapImage TypeIconLarge
        {
            get 
            {
                if (DataLogger != null)
                    return UFUAEditorManagerComponent.GetBitmapImage("UFUASDataLoggerSettings");
                else if (Column != null)
                    return UFUAEditorManagerComponent.GetBitmapImage("UFUASDataLoggerColumn");
                else
                    return UFUAEditorManagerComponent.GetBitmapImage("UFUASDataLoggerSettings"); 
            }
        }
        public override bool IsResourceExpandable(IDocument document = null)
        {
            if (IsRootTreeDocManager)
                CreateDocumentIfDisposed();
            if (Document == null || Document.IsDisposed)
                return IsRootTreeDocManager;

            if (DataLogger != null)
                return DataLogger.Columns.Count > 0;
            else if (Column != null)
                return false;
            else
            {
                if (Properties.Settings.Default.FolderAlwaysExpandible)
                    return true;    
                
                var list = Document.GetDataLoggerSettings();
                return list.Count > 0;
            }
        }

        public override bool CanBeDragged
        {
            get
            {
                return Document != null && !Document.IsDisposed && (DataLogger != null || Column != null);
            }
        }

        public override Object DragContent
        {
            get
            {
                if (Document == null || Document.IsDisposed)
                    return null;

                if (DataLogger != null)
                {
                    return DataLogger;
                }
                else 
                    return Column;
            }
        }

        public override Object BrowsableContent
        {
            get
            {
                if (Document == null || Document.IsDisposed)
                    return null;

                if (DataLogger != null)
                    return DataLogger;
                else if (Column != null)
                    return Column;

                return null;
            }
        }
        #endregion
    }
}
