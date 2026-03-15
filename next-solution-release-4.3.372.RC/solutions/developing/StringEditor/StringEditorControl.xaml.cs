using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using StringManager.Document;
using System.Globalization;
using System.Dynamic;
using Utilities;
using Utilities.WPF;
using Utilities.UndoRedo;
using StringManager.Controls;
using System.Collections.ObjectModel;
using DevExpress.Xpo;
using System.Text;
using System.IO;
using Ookii.Dialogs.Wpf;
using UIMsgBoxAlertService.ComponentService;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Reflection;
using System.Runtime.Serialization;
using StringManager.UndoRedo;
using System.Threading.Tasks;
using WPFUtilities.ImportExportHelpers;
using WPFUtilities;
using UFProjectManager.ComponentService;
using DocumentManager.ComponentService;
using DevExpress.Xpf.Grid;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Mvvm.Native;

namespace StringManager
{
    /// <summary>
    /// Interaction logic for StringEditorControl.xaml
    /// </summary>
    public partial class StringEditorControl : UserControl, IEditableObject, IDisposable
    {
        #region Declarations
        public static string AppID = Properties.Settings.Default.TranslateAppId;

        readonly StringManager.TranslationService.LanguageServiceClient client;
        List<ExpandoObject> listData;
        NewLocale newLocale;
        bool isPopup;
        bool isPopupWasClosed;
        bool isDataContextChanging;
        bool bNeedToReload;
        bool bIsIDChanging;
        IList<String> cultures = null;
        string configurationId = string.Empty;
        string typeLabel = string.Empty;
        string projectName = string.Empty;
        string assembly = string.Empty;
        string oldID = string.Empty;

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

        public string GetSelectedLanguage()
        {
            string language = string.Empty;
            var control = new CultureSelection();
            control.listbox.ItemsSource = Document.GetListLocalCultures();

            var Dialog = new GeneralDialogContent(control);
            Dialog.HelpLink = "StringEditor_RemoveLocale";
            if (Dialog.ShowDialog() == true && control.listbox.SelectedItem != null)
                language = control.listbox.SelectedItem as String;
            return language;
        }

        public StringEditorControl(StringEditorDocument doc, bool bPopup = false, bool bAllowMultiSelection = false)
        {
            InitializeComponent();
            Document = doc;
            IDocument parent = DocumentManager.ComponentService.Helpers.DocumentHelper.GetRootParent(Document, true);
            projectName = parent.Title;
            configurationId = parent.Id.ToString();
            typeLabel = Document.Title;
            assembly =  System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location); 
            try
            {
                Document.IsAutoTranslate = LoadSettings(Properties.Settings.Default.SettingsStorageTitle);
            }
            catch (Exception)
            {

            }
            DataContext = Document;
            Document.EditorManagerComponent.menuControl.DataContext = Document;
            contextMenu.DataContext = Document;

            isPopup = bPopup;

            client = new StringManager.TranslationService.LanguageServiceClient();

            try
            {
                cultures = (from c in Document.GetListLocalCultures().AsParallel() orderby new CultureInfo(c).DisplayName ascending select c).ToList();
            }
            catch
            {
                cultures = Document.GetListLocalCultures();
            }

            foreach (var culture in cultures)
            {
                var ci = new CultureInfo(culture);
                //var column = new GridDataVisibleColumn() { HeaderText = ci.DisplayName, MappingName = culture };
                //gridDataControl.VisibleColumns.Add(column);
                gridDataControl.Columns.Add(new DevExpress.Xpf.Grid.GridColumn()
                {
                    FieldName = culture,
                    Header = ci.DisplayName,
                    AutoFilterCriteria = ClauseType.Contains,
                    CellEditTemplate = gridDataControl.Columns.First().CellEditTemplate
                });
            }

            listData = Document.GetListLocaleFlat();
            gridDataControl.ItemsSource = listData;

            if (isPopup)
            {
                Document.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == "NeedsSave")
                    {
                        if (Document.NeedsSave)
                            bNeedToReload = true;
                    }
                };

