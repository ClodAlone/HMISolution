//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableFilterBarCell.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Text;

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;
using Syncfusion.Collections.BinaryTree;

#if ASPNET
using Syncfusion.Windows.Forms.Grid;
using System.Web.UI.WebControls;
using Syncfusion.Web.UI.WebControls.Grid.Grouping.Localization;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
using Syncfusion.Windows.Forms.Grid.Grouping.Localization;
using System.Data;
using Syncfusion.Drawing;
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// Implements the DataModel part for a TableFilterBar cell.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridTableFilterBarCellModel"/> can serve as model for several <see cref="GridTableFilterBarCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridTableFilterBarCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridTableFilterBarCellModel : GridComboBoxCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridTableFilterBarCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridTableFilterBarCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridTableFilterBarCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = false;
            ButtonBarSize = new Size(SystemInformation.VerticalScrollBarWidth, 0);
            SupportsChoiceList = true;
        }

        /// <summary>
        /// Initializes a new <see cref="GridTableFilterBarCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridTableFilterBarCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }

        static GridTableFilterBarCellModel()
        {
            GridTableFilterBarCellModel.selectAllText = SR.GetString(SR.FilterBarAll);
            GridTableFilterBarCellModel.selectCustomText = SR.GetString(SR.FilterBarCustom);
            GridTableFilterBarCellModel.selectEmptyText = SR.GetString(SR.FilterBarEmpty);
        }
#if ASPNET
#else
        /// <summary>
        /// Creates a <see cref="GridTableFilterBarCellRenderer"/> for this cell model that is specific to the GridControlBase.
        /// </summary>
        /// <param name="control">The <see cref="GridControlBase"/> the cell renderer is created for.</param>
        /// <returns>A new <see cref="GridTableFilterBarCellRenderer"/> specific for a <see cref="GridControlBase"/>.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridTableFilterBarCellRenderer(control, this);
        }
#endif
        /// <summary>
        /// Returns the unique choices to be displayed in the filterbar cell.
        /// </summary>
        /// <param name="tableCellIdentity">Identity for the table cell.</param>
        /// <returns>Filter bar choices.</returns>
        public virtual object[] GetFilterBarChoices(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            if (tableCellIdentity.FilterBarSummaryDescriptor != null)
            {
                ITreeTableSummary[] summaries = tableCellIdentity.DisplayElement.ParentGroup.GetSummaries(tableCellIdentity.Table);
                int summaryIndex = tableCellIdentity.Table.TableDescriptor.Summaries.IndexOf(tableCellIdentity.FilterBarSummaryDescriptor);
                FilterBarChoicesSummary filterBarChoicesSummary = (FilterBarChoicesSummary) summaries[summaryIndex];
                return filterBarChoicesSummary.Values;
            }
            GridGroupingControl groupGrid=((GridEngine)tableCellIdentity.Table.Engine).GroupingControl;
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
#if ASPNET
#else
            GridGroupingControl gc = tableCellIdentity.Table.Engine.ParentControl;
            GridQueryFilterBarChoicesEventArgs qe = new GridQueryFilterBarChoicesEventArgs(tableCellIdentity.Column, false, tableCellIdentity.DisplayElement);
            gc.OnQueryFilterBarChoices(qe);
            if (!qe.Cancel)
            {
                result = qe.UniqueFilterBarValues;
            }
#endif

            return result;
        }

        /// <summary>
        /// Initializes a listbox with data binding information from a <see cref="GridStyleInfo"/>
        /// object.
        /// </summary>
        /// <param name="listBox">The list box to be initialized with data binding information.</param>
        /// <param name="style">The style object with binding information.</param>
        /// <param name="exclusive">A place holder that indicates whether the list box is filled with an exclusive
        /// list of possible choices or if non-standard values are allowed.
        /// </param>
#if ASPNET
        public void FillWithChoices(DropDownList listBox, GridStyleInfo style, out bool exclusive)
#else
        public override void FillWithChoices(ListBox listBox, GridStyleInfo style, out bool exclusive)
