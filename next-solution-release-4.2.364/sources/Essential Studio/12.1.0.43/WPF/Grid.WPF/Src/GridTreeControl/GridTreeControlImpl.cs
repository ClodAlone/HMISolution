#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
#if !SILVERLIGHT
using System.Data;
#else
// using System.Reflection;
#endif
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Styles;
using Syncfusion.Windows.Diagnostics;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Collections.Specialized;
using Syncfusion.Linq;
using System.Windows.Shapes;

using Syncfusion.Windows.Data;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Text;
using System.Collections.ObjectModel;

#if ENABLE_PARTIAL_TRUST
using System.Security;
#endif

#if SILVERLIGHT
// using Syncfusion.Windows.Data;
using ArrayList = System.Collections.Generic.List<object>;
using System.Windows.Controls;
#endif

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// A GridControl derived class that displays multicolumn tree data in a tree-like grid. You populate the tree
    /// by handling a single event, RequestTreeItems in which you return a list of object belong to a particular node.
    /// </summary>
    /// <remarks>
    /// While handling RequestTreeItems is the only thing required for data to appear in your tree, you can use additional
    /// property setting to customized the look and feel of the tree. You use the Columns collection to define content and order
    /// of the multiple columns appearing in the tree. For each column, you can specify Column.StyleInfo which is a GridStyleInfo
    /// object that controls the grid cell display properties (like BackColor, Format, Fonts, etc.) of a the column.
    /// 
    /// The TreeGrid also supports sorting by clicking on a column header. There are also tree-wide properties like ReadOnly, 
    /// SupportRowSizing, MarkRowBrush, ColumnHeaderStyle, RowHeaderStyle, FreezeExpandColumn, LevelStyles, RowHeaderWidth,
    /// ShowColumnHeaders, ShowColumnHeaderBorders, SortingEnabled and ShowRowHeaders that allow you to control the look and
    /// feel of the tree grid.
    /// 
    /// The TreeGridControl is bound to a collection of GridNode objects found in TreeGridControl.Nodes. There is a one to one mapping between the
    /// rows in the underlying GridControl and GridNodes in TreeGridControl.Nodes. Given a GridNode, you can get at the underlying data object
    /// passed in via the RequestTreeItems event by referencing GridNode.Item.
    /// </remarks>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif    

#if ENABLE_PARTIAL_TRUST
    [SecuritySafeCritical]
#endif
    public class GridTreeControlImpl : GridControl, ISupportsRecordSelection<GridTreeNode>, ISupportsSortStates//,IXmlSerializable
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridTreeControlImpl()
            : base()
        {
            //GridTreeModel model = new GridTreeModel();
            //InitializeTreeModel(model);
            //this.Model = model;
            this.CachedStorage = new Dictionary<string, object>();//Temporary storage for RowCache
            //for getting mouse point
#if SILVERLIGHT
           DependencyObjectExtensions.SetEnableMousePosition(Application.Current.RootVisual, true);
#endif
        }

        //changes the selection range at runtime
        void SelectedNodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (!this.EnableNodeSelection)
            //    return;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
#if SILVERLIGHT
                    foreach (var NodeList in e.NewItems)
                    {
                        if (NodeList is GridTreeNode)
                        {
                            this.Model.InvalidateCell(GridRangeInfo.Row(this.GetRowIndexFromNode(NodeList as GridTreeNode)));
                            //if ((NodeList as GridTreeNode).Item != null)
                            //{
                            //    int rowIndex = this.GetRowIndexFromItem((NodeList as GridTreeNode).Item);
                            //    //Selected ranges without RowHeader range
                            //    //int x = this.ShowRowHeaders == true ? 1 : 0;
                            //    GridRangeInfo range = new GridRangeInfo(GridRangeInfoType.Rows, rowIndex);
                            //    if (!this.Model.SelectedRanges.Contains(range))
                            //    {
                            //        this.Model.SelectedRanges.Add(range);
                            //        this.Model.InvalidateCell(range);
                            //    }
                            //}
                            break;
                        }
                    }
                     //   foreach (var node in NodeList as List<GridTreeNode>)
                       
#else
                    foreach (var node in e.NewItems)
                    {
                        if ((node as GridTreeNode).Item != null)
                        {
                            this.Model.InvalidateCell(GridRangeInfo.Row(this.GetRowIndexFromNode(node as GridTreeNode)));
                            //int rowIndex = this.GetRowIndexFromItem((node as GridTreeNode).Item);
                            ////Selected ranges without RowHeader range
                            ////int x = this.ShowRowHeaders == true ? 1 : 0;
                            //GridRangeInfo range = new GridRangeInfo(GridRangeInfoType.Rows, rowIndex);//rowIndex, 1, rowIndex, this.Model.ColumnCount - 1);
                            //if (!this.Model.SelectedRanges.Contains(range))
                            //{
                            //    this.Model.SelectedRanges.Add(range);
                            //    this.Model.InvalidateCell(range);
                            //}
                        }
                    }
#endif

                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
#if SILVERLIGHT
                    foreach (var NodeList in e.OldItems)
                    {
                        if (NodeList is GridTreeNode)
                        {
                            if ((NodeList as GridTreeNode).Item != null)
                            {
                                int rowIndex1 = this.GetRowIndexFromItem((NodeList as GridTreeNode).Item);
                                if (rowIndex1 > -1)
                                {
                                    GridRangeInfo range1 = new GridRangeInfo(GridRangeInfoType.Rows, rowIndex1);
                                    if (this.Model.SelectedRanges.Contains(range1))
                                    {
                                        this.Model.SelectedRanges.Remove(range1);
                                        this.Model.InvalidateCell(range1);
                                    }
                                }
                            }
                        }
                    }
#else
                    foreach (var node in e.OldItems)
                    {
                        if ((node as GridTreeNode).Item != null)
                        {
                            int rowIndex1 = this.GetRowIndexFromItem((node as GridTreeNode).Item);
                            if (rowIndex1 > -1)
                            {
                                GridRangeInfo range1 = new GridRangeInfo(GridRangeInfoType.Rows, rowIndex1);//(rowIndex1, 1, rowIndex1, this.Model.ColumnCount - 1);
                                if (this.Model.SelectedRanges.Contains(range1))
                                {
                                    this.Model.SelectedRanges.Remove(range1);
                                    this.InternalGrid.Model.InvalidateCell(range1);
                                }
                            }
                        }
                    }
#endif
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    GridRangeInfoList rangeList = this.Model.SelectedRanges.Clone();
                    this.Model.SelectedRanges.Clear();
                    foreach (GridRangeInfo r in rangeList)
                    {
                        this.Model.InvalidateCell(r);
                    }
                    break;
            }

            if (this.SelectedNodes.Count > 0)
                this.ParentTreeControl.SelectedNode = this.SelectedNodes[this.SelectedNodes.Count - 1];
            else if (e.Action != NotifyCollectionChangedAction.Remove)
                this.ParentTreeControl.SelectedNode = null;
        }

        protected override void OnResizingColumns(GridResizingColumnsEventArgs args)
        {
            base.OnResizingColumns(args);
            if (!args.AllowResize || args.Handled)
            {
                return;
            }
          
            if (args.Columns.Left == 0)
                args.AllowResize = false;
         
            if (args.Columns.Left == 1 && args.Reason != GridResizeCellsReason.HitTest && !args.Columns.IsEmpty)
            {
                if (args.Width < 5)
                {
                    args.Width = 5;
                }
            }
        }

        protected internal virtual void InitializeTreeModel(GridTreeModel model)
        {
            //this.UnwireModel();

            //this.Model = model;

            //add the custom cell renderers

#if !SILVERLIGHT
            // model.CellModels.Add(this.expanderCellType, new GridCellModel<GridTreeExpandCellRenderer>());
            model.CellModels.Add(this.expanderCellType, new GridCellModel<GridTreeExpanderCellRendererExt>());
            model.CellModels.Add(this.SortHeaderCellType, new GridCellModel<GridTreeHeaderCellRenderer>());
            GridTreeHeaderCellRenderer renderer = this.CellRenderers[this.SortHeaderCellType] as GridTreeHeaderCellRenderer;
            //model.CellModels.Add(this.SortHeaderCellType, new GridCellModel<GridCellSortHeaderRenderer>());
            //GridCellSortHeaderRenderer renderer = this.CellRenderers[this.SortHeaderCellType] as GridCellSortHeaderRenderer;
#else
            model.CellModels.Add(this.expanderCellType, new GridCellModel<GridTreeExpanderCellRendererExt>());

            model.CellModels.Add(this.SortHeaderCellType, new GridCellModel<GridTreeHeaderCellRenderer>());
            GridTreeHeaderCellRenderer renderer = this.CellRenderers[this.SortHeaderCellType] as GridTreeHeaderCellRenderer;

#endif
            //virtual events - to to provide values from the items to the grid and to save edit changes.
            model.QueryCellInfo += new GridQueryCellInfoEventHandler(this.Model_QueryCellInfo);
            model.CommitCellInfo += new GridCommitCellInfoEventHandler(this.Model_CommitCellInfo);

            //used to maintain rowheight changes...
            model.RowHeights.LineSizeChanged += new RangeChangedEventHandler(this.RowHeights_LineSizeChanged);

            //used to refresh headers when colwidths change
            model.ColumnWidths.LineSizeChanged += new RangeChangedEventHandler(this.ColumnWidths_LineSizeChanged);
            CurrentCellMoved += new GridCurrentCellMovedEventHandler(GridTreeControlImpl_CurrentCellMoved);

            //used to catch click on expand cell
#if !SILVERLIGHT
            this.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(TreeGrid_PreviewMouseLeftButtonDown);
            //this.CurrentCellMoving += new GridCurrentCellMovingEventHandler(GridTreeControlImpl_CurrentCellMoving);
            //this.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    if (ParentTreeControl != null)
            //    {
            //        this.ParentTreeControl.PreviewMouseDown += new MouseButtonEventHandler(ParentTreeControl_PreviewMouseDown);                    
            //        this.ParentTreeControl.PreviewMouseUp += new MouseButtonEventHandler(ParentTreeControl_PreviewMouseUp);
            //    }
            //}), null);
#else
            this.MouseLeftButtonUp += new MouseButtonEventHandler(GridTreeControlImpl_MouseLeftButtonUp);
#endif
            //used to trigger sort...
            this.CellClick += new GridCellClickEventHandler(GridTreeControl_CellClick);
            this.CurrentCellChanged += new GridRoutedEventHandler(GridTreeControlImpl_CurrentCellChanged);
            //Wire the SelectedNodes CollectionChanged event
            //this.SelectedNodes.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectedNodes_CollectionChanged);
            //used to color row under mouse
#if !SILVERLIGHT
            this.CurrentCellMoving += new GridCurrentCellMovingEventHandler(GridTreeControlImpl_CurrentCellMoving);
            this.CurrentCellEditingComplete += new GridRoutedEventHandler(GridTreeControlImpl_CurrentCellEditingComplete);
            this.CurrentCellPreviewKeyDown += new GridCellKeyEventHandler(GridTreeControlImpl_CurrentCellPreviewKeyDown);
            this.PreviewMouseMove += new MouseEventHandler(TreeGrid_PreviewMouseMove);


#else
            // this.MouseMove += new MouseEventHandler(GridTreeControlImpl_MouseMove);
#endif
            this.MouseLeave += new MouseEventHandler(TreeGrid_MouseLeave);

            this.QueryAllowDragColumn += new GridQueryDragColumnHeaderEventHandler(GridTreeControl_QueryAllowDragColumn);
            this.FrozenColumns = FreezeExpandColumn ? 2 : 1; //freeze the expand column

            if (!this.ShowColumnHeaders)
            {
                this.Model.RowHeights.SetHidden(0, 0, true);
            }

            //set this before mousecontroller
            // this.nodeHighlightBrush = this.GetGridTreeNodeHighlightBrush(this.GetVisualStyle(VisualStyle));// new SolidColorBrush(c);

            //remove the default rowheightsizing
            InitializeMouseController(this);

            //default is 2.5 and makes for heavy look
            this.CurrentCellBorderWeight = 1f;


            //set expand column width based on property settings
            model.ColumnWidths.SetHidden(0, 0, !ShowRowHeader);
            model.ColumnWidths[0] = RowHeaderWidth;

            //other miscellaneous properties...
            model.Options.ExcelLikeCurrentCell = false; //default = true
            model.Options.ExcelLikeSelectionFrame = false; //default = false
            Model.Options.ShowCurrentCell = false; //default = true 


            model.RowHeights[0] = 26;
            model.RowHeights.DefaultLineSize = 22d;

        }

        void GridTreeControlImpl_CurrentCellPreviewKeyDown(object sender, GridCellKeyEventArgs args)
        {
            //If the user edit the current cell and pressing escape key should not commit its value to the underlying collection.
            if (args.Key == System.Windows.Input.Key.Escape)
            {
                //this.IsInCancelEdit = true;
                //this.CurrentCell.CancelEdit();                
                if (this.CurrentCell.IsEditing)
                {
                    this.CancelEdit();
                    this.Model.InvalidateCell(GridRangeInfo.Row(this.CurrentCell.RowIndex));
                }
#if SILVERLIGHT
                    args.Handled = true;
#endif
            }
        }

        bool IsInCancelEdit = false;
        public void CancelEdit()
        {
            this.IsInCancelEdit = true;
            this.CurrentCell.CancelEdit();
            this.IsInCancelEdit = false;

        }
#if !SILVERLIGHT

        void GridTreeControlImpl_CurrentCellEditingComplete(object sender, SyncfusionRoutedEventArgs args)
        {
            if ((ParentTreeControl.UpdateMode == UpdateMode.PropertyChanged || ParentTreeControl.UpdateMode == UpdateMode.LostFocus
                && this.AllowSort) && (ParentTreeControl.SortingOptions == GridTreeSortingOptions.Default ||
                ParentTreeControl.SortingOptions == GridTreeSortingOptions.DisableSortingOnPropertyChange) && !ParentTreeControl.ReadOnly)
            {
                string PropertyName = ParentTreeControl.Columns[ResolveIndexToColumnIndex(CurrentCell.ColumnIndex)].MappingName;
                if (!string.IsNullOrEmpty(this.SortProperty) && (this.SortProperty.Contains(PropertyName) || string.IsNullOrEmpty(PropertyName)))
                {
                    SortAfterEdit(CurrentCell.RowIndex, ColumnNameToPosition(PropertyName));
                }
            }

            var style = this.RenderStyles.GetRenderStyleInfo(this.CurrentCell.RowIndex, this.CurrentCell.ColumnIndex);
            var renderer = this.CurrentCell.Renderer;
            //null check for renderer

            if (!this.IsInCancelEdit && renderer != null && renderer.ControlValue != null && style.ModelStyle.CellType=="DataBoundTemplate" && style.ModelStyle.CellValue != null && style.ModelStyle.CellValue.ToString() != renderer.ControlValue.ToString())
                renderer.RaiseSaveChanges();
        }
#endif

        void GridTreeControlImpl_CurrentCellMoved(object sender, GridCurrentCellMovedEventArgs args)
        {
            if (this.Model.SelectedRanges.AnyRangeContains(this.CurrentCell.RangeInfo))
            {
                var ranges = this.Model.SelectedRanges.GetRangesContaining(this.CurrentCell.RangeInfo);
                if (!ranges.ActiveRange.IsEmpty)
                {
                    this.Model.SelectedRanges.Remove(ranges.ActiveRange);
                    this.Model.SelectedRanges.Add(ranges.ActiveRange);
                }
            }
        }

        public GridTreeModel TreeModel
        {
            get
            {
                return this.Model as GridTreeModel;
            }
        }

        // private int lastCurrentRowIndex = -1; The variable is assigned but it is never used

#if SILVERLIGHT
        void GridTreeControlImpl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!EnableHotRowMarker)
                return;

            RowColumnIndex rowColumn = this.PointToCellRowColumnIndex(e);
            Point p = e.GetPosition(this);
            bool outSideGrid = p.X >= this.ColumnWidths.TotalExtent ||
                               p.X < 0 ||
                               p.Y < 0 ||
                               p.Y > this.RowHeights.TotalExtent;

            if (rowColumn.RowIndex != rowColumnUnderMouse.RowIndex || oldOutSideGrid != outSideGrid)
            {
                rowColumnUnderMouse = rowColumn;
                if (rowSpanUnderMouse != null)
                {
                    this.Model.CellSpanBackgrounds.Remove(rowSpanUnderMouse);
                }
                if (rowColumn.RowIndex > 0 && !outSideGrid)
                {
                    rowSpanUnderMouse = new CellSpanBackgroundInfo(rowColumn.RowIndex, 1, rowColumn.RowIndex, this.Model.ColumnCount, true, true, MarkRowBrush, null);
                    if (!this.Model.CellSpanBackgrounds.Contains(rowSpanUnderMouse))
                        this.Model.CellSpanBackgrounds.Add(rowSpanUnderMouse);
                    this.InvalidateVisual();
                }
            }
            else if (outSideGrid)
            {
                if (rowSpanUnderMouse != null)
                {
                    this.Model.CellSpanBackgrounds.Remove(rowSpanUnderMouse);
                    rowSpanUnderMouse = null;
                    this.InvalidateVisual();
                }

                rowColumnUnderMouse = RowColumnIndex.Empty;
            }

            oldOutSideGrid = outSideGrid;

        }
        public bool isGlyphClicked = false;
        void GridTreeControlImpl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GridControlBase grid = (GridControlBase)sender;
            RowColumnIndex cell = grid.PointToCellRowColumnIndex(e);
            bool isValidationMsgShown;
            bool suspendMoveTo;
            if (cell.ColumnIndex == nodeColumnIndex)
            {
                GridTreeNode n;
                GridTreeRowType rowType = GetGridRowType(cell.RowIndex, out n);
                if (rowType == GridTreeRowType.Caption || rowType == GridTreeRowType.Node)
                {
                    // Code commented to disable the Node Expand/Collapse when clicking on Node image
                    //Point pt = e.GetPosition(this);
                    //Rect r = this.RangeToClippedVisibleRect(GridRangeInfo.Cell(cell.RowIndex, cell.ColumnIndex));


                    if(isGlyphClicked) //(pt.X - r.Left < (GetTextIndent(n.Level) + 18))
                    {
                        if (OnExpandStateChanging(n, n.Expanded ? GridTreeNodeActions.Collapsing : GridTreeNodeActions.Expanding))
                        {
                            inExpandCollapseClick = true;
                            if (CurrentCell.IsModified)
                                CurrentCell.ConfirmChanges(out isValidationMsgShown, out suspendMoveTo);

                            saveCurrentCellState = true; //preserve currentcell if possible...
                            if (n.Expanded)
                                CollapseNode(cell.RowIndex, n);
                            else
                                ExpandNode(cell.RowIndex, n);
                            saveCurrentCellState = false;

                            ResetDisplay(cell.RowIndex);
                            e.Handled = true;
                            OnExpandStateChanged(n, n.Expanded ? GridTreeNodeActions.Expanded : GridTreeNodeActions.Collapsed);
                        }
                        inExpandCollapseClick = false;
                        isGlyphClicked = false;
                    }
                }

                this.InvalidateCell(cell);
            }
        }

#endif

        internal void UnwireGridEvents()
        {
            if (this.SelectedNodes != null)
                this.SelectedNodes.CollectionChanged -= new NotifyCollectionChangedEventHandler(SelectedNodes_CollectionChanged);
            if(this.ParentTreeControl.Columns!=null)
                this.ParentTreeControl.Columns.CollectionChanged -= new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
#if !SILVERLIGHT
            this.PreviewMouseLeftButtonDown -= new System.Windows.Input.MouseButtonEventHandler(TreeGrid_PreviewMouseLeftButtonDown);
            //this.CurrentCellMoving -= new GridCurrentCellMovingEventHandler(GridTreeControlImpl_CurrentCellMoving);
            //this.ParentTreeControl.PreviewMouseDown -= new MouseButtonEventHandler(ParentTreeControl_PreviewMouseDown);
            //this.ParentTreeControl.PreviewMouseUp -= new MouseButtonEventHandler(ParentTreeControl_PreviewMouseUp);
#else
            this.MouseLeftButtonUp -= new MouseButtonEventHandler(GridTreeControlImpl_MouseLeftButtonUp);
#endif
            this.CurrentCellMoving += new GridCurrentCellMovingEventHandler(GridTreeControlImpl_CurrentCellMoving);
            this.CellClick -= new GridCellClickEventHandler(GridTreeControl_CellClick);
            this.CurrentCellChanged += new GridRoutedEventHandler(GridTreeControlImpl_CurrentCellChanged);
#if !SILVERLIGHT
            this.CurrentCellEditingComplete -= new GridRoutedEventHandler(GridTreeControlImpl_CurrentCellEditingComplete);
            this.PreviewMouseMove -= new MouseEventHandler(TreeGrid_PreviewMouseMove);
#else
            //this.MouseMove -= new MouseEventHandler(GridTreeControlImpl_MouseMove);
#endif
            CurrentCellMoved -= new GridCurrentCellMovedEventHandler(GridTreeControlImpl_CurrentCellMoved);
            this.MouseLeave -= new MouseEventHandler(TreeGrid_MouseLeave);
            this.QueryAllowDragColumn -= new GridQueryDragColumnHeaderEventHandler(GridTreeControl_QueryAllowDragColumn);
        }



        #region population code
        public void DoLoad()
        {
            if (doPopulateOnce)
            {
                doPopulateOnce = false;
				if(this.ParentTreeControl.Columns!=null)
                	this.ParentTreeControl.Columns.CollectionChanged += new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
                ColumnWidthSizer.inApplySizes = true; //avoids tweaking percentsizevalues...
                PopulateTree();
                this.SelectedNodes.owner = this;
                this.SelectedNodes.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectedNodes_CollectionChanged);

                ColumnWidthSizer.inApplySizes = false;

                switch (ExpandStateAtStartUp)
                {
                    case GridTreeStartUpExpandState.AllNodesExpanded:
                        ExpandAllNodes();
                        break;
                    case GridTreeStartUpExpandState.RootNodesExpanded:
                        foreach (GridTreeNode n in RootNodes)
                        {
                            ExpandNode(n);
                        }
                        break;
                    default:
                        break;
                }
                if (!IsDesignTime() && ItemPropertyType == null)
                {
                    //MessageBox.Show("You must subscribe to the RequestTreeItems event to populate this grid.");
                }
            }

            if (PercentSizingBehavior != GridPercentColumnSizingBehavior.None)
            {
                ColumnWidthSizer.EnableSizing();
                ColumnWidthSizer.ApplySizes();
            }

            this.WireCollectionChangedEvent(this.parentTreeControl.ItemsSource);
        }

        /// <summary>
        /// Handles the CollectionChanged event of the GridTree control's Column colelction
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Code to display the visible columns in view that are adding at the runtime.
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (GridTreeColumn column in e.NewItems)
                {
                    this.ParentTreeControl.RaiseQueryVisibleColumnInfo(new GridTreeQueryVisibleColumnInfoEventArgs() { VisibleColumn = column });
                    if (AutoGenerateColumnsInfo)
                        this.ParentTreeControl.Model.InsertColumns(e.NewStartingIndex + this.ParentTreeControl.Model.HeaderRows, 1);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && AutoGenerateColumnsInfo)
            {
                foreach(GridTreeColumn column in e.OldItems)
                {
                    this.ParentTreeControl.Model.RemoveColumns(e.OldStartingIndex + this.ParentTreeControl.Model.HeaderRows,1 );
                }
            }
        }  

        #region Serialization/DeSerialization

#if !SILVERLIGHT
        /// <summary>
        /// Serializes the specified properties in the <see cref="Syncfusion.Windows.Controls.Grid.GridTreeControlImpl"/>.
        /// </summary>
        /// <param name="model">The file name.</param>
        public void Serialize(string fileName)
#else
        /// <summary>
        /// Serializes the specified properties in the <see cref="Syncfusion.Windows.Controls.Grid.GridTreeControlImpl"/>.
        /// Opens up a SaveFileDialog and saves the serialized data in XML.
        /// </summary>
        public  void Serialize()
