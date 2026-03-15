using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Editors;
using OPCUAViewModel;
using Utilities;
using Utilities.WPF;
using System.Windows.Threading;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService;
using StringManager.ComponentService;
using System.ComponentModel;
using DataAnalisysControl.Converters;
using DataLoggerColumnListControl;
using WPFPenHelpers;
using WPFUtilities;
using System.Collections.ObjectModel;

namespace DataAnalisysControl.Controls
{
    /// <summary>
    /// Interaction logic for SmartControl.xaml
    /// </summary>
    public partial class SmartControl : UserControl
    {

        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(SmartControl), new UIPropertyMetadata(null));
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
        readonly DataAnalisys control;
        ObservableCollection<SerieData> datalist;
        Window parentWindow;
        
        IUFUAEditorManager ufuaEditor;
        int currentSelectedRow;

        bool bLoaded;
        #endregion

        #region Constructors
        public SmartControl(DataAnalisys c)
        {
            InitializeComponent();

            control = c;
            DataContext = c;

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(control);
                if (ufuaEditor == null && Document != null)
                    ufuaEditor = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

                AggregatedTablesConverter converter = TryFindResource("AggregatedTablesConverter") as AggregatedTablesConverter;
                if (converter != null)
                {
                    converter.Document = Document;
                    converter.UFUAEditorManager = ufuaEditor;
                }

                parentWindow = this.FindParent<Window>();
                if (parentWindow != null)
                {
                    parentWindow.Closing += (s, ev) =>
                    {
                        ApplyChanges();
                    };
                }

                datalist = new ObservableCollection<SerieData>(new SerieDataList(control.StaticSeriesSettings));
                gridControl.ItemsSource = datalist;
                gridControl.SelectedItem = datalist.LastOrDefault();

                bool bDirty = false;
                (from SerieData entry in datalist.AsParallel()
                 where entry.tagName != null && entry.tagReference == null &&
                 entry.dlrsource
                 select entry).ToList().ForEach(entry =>
                 {
                     bDirty = true;
                     entry.tagReference = SmartControlUtilities.Helpers.GetTagReference(entry.tagName, Document);
                 });

                if (bDirty)
                    ApplyChanges();
            };
        }
        #endregion

        List<ComboBoxEdit> listFilled = new List<ComboBoxEdit>();
        private void PART_Editor_DropDownOpened(object sender, EventArgs e)
        {
            var combo = sender as ComboBoxEdit;
            if (combo == null || listFilled.Contains(combo))
                return;
            listFilled.Add(combo);
            combo.ItemsSource = control.ListVariables;
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

        private void LINE_Editor_IndexChanged(object sender, EventArgs e)
        {
            var combo = sender as ComboBoxEdit;
            if (combo.SelectedIndex == -1) return;
            var combostring = lines[combo.SelectedIndex];
            datalist[currentSelectedRow].serieTypeLine = combostring;
            datalist[currentSelectedRow].serieTypeLineKey = control.serieTypeKeys[combostring];
            ApplyChanges();
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
            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex >= 0)
                datalist.RemoveAt(listIndex);
            ApplyChanges();
        }

        private void InsertRow(int rowHandle, SerieData data)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex < 0 || listIndex >= datalist.Count) listIndex = -1;
            datalist.Insert(listIndex + 1, data);
            gridControl.SelectedItem = data;
            ApplyChanges();
        }

        private void OnMoveUp(object sender, RoutedEventArgs e)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(tableView.FocusedRowHandle);
            if (listIndex > 0)
            {
                datalist.Move(listIndex, listIndex - 1);
                ApplyChanges();
            }
        }
        private void OnMoveDown(object sender, RoutedEventArgs e)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(tableView.FocusedRowHandle);
            if (listIndex < datalist.Count - 1)
            {
                datalist.Move(listIndex, listIndex + 1);
                ApplyChanges();
            }
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            int i = datalist.Count;
            var _name = string.Format("{0}{1}", Properties.Resources.PenBaseName, i);

            var list = (from s in datalist select s.title).ToList();
            while (list.Contains(_name))
            {
                _name = string.Format("{0}{1}", Properties.Resources.PenBaseName, ++i);
            }

            var random = new Random();
            var c = Color.FromRgb((byte)random.Next(255),
                                            (byte)random.Next(255),
                                            (byte)random.Next(255));

            var newData = new SerieData { title = _name, color = c, serieTypeLine = Properties.Resources.LineSerie, serieTypeLineKey = control.serieTypeKeys[Properties.Resources.LineSerie], thickness = 1, logarithmicbaseYScale = 10.0, showaxis = false, logarithmicYScale = false, authomaticscale = false, min = 0.0, max = 100.0, isVisible = true, isSet = true,  guiId = Guid.NewGuid().ToString()};
            InsertRow(tableView.FocusedRowHandle, newData);
            ApplyChanges();
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            DeleteRow(tableView.FocusedRowHandle);
            ApplyChanges();
        }

        private void tableView_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(tableView.FocusedRowHandle);
            currentSelectedRow = listIndex;
            btnDelete.IsEnabled = listIndex >= 0;
        }
        private void tableView_CellValueChanged(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            ApplyChanges();
        }

        void ApplyChanges()
        {
            if (parentWindow == null || parentWindow.DialogResult == true)
            {
                control.StaticSeriesSettings = new SerieDataList(datalist.ToList());
            }
        }

        private void tableView_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "RowData.Row.dlrsource")
            {
                int focusedrow = tableView.FocusedRowHandle;
                int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
                if (listIndex >= 0 && listIndex < datalist.Count)
                {
                    (datalist.ElementAt(listIndex).tagReference) = null;
                    (datalist.ElementAt(listIndex).tagName) = string.Empty;
                    (datalist.ElementAt(listIndex).historicalName) = string.Empty;
                    (datalist.ElementAt(listIndex).sourcetimestampColumnName) = string.Empty;
                    (datalist.ElementAt(listIndex).useTableAggregation) = false;
                    (datalist.ElementAt(listIndex).minAggregation) = false;
                    (datalist.ElementAt(listIndex).maxAggregation) = false;
                    (datalist.ElementAt(listIndex).avgAggregation) = false;
                    tableView.FocusedRowHandle = focusedrow;
                    ApplyChanges();
                }
            }
            else if (e.Column.FieldName == "RowData.Row.usesourcetimestamp")
            {
                int focusedrow = tableView.FocusedRowHandle;
                int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
                if (listIndex >= 0 && listIndex < datalist.Count)
                {
                    (datalist.ElementAt(listIndex).localizeSourceTimeStamp) = false;
                    tableView.FocusedRowHandle = focusedrow;
                    ApplyChanges();
                }
            }
            else if (e.Column.FieldName == "RowData.Row.authomaticscale" || 
                        e.Column.FieldName == "RowData.Row.showaxis" ||
                        e.Column.FieldName == "RowData.Row.minAggregation" ||
                        e.Column.FieldName == "RowData.Row.maxAggregation" ||
                        e.Column.FieldName == "RowData.Row.avgAggregation" ||
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
                        e.Column.FieldName == "RowData.Row.thickness")
            {
                int focusedrow = tableView.FocusedRowHandle;
                int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
                if (listIndex >= 0 && listIndex < datalist.Count)
                {
                    if (e.Column.FieldName == "RowData.Row.min" || e.Column.FieldName == "RowData.Row.max" || e.Column.FieldName == "RowData.Row.logarithmicbaseYScale")
                    {
                        double cellValue;
                        bool isdouble = double.TryParse(e.Value.ToString(), out cellValue);
                        if (isdouble)
                        {
                            if (e.Column.FieldName == "RowData.Row.min")
                                (datalist.ElementAt(listIndex).min) = cellValue;
                            else if (e.Column.FieldName == "RowData.Row.max")
                                (datalist.ElementAt(listIndex).max) = cellValue;
                            else if (e.Column.FieldName == "RowData.Row.logarithmicbaseYScale")
                                (datalist.ElementAt(listIndex).logarithmicbaseYScale) = cellValue;
                        }
                    }
                    else if (e.Column.FieldName == "RowData.Row.thickness")
                    {
                        int cellValue;
                        bool isint = int.TryParse(e.Value.ToString(), out cellValue);
                        if (isint)
                            (datalist.ElementAt(listIndex).thickness) = cellValue;
                    }
                }
                ApplyChanges();
            }
            else if (e.Column.FieldName == "RowData.Row.useTableAggregation")
            {
                int focusedrow = tableView.FocusedRowHandle;
                int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
                if (listIndex >= 0 && listIndex < datalist.Count)
                {
                    (datalist.ElementAt(listIndex).serieTypeLine) = Properties.Resources.LineSerie;
                    (datalist.ElementAt(listIndex).serieTypeLineKey) = control.serieTypeKeys[Properties.Resources.LineSerie];
                    (datalist.ElementAt(listIndex).minAggregation) = false;
                    (datalist.ElementAt(listIndex).maxAggregation) = false;
                    (datalist.ElementAt(listIndex).avgAggregation) = false;
                    datalist.ElementAt(listIndex).isstatisticenabled = false;
                    tableView.FocusedRowHandle = focusedrow;
                    ApplyChanges();
                }
            }
        }

        private void text_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyChanges();
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
                (datalist.ElementAt(listIndex).tagReference) = null;
                (datalist.ElementAt(listIndex).useTableAggregation) = false;
                (datalist.ElementAt(listIndex).minAggregation) = false;
                (datalist.ElementAt(listIndex).maxAggregation) = false;
                (datalist.ElementAt(listIndex).avgAggregation) = false;
                tableView.FocusedRowHandle = focusedrow;
                gridControl.RefreshRow(focusedrow);
                ApplyChanges();
            }

        }

        private void OnApplyChanges(object sender, EventArgs e)
        {
            bool bForceUpdating = false;
            int focusedrow = tableView.FocusedRowHandle;
            if (sender is SmartControlUtilities.Controls.TagSelector)
            {
                int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
                if (listIndex >= 0 && listIndex < datalist.Count)
                {
                    var data = datalist.ElementAt(listIndex);
                    if (data.dlrsource)
                    {
                        if (string.IsNullOrEmpty(data.historicalName) || !ufuaEditor.UsesAggreagatedTables(Document, data.historicalName))
                        {
                            (data.useTableAggregation) = false;
                            (data.minAggregation) = false;
                            (data.maxAggregation) = false;
                            (data.avgAggregation) = false;
                        }
                    }
                    else
                    {
                        (data.useTableAggregation) = false;
                        (data.minAggregation) = false;
                        (data.maxAggregation) = false;
                        (data.avgAggregation) = false;
                    }
                    bForceUpdating = true;
                }
            }
            gridControl.RefreshRow(focusedrow);
            tableView.FocusedRowHandle = focusedrow;
            if(bForceUpdating)
                UpdateCells(focusedrow);
            ApplyChanges();
        }
        void UpdateCells(int focusedrow)
        {
            List<string> collist = new List<string>() { "tabAggregation", "tabMinAggregation", "tabAvgAggregation", "tabMaxAggregation" };
            collist.ForEach(colname =>
            {
                var col = gridControl.Columns.GetColumnByName(colname);
                if (col != null)
                {
                    var temp = col.CellTemplate;
                    col.CellTemplate = null;
                    col.CellTemplate = temp;
                    var cell = tableView.GetCellElementByRowHandleAndColumn(focusedrow, col);
                    cell?.ApplyTemplate();
                }
            });
        }

        void UpdateSourceData()
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0 && listIndex < datalist.Count)
            {
                tableView.FocusedRowHandle = focusedrow;
                ApplyChanges();
            }
        }

        private void OnBrushSelected(object sender, EventArgs e)
        {
            UpdateSourceData();
        }
    }
}