#endif
        {
            exclusive = true;
            GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo) style;

            object[] items = (object[]) this.GetFilterBarChoices(tableStyleInfo.TableCellIdentity);

            listBox.Items.Clear();

            if (items != null)
            {
                listBox.Items.Add(this.SelectAllText);
#if ASPNET
#else
                listBox.Items.Add(this.SelectCustomText);
#endif
                foreach (object item in items)
                {
                    if (item is DBNull || item == string.Empty)
                    {
                        listBox.Items.Add(this.SelectEmptyText);
                    }
                    else if (item != null)
                    {
                        listBox.Items.Add(style.GetFormattedText(item));
                    }
                }

#if ASPNET
                if(items.Length == 0)
                    listBox.Items.Add(this.SelectEmptyText);
#endif
            }
        }

        internal static string selectAllText;
        internal static string selectCustomText;
        internal static string selectEmptyText;

        /// <summary>
        /// Default: (All)
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
        /// Default: (Custom...)
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
        /// Default: (Empty)
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
        /// Applies a filter criteria. Note: The first two entries are reserved for (All) and (Custom).
        /// An index greater than one represents a valid choice found with GetFilterBarChoices.
        /// </summary>
        /// <param name="tableCellIdentity">Identity for the table cell.</param>
        /// <param name="index">Index of the filter bar choice to be selected.</param>
        public void Select(GridTableCellStyleInfoIdentity tableCellIdentity, int index)
        {
            if (index >= 0)
            {
                if (index == 0)
                {
                    this.ResetFilterBar(tableCellIdentity);
                }
                else if (index == 1)
                {
                    // Custom
                    this.SelectCustomFilterBar(tableCellIdentity);
                }
                else
                {
                    this.SelectItem(tableCellIdentity, index-2);
                }
            }
        }

        /// <summary>
        /// Internal only.
        /// </summary>
        /// <param name="tableCellIdentity">The table cell identity.</param>
        /// <returns>returns the UniqueColumnGroupId</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
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
        /// Internal only.
        /// </summary>
        /// <param name="tableCellIdentity">The table cell identity.</param>
        /// <returns>returns the UniqueGroupId</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public object[] GetUniqueGroupId(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            Group g = tableCellIdentity.DisplayElement.ParentGroup;
            return g.UniqueGroupId;
        }

        /// <internalonly/>
        /// <summary>
        /// Gets array of RecordFilterDescriptors for the specific column
        /// with respect to the particular GroupUniqueID
        /// </summary>
        /// <param name="recordFilters">Record filters collection.</param>
        /// <param name="tableCellIdentity">Specified grid table cell.</param>
        /// <param name="filterName">Name of the filter.</param>
        /// <returns>Array of RecordFilterDescriptors.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        public RecordFilterDescriptor[] GetRecordFilters(RecordFilterDescriptorCollection recordFilters, GridTableCellStyleInfoIdentity tableCellIdentity, string filterName)
        {
            GridTableDescriptor td = tableCellIdentity.Table.TableDescriptor;
            GridRelationDescriptor rd = td.ParentRelation;
            RecordFilterDescriptor[] rfdc = recordFilters.GetRecordFilters(tableCellIdentity.Column.MappingName);
            if (rfdc != null)
            {
                object[] uniqueId = tableCellIdentity.DisplayElement.ParentGroup.UniqueGroupId;
                ArrayList rfdList = new ArrayList();
                foreach (RecordFilterDescriptor rfd in rfdc)
                {
                    if (rfd.CompareUniqueId(uniqueId))
                    {
                        rfdList.Add(rfd);
                    }
                }

                if (rfdList.Count > 0)
                {
                    return (RecordFilterDescriptor[])rfdList.ToArray(typeof(RecordFilterDescriptor));
                }
            }

            return null;
        }

        RecordFilterDescriptorCollection recordFilters;
        TableDescriptor tableDescriptor;
        Syncfusion.Grouping.Table table;

        /// <internalonly/>
        /// <summary>
        /// Selects the item at the selected index in respective cell identifier.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void SelectItem(GridTableCellStyleInfoIdentity tableCellIdentity, int index)
        {
            string filterName = this.GetUniqueColumnGroupId(tableCellIdentity);

            this.tableDescriptor = tableCellIdentity.Table.TableDescriptor;
            this.table = tableCellIdentity.Table;
            this.recordFilters = new RecordFilterDescriptorCollection();
            this.recordFilters.InitializeFrom(tableDescriptor.RecordFilters);

            object[] items = this.GetFilterBarChoices(tableCellIdentity);

            ////check to see if list was shortened after index was set
            if (index >= items.GetLength(0))
            {
                return;
            }

            object value = items[index];

            RecordFilterDescriptor[] removefdc = this.GetRecordFilters(recordFilters, tableCellIdentity, filterName);
            if (removefdc != null)
            {
                foreach (RecordFilterDescriptor rfd in removefdc)
                {
                    this.recordFilters.Remove(rfd);
                }
            }

            RecordFilterDescriptor newFilter = new RecordFilterDescriptor();
            newFilter.Name = filterName;
            newFilter.MappingName = tableCellIdentity.Column.MappingName;
            newFilter.UniqueGroupId = this.GetUniqueGroupId(tableCellIdentity);
            newFilter.Conditions.Add(new FilterCondition(FilterCompareOperator.Equals, value));
                this.recordFilters.Add(newFilter);

#if ASPNET
            // REVIEW: didn't want to change old behavior of this method for ASP.NET
            // For windows forms the ApplyFilters will be called later from renderer.
            ApplyFilters();
#endif
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void ApplyFilters()
        {
            if (this.tableDescriptor != null && recordFilters != null)
            {
                this.tableDescriptor.RecordFilters.InitializeFrom(recordFilters);
            }

            this.recordFilters = null;
        }

        /// <internalonly/>
        /// <summary>
        /// Resets the filter of respective cell identifier.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void ResetFilterBar(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            string filterName = this.GetUniqueColumnGroupId(tableCellIdentity);

            this.tableDescriptor = tableCellIdentity.Table.TableDescriptor;
            this.table = tableCellIdentity.Table;
            this.recordFilters = new RecordFilterDescriptorCollection();
            this.recordFilters.InitializeFrom(tableDescriptor.RecordFilters);

            RecordFilterDescriptor[] rfdc = this.GetRecordFilters(recordFilters, tableCellIdentity, filterName);
            if (rfdc != null)
            {
                foreach (RecordFilterDescriptor rfd in rfdc)
                {
                    this.recordFilters.Remove(rfd);
                }
            }
        }

        /// <internalonly/>
        /// <summary>
        /// A method to initiate collection dialog editor while selecting custom option.
        /// </summary>
        /// <param name="tableCellIdentity">cell identifier</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void SelectCustomFilterBar(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            this.tableDescriptor = tableCellIdentity.Table.TableDescriptor;
            this.table = tableCellIdentity.Table;

            string filterName = this.GetUniqueColumnGroupId(tableCellIdentity);
            RecordFilterDescriptor[] filters = this.GetRecordFilters(tableDescriptor.RecordFilters, tableCellIdentity, filterName);

            RecordFilterDescriptor filter = null;
            if (filters == null)
            {
                filter = new RecordFilterDescriptor();
                filter.Name = tableCellIdentity.Column.Name;
                filter.MappingName = tableCellIdentity.Column.MappingName;
                filter.UniqueGroupId = this.GetUniqueGroupId(tableCellIdentity);
                this.tableDescriptor.RecordFilters.Add(filter);
            }
            if (filter != null)
            {
                foreach (GridColumnDescriptor col in tableCellIdentity.Column.Collection._inner)
                {
                    if (!col.AllowFilter)
                    {
                        if (filter.FilterDisplay == null)
                            filter.FilterDisplay = new ArrayList();
                        filter.FilterDisplay.Add(col.Name);
                    }
                }
            }
            DialogResult result = this.ShowCollectionDialog(tableDescriptor, "RecordFilters", null, typeof(RecordFilterDescriptorCollection));
            if (result == DialogResult.Cancel)
            {
                if (filter != null)
                {
                    this.tableDescriptor.RecordFilters.Remove(filter);
                }
            }
            ////Already applied changes directly in the TableDescriptor...
            this.recordFilters = null;
        }

        /// <summary>
        /// Occurs immediately before the RecordFilterCollectionEditor Dialog is displayed. The ControlEventArgs.Control 
        /// the form.
        /// </summary>
        public event ControlEventHandler ShowingCustomFilterDialog;

        /// <summary>
        /// Shows the collection dialog.
        /// </summary>
        /// <param name="instance">The object instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="provider">The IServiceProvider.</param>
        /// <param name="type">The type value.</param>
        /// <returns>returns the DialogResult</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
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
    }
