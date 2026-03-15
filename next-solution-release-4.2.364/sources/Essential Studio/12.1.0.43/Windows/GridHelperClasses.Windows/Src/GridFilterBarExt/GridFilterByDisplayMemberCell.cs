//-------------------------------------------------------------------------------------------------
// <copyright file="GridFilterByDisplayMemberCell.cs" company="Syncfusion">
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
    using System.Collections.Generic;
    using System.Text;
    using System.Data;
    using System.Windows.Forms;
    using Syncfusion.Windows.Forms;
    using Syncfusion.Grouping;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Windows.Forms.Grid.Grouping;
    using System.Collections;
    using System.Reflection;

    /// <summary>
    /// Implements a data model for FilterByDisplayMember cell.
    /// </summary>
    public class GridFilterByDisplayMemberCellModel : GridTableFilterBarCellModel
    {
        /// <summary>
        /// Constructor for GridFilterByDisplayMemberCellModel.
        /// </summary>
        /// <param name="gm">The grid model.</param>
        public GridFilterByDisplayMemberCellModel(GridModel gm)
            : base(gm)
        {
        }

        /// <summary>
        /// Creates the choce list for filter bar drop down.
        /// </summary>
        /// <param name="listBox">Drop down list box.</param>
        /// <param name="style">Cell style information.</param>
        /// <param name="exclusive">Indicates whether the list box is loaded with exclusive list of possible choices or if non-standard values are allowed.</param>
        public override void FillWithChoices(ListBox listBox, GridStyleInfo style, out bool exclusive)
        {
            exclusive = style.ExclusiveChoiceList;
            GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)style;
            listBox.DataSource = null;
            listBox.Items.Clear();
            if (tableStyleInfo.DataSource != null)
            {
                Type listType = tableStyleInfo.DataSource.GetType();
                if (listType.IsGenericType)
                {
                    DataTable dt;
                   
                    Type elementType = listType.GetGenericArguments()[0];                    
                    dt = new DataTable(elementType.Name + "List");
                    
                    MemberInfo[] miArray = elementType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
                    foreach (MemberInfo mi in miArray)
                    {
                        if (mi.MemberType == MemberTypes.Property)
                        {
                            PropertyInfo pi = mi as PropertyInfo;
                            if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                dt.Columns.Add(pi.Name);
                            }
                            else
                                dt.Columns.Add(pi.Name, pi.PropertyType);
                        }
                        else if (mi.MemberType == MemberTypes.Field)
                        {
                            FieldInfo fi = mi as FieldInfo;
                            dt.Columns.Add(fi.Name, fi.FieldType);
                        }
                    }

                   IList il = tableStyleInfo.DataSource as IList;
                    foreach (object record in il)
                    {
                        int i = 0;
                        object[] fieldValues = new object[dt.Columns.Count];
                        foreach (DataColumn c in dt.Columns)
                        {
                            MemberInfo mi = elementType.GetMember(c.ColumnName)[0];
                            if (mi.MemberType == MemberTypes.Property)
                            {
                                PropertyInfo pi = mi as PropertyInfo;
                                fieldValues[i] = pi.GetValue(record, null);
                            }
                            else if (mi.MemberType == MemberTypes.Field)
                            {
                                FieldInfo fi = mi as FieldInfo;
                                fieldValues[i] = fi.GetValue(record);
                            }
                            i++;
                        }
                        dt.Rows.Add(fieldValues);
            listBox.DataSource = null;
                    }
            listBox.DisplayMember = style.DisplayMember;
            listBox.ValueMember = style.ValueMember;
            if (string.IsNullOrEmpty(style.DisplayMember) && !string.IsNullOrEmpty(style.ValueMember))
            {
                listBox.DisplayMember = style.ValueMember;
            }
            listBox.BindingContext = this.BindingContext;


                    if (dt != null)
                    {

                        listBox.Items.Add(SelectAllText);
                        foreach (DataRow dr in dt.Rows)
                        {
                            listBox.Items.Add(dr[tableStyleInfo.DisplayMember]);
                        }
                    }
                }
                else
                {
                    DataTable dtItems = tableStyleInfo.DataSource as DataTable;
                    if (dtItems != null)
                    {
                        listBox.Items.Add(SelectAllText);
                        foreach (DataRow dr in dtItems.Rows)
                        {
                            listBox.Items.Add(dr[tableStyleInfo.DisplayMember]);
                        }
                    }
                }
            }
       }

        /// <summary>
        /// Creates renderer.
        /// </summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridFilterByDisplayMemberCellRenderer(control, this);
        }
    }

    /// <summary>
    /// Implements renderer for FilterByDisplayMember cell.
    /// </summary>
    public class GridFilterByDisplayMemberCellRenderer : GridTableFilterBarCellRenderer
    {
        /// <summary>
        /// Constructor for GridFilterByDisplayMemberCellRenderer.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        /// <param name="cellModel">The cell model.</param>
        public GridFilterByDisplayMemberCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
        }
        private bool inSet = false;
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
                SetTextBoxText(GetFilterBarDisplayMemberText(StyleInfo), false);// don't call base class - ignore.
                inSet = false;
            }
        }

        /// <summary>
        /// Determines the text from record filter criteria that should be displayed in filterbar cell.
        /// </summary>
        /// <param name="style">Cell style information.</param>
        /// <returns>FilterBar DisplayMember Text.</returns>
        public string GetFilterBarDisplayMemberText(GridStyleInfo style)
        {
            GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)style;
            GridTableCellStyleInfoIdentity tableCellIdentity = tableStyleInfo.TableCellIdentity;
            GridTableDescriptor td = tableCellIdentity.Table.TableDescriptor;
            string filterName = this.Model.GetUniqueColumnGroupId(tableCellIdentity);            
            RecordFilterDescriptor[] filters = this.Model.GetRecordFilters(td.RecordFilters, tableCellIdentity, filterName);
            GridTableControl _grid = Grid as GridTableControl;
            object value = this.Model.SelectAllText;
            if (filters == null)
            {
                value = string.Empty;
            }
            else if (filters.Length == 1 && filters[0].Conditions.Count == 1)
            {
                int _filed = Grid.TableDescriptor.ColIndexToField(ColIndex);
                string _columnName = Grid.TableDescriptor.Columns[_filed].Name;
                if (filters[0].Conditions[0].CompareOperator == FilterCompareOperator.Equals && FilterValue[_columnName] != null && FilterValue[_columnName].ToString() != SR.GetString(SR.All))
                {
                    value = FilterValue[_columnName];
                }
                else
                    return string.Empty;
            }
            return string.Format("{0:" + style.Format + "}", value);
        }

        private Hashtable FilterValue = new Hashtable();
        /// <summary>
        /// Is triggered when the contents are choosed in Filter
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">MouseEventArgs</param>
        protected override void ListBoxMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CurrentCell.CloseDropDown(PopupCloseType.Done);
            GridTableCellStyleInfo tableStyleInfo = (GridTableCellStyleInfo)this.StyleInfo;
            GridTableCellStyleInfoIdentity tableCellIdentity = tableStyleInfo.TableCellIdentity;
            this.ListBoxPart.SelectedValue = GetFilterBarText(StyleInfo);           
            SetTextBoxText(this.ListBoxPart.SelectedItem.ToString(), false);            
            GridTableControl _grid = Grid as GridTableControl;
            int _filed = _grid.TableDescriptor.ColIndexToField(ColIndex);
            string _columnName = _grid.TableDescriptor.VisibleColumns[_filed].Name;
            FilterValue[_columnName] = this.ListBoxPart.SelectedItem.ToString();
            _grid.TableDescriptor.RecordFilters.Remove(_columnName);
            if (FilterValue[_columnName]!=null && FilterValue[_columnName].ToString() != SR.GetString(SR.All))
            {
                int index = this.ListBoxPart.SelectedIndex - 1;
                if (index >= 0)
                {
                    object selectedValue = null;
                    if (tableStyleInfo.DataSource != null)
                    {
                        Type listType = tableStyleInfo.DataSource.GetType();
                        if (listType.IsGenericType)
                        {

                            DataTable dt;
                            Type elementType = listType.GetGenericArguments()[0];
                            dt = new DataTable(elementType.Name + "List");

                            MemberInfo[] miArray = elementType.GetMembers(
                                BindingFlags.Public | BindingFlags.Instance);
                            foreach (MemberInfo mi in miArray)
                            {
                                if (mi.MemberType == MemberTypes.Property)
                                {
                                    PropertyInfo pi = mi as PropertyInfo;
                                    if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                    {
                                        dt.Columns.Add(pi.Name);
                                    }
                                    else
                                        dt.Columns.Add(pi.Name, pi.PropertyType);
                                }
                                else if (mi.MemberType == MemberTypes.Field)
                                {
                                    FieldInfo fi = mi as FieldInfo;
                                    dt.Columns.Add(fi.Name, fi.FieldType);
                                }
                            }

                            IList il = tableStyleInfo.DataSource as IList;
                            foreach (object record in il)
                            {
                                int i = 0;
                                object[] fieldValues = new object[dt.Columns.Count];
                                foreach (DataColumn c in dt.Columns)
                                {
                                    MemberInfo mi = elementType.GetMember(c.ColumnName)[0];
                                    if (mi.MemberType == MemberTypes.Property)
                                    {
                                        PropertyInfo pi = mi as PropertyInfo;
                                        fieldValues[i] = pi.GetValue(record, null);
                                    }
                                    else if (mi.MemberType == MemberTypes.Field)
                                    {
                                        FieldInfo fi = mi as FieldInfo;
                                        fieldValues[i] = fi.GetValue(record);
                                    }
                                    i++;
                                }
                                dt.Rows.Add(fieldValues);
                            }
                            selectedValue = dt.Rows[index][tableStyleInfo.ValueMember];
                        }
                        else
                        {
                            DataTable dtItems = tableStyleInfo.DataSource as DataTable;
                            selectedValue = dtItems.Rows[index][tableStyleInfo.ValueMember];
                        }
                        _grid.TableDescriptor.RecordFilters.Add(_columnName, FilterCompareOperator.Equals, selectedValue);

                    }
                }
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
        protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            int _filed = Grid.TableDescriptor.ColIndexToField(colIndex);
            string _columnName = Grid.TableDescriptor.VisibleColumns[_filed].Name;
            if (FilterValue[_columnName] != null && FilterValue[_columnName].ToString() != SR.GetString(SR.All))
                style.CellValue = FilterValue[_columnName];
            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
        }
    }
}
