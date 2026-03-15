using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utilities;
using Utilities.WPF;
using OPCUAViewModel;
using OPCUAViewModelService.ComponentService;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService;

namespace SpreadSheet.Controls
{
    /// <summary>
    /// Interaction logic for SmartControl.xaml
    /// </summary>
    public partial class SmartControl : UserControl
    {
        #region Declarations
        readonly SpreadSheet parent;
        SafeObservableCollection<SpreadSheetItem> datalist;
        Window parentWindow;

        IDocument document;
        IUFUAEditorManager ufuaEditor;

        bool bLoaded;
        #endregion

        #region Constructors
        public SmartControl(SpreadSheet parent)
        {
            InitializeComponent();

            this.parent = parent;
            DataContext = parent;
            tableView.KeyUp += view_KeyUp;

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                parentWindow = this.FindParent<Window>();
                if (parentWindow != null)
                {
                    parentWindow.Closing += (s, ev) =>
                    {
                        ApplyChanges();
                    };
                }

                datalist = new SafeObservableCollection<SpreadSheetItem>(new SpreadSheetDataList(parent.SpreadSheetDataList));
                gridControl.ItemsSource = datalist;
            };
        }
        #endregion

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                var value = (OPCUAEntityReference)(datalist.ElementAt(listIndex).TagReference);
                if (value == null)
                    value = new OPCUAEntityReference(null);

                if (value.Edit())
                {
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        (datalist.ElementAt(listIndex).TagReference) = value;
                        tableView.FocusedRowHandle = focusedrow;
                        ApplyChanges();
                    });
                }
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                (datalist.ElementAt(listIndex).TagReference) = null;
                tableView.FocusedRowHandle = focusedrow;
                ApplyChanges();
            }
        }

        void view_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                OnDelete(null, null);
            }
            if (e.Key == Key.Insert)
            {
                OnAdd(null, null);
            }
        }
        private void DeleteRow(int rowHandle)
        {
            if (tableView.IsEditing)
                return;

            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex >= 0)
                datalist.RemoveAt(listIndex);
            ApplyChanges();
        }

        private void InsertRow(int rowHandle, SpreadSheetItem data)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex < 0 || listIndex >= datalist.Count) listIndex = -1;
            datalist.Insert(listIndex + 1, data);
            ApplyChanges();
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            //var c = Color.FromRgb((byte)random.Next(255),
            //                                (byte)random.Next(255),
            //                                (byte)random.Next(255));

            var newData = new SpreadSheetItem();
            InsertRow(tableView.FocusedRowHandle, newData);
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            DeleteRow(tableView.FocusedRowHandle);
        }

        private void tableView_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(tableView.FocusedRowHandle);
            btnDelete.IsEnabled = listIndex >= 0;
        }

        private void tableView_CellValueChanged(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            ApplyChanges();
        }

        private void tableView_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "RowData.Row.CellName" || e.Column.FieldName == "RowData.Row.Sheet")
            {
                int focusedrow = tableView.FocusedRowHandle;
                int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
                if (listIndex >= 0 && listIndex < datalist.Count)
                {
                    if (e.Column.FieldName == "RowData.Row.Sheet")
                    {
                        int cellValue;
                        bool isint = int.TryParse(e.Value?.ToString(), out cellValue);
                        if (isint)
                        {
                            if (e.Column.FieldName == "RowData.Row.Sheet")
                                (datalist.ElementAt(listIndex).Sheet) = cellValue;
                        }
                    }
                    else if (e.Column.FieldName == "RowData.Row.CellName")
                        (datalist.ElementAt(listIndex).CellName) = e.Value == null ? null : e.Value.ToString();
                }
                ApplyChanges();
            }
        }

        private async void uriLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0 && datalist.Count > listIndex)
            {
                DocumentManager.ComponentService.IDocument doc = OPCUAViewModelComponent.workspaceService.ContextDocument as DocumentManager.ComponentService.IDocument;
                var uriLabel = (sender as DevExpress.Xpf.Editors.TextEdit);
                if (uriLabel == null || uriLabel.Text == null || !bEditing || doc == null)
                {
                    bEditing = false;
                    if (!String.IsNullOrEmpty(uriLabel.Tag.ToString()))
                    {
                        uriLabel.Text = uriLabel.Tag.ToString();
                        uriLabel.Tag = string.Empty;
                    }
                    return;
                }

                bEditing = false;

                var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                if (editor == null)
                    return;

                var split = uriLabel.Text.Split(':');
                var instance = split[0];
                var name = split[0];
                if (split.Length > 1)
                    name = split[1];
                else
                    instance = null;

                var xml = editor.GetTagEntityReference(doc, name, instance);
                if (String.IsNullOrEmpty(xml))
                {
                    var oldColor = uriLabel.Foreground;
                    uriLabel.Foreground = Brushes.Red;
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    uriLabel.Foreground = oldColor;
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    uriLabel.Foreground = Brushes.Red;
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    uriLabel.Foreground = oldColor;

                    if (!String.IsNullOrEmpty(uriLabel.Tag.ToString()))
                    {
                        uriLabel.Text = uriLabel.Tag.ToString();
                        uriLabel.Tag = string.Empty;
                    }
                }
                else
                {
                    var tag = xml.FromXml<OPCUAEntityReference>();

                    if (document == null)
                        document = ScreenSettings.ScreenDocument.GetScreenDocument(parent);

                    if (ufuaEditor == null && document != null)
                        ufuaEditor = document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

                    datalist.ElementAt(listIndex).TagReference = tag;
                    uriLabel.Tag = string.Empty;
                    uriLabel.Text = tag.HumanReadable;
                    tableView.FocusedRowHandle = focusedrow;
                    gridControl.RefreshRow(focusedrow);
                    ApplyChanges();
                }
            }
        }

        bool bEditing;
        private void uriLabel_KeyDown(object sender, KeyEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0 && datalist.Count > listIndex)
            {
                bEditing = true;
            }
        }

        private void uriLabel_GotFocus(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0 && datalist.Count > listIndex)
            {
                var uriLabel = (sender as DevExpress.Xpf.Editors.TextEdit);
                OPCUAEntityReference value = (OPCUAEntityReference)datalist.ElementAt(listIndex).TagReference;
                if (value != null)
                {
                    if (String.IsNullOrEmpty(uriLabel.Tag.ToString()))
                    {
                        uriLabel.Tag = uriLabel.Text;
                        uriLabel.Text = value.HumanReadableNoProject;
                    }
                }

                uriLabel.SelectAll();
            }
        }

        void ApplyChanges()
        {
            if (parentWindow == null || parentWindow.DialogResult == true)
            {
                parent.SpreadSheetDataList = new SpreadSheetDataList(datalist);
            }
        }
    }
}
