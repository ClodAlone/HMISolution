#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Windows.Forms.Grid.Grouping;
using System.ComponentModel;
using Syncfusion.Grouping;
using Syncfusion.Collections;
using System.Drawing;

namespace Syncfusion.GridHelperClasses
{  
    /// <summary>
    /// Implements Office2007 filter.
    /// </summary>
    public class GridOffice2007Filter
    {
        /// <summary>
        /// Intializes a new <see cref="GridOffice2007Filter"/>
        /// </summary>
        public GridOffice2007Filter()
            : base()
        {
        }

        # region ShowOffice2007FilterOnMouseHover
        private static bool showOffice2007FilterOnMouseHover = true;
        /// <summary>
        /// set this property to enable the Office2007 filter on mouse over.if false is set, the filter displays directly when the grid has been wired.
        /// </summary>
        [DefaultValue("true")]
        public static bool ShowOffice2007FilterOnMouseHover
        {
            get
            {
                return showOffice2007FilterOnMouseHover;
            }
            set
            {
                if (showOffice2007FilterOnMouseHover != value)
                {
                    showOffice2007FilterOnMouseHover = value;
                }
            }
        }
        # endregion

        #region EnableFilteredColumnIcon
        private static bool enableFilteredColumnIcon = false;
        /// <summary>
        /// Gets or Sets this property to stick the filter icon in the filtered column. 
        /// This property is effective only if the ShowOffice2007FilterOnMouseHover property is in true.
        /// </summary>
        [DefaultValue(false)]
        public static bool EnableFilteredColumnIcon
        {
            get
            {
                return enableFilteredColumnIcon;
            }
            set
            {
                if (enableFilteredColumnIcon != value)
                {
                    enableFilteredColumnIcon = value;
                }
            }
        }
        #endregion

        /// <summary>
        /// This event is used to provide list of choices.
        /// </summary>
        /// <param name="sender">Sender as GridGroupingControl</param>
        /// <param name="e">event data.</param>
        void gridGroupingControl1_QueryFilterBarChoices(object sender, GridQueryFilterBarChoicesEventArgs e)
        {
            List<object> choicesOrdered = new List<object>();
            List<object> choices = new List<object>();

            GridGroupingControl ggc = sender as GridGroupingControl;

            if (ggc != null && ggc.Table.TopLevelGroup != null)
            {
                string tableName = e.Column.TableDescriptor.Name;


                if (tableName != ggc.TableDescriptor.Name)
                    return;

                GridTable table = ggc.GetTable(tableName);

                string format = (e.Column.Appearance.AnyRecordFieldCell.Format != null && e.Column.Appearance.AnyRecordFieldCell.Format.Length > 0)
                    ? string.Format("{{0:{0}}}", e.Column.Appearance.AnyRecordFieldCell.Format) : "{0}";
                if (table != null && table.FilteredRecords != null)
                {
                    foreach (Record o in table.FilteredRecords)
                    {
                        string val = string.Format(format, o.GetValue(e.Column.MappingName));// pdc[e.Column.MappingName].GetValue(o.GetData()));
                        int loc = choicesOrdered.BinarySearch(val);
                        if (loc < 0)
                        {
                            choicesOrdered.Insert(-loc - 1, val);
                            choices.Add(val);
                        }
                    }
                }
            }

            e.UniqueFilterBarValues = choices.ToArray();
        }

        /// <summary>
        /// Hook the grouping grid to the office2007 filter.
        /// </summary>
        /// <param name="groupingGrid">The grouping grid.</param>
        public void WireGrid(GridGroupingControl groupingGrid)
        {
            if (groupingGrid != null)
            {                
                groupingGrid.BeginUpdate();
                groupingGrid.TableModel.CellModels["ColumnHeaderCell"] = new Grid2007ExcelFilterCellModel(groupingGrid.TableModel);            
                groupingGrid.TableDescriptor.IsExcelFilterWired = true;
                groupingGrid.QueryFilterBarChoices += new GridQueryFilterBarChoicesEventHandler(gridGroupingControl1_QueryFilterBarChoices);
                groupingGrid.TableModel.UpdateColumnWidths(true);
                groupingGrid.EndUpdate(true);
                groupingGrid.Refresh();
            }
        }

