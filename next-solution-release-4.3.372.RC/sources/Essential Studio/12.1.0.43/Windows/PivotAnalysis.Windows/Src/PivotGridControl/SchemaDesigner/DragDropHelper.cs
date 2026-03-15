#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.PivotAnalysis.Base;
using System.Drawing;
using Syncfusion.Windows.Forms.Grid;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Collections;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    public class DragDropHelper
    {
        PivotGridControl GridControl;
        
        /// <summary>
        /// Constructor function
        /// </summary>
        public DragDropHelper()
        {

        }

        /// <summary>
        /// Constructor for DragDropHelpwe
        /// </summary>
        /// <param name="grid">PivotGridControl</param>
        public DragDropHelper(PivotGridControl grid)
        {
            this.GridControl = grid;
        }
        internal Dictionary<string, PivotItem> GridRowList = new Dictionary<string, PivotItem>();
        internal Dictionary<string, PivotItem> GridColumnList = new Dictionary<string, PivotItem>();
        internal Dictionary<string, FilterExpression> GridFilterList = new Dictionary<string, FilterExpression>();
        internal Dictionary<string, PivotComputationInfo> GridCalcList = new Dictionary<string, PivotComputationInfo>();

        #region Inter DragDrop from grid to schema

        /// <summary>
        /// Returns the Groups mean range
        /// </summary>
        private int GetGroupMeanRange()
        {
            return (2 * this.GridControl.PivotCalculations.Count) + 1;
        }

        /// <summary>
        /// Returns the colcount of grid
        /// </summary>
        /// <returns></returns>
        private int GetGroupColumnMaxRange()
        {
            return (GetGroupMeanRange() + ((2 * this.GridControl.PivotColumns.Count) + 1));
        }

        /// <summary>
        /// Returns only the essential item which is supposed to be removed in their collection from where it was picked. 
        /// If the other collections also have an item with same diemnsion then it will be reset to null
        /// </summary>
        private bool GetPivotItem(GridControlBase sourceControl, out PivotItem colItem, out PivotItem rowItem,
            out PivotComputationInfo compItem, out FilterExpression exp, GridStyleInfo style, int[] indices, object[] arrayItems)
        {
            int dragIndex = indices[0];
            int dropIndex = indices[1];

            colItem = (PivotItem)arrayItems[0];
            rowItem = (PivotItem)arrayItems[1];
            compItem = (PivotComputationInfo)arrayItems[2];
            exp = (FilterExpression)arrayItems[3];

            if (sourceControl is FilterBar)
            {
                // dont remove , its because when filter is dropped onto something then 
            }
            else if (sourceControl is GroupBar)
            {
                if (dragIndex > -1 && dropIndex > -1)
                {
                    if (dragIndex < ((2 * this.GridControl.PivotCalculations.Count) + 1))
                    {
                        if (colItem != null)
                            colItem = null;
                    }
                    else if (dragIndex > ((2 * this.GridControl.PivotCalculations.Count) + 1) && dragIndex < (((2 * this.GridControl.PivotCalculations.Count) + 1) + (2 * this.GridControl.PivotColumns.Count)))
                    {
                        if (compItem != null)
                            compItem = null;
                    }
                    if (exp != null)
                        exp = null;
                    if (rowItem != null)
                        rowItem = null;
                }
                else
                {
                    if (style.Tag != null)
                    {
                        switch (style.Tag.ToString())
                        {
                            case "PivotRows":
                                break;
                            case "PivotColumns":
                                if (compItem != null)
                                    compItem = null;
                                break;
                            case "PivotFilters":
                                break;
                            case "PivotCalculations":
                                if (colItem != null)
                                    colItem = null;
                                break;
                        }
                        if (exp != null)
                            exp = null;
                        if (rowItem != null)
                            rowItem = null;
                    }
                }

                
            }
            else if (sourceControl is RowGroupBar)
            {
                if (colItem != null)
                    colItem = null;
                if (compItem != null)
                    compItem = null;
                if (exp != null)
                    exp = null;
            }

            return true;
        }

        /// <summary>
        /// Interchange the order of items within the same collection
        /// </summary>
        /// <param name="dragIndex">index where the item was dragged</param>
        /// <param name="dropIndex">index where the item was dropped</param>
        /// <param name="collection">dropped collection</param>
        /// <returns></returns>
        private bool InterChangeItems(int dragIndex, int dropIndex, string collection)
        {
            int n1, n2;
            if (dragIndex > -1 && dropIndex > -1)
            {
                switch (collection)
                {
                    case "columns":
                        if (dragIndex > GetGroupMeanRange() && dropIndex > GetGroupMeanRange())
                        {
                            if (dropIndex >= GetGroupColumnMaxRange())
                                dropIndex = GetGroupColumnMaxRange() - 1;
                            n1 = (dragIndex - (GetGroupMeanRange() + 1)) / 2;
                            n2 = (dropIndex - (GetGroupMeanRange() + 1)) / 2;
                            this.GridControl.PivotColumns.Move(n1, n2);
                        }
                        break;
                    case "rows":
                        if (dropIndex > 1 && dragIndex > 1)
                        {
                            n1 = (dragIndex / 2) - 1;
                            n2 = (dropIndex / 2) - 1;
                            this.GridControl.PivotRows.Move(n1, n2);
                        }
                        break;
                    case "calculation":
                        if (dragIndex <= GetGroupMeanRange() && dropIndex <= GetGroupMeanRange())
                        {
                            n1 = (dragIndex / 2) - 1;
                            n2 = (dropIndex / 2) - 1;
                            this.GridControl.PivotCalculations.Move(n1, n2);
                        }
                        break;
                    default:
                        break;
                }
                return true;
            }
            else
                return false;

        }
        
        ///////////////////////////////////////////////////////////////
        /// <summary>
        /// Handles to drop the item in the PivotRowCollections. Updates the layout of the Grid and Schema with the changes after modification in the PivotCollections
        /// </summary>
        /// <param name="style">styleinfo for the cell/item to be dropped</param>
        internal void DropinPivotRows(GridStyleInfo style)
        {
            DropinPivotRows(style, -1, -1, null);
        }

        /// <summary>
        /// Handles to drop the item in the PivotColumnCollections. Updates the layout of the Grid and Schema with the changes after modification in the PivotCollections
        /// </summary>
        /// <param name="style">styleinfo for the cell/item to be dropped</param>
        internal void DropinPivotColumns(GridStyleInfo style)
        {
            DropinPivotColumns(style, -1, -1, null);
        }

        /// <summary>
        /// Handles to drop the item in the PivotCalculationCollections. Updates the layout of the Grid and Schema with the changes after modification in the PivotCollections
        /// </summary>
        /// <param name="style">styleinfo for the cell/item to be dropped</param>
        internal void DropinPivotCalculations(GridStyleInfo style)
        {
            DropinPivotCalculations(style, -1, -1, null);
        }

        /// <summary>
        /// Handles to drop the item in the FilterCollections. Updates the layout of the Grid and Schema with the changes after modification in the PivotCollections
        /// </summary>
        /// <param name="style">styleinfo for the cell/item to be dropped</param>
        internal void DropinPivotFilters(GridStyleInfo style)
        {
            DropinPivotFilters(style, -1, null);
        }

        /// <summary>
        /// Removes the item from the pivotcollections. Updates the layout of the Grid and Schema with the changes after modification in the PivotCollections
        /// </summary>
        /// <param name="style">styleinfo for the cell/item to be dropped</param>
        internal void DropInTableFieldList(GridStyleInfo style)
        {
            DropInTableFieldList(style, -1, null);
        }

        ///////////////////////////////////////////////////////////////

        /// <summary>
        /// Handles to drop the item in the PivotRowCollections. Updates the layout of the Grid and Schema with the changes after modification in the PivotCollections
        /// </summary>
        /// <param name="style">styleinfo for the cell/item to be dropped</param>
        /// <param name="sourceControl">Location of the item where it was dragged</param>
        internal void DropinPivotRows(GridStyleInfo style, GridControlBase sourceControl)
        {
            DropinPivotRows(style, -1, -1, sourceControl);
        }

        /// <summary>
        /// Handles to drop the item in the PivotColumnCollections. Updates the layout of the Grid and Schema with the changes after modification in the PivotCollections
        /// </summary>
        /// <param name="style">styleinfo for the cell/item to be dropped</param>
        /// <param name="sourceControl">Location of the item where it was dragged</param>
        internal void DropinPivotColumns(GridStyleInfo style, GridControlBase sourceControl)
        {
            DropinPivotColumns(style, -1, -1, sourceControl);
        }

        /// <summary>
        /// Handles to drop the item in the PivotCalculationCollections. Updates the layout of the Grid and Schema with the changes after modification in the PivotCollections
        /// </summary>
        /// <param name="style">styleinfo for the cell/item to be dropped</param>
        /// <param name="sourceControl">Location of the item where it was dragged</param>
        internal void DropinPivotCalculations(GridStyleInfo style, GridControlBase sourceControl)
        {
            DropinPivotCalculations(style, -1, -1, sourceControl);
        }

        /// <summary>
        /// Handles to drop the item in the FilterCollections. Updates the layout of the Grid and Schema with the changes after modification in the PivotCollections
        /// </summary>
        /// <param name="style">styleinfo for the cell/item to be dropped</param>
        /// <param name="sourceControl">Location of the item where it was dragged</param>
        internal void DropinPivotFilters(GridStyleInfo style, GridControlBase sourceControl)
        {
            DropinPivotFilters(style, -1, sourceControl);
        }

        /// <summary>
        /// Removes the item from the pivotcollections. Updates the layout of the Grid and Schema with the changes after modification in the PivotCollections
        /// </summary>
        /// <param name="style">styleinfo for the cell/item to be dropped</param>
        /// <param name="sourceControl">Location of the item where it was dragged</param>
        internal void DropInTableFieldList(GridStyleInfo style, GridControlBase sourceControl)
        {
            DropInTableFieldList(style, -1, sourceControl);
        }

        ///////////////////////////////////////////////////////////////

        /// <summary>
        /// Handles to drop the item in the PivotRowCollections. Updates the layout of the Grid and Schema with the changes after modification in the PivotCollections
        /// </summary>
        /// <param name="style">styleinfo for the cell/item to be dropped</param>
        /// <param name="sourceControl">Location of the item where it was dragged</param>
        internal void DropinPivotRows(GridStyleInfo style, int dragIndex, int dropIndex, GridControlBase sourceControl)
        {
            AssignItemLists();
            PivotItem rowItem, colItem;
            PivotComputationInfo compItem;
            FilterExpression exp;
            string value;
            int[] indices = new int[2];
            indices[0] = dragIndex;
            indices[1] = dropIndex;
            if (style.Tag == null || style.Tag.ToString() != "TableFieldList")
            {
                GridRowList.TryGetValue(style.Text, out rowItem);
                GridColumnList.TryGetValue(style.Text, out colItem);
                GridCalcList.TryGetValue(style.Text, out compItem);
                GridFilterList.TryGetValue(style.Text, out exp);
                value = style.Text;
            }
            else
            {
                if (sourceControl is RowGroupBar)
                {
                    foreach (PivotItem item in this.GridControl.PivotColumns)
                    {
                        if (style.Description == item.FieldMappingName)
                            style.Description = item.FieldHeader;
                    }
                }
                GridRowList.TryGetValue(style.Description, out rowItem);
                GridColumnList.TryGetValue(style.Description, out colItem);
                GridCalcList.TryGetValue(style.Description, out compItem);
                GridFilterList.TryGetValue(style.Description, out exp);
                value = style.Description;
                if (colItem != null && GridColumnList.Count == 1 || rowItem != null && GridRowList.Count == 1 || compItem != null && GridCalcList.Count == 1 || exp != null && GridFilterList.Count == 1)
                {
                    return;
                }
            }

            if (sourceControl != null)
            {
                object[] arrayItems = new object[4];
                arrayItems[0] = colItem;
                arrayItems[1] = rowItem;
                arrayItems[2] = compItem;
                arrayItems[3] = exp;
                GetPivotItem(sourceControl, out colItem, out rowItem, out compItem, out exp, style, indices, arrayItems);
            }
            bool exist = IsPivotRowsExist(value);
            if (colItem != null)
            {
                if (!exist)
                {
                    if (dropIndex > -1)//
                    {
                        if ((dropIndex == (this.GridControl.PivotRows.Count * 2) - 1))
                        {
                            this.GridControl.PivotRows.Insert(dropIndex - (this.GridControl.PivotRows.Count), colItem);
                        }
                        else if (dropIndex == (this.GridControl.PivotRows.Count * 2))
                            this.GridControl.PivotRows.Add(colItem);
                        else
                            this.GridControl.PivotRows.Insert((dropIndex / 2) - 1, colItem);
                    }
                    else
                        this.GridControl.PivotRows.Add(colItem);
                    this.GridControl.PivotColumns.Remove(colItem);
                }
                else
                    InterChangeItems(dragIndex, dropIndex, "rows");

            }
            if (compItem != null)
            {

                if (this.GridControl.PivotRows != null)
                    exist = IsPivotRowsExist(value);
                if (!exist)
                {
                    PivotItem newItem = new PivotItem();
                    newItem.FieldMappingName = compItem.FieldName;
                    newItem.FieldHeader = compItem.FieldHeader;
                    if (dropIndex > -1)//
                    {
                        if ((dropIndex == (this.GridControl.PivotRows.Count * 2) - 1))
                        {
                            this.GridControl.PivotRows.Insert(dropIndex - (this.GridControl.PivotRows.Count), newItem);
                        }
                        else if (dropIndex == (this.GridControl.PivotRows.Count * 2))
                            this.GridControl.PivotRows.Add(newItem);
                        else
                            this.GridControl.PivotRows.Insert((dropIndex / 2) - 1, newItem);
                    }
                    else
                        this.GridControl.PivotRows.Add(newItem);

                    this.GridControl.TableControl.PivotCalculations.Remove(compItem);
                }
                else
                    InterChangeItems(dragIndex, dropIndex, "rows");
            }
            if (exp != null)
            {
                if (this.GridControl.PivotRows != null)
                    exist = IsPivotRowsExist(value);
                if (!exist)
                {
                    PivotItem newItem = new PivotItem();
                    if (!string.IsNullOrEmpty(exp.DimensionHeader))
                        newItem.FieldHeader = exp.DimensionHeader;
                    newItem.FieldMappingName = exp.DimensionName;

                    if (dropIndex > -1)//
                    {
                        if ((dropIndex == (this.GridControl.PivotRows.Count * 2) - 1))
                        {
                            this.GridControl.PivotRows.Insert(dropIndex - (this.GridControl.PivotRows.Count), newItem);
                        }
                        else if (dropIndex == (this.GridControl.PivotRows.Count * 2))
                            this.GridControl.PivotRows.Add(newItem);
                        else
                            this.GridControl.PivotRows.Insert((dropIndex / 2) - 1, newItem);
                    }
                    else
                        this.GridControl.PivotRows.Add(newItem);

                    this.GridControl.Filters.Remove(exp);
                }
                else
                    if (!InterChangeItems(dragIndex, dropIndex, "rows"))
                        this.GridControl.Filters.Remove(exp);

                if (FilterDropDown.FilterDimensions.Contains(exp.DimensionName))
                    FilterDropDown.FilterDimensions.Remove(exp.DimensionName);
            }
            if (compItem == null && colItem == null && exp == null)
            {
                if (this.GridControl.PivotRows != null)
                    exist = IsPivotRowsExist(value);
                if (rowItem == null)
                {
                    if (!exist)
                    {
                        PivotItem newItem = new PivotItem();
                        if (style.Tag != null)
                            newItem.FieldHeader = style.Tag.ToString() == "TableFieldList" ? style.Description : style.Text;
                        newItem.FieldMappingName = newItem.FieldHeader;
                        this.GridControl.TableControl.fieldNameCollectionTablelist.TryGetValue(style.Description, out newItem);
                        if (newItem == null)
                            this.GridControl.TableControl.completeTablelist.TryGetValue(style.Description, out newItem);
                        this.GridControl.PivotRows.Add(newItem);
                    }
                }
                else
                    InterChangeItems(dragIndex, dropIndex, "rows");
            }

            RefreshGridSchemaLayout();
            if (this.GridControl.pivotSchemaDesigner != null)
                this.GridControl.pivotSchemaDesigner.PopulateTableFieldList();
        }
        /// <summary>
        /// To Check whether the passed key value of PivotRows matches the FieldHeader/FieldMappingName.
        /// </summary>
        /// <param name="value">Key Value to be matched with the FieldMappingName/FieldHeader</param>
        /// <returns>bool</returns>
        private bool IsPivotRowsExist(string value)
        {
            bool exist = (from o in this.GridControl.PivotRows.Where(l => l.FieldHeader == value)
                          select o).Any();
            if (!exist)
                exist = (from o in this.GridControl.PivotRows.Where(l => l.FieldMappingName == value)
                         select o).Any();
            return exist;
        }        
        /// <summary>
        /// Handles to drop the item in the PivotColumnCollections. Updates the layout of the Grid and Schema with the changes after modification in the PivotCollections
        /// </summary>
        /// <param name="style">styleinfo for the cell/item to be dropped</param>
        /// <param name="sourceControl">Location of the item where it was dragged</param>
        internal void DropinPivotColumns(GridStyleInfo style, int dragIndex, int dropIndex, GridControlBase sourceControl)
        {
            AssignItemLists();
            PivotItem rowItem, colItem;
            PivotComputationInfo compItem;
            FilterExpression exp;
            string value;
            int[] indices = new int[2];
            indices[0] = dragIndex;
            indices[1] = dropIndex;
            if (style.Tag == null || style.Tag.ToString() != "TableFieldList")
            {
                GridRowList.TryGetValue(style.Text, out rowItem);
                GridColumnList.TryGetValue(style.Text, out colItem);
                GridCalcList.TryGetValue(style.Text, out compItem);
                GridFilterList.TryGetValue(style.Text, out exp);
                value = style.Text;
            }
            else
            {
                GridRowList.TryGetValue(style.Description, out rowItem);
                GridColumnList.TryGetValue(style.Description, out colItem);
                GridCalcList.TryGetValue(style.Description, out compItem);
                GridFilterList.TryGetValue(style.Description, out exp);
                value = style.Description;
                if (colItem != null && GridColumnList.Count == 1 || rowItem != null && GridRowList.Count == 1 || compItem != null && GridCalcList.Count == 1 || exp != null && GridFilterList.Count == 1)
                {
                    return;
                }
            }

            if (sourceControl != null)
            {
                object[] arrayItems = new object[4];
                arrayItems[0] = colItem;
                arrayItems[1] = rowItem;
                arrayItems[2] = compItem;
                arrayItems[3] = exp;
                GetPivotItem(sourceControl, out colItem, out rowItem, out compItem, out exp, style, indices, arrayItems);
            }

            bool exist = IsPivotColumnsExist(value);
            if (compItem != null)
            {
                if (!exist)
                {
                    PivotItem newItem = new PivotItem();
                    newItem.FieldMappingName = compItem.FieldName;
                    newItem.FieldHeader = compItem.FieldHeader;
                    if (dropIndex > -1 && dropIndex < (GetGroupMeanRange() + ((2 * this.GridControl.PivotColumns.Count) + 1)))
                    {
                        this.GridControl.PivotColumns.Insert((dropIndex - (GetGroupMeanRange() + 1)) / 2, newItem);
                    }
                    else
                        this.GridControl.PivotColumns.Add(newItem);
                    this.GridControl.TableControl.PivotCalculations.Remove(compItem);

                }
                else
                    InterChangeItems(dragIndex, dropIndex, "columns");

            }
            if (rowItem != null)
            {
                exist = IsPivotColumnsExist(value);
                if (sourceControl is FilterBar)
                    exist = true;

                if (!exist)
                {
                    if (dropIndex > -1 && dropIndex < (GetGroupMeanRange() + ((2 * this.GridControl.PivotColumns.Count) + 1)))
                    {
                        this.GridControl.PivotColumns.Insert(/*(dropIndex / 2) - 1*/(dropIndex - (GetGroupMeanRange() + 1)) / 2, rowItem);
                    }
                    else
                        this.GridControl.PivotColumns.Add(rowItem);
                    this.GridControl.TableControl.PivotRows.Remove(rowItem);
                }
                else
                {
                    InterChangeItems(dragIndex, dropIndex, "columns");
                }

            }
            if (exp != null)
            {
                exist = IsPivotColumnsExist(value);
                if (!exist)
                {
                    PivotItem newItem = new PivotItem();
                    if (!string.IsNullOrEmpty(exp.DimensionHeader))
                        newItem.FieldHeader = exp.DimensionHeader;
                    newItem.FieldMappingName = exp.DimensionName;
                    if (dropIndex > -1 && dropIndex < (GetGroupMeanRange() + ((2 * this.GridControl.PivotColumns.Count) + 1)))
                    {
                        this.GridControl.PivotColumns.Insert((dropIndex - (GetGroupMeanRange() + 1)) / 2, newItem);
                    }
                    else
                        this.GridControl.PivotColumns.Add(newItem);
                    this.GridControl.Filters.Remove(exp);
                }
                else
                    if (!InterChangeItems(dragIndex, dropIndex, "columns"))
                        this.GridControl.Filters.Remove(exp);
                
                if (FilterDropDown.FilterDimensions.Contains(exp.DimensionName))
                    FilterDropDown.FilterDimensions.Remove(exp.DimensionName);
            }
            if (compItem == null && rowItem == null && exp == null)
            {
                exist = IsPivotColumnsExist(value);
                if (colItem == null)
                {
                    PivotItem newItem = new PivotItem();
                    newItem.FieldHeader = style.Tag.ToString() == "TableFieldList" ? style.Description : style.Text;
                    this.GridControl.TableControl.fieldNameCollectionTablelist.TryGetValue(style.Description, out newItem);
                    if (newItem == null)
                        this.GridControl.TableControl.completeTablelist.TryGetValue(style.Description, out newItem);
                    this.GridControl.PivotColumns.Add(newItem);
                }
                else
                    InterChangeItems(dragIndex, dropIndex, "columns");
            }

            RefreshGridSchemaLayout();

            if (this.GridControl.pivotSchemaDesigner != null)
                this.GridControl.pivotSchemaDesigner.PopulateTableFieldList();
        }
        /// <summary>
        /// To Check whether the passed key value of PivotColumns matches the FieldHeader/FieldMappingName.
        /// </summary>
        /// <param name="value">Key Value to be matched with the FieldMappingName/FieldHeader</param>
        /// <returns>bool</returns>
        private bool IsPivotColumnsExist(string value)
        {
            bool exist = (from o in this.GridControl.PivotColumns.Where(l => l.FieldHeader == value)
                          select o).Any();
            if (!exist)
                exist = (from o in this.GridControl.PivotColumns.Where(l => l.FieldMappingName == value)
                         select o).Any();
            return exist;
        }
        /// <summary>
        /// Handles to drop the item in the PivotCalculationCollections. Updates the layout of the Grid and Schema with the changes after modification in the PivotCollections
        /// </summary>
        /// <param name="style">styleinfo for the cell/item to be dropped</param>
        /// <param name="sourceControl">Location of the item where it was dragged</param>
        internal void DropinPivotCalculations(GridStyleInfo style, int dragIndex, int dropIndex, GridControlBase sourceControl)
        {
            AssignItemLists();
            PivotItem rowItem, colItem;
            PivotComputationInfo compItem;
            FilterExpression exp;
            string value;
            int[] indices = new int[2];
            indices[0] = dragIndex;
            indices[1] = dropIndex;
            int groupMeanRange = (2 * this.GridControl.PivotCalculations.Count) + 1;
            if (style.Tag == null || style.Tag.ToString() != "TableFieldList")
            {
                GridRowList.TryGetValue(style.Text, out rowItem);
                GridColumnList.TryGetValue(style.Text, out colItem);
                GridCalcList.TryGetValue(style.Text, out compItem);
                GridFilterList.TryGetValue(style.Text, out exp);
                value = style.Text;
            }
            else
            {
                GridRowList.TryGetValue(style.Description, out rowItem);
                GridColumnList.TryGetValue(style.Description, out colItem);
                GridCalcList.TryGetValue(style.Description, out compItem);
                GridFilterList.TryGetValue(style.Description, out exp);
                value = style.Description;
                if (colItem != null && GridColumnList.Count == 1 || rowItem != null && GridRowList.Count == 1 || compItem != null && GridCalcList.Count == 1 || exp != null && GridFilterList.Count == 1)
                {
                    return;
                }
            }

            if (sourceControl != null)
            {
                object[] arrayItems = new object[4];
                arrayItems[0] = colItem;
                arrayItems[1] = rowItem;
                arrayItems[2] = compItem;
                arrayItems[3] = exp;
                GetPivotItem(sourceControl, out colItem, out rowItem, out compItem, out exp, style, indices, arrayItems);
            }
            bool exist = PivotCalculationExist(value);

            if (colItem != null)
            {
                if (!exist)
                {
                    PivotComputationInfo newCompItem = new PivotComputationInfo();
                    newCompItem.FieldHeader = colItem.FieldHeader;
                    newCompItem.FieldName = colItem.FieldMappingName;

                    if (dropIndex > -1 && dropIndex < groupMeanRange)
                        this.GridControl.PivotCalculations.Insert((dropIndex / 2 - 1), newCompItem);
                    else
                        this.GridControl.PivotCalculations.Add(newCompItem);

                    this.GridControl.PivotColumns.Remove(colItem);
                }
                else
                    InterChangeItems(dragIndex, dropIndex, "calculation");
            }
            if (rowItem != null)
            {
                if (!(sourceControl is RowGroupBar))
                    exist = (from o in this.GridControl.PivotCalculations.Where(l => l.FieldName == value)
                             select o).Any();
                if (!exist)
                {
                    PivotComputationInfo newCompItem = new PivotComputationInfo();
                    newCompItem.FieldHeader = rowItem.FieldHeader;
                    newCompItem.FieldName = rowItem.FieldMappingName;

                    if (dropIndex > -1 && dropIndex < groupMeanRange)
                    {
                        this.GridControl.PivotCalculations.Insert((dropIndex / 2 - 1), newCompItem);
                    }
                    else
                        this.GridControl.PivotCalculations.Add(newCompItem);

                    this.GridControl.PivotRows.Remove(rowItem);
                }
                else
                    InterChangeItems(dragIndex, dropIndex, "calculation");
            }
            if (exp != null)
            {
                exist = PivotCalculationExist(value);
                if (!exist)
                {
                    PivotComputationInfo newCompItem = new PivotComputationInfo();
                    newCompItem.FieldHeader = exp.DimensionHeader;
                    newCompItem.FieldName = exp.DimensionName;

                    if (dropIndex > -1 && dropIndex < groupMeanRange)
                        this.GridControl.PivotCalculations.Insert((dropIndex / 2 - 1), newCompItem);
                    else
                        this.GridControl.PivotCalculations.Add(newCompItem);

                    this.GridControl.Filters.Remove(exp);
                }
                else
                    if (!InterChangeItems(dragIndex, dropIndex, "calculation"))
                        this.GridControl.Filters.Remove(exp);

                if (FilterDropDown.FilterDimensions.Contains(exp.DimensionName))
                    FilterDropDown.FilterDimensions.Remove(exp.DimensionName);
            }
            if (colItem == null && rowItem == null && exp == null)
            {
                exist = PivotCalculationExist(value);

                if (compItem == null)
                {
                    if (!exist)
                    {
                        PivotComputationInfo newCompItem = new PivotComputationInfo();
                        newCompItem.FieldHeader = style.Tag != null && style.Tag.ToString() == "TableFieldList" ? style.Description : style.Text;
                        newCompItem.FieldName = newCompItem.FieldName;

                        this.GridControl.TableControl.pivotComputationCollection.TryGetValue(style.Description, out newCompItem);
                        if (newCompItem == null)
                        {
                            PivotComputationInfo extCompItem = new PivotComputationInfo();
                            PivotItem newItem = new PivotItem();
                            this.GridControl.TableControl.completeTablelist.TryGetValue(style.Description, out newItem);
                            if (newItem.FieldHeader != null)
                                extCompItem.FieldHeader = newItem.FieldHeader.ToString();
                            else
                                extCompItem.FieldHeader = newItem.FieldMappingName.ToString();
                            extCompItem.FieldName = newItem.FieldMappingName.ToString();
                            this.GridControl.PivotCalculations.Add(extCompItem);
                        }
                        else
                            this.GridControl.PivotCalculations.Add(newCompItem);
                    }
                }
                else
                    InterChangeItems(dragIndex, dropIndex, "calculation");
            }

            RefreshGridSchemaLayout();
            if (this.GridControl.pivotSchemaDesigner != null)
                this.GridControl.pivotSchemaDesigner.PopulateTableFieldList();
        }

        /// <summary>
        /// To Check whether the passed key value of PivotCalculation matches the FieldHeader/FieldMappingName.
        /// </summary>
        /// <param name="value">Key Value to be matched with the FieldMappingName/FieldHeader</param>
        /// <returns>bool</returns>
        private bool PivotCalculationExist(string value)
        {
            bool exist = (from o in this.GridControl.PivotCalculations.Where(l => l.FieldHeader == value)
                          select o).Any();
            if (!exist)
                exist = (from o in this.GridControl.PivotCalculations.Where(l => l.FieldName == value)
                         select o).Any();
            return exist;
        }
        /// <summary>
        /// Handles to drop the item in the FilterCollections. Updates the layout of the Grid and Schema with the changes after modification in the PivotCollections
        /// </summary>
        /// <param name="style">styleinfo for the cell/item to be dropped</param>
        /// <param name="sourceControl">Location of the item where it was dragged</param>
        internal void DropinPivotFilters(GridStyleInfo style, int dragIndex, GridControlBase sourceControl)
        {
            AssignItemLists();
            PivotItem rowItem, colItem;
            PivotComputationInfo compItem;
            FilterExpression exp;
            FilterHelper helper = new FilterHelper(this.GridControl.TableControl);
            int[] indices = new int[2];
            indices[0] = dragIndex;
            //indices[1] = dropIndex;
            string filterDisplayName;

            if (style.Tag == null || style.Tag.ToString() != "TableFieldList")
            {
                GridRowList.TryGetValue(style.Text, out rowItem);
                GridColumnList.TryGetValue(style.Text, out colItem);
                GridCalcList.TryGetValue(style.Text, out compItem);
                GridFilterList.TryGetValue(style.Text, out exp);
            }
            else
            {
                GridRowList.TryGetValue(style.Description, out rowItem);
                GridColumnList.TryGetValue(style.Description, out colItem);
                GridCalcList.TryGetValue(style.Description, out compItem);
                GridFilterList.TryGetValue(style.Description, out exp);
                if (colItem != null && GridColumnList.Count == 1 || rowItem != null && GridRowList.Count == 1 || compItem != null && GridCalcList.Count == 1 || exp != null && GridFilterList.Count == 1)
                {
                    return;
                }
            }

            if (sourceControl != null)
            {
                object[] arrayItems = new object[4];
                arrayItems[0] = colItem;
                arrayItems[1] = rowItem;
                arrayItems[2] = compItem;
                arrayItems[3] = exp;
                GetPivotItem(sourceControl, out colItem, out rowItem, out compItem, out exp, style, indices, arrayItems);
            }
            if (colItem != null)
            {
                int filterIndex = this.GridControl.PivotColumns.IndexOf(colItem);

                PropertyDescriptor descriptor = helper.GetPropertyDescriptor(this.GridControl.TableControl.PivotColumns[filterIndex].FieldMappingName);
                FilterExpression fItem = this.GridControl.TableControl.PivotFilters.Where(f => f.Name == this.GridControl.TableControl.PivotColumns[filterIndex].FieldMappingName).FirstOrDefault();
                FilterItemsCollection filteritem = new FilterItemsCollection();

                filteritem = helper.GetFilterItem(descriptor);
                if (fItem == null)
                {
                    bool isExist = (from o in this.GridControl.TableControl.Filters.Where(l => l.DimensionName == filteritem.Name) select o).Any();
                    if (!string.IsNullOrEmpty(this.GridControl.PivotColumns[filterIndex].FieldHeader))
                        filterDisplayName = this.GridControl.PivotColumns[filterIndex].FieldHeader;
                    else
                        filterDisplayName = this.GridControl.PivotColumns[filterIndex].FieldMappingName;
                    if ((this.GridControl.TableControl.ItemSource is DataView || this.GridControl.TableControl.ItemSource is DataTable) && !isExist)
                    {
                        this.GridControl.TableControl.Filters.Add(new FilterExpression { Name = filteritem.Name, Expression = filteritem.GetFilterExpressionForDataView(), Tag = filteritem, DimensionName = filteritem.Name });
                    }
                    else if ((this.GridControl.TableControl.ItemSource is IEnumerable) && !isExist)
                    {
                        this.GridControl.TableControl.Filters.Add(new FilterExpression { Name = filteritem.Name, Expression = filteritem.GetFilterExpression(true), Tag = filteritem, DimensionName = filteritem.Name, DimensionHeader = filterDisplayName });
                    }
                    else if ((this.GridControl.TableControl.ItemSource is IListSource) && !isExist)
                    {
                        this.GridControl.TableControl.Filters.Add(new FilterExpression { Name = filteritem.Name, Expression = filteritem.GetFilterExpression(false), Tag = filteritem, DimensionName = filteritem.Name, DimensionHeader = filterDisplayName });
                    }
                    this.GridControl.PivotColumns.Remove(colItem);
                }


            }
            if (rowItem != null)
            {

                int filterIndex = this.GridControl.PivotRows.IndexOf(rowItem);

                PropertyDescriptor descriptor = helper.GetPropertyDescriptor(this.GridControl.TableControl.PivotRows[filterIndex].FieldMappingName);
                FilterExpression fItem = this.GridControl.TableControl.PivotFilters.Where(f => f.Name == this.GridControl.TableControl.PivotRows[filterIndex].FieldMappingName).FirstOrDefault();
                FilterItemsCollection filteritem = new FilterItemsCollection();


                filteritem = helper.GetFilterItem(descriptor);

                if (fItem == null)
                {
                    bool isExist = (from o in this.GridControl.TableControl.Filters.Where(l => l.DimensionName == filteritem.Name) select o).Any();
                    if (!string.IsNullOrEmpty(this.GridControl.PivotRows[filterIndex].FieldHeader))
                        filterDisplayName = this.GridControl.PivotRows[filterIndex].FieldHeader;
                    else
                        filterDisplayName = this.GridControl.PivotRows[filterIndex].FieldMappingName;
                    if ((this.GridControl.TableControl.ItemSource is DataView || this.GridControl.TableControl.ItemSource is DataTable) && !isExist)
                    {
                        this.GridControl.TableControl.Filters.Add(new FilterExpression { Name = filteritem.Name, Expression = filteritem.GetFilterExpressionForDataView(), Tag = filteritem, DimensionName = filteritem.Name, DimensionHeader =  filterDisplayName});
                    }
                    else if ((this.GridControl.TableControl.ItemSource is IEnumerable) && !isExist)
                    {
                        this.GridControl.TableControl.Filters.Add(new FilterExpression { Name = filteritem.Name, Expression = filteritem.GetFilterExpression(true), Tag = filteritem, DimensionName = filteritem.Name, DimensionHeader = filterDisplayName });
                    }
                    else if ((this.GridControl.TableControl.ItemSource is IListSource) && !isExist)
                    {
                        this.GridControl.TableControl.Filters.Add(new FilterExpression { Name = filteritem.Name, Expression = filteritem.GetFilterExpression(false), Tag = filteritem, DimensionName = filteritem.Name, DimensionHeader = filterDisplayName });
                    }
                    this.GridControl.PivotRows.Remove(rowItem);
                }


            }
            if (compItem != null)
            {
                int filterIndex = this.GridControl.PivotCalculations.IndexOf(compItem);

                PropertyDescriptor descriptor = helper.GetPropertyDescriptor(this.GridControl.TableControl.PivotCalculations[filterIndex].FieldName);
                FilterExpression fItem = this.GridControl.TableControl.PivotFilters.Where(f => f.Name == this.GridControl.TableControl.PivotCalculations[filterIndex].FieldName).FirstOrDefault();
                FilterItemsCollection filteritem = new FilterItemsCollection();

                filteritem = helper.GetFilterItem(descriptor);

                if (fItem == null)
                {
                    bool isExist = (from o in this.GridControl.TableControl.Filters.Where(l => l.DimensionName == filteritem.Name) select o).Any();
                    if (!string.IsNullOrEmpty(this.GridControl.PivotCalculations[filterIndex].FieldHeader))
                        filterDisplayName = this.GridControl.PivotCalculations[filterIndex].FieldHeader;
                    else
                        filterDisplayName = this.GridControl.PivotCalculations[filterIndex].FieldName;
                    if ((this.GridControl.TableControl.ItemSource is DataView || this.GridControl.TableControl.ItemSource is DataTable) && !isExist)
                    {
                        this.GridControl.TableControl.Filters.Add(new FilterExpression { Name = filteritem.Name, Expression = filteritem.GetFilterExpressionForDataView(), Tag = filteritem, DimensionName = filteritem.Name, DimensionHeader = filterDisplayName });
                    }
                    else if ((this.GridControl.TableControl.ItemSource is IEnumerable) && !isExist)
                    {
                        this.GridControl.TableControl.Filters.Add(new FilterExpression { Name = filteritem.Name, Expression = filteritem.GetFilterExpression(true), Tag = filteritem, DimensionName = filteritem.Name, DimensionHeader = filterDisplayName });
                    }
                    else if ((this.GridControl.TableControl.ItemSource is IListSource) && !isExist)
                    {
                        this.GridControl.TableControl.Filters.Add(new FilterExpression { Name = filteritem.Name, Expression = filteritem.GetFilterExpression(false), Tag = filteritem, DimensionName = filteritem.Name, DimensionHeader = filterDisplayName });
                    }
                    this.GridControl.PivotCalculations.Remove(compItem);                    
                }


            }

            if (colItem == null && rowItem == null && compItem == null)
            {

                if (exp == null)
                {
                    FilterExpression newExp = new FilterExpression(); string fieldName = string.Empty;
                    fieldName = style.Tag != null && style.Tag.ToString() == "TableFieldList" ? style.Description : style.Text;
                    PropertyDescriptor descriptor = helper.GetPropertyDescriptor(fieldName);
                    FilterExpression fItem = this.GridControl.TableControl.PivotFilters.Where(f => f.Name == fieldName).FirstOrDefault();
                    FilterItemsCollection filteritem = new FilterItemsCollection();
                    filteritem = helper.GetFilterItem(descriptor);
                    PivotItem item;

                    if (fItem == null)
                    {
                        this.GridControl.TableControl.collectionTablelist.TryGetValue(filteritem.Name, out item);
                        if (item == null)
                            this.GridControl.TableControl.completeTablelist.TryGetValue(filteritem.Name, out item);
                        filterDisplayName = item.FieldHeader;
                        if (this.GridControl.TableControl.ItemSource is DataView || this.GridControl.TableControl.ItemSource is DataTable)
                        {
                            this.GridControl.TableControl.Filters.Add(new FilterExpression { Name = filteritem.Name, Expression = filteritem.GetFilterExpressionForDataView(), Tag = filteritem, DimensionName = filteritem.Name, DimensionHeader = filterDisplayName });
                        }
                        else if (this.GridControl.TableControl.ItemSource is IEnumerable)
                        {
                            this.GridControl.TableControl.Filters.Add(new FilterExpression { Name = filteritem.Name, Expression = filteritem.GetFilterExpression(true), Tag = filteritem, DimensionName = filteritem.Name , DimensionHeader = filterDisplayName});
                        }
                        else if (this.GridControl.TableControl.ItemSource is IListSource)
                        {
                            this.GridControl.TableControl.Filters.Add(new FilterExpression { Name = filteritem.Name, Expression = filteritem.GetFilterExpression(false), Tag = filteritem, DimensionName = filteritem.Name, DimensionHeader = filterDisplayName});
                        }
                    }

                }

            }
            RefreshGridSchemaLayout();
        }

        /// <summary>
        /// Removes the item from the pivotcollections. Updates the layout of the Grid and Schema with the changes after modification in the PivotCollections
        /// </summary>
        /// <param name="style">styleinfo for the cell/item to be dropped</param>
        /// <param name="sourceControl">Location of the item where it was dragged</param>
        internal void DropInTableFieldList(GridStyleInfo style, int dragIndex, GridControlBase sourceControl)
        {
            AssignItemLists();
            PivotItem rowItem, colItem;
            PivotComputationInfo compItem;
            FilterExpression exp;
            GridRowList.TryGetValue(style.Text, out rowItem);
            GridColumnList.TryGetValue(style.Text, out colItem);
            GridCalcList.TryGetValue(style.Text, out compItem);
            GridFilterList.TryGetValue(style.Text, out exp);
           
            if (sourceControl != null)
            {
                if (sourceControl is GroupBar)
                {
                    if (dragIndex < ((2 * this.GridControl.PivotCalculations.Count) + 1))
                    {
                        if (colItem != null)
                            colItem = null;
                    }
                    else if (dragIndex > ((2 * this.GridControl.PivotCalculations.Count) + 1) && dragIndex < (((2 * this.GridControl.PivotCalculations.Count) + 1) + (2 * this.GridControl.PivotColumns.Count)))
                    {
                        if (compItem != null)
                            compItem = null;
                    }

                    if (rowItem != null)
                        rowItem = null;
                }
                else if (sourceControl is RowGroupBar)
                {
                    if (compItem != null)
                        compItem = null;
                    if (colItem != null)
                        colItem = null;
                }
                
            }


            if (rowItem != null)
            {
                this.GridControl.TableControl.PivotRows.Remove(rowItem);
            }
            if (colItem != null)
            {
                this.GridControl.TableControl.PivotColumns.Remove(colItem);
            }
            if (compItem != null)
            {
                this.GridControl.TableControl.PivotCalculations.Remove(compItem);
            }
            if (exp!= null)
            {
                this.GridControl.TableControl.Filters.Remove(exp);
                if (FilterDropDown.FilterDimensions.Contains(exp.DimensionName))
                    FilterDropDown.FilterDimensions.Remove(exp.DimensionName);
            }
            RefreshGridSchemaLayout();
            if (this.GridControl.pivotSchemaDesigner != null)
                this.GridControl.pivotSchemaDesigner.PopulateTableFieldList();

        }

        /// <summary>
        /// Assign the dictionaries with the pivot items
        /// </summary>
        internal void AssignItemLists()
        {
            ResetItemLists();
            PopulateItemCollectionLists();
        }

        /// <summary>
        /// Populate the dictionaries with the pivot items
        /// </summary>
        internal void PopulateItemCollectionLists()
        {

            foreach (PivotItem item in this.GridControl.PivotRows)
            {
                if (!string.IsNullOrEmpty(item.FieldHeader))
                {
                    if (!GridRowList.ContainsKey(item.FieldHeader))
                        GridRowList.Add(item.FieldHeader, item);
                }
                else if (!GridRowList.ContainsKey(item.FieldMappingName))
                    GridRowList.Add(item.FieldMappingName, item);
            }

            foreach (PivotItem item in this.GridControl.PivotColumns)
            {
                if (!string.IsNullOrEmpty(item.FieldHeader))
                {
                    if (!GridColumnList.ContainsKey(item.FieldHeader))
                        GridColumnList.Add(item.FieldHeader, item);
                }
                else if (!GridColumnList.ContainsKey(item.FieldMappingName))
                    GridColumnList.Add(item.FieldMappingName, item);
            }

            foreach (FilterExpression item in this.GridControl.Filters)
            {
                if (!string.IsNullOrEmpty(item.DimensionHeader))
                {
                    if (!GridFilterList.ContainsKey(item.DimensionHeader))
                        GridFilterList.Add(item.DimensionHeader, item);
                }
                else if (!GridFilterList.ContainsKey(item.DimensionName))
                    GridFilterList.Add(item.DimensionName, item);
            }

            foreach (PivotComputationInfo item in this.GridControl.PivotCalculations)
            {
                if (!string.IsNullOrEmpty(item.FieldHeader))
                {
                    if (!GridCalcList.ContainsKey(item.FieldHeader))
                        GridCalcList.Add(item.FieldHeader, item);
                }
                else if (!GridCalcList.ContainsKey(item.FieldName))
                    GridCalcList.Add(item.FieldName, item);
            }

        }
        
        /// <summary>
        /// Reset the item collection dictionaries
        /// </summary>
        private void ResetItemLists()
        {
            this.GridRowList.Clear();
            this.GridColumnList.Clear();
            this.GridFilterList.Clear();
            this.GridCalcList.Clear();
        }

        /// <summary>
        /// Gets the schema sub item for the corresponding point
        /// </summary>
        internal string GetSchemaContainer(Point location)
        {
            Rectangle tableFieldBounds = this.GridControl.pivotSchemaDesigner.gridColumnList.Bounds;

            if (this.GridControl.pivotSchemaDesigner.pivotRowLists.Bounds.Contains(this.GridControl.pivotSchemaDesigner.pivotRowLists.Parent.PointToClient(Control.MousePosition)))
            {
                return "PivotRows";
            }
            if (this.GridControl.pivotSchemaDesigner.pivotColLists.Bounds.Contains(this.GridControl.pivotSchemaDesigner.pivotColLists.Parent.PointToClient(Control.MousePosition)))
            {
                return "PivotColumns";
            }
            if (this.GridControl.pivotSchemaDesigner.pivotCalcLists.Bounds.Contains(this.GridControl.pivotSchemaDesigner.pivotCalcLists.Parent.PointToClient(Control.MousePosition)))
            {
                return "PivotCalculations";
            }
            if (this.GridControl.pivotSchemaDesigner.RectangleToScreen(tableFieldBounds).Contains(Control.MousePosition))
            {
                return "TableFieldList";
            }
            if (this.GridControl.pivotSchemaDesigner.pivotFilterLists.Bounds.Contains(this.GridControl.pivotSchemaDesigner.pivotFilterLists.Parent.PointToClient(Control.MousePosition)))
            {
                return "PivotFilters";
            }

            return "";
        }

        #endregion


        #region ContextMenu Navigation

        /// <summary>
        /// To navigate the item to up one position using context menu
        /// </summary>
        internal void NavigateUp(string tag, GridStyleInfo selectedItemStyle)
        {
            AssignItemLists();
            PivotItem rowItem,colItem;
            PivotComputationInfo compItem; int sourceIndex;
            switch (tag)
            {
                case "PivotRows":
                    if (GridRowList.TryGetValue(selectedItemStyle.Text, out rowItem))
                    {
                        sourceIndex = this.GridControl.PivotRows.IndexOf(rowItem);
                        if (sourceIndex > 0)
                        {
                            this.GridControl.PivotRows.Move(sourceIndex, sourceIndex - 1);
                            RefreshGridSchemaLayout();
                        }
                    }

                    break;
                case "PivotColumns":
                    if (GridColumnList.TryGetValue(selectedItemStyle.Text, out colItem))
                    {
                        sourceIndex = this.GridControl.PivotColumns.IndexOf(colItem);
                        if (sourceIndex > 0)
                        {
                            this.GridControl.PivotColumns.Move(sourceIndex, sourceIndex - 1);
                            RefreshGridSchemaLayout();
                        }
                    }

                    break;
                case "PivotCalculations":
                    if (GridCalcList.TryGetValue(selectedItemStyle.Text, out compItem))
                    {
                        sourceIndex = this.GridControl.PivotCalculations.IndexOf(compItem);
                        if (sourceIndex > 0)
                        {
                            this.GridControl.PivotCalculations.Move(sourceIndex, sourceIndex - 1);
                            RefreshGridSchemaLayout();
                        }
                    }
                    break;
            }
          
        }

        /// <summary>
        /// To navigate the item to down one position using context menu
        /// </summary>
        internal void NavigateDown(string tag, GridStyleInfo selectedItemStyle)
        {
            AssignItemLists();
            PivotItem rowItem, colItem;
            PivotComputationInfo compItem; int sourceIndex;
            switch (tag)
            {
                case "PivotRows":
                    if (GridRowList.TryGetValue(selectedItemStyle.Text, out rowItem))
                    {
                        sourceIndex = this.GridControl.PivotRows.IndexOf(rowItem);
                        if (sourceIndex < this.GridControl.PivotRows.Count - 1)
                        {
                            this.GridControl.PivotRows.Move(sourceIndex, sourceIndex + 1);
                            RefreshGridSchemaLayout();
                        }
                    }

                    break;
                case "PivotColumns":
                    if (GridColumnList.TryGetValue(selectedItemStyle.Text, out colItem))
                    {
                        sourceIndex = this.GridControl.PivotColumns.IndexOf(colItem);
                        if (sourceIndex < this.GridControl.PivotColumns.Count - 1)
                        {
                            this.GridControl.PivotColumns.Move(sourceIndex, sourceIndex + 1);
                            RefreshGridSchemaLayout();
                        }
                    }

                    break;
                case "PivotCalculations":
                    if (GridCalcList.TryGetValue(selectedItemStyle.Text, out compItem))
                    {
                        sourceIndex = this.GridControl.PivotCalculations.IndexOf(compItem);
                        if (sourceIndex < this.GridControl.PivotCalculations.Count - 1)
                        {
                            this.GridControl.PivotCalculations.Move(sourceIndex, sourceIndex + 1);
                            RefreshGridSchemaLayout();
                        }
                    }

                    break;
            }
           
        }

        /// <summary>
        /// To navigate the item to top position using context menu
        /// </summary>
        internal void NavigateBeginning(string tag, GridStyleInfo selectedItemStyle)
        {
            AssignItemLists();
            PivotItem rowItem, colItem;
            PivotComputationInfo compItem; int sourceIndex;
            switch (tag)
            {
                case "PivotRows":
                    if (GridRowList.TryGetValue(selectedItemStyle.Text, out rowItem))
                    {
                        sourceIndex = this.GridControl.PivotRows.IndexOf(rowItem);
                        if (sourceIndex > 0)
                        {
                            this.GridControl.PivotRows.Move(sourceIndex, 0);
                            RefreshGridSchemaLayout();
                        }
                    }

                    break;
                case "PivotColumns":
                    if (GridColumnList.TryGetValue(selectedItemStyle.Text, out colItem))
                    {
                        sourceIndex = this.GridControl.PivotColumns.IndexOf(colItem);
                        if (sourceIndex > 0)
                        {
                            this.GridControl.PivotColumns.Move(sourceIndex, 0);
                            RefreshGridSchemaLayout();
                        }
                    }

                    break;
                case "PivotCalculations":
                    if (GridCalcList.TryGetValue(selectedItemStyle.Text, out compItem))
                    {
                        sourceIndex = this.GridControl.PivotCalculations.IndexOf(compItem);
                        if (sourceIndex > 0)
                        {
                            this.GridControl.PivotCalculations.Move(sourceIndex, 0);
                            RefreshGridSchemaLayout();
                        }
                    }
                    break;
            }

        }

        /// <summary>
        /// To navigate the item to bottom position using context menu
        /// </summary>
        internal void NavigateEnd(string tag, GridStyleInfo selectedItemStyle)
        {
            AssignItemLists();
            PivotItem rowItem, colItem;
            PivotComputationInfo compItem; int sourceIndex;
            switch (tag)
            {
                case "PivotRows":
                    if (GridRowList.TryGetValue(selectedItemStyle.Text, out rowItem))
                    {
                        sourceIndex = this.GridControl.PivotRows.IndexOf(rowItem);
                        if (sourceIndex < this.GridControl.PivotRows.Count - 1)
                        {
                            this.GridControl.PivotRows.Move(sourceIndex, this.GridControl.PivotRows.Count - 1);
                            RefreshGridSchemaLayout();
                        }
                    }

                    break;
                case "PivotColumns":
                    if (GridColumnList.TryGetValue(selectedItemStyle.Text, out colItem))
                    {
                        sourceIndex = this.GridControl.PivotColumns.IndexOf(colItem);
                        if (sourceIndex < this.GridControl.PivotColumns.Count - 1)
                        {
                            this.GridControl.PivotColumns.Move(sourceIndex, this.GridControl.PivotColumns.Count - 1);
                            RefreshGridSchemaLayout();
                        }
                    }

                    break;
                case "PivotCalculations":
                    if (GridCalcList.TryGetValue(selectedItemStyle.Text, out compItem))
                    {
                        sourceIndex = this.GridControl.PivotCalculations.IndexOf(compItem);
                        if (sourceIndex < this.GridControl.PivotCalculations.Count - 1)
                        {
                            this.GridControl.PivotCalculations.Move(sourceIndex, this.GridControl.PivotCalculations.Count - 1);
                            RefreshGridSchemaLayout();
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// To remove the item from the collection
        /// </summary>
        internal void Remove(string tag, GridStyleInfo selectedItemStyle)
        {
            AssignItemLists();
            PivotItem rowItem, colItem;
            PivotComputationInfo compItem;
            FilterExpression exp;
            switch (tag)
            {
                case "PivotRows":
                    if (GridRowList.TryGetValue(selectedItemStyle.Text, out rowItem))
                    {
                        this.GridControl.PivotRows.Remove(rowItem);
                    }

                    break;
                case "PivotColumns":
                    if (GridColumnList.TryGetValue(selectedItemStyle.Text, out colItem))
                    {
                        this.GridControl.PivotColumns.Remove(colItem);
                    }

                    break;
                case "PivotCalculations":
                    if (GridCalcList.TryGetValue(selectedItemStyle.Text, out compItem))
                    {
                        this.GridControl.PivotCalculations.Remove(compItem);
                    }
                    break;
                case "PivotFilters":
                    if (GridFilterList.TryGetValue(selectedItemStyle.Text, out exp))
                    {
                        this.GridControl.Filters.Remove(exp);
                        if (FilterDropDown.FilterDimensions.Contains(exp.DimensionName))
                            FilterDropDown.FilterDimensions.Remove(exp.DimensionName);
                    }
                    break;
            }
           
            RefreshGridSchemaLayout();
            if (this.GridControl.pivotSchemaDesigner != null)
                this.GridControl.pivotSchemaDesigner.PopulateTableFieldList();
        }
        
        #endregion

        /// <summary>
        /// Refreshes PivotGridControl along with schema designer
        /// </summary>
        internal void RefreshGridSchemaLayout()
        {
            RefreshLayout();
            if (this.GridControl.pivotSchemaDesigner != null)
            {
                this.GridControl.pivotSchemaDesigner.ResetItemCollectionLists(false);
                this.GridControl.pivotSchemaDesigner.PopulatePivotItems();
            }
        }

        /// <summary>
        /// Refreshes PivotGridControl along with its sub controls except schema designer
        /// </summary>
        internal void RefreshLayout()
        {
            this.GridControl.TableControl.BeginUpdate();
            this.GridControl.TableControl.RemoveCollapsedNodes();
            this.GridControl.TableControl.ApplyRowCols();
            this.GridControl.TableControl.GroupBarSize();
            this.GridControl.TableControl.SubTotalsRendering();
            this.GridControl.TableControl.GrandTotalsRendering();
            this.GridControl.TableControl.GroupDropArea.BeginUpdate();
            this.GridControl.TableControl.GroupDropArea.ApplyItems();
            this.GridControl.TableControl.GroupDropArea.ApplySize();
            this.GridControl.TableControl.GroupDropArea.EndUpdate(true);
            this.GridControl.TableControl.GroupDropArea.Refresh();
            this.GridControl.TableControl.RowGroupDropArea.BeginUpdate();
            this.GridControl.TableControl.RowGroupDropArea.ApplyItems();
            this.GridControl.TableControl.RowGroupDropArea.ApplySize();
            this.GridControl.TableControl.RowGroupDropArea.EndUpdate(true);
            this.GridControl.TableControl.RowGroupDropArea.Refresh();
            this.GridControl.TableControl.FilterArea.BeginUpdate();
            this.GridControl.TableControl.FilterArea.ApplyItems();
            this.GridControl.TableControl.FilterArea.ApplySize();
            this.GridControl.TableControl.FilterArea.Refresh();
            this.GridControl.TableControl.FilterArea.EndUpdate(true);
            this.GridControl.TableControl.EndUpdate(true);
            this.GridControl.TableControl.Refresh();
        }
    }
}