#if ASPNET
#else
    /// <summary>
    /// Implements the renderer part of a TableFilterBar cell.
    /// </summary>
    /// <remarks>
    /// <see cref="GridTableFilterBarCellRenderer"/> can be customized with
    /// <see cref="GridStyleInfo.DataSource"/>, <see cref="GridStyleInfo.ValueMember"/>,
    /// and <see cref="GridStyleInfo.DisplayMember"/> properties of a <see cref="GridStyleInfo"/> instance.
    /// <para/>
    /// If you do not have a datasource object, you can also fill the drop-down list contents
    /// with a <see cref="GridStyleInfo.ChoiceList"/> and optionally specify <see cref="GridStyleInfo.ExclusiveChoiceList"/>.
    /// <para/>
    /// The TableFilterBar cell is XP Themes enabled. It will be drawn themed if <see cref="GridControlBase.ThemesEnabled"/> is True.
    /// <para/>
    /// There can be several renderers
    /// associated with one <see cref="GridTableFilterBarCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// <para/>
    /// Use "TableFilterBar" as identifier in <see cref="GridStyleInfo.CellType"/> of a cell's <see cref="GridStyleInfo"/>
    /// to associate this cell type with a cell.
    /// <para/>
    /// <para/>
    /// The cell's behavior and appearance can be customized with the following properties of the <see cref="GridStyleInfo"/> class.
    /// <para/>
    /// <para/>
    /// </remarks>
    public class GridTableFilterBarCellRenderer : GridComboBoxCellRenderer
    {
        GridTableCellStyleInfoIdentity tableCellIdentity;

        /// <summary>
        /// Initializes a new <see cref="GridTableFilterBarCellRenderer"/> object for the given GridControlBase
        /// and <see cref="GridCellModelBase"/>.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridTableFilterBarCellModel"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase
        /// and GridCellModelBase will be saved.</remarks>
        public GridTableFilterBarCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
        }

        /// <summary>
        /// A reference to the parent grid.
        /// </summary>
        public new GridTableControl Grid
        {
            get
            {
                return (GridTableControl) base.Grid;
            }
        }

        /// <summary>
        /// A reference to the cell model.
        /// </summary>
        public new GridTableFilterBarCellModel Model
        {
            get
            {
                return (GridTableFilterBarCellModel) base.Model;
            }
        }

        /// <override/>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo) StyleInfo;
            this.tableCellIdentity = tableStyleInfo.TableCellIdentity;

            base.OnInitialize(rowIndex, colIndex);
        }

        /// <override/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && this.StyleInfo.DropDownStyle == GridDropDownStyle.AutoComplete)
            {
                CurrentCell.CloseDropDown(PopupCloseType.Done);
                int index = this.FindItem(this.ControlText, false, -1, true);
                if (index > -1)
                {
                    this.Model.Select(tableCellIdentity, index);
                    this.Grid.GroupingControl.Table.CurrentRecordManager.NavigateTo(null);
                    // Apply filter.
                    this.Model.ApplyFilters();
                }
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Handles the <see cref="Control.MouseUp"/> event of the list box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with event data.</param>
        protected override void ListBoxMouseUp(object sender, MouseEventArgs e)
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
            if (this.ListBoxPart.SelectedIndex >= 0)
            {
                FilterBarSelectedItemChangingEventArgs fce = new FilterBarSelectedItemChangingEventArgs(this.ListBoxPart, column, this.ListBoxPart.SelectedIndex, this.ListBoxPart.SelectedItem.ToString());
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
                    this.Model.Select(tableCellIdentity, selectedIndex);
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

            ////GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo) StyleInfo;
            ////GridTableCellStyleInfoIdentity tableCellIdentity2 = tableStyleInfo.TableCellIdentity;

            //// If this filterbar belongs to a child group or nested table then there is a good chance
            //// the current row is no FilterBar anymore. Best is to reset the current cell.
            ////if (tableCellIdentity2 == null || tableCellIdentity2.DisplayElement != tableCellIdentity.DisplayElement)
            //    CurrentCell.ResetCurrentCellWithoutDeactivate();
            ////else
            //    ControlValue = GetFilterBarText(StyleInfo);// don't call base class - ignore.
        }

        /// <override/>
        /// <summary>Allows custom formatting of a cell by changing its style object.</summary>
        /// <param name="e">Event data.</param>
        public override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            if (!e.Style.CellModel.GetType().Name.Equals("GridFilterByDisplayMemberCellModel"))
            {
                e.Style.CellValue = this.GetFilterBarText(e.Style);
            }
            base.OnPrepareViewStyleInfo(e);
        }

        /// <summary>
        /// Determines the text from record filter criteria that should be displayed in filterbar cell.
        /// </summary>
        /// <param name="style">Cell style information.</param>
        /// <returns>Filter bar text.</returns>
        public string GetFilterBarText(GridStyleInfo style)
        {
            GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo) style;
            GridTableCellStyleInfoIdentity tableCellIdentity = tableStyleInfo.TableCellIdentity;
            GridTableDescriptor td = tableCellIdentity.Table.TableDescriptor;
            string filterName = this.Model.GetUniqueColumnGroupId(tableCellIdentity);
            ////More than one filter descriptor.. -then custom
            RecordFilterDescriptor[] filters = this.Model.GetRecordFilters(td.RecordFilters, tableCellIdentity, filterName);
            object value = this.Model.SelectCustomText;
            if (filters == null)
            {
                if (style.DropDownStyle != GridDropDownStyle.AutoComplete)
                    value = this.Model.SelectAllText;
                else
                    value = string.Empty;
            }
            else if (filters.Length == 1 && filters[0].Conditions.Count == 1)
            {
                if (filters[0].Conditions[0].CompareOperator == FilterCompareOperator.Equals)
                {
                    value = filters[0].Conditions[0].CompareValue;
                    if (filters[0].Conditions[0].CompareText == "(null)" || filters[0].Conditions[0].CompareText == string.Empty)
                        value = this.Model.SelectEmptyText;
                }
            }

            return string.Format("{0:"+style.Format+"}", value);
        }

        /// <override/>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            if (!style.CellModel.GetType().Name.Equals("GridFilterByDisplayMemberCellModel"))
            {
                style.CellValue = this.GetFilterBarText(style);
            }
            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
        }

        /// <override/>
        /// <summary>Specifies the active text that is displayed on the cell.</summary>
        public override string ControlText
        {
            get
            {
                return base.ControlText;
            }

            set
            {
                if (inSet)
                    return;
                inSet = true;
             	SetTextBoxText(GetFilterBarText(StyleInfo), false);// don't call base class - ignore.
                inSet = false;
            }
        }

		//handle recursive calls for delete key = Defect#12292
        private bool inSet = false;
		
        /// <override/>
        /// <summary>Occurs when the drop down is about to be shown.</summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event data.</param>
        public override void DropDownContainerShowingDropDown(object sender, CancelEventArgs e)
        {
            GridComboBoxListBoxPart listBoxPart = (GridComboBoxListBoxPart) ListBoxPart;

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
    }