#endif
        {
            InitializeSerializableProperty();
            try
            {
                var xs = new XmlSerializer(typeof(GridTreeSerializableProperty));
#if !SILVERLIGHT
                using (var sw = new XmlTextWriter(fileName, Encoding.Default))
                {
                    xs.Serialize(sw.BaseStream, this._serializableProperty);
                }
#else
                SaveFileDialog sfd = new SaveFileDialog() { Filter = "XML Files (*.xml)|*.xml", FilterIndex = 1 };
                if (sfd.ShowDialog() == true)
                {
                    var stream = sfd.OpenFile();
                    if (stream != null)
                    {
                        using (var sw = new StreamWriter(stream))
                        {
                            xs.Serialize(sw.BaseStream, this._serializableProperty);
                        }
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Serializes to stream.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="textWriter">The text writer.</param>
#if !SILVERLIGHT
        public void SerializeToStream(TextWriter textWriter)
#else
        public  void SerializeToStream(Stream stream)
#endif
        {
            InitializeSerializableProperty();
            try
            {
                var xs = new XmlSerializer(typeof(GridTreeSerializableProperty));
#if !SILVERLIGHT
                using (var sw = new XmlTextWriter(textWriter))
                {
                    xs.Serialize(sw.BaseStream, this._serializableProperty);
                }
#else
                using (var sw = new StreamWriter(stream))
                {
                    xs.Serialize(sw.BaseStream, this._serializableProperty);
                }
#endif
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Serializes the <see cref="Syncfusion.Windows.Controls.Grid.GridTreeControlImpl"/> properties as string.
        /// </summary>
        /// <returns></returns>
        public string SerializeAsString()
        {
            var result = string.Empty;
            InitializeSerializableProperty();
            try
            {
                var xs = new XmlSerializer(typeof(GridTreeSerializableProperty));
                using (var sWriter = new StringWriter())
                {
#if !SILVERLIGHT
                    using (var sw = new XmlTextWriter(sWriter))
                    {
                        xs.Serialize(sw, this._serializableProperty);
                    }
#else
                    using (var sw = XmlWriter.Create(sWriter))
                    {
                        xs.Serialize(sw, this._serializableProperty);
                    }
#endif
                    result = sWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Deserializes the specified <see cref="Syncfusion.Windows.Controls.Grid.GridTreeControlImpl"/>.
        /// </summary>
        public void Deserialize(string fileName)
#else
        /// <summary>
        /// Deserializes the specified <see cref="Syncfusion.Windows.Controls.Grid.GridTreeControlImpl"/> 
        /// from an XML file read with an OpenFileDialog.
        /// </summary>
        public  void Deserialize()
#endif
        {
            try
            {
                var xs = new XmlSerializer(typeof(GridTreeSerializableProperty));
#if !SILVERLIGHT
                using (var sr = new XmlTextReader(fileName))
                {
                    var serializableProperties = xs.Deserialize(sr) as GridTreeSerializableProperty;
                    ApplySerializableProperty(serializableProperties);
                }
#else
                OpenFileDialog ofd = new OpenFileDialog() { Filter = "XML Files (*.xml)|*.xml", FilterIndex = 1 };

                if (ofd.ShowDialog() == true)
                {
                    var stream = ofd.File.OpenRead();
                    using (var sr = new StreamReader(stream, Encoding.UTF8))
                    {
                        var tableProperties = xs.Deserialize(sr) as GridTreeSerializableProperty;
                        ApplySerializableProperty( tableProperties);
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Deserializes from stream.
        /// </summary>
        /// <param name="textReader">The text reader.</param>
#if !SILVERLIGHT
        public void DeserializeFromStream(TextReader textReader)
#else
        public  void DeserializeFromStream(Stream stream)
#endif
        {
            try
            {
                var xs = new XmlSerializer(typeof(GridTreeSerializableProperty));
#if !SILVERLIGHT
                using (var sr = new XmlTextReader(textReader))
                {
                    var serializableProperties = xs.Deserialize(sr) as GridTreeSerializableProperty;
                    ApplySerializableProperty(serializableProperties);
                }
#else
                using (var sr = new StreamReader(stream, Encoding.UTF8))
                {
                    var serializableProperties = xs.Deserialize(sr) as GridTreeSerializableProperty;
                    ApplySerializableProperty(serializableProperties);
                }
#endif
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

#if SILVERLIGHT
        /// <summary>
        /// Deserializes the specified <see cref="Syncfusion.Windows.Controls.Grid.GridTreeControlImpl"/> from a FileStream. 
        /// The File can be read from an Isolated Storage with the <see cref="System.IO.FileStream"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="System.IO.FileStream"/> has to be closed or used inside a closure when calling this method.
        /// </remarks>
        /// <param name="fileStream">The file stream.</param>
        public  void Deserialize(FileStream fileStream)
        {
            var xs = new XmlSerializer(typeof(GridTreeSerializableProperty));
            try
            {
                using (var sr = new StreamReader(fileStream))
                {
                    var serializableProperties = xs.Deserialize(sr) as GridTreeSerializableProperty;
                    ApplySerializableProperty(serializableProperties);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
#endif

        /// <summary>
        /// Deserializes <see cref="Syncfusion.Windows.Controls.Grid.GridTreeControlImpl"/> properties from string.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="content">The content.</param>
        public void DeserializeFromString(string content)
        {
            var xs = new XmlSerializer(typeof(GridTreeSerializableProperty));
            using (var sReader = new StringReader(content))
            {
#if !SILVERLIGHT
                using (var sr = new XmlTextReader(sReader))
                {
                    var serializableProperties = xs.Deserialize(sr) as GridTreeSerializableProperty;
                    ApplySerializableProperty(serializableProperties);
                }
#else
                using (var sr = XmlReader.Create(sReader))
                {
                    var serializableProperties = xs.Deserialize(sr) as GridTreeSerializableProperty;
                    ApplySerializableProperty(serializableProperties);
                }
#endif
            }
        }

        GridTreeSerializableProperty _serializableProperty;
        /// <summary>
        /// Initializes the serializable property.
        /// </summary>
        private void InitializeSerializableProperty()
        {
            _serializableProperty = new GridTreeSerializableProperty();
            _serializableProperty.AllowAutoSizingNodeColumn = this.ParentTreeControl.AllowAutoSizingNodeColumn;
            _serializableProperty.AllowDragColumns = this.ParentTreeControl.AllowDragColumns;
            _serializableProperty.AllowSort = this.ParentTreeControl.AllowSort;
            _serializableProperty.SortStates = this.SortStates;
            _serializableProperty.SortProperty = this.sortProperty;
            _serializableProperty.EnableHotRowMarker = this.ParentTreeControl.EnableHotRowMarker;
            _serializableProperty.EnableNodeSelection = this.ParentTreeControl.EnableNodeSelection;
            _serializableProperty.EnableSelections = this.ParentTreeControl.EnableSelections;
            _serializableProperty.ExpandStateAtStartUp = this.ParentTreeControl.ExpandStateAtStartUp;
            _serializableProperty.FreezeExpandColumn = this.ParentTreeControl.FreezeExpandColumn;
            _serializableProperty.HideEmptyChildGlyphs = this.ParentTreeControl.HideEmptyChildGlyphs;
            _serializableProperty.IgnoreResetOnListChanged = this.ParentTreeControl.IgnoreResetOnListChanged;
            _serializableProperty.ReadOnly = this.ParentTreeControl.ReadOnly;
            _serializableProperty.ShowColumnHeaders = this.ParentTreeControl.ShowColumnHeaders;
            _serializableProperty.ShowExpandColumnBorders = this.ParentTreeControl.ShowExpandColumnBorders;
            _serializableProperty.ShowRowHeader = this.ParentTreeControl.ShowRowHeader;
            _serializableProperty.SupportNodeImages = this.ParentTreeControl.SupportNodeImages;
            _serializableProperty.SupportRowSizing = this.ParentTreeControl.SupportRowSizing;
            _serializableProperty.TrackSelectionOnCollectionChange = this.ParentTreeControl.TrackSelectionOnCollectionChange;
            _serializableProperty.VisualStyle = this.ParentTreeControl.VisualStyle;

        }

        /// <summary>
        /// Applies the serializable property.
        /// </summary>
        /// <param name="property">The property.</param>
        private void ApplySerializableProperty(GridTreeSerializableProperty property)
        {
            this.ParentTreeControl.AllowAutoSizingNodeColumn = property.AllowAutoSizingNodeColumn;
            this.ParentTreeControl.AllowDragColumns = property.AllowDragColumns;
            this.ParentTreeControl.AllowSort = property.AllowSort;
            if (property.SortStates.Count > 0)
            {
                this.SortProperty = property.SortProperty;
                this.SortStates = property.SortStates;
                ((GridTreeModel)Model).inSort = true;
                if (RootNodes.Count > 1)
                {
                    this.Model.SelectedRanges.Clear();
                    RootNodes.Sort(SortComparer);
                }
                ReloadNodes();
                ((GridTreeModel)Model).inSort = false;
            }
            this.ParentTreeControl.EnableHotRowMarker = property.EnableHotRowMarker;
            this.ParentTreeControl.EnableNodeSelection = property.EnableNodeSelection;
            this.ParentTreeControl.EnableSelections = property.EnableSelections;
            this.ParentTreeControl.ExpandStateAtStartUp = property.ExpandStateAtStartUp;
            this.ParentTreeControl.FreezeExpandColumn = property.FreezeExpandColumn;
            this.ParentTreeControl.HideEmptyChildGlyphs = property.HideEmptyChildGlyphs;
            this.ParentTreeControl.IgnoreResetOnListChanged = property.IgnoreResetOnListChanged;
            this.ParentTreeControl.ReadOnly = property.ReadOnly;
            this.ParentTreeControl.ShowColumnHeaders = property.ShowColumnHeaders;
            this.ParentTreeControl.ShowExpandColumnBorders = property.ShowExpandColumnBorders;
            this.ParentTreeControl.ShowRowHeader = property.ShowRowHeader;
            this.ParentTreeControl.SupportNodeImages = property.SupportNodeImages;
            this.ParentTreeControl.SupportRowSizing = property.SupportRowSizing;
            this.ParentTreeControl.TrackSelectionOnCollectionChange = property.TrackSelectionOnCollectionChange;
            this.ParentTreeControl.VisualStyle = property.VisualStyle;
            this.Model.Options.ActivateCurrentCellBehavior = property.Options.ActivateCurrentCellBehavior;
            this.Model.Options.AllowExcelLikeResizing = property.Options.AllowExcelLikeResizing;
            this.Model.Options.ExcelLikeCurrentCell = property.Options.ExcelLikeCurrentCell;
            this.Model.Options.ExcelLikeSelection = property.Options.ExcelLikeSelection;
            this.Model.Options.ExcelLikeSelectionFrame = property.Options.ExcelLikeSelectionFrame;
            this.Model.Options.ShowCurrentCell = property.Options.ShowCurrentCell;
            this.Model.Options.WrapCell = property.Options.WrapCell;
            this.Model.Options.WrapCellBehavior = property.Options.WrapCellBehavior;
        }

        #endregion

        bool doPopulateOnce = true;
        /// <summary>
        /// This method is overridden to trigger the initial loading of the root items. It does this by raising
        /// the RequestTreeItems event and passing null for the ParentItem in teh event arguments class.
        /// </summary>
        /// <param name="e">The routed event arguments.</param>
        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            DoLoad();
            if (this.Nodes.Count == 0 && this.ParentTreeControl != null && this.ParentTreeControl.ItemsSource == null)
                this.doPopulateOnce = true;
        }

        private GridTreeColumnWidthSizer columnWidthSizer;

        internal GridTreeColumnWidthSizer ColumnWidthSizer
        {
            get
            {
                if (columnWidthSizer == null)
                    columnWidthSizer = new GridTreeColumnWidthSizer(this.Model, (IEnumerable)this.Columns);
                return columnWidthSizer;
            }
        }

        /// <exclude/>
        public static readonly DependencyProperty AllowAutoSizingNodeColumnProperty;

        /// <summary>
        /// Gets or sets whether the tree node column's width is automatically adjusted as the node levels increase.
        /// </summary>
        /// <remarks>This property is ignored is PercentageSizing is enabled and the node column is marked to participate
        /// in the percentage sizing calculations.</remarks>
        public bool AllowAutoSizingNodeColumn
        {
            get { return (bool)GetValue(AllowAutoSizingNodeColumnProperty); }
            set { SetValue(AllowAutoSizingNodeColumnProperty, value); }
        }

        /// <exclude/>
        public static readonly DependencyProperty PercentSizingBehaviorProperty;

        /// <summary>
        /// Gets or sets the percentage sizing behavior in the GridTreeControl.
        /// </summary>
        /// <remarks>
        /// Set the GridTreeColumn.PercentWidth property to enable a particular column to
        /// participate in the automatic sizing as the TreeGridControl client width changes. 
        /// Depending upon the value of PercentSizingBehavior, the free client width left after
        /// all the columns whose PrecentWidth is not set has been subtracted, is proportionally
        /// allocated among all those columns whose PercentWidth is set.
        /// </remarks>
        public GridPercentColumnSizingBehavior PercentSizingBehavior
        {
            get { return (GridPercentColumnSizingBehavior)GetValue(PercentSizingBehaviorProperty); }
            set
            {
                SetValue(PercentSizingBehaviorProperty, value);
            }
        }

        private bool IsDesignTime()
        {
            return DesignerProperties.GetIsInDesignMode(this);
        }
        /// <summary>
        /// This Method used to Get the TotalColumns in the Collection.
        /// </summary>
        public List<GridTreeColumn> GetVisibleColumns()
        {
            List<GridTreeColumn> totalColumn = new List<GridTreeColumn>();
            if (ItemPropertyType != null)
            {

                bool usesChildrenCollection = this.parentTreeControl != null &&
                                             this.parentTreeControl.ChildPropertyName != null &&
                                             this.parentTreeControl.ChildPropertyName.Length > 0;
                foreach (var pd in ItemProperties)
                {

#if SILVERLIGHT
                    if (usesChildrenCollection && pd.Value.Name == this.parentTreeControl.ChildPropertyName)
                        continue;
                    GridTreeColumn tc = new GridTreeColumn(pd.Value.Name);
#else
                    if (usesChildrenCollection && ((PropertyDescriptor)pd).Name == this.parentTreeControl.ChildPropertyName)
                        continue;
                    GridTreeColumn tc = new GridTreeColumn(((PropertyDescriptor)pd).Name);
#endif

                    tc.StyleInfo = new GridStyleInfo();
                    totalColumn.Add(tc);

                }
            }
            return totalColumn;
        }

        /// <summary>
        /// This method is only called if the Columns collection has not been explicitly populated by the user
        /// before the initial raising of the RequestTreesItems event in OnLoad.
        /// </summary>
        public void AutoPopulateColumnInfo()
        {
            if (ItemPropertyType != null)
            {
                this.Columns.Clear();
                bool usesChildrenCollection = this.parentTreeControl != null &&
                                             this.parentTreeControl.ChildPropertyName != null &&
                                             this.parentTreeControl.ChildPropertyName.Length > 0;
                foreach (var pd in ItemProperties)
                {

#if SILVERLIGHT
                    if (usesChildrenCollection && pd.Value.Name == this.parentTreeControl.ChildPropertyName)
                        continue;
                    GridTreeColumn tc = new GridTreeColumn(pd.Value.Name);
#else
                    if (usesChildrenCollection && ((PropertyDescriptor)pd).Name == this.parentTreeControl.ChildPropertyName)
                        continue;
                    GridTreeColumn tc = new GridTreeColumn(((PropertyDescriptor)pd).Name);
#endif
                    // tc.StyleInfo = new GridStyleInfo(this.Model.TableStyle.Store);
					// Code defines the CellType based on the values loaded on the column
                    if (this.AutoPopulateColumns && this.AutoGenerateColumnsInfo)
                    {
#if SILVERLIGHT
                        tc = this.DefineCellType(tc, (PropertyInfo)pd.Value);
#else
                        tc = this.DefineCellType(tc, (PropertyDescriptor)pd);
#endif
                    }
                    else
                        tc.StyleInfo = new GridStyleInfo();

                    this.Columns.Add(tc);
                    //Refresh the added column
                    this.InvalidateCell(GridRangeInfo.Col(this.Columns.Count));
                }
            }
        }

        /// <summary>
        /// Method defines the CellType of the column based on the value loaded.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="pd">The pd.</param>
        /// <returns></returns>
#if !SILVERLIGHT
        private GridTreeColumn DefineCellType(GridTreeColumn column, PropertyDescriptor pd)
        {
#else
        private GridTreeColumn DefineCellType(GridTreeColumn column, PropertyInfo pd)
        {
#endif
            if (pd.PropertyType.IsEnum)
            {
#if !SILVERLIGHT
                Array collection = Enum.GetValues(pd.PropertyType.UnderlyingSystemType);
#else
                Array collection = GridDataTableModelHelper.GetValues(pd.PropertyType.UnderlyingSystemType);
#endif
                column.StyleInfo.CellType = "ComboBox";
                column.StyleInfo.ItemsSource = collection;
                column.StyleInfo.DropDownStyle = GridDropDownStyle.Editable;
            }
            else if (typeof(string).IsAssignableFrom(pd.PropertyType))
                column.StyleInfo.CellType = "TextBox";

            else if (typeof(bool).IsAssignableFrom(pd.PropertyType))
            {
                column.StyleInfo.CellType = "CheckBox";
                column.StyleInfo.HorizontalAlignment = HorizontalAlignment.Center;
                column.StyleInfo.IsThreeState = false;
            }

            else if (typeof(bool?).IsAssignableFrom(pd.PropertyType))
            {
                column.StyleInfo.CellType = "CheckBox";
                column.StyleInfo.HorizontalAlignment = HorizontalAlignment.Center;
                column.StyleInfo.IsThreeState = true;
            }

            else if (typeof(int).IsAssignableFrom(pd.PropertyType) || typeof(Int32).IsAssignableFrom(pd.PropertyType)
                    || typeof(Int64).IsAssignableFrom(pd.PropertyType) || typeof(Int16).IsAssignableFrom(pd.PropertyType))
            {
                column.StyleInfo.CellType = "IntegerEdit";
                column.StyleInfo.HorizontalAlignment = HorizontalAlignment.Right;
                column.StyleInfo.IntegerEdit.UseNullOption = false;
            }

            else if (typeof(int?).IsAssignableFrom(pd.PropertyType) || typeof(Int32?).IsAssignableFrom(pd.PropertyType)
                || typeof(Int64?).IsAssignableFrom(pd.PropertyType) || typeof(Int16?).IsAssignableFrom(pd.PropertyType))
            {
                column.StyleInfo.CellType = "IntegerEdit";
                column.StyleInfo.HorizontalAlignment = HorizontalAlignment.Right;
                column.StyleInfo.IntegerEdit.UseNullOption = true;
            }

            else if (typeof(double).IsAssignableFrom(pd.PropertyType) || typeof(decimal).IsAssignableFrom(pd.PropertyType))
            {
                column.StyleInfo.CellType = "DoubleEdit";
                column.StyleInfo.HorizontalAlignment = HorizontalAlignment.Right;
                column.StyleInfo.DoubleEdit.UseNullOption = false;
            }

            else if (typeof(double?).IsAssignableFrom(pd.PropertyType) || typeof(decimal?).IsAssignableFrom(pd.PropertyType))
            {
                column.StyleInfo.CellType = "DoubleEdit";
                column.StyleInfo.HorizontalAlignment = HorizontalAlignment.Right;
                column.StyleInfo.DoubleEdit.UseNullOption = true;
            }

            else if (typeof(DateTime).IsAssignableFrom(pd.PropertyType) || typeof(DateTime?).IsAssignableFrom(pd.PropertyType))
            {
                column.StyleInfo.CellType = "DateTimeEdit";
                column.StyleInfo.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else if (typeof(TimeSpan).IsAssignableFrom(pd.PropertyType))
            {
                column.StyleInfo.CellType = "TimeSpanEdit";
                column.StyleInfo.HorizontalAlignment = HorizontalAlignment.Right;
            }
            return column;
        }
		
		 internal bool isChildPropertyexist=true;
        /// <summary>
        /// This method is called from OnLoad to initially populate the tree grid.
        /// </summary>
        public void PopulateTree()
        {
            //loads the root nodes... (by passing null as the parentItem)
            GridTreeRequestChildListEventArgs args = new GridTreeRequestChildListEventArgs(null);
            OnRequestChildList(args);

            if (!LoadAllAtStartUp)
            {
                //auto populate properties if they have not been set...
                if (ItemPropertyType == null && rootNodes != null && rootNodes.Count > 0 && rootNodes[0].Item != null)
                {
#if !SILVERLIGHT
                    if (pdc == null && args.ChildList is ITypedList)
                    {
                        pdc = ((ITypedList)args.ChildList).GetItemProperties(null);
                    }
#endif
                    ItemPropertyType = rootNodes[0].Item.GetType();
                }
                else if (ItemPropertyType != null && rootNodes != null && rootNodes.Count > 0 && rootNodes[0].Item != null)
                {
#if !SILVERLIGHT
                    PropertyDescriptorCollection tempPdc = null;
                    if (args.ChildList is ITypedList)
                    {
                        tempPdc = ((ITypedList)args.ChildList).GetItemProperties(null);
                    }
                    else if (args.ChildList != null)
                    {
                        foreach (object o in args.ChildList)
                        {
                            tempPdc = TypeDescriptor.GetProperties(o.GetType());
                            break;
                        }
                    }

                    if (pdc != tempPdc)
                    {
                        pdc = tempPdc;
                        AutoPopulateColumnInfo();
                    }
#endif
                }
#if !SILVERLIGHT
                if (this.ItemProperties != null && this.ItemProperties[this.ParentTreeControl.ChildPropertyName]==null)
                    this.isChildPropertyexist = false; 
#else
                if (this.ItemProperties != null && !this.ItemProperties.ContainsKey(ParentTreeControl.ChildPropertyName))
                    this.isChildPropertyexist = false;      
#endif
                //populate columninfo if not set
                if (Columns.Count == 0 && ItemPropertyType != null && rootNodes.Count > 0 && this.AutoPopulateColumns)
                {
                    AutoPopulateColumnInfo();
                }
                if (this.AllowSort && this.SortStates.Count > 0)
                    RootNodes.Sort(SortComparer);
                PopulateGridNodes(true, false);
            }
            else
            {
                //auto populate properties if they have not been set...
                if (ItemPropertyType == null && Nodes != null && Nodes.Count > 0 && Nodes[0].Item != null)
                {
                    ItemPropertyType = Nodes[0].Item.GetType();
                }
                //populate columninfo if not set
                if (Columns.Count == 0 && ItemPropertyType != null)
                {
                    AutoPopulateColumnInfo();
                }

                maxLevel = -1;
                ResetGrid();
                int i = nodeColumnIndex;
                foreach (GridTreeColumn tc in Columns)
                {
                    if (i == nodeColumnIndex)
                    {
                        this.ColumnWidths[i] = GetWidthOfNodeColumn(maxLevel);
                    }
                    else
                    {
                        this.ColumnWidths[i] = !double.IsNegativeInfinity(tc.Width) ? tc.Width : this.ColumnWidths.DefaultLineSize;
                    }
                    i++;
                }
            }
            if (doPopulateOnce && this.ParentTreeControl != null && this.ParentTreeControl.ItemsSource == null)
            {
                doPopulateOnce = false;
                switch (ExpandStateAtStartUp)
                {
                    case GridTreeStartUpExpandState.AllNodesExpanded:
                        ExpandAllNodes();
                        break;
                    case GridTreeStartUpExpandState.RootNodesExpanded:
                        foreach (GridTreeNode n in RootNodes)
                        {
                            ExpandNode(n);
                        }
                        break;
                    default:
                        break;
                }
            }
        }


        private bool loadAllAtStartUp = false;

        /// <summary>
        /// Gets or sets whether your RequestTreeItems event will load the entire tree when it sees the rwuest for the root nodes.
        /// </summary>
        public bool LoadAllAtStartUp
        {
            get { return loadAllAtStartUp; }
            set { loadAllAtStartUp = value; }
        }

        private bool hideEmptyChildGlyphs = true;

        /// <summary>
        /// Gets or sets whether empty child nodes show the +/- cells when they are initially displayed.
        /// </summary>
        /// <value><c>true</c> if empty child nodes should not show the +/- ; otherwise, <c>false</c>.</value>
        /// <remarks>The default GridTreeControl raises the RequestTreeItem event for each child node as it becomes
        /// visible the first time so the GridTreeControl will not display the +/- for nodes that have no 
        /// children. If you set this property to false, the GridTreeControl will not raise the RequestTreeItems
        /// for the child until the child node is clicked to be expanded. At that point, if the child node has
        /// no children, the +/- will go away.
        /// </remarks>
        public bool HideEmptyChildGlyphs
        {
            get { return hideEmptyChildGlyphs; }
            set { hideEmptyChildGlyphs = value; }
        }

        private GridTreeStartUpExpandState expandStateAtStartUp = GridTreeStartUpExpandState.RootNodesExpanded;
        /// <summary>
        /// Gets or sets the expand states for nodes when the tree is initially displayed.
        /// </summary>
        public GridTreeStartUpExpandState ExpandStateAtStartUp
        {
            get { return expandStateAtStartUp; }
            set { expandStateAtStartUp = value; }
        }


        private int maxLevel = -1;

        /// <summary>
        /// Used internally. Called to populate all open nodes in a tree.
        /// </summary>
        /// <param name="forceRepopulate">True if you want the child lists to be reloaded.</param>
        /// <remarks>
        /// This method is called initally as the tree is first loaded, and then after sorts are done to
        /// redisplay the tree contents in a possible new order.
        /// </remarks>
        public void PopulateGridNodes(bool forceRepopulate)
        {
            // To fix the issue of Width of the expander cell column is increasing on repopulating the Grid old value is false.
            PopulateGridNodes(forceRepopulate, true);
        }

        public void PopulateGridNodes(bool forceRepopulate, bool ignoreColWidths)//We have changed this method to public because In gantt control while adding the column   dynamically with doesnt set.
        {
            maxLevel = -1;
            Nodes.Clear();
            foreach (GridTreeNode rootNode in RootNodes)
                AddNode(rootNode, forceRepopulate);

            ResetGrid();
            if (this.ParentTreeControl != null)
                this.ParentTreeControl.OnNodesPopulated();

            if (ignoreColWidths)
                return;

            int i = nodeColumnIndex;

            ColumnWidthSizer.inApplySizes = true;
            foreach (GridTreeColumn tc in Columns)
            {
                if (i == nodeColumnIndex && AllowAutoSizingNodeColumn)
                {
                    this.ColumnWidths[i] = GetWidthOfNodeColumn(maxLevel);
                }
                else
                {
                    this.ColumnWidths[i] = !double.IsNegativeInfinity(tc.Width) ?
                                            (tc.Width == 80d) ?
                                            (this.ColumnWidths.DefaultLineSize == 90d ?
                                            tc.Width : this.ColumnWidths.DefaultLineSize) :
                                            tc.Width : this.ColumnWidths.DefaultLineSize;
                }
                i++;
            }

            if (!ignoreColWidths && this.AllowAutoSizingNodeColumn && this.Columns.Count > 0)
            {
                if (this.PercentSizingBehavior == GridPercentColumnSizingBehavior.None)
                {
                    if (this.ParentTreeControl.AutosizingNodeColumnOption == GridNodeAutosizingOption.BasedOnNodeCount)
                    {

                        this.Columns[0].Width = GetWidthOfNodeColumn(Nodes.Count);
                    }
                    else
                    {
                        this.Columns[0].Width = GetWidthOfNodeColumn(maxLevel);
                    }
                }
                else
                    ColumnWidthSizer.ApplySizes();
            }
            ColumnWidthSizer.inApplySizes = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridNode"></param>
        /// <param name="forceRepopulate"></param>
        /// <param name="ignoreColWidths"></param>
        private void PopulateGridNode(GridTreeNode gridNode, bool forceRepopulate, bool ignoreColWidths)
        {
            maxLevel = -1;

            AddNode(gridNode, forceRepopulate);

            ResetGrid();
            if (this.ParentTreeControl != null)
                this.ParentTreeControl.OnNodesPopulated();

            if (ignoreColWidths)
                return;

            int i = nodeColumnIndex;

            ColumnWidthSizer.inApplySizes = true;
            foreach (GridTreeColumn tc in Columns)
            {
                if (i == nodeColumnIndex && AllowAutoSizingNodeColumn)
                {
                    this.ColumnWidths[i] = GetWidthOfNodeColumn(maxLevel);
                }
                else
                {
                    this.ColumnWidths[i] = !double.IsNegativeInfinity(tc.Width) ? tc.Width : this.ColumnWidths.DefaultLineSize;
                }
                i++;
            }

            if (!ignoreColWidths && this.AllowAutoSizingNodeColumn && this.Columns.Count > 0)
            {
                if (this.PercentSizingBehavior == GridPercentColumnSizingBehavior.None)
                {
                    if (this.ParentTreeControl.AutosizingNodeColumnOption == GridNodeAutosizingOption.BasedOnNodeCount)
                    {

                        this.Columns[0].Width = GetWidthOfNodeColumn(Nodes.Count);
                    }
                    else
                    {
                        this.Columns[0].Width = GetWidthOfNodeColumn(maxLevel);
                    }
                }
                else
                    ColumnWidthSizer.ApplySizes();
            }
            ColumnWidthSizer.inApplySizes = false;
        }

        private double GetWidthOfNodeColumn(int level)
        {
            GridTreeColumn tc = Columns[0];
            double w =

#if !SILVERLIGHT
 ExpandGlyphType == GridTreeExpandGlyph.PlusMinus || ExpandGlyphType == GridTreeExpandGlyph.PlusMinusLines
                       ? nodeColumnWidth - 7 :
#endif
 nodeColumnWidth;

            return Math.Max(nodeColumnWidth, (level + 2) * w) +
                                          (!double.IsNegativeInfinity(tc.Width) ? tc.Width : this.ColumnWidths.DefaultLineSize);

        }

        private void AddNode(GridTreeNode gridNode, bool forceRepopulate)
        {
            if (maxLevel < gridNode.Level)
                maxLevel = gridNode.Level;
            Nodes.Add(gridNode);

            if (!forceRepopulate && parentTreeControl.EnableRenderCheckIfGlyphNeeded && !gridNode.Expanded)
                return;

            GridTreeRequestChildListEventArgs e = new GridTreeRequestChildListEventArgs(gridNode, gridNode.Item, forceRepopulate);
            OnRequestChildList(e);

            if (!gridNode.HasChildNodes)
            {
                gridNode.HasChildNodes = e.ParentNode.ChildNodes != null && e.ParentNode.ChildNodes.Count > 0;
            }

            if (gridNode.Expanded)
            {
                foreach (GridTreeNode child in gridNode.ChildNodes)
                    AddNode(child, forceRepopulate);
            }
        }

        /// <summary>
        /// Ensures the grid properly reflects the rows and columns in the current expanded nodes.
        /// </summary>
        public void ResetGrid()
        {
            ResetGrid(1);
        }
        /// <summary>
        /// Resets the contents of the tree starting at the passed in grid row index. Ensures the grid properly 
        /// reflects the rows and columns in the current expanded nodes.
        /// </summary>
        /// <param name="startRow">The start row from which the grid is refreshed.</param>
        public void ResetGrid(int startRow)
        {
            this.Model.ColumnCount = 1 + Columns.Count;
            this.Model.RowCount = Nodes.Count + this.Model.HeaderRows + this.UnboundRowsCount;
            ResetRowHeights(startRow); //reset all row heights beyound startrow
            this.InvalidateVisual(true);
        }

        private void ResetRowHeights(int startRow)
        {
            if (!SupportRowSizing)
                return;

            for (int rowIndex = startRow; rowIndex < this.Model.RowCount; ++rowIndex)
            {
                if (Nodes[rowIndex - 1].NodeHeight != double.NaN)
                    this.Model.RowHeights[rowIndex] = Nodes[rowIndex - 1].NodeHeight;
            }
        }
        /// <exclude/>
        /// <summary>
        /// Used internally. Clears the Nodes collection and re-adds the current RootNodes.
        /// </summary>
        public void ReloadNodes()
        {
            Nodes.Clear();
            foreach (GridTreeNode rootNode in this.RootNodes)
                AddNode(rootNode, false);
            //ResetGrid();
            ResetDisplay();
        }

        /// <summary>
        /// Used internally. To add Particular node.
        /// </summary>
        /// <param name="node"></param>
        public void ReloadNode(GridTreeNode node)
        {
            AddNode(node, true);
            ResetGrid();
        }

        /// <exclude/>
        /// <summary>
        /// Used internally. Populates the RootNodes collection with the data items passed in.
        /// </summary>
        /// <param name="rootItems">A collection of data items that determine the contents of the root nodes.</param>
        public void PopulateRootNodes(object[] rootItems)
        {
            PopulateRootNodes(rootItems, true, false);
        }

        /// <summary>
        /// Used internally. Populates or adds to the RootNodes collection.
        /// </summary>
        /// <param name="rootItems">A collection of data items that are to be added as RootNodes.</param>
        /// <param name="resetRootNodes">True if you want  to reset the RootNodes collection before adding the items.</param>
        /// <param name="expanded">Whether the added nodes are initially expanded or not.</param>
        public void PopulateRootNodes(object[] rootItems, bool resetRootNodes, bool expanded)
        {
            if (resetRootNodes)
            {
                RootNodes.Clear();
                foreach (object item in rootItems)
                {
                    RootNodes.Add(OnCreatingTreeNode(0, item, expanded, null));
                }
            }
        }
        #endregion

        #region hot row coloring...

#if SILVERLIGHT
        private RowColumnIndex rowColumnUnderMouse = RowColumnIndex.Empty;
        internal CellSpanBackgroundInfo rowSpanUnderMouse = null;
#else
        private new RowColumnIndex rowColumnUnderMouse = RowColumnIndex.Empty;
        internal new CellSpanBackgroundInfo rowSpanUnderMouse = null;
#endif

        private bool oldOutSideGrid = true;

        private Brush markRowBrush = null;

        /// <summary>
        /// Brush used to draw the background of the row currently under the mouse location.
        /// </summary>
        public Brush MarkRowBrush
        {
            get
            {
                if (markRowBrush == null)
                {
                    GradientStopCollection stops = new GradientStopCollection()
                    {
#if SILVERLIGHT
                        new GradientStop(){ Color=Color.FromArgb(0xFF, 0x95, 0x93, 0xB2) , Offset=0 },
                        new GradientStop(){ Color=Color.FromArgb(0xFF, 0xC1, 0xC2, 0xCC) , Offset=.4 },
                        new GradientStop(){ Color=Color.FromArgb(0xFF, 0xC1, 0xC2, 0xCC) , Offset=.6 },
                        new GradientStop(){ Color=Color.FromArgb(0xFF, 0x95, 0x93, 0xB2) , Offset=1 }

#else  
                        new GradientStop(Color.FromArgb(0xFF, 0x95, 0x93, 0xB2), 0),
                        new GradientStop(Color.FromArgb(0xFF, 0xC1, 0xC2, 0xCC), .4),
                        new GradientStop(Color.FromArgb(0xFF, 0xC1, 0xC2, 0xCC), .6),
                        new GradientStop(Color.FromArgb(0xFF, 0x95, 0x93, 0xB2), 1)
#endif

                      
                    };
#if SILVERLIGHT
                    markRowBrush = new LinearGradientBrush() { GradientStops = stops, StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
#else
                    markRowBrush = new LinearGradientBrush(stops, new Point(0, 0), new Point(0, 1));
#endif
                }
                return markRowBrush;
            }
            set { markRowBrush = value; }
        }
#if !SILVERLIGHT

        void TreeGrid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!EnableHotRowMarker)
                return;

            RowColumnIndex rowColumn = this.PointToCellRowColumnIndex(e);
            Point p = e.GetPosition(this);
            bool outSideGrid = p.X >= this.ColumnWidths.TotalExtent ||
                               p.X < 0 ||
                               p.Y < 0 ||
                               p.Y > this.RowHeights.TotalExtent;

            if (rowColumn.RowIndex != rowColumnUnderMouse.RowIndex || oldOutSideGrid != outSideGrid)
            {
                rowColumnUnderMouse = rowColumn;
                if (rowSpanUnderMouse != null)
                {
                    oldrowSpanUnderMouse = rowSpanUnderMouse;
                    this.InvalidateCell(rowSpanUnderMouse);
                    rowSpanUnderMouse = null;
                }
                if (rowColumn.RowIndex > 0 && !outSideGrid)
                {
                    //if (this.EnableLegacyStyle)
                    //    rowSpanUnderMouse = new CellSpanBackgroundInfo(rowColumn.RowIndex, 2, rowColumn.RowIndex, this.Model.ColumnCount, true, true, MarkRowBrush, null);
                    //else
                    rowSpanUnderMouse = new CellSpanBackgroundInfo(rowColumn.RowIndex, 1, rowColumn.RowIndex, this.Model.ColumnCount);
                    if (!this.Model.CellSpanBackgrounds.Contains(rowSpanUnderMouse))
                    {
                        this.InvalidateCell(rowSpanUnderMouse);
                    }
                    this.InvalidateVisual(false);
                    //this.Model.CellSpanBackgrounds.Add(rowSpanUnderMouse);
                    //this.InvalidateCellBackground(0, 0);                    
                }
                //else if (rowColumn.RowIndex == 0)
                //{
                //    this.InvalidateCell(rowSpanUnderMouse);
                //    rowSpanUnderMouse = null;
                //    //  rowColumnUnderMouse = RowColumnIndex.Empty;
                //    //this.InvalidateCell(GridRangeInfo.Cell(1, 1));
                //    //this.InvalidateCellBackground(0, 0);
                //}
            }
            else if (outSideGrid)
            {
                if (rowSpanUnderMouse != null)
                {
                    this.InvalidateCell(rowSpanUnderMouse);
                    rowSpanUnderMouse = null;
                    this.InvalidateVisual(false);
                    //this.Model.CellSpanBackgrounds.Remove(rowSpanUnderMouse);                    
                    //this.InvalidateCellBackground(0, 0);                    
                }
            }
            oldOutSideGrid = outSideGrid;
            //if (!rowColumn.IsEmpty)
            //{
            //    this.InvalidateCell(rowSpanUnderMouse);
            //}
        }
#endif

        void TreeGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (rowSpanUnderMouse != null)
            {
                this.InvalidateCell(rowSpanUnderMouse);
                rowSpanUnderMouse = null;
                //this.Model.CellSpanBackgrounds.Remove(rowSpanUnderMouse);
                //int row = rowSpanUnderMouse.Top;                
#if !SILVERLIGHT
                //this.InvalidateCell(GridRangeInfo.Cell(row, 1));
                //this.InvalidateCellBackground(0, 0);
                this.InvalidateVisual(false);
#else
                this.InvalidateVisual();
#endif
            }
            //this.InvalidateCell(rowSpanUnderMouse);
            //rowSpanUnderMouse = null;
            //rowColumnUnderMouse = RowColumnIndex.Empty;
        }

        #endregion

        #region   Expand/Collapse code

        void GridTreeControl_CellClick(object sender, GridCellClickEventArgs e)
        {
            if (this.ShowRowHeader && e.ColumnIndex == 0 && e.RowIndex == 0)
            {
                return; //if we click on the 0th column while ShowRowHeaders=true then sorting should not take place. 
            }
#if SILVERLIGHT
            if (e.RowIndex == 0 && !e.Handled)
#else
            if (e.RowIndex == this.Model.HeaderRows - 1)
#endif
            {
                if (e.ColumnIndex >= nodeColumnIndex || RootNodes.Count > 1)
                {
                    if (AllowSort)
                    {

#if !SILVERLIGHT
                        if (this.ParentTreeControl.SortClickAction == SortClickAction.DoubleClick && e.ClickCount != 2)
                            return;
#endif

                        ((GridTreeModel)Model).inSort = true;
                        //do sort
                        string name = ColumnIndexToName(e.ColumnIndex);
                        int pos = e.ColumnIndex - nodeColumnIndex;
                        GridTreeColumn column = this.Columns[pos];
                        if (!column.AllowSort)
                        {
                            return;
                        }
                        bool clear = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.None || !this.ParentTreeControl.EnableMultiColumnSorting;

                        List<SortState> states = SortState.GetSortStatesFromString(SortProperty);
                        SortState state = new SortState();
                        state.Property = name;
                        bool ClearSortforTriState = false;
                        int loc = states.IndexOf(state);
                        if (loc > -1)
                        {
                            state = states[loc];
                            //In the TriState Sorting After Decending have to clear
                            ClearSortforTriState = state.Direction == ListSortDirection.Descending && this.ParentTreeControl.EnableTriStateSorting;
                            state.Direction = state.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                            if (!clear && ClearSortforTriState)
                            {
                                states.Remove(state);
                            }
                        }
                        else
                        {
                            states.Add(state);
                        }

                        if (clear)
                        {
                            states.Clear();
                            if (!ClearSortforTriState)
                                states.Add(state);
                        }

                        if (this.CurrentCell.HasCurrentCell)
                        {
                            this.CurrentCell.Deactivate();
                            if (this.ParentTreeControl.UpdateMode == UpdateMode.RowCachedMode)//While Current cell is in Edit Mode if We Sort then edited value should commit. So here we have commit the value and clear the CacheStorage
                            {
                                GridTreeNode n;
                                GridTreeRowType rowType = GetGridRowType(oldRowColIndex.RowIndex, out n);
                                this.CommitValueforRowCache(n);
                                this.CachedStorage.Clear();
                                this.oldRowColIndex.RowIndex = 0;
                            }

                        }
                        this.SortProperty = SortState.GetSortStringFromStates(states);

                        // if col 0 then resort the root nodes
                        if (RootNodes.Count > 1)
                        {
                            this.Model.SelectedRanges.Clear();
                            RootNodes.Sort(SortComparer);
                        }
                        ReloadNodes();
                        e.Handled = true;

                        if (Model.Options.ExcelLikeSelectionFrame)
                        {
                            Model.SelectedRanges.Clear();
                        }

                        if (e.RowIndex == 0 && ParentTreeControl.SelectedNode != null)
                        {
                            int currentIndex = this.GetRowIndexFromItem((this.ParentTreeControl.SelectedNode as GridTreeNode).Item);
                            if (currentIndex > 0 && currentIndex != e.RowIndex)
                                CurrentCell.MoveTo(currentIndex, e.ColumnIndex);
                        }
                        ((GridTreeModel)Model).inSort = false;
                    }
                    else
                    {
                        if (this.ParentTreeControl.UpdateMode == UpdateMode.RowCachedMode)//If AllowSort==False then while UpdateMode RowCache. Before value added to CachedStorage it was commited previously. So here we have Deactivate the current cell then commit the value.
                        {
                            if (this.CurrentCell.HasCurrentCell)
                            {
                                GridTreeNode n;
                                GridTreeRowType rowType = GetGridRowType(oldRowColIndex.RowIndex, out n);
                                this.CurrentCell.Deactivate();
                                this.CommitValueforRowCache(n);
                                this.CachedStorage.Clear();
                                this.oldRowColIndex.RowIndex = 0;
                            }

                        }
                    }
                }
            }
        }

        int rowIndexOfExpandingNode = -1;
        GridRangeInfo selectFrameRange = GridRangeInfo.Empty;
        GridRangeInfo collapsedRange = GridRangeInfo.Empty;
        List<GridTreeNode> hiddenNodes = new List<GridTreeNode>();

        internal bool inExpandCollapseClick = false;

#if !SILVERLIGHT

        private bool QueryWantsMouseInput(DependencyObject el)
        {
            //return !(Owner is ScrollControl && ((ScrollControl)owner).Children.Contains(el));
            if (VisualContainer.GetWantsMouseInput(el, this) == false)
                return false;

            //IQueryWantsMouseInput aht = VirtualizingCellsControl.GetQueryWantsMouseInput(el);
            //if (aht != null)
            //    return aht.QueryWantsMouseInput(el);
            return true;
        }

        protected virtual bool QueryWantsMouseInput(MouseDevice mouseDevice)
        {
            DependencyObject el = mouseDevice.DirectlyOver as DependencyObject;
            return el != null && el != this && QueryWantsMouseInput(el);
        }

        void TreeGrid_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (this.CurrentCell.HasCurrentCell && this.CurrentCell.IsDroppedDown)
            {
                return;
            }

            GridControlBase grid = (GridControlBase)sender;
            RowColumnIndex cell = this.PointToCellRowColumnIndex(e);
            if (rowSpanUnderMouse != null && rowSpanUnderMouse.Contains(cell))
            {
                rowSpanUnderMouse = null;
            }
            //ignore clicks outside of grid SPID 409, 414
            Point p = e.GetPosition(this);
            bool outSideGrid = p.X >= this.ColumnWidths.TotalExtent ||
                               p.X < 0 ||
                               p.Y < 0 ||
                               p.Y > this.RowHeights.TotalExtent;
            if (outSideGrid)
            {
                if (QueryWantsMouseInput(Mouse.PrimaryDevice))
                    return;
                e.Handled = true;
                return;
            }

            //bool shift = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            //bool ctl = (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            //if (shift && !ctl && this.EnableNodeSelection && cell.ColumnIndex != nodeColumnIndex && cell.RowIndex >= grid.Model.HeaderRows)
            //{
            //    //((GridTreeModel)this.Model).ClearNodes();
            //    this.SelectedNodes.Add(this.GetNodeAtRowIndex(cell.RowIndex));
            //    this.InvalidateCell(GridRangeInfo.Row(cell.RowIndex));
            //    this.InvalidateVisual();
            //}

            double currentcellwidth = 0.0;
            double selectedportion = 0.0;
            if (cell.ColumnIndex == nodeColumnIndex)
            {
                currentcellwidth = this.InternalGrid.ColumnWidths[cell.ColumnIndex];
                Rect currentCellRect = this.InternalGrid.RangeToClippedVisibleRect(cell);
                double CurrentViewWidth = this.InternalGrid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body).Width;
                selectedportion = currentcellwidth - currentCellRect.Width;
                GridTreeNode n;
                GridTreeRowType rowType = GetGridRowType(cell.RowIndex, out n);
                if (rowType == GridTreeRowType.Caption || rowType == GridTreeRowType.Node)
                {
                    Point pt = e.GetPosition(this);
                    Rect r = this.RangeToClippedVisibleRect(GridRangeInfo.Cell(cell.RowIndex, cell.ColumnIndex));
                    //when the InternalGrid's FrozenColumns value is set to 1 then the following if condition is always false.
                    //Because of the selectedportion is greater than 10 and CurrentViewWidth is not equal to currentCellRect.Width + NodeColumnWidth, because here the FrozenColumns is equal to 1.
                    //So the condition is changed as follows                  
                    if (pt.X - r.Left < GetTextIndent(n.Level) && ((selectedportion < 10) || CurrentViewWidth == currentCellRect.Width + NodeColumnWidth || CurrentViewWidth == currentCellRect.Width))
                    {

                        var expandRendererext = this.CellRenderers["ExpanderCell"] as GridTreeExpanderCellRendererExt;
                        if (expandRendererext == null)
                        {
                            this.ExpandCollapseOnExpanderClick();
                            e.Handled = true;
                        }
                    }
                    //else
                    //{
                    //    if (!shift && !ctl && this.EnableNodeSelection && !this.SelectedNodes.Contains(this.GetNodeAtRowIndex(cell.RowIndex)))
                    //    {
                    //        ((GridTreeModel)this.Model).ClearNodes();
                    //        this.SelectedNodes.Add(this.GetNodeAtRowIndex(cell.RowIndex));
                    //        this.InvalidateCell(GridRangeInfo.Row(cell.RowIndex));
                    //        this.InvalidateVisual();
                    //    }
                    //}
                }
            }
        }
#endif

#if !SILVERLIGHT
        internal void ExpandCollapseOnExpanderClick()
        {

            Point e = Mouse.GetPosition(this);
            RowColumnIndex cell = this.PointToCellRowColumnIndex(e);
            GridTreeNode n;
            GridTreeRowType rowType = GetGridRowType(cell.RowIndex, out n);
            if (this.Model.Options.ExcelLikeSelectionFrame)
            {
                rowIndexOfExpandingNode = cell.RowIndex;
                selectFrameRange = this.Model.SelectedRanges.ActiveRange;
                if (n.Expanded)
                {
                    int i = rowIndexOfExpandingNode;
                    int level = n.Level;
                    hiddenNodes.Clear();
                    while (i < Nodes.Count && Nodes[i].Level > level)
                    {
                        if (Nodes[i].IsSelected)
                            hiddenNodes.Add(Nodes[i]);
                        i++;
                    }
                    collapsedRange = GridRangeInfo.Cells(rowIndexOfExpandingNode + 1, selectFrameRange.Left, i, selectFrameRange.Right);
                }
            }

            if (OnExpandStateChanging(n, n.Expanded ? GridTreeNodeActions.Collapsing : GridTreeNodeActions.Expanding))
            {
                inExpandCollapseClick = true;
                if (CurrentCell.IsModified)
                    CurrentCell.ConfirmChanges();
                GridRangeInfoList range = Model.SelectedRanges.Clone();
                saveCurrentCellState = true; //preserve currentcell if possible...
                if (n.Expanded)
                    CollapseNode(cell.RowIndex, n);
                else
                    ExpandNode(cell.RowIndex, n);
                saveCurrentCellState = false;
                if (!range.ToString().Equals(Model.SelectedRanges.ToString()) && !EnableNodeSelection)
                {
                    Model.SelectedRanges.Clear();
                    foreach (GridRangeInfo r in range)
                        Model.SelectedRanges.Add(r);
                }
                ResetDisplay(cell.RowIndex);
                //e.Handled = true;
                OnExpandStateChanged(n, n.Expanded ? GridTreeNodeActions.Expanded : GridTreeNodeActions.Collapsed);
                inExpandCollapseClick = false;
            }
        }
#endif
        /// <summary>
        /// A cancelable event raised before the expand state of a node changes as a result of the user clicking the expand button.
        /// </summary>
        public event GridTreeNodeCancelEventHandler ExpandStateChanging;

        /// <summary>
        /// A notification event that is raised after a node has been expand or collapsed as the result of the user clicking the expand button.
        /// </summary>
        public event GridTreeNodeEventHandler ExpandStateChanged;

        private int nodeCountBeforeChange = 0;
        /// <summary>
        /// Raises the ExpandStateChanging event.
        /// </summary>
        /// <param name="node">The node that was clicked.</param>
        /// <param name="action">The action to be taken.</param>
        /// <returns>True if the action should be completed, false otherwise.</returns>
        protected bool OnExpandStateChanging(GridTreeNode node, GridTreeNodeActions action)
        {
            nodeCountBeforeChange = this.Nodes.Count;

            if (parentTreeControl != null)
            {
                if (!parentTreeControl.OnExpandStateChanging(node, action))
                {
                    return false;
                }
            }
            if (ExpandStateChanging != null)
            {
                GridTreeNodeCancelEventArgs e = new GridTreeNodeCancelEventArgs(node, action);
                ExpandStateChanging(this, e);
                return !e.Cancel;
            }
            return true;
        }

        /// <summary>
        /// Raises the ExpandStateChanged event.
        /// </summary>
        /// <param name="node">The node that was clicked.</param>
        /// <param name="action">The action that was taken.</param>
        protected void OnExpandStateChanged(GridTreeNode node, GridTreeNodeActions action)
        {
            if (parentTreeControl != null)
            {
                parentTreeControl.OnExpandStateChanged(node, action);
            }
            if (ExpandStateChanged != null)
            {
                GridTreeNodeEventArgs e = new GridTreeNodeEventArgs(node, action);
                ExpandStateChanged(this, e);
            }

            if (Model.Options.ExcelLikeSelectionFrame && this.Nodes.Count != nodeCountBeforeChange)
            {
                GridRangeInfo range = selectFrameRange;
                if (range.IsEmpty)
                    return;
                if (action == GridTreeNodeActions.Expanded)
                {
                    if (range.Top <= rowIndexOfExpandingNode && range.Bottom >= rowIndexOfExpandingNode)
                    {
                        range = GridRangeInfo.Cells(range.Top, range.Left, range.Bottom + this.Nodes.Count - nodeCountBeforeChange, range.Right);
                        this.Model.Selections.Clear();
                        this.Model.Selections.SelectRange(range, true);
                    }
                    else if (hiddenNodes.Count > 0)
                    {
                        int i = rowIndexOfExpandingNode;
                        while (i <= Nodes.Count)
                        {
                            if (hiddenNodes.IndexOf(Nodes[i - 1]) > -1)
                            {
                                range = GridRangeInfo.Cells(i, collapsedRange.Left, i + hiddenNodes.Count - 1, collapsedRange.Right);
                                this.Model.Selections.Clear();
                                this.Model.Selections.SelectRange(range, true);
                                hiddenNodes.Clear();
                                break;
                            }
                            i++;
                        }
                    }
                }
                else if (action == GridTreeNodeActions.Collapsed)
                {
                    if (selectFrameRange.IntersectRange(collapsedRange).IsEmpty)
                    { //no intersection so just adjust if the collpased rows are above the selection....
                        if (selectFrameRange.Bottom > collapsedRange.Top)
                        {
                            range = GridRangeInfo.Cells(range.Top + this.Nodes.Count - nodeCountBeforeChange, range.Left, range.Bottom + this.Nodes.Count - nodeCountBeforeChange, range.Right);
                            this.Model.Selections.Clear();
                            this.Model.Selections.SelectRange(range, true);
                        }
                    }
                    else if (selectFrameRange.IntersectRange(collapsedRange) == selectFrameRange)
                    {
                        //collapsedrange contains the selectedrange
                        this.Model.Selections.Clear();
                    }
                    else if (collapsedRange.IntersectRange(selectFrameRange) == collapsedRange)
                    {
                        //selectedrange contains the collapsedrange
                        range = GridRangeInfo.Cells(selectFrameRange.Top, range.Left, selectFrameRange.Bottom + this.Nodes.Count - nodeCountBeforeChange, range.Right);
                        this.Model.Selections.Clear();
                        this.Model.Selections.SelectRange(range, true);
                    }

                    else if (selectFrameRange.Bottom >= collapsedRange.Top && selectFrameRange.Bottom <= collapsedRange.Bottom)
                    {
                        //selectedrange intersects top of collapsedrange
                        range = GridRangeInfo.Cells(selectFrameRange.Top, range.Left, collapsedRange.Top - 1, range.Right);
                        this.Model.Selections.Clear();
                        this.Model.Selections.SelectRange(range, true);
                    }
                    else
                    {
                        //selectedrange intersects bottom of collapsedrange
                        range = GridRangeInfo.Cells(rowIndexOfExpandingNode, range.Left, rowIndexOfExpandingNode + selectFrameRange.Height - hiddenNodes.Count, range.Right);
                        this.Model.Selections.Clear();
                        this.Model.Selections.SelectRange(range, true);
                    }
                }
            }
            else if (Model.Options.ListBoxSelectionMode != GridSelectionMode.None)
                Model.Selections.Clear();
        }

        /// <summary>
        /// Collapses all expanded nodes.
        /// </summary>
        public void CollapseAllNodes()
        {
            foreach (GridTreeNode rootNode in RootNodes)
            {
                CollapseAllNodes(rootNode);
            }
        }

        private void SetAllNodeExpandValues(bool expand, GridTreeNode n)
        {
            if (expand)
            {
                n.Expanded = true;
                GridTreeRequestChildListEventArgs e = new GridTreeRequestChildListEventArgs(n, n.Item, false);
                OnRequestChildList(e);
                n.HasChildNodes = e.ParentNode.ChildNodes != null && e.ParentNode.ChildNodes.Count > 0;
            }
            if (n.ChildNodes != null)
            {
                foreach (GridTreeNode n1 in n.ChildNodes)
                {
                    SetAllNodeExpandValues(expand, n1);
                }
            }
            if (!expand)
                n.Expanded = expand;
        }

        /// <summary>
        /// Collapse all passed-in node as well as child nodes of the passed-in node.
        /// </summary>
        /// <param name="n">The node to be collapsed.</param>
        public void CollapseAllNodes(GridTreeNode n)
        {
            SetAllNodeExpandValues(false, n);
            ReloadNodes();
            //ResetGrid();
            this.InvalidateCells();
            this.InvalidateVisual(true);
        }

        /// <summary>
        /// Expand all nodes.
        /// </summary>
        public void ExpandAllNodes()
        {
            bool hasNode = false;
            foreach (GridTreeNode rootNode in RootNodes)
            {
                // Code change reverted to avoid Grid refreshing issue on collection change.
                if (!rootNode.HasChildNodes)
                {
                    ResetGrid();
                    continue;
                }

                ExpandAllNodes(rootNode);
                hasNode = true;
            }
            if (hasNode)
            {
                this.UnloadArrangedCells();
                this.InvalidateCells();
                this.InvalidateVisual(true);
            }
        }

        /// <summary>
        /// Expands the given node and all of its child nodes.
        /// </summary>
        /// <param name="n">The node to be expanded.</param>
        public void ExpandAllNodes(GridTreeNode n)
        {
            SetAllNodeExpandValues(true, n);
            //ReloadNodes();
            ResetGrid();
            CollapseNode(n);
            ExpandNode(n);
            this.InvalidateCells();
            this.InvalidateVisual(true);
        }

        /// <summary>
        /// Expands the single node.
        /// </summary>
        /// <param name="n">The node to be expanded.</param>
        public void ExpandNode(GridTreeNode n)
        {
            // Code change reverted to avoid Grid refreshing issue on collection change.
            //if (n.Expanded || !n.HasChildNodes)
            if (n.Expanded)
                return;

            int loc = this.GetRowIndexFromNode(n);  //this.Nodes.IndexOf(n);
            if (loc > -1)
            {
                ExpandNode(loc, n);
            }
        }

        internal void ExpandNode(GridTreeNode n, GridTreeNode changedChildNode)
        {
            if (n.Expanded)
                return;

            int loc = this.GetRowIndexFromNode(n);
            if (loc > -1)
            {
                ExpandNode(loc, n,changedChildNode);
            }
        }

        /// <summary>
        /// Collapses a single node.
        /// </summary>
        /// <param name="n">The node to be collapsed.</param>
        /// <param name="shouldMaintainSelection" use to check whether selection should maintain or not while collapsing GridTreeNode></param>
        public void CollapseNode(GridTreeNode n, bool shouldMaintainSelection)
        {
            if (!n.Expanded)
                return;

            int loc = GetRowIndexFromNode(n); //this.Nodes.IndexOf(n);
            if (loc > -1)
            {
                if (shouldMaintainSelection)
                    inExpandCollapseClick = true;
                CollapseNode(loc, n);
                if (shouldMaintainSelection)
                    inExpandCollapseClick = false;
            }
        }

        /// <summary>
        /// Collapses a single node.
        /// </summary>
        /// <param name="n">The node to be collapsed.</param>
        public void CollapseNode(GridTreeNode n)
        {
            CollapseNode(n, false);
            //if (!n.Expanded)
            //    return;

            //int loc = this.Nodes.IndexOf(n);
            //if (loc > -1)
            //{
            //    inExpandCollapseClick = true;
            //    CollapseNode(loc + Model.HeaderRows, n);
            //    inExpandCollapseClick = false;
            //}
        }

        /// <summary>
        /// Expand the GridNode that corresponds to the given grid row index.
        /// </summary>
        /// <param name="gridRowIndex">The rowIndex of the GridNode to be expanded.</param>
        public void ExpandNode(int gridRowIndex)
        {
            GridTreeNode n;
            GridTreeRowType rt = GetGridRowType(gridRowIndex, out n);
            if (n.Expanded)
                return;
            OnExpandStateChanging(n, GridTreeNodeActions.Expanding);
            if (rt == GridTreeRowType.Caption || rt == GridTreeRowType.Node)
            {
                ExpandNode(gridRowIndex, n);
            }
            OnExpandStateChanged(n, GridTreeNodeActions.Expanded);
        }

        /// <summary>
        /// Collapse the GridNode that corresponds to the given grid row index.
        /// </summary>
        /// <param name="gridRowIndex">The rowIndex of the GridNode to be collapsed.</param>
        public void CollapseNode(int gridRowIndex)
        {
            GridTreeNode n;
            GridTreeRowType rt = GetGridRowType(gridRowIndex, out n);
            if (!n.Expanded)
                return;
            OnExpandStateChanging(n, GridTreeNodeActions.Collapsing);
            if (rt == GridTreeRowType.Caption || rt == GridTreeRowType.Node)
            {
                CollapseNode(gridRowIndex, n);
            }
            OnExpandStateChanged(n, GridTreeNodeActions.Collapsed);
        }

        private GridTreeNode savedNode = null;
        //   private int savedColIndex = -1;
        private string savedColumnName = "";
        private bool saveCurrentCellState = false;
        private RowColumnIndex CCRowColl;
        internal void SaveCurrentCellState()
        {
            if (this.CurrentCell.HasCurrentCell && ((CCRowColl.RowIndex != CurrentCell.RowIndex || CCRowColl.ColumnIndex != CurrentCell.ColumnIndex) 
                ||savedNode != this.ParentTreeControl.SelectedNode as GridTreeNode))
            {
                savedNode = this.GetNodeAtRowIndex(this.CurrentCell.RowIndex);
                savedColumnName = this.ColumnIndexToName(this.CurrentCell.ColumnIndex);
                this.CurrentCell.Deactivate();
                CCRowColl.RowIndex = CurrentCell.RowIndex;
                CCRowColl.ColumnIndex = CurrentCell.ColumnIndex;
            }
        }

        internal void RestoreCurrentCellState()
        {
            if (savedNode != null)
            {
                int rowIndex=0;
                if (savedNode == this.ParentTreeControl.SelectedNode as GridTreeNode)
                    rowIndex = this.GetRowIndexFromNode(this.ParentTreeControl.SelectedNode as GridTreeNode);
                else
                    rowIndex = this.GetRowIndexFromNode(savedNode);
                if (rowIndex > 0)
                {
                    int colIndex = this.ColumnNameToPosition(savedColumnName) + 1;
                    this.InvalidateCell(GridRangeInfo.Cell(rowIndex, colIndex));//savedColIndex));
                    int topRow = this.TopRowIndex;
#if !SILVERLIGHT                    
                    this.CurrentCell.MoveTo(rowIndex, colIndex,new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.NoSelectRange));
#else
                    this.CurrentCell.MoveTo(rowIndex, colIndex);//savedColIndex);
#endif
                    if (this.TopRowIndex != topRow && topRow > 0 && topRow < this.Nodes.Count)
                    {
                        this.TopRowIndex = topRow;
                    }
                }
            }
        }

        private void ExpandNode(int gridRowIndex, GridTreeNode n)
        {
            ExpandNode(gridRowIndex, n, null);
        }

        private void ExpandNode(int gridRowIndex, GridTreeNode n, GridTreeNode changedChildNode)
        {
            lockSelectedNodes = true;
            if (saveCurrentCellState)
            {
                SaveCurrentCellState();
            }

            int start = gridRowIndex;
            n.Expanded = true;
            GridTreeRequestChildListEventArgs e = new GridTreeRequestChildListEventArgs(n, n.Item, false);
            OnRequestChildList(e);

            if (!n.HasChildNodes)
            {
                n.HasChildNodes = e.ParentNode.ChildNodes != null && e.ParentNode.ChildNodes.Count > 0;
            }

            int loc = Nodes.IndexOf(n);
            int save = loc;
            List<GridTreeNode> newNodes = new List<GridTreeNode>();
            bool enablerendercheckifglyphneeded = this.ParentTreeControl.EnableRenderCheckIfGlyphNeeded;
            foreach (GridTreeNode n1 in n.ChildNodes)
            {
                loc++;
                newNodes.Add(n1);

                if (HideEmptyChildGlyphs && !enablerendercheckifglyphneeded)
                {
                    if (changedChildNode == null || changedChildNode.Equals(n1))
                    {
                        e = new GridTreeRequestChildListEventArgs(n1, n1.Item, false);
                        OnRequestChildList(e);

                        if (!n1.HasChildNodes)
                        {
                            n1.HasChildNodes = e.ParentNode.ChildNodes != null && e.ParentNode.ChildNodes.Count > 0;
                        }
                    }
                }

                gridRowIndex++;

                if (n1.Expanded)
                {
                    if (HideEmptyChildGlyphs ||
                        n1.ChildNodes.Count > 0)
                    {
                        InsertExpandedChildNodes(n1, ref loc, ref gridRowIndex, ref newNodes);
                    }
                }
            }
            if (n.HasChildNodes && AllowAutoSizingNodeColumn &&
                (this.PercentSizingBehavior == GridPercentColumnSizingBehavior.None || Columns[0].PercentWidth == GridTreeColumnWidthSizer.NotSet))
            {
                double w = GetWidthOfNodeColumn(n.Level + 1);
                if (w > Model.ColumnWidths[nodeColumnIndex])
                    Model.ColumnWidths[nodeColumnIndex] = w;
            }
            if (gridRowIndex > start)
            {
                Model.InsertRows(start + 1, gridRowIndex - start);
                Nodes.InsertRange(save + 1, newNodes);
                if (SupportRowSizing)
                {
                    gridRowIndex = start + 1;
                    foreach (GridTreeNode node in newNodes)
                    {
                        if (node.NodeHeight != double.NaN)
                        {
                            this.RowHeights[gridRowIndex++] = node.NodeHeight;
                        }
                    }
                }
            }
            if (saveCurrentCellState)
            {
                RestoreCurrentCellState();
            }
            lockSelectedNodes = false;
        }

        private void InsertExpandedChildNodes(GridTreeNode n, ref int loc, ref int gridRowIndex, ref List<GridTreeNode> newNodes)
        {
            foreach (GridTreeNode n1 in n.ChildNodes)
            {
                loc++;
                newNodes.Add(n1);
                gridRowIndex++;
                if (n1.Expanded)
                {
                    if (n.HasChildNodes && AllowAutoSizingNodeColumn &&
               (this.PercentSizingBehavior == GridPercentColumnSizingBehavior.None || Columns[0].PercentWidth == GridTreeColumnWidthSizer.NotSet))
                    {
                        double w = GetWidthOfNodeColumn(n.Level + 1);
                        if (w > Model.ColumnWidths[nodeColumnIndex])
                            Model.ColumnWidths[nodeColumnIndex] = w;
                    }

                    InsertExpandedChildNodes(n1, ref loc, ref gridRowIndex, ref newNodes);
                }
            }
        }

        internal bool lockSelectedNodes = false;
        private void CollapseNode(int gridRowIndex, GridTreeNode n)
        {
            lockSelectedNodes = true;

            if (saveCurrentCellState)
            {
                SaveCurrentCellState();
            }

            n.Expanded = false;
            gridRowIndex++;

            int save = gridRowIndex;
            int count = 0;

            if (gridRowIndex < Model.RowCount - Model.FooterRows)
            {
                GridTreeNode n1;
                GridTreeRowType rt;
                rt = GetGridRowType(gridRowIndex, out n1);
                if (n1 != null)
                {
                    int loc = Nodes.IndexOf(n1);
                    while (n1 != null && n1.Level > n.Level && loc + count < Nodes.Count)
                    {
                        count++;
                        if (loc + count < Nodes.Count)
                            n1 = Nodes[loc + count];
                        else
                            n1 = null;
                    }
                    Nodes.RemoveRange(loc, count);
                }
            }
            ResetGrid(save);
            if (saveCurrentCellState)
            {
                RestoreCurrentCellState();
            }
            lockSelectedNodes = false;
        }

        #endregion

        #region sorting

        /// <exclude/>
        public static readonly DependencyProperty SortingEnabledProperty;

        /// <summary>
        /// Gets or sets whether clicking on column headers will sort the column.
        /// </summary>
        public bool AllowSort
        {
            get { return (bool)GetValue(SortingEnabledProperty); }
            set
            {
                if (AllowSort != value)
                {
                    SetValue(SortingEnabledProperty, value);
                    this.InvalidateCell(GridRangeInfo.Row(this.Model.HeaderRows - 1));

                    //if (HasDataSource)
                    //    ReloadNodes();
                }
            }
        }

        //private ListSortDirection sortDirection = ListSortDirection.Ascending;

        ///// <summary>
        ///// Gets or sets the sort direction to be used when the tree is next displayed,  
        ///// </summary>
        ///// <remarks>The value is automatically toggled when you click the column header to trigger
        ///// a new sort. 
        ///// 
        ///// You should use SortTree to programatically sort the tree contents.</remarks>
        //public ListSortDirection SortDirection
        //{
        //    get { return sortDirection; }
        //    set { sortDirection = value; }
        //}

        private string sortProperty = "";

        /// <summary>
        /// Gets or sets the property to be sorted.
        /// </summary>
        /// <remarks>This string holds the column name to be sorted. You can specify a SortDirection by
        /// appending a space followed by either ASC or DESC. In addition, you can specify multicolumn sorts
        /// by passing several columns separated by commas. For example, "Price ASC, Weight DESC" would 
        /// indicate to first sort in ascending order by the Price column, and then sort in descending
        /// order by the Weight column.</remarks>
        public string SortProperty
        {
            get
            {
                if (sortStates == null)
                {
                    sortStates = SortState.GetSortStatesFromString(sortProperty);
                }
                return sortProperty;
            }
            set
            {
                if (sortStates != null)
                {
                    sortStates.Clear();
                }
                sortProperty = value;
                if (value == "")
                {
                    PopulateGridNodes(true);
                    //CollapseAllNodes();
                }
                else
                {
                    sortStates = SortState.GetSortStatesFromString(sortProperty);
                }
            }
        }

        private List<SortState> sortStates;

        /// <summary>
        /// Gets or sets a list of the SortStates of currently sorted columns.
        /// </summary>
        public List<SortState> SortStates
        {
            get
            {
                if (sortStates == null)
                {
                    sortStates = new List<SortState>();
                }
                return sortStates;
            }
            set { sortStates = value; }
        }

        /// <summary>
        /// Gets the property name for the given column index.
        /// </summary>
        /// <param name="columnIndex">The column index.</param>
        /// <returns>The name of the property appearing in this column.</returns>
        public string PropertyNameFromColumnIndex(int columnIndex)
        {
            if (columnIndex > 0 && columnIndex <= Columns.Count)
            {
                return Columns[columnIndex - 1].MappingName;
            }

            return "";
        }

        private void SortList(List<GridTreeNode> list)
        {
            if (SortProperty == null || SortProperty.Length == 0)
            {
                //no sort...
            }
            else
            {
                list.Sort(SortComparer);
            }
        }

        /// <summary>
        /// Sorts the tree contents within tree levels.
        /// </summary>
        /// <param name="colName">The name of the column to be sorted.</param>
        /// <param name="dir">The direction of the sort.</param>
        public void SortTree(string colName, ListSortDirection dir)
        {
            SortTree(colName, dir, true);
        }
        /// <summary>
        /// Sorts the tree contents within tree levels.
        /// </summary>
        /// <param name="colName">The name of the column to be sorted.</param>
        /// <param name="dir">The direction of the sort.</param>
        /// <param name="clearSort">Whether any existing sort should be cleared before the new sort is applied.</param>
        public void SortTree(string colName, ListSortDirection dir, bool clearSort)
        {
            if (!AllowSort)
                return;
            if (clearSort || SortProperty.Length == 0)
            {
                SortProperty = SortState.GetCompositeString(colName, dir);
            }
            else
            {
                SortProperty += string.Format(",{0}", SortState.GetCompositeString(colName, dir));
            }

            if (RootNodes.Count > 1)
            {
                RootNodes.Sort(SortComparer);
            }

            ReloadNodes();
        }

        public bool IsPropertySorted(string propertyName, out SortState state)
        {
            bool b = false;
            state = null;
            foreach (SortState s in SortState.GetSortStatesFromString(SortProperty))
            {
                if (s.Property == propertyName)
                {
                    b = true;
                    state = s;
                    break;
                }
            }
            return b;
        }
        IComparer<GridTreeNode> sortComparer;

        /// <summary>
        /// Gets or sets the IComparer object used in the tree sorting. 
        /// </summary>
        /// <remarks>
        /// The default implementation assumes the underlying node items implement IComparable 
        /// and uses that implementation for the sorting comparisons within the columns. If your
        /// objects are not IComparable, then you need to provide a SortComparer that properly
        /// sorts the GridNodes depending upon the SortProperty value of GridNode.Item.
        /// </remarks>
        public IComparer<GridTreeNode> SortComparer
        {
            get
            {
                if (sortComparer == null)
                {
                    sortComparer = new NodeComparer(this);
                }
                return sortComparer;
            }
            set { sortComparer = value; }
        }

        /// <summary>
        /// Forces the whole tree to redraw itself.
        /// </summary>
        public void ResetDisplay()
        {
            UnloadArrangedCells();
            ResetGrid();
            InvalidateVisual(true);
        }


        /// <summary>
        /// Forces the whole grid to redraw itself with row heights being reset from given grid rowindex.
        /// </summary>
        /// <param name="startRow">The grid rowindex from where the redrawing should take place.</param>
        public void ResetDisplay(int startRow)
        {
            UnloadArrangedCells();
            ResetGrid(startRow);
            InvalidateVisual(true);
        }


        public int GetRowIndexFromItem(object item)
        {
            int recordIndex = GetRecordsIndexFromItem(item);
            if (recordIndex != -1)
            {
                if (this.UnboundRowPosition == Position.Top)
                    return recordIndex + this.Model.HeaderRows + this.UnboundRowsCount;
                else
                    return recordIndex + this.Model.HeaderRows;
            }
            return -1;
        }

        public int GetRowIndexFromNode(GridTreeNode n)
        {
            int recordIndex = this.Nodes.IndexOf(n);
            if (recordIndex != -1)
            {
                if (this.UnboundRowPosition == Position.Top)
                    return recordIndex + this.Model.HeaderRows + this.UnboundRowsCount;
                else
                    return recordIndex + this.Model.HeaderRows;
            }
            return -1;
        }

        public int GetRecordsIndexFromRowIndex(int rowindex)
        {
            if (rowindex != -1)
            {
                if (this.UnboundRowPosition == Position.Top)
                    return rowindex - this.Model.HeaderRows - this.UnboundRowsCount;
                else
                    return rowindex - this.Model.HeaderRows;
            }
            return -1;
        }

        public int ResolveIndexToUnboundRowPosition(int rowindex)
        {
            if (rowindex != -1)
            {
                if (this.UnboundRowPosition == Position.Top)
                    return rowindex - this.Model.HeaderRows;
                else
                    return rowindex - this.Model.HeaderRows - Nodes.Count;
            }
            return -1;
        }

        public int ResolveIndexToColumnIndex(int index)
        {
            if (index >-1)
                return index - this.Model.HeaderRows;
            else return index;
        }

        public int GetRecordsIndexFromItem(object item)
        {
#if !SILVERLIGHT
            if (item is DataRowView)
            {
                DataRow dr = ((DataRowView)item).Row;
                for (int count = 0; count < this.Nodes.Count; count++)
                {
                    if (((DataRowView)nodes[count].Item).Row.Equals(dr))
                    {
                        return count;
                    }
                }
            }
            else
#endif
            {
                for (int count = 0; count < this.Nodes.Count; count++)
                {
                    if (nodes[count].Item.Equals(item))
                    {
                        return count;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the Node at the specified grid row index.
        /// </summary>
        /// <param name="gridRowIndex">The grid row index.</param>
        /// <returns>The Node at the specified grid row index.</returns>
        public GridTreeNode GetNodeAtRowIndex(int gridRowIndex)
        {
            int recordindex = GetRecordsIndexFromRowIndex(gridRowIndex);
            if (recordindex > -1 && Nodes.Count > recordindex)
                return Nodes[recordindex];
            return null;
            //  return (gridRowIndex >= Model.HeaderRows && gridRowIndex - Model.HeaderRows < Nodes.Count)
            //? Nodes[gridRowIndex - Model.HeaderRows] : null;
        }

        public GridTreeRowType GetGridRowType(int gridRowIndex)
        {
            GridTreeNode n;
            return GetGridRowType(gridRowIndex, out n);
        }

        internal GridTreeRowType GetGridRowType(int gridRowIndex, out GridTreeNode n)
        {
            n = GetNodeAtRowIndex(gridRowIndex);
            GridTreeRowType t = GridTreeRowType.Node;
            if (gridRowIndex < Model.HeaderRows)
            {
                t = GridTreeRowType.Header;
            }
            else if (this.UnboundRowsCount > 0)
            {
                if (this.UnboundRowPosition == Position.Top && gridRowIndex < Model.HeaderRows + this.UnboundRowsCount)
                    t = GridTreeRowType.UnboundRow;
                else if (this.UnboundRowPosition == Position.Bottom && gridRowIndex >= Model.RowCount - this.UnboundRowsCount)
                    t = GridTreeRowType.UnboundRow;
            }
            //else if (gridRowIndex > Model.RowCount - Model.FooterRows)
            //{
            //    t = GridTreeRowType.Footer;
            //}
            else
            {
                if (n != null && n.Item is string)
                    t = GridTreeRowType.Caption;
            }

            return t;
        }

        /// <summary>
        /// Returns the Column.MappingName for the column
        /// whose grid ColumnIndex is given.
        /// </summary>
        /// <param name="gridColumnIndex">The grid ColumnIndex.</param>
        /// <returns>The MappingName.</returns>
        public string ColumnIndexToName(int gridColumnIndex)
        {
            int pos = gridColumnIndex - nodeColumnIndex;
            if (pos > -1 && pos < Columns.Count)
                return Columns[pos].MappingName;

            return "";
        }

        /// <summary>
        /// Returns the position in the Columns collection for the given grid column index.
        /// </summary>
        /// <param name="gridColumnIndex">The grid column index.</param>
        /// <returns>The position in the Columns collection.</returns>
        public int ColumnIndexToPosition(int gridColumnIndex)
        {
            int pos = gridColumnIndex - nodeColumnIndex;
            if (pos > -1 && pos < Columns.Count)
                return pos;
            return -1;
        }

        /// <summary>
        /// Returns the grid column index for the given position.
        /// </summary>
        /// <param name="position">An valid inndex of the Columns collection.</param>
        /// <returns>The column index in the grid, and a -1 if the position is invalid.</returns>
        public int PositionToColumnIndex(int position)
        {
            int gridColumnIndex = position + nodeColumnIndex;
            if (gridColumnIndex > -1 && gridColumnIndex <= this.Model.ColumnCount)
                return gridColumnIndex;
            return -1;
        }

        /// <summary>
        /// Returns the position in the Columns collection for the given column name.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>The position in the Columns collection.</returns>
        public int ColumnNameToPosition(string columnName)
        {
            int pos = -1;
            bool found = false;
            foreach (GridTreeColumn tc in Columns)
            {
                pos++;
                if (tc.MappingName == columnName)
                {
                    found = true;
                    break;
                }
            }
            if (found)
                return pos;

            return -1;
        }

        #endregion

        public event GridTreeQueryUnboundCellInfoEventHandler QueryUnboundCellInfo;

        #region grid events - QueryCellInfo / CommitCellInfo / LineSizedChanged / QueryAllowDragColumns

        internal void Model_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            GridTreeNode n;
            GridTreeRowType rowType = GetGridRowType(e.Cell.RowIndex, out n);

            if (n != null && n.Level > -1 && n.Level < LevelStyles.Count && !e.Handled)
            {
                e.Style.ModifyStyle(LevelStyles[n.Level], StyleModifyType.Override);
            }
            //Header Rows
            if (rowType == GridTreeRowType.Header && e.Cell.RowIndex >= this.Model.HeaderRows - 1)
            {
                e.Style.ModifyStyle(ColumnHeaderStyle, StyleModifyType.Override);

                if (e.Cell.ColumnIndex >= nodeColumnIndex)
                {
                    GridTreeColumn tc = Columns[e.Cell.ColumnIndex - nodeColumnIndex];
                    e.Style.Text = (tc.HeaderText != null && tc.HeaderText.Length > 0) ? tc.HeaderText : tc.MappingName;
                    e.Style.CellType = SortHeaderCellType;

                    if (SortProperty != null && SortProperty.Length > 0)
                    {
                        int loc = FindPropertyNameInStates(ColumnIndexToName(e.Cell.ColumnIndex));
                        if (loc > -1)
                        {
                            {
                                e.Style.Tag = sortStates[loc].Direction;
                            }
                        }
                    }
                }
            }
            //Expander Column
            else if ((n != null) && e.Cell.ColumnIndex == nodeColumnIndex)
            {
                if (rowType == GridTreeRowType.Caption || rowType == GridTreeRowType.Node)
                {
                    GridTreeColumn tc = Columns[0];
                    if (!e.Handled)
                    {
                        e.Style.ModifyStyle(tc.StyleInfo, StyleModifyType.Override);
                    }
                    if (this.ParentTreeControl.UpdateMode != UpdateMode.RowCachedMode)
                        e.Style.CellValue = GetValueFromNode(tc.MappingName, n);
                    else
                    {
                        if (oldRowColIndex.RowIndex != e.Style.CellRowColumnIndex.RowIndex)
                            e.Style.CellValue = GetValueFromNode(tc.MappingName, n);
                        else
                        {

                            if (this.CachedStorage.ContainsKey(tc.MappingName))//while Update Mode is RowCache, CellValue should get from CatchedStroage not from GridTreeItem.
                                e.Style.CellValue = this.CachedStorage[tc.MappingName];
                            else
                                e.Style.CellValue = GetValueFromNode(tc.MappingName, n);
                        }
                    }
                    e.Style.CellType = expanderCellType;
                    e.Style.Tag = n;
                    double dx = GetTextIndent(n.Level);
                    e.Style.TextMargins.Left = dx;
                    if (!ShowExpandColumnBorders)
                    {
                        e.Style.Borders.Bottom = null;
                        e.Style.Borders.Top = null;
                    }
                }
            } // Code for UnboundColumn
            else if (!e.Handled && e.Cell.ColumnIndex > this.Model.HeaderRows && this.HasUnboundColumns && this.Columns[ResolveIndexToColumnIndex(e.Cell.ColumnIndex)].IsUnbound)
            {
                GridTreeUnboundColumn column = this.Columns[ResolveIndexToColumnIndex(e.Cell.ColumnIndex)] as GridTreeUnboundColumn;

                if(column.StyleInfo!=null)
                    e.Style.ModifyStyle(column.StyleInfo, StyleModifyType.Override);

                if (QueryUnboundCellInfo != null )
                    QueryUnboundCellInfo(this, new GridTreeUnboundCellInfoEventArgs(e.Cell, e.Style, column));

                this.RaiseQueryUnboundCellInfo(column, e.Style, n);
            }
            else if ((n != null) && e.Cell.ColumnIndex > nodeColumnIndex && e.Cell.RowIndex > this.Model.HeaderRows - 1)
            {
                if (rowType == GridTreeRowType.Caption || rowType == GridTreeRowType.Node && e.Cell.ColumnIndex - nodeColumnIndex < Columns.Count)
                {
                    GridTreeColumn tc = Columns[e.Cell.ColumnIndex - nodeColumnIndex];
                    if (!e.Handled)
                    {
                        e.Style.ModifyStyle(tc.StyleInfo, StyleModifyType.Override);
                    }
                    if (this.ParentTreeControl.UpdateMode != UpdateMode.RowCachedMode)
                        e.Style.CellValue = GetValueFromNode(tc.MappingName, n);
                    else
                    {
                        if (oldRowColIndex.RowIndex != e.Style.CellRowColumnIndex.RowIndex)
                            e.Style.CellValue = GetValueFromNode(tc.MappingName, n);
                        else //if (editingIsInSameRow)
                        {
                            if (this.CachedStorage.ContainsKey(tc.MappingName))
                                e.Style.CellValue = this.CachedStorage[tc.MappingName];
                            else
                                e.Style.CellValue = GetValueFromNode(tc.MappingName, n);
                        }
                    }
                }
            }
            //Unbound Row
            else if (rowType == GridTreeRowType.UnboundRow)
            {
                e.Style.Tag = "UnboundRow";
            }
            else
            {
                e.Style.ModifyStyle(RowHeaderStyle, StyleModifyType.Override);
            }
            //this is modified regarding the issue id 9876
            //if (e.Cell.ColumnIndex > 0 && n != null && n.IsSelected && ((this.Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.AlphaBlend) == 0)
            //    && (this.EnableNodeSelection || cellIsSelected(n, e.Cell.ColumnIndex)) && Model.SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(e.Cell.RowIndex, e.Cell.ColumnIndex)))
            //    if (e.Cell.ColumnIndex > 0 && n != null && n.IsSelected && ((this.Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.AlphaBlend) == 0)
            //        && (this.EnableNodeSelection || cellIsSelected(n, e.Cell.ColumnIndex)))
            //    {
            //        if ((this.Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceBackground) != 0)
            //        {
            //            if (this.IsLegacyStyleEnabled)
            //                e.Style.Background = Model.Options.HighlightSelectionBackground;
            //            else
            //                e.Style.Background = this.GetVisualStyle(VisualStyle).HighlightSelectionBackground;
            //        }
            //        else
            //        {
            //            e.Style.Background = this.nodeHighlightBrush;
            //        }

            //        if ((this.Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceTextColor) != 0)
            //        {
            //            e.Style.Foreground = Model.Options.HighlightSelectionForeground;
            //        }
            //    }
        }
        public event GridTreeQueryUnboundColumnEventHandler QueryUnboundColumnValue;

        /// <summary>
        /// Raises the query unbound cell info.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="style">The style.</param>
        /// <param name="Node">The node.</param>
        private void RaiseQueryUnboundCellInfo(GridTreeUnboundColumn column, GridStyleInfo style, GridTreeNode Node)
        {
            object record = Node.Item;
            var Args=new GridTreeUnboundColumnEventArgs(style, Node, record, column);

            if (QueryUnboundColumnValue != null)
                QueryUnboundColumnValue(this, Args);
            //Cell value will set when the QueryUnboundColumnValue event is not hooked in Application side
            if (!Args.Handled && record != null && column != null && (column.Expression != string.Empty || column.Format != string.Empty))
                style.CellValue = this.GetValueFromNode(column.MappingName, Node);
        }

        public object GetUnboundColumnValue(object record, GridTreeUnboundColumn column)
        {
            var model = this.Model;
            object value = null;
            if (column.Format != string.Empty)
            {
                value = column.Format.FormatByName(null, (key) =>
                {
                    var itemProperties = this.ItemProperties;
                    var pd = itemProperties.GetPropertyDescriptor(key);
                    if (pd != null)
                    {
                        return pd.GetValue(record);
                    }

                    return null;
                });
            }
            else if (column.Expression != string.Empty)
            {
                value = column.ComputedValue(record);
            }

            return value;
        }

        private bool cellIsSelected(GridTreeNode n, int colIndex)
        {
            return n.SelectedColumns.IndexOf(this.ColumnIndexToName(colIndex)) > -1;
        }


#if !SILVERLIGHT
        /// <exclude/>
        protected override void OnRenderCell(DrawingContext dc, RenderCellArgs rca)
        {
            base.OnRenderCell(dc, rca);

            if ((this.Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.AlphaBlend) != 0)
            {
                GridTreeNode n;
                GridTreeRowType rowType = GetGridRowType(rca.CellRowColumnIndex.RowIndex, out n);
                if (rca.CellRowColumnIndex.ColumnIndex > 0 && n != null && n.IsSelected
                    && (this.EnableNodeSelection || cellIsSelected(n, rca.CellRowColumnIndex.ColumnIndex)))
                {
                    var newRect = new Rect(new Point(rca.CellRect.Left - this.Model.TableStyle.BorderMargins.Left, rca.CellRect.Top - this.Model.TableStyle.BorderMargins.Top),
                           new Point(rca.CellRect.Right + this.Model.TableStyle.BorderMargins.Right, rca.CellRect.Bottom + this.Model.TableStyle.BorderMargins.Bottom));
                    dc.DrawRectangle(this.Model.Options.HighlightSelectionAlphaBlend, null, newRect);
                }
            }
        }
#endif

        /// <summary>
        /// Returns the index of the property name in the SortStates collection
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>The index.</returns>
        public int FindPropertyNameInStates(string name)
        {
            int loc = 0;
            bool found = false;
            foreach (SortState state in sortStates)
            {
                if (state.Property == name)
                {
                    found = true;
                    break;
                }
                loc++;
            }
            if (!found)
            {
                loc = -1;
            }
            return loc;
        }

        internal double GetTextIndent(int level)
        {
            double d = (level + 1) * nodeColumnWidth + 1;
#if !SILVERLIGHT
            switch (ExpandGlyphType)
            {
                case GridTreeExpandGlyph.PlusMinusLines:
                case GridTreeExpandGlyph.PlusMinus:
                    d = (level + 1) * (nodeColumnWidth - 9) + 10;
                    break;
                case GridTreeExpandGlyph.Custom:
                    d = (level + 1) * nodeColumnWidth + 5;
                    break;
                case GridTreeExpandGlyph.Themed:
                    d = (level + 1) * nodeColumnWidth + 5;
                    break;
            }
#endif
            return d;
        }

        /// <summary>
        /// Gets the value of a particular column in a GridTreeNode.
        /// </summary>
        /// <param name="propertyName">The column name.</param>
        /// <param name="node">The GridTreeNode.</param>
        /// <returns>The value.</returns>
        public object GetValueFromNode(string propertyName, GridTreeNode node)
        {
            object o = null;
            o = GetValueFromComponent(propertyName, node.Item);
#if !SILVERLIGHT
            if (o == null && ItemProperties[propertyName] == null)
#else
            if (o == null && ! ItemProperties.ContainsKey(propertyName))
#endif
            {
                var unboundColumns = Columns.Where(c => c.MappingName == propertyName && c.IsUnbound == true).ToList();
                if (unboundColumns.Count > 0)
                    o = this.GetUnboundColumnValue(node.Item, unboundColumns[0] as GridTreeUnboundColumn);
            }
            if (o == null)
                o = OnQueryUnknownProperty(node, propertyName);
            return o;
        }
        private object GetValueFromComponent(string propertyName, object comp)
        {
#if !SILVERLIGHT
            if (comp is DataRowView)
            {
                return ((System.Data.DataRowView)comp)[propertyName];
            }
            else if (comp is DataRow)
            {
                return ((System.Data.DataRow)comp)[propertyName];
            }
            else
            {
                //if (ItemProperties[propertyName] != null)
                {
                    return ItemProperties.GetValue(comp, propertyName);
                    //return ItemProperties[propertyName].GetValue(comp);
                }
            }
#else
            if (ItemProperties.GetValue(comp, propertyName) != null)
            {
                return ItemProperties.GetValue(comp, propertyName);
            }
            return null; 
#endif

        }

        /// <summary>
        /// Event raised whenever a value for an unknown property is required.
        /// </summary>
        public event GridTreeQueryUnknownPropertyHandler QueryUnknownProperty;

        /// <summary>
        /// Raises the QueryUnknownProperty event.
        /// </summary>
        /// <param name="node">The node whose property is being requested.</param>
        /// <param name="propertyName">The requested property name.</param>
        /// <returns>The requested value.</returns>
        protected virtual object OnQueryUnknownProperty(GridTreeNode node, string propertyName)
        {
            if (QueryUnknownProperty != null)
            {
                GridTreeQueryUnknownPropertyEventArgs e = new GridTreeQueryUnknownPropertyEventArgs(node, propertyName);
                QueryUnknownProperty(this, e);
                return e.Value;
            }
            return null;

        }
        internal void Model_CommitCellInfo(object sender, GridCommitCellInfoEventArgs e)
        { 
            if (e.Style.ReadOnly)
            {
                e.Handled = true;
                return;
            }


            if (this.ParentTreeControl.UpdateMode == UpdateMode.LostFocus || ParentTreeControl.UpdateMode== UpdateMode.PropertyChanged || !this.CurrentCell.IsEditing)//While pasting pasted value should commit even UpdateMode not equal to lost focus. So here we check the condition CurrentCell.IsEditing.
                SaveValuesToCollection(e.Style.CellValue, e.Style.RowIndex, e.Style.ColumnIndex);
            else if (this.ParentTreeControl.UpdateMode == UpdateMode.RowCachedMode)
            {
                //Here we have added the CatchedStroage value.
                GridTreeNode n;
                GridTreeRowType rowType = GetGridRowType(e.Cell.RowIndex, out n);
                if (e.Cell.ColumnIndex >= nodeColumnIndex)
                {
                    if (rowType == GridTreeRowType.Caption || rowType == GridTreeRowType.Node)
                    {
                        GridTreeColumn tc = Columns[e.Cell.ColumnIndex - nodeColumnIndex];
                        if (this.CachedStorage.ContainsKey(tc.MappingName))
                        {
                            this.CachedStorage.Remove(tc.MappingName);
                        }
                        this.CachedStorage.Add(tc.MappingName, e.Style.CellValue);

                        GridTreeNode oldNode;
                        GridTreeRowType oldRowType = GetGridRowType(oldRowColIndex.RowIndex, out oldNode);
                        //RowCache flow=>While editing the Same row, value  is stored in CachedStroage in CommitCellInfo event. while moving to another row. in currentcellMoving Event we have check CurrentRow and OldRow. if both are Unequal means commt the CachedStorage value. But in Key navigation CurrentCellMoving event fired fist then only CommitCellInfo event fired. So last edited value not stroed. so here we have check  OldRow and currentrow then commit here.
                        if ((oldRowType == GridTreeRowType.Caption || oldRowType == GridTreeRowType.Node) && oldRowColIndex.RowIndex != e.Cell.RowIndex)
                        {
                            CommitValueforRowCache(n);
                            this.CachedStorage.Clear();
                        }
                        oldRowColIndex.RowIndex = e.Cell.RowIndex;
                    }
                }

            }

            e.Handled = true;
        }


        RowColumnIndex oldRowColIndex;


        /// <summary>
        /// CatchedStorage used to Store the row values while UpdateMode=RowCache
        /// </summary>
        private Dictionary<string, object> CachedStorage
        {
            get;
            set;
        }

        /// <summary>
        /// Handles the CurrentCellMoving event of the GridTreeControlImpl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.GridCurrentCellMovingEventArgs"/> instance containing the event data.</param>
        void GridTreeControlImpl_CurrentCellMoving(object sender, GridCurrentCellMovingEventArgs args)
        {
            if (this.ParentTreeControl.UpdateMode == UpdateMode.RowCachedMode)
            {
                if (oldRowColIndex.RowIndex != args.CellRowColumnIndex.RowIndex)
                {
                    GridTreeNode n;
                    GridTreeRowType rowType = GetGridRowType(oldRowColIndex.RowIndex, out n);
                    CommitValueforRowCache(n);
                    this.CachedStorage.Clear();
                    if (rowType != GridTreeRowType.Header)
                        oldRowColIndex = args.CellRowColumnIndex;
                }
            }
        }


        /// <summary>
        /// Commits the valuefor row cache.
        /// </summary>
        /// <param name="n">The n.</param>
        private void CommitValueforRowCache(GridTreeNode n)
        {
            // GridTreeRowType rowType = GetGridRowType(oldRowColIndex.RowIndex, out n);
            foreach (var item in this.CachedStorage)
            {
                if (this.ItemProperties[item.Key] != null &&
                 (!item.Value.Equals(GetValueFromComponent(item.Key, n.Item)) &&
                 !((GetValueFromComponent(item.Key, n.Item) != null &&
                item.Value.Equals(GetValueFromComponent(item.Key, n.Item).ToString())))))
                {
                    SetValueToComponent(item.Key, n.Item, NullableHelper.ChangeType(item.Value, ItemProperties[item.Key].PropertyType));
                }
            }
            this.CachedStorage.Clear();
        }

        /// <summary>
        /// Handles the CurrentCellChanged event of the GridTreeControlImpl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.ComponentModel.SyncfusionRoutedEventArgs"/> instance containing the event data.</param>
        void GridTreeControlImpl_CurrentCellChanged(object sender, SyncfusionRoutedEventArgs args)
        {
            if (this.ParentTreeControl.UpdateMode == UpdateMode.PropertyChanged)
            {
#if SILVERLIGHT
                bool isValidationMsg,suspendMoveTo;
                if (!this.CurrentCell.ConfirmChanges(out isValidationMsg,out suspendMoveTo))
#else
                if (!this.CurrentCell.ConfirmChanges())
#endif
                {
                    args.Handled = true;
                }
                else
                    SaveValuesToCollection(this.CurrentCell.Renderer.ControlValue, this.CurrentCell.RowIndex, this.CurrentCell.ColumnIndex);
            }
        }


        /// <summary>
        /// Saves the values to collection.
        /// </summary>
        /// <param name="cellValue">The cell value.</param>
        /// <param name="ColumnIndex">Index of the column.</param>
        private void SaveValuesToCollection(object cellValue, int RowIndex, int ColumnIndex)
        {
            GridTreeNode n;
            GridTreeRowType rowType = GetGridRowType(RowIndex, out n);
            if (ColumnIndex >= nodeColumnIndex)
            {
                if (rowType == GridTreeRowType.Caption || rowType == GridTreeRowType.Node)
                {
                    GridTreeColumn tc = Columns[ColumnIndex - nodeColumnIndex];
                    //when cellvalue is not equal to component value then only it will set the component value.                   
                    if (tc.IsUnbound)
                        return;
                    var pd = ItemProperties.GetPropertyDescriptor(tc.MappingName);
                    if (pd == null)
                        throw new InvalidOperationException(string.Format("{0} - {1}", GridDataResourceWrapper.InvalidColumn, tc.MappingName));

                    if (cellValue == null)
                        SetValueToComponent(tc.MappingName, n.Item, NullableHelper.ChangeType(cellValue, pd.PropertyType));
                    else if ((!cellValue.Equals(GetValueFromComponent(tc.MappingName, n.Item)) &&
                        !((GetValueFromComponent(tc.MappingName, n.Item) != null &&
                        cellValue.Equals(GetValueFromComponent(tc.MappingName, n.Item).ToString())))))
                    {
                        SetValueToComponent(tc.MappingName, n.Item, NullableHelper.ChangeType(cellValue, pd.PropertyType));
                    }
                }
            }

        }

#if !SILVERLIGHT

        /// <summary>
        /// This method sort the GridTreeControl After Edit the values in the cell
        /// </summary>
        public void SortAfterEdit(int rowIdx, int colIdx)
        {
            if (rowIdx == -1)
                return;
            List<GridTreeNode> MultiNodes = null;
            int MouseClickNodeIndex = 0;
            if (this.EnableNodeSelection && this.SelectedNodes.Count > 0)
            {
                MultiNodes = new List<GridTreeNode>();
                MultiNodes.AddRange(this.SelectedNodes);
                MouseClickNodeIndex = this.Nodes.IndexOf(MultiNodes[MultiNodes.Count - 1]);
            }
            var CurrentNode = this.GetNodeAtRowIndex(rowIdx);
            if (RootNodes.Contains(CurrentNode))
            {
                if (RootNodes.Count > 1)
                {
                    int index = this.RootNodes.IndexOf(CurrentNode);
                    int loc = this.GetComparerIndex(CurrentNode, index);
                    RootNodes.Insert(loc, CurrentNode);
                    this.ReloadNodes();
                }
            }
            else
            {
                var parent = CurrentNode.ParentNode;
                if (parent.ChildNodes.Count > 1 && parent.Expanded)
                {
                    //int index = parent.ChildNodes.IndexOf(CurrentNode);
                    //int loc = this.GetChildComparerIndex(parent.ChildNodes, CurrentNode, index);
                    //parent.ChildNodes.Insert(loc, CurrentNode);
                    int rowIndex = GetVisibleRowIndexOf(parent.Item);
                    if (rowIndex > -1)
                    {
                        CollapseNode(parent);
                        ExpandNode(parent,CurrentNode);
                    }
                }
            }
            if (this.EnableNodeSelection)
            {
                if (MultiNodes != null && MultiNodes.Count > 0)
                {
                    SelectedNodes.RemoveRange(SelectedNodes.ToList());
                    foreach (var Node in MultiNodes)
                        if (Nodes.Contains(Node))
                            SelectedNodes.Add(Node);
                }
                var index = this.GetRowIndexFromNode(CurrentNode); //this.Nodes.IndexOf(CurrentNode) + this.InternalGrid.Model.HeaderRows;
                this.InternalGrid.Model.SelectedRanges.Clear();
                this.InternalGrid.Model.SelectedRanges.Add(GridRangeInfo.Row(index));
                if (this.CurrentCell.HasCurrentCell)
                {
                    if (this.CurrentCell.IsEditing)
                        this.CurrentCell.CancelEdit();
                    this.CurrentCell.Deactivate();
                }
            }
            else if (this.ParentTreeControl.EnableSelections)
            {
                if (IsMouseCaptured)
                {
                    SelectedNodes.Clear();
                    SelectedNodes.Add(this.Nodes[MouseClickNodeIndex]);
                }
                var index = this.GetRowIndexFromNode(CurrentNode); //this.Nodes.IndexOf(CurrentNode) + this.InternalGrid.Model.HeaderRows;
                this.InternalGrid.Model.SelectedRanges.Clear();
                this.InternalGrid.Model.SelectedRanges.Add(GridRangeInfo.Cell(index, this.CurrentCell.ColumnIndex));
                if (this.CurrentCell.HasCurrentCell)
                {
                    this.CurrentCell.Deactivate();
                }
            }
            else
            {
                var rowidx = this.GetRowIndexFromNode(CurrentNode);//this.Nodes.IndexOf(CurrentNode) + this.InternalGrid.Model.HeaderRows;
                if (this.CurrentCell.HasCurrentCell)
                {
                    if (!IsMouseCaptured)
                    {
                        this.CurrentCell.Deactivate();
                    }
                }
            }
        }

#endif
        internal void SetValueToComponent(string propertyName, object comp, object val)
        {         
#if !SILVERLIGHT
            if (comp is DataRowView)
                ((System.Data.DataRowView)comp)[propertyName] = val;
            else if (comp is DataRow)
                ((System.Data.DataRow)comp)[propertyName] = val;
            else
                ItemProperties.SetValue(comp, val, propertyName);
            //ItemProperties[propertyName].SetValue(comp, val);
#else
            ItemProperties[propertyName].SetValue(comp, val);
#endif
            if (comp is IEditableObject)
            {               
                ((IEditableObject)comp).EndEdit();
            }
        }

        //used to refresh headers when colwidths change
        internal void ColumnWidths_LineSizeChanged(object sender, RangeChangedEventArgs e)
        {
            if (ShowColumnHeaders)
            {
                this.InvalidateCell(GridRangeInfo.Row(0));
            }
        }

        internal void RowHeights_LineSizeChanged(object sender, RangeChangedEventArgs e)
        {
            if (e.From > 0)
            {
                Nodes[e.From - 1].NodeHeight = this.Model.RowHeights[e.From];
                this.InvalidateCells();
            }
        }

        void GridTreeControl_QueryAllowDragColumn(object sender, GridQueryDragColumnHeaderEventArgs e)
        {
            if (e.Column <= nodeColumnIndex)
            {
                e.AllowDrag = false;
            }
            else if (e.InsertBeforeColumn <= nodeColumnIndex && e.InsertBeforeColumn >= 0)
            {
                e.AllowDrag = false;
            }
        }
        #endregion

        #region Properties
        private Position unboundrowposition = Position.Bottom;
        public Position UnboundRowPosition
        {
            get { return unboundrowposition; }
            set { unboundrowposition = value; }
        }

        private int unboundrowcount = 0;
        public int UnboundRowsCount
        {
            get { return unboundrowcount; }
            set
            {
                unboundrowcount = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has unbound columns.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has unbound columns; otherwise, <c>false</c>.
        /// </value>
        public bool HasUnboundColumns
        {
            get
            {
                return this.Columns.OfType<GridTreeUnboundColumn>().Count() > 0;
            }
        }

#if !SILVERLIGHT
        public static readonly RoutedEvent RequestTreeItemsEvent = EventManager.RegisterRoutedEvent(
            "RequestTreeItems",
            RoutingStrategy.Direct,
            typeof(GridTreeRequestTreeItemsHandler),
            typeof(GridTreeControlImpl));

        public static readonly RoutedEvent RequestNodeImageEvent = EventManager.RegisterRoutedEvent(
            "RequestNodeImage",
            RoutingStrategy.Direct,
            typeof(GridTreeRequestNodeImageHandler),
            typeof(GridTreeControlImpl));
#else
        private event GridTreeRequestTreeItemsHandler RequestTreeItemsEvent;


        private event GridTreeRequestNodeImageHandler RequestNodeImageEvent;
#endif

        //needed for dependency properties
        static GridTreeControlImpl()
        {
#if !SILVERLIGHT
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridTreeControlImpl), new FrameworkPropertyMetadata(typeof(GridTreeControlImpl)));
#else
#endif
            PropertyMetadata mDataFalse = new PropertyMetadata(false, new PropertyChangedCallback(OnMouseController)
#if !SILVERLIGHT
, null
#endif
);


            SupportRowSizingProperty = DependencyProperty.Register("SupportRowSizing", typeof(bool), typeof(GridTreeControlImpl), mDataFalse);

            PropertyMetadata mDataTrue = new PropertyMetadata(true, new PropertyChangedCallback(OnNeedRedrawGrid)
#if !SILVERLIGHT
, null
#endif
);

            FreezeExpandColumnProperty = DependencyProperty.Register("FreezeExpandColumn", typeof(bool), typeof(GridTreeControlImpl), mDataTrue);

            mDataTrue = new PropertyMetadata(true, new PropertyChangedCallback(OnReadOnlyChange)
#if !SILVERLIGHT
, null
#endif
);

            NotifyPropertyChangesProperty = DependencyProperty.Register("NotifyPropertyChanges", typeof(bool), typeof(GridTreeControlImpl), mDataTrue);

            mDataTrue = new PropertyMetadata(false, new PropertyChangedCallback(OnReadOnlyChange)
#if !SILVERLIGHT
, null
#endif
);

            ReadOnlyProperty = DependencyProperty.Register("ReadOnly", typeof(bool), typeof(GridTreeControlImpl), mDataTrue);

            mDataTrue = new PropertyMetadata(true, new PropertyChangedCallback(OnNeedRedrawGrid)
#if !SILVERLIGHT
, null
#endif
);

            ShowExpandColumnBordersProperty = DependencyProperty.Register("ShowExpandColumnBorders", typeof(bool), typeof(GridTreeControlImpl), mDataTrue);

            mDataFalse = new PropertyMetadata(false, new PropertyChangedCallback(OnNeedRedrawGrid)
#if !SILVERLIGHT
, null
#endif
);

            ShowRowHeadersProperty = DependencyProperty.Register("ShowRowHeaders", typeof(bool), typeof(GridTreeControlImpl), mDataFalse);

            mDataTrue = new PropertyMetadata(true, new PropertyChangedCallback(OnNeedRedrawGrid)
#if !SILVERLIGHT
, null
#endif
);
            ShowColumnHeadersProperty = DependencyProperty.Register("ShowColumnHeaders", typeof(bool), typeof(GridTreeControlImpl), mDataTrue);

            mDataTrue = new PropertyMetadata(new ObservableCollection<GridTreeColumn>(), new PropertyChangedCallback(OnColumnsChanged)
#if !SILVERLIGHT
, null
#endif
);
            ColumnsProperty = DependencyProperty.Register("Columns", typeof(ObservableCollection<GridTreeColumn>), typeof(GridTreeControlImpl), mDataTrue);

            mDataFalse = new PropertyMetadata(true, new PropertyChangedCallback(OnMouseController)
#if !SILVERLIGHT
, null
#endif
);
            EnableNodeSelectionProperty = DependencyProperty.Register("EnableNodeSelection", typeof(bool), typeof(GridTreeControlImpl), mDataFalse);

            mDataFalse = new PropertyMetadata(true, new PropertyChangedCallback(OnEnableSelectionsChanged)
#if !SILVERLIGHT
, null
#endif
);
            EnableSelectionsProperty = DependencyProperty.Register("EnableSelections", typeof(bool), typeof(GridTreeControlImpl), mDataFalse);

            mDataFalse = new PropertyMetadata(true, new PropertyChangedCallback(OnEnableHotRowMarkerChanged)
#if !SILVERLIGHT
, null
#endif
);
            EnableHotRowMarkerProperty = DependencyProperty.Register("EnableHotRowMarker", typeof(bool), typeof(GridTreeControlImpl), mDataFalse);

            mDataFalse = new PropertyMetadata(20d, new PropertyChangedCallback(OnRowHeaderWidthChanged)
#if !SILVERLIGHT
, null
#endif
);
            RowHeaderWidthProperty = DependencyProperty.Register("RowHeaderWidth", typeof(double), typeof(GridTreeControlImpl), mDataFalse);

            mDataFalse = new PropertyMetadata(true, new PropertyChangedCallback(OnSortingEnabledChanged)
#if !SILVERLIGHT
, null
#endif
);
            SortingEnabledProperty = DependencyProperty.Register("SortingEnabled", typeof(bool), typeof(GridTreeControlImpl), mDataFalse);

            mDataFalse = new PropertyMetadata(GridPercentColumnSizingBehavior.None, new PropertyChangedCallback(OnPercentSizingBehavior)
#if !SILVERLIGHT
, null
#endif
);
            PercentSizingBehaviorProperty = DependencyProperty.Register("PercentSizingBehavior", typeof(GridPercentColumnSizingBehavior), typeof(GridTreeControlImpl), mDataFalse);

            mDataFalse = new PropertyMetadata(null);
            CustomVisualStyleProperty = DependencyProperty.Register("CustomVisualStyle", typeof(IGridTreeVisualStyle), typeof(GridTreeControlImpl), mDataFalse);

            mDataFalse = new PropertyMetadata(VisualStyle.Default, new PropertyChangedCallback(OnVisualStyleChanged)
#if !SILVERLIGHT
, null
#endif
);
            VisualStyleProperty = DependencyProperty.Register("VisualStyle", typeof(VisualStyle), typeof(GridTreeControlImpl), mDataFalse);

            mDataFalse = new PropertyMetadata(true, new PropertyChangedCallback(OnAllowAutoSizingNodeColumnChanged)
#if !SILVERLIGHT
, null
#endif
);
            AllowAutoSizingNodeColumnProperty = DependencyProperty.Register("AllowAutoSizingNodeColumn", typeof(bool), typeof(GridTreeControlImpl), mDataFalse);

            mDataFalse = new PropertyMetadata(false, new PropertyChangedCallback(OnNeedRedrawGrid)
#if !SILVERLIGHT
, null
#endif
);
            SupportNodeImagesProperty = DependencyProperty.Register("SupportNodeImages", typeof(bool), typeof(GridTreeControlImpl), mDataFalse);

        }


        static void OnAllowAutoSizingNodeColumnChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OnNeedRedrawGrid(o, e);
        }

        static void OnPercentSizingBehavior(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControlImpl tree = o as GridTreeControlImpl;
            if (tree != null)
            {
                tree.ColumnWidthSizer.SizingBehavior = (GridPercentColumnSizingBehavior)e.NewValue;
                //tree.ColumnWidthSizer.ApplySizes();
            }

        }

        static void OnSortingEnabledChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            //nop
        }

        static void OnRowHeaderWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OnNeedRedrawGrid(o, e);
        }

        static void OnEnableHotRowMarkerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OnNeedRedrawGrid(o, e);
        }

        //force the grid to redraw after a property change....
        static void OnNeedRedrawGrid(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControlImpl tree = o as GridTreeControlImpl;
            if (tree != null)
            {
                tree.UnloadArrangedCells();
                tree.InvalidateVisual(true);
            }
        }

        static void OnColumnsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControlImpl tree = o as GridTreeControlImpl;
            if (tree != null)
            {
                //   tree.SetValue(e.Property, e.NewValue);
            }
        }

        static void OnReadOnlyChange(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControlImpl tree = o as GridTreeControlImpl;
            if (tree != null)
            {
                //   tree.SetValue(e.Property, e.NewValue);
            }
        }

        static void OnEnableSelectionsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControlImpl tree = o as GridTreeControlImpl;
            if (tree != null)
            {
                //   tree.SetValue(e.Property, e.NewValue);
            }
        }


        //force the grid to redraw after a property change....
        static void OnMouseController(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridTreeControlImpl tree = o as GridTreeControlImpl;
            if (tree != null)
            {
                InitializeMouseController(tree);
            }
        }

        /// <exclude/>
        public static readonly DependencyProperty NotifyPropertyChangesProperty;
        /// <summary>
        /// Gets of sets whether the column with the expand/contract glymp is scrollable.
        /// </summary>
        public bool NotifyPropertyChanges
        {
            get { return (bool)GetValue(NotifyPropertyChangesProperty); }
            set
            {
                SetValue(NotifyPropertyChangesProperty, value);
            }
        }

        /// <exclude/>
        public static readonly DependencyProperty FreezeExpandColumnProperty;
        /// <summary>
        /// Gets of sets whether the column with the expand/contract glymp is scrollable.
        /// </summary>
        public bool FreezeExpandColumn
        {
            get { return (bool)GetValue(FreezeExpandColumnProperty); }
            set
            {
                SetValue(FreezeExpandColumnProperty, value);
                this.FrozenColumns = value ? 2 : 1;
            }
        }

        /// <exclude/>
        public static readonly DependencyProperty ReadOnlyProperty;
        /// <summary>
        /// Gets or sets whether the TreeGrid is ReadOnly.
        /// </summary>
        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set
            {
                SetValue(ReadOnlyProperty, value);
                this.Model.TableStyle.ReadOnly = value;
            }
        }
        /// <override/>
        protected override void OnCurrentCellRejectedChanges(SyncfusionRoutedEventArgs e)
        {
            base.OnCurrentCellRejectedChanges(e);
            GridTreeNode n;
            RowColumnIndex cell = this.CurrentCell.CellRowColumnIndex;
            GridTreeRowType rowType = GetGridRowType(cell.RowIndex, out n);
            if (cell.ColumnIndex >= nodeColumnIndex && n.Item is IEditableObject)
            {
                if (rowType == GridTreeRowType.Caption || rowType == GridTreeRowType.Node)
                {
                    ((IEditableObject)n.Item).CancelEdit();
                }
            }
        }
        /// <override/>
        protected override void OnCurrentCellStartEditing(SyncfusionCancelRoutedEventArgs e)
        {
            base.OnCurrentCellStartEditing(e);
            if (!e.Cancel)
            {
                GridTreeNode n;
                RowColumnIndex cell = this.CurrentCell.CellRowColumnIndex;
                GridTreeRowType rowType = GetGridRowType(cell.RowIndex, out n);
                if (cell.ColumnIndex >= nodeColumnIndex && n != null && n.Item is IEditableObject)
                {
                    if (rowType == GridTreeRowType.Caption || rowType == GridTreeRowType.Node)
                    {
                        ((IEditableObject)n.Item).BeginEdit();
                    }
                }

            }
        }

        /// <exclude/>
        public static readonly DependencyProperty SupportRowSizingProperty;
        /// <summary>
        /// Gets or sets whether your user can size row heights with the mouse.
        /// </summary>
        public bool SupportRowSizing
        {
            get
            {
                return (bool)GetValue(SupportRowSizingProperty);
            }
            set
            {
                SetValue(SupportRowSizingProperty, value);
                InitializeMouseController(this);
            }
        }
        #region TrackSelectionOnCollectionChange

        /// <summary>
        /// Gets or Sets whether to track the selection when the binded collection changed
        /// </summary>
        public bool TrackSelectionOnCollectionChange
        {
            get
            {
                return (bool)GetValue(TrackSelectionOnCollectionChangeProperty);
            }
            set
            {
                SetValue(TrackSelectionOnCollectionChangeProperty, value);
            }
        }

        public static readonly DependencyProperty TrackSelectionOnCollectionChangeProperty =
            DependencyProperty.Register("TrackSelectionOnCollectionChange", typeof(bool), typeof(GridTreeControlImpl), new PropertyMetadata(false));
        #endregion TrackSelectionOnCollectionChange


        /// <summary>
        /// Gets or sets a value indicating whether [expand new node on creation].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [expand new node on creation]; otherwise, <c>false</c>.
        /// </value>
        public bool ExpandNewNodeOnCreation
        {
            get { return (bool)GetValue(ExpandNewNodeOnCreationProperty); }
            set { SetValue(ExpandNewNodeOnCreationProperty, value); }
        }

        /// <summary>
        /// Dependency Registartion for Expand New Node on Creation
        /// </summary>
        public static readonly DependencyProperty ExpandNewNodeOnCreationProperty =
            DependencyProperty.Register("ExpandNewNodeOnCreation", typeof(bool), typeof(GridTreeControlImpl), new PropertyMetadata(false));

        /// <exclude/>
        public static readonly DependencyProperty SupportNodeImagesProperty;
        /// <summary>
        /// Gets or sets whether an event is raised to request an image to be displayed for a node.
        /// </summary>       

        public bool SupportNodeImages
        {
            get
            {
                return (bool)GetValue(SupportNodeImagesProperty);
            }
            set
            {
                SetValue(SupportNodeImagesProperty, value);
            }
        }

        /// <exclude/>
        /// <summary>
        /// Gets the GridControlBase associated with the GridTreeControl.
        /// </summary>
        public GridControlBase InternalGrid
        {
            get { return this; }
        }
#if !SILVERLIGHT
        public override void Dispose(bool disposing)
        {
            ClearChildBindingsFromIListChanged();
            ClearChildBindingsFromICollectionChanged();

            if (this.ParentTreeControl.Columns != null)
                this.ParentTreeControl.Columns.CollectionChanged -= new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
            //Unwire the SelectedNodes CollectionChanged event.
            //if (SelectedNodes != null)
            //{
            //    this.SelectedNodes.CollectionChanged -= new NotifyCollectionChangedEventHandler(SelectedNodes_CollectionChanged);
            //}
            base.Dispose(disposing);
        }
#endif


        /// <exclude/>
        public static readonly DependencyProperty EnableSelectionsProperty;
        /// <summary>
        /// Gets or sets whether your user can select whole row when a cell is clicked with the mouse.
        /// </summary>
        /// <remarks>
        /// Setting this property to true also sets GridTreeControl.Model.Options.ListBoxSelectionMode to
        /// MultiExtended. If you want the selected behavior to be something other than MultiExtended, you
        /// will need to explicitly set it after setting this property.
        /// </remarks>
        public bool EnableSelections
        {
            get
            {
                return (bool)GetValue(EnableSelectionsProperty);
            }
            set
            {
                SetValue(EnableSelectionsProperty, value);
            }

        }
        // private Brush nodeHighlightBrush; Variable is never used

        /// <exclude/>
        public static readonly DependencyProperty EnableNodeSelectionProperty;
        /// <summary>
        /// Gets or sets whether your user can select whole row when a cell is clicked with the mouse.
        /// </summary>
        /// <remarks>
        /// Setting this property to true also sets GridTreeControl.Model.Options.ListBoxSelectionMode to
        /// MultiExtended. If you want the selected behavior to be something other than MultiExtended, you
        /// will need to explicitly set it after setting this property.
        /// </remarks>
        public bool EnableNodeSelection
        {
            get
            {
                bool b = (bool)GetValue(EnableNodeSelectionProperty);
                if (b && this.Model.Options.ListBoxSelectionMode == GridSelectionMode.None)
                {
                    this.Model.Options.AllowSelection = GridSelectionFlags.None;
                    this.Model.Options.ListBoxSelectionMode = GridSelectionMode.MultiExtended;
                    // nodeHighlightBrush = this.GetGridTreeNodeHighlightBrush(this.GetVisualStyle(VisualStyle)); //HighlightBrush;
                }
                return b;
            }
            set
            {
                SetValue(EnableNodeSelectionProperty, value);
                if (value)
                {
                    if (this.Model.Options.ListBoxSelectionMode == GridSelectionMode.None)
                        this.Model.Options.ListBoxSelectionMode = GridSelectionMode.MultiExtended;
                    this.Model.Options.AllowSelection = GridSelectionFlags.None;
                    //nodeHighlightBrush = this.GetGridTreeNodeHighlightBrush(this.GetVisualStyle(VisualStyle));
                    InitializeMouseController(this);
                }
                else
                {
                    this.Model.Options.AllowSelection = GridSelectionFlags.Any;
                    this.Model.Options.ListBoxSelectionMode = GridSelectionMode.None;
                    // nodeHighlightBrush = this.GetGridTreeNodeHighlightBrush(this.GetVisualStyle(VisualStyle));
                    this.Model.Options.ExcelLikeCurrentCell = true;
                    //this.Model.Options.ShowCurrentCell = true;
                }
            }
        }

        private Brush MakeSolidBrush(Brush highlightBrush)
        {
            if (!EnableNodeSelection && Model.Options.AllowSelection != GridSelectionFlags.None
                && highlightBrush is SolidColorBrush)
            {
                SolidColorBrush b = highlightBrush as SolidColorBrush;
                return new SolidColorBrush(Color.FromArgb(255, b.Color.R, b.Color.G, b.Color.B));
            }
            return highlightBrush;
        }

        ///// <exclude/>
        protected override void RenderSelectedCells()
        {
            //never render selected cells...

            //but do possibly render excel frame
            if (this.Model.Options.ExcelLikeSelectionFrame)
                base.RenderActiveRangeBorder();


        }

        /// <exclude/>
        public static readonly DependencyProperty EnableHotRowMarkerProperty;
        /// <summary>
        /// Gets or sets whether the row under the current mouse position is redrawn using
        /// the HotRowMarker as a background brush.
        /// </summary>
        public bool EnableHotRowMarker
        {
            get
            {

                return (bool)GetValue(EnableHotRowMarkerProperty);
            }
            set
            {
                SetValue(EnableHotRowMarkerProperty, value);
            }
        }

        internal static void InitializeMouseController(GridTreeControlImpl g)
        {
            //swapout mousecontroller
            IMouseController mc = null;
#if SILVERLIGHT
            mc = g.MouseControllerDispatcher.Find("GridResizeRowsMouseController");
#else
            mc = g.MouseControllerDispatcher.Find("ResizeRowsMouseController");
#endif
            if (mc != null)
            {
                g.MouseControllerDispatcher.Remove(mc);
            }

            if (g.SupportRowSizing)
            {
                mc = g.MouseControllerDispatcher.Find("ResizeInsideGridRowsMouseController");
                if (mc == null)
                {
                    g.MouseControllerDispatcher.Add(new GridTreeResizeRowsMouseController(g));
                }
            }
            else
            {
                mc = g.MouseControllerDispatcher.Find("ResizeInsideGridRowsMouseController");
                if (mc != null)
                {
                    g.MouseControllerDispatcher.Remove(mc);
                }
            }
            mc = g.MouseControllerDispatcher.Find("SelectCellsMouseController");
            if (g.EnableNodeSelection)
            {
                if (mc != null)
                {
                    g.MouseControllerDispatcher.Remove(mc);
                    ((GridSelectCellsMouseController)mc).Dispose();
                }
                mc = g.MouseControllerDispatcher.Find("SelectNodesMouseController");
                if (mc == null)
                {
                    g.MouseControllerDispatcher.Add(new GridTreeSelectNodesMouseController<GridTreeNode>(g));
                }
            }
            else
            {
                mc = g.MouseControllerDispatcher.Find("SelectNodesMouseController");
                if (mc != null)
                {
                    g.MouseControllerDispatcher.Remove(mc);
                    ((GridTreeSelectNodesMouseController<GridTreeNode>)mc).Dispose();
                }
                mc = g.MouseControllerDispatcher.Find("SelectCellsMouseController");
                if (mc == null)
                {
                    g.MouseControllerDispatcher.Add(new GridSelectCellsMouseController(g));
                }
            }
        }

        /// <exclude/>
        public static readonly DependencyProperty ShowExpandColumnBordersProperty;

        /// <summary>
        /// Gets or sets whether the grid lines are seen in the expand column.
        /// </summary>
        public bool ShowExpandColumnBorders
        {
            get { return (bool)GetValue(ShowExpandColumnBordersProperty); }
            set { SetValue(ShowExpandColumnBordersProperty, value); }
        }

        /// <summary>
        /// Gets whether root nodes have been set for this tree grid.
        /// </summary>
        public bool HasDataSource
        {
            get { return rootNodes != null && rootNodes.Count > 0; }
        }

        private List<GridTreeNode> rootNodes;

        /// <summary>
        /// Gets or sets the root node(s) for this tree grid.
        /// </summary>
        public List<GridTreeNode> RootNodes
        {
            get
            {
                if (rootNodes == null)
                    rootNodes = new List<GridTreeNode>();
                return rootNodes;
            }
            set
            {
                rootNodes = value;
                foreach (GridTreeNode rootNode in rootNodes)
                    rootNode.ParentItem = null;
            }
        }

        /// <exclude/>
        public static readonly DependencyProperty ShowRowHeadersProperty;
        /// <summary>
        /// Gets or sets whether a header column is visible
        /// </summary>
        public bool ShowRowHeader
        {
            get { return (bool)GetValue(ShowRowHeadersProperty); }
            set
            {
                SetValue(ShowRowHeadersProperty, value);
                this.Model.ColumnWidths.SetHidden(0, 0, !value);
            }
        }

        /// <exclude/>
        public static readonly DependencyProperty RowHeaderWidthProperty;

        /// <summary>
        /// Gets or sets the width of the row header column.
        /// </summary>
        public double RowHeaderWidth
        {
            get { return (double)GetValue(RowHeaderWidthProperty); }
            set
            {
                SetValue(RowHeaderWidthProperty, value);
                this.ColumnWidths[0] = value;
            }
        }

        /// <exclude/>
        public static readonly DependencyProperty ShowColumnHeadersProperty;

        /// <summary>
        /// Gets or sets whether a header row is visible
        /// </summary>
        public bool ShowColumnHeaders
        {
            get { return (bool)GetValue(ShowColumnHeadersProperty); }
            set
            {

                SetValue(ShowColumnHeadersProperty, value);
                this.Model.RowHeights.SetHidden(0, 0, !value);
            }
        }

        List<GridTreeNode> nodes;

        /// <summary>
        /// Gets the list of GridNodes associated with the displayed tree.
        /// </summary>
        /// <remarks>
        /// There is a one-one mapping between GridTreeNodes in the <see cref="Nodes"/> list and the rows in the GridControl possibly excluding the column header.
        /// Given a GridTreeNode, you can access the underlying data object for the row using node.Item property.
        /// </remarks>
        public List<GridTreeNode> Nodes
        {
            get
            {
                if (nodes == null)
                    nodes = new List<GridTreeNode>();
                return nodes;
            }
        }

        /// <exclude/>
        public static readonly DependencyProperty ColumnsProperty;

        /// <summary>
        /// A collection of the TreeColumns that control the number and order of the columns
        /// that appear in the Tree.
        /// </summary>
        public ObservableCollection<GridTreeColumn> Columns
        {
            get
            {
                return (ObservableCollection<GridTreeColumn>)GetValue(ColumnsProperty);
            }
            set { SetValue(ColumnsProperty, value); }
        }

        private Type itemPropertyType;

        /// <summary>
        /// The underlying type of the object that holds the data for a tree node.
        /// </summary>
        public Type ItemPropertyType
        {
            get { return itemPropertyType; }
            set
            {
                if (itemPropertyType != value)
                {
                    itemPropertyType = value;
                }
            }
        }

#if SILVERLIGHT

        internal PropertyInfoCollection pdc;

        /// <summary>
        /// Gets a PropertyDescriptorCollection describing the type of the underlying data item.
        /// </summary>
        public PropertyInfoCollection ItemProperties
        {
            get
            {
                if (itemPropertyType != null && pdc == null)
                {
                    pdc = new PropertyInfoCollection(itemPropertyType);
                }
                return pdc;
            }
        }

#else
        internal PropertyDescriptorCollection pdc;

        /// <summary>
        /// Gets a PropertyDescriptorCollection describing the type of the underlying data item.
        /// </summary>
        public PropertyDescriptorCollection ItemProperties
        {
            get
            {
                if (itemPropertyType != null && pdc == null)
                {
                    pdc = TypeDescriptor.GetProperties(itemPropertyType);
                }
                else if (itemPropertyType == null && pdc == null && Nodes.Count > 0 && Nodes[0].Item != null)
                {

                    itemPropertyType = Nodes[0].Item.GetType();
                    if (itemPropertyType == typeof(DataRowView))
                    {
                        pdc = ((ITypedList)((DataRowView)Nodes[0].Item).DataView).GetItemProperties(null);
                    }
                    else if (itemPropertyType == typeof(DataRow))
                    {
                        pdc = ((ITypedList)((DataRow)Nodes[0].Item).Table.DefaultView).GetItemProperties(null);
                    }
                    else
                    {
                        pdc = TypeDescriptor.GetProperties(itemPropertyType);
                    }
                }
                return pdc;
            }
        }
#endif
        private bool supportsVisualStyles = true;
        /// <summary>
        /// Gets or sets whether the GridTreeControlImpl should adjust its appearance according to the <see cref="SkinStorage.VisualStyle"/> setting.
        /// </summary>

        public bool SupportsVisualStyles
        {
            get
            {
                return supportsVisualStyles;
            }
            set
            {
                supportsVisualStyles = value;
                if (value == false)
                {
                    ApplyGridVisualStyle(GetVisualStyle(VisualStyle.Default));
                }
                this.InvalidateCells();
            }
        }

        #region styles

        private GridStyleInfo rowHeaderStyle;
        /// <summary>
        /// Gets or sets a GridStyleInfo object that is used to defined the row headers.
        /// </summary>
        public GridStyleInfo RowHeaderStyle
        {
            get
            {
                if (rowHeaderStyle == null)
                {
                    rowHeaderStyle = new GridStyleInfo();
                    rowHeaderStyle.ModifyStyle(this.Model.BaseStylesMap["Header"].StyleInfo, StyleModifyType.Override);
                }
                return rowHeaderStyle;
            }
            set { rowHeaderStyle = value; }
        }

        List<GridStyleInfo> levelStyles;

        /// <summary>
        /// Gets or sets a collection of GridStyleInfo objects that will be applied to specific levels.
        /// </summary>
        public List<GridStyleInfo> LevelStyles
        {
            get
            {
                if (levelStyles == null)
                    levelStyles = new List<GridStyleInfo>();

                return levelStyles;
            }
            set { levelStyles = value; }
        }


        GridStyleInfo columnHeaderStyle;

        /// <summary>
        /// Gets or sets a GridStyleInfo object that defines the style information for the column headers.
        /// </summary>
        public GridStyleInfo ColumnHeaderStyle
        {
            get
            {
                if (columnHeaderStyle == null)
                {
                    columnHeaderStyle = new GridStyleInfo();
                    columnHeaderStyle.ModifyStyle(this.Model.BaseStylesMap["Header"].StyleInfo, StyleModifyType.Override);
                    columnHeaderStyle.Font = this.GetGridTreeHeaderFont(GetVisualStyle(this.VisualStyle));
                    columnHeaderStyle.TextMargins.Left = 3;
                }
                return columnHeaderStyle;
            }
            set
            {
                columnHeaderStyle = value;
                if (value == GridStyleInfo.Empty && this.supportsVisualStyles)
                {
                    ApplyGridVisualStyle(GetVisualStyle(this.VisualStyle));
                }
            }
        }


        #endregion

        private InvalidateAction invalidateActionOnCellChange = InvalidateAction.Cell;

        /// <summary>
        /// Gets or sets what part of the grid is invalidated when a cell value changes.
        /// </summary>
        /// <remarks>This property only has an effect if <see cref="GridTreeControl.NotifyPropertyChanges"/> is true. The default
        /// value is InvalidateAction.Cell.</remarks>
        public InvalidateAction InvalidateActionOnCellChange
        {
            get { return invalidateActionOnCellChange; }
            set { invalidateActionOnCellChange = value; }
        }


        #endregion

        #region  RequestNodeImage event support

        /// <summary>
        /// This event is used to request a node image for a particular node item.
        /// </summary>
        /// <remarks>
        /// To see images next to the expand glyph, subscribe to this event and provide the image
        /// needed for the node item passed in the event argument.
        /// </remarks>
        [Obsolete("Not sure if needed")]
        public event GridTreeRequestNodeImageHandler RequestNodeImage
        {
            add
            {
#if !SILVERLIGHT
                AddHandler(GridTreeControlImpl.RequestNodeImageEvent, value, false);
#else
                this.RequestNodeImageEvent += value;

#endif
            }
            remove
            {
#if !SILVERLIGHT
                RemoveHandler(GridTreeControlImpl.RequestNodeImageEvent, value);
#else
                this.RequestNodeImageEvent -= value;

#endif
            }
        }

        /// <summary>
        /// Raises the RequestTreeItems event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public virtual void OnRequestNodeImage(GridTreeRequestNodeImageEventArgs e)
        {
            if (!HasDataSource && e.Item == null)
                return; //do not raise the event before the grid knows it datasource

            if (ParentTreeControl == null)
            {
                GridTreeRequestNodeImageEventArgs e1 = new GridTreeRequestNodeImageEventArgs(e.Item,
#if !SILVERLIGHT
 GridTreeControlImpl.RequestNodeImageEvent,
#endif
 this);
#if !SILVERLIGHT
                base.RaiseEvent(e1);
#else
                this.RaiseRequestNodeImage(e, this);
#endif
                e.NodeImage = e1.NodeImage;
            }
            else
            {
                ParentTreeControl.RaiseRequestNodeImage(e);
            }

        }

#if SILVERLIGHT
        private void RaiseRequestNodeImage(GridTreeRequestNodeImageEventArgs e, GridTreeControlImpl gridTreeControlImpl)
        {
            if (this.RequestNodeImageEvent != null)
            {
                this.RequestNodeImageEvent(this, e);
            }
        }
#endif
        #endregion

        #region  RequestChildList event support

        /// <summary>
        /// This event is used to request an IEnumerable object that holds the child item objects for a particular parent item.
        /// </summary>
        /// <remarks>
        /// In order to see data displayed in the TreeGrid, you must handle this event. The events args pass in a parent object, 
        /// and your event handler needs to provide an IEnumerable object that contains teh childs objects for this pareent object.
        /// If the parent object is null, you should provide the collection of root objects for the tree.
        /// </remarks>
        public event GridTreeRequestTreeItemsHandler RequestTreeItems
        {
            add
            {
#if !SILVERLIGHT
                AddHandler(GridTreeControlImpl.RequestTreeItemsEvent, value, false);
#else
                this.RequestTreeItemsEvent += value;
#endif
            }
            remove
            {
#if !SILVERLIGHT
                RemoveHandler(GridTreeControlImpl.RequestTreeItemsEvent, value);
#else
                this.RequestTreeItemsEvent -= value;
#endif
            }
        }

        private void GetChildNodes(GridTreeNode parent)
        {
            GridTreeRequestChildListEventArgs e = new GridTreeRequestChildListEventArgs(parent, parent.Item);
            OnRequestChildList(e);
        }

        /// <summary>
        /// Gets the GridTreeControl associated with this GridTreeControlImpl.
        /// </summary>
        public GridTreeControl ParentTreeControl
        {
            get
            {
#if !SILVERLIGHT
                DependencyObject o = this.Parent;
                while (o != null && !(o is GridTreeControl))
                {
                    o = o.Parent();
                }
                return o as GridTreeControl;
#else
                DependencyObject o = this.Parent;
                while (o != null && !(o is GridTreeControl))
                {
                    o = (DependencyObject)(((FrameworkElement)o).Parent);
                }

                return this.FindParentElementOfType<GridTreeControl>() as GridTreeControl;
#endif
            }
        }

        /// <summary>
        /// Event raised whenever a new GridTreeNode is needed so derived GridTreeNodes can be used if desired.
        /// </summary>
        public GridTreeCreatingNodeHandler CreatingTreeNode;

        /// <summary>
        /// Raises the CreatingTreeNode event.
        /// </summary>
        /// <param name="level">The level of the new node.</param>
        /// <param name="item">The underlying data item for the new node.</param>
        /// <param name="expanded">Whether the new node is initially expanded.</param>
        /// <param name="parentNode">The parent node of this new node.</param>
        /// <returns></returns>
        protected virtual GridTreeNode OnCreatingTreeNode(int level, object item, bool expanded, GridTreeNode parentNode)
        {
            if (ParentTreeControl != null)
            {
                GridTreeCreatingNodeEventArgs e = new GridTreeCreatingNodeEventArgs(level, item, expanded, parentNode);
                ParentTreeControl.RaiseCreatingTreeNode(e);
                if (e.Node != null)
                {
                    return e.Node;
                }
            }

            if (CreatingTreeNode != null)
            {
                GridTreeCreatingNodeEventArgs e = new GridTreeCreatingNodeEventArgs(level, item, expanded, parentNode);
                CreatingTreeNode(this, e);
                if (e.Node != null)
                {
                    return e.Node;
                }
            }
            return new GridTreeNode(level, item, expanded, parentNode);
        }

        private ArrayList GetChildListFromParentValue(object x)
        {
            ArrayList returnList = new ArrayList();
            // int i = 0; Unused variables
            foreach (object o in (IEnumerable)this.parentTreeControl.ItemsSource)
            {
                object childValue = this.ItemProperties[this.parentTreeControl.ChildPropertyName].GetValue(o);
                if (x.GetType() != childValue.GetType())
                {
#if SILVERLIGHT
                    x = Convert.ChangeType(x, childValue.GetType(), null);
#else
                    x = Convert.ChangeType(x, childValue.GetType());
#endif
                }


                if ((childValue != null && childValue.Equals(x)) || (x == null && childValue == null))
                    returnList.Add(o);
            }
            return returnList;
        }
        /// <summary>
        /// Raises the RequestTreeItems event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        internal virtual void OnRequestChildList(GridTreeRequestChildListEventArgs e)
        {
            if (!HasDataSource && e.ParentNode != null)
                return; //do not raise the event before the grid knows it datasource

            //only raise event if needed unless e.ResetChildAndRepopulate is true
            bool b = e.ParentNode != null && e.ParentNode.ChildNodes.Count > 0 && !e.ResetChildAndRepopulate;

            if (!b)
            {
                //(e.ParentItem == null && this.parentTreeControl != null && this.parentTreeControl.ItemsSource != null - Adding this or condition to populate the FlatData in GridTreeControl
                if (this.parentTreeControl != null && this.parentTreeControl.ItemsSource != null && (ParentTreeControl.ChildPropertyName.Length > 0 ))
                   // || (e.ParentItem == null && this.parentTreeControl != null && this.parentTreeControl.ItemsSource != null))
                {
                    bool selfRelation = this.parentTreeControl.ParentPropertyName != null && this.parentTreeControl.ParentPropertyName.Length > 0;

                    if (e.ParentItem == null)
                    {
                        if (selfRelation)
                        {
                            if (ItemPropertyType == null)
                            {
                                foreach (object o in (IEnumerable)this.parentTreeControl.ItemsSource)
                                {
                                    this.ItemPropertyType = o.GetType();
                                    break;
                                }
                            }
                            e.ChildList = GetChildListFromParentValue(this.parentTreeControl.SelfRelationRootValue);
                        }
                        else
                        {
                            e.ChildList = parentTreeControl.ItemsSource as IEnumerable;
                        }
                    }
                    else
                    {
                        if (selfRelation)
                        {

                            object parentValue = this.ItemProperties[this.parentTreeControl.ParentPropertyName].GetValue(e.ParentItem);
                            e.ChildList = GetChildListFromParentValue(parentValue);
                        }
                        else if (!isNodeDeleted && this.isChildPropertyexist)
                        {
#if SILVERLIGHT
                            //This code added only for silverlight. Same working fine with WPF
                            if (ItemPropertyType == null)
                            {
                                foreach (object o in (IEnumerable)this.parentTreeControl.ItemsSource)
                                {
                                    this.ItemPropertyType = o.GetType();
                                    break;
                                }
                            }
#endif
                            object childValue = this.ItemProperties[this.parentTreeControl.ChildPropertyName].GetValue(e.ParentItem);
                            e.ChildList = childValue as IEnumerable;
                        }
                    }
                }
                else if (ParentTreeControl == null)
                {
                    GridTreeRequestTreeItemsEventArgs e1 = new GridTreeRequestTreeItemsEventArgs(e.ParentItem,
#if !SILVERLIGHT
 GridTreeControlImpl.RequestTreeItemsEvent,
#endif
 this);
#if !SILVERLIGHT
                    base.RaiseEvent(e1);
#else
                    OnRaiseGridTreeItemEventArgs(e1);
#endif
                    e.ChildList = e1.ChildList;
                }
                else
                {
                    ParentTreeControl.RaiseRequestTreeItems(e);
                }
                ProcessListRequest(e);
            }
            if (e.ParentNode != null && SortProperty != null && SortProperty.Length > 0)
            {
                SortList(e.ParentNode.ChildNodes);
            }
        }
#if SILVERLIGHT
        private void OnRaiseGridTreeItemEventArgs(GridTreeRequestTreeItemsEventArgs e1)
        {
            if (this.RequestTreeItemsEvent != null)
            {
                this.RequestTreeItemsEvent(this, e1);
            }
        }
#endif

        #region change events

        void WireCollectionChangeEvents(INotifyCollectionChanged collection, GridTreeNode node)
        {
            if (collection == null)
                return;

            if (childListToParentNode == null)
                childListToParentNode = new Dictionary<INotifyCollectionChanged, GridTreeNode>();
            
            if (!childListToParentNode.ContainsKey(collection))
            {
                collection.CollectionChanged += new NotifyCollectionChangedEventHandler(collection_CollectionChanged);
                childListToParentNode.Add(collection, node);
            }
            else
            {
                childListToParentNode[collection] = node;
            }
        }

        internal void UnwireCollectionChangeEvents(INotifyCollectionChanged collection)
        {
            if (collection == null)
                return;

            if (collection is IEnumerable)
            {
                foreach (var o in (IEnumerable)collection)
                {
                    if (o is INotifyPropertyChanged)
                        UnwirePropertyChangedEvents((INotifyPropertyChanged)o);
                }
            }
            collection.CollectionChanged -= new NotifyCollectionChangedEventHandler(collection_CollectionChanged);
        }

        internal void ClearChildBindingsFromICollectionChanged()
        {
            if (childListToParentNode == null)
                return;

            foreach (var item in childListToParentNode)
            {
                UnwireCollectionChangeEvents(item.Key);
            }
            childListToParentNode.Clear();
        }

        internal void WireCollectionChangedEvent(object itemsSource)
        {
            if (itemsSource != null && itemsSource is INotifyCollectionChanged)
            {
                var inotifyCollectionChangedSource = itemsSource as INotifyCollectionChanged;
                inotifyCollectionChangedSource.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCollectionChanged);
                inotifyCollectionChangedSource.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
            }
#if !SILVERLIGHT
            else if (itemsSource != null && itemsSource is IBindingList)
            {
                var bindingList = itemsSource as IBindingList;
                bindingList.ListChanged -= new ListChangedEventHandler(OnBindingListChanged);
                bindingList.ListChanged += new ListChangedEventHandler(OnBindingListChanged);
            }
#endif
        }

        //this will be created for unwire the old DataTable collection object or rootnode collection
        internal void UnWireCollectionChangedEvent(object itemsSource)
        {
            if (itemsSource != null && itemsSource is INotifyCollectionChanged)
            {
                var inotifyCollectionChangedSource = itemsSource as INotifyCollectionChanged;
                inotifyCollectionChangedSource.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCollectionChanged);
            }
#if !SILVERLIGHT
            else if (itemsSource != null && itemsSource is IBindingList)
            {
                var bindingList = itemsSource as IBindingList;
                bindingList.ListChanged -= new ListChangedEventHandler(OnBindingListChanged);
            }
#endif
        }

        //flag for deletedNode.
        bool isNodeDeleted = false;
#if !SILVERLIGHT
        void WireBindingListEvents(IBindingList collection, GridTreeNode node)
        {
            if (collection == null)
                return;

            if (childBindingListToParentNode == null)
            {
                parentNodeToBindingList = new Dictionary<GridTreeNode, IBindingList>();
                childBindingListToParentNode = new Dictionary<IBindingList, GridTreeNode>();
                if (node == null)
                {
                    childBindingListToParentNode.Add(collection, null);
                }
            }

            if (node != null && parentNodeToBindingList != null && !parentNodeToBindingList.ContainsKey(node))
            {
                parentNodeToBindingList.Add(node, collection);
                collection.ListChanged += new ListChangedEventHandler(bindingList_Changed);
                childBindingListToParentNode.Add(collection, node);
            }
            else
            {
                 childBindingListToParentNode[collection] = node;
            }
        }

        internal void UnwireBindingListEvents(IBindingList collection)
        {
            if (collection == null)
                return;
            if (collection is IEnumerable)
            {
                foreach (INotifyPropertyChanged o in (IEnumerable)collection)
                    UnwirePropertyChangedEvents(o);
            }
            collection.ListChanged -= new ListChangedEventHandler(bindingList_Changed);
        }

        internal void ClearChildBindingsFromIListChanged()
        {
            if (childBindingListToParentNode == null)
                return;

            foreach (IBindingList col in childBindingListToParentNode.Keys)
            {
                UnwireBindingListEvents(col);
            }

            childBindingListToParentNode.Clear();
            childBindingListToParentNode = null;

            if (parentNodeToBindingList == null)
                return;

            foreach (GridTreeNode n in parentNodeToBindingList.Keys)
            {
                UnhookBindingListChild(n);
            }
            parentNodeToBindingList.Clear();
            parentNodeToBindingList = null;
        }

        void UnhookBindingListChild(GridTreeNode n)
        {
            if (parentNodeToBindingList == null)
                return;
            if (parentNodeToBindingList.ContainsKey(n))
            {
                IBindingList col = parentNodeToBindingList[n];
                if (col != null)
                {
                    col.ListChanged -= new ListChangedEventHandler(bindingList_Changed);
                }
                foreach (GridTreeNode n1 in n.ChildNodes)
                {
                    UnhookBindingListChild(n1);
                }
            }
        }

        //gets called when root is changed
        void OnBindingListChanged(object sender, ListChangedEventArgs e)
        {
            IBindingList col = sender as IBindingList;
            if (col == null)
                return;

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    {
                        int loc = e.NewIndex;
                        object n = col[loc];
                        int row = GetVisibleRowIndexOf(n); //Nodes.IndexOf(n);
                        this.InvalidateCell(GridRangeInfo.Row(row));//this.InvalidateCell(GridRangeInfo.Row(row + 1));
                    }
                    break;
                case ListChangedType.ItemAdded:
                    {
                        this.CurrentCell.Deactivate();
                        int loc = e.NewIndex;
                        var newObj = col[loc];
                        {
                            //RootNodes.Add(OnCreatingTreeNode(0, newObj, false, null));
                            RootNodes.Insert(loc, OnCreatingTreeNode(0, newObj, false, null));
                            if (newObj is INotifyPropertyChanged)//PropertyChanged Event hook here for Root
                                WirePropertyChangedEvents((INotifyPropertyChanged)newObj);
                        }
                        if (!this.ParentTreeControl.EnableRenderCheckIfGlyphNeeded)
                            PopulateGridNodes(false, true);
                        else
                            PopulateGridNode(RootNodes[loc], false, true);
                        if (TrackSelectionOnCollectionChange)
                        {
                            SelectedNodes.Clear();
                            SelectedNodes.Add(RootNodes[loc]);
                            InvalidateCells();
                        }
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    {
                        //this for rootnode
                        //the currentsell is deactivate
                        this.CurrentCell.Deactivate(true);
                        // GridTreeNode node = GetNodeOf(col);
                        int loc = e.NewIndex;
                        if (loc > -1 && loc < RootNodes.Count)
                        {
                            //finding the deleted node in rootnode collection 
                            GridTreeNode n = FindDeletedNodeFromNodeList(col, RootNodes);
                            if (n == null)
                            {
                                break;
                            }
                            //if SelectedNodes contains the deleted node then remove from SelectedNodes.
                            if (SelectedNodes != null && SelectedNodes.Contains(n))
                            {
                                SelectedNodes.Remove(n);
                            }
                            this.CollapseNode(n);
                            int row = Nodes.IndexOf(n);
                            if (n.Item is INotifyPropertyChanged)
                                UnwirePropertyChangedEvents((INotifyPropertyChanged)n.Item);
                            this.Nodes.Remove(n);
                            UnhookBindingListChild(n);
                            RootNodes.Remove(n);
                            isNodeDeleted = true;
                            PopulateGridNodes(false, true);
                            isNodeDeleted = false;
                            if (TrackSelectionOnCollectionChange)
                            {
                                if (Nodes.Count > 0)
                                {
                                    SelectedNodes.Clear();
                                    if (row > 0)
                                        SelectedNodes.Add(Nodes[row - 1]);
                                    else
                                        SelectedNodes.Add(Nodes[row]);
                                }
                            }
                            // this.InvalidateCells();
                            this.InvalidateCell(GridRangeInfo.Rows(row, this.ScrollRows.LastBodyVisibleLineIndex));
                        }
                    }
                    break;
                case ListChangedType.Reset:
                    //Initialize RootNodes incase of RootNodes count = 0 and DataTable Resets

                    if (this.RootNodes != null && this.RootNodes.Count <= 0)
                    {
                        ClearChildBindingsFromIListChanged();
                        GridTreeRequestChildListEventArgs args = new GridTreeRequestChildListEventArgs(null);
                        OnRequestChildList(args);
                    }
                    PopulateGridNodes(false, true);
                    this.InvalidateCells();
                    break;
                default:
                    break;
            }
        }

        private GridTreeNode FindDeletedNodeFromNodeList(IBindingList col, List<GridTreeNode> node_list)
        {
            //finding the deleted node in childnode collection 
            GridTreeNode n2 = null;

            //if this is a dataview, we could find it faster.
            foreach (GridTreeNode n1 in node_list)
            {
                DataRowView dr = n1.Item as DataRowView;
                if (dr != null && (dr.Row.RowState == DataRowState.Deleted || dr.Row.RowState == DataRowState.Detached))
                {
                    if (col.IndexOf(n1.Item) == -1)
                    {
                        n2 = n1;
                        break;
                    }
                }
            }

            //the old generic way to find
            if (n2 == null)
            {
                foreach (GridTreeNode n1 in node_list)
                {
                    if (col.IndexOf(n1.Item) == -1)
                    {
                        n2 = n1;
                        break;
                    }
                }
            }
            return n2;
        }

#endif
        private void OnCollectionChanged(object ItemsSource, NotifyCollectionChangedEventArgs e)
        {
            //not used as all lists are hooked to collection_CollectionChanged.
        }

        ///<summary>
        /// To Find the node in which we have to add new node or delete old node 
        ///</summary>   
        public GridTreeNode GetTreeNodeOfRecord(object record)
        {
            if (this.ParentTreeControl.ParentPropertyName == null)
            {
                throw new ArgumentNullException("GridTreeControl.ParentPropertyName is not set");
            }

            var parentValue = this.GetValueFromComponent(this.ParentTreeControl.ChildPropertyName, record);
            // loop thru the nodes to find the parent value           
            var parentNode = this.Nodes.FirstOrDefault(n =>
                {
                    var value = this.GetValueFromComponent(this.ParentTreeControl.ParentPropertyName, n.Item);
                    if (value != null)
                    {
                        return value.Equals(parentValue);
                    }

                    return false;
                });

            return parentNode;
        }

        //flag for checking the Selection and Maintains the subnode selection of Collapsing the parentnode from code behind.
        internal bool isSelectedNodesChangedOnCollapsing = false;
#if !SILVERLIGHT
        //gets called when children change...
        void bindingList_Changed(object sender, ListChangedEventArgs e)
        {
            if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
            {
                this.Dispatcher.Invoke(new ListChangedEventHandler(bindingList_Changed), new object[] { sender, e });
                return;
            }
            
            //When childList Item changed then that row has been refreshed.
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                var col = sender as IBindingList;
                var node = GetNodeOf(col);
                int index = e.NewIndex;
                if (node != null)
                {
                    //var obj = node.ChildNodes[index].Item;
                    var obj = col[index];
                    int rowIndex = GetVisibleRowIndexOf(obj);
                    this.InvalidateCell(GridRangeInfo.Row(rowIndex));
                }
            }
            else if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                this.CurrentCell.Deactivate();
                var col = sender as IBindingList;
                var node = GetNodeOf(col);
                if (node != null)
                {
                    var flag = false;
                    if (node.Expanded || node.ChildNodes == null || node.ChildNodes.Count == 0)
                    {
                        isSelectedNodesChangedOnCollapsing = true;
                        CollapseNode(node);
                        isSelectedNodesChangedOnCollapsing = false;
                        flag = true;
                    }

                    int loc = e.NewIndex;
                    var newObj = col[loc];
                    {
                        // GridTreeNode treeNode = new GridTreeNode(node.Level + 1, newObj, true, node);
                        var treeNode = new GridTreeNode(node.Level + 1, newObj, false, node);
                        //node.ChildNodes.Add(treeNode);
                        node.ChildNodes.Insert(loc, treeNode);
                        loc++;
                        if (newObj is INotifyPropertyChanged)
                            WirePropertyChangedEvents((INotifyPropertyChanged)newObj);
                    }

                    if (flag)
                    {
                        ExpandNode(node);
                    }
                    int rowIndex = GetVisibleRowIndexOf(node.Item);
                    if (rowIndex > -1)
                    {
                        InvalidateCell(GridRangeInfo.Rows(rowIndex, this.ScrollRows.LastBodyVisibleLineIndex));
                        ///InvalidateCells();
                    }
                    if (TrackSelectionOnCollectionChange)
                    {
                        SelectedNodes.Clear();
                        SelectedNodes.Add(node.ChildNodes[loc - 1]);

                    }
                }
                else //adding a new root item...
                {
                    int loc = e.NewIndex;
                    var newObj = col[loc];
                    {
                        RootNodes.Add(OnCreatingTreeNode(0, newObj, false, null));
                    }
                    PopulateGridNodes(false, true);
                }
            }
            else if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                //this for childnode, and deactivate the currentcell
                CurrentCell.Deactivate(true);
                var col = sender as IBindingList;
                var node = GetNodeOf(col);
                if (node != null)
                {
                    var flag = node.Expanded;
                    //ExpandNode(node);
                    //int index = e.NewIndex;
                    //var obj = node.ChildNodes[index].Item;
                    //if (col is DataView && ((DataRowView)obj).Row.RowState != DataRowState.Deleted)
                    //{
                    //    foreach (GridTreeNode n1 in node.ChildNodes)
                    //    {
                    //        DataRowView dr = n1.Item as DataRowView;
                    //        if (dr.Row.RowState == DataRowState.Deleted || dr.Row.RowState == DataRowState.Detached)
                    //        {
                    //            obj = dr;
                    //            break;
                    //        }
                    //    }
                    //}
                    //finding the deleted node in childnode collection 
                    GridTreeNode n2 = null;
                    foreach (var n1 in node.ChildNodes)
                    {
                        //if (col.IndexOf(n1.Item) == -1)
                        var dr = n1.Item as DataRowView;
                        if (dr.Row.RowState == DataRowState.Deleted || dr.Row.RowState == DataRowState.Detached ||dr.Row.RowState==DataRowState.Modified)
                        {
                            n2 = n1;
                            break;
                        }
                    }
                    if (n2 == null)
                    {
                        return;
                    }
                    var obj = n2.Item;

                    if (obj is INotifyPropertyChanged)
                    {
                        UnwirePropertyChangedEvents((INotifyPropertyChanged)obj);
                    }
                    int rowIndex = GetVisibleRowIndexOf(obj);

                    if (rowIndex > 0)
                    {
                        GridTreeNode n = Nodes[rowIndex - 1];
                        //SelectedNode contains the delete node then remove from selectednode
                        if (SelectedNodes != null && SelectedNodes.Contains(n))
                        {
                            SelectedNodes.Remove(n);
                        }
                        CollapseNode(n);
                        Nodes.RemoveAt(rowIndex - 1);
                        node.ChildNodes.Remove(n);

                        if (TrackSelectionOnCollectionChange)
                        {
                            if (Nodes.Count > 0)
                            {
                                SelectedNodes.Clear();
                                if (rowIndex < Nodes.Count)
                                    SelectedNodes.Add(Nodes[rowIndex - 1]);
                                else if (rowIndex - 1 == Nodes.Count)
                                    SelectedNodes.Add(Nodes[rowIndex - 2]);
                            }
                        }

                    }
                    else if (node.ChildNodes != null)
                    {
                        CollapseNode(n2);
                        node.ChildNodes.Remove(n2);
                        int rowIdx = e.NewIndex;
                        if (TrackSelectionOnCollectionChange)
                        {
                            if (node.ChildNodes.Count > 0)
                            {
                                SelectedNodes.Clear();
                                if (rowIdx < node.ChildNodes.Count)
                                    SelectedNodes.Add(node.ChildNodes[rowIdx]);
                                else if (rowIdx == Nodes.Count)
                                    SelectedNodes.Add(node.ChildNodes[rowIdx - 1]);
                            }
                            else
                            {
                                SelectedNodes.Clear();
                                selectedNodes.Add(node);
                            }
                        }

                    }
                    CollapseNode(node);
                    if (node.ChildNodes != null && node.ChildNodes.Count == 0)
                        node.HasChildNodes = false;
                    if (flag)
                    {
                        isNodeDeleted = true;
                        ExpandNode(node);
                        isNodeDeleted = false;
                    }

                    int rowIndex1 = GetVisibleRowIndexOf(node.Item);
                    if (rowIndex1 > -1)
                    {
                        InvalidateCell(GridRangeInfo.Rows(rowIndex1, this.ScrollRows.LastBodyVisibleLineIndex));
                    }
                }
                else //removing root node
                {
                    int loc = e.NewIndex;
                    if (loc > -1 && loc < RootNodes.Count)
                    {
                        RootNodes.RemoveAt(loc);
                        PopulateGridNodes(false, true);
                    }
                }
            }
            else if (e.ListChangedType == ListChangedType.Reset)
            {
                IBindingList col = sender as IBindingList;
                GridTreeNode node = GetNodeOf(col);
                if (node != null)
                {
                    CollapseNode(node);
                    node.ChildNodes.Clear();
                    if (col.IndexOf(node.Item) != -1)
                        ExpandNode(node);
                }
                else
                {  //root
                    this.PopulateTree();
                }
            }

        }
#endif

        private void AddNodesToTreeGrid(object sender, NotifyCollectionChangedEventArgs e)
        {
            INotifyCollectionChanged col = sender as INotifyCollectionChanged;
            GridTreeNode node = GetNodeOf(col);
            if (node != null)
            {
                bool flag = false;
                if (node.Expanded || node.ChildNodes == null || node.ChildNodes.Count == 0)
                {
                    CollapseNode(node);
                    flag = true;
                }

                int loc = e.NewStartingIndex;
                foreach (var newObj in e.NewItems)
                {
                    // GridTreeNode treeNode = new GridTreeNode(node.Level + 1, newObj, true, node);
                    //This code has been commented to create the node based on user specified expand state
                    //GridTreeNode treeNode = new GridTreeNode(node.Level + 1, newObj, false, node);

                    /// this.ExpandNewNodeOnCreation && flag this condition is added to set the expand state based on parent's expand state
                    GridTreeNode treeNode = new GridTreeNode(node.Level + 1, newObj, this.ExpandNewNodeOnCreation && flag, node);
                    //node.ChildNodes.Add(treeNode);
                    node.ChildNodes.Insert(loc, treeNode);
                    loc++;
                    if (newObj is INotifyPropertyChanged)
                        WirePropertyChangedEvents((INotifyPropertyChanged)newObj);
                }
                if (flag)
                {
                    ExpandNode(node);
                }

                int rowIndex = GetRowIndexFromItem(e.NewItems[0]);
                if (rowIndex > -1)
                {
                    InvalidateCell(GridRangeInfo.Rows(Math.Max(rowIndex, this.ScrollRows.ScrollLineIndex), this.ScrollRows.LastBodyVisibleLineIndex));
                }
            }
            else //adding a new root item...
            {
                int loc = e.NewStartingIndex;
                foreach (var newObj in e.NewItems)
                {
                    // This code has been commented to create the node based on user specified expand state
                    // RootNodes.Insert(loc, OnCreatingTreeNode(0, newObj, false, null));
                    if (this.AllowSort && !string.IsNullOrEmpty(this.SortProperty))
                    {
                        if (RootNodes.Count > 0)
                        {
                            //get the index of new item
                            int index = this.RootNodes.IndexOf(OnCreatingTreeNode(0, newObj, this.ExpandNewNodeOnCreation, null));
                            //get the index where the item to be inserted.
                            loc = this.GetComparerIndex(OnCreatingTreeNode(0, newObj, this.ExpandNewNodeOnCreation, null), index);
                            RootNodes.Insert(loc, OnCreatingTreeNode(0, newObj, this.ExpandNewNodeOnCreation, null));
                            loc++;
                            if (newObj is INotifyPropertyChanged)
                                WirePropertyChangedEvents((INotifyPropertyChanged)newObj);
                        }
                    }
                    else
                    {
                        /// ExpandNewNodeOnCreation is added, to create the node with the expand state specified by the user.
                        RootNodes.Insert(loc, OnCreatingTreeNode(0, newObj, this.ExpandNewNodeOnCreation, null));
                        loc++;
                        if (newObj is INotifyPropertyChanged)
                            WirePropertyChangedEvents((INotifyPropertyChanged)newObj);
                    }
                }
                PopulateGridNodes(false, true);
                int rowIndex = GetRowIndexFromItem(e.NewItems[0]);
                if (rowIndex > -1)
                {
                    InvalidateCell(GridRangeInfo.Rows(Math.Max(rowIndex, this.ScrollRows.ScrollLineIndex), this.ScrollRows.LastBodyVisibleLineIndex));
                }
            }
        }
        //this is for rootnodes
        private int GetComparerIndex(GridTreeNode record, int removeAtIndex)
        {
            int comparerindex = InternalBinarySearch(RootNodes, removeAtIndex, record, SortComparer);
            return ~comparerindex;
        }
        //this is for childnodes
        private int GetChildComparerIndex(List<GridTreeNode> internalList, GridTreeNode record, int removeAtIndex)
        {
            int comparerindex = InternalBinarySearch(internalList, removeAtIndex, record, SortComparer);
            return ~comparerindex;
        }

        internal int InternalBinarySearch(List<GridTreeNode> internalList, int removeatIndex, GridTreeNode value, IComparer<GridTreeNode> SortComparer)
        {
            if (removeatIndex >= 0)//if the value is already in the list, it will be removed
                internalList.RemoveAt(removeatIndex);
            int num = 0;
            int num2 = (internalList.Count) - 1;
            while (num <= num2)
            {
                int num3 = num + ((num2 - num) >> 1);
                int num4 = SortComparer.Compare(internalList[num3], value);
                if (num4 == 0)
                {
                    return num3;
                }
                if (num4 < 0)
                {
                    num = num3 + 1;
                }
                else
                {
                    num2 = num3 - 1;
                }
            }
            return ~num;
        }

        private void RemoveNodeFromTreeGrid(object sender, NotifyCollectionChangedEventArgs e)
        {
            INotifyCollectionChanged col = sender as INotifyCollectionChanged;
            GridTreeNode node = GetNodeOf(col);
            if (node != null)
            {
                bool flag = false;
                if (node.Expanded)
                {
                    flag = true;
                }

                ExpandNode(node);
                int index = e.OldStartingIndex;
                foreach (var obj in e.OldItems)
                {
                    if (obj is INotifyPropertyChanged)
                    {
                        UnwirePropertyChangedEvents((INotifyPropertyChanged)obj);
                    }
                    int rowIndex = GetVisibleRowIndexOf(obj);

                    if (rowIndex > 0)
                    {
                        GridTreeNode n = this.GetNodeAtRowIndex(rowIndex);
                        //GridTreeNode n = Nodes[rowIndex - 1];
                        ZapCollectionsEvents(n);
                        CollapseNode(n);
                        if (SelectedNodes.Contains(n))
                            SelectedNodes.Remove(n);
                        Nodes.Remove(n); //rowIndex - 1);
                        node.ChildNodes.Remove(n);
                    }
                    else if (node.ChildNodes != null && node.ChildNodes.Count > index)
                    {
                        CollapseNode(node.ChildNodes[index]);
                        node.ChildNodes.RemoveAt(index);
                    }
                }
                CollapseNode(node);
                if (node.ChildNodes != null && node.ChildNodes.Count == 0)
                    node.HasChildNodes = false;
                if (flag)
                {
                    ExpandNode(node);
                }

                int rowIndex1 = GetRowIndexFromItem(node.Item);
                if (rowIndex1 > -1 && rowIndex1 <= this.ScrollRows.LastBodyVisibleLineIndex)
                {
                    InvalidateCell(GridRangeInfo.Rows(Math.Max(rowIndex1, this.ScrollRows.ScrollLineIndex), this.ScrollRows.LastBodyVisibleLineIndex));
                }
            }
            else //removing root node
            {
                int loc = e.OldStartingIndex;
                int rowIndex = GetRowIndexFromItem(e.OldItems[0]); // -1;

                foreach (var newObj in e.OldItems)
                {
                    var node1 = this.GetNodeAtRowIndex(rowIndex);

                    if (node1 != null && SelectedNodes.Contains(node1))
                    {
                        SelectedNodes.Remove(node1);
                    }
                    ZapCollectionsEvents(Nodes[this.GetRecordsIndexFromRowIndex(rowIndex)]);
                    RootNodes.Remove(node1);
                }
                PopulateGridNodes(false, true);
                if (rowIndex > -1 && rowIndex <= this.ScrollRows.LastBodyVisibleLineIndex)
                {
                    InvalidateCell(GridRangeInfo.Rows(Math.Max(rowIndex, this.ScrollRows.ScrollLineIndex), this.ScrollRows.LastBodyVisibleLineIndex));
                }
            }
        }

        void collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
#if !SILVERLIGHT

            if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
            {
                this.Dispatcher.Invoke(new NotifyCollectionChangedEventHandler(collection_CollectionChanged), new object[] { sender, e });
                return;
            }
#endif
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                this.AddNodesToTreeGrid(sender, e);
            }

            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                this.RemoveNodeFromTreeGrid(sender, e);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset || e.Action == NotifyCollectionChangedAction.Replace)
            {
                INotifyCollectionChanged col = sender as INotifyCollectionChanged;
                GridTreeNode node = GetNodeOf(col);
                if (node != null)
                {
                    bool keepExpandState = node.Expanded;
                    CollapseNode(node);
                    node.ChildNodes.Clear();
                    if (keepExpandState)
                        ExpandNode(node);
                }
                else
                {  //root
                    this.PopulateTree();
                }
            }
#if !SILVERLIGHT
            else if (e.Action == NotifyCollectionChangedAction.Move)
            {
                this.RemoveNodeFromTreeGrid(sender, e);
                this.AddNodesToTreeGrid(sender, e);

            }
#endif
        }

        private void ZapCollectionsEvents(GridTreeNode n)
        {
            INotifyCollectionChanged col = GetCollection(n);
            if (col != null)
            {
                this.UnwireCollectionChangeEvents(col);
            }
            if (n.HasChildNodes)
            {
                foreach (GridTreeNode n1 in n.ChildNodes)
                {
                    ZapCollectionsEvents(n1);
                }
            }
        }

        private INotifyCollectionChanged GetCollection(GridTreeNode n)
        {
            INotifyCollectionChanged col = null;
            object o = n.Item;
            if (ParentTreeControl.ChildPropertyName != null && pdc[ParentTreeControl.ChildPropertyName] != null)
            {
                col = pdc[ParentTreeControl.ChildPropertyName].GetValue(o) as INotifyCollectionChanged;
            }

            return col;
        }

        private Dictionary<INotifyCollectionChanged, GridTreeNode> childListToParentNode = null;

#if !SILVERLIGHT
        private Dictionary<IBindingList, GridTreeNode> childBindingListToParentNode = null;
        private Dictionary<GridTreeNode, IBindingList> parentNodeToBindingList = null;

        private GridTreeNode GetNodeOf(IBindingList col)
        {
            if (childBindingListToParentNode != null && childBindingListToParentNode.ContainsKey(col))
                return childBindingListToParentNode[col];
            return null;
        }
#endif

        private GridTreeNode GetNodeOf(INotifyCollectionChanged col)
        {
            if (childListToParentNode != null && childListToParentNode.ContainsKey(col))
                return childListToParentNode[col];
            return null;
        }

        void WirePropertyChangedEvents(INotifyPropertyChanged item)
        {
            if (item != null)
            {
                item.PropertyChanged += new PropertyChangedEventHandler(OnItemPropertychanged);
            }
        }

        void UnwirePropertyChangedEvents(INotifyPropertyChanged item)
        {
            if (item != null)
            {
                item.PropertyChanged -= new PropertyChangedEventHandler(OnItemPropertychanged);
            }
        }

        protected virtual void OnItemPropertychanged(object sender, PropertyChangedEventArgs e)
        {

#if !SILVERLIGHT
            if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
            {
                this.Dispatcher.Invoke(new PropertyChangedEventHandler(OnItemPropertychanged), new object[] { sender, e });
                return;
            }

            if (this.EnableRenderOptimization == EnableRenderOptimization.DisableBackgroundFrameRendering)
                this.needRenderStyleBackgrounds = false;
#endif

            if (this.parentTreeControl != null && this.parentTreeControl.ChildPropertyName == e.PropertyName)
            {
                bool isNodeAlreadyCollapsed = false;
                int rowIndex = GetRowIndexFromItem(sender);
                GridTreeNode node = GetNodeAtRowIndex(rowIndex);
                isNodeAlreadyCollapsed = !node.Expanded;
                CollapseNode(node);
                node.HasChildNodes = false;//reset them so it is reloaded
                node.ChildNodes = new List<GridTreeNode>();
                ExpandNode(node);
                // Code to persist the previous state
                if (isNodeAlreadyCollapsed)
                    CollapseNode(node);
                if (rowIndex <= this.ScrollRows.LastBodyVisibleLineIndex)
                {
                    InvalidateCell(GridRangeInfo.Rows(rowIndex, this.ScrollRows.LastBodyVisibleLineIndex));
                }
                return;
            }

            if (InvalidateActionOnCellChange == InvalidateAction.Grid)
            {
                InvalidateCells();
            }
            else
            {
                int rowIndex = GetVisibleRowIndexOf(sender);
                if (rowIndex > -1)
                {
                    switch (InvalidateActionOnCellChange)
                    {
                        case InvalidateAction.Cell:
                            int colIndex = ColumnNameToPosition(e.PropertyName);
                            if (colIndex > -1)
                            {
                                InvalidateCell(GridRangeInfo.Cell(rowIndex, colIndex + 1));
#if !SILVERLIGHT
                                MarkIndividualCellBackgroundDirty(rowIndex, colIndex + 1);
#endif
                                var visiblecolumn = Columns[colIndex];
                                if (visiblecolumn != null && !string.IsNullOrEmpty(visiblecolumn.ReferenceFields))
                                {
                                    string[] Fields = visiblecolumn.ReferenceFields.Split(';');
                                    foreach (var field in Fields)
                                    {
                                        var columnIndex = ColumnNameToPosition(field);
                                        if (columnIndex > -1)
                                        {
                                            this.InvalidateCell(new RowColumnIndex(rowIndex, columnIndex + 1));
#if !SILVERLIGHT
                                            this.MarkIndividualCellBackgroundDirty(rowIndex, columnIndex + 1);
#endif
                                        }
                                    }
                                }
#if SILVERLIGHT
                                this.InvalidateVisual();
#endif
                            }
                            else
                                this.InvalidateCells();
                            break;
                        case InvalidateAction.Row:
                            InvalidateCell(GridRangeInfo.Row(rowIndex));
#if !SILVERLIGHT
                            MarkIndividualRowBackgroundDirty(rowIndex);
#endif
                            break;
                        default:
                            break;
                    }
                }
            }
#if !SILVERLIGHT
            if (this.ParentTreeControl != null && (!CurrentCell.IsEditing && (ParentTreeControl.SortingOptions & GridTreeSortingOptions.DisableSortingOnPropertyChange) == 0))
            {
                if (this.AllowSort && !string.IsNullOrEmpty(this.SortProperty) && (this.SortProperty.Contains(e.PropertyName) ||string.IsNullOrEmpty(e.PropertyName)))
                {
                    SortAfterEdit(GetRowIndexFromItem(sender), ColumnNameToPosition(e.PropertyName));
                }
            }
#endif
        }

        private int GetVisibleRowIndexOf(object item)
        {
            int toprowindex = 0;
            if (this.UnboundRowPosition == Position.Top)
                toprowindex = this.Model.HeaderRows + this.UnboundRowsCount;
            else
                toprowindex = this.Model.HeaderRows;

            //search only the visible rows in the grid...
            int n = -1;
            for (int i = toprowindex; i <= this.ScrollRows.LastBodyVisibleLineIndex; i++)
            {
                var node = GetNodeAtRowIndex(i);
                if (node != null && node.Item.Equals(item))
                {
                    n = i;
                    break;
                }
            }
            ////code that would search all the nodes...
            //for (int i = 0; i < Nodes.Count; ++i)
            //{
            //    if (Nodes[i].Item.Equals(item))
            //    {
            //        n = i + 1;
            //        break;
            //    }
            //}
            return n;
        }

        #endregion

        void ProcessListRequest(GridTreeRequestChildListEventArgs args)
        {
            if (args.ParentItem == null)
            {
                if (args.ChildList != null)
                {
                    ArrayList list = new ArrayList();
                    foreach (object o in args.ChildList)
                    {
                        list.Add(o);
                    }

                    PopulateRootNodes(list.ToArray());

                    CheckIfNotifyPropertyChangeNeeded(args.ChildList, args.ParentNode);

                }
            }
            else if (args.ParentNode.ChildNodes.Count == 0 || args.ResetChildAndRepopulate)
            {
                args.ParentNode.ChildNodes.Clear();
                object emp = args.ParentNode.Item;
                if (args.ChildList != null)
                {
                    ArrayList list = new ArrayList();
                    foreach (object o in args.ChildList)
                    {
                        GridTreeNode childNode = OnCreatingTreeNode(args.ParentNode.Level + 1, o, false, args.ParentNode);// new GridTreeNode(args.ParentNode.Level + 1, o, false, args.ParentNode);
                        childNode.ParentItem = args.ParentNode.Item;
                        foreach (var n in SelectedNodes)
                            if (n.Item == childNode.Item)
                                childNode.IsSelected = true;
                        args.ParentNode.ChildNodes.Add(childNode);
                    }
                    CheckIfNotifyPropertyChangeNeeded(args.ChildList, args.ParentNode);
                }
            }
        }

        private void CheckIfNotifyPropertyChangeNeeded(IEnumerable list, GridTreeNode node)
        {
            if (this.NotifyPropertyChanges)
            {
                if (list is INotifyCollectionChanged)
                {
                    WireCollectionChangeEvents((INotifyCollectionChanged)list, node);
                }
#if !SILVERLIGHT
                else if (list is IBindingList)
                {
                    WireBindingListEvents((IBindingList)list, node);
                }
#endif

                bool? b = null;
                foreach (object o in list)
                {
                    if (!b.HasValue)
                    {
                        b = o is INotifyPropertyChanged;
                    }
                    if (!b.Value)
                    {
                        break;
                    }
                    WirePropertyChangedEvents((INotifyPropertyChanged)o);
                }
            }
        }

        #endregion

        #region fields

        //used only when this GridTreeControlImpl object is embedded in a GridTreeControl
        internal GridTreeControl parentTreeControl = null;

        private int nodeColumnIndex = 1; // grid column index for the expand cell
        internal string expanderCellType = "ExpanderCell"; //name of expand cell type
        internal string SortHeaderCellType = "SortHeader"; //name of sort header cell type
        private double nodeColumnWidth = 10; //was 10

        private GridTreeExpandGlyph expandGlyphType = GridTreeExpandGlyph.Triangle;

        /// <summary>
        /// Gets or sets the type of the glyph shown in the expand cell.
        /// </summary>
        /// <remarks>
        /// The default value is a triangle. You can also set a +- glyph, or a +-glyph with tree lines, or
        /// a custom drawn glyph. The property NodeColumnWidth reserves the required width of your glyph. The
        /// default value of NodeColumnWidth is 10 which is the setting used for the triangle glyph. For the
        /// +- glyph, the value of NodeColumnWidth is set to 14. If you want to explicitly provide a particular
        /// NodeColumnWidth, then you need to explicitly reset its value after you set ExpandGlyphType as setting
        /// ExpandGlypType also possibly resets NodeColumnWidth.
        /// </remarks>
        public GridTreeExpandGlyph ExpandGlyphType
        {
            get { return expandGlyphType; }
            set
            {
                expandGlyphType = value;
                switch (expandGlyphType)
                {
                    case GridTreeExpandGlyph.PlusMinus:
#if !SILVERLIGHT
                    case GridTreeExpandGlyph.PlusMinusLines:
#endif
                        NodeColumnWidth = 20;// 14;

                        break;
                    case GridTreeExpandGlyph.Triangle:
                        NodeColumnWidth = 14;
                        break;

                    case GridTreeExpandGlyph.Themed:
                        NodeColumnWidth = 10;
                        break;


#if SILVERLIGHT
                    case GridTreeExpandGlyph.Custom:
                        NodeColumnWidth = 20;
                        break;
#else
                    case GridTreeExpandGlyph.Custom:
                        NodeColumnWidth = 14;
                        break;
#endif
                    default:
                        break;
                }
#if !SILVERLIGHT
                GridTreeExpandCellRenderer renderer = this.CellRenderers[expanderCellType] as GridTreeExpandCellRenderer;
                GridTreeExpanderCellRendererExt rendererext = this.CellRenderers[expanderCellType] as GridTreeExpanderCellRendererExt;
#else
                GridTreeExpanderCellRenderer renderer = this.CellRenderers[expanderCellType] as GridTreeExpanderCellRenderer;
                GridTreeExpanderCellRendererExt rendererext = this.CellRenderers[expanderCellType] as GridTreeExpanderCellRendererExt;
#endif
                if (rendererext != null)
                {
                    rendererext.NodeColumnWidth = NodeColumnWidth;
                    rendererext.ExpandGlyphType = expandGlyphType;
#if !SILVERLIGHT

                    rendererext.ExpandGlyphType = expandGlyphType;
#endif
                    this.UnloadArrangedCells();

                }

                else if (renderer != null)
                {
                    renderer.NodeColumnWidth = NodeColumnWidth;
                    renderer.ExpandGlyphType = expandGlyphType;
#if !SILVERLIGHT
                    //SetBrushDefaultsOnRenderer(renderer);
#endif
                    this.UnloadArrangedCells();
                }
            }
        }

#if !SILVERLIGHT
        private void SetBrushDefaultsOnRenderer(GridTreeExpandCellRenderer renderer)
        {

            switch (ExpandGlyphType)
            {
                case GridTreeExpandGlyph.PlusMinus:
                case GridTreeExpandGlyph.PlusMinusLines:
                    SetExpandBrushesAndPen(Brushes.Black, Brushes.LightGray, new Pen(Brushes.Black, penWidth));
                    break;
                case GridTreeExpandGlyph.Triangle:
                    SetExpandBrushesAndPen(Brushes.Blue, Brushes.LightBlue, new Pen(Brushes.Blue, .2));
                    break;
                default:
                    break;
            }

        }
#endif
        /// <summary>
        /// Sets the brushes and pen used in drawing the glyph in the expand cell.
        /// </summary>
        /// <param name="expandWidgetBrush">The Brush for drawing the interior of the normal glyph.</param>
        /// <param name="hotExpandWidgetBrush">The Brush for drawing the interior of the glyph when teh mouse is over it.</param>
        /// <param name="expandWidgetPen">The Pen for drawing the glyph border.</param>
        public void SetExpandBrushesAndPen(Brush expandWidgetBrush, Brush hotExpandWidgetBrush, Pen expandWidgetPen)
        {
#if !SILVERLIGHT
            GridTreeExpandCellRenderer renderer = null;
            GridTreeExpanderCellRendererExt rendererext = null;
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                renderer = this.CellRenderers[expanderCellType] as GridTreeExpandCellRenderer;
                rendererext = this.CellRenderers[expanderCellType] as GridTreeExpanderCellRendererExt;
            }
#else
            GridTreeExpanderCellRenderer renderer = this.CellRenderers[expanderCellType] as GridTreeExpanderCellRenderer;            
#endif


#if !SILVERLIGHT
            if (renderer != null)
            {
                renderer.ExpandWidgetBrush = expandWidgetBrush;
                renderer.ExpandWidgetPen = expandWidgetPen;
                renderer.HotExpandWidgetBrush = hotExpandWidgetBrush;
            }
            else if (rendererext != null)
            {
                rendererext.ExpandWidgetBrush = expandWidgetBrush;
                rendererext.ExpandWidgetPen = expandWidgetPen;
                rendererext.HotExpandWidgetBrush = hotExpandWidgetBrush;
            }
#endif



            this.UnloadArrangedCells();

        }

        /// <summary>
        /// Gets or sets the width of the expand glyph area.
        /// </summary>
        /// <remarks>
        /// When the expand glyph is the triangle, this value is set to 10. When the expand
        /// glyph is a +-, this value is set to 14. If you want to explicitly provide a particular
        /// NodeColumnWidth, then you need to explicitly reset its value after you set ExpandGlyphType as setting
        /// ExpandGlypType also possibly resets NodeColumnWidth.
        /// </remarks>
        public double NodeColumnWidth
        {
            get { return nodeColumnWidth; }
            set
            {
                nodeColumnWidth = value;
#if !SILVERLIGHT
                GridTreeExpandCellRenderer renderer = this.CellRenderers[expanderCellType] as GridTreeExpandCellRenderer;
                GridTreeExpanderCellRendererExt rendererext = this.CellRenderers[expanderCellType] as GridTreeExpanderCellRendererExt;
#else
                GridTreeExpanderCellRenderer renderer = this.CellRenderers[expanderCellType] as GridTreeExpanderCellRenderer;
                GridTreeExpanderCellRendererExt rendererext = this.CellRenderers[expanderCellType] as GridTreeExpanderCellRendererExt;
#endif
                if (renderer != null)
                {
                    renderer.NodeColumnWidth = NodeColumnWidth;
                    this.UnloadArrangedCells();
                }
                else if (rendererext != null)
                {
                    rendererext.NodeColumnWidth = NodeColumnWidth;

                }
            }
        }

        private GridSelectedTreeNodes selectedNodes = null;
        internal void SetSelectedNodeCollection(GridSelectedTreeNodes sNodes)
        {
            this.selectedNodes = sNodes;
            this.selectedNodes.owner = this;
        }
        /// <summary>
        /// Gets a collection of the selected nodes within the GridTreeControl. This property is only valid when 
        /// EnableNodeSelection is true.
        /// </summary>
        public GridSelectedTreeNodes SelectedNodes
        {
            get
            {
                if (this.ParentTreeControl == null)
                {
                    selectedNodes = new GridSelectedTreeNodes(this);
                    return selectedNodes;
                }
                return this.ParentTreeControl.SelectedNodes;

            }
        }

        #endregion

        #region ISupportsRecordSelection<GridTreeNode> Members

        GridSelectedObjectsBase<GridTreeNode> ISupportsRecordSelection<GridTreeNode>.SelectedNodes
        {
            get { return this.SelectedNodes; }
        }

        bool ISupportsRecordSelection<GridTreeNode>.EnableNodeSelection
        {
            get
            {
                return this.EnableNodeSelection;
            }
            set
            {
                this.EnableNodeSelection = value;
            }
        }

        GridControlBase ISupportsRecordSelection<GridTreeNode>.InternalGrid
        {
            get { return this.InternalGrid; }
        }

        GridTreeNode ISupportsRecordSelection<GridTreeNode>.GetNodeAtRowIndex(int gridRowIndex)
        {
            return this.GetNodeAtRowIndex(gridRowIndex);
        }

        int ISupportsRecordSelection<GridTreeNode>.GetRowIndexFromItem(object item)
        {
            return this.GetRowIndexFromItem(item);
        }

        #endregion

        #region VisualStyles support

        /// <summary>
        /// DependencyProperty for <see cref = "GridTreeControl.VisualStyle"/>.
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty;

        /// <summary>
        /// Gets or sets the visual style.
        /// </summary>
        /// <value>The visual style.</value>
        public VisualStyle VisualStyle
        {
            get
            {
                return (VisualStyle)this.GetValue(GridTreeControlImpl.VisualStyleProperty);
            }

            set
            {
                this.SetValue(GridTreeControlImpl.VisualStyleProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref = "GridTreeControl.CustomVisualStyle"/>.
        /// </summary>
        public static readonly DependencyProperty CustomVisualStyleProperty;

        /// <summary>
        /// Gets or sets the custom visual style.
        /// </summary>
        /// <value>The custom visual style.</value>
        public IGridTreeVisualStyle CustomVisualStyle
        {
            get
            {
                return (IGridTreeVisualStyle)this.GetValue(GridTreeControlImpl.CustomVisualStyleProperty);
            }

            set
            {
                this.SetValue(GridTreeControlImpl.CustomVisualStyleProperty, value);
            }
        }

        private static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridTreeControlImpl grid = d as GridTreeControlImpl;
            grid.VisualStyle = (VisualStyle)args.NewValue;
            grid.ApplyGridVisualStyle(grid.GetVisualStyle(grid.VisualStyle));
        }


        # region IsLegacyStyleEnabled
        public bool EnableLegacyStyle
        {
            get { return (bool)GetValue(EnableLegacyStyleProperty); }
            set { SetValue(EnableLegacyStyleProperty, value); }
        }

        public static readonly DependencyProperty EnableLegacyStyleProperty =
            DependencyProperty.Register("EnableLegacyStyle", typeof(bool), typeof(GridTreeControlImpl), new PropertyMetadata(false));

        # endregion

        # region AutoPopulateColumns
        /// <summary>
        /// If AutoPopulateColumns is set to True, then visible column defined will be populated in the View.
        /// else all the columns in the itemsource will be populated in the view
        /// </summary>
        public bool AutoPopulateColumns
        {
            get { return (bool)GetValue(AutoPopulateColumnsProperty); }
            set { SetValue(AutoPopulateColumnsProperty, value); }
        }

        public static readonly DependencyProperty AutoPopulateColumnsProperty =
            DependencyProperty.Register("AutoPopulateColumns", typeof(bool), typeof(GridTreeControlImpl), new PropertyMetadata(true));

        # endregion

        #region AutoGenerateColumnsInfo

        /// <summary>
        /// Gets or sets a value indicating whether to generate the CellType automatically
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [auto generate columns info]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoGenerateColumnsInfo 
        {
            get { return (bool)GetValue(AutoGenerateColumnsInfoProperty); }
            set { SetValue(AutoGenerateColumnsInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoGenerateColumnsInfo .
        public static readonly DependencyProperty AutoGenerateColumnsInfoProperty =
            DependencyProperty.Register("AutoGenerateColumnsInfo ", typeof(bool), typeof(GridTreeControlImpl), new PropertyMetadata(false));

        
        #endregion
        //this method is added for setting the themes backgroung property for the issue 9876
        internal override void RaisePrepareRenderCell(GridPrepareRenderCellEventArgs e)
        {
            var visualstyle = this.GetVisualStyle(VisualStyle);
            RowColumnIndex rci = new RowColumnIndex();
            rci.RowIndex = e.Cell.RowIndex;
            rci.ColumnIndex = e.Cell.ColumnIndex;
#if !SILVERLIGHT
            if (this.EnableHotRowMarker)
            {
                Brush Background = e.Style.Background;
                if (rowSpanUnderMouse != null)
                {
                    if (rowSpanUnderMouse.Contains(rci))
                    {
                        if (!this.EnableLegacyStyle)
                        {
                            e.Style.Background = this.GetGridTreeRowHoverBackgroundBrush(visualstyle);
                            e.Style.Foreground = this.GetGridTreeRowHoverForegroundBrush(visualstyle);
                        }
                        else
                            e.Style.Background = this.MarkRowBrush;
                    }
                }
                if (oldrowSpanUnderMouse != null)
                {
                    if (oldrowSpanUnderMouse.Contains(rci))
                    {
                        e.Style.Background = Background;
                    }
                    oldrowSpanUnderMouse = null;
                }
            }
#endif

            if (!EnableLegacyStyle && Model.Options != null && ((Model.Options.DrawSelectionOptions & (GridDrawSelectionOptions.ReplaceBackground | GridDrawSelectionOptions.ReplaceTextColor)) != 0))
            {
                GridTreeNode n;
                GridTreeRowType rowType = GetGridRowType(rci.RowIndex, out n);
#if SILVERLIGHT
                if (n != null && rci.ColumnIndex > 0 && n != null && n.IsSelected && (this.EnableNodeSelection || cellIsSelected(n, rci.ColumnIndex)))
#else
                if (n != null && rci.ColumnIndex > 0 && n != null && n.IsSelected && (this.EnableNodeSelection || cellIsSelected(n, rci.ColumnIndex)))
#endif
                {

                    if (CurrentCell.RowIndex == e.Style.RowIndex && CurrentCell.ColumnIndex == e.Style.ColumnIndex && this.Model.Options.ShowCurrentCell)
                    {
                        if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceBackground) != 0)
                        {
#if SILVERLIGHT
                             if (!this.CurrentCell.IsEditing)
                                 e.Style.Background = this.GetGridTreeCurrentCellSelectionBackground(visualstyle);
                             else
                                 e.Style.Background = Brushes.White;
#else
                            e.Style.Background = this.GetGridTreeCurrentCellSelectionBackground(visualstyle);
#endif
                        }
                        if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceTextColor) != 0)
                        {
#if SILVERLIGHT
                             if (!this.CurrentCell.IsEditing)
                                 e.Style.Foreground = this.GetGridTreeCurrentCellSelectionForeground(visualstyle);
                             else
                                 e.Style.Foreground = Brushes.Black;
#else
                            e.Style.Foreground = this.GetGridTreeCurrentCellSelectionForeground(visualstyle);
#endif
                        }
                    }
                    else
                    {
                        if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceBackground) != 0)
                        {
                            e.Style.Background = this.Model.Options.HighlightSelectionBackground;
                        }
                        if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceTextColor) != 0)
                        {
                            e.Style.Foreground = this.Model.Options.HighlightSelectionForeground;
                        }
                    }
                }
#if !SILVERLIGHT
                else
                {
                    if (rci.ColumnIndex > 0 && rci.RowIndex > 0 && (!this.EnableNodeSelection || this.Model.SelectedRanges.Contains(GridRangeInfo.Table())) && Model.SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(e.Cell.RowIndex, e.Cell.ColumnIndex)))
                    {
                        if (CurrentCell.RowIndex == e.Style.RowIndex && CurrentCell.ColumnIndex == e.Style.ColumnIndex && this.Model.Options.ShowCurrentCell)
                        {
                            if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceBackground) != 0)
                            {

                                e.Style.Background = this.GetGridTreeCurrentCellSelectionBackground(visualstyle);
                            }
                            if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceTextColor) != 0)
                            {
                                e.Style.Foreground = this.GetGridTreeCurrentCellSelectionForeground(visualstyle);
                            }
                        }
                        else
                        {
                            if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceBackground) != 0)
                            {
                                e.Style.Background = this.Model.Options.HighlightSelectionBackground;
                            }
                            if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceTextColor) != 0)
                            {
                                e.Style.Foreground = this.Model.Options.HighlightSelectionForeground;
                            }
                        }
                    }
                }
