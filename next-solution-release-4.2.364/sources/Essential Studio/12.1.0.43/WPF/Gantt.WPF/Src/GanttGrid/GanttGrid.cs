#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using Syncfusion.Windows.Controls.Grid;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Media;
using System.Collections.Generic;
using Syncfusion.Windows.Controls.Gantt;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;

namespace Syncfusion.Windows.Controls.Gantt.Grid
{
    /// <summary>
    /// Represents a control that can display the project infromation in table format.
    /// </summary>
    public class GanttGrid : GridTreeControl
    {
        #region Internal & private members

        private bool isHeaderBackgroundChanged = false;
        private bool isHeaderForegroundChanged = false;
        private bool isInDefaultView = true;
        private bool isGridLoaded = false;

        internal GanttControl ParentControl;
#if !SILVERLIGHT
        internal ScrollViewer GridScrollViewer;
#else
        internal ScrollableContentViewer GridScrollViewer;
#endif
        int oldRowIndex = 0;
        GridTreeNode currentNode;

        /// <summary>
        /// Variables to Store the Required Column Indexes.
        /// </summary>
        int taskIDColumnIndex = -1;
        int stDtColumnIndex = 0;
        int fsDtColumnIndex = 0;
        int cstColumnIndex = 0;
        int baseStDtColumnIndex = 0;
        int baseFsDtColumnIndex = 0;
        int baseCstColumnIndex = 0;     

        // Default Height for Grid Row Header
        private double defaultGridHeaderHeight = 40d;

        // Collection Contains list of Existing Columns in Grid.
        private Dictionary<GridTreeColumn, string> ExistingColumns = new Dictionary<GridTreeColumn, string>();

        // ItemsSource for The AddNewColum ComboBox      
        private string[] NewColumns = new string[6];       
        
        // Determines Hide/Display of AddNew Column
        private bool CanCreateAddNewColumn = false;
    
        // Determines current Process[whether it is TableView/ Entry View]
        private bool IsShowTableView = false;

        // Cheks whether the cost column exists or not
        private bool IsCostColumnExists = false;

        // Creates a New Colum to store the AddNewColumn 
        private GridTreeColumn addNewColumn = new GridTreeColumn()
        {
            MappingName = "Add New Column",
            HeaderText = "Add New Column",
            Width = 122d,
            StyleInfo = new GridStyleInfo() { CellType = "Static" }
        };

        #endregion

        #region Public properties

        #region  Gantt Model

        /// <summary>
        /// Gets the gantt model.
        /// </summary>
        /// <value>The gantt model.</value>
        public GanttModel GanttModel
        {
            get
            {
                return this.ParentControl != null ? this.ParentControl.Model : null;
            }
        }

        #endregion

        #region  Header Background & Foreground

        /// <summary>
        /// Gets or sets the header background.
        /// </summary>
        /// <value>The header background.</value>
        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(GanttGrid), new PropertyMetadata(null,OnHeaderBackgroundChanged));