#endif

    //// eva GridFilterBarChoices SyncfusionCancel GridColumnDescriptor column bool ShouldCreateSummaryDescriptor Element element

    /// <summary>
    /// Provides data for the GridGroupingControl.QueryFilterBarChoices even which occurs
    /// when GridTableDescriptor is initializing columns with .AllowFilter set and gives
    /// you the option to handle filterbarchoices through custom code. In such case the event is also raised
    /// when the user clicks on on dropdown button of a GridTableFilterBarCell.
    /// </summary>
    /// <remarks>
    /// In event handler code you should check whether Element is null (Nothing).<para/>
    /// If it is null then only set ShouldCreateSummaryDescriptor to false if you want to
    /// provide your own choice list.<para/>
    /// If it is not null then you should initialize the UniqueFilterBarValues with choices to
    /// be displayed to the user.
    /// </remarks>
    public sealed class GridQueryFilterBarChoicesEventArgs : SyncfusionCancelEventArgs
    {
        // Event is raised so that users have a chance to
        // provide their own filter bar implementation, e.g. if they have a SQL database or if they use LINQ
        // then they could simply do a "SELECT UNIQUE column from table" call which will return all the
        // unique element for the table. Optionally a user can also do a "SELECT UNIQUE column from table
        // WHERE xyz" call and specify filters.
        GridColumnDescriptor column;
        bool shouldCreateSummaryDescriptor;
        Element element;
        object[] uniqueFilterBarValues;

        /// <summary>
        /// Constructor for GridQueryFilterBarChoicesEventArg.
        /// </summary>
        /// <param name="column">Column name.</param>
        /// <param name="shouldCreateSummaryDescriptor">True if summary descriptor should be created.</param>
        /// <param name="element">The element at the filter row.</param>
        public GridQueryFilterBarChoicesEventArgs(GridColumnDescriptor column, bool shouldCreateSummaryDescriptor, Element element)
        {
            this.column = column;
            this.shouldCreateSummaryDescriptor = shouldCreateSummaryDescriptor;
            this.element = element;
        }

        /// <summary>
        /// The column for which choices should be shown
        /// </summary>
        [TraceProperty(true)]
        public GridColumnDescriptor Column
        {
            get
            {
                return this.column;
            }
        }

        /// <summary>
        /// Set this property false if you do not want a summary being created for the specified column. The
        /// engine will raise the event with <see cref="Element"/> = null (Nothing) in order to investigate
        /// for which columns it should create internal summaries.
        /// </summary>
        [TraceProperty(true)]
        public bool ShouldCreateSummaryDescriptor
        {
            get
            {
                return this.shouldCreateSummaryDescriptor;
            }

            set
            {
                this.shouldCreateSummaryDescriptor = value;
            }
        }

        /// <summary>
        /// When a user clicks on the dropdown button of a FilterBarCell then this event is raised and
        /// the Element will contain the element at the given filter bar row. You can check Element.ParentGroup
        /// to find out which group the element belongs to. If Element is null (Nothing) you should simply
        /// set <see cref="ShouldCreateSummaryDescriptor"/> as needed.
        /// </summary>
        public Element Element
        {
            get
            {
                return this.element;
            }
        }

        /// <summary>
        /// When a user clicks on the dropdown button of a FilterBarCell then this event is raised and
        /// the Element will contain the element at the given filter bar row. You should then initialize
        /// this array with values that should be displayed in the dropdown.
        /// </summary>
        public object[] UniqueFilterBarValues
        {
            get
            {
                return this.uniqueFilterBarValues;
            }

            set
            {
                this.uniqueFilterBarValues = value;
            }
        }
    }
    /// <summary>
    /// Provides data for GridGroupingControl.FilterBarSelectedItemChanged event which occurs after
    /// an item selected through the filtered dropdown.
    /// </summary>
    public sealed class FilterBarSelectedItemChangedEventArgs : SyncfusionEventArgs
    {
        private int selectedIndex;
        private string selectedText;
        private GridColumnDescriptor column;
        /// <summary>
        /// Constructor for FilterBarSelectedItemChangedEventArgs.
        /// </summary>
        /// <param name="selectedIndex">Index of the selected item.</param>
        /// <param name="selectedText">Text of the selected item.</param>
        /// <param name="column">GridColumnDescriptor.</param>
        public FilterBarSelectedItemChangedEventArgs(GridColumnDescriptor column, int selectedIndex, string selectedText)
        {
            this.column = column;
            this.selectedIndex = selectedIndex;
            this.selectedText = selectedText;
        }
        /// <summary>
        /// Gets the currently selected column descriptor.
        /// </summary>
        public GridColumnDescriptor Column
        {
            get
            {
                return column;
            }
        }
        /// <summary>
        /// Gets the selected index of the filtered item.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
        }
        /// <summary>
        /// Gets the selected text of the filtered item.
        /// </summary>
        public string SelectedText
        {
            get
            {
                return selectedText;
            }
        }
    }
    /// <summary>
    /// Provides data for GridGroupingControl.FilterBarSelectedItemChanging event which occurs when
    /// an item selected through the filtered dropdown.
    /// </summary>
    public sealed class FilterBarSelectedItemChangingEventArgs : SyncfusionCancelEventArgs
    {
        private int selectedIndex;
        private string selectedText;
        private ListBox list;
        private GridListControl gridList;
        private GridColumnDescriptor column;
        /// <summary>
        /// Constructor for FilterBarSelectedItemChangingEventArgs.
        /// </summary>
        /// <param name="selectedIndex">Index of the selected item.</param>
        /// <param name="selectedText">Text of the selected item.</param>
        /// <param name="list">List box.</param>
        /// <param name="column">GridColumnDescriptor.</param>
        public FilterBarSelectedItemChangingEventArgs(ListBox list, GridColumnDescriptor column, int selectedIndex, string selectedText)
        {
            this.list = list;
            this.column = column;
            this.selectedIndex = selectedIndex;
            this.selectedText = selectedText;
        }
        /// <summary>
        /// provides data for GridGroupingControl.FilterBarSelectedItemChanging event which occurs when
        /// </summary>
        /// <param name="gridList"></param>
        /// <param name="column"></param>
        /// <param name="selectedIndex"></param>
        /// <param name="selectedText"></param>
        public FilterBarSelectedItemChangingEventArgs(GridListControl gridList, GridColumnDescriptor column, int selectedIndex, string selectedText)
        {
            this.gridList = gridList;
            this.column = column;
            this.selectedIndex = selectedIndex;
            this.selectedText = selectedText;
        }
        /// <summary>
        /// Gets the currently selected column descriptor.
        /// </summary>
        public GridColumnDescriptor Column
        {
            get
            {
                return column;
            }
        }
        /// <summary>
        /// Gets/Sets the selected index of the filtered item.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                if (selectedIndex != value)
                {
                    selectedIndex = value;
                    if (list.Items.Count > value)
                    {
                        selectedText = list.Items[value].ToString();
                    }
                }
            }
        }
        /// <summary>
        /// Gets/Sets the selected text of the filtered item.
        /// </summary>
        public string SelectedText
        {
            get
            {
                return selectedText;
            }
            set
            {
                if (selectedText != value)
                {
                    selectedText = value;
                    if (list.Items.Contains(value))
                    {
                        selectedIndex = this.list.Items.IndexOf(value);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Handles the <see cref="ExceptionManager.ExceptionCatched"/> event.
    /// </summary>
    /// <param name="sender">The object instance</param>
    /// <param name="e">The GridQueryFilterBarChoicesEventArgs</param>
    public delegate void GridQueryFilterBarChoicesEventHandler(object sender, GridQueryFilterBarChoicesEventArgs e);
    /// <summary>
    /// Handles the FilterBarSelectedItemChanging event.
    /// </summary>
    /// <param name="sender">The object instance</param>
    /// <param name="e">The FilterBarSelectedItemChangingEventArgs data.</param>
    public delegate void FilterBarSelectedItemChangingEventHandler(object sender, FilterBarSelectedItemChangingEventArgs e);
    /// <summary>
    /// Handles the FilterBarSelectedItemChanged event.
    /// </summary>
    /// <param name="sender">The object instance</param>
    /// <param name="e">The FilterBarSelectedItemChangedEventArgs data.</param>
    public delegate void FilterBarSelectedItemChangedEventHandler(object sender, FilterBarSelectedItemChangedEventArgs e);
    /// <summary>
    /// Implements the DataModel part for a TableFilterBar GridListControl cell.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridTableFilterBarGridListCellModel"/> can serve as model for several <see cref="GridTableFilterBarGridListCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridTableFilterBarGridListCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridTableFilterBarGridListCellModel : GridDropDownGridListControlCellModel
    {
        internal bool isCombobox = false;
        /// <overload>
        /// Initializes a new <see cref="GridTableFilterBarCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridTableFilterBarCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridTableFilterBarGridListCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = false;
            ButtonBarSize = new Size(SystemInformation.VerticalScrollBarWidth, 0);
            SupportsChoiceList = true;
            this.isCombobox = false;
        }

        /// <overload>
        /// Initializes a new <see cref="GridTableFilterBarCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridTableFilterBarCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <param name="isCombobox"> To set GridListControl's appearance as Combobox</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridTableFilterBarGridListCellModel(GridModel grid, bool isCombobox)
            : base(grid)
        {
            AllowFloating = false;
            ButtonBarSize = new Size(SystemInformation.VerticalScrollBarWidth, 0);
            SupportsChoiceList = true;
            this.isCombobox = isCombobox;
        }
        /// <summary>
        /// Initializes a new <see cref="GridTableFilterBarCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridTableFilterBarGridListCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        static GridTableFilterBarGridListCellModel()
        {
            GridTableFilterBarGridListCellModel.selectAllText = SR.GetString(SR.FilterBarAll);
            GridTableFilterBarGridListCellModel.selectCustomText = SR.GetString(SR.FilterBarCustom);
            GridTableFilterBarGridListCellModel.selectEmptyText = SR.GetString(SR.FilterBarEmpty);
        }
#if ASPNET
#else
        /// <summary>
        /// Creates a <see cref="GridTableFilterBarGridListCellRenderer"/> for this cell model that is specific to the GridControlBase.
        /// </summary>
        /// <param name="control">The <see cref="GridControlBase"/> the cell renderer is created for.</param>
        /// <returns>A new <see cref="GridTableFilterBarGridListCellRenderer"/> specific for a <see cref="GridControlBase"/>.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridTableFilterBarGridListCellRenderer(control, this);
        }
#endif
        /// <summary>
        /// Returns the unique choices to be displayed in the filterbar cell.
        /// </summary>
        /// <param name="tableCellIdentity">Identity for the table cell.</param>
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
#if ASPNET
#else
            GridGroupingControl gc = tableCellIdentity.Table.Engine.ParentControl;
            GridQueryFilterBarChoicesEventArgs qe = new GridQueryFilterBarChoicesEventArgs(tableCellIdentity.Column, false, tableCellIdentity.DisplayElement);
            gc.OnQueryFilterBarChoices(qe);
            if (!qe.Cancel)
            {
                result = qe.UniqueFilterBarValues;
            }
#endif

            return result;
        }
        /// <summary>
        /// Gets the datasource for filter bar choices.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public override object GetDataSource(GridStyleInfo style)
        {
            GridTableCellViewStyleInfoIdentity cellIdentity = (GridTableCellViewStyleInfoIdentity)style.CellIdentity;
            object [] obj = this.GetFilterBarChoices(cellIdentity);
            return obj;
        }
        /// <summary>
        /// Provides data for fill with choices.
        /// </summary>
        /// <param name="listBox"></param>
        /// <param name="style"></param>
        /// <param name="exclusive"></param>
        public override void FillWithChoices(ListBox listBox, GridStyleInfo style, out bool exclusive)
        {
            exclusive = true;
            base.FillWithChoices(listBox, style, out exclusive);
        }
        /// <summary>
        /// Initializes a listbox with data binding information from a <see cref="GridStyleInfo"/>
        /// object.
        /// </summary>
        /// <param name="listBox">The list box to be initialized with data binding information.</param>
        /// <param name="style">The style object with binding information.</param>
        /// <param name="exclusive">A place holder that indicates whether the GridListControl is filled with an exclusive
        /// list of possible choices or if non-standard values are allowed.
        /// </param>
#if ASPNET
        public void FillWithChoices(DropDownList listBox, GridStyleInfo style, out bool exclusive)
#else
        public void FillWithChoices(GridListControl listBox, GridStyleInfo style, out bool exclusive)
#endif
        {
            exclusive = true;
            GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)style;
            listBox.BeginUpdate();
            object[] items = (object[])this.GetFilterBarChoices(tableStyleInfo.TableCellIdentity);
            if (items != null)
            {
                listBox.DataSource = null;
                listBox.DisplayMember = tableStyleInfo.TableCellIdentity.Column.MappingName;
                listBox.ValueMember = tableStyleInfo.TableCellIdentity.Column.MappingName;
                DataTable dt = new DataTable();
                dt.Columns.Add(tableStyleInfo.TableCellIdentity.Column.Name);

                dt.Rows.Add(this.SelectAllText);
                dt.Rows.Add(this.SelectCustomText);
                foreach (object item in items)
                {
                    if (item is DBNull || item == string.Empty)
                    {
                        dt.Rows.Add(this.SelectEmptyText);
                    }
                    else if (item != null)
                    {
                        dt.Rows.Add(item);
                    }
                }
                listBox.DataSource = dt;
            }
            listBox.EndUpdate();
        }

        internal static string selectAllText;
        internal static string selectCustomText;
        internal static string selectEmptyText;

        /// <summary>
        /// Default: (All)
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
        /// Default: (Custom...)
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
        /// Default: (Empty)
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
        /// Applies a filter criteria. Note: The first two entries are reserved for (All) and (Custom).
        /// An index greater than one represents a valid choice found with GetFilterBarChoices.
        /// </summary>
        /// <param name="tableCellIdentity">Identity for the table cell.</param>
        /// <param name="index">Index of the filter bar choice to be selected.</param>
        public void Select(GridTableCellStyleInfoIdentity tableCellIdentity, int index)
        {
            if (index >= 0)
            {
                if (index == 0)
                {
                    this.ResetFilterBar(tableCellIdentity);
                }
                else if (index == 1)
                {
                    // Custom
                    this.SelectCustomFilterBar(tableCellIdentity);
                }
                else
                {
                    this.SelectItem(tableCellIdentity, index - 2);
                }
            }
        }

        /// <summary>
        /// Internal only.
        /// </summary>
        /// <param name="tableCellIdentity">The table cell identity.</param>
        /// <returns>returns the UniqueColumnGroupId</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
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
                return tableCellIdentity.Column.Name + "@" + g.UniqueGroupIdsToString();
            }
        }

        /// <summary>
        /// Internal only.
        /// </summary>
        /// <param name="tableCellIdentity">The table cell identity.</param>
        /// <returns>returns the UniqueGroupId</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public object[] GetUniqueGroupId(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            Group g = tableCellIdentity.DisplayElement.ParentGroup;
            return g.UniqueGroupId;
        }

        /// <internalonly/>
        /// <summary>
        /// Gets array of RecordFilterDescriptors for the specific column
        /// with respect to the particular GroupUniqueID
        /// </summary>
        /// <param name="recordFilters">Record filters collection.</param>
        /// <param name="tableCellIdentity">Specified grid table cell.</param>
        /// <param name="filterName">Name of the filter.</param>
        /// <returns>Array of RecordFilterDescriptors.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        public RecordFilterDescriptor[] GetRecordFilters(RecordFilterDescriptorCollection recordFilters, GridTableCellStyleInfoIdentity tableCellIdentity, string filterName)
        {
            GridTableDescriptor td = tableCellIdentity.Table.TableDescriptor;
            GridRelationDescriptor rd = td.ParentRelation;
            RecordFilterDescriptor[] rfdc = recordFilters.GetRecordFilters(tableCellIdentity.Column.MappingName);
            if (rfdc != null)
            {
                object[] uniqueId = tableCellIdentity.DisplayElement.ParentGroup.UniqueGroupId;
                ArrayList rfdList = new ArrayList();
                foreach (RecordFilterDescriptor rfd in rfdc)
                {
                    if (rfd.CompareUniqueId(uniqueId))
                    {
                        rfdList.Add(rfd);
                    }
                }

                if (rfdList.Count > 0)
                {
                    return (RecordFilterDescriptor[])rfdList.ToArray(typeof(RecordFilterDescriptor));
                }
            }
            return null;
        }

        RecordFilterDescriptorCollection recordFilters;
        TableDescriptor tableDescriptor;
        Syncfusion.Grouping.Table table;

        /// <internalonly/>
        /// <summary>
        /// Selects the item at the selected index in respective cell identifier.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void SelectItem(GridTableCellStyleInfoIdentity tableCellIdentity, int index)
        {
            string filterName = this.GetUniqueColumnGroupId(tableCellIdentity);

            this.tableDescriptor = tableCellIdentity.Table.TableDescriptor;
            this.table = tableCellIdentity.Table;
            this.recordFilters = new RecordFilterDescriptorCollection();
            this.recordFilters.InitializeFrom(tableDescriptor.RecordFilters);

            object[] items = this.GetFilterBarChoices(tableCellIdentity);

            ////check to see if list was shortened after index was set
            if (index >= items.GetLength(0))
            {
                return;
            }

            object value = items[index];

            RecordFilterDescriptor[] removefdc = this.GetRecordFilters(recordFilters, tableCellIdentity, filterName);
            if (removefdc != null)
            {
                foreach (RecordFilterDescriptor rfd in removefdc)
                {
                    this.recordFilters.Remove(rfd);
                }
            }

            RecordFilterDescriptor newFilter = new RecordFilterDescriptor();
            newFilter.Name = filterName;
            newFilter.MappingName = tableCellIdentity.Column.MappingName;
            newFilter.UniqueGroupId = this.GetUniqueGroupId(tableCellIdentity);
            newFilter.Conditions.Add(new FilterCondition(FilterCompareOperator.Equals, value));
            this.recordFilters.Add(newFilter);
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void ApplyFilters()
        {
            if (this.tableDescriptor != null && recordFilters != null)
            {
                this.tableDescriptor.RecordFilters.InitializeFrom(recordFilters);
            }

            this.recordFilters = null;
        }

        /// <internalonly/>
        /// <summary>
        /// Resets the filter of respective cell identifier.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void ResetFilterBar(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            string filterName = this.GetUniqueColumnGroupId(tableCellIdentity);

            this.tableDescriptor = tableCellIdentity.Table.TableDescriptor;
            this.table = tableCellIdentity.Table;
            this.recordFilters = new RecordFilterDescriptorCollection();
            this.recordFilters.InitializeFrom(tableDescriptor.RecordFilters);
            RecordFilterDescriptor[] rfdc = this.GetRecordFilters(recordFilters, tableCellIdentity, filterName);
            if (rfdc != null)
            {
                foreach (RecordFilterDescriptor rfd in rfdc)
                {
                    this.recordFilters.Remove(rfd);
                }
            }
        }

        /// <internalonly/>
        /// <summary>
        /// A method to initiate collection dialog editor while selecting custom option.
        /// </summary>
        /// <param name="tableCellIdentity">cell identifier</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void SelectCustomFilterBar(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            this.tableDescriptor = tableCellIdentity.Table.TableDescriptor;
            this.table = tableCellIdentity.Table;

            string filterName = this.GetUniqueColumnGroupId(tableCellIdentity);
            RecordFilterDescriptor[] filters = this.GetRecordFilters(tableDescriptor.RecordFilters, tableCellIdentity, filterName);

            RecordFilterDescriptor filter = null;
            if (filters == null)
            {
                filter = new RecordFilterDescriptor();
                filter.Name = tableCellIdentity.Column.Name;
                filter.MappingName = tableCellIdentity.Column.MappingName;
                filter.UniqueGroupId = this.GetUniqueGroupId(tableCellIdentity);
                this.tableDescriptor.RecordFilters.Add(filter);
            }

            DialogResult result = this.ShowCollectionDialog(tableDescriptor, "RecordFilters", null, typeof(RecordFilterDescriptorCollection));
            if (result == DialogResult.Cancel)
            {
                if (filter != null)
                {
                    this.tableDescriptor.RecordFilters.Remove(filter);
                }
            }
            ////Already applied changes directly in the TableDescriptor...
            this.recordFilters = null;
        }

        /// <summary>
        /// Occurs immediately before the RecordFilterCollectionEditor Dialog is displayed. The ControlEventArgs.Control 
        /// the form.
        /// </summary>
        public event ControlEventHandler ShowingCustomFilterDialog;

        /// <summary>
        /// Shows the collection dialog.
        /// </summary>
        /// <param name="instance">The object instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="provider">The IServiceProvider.</param>
        /// <param name="type">The type value.</param>
        /// <returns>returns the DialogResult</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
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
    }