#endif
            }
            else if (EnableLegacyStyle && Model.Options != null && ((Model.Options.DrawSelectionOptions & (GridDrawSelectionOptions.ReplaceBackground | GridDrawSelectionOptions.ReplaceTextColor)) != 0))
            {
                GridTreeNode n;
                GridTreeRowType rowType = GetGridRowType(rci.RowIndex, out n);
#if SILVERLIGHT
                if (n != null && rci.ColumnIndex > 0 && n != null && n.IsSelected && (this.EnableNodeSelection ||cellIsSelected(n, rci.ColumnIndex)))
#else
                if (n != null && rci.ColumnIndex > 0 && n != null && n.IsSelected && (this.EnableNodeSelection || cellIsSelected(n, rci.ColumnIndex)))
#endif
                {
                    e.Style.Background = Model.Options.HighlightSelectionAlphaBlend;
                }
#if !SILVERLIGHT
                else
                {
                    if (rci.ColumnIndex > 0 && !this.EnableNodeSelection && Model.SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(e.Cell.RowIndex, e.Cell.ColumnIndex)))
                    {
                        e.Style.Background = Model.Options.HighlightSelectionAlphaBlend;
                    }
                }
#endif
            }

            OnPrepareRenderCell(e);
        }
//We have already set the currentcellbackgroung from RaisePrepareRenderCell()RaisePrepareRenderCell method, so no need this code.
//#if SILVERLIGHT


