using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using Utilities.WPF;
using System.Windows.Threading;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService;
using StringManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using System.Windows.Media;

namespace StatesChartControl.Controls
{
    /// <summary>
    /// Interaction logic for SmartControl.xaml
    /// </summary>
    public partial class Settings : UserControl, IDisposable
    {

        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(Settings), new UIPropertyMetadata(null));
        public IDocument Document
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IDocument)GetValue(DocumentProperty);
            }
            set
            {
                SetValue(DocumentProperty, value);
            }
        }
        #endregion

        #region Declarations
        StatesChartControl control;
        TagPenList datalist;
        IUFUAEditorManager UFUAEditor;
        IUIMsgBoxAlertService UIMsgBoxAlertService;
        bool bLoaded;
        bool bDirty;
        #endregion
        #region IDisposable
        public void Dispose()
        {
            datalist?.Clear();
            datalist = null;
        }
        #endregion
        #region ctor
        public Settings(StatesChartControl c)
        {
            InitializeComponent();
            control = c;
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    if (Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(control);

                    ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
                    if (Document == null || control == null)
                    {
                        IsEnabled = false;
                        return;
                    }

                    UIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

                    if (control?.TPenList != null)
                        datalist = new TagPenList(control.TPenList);
                    else
                        datalist = new TagPenList();

                    gridControl.ItemsSource = datalist;
                    var wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        wnd.Closing += (s, ce) =>
                        {
                            var dlg = wnd as GeneralDialog;
                            if (bDirty && dlg?.DialogResult != false)
                            {
                                if (UIMsgBoxAlertService?.ShowOkCancel($"{Properties.Resources.SaveChanges}", CustomDialogIcons.Question) == CustomDialogResults.OK)
                                {
                                    control.TPenList = new TagPenList(datalist);
                                    bDirty = false;
                                }
                            }
                        };
                    }
                }
            };
        }
        #endregion
        #region Methods

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            if (datalist == null)
                datalist = new TagPenList();

            int i = datalist.Count;
            var _name = string.Format("{0}{1}", Properties.Resources.PenBaseName, i);

            while ((from c in datalist select c.title).ToList().Contains(_name))
            {
                _name = string.Format("{0}{1}", Properties.Resources.PenBaseName, ++i);
            }

            var newData = new PenList() { title = _name };
            InsertRow(tableView.FocusedRowHandle, newData);
            ApplyChangesInBackground();
        }

        private void InsertRow(int rowHandle, PenList data)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex < 0 || listIndex >= datalist.Count) listIndex = -1;
            datalist.Insert(listIndex + 1, data);

            gridControl.ItemsSource = null;
            gridControl.ItemsSource = datalist;
            ApplyChangesInBackground();
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            DeleteRow(tableView.FocusedRowHandle);
        }

        private void DeleteRow(int rowHandle)
        {
            if (tableView.IsEditing)
                return;

            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);

            if (listIndex >= 0)
                datalist.RemoveAt(listIndex);

            gridControl.ItemsSource = null;
            gridControl.ItemsSource = datalist;
            ApplyChangesInBackground();
        }

        void ApplyChangesInBackground()
        {
            bDirty = true;
        }

        private void tableView_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(tableView.FocusedRowHandle);
            btnDelete.IsEnabled = listIndex >= 0;
        }

        private void DlgButton_Click_ClearName(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                datalist.ElementAt(listIndex).title = string.Empty;

                gridControl.ItemsSource = null;
                gridControl.ItemsSource = datalist;
                tableView.FocusedRowHandle = focusedrow;
                ApplyChangesInBackground();
            }
        }

        private void DlgButton_Click_Name(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                if (Document == null)
                    return;
                Button button = (Button)sender;
                String value;
                value = (String)button.Tag;

                IStringEditorManager stringEditorManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                if (stringEditorManager == null)
                    return;
                var stringEditor = stringEditorManager.GetStringEditor(Document);

                if (stringEditor == null)
                    return;

                var Dialog = new GeneralDialogContent(stringEditor)
                {
                    Owner = this.FindParent<Window>(),
                    DialogKeepContent = true,
                    Title = Properties.Resources.SelectStringEditor,
                    HelpLink = "StringEditor"
                };
                if (Dialog.ShowDialog() != true)
                    return;

                if (stringEditor.DataContext != null && (stringEditor.DataContext as String) != null)
                {
                    (datalist.ElementAt(listIndex).title) = new String((stringEditor.DataContext as String).ToArray());

                    gridControl.ItemsSource = null;
                    gridControl.ItemsSource = datalist;
                    tableView.FocusedRowHandle = focusedrow;
                    ApplyChangesInBackground();
                }

            }
        }

        private void tableView_CellValueChanged(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            ApplyChangesInBackground();
        }

        private void text_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyChangesInBackground();
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            datalist.Clear();
            gridControl.ItemsSource = null;
            gridControl.ItemsSource = datalist;
            ApplyChangesInBackground();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0 && listIndex < datalist.Count)
            {
                (datalist.ElementAt(listIndex).tagName) = string.Empty;
                (datalist.ElementAt(listIndex).historicalName) = string.Empty;
                (datalist.ElementAt(listIndex).sourcetimestampColumnName) = string.Empty;
                (datalist.ElementAt(listIndex).nodeID) = string.Empty;
                (datalist.ElementAt(listIndex).tagreferenceXml) = string.Empty;
                (datalist.ElementAt(listIndex).useTableAggregation) = false;
                (datalist.ElementAt(listIndex).minAggregation) = false;
                (datalist.ElementAt(listIndex).maxAggregation) = false;
                (datalist.ElementAt(listIndex).avgAggregation) = false;
                gridControl.ItemsSource = null;
                gridControl.ItemsSource = datalist;
                tableView.FocusedRowHandle = focusedrow;
                gridControl.RefreshRow(focusedrow);
                ApplyChangesInBackground();
            }
        }

        private void ButtonImageClear_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                (gridControl.SelectedItem as PenList).Valueitem = null;
                (gridControl.SelectedItem as PenList).Valuelist.Clear();
                (gridControl.SelectedItem as PenList).FirstBackground = Brushes.Transparent;
                gridControl.RefreshData();
                tableView.FocusedRowHandle = focusedrow;

                ApplyChangesInBackground();
            }
        }
        private void ButtonImage_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                var penList = new PenList((PenList)gridControl.SelectedItem);
                PenValuesListEditor urieditor = new PenValuesListEditor(penList, Document);
                if (urieditor == null)
                    return;

                var Dialog = new GeneralDialogContent(urieditor)
                {
                    Owner = this.FindParent<Window>(),
                    DialogKeepContent = true,
                    Title = Properties.Resources.ValuesListEditor,
                    HelpLink = Properties.Resources.PenValuesEditor
                };

                Dialog.Closing += (o, ea) =>
                {
                    if (Dialog.DialogResult == true && penList.Valuelist.CheckDuplicateItems())
                        ea.Cancel = true;
                };

                if (Dialog.ShowDialog() == true)
                {
                    var index = datalist.IndexOf((PenList)gridControl.SelectedItem);
                    if (index != -1)
                    {
                        datalist.RemoveAt(index);
                        datalist.Insert(index, penList);
                        gridControl.SelectedItem = penList;
                        ApplyChangesInBackground();
                    }
                }
            }
        }

        #endregion

        private void OnApplyChanges(object sender, EventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            gridControl.RefreshRow(focusedrow);
            ApplyChangesInBackground();
        }
    }
}