        /// <summary>
        /// Called when [header background changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHeaderBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttGrid grid = sender as GanttGrid;
            if (grid == null || grid.InternalGrid == null)
            {
                grid.isHeaderBackgroundChanged = true;
                return;
            }            
            grid.InternalGrid.ColumnHeaderStyle.Background = (Brush)args.NewValue;
            grid.InternalGrid.RowHeaderStyle.Background = (Brush)args.NewValue;
            grid.InternalGrid.InvalidateCell(GridRangeInfo.Row(0));
            grid.InternalGrid.InvalidateCell(GridRangeInfo.Col(0));
            grid.InternalGrid.InvalidateVisual();

        }

        /// <summary>
        /// Gets or sets the header foreground.
        /// </summary>
        /// <value>The header foreground.</value>
        public Brush HeaderForeground
        {
            get { return (Brush)GetValue(HeaderForegroundProperty); }
            set { SetValue(HeaderForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(GanttGrid), new PropertyMetadata(null, OnHeaderForegroundChanged));

        /// <summary>
        /// Called when [header foreground changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHeaderForegroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttGrid grid = sender as GanttGrid;
            if (grid == null || grid.InternalGrid == null)
            {
                grid.isHeaderForegroundChanged = true;
                return;
            }

            grid.InternalGrid.ColumnHeaderStyle.Foreground = (Brush)args.NewValue;
            grid.InternalGrid.RowHeaderStyle.Foreground = (Brush)args.NewValue;
            grid.InternalGrid.InvalidateCell(GridRangeInfo.Row(0));
            grid.InternalGrid.InvalidateCell(GridRangeInfo.Col(0));
            grid.InternalGrid.InvalidateVisual();            
        }

        #endregion

        #region ShowAddNewColumn Property

        /// <summary>
        /// Gets/Sets the ShowAddnewColumn property
        /// </summary>
        public static readonly DependencyProperty ShowAddNewColumnProperty =
            DependencyProperty.Register("ShowAddNewColumn", typeof(bool), typeof(GanttGrid), new PropertyMetadata(true, OnShowAddNewColumnChanged));

        /// <summary>
        /// Gets or sets a value indicating whether [show add new column].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show add new column]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowAddNewColumn
        {
            get
            {
                return (bool)GetValue(ShowAddNewColumnProperty);
            }
            set
            {
                SetValue(ShowAddNewColumnProperty, value);
            }
        }

        /// <summary>
        /// Called when [show add new column changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowAddNewColumnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttGrid grid = sender as GanttGrid;
            if (grid == null)
                return;

            // Hides/Displays the AddNewColumn
            if (!grid.ShowAddNewColumn)
            {
                grid.Columns.Remove(grid.addNewColumn);
            }
            else
                grid.Columns.Add(grid.addNewColumn);
#if !SILVERLIGHT
            if (grid.IsLoaded && grid.InternalGrid != null)
#else
            if(grid.InternalGrid!=null)
#endif
            {               
                grid.InternalGrid.PopulateGridNodes(true);
                grid.InternalGrid.InvalidateCells();
            }
        }
        #endregion

        #region Header Height Calculation
       
        /// <summary>
        /// Calculates the height of the Grid header.
        /// </summary>
        internal void CalculateHeaderHeight()
        {
            var tempHeight = this.ParentControl.GanttSchedule.Items.Count - 2;
            var tempdefaultHeight = this.defaultGridHeaderHeight;

            tempdefaultHeight += (tempHeight * 20);

            if (this.InternalGrid != null && tempdefaultHeight > 0)
            {
                this.Model.RowHeights[0] = tempdefaultHeight;
                this.InternalGrid.InvalidateCell(GridRangeInfo.Row(0));
                this.InternalGrid.InvalidateVisual(true);
            }
        }
        #endregion

        #region ShowDateWithTime

        /// <summary>
        /// Gets or sets a value indicating whether [show date with time].
        /// </summary>
        /// <value><c>true</c> if [show date with time]; otherwise, <c>false</c>.</value>
        public bool ShowDateWithTime
        {
            get { return (bool)GetValue(ShowDateWithTimeProperty); }
            set { SetValue(ShowDateWithTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowHoursInTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowDateWithTimeProperty =
            DependencyProperty.Register("ShowDateWithTime", typeof(bool), typeof(GanttGrid), new PropertyMetadata(false, OnShowDateWithTimeChanged));

        /// <summary>
        /// Called when [show date with time changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowDateWithTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttGrid gantGrid = sender as GanttGrid;

            if (gantGrid == null || gantGrid.InternalGrid == null)
                return;

            Dictionary<string, string> Columns = gantGrid.ParentControl.TaskAttributeMapping.MappedAttributes;

            if (Columns.Count <= 0)
                return;

            // To change the format and width of the cell
            if (Columns.Keys.Contains("DurationMapping"))
            {
                int index = gantGrid.ColumnNameToPosition(Columns["DurationMapping"]) -1;                
                if((bool)args.NewValue)
                {
                    gantGrid.Columns[index].StyleInfo = new GridStyleInfo { CellType = "TimeSpanEdit", TimeSpanEdit = new GridTimeSpanEditStyleInfo { Format = "d'd 'h'h 'm'm'" } };
                    gantGrid.Columns[index].Width = 110;
                }
                else
                {
                    gantGrid.Columns[index].StyleInfo = new GridStyleInfo { CellType = "TimeSpanEdit", TimeSpanEdit = new GridTimeSpanEditStyleInfo { Format = "d' days'" } };
                    gantGrid.Columns[index].Width = 85;
                }
            }
            
            if (Columns.Keys.Contains("StartDateMapping"))
            {
                 int index = gantGrid.ColumnNameToPosition(Columns["StartDateMapping"]) -1;
                 gantGrid.SetDateTimeStyle(gantGrid.Columns[index]);
            }

            if (Columns.Keys.Contains("FinishDateMapping"))
            {
                int index = gantGrid.ColumnNameToPosition(Columns["FinishDateMapping"]) -1;
                gantGrid.SetDateTimeStyle(gantGrid.Columns[index]);
            }

            // To rearrange the cells based on new width
            gantGrid.InternalGrid.InvalidateCells();
        }

        #endregion

        #endregion 

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GanttGrid"/> class.
        /// </summary>
        public GanttGrid()
        {
            // Initializing default and standard value          
            this.ExpandStateAtStartUp = GridTreeStartUpExpandState.AllNodesExpanded;
            this.NotifyPropertyChanges = true;
            this.ShowRowHeader = true;
            this.ReadOnly = false;
            this.AllowSort = false;

            this.Model.CellModels.Add("PredecessorCell", new PredecessorCellModel());
            this.Model.CellModels.Add("ResourceCell", new ResourceCellModel());            
#if SILVERLIGHT
            this.Model.CellModels.Add("AddNewColumnCell", new AddNewColumnCellModel());
#endif
            // Hook the required events 
            this.WireEvents();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Wires the events.
        /// </summary>
        private void WireEvents()
        {
            // To handle the loaded event of Gantt Grid
            this.Loaded += new RoutedEventHandler(GanttGrid_Loaded);

            // To listen the items source change of Gantt Grid
            this.ItemsSourceChanged += GanttGrid_ItemsSourceChanged;

            // To handle the model loaded event of Gantt Grid
            this.ModelLoaded += new System.EventHandler(GanttGrid_ModelLoaded);

            // To handle the expand state chagned event of Gantt Grid nodes
            this.ExpandStateChanged += GanttGrid_ExpandStateChanged;
        }

        /// <summary>
        /// Wires the internal grid events.
        /// </summary>
        private void WireInternalGridEvents()
        {
            // To handle the Querycell info event of Gantt Grid
            this.InternalGrid.QueryCellInfo += InternalGrid_QueryCellInfo;

            // To listen the vertical scrolling of Gantt Grid
            this.InternalGrid.VScrollBar.PropertyChanged += VScrollBar_PropertyChanged;

#if !SILVERLIGHT
            this.InternalGrid.DropDownSelectionChanged += new GridCellComboValueChangedEventHandler(InternalGrid_DropDownSelectionChanged);
#endif
        }

        /// <summary>
        /// Handles the ModelLoaded event of the GanttGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void GanttGrid_ModelLoaded(object sender, System.EventArgs e)
        {
            // Hooking node selection chagne event
            this.SelectedNodes.CollectionChanged -= SelectedNodes_CollectionChanged;
            this.SelectedNodes.CollectionChanged += SelectedNodes_CollectionChanged;

            InternalGrid.Model.Options.ActivateCurrentCellBehavior = GridCellActivateAction.DblClickOnCell;

            if (isHeaderBackgroundChanged)
            {
                //Model.HeaderStyle.Background = HeaderBackground;   
                this.InternalGrid.ColumnHeaderStyle.Background = HeaderBackground;
                this.InternalGrid.RowHeaderStyle.Background = HeaderBackground;                             
            }

            if (isHeaderForegroundChanged)
            {
                //Model.HeaderStyle.Foreground = Foreground;
                this.InternalGrid.ColumnHeaderStyle.Foreground = HeaderForeground;
                this.InternalGrid.RowHeaderStyle.Foreground = HeaderForeground;
            }

            // Initializing default and standard value
            this.FreezeExpandColumn = false;

            this.InternalGrid.ExpandGlyphType = GridTreeExpandGlyph.PlusMinus;
            this.InternalGrid.ExpandNewNodeOnCreation = true;
            this.InternalGrid.EnableHotRowMarker = false;

#if SILVERLIGHT
            // To avoid overlapping of the text, since Silverlight text box didnt support
            // text trimming we have used this.
            this.Model.TableStyle.TextWrapping = TextWrapping.NoWrap;
#endif

            // Hook the required events 
            this.WireInternalGridEvents();
        }

        /// <summary>
        /// Handles the ExpandStateChanged event of the GanttGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.GridTreeNodeEventArgs"/> instance containing the event data.</param>
        void GanttGrid_ExpandStateChanged(object sender, GridTreeNodeEventArgs e)
        {
            //this.GanttModel.UpdateExpandedState(e.Node.Item, e.Action);
            int recordIndex = this.InternalGrid.GetRecordsIndexFromItem(e.Node.Item);
            //this.GanttModel.SetExpandState(recordIndex, e.Action);
            this.GanttModel.SetExpandState(e.Node.Item, e.Action);
            SetDistance();
        }

#if !SILVERLIGHT
        /// <summary>
        /// Handles the DropDownSelectionChanged event of the InternalGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.GridCellComboValueChangedEventArgs"/> instance containing the event data.</param>
        void InternalGrid_DropDownSelectionChanged(object sender, GridCellComboValueChangedEventArgs args)
        {          
            this.AddNewColumn(args.SelectedItem.ToString());
            this.RefreshGrid();
        }
#endif
        /// <summary>
        /// Handles the ItemsSourceChanged event of the GanttGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.ComponentModel.SyncfusionRoutedEventArgs"/> instance containing the event data.</param>
        void GanttGrid_ItemsSourceChanged(object sender, ComponentModel.SyncfusionRoutedEventArgs args)
        {
            this.ChildPropertyName = this.ParentControl.TaskAttributeMapping.ChildMapping;
            if (this.ChildPropertyName == string.Empty)
                this.ChildPropertyName = "Child";

            if (this.Columns.Count == 0 && this.ParentControl.IsLoadedWithInbuiltSource)
            {
                // To load the Gantt Grid with default columns
                 this.AddDefaultColumns();
            }
            else if (this.Columns.Count == 0 && !this.ParentControl.IsLoadedWithInbuiltSource)
            {
                this.AddDefaultColumns();
            }            
        }
        
        /// <summary>
        /// Handles the Loaded event of the GanttGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void GanttGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // Initializing default and standard value
            this.CalculateHeaderHeight();
            if (this.ParentControl.isRowHeightAppliedBeforeGridLoaded)
            {
                this.Model.RowHeights.DefaultLineSize = this.ParentControl.RowHeight;
               // this.ParentControl.isRowHeightAppliedBeforeGridLoaded = false;
            }
            else
                this.Model.RowHeights.DefaultLineSize = 24d;
            this.Model.RowHeights.DefaultLineSizeChanged += RowHeights_DefaultLineSizeChanged;
            this.Model.ColumnWidths[0] = 42d;

            var r = this.FindElementsOfType<ScrollBar>();
            var en = r.GetEnumerator();
            
            if (this.ExpandStateAtStartUp == GridTreeStartUpExpandState.NoNodesExpanded)
            {

                IEnumerable<GanttRecord> rootNodes = this.GanttModel.ExpandedCollection.Where(rec => rec.ChildRecords.Count > 0);
                for (int i = rootNodes.Count()-1; i >= 0; i--)
                {
                    GanttRecord record=rootNodes.ElementAt(i);
                    this.GanttModel.SetExpandState(record.DataItem, GridTreeNodeActions.Collapsed);
                }
            }
        }

        /// <summary>
        /// Handles the DefaultLineSizeChanged event of the RowHeights control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Scroll.DefaultLineSizeChangedEventArgs"/> instance containing the event data.</param>
        void RowHeights_DefaultLineSizeChanged(object sender, DefaultLineSizeChangedEventArgs e)
        {
            this.ParentControl.RowHeight = (double)e.NewValue;
        }

        /// <summary>
        /// Handles the PropertyChanged event of the VScrollBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void VScrollBar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            List<string> properties = new List<string> {"Minimum", "Maximum", "LargeChange", "SmallChange", "Value"};
            if(!properties.Contains(e.PropertyName))
                return ;

            var scrollbar = sender as ScrollInfo;
            if (ParentControl != null && ParentControl.GanttChartScrollViewer != null)
            {
                var newvalue = (scrollbar.Value - scrollbar.Minimum);
                ParentControl.GanttChartScrollViewer.ScrollToVerticalOffset(newvalue);
            }
        }

        #endregion 

        #region  Query Cell Info

        /// <summary>
        /// Handles the QueryCellInfo event of the InternalGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.GridQueryCellInfoEventArgs"/> instance containing the event data.</param>
        void InternalGrid_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            //// Code for Add New Columnn Header Cell Template 
            if (e.Cell.RowIndex == 0 && e.Cell.ColumnIndex == (this.ColumnNameToPosition("Add New Column")))
            {
                e.Style.ReadOnly = false;
#if !SILVERLIGHT
                e.Style.CellType = "ComboBox";   
#else 
                e.Style.CellType="AddNewColumnCell";                
#endif
                e.Style.ItemsSource = NewColumns;               
            }
          

            if (e.Cell.RowIndex > 0)
            {
                if (this.taskIDColumnIndex > -1)
                {
                    // Populating the Task Id by fetching it from the place holder column
                    if (e.Cell.ColumnIndex == 0)
                    {
                        e.Style.CellValue = this.Model[e.Cell.RowIndex, taskIDColumnIndex + 1].CellValue;
                        e.Style.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    }
                }

                if (oldRowIndex != e.Cell.RowIndex)
                {
                    currentNode = this.InternalGrid.GetNodeAtRowIndex(e.Cell.RowIndex);
                    oldRowIndex = e.Cell.RowIndex;
                }

                // Disable Editing of Parent Task Fields Except Name,Predecessor,ResourceInfo
                if (currentNode != null && currentNode.HasChildNodes)
                {
                    e.Style.Font = new GridFontInfo { FontWeight = FontWeights.Bold };
                    if ((e.Cell.ColumnIndex == this.ColumnNameToPosition(this.ParentControl.TaskAttributeMapping.TaskNameMapping)))
                        e.Style.ReadOnly = false;
                    else if (e.Cell.ColumnIndex == this.ColumnNameToPosition(this.ParentControl.TaskAttributeMapping.PredecessorMapping))
                        e.Style.ReadOnly = false;                            
                    else
                        e.Style.ReadOnly = true;
                }
               
                // The Following calculations are used to Display  the Variances in Grid
                if (e.Cell.ColumnIndex != 0)
                {
                    // If current cell is Start Variance Column cell, then Calculates Variance
                    if (e.Cell.ColumnIndex == this.ColumnNameToPosition("StartVariance"))
                    {
                        if (stDtColumnIndex == -1 || baseStDtColumnIndex == -1)
                            e.Style.CellValue = new TimeSpan(0, 0, 0, 0);
                        else
                            e.Style.CellValue = this.GetDateVariance(e.Cell.RowIndex, stDtColumnIndex, baseStDtColumnIndex);
                        e.Style.ReadOnly = true;
                    }

                    // If current cell is Finish Variance Column cell, then Calculates Variance
                    if (e.Cell.ColumnIndex == this.ColumnNameToPosition("FinishVariance"))
                    {
                        if (fsDtColumnIndex == -1 || baseFsDtColumnIndex == -1)
                            e.Style.CellValue = new TimeSpan(0, 0, 0, 0);
                        else
                            e.Style.CellValue = this.GetDateVariance(e.Cell.RowIndex, fsDtColumnIndex, baseFsDtColumnIndex);
                        e.Style.ReadOnly = true;
                    }

                    // If current cell is Cost Variance Column cell, then Calculates Variance
                    if (e.Cell.ColumnIndex == this.ColumnNameToPosition("CostVariance"))
                    {
                        if (cstColumnIndex == -1 || baseCstColumnIndex == -1)
                            e.Style.CellValue = 0d;
                        else
                            e.Style.CellValue = this.GetCostVariance(e.Cell.RowIndex, cstColumnIndex, baseCstColumnIndex);
                        e.Style.ReadOnly = true;
                    }                    
                }  
                
            }
        }

        #endregion

        #region Overriden Methods

        /// <summary>
        /// Called when [apply template].
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

