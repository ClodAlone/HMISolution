#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows.Media;
    using Syncfusion.Windows.Controls.Cells;
    using System.Windows.Data;
    using System;
    using Syncfusion.Linq;
    using Syncfusion.Windows.Data;
    using System.ComponentModel;
    using Syncfusion.Windows.Collections;

    public class GridDataVolatileCellStyles : GridVolatileCellStyles
    {
        public GridDataVolatileCellStyles(IGridVolatileCellStylesHost host)
            : base(host)
        {
        }

        public GridDataTableModel Model
        {
            get
            {
                return this.Host as GridDataTableModel;
            }
        }

        public GridDataTableProperties TableProperties
        {
            get
            {
                return this.Model.TableProperties;
            }
        }

        public GridDataTable Table
        {
            get
            {
                return this.Model.Table;
            }
        }

        public GridDataCurrentRecordManager CurrencyManager
        {
            get
            {
                return this.Model.CurrencyManager;
            }
        }

        protected override GridStyleInfo CreateStyle(RowColumnIndex cell)
        {
            var style = new GridDataStyleInfo(new GridDataTableStyleInfoIdentity(this, cell));
            style.BeginInit();
            GridDataTableStyleInfoIdentity tableStyleIdentity = style.CellIdentity;
            var rowIndex = cell.RowIndex;
            var colIndex = cell.ColumnIndex;
            var isHeaderOrNot = this.TableProperties.ShowRowHeader ? 0 : -1;
            var headerIndex = this.Model.HeaderRows > 0 ? this.Model.HeaderRows - 1 : -1;
            if (rowIndex == 0 && colIndex == 0 && this.TableProperties.ShowRowHeader)
            {
                // top left cell
                tableStyleIdentity.TableCellType = GridDataTableCellType.TopLeftHeaderCell;
            }
            else if (this.Model.HeaderRows > 0 && rowIndex < this.Model.HeaderRows && colIndex > isHeaderOrNot)
            {
                if (rowIndex == headerIndex)
                {
                    style = this.CreateStyleForColumnHeader(style, tableStyleIdentity, colIndex);
                    //return style;
                }
                else if (rowIndex < headerIndex)
                {
                    style = this.CreateStyleForStackedHeader(style, tableStyleIdentity, cell);
                    //return style;
                }
            }
            else if (rowIndex > headerIndex && colIndex >= 0)
            {
                if (this.TableProperties.ShowRowHeader && colIndex == 0)
                {
                    //if (this.TableProperties.AddNewRowPosition == Position.Top)
                    //{
                    //    if (this.TableProperties.ShowAddNewRow && cell.RowIndex == this.Model.UnboundRowsCount + 1)
                    //    {
                    //        style.CellType = "AddNewHeaderCell";
                    //        tableStyleIdentity.TableCellType = GridDataTableCellType.AddNewRowHeaderCell;
                    //        //return style;
                    //    }
                    //}
                    //else if (this.TableProperties.AddNewRowPosition == Position.Bottom)
                    //{
                    //    if (this.TableProperties.ShowAddNewRow && cell.RowIndex == (this.Model.RowCount - 1))
                    //    {
                    //        style.CellType = "AddNewHeaderCell";
                    //        tableStyleIdentity.TableCellType = GridDataTableCellType.AddNewRowHeaderCell;
                    //        //return style;
                    //    }
                    //}

                    // var posIndex = this.Model.ResolveIndexToPosition(cell.RowIndex);
                    // var record = posIndex > -1 && posIndex < this.Table.Records.Count ? this.Table.Records[posIndex] : null;
                    // if (record != null && record.HasModified)
                    // {
                    //    style.CellType = "ImageContent";
                    //    style.CellValue = RowHeaderBrushes.StaticPencil;
                    // }

                    // style.celltype is set in the current record manager, so just return from here
                    tableStyleIdentity.TableCellType = GridDataTableCellType.RowHeaderCell;
                    //return style;
                }
                else if (this.Model.IsInUnboundRow(cell.RowIndex))
                {
                    tableStyleIdentity.TableCellType = GridDataTableCellType.UnboundRecordCell;
                    var unboundCellInfo = new GridQueryCellInfoEventArgs(cell, style);
                    this.Model.RaiseQueryUnboundCellInfo(unboundCellInfo);
                    if (unboundCellInfo.Handled)
                    {
                        style = unboundCellInfo.Style as GridDataStyleInfo;
                    }
                    //return style;
                }
                else if (this.TableProperties.ShowFilterBar &&
                    ((this.TableProperties.ShowAddNewRow && this.TableProperties.AddNewRowPosition == Position.Top && cell.RowIndex == (this.Model.ResolveStartIndexBasedOnPosition() - 2))
                    || (this.TableProperties.ShowAddNewRow == true && this.TableProperties.AddNewRowPosition == Position.Bottom && cell.RowIndex == (this.Model.ResolveStartIndexBasedOnPosition() - 1))
                    || (this.TableProperties.ShowAddNewRow == false && cell.RowIndex == (this.Model.ResolveStartIndexBasedOnPosition() - 1))))
                {
                    var cIndex = this.Model.ResolvePositionToVisibleColumnIndex(cell.ColumnIndex);
                    var column = cIndex > -1 && cIndex < this.TableProperties.VisibleColumns.Count ? this.TableProperties.VisibleColumns[cIndex] : null;
                    if (column != null)
                    {
                        tableStyleIdentity.Column = column;
                        if (column.FilterBarStyle != null && column.FilterBarStyle.CellType == CellType.ComboBox)
                        {
                            tableStyleIdentity.TableCellType = GridDataTableCellType.DropDownFilterCell;
                            style.CellType = "DropDownFilterCell";
                            if (column.FilterBarStyle.ItemsSource != null)
                                style.ItemsSource = column.FilterBarStyle.ItemsSource;
                            if (column.FilterBarStyle.DisplayMember != null)
                                style.DisplayMember = column.FilterBarStyle.DisplayMember;
                            if (column.FilterBarStyle.ValueMember != null)
                                style.ValueMember = column.FilterBarStyle.ValueMember;
                            style.CellValue = GridDataResourceWrapper.AllFilter;
                        }
                        else
                        {
                            tableStyleIdentity.TableCellType = GridDataTableCellType.FilterBarCell;
                            style.CellType = "FilterBarCell";
                        }

                        if (column.Filters.Count > 0)
                        {
                            string filtrd = "";
                            foreach (var item in column.Filters)
                            {
                                if (filtrd == "")
                                    filtrd += Syncfusion.Windows.Controls.Grid.GridDataFilterBarCellRenderer.GridDataFiltersTokenizer.GetFilterTypeString(item.FilterValue.ToString(), item.FilterType);
                                else
                                    filtrd += " " + item.PredicateType.ToString().ToLower() + " " + Syncfusion.Windows.Controls.Grid.GridDataFilterBarCellRenderer.GridDataFiltersTokenizer.GetFilterTypeString(item.FilterValue.ToString(), item.FilterType);
                            }

                            if (filtrd.EndsWith("%") && this.TableProperties.AlphaNumericFilterType == AlphaNumericFilterType.WithoutWildcard)
                            {
                                filtrd = filtrd.Remove(filtrd.Length - 1);
                            }
                            style.CellValue = filtrd;
                        }
                    }
                }
                else if ((this.TableProperties.AddNewRowPosition == Position.Top && this.TableProperties.ShowAddNewRow && cell.RowIndex == this.Model.ResolveStartIndexBasedOnPosition() - 1)
                    || (this.TableProperties.AddNewRowPosition == Position.Bottom && this.TableProperties.UnboundRowPosition == Position.Top && this.TableProperties.ShowAddNewRow && cell.RowIndex == (this.Model.RowCount - (1 + (this.Table.HasTableSummaries && this.TableProperties.ShowTableSummaries && this.TableProperties.TableSummaryPosition == Position.Bottom ? 1 : 0))))
                    || (this.TableProperties.AddNewRowPosition == Position.Bottom && this.TableProperties.UnboundRowPosition == Position.Bottom && this.TableProperties.ShowAddNewRow && cell.RowIndex == (this.Model.RowCount - this.Model.UnboundRowsCount - 1)))
                {
                    if ((cell.ColumnIndex < this.Model.ResolveDefaultColumnOffset()) && ((this.TableProperties.ShowGroupCaptionPlusMinus && this.Table.HasGroups) || this.Table.HasNestedTables || this.Table.HasDetailsView))
                    {
                        // an indent cell for the column header
                        tableStyleIdentity.TableCellType = GridDataTableCellType.ColumnHeaderIndentCell;
                        style.CellType = "Static";
                    }
                    else
                    {
                        colIndex = this.Model.ResolvePositionToVisibleColumnIndex(cell.ColumnIndex);
                        var column = colIndex < this.TableProperties.VisibleColumns.Count ? this.TableProperties.VisibleColumns[colIndex] : null;
                        if (column == null)
                        {
                            // we may have null columns if we have a nested table, since the last column of the nested table is just there to show the expanded records
                            style = this.CreateEmptyCell(style, tableStyleIdentity);
                        }
                        else if (column != null && column.IsUnbound)
                        {
                            RowColumnIndex cellRowColIndex=new RowColumnIndex(cell.RowIndex,cell.ColumnIndex);
                            style = CreateStyleForUnboundColumn(cellRowColIndex, -1, column, style, tableStyleIdentity);
                        }
                        else
                        {
                            // if we have to show a AddNew row record no need to extract any data because it would an empty row
                            if (column.ColumnStyle != null)
                            {
                                style.ModifyStyle(column.ColumnStyle, Syncfusion.Windows.Styles.StyleModifyType.Override);
                            }

                            if (column.CellItemTemplate != null)
                            {
                                style.CellType = "GridDataBoundTemplate";
                                style.CellItemTemplate = column.CellItemTemplate;
                                style.CellEditTemplate = column.CellEditItemTemplate;
                            }
                            tableStyleIdentity.TableCellType = GridDataTableCellType.AddNewRecordCell;
                            object value = this.CurrencyManager.GetValueFromCache(column.MappingName);
                            if (value == null)
                            {
                                style.CellValue = string.Empty;
                            }
                            else
                            {
                                style.CellValue = value;
                            }
                        }
                        //return style;
                    }
                }
                else
                {
                    var isNotSummary = !this.Model.IsInSummaryPosition(cell.RowIndex);
                    if (isNotSummary)
                    {
                        if (!this.Table.HasGroups)
                        {
                            style = this.CreateStyleForFlatTable(rowIndex, colIndex, cell, style, tableStyleIdentity, isHeaderOrNot);
                        }
                        else
                        {
                            style = this.CreateStyleForGroup(rowIndex, colIndex, cell, style, tableStyleIdentity, isHeaderOrNot);
                        }
                    }
                    else
                    {
                        if (this.TableProperties.ShowTableSummaries)
                        {
                            style = this.CreateStyleForTableSummary(cell, style, tableStyleIdentity);
                        }
                    }
                }
            }
            style.EndInit();
            return style;
        }

        private GridDataStyleInfo CreateStyleForColumnHeader(GridDataStyleInfo style, GridDataTableStyleInfoIdentity tableStyleIdentity, int colIndex)
        {
            if (colIndex < this.Model.ResolveDefaultColumnOffset())
            {
                // an indent cell for the column header
                tableStyleIdentity.TableCellType = GridDataTableCellType.ColumnHeaderIndentCell;
                style.CellType = "Static";
                return style;
                //return this.CreateEmptyCell(style, tableStyleIdentity);
            }
            else
            {
                colIndex = this.Model.ResolvePositionToVisibleColumnIndex(colIndex);
                // var visibleColumns = this.TableProperties.VisibleColumns.GetActualVisibleColumns();
                var column = colIndex > -1 && colIndex < this.TableProperties.VisibleColumns.Count ? this.TableProperties.VisibleColumns[colIndex] : null;
                if (column != null)
                {
                    tableStyleIdentity.TableCellType = !column.IsUnbound ? GridDataTableCellType.ColumnHeaderCell : GridDataTableCellType.UnboundColumnHeaderCell;
                    tableStyleIdentity.Column = column;
                    var value = string.Empty;

                    if (column.HeaderText != null)/* && column.HeaderText != string.Empty)*/
                    {
                        value = column.HeaderText;
                    }
                    else if (column.MappingName == null)
                        value = null;
                    else
                    {
#if SyncfusionFramework4_0
#if !SILVERLIGHT
                        string propName = column.MappingName;
                        string[] propertyNameList = propName.Split('.');
                        int complexPropertyCount = propertyNameList.Count();
                        if (complexPropertyCount > 1 && !this.TableProperties.AutoPopulateColumns)
                        {
                            bool hasDisplayAttribute = false;
                            bool hasBindableAttribute = false;
                            var itemProperties = this.Model.View != null ? this.Model.View.GetItemProperties() : null;
                            for (int iterator = 0; iterator < complexPropertyCount - 1; iterator++)
                            {
                                if (itemProperties != null)
                                {
                                    var tempProperyDescriptor = itemProperties.Find(propertyNameList[iterator], true);
                                    if (tempProperyDescriptor != null)
                                        itemProperties = TypeDescriptor.GetProperties(tempProperyDescriptor.PropertyType);
                                }
                            }
                            if (itemProperties != null)
                            {
                                foreach (PropertyDescriptor pd in itemProperties)
                                {
                                    var canAddColumn = !ListUtil.IsComplexType(pd) || typeof(byte[]).IsAssignableFrom(pd.PropertyType);
                                    System.ComponentModel.DataAnnotations.DisplayAttribute displayAttribute = null;
                                    this.Model.GetDisplayAttribute(pd, ref canAddColumn, ref hasDisplayAttribute, out displayAttribute);

                                    System.ComponentModel.BindableAttribute bindableAttribute = null;
                                    this.Model.GetBindableAttribute(pd, ref canAddColumn, ref hasBindableAttribute, out bindableAttribute);
                                    if (hasBindableAttribute)
                                    {
                                        hasDisplayAttribute = false;
                                    }

                                    if (canAddColumn)
                                    {
                                        if (pd.Name == propertyNameList[complexPropertyCount - 1]
                                            && string.IsNullOrEmpty(column.HeaderText) && hasDisplayAttribute && displayAttribute != null)
                                        {
                                            string ShortName = displayAttribute.GetShortName();
                                            if (!string.IsNullOrEmpty(ShortName))
                                            {
                                                column.HeaderText = ShortName;
                                                value = ShortName;
                                            }
                                        }
                                    }
                                }
                                if (column.HeaderText == null)
                                {
                                    column.HeaderText = column.MappingName;
                                    value = column.MappingName;
                                }
                            }
                        }
                        else
#endif
#endif
                            value = column.MappingName;
                    }

                    style.CellValue = value;

                    var groupedColumn = this.TableProperties.GroupedColumns.Where(g => g.ColumnName == column.MappingName).FirstOrDefault();
                    // Check if the column is sorted
                    var sortColumnName = column.SortMemberPath != null ? column.SortMemberPath : column.MappingName;
                    var sortedColumn = this.TableProperties.SortColumns.Where(s => s.ColumnName == sortColumnName).FirstOrDefault();
#if !SILVERLIGHT
                    if (!GridDataTableModelHelper.IsInDesignMode)
                    {
                        style.CellType = "HeaderCell";
                    }
#else
                        style.CellType = "HeaderCell";
#endif
#if SILVERLIGHT

                    if (column.HeaderCellTemplate != null)
                    {
                        style.CellType = "GridDataBoundTemplate";
                        style.CellItemTemplate = column.HeaderCellTemplate;
                    }
#else
                    style.CellItemTemplate = column.HeaderCellTemplate;
#endif
                    if (column.AllowFilter || column.ShowColumnOptions)
                    {
                        style.StaysOpenOnEdit = true;
                    }
                    else
                    {
                        style.StaysOpenOnEdit = false;
                    }

                    if (groupedColumn != null)
                    {
                        var sortColForGroup = this.TableProperties.GetSortColumnForGroup(groupedColumn);
                        if (sortColForGroup != null)
                        {
                            style.Tag = sortColForGroup.SortDirection;
                        }
                    }
                    else if (sortedColumn != null)
                    {
                        style.Tag = sortedColumn.SortDirection;
                    }
                    else
                    {
                        style.Tag = null;
                    }
                    if (column.HeaderStyle != null)
                    {
                        var clonedStyle = new GridStyleInfo();
                        clonedStyle.BeginInit();
                        clonedStyle.CopyFrom(column.HeaderStyle);
                        var baseStyle = this.Model.HeaderStyle;
                        clonedStyle.ModifyStyle(baseStyle, Syncfusion.Windows.Styles.StyleModifyType.ApplyNew);
                        clonedStyle.EndInit();
                        style.ModifyStyle(clonedStyle, Syncfusion.Windows.Styles.StyleModifyType.ApplyNew);
                    }
                }
                else
                {
                    tableStyleIdentity.TableCellType = GridDataTableCellType.ColumnHeaderIndentCell;
                    style.CellType = "Static";
                }
            }

            return style;
        }

        private GridDataStyleInfo CreateStyleForStackedHeader(GridDataStyleInfo style, GridDataTableStyleInfoIdentity tableStyleIdentity, RowColumnIndex cell)
        {
            style.CellType = "Static";
            var colIndex = cell.ColumnIndex;
            // var rowIndex = cell.RowIndex; Unused local variable
            if (colIndex < this.Model.ResolveDefaultColumnOffset())
            {
                // an indent cell for the column header
                tableStyleIdentity.TableCellType = GridDataTableCellType.ColumnHeaderIndentCell;
                return style;
            }
            else
            {
                var stackRowIdx = this.Model.ResolveIndexToStackHeaderRowPosition(cell.RowIndex);
                var stackRow = stackRowIdx > -1 && stackRowIdx < this.TableProperties.StackedHeaderRows.Count ? this.TableProperties.StackedHeaderRows[stackRowIdx] : null;
                if (stackRow != null)
                {
                    tableStyleIdentity.StackHeaderRow = stackRow;
                    var stackHeaderCol = this.Model.ResolveIndexToStackHeaderColumn(stackRow, cell.ColumnIndex);
                    if (stackHeaderCol != null)
                    {
                        tableStyleIdentity.TableCellType = GridDataTableCellType.StackedColumnHeaderCell;
                        tableStyleIdentity.StackHeaderColumn = stackHeaderCol;
                        style.CellValue = stackHeaderCol.HeaderText != null && stackHeaderCol.HeaderText != string.Empty ? stackHeaderCol.HeaderText : stackHeaderCol.Name;
                        style.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        if (stackHeaderCol.ColumnStyle != null)
                        {
                            style.ModifyStyle(stackHeaderCol.ColumnStyle, Syncfusion.Windows.Styles.StyleModifyType.Override);
                        }
                    }
                    else
                    {
                        style = this.CreateEmptyCell(style, tableStyleIdentity);
                    }
                }
            }
            return style;
        }

        private GridDataStyleInfo CreateEmptyCell(GridDataStyleInfo style, GridDataTableStyleInfoIdentity tableStyleIdentity)
        {
            // for addnew row, cell value should be empty
            tableStyleIdentity.TableCellType = GridDataTableCellType.EmptyCell;
            style.CellType = "Static";
            style.CellValue = null;
            //style.Borders = new CellBordersInfo()
            //{
            //    Left = new Pen(),
            //    Right = new Pen(),
            //    Bottom = new Pen(),
            //    Top = new Pen()
            //};
            return style;
        }

        private GridDataStyleInfo CreateGroupCaptionEmptyCell(GridDataStyleInfo style, GridDataTableStyleInfoIdentity tableStyleIdentity)
        {            
            tableStyleIdentity.TableCellType = GridDataTableCellType.GroupCaptionSummaryEmptyCell;
            style.CellType = "Static";
            style.CellValue = null;            
            return style;
        }


        private GridDataStyleInfo CreateSummaryEmptyCell(GridDataStyleInfo style, GridDataTableStyleInfoIdentity tableStyleIdentity)
        {            
            tableStyleIdentity.TableCellType = GridDataTableCellType.SummaryEmptyCell;
            style.CellType = "Static";
            style.CellValue = null;         
            return style;
            
        }

        private GridDataStyleInfo CreateStyleForFlatTable(int rowIndex, int colIndex, RowColumnIndex cell, GridDataStyleInfo style, GridDataTableStyleInfoIdentity tableStyleIdentity, int isHeaderOrNot)
        {
            rowIndex = this.Model.ResolveIndexToRecordPosition(cell.RowIndex);
            //GridDataRecord record = rowIndex > -1 && rowIndex < this.Model.View.Records.Count ? this.Model.View.Records[rowIndex] as GridDataRecord : null;
            GridDataRecord record = rowIndex > -1 && rowIndex < this.Model.SourceListCount && rowIndex < this.Model.View.Records.Count ? this.Model.View.Records[rowIndex] as GridDataRecord : null;
            if (record == null)
            {
                return this.CreateEmptyCell(style, tableStyleIdentity);
            }

            tableStyleIdentity.RecordEntry = record;
            //var record = this.Table.GetRecordBoundState(rowIndex, false); //rowIndex > -1 && rowIndex < this.Model.SourceListCount ? this.Table.Records[rowIndex] : null;
            if ((colIndex == isHeaderOrNot + 1) && this.Table.HasNestedTables)
            {
                if (this.TableProperties.AddNewRowPosition == Position.Top)
                {
                    if (this.TableProperties.ShowAddNewRow && cell.RowIndex == this.Model.UnboundRowsCount + 1)
                    {
                        return this.CreateEmptyCell(style, tableStyleIdentity);
                    }
                }
                else if (this.TableProperties.AddNewRowPosition == Position.Bottom)
                {
                    if (this.TableProperties.ShowAddNewRow && cell.RowIndex == (this.Model.RowCount - 1))
                    {
                        return this.CreateEmptyCell(style, tableStyleIdentity);
                    }
                }

                if (this.Model.IsInNestedIndex(cell.RowIndex))
                {
                    return this.CreateEmptyCell(style, tableStyleIdentity);
                }

                if (!record.Model.Table.ShouldExpand(record))
                {
                    return this.CreateEmptyCell(style, tableStyleIdentity);
                }

                tableStyleIdentity.TableCellType = GridDataTableCellType.RecordPlusMinusCell;
                style.CellType = "ExpandCollapseCell";
                style.CellValue = record != null ? record.IsExpanded : false;
            }
            //Identifying the expander cell for Details view cell, this can be mixed with above code, but it will 
            //become too dificualt to understand and it will involve complex conditions, hence added as separate block
            else if (((colIndex == isHeaderOrNot + 1) || (colIndex == isHeaderOrNot + 2 && this.Table.HasNestedTables)) &&
                     this.Table.HasDetailsView)
            {
                if (this.TableProperties.AddNewRowPosition == Position.Top)
                {
                    if (this.TableProperties.ShowAddNewRow && cell.RowIndex == this.Model.UnboundRowsCount + 1)
                    {
                        return this.CreateEmptyCell(style, tableStyleIdentity);
                    }
                }
                else if (this.TableProperties.AddNewRowPosition == Position.Bottom)
                {
                    if (this.TableProperties.ShowAddNewRow && cell.RowIndex == (this.Model.RowCount - 1))
                    {
                        return this.CreateEmptyCell(style, tableStyleIdentity);
                    }
                }

                if (this.Model.IsInNestedIndex(cell.RowIndex))
                {
                    return this.CreateEmptyCell(style, tableStyleIdentity);
                }

                tableStyleIdentity.TableCellType = GridDataTableCellType.RecordPlusMinusCell;
                style.CellType = "ExpandCollapseCell";
                style.CellValue = record != null ? record.IsDetailsViewExpanded : false;
            }
            else
            {
                colIndex = this.Model.ResolvePositionToVisibleColumnIndex(colIndex);
                var column = colIndex < this.TableProperties.VisibleColumns.Count
                                 ? this.TableProperties.VisibleColumns[colIndex]
                                 : null;
                if (column == null)
                {
                    tableStyleIdentity.TableCellType = GridDataTableCellType.EmptyCell;
                    style.CellType = "Static";
                    return style;
                }
                else if (this.Table.Model.TableProperties.Relations.Count > 0)
                {
                    string relationalColumn = column.MappingName;

                    foreach (GridDataRelation rel in this.Table.Model.TableProperties.Relations)
                    {
#if !SILVERLIGHT
                        if (rel.RelationType == RelationType.ForeignKeyReference &&
                            rel.RelationalColumn.Equals(relationalColumn))
                        {
                            style.CellType = "GridDataControlCell";
                            style.ItemsSource = rel.ChildItemsSource;
                            style.ChildRelationalColumn = rel.ChildRelationalColumn;
                            style.CellValue2 = rel.TableProperties;
                            style.DropDownStyle = GridDropDownStyle.Exclusive;
                            break;
                        }
#endif
                    }

                    tableStyleIdentity.Column = column;
                }
                else
                {
                    tableStyleIdentity.Column = column;
                }

                if (!this.Model.IsInNestedIndex(cell.RowIndex))
                {
                    if (column.ColumnStyle != null)
                    {
                        style.ModifyStyle(column.ColumnStyle, Syncfusion.Windows.Styles.StyleModifyType.Override);
                    }

                    if (column.CellItemTemplate != null)
                    {
                        style.CellType = "GridDataBoundTemplate";
                        style.CellItemTemplate = column.CellItemTemplate;
                        style.CellEditTemplate = column.CellEditItemTemplate;
                    }

                    if (!column.IsUnbound)
                    {
                        // we get the value from the cache instance
                        style.CellValue = rowIndex > -1 && rowIndex < this.Model.SourceListCount
                                              ? this.Model.Table.GetValue(rowIndex, column.MappingName)
                                              : null;
                        style.CellValueType = column.ColumnStyle != null && column.ColumnStyle.CellValueType != null
                                                  ? column.ColumnStyle.CellValueType
                                                  : column.ColumnType;
                        //tableStyleIdentity.RecordIndex = rowIndex;
                        tableStyleIdentity.TableCellType = GridDataTableCellType.RecordCell;
                        //style.ShowTooltip = this.TableProperties.ShowTooltips ? true : false;
                    }
                    else
                    {
                        style = this.CreateStyleForUnboundColumn(cell, rowIndex, column, style, tableStyleIdentity);
                    }
                }
                else
                {
                    int parentRow = this.Model.ResolvePositionToIndex(rowIndex);

                    //Identifying the cell type to be rendered
                    if (!this.Model.Table.HasDetailsView || style.RowIndex - parentRow > 1)
                    {
                        //// Processing nested table cell
                        if (record.Model.Table.ShouldExpand(record))
                            this.CreateStyleForNestedGrid(cell, style, tableStyleIdentity, record);
                    }
                    else
                    {
                        //processing Details view cell
                        tableStyleIdentity.TableCellType = GridDataTableCellType.DetailsViewCell;
                        style.CellType = "GridDataBoundTemplate";
                        style.CellItemTemplate = record.DetailsViewTemplate;
                        style.CellValue = record.DetailsViewDataContext;
                        style.Padding = new CellMarginsInfo(6, 12.5, 6, 12.5);
                    }
                }
            }

            return style;
        }

        private GridDataStyleInfo CreateStyleForUnboundColumn(RowColumnIndex cell, int recordIndex, GridDataVisibleColumn column, GridDataStyleInfo style, GridDataTableStyleInfoIdentity tableStyleIdentity)
        {
            tableStyleIdentity.TableCellType = GridDataTableCellType.UnboundColumnCell;

            if (column is GridDataUnboundVisibleColumn)
            {
                var unbound = column as GridDataUnboundVisibleColumn;
                this.Model.RaiseQueryUnboundValue(cell, style, recordIndex, unbound, true);
            }
            return style;
        }

        private GridDataStyleInfo CreateStyleForNestedGrid(RowColumnIndex cell, GridDataStyleInfo style, GridDataTableStyleInfoIdentity tableStyleIdentity, GridDataRecord record)
        {
            if (record != null && record.IsExpanded)
            {
                tableStyleIdentity.TableCellType = GridDataTableCellType.NestedTableCell;
                var key = this.Model.GetOrderForChildTableBasedOnIndex(cell.RowIndex -1);

                //while the record is in detail view the order of child will always retrun the key as one less than the exact one,
                //this can be fixed in GetOrderForChildTableBaseOnIndex method itself, but we need to fetch the record there to check
                //its Detail view state. So to avoid fetching the record following code is added here to get correct key.
                key += record.IsDetailsViewExpanded && this.Model.Table.HasGroups ? 1 : 0;

                int nestedcolIndex = 0;
                if (!this.Table.HasGroups)
                {
                    nestedcolIndex = this.TableProperties.ShowRowHeader ? 1 : 0;
                    nestedcolIndex = this.TableProperties.ShowRecordPlusMinus ? nestedcolIndex + 1 : nestedcolIndex;
                    nestedcolIndex += this.Table.HasDetailsView ? 1 : 0;
                }
                else
                {
                    nestedcolIndex = this.Model.ResolveDefaultColumnOffset();
                }
                if (cell.ColumnIndex >= nestedcolIndex && record.ChildModels.ContainsKey(key))
                {
                    var childModel = record.ChildModels[key];
                    style.CellType = "NestedGrid";
                    style.CellValue = record.ChildModels[key];

                    style.Borders.Left = new Pen();
                    style.Borders.Top = new Pen();
                    style.Borders.Right = new Pen();

                    style.Borders.Bottom = this.Model.GridVisualStyle.ValueCellBorders.Bottom;// new Pen(Brushes.Black, 1);
                    style.Background = Brushes.Transparent;
                    if (this.TableProperties.AllowNestedGridPadding)
                    {
                        style.Padding = new CellMarginsInfo(6, 6, 2, 12.5);// the total PaddingDistance = 20d, we need an extra 14d in the bottom to cover the extra space.
                    }
                    else
                    {
                        style.Padding = new CellMarginsInfo(0,0,0,0);
                    }

                    style.CellIdentity.RecordEntry = record;
                    childModel.InvalidateCell(GridRangeInfo.Table());
                }
                else
                {
                    // these will be the spanned cells
                    tableStyleIdentity.TableCellType = GridDataTableCellType.NestedTableEmptyCell;
                }
            }

            return style;
        }

        private GridDataStyleInfo CreateStyleForGroup(int rowIndex, int colIndex, RowColumnIndex cell, GridDataStyleInfo style, GridDataTableStyleInfoIdentity tableStyleIdentity, int isHeaderOrNot)
        {
            //Since the display element of details view row will always null, the row info has to be ensured before fetching the display element
            if (this.Model.Table.HasDetailsView && this.Model.DetailsViewRows.ContainsKey(cell.RowIndex))
            {
                //Fetching the record from preserved collection.
                var record = this.Model.DetailsViewRows[cell.RowIndex];

                //To identify whether the grop is in expand state or not
                bool canProcess = record.Parent is Group && (record.Parent as Group).IsExpanded;

                if (canProcess)
                {
                    //Check whether it is data column or expander cell column
                    if (colIndex < this.Model.ResolveDefaultColumnOffset())
                    {
                        if (colIndex < this.Model.ResolveDefaultColumnOffset() - (this.Model.Table.HasNestedTables ? 2 : 1))
                        {
                            style.Background = this.Model.GetSummaryCaptionBackground();
                            style.Foreground = this.Model.GetSummaryCaptionForeground();
                            style.Borders.Top = new Pen(this.Model.GetSummaryCaptionBackground(), 1d);
                            style.Borders.Right = new Pen(this.Model.GetValueCellBorders().Right != null ? this.Model.GetValueCellBorders().Right.Brush : Brushes.Transparent, 1d);
                        }

                        return this.CreateEmptyCell(style, tableStyleIdentity);
                    }

                    //Creating style for Details view cell.
                    if (record != null)
                    {
                        tableStyleIdentity.TableCellType = GridDataTableCellType.DetailsViewCell;
                        style.CellType = "GridDataBoundTemplate";
                        style.CellItemTemplate = record.DetailsViewTemplate;
                        style.CellValue = record.DetailsViewDataContext;
                        tableStyleIdentity.RecordEntry = record;
                        style.Padding = new CellMarginsInfo(6, 12.5, 6, 12.5);
                        return style;
                    }
                }
            }

            rowIndex = this.Model.ResolveIndexToGroupPosition(cell.RowIndex);

            var displayEl = rowIndex > -1 && rowIndex < this.Table.GroupModel.DisplayElements.Count ? this.Table.GroupModel.DisplayElements[rowIndex] : null;
            if (displayEl == null)
            {
                return this.CreateEmptyCell(style, tableStyleIdentity);
            }

            var isInGroupRecordIndex = displayEl is RecordEntry && !(displayEl is SummaryRecordEntry);
            var isInNestedIndex = displayEl is NestedRecordEntry;
            if (isInNestedIndex)
            {
                if (!this.Model.TableProperties.IsLegacyStyleEnabled)
                {
                    if (colIndex <= displayEl.Level + isHeaderOrNot)
                    {
                        style.Background = this.Model.GetSummaryCaptionBackground();
                        style.Foreground = this.Model.GetSummaryCaptionForeground();
                        style.Borders.Top = new Pen(this.Model.GetSummaryCaptionBackground(), 1d);
                        style.Borders.Right = new Pen(this.Model.GetValueCellBorders().Right != null ? this.Model.GetValueCellBorders().Right.Brush : Brushes.Transparent, 1d);
                        return this.CreateEmptyCell(style, tableStyleIdentity);
                    }
                }

                if (colIndex <= isHeaderOrNot + 2)
                {
                    return this.CreateEmptyCell(style, tableStyleIdentity);
                }

                var nested = displayEl as NestedRecordEntry;
                var record = nested.Parent as GridDataRecord;
                return this.CreateStyleForNestedGrid(cell, style, tableStyleIdentity, record);
            }

            var groupKey = displayEl as Group;
            tableStyleIdentity.Group = groupKey;
            var isInGroupKey = groupKey != null;
            var tLevel = this.TableProperties.ShowRowHeader ? displayEl.Level : displayEl.Level - 1;
            tLevel = this.TableProperties.ShowGroupCaptionPlusMinus ? tLevel + 1 : tLevel;
            if (this.TableProperties.ShowGroupCaptionPlusMinus && isInGroupKey && cell.ColumnIndex == (this.TableProperties.ShowRowHeader ? displayEl.Level : displayEl.Level - 1))
            {
                tableStyleIdentity.TableCellType = GridDataTableCellType.GroupCaptionPlusMinusCell;
                style.CellType = "ExpandCollapseCell";
                if (this.Model.TableProperties.IsLegacyStyleEnabled)
                {
                    // Only top border applicable so double it to match cell border thickness
                    var topThickness = this.Model.GetValueCellBorders().Top != null ? this.Model.GetValueCellBorders().Top.Thickness : 0.275;
                    if ((groupKey.Level == 1) && (tableStyleIdentity.Group.IsBottomLevel))
                    {
                        topThickness *= 2;
                    }
                    style.Borders.Top = new Pen(this.Model.GetValueCellBorders().Top != null ? this.Model.GetValueCellBorders().Top.Brush : Brushes.Transparent, topThickness);
                    style.Borders.Right = new Pen(); 
                }
                else
                {
                    style.Borders.Bottom = new Pen(this.Model.GetSummaryCaptionBackground(), 1d);
                    style.Borders.Left = null;
                    style.Background = this.Model.GetSummaryCaptionBackground();
                    style.Foreground = this.Model.GetSummaryCaptionForeground();
                    style.Borders.Top = new Pen(this.Model.GetValueCellBorders().Top != null ? this.Model.GetValueCellBorders().Top.Brush : Brushes.Transparent, 1);
                    style.Borders.Right = new Pen(this.Model.GetSummaryCaptionBackground(), 1d);
                }
                style.CellValue = groupKey.IsExpanded ? true : false;
            }
            else if (isInGroupKey)
            {
                var columnHeaderNameValue = string.Empty;
                if (groupKey.Level > 0)
                {
                    var groupedColumn = this.Model.View.GroupDescriptions[groupKey.Level - 1] as PropertyGroupDescription;
                    if (groupedColumn != null)
                    {
                        var visibleCol = this.TableProperties.VisibleColumns.Where(v => v.MappingName == groupedColumn.PropertyName).FirstOrDefault();
                        if (visibleCol != null)
                        {
                            columnHeaderNameValue = visibleCol.HeaderText != null ? visibleCol.HeaderText : visibleCol.MappingName;
                        }
                    }
                }

                GridDataSummaryRow summaryRow = null;
                // var isBottomLevel = groupKey.IsBottomLevel; Unused local variable
                var showGroupSummaryInCaption = this.TableProperties.ShowGroupSummaryInCaption && this.TableProperties.CaptionSummaryRow != null;
                if (showGroupSummaryInCaption)
                {
                    summaryRow = this.TableProperties.CaptionSummaryRow;
                }
                if (!showGroupSummaryInCaption)
                {
                    if (cell.ColumnIndex == tLevel)
                    {
                        style.Background = this.Model.GetSummaryCaptionBackground();
                        style.Foreground = this.Model.GetSummaryCaptionForeground();
                        style.Borders.Top = this.Model.GetValueCellBorders().Top ?? new Pen(Brushes.Transparent, 0.2d);
                        style.Borders.Right = new Pen(this.Model.GetValueCellBorders().Right != null ? this.Model.GetValueCellBorders().Right.Brush : Brushes.Transparent, style.Borders.Top.Thickness * 2);
                        style.Borders.Bottom = this.Model.GetValueCellBorders().Bottom;
                        style.Font = this.Model.GetSummaryCaptionFont();
                        tableStyleIdentity.TableCellType = GridDataTableCellType.GroupCaptionCell;
                        style.CellType = "Static";

#if !SILVERLIGHT
                        if (this.TableProperties.CaptionSummaryRow != null && this.TableProperties.CaptionSummaryRow.Converter != null)
                            style.Text = this.Table.GroupModel.GetGroupCaptionText(groupKey, this.TableProperties.GroupCaptionText ?? GridDataControl.GroupCaptionConstant, columnHeaderNameValue, this.TableProperties.CaptionSummaryRow.Converter);
                        else
#endif
                        style.Text = this.Table.GroupModel.GetGroupCaptionText(groupKey, this.TableProperties.GroupCaptionText ?? GridDataControl.GroupCaptionConstant, columnHeaderNameValue);

                        if (!this.Model.TableProperties.IsLegacyStyleEnabled)
                        {
                            style.Borders.Top = this.Model.GetValueCellBorders().Top != null ? new Pen(this.Model.GetValueCellBorders().Top.Brush, 1) : new Pen(Brushes.Transparent, 0.2d);
                            style.Borders.Left = new Pen(this.Model.GetSummaryCaptionBackground(), 1d);
                        }
                    }
                    else
                    {
                        if (!this.Model.TableProperties.IsLegacyStyleEnabled)
                        {
                            style.Borders.Top = new Pen(this.Model.GetSummaryCaptionBackground(), 1d);
                            style.Borders.Bottom = new Pen(this.Model.GetSummaryCaptionBackground(), 1d);
                            style.Borders.Left = null;
                            style.Borders.Right = new Pen(this.Model.GetValueCellBorders().Right != null ? this.Model.GetValueCellBorders().Right.Brush : Brushes.Transparent, 1d);
                            style.Background = this.Model.GetSummaryCaptionBackground();
                            style.Foreground = this.Model.GetSummaryCaptionForeground();
                        }

                        style = this.CreateEmptyCell(style, tableStyleIdentity);
                        if (cell.ColumnIndex > tLevel)
                            tableStyleIdentity.TableCellType = GridDataTableCellType.GroupCaptionCell;
                    }
                }
                else if (showGroupSummaryInCaption)
                {
                    var colidx = cell.ColumnIndex;
                    if(colidx > 0)
                        //Below cell.ColumnIndex -1 for checking previous column whether it is hidden or not.
                        //while loop is to check continious hidden columns i.e., first and second columns are hidden.
                        while (this.Model.ColumnWidths[colidx - 1] == 0 && colidx > 0)
                        {
                            tLevel++;
                            colidx--;
                        }
                    if (cell.ColumnIndex == tLevel)
                    {
                        style.Background = this.Model.GetSummaryCaptionBackground();
                        style.Foreground = this.Model.GetSummaryCaptionForeground();
                        style.Borders.Top = this.Model.GetValueCellBorders().Top;
                        style.Borders.Bottom = this.Model.GetValueCellBorders().Bottom;

                        style.Font = this.Model.GetSummaryCaptionFont();
                        if (summaryRow.RowStyle != null)
                        {
                            style.ModifyStyle(summaryRow.RowStyle, Syncfusion.Windows.Styles.StyleModifyType.Override);
                        }
                        style.CellType = "Static";
#if !SILVERLIGHT
                        if (this.TableProperties.CaptionSummaryRow != null && this.TableProperties.CaptionSummaryRow.Converter != null)
                            style.Text = this.Table.GroupModel.GetGroupCaptionText(groupKey, string.Empty, columnHeaderNameValue, this.TableProperties.CaptionSummaryRow.Converter);
                        else
#endif
                            style.Text = this.Table.GroupModel.GetGroupCaptionText(groupKey, this.TableProperties.CaptionSummaryRow.Title, columnHeaderNameValue);
                        tableStyleIdentity.TableCellType = GridDataTableCellType.GroupCaptionSummaryTitleCell;
                        tableStyleIdentity.SummaryRow = this.TableProperties.CaptionSummaryRow;
                    }
                    else
                    {
                        if (!this.Model.TableProperties.IsLegacyStyleEnabled)
                        {
                            style.Background = this.Model.GetSummaryCaptionBackground();
                            style.Foreground = this.Model.GetSummaryCaptionForeground();
                            style.Borders.Top = this.Model.GetValueCellBorders().Top;
                            style.Borders.Bottom = this.Model.GetValueCellBorders().Bottom;
                        }
                        tableStyleIdentity.TableCellType = GridDataTableCellType.GroupCaptionCell;
                       var summaryRecordEntry = groupKey.SummaryDetails as SummaryRecordEntry;
                        style = this.CreateStyleForGroupSummary(cell, style, tableStyleIdentity, summaryRecordEntry, true);
                    }
                }
                else
                {
                    return this.CreateEmptyCell(style, tableStyleIdentity);
                }
            }
            else if (isInGroupRecordIndex)
            {
                tableStyleIdentity.RecordEntry = displayEl as GridDataRecord;
                int recordColumn = this.Model.ResolveDefaultColumnOffset();
                if ((colIndex < recordColumn) && (this.TableProperties.ShowGroupCaptionPlusMinus && this.Table.HasGroups))
                {
                    if (this.Table.HasNestedTables && ((cell.ColumnIndex ==  recordColumn - 1 && !this.Table.HasDetailsView) || (cell.ColumnIndex == recordColumn - 2 && this.Table.HasDetailsView)))
                    {
                        // rowIndex = this.Model.ResolveIndexToRecordPosition(cell.RowIndex);
                        var record = displayEl as GridDataRecord;
                        //var record = this.Table.GetRecordBoundState(rowIndex, false);
                        if (!record.Model.Table.ShouldExpand(record))
                        {
                            return this.CreateEmptyCell(style, tableStyleIdentity);
                        }

                        tableStyleIdentity.TableCellType = GridDataTableCellType.RecordPlusMinusCell;
                        style.CellType = "ExpandCollapseCell";
                        style.CellValue = record != null ? record.IsExpanded : false;
                        return style;
                    }
                    //Adding style for the details view expander cell
                    else if ((cell.ColumnIndex == recordColumn - 1) && this.Table.HasDetailsView)
                    {
                        var record = displayEl as GridDataRecord;

                        tableStyleIdentity.TableCellType = GridDataTableCellType.RecordPlusMinusCell;
                        style.CellType = "ExpandCollapseCell";
                        style.CellValue = record != null && record.IsDetailsViewExpanded;
                        return style;
                    }
                    else if (!this.Model.TableProperties.IsLegacyStyleEnabled)
                    {
                        this.CreateEmptyCell(style, tableStyleIdentity);
                        style.Background = this.Model.GetSummaryCaptionBackground();
                        style.Foreground = this.Model.GetSummaryCaptionForeground();
                        style.Borders.Top = new Pen(this.Model.GetSummaryCaptionBackground(), 1d);
                        style.Borders.Bottom = new Pen(this.Model.GetSummaryCaptionBackground(), 1d);
                        style.Borders.Right = new Pen(this.Model.GetValueCellBorders().Right != null ? this.Model.GetValueCellBorders().Right.Brush : Brushes.Transparent, 1d);
                        style.Borders.Left = null;
                        return style;
                    }

                    // an indent cell for the column header
                    return this.CreateEmptyCell(style, tableStyleIdentity);
                }

                var column = GetColumn(ref cell);
                if (column == null)
                {
                    return this.CreateEmptyCell(style, tableStyleIdentity);
                }
                else
                {
                    tableStyleIdentity.Column = column;
                }

                // it is inside a group index
                if (column.ColumnStyle != null)
                {
                    style.ModifyStyle(column.ColumnStyle, Syncfusion.Windows.Styles.StyleModifyType.Override);
                }

                if (column.CellItemTemplate != null)
                {
                    style.CellType = "GridDataBoundTemplate";
                    style.CellItemTemplate = column.CellItemTemplate;
                    style.CellEditTemplate = column.CellEditItemTemplate;
                }

                if (!isInNestedIndex)
                {
                    if (!column.IsUnbound)
                    {
                        // we get the value from the cache instance
                        var item = (displayEl as RecordEntry).Data;
                        style.CellValue = this.Table.GetValue(item, column.MappingName);
                        style.CellValueType = column.ColumnType;
                        //tableStyleIdentity.RecordIndex = this.Model.Binder.IndexOf(item);
                        tableStyleIdentity.Group = ((GridDataRecord)displayEl).Parent as Group;
                        tableStyleIdentity.TableCellType = GridDataTableCellType.RecordCell;
                        //style.ShowTooltip = this.TableProperties.ShowTooltips ? true : false;
                    }
                    else
                    {
                        //var rec = displayEl as RecordEntry;
                        //var recordIndex = this.Model.View.Records.IndexOf(rec);
                        style = this.CreateStyleForUnboundColumn(cell, rowIndex, column, style, tableStyleIdentity);
                    }
                }
            }
            else if (displayEl is SummaryRecordEntry)
            {
                var summaryRecordEntry = displayEl as SummaryRecordEntry;
                this.CreateStyleForGroupSummary(cell, style, tableStyleIdentity, summaryRecordEntry, false);
            }

            return style;
        }

        private GridDataStyleInfo CreateStyleForGroupSummary(RowColumnIndex cell, GridDataStyleInfo style, GridDataTableStyleInfoIdentity tableStyleIdentity, SummaryRecordEntry summaryRecordEntry, bool isCaptionRow)
        {
            if (summaryRecordEntry == null || summaryRecordEntry.SummaryRow == null)
            {
                return this.CreateEmptyCell(style, tableStyleIdentity);
            }

            var summaryRow = summaryRecordEntry.SummaryRow;
            var group = summaryRecordEntry.Parent as Group;
            var column = this.GetColumn(ref cell);
            if (column == null)
            {
                if (!this.Model.TableProperties.IsLegacyStyleEnabled)
                {
                    style.Borders.Top = new Pen(this.Model.GetSummaryCaptionBackground(), 1d);
                    style.Borders.Bottom = new Pen(this.Model.GetSummaryCaptionBackground(), 1d);
                    style.Borders.Left = null;
                    style.Borders.Right = new Pen(this.Model.GetValueCellBorders().Right != null ? this.Model.GetValueCellBorders().Right.Brush : Brushes.Transparent, 1d);
                    style.Background = this.Model.GetSummaryCaptionBackground();
                    style.Foreground = this.Model.GetSummaryCaptionForeground();
                }
                return this.CreateEmptyCell(style, tableStyleIdentity);
            }
            else
            {
                tableStyleIdentity.Column = column;
                var summaryColumn = summaryRow.SummaryColumns.Where(s => s.MappingName == column.MappingName).FirstOrDefault();
                tableStyleIdentity.SummaryColumn = (GridDataSummaryColumn)summaryColumn;
                tableStyleIdentity.SummaryRow = (GridDataSummaryRow)summaryRow;
            }

            style.Background = isCaptionRow ? this.Model.GetSummaryCaptionBackground() : this.Model.GetSummaryRowBackground();
            style.Foreground = isCaptionRow ? this.Model.GetSummaryCaptionForeground() : this.Model.GetSummaryRowForeground();
            style.Font = isCaptionRow ? this.Model.GetSummaryCaptionFont() : this.Model.GetSummaryRowFont();
            if (((GridDataSummaryRow)summaryRow).RowStyle != null)
            {
                style.ModifyStyle(((GridDataSummaryRow)summaryRow).RowStyle, Syncfusion.Windows.Styles.StyleModifyType.Override);
            }
            if (!summaryRow.ShowSummaryInRow)
            {
                // first priority is for AggregateColumn property
                var summaryColumn = summaryRow.SummaryColumns.OfType<GridDataSummaryColumn>().Where(s => (s.AggregateColumn != null && s.AggregateColumn == column.MappingName)).FirstOrDefault();
                if (summaryColumn == null)
                {
                    // then check with the Mapping property
                    summaryColumn = summaryRow.SummaryColumns.OfType<GridDataSummaryColumn>().Where(s => s.MappingName == column.MappingName && (s.AggregateColumn == null || s.AggregateColumn == string.Empty)).FirstOrDefault();
                }

                if (summaryColumn == null)
                {

                    if (isCaptionRow)
                        return this.CreateGroupCaptionEmptyCell(style, tableStyleIdentity);
                    else                             
                        return this.CreateSummaryEmptyCell(style, tableStyleIdentity);
                    
                }
                else if (summaryColumn != null && summaryColumn.MappingName == null)
                {
                    return this.CreateEmptyCell(style, tableStyleIdentity);
                }

                var summaryValue = SummaryCreator.GetSummaryDisplayText(summaryRecordEntry, summaryColumn.MappingName, this.Model.View, group);
                if (summaryValue != null)
                {
                    if (((GridDataSummaryColumn)summaryColumn).ColumnStyle != null)
                    {
                        style.ModifyStyle(((GridDataSummaryColumn)summaryColumn).ColumnStyle, Syncfusion.Windows.Styles.StyleModifyType.Override);
                    }
                }
                style.CellType = "Static";
                tableStyleIdentity.Group = group;
                if (summaryValue != null)
                {
                    style.CellValue = summaryValue;
                    tableStyleIdentity.TableCellType = isCaptionRow ? GridDataTableCellType.GroupCaptionSummaryRecordCell : GridDataTableCellType.SummaryRecordCell;
                }
                else
                {
                    tableStyleIdentity.TableCellType = isCaptionRow ? GridDataTableCellType.GroupCaptionSummaryEmptyCell : GridDataTableCellType.SummaryEmptyCell;
                }
            }
            else
            {
                var visibleColIndex = this.TableProperties.VisibleColumns.Where(v => !v.IsHidden).ToList().IndexOf(column);
                if (visibleColIndex == 0)
                {
                    style.CellValue = SummaryCreator.GetSummaryDisplayTextForRow(summaryRecordEntry, this.Model.View);
                }
                style.CellType = "Static";
                tableStyleIdentity.Group = group;
                tableStyleIdentity.TableCellType = isCaptionRow ? GridDataTableCellType.GroupCaptionSummaryCoveredCell : GridDataTableCellType.SummaryCoveredCell;
            }

            return style;
        }

        private GridDataVisibleColumn GetColumn(ref RowColumnIndex cell)
        {
            var colIndex = this.Model.ResolvePositionToVisibleColumnIndex(cell.ColumnIndex);
            var column = colIndex > -1 && colIndex < this.TableProperties.VisibleColumns.Count ? this.TableProperties.VisibleColumns[colIndex] : null;
            return column;
        }

        private GridDataStyleInfo CreateStyleForTableSummary(RowColumnIndex cell, GridDataStyleInfo style, GridDataTableStyleInfoIdentity tableStyleIdentity)
        {
            int rowIndex = this.Model.ResolveIndexToSummaryPosition(cell.RowIndex);
            var summaryRecordEntry = this.Model.View != null ? this.Model.View.Records != null ?
                rowIndex < this.Model.View.Records.TableSummaries.Count ?
                this.Model.View.Records.TableSummaries[rowIndex] : null : null : null;
            var column = this.GetColumn(ref cell);
            if (column == null)
            {
                if (!this.Model.TableProperties.IsLegacyStyleEnabled)
                {
                    style.Borders.Top = new Pen(this.Model.GetValueCellBorders().Top.Brush, this.Model.GetValueCellBorders().Top.Thickness);
                    style.Borders.Bottom = new Pen(this.Model.GetValueCellBorders().Bottom.Brush, this.Model.GetValueCellBorders().Bottom.Thickness);
                    style.Borders.Left = null;
                    style.Borders.Right = new Pen(this.Model.GetSummaryRowBackground(), 1d);
                    style.Background = this.Model.GetSummaryRowBackground();
                    style.Foreground = this.Model.GetSummaryRowForeground();
                }
                return this.CreateEmptyCell(style, tableStyleIdentity);
            }
            else
            {
                tableStyleIdentity.Column = column;
            }

            if (summaryRecordEntry == null)
            {
                return this.CreateEmptyCell(style, tableStyleIdentity);
            }

            style.CellType = "Static";
            var summaryIdx = this.Model.ResolveIndexToSummaryPosition(cell.RowIndex);
            // var summaryRow = summaryIdx > -1 && summaryIdx < this.TableProperties.TableSummaryRows.Count ? this.TableProperties.TableSummaryRows[summaryIdx] : null;
            var summaryRow = summaryRecordEntry.SummaryRow as GridDataSummaryRow;
            if (summaryRow == null)
            {
                return this.CreateEmptyCell(style, tableStyleIdentity);
            }

            if (this.Model.GridVisualStyle != null)
            {
                style.Background = this.Model.GetSummaryRowBackground();
                style.Foreground = this.Model.GetSummaryRowForeground();
                style.Font = this.Model.GetSummaryRowFont();
            }

            tableStyleIdentity.SummaryRow = summaryRow;
            if (summaryRow.RowStyle != null)
            {
                style.ModifyStyle(summaryRow.RowStyle, Syncfusion.Windows.Styles.StyleModifyType.Override);
            }

            if (!summaryRow.ShowSummaryInRow)
            {
                // first priority is for AggregateColumn property
                var summaryColumn = summaryRow.SummaryColumns.OfType<GridDataSummaryColumn>().Where(s => (s.AggregateColumn != null && s.AggregateColumn == column.MappingName)).FirstOrDefault();
                if (summaryColumn == null)
                {
                    // then check with the Mapping property
                    summaryColumn = summaryRow.SummaryColumns.OfType<GridDataSummaryColumn>().Where(s => s.MappingName == column.MappingName && (s.AggregateColumn == null || s.AggregateColumn == string.Empty)).FirstOrDefault();
                }

                if (summaryColumn == null || summaryColumn.MappingName == null)
                {
                    return this.CreateSummaryEmptyCell(style, tableStyleIdentity);
                }
                //else if (summaryColumn != null && summaryColumn.MappingName == null)
                //{
                //    return this.CreateEmptyCell(style, tableStyleIdentity);
                //}

                tableStyleIdentity.SummaryColumn = (GridDataSummaryColumn)summaryColumn;
                if (((GridDataSummaryColumn)summaryColumn).ColumnStyle != null)
                {
                    style.ModifyStyle(((GridDataSummaryColumn)summaryColumn).ColumnStyle, Syncfusion.Windows.Styles.StyleModifyType.Override);
                }

                var summaryValue = SummaryCreator.GetSummaryDisplayText(summaryRecordEntry, summaryColumn.MappingName, this.Model.View);
                if (summaryValue != null)
                {
                    style.CellValue = summaryValue;
                    tableStyleIdentity.TableCellType = GridDataTableCellType.SummaryRecordCell;
                }
                else
                {
                    tableStyleIdentity.TableCellType = GridDataTableCellType.SummaryEmptyCell;
                }
            }
            else
            {
                var visibleColIndex = this.TableProperties.VisibleColumns.Where(v => !v.IsHidden).ToList().IndexOf(column);
                if (visibleColIndex == 0)
                {
                    style.CellValue = this.Model.GetFormattedTableSummaryForCaption(summaryRecordEntry);
                }
                style.CellType = "Static";
                tableStyleIdentity.TableCellType = GridDataTableCellType.SummaryCoveredCell;
            }
            return style;
        }
    }
}
