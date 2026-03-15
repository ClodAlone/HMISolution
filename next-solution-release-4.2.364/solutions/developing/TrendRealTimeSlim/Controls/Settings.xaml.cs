using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using DevExpress.Xpf.Editors;
using OPCUAViewModel;
using OPCUAViewModelService.ComponentService;
using Utilities;
using Utilities.WPF;
using System.Windows.Threading;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService;
using StringManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using WPFUtilities;
using DataLoggerColumnListControl;
using WPFPenHelpers;

namespace TrendRealTimeSlim.Controls
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
        RealTimeTrendControl Control;
        PenItemList PenList;
        IUIMsgBoxAlertService UIMsgBoxAlertService;
        bool bLoaded;
        bool bDirty;
        #endregion
        #region IDisposable
        public void Dispose()
        {
            PenList?.Clear();
            PenList = null;
        }
        #endregion
        #region ctor
        public Settings(RealTimeTrendControl control)
        {
            InitializeComponent();
            Control = control;

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    if (Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(Control);

                    if (Document == null || control == null)
                    {
                        this.IsEnabled = false;
                        return;
                    }

                    UIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

                    if (control?.PenList != null)
                        PenList = new PenItemList(control.PenList);
                    else
                        PenList = new PenItemList();
                    gridControl.ItemsSource = PenList;

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
                                    Control.PenList = new PenItemList(PenList);
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

        private void DlgButton_Click_ClearName(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                (PenList.ElementAt(listIndex).title) = string.Empty;

                gridControl.ItemsSource = null;
                gridControl.ItemsSource = PenList;
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
                Button button = (Button)sender;

                String value;
                value = (String)button.Tag;

                IDocument Document = ScreenSettings.ScreenDocument.GetScreenDocument(Control);
                if (Document == null)
                    return;
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
                    HelpLink = "StringSelector"
                };
                if (Dialog.ShowDialog() != true)
                    return;

                if (stringEditor.DataContext != null && (stringEditor.DataContext as String) != null)
                {
                    (PenList.ElementAt(listIndex).title) = new String((stringEditor.DataContext as String).ToArray());

                    gridControl.ItemsSource = null;
                    gridControl.ItemsSource = PenList;
                    tableView.FocusedRowHandle = focusedrow;
                    ApplyChangesInBackground();
                }
            }
        }

        List<ComboBoxEdit> listLineFilled = new List<ComboBoxEdit>();
        List<String> lines = new List<String>()
        {
            Properties.Resources.LineSerie,
            Properties.Resources.LineStepTitle,
            Properties.Resources.LineStackedSerieTitle,
            Properties.Resources.LineFullStackedTitle,
            Properties.Resources.AreaSerieTitle,
            Properties.Resources.AreaStepSerieTitle,
            Properties.Resources.AreaStackedSerieTitle,
            Properties.Resources.AreaFullStackedSerieTitle,
            Properties.Resources.BarSideSerieTitle
        };

        private void LINE_Editor_DropDownOpened(object sender, RoutedEventArgs e)
        {
            var combo = sender as ComboBoxEdit;
            if (combo == null || listLineFilled.Contains(combo))
                return;
            listLineFilled.Add(combo);
            combo.ItemsSource = lines;
        }

        private void DeleteRow(int rowHandle)
        {
            if (tableView.IsEditing)
                return;

            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex >= 0)
                PenList.RemoveAt(listIndex);

            gridControl.ItemsSource = null;
            gridControl.ItemsSource = PenList;
            ApplyChangesInBackground();
        }

        private void InsertRow(int rowHandle, PenItem data)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex < 0 || listIndex >= PenList.Count) listIndex = -1;
            PenList.Insert(listIndex + 1, data);

            gridControl.ItemsSource = null;
            gridControl.ItemsSource = PenList;
            ApplyChangesInBackground();
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            int i = PenList.Count;
            var _name = string.Format("{0}{1}", Properties.Resources.PenBaseName, i);

            var list = (from s in PenList select s.title).ToList();
            while (list.Contains(_name))
            {
                _name = string.Format("{0}{1}", Properties.Resources.PenBaseName, ++i);
            }

            var random = new Random();
            var c = Color.FromRgb((byte)random.Next(255),
                                            (byte)random.Next(255),
                                            (byte)random.Next(255));

            var newData = new PenItem { title = _name, LColor = c, PenStyle = PredefinedPenKinds.seriesLineStyle, StrokeThickness = 1, ShowAxis = false, AutoScale = false, Range = new Opc.Ua.Range(0.0, 100.0), Visible = true, isSet = true, guiId = Guid.NewGuid().ToString() };
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
            ApplyChangesInBackground();
        }
        void ApplyChangesInBackground()
        {
            bDirty = true;
        }

        private void tableView_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "RowData.Row.dlrsource")
            {
                int focusedrow = tableView.FocusedRowHandle;
                int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
                if (listIndex >= 0 && listIndex < PenList.Count)
                {
                    (PenList.ElementAt(listIndex).TagName) = string.Empty;
                    gridControl.ItemsSource = null;
                    gridControl.ItemsSource = PenList;
                    tableView.FocusedRowHandle = focusedrow;
                    ApplyChangesInBackground();
                }
            }
            else if (e.Column.FieldName == "RowData.Row.usesourcetimestamp")
            {
                int focusedrow = tableView.FocusedRowHandle;
                int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
                if (listIndex >= 0 && listIndex < PenList.Count)
                {
                    gridControl.ItemsSource = null;
                    gridControl.ItemsSource = PenList;
                    tableView.FocusedRowHandle = focusedrow;
                    ApplyChangesInBackground();
                }
            }
            else if (e.Column.FieldName == "RowData.Row.authomaticscale" ||
                        e.Column.FieldName == "RowData.Row.showaxis" ||
                        e.Column.FieldName == "RowData.Row.minAggregation" ||
                        e.Column.FieldName == "RowData.Row.maxAggregation" ||
                        e.Column.FieldName == "RowData.Row.avgAggregation" ||
                        e.Column.FieldName == "RowData.Row.useeuminmax" ||
                        e.Column.FieldName == "RowData.Row.color" ||
                        e.Column.FieldName == "RowData.Row.localizeSourceTimeStamp" ||
                        e.Column.FieldName == "RowData.Row.isstatisticenabled" ||
                        e.Column.FieldName == "RowData.Row.logarithmicYScale"
                        )
            {
                UpdateSourceData();
            }
            else if (e.Column.FieldName == "RowData.Row.logarithmicbaseYScale" ||
                        e.Column.FieldName == "RowData.Row.min" ||
                        e.Column.FieldName == "RowData.Row.max" ||
                        e.Column.FieldName == "RowData.Row.thickness" ||
                        e.Column.FieldName == "RowData.Row.arrayindex")
            {
                int focusedrow = tableView.FocusedRowHandle;
                int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
                if (listIndex >= 0 && listIndex < PenList.Count)
                {
                    if (e.Column.FieldName == "RowData.Row.min" || e.Column.FieldName == "RowData.Row.max" || e.Column.FieldName == "RowData.Row.logarithmicbaseYScale")
                    {
                        double cellValue;
                        bool isdouble = double.TryParse(e.Value.ToString(), out cellValue);
                        if (isdouble)
                        {
                            if (e.Column.FieldName == "RowData.Row.min")
                            {
                                (PenList.ElementAt(listIndex).Range.Low) = cellValue;
                            }                                
                            else if (e.Column.FieldName == "RowData.Row.max")
                                (PenList.ElementAt(listIndex).Range.High) = cellValue;
                        }
                    }
                    else if (e.Column.FieldName == "RowData.Row.arrayindex" || e.Column.FieldName == "RowData.Row.thickness")
                    {
                        int cellValue;
                        bool isint = int.TryParse(e.Value.ToString(), out cellValue);
                        if (isint)
                        {
                            if (e.Column.FieldName == "RowData.Row.arrayindex")
                                (PenList.ElementAt(listIndex).ArrayIndex) = cellValue;
                            else if (e.Column.FieldName == "RowData.Row.thickness")
                                (PenList.ElementAt(listIndex).StrokeThickness) = cellValue;
                        }
                    }
                }
                ApplyChangesInBackground();
            }
        }

        private void text_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyChangesInBackground();
        }
        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            PenList.Clear();
            gridControl.ItemsSource = null;
            gridControl.ItemsSource = PenList;
            ApplyChangesInBackground();
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0 && listIndex < PenList.Count)
            {
                (PenList.ElementAt(listIndex).TagName) = string.Empty;
                (PenList.ElementAt(listIndex).NodeId) = string.Empty;
                (PenList.ElementAt(listIndex).TagReferenceXml) = string.Empty;
                gridControl.ItemsSource = null;
                gridControl.ItemsSource = PenList;
                tableView.FocusedRowHandle = focusedrow;
                gridControl.RefreshRow(focusedrow);
                ApplyChangesInBackground();
            }

        }
        #endregion

        private void TagSelector_OnApplyChanges(object sender, EventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            gridControl.RefreshRow(focusedrow);
            ApplyChangesInBackground();
        }

        void UpdateSourceData()
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0 && listIndex < PenList.Count)
            {
                gridControl.ItemsSource = null;
                gridControl.ItemsSource = PenList;
                tableView.FocusedRowHandle = focusedrow;
                ApplyChangesInBackground();
            }
        }

        private void OnBrushSelected(object sender, EventArgs e)
        {
            UpdateSourceData();
        }
    }
}