//        internal override Brush GetCurrentCellBackground()
//        {
//            return this.GetGridTreeCurrentCellSelectionBackground(this.GetVisualStyle(VisualStyle));
//        }
//#endif

        internal IGridTreeVisualStyle GetVisualStyle(VisualStyle style)
        {
            switch (style)
            {
                case VisualStyle.Default:
                    return new GridTreeDefaultGridVisualStyle();

                case VisualStyle.Office2007Blue:
                    return new GridTreeOffice2007BlueVisualStyle();

                case VisualStyle.Office2007Silver:
                    return new GridTreeOffice2007SilverVisualStyle();

                case VisualStyle.Office2007Black:
                    return new GridTreeOffice2007BlackVisualStyle();

                case VisualStyle.Blend:
                    return new GridTreeBlendVisualStyle();

                case VisualStyle.GlassyGreen:
                    return new GridTreeGlassyGreenVisualStyle();

                case VisualStyle.SunBlack:
                    return new GridTreeSunBlackVisualStyle();

                case VisualStyle.ShinyRed:
                    return new GridTreeShinyRedVisualStyle();

                case VisualStyle.ShinyBlue:
                    return new GridTreeShinyBlueVisualStyle();

                case VisualStyle.BureauBlue:
                    return new GridTreeBureauBlueVisualStyle();

                case VisualStyle.TwilightBlue:
                    return new GridTreeTwilightBlueVisualStyle();

                case VisualStyle.Office14Blue:
                    return new GridTreeOffice14BlueVisualStyle();

                case VisualStyle.Office14Black:
                    return new GridTreeOffice14BlackVisualStyle();

                case VisualStyle.Office14Silver:
                    return new GridTreeOffice14SilverVisualStyle();

                case VisualStyle.VS2010:
                    return new GridTreeVS2010VisualStyle();

                case VisualStyle.Metro:
                    return new GridTreeMetroVisualStyle();

                case VisualStyle.Windows7:
                    return new GridTreeWindows7VisualStyle();

                case VisualStyle.SyncfusionTheme:
                    return new GridTreeSyncfusionVisualStyle();
                case VisualStyle.Custom:
                    return this.CustomVisualStyle;

            }

            return new GridTreeDefaultGridVisualStyle();
        }

        private void ApplyGridVisualStyle(IGridTreeVisualStyle value)
        {
            if (!this.SupportsVisualStyles)
            {
                this.ColumnHeaderStyle = null;
                this.InvalidateCells();
                return;
            }
            if (!this.EnableLegacyStyle)
            {
                this.Model.Options.HighlightSelectionBackground = this.GetGridTreeHighlightSelectionBackground(value);
                this.Model.Options.HighlightSelectionForeground = this.GetGridTreeHighlightSelectionForeground(value);
            }

            this.ColumnHeaderStyle.Background = this.GetGridTreeHeaderBackgroundBrush(value);
            this.ColumnHeaderStyle.Foreground = this.GetGridTreeHeaderForegroundBrush(value);

            this.ColumnHeaderStyle.Font = this.GetGridTreeHeaderFont(value);
            this.ColumnHeaderStyle.TextMargins = this.GetGridTreeHeaderTextMargins(value);
            this.ColumnHeaderStyle.VerticalAlignment = VerticalAlignment.Center;

            this.RowHeaderStyle.Background = this.GetGridTreeRowHeaderBackgroundBrush(value);
            this.RowHeaderStyle.Foreground = this.GetGridTreeRowHeaderForegroundBrush(value);
            this.RowHeaderStyle.Font = this.GetGridTreeHeaderFont(value);
            this.RowHeaderStyle.TextMargins.Left = 3;
            this.RowHeaderStyle.VerticalAlignment = VerticalAlignment.Center;

            var CellBorders = this.GetGridTreeCellBorders(value);

            if (CellBorders != null)
            {
#if !SILVERLIGHT
                this.ApplyBorders(CellBorders);
#else

                this.Model.TableStyle.Borders = this.GetGridTreeCellBorders(this.GetVisualStyle(VisualStyle));
                //double w = this.VisualStyle != VisualStyle.Default && this.VisualStyle != VisualStyle.Office2007Black
                //            && this.VisualStyle != VisualStyle.Blend ? 2 * penWidth : penWidth;
                //Pen p2 = new Pen((this.GetGridTreeCellBorders(value) == null || this.GetGridTreeCellBorders(value).Bottom == null) ?
                //                                    Brushes.Black : this.GetGridTreeCellBorders(value).Bottom.Brush, w);
                //this.Model.TableStyle.Borders.All = p2;// value.ValueCellBorders;
#endif
            }

            Model.Options.CurrentCellBorderWidth = this.GetGridTreeCurrentCellBorderWidth(value);
            Model.Options.CurrentCellBorder = this.GetGridTreeCurrentCellBorderBrush(value);

            Model.TableStyle.TextMargins = this.GetGridTreeCellTextMargins(value);
            Model.TableStyle.Font = this.GetGridTreeCellFont(value);
            Model.TableStyle.Background = this.GetGridTreeCellBackgroundBrush(value);
            Model.TableStyle.Foreground = this.GetGridTreeCellForegroundBrush(value);
            if (this.ParentTreeControl is GridTreeControl)
            {
                this.ParentTreeControl.Background = this.GetGridTreeCellBackgroundBrush(value);
                if (!this.ParentTreeControl.BorderThickness.Equals(new Thickness(0)))
                {
                    this.ParentTreeControl.BorderBrush = this.GetGridTreeBorderBrush(value);
                    this.ParentTreeControl.BorderThickness = this.GetGridTreeBorderThickness(value);
                }
            }

#if !SILVERLIGHT
            var sortRenderer = this.CellRenderers[SortHeaderCellType] as GridCellSortHeaderRenderer;
            if (sortRenderer != null)
            {
                sortRenderer.SortWidgetBrush = this.GetGridTreeSortWidgetBrush(value);
            }
            var headerRenderer = this.CellRenderers[SortHeaderCellType] as GridTreeHeaderCellRenderer;
            if (headerRenderer != null)
            {
                headerRenderer.VisualStyle = value;
            }
            var expandRendererExt = this.CellRenderers["ExpanderCell"] as GridTreeExpanderCellRendererExt;
            if (expandRendererExt != null)
            {
                expandRendererExt.VisualStyle = value;
            }
            var expandRenderer = this.CellRenderers["ExpanderCell"] as GridTreeExpandCellRenderer;
            if (expandRenderer != null)
            {
                expandRenderer.VisualStyle = value;
            }
#else
            var headerRenderer = this.CellRenderers[SortHeaderCellType] as GridTreeHeaderCellRenderer;
            if (headerRenderer != null)
            {
                headerRenderer.VisualStyle = value;
            }

            //set the GridTreeExpanderCellRenderer VisualStyle from GridTreeControl VisualStyle
            var expandRenderer = this.CellRenderers["ExpanderCell"] as GridTreeExpanderCellRendererExt;
            if (expandRenderer != null)
            {
                expandRenderer.VisualStyle = value;
            }
#endif

            #region OldMarkRowBrush


            //            if (this.VisualStyle != VisualStyle.Office2007Blue && this.VisualStyle != VisualStyle.Office2003)
            //            {
            //                GradientStopCollection stops = new GradientStopCollection()
            //                        {
            //#if SILVERLIGHT
            //                            new GradientStop(){ Color=Color.FromArgb(0xFF, 0x95, 0x93, 0xB2) , Offset=0 },
            //                            new GradientStop(){ Color=Color.FromArgb(0xFF, 0xC1, 0xC2, 0xCC) , Offset=.4 },
            //                            new GradientStop(){ Color=Color.FromArgb(0xFF, 0xC1, 0xC2, 0xCC) , Offset=.6 },
            //                            new GradientStop(){ Color=Color.FromArgb(0xFF, 0x95, 0x93, 0xB2) , Offset=1 }
            //#else
            //                            new GradientStop(Color.FromArgb(0xFF, 0x95, 0x93, 0xB2), 0),
            //                            new GradientStop(Color.FromArgb(0xFF, 0xC1, 0xC2, 0xCC), .4),
            //                            new GradientStop(Color.FromArgb(0xFF, 0xC1, 0xC2, 0xCC), .6),
            //                            new GradientStop(Color.FromArgb(0xFF, 0x95, 0x93, 0xB2), 1)
            //#endif
            //                        };
            //#if SILVERLIGHT
            //                MarkRowBrush = new LinearGradientBrush() { GradientStops = stops, StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
            //#else
            //                MarkRowBrush = new LinearGradientBrush(stops, new Point(0, 0), new Point(0, 1));
            //#endif
            //            }
            //            else
            //            {
            //                //MarkRowBrush = value.HighlightSelectionBackground;//.HeaderBackgroundBrush;
            //                GradientStopCollection stops = new GradientStopCollection()
            //                        {
            //#if SILVERLIGHT
            //                            new GradientStop(){ Color=Color.FromArgb(0xFF, 83, 139, 208) , Offset=0 },
            //                            new GradientStop(){ Color=Color.FromArgb(0xFF, 176, 210, 255) , Offset=.4 },
            //                            new GradientStop(){ Color=Color.FromArgb(0xFF, 176, 210, 255) , Offset=.6 },
            //                            new GradientStop(){ Color=Color.FromArgb(0xFF, 83, 139, 208) , Offset=1 }
            //#else
            //                            new GradientStop(Color.FromArgb(0xFF, 83, 139, 208), 0),
            //                            new GradientStop(Color.FromArgb(0xFF, 176, 210, 255), .4),
            //                            new GradientStop(Color.FromArgb(0xFF, 176, 210, 255), .6),
            //                            new GradientStop(Color.FromArgb(0xFF, 83, 139, 208), 1)
            //#endif
            //                        };
            //#if SILVERLIGHT
            //                MarkRowBrush = new LinearGradientBrush() { GradientStops = stops, StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
            //#else
            //                MarkRowBrush = new LinearGradientBrush(stops, new Point(0, 0), new Point(0, 1));
            //#endif
            //            }
            #endregion

            //lines
            var _expanderForeground = this.GetGridTreeExpanderBorderBrush(value);
            Pen p = new Pen((_expanderForeground == null) ?
                                                    Brushes.Black : _expanderForeground, penWidth);

            SetExpandBrushesAndPen(this.GetGridTreeExpanderBackground(value), this.GetGridTreeExpanderHoverBackground(value), p);

            this.InvalidateCells();
        }

        #region VisualStyles


        internal Thickness GetGridTreeBorderThickness(IGridTreeVisualStyle value)
        {
            return value.GridTreeBorderThickness;
        }

        internal Brush GetGridTreeBorderBrush(IGridTreeVisualStyle value)
        {

            return value.GridTreeBorderBrush;
        }

        internal Brush GetGridTreeHeaderBackgroundBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.HeaderBackgroundBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.HeaderBackgroundBrush;
            }

            return value.GridTreeHeaderBackgroundBrush;
        }

        internal Brush GetGridTreeHeaderForegroundBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.HeaderForegroundBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.HeaderForegroundBrush;
            }

            return value.GridTreeHeaderForegroundBrush;
        }

        internal Brush GetGridTreeHeaderHoverBackgroundBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.HeaderHoverBackgroundBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.HeaderHoverBackgroundBrush;
            }

            return value.GridTreeHeaderHoverBackgroundBrush;
        }

        internal Brush GetGridTreeSortWidgetBorderBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.SortWidgetBorderBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.SortWidgetBorderBrush;
            }

            return value.GridTreeSortWidgetBorderBrush;
        }

        internal Brush GetGridTreeSortWidgetBorderHoverBackgroundBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.SortWidgetBorderHoverBackgroundBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.SortWidgetBorderHoverBackgroundBrush;
            }

            return this.GetVisualStyle(VisualStyle).GridTreeSortWidgetBorderHoverBackgroundBrush;
        }

        //internal Brush GetGridTreeHeaderInnerBorderBrush(IGridTreeVisualStyle value)
        //{
        //    if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
        //        && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance != null
        //        && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.HeaderInnerBorderBrush != null)
        //    {
        //        return (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.HeaderInnerBorderBrush;
        //    }

        //    return value.GridTreeHeaderInnerBorderBrush;
        //}

        //internal Thickness GetGridTreeHeaderInnerBorderThickness(IGridTreeVisualStyle value)
        //{
        //    if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
        //        && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance != null
        //        && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.HeaderInnerBorderThickness != null)
        //    {
        //        return (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.HeaderInnerBorderThickness;
        //    }

        //    return value.GridTreeHeaderInnerBorderThickness;
        //}

        internal Brush GetGridTreeHeaderHoverForegroundBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.HeaderHoverForegroundBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.HeaderHoverForegroundBrush;
            }

            return value.GridTreeHeaderHoverForegroundBrush;
        }

        internal GridFontInfo GetGridTreeHeaderFont(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.HeaderFont != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.HeaderFont;
            }

            return value.GridTreeHeaderFont;
        }

        internal CellMarginsInfo GetGridTreeHeaderTextMargins(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.HeaderTextMargins != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.HeaderTextMargins;
            }

            return value.GridTreeHeaderTextMargins;
        }

        internal Brush GetGridTreeSortWidgetBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.SortWidgetBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.HeaderAppearance.SortWidgetBrush;
            }

            return value.GridTreeSortWidgetBrush;
        }

        internal Brush GetGridTreeHighlightSelectionBackground(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.HighlightSelectionBackground != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.HighlightSelectionBackground;
            }

            return value.GridTreeHighlightSelectionBackground;
        }

        internal Brush GetGridTreeHighlightSelectionForeground(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.HighlightSelectionForeground != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.HighlightSelectionForeground;
            }

            return value.GridTreeHighlightSelectionForeground;
        }

        internal Brush GetGridTreeCurrentCellSelectionBackground(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.CurrentCellSelectionBackground != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.CurrentCellSelectionBackground;
            }

            return value.GridTreeCurrentCellSelectionBackground;
        }

        internal Brush GetGridTreeRowHeaderBackgroundBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.RowHeaderBackgroundBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.RowHeaderBackgroundBrush;
            }

            return value.GridTreeRowHeaderBackgroundBrush;
        }

        internal Brush GetGridTreeRowHeaderForegroundBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.RowHeaderForegroundBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.RowHeaderForegroundBrush;
            }

            return value.GridTreeRowHeaderForegroundBrush;
        }

        internal Brush GetGridTreeRowHoverBackgroundBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.RowHoverBackgroundBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.RowHoverBackgroundBrush;
            }

            return value.GridTreeRowHoverBackgroundBrush;
        }


        internal Brush GetGridTreeRowHoverForegroundBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.RowHoverForegroundBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.RowHoverForegroundBrush;
            }

            return value.GridTreeRowHoverForegroundBrush;
        }

        internal Brush GetGridTreeCurrentCellSelectionForeground(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.CurrentCellSelectionForeground != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.CurrentCellSelectionForeground;
            }

            return value.GridTreeCurrentCellSelectionForeground;
        }



        internal Brush GetGridTreeCurrentCellBorderBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.CurrentCellBorderBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.CurrentCellBorderBrush;
            }

            return value.GridTreeCurrentCellBorderBrush;
        }

        internal double GetGridTreeCurrentCellBorderWidth(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance != null)
                // && (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.CurrentCellBorderWidth != null) double is never equal to 'null'
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.RowAppearance.CurrentCellBorderWidth;
            }

            return value.GridTreeCurrentCellBorderWidth;
        }