        /// <summary>
        /// Unhook the grouping grid from the office2007 filter.
        /// </summary>
        /// <param name="groupingGrid">The grouping grid.</param>
        public void UnWireGrid(GridGroupingControl groupingGrid)
        {
            if (groupingGrid != null)
            {
                groupingGrid.BeginUpdate();
                groupingGrid.TableModel.CellModels["ColumnHeaderCell"] = new GridTableColumnHeaderCellModel(groupingGrid.TableModel);
                groupingGrid.TableDescriptor.IsExcelFilterWired = false;
                groupingGrid.QueryFilterBarChoices -= new GridQueryFilterBarChoicesEventHandler(gridGroupingControl1_QueryFilterBarChoices);
                groupingGrid.TableModel.UpdateColumnWidths(true);
                groupingGrid.EndUpdate(true);
                groupingGrid.Refresh();
                groupingGrid = null;
            }
        }
    }

    /// <summary>
    /// Implements optimized Office2007 filter.
    /// </summary>
    public class GridExcelFilter
    {
        /// <summary>
        /// Intializes a new <see cref="GridExcelFilter"/>
        /// </summary>
        public GridExcelFilter()
            : base()
        {
        }
        private GridGroupingControl grid = null;
        /// <summary>
        /// Hook the grouping grid to the optimized office2007 filter.
        /// </summary>
        /// <param name="groupingGrid">The grouping grid.</param>
        public void WireGrid(GridGroupingControl groupingGrid)
        {
            if (groupingGrid != null)
            {
                groupingGrid.BeginUpdate();
                if (!AllowIndividualColumnWiring)
                    groupingGrid.TableModel.CellModels["ColumnHeaderCell"] = new GridExcelFilterCellModel(groupingGrid.TableModel, this);
                else
                    groupingGrid.TableModel.CellModels.Add("GridExcelFilterCell", new GridExcelFilterCellModel(groupingGrid.TableModel, this));
                RemoveStandardFilterLists(groupingGrid);
                groupingGrid.TableDescriptor.IsExcelFilterWired = true;
                groupingGrid.TableModel.UpdateColumnWidths(true);
                groupingGrid.QueryFilterBarChoices += new GridQueryFilterBarChoicesEventHandler(gridGroupingControl1_QueryFilterBarChoices);
                grid = groupingGrid;
                groupingGrid.EndUpdate(true);
                groupingGrid.Refresh();
            }
        }
        /// <summary>
        ///This event ,occurs after an item filtered.
        /// </summary>
        [Description("Occurs after an item filtered"), Category("Filter")]
        public event ListPropertyChangedEventHandler RecordFiltersItemChanged;

        /// <summary>
        /// Raises the <see cref="RecordFiltersItemChanged"/> event.
        /// </summary>
        /// <param name="e">Event data.</param>
        public virtual void OnRecordFiltersItemChanged(ListPropertyChangedEventArgs e)
        {
            if (RecordFiltersItemChanged != null)
            {
                RecordFiltersItemChanged(this, e);
            }
        }

        /// <summary>
        ///This event occurs after an item filtered.
        /// </summary>
        [Description("Occurs after an item filtered"), Category("Filter")]
        public event ListPropertyChangedEventHandler RecordFiltersItemChanging;

