#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using Syncfusion.PivotAnalysis.Base;
using Syncfusion.Licensing;
using Syncfusion.Windows.Forms.Grid;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using System.Windows.Markup;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel.Design;
using System.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows;
using Syncfusion.Windows.Forms.ComponentModel;
using System.Linq.Expressions;
using Syncfusion.Linq;
using Syncfusion.Windows.Forms.Scrolling;
using System.Drawing.Design;
using System.Data;


namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    /// <summary>
    /// PivotGridControl provides the functionality of a pivot table. A pivot table is a data summarization tool
    /// which automatically sorts, count and total the table data and represents in cross tabular format.
    /// </summary>
    /// <remarks>
    /// To use this control, you must set the <see cref="ItemSource"/> to some IList object.
    /// The object in the IList should have public property which you want to use it in the pivot table.
    /// To represent the data in cross tabular format you should specify the pivoting info in these 
    /// properties <see cref="PivotRows"/>, <see cref="PivotColumns"/> and <see cref="PivotCalculations"/>
    /// </remarks>
    
    public class PivotGridControlBase : GridControlBaseImp , IPivotControl
    {
        private Syncfusion.Windows.Forms.PivotAnalysis.RowGroupBar groupbar;
       
        #region [ Private Variables]

        /// <summary>
        /// FieldInfo collection for default fields
        /// </summary>
        private List<FieldInfo> allowedFields;

        /// <summary>
        /// PivotItem collection for PivotRows
        /// </summary>
        /// 
        ObservableCollection<PivotItem> _PivotRows;

        /// <summary>
        /// PivotItem collection for PivotColumns
        /// </summary>
        ObservableCollection<PivotItem> _PivotColumns;

        

        /// <summary>
        /// PivotCalculation collection
        /// </summary>
        ObservableCollection<PivotComputationInfo> _PivotCalculations;

        /// <summary>
        /// PivotItem collection for PivotFields
        /// </summary>
        ObservableCollection<PivotItem> _PivotFields;

        /// <summary>
        /// Filter expression collection
        /// </summary>
        /// _Filters is renamed to PivotFilters, it is because an underscore - 
        /// shouldn't start an identifier with a visible (public/protected) field- this violates CLS compliant 
        /// Declared as Public since it is used in GroupBar.cs
        public ObservableCollection<FilterExpression> PivotFilters;
        
        /// <summary>
        /// FilterItem collection for PivotFields
        /// </summary>
        //internal ObservableCollection<FilterItemsCollection> FilterItems;

        private bool _IsExtenalEngine;

        //private int m_AutoSizeColumnCount = 0;

        private PivotEngine engine = null;

#if SILVERLIGHT
        private double m_verticalOffset = 0;

        internal double m_actualVerticalOffset = 0;
#endif

#if !SILVERLIGHT
        //private int m_AutoSizeRowCount = 0;
#endif

        #endregion

        #region [ Initialize/Finalize ]

        public PivotGridControlBase(GridModel model)
            : base(model)
        {
          
            this.Model = model;
        }

        public PivotGridControlBase()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(PivotGridControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.HiddenRowGroups = new List<HiddenGroup>();
            this.HiddenColumnGroups = new List<HiddenGroup>();
            this.HiddenSubTotalsRowGroups = new List<HiddenGroup>();
            this.HiddenSubTotalsColumnGroups = new List<HiddenGroup>();
            this.HiddenRowGroupStore = new List<HiddenGroup>();
            this.HiddenColumnGroupStore = new List<HiddenGroup>();
            this.Model.CellModels.Add("PivotGridExpandCell", new PivotGridExpandCellCellModel(this.Model));
            this.Model.CellModels.Add("HyperlinkCell", new PivotGridHyperlinkCellModel(this.Model));
            this.Model.CellModels.Add("PivotGridHeaderCell", new PivotGridHeaderCellModel(this.Model));
            this.Model.CellModels.Add("PivotGridTotalCell", new PivotGridTotalCellModel(this.Model));
            this.Model.CellModels.Add("PivotGridTotalValueCell", new PivotGridTotalValueCellModel(this.Model));
            this.Model.CellModels.Add("PivotGridTemplateCell", new PivotGridTemplateCellModel(this.Model));
            this.Model.CellModels.Add("ColumnHeaderCell", new PivotGridColumnHeaderCellModel(this.Model));
            this.Model.CellModels.Add("TopLeftCell", new TopLeftCellModel(this.Model));
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint,true);
            
            //// Initializing the collection variables
            this._PivotRows = new ObservableCollection<PivotItem>();
            this._PivotColumns = new ObservableCollection<PivotItem>();
            this._PivotCalculations = new ObservableCollection<PivotComputationInfo>();
            this.PivotFilters = new ObservableCollection<FilterExpression>();
            this._PivotFields = new ObservableCollection<PivotItem>();
            this.ConditionalFormats = new ObservableCollection<PivotGridConditionalFormat>();
            this.NewRuleConditionalFormat = new ObservableCollection<PivotGridNewRuleConditionalFormat>();
            this.allowedFields = new List<FieldInfo>();
            this.engine = new PivotEngine();
            this.engine.PivotSchemaChanged += new PivotSchemaChangedEventHandler(engine_PivotSchemaChanged);
            //// Wiring events
            this.CurrentCellActivating += new GridCurrentCellActivatingEventHandler(PivotGridControlBase_CurrentCellActivating);
            this._PivotRows.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnPivotRowsCollectionChanged);
            this._PivotColumns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnPivotColumnsCollectionChanged);
            this.PivotCalculations.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnCalculationsCollectionChanged);
            this.PivotFilters.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnFiltersCollectionChanged);
            this.CellButtonClicked += new GridCellButtonClickedEventHandler(PivotGridControlBase_CellButtonClicked);
            this.Model.QueryCellInfo += new GridQueryCellInfoEventHandler(Model_QueryCellInfo);
            this.Model.SelectionChanging += new GridSelectionChangingEventHandler(Model_SelectionChanging);
            this.CurrentCellStartEditing += new CancelEventHandler(PivotGridControlBase_CurrentCellStartEditing);            
            this.WindowScrolling += new ScrollWindowEventHandler(PivotGridControlBase_WindowScrolling);
            this.HScrollPixel = true;
            this.VScrollPixel = true;
            IntializeModel();
            //Initialize();

        }

        void PivotGridControlBase_CurrentCellActivating(object sender, GridCurrentCellActivatingEventArgs e)
        {
            PivotCellInfo cellInfo = this.PivotEngine[e.RowIndex - 1, e.ColIndex - 1];
            if (cellInfo.CellType == PivotCellType.TopLeftCell && !this.ShowGroupBar)
                e.Cancel = true;
            else
                e.Cancel = false;
        }

        /// <summary>
        /// Occurs before the current cell switches into edit mode.
        /// </summary>
        /// <param name="sender"> control </param>
        /// <param name="e"> event data</param>
        protected void PivotGridControlBase_CurrentCellStartEditing(object sender, CancelEventArgs e)
        {
            GridCurrentCell currentCell = this.CurrentCell;
            PivotCellInfo cellInfo = this.PivotEngine[this.CurrentCell.RowIndex - 1, this.CurrentCell.ColIndex - 1];
            if (cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) || cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell))
                e.Cancel = !(this.EnableValueEditing && this.EditManager != null && this.EditManager.AllowEditingOfTotalCells);
            else if (cellInfo.CellType == PivotCellType.ValueCell)
                e.Cancel = !this.EnableValueEditing;
        }   

        /// <summary>
        /// Occurs to invalidate the GroupBar
        /// </summary>
        void PivotGridControlBase_WindowScrolling(object sender, ScrollWindowEventArgs e)
        {
            if (this.groupbar != null)
                this.groupbar.Invalidate();
        }

        internal void IntializeModel()
        {
            this.groupbar = new RowGroupBar(this);
            this.groupbar.Location = new System.Drawing.Point(0, 0);
            this.groupbar.TabStop = false;
            if (this.ShowGroupBar)
            {
                this.Controls.Add(this.groupbar);
            }
            #region default settings
            this.Model.BeginUpdate();
            this.SuspendLayout();
            this.Model.RowCount = 10;
            this.Model.ColCount = 10;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.ThemesEnabled = true;
            this.Model.Properties.RowHeaders = false;
            this.Model.Properties.ColHeaders = false;
            this.Model.Options.ExcelLikeCurrentCell = false;
            this.Model.Options.ExcelLikeSelectionFrame = false;
            this.Model.Options.ListBoxSelectionMode = SelectionMode.None;
            this.Model.Options.AllowSelection = GridSelectionFlags.None;
            this.Model.Options.ActivateCurrentCellBehavior = GridCellActivateAction.None;
            this.GridVisualStyles = GridVisualStyles.Office2007Blue;
            this.DefaultColWidth = 90;
            this.DefaultRowHeight = 30;
            this.ResumeLayout();
            this.Model.EndUpdate();
            this.Model.Refresh();
            #endregion
        }

        /// <summary>
        /// Occurs before selecting a range of cells.
        /// </summary>
        /// <param name="sender"> control </param>
        /// <param name="e"> event data</param>
        protected void Model_SelectionChanging(object sender, GridSelectionChangingEventArgs e)
        {
            if ((e.Reason == GridSelectionReason.MouseDown || e.Reason == GridSelectionReason.MouseMove)
                && (e.ClickRange.Top > 0 && e.ClickRange.Left > 0))
            {
                PivotCellInfo cellInfo = this.PivotEngine[e.ClickRange.Top - 1, e.ClickRange.Left - 1];
                switch (cellInfo.CellType)
                {
                    case (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell):
                    case (PivotCellType.ColumnHeaderCell | PivotCellType.ExpanderCell):
                    case (PivotCellType.RowHeaderCell | PivotCellType.HeaderCell):
                    case (PivotCellType.CalculationHeaderCell | PivotCellType.RowHeaderCell):
                    case (PivotCellType.ColumnHeaderCell | PivotCellType.HeaderCell):
                    case (PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell):
                    case (PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell):
                    case (PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell):
                    case (PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell | PivotCellType.HeaderCell):
                    case (PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell | PivotCellType.HeaderCell):
                    case (PivotCellType.TotalCell | PivotCellType.RowHeaderCell):
                    case (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell):
                    case (PivotCellType.TotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell):
                    case (PivotCellType.TotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.RowHeaderCell):
                    case (PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell):
                    case (PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.RowHeaderCell):
                    case (PivotCellType.TopLeftCell):
                        e.Cancel = true;
                        break;
                }
            }
        }
        #region Hidden Entries
        internal List<HiddenGroup> HiddenRowGroups { get; set; }

        internal List<HiddenGroup> HiddenColumnGroups { get; set; }

        internal List<HiddenGroup> HiddenSubTotalsRowGroups { get; set; }

        internal List<HiddenGroup> HiddenSubTotalsColumnGroups { get; set; }
        internal List<HiddenGroup> HiddenRowGroupStore { get; set; }
        internal List<HiddenGroup> HiddenColumnGroupStore { get; set; }
        #endregion

        #region Expand/Collapse

        void PivotGridControlBase_CellButtonClicked(object sender, GridCellButtonClickedEventArgs e)
        {
            this.Model.BeginUpdate();
            if (!setState)
                setState = true;
            SetExpandCollapse(e.RowIndex, e.ColIndex);
            this.Model.EndUpdate();
            this.Model.Refresh();
        }

        private void SetExpandCollapse(int rowIndex, int colIndex)
        {
            if ((this.PivotEngine[rowIndex - 1, colIndex - 1].CellType == (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell) && this.Model[rowIndex, colIndex].CellType == "PivotGridExpandCell")
                || (this.PivotEngine[rowIndex - 1, colIndex - 1].CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell) && this.Model[rowIndex, colIndex].CellType == "PivotGridExpandCell"))
            {
                ClickRow(rowIndex, colIndex);
            }
            else if ((this.PivotEngine[rowIndex - 1, colIndex - 1].CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.ExpanderCell) && this.Model[rowIndex, colIndex].CellType == "PivotGridExpandCell")
                || (this.PivotEngine[rowIndex - 1, colIndex - 1].CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) && this.Model[rowIndex, colIndex].CellType == "PivotGridExpandCell"))
            {
                ClickCol(rowIndex, colIndex);
            }
        }

        /// <summary>
        /// Refreshes the PivotGridControl's Layout 
        /// </summary>
        /// <param name="shouldPopulateEngine">It will repopulate the PivotEngine if True</param>
        public new void Refresh(bool shouldPopulateEngine)
        {
            if (!this.IgnoreRefresh && (this.PivotEngine.DataSourceList != null && this.PivotEngine.DataSourceList.ToList<object>().Count() >= 0))
            {
                DataRefreshingEventArgs args = new DataRefreshingEventArgs();
                this.RaiseDataRefreshing(args);
                if (!args.Cancel)
                {
                    if (shouldPopulateEngine)
                    {
                        PivotEngine.CoveredRanges.Clear();
                        this.ApplyRowCols();
                    }
                    QueryGridAppearance();
                    InitTargetGrid();
                }
                this.RaiseDataRefreshed(new DataRefreshedEventArgs());
            }
        }
        Hashtable cachedValuesForQueryCellInfo = new Hashtable();
        private Hashtable collapsedNodes = new Hashtable();
        private Hashtable TotalheaderCells = new Hashtable();

        private System.Drawing.Point GetCellHashKey(int rowIndex, int colIndex)
        {
            return new System.Drawing.Point(colIndex, rowIndex);
        }

        private System.Drawing.Point GetCellIndex(int rowIndex, int colIndex)
        {
            return new System.Drawing.Point(rowIndex, colIndex);
        }

        private string GetCachedValue(System.Drawing.Point pt)
        {
            Stack a = cachedValuesForQueryCellInfo[pt] as Stack;
            return a.Peek().ToString();
        }
        private void RemoveCachedValue(int rowIndex, int colIndex)
        {
            System.Drawing.Point pt = GetCellHashKey(rowIndex, colIndex);
            Stack a = cachedValuesForQueryCellInfo[pt] as Stack;
            if (a != null)
            {
                a.Pop();
                if (a.Count == 0)
                {
                    cachedValuesForQueryCellInfo.Remove(pt);
                }
            }
        }
        private void CacheValue(int rowIndex, int colIndex, string value)
         {
            System.Drawing.Point pt = GetCellHashKey(rowIndex, colIndex);
            Stack a = cachedValuesForQueryCellInfo[pt] as Stack;
            if (a == null)
            {
                a = new Stack();
                cachedValuesForQueryCellInfo.Add(pt, a);
            }
            a.Push(value);
        }
        int summaryCount = 0; private bool setState = false; 
        private void ClickRow(int rowIndex, int colIndex)
        {
            if (rowIndex >= 0 && rowIndex <= this.Model.RowCount)
            {
                int leastRowRange = PivotRows.Count - 1; 
                object key = GetCellHashKey(rowIndex, colIndex);
                if (this.collapsedNodes.ContainsKey(key))
                {
                    collapsedNodes.Remove(key);
                    int[] collapsedRanges = GetCollapsedRows(rowIndex, colIndex);
                    key = GetCellHashKey(collapsedRanges[0], collapsedRanges[1]);
                    
                    object[] saved = collapsedNodes[key] as object[];
                    int[] sizes = saved[0] as int[];
                    string[] values = saved[2] as string[];
                    GridRangeInfoList ranges = saved[1] as GridRangeInfoList;

                    collapsedNodes.Remove(key);
                    
                    this.Model[collapsedRanges[0], collapsedRanges[1]].Description = "-";
                    GridRangeInfo range = this.Model.CoveredRanges.FindRange(collapsedRanges[0], collapsedRanges[1]);
                    if (range.IsEmpty)
                    {
                        range = GridRangeInfo.Cell(collapsedRanges[0], collapsedRanges[1]);
                        this.Model.CoveredRanges.Add(range);
                    }

                    Model.RowHeights.SetRange(collapsedRanges[0] , collapsedRanges[0] + sizes.GetLength(0) - 1, sizes, true);

                    this.Model.CoveredRanges.Remove(range);
                    foreach (GridRangeInfo r in ranges)
                        this.Model.CoveredRanges.Add(r);

                    int count = collapsedRanges[0] + 1;// range.Right + 1;
                    for (int n = colIndex + 1; n < count; ++n)
                    {
                        RemoveCachedValue(rowIndex, n);
                    }
                    if (!showSubTotals)
                    {
                        var coveredCell = this.Model.CoveredRanges.FindRange(rowIndex, colIndex);
                        {
                            for (int i = coveredCell.Top; i <= coveredCell.Bottom; i++)
                            {
                                this.Model.HideRows[i] = true;
                            }
                        }
                    }
                    this.Model[rowIndex, colIndex].CellType = "TextBox";
                    this.Model[rowIndex, colIndex].VerticalAlignment = GridVerticalAlignment.Middle;

                }
                else
                {
                    //collapsing
                    object[] save = new object[3];
                    GridRangeInfo range = this.Model.CoveredRanges.FindRange(rowIndex, colIndex);
                    if (range.IsEmpty)
                        range = GridRangeInfo.Cell(rowIndex, colIndex);
                    GridRangeInfo newRange = GridRangeInfo.Cells(rowIndex, colIndex, range.Bottom /*rowIndex + range.Height*/, Model.ColCount /*- PivotEngine.PivotCalculations.Count*/);

                    if (range.IsEmpty) 
                        return;

                    save[0] = Model.RowHeights.GetRange(rowIndex , range.Bottom + Math.Max(summaryCount, 1));
                    save[1] = this.Model.CoveredRanges.Ranges.GetRangesIntersecting(newRange);

                    int count = range.Bottom + 1; 

                    for (int n = colIndex + 1; n < count; ++n)
                    {
                        CacheValue(rowIndex, n, this.Model[n,colIndex].Text);
                    }

                    collapsedNodes.Add(key, save);
                    SetCollapsedRowRange(rowIndex, colIndex);
                    Model.RowHeights.SetRange(rowIndex , range.Bottom, 0);

                    if (!showSubTotals)
                    {
                        var coveredCell = this.Model.CoveredRanges.FindRange(range.Bottom + 1, colIndex);
                        if (this.Model.HideRows[coveredCell.Top])// && this.Model.HideCols[coveredCell.Right])
                        {
                            for (int i = coveredCell.Top; i <= coveredCell.Bottom; i++)
                            {
                                this.Model.HideRows[i] = false;
                            }
                        }
                    }
                    this.Model[range.Bottom + 1, colIndex].CellType = "PivotGridExpandCell";
                    this.Model[range.Bottom + 1, colIndex].CellValue = this.PivotEngine[range.Bottom, colIndex - 1].FormattedText;
                    this.PivotEngine[range.Bottom, colIndex - 1].Tag = "TotalHeaderCell";
                    this.Model[range.Bottom+1, colIndex].Description = "+";
                    this.Model[range.Bottom + 1, colIndex].VerticalAlignment = GridVerticalAlignment.Top;
                    key = GetCellHashKey(range.Bottom + 1, colIndex);

                    collapsedNodes.Add(key, save);

                    foreach (GridRangeInfo r in save[1] as GridRangeInfoList)
                    {
                        this.Model.CoveredRanges.Remove(r);
                    }
                }
            }
            this.Model.Refresh();
        }
        private void ClickCol(int rowIndex, int colIndex)
        {
            if (colIndex >= 1 && colIndex < Model.ColCount)
            {
                int leastColRange = PivotColumns.Count - 1;
                object key = GetCellHashKey(rowIndex, colIndex);
                if (collapsedNodes.ContainsKey(key))
                {
                    collapsedNodes.Remove(key);
                    int[] collapsedRanges = GetCollapsedCols(rowIndex, colIndex);
                    key = GetCellHashKey(collapsedRanges[0], collapsedRanges[1]);
                    
                    
                    object[] saved = collapsedNodes[key] as object[];
                    int[] sizes = saved[0] as int[];
                    string[] values = saved[2] as string[];
                    GridRangeInfoList ranges = saved[1] as GridRangeInfoList;
                    collapsedNodes.Remove(key);
                    this.Model[collapsedRanges[0], collapsedRanges[1]].Description = "-";

                    GridRangeInfo range = this.Model.CoveredRanges.FindRange(collapsedRanges[0], collapsedRanges[1]);
                    if (range.IsEmpty)
                    {
                        range = GridRangeInfo.Cell(collapsedRanges[0], collapsedRanges[1]);
                        this.Model.CoveredRanges.Add(range);
                    }

                    if (collapsedRanges[0] == leastColRange)
                        this.Model.ColWidths.SetRange(collapsedRanges[1], (collapsedRanges[1] + sizes.GetLength(0)) - 1, sizes, true);
                    else
                        this.Model.ColWidths.SetRange(collapsedRanges[1] + 1, (collapsedRanges[1] + sizes.GetLength(0)) - 1, sizes, true);

                    this.Model.CoveredRanges.Remove(range);


                    foreach (GridRangeInfo r in ranges)
                        this.Model.CoveredRanges.Add(r);


                    int count = collapsedRanges[1] + 1;
                    for (int k = 0; k < count/*summaryCount*/; k++)
                    {
                        for (int n = bottomRow + 1; n < count; ++n)
                        {
                            RemoveCachedValue(n, colIndex + k);
                        }
                    }

                    if (!ShowSubTotals)
                    {
                        var coveredCell = this.Model.CoveredRanges.FindRange(rowIndex, colIndex);
                        {
                            for (int i = coveredCell.Left; i <= coveredCell.Right; i++)
                            {
                                this.Model.HideCols[i] = true;
                            }
                        }
                    }
                    this.Model[rowIndex, colIndex].CellType = "TextBox";
                    this.Model[rowIndex, colIndex].HorizontalAlignment = GridHorizontalAlignment.Center;
                    this.Model[rowIndex, colIndex].VerticalAlignment = GridVerticalAlignment.Middle; 
                }
                else
                {
                    object[] save = new object[3];
                    GridRangeInfo range = this.Model.CoveredRanges.FindRange(rowIndex, colIndex);
                    if (range.IsEmpty)
                        range = GridRangeInfo.Cell(rowIndex, colIndex);
                    GridRangeInfo newRange = GridRangeInfo.Cells(rowIndex, colIndex,PivotEngine.PivotColumns.Count, colIndex);

                    save[0] = this.Model.ColWidths.GetRange(colIndex + 1, range.Right + Math.Max(summaryCount, 1));
                    save[1] = this.Model.CoveredRanges.Ranges.GetRangesIntersecting(newRange);
                    
                    int count = range.Right + 1; //PivotEngine.PivotColumns.Count + 1;// range.Bottom
     
                    for (int n = colIndex + 1; n < count; ++n)
                    {
                        CacheValue(rowIndex, n, this.Model[rowIndex, n].Text);
                    }

                    collapsedNodes.Add(key, save);
                    SetCollapsedColRange(rowIndex, colIndex);

                    this.Model.ColWidths.SetRange(colIndex , range.Right, 0); 

                    this.Model[rowIndex, range.Right + 1].Description = "+";
                    this.Model[rowIndex, range.Right + 1].CellType = "PivotGridExpandCell";
                    this.Model[rowIndex, range.Right + 1].CellValue = this.PivotEngine[rowIndex - 1, range.Right].FormattedText;
                    this.PivotEngine[rowIndex - 1, range.Right].Tag = "TotalHeaderCell";
                    this.Model[rowIndex, range.Right + 1].HorizontalAlignment = GridHorizontalAlignment.Left;
                    this.Model[rowIndex, range.Right + 1].VerticalAlignment = GridVerticalAlignment.Top;
                    key = GetCellHashKey(rowIndex, range.Right+1);

                    collapsedNodes.Add(key, save);

                    if (!ShowSubTotals)
                    {
                        var coveredCell = this.Model.CoveredRanges.FindRange(rowIndex, range.Right + 1);
                        if (this.Model.HideCols[coveredCell.Left] && this.Model.HideCols[coveredCell.Right])
                        {
                            for (int i = coveredCell.Left; i <= coveredCell.Right; i++)
                            {
                                this.Model.HideCols[i] = false;
                            }
                        }
                    }
                    this.Model[rowIndex, colIndex].Description = "+";
                    foreach (GridRangeInfo r in save[1] as GridRangeInfoList)
                    {
                        this.Model.CoveredRanges.Remove(r);
                    }
                    this.Model.CoveredRanges.UpdateList();
                    this.Model.CoveredRanges.ResetCache();
                    
                }
            }
            this.Model.Refresh();
        }

        int AiMax = 100; int[] Ai = new int[1000]; int arrLast = 0; 

        private void SetCollapsedColRange(int rowIndex, int colIndex)
        {
            GridRangeInfo range = this.Model.CoveredRanges.FindRange(rowIndex, colIndex);
            if (range.IsEmpty)
            {
                range = GridRangeInfo.Cell(rowIndex, colIndex);
                this.Model.CoveredRanges.Add(range);
            }
            int extendCol = range.Right + 1;
            
            for (int j = 1; j < AiMax; j++)
            {
                if (Ai[j] != 0)
                    arrLast = j;
                else
                    break;
            }
            for (int i = 1,k=arrLast+1; i <= 1; i++)
            {
                Ai[k] = rowIndex; k++;
                Ai[k] = colIndex; k++;
                Ai[k] = extendCol;
            }
        }

        private int[] GetCollapsedCols(int rowIndex, int extendCol)
        {
            for(int i=1;i<AiMax;i++)
            {
                for (int j = 1; j < AiMax; j++)
                {
                    if (Ai[j] == rowIndex)
                    {
                        for (int k = j; k < AiMax; k++)
                        {
                            if (Ai[k + 2] == extendCol)
                            {
                                
                                int[] collapsedRanges = new int[3];
                                collapsedRanges[0] = Ai[k]; ;
                                collapsedRanges[1] = Ai[k+1];
                                return collapsedRanges;
                            }
                        }

                    }
                }
            }

            return null;
        }

        int BiMax = 100; int[] Bi = new int[1000]; int arrayLast = 0; 
        private void SetCollapsedRowRange(int rowIndex, int colIndex)
        {
            GridRangeInfo range = this.Model.CoveredRanges.FindRange(rowIndex, colIndex);
            if (range.IsEmpty)
            {
                range = GridRangeInfo.Cell(rowIndex, colIndex);
                this.Model.CoveredRanges.Add(range);
            }
            int extendRow = range.Bottom + 1;

            for (int j = 1; j < BiMax; j++)
            {
                if (Bi[j] != 0)
                    arrayLast = j;
                else
                    break;
            }
            for (int i = 1, k = arrayLast + 1; i <= 1; i++)
            {
                Bi[k] = colIndex; k++;
                Bi[k] = rowIndex; k++;
                Bi[k] = extendRow;
            }
        }
        private int[] GetCollapsedRows(int extendRow, int colIndex)
        {
            for (int i = 1; i < BiMax; i++)
            {
                for (int j = 1; j < BiMax; j++)
                {
                    if (Bi[j] == colIndex)
                    {
                        for (int k = j; k < AiMax; k++)
                        {
                            if (Bi[k + 2] == extendRow)
                            {

                                int[] collapsedRanges = new int[3];
                                collapsedRanges[0] = Bi[k +1]; ;
                                collapsedRanges[1] = Bi[k ];
                                return collapsedRanges;
                            }
                        }

                    }
                }
            }

            return null;
        }
        #endregion

        #region Assigning values to cell
        int bottomRow = 0;
        
        private void SetCellValue(GridStyleInfo styleInfo, PivotCellInfo cellInfo, object tag)
        {
            if (cellInfo != null)
            {
                styleInfo.CellValue = cellInfo.FormattedText;

                if (this.GridVisualStyles == GridVisualStyles.Metro)
                    styleInfo.Font.Size = 9f;
                if (tag == null)
                {
                    styleInfo.HorizontalAlignment = GridHorizontalAlignment.Right;
                    styleInfo.VerticalAlignment = GridVerticalAlignment.Middle;
                }
                else if ((tag.ToString() == "HeaderCell") || (tag.ToString() == "TotalCell"))
                {
                    styleInfo.CellType = "PivotGridHeaderCell";
                    styleInfo.VerticalAlignment = GridVerticalAlignment.Middle;
                    if (cellInfo.CellType == (PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell))
                    {
                        styleInfo.HorizontalAlignment = GridHorizontalAlignment.Center;
                    }
                }
                else if (tag.ToString() == "ExpanderCell")
                {
                    if (this.EnableValueEditing && this.EditManager != null && this.EditManager.HideExpanders)
                        styleInfo.CellType = "PivotGridHeaderCell";
                    else
                        styleInfo.CellType = "PivotGridExpandCell";
                    styleInfo.VerticalAlignment = GridVerticalAlignment.Top;
                }
                else if (tag.ToString() == "TotalValueCell")
                {
                    if (this.EnableValueEditing && this.EditManager != null && this.EditManager.AllowEditingOfTotalCells)
                        styleInfo.CellType = "TextBox";
                    else
                        styleInfo.CellType = "PivotGridTotalValueCell";

                    if (this.GridVisualStyles == GridVisualStyles.Metro)
                    {
                        styleInfo.BackColor = Color.FromArgb(246, 247, 247);
                        styleInfo.TextColor = Color.FromArgb(92,92,92);
                        styleInfo.Font.Bold = true;
                    }
                    else
                        styleInfo.BackColor = Color.FromArgb(251, 226, 146);
                    styleInfo.HorizontalAlignment = GridHorizontalAlignment.Right;
                    styleInfo.VerticalAlignment = GridVerticalAlignment.Middle;
                }
                else if (tag.ToString() == "TotalHeaderCell")
                {
                    if (cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell) ||
                        cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) ||
                        cellInfo.CellType == (PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell) ||
                        cellInfo.CellType == (PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell | PivotCellType.CalculationHeaderCell) ||
                        cellInfo.CellType == (PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell) ||
                        cellInfo.CellType == (PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell | PivotCellType.TotalCell))
                    {
                        styleInfo.VerticalAlignment = GridVerticalAlignment.Middle;
                    }
                    if (cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) ||
                        cellInfo.CellType == (PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell) ||
                        cellInfo.CellType == (PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell | PivotCellType.CalculationHeaderCell)||
                        cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell))
                    {
                        styleInfo.HorizontalAlignment = GridHorizontalAlignment.Center;
                    }

                    styleInfo.CellType = "PivotGridTotalValueCell";
                    styleInfo.CellValue = cellInfo.FormattedText;
                    if (this.GridVisualStyles == GridVisualStyles.Metro)
                    {
                        styleInfo.BackColor = Color.FromArgb(246, 247, 247);
                        styleInfo.TextColor = Color.FromArgb(88, 88, 88);
                        styleInfo.Font.Bold = true;
                    }
                    else
                        styleInfo.BackColor = Color.FromArgb(251, 226, 146);
                }
                else if (tag.ToString() == "TopLeftCell")
                {
                    if (this.ShowGroupBar)
                    {
                        styleInfo.CellType = "TopLeftCell";
                        styleInfo.Control = this.groupbar;
                        if (this.IsRightToLeft())
                        {
                            Rectangle rect = this.RangeInfoToRectangle(GridRangeInfo.Cell(styleInfo.CellIdentity.RowIndex, styleInfo.CellIdentity.ColIndex));
                            this.groupbar.Location = new System.Drawing.Point(rect.Location.X - rect.Width, rect.Y);
                        }
                        else
                            this.groupbar.Location = new System.Drawing.Point(0, 0);
                    }
                    else
                    {
                        styleInfo.CellType = GridCellTypeName.Static;
                        switch (this.GridVisualStyles)
                        {
                            case GridVisualStyles.Office2007Blue:
                            case GridVisualStyles.Office2010Blue:
                                styleInfo.BackColor = Color.FromArgb(227, 239, 255);
                                break;
                            case GridVisualStyles.Office2007Black:
                            case GridVisualStyles.Office2007Silver:
                                styleInfo.BackColor = Color.FromArgb(240, 241, 242);
                                break;
                            case GridVisualStyles.Office2010Black:
                                styleInfo.BackColor = Color.FromArgb(100, 100, 110);
                                break;
                            case GridVisualStyles.Office2010Silver:
                                styleInfo.BackColor = Color.FromArgb(223, 227, 222);
                                break;
                            case GridVisualStyles.Metro:
                                styleInfo.BackColor = Color.FromArgb(255,255,255);
                                break;
                            default:
                                styleInfo.BackColor = SystemColors.Control;
                                break;
                        }
                    }
                }

                if (cellInfo.CellType == (PivotCellType.GrandTotalCell | PivotCellType.ValueCell) || cellInfo.CellType == (PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell) ||
                        cellInfo.CellType == (PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell | PivotCellType.CalculationHeaderCell))
                {
                    if (this.GridVisualStyles == Forms.GridVisualStyles.Metro)
                    {
                        styleInfo.TextColor = Color.Black;
                    }
                    styleInfo.VerticalAlignment = GridVerticalAlignment.Middle;
                }
            }
        }
        
        void Model_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {

            if (e.RowIndex > 0 && e.ColIndex > 0 && e.RowIndex< PivotEngine.RowCount&&e.ColIndex<PivotEngine.ColumnCount)
            {
                var styleInfo = e.Style;
                PivotCellInfo cellInfo = this.PivotEngine[e.RowIndex - 1, e.ColIndex - 1];
              
                    if (cellInfo!= null && cellInfo.Tag == null)
                    {
                        switch (cellInfo.CellType)
                        {
                            case (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell):
                            case (PivotCellType.ColumnHeaderCell | PivotCellType.ExpanderCell):
                                styleInfo.Tag = "ExpanderCell";
                                this.SetCellValue(styleInfo, cellInfo, styleInfo.Tag);
                                object key = this.GetCellHashKey(e.RowIndex, e.ColIndex);
                                if (!this.collapsedNodes.ContainsKey(key))
                                {
                                    styleInfo.Description = "-";
                                }
                                break;

                            case (PivotCellType.RowHeaderCell | PivotCellType.HeaderCell):
                            case (PivotCellType.CalculationHeaderCell | PivotCellType.RowHeaderCell):
                                styleInfo.Tag = "HeaderCell";
                                this.SetCellValue(styleInfo, cellInfo, styleInfo.Tag);
                                break;

                            case (PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell):
                            case (PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell):
                            case (PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell | PivotCellType.HeaderCell):
                            case (PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell | PivotCellType.HeaderCell):
                            case (PivotCellType.TotalCell | PivotCellType.RowHeaderCell):
                            case (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell):
                            case (PivotCellType.TotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell):
                            case (PivotCellType.TotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.RowHeaderCell):
                            case (PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell):
                            case (PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.RowHeaderCell):
                                styleInfo.Tag = "TotalHeaderCell";
                                this.SetCellValue(styleInfo, cellInfo, styleInfo.Tag);
                                break;

                            case (PivotCellType.TopLeftCell):
                                styleInfo.Tag = "TopLeftCell";
                                this.SetCellValue(styleInfo, cellInfo, styleInfo.Tag);
                                break;

                            case (PivotCellType.GrandTotalCell | PivotCellType.ValueCell):
                            case (PivotCellType.TotalCell | PivotCellType.ValueCell):
                            case (PivotCellType.ValueCell | PivotCellType.TotalCell | PivotCellType.GrandTotalCell):
                                styleInfo.Tag = "TotalValueCell";
                                this.SetCellValue(styleInfo, cellInfo, styleInfo.Tag);
                                break;

                            case (PivotCellType.ColumnHeaderCell | PivotCellType.HeaderCell):
                            case (PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell):
                                styleInfo.Tag = "HeaderCell";
                                this.SetCellValue(styleInfo, cellInfo, styleInfo.Tag);
                                break;
                            default:
                                this.SetCellValue(styleInfo, cellInfo, null);
                                break;
                        }

                        if (cellInfo.Value != null && cellInfo.ParentCell == null)
                        {
                            bool canApplyConditionalFormat = false;
                            foreach (var condition in this.ConditionalFormats)
                            {
                                switch (condition.PivotCellInfo.CellType)
                                {
                                    case PivotCellType.ValueCell:
                                        if (cellInfo.CellType == PivotCellType.ValueCell)
                                            canApplyConditionalFormat = true;
                                        break;
                                    default:
                                        canApplyConditionalFormat = false;
                                        break;                                   
                                }
                                if (canApplyConditionalFormat)
                                {
                                    string associatedMeasure = GetAssociatedMeasure(cellInfo, e.RowIndex, e.ColIndex);
                                    if (associatedMeasure != null)
                                    {
                                        bool applyFormat = condition.ApplyFormat(cellInfo, associatedMeasure);
                                        if (applyFormat)
                                        {
                                            this.ApplyStyle(e.Style, condition.PivotCellStyle);                                          
                                        }                                        
                                    }                                    
                                }
                            }

                            foreach (var condition in this.ConditionalFormats)
                            {
                                switch (condition.PivotCellInfo.CellType)
                                {
                                    case PivotCellType.ValueCell:
                                        if (cellInfo.CellType == PivotCellType.ValueCell)
                                            canApplyConditionalFormat = true;
                                        break;
                                    default:
                                        canApplyConditionalFormat = false;
                                        break;
                                }
                                if (canApplyConditionalFormat)
                                {
                                    string associatedMeasure = GetAssociatedMeasure(cellInfo, e.RowIndex, e.ColIndex);
                                    if (associatedMeasure != null)
                                    {
                                        bool applyFormat = condition.ApplyFormat(cellInfo, associatedMeasure);
                                        if (applyFormat)
                                        {                                            
                                            this.ApplyStyle(e.Style, condition.PivotCellStyle);
                                        }
                                    }
                                }
                            }
                            foreach (var newRule in this.NewRuleConditionalFormat)
                            {
                                switch (newRule.PivotCellInfo.CellType)
                                {
                                    case PivotCellType.ValueCell:
                                        if (cellInfo.CellType == PivotCellType.ValueCell)
                                            canApplyConditionalFormat = true;
                                        break;
                                    default:
                                        canApplyConditionalFormat = false;
                                        break;
                                }
                                if (canApplyConditionalFormat)
                                {
                                    newRule.ApplyStyle(e.Style, e.RowIndex, e.ColIndex, this, cellInfo);                                   
                                }
                            }
                        }
                    }

                if (FreezeHeaders)
                    InitTargetGrid();

                QueryGridAppearance();

            }

        }

        /// <summary>
        /// Applying style for supplied GridStyleInfo
        /// </summary>
        /// <param name="styleInfo">GridStyleInfo</param>
        /// <param name="cellStyle">GridStyleInfo</param>
        private void ApplyStyle(GridStyleInfo styleInfo, GridStyleInfo cellStyle)
        {
            if (cellStyle != null)
            {
                styleInfo.BackColor = cellStyle.BackColor;
                styleInfo.Font = cellStyle.Font;
                styleInfo.TextColor = cellStyle.TextColor;
                styleInfo.HorizontalAlignment = cellStyle.HorizontalAlignment;
                styleInfo.VerticalAlignment = cellStyle.VerticalAlignment;
                styleInfo.Themed = cellStyle.Themed;
                styleInfo.TextAlign = cellStyle.TextAlign;
                styleInfo.Borders = cellStyle.Borders;
            }
        }
        /// <summary>
        /// Gets the associated measure.
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        private string GetAssociatedMeasure(PivotCellInfo cellInfo, int rowIndex, int columnIndex)
        {
            if (this.PivotCalculations.Count > 0)
            {
                if (this.PivotCalculations.Count > 1)
                {
                    if (this.ShowCalculationsAsColumns)
                    {
                        return this.PivotEngine[this.PivotColumns.Count , columnIndex - 1].FormattedText;                                             
                    }
                    else
                    {
                        return this.PivotEngine[this.PivotRows.Count, columnIndex - 1].FormattedText;
                    }
                }
                else
                    return this.PivotCalculations[0].FieldName;
            }
            else
                return string.Empty;
        }      

        private void QueryGridAppearance()
        {
            if (Model.Properties.ColHeaders || Model.Properties.RowHeaders)
            {
                Model.Properties.ColHeaders = false;
                Model.Properties.RowHeaders = false;
                Model.Refresh();
            }
            
        }

        #endregion

        internal void PivotGridReCalculateSize()
        {
        }      

        #endregion

        #region [ Properties ]

        internal bool RefreshFromGroupingBar { get; set; }

        internal bool UpdateGridLayout { get; set; }

        internal bool IgnoreRefresh { get; set; }

        internal GridBorder GridOuterBorder { get; set; }


        private object itemSource = null;
        /// <summary>
        /// Gets or sets source of data for this pivot table. This object should be either 
        /// an IEnumerable list, or a DataTable.
        /// </summary>
        [Category("Pivot")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object ItemSource
        {
            get
            {
                return itemSource;
            }
            set
            {

                if (this.PivotEngine != null)
                {
                    this.PivotEngine.DataSource = value;
                    ItemSourceChangedEventArgs args = new ItemSourceChangedEventArgs()
                    {
                        NewValue = value,
                        OldValue = this.itemSource,
                    };

                    OnItemSourceChanged(this, args);
                }
                else
                    itemSource = value;

            }
        }

        /// <summary>
        /// Gets or sets the VisualStylesDrawing object
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets the VisualStylesDrawing object")]
        [Category("Grid")]
        public IVisualStylesDrawing GridVisualStylesDrawing
        {
            get
            {
                return Model.Options.GridVisualStylesDrawing;
            }

            set
            {
                Model.Options.GridVisualStylesDrawing = value;
            }
        }

        /// <summary>
        /// Gets or sets the VisualStyles (skins) like Office2010, Office2007, Office2003
        /// </summary>
        [Browsable(true)]
        [Description("Specifies look and feel skins for the Grid")]
        [Category("Grid")]
        public GridVisualStyles GridVisualStyles
        {
            get
            {
                return Model.Options.GridVisualStyles;
            }

            set
            {
                this.groupbar.GridVisualStyles = value;
                Model.Options.GridVisualStyles = value;
                {
                    switch (value)
                    {
                        case GridVisualStyles.Office2007Blue:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                            this.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Blue;
                            this.Model.TableStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(206, 221, 242));//(169, 195, 232));
                            this.Model.TableStyle.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(206, 221, 242));//169, 195, 232));
                            break;
                        case GridVisualStyles.Office2007Black:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                            this.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Black;
                            this.Model.TableStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            this.Model.TableStyle.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            break;
                        case GridVisualStyles.Office2007Silver:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                            this.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Silver;
                            this.Model.TableStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            this.Model.TableStyle.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            break;
                        case GridVisualStyles.Office2010Blue:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                            this.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Blue;
                            this.Model.TableStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            this.Model.TableStyle.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            break;
                        case GridVisualStyles.Office2010Black:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                            this.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Black;
                            this.Model.TableStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            this.Model.TableStyle.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            break;
                        case GridVisualStyles.Office2010Silver:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                            this.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Silver;
                            this.Model.TableStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            this.Model.TableStyle.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            break;
                        default:
                            this.GridOfficeScrollBars = OfficeScrollBars.None;
                            this.Model.TableStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.Gray);
                            this.Model.TableStyle.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.Gray);
                            break;
                    }
                }
            }
        }       

        private ObservableCollection<PivotGridConditionalFormat> conditionalFormats;

        /// <summary>
        /// Gets the collection of ConditionalFormats
        /// </summary>
        [Category("Pivot")]
        [XmlIgnore]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObservableCollection<PivotGridConditionalFormat> ConditionalFormats
        {
            get
            {
                return conditionalFormats;
            }
            set
            {
                conditionalFormats = value;
            }
        }

        private ObservableCollection<PivotGridNewRuleConditionalFormat> newRuleConditionalFormat;

        /// <summary>
        /// Gets the collection of NewRuleConditionalFormat
        /// </summary>
        [Category("Pivot")]
        [XmlIgnore]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObservableCollection<PivotGridNewRuleConditionalFormat> NewRuleConditionalFormat
        {
            get
            {
                return newRuleConditionalFormat;
            }
            set
            {
                newRuleConditionalFormat = value;
            }
        }

        #region IPivotControl Members
        /// <summary>
        /// Gets the collection of PivotItems of PivotRows
        /// </summary>
        [Category("Pivot")]
        [XmlIgnore]
        public ObservableCollection<PivotItem> PivotRows
        {
            get
            {
                return _PivotRows;
            }
        }

        /// <summary>
        /// Gets the collection of PivotItems of PivotColumns
        /// </summary>
        [Category("Pivot")]
        [XmlIgnore]
        public ObservableCollection<PivotItem> PivotColumns
        {
            get
            {
                return _PivotColumns;
            }
        }

        /// <summary>
        /// Gets the collection of FilterExpressoin
        /// </summary>
        [Category("Pivot")]
        [XmlIgnore]
        public ObservableCollection<FilterExpression> Filters
        {
            get
            {
                return PivotFilters;
            }
        }

        /// <summary>
        /// Gets the collection of PivotCalculations
        /// </summary>
        [Category("Pivot")]
        [XmlIgnore]
        public ObservableCollection<PivotComputationInfo> PivotCalculations
        {
            get
            {
                return _PivotCalculations;
            }
        }

        /// <summary>
        /// Gets the collection of PivotItems of Pivot FieldList
        /// </summary>
        [Category("Pivot")]
        [XmlIgnore]
        public ObservableCollection<PivotItem> PivotFields
        {
            get
            {
                return _PivotFields;
            }
        }

        #endregion

        bool isDynamicData = false;
        /// <summary>
        /// Gets or sets a value indicating whether the supplied Item Source is dynamic
        /// </summary>
        public bool IsDynamicData
        {
            get 
            {
                return this.isDynamicData; 
            }
            set
            {
                this.isDynamicData = value;
            }
        }

        bool showSubTotals = true;
        public bool ShowSubTotals
        {
            get
            {
                return this.showSubTotals;
            }
            set
            {
                this.showSubTotals = value;
                SubTotalsRendering();
            }
        }

        bool allowFiltering = true;
        /// <summary>
        /// Gets or sets a value indicating to enable or disable the filter
        /// </summary>
        public bool AllowFiltering
        {
            get
            {
                return this.allowFiltering;
            }
            set
            {
                this.allowFiltering = value;
            }
        }

        bool allowSorting = true;
        /// <summary>
        /// Gets or sets a value indicating to enable or disable the sorting
        /// </summary>
        public bool AllowSorting
        {
            get
            {
                return this.allowSorting;
            }
            set
            {
                this.allowSorting = value;
            }
        }

        bool showGroupBar = false;
        /// <summary>
        /// Gets or sets a value indicating to enable or disable the group bar
        /// </summary>
        public bool ShowGroupBar
        {
            get
            {
                return this.showGroupBar;
            }
            set
            {
                this.showGroupBar = value;
                GroupBarSize();
            }
        }

        public void GroupBarSize()
        {
            if (this.ShowGroupBar)
            {
                this.BeginUpdate();
                this.groupbar.Visible = true;
                int pWidth;
                if (this.ShowCalculationsAsColumns)
                {
                    pWidth = this.PivotRows.Count * 90;
                }
                else
                {
                    if (this.PivotCalculations.Count > 1)
                    {
                        pWidth = (this.PivotRows.Count + 1) * 90;
                    }
                    else
                    {
                        pWidth = this.PivotRows.Count * 90;
                    }
                }
                int pHeight = 0;
                if (PivotCalculations.Count < 2)
                    pHeight = (this.PivotColumns.Count * 30) - 2;
                else
                    pHeight = this.ShowCalculationsAsColumns ? ((this.PivotColumns.Count) * 30) - 2 : ((this.PivotColumns.Count ) * 30) - 2;
                this.groupbar.Size = new System.Drawing.Size(pWidth, pHeight);
                this.Controls.Add(this.groupbar);
                this.EndUpdate();
                this.Refresh();
            }
            else
            {
                if (this.Controls.Contains(this.groupbar))
                {
                    this.SuspendLayout();
                    this.groupbar.Visible = true;
                    this.Controls.Remove(this.groupbar);
                    this.ResumeLayout();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the supplied Item Source is dynamic
        /// </summary>
        public bool ShowCalculationsAsColumns
        {
            get
            {
                return this.PivotEngine.ShowCalculationsAsColumns;
            }
            set
            {
                this.PivotEngine.ShowCalculationsAsColumns = value;
            }
        }


        private bool enableValueEditing = false;
        /// <summary>
        /// Gets or sets a value indicating to enable or disable editing the cells
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool EnableValueEditing
        {
            get
            {
                return enableValueEditing;
            }
            set
            {
                enableValueEditing = value;
                if (value)
                {
                    this.Model.Options.ActivateCurrentCellBehavior = GridCellActivateAction.ClickOnCell;
                    if (this.EditManager == null)
                    {
                        this.EditManager = new PivotEditingManager(this);
                    }
                }
                else
                {
                    this.Model.Options.ActivateCurrentCellBehavior = GridCellActivateAction.None;
                    editManager = null;
                }
            }
        }

        PivotEditingManager editManager = null;
        /// <summary>
        /// Initiates a PivotEditingManager reference to PivotGridControl
        /// </summary>
        internal PivotEditingManager EditManager
        {
            get
            {
                return editManager;
            }

            set
            {
                editManager = value;
            }

        }


        private bool enableUpdating = false;
        /// <summary>
        /// Gets or sets a value indicating to enable or disable updating the cells
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool EnableUpdating
        {
            get
            {
                return enableUpdating;
            }
            set
            {
                enableUpdating = value;
                if (value)
                {
                    if (this.updateManager == null)
                    {
                        this.updateManager = new PivotUpdatingManager(this);
                    }
                }
                else
                {
                    updateManager = null;
                }
            }
        }

        PivotUpdatingManager updateManager = null;
        /// <summary>
        /// Initiates a PivotUpdatingManager reference to PivotGridControl
        /// </summary>
        internal PivotUpdatingManager UpdateManager
        {
            get
            {
                return updateManager;
            }

            set
            {
                updateManager = value;
            }

        }

        /// <summary>
        /// Gets or sets the PivotEngine
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PivotEngine PivotEngine
        {
            get
            {
                return engine;
            }
            set
            {
                engine = value;
                SynchronizeEngine();
            }
        }

        private void SynchronizeEngine()
        {
            if (!IsExternalEngine || PivotEngine == null) return;

            ItemSource = PivotEngine.DataSource;

            PivotEngine.PivotSchemaChanged += new PivotSchemaChangedEventHandler(engine_PivotSchemaChanged);

            foreach (var item in PivotEngine.PivotColumns.Where(item => !PivotColumns.Contains(item)))
            {
                PivotColumns.Add(item);
            }

            foreach (var item in PivotEngine.PivotRows.Where(item => !PivotRows.Contains(item)))
            {
                PivotRows.Add(item);
            }

            foreach (var item in PivotEngine.PivotCalculations.Where(item => !PivotCalculations.Contains(item)))
            {
                PivotCalculations.Add(item);
            }

            foreach (var item in PivotEngine.Filters.Where(item => !Filters.Contains(item)))
            {
                Filters.Add(item);
            }

           this.Refresh(true);
        }

        void engine_PivotSchemaChanged(object sender, PivotSchemaChangedArgs e)
        {
            this.SynchronizeGrid(e);
        }

        internal void SynchronizeGrid(PivotSchemaChangedArgs e)
        {
            if (!this.DeferLayoutUpdate || e.OverrideDeferLayoutUpdate)
            {
                if (this.PivotEngine.UseIndexedEngine)
                {
                    e.ChangeHints = SchemaChangeHints.None;
                }

                if (e.ChangeHints == SchemaChangeHints.GrandTotalVisibility)
                {
                    this.DeferLayoutUpdate = true;
                    this.Refresh(false);
                }
                else if (e.ChangeHints == SchemaChangeHints.CalculationChanged)
                {
                    this.PivotEngine.PopulateValueCells();
                    this.InvalidateCells();
                }
                else if (e.ChangeHints == SchemaChangeHints.HeadersChanged)
                {
                    this.PivotEngine.Populate();
                    this.InvalidateCells();
                }
                else
                {
                    this.Refresh(true);
                }

            }
        }
        /// <summary>
        /// Gets a collection of Syncfusion.PivotAnalysis.Base.FieldInfo objects that
        /// hold field names that you want to be visible in the engine. The names can
        /// be either public property names of the underlying data objects, or they can
        /// be expression field.
        /// </summary>
        /// <remarks>
        /// If your data contains fields that you do not want exposed to the pivoting
        /// process, then add the names of the properties you want to include to this
        /// list. All other fields will be excluded. If you leave this collection empty,
        /// the default behavior will be to make all public properties available for
        /// use in the pivot table.  To add an expression field, set the FieldInfo.FieldType
        /// to FieldsType.Expression and set FieldInfo.Expression to be a string holding
        /// a well formed expression defining the value that should appear in this field.
        /// </remarks>
        [Browsable(false)]
        public List<FieldInfo> AllowedFields
        {
            get
            {
                if (this.PivotEngine != null)
                    allowedFields = this.PivotEngine.AllowedFields;
                return allowedFields;
            }
        }


        /// <summary>
        /// Gets or sets whether the engine of this control is internally created or 
        /// supplied from external source
        /// </summary>
        [Browsable(false)]
        public bool IsExternalEngine
        {
            get
            {
                return _IsExtenalEngine;
            }
            private set
            {
                _IsExtenalEngine = value;
            }
        }

       bool freezeHeaders = false;
        /// <summary>
        /// Gets or sets whether the column, row headers should freeze or not
        /// </summary>
        [Category("Customization")]
        public bool FreezeHeaders
        {
            get
            {
                return this.freezeHeaders;
            }
            set
            {
                this.freezeHeaders = value;
            }
        }

        bool deferLayoutUpdate = false;
        /// <summary>
        /// Gets or sets whether the layout should be updated immediately after the 
        /// pivoting info update or it should wait for a Refresh() call.
        /// </summary>
        [Category("Customization")]
        public bool DeferLayoutUpdate
        {
            get
            {
                return this.deferLayoutUpdate;
            }
            set
            {
                this.deferLayoutUpdate = value;
            }
        }

        bool showGrandTotals = true;
        /// <summary>
        /// Gets or sets whether grand total calculations should be computed by the engine.
        /// </summary>
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        [Category("Customization")]
        public bool ShowGrandTotals
        {
            get
            {
                return this.showGrandTotals;
            }
            set
            {
                this.showGrandTotals = value;
                GrandTotalsRendering();

            }
        }
        private bool _statePersistenceEnabled = false;
        internal bool StatePersistenceEnabled
        {
            get
            {
                return _statePersistenceEnabled;
            }
            set
            {
                _statePersistenceEnabled = value;
            }
        }

        
        bool allowSelection = false;
        /// <summary>
        /// Gets or sets a value indicating whether to Allow Selection of Cells as Like in Excel
        /// </summary>
        /// <value><c>true</c> if [allow selection]; otherwise, <c>false</c>.</value>
        [Category("Customization")]
        public bool AllowSelection
        {
            get
            {
                return this.allowSelection;
            }
            set
            {
                this.allowSelection = value;
            }
        }

        #endregion

        #region [ Events ]

        public event ItemSourceChangedEventHandler ItemSourceChanged;

        public virtual event SelectionChanged SelectionChanged;

        public virtual event Expanding Expanding;

        public virtual event Expanded Expanded;

        public virtual event Collapsing Collapsing;

        public virtual event Collapsed Collapsed;

        public virtual event HyperlinkCellClick HyperlinkCellClick;

        public virtual event DataRefreshing DataRefreshing;

        public virtual event DataRefreshed DataRefreshed;

        // Need to implement after PivotSchemaDesigner
        //public virtual event GroupingBarLoaded GroupingBarLoaded;

        public virtual event EventHandler ShowDisabledGroupBackgroundPropertyChanged;

        #endregion

        #region [ PropertyChanged Events ]

        protected virtual void OnShowDisabledGroupBackgroundPropertyChanged(object sender, EventArgs e)
        {
            if (this.ShowDisabledGroupBackgroundPropertyChanged != null)
            {
                this.ShowDisabledGroupBackgroundPropertyChanged(this, e);
            }

           
        }
        void OnPivotRowsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            if (this.PivotEngine != null)
            {
                SynchronizePivotItems(e, true);
            }
        }


        void OnPivotColumnsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.PivotEngine != null)
            {
                SynchronizePivotItems(e, false);
            }
        }

        void OnCalculationsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.PivotEngine != null)
            {
                SynchronizeCalculations(e);
            }
        }

        void OnFiltersCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.PivotEngine != null)
            {
                SynchronizeFilters(e);
            }
        }

        void OnItemSourceChanged(object sender, ItemSourceChangedEventArgs e)
        {
            PivotGridControlBase gridControl = (PivotGridControlBase)sender;
            if (gridControl.PivotEngine != null)
            {
                gridControl.itemSource = e.NewValue;
                gridControl.PivotEngine.DataSource = gridControl.ItemSource;

                if (gridControl.ItemSourceChanged != null)
                {
                    gridControl.ItemSourceChanged(gridControl, new ItemSourceChangedEventArgs { OldValue = e.OldValue, NewValue = e.NewValue });
                }

                if (gridControl.UpdateManager != null)
                {
                    gridControl.UpdateManager.InitializeUpdatingManager(gridControl);
                }
            }
        }

        static void OnPivotEngineChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControlBase gridControl = null;
            gridControl.IsExternalEngine = true;
        }

        static void OnShowCalculationsAsColumnsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControlBase gridControl = null;
            if (gridControl.PivotEngine != null)
            {
                //gridControl.PivotEngine.ShowCalculationsAsColumns = gridControl.ShowCalculationsAsColumns;
            }
        }

        #endregion

        #region [ Overrides ]



        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        protected override void  OnCreateControl()
        {
            base.OnCreateControl();
            this.PivotRows.Cast<PivotItem>().Where(i => String.IsNullOrEmpty(i.FieldHeader)).Select(i => i.FieldHeader = i.FieldMappingName).ToList();
            this.PivotColumns.Cast<PivotItem>().Where(i => String.IsNullOrEmpty(i.FieldHeader)).Select(i => i.FieldHeader = i.FieldMappingName).ToList();
            this.PivotFields.Cast<PivotItem>().Where(i => String.IsNullOrEmpty(i.FieldHeader)).Select(i => i.FieldHeader = i.FieldMappingName).ToList();
            this.PivotCalculations.Cast<PivotComputationInfo>().Where(i => String.IsNullOrEmpty(i.FieldHeader)).Select(i => i.FieldHeader = i.FieldName).ToList();            
          

            if (this.PivotEngine != null)
            {
                if (this.PivotEngine.DataSource == null && this.ItemSource != null)
                    this.PivotEngine.DataSource = this.ItemSource;
            }
            
        }
        
        protected override void OnDrawCell(GridDrawCellEventArgs e)
        {
            if (!this.IsDesignMode())
            {
                if (PivotEngine.RowCount != 0)
                {
                    this.Model.RowCount = PivotEngine.RowCount - 1;
                    this.Model.ColCount = PivotEngine.ColumnCount - 1;
                }
                else
                {
                    this.Model.RowCount = 1;
                    this.Model.ColCount = 1;
                }
            }
            else
            {
                this.Model.RowCount = 1;
                this.Model.ColCount = 1;
            }
            base.OnDrawCell(e);

        }
        
        PivotGridControl pivot = null;
        /// <summary>
        /// Used internally.
        /// </summary>
        internal PivotGridControl Pivot
        {
            get
            {
                return pivot;
            }
            set
            {
                pivot = value;
            }
        }
        //Overriden to handle the PivotGridControl loading issues.
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            this.UpdateScrollBars();
            if (Pivot != null)
            {
                if (Pivot.ShowPivotTableFieldList)
                {
                    pivot.Schema();
                }
            }
            base.OnRightToLeftChanged(e);
        }
        private static string GetResourceDictionaryFullyQualifiedName(string visualStyleName, ref string prefix, ref string suffix)
        {
            return string.Empty;
        }

        #endregion

        #region [ Public Methods ]
      
        /// <summary>
        /// Populates the default property fields.
        /// </summary>
        public void PopulateDefaultPropertyFields()
        {
            if(this.PivotEngine != null)
                this.PivotEngine.PopulateDefaultPropertyFields();
        }

        /// <summary>
        /// Resets the pivot data (resets the pivot engine, pivot rows, pivot columns, pivot fields, pivot calculations and items source).
        /// </summary>
        public void ResetPivotData()
        {
            this.PivotCalculations.Clear();
            this.PivotColumns.Clear();
            this.PivotFields.Clear();
            this.PivotRows.Clear();
            if (this.PivotEngine != null)
            {
                this.PivotEngine.Reset();
            }
            
        }


        /// <summary>
        /// Refresh the control without affecting persistence of expand/collapse states.
        /// </summary>
        internal void InternalRefresh()
        {
            if (this.PivotEngine != null )//&& !DesignerProperties.GetIsInDesignMode(this))
            {
                this.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
            }
        }


        #region [ Expand/Collapse methods ]
        /// <summary>
        /// Raises expanding and expanded events for the provided cell info.
        /// </summary>
        /// <param name="cellInfo">Cell info to be expanded.</param>
        /// <param name="row">Row index of the expander cell.</param>
        /// <param name="column">Column index of the expander cell.</param>
        private void RaiseExpandEvents(PivotCellInfo cellInfo, int row, int column)
        {
            ExpandingEventArgs args = new ExpandingEventArgs(cellInfo);
            this.RaiseOnExpanding(args, cellInfo);
            if (!args.Cancel)
            {
                SetExpandCollapse(row, column);
                ExpandedEventArgs arg = new ExpandedEventArgs(cellInfo);
                this.RaiseOnExpanded(arg);
            }
        }

        /// <summary>
        /// Raises collapsing and collapsed events for the provided cell info.
        /// </summary>
        /// <param name="cellInfo">Cell info to be expanded.</param>
        /// <param name="styleInfo">Style of the cell.</param>
        /// <param name="row">Row index of the expander cell.</param>
        /// <param name="column">Column index of the expander cell.</param>
        private void RaiseCollapseEvents(PivotCellInfo cellInfo, GridStyleInfo styleInfo, int row, int column)
        {
            CollapsingEventArgs args = new CollapsingEventArgs(cellInfo);
            this.RaiseOnCollapsing(args, styleInfo);
            if (!args.Cancel)
            {
                SetExpandCollapse(row, column);
                CollapsedEventArgs arg = new CollapsedEventArgs(cellInfo);
                this.RaiseOnCollapsed(arg);
            }
        }

        /// <summary>
        /// Expands all the grouped rows.
        /// </summary>
        public void ExpandAllRowGroups()
        {
            int row, column;
            PivotCellInfo cellInfo = null;
            GridStyleInfo styleInfo = null;
            this.Model.BeginUpdate();
            if (this.PivotEngine != null)
            {
                for (row = this.PivotEngine.PivotColumns.Count + (!this.PivotEngine.ShowCalculationsAsColumns ? 1 : 0); row < this.PivotEngine.RowCount; row++)
                {
                    for (column = 0; column < this.PivotEngine.PivotRows.Count - 1; column++)
                    {
                        cellInfo = this.PivotEngine[row, column];
                        styleInfo = this.Model[row + 1, column + 1];
                        if (cellInfo != null && cellInfo.UniqueText != null && cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.TotalCell) && styleInfo.CellType == "PivotGridExpandCell")
                        {
                            RaiseExpandEvents(cellInfo, row + 1, column + 1);
                        }
                    }
                }
            }
            this.Model.EndUpdate();
        }

        /// <summary>
        /// Expands all the grouped columns.
        /// </summary>
        public void ExpandAllColumnGroups()
        {
            int row, column;
            PivotCellInfo cellInfo = null;
            GridStyleInfo styleInfo = null;
            this.Model.BeginUpdate();
            if (this.PivotEngine != null)
            {
                for (column = this.PivotEngine.PivotRows.Count; column < this.PivotEngine.ColumnCount; column++)
                {
                    for (row = 0; row < this.PivotEngine.PivotColumns.Count - 1; row++)
                    {
                        cellInfo = this.PivotEngine[row, column];
                        styleInfo = this.Model[row + 1, column + 1];
                        if (cellInfo != null && cellInfo.UniqueText != null && cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) && styleInfo.CellType == "PivotGridExpandCell")
                        {
                            RaiseExpandEvents(cellInfo, row + 1, column + 1);
                        }
                    }
                }
            }
            this.Model.EndUpdate();
        }

        /// <summary>
        /// Expands all the groups.
        /// </summary>
        public void ExpandAllGroups()
        {
            this.Model.BeginUpdate();
            ExpandAllRowGroups();
            ExpandAllColumnGroups();
            this.Model.EndUpdate();
        }

        /// <summary>
        /// Collapses all the grouped rows.
        /// </summary>
        public void CollapseAllRowGroups()
        {
            this.Model.BeginUpdate();
            int row, column;
            PivotCellInfo cellInfo = null;
            GridStyleInfo styleInfo = null;
            if (this.PivotEngine != null)
            {
                for (row = this.PivotEngine.PivotColumns.Count + (!this.PivotEngine.ShowCalculationsAsColumns ? 1 : 0); row < this.PivotEngine.RowCount; row++)
                {
                    for (column = 0; column < this.PivotEngine.PivotRows.Count - 1; column++)
                    {
                        cellInfo = this.PivotEngine[row, column];
                        styleInfo = this.Model[row + 1, column + 1];
                        if (cellInfo != null && cellInfo.UniqueText != null && cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell) && styleInfo.CellType == "PivotGridExpandCell")
                        {
                            object key = GetCellHashKey(row + 1, column + 1);
                            if (!collapsedNodes.Contains(key))
                            {
                                RaiseCollapseEvents(cellInfo, styleInfo, row + 1, column + 1);
                            }
                        }
                    }
                }
            }
            this.Model.EndUpdate();
        }

        /// <summary>
        /// Collapses all the grouped columns.
        /// </summary>
        public void CollapseAllColumnGroups()
        {
            this.Model.BeginUpdate();
            int row, column;
            PivotCellInfo cellInfo = null;
            GridStyleInfo styleInfo = null;
            if (this.PivotEngine != null)
            {
                for (column = this.PivotEngine.PivotRows.Count; column < this.PivotEngine.ColumnCount; column++)
                {
                    for (row = 0; row < this.PivotEngine.PivotColumns.Count - 1; row++)
                    {
                        cellInfo = this.PivotEngine[row, column];
                        styleInfo = this.Model[row + 1, column + 1];
                        if (cellInfo != null && cellInfo.UniqueText != null && cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.ExpanderCell) && styleInfo.CellType == "PivotGridExpandCell")
                        {
                            object key = GetCellHashKey(row + 1, column + 1);
                           if (!collapsedNodes.Contains(key))
                           {
                               RaiseCollapseEvents(cellInfo, styleInfo, row + 1, column + 1);
                           }
                        }
                    }
                }
            }
            this.Model.EndUpdate();
        }

        /// <summary>
        /// Collapses all the groups.
        /// </summary>
        public void CollapseAllGroups()
        {
            this.Model.BeginUpdate();
            CollapseAllRowGroups();
            CollapseAllColumnGroups();
            this.Model.EndUpdate();
        }

        /// <summary>
        /// Expands the row for the specified unique text.
        /// </summary>
        /// <param name="uniqueText">An unique way of representation for the header cell which has a parent cell <see cref="PivotCellInfo.UniqueText"/>.</param>
        public void ExpandRow(string uniqueText)
        {
            int row, column;
            PivotCellInfo cellInfo = null;
            GridStyleInfo styleInfo = null;
            string uniqueString = uniqueText;
            string delimeter = new string((char)131, 1);
            this.Model.BeginUpdate();
            if (this.PivotEngine != null)
            {
                for (row = this.PivotEngine.PivotColumns.Count + (!this.PivotEngine.ShowCalculationsAsColumns ? 1 : 0); row < this.PivotEngine.RowCount; row++)
                {
                    for (column = 0; column < this.PivotEngine.PivotRows.Count - 1; column++)
                    {
                        cellInfo = this.PivotEngine[row, column];
                        styleInfo = this.Model[row + 1, column + 1];
                        uniqueString = uniqueText + delimeter + this.PivotEngine.PivotRows[column].TotalHeader;
                        if (cellInfo != null && cellInfo.UniqueText != null && cellInfo.UniqueText.Equals(uniqueString)
                            && cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.TotalCell) && styleInfo.CellType == "PivotGridExpandCell")
                        {
                            RaiseExpandEvents(cellInfo, row + 1, column + 1);
                        }
                    }
                }
            }
            this.Model.EndUpdate();
        }

        /// <summary>
        /// Expand the rows for provided array of unique text.
        /// </summary>
        /// <param name="uniqueText">Array of Unique text of the Expander headers.</param>
        public void ExpandRow(List<string> list)
        {
            this.Model.BeginUpdate();
            foreach (string uniqueText in list)
                ExpandRow(uniqueText);
            this.Model.EndUpdate();
        }

        /// <summary>
        /// Collapses the row for the specified unique text.
        /// </summary>
        /// <param name="uniqueText">An unique way of representation for the header cell which has a parent cell <see cref="PivotCellInfo.UniqueText"/>.</param>
        public void CollapseRow(string uniqueText)
        {
            this.Model.BeginUpdate();
            int row, column;
            PivotCellInfo cellInfo = null;
            GridStyleInfo styleInfo = null;
            if (this.PivotEngine != null)
            {
                for (row = this.PivotEngine.PivotColumns.Count + (!this.PivotEngine.ShowCalculationsAsColumns ? 1 : 0); row < this.PivotEngine.RowCount; row++)
                {
                    for (column = 0; column < this.PivotEngine.PivotRows.Count - 1; column++)
                    {
                        cellInfo = this.PivotEngine[row, column];
                        styleInfo = this.Model[row + 1, column + 1];
                        if (cellInfo != null && cellInfo.UniqueText != null && cellInfo.UniqueText.Equals(uniqueText) 
                            && cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell) && styleInfo.CellType == "PivotGridExpandCell")
                        {
                            object key = GetCellHashKey(row + 1, column + 1);
                            if (!collapsedNodes.Contains(key))
                            {
                                RaiseCollapseEvents(cellInfo, styleInfo, row + 1, column + 1);
                            }
                        }
                    }
                }
            }
            this.Model.EndUpdate();
        }

        /// <summary>
        /// Collapse the rows for the specified unique text.
        /// </summary>
        /// <param name="uniqueText">Array of Unique text of the Expander headers.</param>
        public void CollapseRow(List<string> list)
        {
            this.Model.BeginUpdate();
            foreach (string uniqueText in list)
                CollapseRow(uniqueText);
            this.Model.EndUpdate();
        }

        /// <summary>
        /// Expands the column for the specified unique text.
        /// </summary>
        /// <param name="uniqueText">An unique way of representation for the header cell which has a parent cell <see cref="PivotCellInfo.UniqueText"/>.</param>
        public void ExpandColumn(string uniqueText)
        {
            int row, column;
            PivotCellInfo cellInfo = null;
            GridStyleInfo styleInfo = null;
            string uniqueString = uniqueText;
            string delimeter = new string((char)131, 1);
            this.Model.BeginUpdate();
            if (this.PivotEngine != null)
            {
                for (column = this.PivotEngine.PivotRows.Count; column < this.PivotEngine.ColumnCount; column++)
                {
                    for (row = 0; row < this.PivotEngine.PivotColumns.Count - 1; row++)
                    {
                        cellInfo = this.PivotEngine[row, column];
                        styleInfo = this.Model[row + 1, column + 1];
                        uniqueString = uniqueText + delimeter + this.PivotEngine.PivotColumns[row].TotalHeader;
                        if (cellInfo != null && cellInfo.UniqueText != null && cellInfo.UniqueText.Equals(uniqueString) && cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) && styleInfo.CellType == "PivotGridExpandCell")
                        {
                            RaiseExpandEvents(cellInfo, row + 1, column + 1);
                        }
                    }
                }
            }
            this.Model.EndUpdate();
        }

        /// <summary>
        /// Expand the Columns for provided array of unique text.
        /// </summary>
        /// <param name="uniqueText">Array of Unique text of the Expander headers.</param>
        public void ExpandColumn(List<string> list)
        {
            this.Model.BeginUpdate();
            foreach (string uniqueText in list)
                ExpandColumn(uniqueText);
            this.Model.EndUpdate();
        }

        /// <summary>
        /// Collapses the column for the specified unique text.
        /// </summary>
        /// <param name="uniqueText">An unique way of representation for the header cell which has a parent cell <see cref="PivotCellInfo.UniqueText"/>.</param>
        public void CollapseColumn(string uniqueText)
        {
            this.Model.BeginUpdate();
            int row, column;
            PivotCellInfo cellInfo = null;
            GridStyleInfo styleInfo = null;
            if (this.PivotEngine != null)
            {
                for (column = this.PivotEngine.PivotRows.Count; column < this.PivotEngine.ColumnCount; column++)
                {
                    for (row = 0; row < this.PivotEngine.PivotColumns.Count - 1; row++)
                    {
                        cellInfo = this.PivotEngine[row, column];
                        styleInfo = this.Model[row + 1, column + 1];
                        if (cellInfo != null && cellInfo.UniqueText != null && cellInfo.UniqueText.Equals(uniqueText) 
                            && cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.ExpanderCell) && styleInfo.CellType == "PivotGridExpandCell")
                        {
                            object key = GetCellHashKey(row + 1, column + 1);
                            if (!collapsedNodes.Contains(key))
                            {
                                RaiseCollapseEvents(cellInfo, styleInfo, row + 1, column + 1);
                            }
                        }
                    }
                }
            }
            this.Model.EndUpdate();
        }

        /// <summary>
        /// Collapse the Columns for the specified unique text.
        /// </summary>
        /// <param name="uniqueText">Array of Unique text of the Expander headers.</param>
        public void CollapseColumn(List<string> list)
        {
            this.Model.BeginUpdate();
            foreach (string uniqueText in list)
                CollapseColumn(uniqueText);
            this.Model.EndUpdate();
        }

        /// <summary>
        /// Invalidates the cells.
        /// </summary>
        public void InvalidateCells()
        {
            this.RefreshRange(GridRangeInfo.Table());
        }

        /// <summary>
        /// Invalidates a range of cells.
        /// </summary>
        /// <param name="range">range to be invalidated</param>
        public void InvalidateCells(GridRangeInfo range)
        {
            this.RefreshRange(range);
        }
        #endregion

        #endregion

        #region [ Helper Methods ]


        internal void RemoveCollapsedNodes()
        {
           
            this.collapsedNodes.Clear();
            this.Model.RowHeightEntries.Clear();
            this.Model.ColWidthEntries.Clear();
        }

        internal void ApplyRowCols()
        {
            if (this.PivotEngine.DataSource != null && this.PivotCalculations.Count > 0)
            {
                this.PivotEngine.Populate();
            }
            this.Model.CoveredRanges.Clear();
            this.Model.CoveredRanges.Ranges.Clear();
            this.Model.CoveredRanges.UpdateList();
            this.Model.CoveredRanges.ResetCache();

            #region alternate
            //for (int i = 0; i < this.PivotEngine.CoveredRanges.Count; i++)
            //{
            //    this.Model.CoveredRanges.Add(GridRangeInfo.Cells(this.PivotEngine.CoveredRanges[i].Top + 1, this.PivotEngine.CoveredRanges[i].Left + 1, this.PivotEngine.CoveredRanges[i].Bottom + 1, this.PivotEngine.CoveredRanges[i].Right + 1));
            //} 
            #endregion

            foreach (var range in this.PivotEngine.CoveredRanges)
            {
                this.Model.CoveredRanges.Add(GridRangeInfo.Cells(range.Top + 1, range.Left + 1, range.Bottom + 1, range.Right + 1));
            }

            if (!showGrandTotals)
            {
                this.Model.HideCols[this.PivotEngine.RowCount -1 ]=true;
                this.Model.HideRows[this.PivotEngine.ColumnCount - 1] = true;

            }
            this.Model.Refresh();
        }

        private void InitTargetGrid()
        {
            if (FreezeHeaders)
            {
                int freezecols = PivotEngine.PivotRows.Count;
                int freezeRows = PivotEngine.PivotCalculations.Count > 1 ? PivotEngine.PivotColumns.Count + 1 : PivotEngine.PivotColumns.Count;
                this.Model.Rows.FreezeRange(1, freezeRows);
                this.Model.Cols.FreezeRange(1, freezecols);
            }

        }

        internal void GrandTotalsRendering()
        {
            int row, column;
            PivotCellInfo cellInfo = null;
            GridStyleInfo styleInfo = null;
            //Shows the pivotGrid SubTotals if ShowSubTotals is true
            if (showGrandTotals)
            {
                if (this.PivotEngine != null)
                {
                    for (row = (this.PivotEngine.PivotColumns.Count + (this.PivotEngine.PivotCalculations.Count > 1 ? 1 : 0)); row < this.PivotEngine.RowCount; row++)
                    {
                        for (column = 1; column <= this.PivotEngine.PivotRows.Count; column++)
                        {
                            cellInfo = this.PivotEngine[row, column - 1];
                            if (cellInfo != null)
                            {
                                styleInfo = this.Model[row + 1, column];
                                if (cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) || cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell))
                                {

                                    this.Model.HideRows.SetRange(this.Model.RowCount, this.Model.RowCount, false);
                                }
                            }
                        }
                    }
                    for (column = this.PivotEngine.PivotRows.Count + 1; column < this.PivotEngine.ColumnCount; column++)
                    {
                        for (row = 1; row < this.PivotEngine.PivotColumns.Count + (this.PivotCalculations.Count > 1 ? 1 : 0); row++)
                        {
                            cellInfo = this.PivotEngine[row - 1, column];
                            if (cellInfo != null)
                            {
                                styleInfo = this.Model[row, column + 1];
                                if (cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell))
                                {
                                    var coveredcellInfo = this.Model.CoveredRanges.FindRange(row, column + 1);
                                    if (coveredcellInfo != null)
                                    {
                                        this.Model.HideCols.SetRange(coveredcellInfo.Left, coveredcellInfo.Right, false);
                                    }
                                }
                            }
                        }
                    }

                    this.InvalidateCells();
                }
                else
                {
                    throw new PivotGridException("Invalid operation, PivotEngine is null");
                }
            }

            //Hides the PivotGrid GrandTotals Field when ShowSubTotals is false
            else
            {
                if (this.PivotEngine != null)
                {
                    for (row = (this.PivotEngine.PivotColumns.Count + (this.PivotEngine.PivotCalculations.Count > 1 ? 1 : 0)); row < this.PivotEngine.RowCount; row++)
                    {
                        for (column = 1; column <= this.PivotEngine.PivotRows.Count; column++)
                        {
                            cellInfo = this.PivotEngine[row, column - 1];
                            if (cellInfo != null)
                            {
                                styleInfo = this.Model[row, column];
                                if (cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) || cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell))
                                {
                                    this.Model.HideRows.SetRange(this.Model.RowCount, this.Model.RowCount, true);
                                }
                            }
                        }
                    }
                    for (column = this.PivotEngine.PivotRows.Count; column < this.PivotEngine.ColumnCount; column++)
                    {
                        for (row = 1; row < this.PivotEngine.PivotColumns.Count; row++)
                        {
                            cellInfo = this.PivotEngine[row - 1, column];
                            if (cellInfo != null)
                            {
                                styleInfo = this.Model[row, column];
                                if (cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell))
                                {
                                    var coveredcellsInfo = this.Model.CoveredRanges.FindRange(row , column+1);
                                    var parentCellInfo = this.Model.CoveredRanges.FindRange(row, column);
                                    if (coveredcellsInfo != null)
                                    {
                                        this.Model.HideCols.SetRange(coveredcellsInfo.Left, coveredcellsInfo.Right, true);
                                    }
                                }
                            }
                        }
                    }
                    this.InvalidateCells();
                }
                else
                {
                    throw new PivotGridException("Invalid operation, PivotEngine is null");
                }
            }
        }

        private bool isHidden = false;
        internal void SubTotalsRendering()
        {
            int row, column;
            PivotCellInfo cellInfo = null;
            GridStyleInfo styleInfo = null;
            //Shows the pivotGrid SubTotals if ShowSubTotals is true
            if (ShowSubTotals)
            {
                if (isHidden)
                {
                    if (this.PivotEngine != null)
                    {
                        for (row = (this.PivotEngine.PivotColumns.Count + (this.PivotEngine.PivotCalculations.Count > 1 ? 1 : 0)); row < this.PivotEngine.RowCount; row++)
                        {
                            for (column = 1; column < this.PivotEngine.PivotRows.Count; column++)
                            {
                                cellInfo = this.PivotEngine[row, column - 1];
                                if (cellInfo != null)
                                {
                                    styleInfo = this.Model[row+1, column];
                                    if (cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.TotalCell))
                                    {
                                        var coveredcellInfo = this.Model.CoveredRanges.FindRange(row+1, column);
                                        if (coveredcellInfo != null)
                                        {
                                            this.Model.HideRows.SetRange(coveredcellInfo.Top, coveredcellInfo.Bottom, false);
                                            this.HiddenSubTotalsRowGroups.Remove(this.HiddenRowGroups.Where(i => i.From == coveredcellInfo.Top && i.To == coveredcellInfo.Bottom).FirstOrDefault());
                                        }
                                    }
                                }
                            }
                        }
                        for (column = this.PivotEngine.PivotRows.Count+1 ; column < this.PivotEngine.ColumnCount; column++)
                        {
                            for (row = 1; row < this.PivotEngine.PivotColumns.Count +(this.PivotCalculations.Count>1?1:0); row++)
                            {
                                if (column == 10)
                                {
                                }
                                cellInfo = this.PivotEngine[row - 1, column];
                                if (cellInfo != null)
                                {
                                    styleInfo = this.Model[row, column+1];
                                    if (cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.TotalCell))
                                    {
                                        var coveredcellInfo = this.Model.CoveredRanges.FindRange(row, column+1);
                                        if (coveredcellInfo != null)
                                        {
                                            this.Model.HideCols.SetRange(coveredcellInfo.Left, coveredcellInfo.Right, false);
                                            this.HiddenSubTotalsColumnGroups.Remove(this.HiddenColumnGroups.Where(i => i.From == coveredcellInfo.Left && i.To == coveredcellInfo.Right).FirstOrDefault());
                                        }
                                    }
                                }
                            }
                        }

                        this.InvalidateCells();
                        isHidden = false;
                    }
                    else
                    {
                        throw new PivotGridException("Invalid operation, PivotEngine is null");
                    }
                }
            }

            //Hides the PivotGrid SubTotals Field when ShowSubTotals is false
            else
            {
                if (!isHidden)
                {
                    if (this.PivotEngine != null)
                    {
                        PivotItem pivotItem = null;
                        for (row = (this.PivotEngine.PivotColumns.Count + (this.PivotEngine.PivotCalculations.Count > 1 ? 1 : 0)); row < this.PivotEngine.RowCount; row++)
                        {
                            for (column = 1; column < this.PivotEngine.PivotRows.Count; column++)
                            {
                                cellInfo = this.PivotEngine[row, column - 1];
                                if (cellInfo != null)
                                {
                                    styleInfo = this.Model[row, column];
                                    if (cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.TotalCell))
                                    {
                                        var coveredcellsInfo = this.Model.CoveredRanges.FindRange(row + 1, column);
                                        var parentCellInfo = this.Model.CoveredRanges.FindRange(row, column);
                                        pivotItem = this.PivotEngine.PivotRows[column];
                                        if (coveredcellsInfo != null)
                                        {
                                            if (parentCellInfo.Height >= coveredcellsInfo.Height)
                                            {
                                                this.Model.HideRows.SetRange(coveredcellsInfo.Top, coveredcellsInfo.Bottom, true);
                                                if (!this.HiddenSubTotalsRowGroups.Has(new HiddenGroup(coveredcellsInfo.Top, coveredcellsInfo.Bottom, coveredcellsInfo.Left, cellInfo.FormattedText, pivotItem.TotalHeader)))
                                                {
                                                    this.HiddenSubTotalsRowGroups.Add(new HiddenGroup(coveredcellsInfo.Top, coveredcellsInfo.Bottom, coveredcellsInfo.Left, cellInfo.FormattedText, pivotItem.TotalHeader));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        for (column = this.PivotEngine.PivotRows.Count; column < this.PivotEngine.ColumnCount; column++)
                        {
                            for (row = 1; row < this.PivotEngine.PivotColumns.Count; row++)
                            {
                                cellInfo = this.PivotEngine[row - 1, column];
                                if (cellInfo != null)
                                {
                                    styleInfo = this.Model[row, column];
                                    if (cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.TotalCell))
                                    {
                                         var parentCellInfo = this.Model.CoveredRanges.FindRange(row, column);
                                        var coveredcellsInfo = this.Model.CoveredRanges.FindRange(row, column + 1);
                                        pivotItem = this.PivotEngine.PivotColumns[row];
                                        if (coveredcellsInfo != null)
                                        {
                                            if (parentCellInfo.Width > coveredcellsInfo.Width)
                                            {
                                                this.Model.HideCols.SetRange(coveredcellsInfo.Left, coveredcellsInfo.Right, true);
                                                if (!this.HiddenSubTotalsColumnGroups.Has(new HiddenGroup(coveredcellsInfo.Left, coveredcellsInfo.Right, coveredcellsInfo.Top, cellInfo.FormattedText, pivotItem.TotalHeader)))
                                                {
                                                    this.HiddenSubTotalsColumnGroups.Add(new HiddenGroup(coveredcellsInfo.Left, coveredcellsInfo.Right, coveredcellsInfo.Top, cellInfo.FormattedText, pivotItem.TotalHeader));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        this.InvalidateCells();
                        isHidden = true;
                    }
                    else
                    {
                        throw new PivotGridException("Invalid operation, PivotEngine is null");
                    }
                }
            }
            //EnsureHiddenGroupsShown();
        }
        /// <summary>
        /// Adds the pivotitem as collection for internal check in PivotschemaDesigner.
        /// </summary>
        /// <param name="item">The PivotItem to be added to collection</param>
        private void QueryItemAvailability(PivotItem item)
        {
            if (item != null)
            {
                if (!collectionTablelist.ContainsKey(item.FieldMappingName))
                    collectionTablelist.Add(item.FieldMappingName, item);
                if (!fieldNameCollectionTablelist.ContainsKey(item.FieldHeader))
                    fieldNameCollectionTablelist.Add(item.FieldHeader, item);
                if (hitCount < fieldNameCollectionTablelist.Count)
                {
                    foreach (string passedKey in this.propertyDescTableList.Keys)
                    {
                        if (passedKey == item.FieldMappingName)
                        {
                            completeTablelist.Remove(passedKey);
                            if (!string.IsNullOrEmpty(item.FieldHeader))
                                completeTablelist.Add(item.FieldHeader, item);
                            else
                                completeTablelist.Add(item.FieldMappingName, item);
                        }
                    }
                    hitCount++;
                }
            }
        }
        PropertyDescriptorCollection pdc = null; 
        int hitCount = 0;
        /// <summary>
        /// used for populating completeTablelist dictionary with fieldHeader/ fieldMapping Name as key.
        /// </summary>
        private void populateCollection()
        {
            if (this.ItemSource is IList)
            {
                this.list = this.ItemSource as IList;
            }
            else if (this.ItemSource is DataTable)
            {
                this.list = ((DataTable)this.ItemSource).DefaultView;
            }
            ITypedList typedList = list as ITypedList;

            if (typedList == null)
            {
                pdc = this.PivotEngine.ItemProperties;
            }
            else
            {
                pdc = typedList.GetItemProperties(null);
            }
            foreach (PropertyDescriptor pd in pdc)
            {
                PivotItem item = new PivotItem();
                item.FieldMappingName = pd.Name;
                propertyDescTableList.Add(pd.Name, item);
                completeTablelist.Add(pd.Name, item);
            }
            isPopulated = true;
        }
        bool isPopulated = false;
        bool isDuplicated = false;
        internal Dictionary<string, PivotItem> fieldNameCollectionTablelist = new Dictionary<string, PivotItem>();
        internal Dictionary<string, PivotItem> collectionTablelist = new Dictionary<string, PivotItem>();
        internal Dictionary<string, PivotComputationInfo> pivotComputationCollection = new Dictionary<string, PivotComputationInfo>();
        internal Dictionary<string, PivotItem> propertyDescTableList = new Dictionary<string, PivotItem>();IList list;
        internal Dictionary<string, PivotItem> completeTablelist = new Dictionary<string, PivotItem>();
        public void SynchronizePivotItems(System.Collections.Specialized.NotifyCollectionChangedEventArgs e, bool isRow)
        {
            if(!isPopulated)
            {
                populateCollection();
            }
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        foreach (PivotItem item in e.NewItems)
                        {
                            if (item!=null && string.IsNullOrEmpty(item.FieldHeader))
                                item.FieldHeader = item.FieldMappingName;
                            if (isRow)
                            {
                                isDuplicated = (from collection in this.PivotEngine.PivotRows.Where(l => l.FieldMappingName == item.FieldMappingName)
                                                select collection).Any();
                                if (!isDuplicated)
                                    this.PivotEngine.InsertRowPivot(e.NewStartingIndex, item);
                                else
                                    this.PivotRows.Remove(item);
                                isDuplicated = false;
                            }
                            else
                            {
                                isDuplicated = (from collection in this.PivotEngine.PivotColumns.Where(l => l.FieldMappingName == item.FieldMappingName)
                                                select collection).Any();
                                if (!isDuplicated)
                                    this.PivotEngine.InsertColumnPivot(e.NewStartingIndex, item);
                                else
                                    this.PivotColumns.Remove(item);
                                isDuplicated = false;
                            }
                            QueryItemAvailability(item);
                        }
                        
                        break;
                    }
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    {
                        foreach (PivotItem item in e.OldItems)
                        {
                            if (isRow)
                                this.PivotEngine.RemoveRowPivot(item);
                            else
                                this.PivotEngine.RemoveColumnPivot(item);
                        }

                        foreach (PivotItem item in e.NewItems)
                        {
                            if (isRow)
                                this.PivotEngine.InsertRowPivot(e.NewStartingIndex, item);
                            else
                                this.PivotEngine.InsertColumnPivot(e.NewStartingIndex, item);
                            QueryItemAvailability(item);
                        }
                    }
                   
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        foreach (PivotItem item in e.OldItems)
                        {
                            if (isRow)
                                this.PivotEngine.RemoveRowPivot(item);
                            else
                                this.PivotEngine.RemoveColumnPivot(item);
                        }
                        break;
                    }
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    {
                        if (isRow)
                            this.PivotEngine.PivotRows = this.PivotRows.ToList<PivotItem>();
                        else
                            this.PivotEngine.PivotColumns = this.PivotColumns.ToList<PivotItem>();
                        break;
                    }
                default:
                    break;
            }
        }

        public void SynchronizeCalculations(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        foreach (PivotComputationInfo item in e.NewItems)
                        {
                            if (String.IsNullOrEmpty(item.FieldHeader)) item.FieldHeader = item.FieldName;
                            this.PivotEngine.InsertPivotCalculation(e.NewStartingIndex, item);
                            if (!this.pivotComputationCollection.ContainsKey(item.FieldHeader))
                                this.pivotComputationCollection.Add(item.FieldHeader, item);
                        }
                        break;
                    }
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    {
                        foreach (PivotComputationInfo item in e.NewItems)
                        {
                            this.PivotEngine.RemovePivotCalculation(item);
                        }

                        foreach (PivotComputationInfo item in e.NewItems)
                        {
                            this.PivotEngine.InsertPivotCalculation(e.NewStartingIndex, item);
                        }
                    }
                    
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        foreach (PivotComputationInfo item in e.OldItems)
                        {
                            this.PivotEngine.RemovePivotCalculation(item);
                        }
                      
                        break;
                    }

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    {
                        this.PivotEngine.PivotCalculations = this.PivotCalculations.ToList<PivotComputationInfo>();
                        
                        break;
                    }
                default:
                    break;
            }

            ApplyRowCols(); // TO populate cells only if calculation is not empty
        }

        public void SynchronizeFilters(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        foreach (FilterExpression item in e.NewItems)
                        {
                            this.PivotEngine.InsertFilter(e.NewStartingIndex, item);
                            this.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs() { ChangeHints = SchemaChangeHints.CalculationChanged });
                        }
                        break;
                    }
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        foreach (FilterExpression item in e.OldItems)
                        {
                            this.PivotEngine.RemoveFilter(item);
                        }
                        break;
                    }
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    {
                        this.PivotEngine.Filters = this.Filters.ToList<FilterExpression>();
                        this.PivotEngine.Filters.Clear();
                        this.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs() { ChangeHints = SchemaChangeHints.CalculationChanged });
                        break;
                    }
                default:
                    break;
            }
            
            ApplyRowCols();
        }

        /// <summary>
        /// Raises the Grid Selection event.
        /// </summary>
        /// <param name="SelectionChangingEventArgs">The <see cref="Syncfusion.Windows.Controls.PivotGrid.PivotGridSelectionChangedEventArgs"/> instance containing the event data.</param>
        internal void RaiseSelectionEvent(PivotGridSelectionChangedEventArgs SelectionChangingEventArgs)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, SelectionChangingEventArgs);
            }
        }

        /// <summary>
        /// Raises the OnExpanding event.
        /// </summary>
        /// <param name="expandingEventArgs">The <see cref="Syncfusion.Windows.Controls.PivotGrid.ExpandingEventArgs"/> instance containing the event data.</param>
        public void RaiseOnExpanding(ExpandingEventArgs expandingEventArgs, PivotCellInfo OriginalCell)
        {
            if (Expanding != null)
            {
                Expanding(this, expandingEventArgs);
                if (!expandingEventArgs.Cancel)
                {
                    GridStyleInfo styleInfo = expandingEventArgs.PivotCellInfo.Tag as GridStyleInfo;
                
                    this.RaiseOnExpanded(new ExpandedEventArgs(OriginalCell));
                }
            }
            else
            {
                GridStyleInfo styleInfo = expandingEventArgs.PivotCellInfo.Tag as GridStyleInfo;
               
                this.RaiseOnExpanded(new ExpandedEventArgs(OriginalCell));
            }
        }

        /// <summary>
        /// Raises the on OnExpanded Event.
        /// </summary>
        /// <param name="expandedEventArgs">The <see cref="Syncfusion.Windows.Controls.PivotGrid.ExpandedEventArgs"/> instance containing the event data.</param>
        internal void RaiseOnExpanded(ExpandedEventArgs expandedEventArgs)
        {
            if (Expanded != null)
            {
                Expanded(this, expandedEventArgs);
            }

          

        }

        /// <summary>
        /// Raises the OnCollapsing Event.
        /// </summary>
        /// <param name="collapsingEventArgs">The <see cref="Syncfusion.Windows.Controls.PivotGrid.CollapsingEventArgs"/> instance containing the event data.</param>
        public void RaiseOnCollapsing(CollapsingEventArgs collapsingEventArgs, GridStyleInfo styleInfo)
        {
            if (Collapsing != null)
            {
                Collapsing(this, collapsingEventArgs);
                if (!collapsingEventArgs.Cancel)
                {
                    this.RaiseOnCollapsed(new CollapsedEventArgs(collapsingEventArgs.PivotCellInfo));
                }
            }
            else
            {
                this.RaiseOnCollapsed(new CollapsedEventArgs(collapsingEventArgs.PivotCellInfo));
            }
        }

        /// <summary>
        /// Raises the OnCollapsed Event.
        /// </summary>
        /// <param name="collapsedEventArgs">The <see cref="Syncfusion.Windows.Controls.PivotGrid.CollapsedEventArgs"/> instance containing the event data.</param>
        internal void RaiseOnCollapsed(CollapsedEventArgs collapsedEventArgs)
        {
            if (Collapsed != null)
            {
                Collapsed(this, collapsedEventArgs);
            }

        }

        /// <summary>
        /// Raises the hyperlink cell click.
        /// </summary>
        /// <param name="hyperlinkCellClickEventArgs">The <see cref="Syncfusion.Windows.Controls.PivotGrid.HyperlinkCellClickEventArgs"/> instance containing the event data.</param>
        internal void RaiseHyperlinkCellClick(HyperlinkCellClickEventArgs hyperlinkCellClickEventArgs)
        {
            if (HyperlinkCellClick != null)
            {
                HyperlinkCellClick(this, hyperlinkCellClickEventArgs);
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the CellStyle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        public void CellStyle_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.InvalidateCells();
        }

        internal void RaiseDataRefreshing(DataRefreshingEventArgs e)
        {
            if (DataRefreshing != null)
            {
                this.DataRefreshing(this, e);
            }
        }


        internal void RaiseDataRefreshed(DataRefreshedEventArgs e)
        {
            if (DataRefreshed != null)
            {
                this.DataRefreshed(this, e);
            }


    }

        #endregion

        /// <summary>
        /// Used Internally to process BorderColor of bitmap.
        /// </summary>
        /// <param name="bm">Gets the bitmap to be processed</param>
        /// <returns>bitmap</returns>
        internal Bitmap ProcessBitmap(Bitmap bm)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int X = 0; X < bm.Width; X++)
                {
                    for (int Y = 0; Y < bm.Height; Y++)
                    {
                        if (i == 1)
                        {
                            if (X == 1)
                                X = bm.Width - 1;
                        }
                        else
                        {
                            if (Y == 1)
                                Y = bm.Height - 1;
                        }
                        bm.SetPixel(X, Y, System.Drawing.Color.LightGray);
                    }
                }
            }
            return bm;
        }

        public GroupBar GroupDropArea
        {
            get
            {
                if (groupDropArea == null)
                {
                    groupDropArea = (GroupBar)Model.ActiveGridView; 
                }

                return groupDropArea;
            }

            set
            {
                groupDropArea = value;
            }
        }
        GroupBar groupDropArea;

        public RowGroupBar RowGroupDropArea
        {
            get
            {
                if (rowGroupDropArea == null)
                {
                    rowGroupDropArea = (RowGroupBar)Model.ActiveGridView; 
                }

                return rowGroupDropArea;
            }

            set
            {
                rowGroupDropArea = value;
            }
        }
        RowGroupBar rowGroupDropArea;

        public FilterBar FilterArea
        {
            get
            {
                if (filterArea == null)
                {
                    filterArea = (FilterBar)Model.ActiveGridView;
                }

                return filterArea;
            }

            set
            {
                filterArea = value;
            }
        }
        FilterBar filterArea;
      

    }

   
}