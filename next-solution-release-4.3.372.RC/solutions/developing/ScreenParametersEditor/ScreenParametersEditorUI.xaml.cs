using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using DocumentManager.ComponentService;
using ScreenParametersEditor.ComponentService;
using ScreenParametersEditor.Controls;
using ScreenParametersEditor.Converters;
using ScreenParametersEditor.UndoRedo;
using ScreenParameterSettings;
using ScreenParametersSettings.Documents;
using UFMenuEditor.UndoRedo;
using Utilities;
using Utilities.WPF;
using WPFUtilities;

namespace ScreenParametersEditor
{
    /// <summary>
    /// Interaction logic for ScreenParametersEditorUI.xaml
    /// </summary>
    public partial class ScreenParametersEditorUI : UserControl, IEditableObject, IDisposable
    {

        readonly UndoRedoManager undoRedoManager = new UndoRedoManager(Properties.Settings.Default.MaxUndoRedoActions);
        
        public ScreenParametersEditorUI(ScreenParametersEditorComponent ed, ScreenParametersDocument doc)
        {
            Clipboard.Clear();
            InitializeComponent();

            editorcomponent = ed;
            document = doc;

            gridDataControl.SelectedItemChanged += (s, e) =>
            {
                    e.Handled = true;
                    if (gridDataControl.SelectedItems.Count == 0)
                    {
                        editorcomponent.Workspace.ContextObject = null;
                    }
                    else if (gridDataControl.SelectedItems.Count == 1)
                    {
                        editorcomponent.Workspace.ContextObject = gridDataControl.SelectedItem;
                    }
                    else
                    {
                        editorcomponent.Workspace.ContextObjects = gridDataControl.SelectedItems;
                    }
            };

            gridDataControl.ItemsSource = Document.GetParametersList();
            gridDataControl.Loaded += (s, e) =>
            {
                tableView.BestFitColumns();
            };
        }

        #region properties
        ScreenParametersEditorComponent editorcomponent;
        public ScreenParametersEditorComponent EditorComponent
        { get { return editorcomponent; } }

        ScreenParametersDocument document;
        public ScreenParametersDocument Document
        {
            get {return document;}
        }
        #endregion

        #region Methods

        void FlatGridRefresh()
        {
            //var list = new List<GridDataGroupColumn>();
            //foreach (var group in gridDataControl.GroupedColumns)
            //    list.Add(new GridDataGroupColumn() { ColumnName = group.ColumnName });

            //while (gridDataControl.GroupedColumns.Count > 0)
            //    gridDataControl.GroupedColumns.Remove(gridDataControl.GroupedColumns[0]);

            var selected = gridDataControl.SelectedItem;
            gridDataControl.ItemsSource = null;
            gridDataControl.ItemsSource = Document.GetParametersList();
            tableView.BestFitColumns();
            gridDataControl.SelectedItem = selected;

            //foreach (var group in list)
            //{
            //    gridDataControl.GroupedColumns.Add(new GridDataGroupColumn() { ColumnName = group.ColumnName });
            //}
        }
        #endregion
        
        #region Event Handlers
        private void OnAddParameter(object sender, ExecutedRoutedEventArgs e)
        {
            var newItem = new ParameterItem(Document);

            if (EditorComponent.PropertyControl != null)
            {
                var p = document.AddParameter(newItem.ID, newItem.text);

                var undoDataObject = new UndoRedoDataObject(UndoRedoAction.Added);
                undoDataObject.AddDataObject(p);
                undoRedoManager.AddUndoAction(undoDataObject);

                FlatGridRefresh();
                gridDataControl.SelectedItem = p;
                EditorComponent.PropertyControl.Activate();
            }
            else
            {
                var newParam = new NewParameter(newItem);
                GeneralDialogContent Dialog = new GeneralDialogContent(newParam)
                {
                    Title = Properties.Resources.NewParameterTitle,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "ScreenParametersEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    var p = document.AddParameter(newItem.ID, newItem.text);

                    var undoDataObject = new UndoRedoDataObject(UndoRedoAction.Added);
                    undoDataObject.AddDataObject(p);
                    undoRedoManager.AddUndoAction(undoDataObject);

                    FlatGridRefresh();
                }
            }
        }

        

