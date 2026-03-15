using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utilities;
using Utilities.WPF;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService;

namespace Trends.Controls
{
    /// <summary>
    /// Interaction logic for SmartControl.xaml
    /// </summary>
    public partial class ChartSmartControl : UserControl
    {
        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(ChartSmartControl), new UIPropertyMetadata(null));
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
        Chart chartcontrol;
        PenItemList datalist;
        Window parentWindow;
        
        bool bLoaded;
        #endregion

        #region Constructors
        public ChartSmartControl(Chart c)
        {
            InitializeComponent();

            chartcontrol = c;
            DataContext = c;

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(chartcontrol);

                parentWindow = this.FindParent<Window>();
                if (parentWindow != null)
                {
                    parentWindow.Closing += (s, ev) =>
                    {
                        ApplyChanges();
                    };
                }

                arrayIndex.Visible = false;
                dlrsource.Visible = false;
                historicalName.Visible = false;

                datalist = new PenItemList(chartcontrol.PenList);
                gridControl.ItemsSource = datalist;
                gridControl.SelectedItem = datalist.LastOrDefault();
            };
        }
        #endregion

        #region Methods
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

            while ((from c in datalist select c.Name).ToList().Contains(_name))
            {
                _name = string.Format("{0}{1}", Properties.Resources.PenBaseName, ++i);
            }

            var _color = SmartControlUtilities.TagColorHelper.RandomColor();

            var newData = new PenItem
            {
                Name = _name,
                NodeId = Guid.NewGuid().ToString(),
                LColor = _color,
                PenStyle = PredefinedPenKinds.seriesLineStyle,
                Range = new Opc.Ua.Range(100.0, 0.0),
                UseEURange = true,
                Visible = true,
                PointPrecision = -1
            };

            InsertRow(tableView.FocusedRowHandle, newData);
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

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            DeleteRow(tableView.FocusedRowHandle);
        }

        private void DeleteRow(int rowHandle)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex >= 0)
                datalist.RemoveAt(listIndex);
            ApplyChanges();
        }

        void ApplyChanges()
        {
            if (parentWindow == null || parentWindow.DialogResult == true)
            {
                chartcontrol.PenList = datalist;
            }
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
        private void text_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyChanges();
        }

        #endregion
       private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0 && listIndex < datalist.Count)
            {
                (datalist.ElementAt(listIndex).TagReference) = null;
                (datalist.ElementAt(listIndex).HistoricalName) = string.Empty;
                (datalist.ElementAt(listIndex).ColuName) = string.Empty;
                tableView.FocusedRowHandle = focusedrow;
                gridControl.RefreshRow(focusedrow);
                ApplyChanges();
            }
        }
        
        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0 && listIndex < datalist.Count)
            {
                tableView.FocusedRowHandle = focusedrow;
                gridControl.RefreshRow(focusedrow);
                ApplyChanges();
            }
        }

        private void ButtonPColorClear_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                (gridControl.SelectedItem as PenItem).PointSettings.Clear();
                gridControl.RefreshData();
                tableView.FocusedRowHandle = focusedrow;
                ApplyChanges();
            }
        }
        private void ButtonPColor_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                var pointSettings = new PointSettings((gridControl.SelectedItem as PenItem).PointSettings);
                PointSettingsListEditor urieditor = new PointSettingsListEditor(pointSettings, Document);
                var Dialog = new GeneralDialogContent(urieditor)
                {
                    Owner = this.FindParent<Window>(),
                    DialogKeepContent = true,
                    Title = Properties.Resources.PointSettingsEditor,
                    HelpLink = "PointSettingsEditor"
                };

                if (Dialog.ShowDialog() == true)
                {
                    (gridControl.SelectedItem as PenItem).PointSettings = pointSettings;
                    gridControl.RefreshData();
                    tableView.FocusedRowHandle = focusedrow;
                    ApplyChanges();
                }
            }

        }

        private void tableView_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "RowData.Row.LColor")
                UpdateSourceData();
            if (e.Column.FieldName == "RowData.Row.Range.Low" || 
                e.Column.FieldName == "RowData.Row.Range.High" || 
                e.Column.FieldName == "RowData.Row.StrokeThickness")
            {
                int focusedrow = tableView.FocusedRowHandle;
                int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
                double cellValue;
                bool isdouble = double.TryParse(e.Value.ToString(), out cellValue);
                if (listIndex >= 0 && listIndex < datalist.Count && isdouble)
                {
                    if (e.Column.FieldName == "RowData.Row.Range.High")
                        (datalist.ElementAt(listIndex).Range.High) = cellValue;
                    if (e.Column.FieldName == "RowData.Row.Range.Low")
                        (datalist.ElementAt(listIndex).Range.Low) = cellValue;
                    if (e.Column.FieldName == "RowData.Row.StrokeThickness")
                        (datalist.ElementAt(listIndex).StrokeThickness) = cellValue;                    
                }
                ApplyChanges();
            }
            if (e.Column.FieldName == "RowData.Row.PointPrecision")
            {
                int focusedrow = tableView.FocusedRowHandle;
                int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
                int cellValue;
                bool isInt = int.TryParse(e.Value.ToString(), out cellValue);                
                if(listIndex >= 0 && listIndex < datalist.Count && isInt)
                    (datalist.ElementAt(listIndex).PointPrecision) = cellValue;

                ApplyChanges();
            }
        }

        private void PenStyleChanged(object sender, EventArgs e)
        {
            UpdateSourceData();
        }

        private void UpdateSourceData()
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0 && listIndex < datalist.Count)
            {
                tableView.FocusedRowHandle = focusedrow;
                ApplyChanges();
            }
        }

        private void OnApplyChanges(object sender, EventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            gridControl.RefreshRow(focusedrow);
            ApplyChanges();
        }

        private void OnBrushSelected(object sender, EventArgs e)
        {
            UpdateSourceData();
        }
    }
}