        /// <summary>
        /// Raises the <see cref="RecordFiltersItemChanged"/> event.
        /// </summary>
        /// <param name="e">Event data.</param>
        public virtual void OnRecordFiltersItemChanging(ListPropertyChangedEventArgs e)
        {
            if (RecordFiltersItemChanging != null)
            {
                RecordFiltersItemChanging(this, e);
            }
        }
        private Dictionary<string, List<string>> filterCollection = null;
        /// <summary>
        /// Get and sets the collection of objects for filter
        /// </summary>
        public Dictionary<string, List<string>> FilterCollection
        {
            get
            {
                GridExcelFilterCellRenderer renderer = grid.TableControl.CellRenderers["ColumnHeaderCell"] as GridExcelFilterCellRenderer;
                if (renderer != null)
                    filterCollection = renderer.FilterCollection;
                else
                    return null;
                return filterCollection;
            }
            set
            {
                filterCollection = value;
                GridExcelFilterCellRenderer renderer = grid.TableControl.CellRenderers["ColumnHeaderCell"] as GridExcelFilterCellRenderer;
                renderer.FilterCollection = filterCollection;
            }

        }
        [Obsolete("This Method is duplicated. You can use FilterCollecion instead")]
        public Dictionary<string, List<string>> GetFilterCollection(GridGroupingControl grid)
        {
            return FilterCollection;
        }
        private static bool enableFilteredColumnIcon = false;
        /// <summary>
        /// Gets or Sets this property to stick the filter icon in the filtered column. 
        /// </summary>
        [DefaultValue(false)]
        public static bool EnableFilteredColumnIcon
        {
            get
            {
                return enableFilteredColumnIcon;
            }
            set
            {
                if (enableFilteredColumnIcon != value)
                {
                    enableFilteredColumnIcon = value;
                }
            }
        }
        private bool allowSearch = true;
        /// <summary>
        /// Gets or Sets a value to perform search operation in checkedlist box of filter.
        /// </summary>
        [DefaultValue(true)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowSearch
        {
            get
            {
                return allowSearch;
            }
            set
            {
                allowSearch = value;
            }
        }

        private bool allowFilterByColor = false;
        /// <summary>
        /// Gets or Sets a value to enable or disable the filter by color option.
        /// </summary>
        [DefaultValue(false)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowFilterByColor
        {
            get
            {
                return allowFilterByColor;
            }
            set
            {
                allowFilterByColor = value;
            }
        }

        private bool allowResize = false;
        /// <summary>
        /// Gets or Sets a value to resize the filterdialog.
        /// </summary>
        [DefaultValue(false)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowResize
        {
            get
            {
                return allowResize;
            }
            set
            {
                allowResize = value;
            }
        }
        bool allowIndividualColumnWiring = false;
        bool enableNumberFilter = false;
        bool enableDateFilter = false;
        /// <summary>
        /// It allows to set the desired filter on specified column,if the value is set to True, 
        /// through which the filter can be set in column using 'this.gridGroupingControl1.TableDescriptor.Columns[ColumnName].Appearance.ColumnHeaderCell.CellType = "GridExcelFilterCell"'
        /// </summary>
        [DefaultValue(false)]
        public bool AllowIndividualColumnWiring
        {
            get
            {
                return allowIndividualColumnWiring;
            }
            set
            {
                allowIndividualColumnWiring = value;
            }
        }
        /// <summary>
        /// It allows to set enable the number filter 
        /// </summary>
        [DefaultValue(false)]
        public bool EnableNumberFilter
        {
            get
            {
                return enableNumberFilter;
            }
            set
            {
                enableNumberFilter = value;
            }
        }

        /// <summary>
        /// It allows to set enable the date filter 
        /// </summary>
        [DefaultValue(false)]
        public bool EnableDateFilter
        {
            get
            {
                return enableDateFilter;
            }
            set
            {
                enableDateFilter = value;
            }
        }
        /// <summary>
        /// An internal method to remove the choices.
        /// </summary>
        /// <param name="gridGroupingControl1">The grouping grid.</param>
        private void RemoveStandardFilterLists(GridGroupingControl gridGroupingControl1)
        {
            List<SummaryDescriptor> toRemove = new List<SummaryDescriptor>();
            foreach (SummaryDescriptor sd in gridGroupingControl1.TableDescriptor.Summaries)
            {
                if (sd.Name.EndsWith("FilterBarChoices"))
                {
                    toRemove.Add(sd);
                }
            }

            foreach (SummaryDescriptor sd in toRemove)
            {
                gridGroupingControl1.TableDescriptor.Summaries.Remove(sd);
            }
        }

        /// <summary>
        /// Clears all the filter choices of the GridGroupingControl.
        /// </summary>
        /// <param name="grid">GridGroupingControl</param>
        public void ClearFilters(GridGroupingControl grid)
        {
            GridExcelFilterCellRenderer renderer = grid.TableControl.CellRenderers["ColumnHeaderCell"] as GridExcelFilterCellRenderer;
            if (renderer != null)
            {
                renderer.ClearFilters();
                grid.Table.TableDirty = true;
                grid.Refresh();
            }
        }

        /// <summary>
        /// This event is used to provide list of choices.
        /// </summary>
        /// <param name="sender">Sender as GridGroupingControl</param>
        /// <param name="e">event data.</param>
        void gridGroupingControl1_QueryFilterBarChoices(object sender, GridQueryFilterBarChoicesEventArgs e)
        {
            List<object> choicesOrdered = new List<object>();
            List<object> choices = new List<object>();

            GridGroupingControl ggc = sender as GridGroupingControl;

            if (ggc != null)
            {
                string tableName = e.Column.TableDescriptor.Name;


                if (tableName != ggc.TableDescriptor.Name)
                    return;

                table = ggc.GetTable(tableName);

                string format = (e.Column.Appearance.AnyRecordFieldCell.Format != null && e.Column.Appearance.AnyRecordFieldCell.Format.Length > 0)
                    ? string.Format("{{0:{0}}}", e.Column.Appearance.AnyRecordFieldCell.Format) : "{0}";

                int i = 0;
                backColorCollection.Clear();
                name = e.Column.MappingName;

                foreach (Record o in table.FilteredRecords)
                {
                    string val = string.Format(format, o.GetValue(e.Column.MappingName));// pdc[e.Column.MappingName].GetValue(o.GetData()));


                    int loc = choicesOrdered.BinarySearch(val);
                    if (loc < 0)
                    {
                        choicesOrdered.Insert(-loc - 1, val);
                        choices.Add(val);
                    }
                    i++;
                }
            }

            e.UniqueFilterBarValues = choices.ToArray();
        }

        GridTable table;
        string name;
        /// <summary>
        /// Used internally to get the available font color's and available cell backcolor of 
        /// </summary>
        internal void ColorCollection(string columnName)
        {
            if (name != columnName)
                name = columnName;
            backColorCollection.Clear();
            fontColorCollection.Clear();
            int i = this.grid.TableControl.TopRowIndex;
            foreach (Record o in table.Records)
            {
                if (!o.IsCaption() && o.Kind == DisplayElementKind.Record && o.ParentRecord == null)
                {
                    Color backColor = this.grid.TableModel[o.GetRowIndex(), this.grid.TableModel.NameToColIndex(name)].BackColor;
                    Color fontColor = this.grid.TableModel[o.GetRowIndex(), this.grid.TableModel.NameToColIndex(name)].TextColor;
                    if (!backColorCollection.ContainsValue(backColor))
                        backColorCollection.Add(o.GetRowIndex(), backColor);
                    if (!fontColorCollection.ContainsValue(fontColor))
                        fontColorCollection.Add(o.GetRowIndex(), fontColor);
                    i++;
                }
            }

            if (!uniqueColorCollections.ContainsKey(name))
                uniqueColorCollections.Add(name, backColorCollection);
        }
        internal Dictionary<int, Color> backColorCollection = new Dictionary<int, Color>();
        internal Dictionary<int, Color> fontColorCollection = new Dictionary<int, Color>();
        internal Dictionary<string, Dictionary<int, Color>> uniqueColorCollections = new Dictionary<string, Dictionary<int, Color>>();
        /// <summary>
        /// Unhook the grouping grid from the optimized office2007 filter.
        /// </summary>
        /// <param name="groupingGrid">The grouping grid.</param>
        public void UnWireGrid(GridGroupingControl groupingGrid)
        {
            if (groupingGrid != null)
            {
                groupingGrid.BeginUpdate();
                groupingGrid.TableModel.CellModels["ColumnHeaderCell"] = new GridTableColumnHeaderCellModel(groupingGrid.TableModel);
                groupingGrid.QueryFilterBarChoices -= new GridQueryFilterBarChoicesEventHandler(gridGroupingControl1_QueryFilterBarChoices);
                groupingGrid.TableDescriptor.IsExcelFilterWired = false;
                groupingGrid.TableModel.UpdateColumnWidths(true);
                groupingGrid.EndUpdate(true);
                groupingGrid.Refresh();
                groupingGrid = null;
                if (grid != null)
                {
                    grid.BeginUpdate();
                    grid.TableModel.CellModels["ColumnHeaderCell"] = new GridTableColumnHeaderCellModel(grid.TableModel);
                    grid.TableDescriptor.IsExcelFilterWired = false;
                    grid.TableModel.UpdateColumnWidths(true);
                    grid.EndUpdate(true);
                    grid.Refresh();
                    grid = null;
                }
            }
        }
    }
}