#if ASPNET
#else
    /// <summary>
    /// Implements the renderer part of a TableFilterBar GridListControl cell.
    /// </summary>
    /// <remarks>
    /// <see cref="GridTableFilterBarGridListCellRenderer"/> can be customized with
    /// <see cref="GridStyleInfo.DataSource"/>, <see cref="GridStyleInfo.ValueMember"/>,
    /// and <see cref="GridStyleInfo.DisplayMember"/> properties of a <see cref="GridStyleInfo"/> instance.
    /// <para/>
    /// If you do not have a datasource object, you can also fill the drop-down list contents
    /// with a <see cref="GridStyleInfo.ChoiceList"/> and optionally specify <see cref="GridStyleInfo.ExclusiveChoiceList"/>.
    /// <para/>
    /// The TableFilterBar cell is XP Themes enabled. It will be drawn themed if <see cref="GridControlBase.ThemesEnabled"/> is True.
    /// <para/>
    /// There can be several renderers
    /// associated with one <see cref="GridTableFilterBarGridListCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// <para/>
    /// Use "TableFilterBar" as identifier in <see cref="GridStyleInfo.CellType"/> of a cell's <see cref="GridStyleInfo"/>
    /// to associate this cell type with a cell.
    /// <para/>
    /// <para/>
    /// The cell's behavior and appearance can be customized with the following properties of the <see cref="GridStyleInfo"/> class.
    /// <para/>
    /// <para/>
    /// </remarks>
    public class GridTableFilterBarGridListCellRenderer : GridDropDownGridListControlCellRenderer
    {
        GridTableCellStyleInfoIdentity tableCellIdentity;
        private bool isFiltered = false;

        /// <summary>
        /// Initializes a new <see cref="GridTableFilterBarGridListCellRenderer"/> object for the given GridControlBase
        /// and <see cref="GridCellModelBase"/>.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridTableFilterBarGridListCellModel"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase
        /// and GridCellModelBase will be saved.</remarks>
        public GridTableFilterBarGridListCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
        }

        /// <summary>
        /// A reference to the parent grid.
        /// </summary>
        public new GridTableControl Grid
        {
            get
            {
                return (GridTableControl)base.Grid;
            }
        }

        /// <summary>
        /// A reference to the cell model.
        /// </summary>
        public new GridTableFilterBarGridListCellModel Model
        {
            get
            {
                return (GridTableFilterBarGridListCellModel)base.Model;
            }
        }

        /// <override/>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)StyleInfo;
            this.tableCellIdentity = tableStyleInfo.TableCellIdentity;
            bool exclusive;

            this.Model.FillWithChoices(this.ListControlPart, StyleInfo, out exclusive);

                this.ListControlPart.ShowColumnHeader = false;
                this.ListControlPart.MultiColumn = false;
                this.ListControlPart.Grid.Model.EnableLegacyStyle = this.Grid.Model.EnableLegacyStyle;
                this.ListControlPart.GridVisualStyles = this.Grid.GetGridVisualStyles();
                this.ListControlPart.Grid.Properties.DisplayHorzLines = true;
        }

        /// <override/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            
            if (e.KeyCode == Keys.Enter && this.StyleInfo.DropDownStyle == GridDropDownStyle.AutoComplete)
            {
                CurrentCell.CloseDropDown(PopupCloseType.Done);
                int index = this.FindItem(this.ControlText, false, -1, true);
                if (index > -1)
                {
                    this.Model.Select(tableCellIdentity, index);
                    this.Grid.GroupingControl.Table.CurrentRecordManager.NavigateTo(null);
                    // Apply filter.
                    this.Model.ApplyFilters();
                }
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }

        /// <override/>
        protected override void ListControlGridPrepareViewStyleInfo(object sender, GridPrepareViewStyleInfoEventArgs e)
        {
            if (this.Model.isCombobox)
                e.Style.Borders.Bottom = GridBorder.Empty;
            base.ListControlGridPrepareViewStyleInfo(sender, e);
        }

        /// <summary>
        /// Handles the <see cref="Control.MouseUp"/> event of the list box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with event data.</param>
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
            if (listBoxPart.SelectedIndex >= 0)
            {
                FilterBarSelectedItemChangingEventArgs fce = new FilterBarSelectedItemChangingEventArgs(this.ListControlPart, column, this.ListControlPart.SelectedIndex, this.ListControlPart.SelectedItem.ToString());
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
                    if (this.ListControlPart.SelectedIndex > 0 && this.Grid.TableDescriptor.RecordFilters.Count > 0)
                        isFiltered = true;
                    else
                        isFiltered = false;
                    if (this.ListControlPart.SelectedIndex < 0)
                    {
                        this.ListControlPart.SelectedIndex = selectedIndex;
                    }
                    FilterBarSelectedItemChangedEventArgs fe = new FilterBarSelectedItemChangedEventArgs(column, this.ListControlPart.SelectedIndex, this.ListControlPart.SelectedItem.ToString());
                    gc.OnFilterBarSelectedItemChanged(fe);
                }
            }

        GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo) StyleInfo;
        GridTableCellStyleInfoIdentity tableCellIdentity2 = tableStyleInfo.TableCellIdentity;

         //If this filterbar belongs to a child group or nested table then there is a good chance
         //the current row is no FilterBar anymore. Best is to reset the current cell.
        if (tableCellIdentity2 == null || tableCellIdentity2.DisplayElement != tableCellIdentity.DisplayElement)
            CurrentCell.ResetCurrentCellWithoutDeactivate();
        else
        {
            ControlValue = GetFilterBarText(StyleInfo);// don't call base class - ignore.
           // tableCellIdentity2.te
        }
        }
        /// <summary>
        /// Gets the filter bar text.
        /// </summary>
        protected override void OnBeginEdit()
        {
            ControlValue = null;
            GridStyleInfo style = this.Grid.Model[this.CurrentCell.RowIndex, this.CurrentCell.ColIndex];
            style.CellValue = this.GetFilterBarText(style);
            this.CurrentCell.ShowDropDown();
            base.OnBeginEdit();
        }
        /// <override/>
        /// <summary>Allows custom formatting of a cell by changing its style object.</summary>
        /// <param name="e">Event data.</param>
        public override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            if (!e.Style.CellModel.GetType().Name.Equals("GridFilterByDisplayMemberCellModel"))
            {
                e.Style.CellValue = this.GetFilterBarText(e.Style);
            }
            base.OnPrepareViewStyleInfo(e);
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
            ////More than one filter descriptor.. -then custom
            RecordFilterDescriptor[] filters = this.Model.GetRecordFilters(td.RecordFilters, tableCellIdentity, filterName);
            object value = this.Model.SelectCustomText;
            if (filters == null)
            {
                if (style.DropDownStyle != GridDropDownStyle.AutoComplete)
                    value = this.Model.SelectAllText;
                else
                    value = string.Empty;
            }
            else if (filters.Length == 1 && filters[0].Conditions.Count == 1)
            {
                if (filters[0].Conditions[0].CompareOperator == FilterCompareOperator.Equals)
                {
                    value = filters[0].Conditions[0].CompareValue;
                    if (filters[0].Conditions[0].CompareText == "(null)" || filters[0].Conditions[0].CompareText == string.Empty)
                        value = this.Model.SelectEmptyText;
                }
            }

            return string.Format("{0:" + style.Format + "}", value);
        }

        /// <override/>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            if (style.Text == string.Empty && ControlValue != null)
            {
                style.CellValue = this.GetFilterBarText(style);
            }
            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
        }
        /// <summary>
        /// Events to draw a cell buttons.
        /// </summary>
        /// <param name="button">cell button</param>
        /// <param name="g">Graphics</param>
        /// <param name="rowIndex">Gets row index</param>
        /// <param name="colIndex">Gets col index</param>
        /// <param name="bActive">Active draw cell button</param>
        /// <param name="style">Gets the draw style</param>
        protected override void OnDrawCellButton(GridCellButton button, Graphics g, int rowIndex, int colIndex, bool bActive, GridStyleInfo style)
        {
            if (!this.Grid.Model.EnableLegacyStyle)
            {
                Bitmap bm;
                GridTableCellStyleInfo tablestyle = style as GridTableCellStyleInfo;
                if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro || this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black)
                {
                    if (this.Grid.TableDescriptor.RecordFilters.Contains(tablestyle.TableCellIdentity.Column.Name) && this.isFiltered)
                        bm = GridGroupingBitmaps.IconPainter.GetBitmap("filtered_metro.png");
                    else
                        bm = GridGroupingBitmaps.IconPainter.GetBitmap("filter_metro.png");
                }
                else
                {
                    if (this.Grid.TableDescriptor.RecordFilters.Contains(tablestyle.TableCellIdentity.Column.Name) && this.isFiltered)
                        bm = GridGroupingBitmaps.IconPainter.GetBitmap("filtered_gray.png");
                    else
                        bm = GridGroupingBitmaps.IconPainter.GetBitmap("filter_gray.png");
                }
                Point ptOffset = Point.Empty;

                if (button is Syncfusion.Windows.Forms.Grid.GridCellComboBoxButton)
                {
                    base.OnDrawCellButton(button, g, rowIndex, colIndex, bActive, style);
                    GridGroupingBitmaps.IconPainter.PaintIcon(g, button.Bounds, ptOffset, bm, Color.Blue);
                }
            }
            else
            {
                base.OnDrawCellButton(button, g, rowIndex, colIndex, bActive, style);
            }
        }

        /// <override/>
        /// <summary>Specifies the active text that is displayed on the cell.</summary>
        public override string ControlText
        {
            get
            {
                return base.ControlText;
            }

            set
            {
                if (inSet)
                    return;
                inSet = true;
                SetTextBoxText(GetFilterBarText(StyleInfo), false);// don't call base class - ignore.
                inSet = false;
            }
        }
        private bool inSet = false;
    }
#endif
}
