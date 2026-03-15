#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.Specialized;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Interaction logic for GridDataColumnChooserWindow.xaml
    /// </summary>
    public partial class GridDataColumnChooserWindow : Window
    {
        internal GridDataVisibleColumns newColumnList;
        internal bool Draggingwindow = false;

        public GridDataColumnChooserWindow()
        {
            InitializeComponent();
            newColumnList = new GridDataVisibleColumns();
            this.Closed += new EventHandler(ColumnChooserWindow_Closed);
            if (Application.Current != null)
            {
                mainWindow = Application.Current.MainWindow as Window;
                this.Owner = mainWindow;
            }
            this.WireEvents();
            this.DragColIndex = -1;
            this.Title = "Column Chooser Window";
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void WireEvents()
        {
            if (this.newColumnList != null)
            {
                ((INotifyCollectionChanged)newColumnList).CollectionChanged += new NotifyCollectionChangedEventHandler(ColumnChooserWindow_CollectionChanged);
            }
            if (this.mainWindow != null)
            {
                mainWindow.Deactivated += new EventHandler(mainWindow_Deactivated);
            }
        }

        private void UnWireEvents()
        {
            if (this.newColumnList != null)
            {
                this.newColumnList.Clear();
                ((INotifyCollectionChanged)newColumnList).CollectionChanged -= new NotifyCollectionChangedEventHandler(ColumnChooserWindow_CollectionChanged);
            }
            if (this.mainWindow != null)
            {
                mainWindow.Deactivated -= new EventHandler(mainWindow_Deactivated);
            }
            this.Closed -= new EventHandler(ColumnChooserWindow_Closed);
            this.grid.Model.QueryCellInfo -= new GridQueryCellInfoEventHandler(Model_QueryCellInfo);
            var dropcontroller = this.Grid.MouseControllerDispatcher.Find("GridDataColumnDropMouseController") as GridDataColumnDropMouseController;
            this.Grid.MouseControllerDispatcher.Remove(dropcontroller);
        }

        void mainWindow_Deactivated(object sender, EventArgs e)
        {
            var window = sender as Window;
            if ((window.WindowState == System.Windows.WindowState.Minimized || window.WindowState == System.Windows.WindowState.Normal) && !window.Focus())
            {
                if (!window.IsEnabled)
                    this.WindowState = System.Windows.WindowState.Minimized;
            }
        }

        public int DragColIndex
        {
            get;
            set;
        }      

        internal string MappingName = "";

        void ColumnChooserWindow_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.newColumnList.Count == 0)
            {
                this.grid.Model.RowCount = 1;
            }
            else
                this.grid.Model.RowCount = newColumnList.Count;
            this.grid.InvalidateCells();
        }
        void ColumnChooserWindow_Closed(object sender, EventArgs e)
        {
            this.UnWireEvents();
        }

        internal GridControl Grid
        {
            get
            {
                return this.grid;
            }
        }

        internal Window mainWindow
        {
            get;
            private set;
        }

        public void InitalizeColumnChooser(GridDataTableModel table,GridDataVisibleColumns Vcolumns)
        {
            this.tableModel = table;
            this.ParentGrid = table.Grid;
            this.Grid.MouseControllerDispatcher.Add(new GridDataColumnDropMouseController(this.tableModel.Grid));
            this.grid.Model.ColumnCount = 1;
            this.grid.Model.RowHeights.DefaultLineSize = 24; //300
            this.grid.Model.ColumnWidths.DefaultLineSize = 175; // 200
            foreach (GridDataVisibleColumn vcol in Vcolumns)
            {
                this.newColumnList.Add(vcol);
            }
            this.initializeTable();
            this.grid.Model.HeaderRows = 0;
            if (table.TableStyle.FlowDirection == System.Windows.FlowDirection.RightToLeft)
            {
                var window = (((ScrollViewer) this.grid.Parent).Parent as GridDataColumnChooserWindow);
                if (window != null) window.FlowDirection = FlowDirection.RightToLeft;
                this.grid.Model.TableStyle.FlowDirection = System.Windows.FlowDirection.RightToLeft;
            }
            this.grid.Model.HeaderColumns = 0;
            this.grid.Model.FrozenColumns = 0;
            this.grid.Model.FrozenRows = 0;
            this.grid.Model.TableStyle.CellType = "Static";
            this.grid.Model.TableStyle.Background = this.tableModel.TableProperties.Model.GetHeaderBackground();
            this.grid.Model.TableStyle.Borders = this.tableModel.TableProperties.Model.GetGroupCellBorders();
            this.grid.Model.QueryCellInfo += new GridQueryCellInfoEventHandler(Model_QueryCellInfo);
        }
        void Model_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (newColumnList != null && newColumnList.Count > 0)
            {
                var column = this.newColumnList[e.Cell.RowIndex];
                e.Style.Description = column.HeaderText != null ? column.HeaderText : column.MappingName;
                e.Style.CellValue = e.Style.Description;
                e.Style.Font = this.TableModel.GridVisualStyle.ValueFont;
                e.Style.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                e.Style.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                e.Style.Background = this.tableModel.TableProperties.Model.GetHeaderBackground();
                e.Style.Foreground = this.tableModel.TableProperties.Model.GetHeaderForeground();
            }
            else
                this.grid.InvalidateCells();
        }

        internal GridDataVisibleColumns totalVisiblecolumns
        {
            get;
            set;
        }

        internal void initializeTable()
        {
            totalVisiblecolumns = this.TableModel.GetVisibleColumns();
            //if (newColumnList.Count > 0)
            //    newColumnList.Clear();
#if SyncfusionFramework4_0
            if (this.TableModel.TableProperties.IsDynamicItemsSource)
            {
                totalVisiblecolumns.Clear();
                totalVisiblecolumns = this.TableModel.GetDynamicVisibleColumns();
            }
#endif
            //Code here to check whether the Hiidden Columns can be added in the column chooser window or not based on the CanAddHiddenColumns property
            GridDataVisibleColumn visColumn;
            foreach (var actualColumn in totalVisiblecolumns)
            {
                visColumn = actualColumn;
                //Code fetching the Visible columns
                IEnumerable<GridDataVisibleColumn> colum = from col in this.TableModel.TableProperties.VisibleColumns
                                                           where col.MappingName == actualColumn.MappingName
                                                           select col;

                //If the fetched columns are null then the actual visible column will be added in the collection
                if (colum.Count() > 0)
                {
                    //If the CanAddHiddenColumns property is true then the visible column defined in the GridDataControl with the properties defined are added in the Column chooser window
                    if (this.CanAddHiddenColumns)
                    {
                        if (colum.ElementAt(0).IsHidden)
                            visColumn = colum.ElementAt(0);
                        else
                            continue;
                    }
                    else//If the CanAddHiddenColumns property is false then the column will not added in the Column chooser window
                        continue;
                }
                // Records in the following collection to display in the column chooser window.

                var count = (from item in this.newColumnList
                             where item.MappingName == visColumn.MappingName
                            select item).ToList().Count;
                if (count==0)               
                {
                    this.newColumnList.Add(visColumn);
                }
            }
        }

        internal GridControlBase ParentGrid
        {
            get;
            private set;
        }

        private GridDataTableModel tableModel;
        internal GridDataTableModel TableModel
        {
            get
            {
                return this.tableModel;
            }
        }

        private bool _CanAddHiddenColumns = false;

        /// <summary>
        /// Gets or sets a value indicating whether this instance can add hidden columns.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can add hidden columns; otherwise, <c>false</c>.
        /// </value>
        public bool CanAddHiddenColumns
        {
            get 
            { 
                return _CanAddHiddenColumns; 
            }
            set 
            { 
                _CanAddHiddenColumns = value; 
            }
        }
    }
}