        private void CanAddParameter(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        

        private void gridDataControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (EditorComponent.PropertyControl != null)
                EditorComponent.PropertyControl.Activate();
            else
                EditSelectedItem();
        }
        private void OnCommandCut(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                OnCommandCopy(sender, e);
                OnRemoveItem(sender, e);
            }
        }

        private void CanCommandCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected();
        }

        private void OnCommandCopy(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                if (IsAnyItemSelected())
                {
                    StringBuilder clip = new StringBuilder();

                    if (gridDataControl.SelectedItems.Count == 0)
                        return;

                    clip.Append(Properties.Settings.Default.ImportExportAlias);
                    clip.Append(Utilities.ImportExportUtils.ImportExport.Delimiter);
                    clip.Append(Properties.Settings.Default.ImportExportTag);

                    foreach (var item in gridDataControl.SelectedItems)
                    {
                        var cols = item as ParameterItem;
                        if (cols != null)
                        {
                            clip.Append(Environment.NewLine);
                            clip.Append(cols.text);
                            clip.Append(Utilities.ImportExportUtils.ImportExport.Delimiter);
                            clip.Append(cols.ID);
                        }
                    }
                    Clipboard.Clear();
                    Clipboard.SetText(clip.ToString());
                }
            }
        }

        private void CanCommandCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected();
        }

        private void OnCommandPaste(object sender, ExecutedRoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
                using (var cursor = new WaitCursor())
                {
                    e.Handled = true;
                    var undoDataObject = new UndoRedoDataObject(UndoRedoAction.Added);
                    string clip = Clipboard.GetText();
                    string[] lines = clip.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > 0)
                    {
                        List<object> addlist = new List<object>();
                        bool bDirty = false;
                        var mappingnames = new string[2] { Properties.Settings.Default.ImportExportAlias, Properties.Settings.Default.ImportExportTag };
                        Dictionary<string, int> mappNameIndex = new Dictionary<string, int>();
                        for (int k = 0; k < lines.Length; k++)
                        {
                            string[] elems = lines[k].Split(new string[1] { Utilities.ImportExportUtils.ImportExport.Delimiter }, StringSplitOptions.None);
                            if(elems != null)
                            {
                                //first row is column header list
                                bool columnsMatch = true;
                                if (k == 0)
                                {
                                    for (int j = 0; j < 2 && j < elems.Length; j++)
                                    {
                                        columnsMatch = columnsMatch && mappingnames.Contains(elems[j]) && !mappNameIndex.ContainsKey(elems[j]);// mappingnames[j] == elems[j];
                                        if (columnsMatch)
                                            mappNameIndex.Add(elems[j], j);
                                    }
                                }
                                else
                                    columnsMatch = false;

                                if (!columnsMatch)
                                {
                                    int tagIndex = 1;
                                    int aliasIndex = 0;
                                    if (mappNameIndex.ContainsKey(Properties.Settings.Default.ImportExportTag))
                                        tagIndex = mappNameIndex[Properties.Settings.Default.ImportExportTag];
                                    if (mappNameIndex.ContainsKey(Properties.Settings.Default.ImportExportAlias))
                                        aliasIndex = mappNameIndex[Properties.Settings.Default.ImportExportAlias];
                                    var p = document.AddParameter(elems[tagIndex], Document.NewText(elems[aliasIndex]));
                                    undoDataObject.AddDataObject(p);
                                    FlatGridRefresh();
                                    bDirty = true;
                                }
                            }
                        }
                        
                        // undo/redo handling
                        if(bDirty)
                            undoRedoManager.AddUndoAction(undoDataObject);
                    }
                }
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if(Clipboard.ContainsText())
            {
                string clip = Clipboard.GetText();
                string[] lines = clip.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 0)
                {
                    e.CanExecute = lines.ToList().TrueForAll(ContainsValidValues);
                }
            }
        }
        private static bool ContainsValidValues(String s)
        {
            string[] elems = s?.Split(new string[1] { Utilities.ImportExportUtils.ImportExport.Delimiter }, StringSplitOptions.None);
            return elems.Count() >= 2;
        }
        UndoRedoDataObject CheckUndoRedo(UndoRedoDataObject dataObject)
        {
            UndoRedoDataObject retDataObject = null;
            if (dataObject.UndoRedoAction == UndoRedoAction.Added)
                retDataObject = new UndoRedoDataObject(UndoRedoAction.Removed);
            else if (dataObject.UndoRedoAction == UndoRedoAction.Removed)
                retDataObject = new UndoRedoDataObject(UndoRedoAction.Added);
            else if (dataObject.UndoRedoAction == UndoRedoAction.Changed)
                retDataObject = new UndoRedoDataObject(UndoRedoAction.Changed);
            else if (dataObject.UndoRedoAction == UndoRedoAction.LayoutState)
                retDataObject = new UndoRedoDataObject(UndoRedoAction.LayoutState);

            if (dataObject.UndoRedoAction == UndoRedoAction.Added)
            {
                if (dataObject.ParameterItemList != null)
                {
                    dataObject.ParameterItemList.ForEach(parameter =>
                    {
                        var item = document.GetParameterItem(parameter.Guid);
                        if (item != null)
                        {
                            retDataObject.AddDataObject(item);
                            Document.RemoveParameterItem(item);
                        }
                    });
                    FlatGridRefresh();
                }
            }
            else if (dataObject.UndoRedoAction == UndoRedoAction.Removed)
            {
                if (dataObject.ParameterItemList != null)
                {
                    dataObject.ParameterItemList.ForEach(parameter =>
                    {
                        var newitem = document.AddParameter(parameter);
                        retDataObject.AddDataObject(newitem);
                    });
                    FlatGridRefresh();
                }
            }
            else if (dataObject.UndoRedoAction == UndoRedoAction.Changed)
            {
                editorcomponent.Workspace.ContextObject = null;

                if (dataObject.ParameterItemList != null)
                {
                    dataObject.ParameterItemList.ForEach(parameter =>
                    {
                        var item = document.GetParameterItem(parameter.Guid);
                        if (item != null)
                        {
                            retDataObject.AddDataObject(item);
                            item.ID = parameter.ID;
                            item.text = parameter.text;
                        }
                    });

                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() => FlatGridRefresh());
                }

            }

            return retDataObject;
        }

        private void OnCommandUndo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                var dataObject = undoRedoManager.Undo();
                if (dataObject == null || !dataObject.IsAnyActionAvailable)
                    return;

                undoRedoManager.AddRedoAction(CheckUndoRedo(dataObject));
            }
        }

        private void CanCommandUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = undoRedoManager.CanUndo();
        }

        private void OnCommandRedo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                var dataObject = undoRedoManager.Redo();
                if (dataObject == null || !dataObject.IsAnyActionAvailable)
                    return;

                undoRedoManager.AddUndoAction(CheckUndoRedo(dataObject));
            }
        }

        private void CanCommandRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = undoRedoManager.CanRedo();
        }

        private void OnRemoveItem(object sender, ExecutedRoutedEventArgs e)
        {
            var dataObject = new UndoRedoDataObject(UndoRedoAction.Removed);
            foreach (var item in gridDataControl.SelectedItems)
            {
                if (item is ParameterItem)
                {
                    dataObject.AddDataObject(item as ParameterItem);
                    Document.RemoveParameterItem(item as ParameterItem);
                    
                }
            }
            FlatGridRefresh();
            if (dataObject.IsAnyActionAvailable)
                undoRedoManager.AddUndoAction(dataObject);

        }

        private void CanRemoveItem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected();
        }

        private void OnCommandProperties(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (EditorComponent.PropertyControl != null)
                EditorComponent.PropertyControl.Activate();
            else
                EditSelectedItem();
        }

        private void CanCommandProperties(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected();
        }

        private void OnCommandSave(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Document.SaveToFile();
        }

        private void CanCommandSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && Document.NeedsSave;
        }
        #endregion

        #region IEditableObject Members

        UndoRedoDataObject undoRedoDataContext;
        public void BeginEdit()
        {
            undoRedoDataContext = new UndoRedoDataObject(UndoRedoAction.Changed);

            foreach (var item in gridDataControl.SelectedItems)
            {
                var parameter = item as ParameterItem;
                if (parameter != null)
                    undoRedoDataContext.AddDataObject(parameter);
            }
        }

        public void CancelEdit()
        {
            if (undoRedoDataContext != null)
                CheckUndoRedo(undoRedoDataContext);
            undoRedoDataContext = null;
        }

        public void EndEdit()
        {
            if (undoRedoDataContext != null)
                undoRedoManager.AddUndoAction(undoRedoDataContext);
            undoRedoDataContext = null;

            FlatGridRefresh();
        }

        #endregion IEditableObject Members

        internal bool IsAnyItemSelected()
        {
            var item = gridDataControl.SelectedItem as ParameterItem;
            return item != null;
        }

        void EditSelectedItem()
        {
            var selected = gridDataControl.SelectedItem as ParameterItem;
            if (selected != null)
            {
                var mod = new ParameterItem(selected);
                var newParam = new NewParameter(mod);
                GeneralDialogContent Dialog = new GeneralDialogContent(newParam)
                {
                    Title = Properties.Resources.EditParameterTitle,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "ScreenParametersEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    if (mod.ID != selected.ID || mod.text != selected.text)
                    {
                        var undoDataObject = new UndoRedoDataObject(UndoRedoAction.Changed);
                        undoDataObject.AddDataObject(selected);
                        undoRedoManager.AddUndoAction(undoDataObject);

                        selected.ID = mod.ID;
                        selected.text = mod.text;

                        FlatGridRefresh();
                    }
                }
            }
        }

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            targetRecords.Clear();
            targetRecords = null;
            editorcomponent.Workspace.ContextObject = null;

            try
            {
                gridDataControl.Dispose();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Drag&Drop
        private void OnPreviewDragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(RecordDragDropData)))
                return;

            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
            uint forbidden = 0;

            var data = e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData;
            foreach (var v in data.Records)
            {
                IDocumentManager model = null;
                if (v is IDocumentManager)
                    model = v as IDocumentManager;
                else if (v is TreeItemControl)
                    model = (v as TreeItemControl).TreeItemInnerObject as IDocumentManager;
                if (model == null)
                {
                    forbidden++;
                    continue;
                }

                if (model.BrowsableContent is UFUAModel.UFUATag)
                {
                    var tag = (UFUAModel.UFUATag)model.BrowsableContent;
                    if (!CanAssignTag(tag))
                        forbidden++;
                }
            }

            if (forbidden == data.Records.Count())
                e.Effects = DragDropEffects.None;
        }

        private void gridDataControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if(targetRecords != null)
            {
                if(targetRecords.Count > 1)
                {
                    var parList = Document.GetParametersList().ToList();
                    bool bDirty = false;
                    var undoDataObject = new UndoRedoDataObject(UndoRedoAction.Added);
                    targetRecords.ForEach(targetRecord =>
                    {
                        if(AddNewItem(targetRecord, undoDataObject, parList))
                            bDirty = true;
                    });
                    if(bDirty)
                    {
                        FlatGridRefresh();
                        undoRedoManager.AddUndoAction(undoDataObject);
                    }
                    undoDataObject = null;
                }
                else if (targetRecords.Count == 1)
                {
                    var targetRecord = targetRecords.FirstOrDefault();
                    if (targetRecord != null && targetRecord.Tag != null)
                    {
                        TableViewHitInfo hi = ((TableView)(sender as GridControl).View).CalcHitInfo(e.OriginalSource as DependencyObject);
                        if (hi.Column != null)
                        {
                            var tag = targetRecord.TagReference; 
                            if (tag != null)
                            {
                                var undoDataObject = new UndoRedoDataObject(UndoRedoAction.Changed);
                                undoDataObject.AddDataObject(targetRecord.ParameterItem);
                                string value = tag.RelativePath;
                                if (hi.Column.FieldName == "text")
                                    targetRecord.ParameterItem.text = value;
                                else if (hi.Column.FieldName == "ID")
                                    targetRecord.ParameterItem.ID = value;

                                undoRedoManager.AddUndoAction(undoDataObject);
                                undoDataObject = null;
                            }
                        }
                        else
                        {
                            var parList = Document.GetParametersList().ToList();
                            var undoDataObject = new UndoRedoDataObject(UndoRedoAction.Added);
                            if (AddNewItem(targetRecord, undoDataObject, parList))
                            {
                                FlatGridRefresh();
                                undoRedoManager.AddUndoAction(undoDataObject);
                            }
                            undoDataObject = null;
                        }
                    }
                }
                targetRecords.Clear();
            }
        }
        bool AddNewItem(TargetRecord targetRecord, UndoRedoDataObject undoDataObject, List<ParameterItem> parameterItems)
        {
            if (targetRecord != null && targetRecord.Tag != null)
            {
                var tag = targetRecord.TagReference;
                if (tag != null)
                {
                    string value = tag.RelativePath;
                    if((from i in parameterItems where i.text == value select i).FirstOrDefault() == null)
                    {
                        var p = document.AddParameter(null, value);
                        parameterItems.Add(p);
                        undoDataObject.AddDataObject(p);
                        return true;
                    }
                }
            }
            return false;
        }

        private void OnPreviewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (!e.EscapePressed)
                return;

            e.Action = DragAction.Cancel;
            e.Handled = true;
        }

        bool CanAssignTag(UFUAModel.UFUATag tag, UFUAModel.UFUATag instance = null)
        {
            return !tag.IsPrototypeMember || tag.IsSubPrototypeMember || instance != null;
        }
        internal class TargetRecord
        {
            internal ParameterItem ParameterItem { get; set; }
            internal UFUAModel.UFUATag Tag { get; set; }
            internal OPCUAViewModel.OPCUAEntityReference TagReference { get; set; }
        }
        List<TargetRecord> targetRecords = new List<TargetRecord>();
        void OnDropRecord(object sender, DropRecordEventArgs e)
        {
            e.Handled = true;
            if (e.IsFromOutside)
            {
                using (var Cursor = new WaitCursor())
                {
                    var data = e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData;
                    if (data == null)
                        return;

                    var listcolumns = new List<IXPSimpleObject>(data.Records.Count());
                    foreach (var v in data.Records)
                    {
                        IDocumentManager model = null;
                        if (v is IDocumentManager)
                            model = v as IDocumentManager;
                        else if (v is TreeItemControl)
                            model = (v as TreeItemControl).TreeItemInnerObject as IDocumentManager;
                        if (model == null)
                            continue;

                        var entity = model.DragContent as OPCUAViewModel.OPCUAEntityReference;
                        if (model.BrowsableContent is UFUAModel.UFUATag)
                        {
                            var tag = (UFUAModel.UFUATag)model.BrowsableContent;
                            targetRecords.Add(new TargetRecord() { ParameterItem = (ParameterItem)e.TargetRecord, Tag = tag, TagReference = entity });
                        }
                    }
                }
            }
        }
        #endregion
    }

}
