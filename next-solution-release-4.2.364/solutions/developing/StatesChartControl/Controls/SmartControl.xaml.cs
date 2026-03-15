using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Xpf.Grid;
using Utilities;
using Utilities.WPF;
using DevExpress.Xpf.Editors;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using WPFUtilities;
using System.Windows.Input;
using System.ComponentModel;

namespace StatesChartControl.Controls
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
        readonly StatesChartControl bcontrol;
        TagPenList datalist;
        Window parentWindow;

        IUFUAEditorManager ufuaEditor;

        bool bLoaded;
        #endregion

        #region Constructors
        public SmartControl(StatesChartControl c)
        {
            InitializeComponent();

            bcontrol = c;
            DataContext = c;

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(bcontrol);
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

                datalist = new TagPenList(bcontrol.TPenList);
                gridControl.ItemsSource = datalist;
                if(datalist.Count > 0)
                    gridControl.SelectedItem = datalist.LastOrDefault();
            };
        }
        #endregion

        #region Methods
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
                ApplyChanges();
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
                        ApplyChanges();
                    }
                }
            }
        }

        private void OnApplyChanges(object sender, EventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            gridControl.RefreshRow(focusedrow);
            ApplyChanges();
        }

    readonly List<ComboBoxEdit> list = new List<ComboBoxEdit>();

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

            var newData = new PenList() { title = _name };
            InsertRow(tableView.FocusedRowHandle, newData);
            ApplyChanges();
        }

        Random randonGen = new Random();
        SolidColorBrush RandomColor()
        {
            SolidColorBrush randomColor = new SolidColorBrush(Color.FromRgb((byte)randonGen.Next(255), (byte)randonGen.Next(255), (byte)randonGen.Next(255)));
            return randomColor;
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            DeleteRow(tableView.FocusedRowHandle);
        }
        private void DeleteRow(int rowHandle)
        {
            if (datalist.Count < 1)
                return;

            if (tableView.IsEditing)
                return;

            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex >= 0)
            {
                datalist.Remove((gridControl.SelectedItem as PenList));
                ApplyChanges();
            }
        }

        private void InsertRow(int rowHandle, PenList data)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex < 0 || listIndex >= datalist.Count) listIndex = -1;
            datalist.Insert(listIndex + 1, data);
            gridControl.SelectedItem = data;
            ApplyChanges();
        }
        private void tableView_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(tableView.FocusedRowHandle);
            btnDelete.IsEnabled = listIndex >= 0;
        }

        void ApplyChanges()
        {
            if (parentWindow == null || parentWindow.DialogResult == true)
            {
                bcontrol.TPenList = datalist;
                bcontrol.RedrawPlotGrid(false, true);
            }
        }
        #endregion
     
        private void tableView_CellValueChanged(object sender, CellValueChangedEventArgs e)
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
                (datalist.ElementAt(listIndex).tagreferenceXml) = string.Empty;
                (datalist.ElementAt(listIndex).useTableAggregation) = false;
                (datalist.ElementAt(listIndex).minAggregation) = false;
                (datalist.ElementAt(listIndex).maxAggregation) = false;
                (datalist.ElementAt(listIndex).avgAggregation) = false;
                tableView.FocusedRowHandle = focusedrow;
                gridControl.RefreshRow(focusedrow);
                ApplyChanges();
            }
        }
    }
}
