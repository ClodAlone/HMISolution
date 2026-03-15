#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Syncfusion.Grouping; 
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms.Grid.Grouping;
using Syncfusion.Windows.Forms;
using Syncfusion.Collections.BinaryTree;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using Syncfusion.Collections;
//using System.Linq;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// Implements the DataModel part for a ExcelFilterCell.
    /// </summary>
    public class Grid2007ExcelFilterCellModel : GridHeaderCellModel
    {
        /// <summary>
        /// Initializes a new <see cref="Grid2007ExcelFilterCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public Grid2007ExcelFilterCellModel(GridModel grid)
            : base(grid)
        {
        }
        /// <summary>
        /// Creates a renderer for hte cell.
        /// </summary>
        /// <param name="control">GridControlBase</param>
        /// <returns></returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new Grid2007ExcelFilterCellRenderer(control, this);
        }
    }
    /// <summary>
    /// Implements the renderer part of a ExcelFilterCell.
    /// </summary>
    public class Grid2007ExcelFilterCellRenderer : GridHeaderCellRenderer
    {
        private FilterDropDownControl ddUser;
        Rectangle filterRect = Rectangle.Empty;
        bool IsMouseOver = false;
        int colindex = -1;
        Hashtable filterImages;
        int filterRow = -1, Filtercol = -1;
        ToolStripMenuItem sortAsc;
        ToolStripMenuItem sortDesc;
        ToolStripMenuItem clearFilter;
        ToolStripMenuItem textFilters;
        ToolStripMenuItem equal;
        ToolStripMenuItem notEqual;
        ToolStripMenuItem beginsWith;
        ToolStripMenuItem endsWith;
        ToolStripMenuItem contains;
        ToolStripMenuItem customFilter;
        List<string> customFiltered = new List<string>();
        private object[] items = null;
        ContextMenuStrip filterDialog;
        string clearFilterString = SR.GetString(SR.ClearFilterFrom);
        int filterHeight = 15, filterWidth = 15;
        private string compareText1, compareText2;
        private Dictionary<string, object[]> filteredColumnCollection = new Dictionary<string, object[]>();
        #region Constructor

        /// <summary>
        /// Initializes a new <see cref="Grid2007ExcelFilterCellRenderer"/> object for the given GridControlBase
        /// and <see cref="GridCellModelBase"/>.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="Grid2007ExcelFilterCellModel"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>
        /// References to GridControlBase and GridCellModelBase will be saved.
        /// </remarks>
        public Grid2007ExcelFilterCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            filterImages = new Hashtable();
            filterImages.Add("Filter_gray", DynamicFilterBitmaps.GetBitmap("filter_funnel_gr"));
            filterImages.Add("ClearFilter_gray", DynamicFilterBitmaps.GetBitmap("filter_set_gr"));
            filterImages.Add("Filter_white", DynamicFilterBitmaps.GetBitmap("filter_metro"));
            filterImages.Add("ClearFilter_white", DynamicFilterBitmaps.GetBitmap("filter_set_wh"));
            //filterImages.Add("RemoveFilter", );

            filterDialog = new ContextMenuStrip();

            this.ddUser = new FilterDropDownControl(this.Grid.GetGridVisualStylesDrawing().VisualStyle.ToString());
            //hook the usercontrol save and cancel events...
            this.ddUser.UserControlSave += new EventHandler(ddUser_UserControlSave);
            this.ddUser.UserControlCancel += new EventHandler(ddUser_UserControlCancel);
            this.ddUser.Dock = DockStyle.Fill;
            sortAsc = new ToolStripMenuItem(SR.GetString(SR.SortAtoZ), DynamicFilterBitmaps.GetBitmap("sortasc"), ItemSelection_Click);
            sortAsc.CheckOnClick = true;
            this.filterDialog.Items.Add(sortAsc);
            sortDesc = new ToolStripMenuItem(SR.GetString(SR.SortZtoA), DynamicFilterBitmaps.GetBitmap("sortdesc"), ItemSelection_Click);
            sortDesc.CheckOnClick = true;
            this.filterDialog.Items.Add(sortDesc);
            this.filterDialog.Items.Add(new ToolStripSeparator());
            this.clearFilter = new ToolStripMenuItem(SR.GetString(SR.ClearFilterFrom), DynamicFilterBitmaps.GetBitmap("filter_delete"), ItemSelection_Click);
            this.filterDialog.Items.Add(clearFilter);
            textFilters = new ToolStripMenuItem(SR.GetString(SR.TextFilters));


            equal = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterEquals), null, CustomItemSelection_Click);
            notEqual = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterNotEquals), null, CustomItemSelection_Click);
            beginsWith = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterBeginswith), null, CustomItemSelection_Click);
            endsWith = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterEndswith), null, CustomItemSelection_Click);
            contains = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterContains), null, CustomItemSelection_Click);
            customFilter = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterCustomFilter), null, CustomItemSelection_Click);
            textFilters.DropDownItems.Add(equal);
            textFilters.DropDownItems.Add(notEqual);
            textFilters.DropDownItems.Add(new ToolStripSeparator());
            textFilters.DropDownItems.Add(beginsWith);
            textFilters.DropDownItems.Add(endsWith);
            textFilters.DropDownItems.Add(contains);
            textFilters.DropDownItems.Add(new ToolStripSeparator());
            textFilters.DropDownItems.Add(customFilter);
            this.filterDialog.Items.Add(textFilters);
            this.filterDialog.Items.Add(new ToolStripSeparator());
            ToolStripControlHost filterDropDownStrip = new ToolStripControlHost(this.ddUser);
            this.filterDialog.Items.Add(filterDropDownStrip);
            this.ddUser.BackColor = filterDropDownStrip.BackColor;
            this.Grid.CellClick += new GridCellClickEventHandler(Grid_CellClick);
            this.filterDialog.Opening += new CancelEventHandler(filterDialog_Opening);
            this.Grid.CurrentCellAcceptedChanges += new CancelEventHandler(Grid_CurrentCellAcceptedChanges);
        }

        /// <summary>
        /// Used to notify the current cell editing.
        /// </summary>
        /// <param name="sender">CurrentCell changes</param>
        /// <param name="e">event data</param>
        void Grid_CurrentCellAcceptedChanges(object sender, CancelEventArgs e)
        {
            if (this.Grid.CurrentCell != null)
            {
                int colIndex = this.Grid.CurrentCell.ColIndex;
                int rowIndex = this.Grid.CurrentCell.RowIndex;
                string colName = string.Empty;
                GridTableCellStyleInfo sty = this.Grid.Model[rowIndex, colIndex] as GridTableCellStyleInfo;
                if (sty.TableCellIdentity != null)
                {
                    string format = sty.Format.Length == 0 ? "{0}" : string.Format("{{0:{0}}}", sty.Format);
                    GridTableDescriptor tableDes = sty.TableCellIdentity.Table.TableDescriptor;
                    string columnName = sty.TableCellIdentity.Column.Name;
                    string item = this.Grid.Model[rowIndex, colIndex].FormattedText;
                    bool notContain = false;
                    RecordFilterDescriptor fil = sty.TableCellIdentity.Table.TableDescriptor.RecordFilters[columnName];
                    if (fil != null && fil.Conditions.Count > 0)
                    {
                        foreach (FilterCondition con in fil.Conditions)
                        {
                            if (!con.CompareText.Equals(item))
                            {
                                notContain = true;
                                break;
                            }
                        }
                        if (notContain)
                        {
                            FilterCondition con = new FilterCondition(FilterCompareOperator.Equals, item);
                            con.CompareText = item;
                            con.CompareValue = item;
                            fil.Conditions.Add(con);
                            sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Add(fil);
                            notContain = false;
                            BindEditedItemIntoCollections(columnName, item, sty);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the record collections.
        /// </summary>
        /// <param name="colName">Filtered Column Name</param>
        /// <param name="sty">Cell style</param>
        /// <returns>Record collections</returns>
        private object[] GetRecord(string colName, GridTableCellStyleInfo sty)
        {
            List<string> choicesOrdered = new List<string>();
            List<string> choices = new List<string>();
            string format = (sty.Format != null && sty.Format.Length > 0)
                    ? string.Format("{{0:{0}}}", sty.Format) : "{0}";

            foreach (Record o in sty.TableCellIdentity.Table.Records)
            {
                string val = string.Format(format, o.GetValue(colName));
                int loc = choicesOrdered.BinarySearch(val);
                if (loc < 0)
                {
                    choicesOrdered.Insert(-loc - 1, val);
                    choices.Add(val);
                }
            }
            choices.Sort();
            return choices.ToArray();
        }

        /// <summary>
        /// Gets the formatted string.
        /// </summary>
        /// <param name="sty">Cell style</param>
        /// <param name="str">Cell value</param>
        /// <returns>Formated String</returns>
        internal string GetFormattedString(GridTableCellStyleInfo sty, string str)
        {
            string format = sty.Format.Length == 0 ? "{0}" : string.Format("{{0:{0}}}", sty.Format);
            int intOutValue;
            Int32 int32OutValue;
            Int64 int64OutValue;
            double doubleOutValue;
            float floatOutValue;
            decimal decimalOutValue;
            DateTime dateTimeOutValue;
            if (sty.CellValueType == typeof(int) && !string.IsNullOrEmpty(str)
                && int.TryParse(str, out intOutValue))
            {
                str = string.Format(format, intOutValue);
            }
            else if (sty.CellValueType == typeof(Int32) && !string.IsNullOrEmpty(str)
                && Int32.TryParse(str, out int32OutValue))
            {
                str = string.Format(format, int32OutValue);
            }
            else if (sty.CellValueType == typeof(Int64) && !string.IsNullOrEmpty(str)
                && Int64.TryParse(str, out int64OutValue))
            {
                str = string.Format(format, int64OutValue);
            }
            else if (sty.CellValueType == typeof(decimal) && !string.IsNullOrEmpty(str)
                && decimal.TryParse(str, out decimalOutValue))
            {
                str = string.Format(format, decimalOutValue);
            }
            else if (sty.CellValueType == typeof(double) && !string.IsNullOrEmpty(str)
                && double.TryParse(str, out doubleOutValue))
            {
                str = string.Format(format, doubleOutValue);
            }
            else if (sty.CellValueType == typeof(float) && !string.IsNullOrEmpty(str)
           && float.TryParse(str, out floatOutValue))
            {
                str = string.Format(format, floatOutValue);
            }
            else if (sty.CellValueType == typeof(DateTime) && !string.IsNullOrEmpty(str)
                && DateTime.TryParse(str, out dateTimeOutValue))
            {
                str = string.Format(format, dateTimeOutValue);
            }
            return str;
        }

        /// <summary>
        /// Gets record list collections.
        /// </summary>
        /// <param name="filterName">Filter column name</param>
        /// <param name="sty">Cell Style</param>
        /// <returns>Formated string list collection</returns>
        internal List<string> GetRecordList(string filterName, GridTableCellStyleInfo sty)
        {
            List<string> list = new List<string>();
            foreach (Record rec in sty.TableCellIdentity.Table.Records)
            {
                list.Add(GetFormattedString(sty, rec.GetValue(filterName).ToString()));
            }
            return list;
        }

        /// <summary>
        /// Gets filtered record collection list.
        /// </summary>
        /// <param name="filterName">Filter column name</param>
        /// <param name="sty">Cell Style</param>
        /// <returns>Formated string list collection</returns>
        internal List<string> GetFilteredRecordList(string filterName, GridTableCellStyleInfo sty)
        {
            List<string> list = new List<string>();
            foreach (Record rec in sty.TableCellIdentity.Table.FilteredRecords)
            {
                list.Add(GetFormattedString(sty, rec.GetValue(filterName).ToString()));
            }
            return list;
        }

        /// <summary>
        /// Used to Modify the edited item into filttered collection.
        /// </summary>
        /// <param name="filterName">Filter column name</param>
        /// <param name="item">Current cell value</param>
        /// <param name="formatStyle">Cell style</param>
        internal void BindEditedItemIntoCollections(string filterName, string item, GridTableCellStyleInfo formatStyle)
        {
            List<string> filterListItemTemp = new List<string>();
            List<string> UnfilteredListItemTemp = new List<string>();
            List<DateTime> valuesDate = new List<DateTime>();
            List<Double> valuesDouble = new List<Double>();
            List<Decimal> valuesDecimal = new List<Decimal>();
            List<int> valuesInt = new List<int>();
            List<Int16> valuesInt16 = new List<Int16>();
            List<Int64> valuesInt64 = new List<Int64>();
            List<float> valuesfloat = new List<float>();

            List<DateTime> filteredCollectionDate = new List<DateTime>();
            List<Double> filteredCollectionDouble = new List<Double>();
            List<Decimal> filteredCollectionDecimal = new List<Decimal>();
            List<int> filteredCollectionInt = new List<int>();
            List<Int16> filteredCollectionInt16 = new List<Int16>();
            List<Int64> filteredCollectionInt64 = new List<Int64>();
            List<float> filteredCollectionfloat = new List<float>();
            List<string> recordList = new List<string>();
            List<string> filteredRecordList = new List<string>();
            recordList = GetRecordList(filterName, formatStyle);
            filteredRecordList = GetFilteredRecordList(filterName, formatStyle);
            bool blank = false;
            string format = formatStyle.Format.Length == 0 ? "{0}" : string.Format("{{0:{0}}}", formatStyle.Format);
            if (filteredColumnCollection != null && filteredColumnCollection.ContainsKey(filterName))
            {
                object[] filteredColumnItem;

                if (filteredColumnCollection.TryGetValue(filterName, out filteredColumnItem))
                {
                    for (int i = 0; i < filteredColumnItem.Length; i++)
                    {
                        int intFilteredOutValue;
                        Int16 int16FilteredOutValue;
                        Int64 int64FilteredOutValue;
                        double doubleFilteredOutValue;
                        float floatFilteredOutValue;
                        decimal decimalFilteredOutValue;
                        DateTime dateTimeFilteredOutValue;
                        if (recordList.Contains(filteredColumnItem[i].ToString()))
                        {
                            if (string.IsNullOrEmpty(filteredColumnItem[i].ToString()))
                                blank = true;
                            else if (formatStyle.CellValueType == typeof(System.DateTime) && !string.IsNullOrEmpty(filteredColumnItem[i].ToString())
                              && DateTime.TryParse(filteredColumnItem[i].ToString(), out dateTimeFilteredOutValue))
                            {
                                if (!filteredCollectionDate.Contains(dateTimeFilteredOutValue))
                                {
                                    filteredCollectionDate.Add(dateTimeFilteredOutValue);
                                }
                            }
                            else if (formatStyle.CellValueType == typeof(System.Double) && !string.IsNullOrEmpty(filteredColumnItem[i].ToString())
                              && Double.TryParse(filteredColumnItem[i].ToString(), out doubleFilteredOutValue) && !filteredCollectionDouble.Contains(doubleFilteredOutValue))
                            {
                                filteredCollectionDouble.Add(doubleFilteredOutValue);
                            }
                            else if (formatStyle.CellValueType == typeof(System.Decimal) && !string.IsNullOrEmpty(filteredColumnItem[i].ToString())
                             && Decimal.TryParse(filteredColumnItem[i].ToString(), out decimalFilteredOutValue) && !filteredCollectionDecimal.Contains(decimalFilteredOutValue))
                            {
                                filteredCollectionDecimal.Add(decimalFilteredOutValue);
                            }
                            else if (formatStyle.CellValueType == typeof(int) && !string.IsNullOrEmpty(filteredColumnItem[i].ToString())
                              && int.TryParse(filteredColumnItem[i].ToString(), out intFilteredOutValue) && !filteredCollectionInt.Contains(intFilteredOutValue))
                            {
                                filteredCollectionInt.Add(intFilteredOutValue);
                            }
                            else if (formatStyle.CellValueType == typeof(System.Int16) && !string.IsNullOrEmpty(filteredColumnItem[i].ToString())
                             && Int16.TryParse(filteredColumnItem[i].ToString(), out int16FilteredOutValue) && !filteredCollectionInt16.Contains(int16FilteredOutValue))
                            {
                                filteredCollectionInt16.Add(int16FilteredOutValue);
                            }
                            else if (formatStyle.CellValueType == typeof(System.Int64) && !string.IsNullOrEmpty(filteredColumnItem[i].ToString())
                             && Int64.TryParse(filteredColumnItem[i].ToString(), out int64FilteredOutValue) && !filteredCollectionInt64.Contains(int64FilteredOutValue))
                            {
                                filteredCollectionInt64.Add(int64FilteredOutValue);
                            }
                            else if (formatStyle.CellValueType == typeof(float) && !string.IsNullOrEmpty(filteredColumnItem[i].ToString())
                            && float.TryParse(filteredColumnItem[i].ToString(), out floatFilteredOutValue) && !filteredCollectionfloat.Contains(floatFilteredOutValue))
                            {
                                filteredCollectionfloat.Add(floatFilteredOutValue);
                            }
                            else
                            {
                                UnfilteredListItemTemp.Add(filteredColumnItem[i].ToString());
                            }
                        }
                    }
                    int intFilteredCurrentItemOutValue;
                    Int16 int16FilteredCurrentItemOutValue;
                    Int64 int64FilteredCurrentItemOutValue;
                    double doubleFilteredCurrentItemOutValue;
                    float floatFilteredCurrentItemOutValue;
                    decimal decimalFilteredCurrentItemValue;
                    DateTime dateTimeFilteredCurrentItemOutValue;

                    if (!string.IsNullOrEmpty(item) && !UnfilteredListItemTemp.Contains(item)
                        && (formatStyle.CellValueType == null || formatStyle.CellValueType == typeof(string)))
                    {
                        UnfilteredListItemTemp.Add(item);
                    }
                    else if (string.IsNullOrEmpty(item))
                        blank = true;
                    else if (formatStyle.CellValueType == typeof(System.DateTime) && !string.IsNullOrEmpty(item)
                      && DateTime.TryParse(item, out dateTimeFilteredCurrentItemOutValue))
                    {
                        if (!filteredCollectionDate.Contains(dateTimeFilteredCurrentItemOutValue))
                        {
                            filteredCollectionDate.Add(dateTimeFilteredCurrentItemOutValue);
                        }
                    }
                    else if (formatStyle.CellValueType == typeof(System.Double) && !string.IsNullOrEmpty(item)
                    && Double.TryParse(item, out doubleFilteredCurrentItemOutValue) && !filteredCollectionDouble.Contains(doubleFilteredCurrentItemOutValue))
                    {
                        filteredCollectionDouble.Add(doubleFilteredCurrentItemOutValue);
                    }
                    else if (formatStyle.CellValueType == typeof(System.Decimal) && !string.IsNullOrEmpty(item)
                     && Decimal.TryParse(item, out decimalFilteredCurrentItemValue) && !filteredCollectionDecimal.Contains(decimalFilteredCurrentItemValue))
                    {
                        filteredCollectionDecimal.Add(decimalFilteredCurrentItemValue);
                    }
                    else if (formatStyle.CellValueType == typeof(int) && !string.IsNullOrEmpty(item)
                     && int.TryParse(item, out intFilteredCurrentItemOutValue) && !filteredCollectionInt.Contains(intFilteredCurrentItemOutValue))
                    {
                        filteredCollectionInt.Add(intFilteredCurrentItemOutValue);
                    }
                    else if (formatStyle.CellValueType == typeof(System.Int16) && !string.IsNullOrEmpty(item)
                     && Int16.TryParse(item, out int16FilteredCurrentItemOutValue) && !filteredCollectionInt16.Contains(int16FilteredCurrentItemOutValue))
                    {
                        filteredCollectionInt16.Add(int16FilteredCurrentItemOutValue);
                    }
                    else if (formatStyle.CellValueType == typeof(System.Int64) && !string.IsNullOrEmpty(item)
                     && Int64.TryParse(item, out int64FilteredCurrentItemOutValue) && !filteredCollectionInt64.Contains(int64FilteredCurrentItemOutValue))
                    {
                        filteredCollectionInt64.Add(int64FilteredCurrentItemOutValue);
                    }
                    else if (formatStyle.CellValueType == typeof(float) && !string.IsNullOrEmpty(item)
                     && float.TryParse(item, out floatFilteredCurrentItemOutValue) && !filteredCollectionfloat.Contains(floatFilteredCurrentItemOutValue))
                    {
                        filteredCollectionfloat.Add(floatFilteredCurrentItemOutValue);
                    }

                    if (formatStyle.CellValueType == typeof(DateTime))
                    {
                        filteredCollectionDate.Sort();
                        foreach (object obj in filteredCollectionDate)
                        {
                            string s = string.Format(format, Convert.ToDateTime(obj.ToString()));
                            string s1 = string.Format(format, Convert.ToDateTime(s));
                            UnfilteredListItemTemp.Add(s1);
                        }
                    }

                    else if (formatStyle.CellValueType == typeof(Double))
                    {
                        filteredCollectionDouble.Sort();
                        foreach (object obj in filteredCollectionDouble)
                        {
                            string s = string.Format(format, Convert.ToDouble(obj));
                            UnfilteredListItemTemp.Add(s.Trim());
                        }
                    }

                    else if (formatStyle.CellValueType == typeof(Decimal))
                    {
                        filteredCollectionDecimal.Sort();
                        foreach (object obj in filteredCollectionDecimal)
                        {
                            string s = string.Format(format, Convert.ToDecimal(obj));
                            UnfilteredListItemTemp.Add(s.Trim());
                        }
                    }
                    else if (formatStyle.CellValueType == typeof(int))
                    {
                        filteredCollectionInt.Sort();
                        foreach (object obj in filteredCollectionInt)
                        {
                            string s = string.Format(format, int.Parse(obj.ToString()));
                            UnfilteredListItemTemp.Add(s.Trim());
                        }
                    }
                    else if (formatStyle.CellValueType == typeof(Int16))
                    {
                        filteredCollectionInt16.Sort();
                        foreach (object obj in filteredCollectionInt16)
                        {
                            string s = string.Format(format, Convert.ToInt16(obj.ToString()));
                            UnfilteredListItemTemp.Add(s.Trim());
                        }
                    }
                    else if (formatStyle.CellValueType == typeof(Int64))
                    {
                        filteredCollectionInt64.Sort();
                        foreach (object obj in filteredCollectionInt64)
                        {
                            string s = string.Format(format, Convert.ToInt64(obj.ToString()));
                            UnfilteredListItemTemp.Add(s.Trim());
                        }
                    }
                    else if (formatStyle.CellValueType == typeof(float))
                    {
                        filteredCollectionfloat.Sort();
                        foreach (object obj in filteredCollectionfloat)
                        {
                            string s = string.Format(format, float.Parse(obj.ToString()));
                            UnfilteredListItemTemp.Add(s);
                        }
                    }
                    else
                        UnfilteredListItemTemp.Sort();

                    if (blank && !UnfilteredListItemTemp.Contains("(Blanks)"))
                        UnfilteredListItemTemp.Add("(Blanks)");
                }
            }
            if (filteredColumnCollection.ContainsKey(filterName))
                filteredColumnCollection.Remove(filterName);
            if (UnfilteredListItemTemp.Count > 0)
                filteredColumnCollection.Add(filterName, UnfilteredListItemTemp.ToArray());
        }

        /// <summary>
        /// Gets formatted text.
        /// </summary>
        /// <param name="s">Cell value</param>
        /// <param name="formatStyle">Cell style</param>
        /// <param name="filterName">filter column name</param>
        /// <returns>Formatted text</returns>
        internal string GetFormatedText(object s, GridTableCellStyleInfo formatStyle, string filterName)
        {
            string cellFormat = formatStyle.TableCellIdentity.Table.TableDescriptor.Columns[filterName].Appearance.AnyRecordFieldCell.Format;
            string format = cellFormat.Length == 0 ? "{0}" : string.Format("{{0:{0}}}", cellFormat);
            int intOutValue;
            Int32 int32OutValue;
            Int64 int64OutValue;
            double doubleOutValue;
            float floatOutValue;
            decimal decimalOutValue;
            DateTime dateTimeOutValue;
            if (formatStyle.CellValueType == typeof(System.DateTime) && !string.IsNullOrEmpty(s.ToString())
            && DateTime.TryParse(s.ToString(), out dateTimeOutValue))
            {
                s = string.Format(format, dateTimeOutValue);
            }
            else if (formatStyle.CellValueType == typeof(double) && !string.IsNullOrEmpty(s.ToString())
             && double.TryParse(s.ToString(), out doubleOutValue))
            {
                s = string.Format(format, doubleOutValue);
            }
            else if (formatStyle.CellValueType == typeof(decimal) && !string.IsNullOrEmpty(s.ToString())
              && decimal.TryParse(s.ToString(), out decimalOutValue))
            {
                s = string.Format(format, decimalOutValue);
            }
            else if (formatStyle.CellValueType == typeof(int) && !string.IsNullOrEmpty(s.ToString())
             && int.TryParse(s.ToString(), out intOutValue))
            {
                s = string.Format(format, intOutValue);
            }
            else if (formatStyle.CellValueType == typeof(System.Int32) && !string.IsNullOrEmpty(s.ToString())
              && Int32.TryParse(s.ToString(), out int32OutValue))
            {
                s = string.Format(format, int32OutValue);
            }
            else if (formatStyle.CellValueType == typeof(System.Int64) && !string.IsNullOrEmpty(s.ToString())
             && Int64.TryParse(s.ToString(), out int64OutValue))
            {
                s = string.Format(format, int64OutValue);
            }
            else if (formatStyle.CellValueType == typeof(float) && !string.IsNullOrEmpty(s.ToString())
             && float.TryParse(s.ToString(), out floatOutValue))
            {
                s = string.Format(format, floatOutValue);
            }
            return s.ToString();
        }

        /// <summary>
        /// Convertion to respective type in grid table.
        /// </summary>
        /// <param name="itemCollections">Collection of the filtered items</param>
        /// <param name="formatStyle">Cell style</param>
        /// <returns>Collection of the converted filtered items</returns>
        internal object[] ConvertItemCellType(object[] itemCollections, GridTableCellStyleInfo formatStyle)
        {
            object[] item = null;
            List<string> valuesString = new List<string>();
            List<string> values = new List<string>();
            List<DateTime> valuesDate = new List<DateTime>();
            List<Double> valuesDouble = new List<Double>();
            List<Decimal> valuesDecimal = new List<Decimal>();
            List<int> valuesInt = new List<int>();
            List<Int16> valuesInt16 = new List<Int16>();
            List<Int64> valuesInt64 = new List<Int64>();
            List<float> valuesfloat = new List<float>();
            bool blank = false;
            string format = formatStyle.Format.Length == 0 ? "{0}" : string.Format("{{0:{0}}}", formatStyle.Format);
            foreach (object it in itemCollections)
            {
                int intOutValue;
                Int16 int16OutValue;
                Int64 int64OutValue;
                double doubleOutValue;
                float floatOutValue;
                decimal decimalOutValue;
                DateTime dateTimeOutValue;

                if (string.IsNullOrEmpty(it.ToString()))
                    blank = true;
                else if (formatStyle.CellValueType == typeof(System.DateTime) && !string.IsNullOrEmpty(it.ToString())
                && DateTime.TryParse(it.ToString(), out dateTimeOutValue) && !valuesDate.Contains(dateTimeOutValue))
                {
                    valuesDate.Add(dateTimeOutValue);
                }
                else if (formatStyle.CellValueType == typeof(double) && !string.IsNullOrEmpty(it.ToString())
                 && double.TryParse(it.ToString(), out doubleOutValue) && !valuesDouble.Contains(doubleOutValue))
                {
                    valuesDouble.Add(doubleOutValue);
                }
                else if (formatStyle.CellValueType == typeof(decimal) && !string.IsNullOrEmpty(it.ToString())
                  && decimal.TryParse(it.ToString(), out decimalOutValue) && !valuesDecimal.Contains(decimalOutValue))
                {
                    valuesDecimal.Add(decimalOutValue);
                }
                else if (formatStyle.CellValueType == typeof(int) && !string.IsNullOrEmpty(it.ToString())
                 && int.TryParse(it.ToString(), out intOutValue) && !valuesInt.Contains(intOutValue))
                {
                    valuesInt.Add(intOutValue);
                }
                else if (formatStyle.CellValueType == typeof(System.Int16) && !string.IsNullOrEmpty(it.ToString())
                  && Int16.TryParse(it.ToString(), out int16OutValue) && !valuesInt16.Contains(int16OutValue))
                {
                    valuesInt16.Add(int16OutValue);
                }
                else if (formatStyle.CellValueType == typeof(System.Int64) && !string.IsNullOrEmpty(it.ToString())
                 && Int64.TryParse(it.ToString(), out int64OutValue) && !valuesInt64.Contains(int64OutValue))
                {
                    valuesInt64.Add(int64OutValue);
                }
                else if (formatStyle.CellValueType == typeof(float) && !string.IsNullOrEmpty(it.ToString())
                 && float.TryParse(it.ToString(), out floatOutValue) && !valuesfloat.Contains(floatOutValue))
                {
                    valuesfloat.Add(floatOutValue);
                }
                else
                {
                    values.Add(it.ToString());
                }
            }

            if (formatStyle.CellValueType == typeof(DateTime))
            {
                valuesDate.Sort();
                foreach (object obj in valuesDate)
                {
                    string s = string.Format(format, Convert.ToDateTime(obj.ToString()));
                    values.Add(s.ToString());
                }
            }

            else if (formatStyle.CellValueType == typeof(Double))
            {
                valuesDouble.Sort();
                foreach (object obj in valuesDouble)
                {
                    string s = string.Format(format, Convert.ToDouble(obj));
                    values.Add(s.Trim());
                }
            }

            else if (formatStyle.CellValueType == typeof(Decimal))
            {
                valuesDecimal.Sort();
                foreach (object obj in valuesDecimal)
                {
                    string s = string.Format(format, Convert.ToDecimal(obj));
                    values.Add(s.Trim());
                }
            }
            else if (formatStyle.CellValueType == typeof(int))
            {
                valuesInt.Sort();
                foreach (object obj in valuesInt)
                {
                    string s = string.Format(format, int.Parse(obj.ToString()));
                    values.Add(s.Trim());
                }
            }
            else if (formatStyle.CellValueType == typeof(Int16))
            {
                valuesInt16.Sort();
                foreach (object obj in valuesInt16)
                {
                    string s = string.Format(format, Convert.ToInt16(obj.ToString()));
                    values.Add(s.Trim());
                }
            }
            else if (formatStyle.CellValueType == typeof(Int64))
            {
                valuesInt64.Sort();
                foreach (object obj in valuesInt64)
                {
                    string s = string.Format(format, Convert.ToInt64(obj.ToString()));
                    values.Add(s.Trim());
                }
            }
            else if (formatStyle.CellValueType == typeof(float))
            {
                valuesfloat.Sort();
                foreach (object obj in valuesfloat)
                {
                    string s = string.Format(format, float.Parse(obj.ToString()));
                    values.Add(s.Trim());
                }
            }
            else
                values.Sort();

            if (blank && !values.Contains("(Blanks)"))
                values.Add("(Blanks)");

            item = values.ToArray();
            return item;
        }

        void CustomItemSelection_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem selectedItem = sender as ToolStripMenuItem;
            int index = this.textFilters.DropDownItems.IndexOf(selectedItem);
            GridTableCellStyleInfo sty = this.Grid.Model[filterRow, Filtercol] as GridTableCellStyleInfo;
            GridTableDescriptor tableDes = sty.TableCellIdentity.Table.TableDescriptor;
            string columnName = sty.TableCellIdentity.Column.Name;
            string filterName = GetFilterName(sty);
            object[] uniqueList = GetRecord(filterName, sty);
            if (index != -1)
            {
                if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                {
                    MetroCustomRowFilter textFilter = new MetroCustomRowFilter(uniqueList);
                    textFilter.columnName = sty.TableCellIdentity.Column.HeaderText;
                    textFilter.mappingName = sty.TableCellIdentity.Column.MappingName;
                    textFilter.SetCombo(index);
                    if (textFilter.ShowDialog() == DialogResult.OK)
                    {
                        sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Remove(filterName);
                        RecordFilterDescriptor rfc = new RecordFilterDescriptor(filterName, textFilter.FilterString);
                        this.compareText1 = textFilter.compareText1;
                        this.compareText2 = textFilter.compareText2;
                        sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Add(rfc);
                    }
                }
                else
                {
                    CustomRowFilter textFilter = new CustomRowFilter(this.Grid.Model.Options.GridVisualStyles.ToString(), uniqueList);
                    textFilter.columnName = sty.TableCellIdentity.Column.HeaderText;
                    textFilter.mappingName = sty.TableCellIdentity.Column.MappingName;
                    textFilter.SetCombo(index);
                    if (textFilter.ShowDialog() == DialogResult.OK)
                    {
                        sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Remove(filterName);
                        RecordFilterDescriptor rfc = new RecordFilterDescriptor(filterName, textFilter.FilterString);
                        this.compareText1 = textFilter.compareText1;
                        this.compareText2 = textFilter.compareText2;
                        sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Add(rfc);
                    }
                }
                if (filteredColumnCollection.ContainsKey(filterName))
                    filteredColumnCollection[filterName] = this.items;
                else
                    filteredColumnCollection.Add(filterName, this.items);
                if (!customFiltered.Contains(filterName))
                    customFiltered.Add(filterName);
            }
        }

        void ItemSelection_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem selectedItem = sender as ToolStripMenuItem;
            int index = this.filterDialog.Items.IndexOf(selectedItem);
            GridTableCellStyleInfo sty = this.Grid.Model[filterRow, Filtercol] as GridTableCellStyleInfo;
            GridTableDescriptor tableDes = sty.TableCellIdentity.Table.TableDescriptor;
            string columnName = sty.TableCellIdentity.Column.Name;
            string filterName = GetFilterName(sty);
            switch (index)
            {
                case 0:
                    this.sortDesc.Checked = false;
                    if (this.sortAsc.Checked)
                    {
                        tableDes.SortedColumns.Clear();
                        tableDes.SortedColumns.Add(columnName);
                    }
                    else
                    {
                        tableDes.SortedColumns.Remove(columnName);
                    }
                    break;
                case 1:
                    this.sortAsc.Checked = false;
                    if (this.sortDesc.Checked)
                    {
                        tableDes.SortedColumns.Clear();
                        tableDes.SortedColumns.Add(columnName, ListSortDirection.Descending);
                    }
                    else
                    {
                        tableDes.SortedColumns.Remove(columnName);
                    }
                    break;
                case 3:
                    if (filteredColumnCollection.ContainsKey(filterName))
                        filteredColumnCollection.Remove(filterName);
                    sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Remove(filterName);
                    if (customFiltered.Contains(filterName))
                        customFiltered.Remove(filterName);
                    break;
            }

        }

        void filterDialog_Opening(object sender, CancelEventArgs e)
        {
            GridTableCellStyleInfo sty = this.Grid.Model[filterRow, Filtercol] as GridTableCellStyleInfo;
            GridTableDescriptor tableDes = sty.TableCellIdentity.Table.TableDescriptor;
            string columnName = sty.TableCellIdentity.Column.Name;
            if (tableDes.SortedColumns.Contains(columnName))
            {
                if (tableDes.SortedColumns[columnName].SortDirection == ListSortDirection.Ascending)
                {
                    sortAsc.Checked = true;
                    sortDesc.Checked = false;
                }
                else if (tableDes.SortedColumns[columnName].SortDirection == ListSortDirection.Descending)
                {
                    sortAsc.Checked = false;
                    sortDesc.Checked = true;
                }
            }
            else if (sortAsc.Checked || sortDesc.Checked)
            {
                sortAsc.Checked = false;
                sortDesc.Checked = false;
            }

        }

        #endregion

        #region Office2007Filter control events
        void ddUser_UserControlCancel(object sender, EventArgs e)
        {
            this.filterDialog.Hide();
        }

        void ddUser_UserControlSave(object sender, EventArgs e)
        {
            GridTableCellStyleInfo sty = this.Grid.Model[filterRow, Filtercol] as GridTableCellStyleInfo;
            IList list = ddUser.GetValues();
            string filterName = GetFilterName(sty);
            if (sty.TableCellIdentity != null)
            {
                sty.Format = sty.TableCellIdentity.Table.TableDescriptor.Columns[filterName].Appearance.AnyRecordFieldCell.Format;
                sty.CellValueType = sty.TableCellIdentity.Table.TableDescriptor.Columns[filterName].Appearance.AnyRecordFieldCell.CellValueType;
                sty.CultureInfo = sty.TableCellIdentity.Table.TableDescriptor.Columns[filterName].Appearance.AnyRecordFieldCell.CultureInfo;
            }
            RecordFilterDescriptor rfc = new RecordFilterDescriptor(filterName);
            if (sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Contains(filterName))
                sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Remove(filterName);
            if (list.Count > 0)
            {
                rfc.LogicalOperator = FilterLogicalOperator.Or;
                foreach (object s in list)
                {
                    FilterCondition con = new FilterCondition(FilterCompareOperator.Equals, GetFormatedText(s, sty, filterName));
                    if (s.ToString() == "(Blanks)")
                    {
                        con.CompareText = string.Empty;
                        con.CompareValue = DBNull.Value;
                        rfc.Conditions.Add(con);
                    }
                    else
                        rfc.Conditions.Add(con);
                }
                sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Add(rfc);

                if (filteredColumnCollection.ContainsKey(filterName))
                    filteredColumnCollection[filterName] = this.items;
                else
                    filteredColumnCollection.Add(filterName, this.items);
                if (customFiltered.Contains(filterName))
                    customFiltered.Remove(filterName);
            }
            this.filterDialog.Hide();
        }

        #endregion

        #region CellOverriden Methods

        private string FindImageFile(string bitmapName)
        {
            string bitmappath = "";
            for (int n = 0; n < 10; n++)
            {
                if (System.IO.File.Exists(bitmapName))
                    bitmappath = bitmapName;

                bitmapName = @"..\" + bitmapName;
            }
            return bitmappath;
        }
        /// <summary>
        /// Is triggered whe the mouse is hovered over the cell.
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        protected override void OnMouseHoverEnter(int rowIndex, int colIndex)
        {
            IsMouseOver = true;
            colindex = colIndex;
            this.Grid.InvalidateRange(GridRangeInfo.Row(rowIndex));
            base.OnMouseHoverEnter(rowIndex, colIndex);
        }
        /// <summary>
        /// Is triggered when the mouse pointer leaves a particular area
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="e">EventArgs</param>
        protected override void OnMouseHoverLeave(int rowIndex, int colIndex, EventArgs e)
        {
            IsMouseOver = false;
            colindex = colIndex;
            this.Grid.InvalidateRange(GridRangeInfo.Row(rowIndex));
            base.OnMouseHoverLeave(rowIndex, colIndex, e);
        }
        /// <summary>
        /// Is triggered for all the text entered in cell
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="textRectangle">Rectangle</param>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="style">GridStyleInfo</param>
        protected override void OnDrawDisplayText(Graphics g, Rectangle textRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            object tag = style.Tag;
            if (Grid.PrintingMode || !(tag is ListSortDirection))
            {
                tag = null;
            }

            ListSortDirection listSortDirection = ListSortDirection.Ascending;
            int margin = filterWidth;
            if (tag != null)
            {
                listSortDirection = (ListSortDirection)tag;
                margin = filterWidth + 12;
            }

            bool isTextRightToLeft = Grid.SortIconPlacement == SortIconPlacement.Left;
            bool isTextTop = Grid.SortIconPlacement == SortIconPlacement.Top;
            if (isTextRightToLeft || Grid.IsRightToLeft() || style.RightToLeft == RightToLeft.Yes)
            {
                GridUtil.OffsetLeft(ref textRectangle, margin);
            }
            else if (isTextTop)
            {
                GridUtil.OffsetTop(ref textRectangle, 0);
            }
            else
            {
                textRectangle.Width -= margin;
            }

            base.OnDrawDisplayText(g, textRectangle, rowIndex, colIndex, style);

            #region Image Rendering

            Rectangle imageRect;
            if (Grid.IsRightToLeft() || isTextRightToLeft || style.RightToLeft == RightToLeft.Yes)
            {
                imageRect = new Rectangle(textRectangle.Left - filterWidth, textRectangle.Y + (textRectangle.Height / 6), filterWidth, filterHeight);
            }
            else
            {
                imageRect = new Rectangle(textRectangle.Right, textRectangle.Y + (textRectangle.Height / 6), filterWidth, filterHeight);
            }
            GridTableCellStyleInfo tablestyle = style as GridTableCellStyleInfo;
            Image imag_gray = (Image)filterImages["Filter_gray"];
            Image clImage_gray = (Image)filterImages["ClearFilter_gray"];
            Image imag_white = (Image)filterImages["Filter_white"];
            Image clImage_white = (Image)filterImages["ClearFilter_white"];
            if (tablestyle.TableCellIdentity.Column != null && tablestyle.TableCellIdentity.Column.AllowFilter == true)
            {
                GridTableDescriptor tableDesc = ((GridTableControl)this.Grid).TableDescriptor;
                bool isFiltered = false;
                if (tableDesc != null && tableDesc.RecordFilters.Count > 0
                    && tableDesc.RecordFilters.Contains(tablestyle.TableCellIdentity.Column.Name)
                    && GridOffice2007Filter.EnableFilteredColumnIcon)
                {
                    isFiltered = true;
                }
                if (GridOffice2007Filter.ShowOffice2007FilterOnMouseHover)
                {
                    if (IsMouseOver && colindex == colIndex)
                    {
                        filterRect = imageRect;
                    }
                    if (isFiltered && tableDesc.RecordFilters.Contains(tablestyle.TableCellIdentity.Column.Name))
                    {
                        if (Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black || Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                            g.DrawImage(clImage_white, imageRect);
                        else
                            g.DrawImage(clImage_gray, imageRect);
                        isFiltered = false;
                    }
                    else if (((IsMouseOver && colindex == colIndex) || (this.Grid.Model.SelectedRanges.AnyRangeIntersects(GridRangeInfo.Cell(rowIndex, colIndex)))))
                    {
                        if (Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black || Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                            g.DrawImage(imag_white, imageRect);
                        else
                            g.DrawImage(imag_gray, imageRect);
                    }

                }
                else
                {
                    if (IsMouseOver && colindex == colIndex)
                    {
                        filterRect = imageRect;
                    }

                    if (Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black || Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                    {
                        if (!isFiltered)
                            g.DrawImage(imag_white, imageRect);
                        else
                            g.DrawImage(clImage_white, imageRect);
                    }
                    else
                        if (!isFiltered)
                            g.DrawImage(imag_gray, imageRect);
                        else
                            g.DrawImage(clImage_gray, imageRect);

                }
            }

            #endregion
            if (tag != null)
            {
                string s = style.ValueMember;
                int dig = (!string.IsNullOrEmpty(s) && s.Length > 1) ? 2 : 1;

                Rectangle rect;
                if (isTextRightToLeft)
                {
                    rect = new Rectangle(textRectangle.Left - margin, textRectangle.Y, 10, textRectangle.Height);
                }
                else if (isTextTop)
                {
                    rect = new Rectangle(textRectangle.X, textRectangle.Y - 8, textRectangle.Width, textRectangle.Height);
                }
                else
                {
                    rect = new Rectangle(textRectangle.Right + filterWidth, textRectangle.Y, 10, textRectangle.Height);
                }

                rect = GridUtil.CenterInRect(rect, new Size(8 * dig, 8));

                Brush brush = null;
                Pen pen1 = null;
                this.Grid.Model.Options.GridVisualStylesDrawing.GetSortIconBrush(out brush, out pen1);
                if (s != string.Empty)
                {
                    //// Draw sort-position slightly to the right and above.
                    rect.Offset(-4 * (dig - 1), -4);
                    Font numFont = new Font(style.Font.Facename, 6, style.Font.FontStyle);
                    if (isTextTop)
                        DrawText(g, s, numFont, new Rectangle(rect.X + 5, rect.Y + 5, rect.Width, rect.Height), style, pen1.Color, Grid.IsRightToLeft());
                    else
                        DrawText(g, s, numFont, rect, style, pen1.Color, Grid.IsRightToLeft());
                    numFont.Dispose();
                    rect.Offset(4 * (dig - 1), 4);

                    if (isTextRightToLeft || Grid.IsRightToLeft())
                    {
                        rect.Offset(6, 0);
                    }
                    else
                    {
                        rect.Offset(-6, 0);
                    }
                }

                int i2 = Math.Max(0, (rect.Height - 6) / 2);
                rect.Inflate(-i2, -i2);
                GridTriangleDirection triangleDirection = listSortDirection == ListSortDirection.Ascending ? GridTriangleDirection.Up : GridTriangleDirection.Down;
                GridPaintTriangle.Paint(g, rect, triangleDirection, brush, pen1, true);
                pen1.Dispose();
                brush.Dispose();
            }
        }
        /// <summary>
        /// Is triggered whenever the cell is drawn
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="clientRectangle">Rectangle</param>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="style">GridStyleInfo</param>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
            Color clrBottom = Color.Empty;
            Color clrRight = Color.Empty;
            Color clrInteriorFirst = Color.Empty;
            Color clrInteriorLast = Color.Empty;
            Color clrHeaderBottom = Color.Empty;
            if (this.Grid.Model.Options.GridVisualStylesDrawing.GetHeaderBorderColors(out clrBottom, out clrRight, out clrInteriorFirst, out clrInteriorLast) &&
               ((style.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.Grid.ThemesEnabled)
                      || ((style.Themed && this.Grid.ThemesEnabled && ((this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme))))))
            {
                Rectangle rect1 = clientRectangle;
                Pen br = new Pen(clrBottom);
                g.DrawLine(br, new PointF(rect1.X, rect1.Y), new PointF((rect1.X + rect1.Width), rect1.Y));
                br.Dispose();
            }
        }

        private void Grid_CellClick(object sender, GridCellClickEventArgs e)
        {
            if (filterRect.IntersectsWith(new Rectangle(e.MouseEventArgs.X, e.MouseEventArgs.Y, 0, 0)))
            {
                GridRangeInfo range = GridRangeInfo.Cell(e.RowIndex, e.ColIndex);
                Rectangle rec = this.Grid.RangeInfoToRectangle(range);
                Point loc = new Point(rec.Right, rec.Bottom);
                Point locn = new Point(e.MouseEventArgs.X, e.MouseEventArgs.Y);
                Point postion = this.Grid.PointToScreen(locn);
                Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

                Size BottomRect = new Size(screenBounds.Width - postion.X, screenBounds.Height - postion.Y);
                if (BottomRect.Width < filterDialog.Width)
                {
                    locn.X = e.MouseEventArgs.X - filterDialog.Width;
                }
                if (postion.X > screenBounds.Width)
                {
                    locn.X = e.MouseEventArgs.X;
                }
                GridTableControl grid = this.Grid as GridTableControl;
                filterRow = e.RowIndex;
                Filtercol = e.ColIndex;
                GridTableCellStyleInfo sty = this.Grid.Model[filterRow, Filtercol] as GridTableCellStyleInfo;
                string filterName = GetFilterName(sty);
                if (filteredColumnCollection.ContainsKey(filterName))
                {
                    filteredColumnCollection.TryGetValue(filterName, out items);
                }
                else
                    items = GetFilterBarChoices(sty.TableCellIdentity);
                clearFilter.Text = clearFilterString + " " + sty.TableCellIdentity.Column.HeaderText;

                RecordFilterDescriptor rfc = new RecordFilterDescriptor(filterName);
                if (sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Contains(filterName))
                    clearFilter.Enabled = true;
                else
                    clearFilter.Enabled = false;

                IList filteredValues = this.GetFilterTextList(sty);
                GridTableCellStyleInfo formatStyle = new GridTableCellStyleInfo();
                formatStyle.CellValueType = sty.TableCellIdentity.Column.Appearance.AnyRecordFieldCell.CellValueType;
                formatStyle.Format = sty.TableCellIdentity.Column.Appearance.AnyRecordFieldCell.Format;
                formatStyle.CultureInfo = sty.TableCellIdentity.Column.Appearance.AnyRecordFieldCell.CultureInfo;
                if (!filteredColumnCollection.ContainsKey(filterName))
                    items = ConvertItemCellType(items, formatStyle);
                bool contains = customFiltered.Contains(filterName);
                this.ddUser.SetItems(items, filteredValues, formatStyle, sty, clearFilter.Enabled, contains);
                if (sty.GetActiveGridView() != null && sty.GetActiveGridView().GridOfficeScrollBars == Syncfusion.Windows.Forms.OfficeScrollBars.Metro)
                {
                    this.filterDialog.Font = new Font("Segoe UI", 8.25f);
                    this.filterDialog.ForeColor = Color.FromArgb(51, 51, 51);
                }
                this.ddUser.firstTab = true;
                this.filterDialog.Show(grid, locn);
                e.Cancel = true;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Returns an array of category keys for this group and all parent groups which is used by FilterBarCells and FilterBarSummary
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
        /// Returns the unique choices to be displayed in the filterbar cell.
        /// </summary>
        /// <param name="tableCellIdentity">Table cell identifier.</param>
        /// <returns>Filter bar choices.</returns>
        public object[] GetFilterBarChoices(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
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
                if (values != null)
                    Array.Sort(values, new ValueComparer());
                return values;
            }
            else
            {
                int summaryIndex = -1;
                ITreeTableSummary[] summaries = tableCellIdentity.DisplayElement.ParentGroup.GetSummaries(tableCellIdentity.Table);
                if (summaries.Length.Equals(0))
                {
                    SummaryDescriptor sd = new SummaryDescriptor(
                                tableCellIdentity.Column.Name + "FilterBarChoices",
                                tableCellIdentity.Column.MappingName,
                                new CreateSummaryDelegate(FilterBarChoicesSummary.CreateSummaryMethod));
                    sd.IgnoreRecordFilterCriteria = true;
                    tableCellIdentity.Table.TableDescriptor.Summaries.Add(sd);
                    summaries = tableCellIdentity.Table.GetSummaries();
                    summaryIndex = tableCellIdentity.Table.TableDescriptor.Summaries.IndexOf(sd);
                    tableCellIdentity.Table.TableDescriptor.Summaries.Remove(sd);
                }
                else
                {
                    summaryIndex = tableCellIdentity.Table.TableDescriptor.Summaries.IndexOf(tableCellIdentity.Table.TableDescriptor.Summaries[tableCellIdentity.Column.Name + "FilterBarChoices"]);
                }
                if (summaryIndex != -1)
                {
                    FilterBarChoicesSummary filterBarChoicesSummary = (FilterBarChoicesSummary)summaries[summaryIndex];
                    if (filterBarChoicesSummary.Values != null)
                        Array.Sort(filterBarChoicesSummary.Values, new ValueComparer());
                    return filterBarChoicesSummary.Values;
                }
                else
                {
                    object[] result = null;
                    GridGroupingControl gc = tableCellIdentity.Table.Engine.ParentControl;
                    GridQueryFilterBarChoicesEventArgs qe = new GridQueryFilterBarChoicesEventArgs(tableCellIdentity.Column, false, tableCellIdentity.DisplayElement);
                    gc.OnQueryFilterBarChoices(qe);
                    if (!qe.Cancel)
                    {
                        result = qe.UniqueFilterBarValues;
                    }
                    if (result != null)
                        Array.Sort(result, new ValueComparer());
                    return result;
                }
            }
        }

        /// <summary>
        /// Determines the name of the filter based on the cell style information passed in.
        /// </summary>
        /// <param name="sty">cell style information.</param>
        /// <returns>Name of the filter.</returns>
        public string GetFilterName(GridTableCellStyleInfo sty)
        {
            object[] id = GetUniqueGroupId(sty.TableCellIdentity);
            string filterName = "";
            if (id == null || id.Length == 0)
            {
                filterName = sty.TableCellIdentity.Column.Name;
            }
            else
            {
                filterName = sty.TableCellIdentity.Column.Name + "@" + sty.TableCellIdentity.DisplayElement.ParentGroup.UniqueGroupIdsToString();
            }
            return filterName;
        }

        private IList GetFilterTextList(GridTableCellStyleInfo sty)
        {
            List<string> filterValus = new List<string>();
            filterValus.Clear();
            string filterName = GetFilterName(sty);
            if (!GridUtil.IsEmpty(filterName))
            {
                if (sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Contains(filterName))
                {
                    RecordFilterDescriptor fil = sty.TableCellIdentity.Table.TableDescriptor.RecordFilters[filterName];
                    if (fil.Conditions.Count > 0)
                    {
                        foreach (FilterCondition con in fil.Conditions)
                        {
                            filterValus.Add(con.CompareText);
                        }
                    }
                    else if (!string.IsNullOrEmpty(fil.Expression))
                    {
                        filterValus.Add(this.compareText1);
                        filterValus.Add(this.compareText2);
                    }
                }
            }
            return filterValus;
        }
        #endregion
    }

    /// <summary>
    /// Implements the DataModel part for an optimized ExcelFilterCell.
    /// </summary>
    public class GridExcelFilterCellModel : GridHeaderCellModel
    {
        private GridExcelFilter filter;
        /// <summary>
        /// Initializes a new <see cref="GridExcelFilterCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <param name="filter">The <see cref="GridExcelFilter"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="filter"/> class.
        /// </remarks>
        public GridExcelFilterCellModel(GridModel grid,GridExcelFilter filter)
            : base(grid)
        {
            this.filter = filter;
        }
        /// <summary>
        /// Creates the renderer for the cell model.
        /// </summary>
        /// <param name="control">grid</param>
        /// <returns>the renderer</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridExcelFilterCellRenderer(control, this, this.filter);
        }
    }
    /// <summary>
    /// Implements the renderer part of an optimized ExcelFilterCell.
    /// </summary>
    public class GridExcelFilterCellRenderer : GridHeaderCellRenderer
    {
        private OptimizedFilterDropDownControl ddUser;
        private Rectangle filterRect = Rectangle.Empty;
        private bool IsMouseOver = false;
        private int colindex = -1;
        private GridExcelFilter filter;
        private Hashtable filterImages;
        private int filterRow = -1, Filtercol = -1;
        private ToolStripMenuItem selectedDateFilterItem = null;
        private ToolStripMenuItem selectedNumberFilterItem = null;
        private ToolStripMenuItem selectedTextFilterItem = null;
        private ToolStripMenuItem sortAsc;
        private ToolStripMenuItem sortDesc;
        private ToolStripMenuItem clearFilter;
        private ToolStripMenuItem textFilters;
        private ToolStripMenuItem dateTimeFilters;
        private ToolStripMenuItem numberFilter;
        private ToolStripMenuItem equal;
        private ToolStripMenuItem notEqual;
        private ToolStripMenuItem beginsWith;
        private ToolStripMenuItem endsWith;
        private ToolStripMenuItem contains;
        private ToolStripMenuItem customFilter;

        private ToolStripMenuItem dateEqual;
        private ToolStripMenuItem before;
        private ToolStripMenuItem after;
        private ToolStripMenuItem between;
        private ToolStripMenuItem tomorrow;
        private ToolStripMenuItem today;
        private ToolStripMenuItem yesterday;
        private ToolStripMenuItem nextWeek;
        private ToolStripMenuItem thisWeek;
        private ToolStripMenuItem lastWeek;
        private ToolStripMenuItem nextMonth;
        private ToolStripMenuItem thisMonth;
        private ToolStripMenuItem lastMonth;
        private ToolStripMenuItem nextQuarter;
        private ToolStripMenuItem thisQuarter;
        private ToolStripMenuItem lastQuarter;
        private ToolStripMenuItem nextYear;
        private ToolStripMenuItem thisYear;
        private ToolStripMenuItem lastYear;
        private ToolStripMenuItem yearToEnd;
        private ToolStripMenuItem allDatesInThePeriod;
        private ToolStripMenuItem customDateTimeFilter;

        private ToolStripMenuItem quarter1;
        private ToolStripMenuItem quarter2;
        private ToolStripMenuItem quarter3;
        private ToolStripMenuItem quarter4;
        private ToolStripMenuItem january;
        private ToolStripMenuItem february;
        private ToolStripMenuItem march;
        private ToolStripMenuItem april;
        private ToolStripMenuItem may;
        private ToolStripMenuItem june;
        private ToolStripMenuItem july;
        private ToolStripMenuItem august;
        private ToolStripMenuItem september;
        private ToolStripMenuItem october;
        private ToolStripMenuItem november;
        private ToolStripMenuItem december;

        private ToolStripMenuItem numberEqual;
        private ToolStripMenuItem numberNotEqual;
        private ToolStripMenuItem greater; 
        private ToolStripMenuItem numberBetween;
        private ToolStripMenuItem greaterOrEqual;
        private ToolStripMenuItem lessthan;
        private ToolStripMenuItem lessOrEqual;
        private ToolStripMenuItem top10;
        private ToolStripMenuItem aboveAverage;
        private ToolStripMenuItem belowAverage;
        private ToolStripMenuItem numberCustomFilter;

        private int filterHeight = 15, filterWidth = 15;
        private ContextMenuStripper filterDialog;
        private string clearFilterString = SR.GetString(SR.ClearFilterFrom);
        private Dictionary<string, object[]> filteredColumnCollection = new Dictionary<string, object[]>();
        internal bool isCurrentCellEdited = false;
        internal bool isAlldatesSelected = false;
        private ToolStripMenuItem filterByColor;
        private GridExcelFilterPopup filterPopup;
        #region Constructor

        /// <summary>
        /// Initializes a new <see cref="GridExcelFilterCellRenderer"/> object for the given GridControlBase
        /// and <see cref="GridCellModelBase"/>.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="Grid2007ExcelFilterCellModel"/> that holds data for this cell renderer that should
        /// <param name="filter">The <see cref="GridExcelFilter"/> that displays this cell renderer.</param>
        /// be shared among views.</param>
        /// <remarks>
        /// References to GridControlBase and GridCellModelBase will be saved.
        /// </remarks>
        public GridExcelFilterCellRenderer(GridControlBase grid, GridCellModelBase cellModel,GridExcelFilter filter)
            : base(grid, cellModel)
        {
            filterImages = new Hashtable();
            filterImages.Add("Filter_gray", DynamicFilterBitmaps.GetBitmap("filter_funnel_gr"));
            filterImages.Add("ClearFilter_gray", DynamicFilterBitmaps.GetBitmap("filter_set_gr"));
            filterImages.Add("Filter_white", DynamicFilterBitmaps.GetBitmap("filter_metro"));
            filterImages.Add("ClearFilter_white", DynamicFilterBitmaps.GetBitmap("filter_set_wh"));

            filterDialog = new ContextMenuStripper();
            if (grid.GetGridVisualStyles() == GridVisualStyles.Metro)
            {
                filterDialog.ShowImageMargin = true;
                filterDialog.DropShadowEnabled = false;
                //filterDialog.Renderer = new MetroExcelFilterContextMenuRenderer();
                filterDialog.BackColor = Color.White;
            }

            this.filter = filter;
            this.ddUser = new OptimizedFilterDropDownControl(grid.GetGridVisualStylesDrawing().VisualStyle.ToString(), this.filter.AllowSearch);
            //hook the usercontrol save and cancel events...
            this.ddUser.UserControlSave += new EventHandler(ddUser_UserControlSave);
            this.ddUser.UserControlCancel += new EventHandler(ddUser_UserControlCancel);
            //this.ddUser.MouseLeave += new EventHandler(ddUser_MouseLeave);
            this.ddUser.Dock = DockStyle.Fill;
            sortAsc = new ToolStripMenuItem(SR.GetString(SR.SortAtoZ), DynamicFilterBitmaps.GetBitmap("sortasc"), ItemSelection_Click);
            sortAsc.CheckOnClick = true;
            this.filterDialog.Items.Add(sortAsc);
            sortDesc = new ToolStripMenuItem(SR.GetString(SR.SortZtoA), DynamicFilterBitmaps.GetBitmap("sortdesc"), ItemSelection_Click);
            sortDesc.CheckOnClick = true;
            this.filterDialog.Items.Add(sortDesc);
            this.filterDialog.Items.Add(new ToolStripSeparator());
            this.clearFilter = new ToolStripMenuItem(SR.GetString(SR.ClearFilterFrom), DynamicFilterBitmaps.GetBitmap("filter_delete"), ItemSelection_Click);
            this.filterDialog.Items.Add(clearFilter);


            //..FILTERBYCOLOR
            this.filterByColor = new ToolStripMenuItem(SR.GetString(SR.FilterByColor));//"Filter By Color");
            if (this.filter.AllowFilterByColor)
                this.filterDialog.Items.Add(filterByColor);
            filterByColor.CheckOnClick = false;
            ////FILTERBYCOLOR


            textFilters = new ToolStripMenuItem(SR.GetString(SR.TextFilters));
            dateTimeFilters = new ToolStripMenuItem(SR.GetString(SR.DateTimeFilters));
            numberFilter = new ToolStripMenuItem(SR.GetString(SR.NumberFilters));

            equal = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterEquals), null, CustomItemSelection_Click);
            notEqual = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterNotEquals), null, CustomItemSelection_Click);
            beginsWith = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterBeginswith), null, CustomItemSelection_Click);
            endsWith = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterEndswith), null, CustomItemSelection_Click);
            contains = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterContains), null, CustomItemSelection_Click);
            customFilter = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterCustomFilter), null, CustomItemSelection_Click);
            if (filter.EnableDateFilter)
            {
                dateEqual = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterDateEquals), null, CustomItemSelection_Click);
                before = new ToolStripMenuItem(SR.GetString(SR.Office2007Filterbefore), null, CustomItemSelection_Click);
                before = new ToolStripMenuItem(SR.GetString(SR.Office2007Filterbefore), null, CustomItemSelection_Click);
                after = new ToolStripMenuItem(SR.GetString(SR.Office2007Filterafter), null, CustomItemSelection_Click);
                between = new ToolStripMenuItem(SR.GetString(SR.Office2007Filterbetween), null, CustomItemSelection_Click);
                tomorrow = new ToolStripMenuItem(SR.GetString(SR.Office2007Filtertomorrow), null, CustomItemSelection_Click);
                today = new ToolStripMenuItem(SR.GetString(SR.Office2007Filtertoday), null, CustomItemSelection_Click);
                yesterday = new ToolStripMenuItem(SR.GetString(SR.Office2007Filteryesterday), null, CustomItemSelection_Click);
                nextWeek = new ToolStripMenuItem(SR.GetString(SR.Office2007FilternextWeek), null, CustomItemSelection_Click);
                thisWeek = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterthisWeek), null, CustomItemSelection_Click);
                lastWeek = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterlastWeek), null, CustomItemSelection_Click);
                nextMonth = new ToolStripMenuItem(SR.GetString(SR.Office2007FilternextMonth), null, CustomItemSelection_Click);
                thisMonth = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterthisMonth), null, CustomItemSelection_Click);
                lastMonth = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterlastMonth), null, CustomItemSelection_Click);
                nextQuarter = new ToolStripMenuItem(SR.GetString(SR.Office2007FilternextQuarter), null, CustomItemSelection_Click);
                thisQuarter = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterthisQuarter), null, CustomItemSelection_Click);
                lastQuarter = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterlastQuarter), null, CustomItemSelection_Click);
                nextYear = new ToolStripMenuItem(SR.GetString(SR.Office2007FilternextYear), null, CustomItemSelection_Click);
                thisYear = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterthisYear), null, CustomItemSelection_Click);
                lastYear = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterlastYear), null, CustomItemSelection_Click);
                yearToEnd = new ToolStripMenuItem(SR.GetString(SR.Office2007FilteryearToDate), null, CustomItemSelection_Click);
                allDatesInThePeriod = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterallDatesInThePeriod), null, CustomItemSelection_Click);
                customDateTimeFilter = new ToolStripMenuItem(SR.GetString(SR.Office2007FiltercustomDateTimeFilter), null, CustomItemSelection_Click);

                quarter1 = new ToolStripMenuItem(SR.GetString(SR.Office2007Filterquarter1), null, CustomItemSelection_Click);
                quarter2 = new ToolStripMenuItem(SR.GetString(SR.Office2007Filterquarter2), null, CustomItemSelection_Click);
                quarter3 = new ToolStripMenuItem(SR.GetString(SR.Office2007Filterquarter3), null, CustomItemSelection_Click);
                quarter4 = new ToolStripMenuItem(SR.GetString(SR.Office2007Filterquarter4), null, CustomItemSelection_Click);
                january = new ToolStripMenuItem(SR.GetString(SR.Office2007Filterjanuary), null, CustomItemSelection_Click);
                february = new ToolStripMenuItem(SR.GetString(SR.Office2007Filterfebruary), null, CustomItemSelection_Click);
                march = new ToolStripMenuItem(SR.GetString(SR.Office2007Filtermarch), null, CustomItemSelection_Click);
                april = new ToolStripMenuItem(SR.GetString(SR.Office2007Filterapril), null, CustomItemSelection_Click);
                may = new ToolStripMenuItem(SR.GetString(SR.Office2007Filtermay), null, CustomItemSelection_Click);
                june = new ToolStripMenuItem(SR.GetString(SR.Office2007Filterjune), null, CustomItemSelection_Click);
                july = new ToolStripMenuItem(SR.GetString(SR.Office2007Filterjuly), null, CustomItemSelection_Click);
                august = new ToolStripMenuItem(SR.GetString(SR.Office2007Filteraugust), null, CustomItemSelection_Click);
                september = new ToolStripMenuItem(SR.GetString(SR.Office2007Filterseptember), null, CustomItemSelection_Click);
                october = new ToolStripMenuItem(SR.GetString(SR.Office2007Filteroctober), null, CustomItemSelection_Click);
                november = new ToolStripMenuItem(SR.GetString(SR.Office2007Filternovember), null, CustomItemSelection_Click);
                december = new ToolStripMenuItem(SR.GetString(SR.Office2007Filterdecember), null, CustomItemSelection_Click);

                dateTimeFilters.DropDownItems.Add(dateEqual);
                dateTimeFilters.DropDownItems.Add(new ToolStripSeparator());
                dateTimeFilters.DropDownItems.Add(before);
                dateTimeFilters.DropDownItems.Add(after);
                dateTimeFilters.DropDownItems.Add(between);
                dateTimeFilters.DropDownItems.Add(new ToolStripSeparator());

                dateTimeFilters.DropDownItems.Add(tomorrow);
                dateTimeFilters.DropDownItems.Add(today);
                dateTimeFilters.DropDownItems.Add(yesterday);
                dateTimeFilters.DropDownItems.Add(new ToolStripSeparator());

                dateTimeFilters.DropDownItems.Add(nextWeek);
                dateTimeFilters.DropDownItems.Add(thisWeek);
                dateTimeFilters.DropDownItems.Add(lastWeek);
                dateTimeFilters.DropDownItems.Add(new ToolStripSeparator());

                dateTimeFilters.DropDownItems.Add(nextMonth);
                dateTimeFilters.DropDownItems.Add(thisMonth);
                dateTimeFilters.DropDownItems.Add(lastMonth);
                dateTimeFilters.DropDownItems.Add(new ToolStripSeparator());

                dateTimeFilters.DropDownItems.Add(nextQuarter);
                dateTimeFilters.DropDownItems.Add(thisQuarter);
                dateTimeFilters.DropDownItems.Add(lastQuarter);
                dateTimeFilters.DropDownItems.Add(new ToolStripSeparator());

                dateTimeFilters.DropDownItems.Add(nextYear);
                dateTimeFilters.DropDownItems.Add(thisYear);
                dateTimeFilters.DropDownItems.Add(lastYear);

                dateTimeFilters.DropDownItems.Add(new ToolStripSeparator());

                dateTimeFilters.DropDownItems.Add(yearToEnd);
                dateTimeFilters.DropDownItems.Add(new ToolStripSeparator());

                dateTimeFilters.DropDownItems.Add(allDatesInThePeriod);
                dateTimeFilters.DropDownItems.Add(new ToolStripSeparator());

                dateTimeFilters.DropDownItems.Add(customDateTimeFilter);

                allDatesInThePeriod.DropDownItems.Add(quarter1);
                allDatesInThePeriod.DropDownItems.Add(quarter2);
                allDatesInThePeriod.DropDownItems.Add(quarter3);
                allDatesInThePeriod.DropDownItems.Add(quarter4);
                allDatesInThePeriod.DropDownItems.Add(new ToolStripSeparator());

                allDatesInThePeriod.DropDownItems.Add(january);
                allDatesInThePeriod.DropDownItems.Add(february);
                allDatesInThePeriod.DropDownItems.Add(march);
                allDatesInThePeriod.DropDownItems.Add(april);
                allDatesInThePeriod.DropDownItems.Add(may);
                allDatesInThePeriod.DropDownItems.Add(june);
                allDatesInThePeriod.DropDownItems.Add(july);
                allDatesInThePeriod.DropDownItems.Add(august);
                allDatesInThePeriod.DropDownItems.Add(september);
                allDatesInThePeriod.DropDownItems.Add(october);
                allDatesInThePeriod.DropDownItems.Add(november);
                allDatesInThePeriod.DropDownItems.Add(december);
            }
            if (filter.EnableNumberFilter)
            {
                numberNotEqual = new ToolStripMenuItem(SR.GetString(SR.Office2007FilternumberNotEqual), null, CustomItemSelection_Click);
                numberEqual = new ToolStripMenuItem(SR.GetString(SR.Office2007FilternumberEqual), null, CustomItemSelection_Click);
                greater = new ToolStripMenuItem(SR.GetString(SR.Office2007Filtergreater), null, CustomItemSelection_Click);
                greaterOrEqual = new ToolStripMenuItem(SR.GetString(SR.Office2007FiltergreaterOrEqual), null, CustomItemSelection_Click);
                lessthan = new ToolStripMenuItem(SR.GetString(SR.Office2007Filterlessthan), null, CustomItemSelection_Click);
                lessOrEqual = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterlessOEqual), null, CustomItemSelection_Click);
                numberBetween = new ToolStripMenuItem(SR.GetString(SR.Office2007FilternumberBetween), null, CustomItemSelection_Click);
                top10 = new ToolStripMenuItem(SR.GetString(SR.Office2007Filtertop10), null, CustomItemSelection_Click);
                aboveAverage = new ToolStripMenuItem(SR.GetString(SR.Office2007FilteraboveAverage), null, CustomItemSelection_Click);
                belowAverage = new ToolStripMenuItem(SR.GetString(SR.Office2007FilterbelowAverage), null, CustomItemSelection_Click);
                numberCustomFilter = new ToolStripMenuItem(SR.GetString(SR.Office2007FilternumberCustomFilter), null, CustomItemSelection_Click);

                numberFilter.DropDownItems.Add(numberEqual);
                numberFilter.DropDownItems.Add(numberNotEqual);
                numberFilter.DropDownItems.Add(new ToolStripSeparator());
                numberFilter.DropDownItems.Add(greater);
                numberFilter.DropDownItems.Add(greaterOrEqual);
                numberFilter.DropDownItems.Add(lessthan);
                numberFilter.DropDownItems.Add(lessOrEqual);
                numberFilter.DropDownItems.Add(numberBetween);
                numberFilter.DropDownItems.Add(new ToolStripSeparator());
                numberFilter.DropDownItems.Add(top10);
                numberFilter.DropDownItems.Add(aboveAverage);
                numberFilter.DropDownItems.Add(belowAverage);
                numberFilter.DropDownItems.Add(new ToolStripSeparator());
                numberFilter.DropDownItems.Add(numberCustomFilter);
            }
            textFilters.DropDownItems.Add(equal);
            textFilters.DropDownItems.Add(notEqual);
            textFilters.DropDownItems.Add(new ToolStripSeparator());
            textFilters.DropDownItems.Add(beginsWith);
            textFilters.DropDownItems.Add(endsWith);
            textFilters.DropDownItems.Add(contains);
            textFilters.DropDownItems.Add(new ToolStripSeparator());
            textFilters.DropDownItems.Add(customFilter);
            this.filterDialog.Items.Add(textFilters);
            this.filterDialog.Items.Add(new ToolStripSeparator());
            ToolStripControlHost filterDropDownStrip = new ToolStripControlHost(this.ddUser);
            this.filterDialog.Items.Add(filterDropDownStrip);
            this.ddUser.BackColor = filterDropDownStrip.BackColor;
            this.Grid.CellClick += new GridCellClickEventHandler(Grid_CellClick);
            this.filterDialog.Closing += new ToolStripDropDownClosingEventHandler(filterDialog_Closing);
            this.filterDialog.Opening += new CancelEventHandler(filterDialog_Opening);
            filterLists = new Dictionary<string, List<string>>();
            this.Grid.CurrentCellEditingComplete += new EventHandler(Grid_CurrentCellEditingComplete);
            this.Grid.CurrentCellAcceptedChanges += new CancelEventHandler(Grid_CurrentCellAcceptedChanges);
            filterDialog.KeyDown += new KeyEventHandler(filterDialog_KeyDown);
            if (filter.AllowSearch)
            {
                this.ddUser.searchBox1.TextChanged += new EventHandler(searchBox1_TextChanged);
                filterImages.Add("Searchicon", DynamicFilterBitmaps.GetBitmap("search"));
                filterImages.Add("Closeicon", DynamicFilterBitmaps.GetBitmap("close"));
                imgSearch = (Image)filterImages["Searchicon"];
                imgClose = (Image)filterImages["Closeicon"];
                this.ddUser.searchLabel1.Click += new EventHandler(searchLabel1_Click);
                this.ddUser.searchLabel1.MouseHover += new EventHandler(searchLabel1_MouseHover);
                this.ddUser.searchLabel1.MouseLeave += new EventHandler(searchLabel1_MouseLeave);
            }
            this.ddUser.okButton.MouseDown += new MouseEventHandler(okButton_MouseDown);
            this.clearFilter.Click += new EventHandler(clearFilter_Click);

            popupDefaultSize = this.ddUser.Size;
            filterdialogSize = this.filterDialog.ClientSize;
            if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                this.filterDialog.Renderer = new ContextMenuRenderer();
        }


        #region " Excel like ContextMenu Support "

        /// <summary>
        /// To customize the look and feel of the contextmenustripper.
        /// </summary>
        private class ContextMenuStripper : ContextMenuStrip
        {
            /// <summary>
            /// Initilaizes the contextmenustripper.
            /// </summary>
            public ContextMenuStripper()
            {
            }

            /// <summary>
            /// Renderer of the contextmenustrip to process the resizing of the contextmenustrip.
            /// </summary>
            /// <param name="m">windows message</param>
            protected override void WndProc(ref Message m)
            {
                if (Parent != null)
                {
                    if ((Parent as GridExcelFilterPopup).ProcessResizing(ref m))
                    {
                        return;
                    }
                }
                base.WndProc(ref m);
            }
        }

        /// <summary>
        /// ContextMenu renderer.
        /// </summary>
        public class ContextMenuRenderer : ToolStripProfessionalRenderer
        {
            /// <summary>
            /// base method 
            /// </summary>
            public ContextMenuRenderer()
            {
            }

            /// <summary>
            /// base ToolStripBorder
            /// </summary>
            /// <param name="e">event data</param>
            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                base.OnRenderToolStripBorder(e);
            }

            protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
            {
                base.OnRenderGrip(e);
            }

            /// <summary>
            /// Is triggered when the margin for the image is rendered.
            /// </summary>
            /// <param name="e">ToolStripRenderEventArgs</param>
            protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
            {
                e.ToolStrip.GripStyle = ToolStripGripStyle.Visible;
                Rectangle marginRect = e.AffectedBounds;
                using (SolidBrush backBrush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillRectangle(backBrush, marginRect);
                }
            }
            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                e.ArrowColor = ColorTranslator.FromHtml("#777777");
                base.OnRenderArrow(e);
            }
            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                e.Item.BackColor = ColorTranslator.FromHtml("#E2E4E7");
                base.OnRenderSeparator(e);
            }

            protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
            {
                base.OnRenderSplitButtonBackground(e);
                ToolStripSplitButton item = e.Item as ToolStripSplitButton;
                base.DrawArrow(new
                ToolStripArrowRenderEventArgs(e.Graphics, item, item.DropDownButtonBounds,
                SystemColors.ControlText, ArrowDirection.Down));
            }
        }

        #endregion

        #region " Excel Like Color Support "

        // An alternate to create an usercontrol class for the filter by color.    
        private UserControl filterControl;
        private ToolStripControlHost filterHost;
        private bool isFontColorClicked = false;
        private bool isCellColorClicked = false;
        private Color selectedColor = Color.Red;
        private List<Record> deletedRec = null;
        private List<int> deletedRecId = new List<int>();
        private List<int> rowCollection = new List<int>();
        private List<Record> removedRecord = new List<Record>();
        private GridGroupingControl ggcGrid;
        private Dictionary<int, int> rowHeight = new Dictionary<int, int>();
        private List<int> deletedRecordRI;
        private bool isFilterByColorWired = false;
        private bool isColorFilterWired = true;
        private bool offFilterFlag = true;
        private bool filtercleared = false;

        /// <summary>
        /// customizations involved in presenting sort-by-color and filter-by-color.
        /// </summary>
        /// <param name="pressedColor">selected color of the filter</param>
        private void ColorFilter(Color pressedColor)
        {
            selectedColor = pressedColor;
            deletedRecId.Clear();
            rowHeight.Clear();
            rowCollection.Clear();
            ggcGrid = sty.TableCellIdentity.Table.Engine.ParentControl;
            ggcGrid.Table.CollapseAllGroups();
            ggcGrid.Table.CollapseAllRecords();
            foreach (Record o in sty.TableCellIdentity.Table.Records)
            {
                Element el = ggcGrid.Table.DisplayElements[o.GetRowIndex()];
                if (isFontColorClicked)
                {
                    if (this.Grid.Model[o.GetRowIndex(), Filtercol].TextColor == pressedColor && (el.Kind == DisplayElementKind.Record && el.ChildTableGroupLevel == 0))
                        rowCollection.Add(o.GetRowIndex());
                }
                else
                {
                    if (this.Grid.Model[o.GetRowIndex(), Filtercol].BackColor == pressedColor && (el.Kind == DisplayElementKind.Record && el.ChildTableGroupLevel == 0))
                        rowCollection.Add(o.GetRowIndex());
                }
            }

            if (rowCollection.Count > 0)
            {
                deletedRec = new List<Record>();
                deletedRecordRI = new List<int>();
                foreach (Record o in sty.TableCellIdentity.Table.Records)
                {
                    if (rowCollection.Contains(o.GetRowIndex()))
                    {
                        deletedRecId.Add(o.Id);
                        deletedRec.Add(o);
                        deletedRecordRI.Add(o.GetRowIndex());
                    }
                }

                foreach (Record o in deletedRec)
                {
                    if (!rowHeight.ContainsKey(o.GetRowIndex()) && ggcGrid != null)
                        rowHeight.Add(o.Id, this.Grid.Model.RowHeights[o.GetRowIndex()]);
                }

                if (filterPopupClicked)
                {
                    // Filter By Color customization code.
                    filterPopupClicked = false;
                    ggcGrid.TableModel.QueryRowHeight += new GridRowColSizeEventHandler(TableModel_QueryRowHeight);
                }
                if (ggcGrid.TableControl.GridCellsRange.Contains(GridRangeInfo.Cell(3, 3)))
                    ggcGrid.TableControl.ScrollCellInView(GridRangeInfo.Cell(3, 3));
                offFilterFlag = true;
            }
            this.clearFilter.Enabled = true;
            offFilterFlag = true;
            this.Grid.Refresh();
        }
        private bool isClearFilterClicked = false;
        /// <summary>
        /// clears the color filter.
        /// </summary>
        /// <param name="sender">GridExcelFilter</param>
        /// <param name="e">event data</param>
        void clearFilter_Click(object sender, EventArgs e)
        {
            isColorFilterWired = true;
            isClearFilterClicked = false;
            if (isFilterByColorWired)
            {
                selectedCol = selectedRow = 0;
                isClearFilterClicked = true;
                filtercleared = true;
                isFilterByColorWired = false;
                isColorFilterWired = false;
                ggcGrid.TableModel.QueryRowHeight -= new GridRowColSizeEventHandler(TableModel_QueryRowHeight);
                ggcGrid.TableModel.Refresh();
            }
        }

        void TableModel_QueryRowHeight(object sender, GridRowColSizeEventArgs e)
        {
            if (e.Index >= this.Grid.TopRowIndex)
            {
                if (offFilterFlag)
                {
                    if (isColorFilterWired)
                    {
                        if (ggcGrid.Table.DisplayElements[e.Index] != null)
                        {
                            Element el = ggcGrid.Table.DisplayElements[e.Index];
                            Record rec = el.GetRecord();
                            if (el.Kind == DisplayElementKind.Record && el.ChildTableGroupLevel == 0 && !deletedRecId.Contains(rec.Id))
                            {
                                e.Handled = true;
                                e.Size = 0;
                                isFilterByColorWired = true;
                            }
                        }
                    }
                }
            }
        }


        #region " MoreCells Custom Dialog "

        MetroForm MoreCellColor;
        private System.Windows.Forms.Label moreColorlabel1;
        private System.Windows.Forms.Label moreColorNoFill;
        private System.Windows.Forms.Panel moreColorpanel1;
        private System.Windows.Forms.Label moreColorlabel2;
        private System.Windows.Forms.Label moreColorSelectedColor;
        private System.Windows.Forms.Button moreColorbutton3;
        private System.Windows.Forms.Button moreColorbutton4;
        private GridControl moreColorGridControl1;

        private int selectedRow, selectedCol = 0;
        private Rectangle selectionBounds = new Rectangle(0, 0, 0, 0);

        private void ShowMoreCellColors()
        {
            this.moreColorbutton3.Focus();
            MoreCellColor.ShowDialog();
            this.moreColorbutton3.Focus();
        }
        private bool availableCellClr = false;
        private int xLoc = 0, yLoc = 0;
        private int r = 2, c = 0;
        /// <summary>
        /// Inzializes a new custom class which shows the custom color availabilty in the GridGrouping Control.
        /// </summary>
        private void InizilaizeMoreCellColors()
        {
            MoreCellColor = new MetroForm();
            //Componenet Declaration.
            this.moreColorlabel1 = new System.Windows.Forms.Label();
            this.moreColorNoFill = new System.Windows.Forms.Label();
            this.moreColorpanel1 = new System.Windows.Forms.Panel();
            this.moreColorlabel2 = new System.Windows.Forms.Label();
            this.moreColorSelectedColor = new System.Windows.Forms.Label();
            this.moreColorbutton3 = new System.Windows.Forms.Button();
            this.moreColorbutton4 = new System.Windows.Forms.Button();


            //GridControl-colorPicker Tool.            
            moreColorGridControl1 = new GridControl();
            this.moreColorGridControl1.Location = new System.Drawing.Point(13, 71);
            //this.moreColorGridControl1.Size = new System.Drawing.Size(290, 77);
            this.moreColorGridControl1.ShowColumnHeaders = false;
            this.moreColorGridControl1.ShowRowHeaders = false;
            int rowcount = this.filter.backColorCollection.Count / 10;
            int colcount;
            if (this.filter.backColorCollection.Count <= 8)
                colcount = this.filter.backColorCollection.Count * 2;
            else
                colcount = 3;
            this.moreColorGridControl1.ColCount = 17;
            this.moreColorGridControl1.RowCount = 3;
            this.moreColorGridControl1.DefaultColWidth = 15;
            this.moreColorGridControl1.DefaultRowHeight = 10;
            this.moreColorGridControl1.VScrollBehavior = GridScrollbarMode.Disabled;
            this.moreColorGridControl1.HScrollBehavior = GridScrollbarMode.Disabled;
            this.moreColorGridControl1.Model.Options.ResizeRowsBehavior = GridResizeCellsBehavior.None;
            this.moreColorGridControl1.Model.Options.ResizeColsBehavior = GridResizeCellsBehavior.None;
            this.moreColorGridControl1.Model.Options.DefaultGridBorderStyle = GridBorderStyle.None;
            this.moreColorGridControl1.ControllerOptions = ~GridControllerOptions.OleDataSource;
            this.moreColorGridControl1.AllowSelection = GridSelectionFlags.None;
            this.moreColorGridControl1.BackColor = Color.White;
            r = 2; c = 0;
            if (availableCellClr)//!isFontColorClicked)
            {
                foreach (Color color in this.filter.backColorCollection.Values)
                {
                    c += 2;
                    if (c > 16)
                    {
                        c = 0;
                        r += 2;
                        this.moreColorGridControl1.RowCount += 2;
                    }
                    this.moreColorGridControl1[r, c].BackColor = color;
                }
            }
            else
            {
                foreach (Color color in this.filter.fontColorCollection.Values)
                {
                    c += 2;
                    if (c > 16)
                    {
                        c = 0;
                        r += 2;
                        this.moreColorGridControl1.RowCount += 2;
                    }
                    this.moreColorGridControl1[r, c].BackColor = color;
                }
            }
            this.MoreCellColor.CaptionButtonColor = Color.White;
            this.moreColorGridControl1.BrowseOnly = true;
            this.moreColorGridControl1.CellMouseHoverEnter += new GridCellMouseEventHandler(moreColorGridControl1_CellMouseHoverEnter);
            this.moreColorGridControl1.CellMouseHoverLeave += new GridCellMouseEventHandler(moreColorGridControl1_CellMouseHoverLeave);
            this.moreColorGridControl1.CellClick += new GridCellClickEventHandler(moreColorGridControl1_CellClick);
            this.moreColorGridControl1.CurrentCellStartEditing += new CancelEventHandler(moreColorGridControl1_CurrentCellStartEditing);
            this.moreColorGridControl1.DrawCurrentCellBorder += new GridDrawCurrentCellBorderEventHandler(moreColorGridControl1_DrawCurrentCellBorder);
            this.moreColorGridControl1.MouseClick += new MouseEventHandler(moreColorGridControl1_MouseClick);
            xLoc = yLoc = 13;
            //
            //        
            // label1
            // 
            this.moreColorlabel1.AutoSize = true;
            this.moreColorlabel1.Location = new System.Drawing.Point(13, 13);
            this.moreColorlabel1.Name = "label1";
            this.moreColorlabel1.Size = new System.Drawing.Size(142, 13);
            this.moreColorlabel1.TabIndex = 0;
            this.moreColorlabel1.Text = "Select a cell color to filter by:";
            this.moreColorlabel1.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
            // 
            // button1
            // 
            this.moreColorNoFill.Location = new System.Drawing.Point(xLoc - 1, yLoc + 28);//12, 41);
            this.moreColorNoFill.Name = "button1";
            this.moreColorNoFill.BorderStyle = BorderStyle.FixedSingle;
            this.moreColorNoFill.BackColor = ColorTranslator.FromHtml("#F0F0F0");
            this.moreColorNoFill.Size = new System.Drawing.Size(260, 20);
            this.moreColorNoFill.TabIndex = 5;
            this.moreColorNoFill.Font = new Font("segeo ui", 8f);
            this.moreColorNoFill.Text = "No Fill";
            this.moreColorNoFill.TextAlign = ContentAlignment.MiddleCenter;
            this.moreColorNoFill.FlatStyle = FlatStyle.Flat;
            this.moreColorNoFill.Click += new EventHandler(moreColorNoFill_Click);
            if (r > 2)
            {
                this.moreColorGridControl1.RowCount = r + 1;
                xLoc = 13;
                yLoc += (r - 2) + 1 * this.moreColorGridControl1.RowHeights[r];
                this.moreColorGridControl1.Size = new System.Drawing.Size(290, 77 + (r - 2) + 1 * this.moreColorGridControl1.RowHeights[r]);
            }
            else
                this.moreColorGridControl1.Size = new System.Drawing.Size(290, 77);
            // 
            // label2
            // 
            this.moreColorlabel2.AutoSize = true;
            this.moreColorlabel2.Location = new System.Drawing.Point(xLoc - 1, yLoc + 111);//12, 124);
            this.moreColorlabel2.Name = "label2";
            this.moreColorlabel2.Size = new System.Drawing.Size(49, 13);
            this.moreColorlabel2.TabIndex = 3;
            this.moreColorlabel2.BackColor = ColorTranslator.FromHtml("#F0F0F0");
            this.moreColorlabel2.Font = new Font("segeo ui", 8.25f, FontStyle.Bold);
            this.moreColorlabel2.Text = "Selected:";
            // 
            // button2
            // 
            this.moreColorSelectedColor.Location = new System.Drawing.Point(xLoc + 69, yLoc + 106);//82, 119);
            this.moreColorSelectedColor.Name = "button2";
            this.moreColorSelectedColor.Size = new System.Drawing.Size(188, 20);
            this.moreColorSelectedColor.TabIndex = 4;
            this.moreColorSelectedColor.BorderStyle = BorderStyle.FixedSingle;
            this.moreColorSelectedColor.BackColor = ColorTranslator.FromHtml("#F0F0F0");
            if (this.moreColorGridControl1[2, 2].BackColor != null)
            {
                this.moreColorSelectedColor.BackColor = this.moreColorGridControl1[2, 2].BackColor;
            }
            this.moreColorSelectedColor.FlatStyle = FlatStyle.Flat;
            // 
            // button3
            // 
            this.moreColorbutton3.Location = new System.Drawing.Point(xLoc + 91, yLoc + 150);//104, 163);
            this.moreColorbutton3.Name = "button3";
            this.moreColorbutton3.Size = new System.Drawing.Size(75, 23);
            this.moreColorbutton3.TabIndex = 1;
            this.moreColorbutton3.Text = "Ok";
            this.moreColorbutton3.UseVisualStyleBackColor = true;
            this.moreColorbutton3.FlatStyle = FlatStyle.Flat;
            this.moreColorbutton3.Click += new EventHandler(moreColorbutton3_Click);
            // 
            // button4
            // 
            this.moreColorbutton4.Location = new System.Drawing.Point(xLoc + 184, yLoc + 150);//197, 163);
            this.moreColorbutton4.Name = "button4";
            this.moreColorbutton4.Size = new System.Drawing.Size(75, 23);
            this.moreColorbutton4.TabIndex = 6;
            this.moreColorbutton4.Text = "Cancel";
            this.moreColorbutton4.UseVisualStyleBackColor = true;
            this.moreColorbutton4.FlatStyle = FlatStyle.Flat;
            this.moreColorbutton4.Click += new EventHandler(moreColorbutton4_Click);
            // 
            // MoreCellColor
            // 
            MoreCellColor.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            MoreCellColor.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            MoreCellColor.ClientSize = new System.Drawing.Size(280, yLoc + 177);
            MoreCellColor.Controls.Add(this.moreColorbutton4);
            MoreCellColor.Controls.Add(this.moreColorbutton3);
            MoreCellColor.Controls.Add(this.moreColorSelectedColor);
            MoreCellColor.Controls.Add(this.moreColorlabel2);
            MoreCellColor.Controls.Add(this.moreColorNoFill);
            MoreCellColor.Controls.Add(this.moreColorlabel1);
            MoreCellColor.Controls.Add(moreColorGridControl1);
            MoreCellColor.Name = "MoreCellColor";
            MoreCellColor.Text = SR.GetString(SR.AvailableCellColors);
            MoreCellColor.MaximizeBox = false;
            MoreCellColor.MinimizeBox = false;
            MoreCellColor.BackColor = ColorTranslator.FromHtml("#F0F0F0");
            MoreCellColor.FormBorderStyle = FormBorderStyle.FixedSingle;
            MoreCellColor.CaptionBarColor = ColorTranslator.FromHtml("#70BFD5");
            MoreCellColor.BorderColor = ColorTranslator.FromHtml("#70BFD5");
            MoreCellColor.BorderThickness = 15;
            MoreCellColor.ShowIcon = false;
            MoreCellColor.HideControlboxHighlights();
            MoreCellColor.CaptionButtonHoverColor = Color.Maroon;
            MoreCellColor.HelpButton = false;
            this.MoreCellColor.Paint += new PaintEventHandler(MoreCellColor_Paint);
            if (colHover.Count > 0)
                colHover.Clear();
            colHover.Add(20);
            int n = c / 2;
            for (int i = 0; i < n; i ++)
            {
                if (c - (2 * i) > 0)
                    colHover.Add(c - (2 * i));
            }
        }
        List<int> colHover = new List<int>();
        void moreColorNoFill_Click(object sender, EventArgs e)
        {
            
        }

        void moreColorGridControl1_CellMouseHoverLeave(object sender, GridCellMouseEventArgs e)
        {
            Rectangle rect = this.moreColorGridControl1.RangeInfoToRectangle(GridRangeInfo.Cell(e.RowIndex, e.ColIndex));
            if (e.RowIndex % 2 == 0 && e.ColIndex % 2 == 0 && selectionBounds != rect)
            {
                if (!(e.RowIndex >= r) || e.RowIndex == r && colHover.Contains(e.ColIndex))
                {
                    using (Graphics g = this.moreColorGridControl1.CreateGraphics())
                    {
                        g.DrawRectangle(new Pen(this.moreColorGridControl1[e.RowIndex, e.ColIndex].BackColor, 1f), new Rectangle(rect.X, rect.Y, rect.Width, rect.Height));
                    }
                }
            }
        }

        void moreColorGridControl1_CellMouseHoverEnter(object sender, GridCellMouseEventArgs e)
        {
            if (e.RowIndex % 2 == 0 && e.ColIndex % 2 == 0)
            {
                Rectangle rect = this.moreColorGridControl1.RangeInfoToRectangle(GridRangeInfo.Cell(e.RowIndex, e.ColIndex));
                if (!(e.RowIndex >= r) || e.RowIndex == r && colHover.Contains(e.ColIndex))
                {
                    using (Graphics g = this.moreColorGridControl1.CreateGraphics())
                    {
                        g.DrawRectangle(new Pen(Color.Black, 1f), new Rectangle(rect.X, rect.Y, rect.Width, rect.Height));
                    }
                }
            }
        }

        void MoreCellColor_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Brushes.Gray), this.moreColorbutton3.Location.X - 90, this.moreColorbutton3.Location.Y - 13,
               this.moreColorbutton4.Location.X + 73, this.moreColorbutton4.Location.Y - 13);
        }

        void moreColorGridControl1_MouseClick(object sender, MouseEventArgs e)
        {
            this.moreColorGridControl1.Refresh();
        }

        void moreColorGridControl1_DrawCurrentCellBorder(object sender, GridDrawCurrentCellBorderEventArgs e)
        {            
            if (e.RowIndex % 2 == 0 && e.ColIndex % 2 == 0)
            {
                if (!(e.RowIndex >= r) || e.RowIndex == r && colHover.Contains(e.ColIndex))
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 3f), new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 1));
            }
            e.Cancel = true;
        }

        void moreColorbutton4_Click(object sender, EventArgs e)
        {
            //cancel
            MoreCellColor.Close();
        }
        private List<string> filteredByClrCol = new List<string>();
        Color clr = Color.White;

        void moreColorbutton3_Click(object sender, EventArgs e)
        {
            //ok
            clr = moreColorSelectedColor.BackColor;
            ColorFilter(clr);
            string filterName = GetFilterName(sty);
            filterLists.Add(filterName, ddUser.GetValues());
            if (!filteredByClrCol.Contains(filterName))
                filteredByClrCol.Add(filterName);
            if (!filteredColumnCollection.ContainsKey(filterName))
                filteredColumnCollection.Add(filterName, this.items);
            ggcGrid.TableModel.Refresh();
            this.MoreCellColor.Close();
            isClearFilterClicked = true;
        }

        void moreColorGridControl1_CurrentCellStartEditing(object sender, CancelEventArgs e)
        {
            //this.moreColorGridControl1.Refresh();
            e.Cancel = true;
        }

        void moreColorGridControl1_CellClick(object sender, GridCellClickEventArgs e)
        {
            if (this.moreColorGridControl1[e.RowIndex, e.ColIndex].BackColor != Color.White && (e.RowIndex % 2 == 0 && e.ColIndex % 2 == 0))
            {
                this.moreColorSelectedColor.BackColor = this.moreColorGridControl1[e.RowIndex, e.ColIndex].BackColor;
                selectedRow = e.RowIndex;
                selectedCol = e.ColIndex;
            }
            selectionBounds = this.moreColorGridControl1.RangeInfoToRectangle(GridRangeInfo.Cell(e.RowIndex, e.ColIndex));
        }

        #endregion

        #endregion

        #region " Excel Like SearchTextBox "

        /// <summary>
        /// used to modify the color of the searchlabel.
        /// </summary>
        /// <param name="sender">search label</param>
        /// <param name="e">event data</param>
        void searchLabel1_MouseLeave(object sender, EventArgs e)
        {
            if (this.ddUser.searchLabel1.BackColor == Color.Orange)
                this.ddUser.searchLabel1.BackColor = Color.Transparent;
        }

        /// <summary>
        /// used to modify the color of the searchlabel.
        /// </summary>
        /// <param name="sender">search label</param>
        /// <param name="e">event data</param>
        void searchLabel1_MouseHover(object sender, EventArgs e)
        {
            //if (this.ddUser.searchLabel1.Image == imgClose)
            //    this.ddUser.searchLabel1.BackColor = Color.Orange;
        }

        /// <summary>
        /// Corresponds to the search label image clicks.
        /// </summary>
        /// <param name="sender">search label</param>
        /// <param name="e">event data</param>
        void searchLabel1_Click(object sender, EventArgs e)
        {
            if (this.ddUser.searchBox1.Text.Length > 0)
            {
                this.ddUser.searchLabel1.BackColor = Color.Empty;
                this.ddUser.searchLabel1.Image = imgSearch;
                this.ddUser.searchBox1.Text = "";
            }
        }

        /// <summary>
        /// Add the filtered items to the checkedListBox when the filter popup is closed unanimously.
        /// </summary>
        /// <param name="sender">Control of OptimzedDropDownControl</param>
        /// <param name="e">event data</param>
        void filterDialog_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (filter.AllowSearch)
            {
                foreach (KeyValuePair<string, bool> itemCheck in removedCollection)
                {
                    if (!this.ddUser.checkedListBox1.Items.Contains(itemCheck.Key))
                        this.ddUser.checkedListBox1.Items.Add(itemCheck.Key, itemCheck.Value);
                }
            }
            if (this.filter.AllowResize)
            {
                this.ddUser.Width = popupDefaultSize.Width;
                this.ddUser.Height = popupDefaultSize.Height;
                this.filterDialog.ClientSize = new System.Drawing.Size(filterDialog.Width, filterDialog.Height);
                this.filterPopup.Resize -= new EventHandler(filterPopup_Resize);
                this.filterDialog.LostFocus -= new EventHandler(filterDialog_LostFocus);
                this.filterPopup.Close();
            }
        }

        /// <summary>
        /// Add the filtered items to the checkedListBox when the Ok button is triggered.
        /// </summary>
        /// <param name="sender">Control of OptimzedDropDownControl</param>
        /// <param name="e">event data</param>
        void okButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (filter.AllowSearch)
            {
                foreach (KeyValuePair<string, bool> itemCheck in removedCollection)
                {
                    if (!this.ddUser.checkedListBox1.Items.Contains(itemCheck.Key))
                        this.ddUser.checkedListBox1.Items.Add(itemCheck.Key, itemCheck.Value);
                }
            }
        }

        private Image imgSearch, imgClose;
        private int checkItems, previousLength = 0;
        private Dictionary<string, bool> entireCollection = new Dictionary<string, bool>();
        private Dictionary<string, bool> removedCollection = new Dictionary<string, bool>();
        private Dictionary<string, bool> finalCollection = new Dictionary<string, bool>();

        /// <summary>
        /// checkedListBox customized item population based on search via searchTextBox.
        /// </summary>
        /// <param name="sender">searchTextBox</param>
        /// <param name="e">event data</param>
        void searchBox1_TextChanged(object sender, EventArgs e)
        {
            if (filter.AllowSearch)
            {
                if (previousLength < this.ddUser.searchBox1.Text.Length)
                {
                    this.finalCollection.Clear();
                    this.checkedListBoxClear();
                }
                if (this.ddUser.searchBox1.Text.Length >= 1)
                {
                    this.ddUser.searchLabel1.Image = imgClose;
                    foreach (KeyValuePair<string, bool> itemCheck in entireCollection)
                    {
                        if (itemCheck.Key.ToString().ToUpper().Contains(this.ddUser.searchBox1.Text) || itemCheck.Key.ToString().ToLower().Contains(this.ddUser.searchBox1.Text))
                        {
                            if (!this.finalCollection.ContainsKey(itemCheck.Key))
                                this.finalCollection.Add(itemCheck.Key, itemCheck.Value);
                        }
                        else
                        {
                            if (!this.removedCollection.ContainsKey(itemCheck.Key))
                                this.removedCollection.Add(itemCheck.Key, itemCheck.Value);
                        }
                    }
                    this.checkedListBoxClear();
                }
                else if (previousLength != 0)
                {
                    this.ddUser.searchLabel1.Image = imgSearch;
                    foreach (KeyValuePair<string, bool> itemCheck in removedCollection)
                    {
                        if (!this.finalCollection.ContainsKey(itemCheck.Key))
                            this.finalCollection.Add(itemCheck.Key, itemCheck.Value);
                    }
                    this.removedCollection.Clear();
                    this.checkedListBoxClear();
                }
                this.sortingLogic();
                if (this.ddUser.checkedListBox1.Items.Count == 0 || (this.ddUser.checkedListBox1.Items.Count == 1 && this.ddUser.checkedListBox1.Items.Contains(SR.GetString(SR.SelectAll))))
                    this.ddUser.checkedListBox1.Items.Remove(SR.GetString(SR.SelectAll));
                previousLength = this.ddUser.searchBox1.Text.Length;
            }
        }

        /// <summary>
        /// Sorting customization for the elements to be added to the checekedListBox.
        /// </summary>
        private void sortingLogic()
        {
            if (finalCollection.Count >= 1)
            {
                //Array.Sort(this.finalCollection.Keys.ToArray());
                foreach (KeyValuePair<string, bool> sortElement in finalCollection)
                {
                    if (!this.ddUser.checkedListBox1.Items.Contains(sortElement.Key))
                        this.ddUser.checkedListBox1.Items.Add(sortElement.Key, sortElement.Value);
                }
            }
        }

        /// <summary>
        /// To clear and insert the selectall item to the checkedlist item collection.
        /// </summary>
        private void checkedListBoxClear()
        {
            this.ddUser.checkedListBox1.Items.Clear();
            if (!this.ddUser.checkedListBox1.Items.Contains(SR.GetString(SR.SelectAll)))
                this.ddUser.checkedListBox1.Items.Add(SR.GetString(SR.SelectAll));
        }

        #endregion


        void ddUser_MouseLeave(object sender, EventArgs e)
        {
            this.filterDialog.Focus();
        }



        void filterDialog_KeyDown(object sender, KeyEventArgs e)
        {
            Control ctl;
            ctl = (Control)sender;
            this.filterDialog.SelectNextControl(ctl, true, true, true, true);
        }

        /// <summary>
        /// Used to notify the current cell editing.
        /// </summary>
        /// <param name="sender">CurrentCell changes</param>
        /// <param name="e">event data</param>
        void Grid_CurrentCellAcceptedChanges(object sender, CancelEventArgs e)
        {
            isCurrentCellEdited = true;
            if (this.Grid.CurrentCell != null)
            {
                int colIndex = this.Grid.CurrentCell.ColIndex;
                int rowIndex = this.Grid.CurrentCell.RowIndex;
                string colName = string.Empty;
                GridTableCellStyleInfo sty = this.Grid.Model[rowIndex, colIndex] as GridTableCellStyleInfo;
                if (sty.TableCellIdentity != null)//&& sty.TableCellIdentity.TableCellType == GridTableCellType.RecordFieldCell)
                {
                    string format = sty.Format.Length == 0 ? "{0}" : string.Format("{{0:{0}}}", sty.Format);
                    GridTableDescriptor tableDes = sty.TableCellIdentity.Table.TableDescriptor;
                    colName = sty.TableCellIdentity.Column.Name;
                    string item = this.Grid.Model[rowIndex, colIndex].FormattedText;
                    BindEditedItemIntoCollections(colName, item, sty);
                }
            }
        }

        /// <summary>
        /// Used to modify the edited item in filtered collection.
        /// </summary>
        /// <param name="sender">Edited item</param>
        /// <param name="e">event data</param>
        void Grid_CurrentCellEditingComplete(object sender, EventArgs e)
        {           
            isCurrentCellEdited = false;
        }

        /// <summary>
        /// Performs filtering based on the checked items in the CheckedListBox.
        /// </summary>
        /// <param name="sender">Tool strip item</param>
        /// <param name="e">event data</param>
        private void CustomItemSelection_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem selectedItem = sender as ToolStripMenuItem;
            int index = -1;
            if (this.filterDialog.Items.Contains(this.textFilters))
            {
                index = this.textFilters.DropDownItems.IndexOf(selectedItem);
            }
            else if (this.filterDialog.Items.Contains(this.dateTimeFilters))
            {
                index = this.dateTimeFilters.DropDownItems.IndexOf(selectedItem);
                if (index == -1) 
                {
                    index = this.allDatesInThePeriod.DropDownItems.IndexOf(selectedItem);
                    isAlldatesSelected = true;
                    allDatesInThePeriod.Checked = true;
                }
            }
            else if (this.filterDialog.Items.Contains(this.numberFilter))
            {
                index = this.numberFilter.DropDownItems.IndexOf(selectedItem);
            }

            GridTableCellStyleInfo sty = this.Grid.Model[filterRow, Filtercol] as GridTableCellStyleInfo;
            GridTableDescriptor tableDes = sty.TableCellIdentity.Table.TableDescriptor;
            string columnName = sty.TableCellIdentity.Column.Name;
            string filterName = GetFilterName(sty);
            object[] uniqueList = null;
            if (this.filterDialog.Items.Contains(this.numberFilter))
            {
               uniqueList = GetNumericRecord(filterName, sty);
            }
            else
            {
                uniqueList = GetRecord(filterName, sty);
            }
            bool showFilterDialog = true;
            if (index != -1)
            {
                int colIndex = filterLists.ContainsKey(filterName) ? filterLists[filterName].IndexOf(filterName) : -1;
                if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                {
                    bool isContinue = true;
                    MetroCustomRowFilter filterOptions = new MetroCustomRowFilter(uniqueList);
                    if (this.filterDialog.Items.Contains(this.textFilters))
                    {
                        filterOptions.filterNumber = 0;
                        filterOptions.SetCombo(index);
                        filterOptions.GetComboTable();
                    }
                    else if (this.filterDialog.Items.Contains(this.dateTimeFilters))
                    {
                        filterOptions.filterNumber = 1;
                        filterOptions.SetDateCombo(index);
                        filterOptions.GetComboTable();
                    }
                    else if (this.filterDialog.Items.Contains(this.numberFilter))
                    {
                        filterOptions.filterNumber = 2;
                        List<double> numberList = new List<double>();
                        double d = 0;
                        double totalvalue = 0;
                        double average = 0;
                        filterOptions.SetNumberCombo(index);
                        filterOptions.GetComboTable();
                        if (index >= 9)
                        {
                            foreach (string s in uniqueList)
                            {
                                if (double.TryParse(s, out d))
                                {
                                    totalvalue += d;
                                    numberList.Add(d);
                                }
                            }
                            showFilterDialog = false;
                            numberList.Sort();
                            if (index == 9)
                            {
                                MetroTop10AutoFilter metroTop10filter = new MetroTop10AutoFilter(numberList);
                                metroTop10filter.MappingName = sty.TableCellIdentity.Column.MappingName;
                                metroTop10filter.Location = filterOptions.Location;
                                DialogResult metrotop10DialogResult = metroTop10filter.ShowDialog();
                                if (metrotop10DialogResult == DialogResult.OK)
                                {
                                    filterOptions.FilterString = metroTop10filter.FilterString;
                                    showFilterDialog = false;
                                }
                                else if (metrotop10DialogResult == DialogResult.Cancel)
                                {
                                    showFilterDialog = false;
                                    isContinue = false;
                                }
                            }
                            else if (index == 10)
                            {
                                filterOptions.FilterString = '[' + sty.TableCellIdentity.Column.MappingName + ']' + " > '" + average.ToString() + "'";
                            }
                            else if (index == 11)
                            {
                                filterOptions.FilterString = '[' + sty.TableCellIdentity.Column.MappingName + ']' + " < '" + average.ToString() + "'";
                            }
                            else if (index == 13)
                            {
                                showFilterDialog = true;
                            }
                        }
                        
                    }
                    
                    filterOptions.columnName = sty.TableCellIdentity.Column.HeaderText;
                    filterOptions.mappingName = sty.TableCellIdentity.Column.MappingName;
                    if ((this.filterDialog.Items.Contains(this.dateTimeFilters) && index > 5 && index !=30) || isAlldatesSelected)
                    {
                        filterOptions.SetFilterString(index, isAlldatesSelected);
                        showFilterDialog = false;
                    }
                    if (showFilterDialog && filterOptions.ShowDialog() == DialogResult.Cancel)
                    {
                        isContinue = false;
                    }
                    if (isContinue)
                    {
                        if (filterLists.ContainsKey(filterName))
                        {
                            ListPropertyChangedEventArgs remLce = new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, colIndex, filterName, filterName, filterName);
                            this.filter.OnRecordFiltersItemChanging(remLce);
                            if (!remLce.Cancel)
                            {
                                filterLists.Remove(filterName);
                                sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Remove(filterName);
                                this.filter.OnRecordFiltersItemChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, colIndex, filterName, filterName, filterName));
                            }
                        }

                        ListPropertyChangedEventArgs addLce = new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, colIndex, filterName, filterName, filterName);
                        this.filter.OnRecordFiltersItemChanging(addLce);
                        if (!addLce.Cancel)
                        {
                            RecordFilterDescriptor rfc = new RecordFilterDescriptor(filterName, filterOptions.FilterString);
                            sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Add(rfc);
                            if (filteredColumnCollection.ContainsKey(filterName))
                                filteredColumnCollection[filterName] = this.items;
                            else
                                filteredColumnCollection.Add(filterName, this.items);
                            if (!filterLists.ContainsKey(filterName))
                            {
                                filterLists.Add(filterName, GetFilterChoicesInStringList(GetFilterBarChoices(sty.TableCellIdentity)));
                                filterLists[filterName].Sort();
                            }

                            this.filter.OnRecordFiltersItemChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, colIndex, filterName, filterName, filterName));
                        }

                    }
                }
                else
                {
                    bool isContinue = true;
                    CustomRowFilter filterOptions = new CustomRowFilter(this.Grid.Model.Options.GridVisualStyles.ToString(),uniqueList);
                    if (this.filterDialog.Items.Contains(this.textFilters))
                    {
                        filterOptions.filterNumber = 0;
                        filterOptions.SetCombo(index);
                        filterOptions.GetComboTable();
                    }
                    else if (this.filterDialog.Items.Contains(this.dateTimeFilters))
                    {
                        filterOptions.filterNumber = 1;
                        filterOptions.SetDateCombo(index);
                        filterOptions.GetComboTable();
                    }
                    else if (this.filterDialog.Items.Contains(this.numberFilter))
                    {
                        filterOptions.filterNumber = 2;
                        List<double> numberList = new List<double>();
                        double d = 0;
                        filterOptions.SetNumberCombo(index);
                        filterOptions.GetComboTable();
                        double totalvalue = 0;
                        double average = 0;
                        if (index >= 9)
                        {
                            foreach (string s in uniqueList)
                            {
                                if (double.TryParse(s, out d))
                                {
                                    totalvalue += d;
                                    numberList.Add(d);
                                }
                            }
                            showFilterDialog = false;
                            numberList.Sort();
                            average = totalvalue / numberList.Count;
                            if (index == 9)
                            {
                                Top10AutoFilter top10filter = new Top10AutoFilter(numberList);
                                top10filter.MappingName = sty.TableCellIdentity.Column.MappingName;
                                showFilterDialog = false;
                                DialogResult top10DialogResult = top10filter.ShowDialog();
                                if (top10DialogResult == DialogResult.OK)
                                {
                                    filterOptions.FilterString = top10filter.FilterString;
                                }
                                else if (top10DialogResult == DialogResult.Cancel)
                                {
                                    isContinue = false;
                                }
                            }
                            else if (index == 10)
                            {
                                filterOptions.FilterString = '[' + sty.TableCellIdentity.Column.MappingName + ']' + " > '" + average.ToString() + "'";
                            }
                            else if (index == 11)
                            {
                                filterOptions.FilterString = '[' + sty.TableCellIdentity.Column.MappingName + ']' + " < '" + average.ToString() + "'";
                            }
                            else if (index == 13)
                            {
                                showFilterDialog = true;
                            }
                        }

                    }

                    filterOptions.columnName = sty.TableCellIdentity.Column.HeaderText;
                    filterOptions.mappingName = sty.TableCellIdentity.Column.MappingName;
                    if ((this.filterDialog.Items.Contains(this.dateTimeFilters) && index > 5 && index != 30) || isAlldatesSelected)
                    {
                        filterOptions.SetFilterString(index, isAlldatesSelected);
                        showFilterDialog = false;
                    }
                    if (showFilterDialog && filterOptions.ShowDialog() == DialogResult.Cancel)
                    {
                        isContinue = false;
                    }
                    if (isContinue)
                    {
                        ListPropertyChangedEventArgs lce = new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, colIndex, filterName, filterName, filterName);
                        this.filter.OnRecordFiltersItemChanging(lce);
                        if (!lce.Cancel)
                        {
                            if (filterLists.ContainsKey(filterName))
                            {
                                filterLists.Remove(filterName);
                            }
                            sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Remove(filterName);
                            this.filter.OnRecordFiltersItemChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, colIndex, filterName, filterName, filterName));
                        }

                        ListPropertyChangedEventArgs addLce = new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, colIndex, filterName, filterName, filterName);
                        this.filter.OnRecordFiltersItemChanging(addLce);
                        if (!addLce.Cancel)
                        {
                            RecordFilterDescriptor rfc = new RecordFilterDescriptor(filterName, filterOptions.FilterString);
                            sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Add(rfc);
                            if (filteredColumnCollection.ContainsKey(filterName))
                                filteredColumnCollection[filterName] = this.items;
                            else
                                filteredColumnCollection.Add(filterName, this.items);
                            if (!filterLists.ContainsKey(filterName))
                            {                                
                                filterLists.Add(filterName, GetFilterChoicesInStringList(GetFilterBarChoices(sty.TableCellIdentity)));
                                filterLists[filterName].Sort();
                            }
                            
                        }
                    }
                }
                if (index > -1)
                {
                    selectedItem.Checked = true;
                    if (this.filterDialog.Items.Contains(this.textFilters))
                    {
                        this.textFilters.Checked = true;
                        if (selectedTextFilterItem != null)
                            selectedTextFilterItem.Checked = false;
                        selectedTextFilterItem = selectedItem;
                    }
                    else if (this.filterDialog.Items.Contains(this.dateTimeFilters))
                    {
                        this.dateTimeFilters.Checked = true;
                        if (selectedDateFilterItem != null)
                            selectedDateFilterItem.Checked = false;
                        selectedDateFilterItem = selectedItem;
                    }
                    else if (this.filterDialog.Items.Contains(this.numberFilter))
                    {
                        this.numberFilter.Checked = true;
                        if (selectedNumberFilterItem != null)
                            selectedNumberFilterItem.Checked = false;
                        selectedNumberFilterItem = selectedItem;
                    }
                }
            }
    }

        /// <summary>
        /// Used to Modify the edited item into filttered collection.
        /// </summary>
        /// <param name="filterName">Filter column name</param>
        /// <param name="item">Current cell value</param>
        /// <param name="formatStyle">Cell style</param>
        internal void BindEditedItemIntoCollections(string filterName, string item, GridTableCellStyleInfo formatStyle)
        {
            List<string> filterListItemTemp = new List<string>();
            List<string> UnfilteredListItemTemp = new List<string>();
            List<DateTime> valuesDate = new List<DateTime>();
            List<Double> valuesDouble = new List<Double>();
            List<Decimal> valuesDecimal = new List<Decimal>();
            List<int> valuesInt = new List<int>();
            List<Int16> valuesInt16 = new List<Int16>();
            List<Int64> valuesInt64 = new List<Int64>();
            List<float> valuesfloat = new List<float>();

            List<DateTime> filteredCollectionDate = new List<DateTime>();
            List<Double> filteredCollectionDouble = new List<Double>();
            List<Decimal> filteredCollectionDecimal = new List<Decimal>();
            List<int> filteredCollectionInt = new List<int>();
            List<Int16> filteredCollectionInt16 = new List<Int16>();
            List<Int64> filteredCollectionInt64 = new List<Int64>();
            List<float> filteredCollectionfloat = new List<float>();
            List<string> recordList = new List<string>();
            List<string> filteredRecordList = new List<string>();
            recordList = GetRecordList(filterName, formatStyle);
            filteredRecordList = GetFilteredRecordList(filterName, formatStyle);
            bool blank = false;
            string format = formatStyle.Format.Length == 0 ? "{0}" : string.Format("{{0:{0}}}", formatStyle.Format);
            if (filterLists != null && filterLists.ContainsKey(filterName))
            {
                for (int i = 0; i < filteredRecordList.Count; i++)
                {
                    int intOutValue;
                    Int16 int16OutValue;
                    Int64 int64OutValue;
                    double doubleOutValue;
                    float floatOutValue;
                    DateTime dateTimeOutValue;
                    decimal decimalValue;
                    string filteredItem = filteredRecordList[i].ToString();                   
                    if (filteredRecordList.Contains(filteredItem))
                    {
                        if (string.IsNullOrEmpty(filteredItem))
                            blank = true;
                        else if (formatStyle.CellValueType == typeof(System.DateTime) && !string.IsNullOrEmpty(filteredItem)
                           && DateTime.TryParse(filteredItem, out dateTimeOutValue))
                        {                            
                            if (!valuesDate.Contains(dateTimeOutValue))
                            {
                                valuesDate.Add(dateTimeOutValue);
                                filteredCollectionDate.Add(dateTimeOutValue);
                            }
                        }
                        else if (formatStyle.CellValueType == typeof(double) && !string.IsNullOrEmpty(filteredItem)
                          && double.TryParse(filteredItem, out doubleOutValue) && !valuesDouble.Contains(doubleOutValue))
                        {                            
                            valuesDouble.Add(doubleOutValue);
                            filteredCollectionDouble.Add(doubleOutValue);
                        }
                        else if (formatStyle.CellValueType == typeof(decimal) && !string.IsNullOrEmpty(filteredItem)
                          && decimal.TryParse(filteredItem, out decimalValue) && !valuesDecimal.Contains(decimalValue))
                        {
                            valuesDecimal.Add(decimalValue);
                            filteredCollectionDecimal.Add(decimalValue);
                        }
                        else if (formatStyle.CellValueType == typeof(int) && !string.IsNullOrEmpty(filteredItem)
                         && int.TryParse(filteredItem, out intOutValue) && !valuesInt.Contains(intOutValue))
                        {
                            valuesInt.Add(intOutValue);
                            filteredCollectionInt.Add(intOutValue);
                        }
                        else if (formatStyle.CellValueType == typeof(System.Int16) && !string.IsNullOrEmpty(filteredItem)
                        && Int16.TryParse(filteredItem, out int16OutValue) && !valuesInt16.Contains(int16OutValue))
                        {
                            valuesInt16.Add(int16OutValue);
                            filteredCollectionInt16.Add(int16OutValue);
                        }
                        else if (formatStyle.CellValueType == typeof(System.Int64) && !string.IsNullOrEmpty(filteredItem)
                         && Int64.TryParse(filteredItem, out int64OutValue) && !valuesInt64.Contains(int64OutValue))
                        {
                            valuesInt64.Add(int64OutValue);
                            filteredCollectionInt64.Add(int64OutValue);
                        }
                        else if (formatStyle.CellValueType == typeof(float) && !string.IsNullOrEmpty(filteredItem)
                         && float.TryParse(filteredItem, out floatOutValue) && !valuesfloat.Contains(floatOutValue))
                        {
                            valuesfloat.Add(floatOutValue);
                            filteredCollectionfloat.Add(floatOutValue);
                        }
                        else 
                        {
                            if (!filterListItemTemp.Contains(filteredItem))
                            {
                                filterListItemTemp.Add(filteredItem);
                                if (!UnfilteredListItemTemp.Contains(filteredItem))
                                    UnfilteredListItemTemp.Add(filteredItem);
                            }
                        }
                    }
                }
                int intCurrentItemOutValue;
                Int16 int16CurrentItemOutValue;
                Int64 int64CurrentItemOutValue;
                double doubleCurrentItemOutValue;
                float floatCurrentItemOutValue;
                decimal decimalCurrentItemValue;
                DateTime dateTimeCurrentItemOutValue;

                if (string.IsNullOrEmpty(item))
                    blank = true;
                else if (formatStyle.CellValueType == typeof(System.DateTime) && !string.IsNullOrEmpty(item)
                  && DateTime.TryParse(item, out dateTimeCurrentItemOutValue))
                {
                    if (!valuesDate.Contains(dateTimeCurrentItemOutValue))
                    {
                        valuesDate.Add(dateTimeCurrentItemOutValue);
                        filteredCollectionDate.Add(dateTimeCurrentItemOutValue);
                    }
                }
                else if (formatStyle.CellValueType == typeof(double) && !string.IsNullOrEmpty(item)
                 && double.TryParse(item, out doubleCurrentItemOutValue) && !valuesDouble.Contains(doubleCurrentItemOutValue))
                {
                    valuesDouble.Add(doubleCurrentItemOutValue);
                    filteredCollectionDouble.Add(doubleCurrentItemOutValue);
                }
                else if (formatStyle.CellValueType == typeof(decimal) && !string.IsNullOrEmpty(item)
                  && decimal.TryParse(item, out decimalCurrentItemValue) && !valuesDecimal.Contains(decimalCurrentItemValue))
                {
                    valuesDecimal.Add(decimalCurrentItemValue);
                    filteredCollectionDecimal.Add(decimalCurrentItemValue);
                }
                else if (formatStyle.CellValueType == typeof(int) && !string.IsNullOrEmpty(item)
                  && int.TryParse(item, out intCurrentItemOutValue) && !valuesInt.Contains(intCurrentItemOutValue))
                {
                    valuesInt.Add(intCurrentItemOutValue);
                    filteredCollectionInt.Add(intCurrentItemOutValue);
                }
                else if (formatStyle.CellValueType == typeof(System.Int16) && !string.IsNullOrEmpty(item)
                 && Int16.TryParse(item, out int16CurrentItemOutValue) && !valuesInt16.Contains(int16CurrentItemOutValue))
                {
                    valuesInt16.Add(int16CurrentItemOutValue);
                    filteredCollectionInt16.Add(int16CurrentItemOutValue);
                }
                else if (formatStyle.CellValueType == typeof(System.Int64) && !string.IsNullOrEmpty(item)
                 && Int64.TryParse(item, out int64CurrentItemOutValue) && !valuesInt64.Contains(int64CurrentItemOutValue))
                {
                    valuesInt64.Add(int64CurrentItemOutValue);
                    filteredCollectionInt64.Add(int64CurrentItemOutValue);
                }
                else if (formatStyle.CellValueType == typeof(float) && !string.IsNullOrEmpty(item)
                 && float.TryParse(item, out floatCurrentItemOutValue) && !valuesfloat.Contains(floatCurrentItemOutValue))
                {
                    valuesfloat.Add(floatCurrentItemOutValue);
                    filteredCollectionfloat.Add(floatCurrentItemOutValue);
                }
                else 
                {
                    if (!filterListItemTemp.Contains(item))
                    {
                        filterListItemTemp.Add(item);
                        UnfilteredListItemTemp.Add(item);
                    }
                }

                if (formatStyle.CellValueType == typeof(DateTime))
                {
                    valuesDate.Sort();
                    foreach (object obj in valuesDate)
                    {
                        string s = string.Format(format, Convert.ToDateTime(obj.ToString()));
                        string s1 = string.Format(format, Convert.ToDateTime(s));
                        filterListItemTemp.Add(s1);
                    }
                }

                else if (formatStyle.CellValueType == typeof(Double))
                {
                    valuesDouble.Sort();
                    foreach (object obj in valuesDouble)
                    {
                        string s = string.Format(format, Convert.ToDouble(obj));
                        filterListItemTemp.Add(s.Trim());
                    }
                }

                else if (formatStyle.CellValueType == typeof(Decimal))
                {
                    valuesDecimal.Sort();
                    foreach (object obj in valuesDecimal)
                    {
                        string s = string.Format(format, Convert.ToDecimal(obj));
                        filterListItemTemp.Add(s.Trim());
                    }
                }
                else if (formatStyle.CellValueType == typeof(int))
                {
                    valuesInt.Sort();
                    foreach (object obj in valuesInt)
                    {
                        string s = string.Format(format, int.Parse(obj.ToString()));
                        filterListItemTemp.Add(s.Trim());
                    }
                }
                else if (formatStyle.CellValueType == typeof(Int16))
                {
                    valuesInt16.Sort();
                    foreach (object obj in valuesInt16)
                    {
                        string s = string.Format(format, Convert.ToInt16(obj.ToString()));
                        filterListItemTemp.Add(s.Trim());
                    }
                }
                else if (formatStyle.CellValueType == typeof(Int64))
                {
                    valuesInt64.Sort();
                    foreach (object obj in valuesInt64)
                    {
                        string s = string.Format(format, Convert.ToInt64(obj.ToString()));
                        filterListItemTemp.Add(s.Trim());
                    }
                }
                else if (formatStyle.CellValueType == typeof(float))
                {
                    valuesfloat.Sort();
                    foreach (object obj in valuesfloat)
                    {
                        string s = string.Format(format, float.Parse(obj.ToString()));
                        filterListItemTemp.Add(s.Trim());
                    }
                }
                else
                    filterListItemTemp.Sort();

                if (blank && !filterListItemTemp.Contains("(Blanks)"))
                    filterListItemTemp.Add("(Blanks)");

                if (filterLists.ContainsKey(filterName))
                    filterLists.Remove(filterName);
                if (filterListItemTemp.Count > 0)
                {
                    filterLists.Add(filterName, filterListItemTemp);
                    filterLists[filterName].Sort();
                }
            }
            else
                isCurrentCellEdited = false;
            if (filteredColumnCollection != null && filteredColumnCollection.ContainsKey(filterName))
            {
                object[] filteredColumnItem;

                if (filteredColumnCollection.TryGetValue(filterName, out filteredColumnItem))
                {
                    for (int i = 0; i < filteredColumnItem.Length; i++)
                    {
                        int intFilteredOutValue;
                        Int16 int16FilteredOutValue;
                        Int64 int64FilteredOutValue;
                        double doubleFilteredOutValue;
                        float floatFilteredOutValue;
                        decimal decimalFilteredOutValue;
                        DateTime dateTimeFilteredOutValue;
                        string recordItem = filteredColumnItem[i].ToString();
                        if (recordItem.Equals("Blanks"))
                            recordItem = string.Empty;
                        if (recordList.Contains(recordItem))
                        {
                            if (string.IsNullOrEmpty(recordItem))
                                blank = true;
                            else if (formatStyle.CellValueType == typeof(System.DateTime) && !string.IsNullOrEmpty(recordItem)
                              && DateTime.TryParse(recordItem, out dateTimeFilteredOutValue))
                            {
                                if (!filteredCollectionDate.Contains(dateTimeFilteredOutValue))
                                {
                                    filteredCollectionDate.Add(dateTimeFilteredOutValue);
                                }
                            }
                            else if (formatStyle.CellValueType == typeof(System.Double) && !string.IsNullOrEmpty(recordItem)
                              && Double.TryParse(recordItem, out doubleFilteredOutValue) && !filteredCollectionDouble.Contains(doubleFilteredOutValue))
                            {
                                filteredCollectionDouble.Add(doubleFilteredOutValue);
                            }
                            else if (formatStyle.CellValueType == typeof(System.Decimal) && !string.IsNullOrEmpty(recordItem)
                             && Decimal.TryParse(recordItem, out decimalFilteredOutValue) && !filteredCollectionDecimal.Contains(decimalFilteredOutValue))
                            {
                                filteredCollectionDecimal.Add(decimalFilteredOutValue);
                            }
                            else if (formatStyle.CellValueType == typeof(int) && !string.IsNullOrEmpty(recordItem)
                              && int.TryParse(recordItem, out intFilteredOutValue) && !filteredCollectionInt.Contains(intFilteredOutValue))
                            {
                                filteredCollectionInt.Add(intFilteredOutValue);
                            }
                            else if (formatStyle.CellValueType == typeof(System.Int16) && !string.IsNullOrEmpty(recordItem)
                             && Int16.TryParse(recordItem, out int16FilteredOutValue) && !filteredCollectionInt16.Contains(int16FilteredOutValue))
                            {
                                filteredCollectionInt16.Add(int16FilteredOutValue);
                            }
                            else if (formatStyle.CellValueType == typeof(System.Int64) && !string.IsNullOrEmpty(recordItem)
                             && Int64.TryParse(recordItem, out int64FilteredOutValue) && !filteredCollectionInt64.Contains(int64FilteredOutValue))
                            {
                                filteredCollectionInt64.Add(int64FilteredOutValue);
                            }
                            else if (formatStyle.CellValueType == typeof(float) && !string.IsNullOrEmpty(recordItem)
                            && float.TryParse(recordItem, out floatFilteredOutValue) && !filteredCollectionfloat.Contains(floatFilteredOutValue))
                            {
                                filteredCollectionfloat.Add(floatFilteredOutValue);
                            }
                            else 
                            {
                                if (!UnfilteredListItemTemp.Contains(recordItem))
                                    UnfilteredListItemTemp.Add(recordItem);
                            }
                        }
                    }
                    

                    if (formatStyle.CellValueType == typeof(DateTime))
                    {
                        filteredCollectionDate.Sort();
                        foreach (object obj in filteredCollectionDate)
                        {
                            string s = string.Format(format, Convert.ToDateTime(obj.ToString()));
                            string s1 = string.Format(format, Convert.ToDateTime(s));
                            UnfilteredListItemTemp.Add(s1);
                        }
                    }

                    else if (formatStyle.CellValueType == typeof(Double))
                    {
                        filteredCollectionDouble.Sort();
                        foreach (object obj in filteredCollectionDouble)
                        {
                            string s = string.Format(format, Convert.ToDouble(obj));
                            UnfilteredListItemTemp.Add(s.Trim());
                        }
                    }

                    else if (formatStyle.CellValueType == typeof(Decimal))
                    {
                        filteredCollectionDecimal.Sort();
                        foreach (object obj in filteredCollectionDecimal)
                        {
                            string s = string.Format(format, Convert.ToDecimal(obj));
                            UnfilteredListItemTemp.Add(s.Trim());
                        }
                    }
                    else if (formatStyle.CellValueType == typeof(int))
                    {
                        filteredCollectionInt.Sort();
                        foreach (object obj in filteredCollectionInt)
                        {
                            string s = string.Format(format, int.Parse(obj.ToString()));
                            UnfilteredListItemTemp.Add(s.Trim());
                        }
                    }
                    else if (formatStyle.CellValueType == typeof(Int16))
                    {
                        filteredCollectionInt16.Sort();
                        foreach (object obj in filteredCollectionInt16)
                        {
                            string s = string.Format(format, Convert.ToInt16(obj.ToString()));
                            UnfilteredListItemTemp.Add(s.Trim());
                        }
                    }
                    else if (formatStyle.CellValueType == typeof(Int64))
                    {
                        filteredCollectionInt64.Sort();
                        foreach (object obj in filteredCollectionInt64)
                        {
                            string s = string.Format(format, Convert.ToInt64(obj.ToString()));
                            UnfilteredListItemTemp.Add(s.Trim());
                        }
                    }
                    else if (formatStyle.CellValueType == typeof(float))
                    {
                        filteredCollectionfloat.Sort();
                        foreach (object obj in filteredCollectionfloat)
                        {
                            string s = string.Format(format, float.Parse(obj.ToString()));
                            UnfilteredListItemTemp.Add(s);
                        }
                    }
                    else
                        UnfilteredListItemTemp.Sort();

                    if (blank && !UnfilteredListItemTemp.Contains("(Blanks)"))
                        UnfilteredListItemTemp.Add("(Blanks)");
                }
            }
            if (filteredColumnCollection.ContainsKey(filterName))
                filteredColumnCollection.Remove(filterName);
            if (UnfilteredListItemTemp.Count > 0)
                filteredColumnCollection.Add(filterName, UnfilteredListItemTemp.ToArray());
        }

        /// <summary>
        /// Gets record list collections.
        /// </summary>
        /// <param name="filterName">Filter column name</param>
        /// <param name="sty">Cell Style</param>
        /// <returns>Formated string list collection</returns>
        internal List<string> GetRecordList(string filterName, GridTableCellStyleInfo sty)
        {            
            List<string> list = new List<string>();
            foreach (Record rec in sty.TableCellIdentity.Table.Records)
            {
                if (rec.GetValue(filterName) != null)
                {
                    string recordItem = GetFormattedString(sty, rec.GetValue(filterName).ToString());
                    if (!list.Contains(recordItem))
                        list.Add(recordItem);
                }
            }
            return list;
        }

        /// <summary>
        /// Gets filtered record collection list.
        /// </summary>
        /// <param name="filterName">Filter column name</param>
        /// <param name="sty">Cell Style</param>
        /// <returns>Formated string list collection</returns>
        internal List<string> GetFilteredRecordList(string filterName, GridTableCellStyleInfo sty)
        {
            List<string> list = new List<string>();           
            foreach (Record rec in sty.TableCellIdentity.Table.FilteredRecords)
            {
                if (rec.GetValue(filterName) != null)
                {
                    string filteredRecord = GetFormattedString(sty, rec.GetValue(filterName).ToString());
                    if (!list.Contains(filteredRecord))
                        list.Add(filteredRecord);
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the formatted string.
        /// </summary>
        /// <param name="sty">Cell style</param>
        /// <param name="str">Cell value</param>
        /// <returns>Formated String</returns>
        internal string GetFormattedString(GridTableCellStyleInfo sty, string str)
        {
            string format = sty.Format.Length == 0 ? "{0}" : string.Format("{{0:{0}}}", sty.Format);
            int intOutValue;
            Int32 int32OutValue;
            Int64 int64OutValue;
            double doubleOutValue;
            float floatOutValue;
            decimal decimalOutValue;
            DateTime dateTimeOutValue;
            if (sty.CellValueType == typeof(int) && !string.IsNullOrEmpty(str)
                && int.TryParse(str, out intOutValue))
            {
                str = string.Format(format, intOutValue);
            }
            else if (sty.CellValueType == typeof(Int32) && !string.IsNullOrEmpty(str)
                && Int32.TryParse(str, out int32OutValue))
            {
                str = string.Format(format, int32OutValue);
            }
            else if (sty.CellValueType == typeof(Int64) && !string.IsNullOrEmpty(str)
                && Int64.TryParse(str, out int64OutValue))
            {
                str = string.Format(format, int64OutValue);
            }
            else if (sty.CellValueType == typeof(decimal) && !string.IsNullOrEmpty(str)
                && decimal.TryParse(str, out decimalOutValue))
            {
                str = string.Format(format, decimalOutValue);
            }
            else if (sty.CellValueType == typeof(double) && !string.IsNullOrEmpty(str)
                && double.TryParse(str, out doubleOutValue))
            {
                str = string.Format(format, doubleOutValue);
            }
            else if (sty.CellValueType == typeof(float) && !string.IsNullOrEmpty(str)
           && float.TryParse(str, out floatOutValue))
            {
                str = string.Format(format, floatOutValue);
            }
            else if (sty.CellValueType == typeof(DateTime) && !string.IsNullOrEmpty(str)
                && DateTime.TryParse(str, out dateTimeOutValue))
            {
                str = string.Format(format, dateTimeOutValue);
            }
            return str;
        }       

        /// <summary>
        /// Gets all the record collections.
        /// </summary>
        /// <param name="colName">Filtered Column Name</param>
        /// <param name="sty">Cell style</param>
        /// <returns>Record collections</returns>
        private object[] GetRecord(string colName, GridTableCellStyleInfo sty)
        {
            List<string> choicesOrdered = new List<string>();
            List<string> choices = new List<string>();
            string format = (sty.Format != null && sty.Format.Length > 0)
                    ? string.Format("{{0:{0}}}", sty.Format) : "{0}";

            foreach (Record o in sty.TableCellIdentity.Table.Records)
            {
                string val = string.Format(format, o.GetValue(colName));
                int loc = choicesOrdered.BinarySearch(val);
                if (loc < 0)
                {
                    choicesOrdered.Insert(-loc - 1, val);
                    choices.Add(val);
                }
            }
            choices.Sort();
            return choices.ToArray();
        }

        /// <summary>
        /// Gets all the record collections.
        /// </summary>
        /// <param name="colName">Filtered Column Name</param>
        /// <param name="sty">Cell style</param>
        /// <returns>Record collections</returns>
        private object[] GetNumericRecord(string colName, GridTableCellStyleInfo sty)
        {
            List<string> choicesOrdered = new List<string>();
            List<double> choices = new List<double>();
            List<string> choicestoString = new List<string>();
            string format = (sty.Format != null && sty.Format.Length > 0)
                    ? string.Format("{{0:{0}}}", sty.Format) : "{0}";

            foreach (Record o in sty.TableCellIdentity.Table.Records)
            {
                string val = string.Format(format, o.GetValue(colName));
                int loc = choicesOrdered.BinarySearch(val);
                if (loc < 0)
                {
                    choicesOrdered.Insert(-loc - 1, val);
                    double dVal = 0;
                    if (double.TryParse(val, out dVal))
                        choices.Add(dVal);
                }
            }
            choices.Sort();
            foreach (double dou in choices)
            {
                choicestoString.Add(dou.ToString());
            }
            return choicestoString.ToArray();
        }

        GridTableDescriptor tableDes;
        GridTableCellStyleInfo colStlye;
        /// <summary>
        /// Used internally
        /// </summary>
        /// <param name="items">An object array of items</param>
        /// <returns>List of string type</returns>
        private List<string> GetFilterChoicesInStringList(object[] items)
        {
            List<string> stringFilterList = new List<string>();
            foreach (object item in items)
            {
                stringFilterList.Add(item.ToString());
            }
            return stringFilterList;
        }

        /// <summary>
        /// Performs sorting and filtering based on the ToolStrip item's selection.
        /// </summary>
        /// <param name="sender">ToolStrip item</param>
        /// <param name="e">event date</param>
        private void ItemSelection_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem selectedItem = sender as ToolStripMenuItem;
            int index = this.filterDialog.Items.IndexOf(selectedItem);
            colStlye = this.Grid.Model[filterRow, Filtercol] as GridTableCellStyleInfo;
            tableDes = colStlye.TableCellIdentity.Table.TableDescriptor;
            string columnName = colStlye.TableCellIdentity.Column.Name;
            string filterName = GetFilterName(sty);

            if (this.filterDialog.Items.Contains(this.textFilters) && selectedTextFilterItem != null)
            {
                ((ToolStripMenuItem)this.selectedTextFilterItem.OwnerItem).Checked = false;
                selectedTextFilterItem.Checked = false;

            }
            else if (this.filterDialog.Items.Contains(this.dateTimeFilters) && selectedDateFilterItem != null)
            {
                if (isAlldatesSelected)
                    ((ToolStripMenuItem)((ToolStripMenuItem)this.selectedDateFilterItem.OwnerItem).OwnerItem).Checked = false;
                ((ToolStripMenuItem)this.selectedDateFilterItem.OwnerItem).Checked = false;
                selectedDateFilterItem.Checked = false;
            }
            else if (this.filterDialog.Items.Contains(this.numberFilter) && selectedNumberFilterItem != null)
            {
                ((ToolStripMenuItem)this.selectedNumberFilterItem.OwnerItem).Checked = false;
                selectedNumberFilterItem.Checked = false;
            }
            switch (index)
            {
                case 0:
                    this.sortDesc.Checked = false;
                    if (this.sortAsc.Checked)
                    {
                        tableDes.SortedColumns.Clear();
                        tableDes.SortedColumns.Add(columnName);
                    }
                    else
                    {
                        tableDes.SortedColumns.Remove(columnName);
                    }
                    break;
                case 1:
                    this.sortAsc.Checked = false;
                    if (this.sortDesc.Checked)
                    {
                        tableDes.SortedColumns.Clear();
                        tableDes.SortedColumns.Add(columnName, ListSortDirection.Descending);
                    }
                    else
                    {
                        tableDes.SortedColumns.Remove(columnName);
                    }
                    break;
                case 3:
                    if (filterLists.ContainsKey(filterName))
                    {
                        filterLists.Remove(filterName);
                        colStlye.TableCellIdentity.Table.TableDirty = true;
                        this.Grid.Refresh();
                    }
                    if (filteredColumnCollection.ContainsKey(filterName))
                        filteredColumnCollection.Remove(filterName);
                    colStlye.TableCellIdentity.Table.TableDescriptor.RecordFilters.Remove(filterName);
                    break;
            }

        }
        GridTableCellStyleInfo sty;
        string columnName;
        void filterDialog_Opening(object sender, CancelEventArgs e)
        {
            sty = this.Grid.Model[filterRow, Filtercol] as GridTableCellStyleInfo;
            GridTableDescriptor tableDes = sty.TableCellIdentity.Table.TableDescriptor;
            columnName = sty.TableCellIdentity.Column.Name;
            if (tableDes.SortedColumns.Contains(columnName))
            {
                if (tableDes.SortedColumns[columnName].SortDirection == ListSortDirection.Ascending)
                {
                    sortAsc.Checked = true;
                    sortDesc.Checked = false;
                }
                else if (tableDes.SortedColumns[columnName].SortDirection == ListSortDirection.Descending)
                {
                    sortAsc.Checked = false;
                    sortDesc.Checked = true;
                }
            }
            else if (sortAsc.Checked || sortDesc.Checked)
            {
                sortAsc.Checked = false;
                sortDesc.Checked = false;
            }


            if (filter.AllowSearch)
            {
                checkItems = previousLength = 0;
                this.entireCollection.Clear();
                this.removedCollection.Clear();
                this.ddUser.searchLabel1.Image = imgSearch;
                foreach (string item in items)
                {
                    entireCollection.Add(item, this.ddUser.checkedListBox1.GetItemChecked(checkItems + 1));
                    if (!this.ddUser.checkedListBox1.Items.Contains(item))
                        this.ddUser.checkedListBox1.Items.Add(item, this.ddUser.checkedListBox1.GetItemChecked(checkItems + 1));
                    checkItems++;
                }
            }
            this.ddUser.searchBox1.Clear();

            //COLOR ENHANCEMENT//
            this.filter.ColorCollection(columnName);
            this.ExcelLikeColorFilter();
            if (this.filter.AllowFilterByColor)
            {
                if (this.filter.fontColorCollection.Count <= 1 || this.filter.backColorCollection.Count <= 1)
                {
                    this.filterByColor.Enabled = false;
                }
                else if (this.filter.uniqueColorCollections.Count > 0)
                {
                    if (filtercleared)
                    {
                        this.SelectedRowIndex = 0;
                        filtercleared = false;
                    }
                    this.filterByColor.Enabled = true;
                    if (this.filterPopupGrid != null)
                        this.filterPopupGrid.backColorCollection.Clear();
                    foreach (Color clr in this.filter.backColorCollection.Values)
                    {
                        this.filterPopupGrid.backColorCollection.Add(clr);
                    }
                    //this.filterPopupGrid.backColorCollection = this.filter.backColorCollection.Values.ToList();
                }
            }

        }


        /// <summary>
        /// Customization for sortbycolor and filterbycolor.
        /// </summary>
        private void ExcelLikeColorFilter()
        {
            if (this.filter.AllowFilterByColor)
                FilterByColorControl();
        }

        ContextGrid filterPopupGrid;
        /// <summary>
        /// GridExcelFilter FilterByColor customization.
        /// </summary>
        private void FilterByColorControl()
        {
            if (this.filterByColor.DropDownItems.Contains(filterHost))
                this.filterByColor.DropDownItems.Remove(filterHost);

            filterControl = new UserControl();
            filterControl.Size = new System.Drawing.Size(110, 50);

            List<Color> backclr = new List<Color>(), fontclr = new List<Color>();
            foreach (Color clr in this.filter.backColorCollection.Values)
            {
                backclr.Add(clr);
            }
            foreach (Color clr in this.filter.fontColorCollection.Values)
            {
                fontclr.Add(clr);
            }
            filterPopupGrid = new ContextGrid(backclr, fontclr);
            filterPopupGrid.SelectedRowIndex = selectedRowIndex;
            //filterPopupGrid = new ContextGrid(this.filter.backColorCollection.Values.ToList(), this.filter.fontColorCollection.Values.ToList());
            filterPopupGrid.Size = new Size(147, this.filterPopupGrid.RowCount * this.filterPopupGrid.DefaultRowHeight );
            filterControl.Controls.Add(filterPopupGrid);

            filterHost = new ToolStripControlHost(filterControl);
            filterHost.Margin = new Padding(-7, -2, filterHost.Margin.Right - 7, filterHost.Margin.Bottom - 3);
            this.filterByColor.DropDownItems.Add(filterHost);
            this.filterPopupGrid.CellClick += new GridCellClickEventHandler(filterPopupGrid_CellClick);
            this.filterPopupGrid.MouseHover += new EventHandler(filterPopupGrid_MouseHover);
            this.filterPopupGrid.MouseLeave += new EventHandler(filterPopupGrid_MouseLeave);
        }
        private bool isFilterGridHovering = false;
        void filterPopupGrid_MouseLeave(object sender, EventArgs e)
        {
            this.isFilterGridHovering = false;
        }

        void filterPopupGrid_MouseHover(object sender, EventArgs e)
        {
            this.isFilterGridHovering = true;
        }
        private bool filterPopupClicked = false;


        void filterPopupGrid_CellClick(object sender, GridCellClickEventArgs e)
        {
            isFilterGridHovering = false;
            if (e.RowIndex > 1)
            {
                isFilterByColorWired = true;
                if (isFilterByColorWired)
                {
                    this.filterDialog.Close();
                    this.filterPopup.Close();
                    isColorFilterWired = true;
                    isFilterByColorWired = false;
                    if (ggcGrid != null && !isClearFilterClicked)
                    {
                        ggcGrid.TableModel.QueryRowHeight -= new GridRowColSizeEventHandler(TableModel_QueryRowHeight);
                        ggcGrid.TableModel.Refresh();
                        ggcGrid.TableModel.QueryRowHeight += new GridRowColSizeEventHandler(TableModel_QueryRowHeight);
                    }
                    if (isClearFilterClicked)
                        isClearFilterClicked = false;
                }
                filterPopupClicked = true;
                int i = 0;
                Color clr = Color.White;
                string filterName = GetFilterName(sty);
                if (e.RowIndex >= this.filterPopupGrid.cellColorStartIndex + 1 && e.RowIndex < this.filterPopupGrid.cellColorLastIndex + 1)
                {
                    selectedRowIndex = e.RowIndex;
                    isFontColorClicked = !(isCellColorClicked = true);
                    foreach (Color k in this.filter.backColorCollection.Values)
                    {
                        if (i == e.RowIndex - 2)
                        {
                            clr = k;
                            break;
                        }
                        i++;
                    }
                    ColorFilter(clr);
                    if (!filteredByClrCol.Contains(filterName))
                        filteredByClrCol.Add(filterName);
                    filterLists.Add(filterName, ddUser.GetValues());
                }
                else if (e.RowIndex >= this.filterPopupGrid.fontColorStartIndex + 1 && e.RowIndex < this.filterPopupGrid.fontColorLastIndex + 1)
                {
                    selectedRowIndex = e.RowIndex;
                    isCellColorClicked = !(isFontColorClicked = true);
                    foreach (Color k in this.filter.fontColorCollection.Values)
                    {
                        if (i == e.RowIndex - this.filterPopupGrid.fontColorStartIndex - 1)
                        {
                            clr = k;
                            break;
                        }
                        i++;
                    }
                    ColorFilter(clr);
                    if (!filteredByClrCol.Contains(filterName))
                        filteredByClrCol.Add(filterName);
                    filterLists.Add(filterName, ddUser.GetValues());
                }
                if (this.filterPopupGrid.Model[e.RowIndex, e.ColIndex].Text == SR.GetString(SR.MoreCellColors) || this.filterPopupGrid.Model[e.RowIndex, e.ColIndex].Text == SR.GetString(SR.MoreFontColors))
                {
                    if (this.filterPopupGrid.Model[e.RowIndex, e.ColIndex].Text == SR.GetString(SR.MoreFontColors))
                    {
                        availableCellClr = false;
                        isFontColorClicked = true;
                    }
                    else
                    {
                        isFontColorClicked = false;
                        availableCellClr = true;
                    }
                    InizilaizeMoreCellColors();
                    if (this.filterPopupGrid.Model[e.RowIndex, e.ColIndex].Text == SR.GetString(SR.MoreFontColors))
                        MoreCellColor.Text = SR.GetString(SR.AvailableFontColors);
                    else
                        MoreCellColor.Text = SR.GetString(SR.AvailableCellColors);
                    ShowMoreCellColors();
                    this.moreColorbutton3.Focus();
                }
                if (!filteredColumnCollection.ContainsKey(filterName))
                    filteredColumnCollection.Add(filterName, this.items);
                ggcGrid.TableModel.Refresh();
            }
        }
        private int selectedRowIndex = 0;
        private int SelectedRowIndex
        {
            get
            {
                return selectedRowIndex;
            }
            set
            {
                selectedRowIndex = value;
            }
        }

        #region " Color DropDown ContextGrid "

        /// <summary>
        /// Designer grid for showcashing color sorting and filtering option available in gridexcelfilter. 
        /// </summary>
        internal class ContextGrid : GridControl
        {

            #region 'Variable declaration'

            private int rowCountCalculation = 0;
            internal int fontColorStartIndex = 0;
            internal int cellColorStartIndex = 0;
            internal int fontColorLastIndex = 0;
            internal int cellColorLastIndex = 0;
            private bool hasMoreColorLabel = false;
            private string cellColorLabel = SR.GetString(SR.FilterByCellColor);
            private string fontColorLabel = SR.GetString(SR.FilterByFontColor);
            internal List<Color> backColorCollection = new List<Color>();
            internal List<Color> fontColorCollection = new List<Color>();

            #endregion

            #region 'Constructor'

            /// <summary>
            /// contextgrid inizialization.
            /// </summary>
            /// <param name="backColor">list of backcolor collection</param>
            /// <param name="fontColor">list of fontcolor collection</param>
            public ContextGrid(List<Color> backColor, List<Color> fontColor)
            {
                this.ShowRowHeaders = false;
                this.ShowColumnHeaders = false;
                this.VScrollBehavior = GridScrollbarMode.Disabled;
                this.HScrollBehavior = GridScrollbarMode.Disabled;
                this.Model.Options.DefaultGridBorderStyle = GridBorderStyle.None;
                this.Model.Options.ResizeRowsBehavior = GridResizeCellsBehavior.None;
                this.Model.Options.ResizeColsBehavior = GridResizeCellsBehavior.None;
                this.BaseStylesMap.Standard.StyleInfo.Font.Size = 9f;
                this.BaseStylesMap.Standard.StyleInfo.Font.Facename = "Segoe UI";
                this.BaseStylesMap.Standard.StyleInfo.TextColor = Color.FromArgb(94, 94, 94);
                this.BaseStylesMap.Standard.StyleInfo.VerticalAlignment = GridVerticalAlignment.Middle;

                this.RowCount = 16;
                this.ColCount = 02;
                this.DefaultRowHeight = 25;
                this.ColWidths[0] = 0;
                this.ColWidths[1] = 25;
                this.ColWidths[2] = 120;

                //Header label cell type.
                GridStyleInfo style = new GridStyleInfo();
                style.Font.Bold = true;
                style.CellType = GridCellTypeName.Static;
                style.BackColor = Color.FromArgb(238, 238, 238);
                style.TextColor = Color.FromArgb(119, 119, 119);

                this.backColorCollection = backColor;
                this.fontColorCollection = fontColor;

                if (this.backColorCollection.Count > 1)
                {
                    //Filter by Cell Color label.
                    rowCountCalculation += 1;
                    this.CoveredRanges.Add(GridRangeInfo.Cells(rowCountCalculation, 1, rowCountCalculation, 2));
                    this[rowCountCalculation, 1] = style;
                    this[rowCountCalculation, 1].Text = CellColorLabel;

                    this.cellColorStartIndex = rowCountCalculation;
                    if (this.backColorCollection.Count > 5)
                    {
                        hasMoreColorLabel = true;
                        this.rowCountCalculation += 5;//CellColor label.
                    }
                    else
                        this.rowCountCalculation += this.backColorCollection.Count;

                    this.cellColorLastIndex = rowCountCalculation;
                    if (this.backColorCollection.Contains(Color.White))
                    {
                        this.rowCountCalculation += 1;//No Fill.
                        this[rowCountCalculation, 2].Text = SR.GetString(SR.NoFill);
                    }
                    if (hasMoreColorLabel)
                    {
                        hasMoreColorLabel = false;
                        this.rowCountCalculation += 1;//More Cells label.
                        this[rowCountCalculation, 2].Text = SR.GetString(SR.MoreCellColors);
                    }
                }

                if (this.fontColorCollection.Count > 1)
                {
                    //Filter by Font Color label.
                    rowCountCalculation += 1;
                    this.CoveredRanges.Add(GridRangeInfo.Cells(rowCountCalculation, 1, rowCountCalculation, 2));
                    this[rowCountCalculation, 1] = style;
                    this[rowCountCalculation, 1].Text = FontColorLabel;

                    this.fontColorStartIndex = rowCountCalculation;
                    if (this.fontColorCollection.Count > 5)
                    {
                        hasMoreColorLabel = true;
                        this.rowCountCalculation += 5;//FontColor label.
                    }
                    else
                        this.rowCountCalculation += this.fontColorCollection.Count;

                    this.fontColorLastIndex = rowCountCalculation;
                    if (this.fontColorCollection.Contains(Color.Black))
                    {
                        this.rowCountCalculation += 1;//Automatic.
                        this[rowCountCalculation, 2].Text = SR.GetString(SR.Automatic);
                    }
                    if (hasMoreColorLabel)
                    {
                        hasMoreColorLabel = false;
                        this.rowCountCalculation += 1;//More Cells label.
                        this[rowCountCalculation, 2].Text = SR.GetString(SR.MoreFontColors);
                    }
                }

                this.RowCount = rowCountCalculation;
                this.ColCount = 2;
                this.DefaultRowHeight = 25;
                this.ColWidths[0] = 0;
                this.ColWidths[1] = 25;
                this.ColWidths[2] = 120;
            }

            #endregion

            #region 'Properties'

            public string CellColorLabel
            {
                get
                {
                    return cellColorLabel;
                }
                set
                {
                    cellColorLabel = value;
                }
            }
            public string FontColorLabel
            {
                get
                {
                    return fontColorLabel;
                }
                set
                {
                    fontColorLabel = value;
                }
            }
            private int SelectedIndex = 0;
            public int SelectedRowIndex
            {
                get
                {
                    return SelectedIndex;
                }
                set
                {
                    SelectedIndex = value;
                }
            }

            #endregion

            # region 'Check box'

            protected Bitmap CheckButton
            {
                get
                {
                    Bitmap bitmap;
                    Rectangle rcFlash = new Rectangle(0, 0, 10, 14);
                    bitmap = new Bitmap(rcFlash.Width, rcFlash.Height);

                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.Transparent);

                        Point[] points = new Point[]
                                         {
                                                new Point(1,8), 
                                                new Point(4,12),
                                                new Point(11,1)
                                         };

                        using (GraphicsPath path = new GraphicsPath())
                            path.AddLines(points);

                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        Pen pen = new Pen(Color.MidnightBlue, 2f);
                        g.DrawLines(pen, points);
                    }
                    return bitmap;
                }
            }

            #endregion

            #region 'Events customization'

            /// <summary>
            /// used to draw the cell and font color to the contextgrid cells along with check box.
            /// </summary>
            /// <param name="e">event data</param>
            protected override void OnDrawCell(GridDrawCellEventArgs e)
            {
                if (SelectedIndex != 0 && e.RowIndex == SelectedIndex && e.ColIndex == 1)
                {
                    Rectangle rects = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 1, e.Bounds.Height - 2);
                    Rectangle rc = rects;
                    if (rc.Width > 0 && rc.Height > 0)
                    {
                        Size szImage = this.CheckButton.Size;

                        int iImageX = rc.X + (rc.Width - szImage.Width) / 2;
                        int iImageY = rc.Y + (rc.Height - szImage.Height) / 2;
                        e.Graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#E9F5EE")), rects);
                        e.Graphics.DrawImage(this.CheckButton, new Point(iImageX, iImageY));
                    }
                    e.Graphics.DrawRectangle(new Pen(ColorTranslator.FromHtml("#86BFA0")), rects);
                }

                Pen pen = new Pen(Color.Gray);
                Rectangle rect = new Rectangle(e.Bounds.X + 8, e.Bounds.Y + 4, 51, e.Bounds.Height - 10);
                //cell Color.

                if (e.RowIndex > this.cellColorStartIndex && e.RowIndex <= cellColorLastIndex && e.ColIndex == 2)
                {
                    e.Renderer.StyleInfo.TextColor = backColorCollection[e.RowIndex - this.cellColorStartIndex - 1];
                    e.Graphics.FillRectangle(new SolidBrush(backColorCollection[e.RowIndex - this.cellColorStartIndex - 1]), rect);
                    e.Graphics.DrawRectangle(pen, rect);
                }
                //font color.
                if (e.RowIndex > this.fontColorStartIndex && e.RowIndex <= this.fontColorLastIndex && e.ColIndex == 2)
                {
                    e.Renderer.StyleInfo.TextColor = fontColorCollection[e.RowIndex - this.fontColorStartIndex - 1];
                    e.Graphics.FillRectangle(new SolidBrush(this.fontColorCollection[e.RowIndex - this.fontColorStartIndex - 1]), rect);
                    e.Graphics.DrawRectangle(pen, rect);
                }

                base.OnDrawCell(e);
            }

            protected override void OnCellMouseHoverEnter(GridCellMouseEventArgs e)
            {
                this.RowStyles[e.RowIndex].BackColor = Color.FromArgb(211, 240, 224);
                base.OnCellMouseHoverEnter(e);
            }

            protected override void OnCellMouseHoverLeave(GridCellMouseEventArgs e)
            {
                this.RowStyles[e.RowIndex].BackColor = Color.Empty;
                base.OnCellMouseHoverLeave(e);
            }

            protected override void OnCurrentCellActivating(GridCurrentCellActivatingEventArgs e)
            {
                //  e.Cancel = true;
                base.OnCurrentCellActivating(e);
            }

            protected override void OnCurrentCellStartEditing(CancelEventArgs e)
            {
                e.Cancel = true;
                base.OnCurrentCellStartEditing(e);
            }

            protected override void OnDrawCurrentCellBorder(GridDrawCurrentCellBorderEventArgs e)
            {
                e.Cancel = true;
                base.OnDrawCurrentCellBorder(e);
            }

            #endregion

        }

        #endregion

        #endregion

        #region GridExcelFilter control events
        /// <summary>
        /// Hides the filterdialog.
        /// </summary>
        /// <param name="sender">user control</param>
        /// <param name="e">event data</param>
        private void ddUser_UserControlCancel(object sender, EventArgs e)
        {
            this.filterDialog.Hide();
        }
        /// <summary>
        /// Indicates whether the GroupingGrid is hooked with QueryRecordMeetsFilterCriteria event or not
        /// </summary>
        private bool hooked = false;

        /// <summary>
        /// Dictionary for filter choices.
        /// </summary>
        Dictionary<string, List<string>> filterLists = null;

        private Dictionary<string, List<string>> filterCollection = null;
        /// <summary>
        /// Dictionary for filter choices to be exposed.
        /// </summary>
        internal Dictionary<string, List<string>> FilterCollection
        {
            get
            {
                return filterCollection;
            }
            set
            {
                GridTableCellStyleInfo sty = null;
                GridGroupingControl groupingGrid = ((GridTableModel)Model.Grid).Table.Engine.ParentControl as GridGroupingControl;
                filterCollection = value;
                filterLists = filterCollection;
                foreach (string filter in filterLists.Keys)
                {
                    int filteringcolindex = groupingGrid.TableModel.NameToColIndex(filter);
                    sty = this.Grid.Model[1, filteringcolindex] as GridTableCellStyleInfo;
                    object[] tempCollection = GetFilterBarChoices(sty.TableCellIdentity);
                    if (!filteredColumnCollection.ContainsKey(filter))
                        filteredColumnCollection.Add(filter, tempCollection);
                    filterLists[filter].Sort();
                }
                if (!hooked)
                {
                    hooked = true;
                    groupingGrid.QueryRecordMeetsFilterCriteria += new QueryRecordMeetsFilterCriteriaEventHandler(ggc_QueryRecordMeetsFilterCriteria);
                }
                sty.TableCellIdentity.Table.TableDirty = true;
                Grid.Refresh();
            }
        }
        /// <summary>
        /// Used to get the present filtered column name.
        /// </summary>
        private string filteredColumn = String.Empty;

        private object[] oldItems = null;
		private object[] items = null;
        /// <summary>
        /// Performs filtering and hides the dialog when Ok button clicked.
        /// </summary>
        /// <param name="sender">User control</param>
        /// <param name="e">event data</param>
        private void ddUser_UserControlSave(object sender, EventArgs e)
        {
            if (this.isFilterByColorWired)
            {
                isFilterByColorWired = false;
                isColorFilterWired = false;
                ggcGrid.TableModel.QueryRowHeight -= new GridRowColSizeEventHandler(TableModel_QueryRowHeight);
                ggcGrid.TableModel.Refresh();
            }
            if (filteredByClrCol.Count > 0)
            {
                filterLists.Remove(filteredByClrCol[0]);
                filteredByClrCol.Clear();
                ggcGrid.TableModel.QueryRowHeight -= new GridRowColSizeEventHandler(TableModel_QueryRowHeight);
                ggcGrid.TableModel.Refresh();
            }
            GridTableCellStyleInfo sty = this.Grid.Model[filterRow, Filtercol] as GridTableCellStyleInfo;
            string filterName = GetFilterName(sty);
            if (ddUser.GetValues().Count >= 0)
            {
                OptimizedFilterDropDownControl opc = sender as OptimizedFilterDropDownControl;
                int index = filterLists.ContainsKey(filterName) ? filterLists[filterName].IndexOf(filterName) : -1;
                if (!filterLists.ContainsKey(filterName) && opc.checkedListBox1.GetItemCheckState(0) != CheckState.Checked)
                {
                    ListPropertyChangedEventArgs lce = new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, filterName, filterName, filterName);
                    this.filter.OnRecordFiltersItemChanging(lce);
                    if (!lce.Cancel)
                    {
                        filterLists.Add(filterName, ddUser.GetValues());
                        if (!filteredColumnCollection.ContainsKey(filterName))
                            filteredColumnCollection.Add(filterName, this.items);
                        filterLists[filterName].Sort();
                        this.filter.OnRecordFiltersItemChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, filterName, filterName, filterName));
                    }
                }
                else
                {
                    ListPropertyChangedEventArgs lce = new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, filterName, filterName, filterName);
                    this.filter.OnRecordFiltersItemChanging(lce);
                    if (!lce.Cancel)
                    {                        
                        filterLists[filterName] = ddUser.GetValues();
                        if (filteredColumnCollection.ContainsKey(filterName))
                        {
                            filteredColumnCollection.Remove(filterName);
                            if (opc.checkedListBox1.GetItemCheckState(0) == CheckState.Indeterminate)
                                filteredColumnCollection.Add(filterName, this.items);
                        }
                        filterLists[filterName].Sort();
                        if (sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Contains(filterName))
                            sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Remove(filterName);
                        this.filter.OnRecordFiltersItemChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, filterName, filterName, filterName));
                    }

                    if (opc.checkedListBox1.GetItemCheckState(0) == CheckState.Checked)
                    {
                        ListPropertyChangedEventArgs inslce = new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, filterName, filterName, filterName);
                        this.filter.OnRecordFiltersItemChanging(inslce);
                        if (!inslce.Cancel)
                        {                            
                            filterLists.Remove(filterName);
                            this.filter.OnRecordFiltersItemChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, filterName, filterName, filterName));
                        }
                    }
                }
            }

            if (!hooked)
            {
                hooked = true;
                GridGroupingControl groupingGrid = ((GridTableModel)Model.Grid).Table.Engine.ParentControl as GridGroupingControl;
                groupingGrid.QueryRecordMeetsFilterCriteria += new QueryRecordMeetsFilterCriteriaEventHandler(ggc_QueryRecordMeetsFilterCriteria);
            }

            Cursor.Current = Cursors.WaitCursor;
            sty.TableCellIdentity.Table.TableDirty = true;
            this.Grid.Refresh();
            Cursor.Current = Cursors.Default;
            filterCollection = filterLists;
            filteredColumn = filterName;
            oldItems = this.items;            
            this.filterDialog.Hide();
        }
        string filterColumnName;
        /// <summary>
        /// Occurs when a record is checked whether it meets filter criteria and should appear visible in the tables DisplayElements.
        /// </summary>
        /// <param name="sender">grid</param>
        /// <param name="e">event data</param>
        private void ggc_QueryRecordMeetsFilterCriteria(object sender, QueryRecordMeetsFilterCriteriaEventArgs e)
        {
            e.Handled = true;
            e.Result = true;
            string name=string.Empty;
            GridTableDescriptor tableDescriptor = null;
            if (e.Record.ParentChildTable != null)
            {
                name = e.Record.ParentChildTable.Name;
                tableDescriptor = e.Record.ParentChildTable.ParentTableDescriptor as GridTableDescriptor;
            }

            foreach (string filterName in filterLists.Keys)
            {
                if (!isCurrentCellEdited && tableDescriptor!=null)
                {
                    if (tableDescriptor.Columns.Contains(filterName))
                    {
                        string cellFormat = tableDescriptor.Columns[filterName].Appearance.AnyRecordFieldCell.Format;
                        string format = cellFormat.Length == 0 ? "{0}" : string.Format("{{0:{0}}}", cellFormat);
                        filterColumnName = filterName;
                    List<string> thelist = filterLists[filterName];                    
                    if (thelist == null || thelist.Count == 0)
                    {
                        continue;
                        }
                        string val = string.Empty;
                        DateTime recDate;
                        if (tableDescriptor.Columns[filterName].Appearance.AnyRecordFieldCell.CellValueType != null &&
                            tableDescriptor.Columns[filterName].Appearance.AnyRecordFieldCell.CellValueType.Equals(typeof(DateTime)) && DateTime.TryParse(e.Record.GetValue(filterName).ToString(), out recDate))
                            val = string.Format(format, recDate);
                        else
                            val = string.Format(format, e.Record.GetValue(filterName));
                        if (string.IsNullOrEmpty(val))
                            val = "(Blanks)";                       
                        int loc = thelist.BinarySearch(val);
                        e.Result = loc > -1;
                        if (!e.Result)
                        {
                            break;
                        }
                    }
                }
                else
                    e.Handled = false;
            }
            if (tableDescriptor!=null && e.Result && tableDescriptor.RecordFilters.Count > 0)
                e.Handled = false; //if grid also has record filters (from custom filters) check those too.
        }

        #endregion

        #region CellOverriden Methods
        /// <summary>
        /// Disposes the groupingGrid and its events.
        /// </summary>
        /// <param name="disposing">the boolean flag</param>
        protected override void Dispose(bool disposing)
        {
            GridGroupingControl groupingGrid = ((GridTableModel)Model.Grid).Table.Engine.ParentControl as GridGroupingControl;
            if (hooked && groupingGrid != null)
            {
                hooked = !hooked;
                groupingGrid.QueryRecordMeetsFilterCriteria -= new QueryRecordMeetsFilterCriteriaEventHandler(ggc_QueryRecordMeetsFilterCriteria);
            }

            if (Grid != null)
                this.Grid.CellClick -= new GridCellClickEventHandler(Grid_CellClick);

            base.Dispose(disposing);
        }
        /// <summary>
        /// Returns the exact image of the bitmap name.
        /// </summary>
        /// <param name="bitmapName">name of the bitmap</param>
        /// <returns>The image</returns>
        private string FindImageFile(string bitmapName)
        {
            string bitmappath = "";
            for (int n = 0; n < 10; n++)
            {
                if (System.IO.File.Exists(bitmapName))
                    bitmappath = bitmapName;

                bitmapName = @"..\" + bitmapName;
            }
            return bitmappath;
        }
        /// <summary>
        /// Occurs when mouse hover enters the column header.
        /// </summary>
        /// <param name="rowIndex">row index</param>
        /// <param name="colIndex">col index</param>
        protected override void OnMouseHoverEnter(int rowIndex, int colIndex)
        {
            IsMouseOver = true;
            colindex = colIndex;
            this.Grid.InvalidateRange(GridRangeInfo.Row(rowIndex));
            base.OnMouseHoverEnter(rowIndex, colIndex);
        }
        /// <summary>
        /// Occurs when mouse hover leaves the column header.
        /// </summary>
        /// <param name="rowIndex">row index</param>
        /// <param name="colIndex">col index</param>
        /// <param name="e"></param>
        protected override void OnMouseHoverLeave(int rowIndex, int colIndex, EventArgs e)
        {
            IsMouseOver = false;
            colindex = colIndex;
            this.Grid.InvalidateRange(GridRangeInfo.Row(rowIndex));
            base.OnMouseHoverLeave(rowIndex, colIndex, e);
        }

        /// <summary>
        /// Is triggered for all the cells and 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="textRectangle"></param>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <param name="style"></param>
        protected override void OnDrawDisplayText(Graphics g, Rectangle textRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            object tag = style.Tag;
            if (Grid.PrintingMode || !(tag is ListSortDirection))
            {
                tag = null;
            }

            ListSortDirection listSortDirection = ListSortDirection.Ascending;
            int margin = filterWidth;
            if (tag != null)
            {
                listSortDirection = (ListSortDirection)tag;
                margin = filterWidth + 12;
            }

            bool isTextRightToLeft = Grid.SortIconPlacement == SortIconPlacement.Left;
            bool isTextTop = Grid.SortIconPlacement == SortIconPlacement.Top;
            if (isTextRightToLeft || Grid.IsRightToLeft() || style.RightToLeft == RightToLeft.Yes)
            {
                GridUtil.OffsetLeft(ref textRectangle, margin);
            }
            else if (isTextTop)
            {
                GridUtil.OffsetTop(ref textRectangle, 0);
            }
            else
            {
                textRectangle.Width -= margin;
            }

            base.OnDrawDisplayText(g, textRectangle, rowIndex, colIndex, style);
           
            #region ImageRendering
            Rectangle imageRect;

            if (Grid.IsRightToLeft() || isTextRightToLeft || style.RightToLeft == RightToLeft.Yes)
            {
                imageRect = new Rectangle(textRectangle.Left - filterWidth, textRectangle.Y + (textRectangle.Height / 6), filterWidth, filterHeight);
            }
            else
            {
                imageRect = new Rectangle(textRectangle.Right, textRectangle.Y + (textRectangle.Height / 6), filterWidth, filterHeight);
            }
            GridTableCellStyleInfo tablestyle = style as GridTableCellStyleInfo;
            Image imag_gray = (Image)filterImages["Filter_gray"];
            Image clImage_gray = (Image)filterImages["ClearFilter_gray"];
            Image imag_white = (Image)filterImages["Filter_white"];
            Image clImage_white = (Image)filterImages["ClearFilter_white"];

            if (tablestyle.TableCellIdentity.Column != null && tablestyle.TableCellIdentity.Column.AllowFilter == true)
            {
                GridTableDescriptor tableDesc = ((GridTableControl)this.Grid).TableDescriptor;
                bool isFiltered = false;
                if (filterLists.ContainsKey(tablestyle.TableCellIdentity.Column.Name)
                    /*&& GridOffice2007Filter.EnableFilteredColumnIcon*/)
                {
                    isFiltered = true;
                }

                if (GridExcelFilter.EnableFilteredColumnIcon)
                {
                    if (IsMouseOver && colindex == colIndex)
                    {
                        filterRect = imageRect;
                    }
                    if(IsMouseOver && colindex == colIndex && !isFiltered)
                    {
                        if (Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black || Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                            g.DrawImage(imag_white, imageRect);
                        else
                            g.DrawImage(imag_gray, imageRect);
                    }
                    
                }
                else
                {
                    if (IsMouseOver && colindex == colIndex)
                    {
                        filterRect = imageRect;
                    }

                    if (!isFiltered)
                    {
                        if (Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black || Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                            g.DrawImage(imag_white, imageRect);
                        else
                            g.DrawImage(imag_gray, imageRect);
                    }
                }
                if (GridExcelFilter.EnableFilteredColumnIcon || isFiltered)
                {
                    foreach (string filterName in filterLists.Keys)
                    {
                        if (IsMouseOver && colindex == colIndex)
                        {
                            filterRect = imageRect;
                        }
                        if (filterName == tablestyle.TableCellIdentity.Column.Name)
                        {
                            if (Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black || Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                                g.DrawImage(clImage_white, imageRect);
                            else
                                g.DrawImage(clImage_gray, imageRect);
                            isFiltered = false;
                        }
                    }
                }
                
            }

            #endregion

            if (tag != null)
            {
                string s = style.ValueMember; 
                int dig = (!string.IsNullOrEmpty(s) && s.Length > 1) ? 2 : 1;

                Rectangle rect;
                if (isTextRightToLeft)
                {
                    rect = new Rectangle(textRectangle.Left - margin, textRectangle.Y, 10, textRectangle.Height);
                }
                else if (isTextTop)
                {
                    rect = new Rectangle(textRectangle.X, textRectangle.Y - 8, textRectangle.Width, textRectangle.Height);
                }
                else
                {
                    rect = new Rectangle(textRectangle.Right + filterWidth, textRectangle.Y, 10, textRectangle.Height);
                }

                rect = GridUtil.CenterInRect(rect, new Size(8 * dig, 8));

                Brush brush = null;
                Pen pen1 = null;
                this.Grid.Model.Options.GridVisualStylesDrawing.GetSortIconBrush(out brush, out pen1);
                if (s != string.Empty)
                {
                    rect.Offset(-4 * (dig - 1), -4);
                    Font numFont = new Font(style.Font.Facename, 6, style.Font.FontStyle);
                    if (isTextTop)
                        DrawText(g, s, numFont, new Rectangle(rect.X + 5, rect.Y + 5, rect.Width, rect.Height), style, pen1.Color, Grid.IsRightToLeft());
                    else
                        DrawText(g, s, numFont, rect, style, pen1.Color, Grid.IsRightToLeft());
                    numFont.Dispose();
                    rect.Offset(4 * (dig - 1), 4);

                    if (isTextRightToLeft || Grid.IsRightToLeft())
                    {
                        rect.Offset(6, 0);
                    }
                    else
                    {
                        rect.Offset(-6, 0);
                    }
                }

                int i2 = Math.Max(0, (rect.Height - 6) / 2);
                rect.Inflate(-i2, -i2);
                GridTriangleDirection triangleDirection = listSortDirection == ListSortDirection.Ascending ? GridTriangleDirection.Up : GridTriangleDirection.Down;
                GridPaintTriangle.Paint(g, rect, triangleDirection, brush, pen1, true);
                pen1.Dispose();
                brush.Dispose();
            }
        }
        /// <summary>
        /// Is triggered when the cells are drawn
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="clientRectangle">Rectangle</param>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="style">GridStyleInfo</param>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
            Color clrBottom = Color.Empty;
            Color clrRight = Color.Empty;
            Color clrInteriorFirst = Color.Empty;
            Color clrInteriorLast = Color.Empty;
            if (this.Grid.Model.Options.GridVisualStylesDrawing.GetHeaderBorderColors(out clrBottom, out clrRight, out clrInteriorFirst, out clrInteriorLast) &&
               ((style.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.Grid.ThemesEnabled)
                      || ((style.Themed && this.Grid.ThemesEnabled && ((this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme))))))
            {
                Rectangle rect1 = clientRectangle;
                Pen br = new Pen(clrBottom);
                g.DrawLine(br, new PointF(rect1.X, rect1.Y), new PointF((rect1.X + rect1.Width), rect1.Y));
                br.Dispose();
            }
        }

        /// <summary>
        ///  Occurs when the user clicks inside the  cell boundary.
        /// </summary>
        /// <param name="sender">grid</param>
        /// <param name="e">event data</param>
        private void Grid_CellClick(object sender, GridCellClickEventArgs e)
        {
            Rectangle filterRectPadding = new Rectangle(filterRect.X - 1, filterRect.Y - 1, filterRect.Width + 1, filterRect.Height + 1);
            if (filterRectPadding.IntersectsWith(new Rectangle(e.MouseEventArgs.X, e.MouseEventArgs.Y, 0, 0)))
            {
                GridRangeInfo range = GridRangeInfo.Cell(e.RowIndex, e.ColIndex);
                Rectangle rec = this.Grid.RangeInfoToRectangle(range);
                Point loc = new Point(rec.Right, rec.Bottom);
                Point locn = new Point(e.MouseEventArgs.X, e.MouseEventArgs.Y);
                Point postion = this.Grid.PointToScreen(locn);
                Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
                if (this.Grid.CurrentCell != null)
                {
                    this.Grid.CurrentCell.Deactivate(true);
                }
                Size BottomRect = new Size(screenBounds.Width - postion.X, screenBounds.Height - postion.Y);
                if (BottomRect.Width < filterDialog.Width)
                {
                    locn.X = e.MouseEventArgs.X - filterDialog.Width;
                }
                if (postion.X > screenBounds.Width)
                {
                    locn.X = e.MouseEventArgs.X;
                }

                GridTableControl grid = this.Grid as GridTableControl;
                filterRow = e.RowIndex;
                Filtercol = e.ColIndex;
                GridTableCellStyleInfo sty = this.Grid.Model[filterRow, Filtercol] as GridTableCellStyleInfo;

                clearFilter.Text = clearFilterString + " " + sty.TableCellIdentity.Column.HeaderText;

                string filterName = GetFilterName(sty);
                if (filteredColumnCollection.ContainsKey(filterName))
                {
                    filteredColumnCollection.TryGetValue(filterName, out items);
                }
                else
                    items = GetFilterBarChoices(sty.TableCellIdentity);
                if ((filterLists != null && filterLists.ContainsKey(filterName)) || sty.TableCellIdentity.Table.TableDescriptor.RecordFilters.Contains(filterName))                                   
                    clearFilter.Enabled = true;
                else
                    clearFilter.Enabled = false;
                IList filteredValues = this.GetFilterTextList(sty);
                GridTableCellStyleInfo formatStyle = new GridTableCellStyleInfo();
                formatStyle = sty;
                formatStyle.CellValueType = sty.TableCellIdentity.Column.Appearance.AnyRecordFieldCell.CellValueType;
                formatStyle.CellValue = sty.CellValue;
                formatStyle.Format = sty.TableCellIdentity.Column.Appearance.AnyRecordFieldCell.Format;
                int i = 4;
                if (this.filterByColor != null)
                {
                    if (this.filterDialog.Items.Contains(this.filterByColor))
                    {
                        i++;
                    }
                }
                if (filter.EnableDateFilter && formatStyle.CellValueType == (typeof(DateTime)))
                {
                    this.filterDialog.Items.RemoveAt(i);
                    this.sortAsc.Text = SR.GetString(SR.SortOldesttoNewest);
                    this.sortDesc.Text = SR.GetString(SR.SortNewesttoOldest);
                    this.filterDialog.Items.Insert(i, dateTimeFilters);
                }
                else if (filter.EnableNumberFilter && IsNumericType(formatStyle.CellValueType))
                {
                    this.filterDialog.Items.RemoveAt(i);
                    this.sortAsc.Text = SR.GetString(SR.SortSmallesttoLargest);
                    this.sortDesc.Text = SR.GetString(SR.SortLargesttoSmallest);
                    this.filterDialog.Items.Insert(i, numberFilter);
                }
                else
                {
                    this.filterDialog.Items.RemoveAt(i);
                    this.sortAsc.Text = SR.GetString(SR.SortAtoZ);
                    this.sortDesc.Text = SR.GetString(SR.SortZtoA);
                    this.filterDialog.Items.Insert(i, textFilters);
                }

                if (sty.GetActiveGridView() != null && sty.GetActiveGridView().GridOfficeScrollBars == Syncfusion.Windows.Forms.OfficeScrollBars.Metro)
                {
                    this.filterDialog.Font = new Font("Segoe UI", 8.25f);
                    this.filterDialog.ForeColor = Color.FromArgb(51, 51, 51);
                    this.ddUser.checkedListBox1.BorderStyle = BorderStyle.FixedSingle;
                }
                WireButton(this.ddUser.okButton);
                WireButton(this.ddUser.cancelButton);
                if (!filteredColumnCollection.ContainsKey(filterName))
                    items = ConvertItemCellType(items, formatStyle);
                if (this.filterDialog.Items.Contains(this.dateTimeFilters))
                {
                    this.filterDialog.SuspendLayout();
                    this.ddUser.SetDateTimeItems(items, filteredValues, formatStyle, this.Grid.GetGridVisualStyles(), filterLists.Count);
                    this.filterDialog.ResumeLayout();
                }
                else
                    this.ddUser.SetItems(items, filteredValues, formatStyle);
                this.ddUser.firstTab = true;
                if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro && !this.filter.AllowResize)
                {
                    this.ddUser.checkedListBox1.BorderStyle = BorderStyle.None;
                    this.ddUser.Paint += new PaintEventHandler(ddUser_Paint);
                }
                else
                {
                    this.ddUser.checkedListBox1.BorderStyle = BorderStyle.FixedSingle;
                }

                if (this.filter.AllowFilterByColor)
                    ((ToolStripDropDownMenu)this.filterByColor.DropDown).ShowImageMargin = false;
                if (this.filter.AllowResize)
                {
                    this.filterDialog.TopLevel = false;
                    this.filterPopup = null;
                    if (sizeInitialized)
                    {
                        filterdialogSize = this.filterDialog.Size;
                        sizeInitialized = false;
                    }
                    if (this.filterPopup == null)
                        this.filterPopup = new GridExcelFilterPopup(this.filterDialog, filterdialogSize);

                    this.filterPopup.AutoClose = false;
                    this.filterPopup.Resize += new EventHandler(filterPopup_Resize);
                    this.filterDialog.LostFocus += new EventHandler(filterDialog_LostFocus);
                    this.filterPopup.Show(grid, locn);
                }
                else
                {
                    this.ddUser.checkedListBox1.Size = new System.Drawing.Size(185, 154);                   
                    this.filterDialog.Show(grid, locn);
                }
                this.filterDialog.Items[0].Select();
                e.Cancel = true;
            }
        }


        private bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        private Size popupDefaultSize, filterdialogSize;
        private bool sizeInitialized = true;
        /// <summary>
        /// used to draw the border for checkedListbox. 
        /// </summary>
        /// <param name="sender">ddUser</param>
        /// <param name="e">event data</param>
        void ddUser_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(this.ddUser.checkedListBox1.Bounds.X - 1, this.ddUser.checkedListBox1.Bounds.Y - 1,
                this.ddUser.checkedListBox1.Bounds.Width + 2, this.ddUser.checkedListBox1.Bounds.Height + 2);
            e.Graphics.DrawRectangle(new Pen(ColorTranslator.FromHtml("#E2E4E7"), 1f), rect);
        }
        /// <summary>
        /// To close the filterPopup when the filterpopup is lost focus.
        /// </summary>
        /// <param name="sender">filterdialog</param>
        /// <param name="e">event data</param>
        void filterDialog_LostFocus(object sender, EventArgs e)
        {
            if (this.filter.AllowResize)
            {
                if (!this.ddUser.ContainsFocus && !this.isFilterGridHovering)
                {
                    this.filterDialog.Close();
                    this.filterPopup.Close();
                }
            }
        }

        /// <summary>
        /// To resize the filterdialog based on the parent filterpopup bounds.
        /// </summary>
        /// <param name="sender">filterPopup</param>
        /// <param name="e">event data</param>
        void filterPopup_Resize(object sender, EventArgs e)
        {
            this.ddUser.Width = this.filterPopup.ClientRectangle.Width - 60;
            this.ddUser.Height = this.filterPopup.Height - 140;
        }

        /// <summary>
        /// Convertion to respective type in grid table.
        /// </summary>
        /// <param name="itemCollections">Collection of the filtered items</param>
        /// <param name="formatStyle">Cell style</param>
        /// <returns>Collection of the converted filtered items</returns>
        internal object[] ConvertItemCellType(object[] itemCollections, GridTableCellStyleInfo formatStyle)
        {
            object[] item = null;
            List<string> valuesString = new List<string>();
            List<string> values = new List<string>();
            List<DateTime> valuesDate = new List<DateTime>();
            List<Double> valuesDouble = new List<Double>();
            List<Decimal> valuesDecimal = new List<Decimal>();
            List<int> valuesInt = new List<int>();
            List<Int16> valuesInt16 = new List<Int16>();
            List<Int64> valuesInt64 = new List<Int64>();
            List<float> valuesfloat = new List<float>();
            bool blank = false;
            string format = formatStyle.Format.Length == 0 ? "{0}" : string.Format("{{0:{0}}}", formatStyle.Format);
            foreach (object it in itemCollections)
            {
                int intOutValue;
                Int16 int16OutValue;
                Int64 int64OutValue;
                double doubleOutValue;
                float floatOutValue;
                decimal decimalOutValue;
                DateTime dateTimeOutValue; 
               
                if (string.IsNullOrEmpty(it.ToString()))
                    blank = true;
                else if (formatStyle.CellValueType == typeof(System.DateTime) && !string.IsNullOrEmpty(it.ToString())
                && DateTime.TryParse(it.ToString(), out dateTimeOutValue) && !valuesDate.Contains(dateTimeOutValue))
                {                   
                    valuesDate.Add(dateTimeOutValue);
                }
                else if (formatStyle.CellValueType == typeof(double) && !string.IsNullOrEmpty(it.ToString())
                 && double.TryParse(it.ToString(), out doubleOutValue) && !valuesDouble.Contains(doubleOutValue))
                {
                    valuesDouble.Add(doubleOutValue);
                }
                else if (formatStyle.CellValueType == typeof(decimal) && !string.IsNullOrEmpty(it.ToString())
                  && decimal.TryParse(it.ToString(), out decimalOutValue) && !valuesDecimal.Contains(decimalOutValue))
                {
                    valuesDecimal.Add(decimalOutValue);
                }
                else if (formatStyle.CellValueType == typeof(int) && !string.IsNullOrEmpty(it.ToString())
                 && int.TryParse(it.ToString(), out intOutValue) && !valuesInt.Contains(intOutValue))
                {
                    valuesInt.Add(intOutValue);
                }
                else if (formatStyle.CellValueType == typeof(System.Int16) && !string.IsNullOrEmpty(it.ToString())
                  && Int16.TryParse(it.ToString(), out int16OutValue) && !valuesInt16.Contains(int16OutValue))
                {
                    valuesInt16.Add(int16OutValue);
                }
                else if (formatStyle.CellValueType == typeof(System.Int64) && !string.IsNullOrEmpty(it.ToString())
                 && Int64.TryParse(it.ToString(), out int64OutValue) && !valuesInt64.Contains(int64OutValue))
                {
                    valuesInt64.Add(int64OutValue);
                }
                else if (formatStyle.CellValueType == typeof(float) && !string.IsNullOrEmpty(it.ToString())
                 && float.TryParse(it.ToString(), out floatOutValue) && !valuesfloat.Contains(floatOutValue))
                {
                    valuesfloat.Add(floatOutValue);
                }
                else 
                {
                    values.Add(it.ToString());
                }
            }

            if (formatStyle.CellValueType == typeof(DateTime))
            {
                valuesDate.Sort();
                foreach (object obj in valuesDate)
                {
                    string s = string.Format(format, Convert.ToDateTime(obj.ToString()));
                    values.Add(s.ToString());
                }
            }

            if (formatStyle.CellValueType == typeof(Double))
            {
                valuesDouble.Sort();
                foreach (object obj in valuesDouble)
                {
                    string s = string.Format(format, Convert.ToDouble(obj));
                    values.Add(s.Trim());
                }
            }

            else if (formatStyle.CellValueType == typeof(Decimal))
            {
                valuesDecimal.Sort();
                foreach (object obj in valuesDecimal)
                {
                    string s = string.Format(format, Convert.ToDecimal(obj));
                    values.Add(s.Trim());
                }
            }
            else if (formatStyle.CellValueType == typeof(int))
            {
                valuesInt.Sort();
                foreach (object obj in valuesInt)
                {
                    string s = string.Format(format, int.Parse(obj.ToString()));
                    values.Add(s.Trim());
                }
            }
            else if (formatStyle.CellValueType == typeof(Int16))
            {
                valuesInt16.Sort();
                foreach (object obj in valuesInt16)
                {
                    string s = string.Format(format, Convert.ToInt16(obj.ToString()));
                    values.Add(s.Trim());
                }
            }
            else if (formatStyle.CellValueType == typeof(Int64))
            {
                valuesInt64.Sort();
                foreach (object obj in valuesInt64)
                {
                    string s = string.Format(format, Convert.ToInt64(obj.ToString()));
                    values.Add(s.Trim());
                }
            }
            else if (formatStyle.CellValueType == typeof(float))
            {
                valuesfloat.Sort();
                foreach (object obj in valuesfloat)
                {
                    string s = string.Format(format, float.Parse(obj.ToString()));
                    values.Add(s.Trim());
                }
            }
            else
                values.Sort();
            if (blank && !values.Contains("(Blanks)"))
                values.Add("(Blanks)");

            item = values.ToArray();  
            return item;
        }
        /// <summary>
        /// Sets Metro button.
        /// </summary>
        /// <param name="button">button control</param>
        private void WireButton(ButtonAdv button)
        {
           
            if (button.Enabled)
                button.BackColor = Color.FromArgb(22, 165, 220);
            button.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular);

            switch (this.Grid.GetGridVisualStylesDrawing().VisualStyle.ToString())
            {
                case "SystemTheme":
                    button.Appearance = Windows.Forms.ButtonAppearance.None;                    
                    break;
                case "Metro":
                    button.Appearance = Windows.Forms.ButtonAppearance.Metro;
                    button.ForeColor = Color.White;                   
                    break;
                case "Custom":
                    button.Office2007ColorScheme = Windows.Forms.Office2007Theme.Managed;
                    button.Office2007ColorScheme = Windows.Forms.Office2007Theme.Managed;
                    break;
                case "Office2003":
                    button.Appearance = Windows.Forms.ButtonAppearance.Office2003;
                    button.Office2007ColorScheme = Windows.Forms.Office2007Theme.Blue;
                    break;
                case "Office2007Blue":
                case "Office2010Blue":
                    button.Appearance = Windows.Forms.ButtonAppearance.Office2007;                    
                    button.Office2007ColorScheme = Windows.Forms.Office2007Theme.Blue;                    
                    break;
                case "Office2007Black":
                case "Office2010Black":
                    button.Appearance = Windows.Forms.ButtonAppearance.Office2007;                    
                    button.Office2007ColorScheme = Windows.Forms.Office2007Theme.Black;                    
                    break;
                case "Office2007Silver":
                case "Office2010Silver":
                    button.Appearance = Windows.Forms.ButtonAppearance.Office2007;                    
                    button.Office2007ColorScheme = Windows.Forms.Office2007Theme.Silver;                   
                    break;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Returns an array of category keys for this group and all parent groups which is used by FilterBarCells and FilterBarSummary
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
        /// Returns the unique choices to be displayed in the filterbar cell.
        /// </summary>
        /// <param name="tableCellIdentity">Table cell identifier.</param>
        /// <returns>Filter bar choices.</returns>
        public object[] GetFilterBarChoices(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            GridGroupingControl groupGrid = ((GridEngine)tableCellIdentity.Table.Engine).GroupingControl;
            object[] result = null;
            GridGroupingControl gc = tableCellIdentity.Table.Engine.ParentControl;
            GridQueryFilterBarChoicesEventArgs qe = new GridQueryFilterBarChoicesEventArgs(tableCellIdentity.Column, false, tableCellIdentity.DisplayElement);
            gc.OnQueryFilterBarChoices(qe);
            if (!qe.Cancel)
            {
                result = qe.UniqueFilterBarValues;
            }
            Array.Sort(result, new ValueComparer());
            return result;
        }

        /// <summary>
        /// Returns the unique choices to be displayed in the filterbar cell.
        /// </summary>
        /// <param name="tableCellIdentity">Table cell identifier.</param>
        /// <returns>Filter bar choices.</returns>
        public object[] GetDateFilterBarChoices(object[] items, int index, object filteroption)
        {
            MetroCustomRowFilter filterop = (MetroCustomRowFilter)filteroption;
            object dt1 = filterop.compareText1;
            List<object> choices = new List<object>();
            switch (index)
            {
                case 0:
                    foreach (object s in items)
                    {
                        if (s.Equals(dt1))
                            choices.Add(s);
                    }
                    break;
            }
            return choices.ToArray();
        }

        /// <summary>
        /// Determines the name of the filter based on the cell style information passed in.
        /// </summary>
        /// <param name="sty">cell style information.</param>
        /// <returns>Name of the filter.</returns>
        public string GetFilterName(GridTableCellStyleInfo sty)
        {
            object[] id = GetUniqueGroupId(sty.TableCellIdentity);
            string filterName = "";
            if (id == null || id.Length == 0)
            {
                filterName = sty.TableCellIdentity.Column.Name;
            }
            else
            {
                filterName = sty.TableCellIdentity.Column.Name + "@" + sty.TableCellIdentity.DisplayElement.ParentGroup.UniqueGroupIdsToString();
            }
            return filterName;
        }

        /// <summary>
        /// Gets the filter values collection.
        /// </summary>
        /// <param name="sty">cell style</param>
        /// <returns>filter values</returns>
        private IList GetFilterTextList(GridTableCellStyleInfo sty)
        {
            List<string> filterValues = new List<string>();
            filterValues.Clear();
            string filterName = GetFilterName(sty);
            if (!GridUtil.IsEmpty(filterName) && filterLists.ContainsKey(filterName))
            {
                filterValues = filterLists[filterName];
            }
            return filterValues;
        }

        /// <summary>
        /// Clears all the filter choices from dictionary.
        /// </summary>
        public void ClearFilters()
        {
            filterLists.Clear();
        }
        #endregion
    }


    /// <summary>
    /// Renderer is override for metro ContextMenu.
    /// </summary>
    public class MetroExcelFilterContextMenuRenderer : ToolStripProfessionalRenderer
    {
        /// <summary>
        /// base method 
        /// </summary>
        public MetroExcelFilterContextMenuRenderer()
        {
        }
        /// <summary>
        /// base ToolStripBorder
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            base.OnRenderToolStripBorder(e);
        }
        /// <summary>
        /// Is triggered when the margin for the image is rendered.
        /// </summary>
        /// <param name="e">ToolStripRenderEventArgs</param>
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            Rectangle marginRect = e.AffectedBounds;
            using (SolidBrush backBrush = new SolidBrush(Color.White))
            {
                e.Graphics.FillRectangle(backBrush, marginRect);
            }
        }

    }
}