                Loaded += (o, e) =>
                {
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
                                    ReloadAddressSpace();
                                }
                            }
                        };

                        wnd.Closing += (s, c) =>
                        {
                            isPopupWasClosed = true;

                            Document.NeedToReloadAddressSpace = bNeedToReload;
                            SaveFilterSettings(Properties.Settings.Default.FilterStorageTitle);
                            var selected = gridDataControl.SelectedItem as IDictionary<String, object>;
                            if (bAllowMultiSelection && gridDataControl.SelectedItems != null && gridDataControl.SelectedItems.Count > 0)
                            {
                                isDataContextChanging = true;
                                List<string> ret = new List<string>();
                                for (int i = 0; i < gridDataControl.SelectedItems.Count; i++)
                                {
                                    var selectedItem = gridDataControl.SelectedItems[i] as IDictionary<String, object>;
                                    if(selectedItem != null)
                                        ret.Add(selectedItem[StringEditorDocument.idText] as string);
                                }

                                DataContext = ret;
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
                            else if (selected != null)
                            {
                                isDataContextChanging = true;
                                DataContext = selected[StringEditorDocument.idText];
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
                        ReloadAddressSpace();
                    }
                    else if (!bLoaded)
                    {
                        bLoaded = true;
                        InitializeAddressSpace();
                        if (Document.NeedToReloadAddressSpace || bNeedToReload)
                            ReloadAddressSpace(); 
                    }
                    Focusable = true;
                    Focus();

                };

                
            }
            else
            {
                Document.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == "NeedToReloadAddressSpace")
                    {
                        if (Document.NeedToReloadAddressSpace)
                            bNeedToReload = true;
                    }
                };
                Loaded += (o, e) =>
                {
                    if (!bLoaded)
                    {
                        bLoaded = true;
                        InitializeAddressSpace();
                        if (Document.NeedToReloadAddressSpace || bNeedToReload)
                            ReloadAddressSpace();
                    }
                };
            }

            if (cultures.Count == 0)
                AddNewLocale();
        }

        private void InitializeAddressSpace()
        {

            //LoadFilterSettings(Properties.Settings.Default.FilterStorageTitle);
            //LoadSortingOrder(Properties.Settings.Default.SortingStorageTitle);

            //gridDataControl.ItemsSourceChanged += (s, c) =>
            //{
            //    if (bDisposed)
            //        return;

            //    GetSavedFilter();
            //};

            //gridDataControl.Model.FilterChanged += (s, c) =>
            //{
            //    if (bDisposed)
            //        return;

            //    SetFilter();
            //};
        }

        internal void ReloadAddressSpace()
        {
            bNeedToReload = false;
            BeginEdit();
            ColumnsGridRefresh();
            FlatGridRefresh();
            gridDataControl.Refresh();
            EndEdit();
        }

        internal void OnActivate()
        {
            Document.EditorManagerComponent.Workspace.ContextObject = null;
            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (bNeedToReload)
                {
                    bNeedToReload = false;
                    ReloadAddressSpace();
                }
            });
        }

        #region Properties

        StringEditorDocument _Document;
        [Browsable(false)]
        public StringEditorDocument Document
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
            undoRedoDataContext.AddDataObject(new Memento(listData, Document.GetListLocalCultures()));
        }

        public void CancelEdit()
        {
            if (undoRedoDataContext != null)
                CheckUndoRedo(undoRedoDataContext);
            undoRedoDataContext = null;
        }

        public void EndEdit()
        {
            if (undoRedoDataContext != null && undoRedoDataContext.IsAnyActionAvailable)
            {
                var memento = new Memento(Document.GetListLocaleFlat(), Document.GetListLocalCultures());
                if (undoRedoDataContext.DataObjects[0] != memento)
                    undoRedoManager.AddUndoAction(undoRedoDataContext);
            }
            undoRedoDataContext = null;
        }

        #endregion

        #region Commands

        bool bCellSelected = false;
        private void gridDataControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as GridControl;
            TableViewHitInfo hi = ((TableView)grid.View).CalcHitInfo(e.OriginalSource as DependencyObject);
            if (hi.HitTest == TableViewHitTest.RowCell)
                bCellSelected = true;

            if (isPopup && bCellSelected && tableView.FocusedRowHandle >= 0)
            {
                e.Handled = true;
                var selected = gridDataControl.SelectedItem as IDictionary<String, object>;
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

        private int selectedRow = 0;
        private void gridDataControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isPopup)
            {
                var selRows = gridDataControl.GetSelectedRowHandles();
                if(selRows != null && selRows.Length > 0)
                {
                    if (selRows[0] == selectedRow)
                    {
                        bCellSelected = true;
                        gridDataControl.View.ShowEditor(true);
                    }
                    else
                    {
                        bCellSelected = false;
                        selectedRow = selRows[0];
                    }
                }
            }
        }

        private void gridDataControl_ShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            if (!bCellSelected)
                e.Cancel = true;
            else
                bCellSelected = false;
        }

        private void gridDataControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Insert)
            {
                AddNewLocaleText();
            }
            else if (e.Key == Key.F2)
            {
                bCellSelected = true;
                gridDataControl.View.ShowEditor(true);
            }

            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                if (e.Key == Key.Enter)
                {
                    e.Handled = true;

                    var obj = e.OriginalSource as TextBox;
                    if(obj != null)
                    {
                        var selStart = obj.SelectionStart;
                        obj.Text = obj.Text.Insert(selStart, "\n");
                        obj.CaretIndex = selStart + 1;
                        obj.Focus();
                    }
                }
            }
        }

        private void view_CellValueChanged(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            if(e.Value.ToString().Contains("\\n"))
            {
                e.Handled = true;

                dynamic dynObject = gridDataControl.SelectedItem as ExpandoObject;
                if (dynObject == null)
                    return;

                foreach (dynamic ld in listData)
                {
                    if (ld.ID == dynObject.ID)
                    {
                        var p = ld as IDictionary<String, object>;
                        p[e.Column.FieldName] = e.Value.ToString().Replace("\\n", System.Environment.NewLine);
                        break;
                    }
                }

                gridDataControl.ItemsSource = null;
                gridDataControl.ItemsSource = listData;

                
            }

            if(e.Column.FieldName == "ID")
            {
                bIsIDChanging = true;
                oldID = e.OldValue.ToString();
            }
            Document.NeedsSave = true;
        }

        private void gridDataControl_OnValidate(object sender, GridCellValidationEventArgs e)
        {
            var oldValue = e.CellValue;
            var newValue = e.Value;
            if (oldValue != newValue)
            {
                var value = newValue as String;
                if (String.IsNullOrEmpty(value))
                {
                    e.IsValid = false;
                    e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical;
                    e.ErrorContent = Properties.Resources.IDCannotBeEmpty;
                }
                else if (IsIDTextDefined(value, oldValue.ToString()))
                {
                    e.IsValid = false;
                    e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical;
                    e.ErrorContent = Properties.Resources.IDAlreadyExist;
                }              
            }
        }

        private void gridDataControl_HiddenEditor(object sender, EditorEventArgs args)
        {
            if (Document == null || Document.IsDisposed || bDisposed || onTranslating || onRefreshing || tableView.FocusedRowHandle < 0)
                return;

            BeginEdit();
            if (Document.IsAutoTranslate && args.Column.FieldName != colID.FieldName)
            {
                if (Document.EditorManagerComponent.UIInterface == null ||
                    Document.EditorManagerComponent.UIInterface.ShowYesNo(Properties.Resources.PromptPropagateTranslation, CustomDialogIcons.Question) == CustomDialogResults.Yes)
                    TranslateColumns();
                else
                {
                    CommitChanges(args.Row as ExpandoObject);
                }
            }
            else
            {
                CommitChanges(args.Row as ExpandoObject);
            }

            EndEdit();

            bIsIDChanging = false;
            oldID = string.Empty;
        }

        String GetCurrentLanguage()
        {
            return gridDataControl.CurrentColumn.FieldName;
        }

        String GetCurrentLanguageText()
        {
            return gridDataControl.GetFocusedValue() as String;
        }

        bool onTranslating;
        async void TranslateColumns(String from, List<String> listTo)
        {
            try
            {
                onTranslating = true;
                progressBar.Visibility = Visibility.Visible;
                gridDataControl.IsEnabled = false;

                bCancelClicked = false;
                
                dynamic selected = gridDataControl.SelectedItem as ExpandoObject;
                int index = 0;
                var counter = 0;
                foreach (var dynObject in listData)
                {
                    if (bCancelClicked)
                        break;
                    var p = dynObject as IDictionary<String, object>;

                    foreach (var column in listTo)
                    {
retry:
                        if (bCancelClicked)
                            break;
                        try
                        {
                            var text = p[from] as String;
                            await Task.Delay(Properties.Settings.Default.TranslateSleep);
                            var translated = await client.TranslateAsync(AppID, text, from, column, "text/plain", "general");

                            counter = 0;

                            if (text != translated)
                                p[column] = translated;

                        }
                        catch (Exception ex)
                        {
                            await Task.Delay(Properties.Settings.Default.TranslateSleep);
                            if (++counter < 5)
                                goto retry;
                            else
                            {
                                if (Document.EditorManagerComponent.UIInterface != null)
                                    Document.EditorManagerComponent.UIInterface.ShowError(Properties.Resources.TranslationServiuceUnavailableOrOverloaded);
                                goto exitLoop;
                            }
                        }
                    }
                    if (index < listData.Count)
                        listData[index] = dynObject;
                    index++;
                }

exitLoop:
                gridDataControl.ItemsSource = null;
                gridDataControl.ItemsSource = listData;
                gridDataControl.SelectedItem = selected;
                //gridDataControl.ShowFilterBar = listData.Count > 0;

                Document.NeedsSave = true;
            }
            catch (Exception ex)
            {
                // break;
            }
            finally
            {
                gridDataControl.IsEnabled = true;
                progressBar.Visibility = Visibility.Collapsed;
                onTranslating = false;
            }
        }

        async void TranslateColumns()
        {
            try
            {
                if (!Document.IsAutoTranslate)
                    return;

                dynamic dynObject = gridDataControl.SelectedItem as ExpandoObject;
                if (dynObject == null)
                    return;

                int index = -1;
                var listItem = listData.FirstOrDefault(x => ((dynamic)x).ID == ((dynamic)dynObject).ID);
                if (listItem != null)
                    index = listData.IndexOf(listItem);
                if(index >= 0)
                {
                    var p = dynObject as IDictionary<String, object>;

                    progressBar.Visibility = Visibility.Visible;
                    onTranslating = true;
                    bCancelClicked = false;

                    var counter = 0;
                    gridDataControl.IsEnabled = false;

                    var from = GetCurrentLanguage();
                    var text = GetCurrentLanguageText();
                    foreach (var column in gridDataControl.Columns)
                    {
                        if (column == colID)
                            continue;
                        if (column.FieldName == from)
                            continue;
                        retry:
                        if (bCancelClicked)
                            break;

                        try
                        {
                            await Task.Delay(Properties.Settings.Default.TranslateSleep);
                            var translated = await client.TranslateAsync(AppID, text, from, column.FieldName, "text/plain", "general");
                            counter = 0;
                            if (text != translated)
                                p[column.FieldName] = translated;
                        }
                        catch (Exception ex)
                        {
                            await Task.Delay(Properties.Settings.Default.TranslateSleep);
                            if (++counter < 5)
                                goto retry;
                            else
                            {
                                if (Document.EditorManagerComponent.UIInterface != null)
                                    Document.EditorManagerComponent.UIInterface.ShowError(Properties.Resources.TranslationServiuceUnavailableOrOverloaded);
                                goto exitLoop;
                            }
                        }
                    }
                }

                exitLoop:

                if(index >= 0)
                    listData[index] = dynObject;
                gridDataControl.ItemsSource = null;
                gridDataControl.ItemsSource = listData;
                gridDataControl.SelectedItem = dynObject;
                //gridDataControl.ShowFilterBar = listData.Count > 0;

                CommitChanges();
                Document.NeedsSave = true;
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                gridDataControl.IsEnabled = true;
                progressBar.Visibility = Visibility.Collapsed;
                onTranslating = false;
            }
        }

        public void CommitChanges(ExpandoObject obj = null)
        {
            //gridDataControl.Model.CurrencyManager.EndEdit();

            var listLocales = new List<String>();
            foreach (var column in gridDataControl.Columns)
            {
                if (column == colID)
                    continue;
                listLocales.Add(column.FieldName);
            }            

            listData = gridDataControl.ItemsSource as List<ExpandoObject>;
            //gridDataControl.ItemsSource = null;
            if (obj == null)
                Document.UpdateLocaleData(listData, listLocales);
            else
                Document.UpdateSingleLocaleData(obj, listLocales, bIsIDChanging, oldID);
            listData = Document.GetListLocaleFlat();
            //gridDataControl.ItemsSource = listData;
            Document.NeedsSave = true;
        }

        private void OnCommandSave(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (var cursor = new WaitCursor())
            {
                //CommitChanges();
                Document.SaveToFile();
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
        private void OnAddNewLocale(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            AddNewLocale();
        }

        private void AddNewLocale()
        {
            BeginEdit();

            if (newLocale == null)
                newLocale = new NewLocale(client, AppID);
            GeneralDialogContent Dialog = new GeneralDialogContent(newLocale)
            {
                Owner = this.FindParent<Window>(),
                DialogKeepContent = true,
                Title = Properties.Resources.AddNewCulture,
                HelpLink = "AddNewLocale"
            };

            Cursor oldCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = null;
            bool bWasBusy = Document.EditorManagerComponent.Workspace.IsBusy;
            if (bWasBusy) 
                Document.EditorManagerComponent.Workspace.IsBusy = false;

            if (Dialog.ShowDialog() == true)
            {
                var culture = newLocale.GetSelected();
                if (culture != null)
                {
                    if (Document.AddLocale(culture.Name) != null)
                    {
                        //var column = new GridDataVisibleColumn() { HeaderText = culture.DisplayName, MappingName = culture.Name };
                        //gridDataControl.VisibleColumns.Add(column);
                        gridDataControl.Columns.Add(new DevExpress.Xpf.Grid.GridColumn()
                        {
                            FieldName = culture.Name,
                            Header = culture.DisplayName,
                            AutoFilterCriteria = ClauseType.Contains
                        });

                        Document.NeedsSave = true;

                        EndEdit();

                        FlatGridRefresh();
                    }
                    else
                        if (Document.EditorManagerComponent.UIInterface != null)
                            Document.EditorManagerComponent.UIInterface.ShowError(string.Format(Properties.Resources.CultureAlreadyInList, culture.Name));
                }
            }
            Mouse.OverrideCursor = oldCursor;
            Document.EditorManagerComponent.Workspace.IsBusy = bWasBusy;
        }

        private void CanAddNewLocale(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnAddNewLocaleText(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            AddNewLocaleText();
        }

        bool IsIDTextDefined(String id, String currentEditingID = null)
        {
            var listFound = (from c in listData.OfType<IDictionary<String, Object>>().AsParallel()
                             where c.ContainsKey(StringEditorDocument.idText) && c[StringEditorDocument.idText] as String == id
                             && (String.IsNullOrEmpty(currentEditingID) || currentEditingID != id)
                             select c).ToList();
            return listFound.Count > 0;
        }

        String CreateNewStringId(String s)
        {
            int i = 0;
            var id = s;
            while (IsIDTextDefined(id))
                id = String.Format("{0}{1}", Properties.Resources.NewTextID, ++i);
            return id;
        }

        private void AddNewLocaleText()
        {
            BeginEdit();

            string id = CreateNewStringId(Properties.Resources.NewTextID);
            var llt = Document.AddLocaleText(id);

            listData = Document.GetListLocaleFlat();
            gridDataControl.Tag = null;
            foreach (var vc in gridDataControl.Columns)
            {
                gridDataControl.ClearColumnFilter(vc);
            }
            gridDataControl.ItemsSource = null;
            gridDataControl.ItemsSource = listData;
            gridDataControl.SelectedItem = gridDataControl.CurrentItem = listData.Find((o) => {
                var p = o as IDictionary<String, object>;
                if (p != null)
                {
                    if ((p[StringEditorDocument.idText] as String) == id)
                        return true;
                }
                return false;
            });

            Document.NeedsSave = true;

            EndEdit();
        }

        internal void AddNewLocalIdList(IList<String> list)
        {
            if (list == null || list.Count == 0)
                return;

            var selection = new ObservableCollection<object>();

            list.ToList().ForEach(s =>
                {
                    if (string.IsNullOrEmpty(s))
                        return;
                    var listFound = (from c in listData.OfType<IDictionary<String, Object>>().AsParallel()
                     where c.ContainsKey(StringEditorDocument.idText) && c[StringEditorDocument.idText] as String == s
                     select c).ToList();
                    if (listFound.Count == 0)
                    {
                        var llt = Document.AddLocaleText(s);
                    }
                });

            listData = Document.GetListLocaleFlat();
            gridDataControl.Tag = null;
            foreach (var vc in gridDataControl.Columns)
            {
                gridDataControl.ClearColumnFilter(vc);
            }
            gridDataControl.ItemsSource = null;
            gridDataControl.ItemsSource = listData;
            gridDataControl.SelectedItem = listData.Find((o) => {
                var p = o as IDictionary<String, object>;
                if (p != null)
                {
                    if ((p[StringEditorDocument.idText] as String) == list.FirstOrDefault())
                        return true;
                }
                return false;
            });

            Document.NeedsSave = true;
        }

        private void CanAddNewLocaleText(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #region Methods
        bool IsAnyItemSelected()
        {
            return gridDataControl.SelectedItem is IDictionary<string, object>;
        }

        void PasteFromClipboard()
        {
            if (Clipboard.ContainsText())
            {
                try
                {
                    string clip = Clipboard.GetText();
                    string[] lines = clip.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > 0)
                    {
                        string[] mappingnames = null;
                        List<object> addlist = new List<object>();
                        for (int k = 0; k < lines.Length; k++)
                        {
                            string[] elems = lines[k].Split(new string[1] { Utilities.ImportExportUtils.ImportExport.Delimiter }, StringSplitOptions.None);
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

        void PasteListLocaleText(List<object> addlist, string [] mappingnames, bool update = true)
        {
            BeginEdit();

            var lisths = Document.PasteListLocaleText(addlist, mappingnames, update);
            if (lisths.Count > 0)
            {
                var idList = (from localeText in lisths select localeText.Text).Distinct().ToList();
                listData = Document.GetListLocaleFlat();

                gridDataControl.Tag = null;
                foreach (var vc in gridDataControl.Columns)
                {
                    gridDataControl.ClearColumnFilter(vc);
                }
                gridDataControl.ItemsSource = null;
                gridDataControl.ItemsSource = listData;
                List<object> items = new List<object>();
                idList.ForEach(id =>
                {
                    object obj = listData.Find((o) =>
                    {
                        var p = o as IDictionary<String, object>;
                        if (p != null)
                        {
                            if ((p[StringEditorDocument.idText] as String) == id)
                                return true;
                        }
                        return false;
                    });
                    if (obj != null)
                        items.Add(obj);
                });

                if (items.Count > 0)
                {
                    if (items.Count > 1)
                        gridDataControl.SelectedItems = items;
                    else
                        gridDataControl.SelectedItem = items[0];
                    gridDataControl.CurrentItem = items[0];
                }

                Document.NeedsSave = true;

                EndEdit();
            }
        }

        void gridDataControl_OnCopyingToClipboard(object sender, CopyingToClipboardEventArgs e)
        {
            if (tableView.FocusedRowHandle >= 0)
                CopySelectedToClipboard();
            e.Handled = true;
        }

        void gridDataControl_OnPastingFromClipboard(object sender, PastingFromClipboardEventArgs e)
        {
            if (tableView.IsEditing)
                return;

            e.Handled = true;
            PasteFromClipboard(); 
        }

        void CopySelectedToClipboard()
        {
            StringBuilder clip = new StringBuilder();

            if (gridDataControl.SelectedItems.Count == 0)
                return;
            //first row with culture references
            for (int j = 0; j < gridDataControl.Columns.Count; j++)
            {
                clip.Append(gridDataControl.Columns[j].FieldName);
                if (j != gridDataControl.Columns.Count - 1)
                    clip.Append(Utilities.ImportExportUtils.ImportExport.Delimiter);
            }

            foreach (var item in gridDataControl.SelectedItems)
            {
                var cols = item as IDictionary<string, object>;
                if (cols != null)
                {
                    clip.Append(Environment.NewLine);
                    for (int i = 0; i < gridDataControl.Columns.Count; i++)
                    {
                        if (cols.ContainsKey(gridDataControl.Columns[i].FieldName))
                            clip.Append(cols[gridDataControl.Columns[i].FieldName] as string);
                        if (i != gridDataControl.Columns.Count - 1)
                            clip.Append(Utilities.ImportExportUtils.ImportExport.Delimiter);
                    }
                }
            }
            Clipboard.Clear();
            Clipboard.SetText(clip.ToString());

        }

        void DeleteSelectedItems()
        {
            if (gridDataControl.SelectedItems.Count == 0 || tableView.FocusedRowHandle < 0)
                return;

            BeginEdit();

            var items = new List<StringModel.UFStringLocaleText>();
            foreach (var item in gridDataControl.SelectedItems)
            {
                var cols = item as IDictionary<string, object>;
                if (cols != null)
                {
                    string id = cols[StringEditorDocument.idText] as string;
                    var idtext = Document.GetLocaleTextFromId(id);
                    if (idtext.Count > 0)
                        items.AddRange(idtext);
                }
            }

            foreach (var ltext in items)
            {
                var slocale = ltext.UFStringLocale;
                if (slocale != null)
                {
                    var idx = slocale.UFStringLocaleTexts.IndexOf(ltext);
                    if (idx != -1)
                        slocale.UFStringLocaleTexts[idx].Delete();
                }
                ltext.Delete();
            }

            items.Clear();
            FlatGridRefresh();
            CommitChanges();

            EndEdit();
        }

        bool onRefreshing;
        void ColumnsGridRefresh()
        {
            try
            {
                onRefreshing = true;
                var cultures = Document.GetListLocalCultures();
                ColumnsGridRefresh(cultures);
            }
            finally
            {
                onRefreshing = false;
            }
        }

        void ColumnsGridRefresh(IList<String> cultures)
        {
            var columnWidths = new Dictionary<String, double>();

            int i = 0;
            while (gridDataControl.Columns.Count > 1)
            {
                if (!columnWidths.ContainsKey(gridDataControl.Columns[i].FieldName))
                    columnWidths.Add(gridDataControl.Columns[i].FieldName, gridDataControl.Columns[i].ActualWidth);

                if (gridDataControl.Columns[i].FieldName == StringEditorDocument.idText)
                    i++;
                else
                    gridDataControl.Columns.RemoveAt(i);
            }

            foreach (var culture in cultures)
            {
                var ci = new CultureInfo(culture);
                //var column = new GridDataVisibleColumn() { HeaderText = ci.DisplayName, MappingName = culture };
                var column = new DevExpress.Xpf.Grid.GridColumn() { Header = ci.DisplayName, FieldName = culture, AutoFilterCriteria = ClauseType.Contains };
                if (columnWidths.ContainsKey(culture))
                    column.Width = new DevExpress.Xpf.Grid.GridColumnWidth(columnWidths[culture]);
                gridDataControl.Columns.Add(column);
            }
        }

        void FlatGridRefresh()
        {
            //var list = new List<GridDataGroupColumn>();
            //foreach (var group in gridDataControl.GroupedColumns)
            //    list.Add(new GridDataGroupColumn() { ColumnName = group.ColumnName });

            //while (gridDataControl.GroupedColumns.Count > 0)
            //    gridDataControl.GroupedColumns.Remove(gridDataControl.GroupedColumns[0]);

            var selected = gridDataControl.SelectedItem;
            listData = Document.GetListLocaleFlat();
            gridDataControl.ItemsSource = null;
            gridDataControl.ItemsSource = listData;
            if (listData.Contains(selected))
                gridDataControl.SelectedItem = selected;

            //foreach (var group in list)
            //{
            //    gridDataControl.GroupedColumns.Add(new GridDataGroupColumn() { ColumnName = group.ColumnName });
            //}
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
            return obj is StringModel.UFStringLocaleText;
        }

        void UndoAction()
        {
            var dataObject = undoRedoManager.Undo();
            if (dataObject == null || !dataObject.IsAnyActionAvailable)
                return;

            if (dataObject.UndoRedoAction == UndoRedoAction.Snapshot)
            {
                var action = new UndoRedoDataObject<Memento>(UndoRedoAction.Snapshot);
                action.AddDataObject(new Memento(listData, Document.GetListLocalCultures()));
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
                action.AddDataObject(new Memento(listData, Document.GetListLocalCultures()));
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
                    
                    Document.UpdateLocaleData(dataObject.DataObjects[0].Texts, dataObject.DataObjects[0].Locales);

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
                SaveSettings(Properties.Settings.Default.SettingsStorageTitle, Document.IsAutoTranslate);
                SaveFilterSettings(Properties.Settings.Default.FilterStorageTitle);
                //SaveSortingOrder(Properties.Settings.Default.SortingStorageTitle);
            }
            catch (Exception)
            {
                
            }
           
            try
            {
                client.Abort();
            }
            catch (Exception ex)
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

        private void OnEnableAutoTranslate(object sender, ExecutedRoutedEventArgs e)
        {
            Document.IsAutoTranslate = !Document.IsAutoTranslate;
        }

        private void CanEnableAutoTranslate(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnTranslateColumns(object sender, ExecutedRoutedEventArgs e)
        {
            BeginEdit();

            var listLocales = new List<String>();
            foreach (var column in gridDataControl.Columns)
            {
                if (column == colID)
                    continue;
                listLocales.Add(column.FieldName);
            }

            var selectTranslate = new SelectTranslate(listLocales);
            while (true)
            {
                var Dialog = new GeneralDialogContent(selectTranslate)
                {
                    Owner = this.FindParent<Window>(),
                    DialogKeepContent = true,
                    Title = Properties.Resources.TranslateColumns,
                    HelpLink = "StringEditor_TranslateColumns"
                };
                if (Dialog.ShowDialog() == true)
                {
                    var from = selectTranslate.SelectedFrom;
                    var to = selectTranslate.SelectedTo;
                    if (to.Contains(from))
                        to.Remove(from);
                    if (to.Count == 0)
                    {
                        if (Document.EditorManagerComponent.UIInterface != null)
                            Document.EditorManagerComponent.UIInterface.ShowError(Properties.Resources.NeedToSelectTargetLocale);
                    }
                    else
                    {
                        TranslateColumns(from, to);
                        CommitChanges();
                        EndEdit();
                        break;
                    }
                }
                else
                    break;
            }
        }

        private void CanTranslateColumns(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!IsLoaded || Document == null || Document.IsDisposed || bDisposed)
                return;
            e.CanExecute = gridDataControl.Columns.Count > 2 && !onTranslating;
        }

        private void OnImportStrings(object sender, ExecutedRoutedEventArgs e)
        {

            var control = new FileSelector();
            List<string> lista = new List<string>();
            control.DataContext = lista;

            GeneralDialogContent dialog = new GeneralDialogContent(control)
            {
                Title = Properties.Resources.SelectFile,
                Owner = Application.Current.Windows.Count > 0 ? Application.Current.Windows[0] : Application.Current.MainWindow,
                HelpLink = Properties.Settings.Default.ImportExportFileHelpLink
            };

            var result = (bool)dialog.ShowDialog();
            if(result)
            {
                var filename = control.FilePath;
                try
                {
                    //using (new WaitCursor())
                    {
                        //using (StreamReader readFile = new StreamReader(filename))
                        {
                            List<object> addlist = new List<object>();
                            string[] mappingnames = null;
                            string[] colnames = null;
                            Utilities.ImportExportUtils.ImportExport.ImportFromFile(filename, addlist, ref colnames, ImportExportHelper<string>.Separators[control.Separator].ToString());
                            if (colnames.Length == 0)
                            {
                                Document.LogGeneralError(string.Format(Properties.Resources.ImportErrorFileNoCols, filename), null);
                                if (Document.EditorManagerComponent.UIInterface != null)
                                    Document.EditorManagerComponent.UIInterface.ShowInformation(string.Format(Properties.Resources.ImportErrorFileNoCols, filename));
                                return;
                            }

                            StringSelector Choose = new StringSelector(colnames, addlist);
                            GeneralDialogContent Dialog = new GeneralDialogContent(Choose)
                            {
                                Owner = this.FindParent<Window>(),
                                HelpLink = "StringEditor_OnImportStrings"
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

                            mappingnames = new string[colnames.Length];
                            mappingnames[0] = colnames[0];
                            var cultures = Document.GetListLocalCultures();
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
                                        //var column = new GridDataVisibleColumn() { HeaderText = culture.DisplayName, MappingName = culture.Name };
                                        var column = new DevExpress.Xpf.Grid.GridColumn() { Header = culture.DisplayName, FieldName = culture.Name, AutoFilterCriteria = ClauseType.Contains };
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
                                PasteListLocaleText(addlist, mappingnames);
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
        }


        private void CanImportStrings(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnExportStrings(object sender, ExecutedRoutedEventArgs e)
        {

            var control = new FileSelector(true);
            List<string> lista = new List<string>();
            control.DataContext = lista;

            GeneralDialogContent dialog = new GeneralDialogContent(control)
            {
                Title = Properties.Resources.SelectFile,
                Owner = Application.Current.Windows.Count > 0 ? Application.Current.Windows[0] : Application.Current.MainWindow,
                HelpLink = Properties.Settings.Default.ImportExportFileHelpLink
            };

            var result = (bool)dialog.ShowDialog();
            if (result)
            {
                var filename = control.FilePath;
                var uIMsgBoxAlertService = Document.EditorManagerComponent.UIInterface;
                if (System.IO.File.Exists(filename) && uIMsgBoxAlertService != null)
                {
                    var res = uIMsgBoxAlertService.ShowYesNoCancel(Properties.Resources.ExportFileWarning, CustomDialogIcons.Question);
                    if (res == CustomDialogResults.Cancel)
                        return;
                    else if (res == CustomDialogResults.No)
                    {
                        int i = 1;
                        while (System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename), string.Format("{0}{1}{2}", System.IO.Path.GetFileNameWithoutExtension(filename), i.ToString(), System.IO.Path.GetExtension(filename).ToLower()))))
                        {
                            i++;
                        }
                        filename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename), string.Format("{0}{1}{2}", System.IO.Path.GetFileNameWithoutExtension(filename), i.ToString(), System.IO.Path.GetExtension(filename).ToLower()));
                    }
                }

                var separator = ImportExportHelper<string>.Separators[control.Separator].ToString();
                StringBuilder clip = new StringBuilder();
                var list = new List<object>();
                bool bSelected = control.ExportOption == Converters.ExportOption.Selected;
                if (gridDataControl.SelectedItems.Count == 0 && bSelected)
                    return;

                for (int i = 0; i < gridDataControl.VisibleRowCount; i++)
                {
                    int rowHandle = gridDataControl.GetRowHandleByVisibleIndex(i);
                    ExpandoObject item = gridDataControl.GetRow(rowHandle) as ExpandoObject;
                    if (!bSelected || bSelected && gridDataControl.SelectedItems.Contains(item))
                        list.Add(item);
                }

                if (list.Count == 0)
                    return;

                for (int i = 0; i < gridDataControl.Columns.Count; i++)
                {
                    clip.Append(gridDataControl.Columns[i].FieldName as string);
                    if (i != gridDataControl.Columns.Count - 1)
                        clip.Append(separator);
                }

                list.ForEach(s =>
                    {
                        var row = s as IDictionary<string, object>;
                        if (row != null)
                        {
                            clip.Append(Environment.NewLine);
                            for (int i = 0; i < gridDataControl.Columns.Count; i++)
                            {
                                if(row.ContainsKey(gridDataControl.Columns[i].FieldName))
                                    clip.Append(row[gridDataControl.Columns[i].FieldName] as string);
                                if (i != gridDataControl.Columns.Count - 1)
                                    clip.Append(separator);
                            }
                        }
                    });
                var exportstring = clip.ToString();
                if (exportstring.Length > 0)
                {
                    try
                    {
                        using (StreamWriter s = new StreamWriter(filename, false, Encoding.Unicode) /*File.CreateText(filename)*/)
                        {
                            s.Write(exportstring);
                            if(uIMsgBoxAlertService != null)
                                uIMsgBoxAlertService.ShowInformation(string.Format(string.Format(Properties.Resources.ExportActionSuccessfully, filename)));
                        }
                    }
                    catch (Exception ex)
                    {
                        //log/signal error!! WriteToEventLog(BaseFileName, Message, EventLogEntryType.FailureAudit);
                        Document.LogGeneralError(string.Format(Properties.Resources.ExportError, filename, ex.Message), ex);
                        if (Document.EditorManagerComponent.UIInterface != null)
                            Document.EditorManagerComponent.UIInterface.ShowInformation(string.Format(Properties.Resources.ExportError, filename, ex.Message));
                    }
                }
            }
        }

        private void CanExportStrings(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnRemoveLocale(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            BeginEdit();

            var col = GetSelectedLanguage();
            if (col.Length > 0)
            {
                Document.RemoveLocale(col);

                //var colToUngroup = (from c in gridDataControl.GroupedColumns where c.ColumnName == col select c).FirstOrDefault();
                //if (colToUngroup != null)
                //    gridDataControl.GroupedColumns.Remove(colToUngroup);
                ColumnsGridRefresh();
                FlatGridRefresh();
                gridDataControl.Refresh();

                EndEdit();
            }
        }

        private void CanRemoveLocale(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #region Storage

        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch { }

            return null;
        }

        String GetStoreFileName(String title)
        {
            return String.Format("{0}.{1}.{2}.{3}.{4}.dat", assembly, configurationId, projectName, title, typeLabel);
        }

        void SaveFilterSettings(string title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage)
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.Create, isoStorage))
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        try
                        {
                            var serializer = new DataContractSerializer(typeof(String));
                            serializer.WriteObject(writer, gridDataControl.Tag.ToXml());
                        }
                        catch (Exception ex)
                        {
                            writer.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        void SaveSettings(String title, bool isAutotranslateEnabled)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.Create, isoStorage))
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        try
                        {
                            var serializer = new DataContractSerializer(typeof(String));
                            serializer.WriteObject(writer, Convert.ToString(isAutotranslateEnabled));
                        }
                        catch (Exception ex)
                        {
                            writer.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        
        bool LoadSettings(String title)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return true;

            try
            {
                var isoStorage = GetStorage();
                if (null != isoStorage && !String.IsNullOrEmpty(title))
                {
                    using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.OpenOrCreate, isoStorage))
                    {
                        XmlReaderSettings settings = new XmlReaderSettings
                        {
                            ConformanceLevel = ConformanceLevel.Document,
                            CloseInput = true
                        };

                        using (XmlReader reader = XmlReader.Create(stream, settings))
                        {
                            var serializer = new DataContractSerializer(typeof(String));
                            return Convert.ToString(serializer.ReadObject(reader) as string).Equals(Boolean.TrueString);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return true;
        }
        //void SaveSortingOrder(String title)
        //{
        //    try
        //    {
        //        var isoStorage = GetStorage();
        //        if (null == isoStorage || string.IsNullOrEmpty(title))
        //            return;

        //        using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.Create, isoStorage))
        //        {
        //            XmlWriterSettings settings = new XmlWriterSettings
        //            {
        //                Indent = true,
        //                OmitXmlDeclaration = false,
        //                Encoding = Encoding.UTF8
        //            };

        //            using (XmlWriter writer = XmlWriter.Create(stream, settings))
        //            {
        //                try
        //                {
        //                    var serializer = new DataContractSerializer(typeof(List<string>));
        //                    List<string> sort = new List<string>();
                            
        //                    foreach(var col in gridDataControl.SortColumns)
        //                    {
        //                        sort.Add($"{col.ColumnName}@{col.SortDirection}");
        //                    }
        //                    serializer.WriteObject(writer, sort);
        //                }
        //                catch (Exception ex)
        //                {
        //                    writer.Close();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        //void LoadSortingOrder(String title)
        //{
        //    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        //        return;

        //    try
        //    {
        //        var isoStorage = GetStorage();
        //        if (null != isoStorage && !String.IsNullOrEmpty(title))
        //        {
        //            using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.OpenOrCreate, isoStorage))
        //            {
        //                XmlReaderSettings settings = new XmlReaderSettings
        //                {
        //                    ConformanceLevel = ConformanceLevel.Document,
        //                    CloseInput = true
        //                };

        //                using (XmlReader reader = XmlReader.Create(stream, settings))
        //                {
        //                    var serializer = new DataContractSerializer(typeof(List<string>));
        //                    try
        //                    {
        //                        List<string> sort = serializer.ReadObject(reader) as List<string>;
        //                        List<string> listNames = new List<string>();
        //                        foreach (var c in gridDataControl.Columns)
        //                            listNames.Add(c.FieldName);

        //                        foreach (var order in sort)
        //                        {
        //                            string colname = order.Split('@')[0] as string;
        //                            string direction = order.Split('@')[1] as string;
        //                            if(listNames.Contains(colname))
        //                            {
        //                                var sortCol = new GridDataSortColumn() { ColumnName = colname, SortDirection = (ListSortDirection)Enum.Parse(typeof(ListSortDirection), direction) };
        //                                gridDataControl.SortColumns.Add(sortCol);
        //                            }
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return;
        //}
        #endregion

        bool bCancelClicked;
        private void CancelClick(object sender, RoutedEventArgs e)
        {
            bCancelClicked = true;
        }

        private void OnColumnSort(object sender, CustomColumnSortEventArgs e)
        {
            if (e.Column == colID)
            {
                try
                {
                    //e.Result = NaturalSorting.Compare(e.Value1 as string, e.Value2 as string);
                    e.Result = WindowsNaturalSorting.Compare(e.Value1 as string, e.Value2 as string);
                    e.Handled = true;
                }
                catch { }
            }
        }

        private void SelectAllText(object sender, MouseButtonEventArgs e)
        {
            SelectText(sender as TextBox);
        }

        private void SelectAllText(object sender, KeyboardFocusChangedEventArgs e)
        {
            SelectText(sender as TextBox);
        }

        void SelectText(TextBox tb)
        {
            if (tb != null)
                tb.SelectAll();
        }

        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                }
            }
        }


    }
}
