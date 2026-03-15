#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Specialized;
using System.Windows.Threading;

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
#if !SILVERLIGHT
    using System.Data;
#else
    using System.Reflection;
#endif
    using System.Linq;
    using System.Text;
    using Syncfusion.Linq;
    using Syncfusion.Windows.Collections;
    using Syncfusion.Windows.Collections.Generic;
    using Syncfusion.Windows.ComponentModel;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Data;
    using System.Windows.Controls;

    /// <summary>
    /// Provides the necessary extended details for the records structure in <see cref="GridDataControl"/>.
    /// </summary>
    public class GridDataRecord : RecordEntry
    {
        #region Private member

        bool _isDetailsViewExpanded;
        private System.Windows.DataTemplate detailsViewTempalte;

        #endregion
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataRecord"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="parentTable">The parent table.</param>
        public GridDataRecord(object data, GridDataTable parentTable)
            : base(null, -1, data)
        {
            this.Table = parentTable;
            this.ChildModels = new Dictionary<int, GridDataChildTableModel>();
            this.errorList = new Dictionary<string, GridDataErrorInfo>();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.DetailsViewDataContext = null;
                this.DetailsViewTemplate = null;
                this.DetailsViewCellSize = System.Windows.Size.Empty;
                if (this.ChildModels != null)
                {
                    foreach (var item in ChildModels)
                    {
                        item.Value.ChildGridSelectedItems.Clear();
                        item.Value.canDisposeGrid = false;
                        item.Value.Dispose();
                        item.Value.canDisposeGrid = true;
                    }
                }
                if (this.errorList != null)
                {
                    this.errorList.Clear();
                    this.errorList = null;
                }
                if (this.Table != null)
                {
                    this.Table = null;
                }
#if !SILVERLIGHT
                if (childSource != null)
                {
                   if(childSource is INotifyCollectionChanged)
                       (childSource as INotifyCollectionChanged).CollectionChanged -= notifyCollectionchanged_CollectionChanged;
                }
                if (iBindingList != null)
                {
                    iBindingList.ListChanged -= GridDataRecord_ListChanged;
                }
#endif 
            }
        }

        /// <summary>
        /// Gets the child group, this would be nested table instance.
        /// </summary>
        /// <value>The child group.</value>
        public Dictionary<int, GridDataChildTableModel> ChildModels
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the <see cref="GridDataTable"/> instance.
        /// </summary>
        /// <value>The table.</value>
        public GridDataTable Table
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public GridDataTableModel Model
        {
            get
            {
                return this.Table.Model;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrors
        {
            get
            {
                return ErrorList.Count > 0;
            }
        }

        private Dictionary<string, GridDataErrorInfo> errorList;
        /// <summary>
        /// Gets the error list.
        /// </summary>
        /// <value>The error list.</value>
        public Dictionary<string, GridDataErrorInfo> ErrorList
        {
            get
            {
                return this.errorList;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is details view expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is details view expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsDetailsViewExpanded
        {
            get
            {
               return _isDetailsViewExpanded;
            }
            set
            {
                if (_isDetailsViewExpanded != value)
                {
                    _isDetailsViewExpanded = value;

                    if (value)
                    {
                        this.ExpandDeatilsViewUI();
                    }
                    else
                    {
                        this.CollapseDetailsViewUI();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the details view cell.
        /// </summary>
        /// <value>The size of the details view cell.</value>
        internal System.Windows.Size DetailsViewCellSize { get; set; }

        /// <summary>
        /// Gets or sets the details view template.
        /// </summary>
        /// <value>The details view template.</value>
        internal System.Windows.DataTemplate DetailsViewTemplate
        {
            get
            {
                return detailsViewTempalte;
            }
            set
            {
                if (detailsViewTempalte != value && value != null)
                {
                    /// Getting the sized of the the cell by appying the template in dummy control.
                    ContentControl contentControl = new ContentControl();
                    //contentControl.ContentTemplate = this.DetailsViewTemplate;
                    contentControl.Content = value.LoadContent();
                    contentControl.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
                    this.DetailsViewCellSize = contentControl.DesiredSize;
                }

                detailsViewTempalte = value;
            }
        }

        /// <summary>
        /// Gets or sets the details view data context.
        /// </summary>
        /// <value>The details view data context.</value>
        internal object DetailsViewDataContext { get; set; }

        /// <summary>
        /// Collapses the details view.
        /// </summary>
        private void CollapseDetailsViewUI()
        {
            if (this.Model.Table.RaiseGridDetailsViewCollapsingEvent(this))
            {
                return;
            }

            var rowIdx = 0;
            if (this.Model.Table.HasGroups)
                rowIdx = this.Model.View.TopLevelGroup.DisplayElements.IndexOf((RecordEntry)this);
            else
                rowIdx = this.Model.View.Records.IndexOf(this);
            
            rowIdx = this.Model.ResolvePositionToIndex(rowIdx) + 1;

            this.Model.RowHeights.SetHidden(rowIdx, rowIdx, true);
            this.Model.RowHeights.SetNestedLines(rowIdx, null);
            //This is necessary to avoid issue on dynamically removing the grouping.
            this.Model.DetailsViewRows.Remove(rowIdx);

            if (this.Model.IsInSort)
            {
                //To unload the cell on collapsing the row while applying sorting.
                this.Model.Grid.ArrangedCellUIElements.Unload(new RowColumnIndex(rowIdx, this.Model.ResolveDefaultColumnOffset()));
            }

            //Refreshing the Grid
            this.Model.Table.Model.RefreshParentGrids();
            this.Model.Table.RootModel.InvalidateVisual(true);
            if (this.Model.Table.NeedsInvalidate)
                if (this.Model.Grid != null)
                    this.Model.Grid.InvalidateMeasure();
            //Raising collapsed event
            this.Table.RaiseGridDetailsViewCollapsedEvent(this);
        }

        /// <summary>
        /// Expands the deatils view.
        /// </summary>
        private void ExpandDeatilsViewUI()
        {
            //Preserving the existing data and template
            this.DetailsViewTemplate = this.Model.TableProperties.DetailsViewTemplate;
            this.DetailsViewDataContext = this.Data;

            //Raising the expanding event
            var expandingArgs = this.Model.Table.RaiseGridDetailsViewExpandingEvent(this);

            if (expandingArgs.Cancel)
            {
                return;
            }

            int rowIdx = 0;
            if (this.Model.Table.HasGroups)
            {
                //Resolving the row index on gropped grid
                rowIdx = this.Model.View.TopLevelGroup.DisplayElements.IndexOf((RecordEntry)this);
                rowIdx = this.Model.ResolvePositionToIndex(rowIdx) + 1;
                GridDataRecord tempRecord = null;

                if (!this.Model.DetailsViewRows.TryGetValue(rowIdx, out tempRecord))
                    this.Model.DetailsViewRows.Add(rowIdx, this);
                else
                    this.Model.DetailsViewRows[rowIdx] = this;
            }
            else
            {
                rowIdx = this.Model.View.Records.IndexOf(this);
                rowIdx = this.Model.ResolvePositionToIndex(rowIdx) + 1;
            }
            
            this.Model.RowHeights.SetHidden(rowIdx, rowIdx, false);
            //Default padding in top and bottom is 12.5, hence extra 25 is added to height
            this.Model.RowHeights.SetNestedLines(rowIdx, new Scroll.LineSizeCollection { LineCount = 1, DefaultLineSize = DetailsViewCellSize.Height + 25 });

            this.Model.AdjustModelWidth(DetailsViewCellSize.Width);

            //Refreshing the Grid.
            this.Model.RefreshParentGrids();

            this.Model.Table.RootModel.InvalidateVisual(true);

            //Raising the expanded event
            this.Table.RaiseGridDetailsViewExpandedEvent(expandingArgs);
        }

        /// <summary>
        /// Called when RecordEntry.IsExpanded = true;
        /// </summary>
        protected override void OnExpanded()
        {
            this.Model.ExpandedRecordCount++;
            if (!this.Table.HasNestedTables)
            {
                return;
            }

            //int level = 0;
            //bool flag = false;
            if (this.ChildViews == null)
            {
                this.ChildViews = new Dictionary<string, NestedRecordEntry>();
            }
            //foreach (var relation in this.Model.TableProperties.Relations)
            //{
            //    level++;
            //    this.PopulateChildView((IRelationDefinition)relation, this.Model.View.GetItemProperties(), level, this.Model.IsLegacyDataTable);
            //    flag = true;
            //}
            //if (flag)
            //{
            this.SetExpandedUI();
            //}
        }

        internal void SetExpandedUI()
        {
            int rowIdx = 0;
            if (this.Model.Table.HasGroups)
            {
                rowIdx = this.Model.View.TopLevelGroup.DisplayElements.IndexOf((RecordEntry)this);
            }
            else
            {
                rowIdx = this.Model.View.Records.IndexOf(this);
            }
            var actualRowIdx = this.Model.ResolvePositionToIndex(rowIdx);
            var colIdx = 0;
            //var colIdx = this.Model.TableProperties.ShowRowHeader ? 1 : 0;
            //colIdx = this.Model.TableProperties.ShowRecordPlusMinus ? colIdx + 1 : colIdx;

            //// if we have any nested tables, get the indented value
            //var indentedValue = 0;
            //this.Model.GetNestedTableIndentedValue(ref indentedValue);
            //colIdx = indentedValue > 0 ? colIdx + indentedValue : colIdx;
            this.SetExpandedUI(rowIdx, actualRowIdx, colIdx);

        }

        internal void SetExpandedUI(int rowIdx, int actualRowIdx, int actualColIdx)
        {
            var parentTableProperties = this.Model.TableProperties;
            if (this.Model.Table.HasNestedTables)
            {

                var args = this.Model.Table.RaiseGridRecordExpandingEvent(this);

                if (args.Cancel)
                {
                    return;
                }

                this.SetExpanded();

                // Invalidate the +- cell
                this.Model.InvalidateCell(GridRangeInfo.Cell(actualRowIdx, !this.Table.HasGroups ? (this.Model.TableProperties.ShowRowHeader ? 1 : 0) : this.Model.ResolveDefaultColumnOffset() - 1));

                foreach (var rd in parentTableProperties.Relations)
                {
                    //var collectionview = this.ChildViews[rd.RelationalColumn].View;
                    //if (collecti+onview != null)
                    //{
                    rd.TableProperties.VisualStyle = this.Model.TableProperties.VisualStyle;
                    rd.TableProperties.EnableLegacyStyle = this.Model.TableProperties.EnableLegacyStyle;
                    GridDataChildTableModel childModel = null;

                    /// insert a new line with the grid, since we need to insert a line after the currentRowIndex add '1'
                    /// if grid has details view the first relation should appear in second row from the record row, other consecutive
                    /// relations will appear one by, hence the incremet for 1'st relation is 2 and other as 1
                    if (parentTableProperties.Relations.IndexOf(rd) == 0)
                        actualRowIdx += this.Table.HasDetailsView ? 2 : 1;
                    else
                        actualRowIdx += 1;

                    // order key would return with start value as 1, we simply subtract it so that we add it to the dictionary in order of 0,1,2..
                    var key = this.Model.GetOrderForChildTableBasedOnIndex(actualRowIdx);

                    /// This increment is based on the logic used in the get order of child, when the record is in detail view it will consider 
                    /// the detail view row as record row and retun the key value as less than 1, hence it is increment here. 
                    key += this.Model.Table.HasGroups && this.IsDetailsViewExpanded ? 1 : 0;
                    if (args.ChildItemsSource != null)
                    {
                        this.ChildModels.Remove(key - 1);
                        this.ChildViews.Remove(this.Model.TableProperties.Relations[key-1].RelationalColumn);
                    }
                    if (!this.ChildModels.ContainsKey(key - 1))
                    {
                        childModel = new GridDataChildTableModel(rd);
#if !SILVERLIGHT
                        childModel.TableStyle.FlowDirection = this.Model.TableStyle.FlowDirection;
                        childModel.EnableContextMenu = this.Model.EnableContextMenu;
#endif

                        childModel.Options.HighlightSelectionBackground = this.Model.Options.HighlightSelectionBackground;
                        childModel.Options.HighlightSelectionForeground = this.Model.Options.HighlightSelectionForeground;
                        childModel.Options.ShowErrorIconOnEditing = this.Model.Options.ShowErrorIconOnEditing;
                        // set the sync properties
                        childModel.ParentRecord = this;
                        childModel.TableProperties = new GridDataTableProperties();
                       // childModel.TableProperties.ColumnSizer = rd.TableProperties.ColumnSizer;
                        childModel.TableProperties.SuspendEvents();
                        if (this.Model.TableProperties.StyleManager != null && rd.TableProperties.StyleManager == null)
                            childModel.TableProperties.StyleManager = this.Model.TableProperties.StyleManager;
                        else
                            childModel.TableProperties.StyleManager = rd.TableProperties.StyleManager;

                        if (this.Model.TableProperties.HeaderStyle != null && rd.TableProperties.HeaderStyle == null)
                            childModel.TableProperties.HeaderStyle = this.Model.TableProperties.HeaderStyle;

                        if (parentTableProperties.AutoPopulateRelations)
                        {
                            childModel.TableProperties.InitializeFromInternal(parentTableProperties, false);
                        }
                        else
                        {
                            childModel.TableProperties.InitializeFrom(rd.TableProperties);
                        }
#if !SILVERLIGHT
                        childModel.TableProperties.ContextMenuOptions = this.Model.TableProperties.ContextMenuOptions;
                        childModel.TableProperties.HeaderContextMenuItems = this.Model.TableProperties.HeaderContextMenuItems;
                        childModel.TableProperties.RecordContextMenuItems = this.Model.TableProperties.RecordContextMenuItems;
#endif
                        childModel.TableProperties.ShowErrorTooltips = this.Model.TableProperties.ShowErrorTooltips;
                        childModel.TableProperties.ShowTooltips = this.Model.TableProperties.ShowTooltips;

                        childModel.TableProperties.ResumeEvents();
                        this.Model.ChildTableModelCollection.Add(childModel);
                        IEnumerable source = null;

                        // add the key based on the order. This would be easy to calculate when QueryCellInfo is called for rendering
#if !SILVERLIGHT
                        if (args.ChildItemsSource == null)
                        {
                            source = this.GetChildSource(rd.RelationalColumn, this.Model.IsLegacyDataTable, this.Model.View);
                        }
                        else if (args.ChildItemsSource != null && args.Handled)
                        {
                            source = args.ChildItemsSource as IEnumerable;
                        }
#else
                        if (args.ChildItemsSource == null)
                        {
                            source = this.GetChildSource(rd.RelationalColumn, false, this.Model.View);
                        }
                        else if (args.ChildItemsSource != null && args.Handled)
                        {
                            source = args.ChildItemsSource as IEnumerable;
                        }
#endif
                        if (source != null)
                        {
#if !SILVERLIGHT
                            if (source is DataView)
                                childModel.RowFilter = (source as DataView).RowFilter;
#endif                            
                            var typedSource = this.Model.GetSourceList(source);
                            var collectionview = childModel.CreateCollectionViewAdv(typedSource);
#if !SILVERLIGHT
                            if(this.Model.Grid != null)
                                (collectionview as CollectionViewAdv).DispatchOwner = this.Model.Grid.Dispatcher;
#endif
                            if (collectionview.Records.Count == 0 && childModel.TableProperties.HideEmptyChildGrid)
                            {
                                continue;
                            }
                            this.PopulateChildView(collectionview, key, rd.RelationalColumn);    
#if !SILVERLIGHT
                            childModel.SuspendEvents();
#endif
                            //this.PopulateChildView((IRelationDefinition)rd, this.Model.View.GetItemProperties(), key, this.Model.IsLegacyDataTable);                        

                            childModel.WireRelationalSourceList(collectionview);
                        }
                        //childModel.TableProperties.VisualStyle = this.Model.TableProperties.VisualStyle
                        //add parent table styles to child table
#if !SILVERLIGHT
                        if (this.Model.TableProperties.EnableParentStyleToChildTable)
                        {
                            childModel.TableStyle = this.Model.TableStyle;
                            childModel.RowHeights.DefaultLineSize = this.Model.RowHeights.DefaultLineSize;
                            childModel.TableProperties.EnableParentStyleToChildTable = this.Model.TableProperties.EnableParentStyleToChildTable;
                        }
#endif                     
                       // Below code was added to refresh the column width and rowheight when the child itemsource is null or empty.
                        if (childModel != null && source == null)
                        {
#if !SILVERLIGHT
                            if (childModel != null && childModel.ParentTable.Model.TableProperties.AllowNestedGridPadding)
                            {
                                childModel.RowHeights.PaddingDistance = 19;
                            }
#endif
                            childModel.RefreshColumns(true, false);
                        }
                        
                        this.ChildModels.Add(key - 1, childModel);
                    }
                    else
                    {
                        childModel = this.ChildModels[key - 1];
                        if (this.Model.TableProperties.OneTimePopulateRelations)
                        {
                            childModel.WireRelationalSourceList(childModel.View);
                        }
                    }
                    childModel.ChildGridSelectedItems = new ObservableCollection<object>();
                    this.Model.RowHeights.SetNestedLines(actualRowIdx, childModel.RowHeights);
                    this.Model.RowHeights.SetHidden(actualRowIdx, actualRowIdx, false);
                    
                    if (childModel.View != null && childModel.View.Records.Count != 0 || !childModel.ParentTable.Model.TableProperties.HideEmptyChildGrid)
                        this.Model.RowHeights.SetHidden(actualRowIdx, actualRowIdx, false);
                    else
                        this.Model.RowHeights.SetHidden(actualRowIdx, actualRowIdx, true);
                    // special case when we need to handle the nested table width adjusting issue
                    // also we have an extra unbound column to set the indenting space if the child table
                    this.Model.AdjustParents(childModel, false);

                    if (this.Model.Table.NeedsInvalidate)
                    {
                        //this.Table.Model.InvalidateVisual(true);
                        //this.Table.Model.Grid.InvalidateCells();
                        this.Model.Table.Model.RefreshParentGrids();
                    }
                    //}
                }

                if (this.Model.Table.NeedsInvalidate)
                {
                    this.Model.Table.RootModel.InvalidateVisual(true);
                }
                this.Model.Table.RaiseGridRecordExpandedEvent(this);

           
            }
        }

        /// <summary>
        /// Called when [collapsed].
        /// </summary>
        protected override void OnCollapsed()
        {
            if (!this.Table.HasNestedTables)
            {
                return;
            }

            this.SetCollapsedUI();
        }

        internal void SetCollapsedUI()
        {
            int rowIdx = 0;
            if (this.Model.Table.HasGroups)
            {
                rowIdx = this.Model.View.TopLevelGroup.DisplayElements.IndexOf((RecordEntry)this);
            }
            else
            {
                rowIdx = this.Model.View.Records.IndexOf(this);
            }
            var actualRowIdx = this.Model.ResolvePositionToIndex(rowIdx);
            var colIdx = this.Model.TableProperties.ShowRowHeader ? 1 : 0;
            colIdx = this.Model.TableProperties.ShowRecordPlusMinus ? colIdx + 1 : colIdx;

            // if we have any nested tables, get the indented value
            var indentedValue = 0;
            this.Model.GetNestedTableIndentedValue(ref indentedValue);
            colIdx = indentedValue > 0 ? colIdx + indentedValue : colIdx;
            this.SetCollapsedUI(rowIdx, actualRowIdx, colIdx);
        }

        internal void SetCollapsedUI(int rowIdx, int actualRowIdx, int actualColIdx)
        {
            if (this.IsExpanded && this.ChildModels.Count > 0)
            {
                if (!this.Model.Table.RaiseGridRecordCollapsingEvent(this))
                {
                    return;
                }

                for (int i = 0; i < this.ChildModels.Count; i++)
                {
                    //adjusting the row index based on the existance of details view.
                    actualRowIdx += Table.HasDetailsView && i == 0 ? 2 : 1;
                    this.Model.RowHeights.SetHidden(actualRowIdx, actualRowIdx, true);
                    this.Model.RowHeights.SetNestedLines(actualRowIdx, null);
                    var childModel = this.ChildModels[i];
                    foreach (GridControlBase childGrid in childModel.Views)
                    {
                        if (childGrid != null && childGrid.CurrentCell != null)
                        {
                            if(childGrid.CurrentCell.IsEditing)        // When we edit some value in add new row and click on collapse cell the current cell does not cancel the editied value.
                                childGrid.CurrentCell.CancelEdit();    // since we do not commit the value the modified cell value should not still appear in the add new row cell. 
                            childGrid.CurrentCell.Deactivate();
                            childModel.SelectedRanges.Clear();

#if SILVERLIGHT
                            var headerCellControls = childGrid.FindElementsOfType<GridDataHeaderCellControl>();
                            foreach (var header in headerCellControls)
                            {
                                if (header != null)
                                {
                                    header.CloseFilterDropDown();
                                }
                            }
#endif
                        }
                    }
                    this.Model.AdjustParents(childModel, true);
                }

                var key = this.Model.GetOrderForChildTableBasedOnIndex(actualRowIdx);
                key += this.Model.Table.HasGroups && this.IsDetailsViewExpanded ? 1 : 0;
                var childTableModel = this.ChildModels[key - 1];
                this.Model.ChildTableModelCollection.Remove(childTableModel);
//                if (childTableModel.Grid != null)
//                {
//#if !SILVERLIGHT
//                    //For silverlight we are reusing the Grid so no need to dispose the Gird.
//                    this.Model.Grid.Dispatcher.BeginInvoke(new Action(childTableModel.Grid.Dispose), DispatcherPriority.ApplicationIdle);
//#endif
//                }
                childTableModel = null;

                this.SetCollapsed();

                // Invalidate the +- cell
                this.Model.InvalidateCell(GridRangeInfo.Cell(actualRowIdx, !this.Table.HasGroups ? (this.Model.TableProperties.ShowRowHeader ? 1 : 0) : this.Model.ResolveDefaultColumnOffset() - 1));


                // sometimes we don't want this to invalidate, in those cases InvalidateVisual(true) has to be called by the user
                if (this.Model.Table.NeedsInvalidate)
                {
                    // hide row
                    this.Model.InvalidateVisual(true);
                    // invalidate measure for the grid to reclaim the height
                    if (this.Model.Grid != null)
                        this.Model.Grid.InvalidateMeasure();
                }

                this.Model.Table.RefreshDeltaColumnWidth();
                this.Model.Table.RaiseGridRecordCollapsedEvent(this);
                this.Model.Table.Model.RefreshParentGrids();
                /* Note: In multipe-nested grid scenarios you will loose expansion
                 state of child records when you dispose here.
                var childModel = this.ChildModels[actualRowIdx];
                childModel.Dispose();
                childModel = null;
                this.ChildModels.Remove(actualRowIdx);*/
            }
        }
        private IEnumerable childSource = null;
#if !SILVERLIGHT
        private IBindingList iBindingList = null;
#endif
        internal IEnumerable GetChildSource(string relationName, bool isLegacyDataTable, ICollectionViewAdv view)
        {
#if !SILVERLIGHT
            if (!isLegacyDataTable)
            {
#endif
                var item = this.Data;
                if (item != null)
                {
                    var provider = view.GetPropertyAccessProvider();
                    if (provider != null)
                    {
                        childSource = provider.GetValue(this.Data, relationName) as IEnumerable;
#if !SILVERLIGHT
                        if (childSource != null && this.Model.TableProperties.HideEmptyChildGrid && !this.IsExpanded)
                        {
                            if (childSource is INotifyCollectionChanged)
                            {
                                var notifyCollectionchanged = childSource as INotifyCollectionChanged;
                                notifyCollectionchanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(notifyCollectionchanged_CollectionChanged);
                                notifyCollectionchanged.CollectionChanged += new NotifyCollectionChangedEventHandler(notifyCollectionchanged_CollectionChanged);
                            }
                        }
#endif
                        return childSource;
                    }

                    return null;
                }
#if !SILVERLIGHT
            }
            else
            {
                var drow = ((DataRowView)this.Data).Row;
                if (drow != null)
                {
                    var dataTable = drow.Table;
                    //PropertyDescriptorCollection pdc = this.Model.View.GetItemProperties();
                    string sort = string.Empty;

                    DataRelation r = dataTable.DataSet.Relations[relationName];
                    DataTable childTable = dataTable.DataSet.Relations[relationName].ChildTable;

                    if (childTable != null && this.Model.TableProperties.HideEmptyChildGrid && !this.IsExpanded)
                    {
                        iBindingList = childTable.AsDataView() as IBindingList;
                        iBindingList.ListChanged -= new ListChangedEventHandler(GridDataRecord_ListChanged);
                        iBindingList.ListChanged += new ListChangedEventHandler(GridDataRecord_ListChanged);
                    }
                    foreach (SortDescription sd in this.Model.View.SortDescriptions)
                    {
                        if (childTable.Columns[sd.PropertyName] != null)
                        {
                            sort += !string.IsNullOrEmpty(sort) ? ", " : string.Empty;//Get sort description for more than one group or multisort when it is nested tables.
                            sort += string.Format("{0}{1}", sd.PropertyName, sd.Direction == ListSortDirection.Ascending ? " ASC" : " DESC");
                        }
                    }
                    
                    // PropertyDescriptor pd = pdc[relationName];
                    object value = drow[r.ParentKeyConstraint.Columns[0].ColumnName];
                    string filter = string.Empty;
                    if (value is DateTime)
                    {
                        filter = string.Format("[{1}] = #{0}#", ((DateTime)value).ToString(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat), r.ChildKeyConstraint.Columns[0].ColumnName);
                    }
                    else if (value == null || value is DBNull)
                    {
                        filter = string.Format("[{1}] is null", value, r.ChildKeyConstraint.Columns[0].ColumnName);
                    }
                    else
                    {
                        filter = string.Format("[{1}] = '{0}'", value, r.ChildKeyConstraint.Columns[0].ColumnName);
                    }
                    //return new DataView(childTable, filter, sort, DataViewRowState.CurrentRows);
                    DataView dv = null;
                    DateTime start = DateTime.Now;
                    while (dv == null && DateTime.Now.Subtract(start).TotalSeconds < 10) //wait a max of 10 secs...
                    {
                        try
                        {
                            dv = new DataView(childTable, filter, sort, DataViewRowState.CurrentRows);
                        }
                        catch (Exception)
                        {
                            //Console.WriteLine("Exception eaten");
                        }
                    }
                    return dv;
                }

                return null;
            }
#endif
            return null;

        }
#if  !SILVERLIGHT
        void GridDataRecord_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded||e.ListChangedType==ListChangedType.ItemDeleted||e.ListChangedType==ListChangedType.Reset)
            {
                if (this.Model.Grid != null)
                {
                    this.Model.Grid.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            int index = !this.Table.HasGroups
                                            ? (this.Model.TableProperties.ShowRowHeader ? 1 : 0)
                                            : this.Model.ResolveDefaultColumnOffset() - 1;
                            this.Model.InvalidateCell(GridRangeInfo.Col(index));
                        }),DispatcherPriority.ApplicationIdle);
                }
            }
        }

        void notifyCollectionchanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if ((e.Action == NotifyCollectionChangedAction.Add && childSource.AsQueryable().Count() == 1) || (e.Action == NotifyCollectionChangedAction.Remove && childSource.AsQueryable().Count() == 0 )||(e.Action==NotifyCollectionChangedAction.Reset))
            {
                if (this.Model.Grid != null)
                {
                    this.Model.Grid.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        int index = !this.Table.HasGroups ? (this.Model.TableProperties.ShowRowHeader ? 1 : 0) : this.Model.ResolveDefaultColumnOffset() - 1;
                        this.Model.InvalidateCell(GridRangeInfo.Col(index));
                    }),DispatcherPriority.ApplicationIdle);
                }               
            }
        }
#endif
    }

    /// <summary>
    /// Provides the data for the error information held by <see cref="GridDataRecord"/>.
    /// </summary>
    public class GridDataErrorInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataErrorInfo"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="errormessage">The errormessage.</param>
        public GridDataErrorInfo(object value, string errormessage)
        {
            this.Value = value;
            this.ErrorMessage = errormessage;
        }

        /// <summary>
        /// Gets the value of the underlying object.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage
        {
            get;
            private set;
        }
    }
}
