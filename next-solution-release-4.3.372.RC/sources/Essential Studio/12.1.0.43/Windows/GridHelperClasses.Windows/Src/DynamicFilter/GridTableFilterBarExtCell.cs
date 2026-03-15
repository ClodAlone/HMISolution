//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableFilterBarExtCell.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Design;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Windows.Forms;
    using System.Text;
    using System.Collections.Specialized;
    using System.IO;
    using System.Reflection;

    using Syncfusion.ComponentModel;
    using Syncfusion.Diagnostics;
    using Syncfusion.Grouping;
    using Syncfusion.Windows.Forms;
    using Syncfusion.Collections.BinaryTree;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Windows.Forms.Grid.Grouping;
    using System.Data;
    using System.Collections.Generic;
    using System.Drawing.Drawing2D;

    #region GridTableFilterBarExtCell Model Class
    /// <summary>
    /// Implements the DataModel part for a ExtendedTableFilterBar cell.
    /// </summary>
    [Serializable]
    public class GridTableFilterBarExtCellModel : GridComboBoxCellModel
    {
        GridTableFilterBarExtCellRenderer renderer = null;
        int buttonWidth = SystemInformation.VerticalScrollBarWidth;
        ListBox compareOprListBox = null;
        internal CustomFilters customFilters = new CustomFilters();
        Hashtable compareOprBitmaps;
        int originalCompareOperCount = 0;
        Hashtable compareOperator = new Hashtable();
        RecordFilterDescriptorCollection recordFilters;
        TableDescriptor tableDescriptor;
        Syncfusion.Grouping.Table table;
        internal static string selectAllText;
        internal static string selectCustomText;
        internal static string selectEmptyText;
        bool applyFilterOnlyOnCellLostFocus;
        /// <overload>
        /// Initializes a new <see cref="GridTableFilterBarExtCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridTableFilterBarExtCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridTableFilterBarExtCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = false;
            ButtonBarSize = new Size(this.ButtonWidth * 2, 0);
            SupportsChoiceList = true;
            AllowDoubleClickChangeSelectedIndex = false;
        }
        /// <summary>
        /// GridTableFilterBarExtCellModel
        /// </summary>
        /// <param name="grid">GridModel</param>
        /// <param name="filterOnlyOnCellLostFocus">bool</param>
        public GridTableFilterBarExtCellModel(GridModel grid, bool filterOnlyOnCellLostFocus)
            : this(grid)
        {
            ApplyFilterOnlyOnCellLostFocus = filterOnlyOnCellLostFocus;
        }
        
        GridTableFilterBarExtCellRenderer Renderer
        {
            get
            {
                return this.renderer;
            }
        }

        internal Hashtable CompareOperators
        {
            get
            {
                return this.compareOperator;
            }
            set
            {
                if (this.compareOperator != value)
                    this.compareOperator = value;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GridTableFilterBarExtCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridTableFilterBarExtCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
            base.ButtonBarSize = new Size(this.ButtonWidth * 2, 0);
        }

        static GridTableFilterBarExtCellModel()
        {
            GridTableFilterBarExtCellModel.selectAllText = SR.GetString(SR.All);
            GridTableFilterBarExtCellModel.selectCustomText = SR.GetString(SR.Custom);
            GridTableFilterBarExtCellModel.selectEmptyText = SR.GetString(SR.Empty);
        }

        internal int ButtonWidth
        {
            get 
            { 
                return this.buttonWidth; 
            }

            set 
            { 
                this.buttonWidth = value; 
            }
        }

        GridGroupingControl GroupingGrid
        {
            get 
            { 
                return ((GridTableModel)this.Grid).GroupingControl; 
            }
        }      

        internal ListBox CompareOperatorListPart
        {
            get
            {
                if (this.compareOprListBox == null)
                {
                    this.compareOprListBox = (ListBox)new GridComboBoxListBoxPart();
                    this.compareOprListBox.Tag = "CompareOperatorList";
                    this.compareOprListBox.Items.AddRange(new string[] {SR.GetString(SR.StartsWith), SR.GetString(SR.EndsWith), SR.GetString(SR.Equal), SR.GetString(SR.NotEquals), SR.GetString(SR.LessThan), SR.GetString(SR.LessThanOrEqualTo), SR.GetString(SR.GreaterThan), SR.GetString(SR.GreaterThanOrEqualTo), SR.GetString(SR.Like), SR.GetString(SR.Match) });
                    this.originalCompareOperCount = this.compareOprListBox.Items.Count;
                    if (this.customFilters != null)
                    {
                        this.customFilters = null;
                    }

                    this.customFilters = new CustomFilters();
                    CreateCompareOperatorListEventArgs arg = new CreateCompareOperatorListEventArgs(this.customFilters);
                    this.CreateCompareOperatorList += new CreateCompareOperatorListHandler(this.GridTableFilterBarExtCellModel_CreateCompareOperatorList);
                    this.RaiseCreateCompareOperatorList(this, arg);
                    if (arg.Filters.Count == 0)
                    {
                        this.customFilters = null;
                    }
                    else
                    {
                        foreach (CustomFilters.ExpressionAndImage expImg in this.customFilters)
                        {
                            this.compareOprListBox.Items.Add(expImg.Name);
                        }
                    }

                    this.InitCompareOprBitmaps();
                }

                return this.compareOprListBox;
            }
        }

        internal void GridTableFilterBarExtCellModel_CreateCompareOperatorList(object sender, CreateCompareOperatorListEventArgs e)
        {
            Bitmap bm = DynamicFilterBitmaps.GetBitmap("em");
            if (!e.Filters.Contains("Expression Match..."))
                e.Filters.Add("Expression Match...", "MATCH '{VALUE}'", bm);
        }

        void InitCompareOprBitmaps()
        {
            if (this.compareOprBitmaps != null)
            {
                this.compareOprBitmaps = null;
            }

            this.compareOprBitmaps = new Hashtable();

            StringCollection operatorNames = new StringCollection();
            operatorNames.AddRange(new string[] 
            {                 
               SR.StartsWith , SR.EndsWith ,SR.Equal , SR.NotEquals ,  SR.LessThan , SR.LessThanOrEqualTo ,
            SR.GreaterThan ,SR.GreaterThanOrEqualTo , SR.Like ,SR.Match
            });

            for (int i = 0; i < operatorNames.Count; i++)
            {
                this.compareOprBitmaps.Add(operatorNames[i], DynamicFilterBitmaps.GetBitmap(operatorNames[i]));
            }

            this.compareOprBitmaps.Add("Default", DynamicFilterBitmaps.GetBitmap("Default"));
            this.compareOprBitmaps.Add("Custom", DynamicFilterBitmaps.GetBitmap("Custom"));

            if (this.customFilters != null)
            {
                foreach (CustomFilters.ExpressionAndImage expImg in this.customFilters)
                {
                    if (expImg.Image != null)
                    {
                        this.compareOprBitmaps.Add(expImg.Name, expImg.Image);
                    }
                }
            }
        }

        #region CreateCompareOperatorList Event
        /// <summary>
        /// Occurs when the filter button is clicked to show a drop down list of supported compare operators.
        /// </summary>
        public event CreateCompareOperatorListHandler CreateCompareOperatorList;

        internal void RaiseCreateCompareOperatorList(object sender, CreateCompareOperatorListEventArgs e)
        {
            if (this.CreateCompareOperatorList != null)
            {
                this.CreateCompareOperatorList(sender, e);
            }
        }

        #endregion

        internal void SetLogicalCompareOperator(object key, string name)
        {
            if (this.compareOperator.ContainsKey(key))
                this.compareOperator[key] = name;
            else
                this.compareOperator.Add(key, name);
        }

        internal string GetCompareOperatorName(object key)
        {
            if (this.compareOperator.ContainsKey(key))
            {
                return (string)this.compareOperator[key];
            }
            else
            {
                return string.Empty;
            }
        }

        internal Bitmap GetCompareOperatorImage(GridStyleInfo style)
        {
            GridTableCellStyleInfo tableCell = (GridTableCellStyleInfo)style;

            string name = this.GetCompareOperatorName(tableCell.TableCellIdentity.Column.Name);
            if (this.compareOprBitmaps == null)
            {
                this.InitCompareOprBitmaps();
            }

            if (this.Renderer.GetFilterBarText(style) == selectCustomText)
            {
                return (Bitmap)this.compareOprBitmaps["Custom"];
            }

            if (name == string.Empty)
            {
                return (Bitmap)this.compareOprBitmaps[SR.StartsWith];
            }

            if (this.compareOprBitmaps.ContainsKey(name))
            {
                return (Bitmap)this.compareOprBitmaps[name];
            }

            if (this.customFilters != null && this.customFilters.Contains(name))
            {
                Bitmap bm = this.customFilters.GetImage(name);
                if (bm != null)
                {
                    return bm;
                }
            }

            // Default Filter Bitmap
            return (Bitmap)this.compareOprBitmaps["Default"];
        }

        /// <summary>
        /// A method that returns logical compareoperator.
        /// </summary>
        /// <param name="key">Column Name</param>
        /// <returns>A FilterCompareOperator.</returns>
        protected object GetLogicalCompareOperator(string key)
        {
            string name = this.GetCompareOperatorName(key);           
            switch (name)
            {
                case SR.Like:
                case SR.StartsWith:
                case SR.EndsWith:
                    return FilterCompareOperator.Like;
                case SR.Equal:
                    return FilterCompareOperator.Equals;
                case SR.NotEquals:
                    return FilterCompareOperator.NotEquals;
                case SR.LessThan:
                    return FilterCompareOperator.LessThan;
                case SR.LessThanOrEqualTo:
                    return FilterCompareOperator.LessThanOrEqualTo;
                case SR.GreaterThan:
                    return FilterCompareOperator.GreaterThan;
                case SR.GreaterThanOrEqualTo:
                    return FilterCompareOperator.GreaterThanOrEqualTo;
                case SR.Match:
                    return FilterCompareOperator.Match;
                default:
                    if (this.customFilters != null && this.customFilters.Contains(name))
                    {
                        return this.customFilters.GetExpression(name);
                    }
                    else
                    {
                        SetLogicalCompareOperator(key, SR.StartsWith);
                        return FilterCompareOperator.Like;
                    }
            }
        }

        /// <summary>
        /// Apply filters to grid data.
        /// </summary>
        public void ApplyFilters()
        {
            if (this.tableDescriptor != null && this.recordFilters != null)
            {
                this.tableDescriptor.RecordFilters.InitializeFrom(this.recordFilters);
            }

            this.recordFilters = null;
        }

        /// <summary>
        /// Creates choice list for the filter drop down.
        /// </summary>
        /// <param name="listBox">Drop down listbox.</param>
        /// <param name="style">Cell style information.</param>
        /// <param name="exclusive">Indicates whether the listbox is loaded with exlusive choice list or if non-standard values are allowed.</param>
        public override void FillWithChoices(ListBox listBox, GridStyleInfo style, out bool exclusive)
        {
            exclusive = false;
            if (listBox.Tag == null || listBox.Tag.ToString() != "CompareOperatorList")
            {
                GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)style;

                object[] items = (object[])this.GetFilterBarChoices(tableStyleInfo.TableCellIdentity);

                listBox.Items.Clear();

                if (items != null)
                {
                    listBox.Items.Add(this.SelectAllText);

                    listBox.Items.Add(this.SelectCustomText);

                    foreach (object item in items)
                    {
                        if (item is DBNull || item == string.Empty)
                        {
                            listBox.Items.Add(this.SelectEmptyText);
                        }
                        else if (item != null && item.ToString() == string.Empty )
                        {
                            listBox.Items.Add(item);
                        }
                        else if (item != null)
                        {
                            listBox.Items.Add(style.GetFormattedText(item));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the unique choices to be displayed in the filterbar cell.
        /// </summary>
        /// <param name="tableCellIdentity">Table cell identifier.</param>
        /// <returns>Filter bar choices.</returns>
        public virtual object[] GetFilterBarChoices(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            if (tableCellIdentity.FilterBarSummaryDescriptor != null)
            {
                ITreeTableSummary[] summaries = tableCellIdentity.DisplayElement.ParentGroup.GetSummaries(tableCellIdentity.Table);
                int summaryIndex = tableCellIdentity.Table.TableDescriptor.Summaries.IndexOf(tableCellIdentity.FilterBarSummaryDescriptor);
                FilterBarChoicesSummary filterBarChoicesSummary = (FilterBarChoicesSummary)summaries[summaryIndex];
                return filterBarChoicesSummary.Values;
            }
            
            GridGroupingControl groupGrid = ((GridEngine)tableCellIdentity.Table.Engine).GroupingControl;
            if (groupGrid.OptimizeFilterPerformance)
            {
                GridCurrentCell cc = tableCellIdentity.Table.TableDescriptor.Engine.TableControl.CurrentCell;
                if (cc.Renderer is GridNestedTableControlCellRenderer)
                {
                    GridNestedTableControlCellRenderer rend = cc.Renderer as GridNestedTableControlCellRenderer;
                    cc = rend.GetNestedCurrentCell();
                }
                cc.Lock();
                tableCellIdentity.Table.CurrentElement = tableCellIdentity.DisplayElement;
                tableCellIdentity.Table.CurrentRecordManager.Lock();
                SummaryDescriptor sd = new SummaryDescriptor(
                                tableCellIdentity.Column.Name + "FilterBarChoices",
                                tableCellIdentity.Column.MappingName,
                                new CreateSummaryDelegate(FilterBarChoicesSummary.CreateSummaryMethod));
                sd.IgnoreRecordFilterCriteria = true;
                tableCellIdentity.Table.TableDescriptor.Summaries.Add(sd);
                ITreeTableSummary[] summaries = tableCellIdentity.Table.GetSummaries();
                int summaryIndex = tableCellIdentity.Table.TableDescriptor.Summaries.IndexOf(sd);
                FilterBarChoicesSummary filtersummary = (FilterBarChoicesSummary)summaries[summaryIndex];
                object[] values = filtersummary.Values;
                tableCellIdentity.Table.TableDescriptor.Summaries.Remove(sd);
                tableCellIdentity.Table.SummariesDirty = true;
                tableCellIdentity.Table.CurrentRecordManager.Unlock();
                cc.Unlock();
                return values;
            }

            object[] result = null;
            GridGroupingControl gc = tableCellIdentity.Table.Engine.ParentControl;
            GridQueryFilterBarChoicesEventArgs qe = new GridQueryFilterBarChoicesEventArgs(tableCellIdentity.Column, false, tableCellIdentity.DisplayElement);
            gc.OnQueryFilterBarChoices(qe);
            if (!qe.Cancel)
            {
                result = qe.UniqueFilterBarValues;
            }

            return result;
        }

        /// <summary>
        /// Returns the desired filter from a list available filters.
        /// </summary>
        /// <param name="recordFilters">A list of available record filters.</param>
        /// <param name="tableCellIdentity">Cell identifier.</param>
        /// <param name="filterName">Name of the filter.</param>
        /// <returns>Record filter.</returns>
        public RecordFilterDescriptor GetRecordFilter(RecordFilterDescriptorCollection recordFilters, GridTableCellStyleInfoIdentity tableCellIdentity, string filterName)
        {
            RecordFilterDescriptor filter = null;

            GridTableDescriptor td = tableCellIdentity.Table.TableDescriptor;
            GridRelationDescriptor rd = td.ParentRelation;
            if (rd == null || rd.RelationKind != RelationKind.UniformChildList)
            {
                filter = recordFilters[filterName];
            }
            else
            {
                object[] uniqueId = tableCellIdentity.DisplayElement.ParentGroup.UniqueGroupId;
                string mappingName = tableCellIdentity.Column.MappingName;
                foreach (RecordFilterDescriptor rfd in recordFilters)
                {
                    if (rfd.MappingName == mappingName && rfd.CompareUniqueId(uniqueId))
                    {
                        filter = rfd;
                        break;
                    }
                }
            }

            return filter;
        }

        /// <summary>
        /// Returns the category for a given cell.
        /// </summary>
        /// <param name="tableCellIdentity">Cell identifier.</param>
        /// <returns>Category key.</returns>
        public string GetUniqueColumnGroupId(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            Group g = tableCellIdentity.DisplayElement.ParentGroup;
            object[] id = g.UniqueGroupId;

            if (id == null || id.Length == 0)
            {
                return tableCellIdentity.Column.Name;
            }
            else
            {
                return tableCellIdentity.Column.Name + "@" + g.UniqueGroupIdsToString(); ////.UniqueGroupId;
            }
        }

        /// <summary>
        /// Returns an array of category keys for this group and all parent groups which is used by FilterBarCells and FitlerBarSummary
        /// to compare whether the conditions should be applied to this group.
        /// </summary>
        /// <param name="tableCellIdentity">Cell identifier.</param>
        /// <returns>An array of category keys.</returns>
        public object[] GetUniqueGroupId(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            Group g = tableCellIdentity.DisplayElement.ParentGroup;
            return g.UniqueGroupId;
        }

        /// <summary>
        /// Determines whether the given cell has a filter.
        /// </summary>
        /// <param name="tableCellIdentity">The table cell identity.</param>
        /// <returns>True if it has a filter; False otherwise.</returns>
        public bool HasFilter(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            return tableCellIdentity.Table.TableDescriptor.RecordFilters.Count > 0;
        }

        internal void ResetFilterBar(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            string filterName = this.GetUniqueColumnGroupId(tableCellIdentity);

            this.tableDescriptor = tableCellIdentity.Table.TableDescriptor;
            this.table = tableCellIdentity.Table;
            this.recordFilters = new RecordFilterDescriptorCollection();
            this.recordFilters.InitializeFrom(tableCellIdentity.Table.TableDescriptor.RecordFilters);

            RecordFilterDescriptor filter = this.recordFilters[filterName];
            if (filter != null)
            {
                this.recordFilters.Remove(filterName);
            }
        }

        /// <summary>
        /// Applies a filter criteria. Note: The first two entries are reserved for (All) and (Custom).
        /// An index greater than one represents a valid choice found with GetFilterBarChoices.
        /// </summary>
        /// <param name="tableCellIdentity">Cell identifier.</param>
        /// <param name="index">Filter choice index.</param>
        public void Select(GridTableCellStyleInfoIdentity tableCellIdentity, int index)
        {
            this.Select(tableCellIdentity, null, index);
        }

        internal void Select(GridTableCellStyleInfoIdentity tableCellIdentity, object value, int index)
        {
            if (index >= 0)
            {
                if (index == 0)
                {
                    this.ResetFilterBar(tableCellIdentity);
                }
                else if (index == 1)
                {
                    //// Custom
                    this.SelectCustomFilterBar(tableCellIdentity);
                }
                else
                {
                    this.SelectItem(tableCellIdentity, index - 2);
                }
            }
            else if (index == -101 && value != null)
            {
                // Entered text filter
                if (value.ToString().Length > 0)
                {
                    this.SelectItem(tableCellIdentity, this.GetLogicalCompareOperator(tableCellIdentity.Column.Name), this.IncludeFilterFormat(value, tableCellIdentity.Column.Name), -101);
                }
                else
                {
                    this.ResetFilterBar(tableCellIdentity);
                }
            }
        }

        internal string IncludeFilterFormat(object value, object key)
        {
            string oprName = this.GetCompareOperatorName(key);
            string filter = Convert.ToString(value);
            switch (oprName)
            {
                case SR.StartsWith:
                    filter = string.Format("{0}*", value);
                    break;
                case SR.EndsWith:
                    filter = string.Format("*{0}", value);
                    break;

                // case 2: //Equals
                // case 3: //Not Equals
                // case 4: //LessThan
                // case 5: //LessThanOrEqualTo
                // case 6: //Greater
                // case 7: //GreaterThanOrEqualTo
                // case 8: //Like
                // case 9: //Match
                //    break;
                default:
                    if (this.customFilters != null && this.customFilters.Contains(oprName))
                    {
                        string format = this.customFilters.GetExpression(oprName);
                        format = format.Replace("{VALUE}", "{0}");
                        filter = string.Format(format, value);
                        filter = string.Format("[{0}] {1}", key, filter);
                    }

                    break;
            }

            return filter;
        }

        internal bool IsExpressionFilter(GridTableCellStyleInfo style)
        {
            string name = this.GetCompareOperatorName(style.TableCellIdentity.Column.Name);
            if (this.customFilters != null && this.customFilters.Contains(name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal virtual string ExcludeFilterFormat(object value, object key)
        {
            string oprName = this.GetCompareOperatorName(key);
            string filter = Convert.ToString(value);
            switch (oprName)
            {
                case SR.StartsWith:
                    if (filter.EndsWith("*"))
                    {
                        filter = filter.Remove(filter.Length - 1);
                    }

                    break;
                case SR.EndsWith:
                    if (filter.StartsWith("*"))
                    {
                        filter = filter.Remove(0, 1);
                    }

                    break;

                // case 2: //Equals
                // case 3: //Not Equals
                // case 4: //LessThan
                // case 5: //LessThanOrEqualTo
                // case 6: //Greater
                // case 7: //GreaterThanOrEqualTo
                // case 8: //Like
                // case 9: //Match
                //    break;
                default:
                    if (this.customFilters != null && this.customFilters.Contains(oprName) && filter != string.Empty && filter!=selectAllText)
                    {
                        string format = this.customFilters.GetExpression(oprName);
                        int i = format.IndexOf("{VALUE}");
                        string sub = format.Substring(i + 7);
                        filter = filter.Replace(string.Format("[{0}] ", key), string.Empty);
                        filter = filter.Substring(i);

                        i = sub.IndexOf("{VALUE}");
                        if (i > 0)
                        {
                            sub = sub.Substring(0, i);
                        }

                        i = filter.IndexOf(sub);
                        if (i > 0)
                        {
                            filter = filter.Substring(0, i);
                        }
                        else if (i.Equals(0))
                        {
                            filter = string.Empty;
                        }
                    }

                    // if (filter.EndsWith("*"))
                    //    filter = filter.Remove(filter.Length - 1);
                    break;
            }

            return filter;
        }
        /// <summary>
        /// Gets or sets select all text Default: (All)
        /// </summary>
        public string SelectAllText
        {
            get
            {
                return selectAllText;
            }

            set
            {
                selectAllText = value;
            }
        }

        /// <summary>
        /// Gets or sets select custom text Default: (Custom...)
        /// </summary>
        public string SelectCustomText
        {
            get
            {
                return selectCustomText;
            }

            set
            {
                selectCustomText = value;
            }
        }

        /// <summary>
        /// Gets or sets select empty text Default: (Empty)
        /// </summary>
        public string SelectEmptyText
        {
            get
            {
                return selectEmptyText;
            }

            set
            {
                selectEmptyText = value;
            }
        }
        
        /// <summary>
        /// Apply filter on cell lost focus
        /// </summary>
        /// 
          [Browsable(false)]
        public bool ApplyFilterOnlyOnCellLostFocus
        {
            get
            {
                return applyFilterOnlyOnCellLostFocus;
            }

            set
            {
                applyFilterOnlyOnCellLostFocus = value;
            }
        }

        /// <summary>
        /// A method to initiate collection dialog editor while selecting custom option.
        /// </summary>
        /// <param name="tableCellIdentity">cell identifier</param>
        protected void SelectCustomFilterBar(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            this.tableDescriptor = tableCellIdentity.Table.TableDescriptor;
            this.table = tableCellIdentity.Table;
            this.recordFilters = new RecordFilterDescriptorCollection();
            this.recordFilters.InitializeFrom(tableCellIdentity.Table.TableDescriptor.RecordFilters);

            string filterName = this.GetUniqueColumnGroupId(tableCellIdentity);
            RecordFilterDescriptor filter = this.recordFilters[filterName];
            if (filter == null)
            {
                filter = new RecordFilterDescriptor();
                filter.Name = tableCellIdentity.Column.Name;
                filter.MappingName = tableCellIdentity.Column.MappingName;
                filter.UniqueGroupId = this.GetUniqueGroupId(tableCellIdentity);
                this.recordFilters.Add(filter);
            }

            RecordFilterDescriptorCollection filters = new RecordFilterDescriptorCollection();
            filters.InitializeFrom(this.recordFilters);
            DialogResult result = this.ShowCollectionDialog(
                tableCellIdentity.Table.TableDescriptor, "RecordFilters", null, typeof(RecordFilterDescriptorCollection));
            if (result == DialogResult.Cancel)
            {
                if (filter != null)
                {
                    this.tableDescriptor.RecordFilters.Remove(filter);
                }
            }
           this.recordFilters = null;
        }
        /// <summary>
        /// SelectItem
        /// </summary>
        /// <param name="tableCellIdentity">GridTableCellStyleInfoIdentity</param>
        /// <param name="index">int</param>
        protected void SelectItem(GridTableCellStyleInfoIdentity tableCellIdentity, int index)
        {
            this.SelectItem(tableCellIdentity, this.GetLogicalCompareOperator(tableCellIdentity.Column.Name), null, index);
        }
        /// <summary>
        /// SelectItem
        /// </summary>
        /// <param name="tableCellIdentity">GridTableCellStyleInfoIdentity</param>
        /// <param name="condition">object</param>
        /// <param name="value">object</param>
        /// <param name="index">int</param>
        protected void SelectItem(GridTableCellStyleInfoIdentity tableCellIdentity, object condition, object value, int index)
        {
            string filterName = this.GetUniqueColumnGroupId(tableCellIdentity);

            this.tableDescriptor = tableCellIdentity.Table.TableDescriptor;
            this.table = tableCellIdentity.Table;
            this.recordFilters = new RecordFilterDescriptorCollection();
            this.recordFilters.InitializeFrom(tableCellIdentity.Table.TableDescriptor.RecordFilters);

            RecordFilterDescriptor filter = this.GetRecordFilter(this.recordFilters, tableCellIdentity, filterName);

            object[] items = this.GetFilterBarChoices(tableCellIdentity);
            StringCollection values = new StringCollection();
            value = (index == -101) ? value : ((items[index] is DBNull || items[index].ToString() == string.Empty) ? items[index] : this.IncludeFilterFormat(items[index], tableCellIdentity.Column.Name));
            if (index != -101)
            {
                double d;
                if (!string.IsNullOrEmpty(tableCellIdentity.Column.Appearance.AnyCell.Format) && double.TryParse(value.ToString(), out d))
                    value =  string.Format("{0:" + tableCellIdentity.Column.Appearance.AnyCell.Format + "}", d);
            }
            foreach (object item in items)
            {
                string val = tableCellIdentity.Column.Appearance.AnyRecordFieldCell.GetFormattedText(item).ToString();
                if (item != null && item.ToString().Contains(value.ToString()) && !val.Contains(value.ToString()))
                {
                    value = null;
                    break;
                }
                if (val.Equals(value))
                {
                    values.Add(item.ToString());
                }
            }
            RecordFilterDescriptor newFilter = new RecordFilterDescriptor();
            newFilter.Name = filterName;
            newFilter.MappingName = tableCellIdentity.Column.MappingName;
            newFilter.UniqueGroupId = this.GetUniqueGroupId(tableCellIdentity);

            if (condition is FilterCompareOperator)
            {
                if (values.Count == 0)
                    newFilter.Conditions.Add(new FilterCondition((FilterCompareOperator)condition, value));
                else
                    foreach (string filterValue in values)
                    {
                        newFilter.Conditions.Add(new FilterCondition((FilterCompareOperator)condition, filterValue));
                    }
            }
            else
            {
                if (value is DBNull)
                    newFilter.Expression = this.IncludeFilterFormat(DBNull.Value.ToString(),tableCellIdentity.Column.Name);
                else
                    newFilter.Expression = (string)value;
            }

            if (filter == null)
            {
                this.recordFilters.Add(newFilter);
            }
            else
            {
                filter.InitializeFrom(newFilter);
            }
        }

        /// <summary>
        /// Occurs immediately before the RecordFilterCollectionEditor Dialog is displayed. The ControlEventArgs.Control 
        /// the form.
        /// </summary>
        public event ControlEventHandler ShowingCustomFilterDialog;
       
        /// <summary>
        /// Initializes the collection dialog.
        /// </summary>
        /// <param name="instance">table descriptor</param>
        /// <param name="propertyName">name of the property</param>
        /// <param name="provider">service provider</param>
        /// <param name="type">type of RecordFilterDescriptorCollection</param>
        /// <returns>Return value of the dialog box.</returns>
        protected DialogResult ShowCollectionDialog(object instance, string propertyName, IServiceProvider provider, Type type)
        {
            GroupingCollectionEditor ce = new GroupingCollectionEditor(type);
            Syncfusion.ComponentModel.WindowsFormsEditorServiceContainer esc = new Syncfusion.ComponentModel.WindowsFormsEditorServiceContainer(provider);
            esc.ShowingDialog += new ControlEventHandler(esc_ShowingDialog);
            PropertyDescriptor pd = TypeDescriptor.GetProperties(instance)[propertyName];
            Syncfusion.ComponentModel.TypeDescriptorContext tdc = new Syncfusion.ComponentModel.TypeDescriptorContext(instance, pd);
            tdc.ServiceProvider = esc;

            ce.EditValue(tdc, esc, pd.GetValue(instance));

            return esc.DialogResult;
        }

        /// <summary>
        /// Used internally
        /// </summary>
        private void esc_ShowingDialog(object sender, ControlEventArgs e)
        {
            if (ShowingCustomFilterDialog != null)
            {
                ShowingCustomFilterDialog(sender, e);
            }
        }

        /// <summary>
        /// Creates a <see cref="GridTableFilterBarExtCellRenderer"/> for this cell model that is specific to the GridControlBase.
        /// </summary>
        /// <param name="control">The <see cref="GridControlBase"/> the cell renderer is created for.</param>
        /// <returns>A new <see cref="GridTableFilterBarCellRenderer"/> specific for a <see cref="GridControlBase"/>.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            this.renderer = new GridTableFilterBarExtCellRenderer(control, this);
            return this.renderer;
        }
    }

    #endregion

    #region GridTableFilterBarExtCell Renderer Class
    /// <summary>
    /// Implements the renderer part of a TableFilterBar cell.
    /// </summary>
    public class GridTableFilterBarExtCellRenderer : GridComboBoxCellRenderer
    {
        GridTableCellStyleInfoIdentity tableCellIdentity;
        ListBox listBoxPart = null;
        bool isComparerListBox = false;
        string oldColumn = string.Empty;
        string currColumn = string.Empty;
        Hashtable mouseHoverAtCells = new Hashtable();

        /// <summary>
        /// Initializes a new <see cref="GridTableFilterBarCellRenderer"/> object for the given GridControlBase
        /// and <see cref="GridCellModelBase"/>.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridTableFilterBarExtCellRenderer"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase
        /// and GridCellModelBase will be saved.</remarks>
        public GridTableFilterBarExtCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            AddButton(new FilterButton(this));
            AddButton(new ClearFilterButton(this));
        }

        void Grid_CurrentCellControlKeyMessage(object sender, GridCurrentCellControlKeyMessageEventArgs e)
        {
            if (e.Control is GridMaskedEditBox || e.Control is GridCurrencyTextBox)
            {
                e.CallBaseProcessKeyMessage = true;
            }
            else
                e.CallBaseProcessKeyMessage = false;
        }

        #region ListBoxes [Unique Column Value Choise List/ CompareOperator Choise List]

        internal ListBox InternalListBoxPart
        {
            get 
            { 
                return this.listBoxPart; 
            }
        }
     
        bool IsComparerListBoxPart
        {
            get 
            { 
                return this.isComparerListBox; 
            }

            set
            {
                if (this.isComparerListBox != value)
                {
                    this.isComparerListBox = value;
                    if (this.isComparerListBox)
                    {
                        this.AttachComparerListBoxPart();
                    }
                    else
                    {
                        this.AttachOriginalListBoxPart();
                    }
                }

                if (!this.isComparerListBox)
                {
                    if (this.ListBoxVersion < 0)
                    {
                        GridTableCellStyleInfo style = this.Grid.GetTableViewStyleInfo(RowIndex, ColIndex);
                        if (style.IsTableCell)
                        {
                            bool exclusive;
                            this.Model.FillWithChoices(this.ListBoxPart, style, out exclusive);
                        }
                    }

                    this.EnsureOriginalListBoxPart();
                }
            }
        }
        
        int ListBoxVersion
        {
            [DebuggerStepThrough()]
            get
            {
                if (this.currColumn.Equals(string.Empty) || !this.currColumn.Equals(this.oldColumn))
                {
                    this.oldColumn = this.currColumn;
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// Called to attach a list box part to this renderer object.
        /// </summary>
        protected void AttachComparerListBoxPart()
        {
            // listBoxPart = (ListBox)this.DropDownContainer.Controls[0];
            this.EnsureOriginalListBoxPart();
            if (this.DropDownContainer.Controls.Contains(this.InternalListBoxPart))
            {
                this.DropDownContainer.Controls.Remove(this.InternalListBoxPart);
            }

            this.ListBoxPart = this.Model.CompareOperatorListPart;
            this.DropDownContainer.Controls.Add(this.Model.CompareOperatorListPart);
        }

        /// <summary>
        /// Called to attach a list box part to this renderer object.
        /// </summary>
        protected void AttachOriginalListBoxPart()
        {
            if (!this.DropDownContainer.Controls.Contains(this.InternalListBoxPart))
            {
                this.DropDownContainer.Controls.RemoveAt(0);
                this.ListBoxPart = this.InternalListBoxPart;
                this.DropDownContainer.Controls.Add(this.InternalListBoxPart);
            }
        }

        private void EnsureOriginalListBoxPart()
        {
            ListBox lb = (ListBox)this.DropDownContainer.Controls[0];
            if (lb.Tag == null || lb.Tag.ToString() != "CompareOperatorList")
            {
                this.listBoxPart = lb;
            }
        }

       #endregion

        #region Buttons Related
        /// <summary>
        /// Wires the grid model to filter
        /// </summary>
        /// <param name="cellModel">GridCellModelBase</param>
        protected override void WireModel(GridCellModelBase cellModel)
        {
            this.Grid.CurrentCellMoved += new GridCurrentCellMovedEventHandler(this.Grid_CurrentCellMoved);
            this.Grid.CurrentCellControlKeyMessage += new GridCurrentCellControlKeyMessageEventHandler(Grid_CurrentCellControlKeyMessage);
            this.Grid.CurrentCellAcceptedChanges += new CancelEventHandler(Grid_CurrentCellAcceptedChanges);
            base.WireModel(cellModel);
        }

        void Grid_CurrentCellAcceptedChanges(object sender, CancelEventArgs e)
        {
            if (this.Model.ApplyFilterOnlyOnCellLostFocus && this.Grid.CurrentCell.Renderer != null && this.Grid.CurrentCell.Renderer is GridTableFilterBarExtCellRenderer)
            {
                this.Model.ApplyFilters();
            }
        }

        private string SelectedLocalizedString(string selecteditem)
        {
            if (selecteditem.Equals(SR.StartsWith))
            {
                return SR.GetString(SR.StartsWith);
            }
            else if (selecteditem.Equals(SR.EndsWith))
            {
                return SR.GetString(SR.EndsWith);
            }
            else if (selecteditem.Equals(SR.Equal))
            {
                return SR.GetString(SR.Equal);
            }
            else if (selecteditem.Equals(SR.NotEquals))
            {
                return SR.GetString(SR.NotEquals);
            }
            else if (selecteditem.Equals(SR.LessThan))
            {
                return SR.GetString(SR.LessThan);
            }
            else if (selecteditem.Equals(SR.LessThanOrEqualTo))
            {
                return SR.GetString(SR.LessThanOrEqualTo);
            }
            else if (selecteditem.Equals(SR.GreaterThan))
            {
                return SR.GetString(SR.GreaterThan);
            }
            else if (selecteditem.Equals(SR.GreaterThanOrEqualTo))
            {
                return SR.GetString(SR.GreaterThanOrEqualTo);
            }
            else if (selecteditem.Equals(SR.Match))
            {
                return SR.GetString(SR.Match);
            }
            else if (selecteditem.Equals(SR.Like))
            {
                return SR.GetString(SR.Like);
            }
            else if (this.Model.customFilters.Contains(selecteditem.ToString()))
            {
                return selecteditem.ToString();
            }
            else
                return "default";

        }

        private string SelectedString(string selecteditem)
        {
            if (selecteditem.Equals(SR.GetString(SR.StartsWith)))
            {
                return SR.StartsWith;
            }
            else if (selecteditem.Equals(SR.GetString(SR.EndsWith)))
            {
                return SR.EndsWith;
            }
            else if (selecteditem.Equals(SR.GetString(SR.Equal)))
            {
                return SR.Equal;
            }
            else if (selecteditem.Equals(SR.GetString(SR.NotEquals)))
            {
                return SR.NotEquals;
            }
            else if (selecteditem.Equals(SR.GetString(SR.LessThan)))
            {
                return SR.LessThan;
            }
            else if (selecteditem.Equals(SR.GetString(SR.LessThanOrEqualTo)))
            {
                return SR.LessThanOrEqualTo;
            }
            else if (selecteditem.Equals(SR.GetString(SR.GreaterThan)))
            {
                return SR.GreaterThan;
            }
            else if (selecteditem.Equals(SR.GetString(SR.GreaterThanOrEqualTo)))
            {
                return SR.GreaterThanOrEqualTo;
            }
            else if (selecteditem.Equals(SR.GetString(SR.Match)))
            {
                return SR.Match;
            }
            else if (selecteditem.Equals(SR.GetString(SR.Like)))
            {
                return SR.Like;
            }
            else if (this.Model.customFilters.Contains(selecteditem.ToString()))
            {
                return selecteditem.ToString();
            }
            else
                return "default";
            
        }
        /// <summary>
        /// Wires the grid model with the filter
        /// </summary>
        /// <param name="cellModel">GridCellModelBase</param>
        protected override void UnwireModel(GridCellModelBase cellModel)
        {
            this.Grid.CurrentCellMoved -= new GridCurrentCellMovedEventHandler(this.Grid_CurrentCellMoved);
            this.Grid.CurrentCellControlKeyMessage -= new GridCurrentCellControlKeyMessageEventHandler(Grid_CurrentCellControlKeyMessage);
            this.Grid.CurrentCellAcceptedChanges -= new CancelEventHandler(Grid_CurrentCellAcceptedChanges);
            base.UnwireModel(cellModel);
        }

        void Grid_CurrentCellMoved(object sender, GridCurrentCellMovedEventArgs e)
        {
            if (this.Grid.CurrentCell != null)
            {
                this.Grid.CurrentCell.Refresh();
                GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)this.Grid.GetViewStyleInfo(this.CurrentCell.RowIndex, this.CurrentCell.ColIndex);
                GridTableCellStyleInfoIdentity tableCellIdentity = tableStyleInfo.TableCellIdentity;                
                if (this.tableCellIdentity != null && this.tableCellIdentity.Column != null && this.Grid.CurrentCell.Renderer is GridTableFilterBarExtCellRenderer)
                    this.Grid.CurrentCell.BeginEdit();
            }
            // version--;
        }
        /// <summary>
        /// ProcessKeyEventArgs
        /// </summary>
        /// <param name="m">Message</param>
        /// <returns>bool</returns>
        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            if (Grid.IsWindowless && !IsDroppedDown && (m.Msg == 258 /*WM_CHAR*/|| m.Msg == 256))
            {
                Keys keyCode = (Keys)((int)m.WParam) & Keys.KeyCode;

                if (keyCode == Keys.Back || keyCode == Keys.Delete)
                {
                    OnKeyDown(new KeyEventArgs(keyCode));
                    //CurrentCell.ConfirmChanges();                    
                }
            }
            else if (m.Msg == 257 /*WM_KEYUP*/)
            {
                Keys keyCode = (Keys)((int)m.WParam) & Keys.KeyCode;
                if (keyCode == Keys.Delete)
                {
                    OnKeyPress(new KeyPressEventArgs('\0'));
                }
            }
            return base.ProcessKeyEventArgs(ref m);
        }
        /// <summary>
        /// When the button is clicked
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="button">int</param>
        protected override void OnButtonClicked(int rowIndex, int colIndex, int button)
        {
            // Clear Filter
            if (button == 2)
            {
                GridTableCellStyleInfo style = this.Grid.GetTableViewStyleInfo(rowIndex, colIndex);
                this.Model.ResetFilterBar(style.TableCellIdentity);
                this.Model.ApplyFilters();
            }
            else
            {
                this.currColumn = this.GetKey(rowIndex, colIndex).ToString();

                if (button == 0)
                {
                    this.IsComparerListBoxPart = false;
                }               
                else if (button == 1)
                { 
                    //// Set Filter criteria
                    string key = this.Model.GetCompareOperatorName(this.currColumn);
                    string filtered= SelectedLocalizedString(key);
                    int selectedIndex = 0;
                    if (!string.IsNullOrEmpty(key))
                        selectedIndex = this.Model.CompareOperatorListPart.Items.IndexOf(filtered);
                    this.Model.CompareOperatorListPart.SelectedIndex = selectedIndex;
                    this.IsComparerListBoxPart = true;

                    //// Show DropDown with comparer choise...
                    //// FilterCompareOperator oper = FilterCompareOperator.Like;
                    //// Model.SetLogicalComparerOperator(GetKey(rowIndex, colIndex), oper);
                }
                base.OnButtonClicked(rowIndex, colIndex, button);
            }
        }
        /// <summary>
        /// Obtains the layout for grid
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="style">GridStyleInfo</param>
        /// <param name="innerBounds">Rectangle</param>
        /// <param name="buttonsBounds">Rectangle</param>
        /// <returns>Rectangle</returns>
        protected override Rectangle OnLayout(int rowIndex, int colIndex, GridStyleInfo style, Rectangle innerBounds, Rectangle[] buttonsBounds)
        {
            //// TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, style, innerBounds, buttonsBounds);

            Rectangle buttonArea = Rectangle.FromLTRB(innerBounds.Left, innerBounds.Top, innerBounds.Left + this.Model.ButtonWidth, innerBounds.Bottom);
            buttonsBounds[2] = GridUtil.CenterInRect(buttonArea, new Size(this.Model.ButtonWidth, innerBounds.Height));

            buttonArea = Rectangle.FromLTRB(innerBounds.Right - this.Model.ButtonWidth, innerBounds.Top, innerBounds.Right, innerBounds.Bottom);
            buttonsBounds[1] = GridUtil.CenterInRect(buttonArea, new Size(this.Model.ButtonWidth, innerBounds.Height));

            buttonArea = Rectangle.FromLTRB((innerBounds.Right - (this.Model.ButtonWidth * 2)), innerBounds.Top, (innerBounds.Right - this.Model.ButtonWidth), innerBounds.Bottom);
            buttonsBounds[0] = GridUtil.CenterInRect(buttonArea, new Size(this.Model.ButtonWidth, innerBounds.Height));

            innerBounds.Width -= this.Model.ButtonWidth * 2;

            // if (CanShowFilterButtons(rowIndex, colIndex))
                return new Rectangle(innerBounds.X + this.Model.ButtonWidth, innerBounds.Y, innerBounds.Width - this.Model.ButtonWidth, innerBounds.Height);

            // return innerBounds;
        }
        /// <summary>
        /// When the cell button is drawn
        /// </summary>
        /// <param name="button">GridCellButton</param>
        /// <param name="g">Graphics</param>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="bActive">bool</param>
        /// <param name="style">GridStyleInfo</param>
        protected override void OnDrawCellButton(GridCellButton button, Graphics g, int rowIndex, int colIndex, bool bActive, GridStyleInfo style)
        {
            if (!this.Grid.Model.EnableLegacyStyle)
            {
                Bitmap bm = DynamicFilterBitmaps.GetBitmap("filter");
                Point ptOffset = Point.Empty;

                if (button is Syncfusion.Windows.Forms.Grid.GridCellComboBoxButton)
                {
                    base.OnDrawCellButton(button, g, rowIndex, colIndex, bActive, style);
                    DynamicFilterBitmaps.IconPainter.PaintIcon(g, button.Bounds, ptOffset, bm, Color.Blue);
                }
                if (!(button is ClearFilterButtonExt) || this.CanShowFilterButtons(rowIndex, colIndex))
                {
                    if (!(button is Syncfusion.Windows.Forms.Grid.GridCellComboBoxButton))
                    {
                        base.OnDrawCellButton(button, g, rowIndex, colIndex, bActive, style);
                    }
                }
            }
            else
            {
                if (!(button is ClearFilterButton) || this.CanShowFilterButtons(rowIndex, colIndex))
                {
                    base.OnDrawCellButton(button, g, rowIndex, colIndex, bActive, style);
                }
            }
        }

        bool CanShowFilterButtons(int rowIndex, int colIndex)
        {
            if (this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
            {
                return true;
            }

            bool mouseHover = false;
            GridRangeInfo key = GridRangeInfo.Cell(rowIndex, colIndex);
            if (this.mouseHoverAtCells.ContainsKey(key))
            {
                mouseHover = (bool)this.mouseHoverAtCells[key];
            }

            if (!mouseHover)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Is triggered when the mouse enters a particular area
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        protected override void OnMouseHoverEnter(int rowIndex, int colIndex)
        {
            GridRangeInfo key = GridRangeInfo.Cell(rowIndex, colIndex);
            if (this.mouseHoverAtCells.ContainsKey(key))
            {
                this.mouseHoverAtCells[key] = true;
            }
            else
            {
                this.mouseHoverAtCells.Add(key, true);
            }

            base.OnMouseHoverEnter(rowIndex, colIndex);

            this.Grid.RefreshRange(key);
        }
        /// <summary>
        /// Is triggered when the mouse leaves a particular area.
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="e">EventArgs</param>
        protected override void OnMouseHoverLeave(int rowIndex, int colIndex, EventArgs e)
        {
            GridRangeInfo key = GridRangeInfo.Cell(rowIndex, colIndex);
            if (this.mouseHoverAtCells.ContainsKey(key))
            {
                this.mouseHoverAtCells[key] = false;
            }
            else
            {
                this.mouseHoverAtCells.Add(key, false);
            }

            base.OnMouseHoverLeave(rowIndex, colIndex, e);
            this.Grid.RefreshRange(key);
        }

        #endregion

        #region Key Overrides
        /// <summary>
        /// Is triggered when the key is pressed down
        /// </summary>
        /// <param name="e">KeyPressEventArgs</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            ListBox listBoxPart = this.ListBoxPart;
            GridComboBoxCellModel model = this.Model as GridComboBoxCellModel;

            if (!e.Handled)
            {
                GridTableCellStyleInfo style = this.Grid.GetTableViewStyleInfo(RowIndex, ColIndex);
                if (!this.HasFocusControl)
                {
                    //// Key pressed for the first time.
                    if (!Char.IsControl(e.KeyChar) && !IsReadOnly())
                    {
                        if (listBoxPart.Items == null)
                        {
                            object ds = model.GetDataSource(this.StyleInfo);
                            if (ds == null)
                            {
                                base.OnKeyPress(e);
                                return;
                            }

                            listBoxPart.DataSource = ds;
                        }

                        //// Combo box mode with editing.
                        CurrentCell.BeginEdit();
                        SetTextBoxText(e.KeyChar.ToString(), true);
                        this.Model.Select(style.TableCellIdentity, e.KeyChar.ToString(), -101);
                        

                        if (this.IsEditing)
                        {
                            this.TextBoxControl.Select(1, Math.Max(0, this.TextBoxControl.Text.Length - 1));
                        }
                        this.ApplyFilter();
                        e.Handled = true;
                    }
                }
                else
                {
                    //// Key pressed after cell has been switched into edit mode.
                    char charCode = (char)(e.KeyChar & 255);

                    //// Pass the e.KeyChar to the IsControl method, not the charCode.
                    if ((!Char.IsControl(e.KeyChar) || charCode == 8 || charCode == 22 /*Paste*/|| charCode == 0 /*Delete*/) && !IsReadOnly())
                    {
                        if (listBoxPart.Items == null)
                        {
                            object ds = model.GetDataSource(this.StyleInfo);
                            if (ds == null)
                            {
                                base.OnKeyPress(e);
                                return;
                            }

                            listBoxPart.DataSource = ds;
                        }

                        int prevSelectionStart = TextBox.SelectionStart;

                        string newText = TextBoxText;
                        newText = TextBoxText.Remove(TextBox.SelectionStart, TextBox.SelectionLength);
                        string s = newText.Insert(TextBox.SelectionStart, (!Char.IsControl(e.KeyChar) ? e.KeyChar.ToString() : string.Empty));
                        
                        
                        if (this.ValidateString(s))
                        {
                            if (this.NotifyCurrentCellChanging())
                            {
                                CurrentCell.IsModified = true;
                                SetTextBoxText(s, true);

                                if (charCode != 8 /*backSpace */ && charCode != 22 /*Paste*/ && charCode != 0 /*Delete*/)
                                    TextBox.SelectionStart = prevSelectionStart + 1;

                                this.Model.Select(style.TableCellIdentity, s, -101);
                                if (!this.Model.ApplyFilterOnlyOnCellLostFocus)
                                {
                                    this.ApplyFilter();
                                }

                            }
                        }
                        
                        e.Handled = true;
                    }
                }
            }
        }
        /// <summary>
        /// Is triggered when the key is released in keyboard
        /// </summary>
        /// <param name="e">KeyEventArgs</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if ((this.Model.ApplyFilterOnlyOnCellLostFocus) && (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Up || e.KeyCode == Keys.Escape))
            {             

                    this.Model.ApplyFilters();               
            }

            base.OnKeyUp(e);

        }

        /// <summary>
        /// Is triggered when the key is pressed in keyboard
        /// </summary>
        /// <param name="e">KeyEventArgs</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            object savedEditState = this.GetEditState();
            bool ignoreWmChar = true;

            if (!e.Handled)
            {
                switch (e.KeyCode)
                {
                    case Keys.Back:
                    case Keys.Delete:
                        if (!CurrentCell.HasControlFocus || !CurrentCell.IsEditing)
                        {
                            if (Control.ModifierKeys != Keys.Alt)
                            {
                                if (this.Grid.ShouldDeleteKeyClearCurrentCellContentsOnly())
                                {
                                    //// cell is not readonly and OnDeleteCell notification returns true
                                    if (!IsReadOnly() && OnDeleting() && this.Grid.RaiseCurrentCellDeleting())
                                    {
                                        //// Delete text, SetTextBoxText call BeginEdit()
                                        SetTextBoxText(string.Empty, true);
                                        CurrentCell.IsModified = true;
                                        CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                    }

                                    e.Handled = true;
                                }

                                // else if (Control.ModifierKeys == Keys.Shift)
                                //                                {
                                //                                    Grid.Model.CutPaste.Cut();
                                //                                }
                                //                                else
                                //                                {
                                //                                    Grid.Model.Clear(Control.ModifierKeys == Keys.Control);
                                //                                }
                            }
                        }
                        else if (Grid.IsWindowless || TextBox is TextBox)  //// TextBox is "Original TextBox", not RichTextbox ...
                        {
                            if (TextBox.SelectionLength > 0)
                            {
                                TextBox.SelectedText = string.Empty;
                            }
                            else if (TextBox.SelectionStart >= 0)
                            {
                                if (e.KeyCode == Keys.Back)
                                {
                                    if (TextBox.SelectionStart > 0)
                                    {
                                        TextBox.Select(TextBox.SelectionStart - 1, 1);
                                    }
                                }
                                else if (e.KeyCode == Keys.Delete)
                                {
                                    TextBox.Select(TextBox.SelectionStart, 1);
                                }
                                else
                                {
                                    TextBox.Select(TextBox.SelectionStart, 1);
                                }

                                TextBox.SelectedText = string.Empty;
                            }

                            e.Handled = true;
                            ignoreWmChar = TextBox is GridOriginalTextBoxControl && TextBox.SelectionLength == 0;
                        }

                        break;

                    case Keys.Escape:
                        if (CurrentCell.IsDroppedDown)
                        {
                            CurrentCell.CloseDropDown(PopupCloseType.Canceled);
                            e.Handled = true;
                        }
                        else if (CurrentCell.IsModified)
                        {
                            CurrentCell.RejectChanges();
                            CurrentCell.CancelEdit();
                            CurrentCell.Refresh();
                            CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                            e.Handled = true;
                        }
                        else
                        {
                            CurrentCell.CancelEdit();
                        }

                        if ((this.Grid.Model.Options.ActivateCurrentCellBehavior & Syncfusion.Windows.Forms.Grid.GridCellActivateAction.SetCurrent) != 0)
                        {
                            CurrentCell.BeginEdit();
                        }

                        return;

                    case Keys.End:
                        if (Control.ModifierKeys == Keys.None && !CurrentCell.HasControlFocus && !this.DisableTextBox)
                        {
                            if (CurrentCell.BeginEdit())
                            {
                                // Position caret.
                                CurrentCell.HasControlFocus = true;
                                TextBox.SelectionStart = TextBox.Text.Length;
                                TextBox.SelectionLength = 0;
                                CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                e.Handled = true;
                            }
                        }

                        break;

                    case Keys.Home:
                        if (Control.ModifierKeys == Keys.None && !CurrentCell.HasControlFocus && !this.DisableTextBox)
                        {
                            if (CurrentCell.BeginEdit())
                            {
                                // Position caret.
                                CurrentCell.HasControlFocus = true;
                                TextBox.SelectionStart = 0;
                                TextBox.SelectionLength = 0;
                                CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                this.Grid.Update();
                                e.Handled = true;
                            }
                        }

                        break;

                    case Keys.Enter:
                        if (Control.ModifierKeys == Keys.None && CurrentCell.IsEditing && this.StyleInfo.AllowEnter && this.NotifyCurrentCellChanging())
                        {
                            SetSelectedText("\n", true);
                            CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                            e.Handled = true;
                        }

                        break;

                    case Keys.Tab:
                        if (this.Grid.WantTabKey)
                        {
                            ignoreWmChar = true;
                        }

                        break;
                }
            }

            // base.OnKeyDown(e);
        }
        #endregion

        #region Filter Related

        /// <summary>
        /// A method to set default compare operator image in FilterButton explicitly.
        /// </summary>
        /// <param name="key">Unique ColumnGroupId.</param>
        /// <param name="value">Logical operator to be used for comparison.</param>
        public void SetLogicalCompareOperatorImage(object key, string value)
        {
            if (this.Model.CompareOperatorListPart.Items.Contains(value))
                this.Model.SetLogicalCompareOperator(key, value);
            else
            {
                if (this.Model.CompareOperatorListPart.Items.Count > 0)
                {
                    this.Model.SetLogicalCompareOperator(key, this.Model.CompareOperatorListPart.Items[0].ToString());
                }
            }
        }

        void ApplyFilter()
        {
            this.Grid.CurrentCell.Lock();
            this.Model.ApplyFilters();
            this.Grid.CurrentCell.Unlock();
        }

        object GetKey()
        {
            return this.GetKey(RowIndex, ColIndex);
        }

        object GetKey(int rowIndex, int colIndex)
        {
            GridTableCellStyleInfo style = this.Grid.GetTableViewStyleInfo(colIndex, colIndex);
            if (!style.IsTableCell || style.TableCellIdentity.Column == null)
            {
                return string.Empty;
            }

            return style.TableCellIdentity.Column.Name;
        }

        /// <summary>
        /// Determines the text from record filter criteria that should be displayed in filterbar cell.
        /// </summary>
        /// <param name="style">Cell style information.</param>
        /// <returns>Filter bar text.</returns>
        public string GetFilterBarText(GridStyleInfo style)
        {
            GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)style;
            GridTableCellStyleInfoIdentity tableCellIdentity = tableStyleInfo.TableCellIdentity;
            GridTableDescriptor td = tableCellIdentity.Table.TableDescriptor;
            string filterName = this.Model.GetUniqueColumnGroupId(tableCellIdentity);
            RecordFilterDescriptor filter = this.Model.GetRecordFilter(td.RecordFilters, tableCellIdentity, filterName);
            object value = this.Model.SelectCustomText;
            if (filter == null)
            {
                value = this.Model.SelectAllText;
            }
            else if (filter.Conditions.Count == 1)
            {
                if (this.Model.GetCompareOperatorName(filterName).Equals(string.Empty))
                {
                    bool set = false;

                    if (filter.Conditions[0].CompareOperator == FilterCompareOperator.Like)
                    {
                        string compareValue = filter.Conditions[0].CompareValue.ToString();

                        if (compareValue.StartsWith("*"))
                        {
                            this.Model.SetLogicalCompareOperator(filterName, SR.EndsWith);
                            
                            if (CurrentCell.HasCurrentCell && CurrentCell.HasCurrentCellAt(RowIndex, ColIndex))
                            {
                                value = this.TextBox.Text = compareValue.Substring(1);
                            }
                            else
                            {
                                value = compareValue.Substring(1);
                            }

                            set = true;
                        }
                        else if (compareValue.EndsWith("*"))
                        {
                            this.Model.SetLogicalCompareOperator(filterName, SR.StartsWith);
                            
                            if (CurrentCell.HasCurrentCell && CurrentCell.HasCurrentCellAt(RowIndex, ColIndex))
                            {
                                value = this.TextBox.Text = compareValue.Remove(compareValue.LastIndexOf("*"));
                            }
                            else
                            {
                                value = compareValue.Remove(compareValue.LastIndexOf("*"));
                            }

                            set = true;
                        }
                    }
                    if (!set)
                    {
                        this.Model.SetLogicalCompareOperator(filterName, filter.Conditions[0].CompareOperator.ToString());
                        value = filter.Conditions[0].CompareValue;
                    }
                }
                else if (filter.Conditions[0].CompareOperator == FilterCompareOperator.Equals)
                {
                    value = filter.Conditions[0].CompareValue;
                    if (filter.Conditions[0].CompareText == "(null)" || filter.Conditions[0].CompareText == string.Empty)
                        value = this.Model.SelectEmptyText;
                }
                else
                {
                    value = filter.Conditions[0].CompareValue;
                    if (filter.Conditions[0].CompareText == "(null)" || filter.Conditions[0].CompareText == string.Empty)
                        value = this.Model.SelectEmptyText;
                }
            }
            else if (this.Model.IsExpressionFilter(tableStyleInfo))
            {
                value = filter.Expression;
            }
            else if (this.Model.GetCompareOperatorName(filterName).Equals(string.Empty) &&
                !filter.Expression.Equals(string.Empty))
            {
                this.Model.SetLogicalCompareOperator(filterName, "Expression Match...");
                this.Model.CompareOperatorListPart.Update();
                value = filter.Expression;
            }

            return this.Model.ExcludeFilterFormat(value, tableStyleInfo.TableCellIdentity.Column.Name);
        }

        /// <summary>
        /// Occurs when <see cref="OnPrepareViewStyleInfo"/> event is called for this cell.
        /// </summary>
        /// <param name="e">Event args.</param>
        public override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            e.Style.DropDownStyle = GridDropDownStyle.Editable;
            e.Style.ExclusiveChoiceList = false;
            e.Style.ShowButtons = GridShowButtons.Show;
            e.Style.Clickable = true;
            e.Style.WrapText = false;
            this.ApplyFormattedValue(e.Style);
            if ((e.Style.Text == this.Model.SelectAllText || e.Style.Text == this.Model.SelectCustomText || e.Style.Text == this.Model.SelectEmptyText))
            {
                e.Style.Text = string.Empty;
            }
            base.OnPrepareViewStyleInfo(e);
        }
        /// <summary>
        /// It is triggered when the cell is drawn
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="clientRectangle">Rectangle</param>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="style">GridStyleInfo</param>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            this.ApplyFormattedValue(style);
            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
        }
        private void ApplyFormattedValue(GridStyleInfo style)
        {
            string filterText = this.GetFilterBarText(style);
            double d;
            if (!string.IsNullOrEmpty(style.Format) && double.TryParse(filterText, out d))
                style.FormattedText = string.Format("{0:" + style.Format + "}", d);
            else
                style.FormattedText = filterText;
        }
        #endregion

        #region DropDown Overrides

        /// <summary>
        /// Occurs when the filter drop down is being shown.
        /// </summary>
        /// <param name="sender">Cell renderer.</param>
        /// <param name="e">Event args.</param>
        public override void DropDownContainerShowingDropDown(object sender, CancelEventArgs e)
        {
            GridComboBoxListBoxPart listBoxPart = (GridComboBoxListBoxPart)ListBoxPart;

            if (!this.DisableTextBox && this.IsControlVisible())
            {
                listBoxPart.BackColor = this.TextBoxControl.BackColor;
                listBoxPart.ForeColor = this.TextBoxControl.ForeColor;
                listBoxPart.Font = this.TextBoxControl.Font;
                listBoxPart.RightToLeft = this.Grid.RightToLeft;
            }
            else
            {
                listBoxPart.BackColor = Color.FromArgb(255, StyleInfo.Interior.BackColor);
                listBoxPart.ForeColor = StyleInfo.TextColor;
                listBoxPart.Font = StyleInfo.GdipFont;
                listBoxPart.RightToLeft = this.Grid.RightToLeft;
            }

            Size size = this.tableCellIdentity.Table.TableOptions.MaxFilterBarChoiceListSize;
            size.Width = this.Grid.GetColWidth(ColIndex);
            GridCurrentCellShowingDropDownEventArgs ce = new GridCurrentCellShowingDropDownEventArgs(size);
            this.Grid.RaiseCurrentCellShowingDropDown(ce);
            if (ce.Cancel)
            {
                e.Cancel = true;
                return;
            }

            listBoxPart.DropDownRows = ce.Size.Height / listBoxPart.ItemHeight;

            if (this.ListBoxPart.Items.Count == 0)
            {
                e.Cancel = true;
                return;
            }

            listBoxPart.Size = ce.Size;
            listBoxPart.Text = TextBoxText;
            this.DropDownContainer.Size = listBoxPart.Size;
        }
        /// <summary>
        /// Is triggered when the drop-down is shown
        /// </summary>
        protected override void OnShowDropDown()
        {
            if (!this.IsComparerListBoxPart)
            {
                base.OnShowDropDown();
                return;
            }

            if (IsDroppedDown)
            {
                return;
            }

            this.Grid.CurrentCell.BeginEdit();
            this.Grid.Update();
            if (this.DropDownContainer != null)
            {
                this.DropDownContainer.PopupParent = this.Grid.DropDownContainerParent;
                PopupRelativeAlignment popupAlign;
                this.DropDownContainer.ShowPopup(this.DropDownPart.GetLocationForPopupAlignment(PopupRelativeAlignment.BottomLeft, out popupAlign));
            }
        }

        /// <summary>
        /// Handles the <see cref="Control.MouseUp"/> event of the list box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with event data.</param>
        protected override void ListBoxMouseUp(object sender, MouseEventArgs e)
        {
            CurrentCell.CloseDropDown(PopupCloseType.Done);
            if (!this.IsComparerListBoxPart)
            {
                GridColumnDescriptor column = null;
                GridGroupingControl gc = Grid.GroupingControl;
                GridCurrentCell cc = gc.TableControl.CurrentCell;
                if (cc.Renderer is GridNestedTableControlCellRenderer)
                {
                    GridNestedTableControlCellRenderer rend = cc.Renderer as GridNestedTableControlCellRenderer;
                    column = rend.Control.TableDescriptor.Columns[rend.Control.TableDescriptor.ColIndexToField(this.ColIndex)];
                }
                else
                {
                     column = gc.TableDescriptor.Columns[gc.TableDescriptor.ColIndexToField(this.ColIndex)];
                }
                if (this.listBoxPart.SelectedIndex >= 0)
                {
                    FilterBarSelectedItemChangingEventArgs fce = new FilterBarSelectedItemChangingEventArgs(this.listBoxPart, column, this.ListBoxPart.SelectedIndex, this.ListBoxPart.SelectedItem.ToString());
                    gc.OnFilterBarSelectedItemChanging(fce);
                    if (!fce.Cancel)
                    {
                        if (fce.SelectedIndex >= 0 && this.ListBoxPart.Items.Count > fce.SelectedIndex)
                        {
                            if (this.ListBoxPart.SelectedIndex != fce.SelectedIndex)
                            {
                                this.ListBoxPart.SelectedIndex = fce.SelectedIndex;
                            }
                            else
                            {
                                if (!this.ListBoxPart.SelectedItem.Equals(fce.SelectedText) && this.ListBoxPart.Items.Contains(fce.SelectedText))
                                {
                                    this.ListBoxPart.SelectedItem = fce.SelectedText;
                                }
                            }
                        }
                        int selectedIndex = this.ListBoxPart.SelectedIndex;
                        this.Model.Select(this.tableCellIdentity, this.ListBoxPart.SelectedIndex);

                        // Reset current element before applying filter.
                        this.Grid.GroupingControl.Table.CurrentRecordManager.NavigateTo(null);

                        // Apply filter.
                        this.Model.ApplyFilters();
                        if (this.ListBoxPart.SelectedIndex < 0)
                        {
                            this.ListBoxPart.SelectedIndex = selectedIndex;
                        }
                        FilterBarSelectedItemChangedEventArgs fe = new FilterBarSelectedItemChangedEventArgs(column, this.ListBoxPart.SelectedIndex, this.ListBoxPart.SelectedItem.ToString());
                        gc.OnFilterBarSelectedItemChanged(fe);
                    }
                }
                // GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo) StyleInfo;
                // GridTableCellStyleInfoIdentity tableCellIdentity2 = tableStyleInfo.TableCellIdentity;

                //// If this filterbar belongs to a child group or nested table then there is a good chance
                //// the current row is no FilterBar anymore. Best is to reset the current cell.
                // if (tableCellIdentity2 == null || tableCellIdentity2.DisplayElement != tableCellIdentity.DisplayElement)
                //    CurrentCell.ResetCurrentCellWithoutDeactivate();
                // else
                //    ControlValue = GetFilterBarText(StyleInfo);// don't call base class - ignore.
            }
            else
            {
                GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)StyleInfo;
                string value = this.GetFilterBarText(StyleInfo);
                this.Model.SetLogicalCompareOperator(tableStyleInfo.TableCellIdentity.Column.Name, SelectedString(ListBoxPart.SelectedItem.ToString()));
                if (value.Length > 0)
                {
                    this.Grid.GroupingControl.Table.CurrentRecordManager.NavigateTo(null);
                    if (value == this.Model.SelectAllText)
                        value = "";
                    this.Model.Select(tableStyleInfo.TableCellIdentity, value, -101);
                    this.ApplyFilter();
                }

                this.Grid.CurrentCell.Refresh();
            }
        }

        #endregion

        /// <summary>
        /// Gets a reference to the cell model.
        /// </summary>
        public new GridTableFilterBarExtCellModel Model
        {
            get
            {
                return (GridTableFilterBarExtCellModel)base.Model;
            }
        }

        /// <summary>
        /// Gets a reference to the parent grid.
        /// </summary>
        public new GridTableControl Grid
        {
            get
            {
                return (GridTableControl)base.Grid;
            }
        }
        /// <summary>
        /// Is executed when the program starts executing
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)StyleInfo;
            this.tableCellIdentity = tableStyleInfo.TableCellIdentity;
            CurrentCell.Lock();
            this.tableCellIdentity.Table.CurrentRecordManager.NavigateTo(null);
            this.tableCellIdentity.Table.CurrentElement = this.tableCellIdentity.DisplayElement;
            CurrentCell.Unlock();
            base.OnInitialize(rowIndex, colIndex);
        }
    }
    #endregion

    #region Filter Table Class
    /// <summary>
    /// Holds the expression filters and defines the operations that can be performed on these filters.
    /// </summary>
    public class CustomFilters : IEnumerator, IEnumerable
    {
        int index = -1;
        string[] keys = null;
        Hashtable expressionFilters;

        /// <summary>
        /// Constructor for CustomFilters.
        /// </summary>
        public CustomFilters()
        {
            this.expressionFilters = new Hashtable();
        }       

        /// <summary>
        /// Gets the number of filters.
        /// </summary>
        public int Count
        {
            get 
            { 
                return this.expressionFilters.Count; 
            }
        }

        /// <summary>
        /// Determines whether the list contains the given filter.
        /// </summary>
        /// <param name="name">Name of the filter expression.</param>
        /// <returns>True if it contains; False otherwise.</returns>
        public bool Contains(string name)
        {
            return this.expressionFilters.ContainsKey(name);
        }

        internal ExpressionAndImage this[string key]
        {
            get
            {
                return (ExpressionAndImage)this.expressionFilters[key];
            }
        }

        /// <summary>
        /// Returns the filter expression given its name.
        /// </summary>
        /// <param name="key">Filter expression name.</param>
        /// <returns>Filter expression.</returns>
        public string GetExpression(string key)
        {
            ExpressionAndImage expImg = (ExpressionAndImage)this.expressionFilters[key];
            return expImg.Expression;
        }

        /// <summary>
        /// For internal use to get image.
        /// </summary>
        /// <param name="key">Filter expression name.</param>
        /// <returns>Bitmap image.</returns>
        public Bitmap GetImage(string key)
        {
            ExpressionAndImage expImg = (ExpressionAndImage)this.expressionFilters[key];
            return expImg.Image;
        }

        /// <summary>
        /// Add a filter expression to the CompareOperaatorList.
        /// </summary>
        /// <param name="name">Name of the filter expression.</param>
        /// <param name="expression">A formula expression with {VALUE} token.</param>
        public void Add(string name, string expression)
        {
            ExpressionAndImage expImg = new ExpressionAndImage(name);
            expImg.Expression = expression;
            this.expressionFilters.Add(name, expImg);
        }

        /// <summary>
        /// Add a filter expression to the CompareOperaatorList.
        /// </summary>
        /// <param name="name">Name of the filter expression.</param>
        /// <param name="expression">A formula expression with {VALUE} token.</param>
        /// <param name="img">Bitmap to display in the button for this filter expression.</param>
        public void Add(string name, string expression, Bitmap img)
        {
            ExpressionAndImage expImg = new ExpressionAndImage(name);
            expImg.Expression = expression;
            expImg.Image = img;
            if (!this.expressionFilters.Contains(name))
                this.expressionFilters.Add(name, expImg);
        }

        internal class ExpressionAndImage
        {
            string expression;
            Bitmap image;
            string name;          

            public ExpressionAndImage(string name)
            {
                this.name = name;
            }

            public Bitmap Image
            {
                get 
                { 
                    return this.image; 
                }

                set 
                { 
                    this.image = value; 
                }
            }

            public string Expression
            {
                get 
                { 
                    return this.expression; 
                }

                set 
                { 
                    this.expression = value; 
                }
            }

            public string Name
            {
                get 
                { 
                    return this.name; 
                }

                set 
                { 
                    this.name = value; 
                }
            }
        }

        #region IEnumerator Members
       
        /// <summary>
        /// Gets the current filter.
        /// </summary>
        public object Current
        {
            get
            {
                if (this.keys == null)
                {
                    return null;
                }

                return this[this.keys[this.index]];
            }
        }

        /// <summary>
        /// Navigates to the next filter in the list.
        /// </summary>
        /// <returns>True if the operation is successful; False otherwise.</returns>
        public bool MoveNext()
        {
            if (this.index == -1)
            {
                if (this.keys != null)
                {
                    this.keys = null;
                }

                this.keys = new string[this.expressionFilters.Count];
                this.expressionFilters.Keys.CopyTo(this.keys, 0);
            }

            if (this.index < this.expressionFilters.Count - 1)
            {
                this.index++;
                return true;
            }
            else
            {
                this.index = -1;
                return false;
            }
        }

        /// <summary>
        /// Resets the navigation key.
        /// </summary>
        public void Reset()
        {
            this.index = -1;
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns the enumerator for the current object..
        /// </summary>
        /// <returns>Enumerator for the current object.</returns>
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }
        #endregion
    }
    #endregion

    #region Filter Event
    /// <summary>
    /// Delegate that creates a list of Compare operators.
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">CreateCompareOperatorListEventArgs</param>
    public delegate void CreateCompareOperatorListHandler(object sender, CreateCompareOperatorListEventArgs e);

    /// <summary>
    /// Holds the event args for the CreateCompareOperatorList event and lets you customized its attributes.
    /// </summary>
    public class CreateCompareOperatorListEventArgs
    {
        /// <summary>
        /// Constructor for CreateCompareOperatorListEventArgs.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public CreateCompareOperatorListEventArgs(CustomFilters filters)
        {
            this.filters = filters;
        }

        CustomFilters filters;

        /// <summary>
        /// Gets the filters.
        /// </summary>
        public CustomFilters Filters
        {
            get
            {
                return this.filters;
            }
        }
    }

    #endregion

    #region GridListFilterBarCell Model Class
    /// <summary>
    /// Implements the DataModel part for a ExtendedTableFilterBar cell.
    /// </summary>
    [Serializable]
    public class GridListFilterBarCellModel : GridDropDownGridListControlCellModel
    {
        GridListFilterBarCellRenderer renderer = null;
        int buttonWidth = SystemInformation.VerticalScrollBarWidth;
        ListBox compareOprListBox = null;
        internal CustomFilters customFilters = new CustomFilters();
        Hashtable compareOprBitmaps;
        int originalCompareOperCount = 0;
        Hashtable compareOperator = new Hashtable();
        RecordFilterDescriptorCollection recordFilters;
        TableDescriptor tableDescriptor;
        Syncfusion.Grouping.Table table;
        internal static string selectAllText;
        internal static string selectCustomText;
        internal static string selectEmptyText;
        bool applyFilterOnlyOnCellLostFocus;
        internal bool isCombobox = false;
        /// <overload>
        /// Initializes a new <see cref="GridTableFilterBarExtCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridTableFilterBarExtCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridListFilterBarCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = false;
            ButtonBarSize = new Size(this.ButtonWidth * 2, 0);
            SupportsChoiceList = true;
            AllowDoubleClickChangeSelectedIndex = false;
        }
        /// <summary>
        /// Applies FilterBar cell to the Grid
        /// </summary>
        /// <param name="grid">GridModel</param>
        /// <param name="filterOnlyOnCellLostFocus">bool</param>
        /// <param name="isCombobox">bool</param>
        public GridListFilterBarCellModel(GridModel grid,bool filterOnlyOnCellLostFocus, bool isCombobox)
            : base(grid)
        {
            AllowFloating = false;
            ButtonBarSize = new Size(this.ButtonWidth * 2, 0);
            SupportsChoiceList = true;
            AllowDoubleClickChangeSelectedIndex = false;
            ApplyFilterOnlyOnCellLostFocus = filterOnlyOnCellLostFocus;
            this.isCombobox = isCombobox;
        }
        /// <summary>
        /// Applies Filter to GridList.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="filterOnlyOnCellLostFocus"></param>
        public GridListFilterBarCellModel(GridModel grid, bool filterOnlyOnCellLostFocus)
            : this(grid)
        {
            ApplyFilterOnlyOnCellLostFocus = filterOnlyOnCellLostFocus;
        }

        GridListFilterBarCellRenderer Renderer
        {
            get
            {
                return this.renderer;
            }
        }

        internal Hashtable CompareOperators
        {
            get
            {
                return this.compareOperator;
            }
            set
            {
                if (this.compareOperator != value)
                    this.compareOperator = value;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GridTableFilterBarExtCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridListFilterBarCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
            base.ButtonBarSize = new Size(this.ButtonWidth * 2, 0);
        }

        static GridListFilterBarCellModel()
        {
            GridListFilterBarCellModel.selectAllText = SR.GetString(SR.All);
            GridListFilterBarCellModel.selectCustomText = SR.GetString(SR.Custom);
            GridListFilterBarCellModel.selectEmptyText = SR.GetString(SR.Empty);
        }

        internal int ButtonWidth
        {
            get
            {
                return this.buttonWidth;
            }
            set
            {
                this.buttonWidth = value;
            }
        }

        GridGroupingControl GroupingGrid
        {
            get
            {
                return ((GridTableModel)this.Grid).GroupingControl;
            }
        }

        internal ListBox CompareOperatorListPart
        {
            get
            {
                if (this.compareOprListBox == null)
                {
                    this.compareOprListBox = (ListBox)new GridComboBoxListBoxPart();
                    this.compareOprListBox.Tag = "CompareOperatorList";
                    this.compareOprListBox.Items.AddRange(new string[] { SR.GetString(SR.StartsWith), SR.GetString(SR.EndsWith), SR.GetString(SR.Equal),
                        SR.GetString(SR.NotEquals), SR.GetString(SR.LessThan), SR.GetString(SR.LessThanOrEqualTo), SR.GetString(SR.GreaterThan),
                        SR.GetString(SR.GreaterThanOrEqualTo), SR.GetString(SR.Like), SR.GetString(SR.Match) });                 
                    this.originalCompareOperCount = this.compareOprListBox.Items.Count;
                    if (this.customFilters != null)
                    {
                        this.customFilters = null;
                    }
                    this.customFilters = new CustomFilters();
                    CreateCompareOperatorListEventArgs arg = new CreateCompareOperatorListEventArgs(this.customFilters);
                    this.CreateCompareOperatorList += new CreateCompareOperatorListHandler(this.GridTableFilterBarExtCellModel_CreateCompareOperatorList);
                    this.RaiseCreateCompareOperatorList(this, arg);
                    if (arg.Filters.Count == 0)
                    {
                        this.customFilters = null;
                    }
                    else
                    {
                        foreach (CustomFilters.ExpressionAndImage expImg in this.customFilters)
                        {
                            this.compareOprListBox.Items.Add(expImg.Name);
                        }
                    }
                    this.InitCompareOprBitmaps();
                }
                return this.compareOprListBox;
            }
        }

        internal void GridTableFilterBarExtCellModel_CreateCompareOperatorList(object sender, CreateCompareOperatorListEventArgs e)
        {
            Bitmap bm = DynamicFilterBitmaps.GetBitmap("em");
            if (!e.Filters.Contains("Expression Match..."))
                e.Filters.Add("Expression Match...", "MATCH '{VALUE}'", bm);
        }

        void InitCompareOprBitmaps()
        {
            if (this.compareOprBitmaps != null)
            {
                this.compareOprBitmaps = null;
            }
            this.compareOprBitmaps = new Hashtable();
            StringCollection operatorNames = new StringCollection();
            operatorNames.AddRange(new string[] 
            {                 
               SR.StartsWith , SR.EndsWith , SR.Equal , SR.NotEquals , SR.LessThan , SR.LessThanOrEqualTo ,
           SR.GreaterThan , SR.GreaterThanOrEqualTo , SR.Like , SR.Match  
            });

            for (int i = 0; i < operatorNames.Count; i++)
            {
                this.compareOprBitmaps.Add(operatorNames[i], DynamicFilterBitmaps.GetBitmap(operatorNames[i]));
            }
            this.compareOprBitmaps.Add("Default", DynamicFilterBitmaps.GetBitmap("Default"));
            this.compareOprBitmaps.Add("Custom", DynamicFilterBitmaps.GetBitmap("Custom"));
            if (this.customFilters != null)
            {
                foreach (CustomFilters.ExpressionAndImage expImg in this.customFilters)
                {
                    if (expImg.Image != null)
                    {
                        this.compareOprBitmaps.Add(expImg.Name, expImg.Image);
                    }
                }
            }
        }

        #region CreateCompareOperatorList Event
        /// <summary>
        /// Occurs when the filter button is clicked to show a drop down list of supported compare operators.
        /// </summary>
        public event CreateCompareOperatorListHandler CreateCompareOperatorList;

        internal void RaiseCreateCompareOperatorList(object sender, CreateCompareOperatorListEventArgs e)
        {
            if (this.CreateCompareOperatorList != null)
            {
                this.CreateCompareOperatorList(sender, e);
            }
        }

        #endregion

        internal void SetLogicalCompareOperator(object key, string name)
        {
            if (this.compareOperator.ContainsKey(key))
                this.compareOperator[key] = name;
            else
                this.compareOperator.Add(key, name);
        }

        internal string GetCompareOperatorName(object key)
        {
            if (this.compareOperator.ContainsKey(key))
            {
                return (string)this.compareOperator[key];
            }
            else
            {
                return string.Empty;
            }
        }

        internal Bitmap GetCompareOperatorImage(GridStyleInfo style)
        {
            GridTableCellStyleInfo tableCell = (GridTableCellStyleInfo)style;
            string name = this.GetCompareOperatorName(tableCell.TableCellIdentity.Column.Name);
            if (this.compareOprBitmaps == null)
            {
                this.InitCompareOprBitmaps();
            }

            if (this.Renderer.GetFilterBarText(style) == selectCustomText)
            {
                return (Bitmap)this.compareOprBitmaps["Custom"];
            }
            if (name == string.Empty)
            {
                return (Bitmap)this.compareOprBitmaps[SR.StartsWith];
            }
            if (this.compareOprBitmaps.ContainsKey(name))
            {
                return (Bitmap)this.compareOprBitmaps[name];
            }
            if (this.customFilters != null && this.customFilters.Contains(name))
            {
                Bitmap bm = this.customFilters.GetImage(name);
                if (bm != null)
                {
                    return bm;
                }
            }
            // Default Filter Bitmap
            return (Bitmap)this.compareOprBitmaps["Default"];
        }

        /// <summary>
        /// A method that returns logical compareoperator.
        /// </summary>
        /// <param name="key">Column Name</param>
        /// <returns>A FilterCompareOperator.</returns>
        protected object GetLogicalCompareOperator(string key)
        {
            string name = this.GetCompareOperatorName(key);
            switch (name)
            {
                case SR.Like:
                case SR.StartsWith:
                case SR.EndsWith:
                    return FilterCompareOperator.Like;
                case SR.Equal:
                    return FilterCompareOperator.Equals;
                case SR.NotEquals:
                    return FilterCompareOperator.NotEquals;
                case SR.LessThan:
                    return FilterCompareOperator.LessThan;
                case SR.LessThanOrEqualTo:
                    return FilterCompareOperator.LessThanOrEqualTo;
                case SR.GreaterThan:
                    return FilterCompareOperator.GreaterThan;
                case SR.GreaterThanOrEqualTo:
                    return FilterCompareOperator.GreaterThanOrEqualTo;
                case SR.Match:
                    return FilterCompareOperator.Match;
                default:
                    if (this.customFilters != null && this.customFilters.Contains(name))
                    {
                        return this.customFilters.GetExpression(name);
                    }
                    else
                    {
                        SetLogicalCompareOperator(key, SR.StartsWith);
                        return FilterCompareOperator.Like;
                    }
            }
        }

        /// <summary>
        /// Apply filters to grid data.
        /// </summary>
        public void ApplyFilters()
        {
            if (this.tableDescriptor != null && this.recordFilters != null)
            {
                this.tableDescriptor.RecordFilters.InitializeFrom(this.recordFilters);
            }
            this.recordFilters = null;
        }

        /// <summary>
        /// Creates choice list for the filter drop down.
        /// </summary>
        /// <param name="listBox">Drop down listbox.</param>
        /// <param name="style">Cell style information.</param>
        /// <param name="exclusive">Indicates whether the listbox is loaded with exlusive choice list or if non-standard values are allowed.</param>
        public override void FillWithChoices(ListBox listBox, GridStyleInfo style, out bool exclusive)
        {
            exclusive = false;
            if (listBox.Tag == null || listBox.Tag.ToString() != "CompareOperatorList")
            {
                GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)style;
                object[] items = (object[])this.GetFilterBarChoices(tableStyleInfo.TableCellIdentity);
                listBox.Items.Clear();
                if (items != null)
                {
                    listBox.Items.Add(SR.GetString(SR.SelectAll));
                    listBox.Items.Add(SR.GetString(SR.Custom));
                    foreach (object item in items)
                    {
                        if (item is DBNull)
                        {
                            listBox.Items.Add(SR.GetString(SR.Empty));
                        }
                        else if (item != null)
                        {
                            listBox.Items.Add(style.GetFormattedText(item));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Fills the filter with choices
        /// </summary>
        /// <param name="listBox">GridListControl</param>
        /// <param name="style">GridStyleInfo</param>
        /// <param name="exclusive">bool</param>
        public void FillWithChoices(GridListControl listBox, GridStyleInfo style, out bool exclusive)
        {
            exclusive = false;
            GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)style;
            listBox.BeginUpdate();
            GridColumnDescriptor column = tableStyleInfo.TableCellIdentity.Column;
            object[] items = (object[])this.GetFilterBarChoices(tableStyleInfo.TableCellIdentity);
            if (items != null)
            {
                listBox.DataSource = null;
                listBox.DisplayMember = tableStyleInfo.TableCellIdentity.Column.MappingName;
                listBox.ValueMember = tableStyleInfo.TableCellIdentity.Column.MappingName;
                DataTable dt = new DataTable();
                dt.Columns.Add(tableStyleInfo.TableCellIdentity.Column.Name);
                dt.Rows.Add(SR.GetString(SR.All));
                dt.Rows.Add(SR.GetString(SR.Custom));
                foreach (object item in items)
                {
                    if (item is DBNull || item == string.Empty)
                    {
                        dt.Rows.Add(SR.GetString(SR.Empty));
                    }
                    else if (item != null && item.ToString() == string.Empty)
                    {
                        listBox.Items.Add(item);
                    }
                    else if (item != null)
                    {
                        if (column.Appearance.AnyRecordFieldCell.CellType == GridCellTypeName.ComboBox)
                            dt.Rows.Add(GetDisplayMember(column.Appearance.AnyRecordFieldCell, item));
                        else
                            dt.Rows.Add(item);
                    }
                }
                listBox.DataSource = dt;
            }
            listBox.EndUpdate();
        }

        public object GetDisplayMember(GridTableCellStyleInfo style, object item)
        {
            if (item.ToString().Equals(string.Empty) || item.ToString().Equals(SelectCustomText) ||item.ToString().Equals(SelectAllText))
                return item;
            object value = null;
            DataTable dt = new DataTable();
            if (style.DataSource is IEnumerable)
            {
                IEnumerable enumlist = style.DataSource as IEnumerable;
                dt = ConvertDataTableFromIEnumerable(enumlist);
            }
            else
            {
                dt = style.DataSource as DataTable;
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr[style.ValueMember].ToString().Equals(item.ToString()))
                    {
                        value = dr[style.DisplayMember];
                        break;
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Creates a Datatable from Ienumerable list and recreate a table using this list
        /// </summary>
        /// <param name="ien">IEnumerable list</param>
        /// <returns>Data Table</returns>
        private DataTable ConvertDataTableFromIEnumerable(IEnumerable ien)
        {
            DataTable dt = new DataTable();
            foreach (object obj in ien)
            {
                Type t = obj.GetType();
                PropertyInfo[] pis = t.GetProperties();
                if (dt.Columns.Count == 0)
                {
                    foreach (PropertyInfo pi in pis)
                    {
                        dt.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo pi in pis)
                {
                    object value = pi.GetValue(obj, null);
                    dr[pi.Name] = value;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }       

        private void SetValueMemberFilter(GridTableCellStyleInfo style, RecordFilterDescriptor filter, FilterCompareOperator condition, object value)
        {
            DataTable dt = new DataTable();
            if (style.DataSource is IEnumerable)
            {
                IEnumerable enumlist = style.DataSource as IEnumerable;
                dt = ConvertDataTableFromIEnumerable(enumlist);
            }
            else
            {
                dt = style.DataSource as DataTable;
            }
            DataRow[] result = dt.Select(GetQuery(condition, style.DisplayMember, value));
            object[] item = new object[result.Length];
            int index = 0;
            foreach (DataRow dr in result)
            {
                item[index] = dr[style.ValueMember];
                filter.Conditions.Add(new FilterCondition(condition, item[index]));
                index++;
            }
        }

        private string GetQuery(FilterCompareOperator condition, string displayMember, object value)
        {
            string operatorName = "LIKE";
            string query = displayMember + " " + operatorName + " '" + value + "'";
            switch (condition)
            {
                case FilterCompareOperator.LessThan: operatorName = "<"; goto default;
                case FilterCompareOperator.GreaterThan: operatorName = ">"; goto default;
                case FilterCompareOperator.NotEquals: operatorName = "<>"; goto default;
                case FilterCompareOperator.LessThanOrEqualTo: operatorName = "<";
                    query = displayMember + " " + operatorName + " '" + value + "' OR " + displayMember + " LIKE '" + value + "*'"; break;
                case FilterCompareOperator.GreaterThanOrEqualTo: operatorName = ">";
                    query = displayMember + " " + operatorName + " '" + value + "' OR " + displayMember + " LIKE '" + value + "*'"; break;
                case FilterCompareOperator.Equals:
                case FilterCompareOperator.Match: operatorName = "LIKE"; goto default;
                default:
                    query = displayMember + " " + operatorName + " '" + value + "'";
                    break;
            }

            return query;
        }

        /// <summary>
        /// Returns the unique choices to be displayed in the filterbar cell.
        /// </summary>
        /// <param name="tableCellIdentity">Table cell identifier.</param>
        /// <returns>Filter bar choices.</returns>
        public virtual object[] GetFilterBarChoices(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            if (tableCellIdentity.FilterBarSummaryDescriptor != null)
            {
                ITreeTableSummary[] summaries = tableCellIdentity.DisplayElement.ParentGroup.GetSummaries(tableCellIdentity.Table);
                int summaryIndex = tableCellIdentity.Table.TableDescriptor.Summaries.IndexOf(tableCellIdentity.FilterBarSummaryDescriptor);
                FilterBarChoicesSummary filterBarChoicesSummary = (FilterBarChoicesSummary)summaries[summaryIndex];
                return filterBarChoicesSummary.Values;
            }
            GridGroupingControl groupGrid = ((GridEngine)tableCellIdentity.Table.Engine).GroupingControl;
            if (groupGrid.OptimizeFilterPerformance)
            {
                GridCurrentCell cc = tableCellIdentity.Table.TableDescriptor.Engine.TableControl.CurrentCell;
                if (cc.Renderer is GridNestedTableControlCellRenderer)
                {
                    GridNestedTableControlCellRenderer rend = cc.Renderer as GridNestedTableControlCellRenderer;
                    cc = rend.GetNestedCurrentCell();
                }
                cc.Lock();
                tableCellIdentity.Table.CurrentElement = tableCellIdentity.DisplayElement;
                tableCellIdentity.Table.CurrentRecordManager.Lock();
                SummaryDescriptor sd = new SummaryDescriptor(
                                tableCellIdentity.Column.Name + "FilterBarChoices",
                                tableCellIdentity.Column.MappingName,
                                new CreateSummaryDelegate(FilterBarChoicesSummary.CreateSummaryMethod));
                sd.IgnoreRecordFilterCriteria = true;
                tableCellIdentity.Table.TableDescriptor.Summaries.Add(sd);
                ITreeTableSummary[] summaries = tableCellIdentity.Table.GetSummaries();
                int summaryIndex = tableCellIdentity.Table.TableDescriptor.Summaries.IndexOf(sd);
                FilterBarChoicesSummary filtersummary = (FilterBarChoicesSummary)summaries[summaryIndex];
                object[] values = filtersummary.Values;
                tableCellIdentity.Table.TableDescriptor.Summaries.Remove(sd);
                tableCellIdentity.Table.SummariesDirty = true;
                tableCellIdentity.Table.CurrentRecordManager.Unlock();
                cc.Unlock();
                return values;
            }

            object[] result = null;
            GridGroupingControl gc = tableCellIdentity.Table.Engine.ParentControl;
            GridQueryFilterBarChoicesEventArgs qe = new GridQueryFilterBarChoicesEventArgs(tableCellIdentity.Column, false, tableCellIdentity.DisplayElement);
            gc.OnQueryFilterBarChoices(qe);
            if (!qe.Cancel)
            {
                result = qe.UniqueFilterBarValues;
            }
            return result;
        }

        /// <summary>
        /// Returns the desired filter from a list available filters.
        /// </summary>
        /// <param name="recordFilters">A list of available record filters.</param>
        /// <param name="tableCellIdentity">Cell identifier.</param>
        /// <param name="filterName">Name of the filter.</param>
        /// <returns>Record filter.</returns>
        public RecordFilterDescriptor GetRecordFilter(RecordFilterDescriptorCollection recordFilters, GridTableCellStyleInfoIdentity tableCellIdentity, string filterName)
        {
            RecordFilterDescriptor filter = null;
            GridTableDescriptor td = tableCellIdentity.Table.TableDescriptor;
            GridRelationDescriptor rd = td.ParentRelation;
            if (rd == null || rd.RelationKind != RelationKind.UniformChildList)
            {
                filter = recordFilters[filterName];
            }
            else
            {
                object[] uniqueId = tableCellIdentity.DisplayElement.ParentGroup.UniqueGroupId;
                string mappingName = tableCellIdentity.Column.MappingName;
                foreach (RecordFilterDescriptor rfd in recordFilters)
                {
                    if (rfd.MappingName == mappingName && rfd.CompareUniqueId(uniqueId))
                    {
                        filter = rfd;
                        break;
                    }
                }
            }
            return filter;
        }

        /// <summary>
        /// Returns the category for a given cell.
        /// </summary>
        /// <param name="tableCellIdentity">Cell identifier.</param>
        /// <returns>Category key.</returns>
        public string GetUniqueColumnGroupId(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            Group g = tableCellIdentity.DisplayElement.ParentGroup;
            object[] id = g.UniqueGroupId;
            if (id == null || id.Length == 0)
            {
                return tableCellIdentity.Column.Name;
            }
            else
            {
                return tableCellIdentity.Column.Name + "@" + g.UniqueGroupIdsToString(); ////.UniqueGroupId;
            }
        }
        /// <summary>
        /// Gets the data source
        /// </summary>
        /// <param name="style">GridStyleInfo</param>
        /// <returns>object</returns>
        public override object GetDataSource(GridStyleInfo style)
        {
            GridTableCellViewStyleInfoIdentity cellIdentity = (GridTableCellViewStyleInfoIdentity)style.CellIdentity;
            object[] obj = this.GetFilterBarChoices(cellIdentity);
            return obj;
        }
        /// <summary>
        /// Returns an array of category keys for this group and all parent groups which is used by FilterBarCells and FitlerBarSummary
        /// to compare whether the conditions should be applied to this group.
        /// </summary>
        /// <param name="tableCellIdentity">Cell identifier.</param>
        /// <returns>An array of category keys.</returns>
        public object[] GetUniqueGroupId(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            Group g = tableCellIdentity.DisplayElement.ParentGroup;
            return g.UniqueGroupId;
        }

        /// <summary>
        /// Determines whether the given cell has a filter.
        /// </summary>
        /// <param name="tableCellIdentity">The table cell identity.</param>
        /// <returns>True if it has a filter; False otherwise.</returns>
        public bool HasFilter(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            return tableCellIdentity.Table.TableDescriptor.RecordFilters.Count > 0;
        }

        internal void ResetFilterBar(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            string filterName = this.GetUniqueColumnGroupId(tableCellIdentity);
            this.tableDescriptor = tableCellIdentity.Table.TableDescriptor;
            this.table = tableCellIdentity.Table;
            this.recordFilters = new RecordFilterDescriptorCollection();
            this.recordFilters.InitializeFrom(tableCellIdentity.Table.TableDescriptor.RecordFilters);
            RecordFilterDescriptor filter = this.recordFilters[filterName];
            if (filter != null)
            {
                this.recordFilters.Remove(filterName);
            }
        }

        /// <summary>
        /// Applies a filter criteria. Note: The first two entries are reserved for (All) and (Custom).
        /// An index greater than one represents a valid choice found with GetFilterBarChoices.
        /// </summary>
        /// <param name="tableCellIdentity">Cell identifier.</param>
        /// <param name="index">Filter choice index.</param>
        public void Select(GridTableCellStyleInfoIdentity tableCellIdentity, int index)
        {
            this.Select(tableCellIdentity, null, index);
        }

        internal void Select(GridTableCellStyleInfoIdentity tableCellIdentity, object value, int index)
        {
            if (index >= 0)
            {
                if (index == 0)
                {
                    this.ResetFilterBar(tableCellIdentity);
                }
                else if (index == 1)
                {
                    //// Custom
                    this.SelectCustomFilterBar(tableCellIdentity);
                }
                else
                {
                    this.SelectItem(tableCellIdentity, index - 2);
                }
            }
            else if (index == -101 && value != null)
            {
                // Entered text filter
                if (value.ToString().Length > 0)
                {
                    this.SelectItem(tableCellIdentity, this.GetLogicalCompareOperator(tableCellIdentity.Column.Name), this.IncludeFilterFormat(value, tableCellIdentity.Column.Name), -101);
                }
                else
                {
                    this.ResetFilterBar(tableCellIdentity);
                }
            }
        }

        internal string IncludeFilterFormat(object value, object key)
        {
            string oprName = this.GetCompareOperatorName(key);
            string filter = Convert.ToString(value);
            switch (oprName)
            {
                case SR.StartsWith:
                    filter = string.Format("{0}*", value);
                    break;
                case SR.EndsWith:
                    filter = string.Format("*{0}", value);
                    break;
                default:
                    if (this.customFilters != null && this.customFilters.Contains(oprName))
                    {
                        string format = this.customFilters.GetExpression(oprName);
                        format = format.Replace("{VALUE}", "{0}");
                        filter = string.Format(format, value);
                        filter = string.Format("[{0}] {1}", key, filter);
                    }
                    break;
            }
            return filter;
        }

        internal bool IsExpressionFilter(GridTableCellStyleInfo style)
        {
            string name = this.GetCompareOperatorName(style.TableCellIdentity.Column.Name);
            if (this.customFilters != null && this.customFilters.Contains(name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal virtual string ExcludeFilterFormat(object value, object key)
        {
            string oprName = this.GetCompareOperatorName(key);
            string filter = Convert.ToString(value);
            switch (oprName)
            {
                case SR.StartsWith:
                    if (filter.EndsWith("*"))
                    {
                        filter = filter.Remove(filter.Length - 1);
                    }
                    break;
                case SR.EndsWith:
                    if (filter.StartsWith("*"))
                    {
                        filter = filter.Remove(0, 1);
                    }
                    break;
                default:
                    if (this.customFilters != null && this.customFilters.Contains(oprName) && filter != string.Empty)
                    {
                        string format = this.customFilters.GetExpression(oprName);
                        int i = format.IndexOf("{VALUE}");
                        string sub = format.Substring(i + 7);
                        filter = filter.Replace(string.Format("[{0}] ", key), string.Empty);
                        filter = filter.Substring(filter.Length);
                        i = sub.IndexOf("{VALUE}");
                        if (i > 0)
                        {
                            sub = sub.Substring(0, i);
                        }
                        i = filter.IndexOf(sub);
                        if (i > 0)
                        {
                            filter = filter.Substring(0, i);
                        }
                        else if (i.Equals(0))
                        {
                            filter = string.Empty;
                        }
                    }
                    break;
            }
            return filter;
        }

        /// <summary>
        /// Gets or sets select all text Default: (All)
        /// </summary>
        public string SelectAllText
        {
            get
            {
                return selectAllText;
            }
            set
            {
                selectAllText = value;
            }
        }

        /// <summary>
        /// Gets or sets select custom text Default: (Custom...)
        /// </summary>
        public string SelectCustomText
        {
            get
            {
                return selectCustomText;
            }
            set
            {
                selectCustomText = value;
            }
        }

        /// <summary>
        /// Gets or sets select empty text Default: (Empty)
        /// </summary>
        public string SelectEmptyText
        {
            get
            {
                return selectEmptyText;
            }
            set
            {
                selectEmptyText = value;
            }
        }

        /// <summary>
        /// Apply filter on cell lost focus
        /// </summary>
        /// 
        [Browsable(false)]
        public bool ApplyFilterOnlyOnCellLostFocus
        {
            get
            {
                return applyFilterOnlyOnCellLostFocus;
            }
            set
            {
                applyFilterOnlyOnCellLostFocus = value;
            }
        }

        /// <summary>
        /// A method to initiate collection dialog editor while selecting custom option.
        /// </summary>
        /// <param name="tableCellIdentity">cell identifier</param>
        protected void SelectCustomFilterBar(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            this.tableDescriptor = tableCellIdentity.Table.TableDescriptor;
            this.table = tableCellIdentity.Table;
            this.recordFilters = new RecordFilterDescriptorCollection();
            this.recordFilters.InitializeFrom(tableCellIdentity.Table.TableDescriptor.RecordFilters);
            string filterName = this.GetUniqueColumnGroupId(tableCellIdentity);
            RecordFilterDescriptor filter = this.recordFilters[filterName];
            if (filter == null)
            {
                filter = new RecordFilterDescriptor();
                filter.Name = tableCellIdentity.Column.Name;
                filter.MappingName = tableCellIdentity.Column.MappingName;
                filter.UniqueGroupId = this.GetUniqueGroupId(tableCellIdentity);
                this.recordFilters.Add(filter);
            }
            RecordFilterDescriptorCollection filters = new RecordFilterDescriptorCollection();
            filters.InitializeFrom(this.recordFilters);
            DialogResult result = this.ShowCollectionDialog(
                tableCellIdentity.Table.TableDescriptor, "RecordFilters", null, typeof(RecordFilterDescriptorCollection));
            if (result == DialogResult.Cancel)
            {
                if (filter != null)
                {
                    this.tableDescriptor.RecordFilters.Remove(filter);
                }
            }
            this.recordFilters = null;
        }
        /// <summary>
        /// For selecting items and applying filer.
        /// </summary>
        /// <param name="tableCellIdentity">GridTableCellStyleInfoIdentity</param>
        /// <param name="index">int</param>
        protected void SelectItem(GridTableCellStyleInfoIdentity tableCellIdentity, int index)
        {
            this.SelectItem(tableCellIdentity, this.GetLogicalCompareOperator(tableCellIdentity.Column.Name), null, index);
        }
        /// <summary>
        /// For selecting items and applying filer.
        /// </summary>
        /// <param name="tableCellIdentity">GridTableCellStyleInfoIdentity</param>
        /// <param name="condition">object</param>
        /// <param name="value">object</param>
        /// <param name="index">int</param>
        protected void SelectItem(GridTableCellStyleInfoIdentity tableCellIdentity, object condition, object value, int index)
        {
            string filterName = this.GetUniqueColumnGroupId(tableCellIdentity);
            this.tableDescriptor = tableCellIdentity.Table.TableDescriptor;
            this.table = tableCellIdentity.Table;
            this.recordFilters = new RecordFilterDescriptorCollection();
            this.recordFilters.InitializeFrom(tableCellIdentity.Table.TableDescriptor.RecordFilters);
            RecordFilterDescriptor filter = this.GetRecordFilter(this.recordFilters, tableCellIdentity, filterName);
            object[] items = this.GetFilterBarChoices(tableCellIdentity);
            value = (index == -101) ? value : ((items[index] is DBNull || items[index].ToString() == string.Empty) ? items[index] : this.IncludeFilterFormat(items[index], tableCellIdentity.Column.Name));
            RecordFilterDescriptor newFilter = new RecordFilterDescriptor();
            newFilter.Name = filterName;
            newFilter.MappingName = tableCellIdentity.Column.MappingName;
            newFilter.UniqueGroupId = this.GetUniqueGroupId(tableCellIdentity);
            GridTableCellStyleInfo style = tableCellIdentity.Column.Appearance.AnyRecordFieldCell;
            if (condition is FilterCompareOperator)
            {
                if (index == -101 && style.CellType == GridCellTypeName.ComboBox)
                    SetValueMemberFilter(style, newFilter, (FilterCompareOperator)condition, value);
                else
                    newFilter.Conditions.Add(new FilterCondition((FilterCompareOperator)condition, value));
            }
            else
            {
                if (value is DBNull)
                    newFilter.Expression = this.IncludeFilterFormat(DBNull.Value.ToString(), tableCellIdentity.Column.Name);
                else
                    newFilter.Expression = (string)value;
            }
            if (filter == null)
            {
                this.recordFilters.Add(newFilter);
            }
            else
            {
                filter.InitializeFrom(newFilter);
            }
        }

        /// <summary>
        /// Occurs immediately before the RecordFilterCollectionEditor Dialog is displayed. The ControlEventArgs.Control 
        /// the form.
        /// </summary>
        public event ControlEventHandler ShowingCustomFilterDialog;

        /// <summary>
        /// Initializes the collection dialog.
        /// </summary>
        /// <param name="instance">table descriptor</param>
        /// <param name="propertyName">name of the property</param>
        /// <param name="provider">service provider</param>
        /// <param name="type">type of RecordFilterDescriptorCollection</param>
        /// <returns>Return value of the dialog box.</returns>
        protected DialogResult ShowCollectionDialog(object instance, string propertyName, IServiceProvider provider, Type type)
        {
            GroupingCollectionEditor ce = new GroupingCollectionEditor(type);
            Syncfusion.ComponentModel.WindowsFormsEditorServiceContainer esc = new Syncfusion.ComponentModel.WindowsFormsEditorServiceContainer(provider);
            esc.ShowingDialog += new ControlEventHandler(esc_ShowingDialog);
            PropertyDescriptor pd = TypeDescriptor.GetProperties(instance)[propertyName];
            Syncfusion.ComponentModel.TypeDescriptorContext tdc = new Syncfusion.ComponentModel.TypeDescriptorContext(instance, pd);
            tdc.ServiceProvider = esc;
            ce.EditValue(tdc, esc, pd.GetValue(instance));
            return esc.DialogResult;
        }

        /// <summary>
        /// Used internally
        /// </summary>
        private void esc_ShowingDialog(object sender, ControlEventArgs e)
        {
            if (ShowingCustomFilterDialog != null)
            {
                ShowingCustomFilterDialog(sender, e);
            }
        }

        /// <summary>
        /// Creates a <see cref="GridTableFilterBarExtCellRenderer"/> for this cell model that is specific to the GridControlBase.
        /// </summary>
        /// <param name="control">The <see cref="GridControlBase"/> the cell renderer is created for.</param>
        /// <returns>A new <see cref="GridTableFilterBarCellRenderer"/> specific for a <see cref="GridControlBase"/>.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            this.renderer = new GridListFilterBarCellRenderer(control, this);
            return this.renderer;
        }
    }

    #endregion

    #region GridListFilterBarCell Renderer Class
    /// <summary>
    /// Implements the renderer part of a TableFilterBar cell.
    /// </summary>
        public class GridListFilterBarCellRenderer : GridDropDownGridListControlCellRenderer
    {
        GridTableCellStyleInfoIdentity tableCellIdentity;
        GridListControl listBoxPart = null;
        ListBox listBoxPart2 = null;
        bool isComparerListBox = false;
        string oldColumn = string.Empty;
        string currColumn = string.Empty;
        Hashtable mouseHoverAtCells = new Hashtable();
        ContextMenuStrip menu = new ContextMenuStrip();
        /// <summary>
        /// A dictionary collection containing the filtercolumn
        /// </summary>
        public Dictionary<int, string> filterColumnColl = new Dictionary<int, string>();
        ToolStripMenuItem[] menuItems;
        /// <summary>
        /// Initializes a new <see cref="GridTableFilterBarCellRenderer"/> object for the given GridControlBase
        /// and <see cref="GridCellModelBase"/>.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridListFilterBarCellRenderer"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase
        /// and GridCellModelBase will be saved.</remarks>
        public GridListFilterBarCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            AddButton(new FilterButtonExt(this));
            ArrayList filterItems = new ArrayList();
            filterItems.Add(SR.GetString(SR.StartsWith));
            filterItems.Add(SR.GetString(SR.EndsWith));
            filterItems.Add(SR.GetString(SR.Equal));
            filterItems.Add(SR.GetString(SR.NotEquals));
            filterItems.Add(SR.GetString(SR.LessThan));
            filterItems.Add(SR.GetString(SR.LessThanOrEqualTo));
            filterItems.Add(SR.GetString(SR.GreaterThan));
            filterItems.Add(SR.GetString(SR.GreaterThanOrEqualTo));
            filterItems.Add(SR.GetString(SR.Match));
            filterItems.Add(SR.GetString(SR.Like));
            filterItems.Add("Expression Match...");

            CustomFilters customFilters = new CustomFilters();
           
            menu.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            menu.Renderer = new ContextMenuRenderer(this.Grid, this);
            menuItems = new ToolStripMenuItem[filterItems.Count];
            Bitmap bmp = DynamicFilterBitmaps.GetBitmap("Default");            
            for (int i = 0; i < filterItems.Count; i++)
            {
                menuItems[i] = new ToolStripMenuItem();
                menuItems[i].Text = filterItems[i].ToString();
                if (menuItems[i].Text.Equals("Expression Match..."))
                {
                    Bitmap bm = DynamicFilterBitmaps.GetBitmap("em");
                    this.Model.customFilters.Add("Expression Match...", "MATCH '{VALUE}'", bm);
                }
                
                string filter = SelectedString(menuItems[i].Text);
                bmp = DynamicFilterBitmaps.GetBitmap(filter);   
                menuItems[i].Image = bmp;
            }
            menu.Items.AddRange(menuItems);
            menu.ItemClicked += new ToolStripItemClickedEventHandler(menu_ItemClicked);
        }


        void menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if ((e.ClickedItem as ToolStripMenuItem).Checked != true)
            {
                string filter = SelectedString(e.ClickedItem.Text);
                SaveSelectedFilterItem(e.ClickedItem.Text);
                foreach (ToolStripMenuItem selecteditem in menuItems)
                {
                    if (selecteditem != e.ClickedItem)
                        selecteditem.Checked = true;
                }

                ApplyFilterValues(filter);
                this.Grid.RefreshRange(GridRangeInfo.Cell(filterRowIndex, filterColumnIndex));
                this.Grid.CurrentCell.Refresh();
                this.Grid.CurrentCell.MoveTo(GridRangeInfo.Cell(filterRowIndex, filterColumnIndex), GridSetCurrentCellOptions.SetFocus);
            }
            else
            {
                ClearFilterFromColumns();
            }
        }

        private void ClearFilterFromColumns()
        {
            GridTableCellStyleInfo style = this.Grid.GetTableViewStyleInfo(filterRowIndex, filterColumnIndex);
            this.Model.ResetFilterBar(style.TableCellIdentity);
            this.Model.ApplyFilters();
            this.ControlText = "";
            if (filterColumnColl.ContainsKey(filterColumnIndex))
                filterColumnColl.Remove(filterColumnIndex);
        }

        private string SelectedString(string selecteditem)
        {
            string filter = string.Empty;
            if (selecteditem.Equals(SR.GetString(SR.StartsWith)))
            {
                filter = "StartsWith";
            }
            else if (selecteditem.Equals(SR.GetString(SR.EndsWith)))
            {
                filter = "EndsWith";
            }
            else if (selecteditem.Equals(SR.GetString(SR.Equal)))
            {
                filter = "Equals";
            }
            else if (selecteditem.Equals(SR.GetString(SR.NotEquals)))
            {
                filter = "NotEquals";
            }
            else if (selecteditem.Equals(SR.GetString(SR.LessThan)))
            {
                filter = "LessThan";
            }
            else if (selecteditem.Equals(SR.GetString(SR.LessThanOrEqualTo)))
            {
                filter = "LessThanOrEqualTo";
            }
            else if (selecteditem.Equals(SR.GetString(SR.GreaterThan)))
            {
                filter = "GreaterThan";
            }
            else if (selecteditem.Equals(SR.GetString(SR.GreaterThanOrEqualTo)))
            {
                filter = "GreaterThanOrEqualTo";
            }
            else if (selecteditem.Equals(SR.GetString(SR.Match)))
            {
                filter = "Match";
            }
            else if (selecteditem.Equals(SR.GetString(SR.Like)))
            {
                filter = "Like";
            }
            else if (selecteditem.Equals("Expression Match..."))
            {
                filter = "em";
            }
            else
                filter = "default";
            return filter;
        }

        private void EvaluateFilterMenuItems()
        {
            GridTableCellStyleInfo style = this.Grid.GetTableViewStyleInfo(filterRowIndex, filterColumnIndex);
            GridTableCellStyleInfo tableCell = (GridTableCellStyleInfo)style;
            string result = this.Model.GetCompareOperatorName(tableCell.TableCellIdentity.Column.Name);
            if (result == string.Empty)
                filterColumnColl.TryGetValue(filterColumnIndex, out result);
            if (result == "Expression Match...")
                result = "em";
            foreach (ToolStripMenuItem selecteditem in menuItems)
            {
                string filter = SelectedString(selecteditem.Text);
                selecteditem.Image = GetBitmap(filter, false);
                if (filter != result)
                    selecteditem.Checked = false;
                else
                {
                    selecteditem.Checked = true;
                    if (this.Grid.TableDescriptor.RecordFilters.Contains(style.TableCellIdentity.Column.Name))
                        selecteditem.Image = GetBitmap(filter, true);
                    else
                    {
                        selecteditem.Image = DynamicFilterBitmaps.GetBitmap(filter);                        
                    }
                }
                if (filter == result)
                    selecteditem.Select();
            }
        }

        private Bitmap GetBitmap(string item, bool iscleared)
        {
            Bitmap bm = null;
            if (iscleared)
            {
                if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black || this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                {
                    bm = DynamicFilterBitmaps.GetBitmap(item + "_white_clear");
                }
                else
                    bm = DynamicFilterBitmaps.GetBitmap(item + "_clear");
            }
            else
            {
                if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black || this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                {

                    bm = DynamicFilterBitmaps.GetBitmap(item + "_white");
                }
                else
                    bm = DynamicFilterBitmaps.GetBitmap(item);
            }
            return bm;
        }

        private void SaveSelectedFilterItem(string clickedItem)
        {
            if (filterColumnColl.ContainsKey(filterColumnIndex))
                filterColumnColl.Remove(filterColumnIndex);
            filterColumnColl.Add(filterColumnIndex, clickedItem);
        }

        private void ApplyFilterValues(string filter)
        {
            if (filter == "em")
                filter = "Expression Match...";
            GridStyleInfo style = this.Grid.Model[this.Grid.TopRowIndex - 1, filterColumnIndex];
            GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)style;
            string value = this.GetFilterBarText(style);
            this.Model.SetLogicalCompareOperator(tableStyleInfo.TableCellIdentity.Column.Name, filter);
            if (value.Length > 0)
            {
                this.Grid.GroupingControl.Table.CurrentRecordManager.NavigateTo(null);
                this.Model.Select(tableStyleInfo.TableCellIdentity, value, -101);
                this.ApplyFilter();
            }
            this.Grid.CurrentCell.Refresh();
        }
            /// <summary>
            /// Is executed when the program starts.
            /// </summary>
            /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)StyleInfo;
            this.tableCellIdentity = tableStyleInfo.TableCellIdentity;
            CurrentCell.Lock();
            this.tableCellIdentity.Table.CurrentRecordManager.NavigateTo(null);
            this.tableCellIdentity.Table.CurrentElement = this.tableCellIdentity.DisplayElement;
            CurrentCell.Unlock();
            base.OnInitialize(rowIndex, colIndex);
            bool exclusive;
            this.Model.FillWithChoices(this.ListControlPart, StyleInfo, out exclusive);
            this.ListControlPart.ShowColumnHeader = false;
            this.ListControlPart.MultiColumn = false;
            this.ListControlPart.Grid.Model.EnableLegacyStyle = this.Grid.Model.EnableLegacyStyle;
            this.ListControlPart.Grid.Properties.DisplayHorzLines = true;
            this.ListControlPart.GridVisualStyles = this.Grid.GetGridVisualStyles();
            this.ListControlPart.Grid.Properties.DisplayHorzLines = true;
        }

        void Grid_CurrentCellControlKeyMessage(object sender, GridCurrentCellControlKeyMessageEventArgs e)
        {
            if (e.Control is GridMaskedEditBox || e.Control is GridCurrencyTextBox)
            {
                e.CallBaseProcessKeyMessage = true;
            }
            else
                e.CallBaseProcessKeyMessage = false;
        }

        #region ListBoxes [Unique Column Value Choise List/ CompareOperator Choise List]

        internal GridListControl InternalListBoxPart
        {
            get
            {
                return this.listBoxPart;
            }
        }

        bool IsComparerListBoxPart
        {
            get
            {
                return this.isComparerListBox;
            }
            set
            {
                if (this.isComparerListBox != value)
                {
                    this.isComparerListBox = value;
                    if (this.isComparerListBox)
                    {
                        this.AttachComparerListBoxPart();
                    }
                    else
                    {
                        this.AttachOriginalListBoxPart();
                    }
                }
                if (!this.isComparerListBox)
                {
                    if (this.ListBoxVersion < 0)
                    {
                        GridTableCellStyleInfo style = this.Grid.GetTableViewStyleInfo(RowIndex, ColIndex);
                        if (style.IsTableCell)
                        {
                            bool exclusive;
                            this.Model.FillWithChoices(this.ListControlPart, style, out exclusive);
                        }
                    }
                    this.EnsureOriginalListBoxPart();
                }
            }
        }

        int ListBoxVersion
        {
            [DebuggerStepThrough()]
            get
            {
                if (this.currColumn.Equals(string.Empty) || !this.currColumn.Equals(this.oldColumn))
                {
                    this.oldColumn = this.currColumn;
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// Called to attach a list box part to this renderer object.
        /// </summary>
        protected void AttachComparerListBoxPart()
        {
            this.EnsureOriginalListBoxPart();
            if (this.DropDownContainer.Controls.Contains(this.InternalListBoxPart))
            {
                this.DropDownContainer.Controls.Remove(this.InternalListBoxPart);
            }

            this.listBoxPart2 = this.Model.CompareOperatorListPart;
            this.DropDownContainer.Controls.Add(this.listBoxPart2);
            this.listBoxPart2.MouseUp += new MouseEventHandler(listBoxPart2_MouseUp);

        }

        void listBoxPart2_MouseUp(object sender, MouseEventArgs e)
        {
            CurrentCell.CloseDropDown(PopupCloseType.Done);
            GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)StyleInfo;
            string value = this.GetFilterBarText(StyleInfo);
            this.Model.SetLogicalCompareOperator(tableStyleInfo.TableCellIdentity.Column.Name, Convert.ToString(listBoxPart2.SelectedItem)); //ListControlPart.SelectedItem));
            if (value.Length > 0)
            {
                this.Grid.GroupingControl.Table.CurrentRecordManager.NavigateTo(null);
                this.Model.Select(tableStyleInfo.TableCellIdentity, value, -101);
                this.ApplyFilter();
            }
            this.Grid.CurrentCell.Refresh();
        }

        /// <summary>
        /// Called to attach a list box part to this renderer object.
        /// </summary>
        protected void AttachOriginalListBoxPart()
        {
            if (!this.DropDownContainer.Controls.Contains(this.InternalListBoxPart))
            {
                this.DropDownContainer.Controls.RemoveAt(0);
                this.ListControlPart.DataSource = this.InternalListBoxPart.DataSource; //DataSource changed
                this.DropDownContainer.Controls.Add(this.InternalListBoxPart);
            }
        }

        private void EnsureOriginalListBoxPart()
        {
            GridListControl lb = (GridListControl)this.DropDownContainer.Controls[0];
            if (lb.Tag == null || lb.Tag.ToString() != "CompareOperatorList")
            {
                this.listBoxPart = lb;
            }
        }

        #endregion

        #region Buttons Related
            /// <summary>
            /// Wires the filter to the grid
            /// </summary>
        /// <param name="cellModel">GridCellModelBase</param>
        protected override void WireModel(GridCellModelBase cellModel)
        {
            this.Grid.CurrentCellMoved += new GridCurrentCellMovedEventHandler(this.Grid_CurrentCellMoved);
            this.Grid.CurrentCellControlKeyMessage += new GridCurrentCellControlKeyMessageEventHandler(Grid_CurrentCellControlKeyMessage);
            this.Grid.CurrentCellAcceptedChanges += new CancelEventHandler(Grid_CurrentCellAcceptedChanges);
            base.WireModel(cellModel);
        }

        void Grid_CurrentCellAcceptedChanges(object sender, CancelEventArgs e)
        {
            if (this.Model.ApplyFilterOnlyOnCellLostFocus && this.Grid.CurrentCell.Renderer != null && this.Grid.CurrentCell.Renderer is GridTableFilterBarExtCellRenderer)
            {
                this.Model.ApplyFilters();
            }
        }

        /// <summary>
        /// Unwire the grid from filter
        /// </summary>
        /// <param name="cellModel">GridCellModelBase</param>
        protected override void UnwireModel(GridCellModelBase cellModel)
        {
            this.Grid.CurrentCellMoved -= new GridCurrentCellMovedEventHandler(this.Grid_CurrentCellMoved);
            this.Grid.CurrentCellControlKeyMessage -= new GridCurrentCellControlKeyMessageEventHandler(Grid_CurrentCellControlKeyMessage);
            this.Grid.CurrentCellAcceptedChanges -= new CancelEventHandler(Grid_CurrentCellAcceptedChanges);
            base.UnwireModel(cellModel);
        }

        void Grid_CurrentCellMoved(object sender, GridCurrentCellMovedEventArgs e)
        {
            if (this.Grid.CurrentCell != null)
            {
                this.Grid.CurrentCell.Refresh();
            }
        }

        /// <summary>
        /// Is triggered when the process key is pressed.
        /// </summary>
        /// <param name="m">Message</param>
        /// <returns>bool</returns>
        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            if (Grid.IsWindowless && !IsDroppedDown && (m.Msg == 258 /*WM_CHAR*/|| m.Msg == 256))
            {
                Keys keyCode = (Keys)((int)m.WParam) & Keys.KeyCode;

                if (keyCode == Keys.Back || keyCode == Keys.Delete)
                {
                    OnKeyDown(new KeyEventArgs(keyCode));
                }
            }
            else if (m.Msg == 257 /*WM_KEYUP*/)
            {
                Keys keyCode = (Keys)((int)m.WParam) & Keys.KeyCode;
                if (keyCode == Keys.Delete)
                {
                    OnKeyPress(new KeyPressEventArgs('\0'));
                }
            }
            return base.ProcessKeyEventArgs(ref m);
        }
        private int filterColumnIndex, filterRowIndex;
            /// <summary>
            /// Is triggered when the button is clicked
            /// </summary>
            /// <param name="rowIndex">int</param>
            /// <param name="colIndex">int</param>
            /// <param name="button">int</param>
        protected override void OnButtonClicked(int rowIndex, int colIndex, int button)
        {
            // Clear Filter
            if (button == 2)
            {
                GridTableCellStyleInfo style = this.Grid.GetTableViewStyleInfo(rowIndex, colIndex);
                this.Model.ResetFilterBar(style.TableCellIdentity);
                this.ControlText = string.Empty;
                this.Model.ApplyFilters();
            }
            else
            {
                this.currColumn = this.GetKey(rowIndex, colIndex).ToString();
                if (button == 0)
                {
                    this.IsComparerListBoxPart = false;
                    base.OnButtonClicked(rowIndex, colIndex, button);
                }
                else if (button == 1)
                {
                    // In order to close the dropdown at normal filterbar row
                    if (this.Grid.CurrentCell.IsDroppedDown)
                        this.Grid.CurrentCell.CloseDropDown(PopupCloseType.Deactivated);

                    filterColumnIndex = colIndex;
                    filterRowIndex = rowIndex;
                    EvaluateFilterMenuItems();
                    Point pt = this.Grid.ViewLayout.RowColToPoint(rowIndex, colIndex, true, GridCellSizeKind.VisibleSize);
                    Point pt2 = this.Grid.PointToScreen(pt);
                    int width = this.Grid.Model.ColWidths[colIndex];
                    int height = this.Grid.Model.RowHeights[rowIndex];
                    Point pt3 = new Point(pt2.X + width, pt2.Y + height);
                    Point pt4 = new Point(Cursor.Position.X, Cursor.Position.Y);
                    if (!Grid.GroupingControl.BrowseOnly)
                        menu.Show(pt4);
                }
               
            }
        }
        
        /// <summary>
        /// Is triggered when the button is drawn in cell
        /// </summary>
        /// <param name="button">GridCellButton</param>
        /// <param name="g">Graphics</param>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="bActive">bool</param>
        /// <param name="style">GridStyleInfo</param>

        protected override void OnDrawCellButton(GridCellButton button, Graphics g, int rowIndex, int colIndex, bool bActive, GridStyleInfo style)
        {
            if (!this.Grid.Model.EnableLegacyStyle)
            {
                Bitmap bm;
                GridTableCellStyleInfo tablestyle = style as GridTableCellStyleInfo;
                if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro || this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black)
                {
                    if (this.Grid.TableDescriptor.RecordFilters.Contains(tablestyle.TableCellIdentity.Column.Name) )
                        bm = DynamicFilterBitmaps.IconPainter.GetBitmap("filtered_metro.png");
                    else
                        bm = DynamicFilterBitmaps.IconPainter.GetBitmap("filter_metro.png");
                }
                else
                {
                    if (this.Grid.TableDescriptor.RecordFilters.Contains(tablestyle.TableCellIdentity.Column.Name) )
                        bm = DynamicFilterBitmaps.IconPainter.GetBitmap("filtered_gray.png");
                    else
                        bm = DynamicFilterBitmaps.IconPainter.GetBitmap("filter_funnel_gr.png");
                }
                Point ptOffset = Point.Empty;

                if (button is Syncfusion.Windows.Forms.Grid.GridCellComboBoxButton)
                {
                    //base.OnDrawCellButton(button, g, rowIndex, colIndex, bActive, style);
                    DynamicFilterBitmaps.IconPainter.PaintIcon(g, button.Bounds, ptOffset, bm, Color.Blue);
                }
                if (!(button is ClearFilterButtonExt) || this.CanShowFilterButtons(rowIndex, colIndex))
                {
                    if (!(button is Syncfusion.Windows.Forms.Grid.GridCellComboBoxButton))
                    {
                        base.OnDrawCellButton(button, g, rowIndex, colIndex, bActive, style);
                    }
                }
            }
            else
            {
                if (!(button is ClearFilterButton) || this.CanShowFilterButtons(rowIndex, colIndex))
                {
                    base.OnDrawCellButton(button, g, rowIndex, colIndex, bActive, style);
                }
            }
        }

        bool CanShowFilterButtons(int rowIndex, int colIndex)
        {
            if (this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
            {
                return true;
            }
            bool mouseHover = false;
            GridRangeInfo key = GridRangeInfo.Cell(rowIndex, colIndex);
            if (this.mouseHoverAtCells.ContainsKey(key))
            {
                mouseHover = (bool)this.mouseHoverAtCells[key];
            }
            if (!mouseHover)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Is triggered when the mouse enters a particular area
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        protected override void OnMouseHoverEnter(int rowIndex, int colIndex)
        {
            GridRangeInfo key = GridRangeInfo.Cell(rowIndex, colIndex);
            if (this.mouseHoverAtCells.ContainsKey(key))
            {
                this.mouseHoverAtCells[key] = true;
            }
            else
            {
                this.mouseHoverAtCells.Add(key, true);
            }
            base.OnMouseHoverEnter(rowIndex, colIndex);
            this.Grid.RefreshRange(key);
        }
            /// <summary>
            /// Is triggered when the mouse leaves a particular area
            /// </summary>
            /// <param name="rowIndex">int</param>
            /// <param name="colIndex">int</param>
            /// <param name="e">EventArgs</param>
        protected override void OnMouseHoverLeave(int rowIndex, int colIndex, EventArgs e)
        {
            GridRangeInfo key = GridRangeInfo.Cell(rowIndex, colIndex);
            if (this.mouseHoverAtCells.ContainsKey(key))
            {
                this.mouseHoverAtCells[key] = false;
            }
            else
            {
                this.mouseHoverAtCells.Add(key, false);
            }
            base.OnMouseHoverLeave(rowIndex, colIndex, e);
            this.Grid.RefreshRange(key);
        }

        #endregion

        #region Key Overrides
        /// <summary>
        /// Is triggered when the key is pressed down
        /// </summary>
        /// <param name="e">KeyPressEventArgs</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            GridListControl listBoxPart = this.ListControlPart;
            GridDropDownGridListControlCellModel model = this.Model as GridDropDownGridListControlCellModel;

            if (!e.Handled)
            {
                GridTableCellStyleInfo style = this.Grid.GetTableViewStyleInfo(RowIndex, ColIndex);
                if (!this.HasFocusControl)
                {
                    //// Key pressed for the first time.
                    if (!Char.IsControl(e.KeyChar) && !IsReadOnly())
                    {
                        if (listBoxPart.Items == null)
                        {
                            object ds = model.GetDataSource(this.StyleInfo);
                            if (ds == null)
                            {
                                base.OnKeyPress(e);
                                return;
                            }
                            listBoxPart.DataSource = ds;
                        }

                        //// Combo box mode with editing.
                        CurrentCell.BeginEdit();
                        SetTextBoxText(e.KeyChar.ToString(), true);
                        this.Model.Select(style.TableCellIdentity, e.KeyChar.ToString(), -101);

                        if (this.IsEditing)
                        {
                            this.TextBoxControl.Select(1, Math.Max(0, this.TextBoxControl.Text.Length - 1));
                        }
                        this.ApplyFilter();
                        e.Handled = true;
                    }
                }
                else
                {
                    //// Key pressed after cell has been switched into edit mode.
                    char charCode = (char)(e.KeyChar & 255);

                    //// Pass the e.KeyChar to the IsControl method, not the charCode.
                    if ((!Char.IsControl(e.KeyChar) || charCode == 8 || charCode == 22 /*Paste*/|| charCode == 0 /*Delete*/) && !IsReadOnly())
                    {
                        if (listBoxPart.Items == null)
                        {
                            object ds = model.GetDataSource(this.StyleInfo);
                            if (ds == null)
                            {
                                base.OnKeyPress(e);
                                return;
                            }
                            listBoxPart.DataSource = ds;
                        }
                        int prevSelectionStart = TextBox.SelectionStart;
                        string newText = TextBoxText;
                        newText = TextBoxText.Remove(TextBox.SelectionStart, TextBox.SelectionLength);
                        string s = newText.Insert(TextBox.SelectionStart, (!Char.IsControl(e.KeyChar) ? e.KeyChar.ToString() : string.Empty));
                        if (this.ValidateString(s))
                        {
                            if (this.NotifyCurrentCellChanging())
                            {
                                CurrentCell.IsModified = true;
                                SetTextBoxText(s, true);

                                if (charCode != 8 /*backSpace */ && charCode != 22 /*Paste*/ && charCode != 0 /*Delete*/)
                                    TextBox.SelectionStart = prevSelectionStart + 1;
                                this.Model.Select(style.TableCellIdentity, s, -101);
                                if (!this.Model.ApplyFilterOnlyOnCellLostFocus)
                                {
                                    this.ApplyFilter();
                                }
                            }
                        }
                        e.Handled = true;
                    }
                }
            }
        }
        /// <summary>
        /// Is triggered when the key is released in keyboard.
        /// </summary>
        /// <param name="e">KeyEventArgs</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if ((this.Model.ApplyFilterOnlyOnCellLostFocus) && (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Up || e.KeyCode == Keys.Escape))
            {
                this.Model.ApplyFilters();
            }
            base.OnKeyUp(e);
        }

        /// <summary>
        /// Is triggered when the key is pressed in keyboard.
        /// </summary>
        /// <param name="e">KeyEventArgs</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            object savedEditState = this.GetEditState();
            bool ignoreWmChar = true;
            if (!e.Handled)
            {
                switch (e.KeyCode)
                {
                    case Keys.Back:
                    case Keys.Delete:
                        if (!CurrentCell.HasControlFocus || !CurrentCell.IsEditing)
                        {
                            if (Control.ModifierKeys != Keys.Alt)
                            {
                                if (this.Grid.ShouldDeleteKeyClearCurrentCellContentsOnly())
                                {
                                    //// cell is not readonly and OnDeleteCell notification returns true
                                    if (!IsReadOnly() && OnDeleting() && this.Grid.RaiseCurrentCellDeleting())
                                    {
                                        //// Delete text, SetTextBoxText call BeginEdit()
                                        SetTextBoxText(string.Empty, true);
                                        CurrentCell.IsModified = true;
                                        CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                    }
                                    e.Handled = true;
                                }
                            }
                        }
                        else if (Grid.IsWindowless || TextBox is TextBox)  //// TextBox is "Original TextBox", not RichTextbox ...
                        {
                            if (TextBox.SelectionLength > 0)
                            {
                                TextBox.SelectedText = string.Empty;
                            }
                            else if (TextBox.SelectionStart >= 0)
                            {
                                if (e.KeyCode == Keys.Back)
                                {
                                    if (TextBox.SelectionStart > 0)
                                    {
                                        TextBox.Select(TextBox.SelectionStart - 1, 1);
                                    }
                                }
                                else if (e.KeyCode == Keys.Delete)
                                {
                                    TextBox.Select(TextBox.SelectionStart, 1);
                                }
                                else
                                {
                                    TextBox.Select(TextBox.SelectionStart, 1);
                                }
                                TextBox.SelectedText = string.Empty;
                            }
                            e.Handled = true;
                            ignoreWmChar = TextBox is GridOriginalTextBoxControl && TextBox.SelectionLength == 0;
                        }

                        break;

                    case Keys.Escape:
                        if (CurrentCell.IsDroppedDown)
                        {
                            CurrentCell.CloseDropDown(PopupCloseType.Canceled);
                            e.Handled = true;
                        }
                        else if (CurrentCell.IsModified)
                        {
                            CurrentCell.RejectChanges();
                            CurrentCell.CancelEdit();
                            CurrentCell.Refresh();
                            CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                            e.Handled = true;
                        }
                        else
                        {
                            CurrentCell.CancelEdit();
                        }
                        if ((this.Grid.Model.Options.ActivateCurrentCellBehavior & Syncfusion.Windows.Forms.Grid.GridCellActivateAction.SetCurrent) != 0)
                        {
                            CurrentCell.BeginEdit();
                        }

                        return;

                    case Keys.End:
                        if (Control.ModifierKeys == Keys.None && !CurrentCell.HasControlFocus && !this.DisableTextBox)
                        {
                            if (CurrentCell.BeginEdit())
                            {
                                // Position caret.
                                CurrentCell.HasControlFocus = true;
                                TextBox.SelectionStart = TextBox.Text.Length;
                                TextBox.SelectionLength = 0;
                                CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                e.Handled = true;
                            }
                        }

                        break;

                    case Keys.Home:
                        if (Control.ModifierKeys == Keys.None && !CurrentCell.HasControlFocus && !this.DisableTextBox)
                        {
                            if (CurrentCell.BeginEdit())
                            {
                                // Position caret.
                                CurrentCell.HasControlFocus = true;
                                TextBox.SelectionStart = 0;
                                TextBox.SelectionLength = 0;
                                CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                this.Grid.Update();
                                e.Handled = true;
                            }
                        }

                        break;

                    case Keys.Enter:
                        if (Control.ModifierKeys == Keys.None && CurrentCell.IsEditing && this.StyleInfo.AllowEnter && this.NotifyCurrentCellChanging())
                        {
                            SetSelectedText("\n", true);
                            CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                            e.Handled = true;
                        }

                        break;

                    case Keys.Tab:
                        if (this.Grid.WantTabKey)
                        {
                            ignoreWmChar = true;
                        }

                        break;
                }
            }
            // base.OnKeyDown(e);
        }
        #endregion

        #region Filter Related

        /// <summary>
        /// A method to set default compare operator image in FilterButton explicitly.
        /// </summary>
        /// <param name="key">Unique ColumnGroupId.</param>
        /// <param name="value">Logical operator to be used for comparison.</param>
        public void SetLogicalCompareOperatorImage(object key, string value)
        {
            if (this.Model.CompareOperatorListPart.Items.Contains(value))
                this.Model.SetLogicalCompareOperator(key, value);
            else
            {
                if (this.Model.CompareOperatorListPart.Items.Count > 0)
                {
                    this.Model.SetLogicalCompareOperator(key, this.Model.CompareOperatorListPart.Items[0].ToString());
                }
            }
        }

        void ApplyFilter()
        {
            this.Grid.CurrentCell.Lock();
            this.Model.ApplyFilters();
            this.Grid.CurrentCell.Unlock();
        }

        object GetKey()
        {
            return this.GetKey(RowIndex, ColIndex);
        }
        object GetKey(int rowIndex, int colIndex)
        {
            GridTableCellStyleInfo style = this.Grid.GetTableViewStyleInfo(colIndex, colIndex);
            if (!style.IsTableCell || style.TableCellIdentity.Column == null)
            {
                return string.Empty;
            }
            return style.TableCellIdentity.Column.Name;
        }

        /// <summary>
        /// Determines the text from record filter criteria that should be displayed in filterbar cell.
        /// </summary>
        /// <param name="style">Cell style information.</param>
        /// <returns>Filter bar text.</returns>
        public string GetFilterBarText(GridStyleInfo style)
        {
            GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)style;
            GridTableCellStyleInfoIdentity tableCellIdentity = tableStyleInfo.TableCellIdentity;
            GridTableDescriptor td = tableCellIdentity.Table.TableDescriptor;
            string filterName = this.Model.GetUniqueColumnGroupId(tableCellIdentity);
            RecordFilterDescriptor filter = this.Model.GetRecordFilter(td.RecordFilters, tableCellIdentity, filterName);
            object value = this.Model.SelectCustomText;
            if (filter == null)
            {
                value = this.Model.SelectAllText;
            }
            else if (filter.Conditions.Count == 1)
            {
                if (this.Model.GetCompareOperatorName(filterName).Equals(string.Empty))
                {
                    bool set = false;
                    if (filter.Conditions[0].CompareOperator == FilterCompareOperator.Like)
                    {
                        string compareValue = filter.Conditions[0].CompareValue.ToString();
                        if (compareValue.StartsWith("*"))
                        {
                            this.Model.SetLogicalCompareOperator(filterName, SR.EndsWith);
                            if (CurrentCell.HasCurrentCell && CurrentCell.HasCurrentCellAt(RowIndex, ColIndex))
                            {
                                value = this.TextBox.Text = compareValue.Substring(1);
                            }
                            else
                            {
                                value = compareValue.Substring(1);
                            }
                            set = true;
                        }
                        else if (compareValue.EndsWith("*"))
                        {
                            this.Model.SetLogicalCompareOperator(filterName, SR.StartsWith);
                            if (CurrentCell.HasCurrentCell && CurrentCell.HasCurrentCellAt(RowIndex, ColIndex))
                            {
                                value = this.TextBox.Text = compareValue.Remove(compareValue.LastIndexOf("*"));
                            }
                            else
                            {
                                value = compareValue.Remove(compareValue.LastIndexOf("*"));
                            }
                            set = true;
                        }
                    }
                    if (!set)
                    {
                        this.Model.SetLogicalCompareOperator(filterName, filter.Conditions[0].CompareOperator.ToString());
                        value = filter.Conditions[0].CompareValue;
                    }
                }
                else if (filter.Conditions[0].CompareOperator == FilterCompareOperator.Equals)
                {
                    value = filter.Conditions[0].CompareValue;
                    if (filter.Conditions[0].CompareText == "(null)" || filter.Conditions[0].CompareText == string.Empty)
                        value = this.Model.SelectEmptyText;
                }
                else
                {
                    value = filter.Conditions[0].CompareValue;
                    if (filter.Conditions[0].CompareText == "(null)" || filter.Conditions[0].CompareText == string.Empty)
                        value = this.Model.SelectEmptyText;
                }
            }
            else if (this.Model.IsExpressionFilter(tableStyleInfo))
            {
                value = filter.Expression;
            }
            else if (this.Model.GetCompareOperatorName(filterName).Equals(string.Empty) &&
                !filter.Expression.Equals(string.Empty))
            {
                this.Model.SetLogicalCompareOperator(filterName, "Expression Match...");
                this.Model.CompareOperatorListPart.Update();
                value = filter.Expression;
            }
            return this.Model.ExcludeFilterFormat(value, tableStyleInfo.TableCellIdentity.Column.Name);
        }

        /// <summary>
        /// Occurs when <see cref="OnPrepareViewStyleInfo"/> event is called for this cell.
        /// </summary>
        /// <param name="e">Event args.</param>
        public override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            GridTableCellStyleInfo styleInfo = ((GridTableCellStyleInfo)e.Style).TableCellIdentity.Column.Appearance.AnyRecordFieldCell;
            if (styleInfo.CellType == GridCellTypeName.ComboBox)
                e.Style.CellValue = this.Model.GetDisplayMember(styleInfo, this.GetFilterBarText(e.Style));
            e.Style.DropDownStyle = GridDropDownStyle.Editable;
            e.Style.ExclusiveChoiceList = false;
            e.Style.ShowButtons = GridShowButtons.Show;
            e.Style.Clickable = true;
            e.Style.WrapText = false;
            if ((e.Style.Text == this.Model.SelectAllText || e.Style.Text == this.Model.SelectCustomText || e.Style.Text == this.Model.SelectEmptyText))
            {
                e.Style.Text = string.Empty;
            }
            base.OnPrepareViewStyleInfo(e);
        }
            /// <summary>
            /// Is triggered for the drawing of the cells
            /// </summary>
            /// <param name="g">Graphics</param>
            /// <param name="clientRectangle">Rectangle</param>
            /// <param name="rowIndex">int</param>
            /// <param name="colIndex">int</param>
            /// <param name="style">GridStyleInfo</param>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            if (style.Text == string.Empty)
            {
               GridTableCellStyleInfo styleInfo = ((GridTableCellStyleInfo)style).TableCellIdentity.Column.Appearance.AnyRecordFieldCell;
            if (styleInfo.CellType == GridCellTypeName.ComboBox)
                style.CellValue = this.Model.GetDisplayMember(styleInfo, this.GetFilterBarText(style));
            else
                style.CellValue = this.GetFilterBarText(style);
            }
            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
        }

        #endregion

        #region DropDown Overrides

        /// <summary>
        /// Occurs when the filter drop down is being shown.
        /// </summary>
        /// <param name="sender">Cell renderer.</param>
        /// <param name="e">Event args.</param>
        public override void DropDownContainerShowingDropDown(object sender, CancelEventArgs e)
        {
            if (!IsComparerListBoxPart)
            {
                //GridListControl listBoxPart = (GridListControl)InternalListBoxPart;
                if (!this.DisableTextBox && this.IsControlVisible())
                {
                    listBoxPart.BackColor = this.TextBoxControl.BackColor;
                    listBoxPart.ForeColor = this.TextBoxControl.ForeColor;
                    listBoxPart.Font = this.TextBoxControl.Font;
                    listBoxPart.RightToLeft = this.Grid.RightToLeft;
                }
                else
                {
                    listBoxPart.BackColor = Color.FromArgb(255, StyleInfo.Interior.BackColor);
                    listBoxPart.ForeColor = StyleInfo.TextColor;
                    listBoxPart.Font = StyleInfo.GdipFont;
                    listBoxPart.RightToLeft = this.Grid.RightToLeft;
                }
                this.listBoxPart.Grid.HScrollBehavior = GridScrollbarMode.DisableAutoScroll;
                Size size = this.tableCellIdentity.Table.TableOptions.MaxFilterBarChoiceListSize;
                size.Width = listBoxPart.Grid.GetColWidth(ColIndex);
                GridCurrentCellShowingDropDownEventArgs ce = new GridCurrentCellShowingDropDownEventArgs(size);
                this.Grid.RaiseCurrentCellShowingDropDown(ce);
                if (ce.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                if (this.ListControlPart.Items.Count == 0)
                {
                    e.Cancel = true;
                    return;
                }
                listBoxPart.Text = TextBoxText;
                this.listBoxPart.Size = ce.Size;
                this.DropDownContainer.Size = this.listBoxPart.Size;
            }
            else if (IsComparerListBoxPart)
            {
                if (!this.DisableTextBox && this.IsControlVisible())
                {
                    listBoxPart2.BackColor = this.TextBoxControl.BackColor;
                    listBoxPart2.ForeColor = this.TextBoxControl.ForeColor;
                    listBoxPart2.Font = this.TextBoxControl.Font;
                    listBoxPart2.RightToLeft = this.Grid.RightToLeft;
                }
                else
                {
                    listBoxPart2.BackColor = Color.FromArgb(255, StyleInfo.Interior.BackColor);
                    listBoxPart2.ForeColor = StyleInfo.TextColor;
                    listBoxPart2.Font = StyleInfo.GdipFont;
                    listBoxPart2.RightToLeft = this.Grid.RightToLeft;
                }
                Size size = this.tableCellIdentity.Table.TableOptions.MaxFilterBarChoiceListSize;
                size.Width = this.Grid.GetColWidth(ColIndex);
                GridCurrentCellShowingDropDownEventArgs ce = new GridCurrentCellShowingDropDownEventArgs(size);
                this.Grid.RaiseCurrentCellShowingDropDown(ce);
                if (ce.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                if (this.ListControlPart.Items.Count == 0)
                {
                    e.Cancel = true;
                    return;
                }
                ((GridComboBoxListBoxPart)listBoxPart2).DropDownRows=ce.Size.Height / listBoxPart2.ItemHeight;
                listBoxPart2.Size = ce.Size;
                listBoxPart2.Text = TextBoxText;
                this.DropDownContainer.Size = listBoxPart2.Size;
            }
        }
        /// <summary>
        /// Is triggered when the dropdown is clicked.
        /// </summary>
        protected override void OnShowDropDown()
        {
            if (!this.IsComparerListBoxPart)
            {
                base.OnShowDropDown();
                return;
            }
            if (IsDroppedDown)
            {
                return;
            }
            this.Grid.CurrentCell.BeginEdit();
            this.Grid.Update();
            if (this.DropDownContainer != null)
            {
                this.DropDownContainer.PopupParent = this.Grid.DropDownContainerParent;
                PopupRelativeAlignment popupAlign;
                this.DropDownContainer.ShowPopup(this.DropDownPart.GetLocationForPopupAlignment(PopupRelativeAlignment.BottomLeft, out popupAlign));
            }
        }
        /// <summary>
        /// Is called when the mouseup event is fired in list control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void ListControlMouseUp(object sender, MouseEventArgs e)
        {
            CurrentCell.CloseDropDown(PopupCloseType.Done);
            GridColumnDescriptor column = null;
            GridGroupingControl gc = Grid.GroupingControl;
            GridCurrentCell cc = gc.TableControl.CurrentCell;
            if (cc.Renderer is GridNestedTableControlCellRenderer)
            {
                GridNestedTableControlCellRenderer rend = cc.Renderer as GridNestedTableControlCellRenderer;
                column = rend.Control.TableDescriptor.Columns[rend.Control.TableDescriptor.ColIndexToField(this.ColIndex)];
            }
            else
            {
                column = gc.TableDescriptor.Columns[gc.TableDescriptor.ColIndexToField(this.ColIndex)];
            }
            GridListControl listBoxPart = (GridListControl)this.ListControlPart;
            ListBox listBoxPart2 = (ListBox)this.listBoxPart2;
            if (this.listBoxPart.SelectedIndex >= 0)
            {
                FilterBarSelectedItemChangingEventArgs fce = new FilterBarSelectedItemChangingEventArgs(this.listBoxPart, column, this.ListControlPart.SelectedIndex, this.ListControlPart.SelectedItem.ToString());
                gc.OnFilterBarSelectedItemChanging(fce);
                if (!fce.Cancel)
                {
                    if (fce.SelectedIndex >= 0 && this.ListControlPart.Items.Count > fce.SelectedIndex)
                    {
                        if (this.ListControlPart.SelectedIndex != fce.SelectedIndex)
                        {
                            this.ListControlPart.SelectedIndex = fce.SelectedIndex;
                        }
                        else
                        {
                            if (!this.ListControlPart.SelectedItem.Equals(fce.SelectedText) && this.ListControlPart.Items.Contains(fce.SelectedText))
                            {
                                this.ListControlPart.SelectedItem = fce.SelectedText;
                            }
                        }
                    }
                    int selectedIndex = this.ListControlPart.SelectedIndex;
                    this.Model.Select(tableCellIdentity, selectedIndex);
                    // Reset current element before applying filter.
                    this.Grid.GroupingControl.Table.CurrentRecordManager.NavigateTo(null);
                    // Apply filter.
                    this.Model.ApplyFilters();

                    if (this.ListControlPart.SelectedIndex < 0)
                    {
                        this.ListControlPart.SelectedIndex = selectedIndex;
                    }
                    FilterBarSelectedItemChangedEventArgs fe = new FilterBarSelectedItemChangedEventArgs(column, this.ListControlPart.SelectedIndex, this.ListControlPart.SelectedItem.ToString());
                    gc.OnFilterBarSelectedItemChanged(fe);
                }
            }

            if (this.listBoxPart2 != null && this.listBoxPart2.SelectedIndex >= 0)
            {
                FilterBarSelectedItemChangingEventArgs fce = new FilterBarSelectedItemChangingEventArgs(this.listBoxPart2, column, this.ListControlPart.SelectedIndex, this.ListControlPart.SelectedItem.ToString());
                gc.OnFilterBarSelectedItemChanging(fce);
                if (!fce.Cancel)
                {
                    if (fce.SelectedIndex >= 0 && this.ListControlPart.Items.Count > fce.SelectedIndex)
                    {
                        if (this.ListControlPart.SelectedIndex != fce.SelectedIndex)
                        {
                            this.ListControlPart.SelectedIndex = fce.SelectedIndex;
                        }
                        else
                        {
                            if (!this.ListControlPart.SelectedItem.Equals(fce.SelectedText) && this.ListControlPart.Items.Contains(fce.SelectedText))
                            {
                                this.ListControlPart.SelectedItem = fce.SelectedText;
                            }
                        }
                    }
                    int selectedIndex = this.ListControlPart.SelectedIndex;
                    this.Model.Select(tableCellIdentity, selectedIndex);
                    // Reset current element before applying filter.
                    this.Grid.GroupingControl.Table.CurrentRecordManager.NavigateTo(null);
                    // Apply filter.
                    this.Model.ApplyFilters();
                    if (this.ListControlPart.SelectedIndex < 0)
                    {
                        this.ListControlPart.SelectedIndex = selectedIndex;
                    }
                    FilterBarSelectedItemChangedEventArgs fe = new FilterBarSelectedItemChangedEventArgs(column, this.ListControlPart.SelectedIndex, this.ListControlPart.SelectedItem.ToString());
                    gc.OnFilterBarSelectedItemChanged(fe);
                }
            }
            GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)StyleInfo;
            GridTableCellStyleInfoIdentity tableCellIdentity2 = tableStyleInfo.TableCellIdentity;

            //If this filterbar belongs to a child group or nested table then there is a good chance
            //the current row is no FilterBar anymore. Best is to reset the current cell.
            if (tableCellIdentity2 == null || tableCellIdentity2.DisplayElement != tableCellIdentity.DisplayElement)
                CurrentCell.ResetCurrentCellWithoutDeactivate();
            else
                ControlValue = GetFilterBarText(StyleInfo);// don't call base class - ignore.
        }
            /// <summary>
            /// Applies Prepeareviewstyleinfo event to ListControl
            /// </summary>
            /// <param name="sender">object</param>
            /// <param name="e">GridPrepareViewStyleInfoEventArgs</param>
        protected override void ListControlGridPrepareViewStyleInfo(object sender, GridPrepareViewStyleInfoEventArgs e)
        {
            if (this.Model.isCombobox)
                e.Style.Borders.Bottom = GridBorder.Empty;
            base.ListControlGridPrepareViewStyleInfo(sender, e);
        }

        #endregion

        /// <summary>
        /// Gets a reference to the cell model.
        /// </summary>
        public new GridListFilterBarCellModel Model
        {
            get
            {
                return (GridListFilterBarCellModel)base.Model;
            }
        }

        /// <summary>
        /// Gets a reference to the parent grid.
        /// </summary>
        public new GridTableControl Grid
        {
            get
            {
                return (GridTableControl)base.Grid;
            }
        }
    }
    #endregion

    #region ContextMenuRenderer
    /// <summary>
    /// class that Renderers the context meu
    /// </summary>
        public class ContextMenuRenderer : ToolStripProfessionalRenderer
        {
            GridControlBase Grid;
            GridListFilterBarCellRenderer Render;
            /// <summary>
            /// Constructor of ContextMenuRenderer
            /// </summary>
            /// <param name="grid">GridControlBase</param>
            /// <param name="render">GridListFilterBarCellRenderer</param>
            public ContextMenuRenderer(GridControlBase grid, GridListFilterBarCellRenderer render)
            {
                Grid = grid;
                Render = render;
            }
            /// <summary>
            /// Is triggered when the margin for the image is rendered.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
            {
                Rectangle marginRect = e.AffectedBounds;
                SolidBrush backBrush;
                switch (this.Grid.Model.Options.GridVisualStyles)
                {
                    case GridVisualStyles.Office2007Black:
                    case GridVisualStyles.Office2010Silver:
                        backBrush = new SolidBrush(Color.FromArgb(227, 227, 227));
                        break;
                    case GridVisualStyles.Office2007Blue:
                        backBrush = new SolidBrush(Color.FromArgb(213, 229, 251));
                        break;
                    case GridVisualStyles.Office2007Silver:
                        backBrush = new SolidBrush(Color.FromArgb(242, 245, 249));
                        break;
                    case GridVisualStyles.Office2010Black:
                        backBrush = new SolidBrush(Color.FromArgb(120, 120, 120));
                        break;
                    case GridVisualStyles.Office2010Blue:
                        backBrush = new SolidBrush(Color.FromArgb(215, 231, 251));
                        break;
                    case GridVisualStyles.Metro:
                        backBrush = new SolidBrush(Color.FromArgb(94, 171, 222)); //Color.FromArgb(42, 191, 241));
                        break;
                    default:
                        backBrush = new SolidBrush(SystemColors.Control);
                        break;
                }

                using (backBrush)
                    e.Graphics.FillRectangle(backBrush, marginRect);
            }
            /// <summary>
            /// Renders the image with different attributes
            /// </summary>
            /// <param name="e"></param>
            protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
            {
                Bitmap bmp;
                if (Render.Grid.Table.GetFilteredRecordCount() >= Render.Grid.Table.Records.Count)
                {
                    string filter = SelectedString(e.Item.Text);
                    if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black || this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                    {
                        bmp = DynamicFilterBitmaps.GetBitmap(filter + "_white");
                        e.Item.Image = bmp;
                    }
                    else
                    {
                        bmp = DynamicFilterBitmaps.GetBitmap(filter);
                        e.Item.Image = bmp;
                    }
                }
                base.OnRenderItemImage(e);
            }

            private string SelectedString(string selecteditem)
            {
                string filter = string.Empty;
                if (selecteditem.Equals(SR.GetString(SR.StartsWith)))
                {
                    filter = "StartsWith";
                }
                else if (selecteditem.Equals(SR.GetString(SR.EndsWith)))
                {
                    filter = "EndsWith";
                }
                else if (selecteditem.Equals(SR.GetString(SR.Equal)))
                {
                    filter = "Equals";
                }
                else if (selecteditem.Equals(SR.GetString(SR.NotEquals)))
                {
                    filter = "NotEquals";
                }
                else if (selecteditem.Equals(SR.GetString(SR.LessThan)))
                {
                    filter = "LessThan";
                }
                else if (selecteditem.Equals(SR.GetString(SR.LessThanOrEqualTo)))
                {
                    filter = "LessThanOrEqualTo";
                }
                else if (selecteditem.Equals(SR.GetString(SR.GreaterThan)))
                {
                    filter = "GreaterThan";
                }
                else if (selecteditem.Equals(SR.GetString(SR.GreaterThanOrEqualTo)))
                {
                    filter = "GreaterThanOrEqualTo";
                }
                else if (selecteditem.Equals(SR.GetString(SR.Match)))
                {
                    filter = "Match";
                }
                else if (selecteditem.Equals(SR.GetString(SR.Like)))
                {
                    filter = "Like";
                }
                else if (selecteditem.Equals("Expression Match..."))
                {
                    filter = "em";
                }
                else
                    filter = "Default";
                return filter;
            }

        }
        #endregion
    
}