#if !SILVERLIGHT
        internal Geometry GetGridTreeExpanderPlusPath(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderPlusPath != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderPlusPath;
            }

            return value.GridTreeExpanderPlusPath;
        }

        internal Geometry GetGridTreeExpanderMinusPath(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderMinusPath != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderMinusPath;
            }

            return value.GridTreeExpanderMinusPath;
        }
#else
        internal System.Windows.Shapes.Path GetGridTreeExpanderPlusPath(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderPlusPath != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderPlusPath;
            }

            return value.GridTreeExpanderPlusPath;
        }

        internal System.Windows.Shapes.Path GetGridTreeExpanderMinusPath(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderMinusPath != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderMinusPath;
            }

            return value.GridTreeExpanderMinusPath;
        }
#endif


        internal Brush GetGridTreeExpanderBackground(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderBackground != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderBackground;
            }

            return value.GridTreeExpanderBackground;
        }

        internal Brush GetGridTreeExpanderBorderBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderBorderBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderBorderBrush;
            }

            return value.GridTreeExpanderBorderBrush;
        }





        internal Brush GetGridTreeExpanderExpandedBackground(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderExpandedBackground != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderExpandedBackground;
            }

            return value.GridTreeExpanderExpandedBackground;
        }

        internal Brush GetGridTreeExpanderExpandedBorderBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderExpandedBorderBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderExpandedBorderBrush;
            }

            return value.GridTreeExpanderExpandedBorderBrush;
        }





        internal Brush GetGridTreeExpanderHoverBackground(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderHoverBackground != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderHoverBackground;
            }

            return value.GridTreeExpanderHoverBackground;
        }

        internal Brush GetGridTreeExpanderHoverBorderBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderHoverBorderBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.ExpanderAppearance.ExpanderHoverBorderBrush;
            }

            return value.GridTreeExpanderHoverBorderBrush;
        }



        internal CellBordersInfo GetGridTreeCellBorders(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.CellAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.CellAppearance.CellBorders != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.CellAppearance.CellBorders;
            }

            return value.GridTreeCellBorders;
        }

        internal GridFontInfo GetGridTreeCellFont(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.CellAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.CellAppearance.CellFont != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.CellAppearance.CellFont;
            }

            return value.GridTreeCellFont;
        }

        internal CellMarginsInfo GetGridTreeCellTextMargins(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.CellAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.CellAppearance.CellTextMargins != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.CellAppearance.CellTextMargins;
            }

            return value.GridTreeCellTextMargins;
        }

        internal Brush GetGridTreeCellBackgroundBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.CellAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.CellAppearance.CellBackgroundBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.CellAppearance.CellBackgroundBrush;
            }

            return value.GridTreeCellBackgroundBrush;
        }

        internal Brush GetGridTreeCellForegroundBrush(IGridTreeVisualStyle value)
        {
            if (this.ParentTreeControl is GridTreeControl && (this.ParentTreeControl as GridTreeControl).StyleManager != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.CellAppearance != null
                && (this.ParentTreeControl as GridTreeControl).StyleManager.CellAppearance.CellForegroundBrush != null)
            {
                return (this.ParentTreeControl as GridTreeControl).StyleManager.CellAppearance.CellForegroundBrush;
            }

            return value.GridTreeCellForegroundBrush;
        }

        #endregion

#if !SILVERLIGHT
        private void ApplyBorders(CellBordersInfo cellBordersInfo)
        {
            CellBordersInfo temp = new CellBordersInfo();
            temp.CopyFrom(this.Model.TableStyle.Borders);

            this.Model.TableStyle.Borders = cellBordersInfo;

            if (this.Model.TableStyle.IsLeftBorderChanged)
            {
                this.Model.TableStyle.Borders.Left = temp.Left;
            }

            if (this.Model.TableStyle.IsTopBorderChanged)
            {
                this.Model.TableStyle.Borders.Top = temp.Top;
            }

            if (this.Model.TableStyle.IsRightBorderChanged)
            {
                this.Model.TableStyle.Borders.Right = temp.Right;
            }

            if (this.Model.TableStyle.IsBottomBorderChanged)
            {
                this.Model.TableStyle.Borders.Bottom = temp.Bottom;
            }
        }
#endif
        private double penWidth = 0.5;
        #endregion
    }
}
