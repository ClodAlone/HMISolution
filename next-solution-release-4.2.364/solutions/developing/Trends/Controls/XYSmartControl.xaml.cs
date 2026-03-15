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
using ViewModelLib;
using System.Windows.Threading;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService;
using StringManager.ComponentService;
using System.ComponentModel;
using WPFUtilities;
using DataLoggerColumnListControl;
using SmartControlUtilities.Controls;

namespace Trends.Controls
{
    /// <summary>
    /// Interaction logic for SmartControl.xaml
    /// </summary>
    public partial class XYSmartControl : UserControl
    {

        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(XYSmartControl), new UIPropertyMetadata(null));
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
        ChartXY chartcontrol;
        XYPenItemList datalist;
        Window parentWindow;

        bool bLoaded;
        #endregion

        #region Constructors
        public XYSmartControl(ChartXY c)
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

                datalist = new XYPenItemList(chartcontrol.PenList);
                gridControl.ItemsSource = datalist;
                gridControl.SelectedItem = datalist.LastOrDefault();
                arrayIndex.Visible = false;
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

            var newData = new XYPenItem {
                NodeId = Guid.NewGuid().ToString(),
                Name = _name,
                LColor = _color,
                LFColor = Colors.White,
                PenStyle = PredefinedPenKinds.seriesLineStyle,
                Range = new Opc.Ua.Range(100.0, 0.0),
                UseEURange = true,
                Visible = true, 
                Dlrsource = false
            };

            InsertRow(tableView.FocusedRowHandle, newData);
            ApplyChanges();
        }

        private void InsertRow(int rowHandle, XYPenItem data)
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
            if (tableView.IsEditing)
                return;

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

        private void UpdateTag(FrameworkElement sender, OPCUAEntityReference tag)
        {
            if (sender == null)
                return;
            try
            {
                var temp = (sender as FrameworkElement).TemplatedParent;
                var rowdata = temp.FindFirstParent<FrameworkElement>();
                var editor = rowdata.GetVisualChildrenOfType<TextBox>();
                (from c in rowdata.GetVisualChildrenOfType<TextBox>()
                 where c.Name == "uriLabel"
                 select c).ToList().ForEach(child =>
                 {
                     if (tag == null)
                     {
                         child.Tag = string.Empty;
                         child.Text = string.Empty;
                     }
                     else
                     {
                         child.Tag = child.Text;
                         if (tag.ReadablePath == null || tag.AppName == null || String.IsNullOrEmpty(tag.ReadablePath))
                             child.Text = tag.HumanReadable;
                         else
                         {
                             Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                             UInt16 ns = (UInt16)(n.Count + 2 - 1);
                             string oldChars = string.Format("{0}:", ns);
                             child.Text = string.Format("{0} ({1})", (tag.ReadablePath).Replace(oldChars, "").Replace('&', '\\'), tag.AppName);
                         }
                     }
                 });
            }
            catch (Exception)
            {
            }
        }

        private void tableView_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "RowData.Row.LColor" ||
                e.Column.FieldName == "RowData.Row.LFColor" ||
                e.Column.FieldName == "RowData.Row.PenStyle")
            {
                UpdateSourceData();
            }
            if (e.Column.FieldName == "RowData.Row.Range.Low" || e.Column.FieldName == "RowData.Row.Range.High" || e.Column.FieldName == "RowData.Row.StrokeThickness")
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
        }

        private void OnApplyChanges(object sender, EventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            gridControl.RefreshRow(focusedrow);
            ApplyChanges();
        }

        void UpdateSourceData()
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0 && listIndex < datalist.Count)
            {
                gridControl.ItemsSource = null;
                gridControl.ItemsSource = datalist;
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
