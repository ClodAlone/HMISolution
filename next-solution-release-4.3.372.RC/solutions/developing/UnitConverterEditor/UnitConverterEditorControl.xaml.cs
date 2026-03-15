using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using UnitConverterManager.Document;
using System.Dynamic;
using Utilities;
using Utilities.WPF;
using UnitConverterManager.Controls;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using Ookii.Dialogs.Wpf;
using UIMsgBoxAlertService.ComponentService;
using System.IO.IsolatedStorage;
using UFInterfaces;
using WPFUtilities;
using Utilities.UndoRedo;
using UnitConverterManager.UndoRedo;
using DevExpress.Xpf.Grid;
using DevExpress.Utils;
using UFProjectManager.ComponentService;
using DocumentManager.ComponentService;
using System.Reflection;

namespace UnitConverterManager
{
    /// <summary>
    /// Interaction logic for UnitConverterEditorControl.xaml
    /// </summary>
    public partial class UnitConverterEditorControl : UserControl, IEditableObject, IDisposable
    {
        #region Declarations
        UnitConverterModel.UFConverterItem selectedconverteritem;
        List<ExpandoObject> listData;
        NewConverter newLocale;
        bool isPopup;
        bool isPopupWasClosed;
        bool isDataContextChanging;
        string configurationId = string.Empty;
        string typeLabel = string.Empty;
        string projectName = string.Empty;
        string assembly = string.Empty;

        readonly UndoRedoManager<Memento> undoRedoManager = new UndoRedoManager<Memento>(Properties.Settings.Default.MaxUndoRedoActions);
        #endregion

        bool bLoaded;
        public bool IsLoaded
        {
            get
            {
                return bLoaded;
            }
        }
       
        public List<T> GetSelectedConverter<T>()
        {
            List<T> language = new List<T>();
            var control = new ConverterSelection();
            control.listbox.ItemsSource = Document.GetListLocalConverter();

            var Dialog = new GeneralDialogContent(control);
            Dialog.HelpLink = "ConverterEditor_SelectCulture";
            if (Dialog.ShowDialog() == true)
                foreach (T item in control.listbox.SelectedItems)
                {
                    language.Add(item);
                }
            return language;
        }

        public UnitConverterEditorControl(UnitConverterEditorDocument doc, bool bPopup = false)
        {
            InitializeComponent();
            Document = doc;
            DataContext = Document;
            contextMenu.DataContext = Document;
            IUFProjectManager iUFProjectManager = Document.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
            IDocument parent = DocumentManager.ComponentService.Helpers.DocumentHelper.GetRootParent(Document, true);
            projectName = parent.Title;
            configurationId = parent.Id.ToString();
            typeLabel = Document.Title;
            assembly = System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);

            isPopup = bPopup;
            bool bLoadFilters = true;
            var converters = Document.GetListLocalConverter();

            if (isPopup)
            {
                toolbar.Visibility = System.Windows.Visibility.Visible;

                bool bNeedToReload = false;
                Document.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == "NeedsSave")
                    {
                        bNeedToReload = true;
                    }
                };