#if !SILVERLIGHT
            GridScrollViewer = this.FindElementOfType<ScrollViewer>();
#else
            GridScrollViewer=this.FindElementOfType<ScrollableContentViewer>();
#endif
            if (GridScrollViewer != null)
            {
                GridScrollViewer.Loaded += GridScrollViewer_Loaded;
            }

            this.isGridLoaded = true;
        }

        /// <summary>
        /// Handles the Loaded event of the GridScrollViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void GridScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
#if !SILVERLIGHT
            var HScrollBar = (ScrollBar)GridScrollViewer.Template.FindName("PART_HorizontalScrollBar", GridScrollViewer);

            if (HScrollBar != null)
            {
                HScrollBar.IsVisibleChanged += HScrollBar_IsVisibleChanged;
            }
#endif
            SetDistance();
        }

#if !SILVERLIGHT
        /// <summary>
        /// Handles the IsVisibleChanged event of the HScrollBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void HScrollBar_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetDistance();
        }
#endif

        /// <summary>
        /// Expand all nodes.
        /// </summary>
        public new void ExpandAllNodes()
        {
            if (!isGridLoaded)
                return;

            bool hasNode = false;
            foreach (GridTreeNode rootNode in this.InternalGrid.RootNodes)
            {
                // To avoid Coalition problem between expand collapse opperation made in UI
                if (rootNode.Expanded)
                    continue;
                
                if (!rootNode.HasChildNodes)
                {
                    this.InternalGrid.ResetGrid();
                    continue;
                }

                ExpandAllNodes(rootNode);
                hasNode = true;
            }
            if (hasNode)
            {
                this.InternalGrid.UnloadArrangedCells();
                this.InternalGrid.InvalidateCells();
                this.InternalGrid.InvalidateVisual(true);
            }
        }

        /// <summary>
        /// Expands the given node and all of its child nodes.
        /// </summary>
        /// <param name="n">The node to be expanded.</param>
        public new void ExpandAllNodes(GridTreeNode n)
        {
            if (!isGridLoaded || n == null || n.Expanded)
                return;

            SetAllNodeExpandValues(true, n);
            this.InternalGrid.ResetGrid();
            this.InternalGrid.CollapseNode(n);
            this.InternalGrid.ExpandNode(n);
        }

        /// <summary>
        /// Collapses all expanded nodes.
        /// </summary>
        public new void CollapseAllNodes()
        {
            if (!isGridLoaded)
                return;

            foreach (GridTreeNode rootNode in this.InternalGrid.RootNodes)
            {
                // To avoid Coalition problem between expand collapse opperation made in UI
                if (rootNode.Expanded)
                    CollapseAllNodes(rootNode);
            }
        }

        /// <summary>
        /// Collapse all passed-in node as well as child nodes of the passed-in node.
        /// </summary>
        /// <param name="n">The node to be collapsed.</param>
        public new void CollapseAllNodes(GridTreeNode n)
        {
            if (!isGridLoaded || n== null || !n.Expanded)
                return;

            SetAllNodeExpandValues(false, n);
            this.InternalGrid.ReloadNodes();
            this.InternalGrid.ResetGrid();
            this.InternalGrid.InvalidateCells();
            this.InternalGrid.InvalidateVisual(true);
        }

        /// <summary>
        /// Sets all node expand values.
        /// </summary>
        /// <param name="expand">if set to <c>true</c> [expand].</param>
        /// <param name="n">The n.</param>
        private void SetAllNodeExpandValues(bool expand, GridTreeNode n)
        {
            if (n.ChildNodes != null && n.ChildNodes.Count > 0)
            {
                if (expand)
                {
                    OnExpandStateChanging(n, GridTreeNodeActions.Expanding);
                    n.Expanded = expand;
                    OnExpandStateChanged(n, GridTreeNodeActions.Expanded);
                }

                foreach (GridTreeNode n1 in n.ChildNodes)
                {
                    // To avoid Coalition problem between expand collapse opperation made in UI
                    if (n1.Expanded != expand)
                        SetAllNodeExpandValues(expand, n1);
                }

                if (!expand)
                {
                    OnExpandStateChanging(n, GridTreeNodeActions.Collapsing);
                    n.Expanded = expand;
                    OnExpandStateChanged(n, GridTreeNodeActions.Collapsed);
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Refreshes the grid.
        /// </summary>
        internal void RefreshGrid()
        {
            this.InternalGrid.PopulateGridNodes(false,true);
            this.InternalGrid.InvalidateCells();
        }

        /// <summary>
        /// Calculating Start and Finish Variances
        /// </summary>
        /// <param name="Row">current row of Grid</param>
        /// <param name="Column1">contains column value of  The Date from which the baseline Date is subtracted</param>
        /// <param name="Column2">contanins the value of the baseline Date</param>
        /// <returns>Timespan</returns>
        private TimeSpan GetDateVariance(int Row, int Column1, int Column2)
        {
            DateTime Date1, Date2;
            DateTime.TryParse((this.Model[Row, Column1].CellValue.ToString()), out Date1);
            DateTime.TryParse(this.Model[Row, Column2].CellValue.ToString(), out Date2);
            if (Date1 == DateTime.MinValue || Date2 == DateTime.MinValue)
                return new TimeSpan(0, 0, 0, 0);
            return Date1.Subtract(Date2);
        }

        /// <summary>
        /// Calculates cost variances
        /// </summary>
        /// <param name="Row">current row of Grid</param>
        /// <param name="Column1">contains column value from which the baseline cost is subtracted</param>
        /// <param name="Column2">contains column value of the baseline cost</param>
        /// <returns>Double</returns>
        private Double GetCostVariance(int Row, int Column1, int Column2)
        {
            Double cost, basecost;
            Double.TryParse(this.Model[Row, Column1].CellValue.ToString(), out cost);
            Double.TryParse(this.Model[Row, Column2].CellValue.ToString(), out basecost);
            return cost - basecost;
        }

        /// <summary>
        /// Adds the Default mapped columns.
        /// </summary>
        private void AddDefaultColumns()
        {
            bool canAddColumn = false;
            bool canContinue = false;                      
            foreach (KeyValuePair<string, string> column in this.ParentControl.TaskAttributeMapping.MappedAttributes)
            {
                if (column.Key.Equals("ChildMapping"))
                    continue;
                
                GridTreeColumn gridColumn = new GridTreeColumn(column.Value);

                // Adding column to display the Task ID 
                if (column.Key.Equals("TaskIdMapping"))
                {
                    gridColumn.Width = 0d;
                    canAddColumn = true; canContinue = true;
                    this.taskIDColumnIndex = this.Columns.Count;
                }

                // Adding column to display Task Name
                else if (column.Key.Equals("TaskNameMapping"))
                {
                    gridColumn.HeaderText = "Task Name";
                    gridColumn.StyleInfo = new GridStyleInfo { VerticalAlignment = System.Windows.VerticalAlignment.Center };
                    // assigning width to the expader cell here, will result to unexpected long width 
                    // hence the following code commented
                    //gridColumn.Width = 180;
                    canAddColumn = true;
                    canContinue = true;

                    if (this.Columns.Count > 0)
                        this.Columns.Insert(0, gridColumn);

                    if (this.taskIDColumnIndex == 0)
                        this.taskIDColumnIndex = 1;
                    else if (this.taskIDColumnIndex > 0)
                        this.taskIDColumnIndex++;

                    if (this.taskIDColumnIndex != -1)
                    {
                        canContinue = false;
                        continue;
                    }
                }

                // Adding column to display the duration to complete the task
                else if (column.Key.Equals("DurationMapping"))
                {
                    gridColumn.HeaderText = "Duration";
                    gridColumn.StyleInfo = new GridStyleInfo { VerticalAlignment = System.Windows.VerticalAlignment.Center };
                    SetTimeSpanStyle(gridColumn);
                    canContinue = true;
                }

                // Adding column to display the start date of the task
                else if (column.Key.Equals("StartDateMapping"))
                {
                    gridColumn.HeaderText = "Start";
                    SetDateTimeStyle(gridColumn);
                    canAddColumn = true; canContinue = true;
                }
                // Adding columns to display the finish date of the task
                else if (column.Key.Equals("FinishDateMapping"))
                {
                    gridColumn.HeaderText = "Finish";
                    SetDateTimeStyle(gridColumn);
                    canAddColumn = true; canContinue = true;
                }

                // Adding columns to display the Progress of the Tasks
                else if (column.Key.Equals("ProgressMapping"))
                {
                    gridColumn.HeaderText = "Progress (%)";
                    gridColumn.StyleInfo = new GridStyleInfo() { HorizontalAlignment = System.Windows.HorizontalAlignment.Right , VerticalAlignment= System.Windows.VerticalAlignment.Center};
                    canContinue = true;
                }

                // Adding columns to display the predecessor of the task
                else if (column.Key.Equals("PredecessorMapping"))
                {
                    gridColumn.HeaderText = "Predecessor";
                    gridColumn.Width = 110d; canContinue = true;
                    gridColumn.StyleInfo = new GridStyleInfo { CellType = "PredecessorCell", VerticalAlignment= System.Windows.VerticalAlignment.Center };
                }

                // Adding columns to display the allocated resource for the task
                else if (column.Key.Equals("ResourceInfoMapping"))
                {
                    gridColumn.HeaderText = "Resource Names";
                    gridColumn.Width = 130d; canContinue = true;
                    gridColumn.StyleInfo = new GridStyleInfo { CellType = "ResourceCell", ReadOnly= true, VerticalAlignment= System.Windows.VerticalAlignment.Center };
                }

                // Adding column to display the Cost of the tasks.
                else if (column.Key.Equals("CostMapping") && !ParentControl.IsLoadedWithInbuiltSource)
                {
                    gridColumn.HeaderText = "Cost";
                    gridColumn.StyleInfo = new GridStyleInfo { CellType = "CurrencyEdit", VerticalAlignment= System.Windows.VerticalAlignment.Center, HorizontalAlignment= System.Windows.HorizontalAlignment.Right, NumberFormat = new System.Globalization.NumberFormatInfo { CurrencyDecimalDigits = 2, CurrencyDecimalSeparator = ".", CurrencyNegativePattern = 0, CurrencyPositivePattern = 0, CurrencySymbol = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencySymbol } };
                    this.IsCostColumnExists = true;
                    canAddColumn = true; canContinue = true;
                }
                else if(column.Key.Equals("MileStoneMapping"))
                {
                    gridColumn.HeaderText="IsMileStone";
                    gridColumn.StyleInfo = new GridStyleInfo() { CellType = "CheckBox",IsThreeState=false, HorizontalAlignment= System.Windows.HorizontalAlignment.Center, VerticalAlignment= System.Windows.VerticalAlignment.Center};
                    canAddColumn = true;
                    canContinue = true;
                }

                // Adding column to display the Start Point
                else if (column.Key.Equals("StartPointMapping"))
                {
                    gridColumn.HeaderText = "Start";
                    gridColumn.StyleInfo = new GridStyleInfo() { HorizontalAlignment = System.Windows.HorizontalAlignment.Right, VerticalAlignment= System.Windows.VerticalAlignment.Center };
                    canContinue = true;
                }

                // Adding column to display the Finish Point
                else if (column.Key.Equals("FinishPointMapping"))
                {
                    gridColumn.HeaderText = "Finish";
                    gridColumn.StyleInfo = new GridStyleInfo() { HorizontalAlignment = System.Windows.HorizontalAlignment.Right, VerticalAlignment= System.Windows.VerticalAlignment.Center };
                    canContinue = true;
                }

                // If LoadVarianceTable Method calls this method, only adds columns that can be Added 
                if (IsShowTableView && canAddColumn)
                {
                    this.Columns.Add(gridColumn);
                    canAddColumn = false;
                }

                else if (!IsShowTableView)
                {
                    if (canContinue)
                    {
                        canContinue = false;
                        this.Columns.Add(gridColumn);
                        continue;
                    }
                    else
                        continue;
                }
            }

            // Sets Column Indexes for Variance Calculations.
            this.SetVarianceIndexes();

            // Creates the ItemsSource for AddNewColumn ComboBox.
            this.CreateNewColumns(); 

            // Add Column to Display Add New Column   
            if (CanCreateAddNewColumn && this.ShowAddNewColumn)
            {                
                this.Columns.Add(addNewColumn);
            }
           
        }
      
        /// <summary>
        /// Checks and adds the Selected Column
        /// </summary>
        /// <param name="colName">Selected Column Name</param>
        internal void AddNewColumn(string colName)
        {
            // Gets the Original Mapping Name of the Column given.
            string columnName = this.GetMappingName(colName);

            if (String.IsNullOrEmpty(colName) || String.IsNullOrEmpty(columnName))
                return;
            // Checks the Existence of Selected Column
            var res = this.Columns.Where((col) =>
            {
                if (col.MappingName == null)
                    return false;
                if (col.MappingName.Equals(columnName))
                    return true;

                return false;
            });

            // Deactivates the Current cell to Insert the Columns
            this.InternalGrid.CurrentCell.Deactivate();

            if (res == null || res.Count() <= 0)
            {
                GridTreeColumn tempGridColumn = new GridTreeColumn();

                // Ensures the Required Column is in the NewColumns...
                if (NewColumns.Contains(colName))
                {
                    // Adding Column To display Cost
                    if (colName.Equals("Cost") && ParentControl.IsLoadedWithInbuiltSource)
                    {
                        tempGridColumn.HeaderText = colName;
                        tempGridColumn.MappingName = columnName;
                        tempGridColumn.StyleInfo = new GridStyleInfo { CellType = "CurrencyEdit", HorizontalAlignment= System.Windows.HorizontalAlignment.Right, VerticalAlignment= System.Windows.VerticalAlignment.Center, NumberFormat = new System.Globalization.NumberFormatInfo { CurrencyDecimalDigits = 2, CurrencyDecimalSeparator = ".", CurrencyNegativePattern = 0, CurrencyPositivePattern = 0, CurrencySymbol = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencySymbol } };
                        this.IsCostColumnExists = true;
                    }

                    // Adding Column to display Baseline start 
                    else if (colName.Equals("Baseline Start"))
                    {
                        tempGridColumn.HeaderText = colName;
                        tempGridColumn.MappingName = columnName;
                        this.SetDateTimeStyle(tempGridColumn);
                    }

                   // Adding Column to display Baseline Finish  
                    else if (colName.Equals("Baseline Finish"))
                    {
                        tempGridColumn.HeaderText = colName;
                        tempGridColumn.MappingName = columnName;
                        this.SetDateTimeStyle(tempGridColumn);
                    }

                    // Adding Column to Display Baseline Cost
                    else if (colName.Equals("Baseline Cost"))
                    {
                        tempGridColumn.HeaderText = colName;
                        tempGridColumn.MappingName = columnName;
                        tempGridColumn.StyleInfo = new GridStyleInfo { CellType = "CurrencyEdit",HorizontalAlignment= System.Windows.HorizontalAlignment.Right, VerticalAlignment= System.Windows.VerticalAlignment.Center, NumberFormat = new System.Globalization.NumberFormatInfo { CurrencyDecimalDigits = 2, CurrencyDecimalSeparator = ".", CurrencyNegativePattern = 0, CurrencyPositivePattern = 0, CurrencySymbol = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencySymbol } };
                    }

                    // Adding Column to Display Start Variance
                    else if (colName.Equals("Start Variance"))
                    {
                        tempGridColumn.HeaderText = colName;
                        tempGridColumn.MappingName = columnName;
                        this.SetTimeSpanStyle(tempGridColumn);
                    }

                    // Adding Column to Display Finish Variance
                    else if (colName.Equals("Finish Variance"))
                    {
                        tempGridColumn.HeaderText = colName;
                        tempGridColumn.MappingName = columnName;
                        this.SetTimeSpanStyle(tempGridColumn);
                    }

                    // Adding Column to Display Cost Variance
                    else if (colName.Equals("Cost Variance"))
                    {
                        tempGridColumn.HeaderText = colName;
                        tempGridColumn.MappingName = columnName;
                        tempGridColumn.StyleInfo = new GridStyleInfo { CellType = "CurrencyEdit", HorizontalAlignment= System.Windows.HorizontalAlignment.Right, VerticalAlignment= System.Windows.VerticalAlignment.Center, NumberFormat = new System.Globalization.NumberFormatInfo { CurrencySymbol = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencySymbol }, ReadOnly = true };
                    }

                    if (!ParentControl.ShowAddNewColumn) 
                        this.Columns.Add(tempGridColumn);                   
                    
                    else                    
                        this.Columns.Insert(this.Columns.Count - 1, tempGridColumn);                    

                    // Refreshing Column Indexes.
                    this.SetVarianceIndexes();
                }
            }
            
        }

        /// <summary>
        /// Sets the date time style.
        /// </summary>
        /// <param name="gridColumn">The grid column.</param>
        private void SetDateTimeStyle(GridTreeColumn gridColumn)
        {
            if (this.ShowDateWithTime)
            {
                gridColumn.StyleInfo = new GridStyleInfo
                {
                    HorizontalAlignment= System.Windows.HorizontalAlignment.Right,
                    VerticalAlignment= System.Windows.VerticalAlignment.Center,
                    CellType = "DateTimeEdit",
#if !SILVERLIGHT
                    DateTimeEdit = new GridDateTimeEditStyleInfo { IsVisibleRepeatButton = false, DateTimePattern = Shared.DateTimePattern.CustomPattern, CustomPattern = "MM/dd/yyyy hh:mm tt" }
#else
                    DateTimeEdit = new GridDateTimeEditStyleInfo { IsVisibleRepeatButton = false, DateTimePattern = Tools.Controls.DateTimePattern.CustomPattern, CustomPattern = "MM/dd/yyyy hh:mm tt" }
#endif
                };
                gridColumn.Width = 140;
            }
            else
            {
                gridColumn.StyleInfo = new GridStyleInfo { CellType = "DateTimeEdit", VerticalAlignment= System.Windows.VerticalAlignment.Center,HorizontalAlignment= System.Windows.HorizontalAlignment.Right, DateTimeEdit = new GridDateTimeEditStyleInfo { IsVisibleRepeatButton = false } };
                gridColumn.Width = 85;
            }
        }

        /// <summary>
        /// Sets the time span style.
        /// </summary>
        /// <param name="gridColumn">The grid column.</param>
        private void SetTimeSpanStyle(GridTreeColumn gridColumn)
        {
            if (this.ShowDateWithTime)
            {
                gridColumn.StyleInfo = new GridStyleInfo { CellType = "TimeSpanEdit", VerticalAlignment= System.Windows.VerticalAlignment.Center, TimeSpanEdit = new GridTimeSpanEditStyleInfo { Format = "d'd 'h'h 'm'm'" } };
                gridColumn.Width = 110;
            }
            else
            {
                gridColumn.StyleInfo = new GridStyleInfo { CellType = "TimeSpanEdit", VerticalAlignment= System.Windows.VerticalAlignment.Center, TimeSpanEdit = new GridTimeSpanEditStyleInfo { Format = "d' days'" }};
                gridColumn.Width = 85;
            }
        }

        /// <summary>
        /// Gets the Mapping Name of  the Given Column
        /// </summary>
        /// <param name="colname">ColumnName</param>
        /// <returns>Mapped Name</returns>
        private string GetMappingName(string colname)
        {
            if (colname == null)
                return string.Empty;
            else if (colname.Equals("Cost"))
                return this.ParentControl.TaskAttributeMapping.CostMapping;
            else if (colname.Equals("Baseline Start"))
                return this.ParentControl.TaskAttributeMapping.BaselineStartMapping;
            else if (colname.Equals("Baseline Finish"))
                return this.ParentControl.TaskAttributeMapping.BaselineFinishMapping;
            else if (colname.Equals("Baseline Cost"))
                return this.ParentControl.TaskAttributeMapping.BaselineCostMapping;
            else if (colname.Equals("Start Variance"))
                return "StartVariance";
            else if (colname.Equals("Finish Variance"))
                return "FinishVariance";
            else if (colname.Equals("Cost Variance"))
                return "CostVariance";
            else
                return string.Empty;
            
        }

        /// <summary>
        /// Creates the ItemsSource for AddNewColumn ComboBox
        /// </summary>
        private void CreateNewColumns()
        {
            // Creates the AddNew column only for dateTime schedules
            if (this.ParentControl.ScheduleType != ScheduleType.CustomNumeric)
            {
                if (!ParentControl.IsLoadedWithInbuiltSource)
                {
                    if (ParentControl.TaskAttributeMapping.MappedAttributes.ContainsKey("BaselineStartMapping"))
                    {
                        NewColumns[0] = "Baseline Start";
                        NewColumns[3] = "Start Variance";
                        CanCreateAddNewColumn = true;
                    }
                    if (ParentControl.TaskAttributeMapping.MappedAttributes.ContainsKey("BaselineFinishMapping"))
                    {
                        NewColumns[1] = "Baseline Finish";
                        NewColumns[4] = "Finish Variance";
                        CanCreateAddNewColumn = true;
                    }
                    if (ParentControl.TaskAttributeMapping.MappedAttributes.ContainsKey("BaselineCostMapping") &&
                        ParentControl.TaskAttributeMapping.MappedAttributes.ContainsKey("CostMapping"))
                    {
                        NewColumns[2] = "Baseline Cost";
                        NewColumns[5] = "Cost Variance";
                        CanCreateAddNewColumn = true;
                    }
                }
                else
                {
                    NewColumns = new string[] { "Cost", "Baseline Start", "Baseline Finish", "Baseline Cost", "Start Variance", "Finish Variance", "Cost Variance" };
                    CanCreateAddNewColumn = true;
                }
            }
        }

        /// <summary>
        /// Sets the Column Index for Corresponding Variables
        /// </summary>
        private void SetVarianceIndexes()
        {
            foreach (KeyValuePair<string, string> column in this.ParentControl.TaskAttributeMapping.MappedAttributes)
            {
                string columnName=column.Value;

                if (column.Key.Equals("StartDateMapping"))
                    this.stDtColumnIndex = this.ColumnNameToPosition(columnName);

                else if (column.Key.Equals("FinishDateMapping"))
                    this.fsDtColumnIndex = this.ColumnNameToPosition(columnName);

                else if (column.Key.Equals("CostMapping"))
                    this.cstColumnIndex = this.ColumnNameToPosition(columnName);

                else if (column.Key.Equals("BaselineStartMapping"))
                    this.baseStDtColumnIndex = this.ColumnNameToPosition(columnName);

                else if (column.Key.Equals("BaselineFinishMapping"))
                    this.baseFsDtColumnIndex = this.ColumnNameToPosition(columnName);

                else if (column.Key.Equals("BaselineCostMapping"))
                    this.baseCstColumnIndex = this.ColumnNameToPosition(columnName);
                else
                    continue;
            }
        }        

        /// <summary>
        /// Display the Variances in Table View
        /// </summary>
        internal void LoadVarianceTableView()
        {
            if (!this.isInDefaultView)
                return;

            // Starting Load Variance View
            IsShowTableView = true;
            this.ExistingColumns.Clear();
            this.SetVarianceIndexes();
            foreach (GridTreeColumn gridcolumn in this.Columns)
            {
                if (!ExistingColumns.ContainsKey(gridcolumn) && !gridcolumn.Equals(this.addNewColumn))
                {
                    this.ExistingColumns.Add(gridcolumn, string.Empty);
                }
            }

            // Removes the Existing Columns 
            this.Columns.Clear();

            // Adds Default Columns Respect to VarianceView
            this.AddDefaultColumns();

            // Adds the Variance Columns.
            foreach (string columnName in NewColumns)
            {
                // Checks for Existence of 'Cost' Column [If Cost Column Exists then Adds BaselineCost and Cost Variance columns]
                if (!IsCostColumnExists && (columnName == "Cost" || columnName == "Baseline Cost" || columnName == "Cost Variance"))
                    continue;
                this.AddNewColumn(columnName);
            }

            // Populating Grid with newly added columns               
            this.InternalGrid.PopulateGridNodes(false, true);
            this.InternalGrid.InvalidateCells();

            // Ending Load Variance View
            this.IsShowTableView = false;

            // In variance view the Variance column's Parent tasks fields are not get updated when changing start date/finish date/cost
            // Since we are disabling the Editing of Grid in Variance view.
            this.ReadOnly = true;

            // Preventing loading this view once again.
            this.isInDefaultView = false;
        }
               
        /// <summary>
        /// Reloads the DefaultView
        /// </summary>
        internal void LoadDefaultTableView()
        {
            if (this.isInDefaultView)
                return;
            // Removes the Existing Columns in Grid
            this.Columns.Clear();

            // Reverting back the Readonly property.. the grid now can be editable.
            this.ReadOnly = false;

            // Adding Additional Existing Columns to Grid.
            foreach (KeyValuePair<GridTreeColumn, string> gridTree in this.ExistingColumns)
            {
                this.Columns.Add(gridTree.Key);
            }

            // Adds the AddNewColumn to Grid
            if (this.ShowAddNewColumn)
                this.Columns.Add(addNewColumn);

            // Refresh Column Indexes
            this.SetVarianceIndexes();

            // Populate Grid With Newly Added Columns.            
            this.InternalGrid.PopulateGridNodes(false, true);
            this.InternalGrid.InvalidateCells();

            // Preventing loading this view once again
            this.isInDefaultView = true;
        }

        /// <summary>
        /// Getting the index of the Column by column Name
        /// </summary>
        /// <param name="columnName">The Column Name</param>
        /// <returns>Index of the Column[int]</returns>
        internal int ColumnNameToPosition(string columnName)
        {
            foreach (GridTreeColumn tc in this.Columns)
            {
                if (tc.MappingName == columnName)
                {
                    return this.Columns.IndexOf(tc) + 1;
                }
            }
            return -1;
        }

        /// <summary>
        /// Sets the distance.
        /// </summary>
        internal void SetDistance()
        {
            LineSizeCollection lines = this.InternalGrid.Model.RowHeights as LineSizeCollection;

            //Condition added to avoid the range setting for Header Row. Otherwise it will crop the text inside the Header Row.
            if (lines.LineCount <= 1)
                return;

            //When the Horizontal Scroll bar is visible in the GanttChart and not visible in Gantt Grid teh last row node will be overlapped by the Horizontal Scroll Bar.
            //To prevent the we increasing the distance of the last row in GanttGrid with scroll bar distance.
            if (this.GridScrollViewer.ComputedHorizontalScrollBarVisibility != System.Windows.Visibility.Visible && this.ParentControl.ScheduleViewScrollViewer.ComputedHorizontalScrollBarVisibility == System.Windows.Visibility.Visible)
            {
                lines.Distances.SetRange(lines.LineCount - 1, lines.LineCount - 1, lines.DefaultLineSize + 17);
            }
            else
                lines.Distances.SetRange(lines.LineCount - 1, lines.LineCount - 1, lines.DefaultLineSize);
        }

        #endregion 

        #region Seleted Items
        internal bool isinexternalselection = false;
        /// <summary>
        /// Handles the CollectionChanged event of the SelectedNodes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void SelectedNodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.GanttModel.IsInSelection)
                return;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                this.GanttModel.SyncSelectedItems(e.NewItems, e.Action);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                this.GanttModel.SyncSelectedItems(e.OldItems, e.Action);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.GanttModel.SyncSelectedItems(null, e.Action);
            }

            // This code has been added to move the current cell within selection region, need to remove once this get fixed in Grid soruce
#if !SILVERLIGHT
            if (this.SelectedNodes.Count > 0  && isinexternalselection)
#else
            //Checking Checkbox set the SelectedItem from CurrentCellMoving
            if (this.SelectedNodes.Count > 0 && !this.InternalGrid.CurrentCell.IsInMove)
#endif
            {
                int colIndex = this.InternalGrid.CurrentCell.ColumnIndex;
                int rowIndex = this.InternalGrid.GetRowIndexFromItem(SelectedNodes[this.SelectedNodes.Count - 1].Item);
                this.InternalGrid.CurrentCell.MoveTo(rowIndex, colIndex);
            }
        }

        /// <summary>
        /// Syncs the selected items.
        /// </summary>
        /// <param name="objects">The objects.</param>
        /// <param name="ChangeAction">The change action.</param>
        internal void SyncSelectedItems(IEnumerable objects, NotifyCollectionChangedAction ChangeAction)
        {
            this.GanttModel.IsInSelection = true;

            if (ChangeAction == NotifyCollectionChangedAction.Reset)
            {
                this.SelectedNodes.Clear();
                this.GanttModel.IsInSelection = false;
                return;
            }

            foreach (object obj in objects)
            {
                if (ChangeAction == NotifyCollectionChangedAction.Add)
                {
                    this.AddSelectedItem(obj);
                }
                else if (ChangeAction == NotifyCollectionChangedAction.Remove)
                {
                    this.RemoveSelectedItem(obj);
                }
            }

            this.GanttModel.IsInSelection = false;
        }

        /// <summary>
        /// Adds the selected item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        internal void AddSelectedItem(object obj)
        {
            ///// To Sync the selected item of Gantt chart and Gantt Grid
            int rowIndex = this.InternalGrid.GetRowIndexFromItem(obj);
            if (rowIndex > -1)
            {
                GridTreeNode node = this.InternalGrid.GetNodeAtRowIndex(rowIndex);
                if (node == null)
                    return;
                isinexternalselection = true;
                if (!this.SelectedNodes.Contains(node))
                {
                    this.SelectedNodes.Add(node);
                }
                isinexternalselection = false;
                // To refresh the code behind selection ie, selection from Gantt Chart.
               this.InternalGrid.InvalidateVisual();
            }
        }

        /// <summary>
        /// Removes the selected item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        internal void RemoveSelectedItem(object obj)
        {
            ///// To Sync the selected item of Gantt chart and Gantt Grid
            int rowIndex = this.InternalGrid.GetRowIndexFromItem(obj);
            if (rowIndex > -1)
            {
                GridTreeNode node = this.InternalGrid.GetNodeAtRowIndex(rowIndex);

                if (node == null)
                    return;
                isinexternalselection = true;
                if (this.SelectedNodes.Contains(node))
                {
                    this.SelectedNodes.Remove(node);
                }
                isinexternalselection = false;
                this.InternalGrid.InvalidateVisual();
            }
        }

        #endregion 
    }
}
