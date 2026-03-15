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
using TrendRealTimeSlim.Converters;
using DataLoggerColumnListControl;
using WPFPenHelpers;
using WPFUtilities;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Grid;
using UFInterfaces;

namespace TrendRealTimeSlim.Controls
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

        #region Properties

        IWorkspace workspace;
        public IWorkspace Workspace
        {
            get
            {
                if (workspace == null && Document != null)
                    workspace = Document.GetService(typeof(IWorkspace)) as IWorkspace;
                return workspace;
            }
        }

        #endregion

        #region Declarations
        readonly RealTimeTrendControl control;
        ObservableCollection<PenItem> datalist;
        Window parentWindow;

        IUFUAEditorManager ufuaEditor;
        int currentSelectedRow;

        bool bLoaded;
        #endregion

        #region Constructors
        public SmartControl(RealTimeTrendControl c)
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

                parentWindow = this.FindParent<Window>();
                if (parentWindow != null)
                {
                    parentWindow.Closing += (s, ev) =>
                    {
                        ApplyChanges();
                    };
                }

                datalist = new ObservableCollection<PenItem>(new PenItemList(control.PenList));
                gridControl.ItemsSource = datalist;
                gridControl.SelectedItem = datalist.LastOrDefault();

                OnExpand(null, null);
            };
        }
        #endregion

        private void OnExpand(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < gridControl.VisibleRowCount; i++)
            {
                var rowHandle = gridControl.GetRowHandleByVisibleIndex(i);
                gridControl.ExpandMasterRow(rowHandle);
            }
        }
        private void OnCollapse(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < gridControl.VisibleRowCount; i++)
            {
                var rowHandle = gridControl.GetRowHandleByVisibleIndex(i);
                gridControl.CollapseMasterRow(rowHandle);
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
            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex >= 0)
                datalist.RemoveAt(listIndex);
            ApplyChanges();
        }

        private void InsertRow(int rowHandle, PenItem data)
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
            var newData = GetDefaultSerie(datalist.ToList()); 
            InsertRow(GetMasterFocusedRowHandle(), newData);
            ApplyChanges();
        }

        PenItem GetDefaultSerie(List<PenItem> serieDatas)
        {
            int i = serieDatas.Count;
            var _name = string.Format("{0}{1}", Properties.Resources.PenBaseName, i);

            var list = (from s in serieDatas select s.title).ToList();
            while (list.Contains(_name))
            {
                _name = string.Format("{0}{1}", Properties.Resources.PenBaseName, ++i);
            }

            if (i > 255)
                i = 0;
            var random = new Random(i);
            var c = Color.FromRgb((byte)random.Next(255),
                                            (byte)random.Next(255),
                                            (byte)random.Next(255));

            var newData = new PenItem
            {
                title = _name,
                LColor = c,
                PenStyle = PredefinedPenKinds.seriesLineStyle,
                StrokeThickness = 1,
                ShowAxis = false,
                AutoScale = true,
                Range = new Opc.Ua.Range(0.0,100.0),
                Visible = true,
                isSet = true,
                guiId = Guid.NewGuid().ToString(),
                UseEURange = false,
                addVirtualPoints = false,
                PointPrecision = -1
            };
            return newData;
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            DeleteRow(GetMasterFocusedRowHandle());
            ApplyChanges();
        }

        int GetMasterFocusedRowHandle()
        {
            var view = gridControl.View.FocusedView;
            return view == tableView ? tableView.FocusedRowHandle : (view.DataControl as GridControl).GetMasterRowHandle();
        }

        private void tableView_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            var focusedRowHandle = GetMasterFocusedRowHandle();
            var listIndex = gridControl.GetListIndexByRowHandle(focusedRowHandle);
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
                control.PenList = new PenItemList(datalist.ToList());
            }
        }

        private void tableView_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            var focusedRowHandle = GetMasterFocusedRowHandle();
            if (e.Column.FieldName == "RowData.Row.dlrsource")
            {
                int listIndex = gridControl.GetListIndexByRowHandle(focusedRowHandle);
                if (listIndex >= 0 && listIndex < datalist.Count)
                {
                    (datalist.ElementAt(listIndex).TagReference) = null;
                    (datalist.ElementAt(listIndex).TagName) = string.Empty;
                    //tableView.FocusedRowHandle = focusedRowHandle;
                    ApplyChanges();
                }
            }
            else if (e.Column.FieldName == "RowData.Row.usesourcetimestamp")
            {
                int listIndex = gridControl.GetListIndexByRowHandle(focusedRowHandle);
                if (listIndex >= 0 && listIndex < datalist.Count)
                {
                    ApplyChanges();
                }
            }
            else if (e.Column.FieldName == "RowData.Row.authomaticscale" ||
                        e.Column.FieldName == "RowData.Row.showaxis" ||
                        e.Column.FieldName == "RowData.Row.minAggregation" ||
                        e.Column.FieldName == "RowData.Row.maxAggregation" ||
                        e.Column.FieldName == "RowData.Row.avgAggregation" ||
                        e.Column.FieldName == "RowData.Row.useeuminmax" ||
                        e.Column.FieldName == "RowData.Row.addVirtualPoints" ||
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
                int listIndex = gridControl.GetListIndexByRowHandle(focusedRowHandle);
                if (listIndex >= 0 && listIndex < datalist.Count)
                {
                    if (e.Column.FieldName == "RowData.Row.min" || e.Column.FieldName == "RowData.Row.max" || e.Column.FieldName == "RowData.Row.logarithmicbaseYScale")
                    {
                        double cellValue;
                        bool isdouble = double.TryParse(e.Value.ToString(), out cellValue);
                        if (isdouble)
                        {
                            if (e.Column.FieldName == "RowData.Row.min")
                                (datalist.ElementAt(listIndex).Range.Low) = cellValue;
                            else if (e.Column.FieldName == "RowData.Row.max")
                                (datalist.ElementAt(listIndex).Range.High) = cellValue;
                        }
                    }
                    else if (e.Column.FieldName == "RowData.Row.arrayindex" || e.Column.FieldName == "RowData.Row.thickness" || e.Column.FieldName == "RowData.Row.pointPrecision")
                    {
                        int cellValue;
                        bool isint = int.TryParse(e.Value.ToString(), out cellValue);
                        if (isint)
                        {
                            if (e.Column.FieldName == "RowData.Row.arrayindex")
                                (datalist.ElementAt(listIndex).ArrayIndex) = cellValue;
                            else if (e.Column.FieldName == "RowData.Row.thickness")
                                (datalist.ElementAt(listIndex).StrokeThickness) = cellValue;
                            else if (e.Column.FieldName == "RowData.Row.pointPrecision")
                                (datalist.ElementAt(listIndex).PointPrecision) = cellValue;
                        }
                    }
                }
                ApplyChanges();
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
                (datalist.ElementAt(listIndex).TagName) = string.Empty;
                (datalist.ElementAt(listIndex).NodeId) = string.Empty;
                (datalist.ElementAt(listIndex).TagReference) = null;
                //tableView.FocusedRowHandle = focusedrow;
                gridControl.RefreshRow(focusedrow);
                ApplyChanges();
            }

        }

        private void OnApplyChanges(object sender, EventArgs e)
        {
            bool bForceUpdating = false;
            int focusedrow = GetMasterFocusedRowHandle();
            if (sender is SmartControlUtilities.Controls.TagSelector)
            {
                int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
                if (listIndex >= 0 && listIndex < datalist.Count)
                {
                    var data = datalist.ElementAt(listIndex);
                    bForceUpdating = true;
                }
            }
            gridControl.RefreshRow(focusedrow);
            //tableView.FocusedRowHandle = focusedrow;
            if (bForceUpdating)
            {
                var focusedView = gridControl.View.FocusedView;
                List<string> collist = new List<string>() { "tabAggregation", "tabMinAggregation", "tabAvgAggregation", "tabMaxAggregation" };
                collist.ForEach(colname =>
                {
                    var col = (focusedView.DataControl as GridControl).Columns.GetColumnByName(colname);
                    if (col != null)
                    {
                        var temp = col.CellTemplate;
                        col.CellTemplate = null;
                        col.CellTemplate = temp;
                        var cell = focusedView.GetCellElementByRowHandleAndColumn(focusedView.FocusedRowHandle, col);
                        cell?.ApplyTemplate();
                    }
                });
            }
            ApplyChanges();
        }

        void UpdateSourceData()
        {
            var focusedRowHandle = GetMasterFocusedRowHandle();
            int listIndex = gridControl.GetListIndexByRowHandle(focusedRowHandle);
            if (listIndex >= 0 && listIndex < datalist.Count)
            {
                //tableView.FocusedRowHandle = focusedRowHandle;
                ApplyChanges();
            }
        }

        private void OnBrushSelected(object sender, EventArgs e)
        {
            UpdateSourceData();
        }
    }
}