                Loaded += (o, e) =>
                {

                    bLoaded = true;

                    var wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        wnd.Activated += (s, c) =>
                        {
                            if (isPopupWasClosed)
                            {
                                isPopupWasClosed = false;
                                if (bNeedToReload)
                                {
                                    bNeedToReload = false;
                                    listData = Document.GetListLocaleFlat();
                                    gridDataControl.ItemsSource = listData;
                                }
                            }
                        };

                        wnd.Closing += (s, c) =>
                        {
                            isPopupWasClosed = true;
                            SaveFilterSettings(Properties.Settings.Default.FilterSettings);
                            var selected = gridDataControl.SelectedItem as IDictionary<String, object>;
                            if (selected != null)
                            {
                                isDataContextChanging = true;
                                DataContext = selected["ID"];
                                isDataContextChanging = false;

                                if (Document.NeedsSave)
                                {
                                    using (var cursor = new WaitCursor())
                                    {
                                        CommitChanges();
                                        Document.SaveToFile();
                                        Document.NeedsSave = false;
                                    }
                                }
                            }
                        };
                    }
                    if (bNeedToReload)
                    {
                        bNeedToReload = false;
                        listData = Document.GetListLocaleFlat();
                        gridDataControl.ItemsSource = listData;
                    }
                    Focusable = true;
                    Focus();

                };


            }
            else
            {
                Loaded += (o, e) =>
                {
                    bLoaded = true;
                };
            }
            Unloaded += (o, e) =>
            {
                bLoaded = false;
            };

            if (converters.Count == 0)
            {
                if (Document.EditorManagerComponent.UIInterface != null)
                {
                    if (Document.EditorManagerComponent.UIInterface.ShowYesNo(Properties.Resources.NeedToLoadStandardConverters, CustomDialogIcons.Question) == CustomDialogResults.Yes)
                    {
                        string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
                        string startingPath = String.Format("{0}.{1}\\Converters\\", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), mainversion);
                        string fileToOpen = string.Empty;

                        fileToOpen = string.Format("{0}{1}\\UnitConverters.csv", startingPath, System.Threading.Thread.CurrentThread.CurrentUICulture.Name);

                        if (!File.Exists(fileToOpen))
                            fileToOpen = string.Format("{0}UnitConverters.csv", startingPath);
                        if (File.Exists(fileToOpen))
                        {
                            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                            {
                                if (bDisposed)
                                    return;

                                ImportFile(fileToOpen);
                            });
                        }
                    }
                }
                bLoadFilters = false;
            }
            if (bLoadFilters)
                LoadFilterSettings(Properties.Settings.Default.FilterSettings);

            ColumnsGridRefresh(converters);
            listData = Document.GetListLocaleFlat();
            gridDataControl.ItemsSource = listData;
        }

        private void UpdateSelectedItem()
        {
            if(!isPopup)
            {
                Document.EditorManagerComponent.Workspace.ContextObject = selectedconverteritem = null;
                List<ExpandoObject> selecteditems = GetSelectedObjects();
                if (selecteditems.Count > 0)
                {
                    var cellValue = gridDataControl.CurrentCellValue;
                    if (cellValue is UnitConverterModel.UFConverterItem)
                    {
                        selectedconverteritem = cellValue as UnitConverterModel.UFConverterItem;
                        Document.EditorManagerComponent.Workspace.ContextObject = selectedconverteritem;
                    }
                }
            }
        }
        internal void OnActivate()
        {
            Document.EditorManagerComponent.Workspace.ContextObject = null;
        }

        #region Properties

        UnitConverterEditorDocument _Document;
        [Browsable(false)]
        public UnitConverterEditorDocument Document
        {
            get
            {
                return _Document;
            }
            private set
            {
                _Document = value;
            }
        }

        #endregion

        #region IEditableObject Members

        UndoRedoDataObject<Memento> undoRedoDataContext;
        public void BeginEdit()
        {
            undoRedoDataContext = new UndoRedoDataObject<Memento>(UndoRedoAction.Snapshot);
            undoRedoDataContext.AddDataObject(new Memento(listData, Document.GetListLocalConverter()));
        }

        public void CancelEdit()
        {
            if (undoRedoDataContext != null)
                CheckUndoRedo(undoRedoDataContext);
            undoRedoDataContext = null;
            gridDataControl.RefreshData();
        }

        public void EndEdit()
        {
            if (undoRedoDataContext != null && undoRedoDataContext.IsAnyActionAvailable)
            {
                var memento = new Memento(Document.GetListLocaleFlat(), Document.GetListLocalConverter());
                if (undoRedoDataContext.DataObjects[0] != memento)
                    undoRedoManager.AddUndoAction(undoRedoDataContext);
            }
            undoRedoDataContext = null;
            gridDataControl.RefreshData();
        }

        #endregion

        #region Commands
        private void gridDataControl_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            UpdateSelectedItem();
        }

        private void gridDataControl_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            e.Handled = true;
            var selected = e.Source.DataControl.SelectedItem as IDictionary<String, object>;
            if (selected != null)
            {
                UpdateSelectedItem();

                if (isPopup)
                {
                    if (selected != null)
                    {
                        var wnd = this.FindParent<Window>();
                        if (wnd != null)
                        {
                            wnd.DialogResult = true;
                            wnd.Close();
                        }
                    }
                }
            }
        }

        private void gridDataControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (gridDataControl.CurrentCellValue is UnitConverterModel.UFConverterItem && e.Key != Key.Insert)
                return;
            
            if (e.Key == Key.Insert)
                AddNewConverterItem();
        }

        public void CommitChanges()
        {
            var listLocales = new List<String>();
            foreach (var column in gridDataControl.Columns)
            {
                if (column == colID)
                    continue;
                listLocales.Add(column.FieldName);
            }

            Document.UpdateLocaleData(listData, listLocales);
            listData = Document.GetListLocaleFlat();
            gridDataControl.ItemsSource = listData;
        }

        private void OnCommandSave(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (var cursor = new WaitCursor())
            {
                //CommitChanges();
                var ret = Document.SaveToFile();
                if (!ret)
                {
                    Document.EditorManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorSavingDocWithoutClosure,
                        String.Format("{0} ({1})", Document.EditorManagerComponent.TypeTitle, Document.Parent.Title)));
                }
                else
                    Document.NeedsSave = false;
            }
        }

        private void CanCommandSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document.NeedsSave;
        }

        private void OnCommandCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                CopySelectedToClipboard();
                DeleteSelectedItems();
            }
        }

        private void CanCommandCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected();
            e.Handled = !e.CanExecute;
        }

        private void OnCommandCopy(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                CopySelectedToClipboard();
            }
        }

        private void CanCommandCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected();
            e.Handled = !e.CanExecute;
        }

        private void OnCommandPaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                PasteFromClipboard();
            }
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            Document.CopyWinClipboardToInMemoryData();
            e.CanExecute = Document.ClipboardContainsLocaleTexts();
            e.Handled = !e.CanExecute;
        }

        private void OnCommandUndo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                UndoAction();
            }
        }

        private void CanCommandUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyUndoActionAvailable();
        }

        private void OnCommandRedo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                RedoAction();
            }
        }

        private void CanCommandRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyRedoActionAvailable();
        }

        private void OnRemoveItem(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteSelectedItems();
        }

        private void CanRemoveItem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected();
        }
        private void OnAddNewConverter(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            AddNewConverter();
        }

        private void AddNewConverter()
        {
            BeginEdit();

            newLocale = new NewConverter();

            GeneralDialogContent Dialog = new GeneralDialogContent(newLocale)
            {
                Owner = this.FindParent<Window>(),
                DialogKeepContent = true,
                Title = Properties.Resources.AddNewConverterSystem,
                HelpLink = "AddNewConverter"
            };
            try
            {
                if (Dialog.ShowDialog() == true)
                {
                    var culture = newLocale.GetSelected();
                    if (!String.IsNullOrWhiteSpace(culture))
                    {
                        if (Document.AddLocale(culture) != null)
                        {
                            gridDataControl.BeginInit();
                            gridDataControl.BeginDataUpdate();

                            var column = new GridColumn() { ReadOnly = true, FieldName = culture, Header = culture,
                                SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom,
                                GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.Value,
                                ShowGroupedColumn = DefaultBoolean.True, AllowGrouping = DefaultBoolean.True };
                            column.CellTemplate = gridDataControl.TryFindResource("cellTemplate") as DataTemplate;
                            gridDataControl.Columns.Add(column);
                            gridDataControl.Tag = null;
                            gridDataControl.FilterCriteria = null;
                            listData = Document.GetListLocaleFlat();
                            gridDataControl.ItemsSource = listData;

                            Document.NeedsSave = true;

                            gridDataControl.EndInit();
                            gridDataControl.EndDataUpdate();
                            EndEdit();
                        }
                        else if (Document.EditorManagerComponent.UIInterface != null)
                        {
                            Document.EditorManagerComponent.UIInterface.ShowError(string.Format(Properties.Resources.CultureAlreadyInList, culture));
                            CancelEdit();
                        }
                    }
                    else
                    {
                        CancelEdit();
                    }
                }
                else
                {
                    CancelEdit();
                }
            }
            catch (Exception)
            {
                gridDataControl.EndInit();
                gridDataControl.EndDataUpdate();
                CancelEdit();
            }
        }

        private void CanAddNewConverter(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnAddNewConverterItem(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            AddNewConverterItem();
        }

        bool IsIDTextDefined(String id)
        {
            var founds = (from c in listData.OfType<IDictionary<String, Object>>()/*.AsParallel()*/
                             where c.ContainsKey(UnitConverterEditorDocument.idText) && 
                             c[UnitConverterEditorDocument.idText] as String == id
                             select c).ToList();
            return founds.Count > 1;
        }

        String CreateNewStringId(String s)
        {
            int i = 0;
            var id = s;
            while (IsIDTextDefined(id))
                id = String.Format("{0}{1}", Properties.Resources.NewConverterID, ++i);
            return id;
        }

        private void AddNewConverterItem()
        {
            BeginEdit();

            string id = CreateNewStringId(Properties.Resources.NewConverterID);
            var llt = Document.AddLocaleText(id);
            listData = Document.GetListLocaleFlat();

            FlatGridRefresh();
            Document.NeedsSave = true;

            EndEdit();
        }

        internal void AddNewLocalIdList(IList<String> list)
        {
            var selection = new ObservableCollection<object>();

            list.ToList().ForEach(s =>
            {
                var found = (from c in listData.OfType<IDictionary<String, Object>>()/*.AsParallel()*/
                                 where c.ContainsKey(UnitConverterEditorDocument.idText) && 
                                 c[UnitConverterEditorDocument.idText] as String == s
                                 select c).FirstOrDefault();
                if (found == null)
                {
                    dynamic dynObject = new ExpandoObject();
                    var p = dynObject as IDictionary<String, object>;

                    p[UnitConverterEditorDocument.idText] = s;
                    listData.Add(dynObject);
                    selection.Add(dynObject);
                }
                else
                    selection.Add(found);
            });
            gridDataControl.ItemsSource = null;
            gridDataControl.ItemsSource = listData;
            gridDataControl.SelectedItems = selection;

            Document.NeedsSave = true;
        }

        private void CanAddNewConverterItem(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = gridDataControl.Columns.Count > 1;
            }
            catch (Exception)
            {
                e.CanExecute = false;
            }
        }

        #endregion

        #region Methods
        bool IsAnyItemSelected()
        {
            return gridDataControl.SelectedItem is IDictionary<string, object> && gridDataControl.VisibleItems != null && gridDataControl.VisibleItems.Contains(gridDataControl.SelectedItem);
        }

        void PasteFromClipboard()
        {
            if (Clipboard.ContainsText())
            {
                try
                {
                    string clip = Clipboard.GetText();
                    string[] lines = clip.Split(new string[1] { UnitConverterModel.ExportUtils.newline }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > 0)
                    {
                        string[] mappingnames = null;
                        List<object> addlist = new List<object>();
                        for (int k = 0; k < lines.Length; k++)
                        {
                            string[] elems = lines[k].Split(new string[1] { UnitConverterModel.ExportUtils.delimiter }, StringSplitOptions.None);
                            if (k == 0)
                            {
                                //first line, cultures?
                                if (elems.Length == 0)
                                {
                                    Document.LogGeneralError(Properties.Resources.ImportErrorClipNoCols, null);
                                    if (Document.EditorManagerComponent.UIInterface != null)
                                    {
                                        using (new ResetCursor())
                                        {
                                            Document.EditorManagerComponent.UIInterface.ShowInformation(Properties.Resources.ImportErrorClipNoCols);
                                        }
                                    }
                                    return;
                                }

                                //var cultures = Document.GetListLocalCultures();
                                List<string> cultures = new List<string>();
                                for (int p = 1; p < gridDataControl.Columns.Count; p++)
                                {
                                    cultures.Add(gridDataControl.Columns[p].FieldName);
                                }

                                int colnumber = System.Math.Min(cultures.Count + 1, elems.Length);
                                mappingnames = new string[colnumber];
                                mappingnames[0] = elems[0];

                                bool positional = false;
                                for (int i = 1; i < colnumber; i++)
                                {

                                    int pos = cultures.IndexOf(elems[i]);
                                    if (pos == -1)
                                    {
                                        positional = true;
                                        break;
                                    }
                                    else
                                        mappingnames[i] = cultures[pos];
                                }
                                if (positional)
                                {
                                    int nlang = System.Math.Min(cultures.Count, colnumber);
                                    for (int i = 0; i < nlang; i++)
                                        mappingnames[i + 1] = cultures[i];
                                }
                                else
                                    continue;
                            }
                            //add row
                            addlist.Add(elems);
                        }

                        if (addlist.Count > 0)
                            PasteListLocaleText(addlist, mappingnames, false);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        void PasteListLocaleText(List<object> addlist, string[] mappingnames, bool update = true)
        {
            BeginEdit();

            var lisths = Document.PasteListLocaleText(addlist, mappingnames, update);
            if (lisths.Count > 0)
            {
                FlatGridRefresh();

                Document.NeedsSave = true;

                EndEdit();
            }
        }

        void CopySelectedToClipboard()
        {
            StringBuilder clip = new StringBuilder();
            List<ExpandoObject> selecteditems = GetSelectedObjects();
            if (selecteditems.Count == 0)
                return;
            //first row with culture references
            for (int j = 0; j < gridDataControl.Columns.Count; j++)
            {
                clip.Append(gridDataControl.Columns[j].FieldName);
                if (j == gridDataControl.Columns.Count - 1)
                    clip.Append(UnitConverterModel.ExportUtils.newline);
                else
                    clip.Append(UnitConverterModel.ExportUtils.delimiter);
            }

            foreach (var item in selecteditems)
            {
                var cols = item as IDictionary<string, object>;
                if (cols != null)
                {

                    for (int i = 0; i < gridDataControl.Columns.Count; i++)
                    {
                        if (cols.ContainsKey(gridDataControl.Columns[i].FieldName))
                        {
                            if (gridDataControl.Columns[i].FieldName == UnitConverterEditorDocument.idText)
                                clip.Append(cols[gridDataControl.Columns[i].FieldName] as string);
                            else
                            {
                                var ci = cols[gridDataControl.Columns[i].FieldName] as UnitConverterModel.UFConverterItem;
                                if (ci != null)
                                {
                                    clip.Append($"{ci.InputExpression}{UnitConverterModel.ExportUtils.ciseparator}{ci.OutputExpression}{UnitConverterModel.ExportUtils.ciseparator}{ci.InputUnit}{UnitConverterModel.ExportUtils.ciseparator}{ci.Description}");
                                }
                                else
                                    clip.Append(string.Empty);
                            }
                        }
                        if (i == gridDataControl.Columns.Count - 1)
                            clip.Append(UnitConverterModel.ExportUtils.newline);
                        else
                            clip.Append(UnitConverterModel.ExportUtils.delimiter);
                    }
                }
            }
            Clipboard.Clear();
            Clipboard.SetText(clip.ToString());

        }

        List<ExpandoObject> GetSelectedObjects()
        {
            List<ExpandoObject> selecteditems = new List<ExpandoObject>();

            if (gridDataControl.SelectedItem != null)
                selecteditems.Add(gridDataControl.SelectedItem as ExpandoObject);

            foreach (var item in gridDataControl.SelectedItems)
            {
                if (!selecteditems.Contains(item))
                    selecteditems.Add(item as ExpandoObject);
            }
            return selecteditems;
        }

        List<UnitConverterModel.UFConverterItem> GetSelectedItems()
        {

            var items = new List<UnitConverterModel.UFConverterItem>();
            List<ExpandoObject> selecteditems = GetSelectedObjects();
            foreach (var item in selecteditems)
            {
                var cols = item as IDictionary<string, object>;
                if (cols != null)
                {
                    string id = cols[UnitConverterEditorDocument.idText] as string;
                    var idtext = Document.GetLocaleTextFromId(id);
                    if (idtext.Count > 0)
                        items.AddRange(idtext);
                }
            }
            return items;
        }

        void DeleteSelectedItems()
        {
            BeginEdit();

            var items = GetSelectedItems();
            foreach (var ltext in items)
            {
                var slocale = ltext.UFConverter;
                if (slocale != null)
                {
                    var idx = slocale.UFConverterItems.IndexOf(ltext);
                    if (idx != -1)
                        slocale.UFConverterItems[idx].Delete();
                }
                ltext.Delete();
            }

            items.Clear();
            FlatGridRefresh();
            CommitChanges();

            EndEdit();
        }

        void ColumnsGridRefresh()
        {
            var cultures = Document.GetListLocalConverter();
            ColumnsGridRefresh(cultures);
        }

        void ColumnsGridRefresh(IList<String> cultures)
        {
            List<string> actualList = (from c in gridDataControl.Columns
                                       where c.FieldName != UnitConverterEditorDocument.idText
                                       select c.FieldName).ToList();

            var toRemoveColList = (from c in gridDataControl.Columns 
                                    where c.FieldName != UnitConverterEditorDocument.idText &&
                                    !cultures.Contains(c.FieldName)
                                    select c).ToList();

            toRemoveColList.ForEach(c => gridDataControl.Columns.Remove(c));
            List<string> toAddList = (from c in cultures where !actualList.Contains(c) select c).ToList();

            foreach (var culture in toAddList)
            {
                var column = new GridColumn() { ReadOnly = true, FieldName = culture, Header = culture,
                    SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom,
                    GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.Value,
                    ShowGroupedColumn = DefaultBoolean.True, AllowGrouping = DefaultBoolean.True };
                column.CellTemplate = gridDataControl.TryFindResource("cellTemplate") as DataTemplate;
                gridDataControl.Columns.Add(column);
            }
        }

        void FlatGridRefresh()
        {
            var selected = gridDataControl.SelectedItem;
            listData = Document.GetListLocaleFlat();
            gridDataControl.ItemsSource = listData;
            if (listData.Count > 0 && selected != null)
            {
                gridDataControl.SelectedItem = listData.Find((o) =>
                {
                    var p = o as IDictionary<String, object>;
                    if (p != null)
                    {
                        if ((p[UnitConverterEditorDocument.idText] as String) == ((selected as IDictionary<String, object>)[UnitConverterEditorDocument.idText] as String))
                            return true;
                    }
                    return false;
                });
            }
        }
        #endregion

        #region Undo/Redo

        bool IsAnyUndoActionAvailable()
        {
            return undoRedoManager.CanUndo();
        }

        bool IsAnyRedoActionAvailable()
        {
            return undoRedoManager.CanRedo();
        }

        void CleanUndoActions()
        {
            undoRedoManager.CleanUndoActions();
        }

        void CleanRedoActions()
        {
            undoRedoManager.CleanRedoActions();
        }

        bool IsUndoRedoSupported(object obj)
        {
            return obj is UnitConverterModel.UFConverterItem;
        }

        void UndoAction()
        {
            var dataObject = undoRedoManager.Undo();
            if (dataObject == null || !dataObject.IsAnyActionAvailable)
                return;

            if (dataObject.UndoRedoAction == UndoRedoAction.Snapshot)
            {
                var action = new UndoRedoDataObject<Memento>(UndoRedoAction.Snapshot);
                action.AddDataObject(new Memento(listData, Document.GetListLocalConverter()));
                undoRedoManager.AddRedoAction(action);
            }

            CheckUndoRedo(dataObject);
        }

        void RedoAction()
        {
            var dataObject = undoRedoManager.Redo();
            if (dataObject == null || !dataObject.IsAnyActionAvailable)
                return;

            if (dataObject.UndoRedoAction == UndoRedoAction.Snapshot)
            {
                var action = new UndoRedoDataObject<Memento>(UndoRedoAction.Snapshot);
                action.AddDataObject(new Memento(listData, Document.GetListLocalConverter()));
                undoRedoManager.AddUndoAction(action);
            }

            CheckUndoRedo(dataObject);
        }

        void CheckUndoRedo(UndoRedoDataObject<Memento> dataObject)
        {
            if (dataObject.UndoRedoAction == UndoRedoAction.Snapshot)
            {
                if (dataObject.DataObjects.Count > 0)
                {
                    bool bUpdateColumns = (gridDataControl.Columns.Count - 1) != dataObject.DataObjects[0].Locales.Count;
                    if (!bUpdateColumns)
                    {
                        foreach (var column in gridDataControl.Columns)
                        {
                            if (column == colID)
                                continue;

                            if (!dataObject.DataObjects[0].Locales.Contains(column.FieldName))
                            {
                                bUpdateColumns = true;
                                break;
                            }
                        }
                    }

                    Document.UpdateLocaleData(dataObject.DataObjects[0].Converters, dataObject.DataObjects[0].Locales);

                    if (bUpdateColumns)
                        ColumnsGridRefresh(dataObject.DataObjects[0].Locales);

                    FlatGridRefresh();
                }
            }
        }

        #endregion

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;

            bDisposed = true;

            try
            {
                SaveFilterSettings(Properties.Settings.Default.FilterSettings);
            }
            catch (Exception)
            {

            }

            try
            {
                gridDataControl.Dispose();

                if (newLocale != null)
                {
                    newLocale.Dispose();
                    newLocale = null;
                }
               
            }
            catch (Exception ex)
            {

            }
        }

        private void OnImportConverters(object sender, ExecutedRoutedEventArgs e)
        {
            string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
            string startingPath = String.Format("{0}.{1}\\Converters\\", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), mainversion);


            var dialog = new VistaOpenFileDialog();
            dialog.InitialDirectory = startingPath;
            dialog.DefaultExt = "CSV Files (*.csv)|*.csv";
            dialog.CheckFileExists = false;
            dialog.Multiselect = false;
            dialog.Filter = "CSV Files (*.csv)|*.csv";//"CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";

            string filename = string.Empty;
            if (dialog.ShowDialog() == true)
            {
                ImportFile(dialog.FileName);
            }
        }

        private void ImportFile(string filename)
        {
            try
            {
                //string line;
                //bool firstline = true;
                // using (new WaitCursor())
                {
                    //using (StreamReader readFile = new StreamReader(filename))
                    {
                        List<object> addlist = new List<object>();
                        string[] mappingnames = null;
                        string[] colnames = null;
                        UnitConverterModel.ExportUtils.ImportFromFile(filename, addlist, ref colnames);
                        if (colnames.Length == 0)
                        {
                            Document.LogGeneralError(string.Format(Properties.Resources.ImportErrorFileNoCols, filename), null);
                            if (Document.EditorManagerComponent.UIInterface != null)
                                Document.EditorManagerComponent.UIInterface.ShowInformation(string.Format(Properties.Resources.ImportErrorFileNoCols, filename));
                            return;
                        }

                        UnitConverterSelector Choose = new UnitConverterSelector(colnames, addlist);
                        GeneralDialogContent Dialog = new GeneralDialogContent(Choose)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = "ConverterEditor_OnImportConverters"
                        };
                        if (Dialog.ShowDialog() != true)
                            return;

                        addlist.Clear();
                        if (Choose.gridDataControl.SelectedItems.Count > 0)
                        {
                            foreach (var si in Choose.gridDataControl.SelectedItems)
                            {

                                var so = (si as IDictionary<String, object>);
                                if (so != null)
                                {
                                    string[] p = new string[so.Values.Count];
                                    for (int i = 0; i < so.Values.Count; i++)
                                        p[i] = so.Values.ToList()[i] as string;
                                    addlist.Add(p);
                                }
                            }
                        }
                        else if (Choose.gridDataControl.SelectedItem != null)
                        {
                            var so = Choose.gridDataControl.SelectedItem as IDictionary<String, string>;
                            if (so != null)
                            {
                                addlist.Add(so.Values);
                            }
                        }

                        if (addlist.Count == 0)
                            return;

                        mappingnames = new string[colnames.Length];
                        mappingnames[0] = colnames[0];
                        var cultures = Document.GetListLocalConverter();
                        for (int i = 1; i < colnames.Length; i++)
                        {

                            int pos = cultures.IndexOf(colnames[i]);
                            if (pos == -1)
                            {
                                //add culture
                                var culture = Document.AddLocale(colnames[i]);
                                if (culture == null)
                                {
                                    Document.LogGeneralError(string.Format(Properties.Resources.ImportErrorFileMissCols, filename), null);
                                    if (Document.EditorManagerComponent.UIInterface != null)
                                        Document.EditorManagerComponent.UIInterface.ShowInformation(string.Format(Properties.Resources.ImportErrorFileMissCols, filename));
                                    return;
                                }
                                else
                                {
                                    var column = new GridColumn()
                                    {
                                        ReadOnly = true,
                                        FieldName = culture,
                                        Header = culture,
                                        SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom,
                                        GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.Value,
                                        ShowGroupedColumn = DefaultBoolean.True,
                                        AllowGrouping = DefaultBoolean.True
                                    };
                                    column.CellTemplate = gridDataControl.TryFindResource("cellTemplate") as DataTemplate;
                                    gridDataControl.Columns.Add(column);
                                    Document.NeedsSave = true;
                                }
                                mappingnames[i] = colnames[i];
                            }
                            else
                            {
                                mappingnames[i] = cultures[pos];
                            }
                        }
                        if (addlist.Count > 0)
                        {
                            PasteListLocaleText(addlist, mappingnames, true);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Document.LogGeneralError(string.Format(Properties.Resources.ImportError, filename, ex.Message), ex);
                if (Document.EditorManagerComponent.UIInterface != null)
                    Document.EditorManagerComponent.UIInterface.ShowInformation(string.Format(Properties.Resources.ImportError, filename, ex.Message));
            }
        }
        private void CanImportConverters(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnExportConverters(object sender, ExecutedRoutedEventArgs e)
        {
            var list = GetSelectedObjects();
            if (list.Count == 0)
            {
                List<ExpandoObject> datalist = gridDataControl.ItemsSource as List<ExpandoObject>;
                if (datalist?.Count > 0)
                    list.AddRange(datalist);
                if (list.Count == 0)
                    return;
            }

            string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
            string startingPath = String.Format("{0}.{1}\\Converters\\", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), mainversion);


            var dialog = new VistaOpenFileDialog();
            dialog.InitialDirectory = startingPath;
            dialog.DefaultExt = "CSV Files (*.csv)|*.csv";
            dialog.CheckFileExists = false;
            dialog.Multiselect = false;
            dialog.Filter = "CSV Files (*.csv)|*.csv";//"CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";

            string filename = string.Empty;
            if (dialog.ShowDialog() == true)
            {
                filename = dialog.FileName;

                StringBuilder clip = new StringBuilder();

                for (int i = 0; i < gridDataControl.Columns.Count; i++)
                {
                    clip.Append(gridDataControl.Columns[i].FieldName as string);
                    if (i == gridDataControl.Columns.Count - 1)
                        clip.Append(UnitConverterModel.ExportUtils.newline);
                    else
                        clip.Append(UnitConverterModel.ExportUtils.csvseparator);
                }

                list.ForEach(s =>
                {
                    var row = s as IDictionary<string, object>;
                    if (row != null)
                    {
                        for (int i = 0; i < gridDataControl.Columns.Count; i++)
                        {
                            if (row.ContainsKey(gridDataControl.Columns[i].FieldName))
                            {
                                if (gridDataControl.Columns[i].FieldName == UnitConverterEditorDocument.idText)
                                    clip.Append(row[gridDataControl.Columns[i].FieldName] as string);
                                else
                                {
                                    var ci = row[gridDataControl.Columns[i].FieldName] as UnitConverterModel.UFConverterItem;
                                    if (ci != null)
                                    {
                                        clip.Append($"{ci.InputExpression}{UnitConverterModel.ExportUtils.ciseparator}{ci.OutputExpression}{UnitConverterModel.ExportUtils.ciseparator}{ci.InputUnit}{UnitConverterModel.ExportUtils.ciseparator}{ci.Description}");
                                    }
                                    else
                                        clip.Append(string.Empty);
                                }
                            }
                            if (i == gridDataControl.Columns.Count - 1)
                                clip.Append(UnitConverterModel.ExportUtils.newline);
                            else
                                clip.Append(UnitConverterModel.ExportUtils.csvseparator);
                        }
                    }
                });
                var exportstring = clip.ToString().TrimEnd(new char[] { '\n', '\r' });
                if (exportstring.Length > 0)
                {
                    try
                    {
                        if (File.Exists(filename) && Document.EditorManagerComponent.UIInterface != null)
                        {
                            var ret = Document.EditorManagerComponent.UIInterface.ShowYesNoCancel(Properties.Resources.ExportFileWarning, CustomDialogIcons.Question);
                            if (ret == CustomDialogResults.Cancel)
                                return;
                            if (ret == CustomDialogResults.No)
                            {
                                int i = 1;
                                while (System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename), string.Format("{0}{1}{2}", System.IO.Path.GetFileNameWithoutExtension(filename), i.ToString(), System.IO.Path.GetExtension(filename).ToLower()))))
                                {
                                    i++;
                                }
                                filename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename), string.Format("{0}{1}{2}", System.IO.Path.GetFileNameWithoutExtension(filename), i.ToString(), System.IO.Path.GetExtension(filename).ToLower()));
                            }
                        }

                        using (StreamWriter s = new StreamWriter(filename, false, Encoding.Unicode) /*File.CreateText(filename)*/)
                        {
                            s.Write(exportstring);
                            Document.LogGeneralInfo(string.Format(Properties.Resources.ExportCompleted, filename));
                            if (Document.EditorManagerComponent.UIInterface != null)
                                Document.EditorManagerComponent.UIInterface.ShowInformation(string.Format(Properties.Resources.ExportCompleted, filename));
                        }
                    }
                    catch (Exception ex)
                    {
                        //log/signal error!! WriteToEventLog(BaseFileName, Message, EventLogEntryType.FailureAudit);
                        Document.LogGeneralError(string.Format(Properties.Resources.ExportError, filename, ex.Message), ex);
                        if (Document.EditorManagerComponent.UIInterface != null)
                            Document.EditorManagerComponent.UIInterface.ShowError(string.Format(Properties.Resources.ExportError, filename, ex.Message));
                    }
                }
            }
        }

        private void CanExportConverters(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnRemoveLocale(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            BeginEdit();

            var list = GetSelectedConverter<string>();
            list.ForEach(col =>
            {
                if (col.Length > 0)
                {
                    Document.RemoveLocale(col);
                }
            });

            ColumnsGridRefresh();
            FlatGridRefresh();
            EndEdit();
        }

        private void CanRemoveLocale(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = gridDataControl.Columns.Count > 1;
            }
            catch (Exception)
            {
                e.CanExecute = false;
            }
        }
        #region Storage

        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }

        String GetStoreFilterFileName(String title)
        {
            return String.Format("{0}.{1}.{2}.{3}.{4}.dat", assembly, configurationId, projectName, title, typeLabel);
        }

        void SaveFilterSettings(String title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFilterFileName(title), FileMode.Create, isoStorage))
                {
                    try
                    {
                        gridDataControl.SaveLayoutToStream(stream);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        bool LoadFilterSettings(String title)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return true;

            try
            {
                var isoStorage = GetStorage();
                if (null != isoStorage && !String.IsNullOrEmpty(title))
                {
                    using (var stream = new IsolatedStorageFileStream(GetStoreFilterFileName(title), FileMode.OpenOrCreate, isoStorage))
                    {
                        try
                        {
                            gridDataControl.RestoreLayoutFromStream(stream);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return true;
        }
        #endregion

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            UpdateSelectedItem();

            if (selectedconverteritem == null)
                return;

            BeginEdit();
            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var _selected = uow.GetNestedObject(selectedconverteritem);
                _selected.InputExpression = null;
                _selected.OutputExpression = null;
                _selected.InputUnit = null;
                _selected.Description = null;
                try
                {
                    uow.CommitChanges();
                    EndEdit();
                }
                catch (Exception ex)
                {
                    Document.LogGeneralError(ex.Message, null);
                    if (Document.EditorManagerComponent.UIInterface != null)
                        Document.EditorManagerComponent.UIInterface.ShowError(ex.Message);
                }
            }
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isPopup && Document != null)
            {
                if (Document.EditorManagerComponent.PropertyControl != null)
                    Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else 
                EditSelectedItem(sender);
        }

        void EditSelectedItem(object sender)
        {
            UpdateSelectedItem();

            if (selectedconverteritem == null)
                return;

            BeginEdit();

            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var newConverterItem = new NewConverterItem(uow.GetNestedObject(selectedconverteritem));
                GeneralDialogContent Dialog = new GeneralDialogContent(newConverterItem)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "EditConverterItem",
                    Title = selectedconverteritem.Name
                };
                if (Dialog.ShowDialog() == true)
                {
                    try
                    {
                        uow.CommitChanges();
                        EndEdit();
                    }
                    catch (Exception ex)
                    {
                        Document.LogGeneralError(ex.Message, null);
                        if (Document.EditorManagerComponent.UIInterface != null)
                            Document.EditorManagerComponent.UIInterface.ShowError(ex.Message);
                    }
                }
            }
        }

        private void OnCommandProperties(object sender, ExecutedRoutedEventArgs e)
        {
            if (!isPopup && Document != null)
            {
                if (Document.EditorManagerComponent.PropertyControl != null)
                    Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
                EditSelectedItem(null);
        }

        private void CanCommandProperties(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = selectedconverteritem != null;
        }

        private void OnRenameItem(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            RenameConverter();
        }
        private void RenameConverter()
        {
            if (gridDataControl.Columns.Count == 0)
                return;

            BeginEdit();

            UnitConverterRenamer renameConverter = new UnitConverterRenamer(Document.GetListLocalConverter());

            GeneralDialogContent Dialog = new GeneralDialogContent(renameConverter)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.RenameConverterSystem,
                HelpLink = "RenameConverter"
            };

            if (Dialog.ShowDialog() == true)
            {
                var renamedColumns = new Dictionary<String, String>();
                renameConverter.GetConverters().ForEach(r =>
                {
                    if (Document.UpdateLocale(this, r.OldValue, r.NewValue) != null)
                    {
                        if (!renamedColumns.ContainsKey(r.OldValue))
                            renamedColumns.Add(r.OldValue, r.NewValue);
                    }
                });

                if (renamedColumns.Count > 0)
                {
                    foreach (var column in gridDataControl.Columns)
                    {
                        if (column.FieldName == UnitConverterEditorDocument.idText)
                            continue;

                        if (renamedColumns.ContainsKey(column.FieldName))
                        {
                            var culture = renamedColumns[column.FieldName];
                            column.Header = column.FieldName = culture;
                        }
                    }

                    FlatGridRefresh();
                    EndEdit();
                }
            }
        }

        private void CanRenameItem(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = gridDataControl.Columns.Count > 1;
            }
            catch (Exception)
            {
                e.CanExecute = false;
            }
        }

        private void GridDataControl_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
        {
            if (e.Column.FieldName != "ID")
            {
                UnitConverterModel.UFConverterItem converter1 = (UnitConverterModel.UFConverterItem)e.Value1;
                UnitConverterModel.UFConverterItem converter2 = (UnitConverterModel.UFConverterItem)e.Value2;
                e.Result = converter1.CompareTo(converter2);
                e.Handled = true;
            }
            else
                e.Handled = false;
        }
    }
}