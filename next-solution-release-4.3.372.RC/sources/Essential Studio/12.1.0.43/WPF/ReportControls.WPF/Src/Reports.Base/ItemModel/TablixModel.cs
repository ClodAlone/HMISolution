//-------------------------------------------------------------------------------------------------
// <copyright file="PageModelTextbox.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.Layout;
using Syncfusion.RDL.DOM;

#if !SILVERLIGHT
using System.Data;
using System.Threading;
#endif

namespace Syncfusion.RDL.ItemModel
{
    /// <summary>
    /// A model for TextboxModel. Contains information about size, position, data and style for a text box.
    /// </summary>
    internal class TablixModel
        : ReportItemModeler
    {
        #region members

        GroupInfoCollection engineGroupCollection = new GroupInfoCollection();      
        List<CoveredCellRange> coveredRanges = null;
        TablixCellInfo[,] cornerCells = null;
        GroupInfo cornerCellsGroupInfo = null;
        Tablix tablixItem = null;
        KeysCalculationValues columnGroupKeyValue = null;
        KeysCalculationValues rowGroupKeyValue = null;

        int rowCountInfo = -1;
        int columnCountInfo = -1;
        int dataRowIndex = 0;
        int dataColumnIndex = 0;
        int columnHeaderCount = 0;
        int rowHeaderCount = 0;
        int headerCount = 0;
        int tablixMemberColumnIndex = 0;
        int columnHierachyColumnIndex = 0;
        int startColumnRowIndex = 0;
        int rowIndex = 0;
        int startRowIndex = 0;
        int mapRowIndex = -1;
        bool hasRepeatHeader = false;
        bool isGroupType = false;

        List<string> isHeader;

        #endregion

        #region  properties

        public Syncfusion.RDL.Internal.TablixRows Data
        {
            get;
            set;
        }

        public List<CoveredCellRange> CoveredRanges
        {
            get
            {
                if (coveredRanges == null)
                {
                    coveredRanges = new List<CoveredCellRange>();
                }

                return coveredRanges;
            }
        }

        public GroupInfoCollection RowGroupcollection
        {
            get;
            set;
        }

        public GroupInfoCollection ColumnGroupcollection
        {
            get;
            set;
        }

        internal int PrintPageColumnCount
        {
            get;
            set;
        }

        internal List<double> PageWidths
        {
            get;
            set;
        }

        internal Dictionary<int, TablixPageInfo> PageSizes
        {
            get;
            set;
        }

        internal List<ColumnInfo> PageColumnInfo
        {
            get;
            set;
        }

        internal Dictionary<int, TablixPageInfo> PrintPageSizes
        {
            get;
            set;
        }

        internal List<ColumnInfo> PrintPageColumnInfo
        {
            get;
            set;
        }

        internal List<double> PrintPageWidths
        {
            get;
            set;
        }

        internal Dictionary<int, TablixPageInfo> FlowPageSizes
        {
            get;
            set;
        }

        public TablixEngine Engine
        {
            get;
            set;
        }

        internal int RowCount
        {
            get;
            set;
        }

        internal int ColumnCount
        {
            get;
            set;
        }

        internal List<double> RowHeights
        {
            get;
            set;
        }

        internal List<double> ColumnWights
        {
            get;
            set;
        }

        internal Dictionary<int, bool> GroupBreakLocations
        {
            get;
            set;
        }

        internal Dictionary<CoveredGroupRange, string> GroupRanges
        {
            get;
            set;
        }

        internal Dictionary<int, RepeatHeaderInfo> RepeatHeaderIndexes
        {
            get;
            set;
        }

        internal List<int> GroupEndPositions
        {
            get;
            set;
        }

        internal List<string> GroupNameHeirachy
        {
            get;
            set;
        }

        internal double ParentPageLeft
        {
            get; 
            set;
        }

        internal double ParentPageTop
        {
            get;
            set;
        }

        internal Dictionary<int, List<int>> ItemPosition
        {
            get; 
            set;
        }

        internal string HiddenExpKey
        {
            get; 
            set;
        }

        internal string DocumentMap
        {
            get;
            set;
        }

        internal DrillDownModel DrillDownInfos
        {
            get;
            set;
        }

        internal List<List<ToggleGropInfo>> ToggleGrops
        {
            get;
            set;
        }

        internal List<CoveredCellRange> DrillSpanRange
        {
            get; 
            set;
        }

        public string NoRowsMessage
        {
            get;
            set;
        }

        public TablixCellInfo NoRowCellInfo
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        internal TablixModel()
        {
        }

        internal TablixModel(ReportItem reportItem, ReportModel pageModel, bool isTablixChild, string dataSetName)
        {
            this.Model = pageModel;
            this.ReportItem = reportItem;
            this.Name = this.ReportItem.Name;
            this.IsTablixChild = isTablixChild;

            if (this.ReportItem.Top != null)
            {
                this.Top = this.ReportItem.Top.PixelValue;
            }

            if (this.ReportItem.Left != null)
            {
                this.Left = this.ReportItem.Left.PixelValue;
            }

            if (this.ReportItem.Width != null)
            {
                this.Width = this.ReportItem.Width.PixelValue;
            }

            if (this.ReportItem.Height != null)
            {
                this.Height = this.ReportItem.Height.PixelValue;
            }

            Tablix tablix = (reportItem as Tablix);
            this.tablixItem = tablix;

            this.DataSetName = tablix.DataSetName;
            this.KeepTogether = tablix.KeepTogether;
            this.NoRowsMessage = tablix.NoRowsMessage;

            if (!String.IsNullOrEmpty(this.NoRowsMessage))
            {
                this.SetNoRowMessageInfo();
            }

            if (this.DataSetName == null)
            {
                this.DataSetName = dataSetName;
            }
            if (tablix.PageBreak != null)
            {
                this.PageBreak = tablix.PageBreak.BreakLocation;
            }

            var dataRegion = this.ReportItem as DataRegion;
            if (dataRegion != null)
            {
                this.ExpFilters = this.Model.ParseFilters(dataRegion.Filters, this.DataSetName, true);
            }
            if (this.ReportItem.Visibility != null)
            {
                this.HiddenExpKey = this.Model.ExpressionEngine.GetExpressionKey(this.ReportItem.Visibility.Hidden, this.DataSetName);
                this.ToggleItem = this.ReportItem.Visibility.ToggleItem;
            }
            if (!string.IsNullOrEmpty(this.ReportItem.DocumentMapLabel))
            {
                this.DocumentMap = this.Model.ExpressionEngine.GetExpressionKey(this.ReportItem.DocumentMapLabel, this.DataSetName);
                this.DocumentMapLable = this.DocumentMap;
            }   
            this.engineGroupCollection = new GroupInfoCollection();
            this.PopulateGroupInformation();
        }

        #endregion

        #region HelperMethods
        List<double> rowHeaderWidths = null;
        List<double> colHeaderHeights = null;
        List<List<double>> tempList = null;

        private void PopulateGroupInformation()
        {
            Tablix tb = this.ReportItem as Tablix;
            this.ModelType = ModelType.TablixModel;            
            this.RowGroupcollection = new GroupInfoCollection();
            this.ColumnGroupcollection = new GroupInfoCollection();

            int maxSize = 0;
            bool hasCorner = this.tablixItem.TablixCorner != null ? true : false;
            colHeaderHeights = new List<double>();
            rowHeaderWidths = new List<double>();
            tempList = new List<List<double>>();
            List<double> lst;

            foreach (var tablixMember in tb.TablixColumnHierarchy.TablixMembers)
            {
                List<double> temp = new List<double>();
                this.GetCellsSize(temp, tablixMember);
                tempList.Add(temp);
            }

            maxSize = hasCorner ? this.tablixItem.TablixCorner.TablixCornerRows.Count : rowHeaderCount;
            rowHeaderCount = 0;
            lst = tempList.FirstOrDefault(list => list.Count() == maxSize && !list.Contains(0));

            if (lst != null && lst.Count > 0)
            {
                colHeaderHeights = lst;
            }
            tempList.Clear();
            
            foreach (var tablixMember in tb.TablixRowHierarchy.TablixMembers)
            {
                List<double> temp = new List<double>();
                this.GetCellsSize(temp, tablixMember);
                tempList.Add(temp);
            }
            
            maxSize = hasCorner ? this.tablixItem.TablixCorner.TablixCornerRows[0].TablixCornerCells.Count : rowHeaderCount;
            rowHeaderCount = 0;
            lst = tempList.FirstOrDefault(list => list.Count() == maxSize && !list.Contains(0));
            this.GroupNameHeirachy = new List<string>();

            if (lst != null && lst.Count > 0)
            {
                rowHeaderWidths = lst;
            }

            columnCountInfo = -1;

            foreach (var tablixMember in tb.TablixColumnHierarchy.TablixMembers)
            {
                headerCount = 0;
                this.ColumnGroupcollection.Add(this.GetGroupInfo(tablixMember, null, false));
            }

            rowCountInfo = -1;

            foreach (var tablixMember in tb.TablixRowHierarchy.TablixMembers)
            {
                headerCount = 0;
                this.RowGroupcollection.Add(this.GetGroupInfo(tablixMember, null, true));
            }

            this.PopulateCornerCells();

            tempList.Clear();
            tempList = null;
            colHeaderHeights.Clear();
            rowHeaderWidths.Clear();
            colHeaderHeights = null;
            rowHeaderWidths = null;
        }

        private void GetCellsSize(List<double> HeaderSizes, TablixMember tablixMember)
        {
            try
            {
                if (tablixMember.TablixHeader != null)
                {
                    headerCount++;
                    HeaderSizes.Add(0);
                    HeaderSizes[headerCount - 1] = HeaderSizes[headerCount - 1] != 0 && tablixMember.TablixHeader.Size.PixelValue > HeaderSizes[headerCount - 1] ?
                                                     HeaderSizes[headerCount - 1] : tablixMember.TablixHeader.Size.PixelValue;

                    if (HeaderSizes.Last() == 0)
                    {
                        HeaderSizes.RemoveAt(HeaderSizes.Count - 1);
                    }
                }
                if (tablixMember.TablixMembers != null)
                {
                    foreach (var member in tablixMember.TablixMembers)
                    {
                        GetCellsSize(HeaderSizes, member);
                    }
                }
                if (tablixMember.TablixHeader != null)
                {
                    rowHeaderCount = headerCount > rowHeaderCount ? headerCount : rowHeaderCount;
                    headerCount--;
                }
            }
            catch { }
        }

        private void PopulateCornerCells()
        {
            if (this.tablixItem.TablixCorner != null)
            {
                GroupInfo groupInfo = new GroupInfo();
                groupInfo.Name = this.ReportItem.Name;
                groupInfo.ParentGroup = null;

                cornerCells = new TablixCellInfo[this.tablixItem.TablixCorner.TablixCornerRows.Count, this.tablixItem.TablixCorner.TablixCornerRows[0].TablixCornerCells.Count];

                for (int i = 0; i < this.tablixItem.TablixCorner.TablixCornerRows.Count; i++)
                {
                    TablixCornerCells cells = this.tablixItem.TablixCorner.TablixCornerRows[i].TablixCornerCells;

                    for (int j = 0; j < cells.Count; j++)
                    {
                        TablixCornerCell cornerCell = cells[j];

                        if (cornerCell.CellContents != null)
                        {
                            ReportItem reportItem = cornerCell.CellContents.ReportItem;
                            try
                            {
                                reportItem.Height = new RDL.DOM.Size(colHeaderHeights[i]);
                                reportItem.Width = new RDL.DOM.Size(rowHeaderWidths[j]);
                            }
                            catch { }
                            TablixCellInfo cellInfo = GetCellValue(reportItem, groupInfo, null);
                            this.cornerCells[i, j] = cellInfo;
                            cellInfo.ColumnSpan = 1;
                            cellInfo.RowSpan = 1;

                            if (cornerCell.CellContents.ColSpan > 1)
                            {
                                cellInfo.ColumnSpan = cornerCell.CellContents.ColSpan;
                            }

                            if (cornerCell.CellContents.RowSpan > 0)
                            {
                                cellInfo.RowSpan = cornerCell.CellContents.RowSpan;
                            }
                        }
                    }
                }

                this.cornerCellsGroupInfo = groupInfo;
                this.engineGroupCollection.Add(groupInfo);
            }
        }

        private List<DataField> GetDataFields(string key)
        {
            if (!string.IsNullOrEmpty(key) && this.Model.ExpressionEngine.FieldInformations.ContainsKey(key))
            {
                return this.Model.ExpressionEngine.FieldInformations[key];                
            }

            return new List<DataField>();
        }

        private string GetExpressionKey(string value)
        {
            string key = this.Model.ExpressionEngine.GetExpressionKey(value, this.DataSetName,true);
            return key;
        }

        private GroupInfo GetGroupInfo(TablixMember tablixMember, GroupInfo parentGroup, bool row)
        {
            GroupInfo groupInfo = new GroupInfo();
            groupInfo.IsRow = row;
            groupInfo.KeepTogether = tablixMember.KeepTogether;
            groupInfo.ParentGroup = parentGroup;
            groupInfo.Name = (tablixMember.Group != null) ? tablixMember.Group.Name : (parentGroup == null) ? this.ReportItem.Name : parentGroup.Name;
            this.engineGroupCollection.Add(groupInfo);

            if (row && !this.isGroupType)
            {
                groupInfo.Columcount = columnCountInfo + 1;
            }
            if (row && tablixMember.Group != null && tablixMember.Group.PageBreak != null)
            {
                groupInfo.PageBreak = tablixMember.Group.PageBreak.BreakLocation;
            }
            if (row)
            {
                if (!this.GroupNameHeirachy.Contains(groupInfo.Name))
                {
                    this.GroupNameHeirachy.Add(groupInfo.Name);
                }
                if (tablixMember.Group != null && tablixMember.KeepWithGroup != KeepWithGroup.None &&
                    tablixMember.RepeatOnNewPage == true)
                {
                    //throw new Exception("The grouping" + groupInfo.Name + "has an invalid TablixMember." + "A TablixMember that is dynamic has group specified or " +
                    //    "has a dynamic decendents must have the KeepWithGroup property set to None.");
                }
                else
                {
                    groupInfo.KeepWithGroup = tablixMember.KeepWithGroup;
                    groupInfo.RepeatOnNewPage = tablixMember.RepeatOnNewPage;
                }
                if (tablixMember.RepeatOnNewPage && tablixMember.KeepWithGroup != KeepWithGroup.None)
                {
                    this.hasRepeatHeader = true;
                }
            }
            else
            {
                string msg = "The tablix" + this.tablixItem.Name + "has an invalid TablixMember";

                if (tablixMember.KeepWithGroup != KeepWithGroup.None)
                {
                    msg += "All TablixMember elements in a TablixColumnHierarchy must have the KeepWithGroup property set to None.";
                    //throw new Exception(msg);
                }
                else if (tablixMember.RepeatOnNewPage != false)
                {
                    msg += "All TablixMember elements in a TablixColumnHierarchy must have the RepeatOnNewPage property set to false.";
                    //throw new Exception(msg);
                }
            }
            if (tablixMember.Group != null && !this.isGroupType)
            {
                if (tablixMember.Group.Parent != null)
                {
                    groupInfo.RecursiveParent = this.GetExpressionKey(tablixMember.Group.Parent);
                    groupInfo.RecursiveParentFields = new List<DataField>();

                    List<DataField> sumFields = this.GetDataFields(groupInfo.RecursiveParent);

                    foreach (var field in sumFields)
                    {
                        if (string.IsNullOrEmpty(field.Expression) && string.IsNullOrEmpty(field.DataSetName))
                        {
                            field.DataSetName = this.DataSetName;
                            field.DataTypeName = this.Model.ExpressionEngine.GetTypeName(field.FieldName, field.DataSetName);
                        }

                        if (field.Expression != null || groupInfo.RecursiveParentFields.Where(dataFied => dataFied.FieldName.Equals(field.FieldName)).Count() == 0)
                        {
                            groupInfo.RecursiveParentFields.Add(field);
                        }
                    }
                }

                if (tablixMember.Group.DocumentMapLabel != null)
                {
                    string key = this.GetExpressionKey(tablixMember.Group.DocumentMapLabel);
                    groupInfo.DocumentMapFields = new List<DataField>();
                    groupInfo.DocumentKey = key;
                    List<DataField> sumFields = this.GetDataFields(key);

                    foreach (var field in sumFields)
                    {
                        if (string.IsNullOrEmpty(field.Expression) && string.IsNullOrEmpty(field.DataSetName))
                        {
                            field.DataSetName = this.DataSetName;
                            field.DataTypeName = this.Model.ExpressionEngine.GetTypeName(field.FieldName, field.DataSetName);
                        }
                        else
                        {
                            groupInfo.DocumentMapFields.Add(field);
                        }
                    }
                }

                if (tablixMember.Group.GroupExpressions != null && tablixMember.Group.GroupExpressions.Count > 0)
                {
                    groupInfo.GroupExpressions = new List<string>();
                    groupInfo.GroupFields = new List<DataField>();
                    foreach (var groupExpression in tablixMember.Group.GroupExpressions)
                    {
                        string key = this.GetExpressionKey(groupExpression.Value);
                        List<DataField> sumFields = this.GetDataFields(key);
                        groupInfo.GroupExpressions.Add(key);

                        foreach (var field in sumFields)
                        {
                            if (string.IsNullOrEmpty(field.Expression) && string.IsNullOrEmpty(field.DataSetName))
                            {
                                field.DataSetName = this.DataSetName;
                                field.DataTypeName = this.Model.ExpressionEngine.GetTypeName(field.FieldName, field.DataSetName);
                            }

                            if (field.Expression != null || groupInfo.GroupFields.Where(dataFied => dataFied.FieldName.Equals(field.FieldName)).Count() == 0)
                            {
                                groupInfo.GroupFields.Add(field);
                            }
                        }
                    }

                    groupInfo.Type = TablixGroupType.Group;
                }
                else
                {
                    groupInfo.Type = TablixGroupType.Detail;
                }
            }
            else
            {
                groupInfo.Type = TablixGroupType.None;
            }

            if (tablixMember.SortExpressions != null && !this.isGroupType)
            {
                foreach (var sortExpression in tablixMember.SortExpressions)
                {
                    string key = this.GetExpressionKey(sortExpression.Value);
                    List<DataField> sumFields = this.GetDataFields(key);
                    groupInfo.SortExpressions = new List<RDL.Internal.SortExpression>();
                    groupInfo.SortFields = new List<DataField>();
                    groupInfo.SortExpressions.Add(new Syncfusion.RDL.Internal.SortExpression() { Expression = key, SortOrder = sortExpression.Direction });

                    foreach (var field in sumFields)
                    {
                        if (string.IsNullOrEmpty(field.Expression) && string.IsNullOrEmpty(field.DataSetName))
                        {
                            field.DataSetName = this.DataSetName;
                            field.DataTypeName = this.Model.ExpressionEngine.GetTypeName(field.FieldName, field.DataSetName);
                        }

                        if (field.Expression != null || groupInfo.SortFields.Where(dataFied => dataFied.FieldName.Equals(field.FieldName)).Count() == 0)
                        {
                            groupInfo.SortFields.Add(field);
                        }
                    }
                }
            }

            if (tablixMember.Visibility != null)
            {
                if (!string.IsNullOrEmpty(tablixMember.Visibility.Hidden))
                {
                    string key = this.GetExpressionKey(tablixMember.Visibility.Hidden);
                    List<DataField> sumFields = this.GetDataFields(key);

                    foreach (var field in sumFields)
                    {
                        if (string.IsNullOrEmpty(field.Expression) && string.IsNullOrEmpty(field.DataSetName))
                        {
                            field.DataSetName = this.DataSetName;
                            field.DataTypeName = this.Model.ExpressionEngine.GetTypeName(field.FieldName,
                                                                                         field.DataSetName);
                        }
                        else
                        {
                            if (groupInfo.HiddenFields == null)
                            {
                                groupInfo.HiddenFields = new List<DataField>();
                            }
                            groupInfo.HiddenFields.Add(field);
                        }
                    }
                    groupInfo.IsHidden = key;
                }
                groupInfo.ToggleItem = tablixMember.Visibility.ToggleItem;
                if (!string.IsNullOrEmpty(groupInfo.ToggleItem))
                {
                    if (ToggleGrops == null)
                    {
                        ToggleGrops = new List<List<ToggleGropInfo>>();
                    }
                    List<ToggleGropInfo> toggleInfos = this.UpdateToogleItems(this.engineGroupCollection, groupInfo.ToggleItem);
                    groupInfo.ToggleGroups = toggleInfos;
                    ToggleGrops.Add(toggleInfos);
                }
            }

            if (tablixMember.TablixHeader != null && !this.isGroupType)
            {
                headerCount++;

                double size = tablixMember.TablixHeader.Size.PixelValue;
                var reportItem = tablixMember.TablixHeader.CellContents.ReportItem;
                double height = 0;
                double width = 0;
                double temp = 0;
                bool valid = false;
                int spanVal = 0;

                if (row)
                {
                    width = size;
                    height = this.tablixItem.TablixBody.TablixRows[rowCountInfo + 1].Height.PixelValue;

                    for (int i = 0; i < rowHeaderWidths.Count; i++)
                    {
                        for (int j = i; j < rowHeaderWidths.Count; j++)
                        {
                            temp += rowHeaderWidths[j];
                            valid = Math.Floor(size) == Math.Floor(temp);
                            spanVal++;

                            if (valid)
                            {

                                width = spanVal > 1 ? rowHeaderWidths[i] : size;
                                break;
                            }
                        }
                        if (valid)
                        {
                            break;
                        }
                        temp = 0;
                        spanVal = 0;
                    }
                }
                else
                {
                    height = size;
                    width = this.tablixItem.TablixBody.TablixColumns[columnCountInfo + 1].Width.PixelValue;

                    for (int i = 0; i < colHeaderHeights.Count; i++)
                    {
                        for (int j = i; j < colHeaderHeights.Count; j++)
                        {
                            temp += colHeaderHeights[j];
                            valid = Math.Floor(size) == Math.Floor(temp);
                            spanVal++;

                            if (valid)
                            {
                                height = spanVal > 1 ? colHeaderHeights[i] : size;
                                break;
                            }
                        }
                        if (valid)
                        {
                            break;
                        }
                        temp = 0;
                        spanVal = 0;
                    }
                }

                reportItem.Height = new Size((height / 96).ToString(CultureInfo.InvariantCulture) + "in");
                reportItem.Width = new Size((width / 96).ToString(CultureInfo.InvariantCulture) + "in");

                TablixCellInfo cellInfo = null;

                if (row)
                {
                    cellInfo = this.GetCellValue(reportItem, groupInfo, null);
                    cellInfo.ColumnSpan = spanVal > 1 ? spanVal : 0;
                }
                else
                {
                    cellInfo = this.GetCellValue(reportItem, null, groupInfo);
                    cellInfo.RowSpan = spanVal > 1 ? spanVal : 0;
                }

                cellInfo.Height = height;
                cellInfo.Width = width;

                groupInfo.HeaderInfo = cellInfo;
                groupInfo.HeaderLevel = headerCount;
                headerCount = spanVal > 1 && valid ? (headerCount + spanVal - 1) : headerCount;
            }

            if (groupInfo.Type == TablixGroupType.None)
            {
                PopulateTablixMember(tablixMember, groupInfo, row);
            }
            else if (groupInfo.Type == TablixGroupType.Detail)
            {
                PopulateTablixMember(tablixMember, groupInfo, row);
            }
            else
            {
                if (tablixMember.TablixMembers.Count == 0)
                {
                    isGroupType = true;
                    groupInfo.Groups.Add(this.GetGroupInfo(tablixMember, groupInfo, row));
                    isGroupType = false;
                }
                else
                {
                    foreach (var tm in tablixMember.TablixMembers)
                    {
                        groupInfo.Groups.Add(this.GetGroupInfo(tm, groupInfo, row));
                    }
                }
            }
            if (groupInfo.HeaderInfo != null)
            {
                headerCount = groupInfo.HeaderInfo.ColumnSpan > 1 ? (headerCount - (groupInfo.HeaderInfo.ColumnSpan - 1)) :
                    groupInfo.HeaderInfo.RowSpan > 1 ? (headerCount - (groupInfo.HeaderInfo.RowSpan - 1)) : headerCount;
            }
            if (tablixMember.TablixHeader != null && !this.isGroupType)
            {
                if (row)
                {
                    rowHeaderCount = headerCount > rowHeaderCount ? headerCount : rowHeaderCount;
                }
                else
                {
                    columnHeaderCount = headerCount > columnHeaderCount ? headerCount : columnHeaderCount;
                }

                headerCount--;
            }

            return groupInfo;
        }

        List<ToggleGropInfo> UpdateToogleItems(List<GroupInfo> ginfos, string itemName)
        {
            foreach (var group in ginfos)
            {
                if (group.HeaderInfo != null)
                {
                    if (group.HeaderInfo.ItemModel.Name == itemName)
                    {
                        if ((group.HeaderInfo.ItemModel as TextboxModel).ToggleGroups == null)
                        {
                            (group.HeaderInfo.ItemModel as TextboxModel).ToggleGroups = new List<ToggleGropInfo>();
                        }
                        return (group.HeaderInfo.ItemModel as TextboxModel).ToggleGroups;
                    }
                }
                foreach (var cell in from cells in @group.RowValues from cell in cells where cell != null && cell.ItemModel.Name == itemName select cell)
                {
                    if ((cell.ItemModel as TextboxModel).ToggleGroups == null)
                    {
                        return (cell.ItemModel as TextboxModel).ToggleGroups = new List<ToggleGropInfo>();
                    }
                    return (cell.ItemModel as TextboxModel).ToggleGroups;
                }
            }
            return null;
        }

        private void PopulateTablixMember(TablixMember tbMember, GroupInfo groupInfo, bool row)
        {
            if (tbMember.TablixMembers != null && tbMember.TablixMembers.Count > 0)
            {
                foreach (var tm in tbMember.TablixMembers)
                {
                    if (groupInfo.Type == TablixGroupType.None)
                    {
                        groupInfo.Groups.Add(this.GetGroupInfo(tm, groupInfo.ParentGroup, row));
                    }
                    else
                    {
                        PopulateTablixMember(tm, groupInfo, row);
                    }
                }

                return;
            }

            if (row)
            {
                groupInfo.Rowcount++;
                rowCountInfo++;
                GroupRow groupRow = new GroupRow();
                tablixMemberColumnIndex = -1;
                this.UpdateTablixMemberColumn(groupInfo, this.ColumnGroupcollection, groupRow);
                groupInfo.RowValues.Add(groupRow);
            }
            else
            {
                groupInfo.Columcount++;
                columnCountInfo++;
            }
        }

        private void UpdateTablixMemberColumn(GroupInfo rowGroupInfo, GroupInfoCollection groupCollection, GroupRow groupRow)
        {
            foreach (var group in groupCollection)
            {
                if (group.Groups.Count > 0)
                {
                    this.UpdateTablixMemberColumn(rowGroupInfo, group.Groups, groupRow);
                }
                else
                {
                    for (int i = 0; i < group.Columcount; i++)
                    {
                        tablixMemberColumnIndex++;

                        if (groupRow.Count <= tablixMemberColumnIndex)
                        {
                            var cellcontent = this.tablixItem.TablixBody.TablixRows[rowCountInfo].TablixCells[tablixMemberColumnIndex].CellContents;
                            var reportItem = cellcontent.ReportItem;

                            Size itemHeight = this.tablixItem.TablixBody.TablixRows[rowCountInfo].Height;
                            Size itemWidth = this.tablixItem.TablixBody.TablixColumns[tablixMemberColumnIndex].Width;
                            double height = itemHeight.PixelValue;
                            double width = itemWidth.PixelValue;
                            reportItem.Height = itemHeight;
                            reportItem.Width = itemWidth;
                            TablixCellInfo cellInfo = GetCellValue(reportItem, rowGroupInfo, group);
                            cellInfo.Height = height;
                            cellInfo.Width = width;

                            groupRow.Add(cellInfo);

                            if (cellcontent.ColSpan > 0)
                            {
                                cellInfo.ColumnSpan = cellcontent.ColSpan;
                                cellInfo.Widths = new List<double>();
                                cellInfo.Widths.Add(cellInfo.Width);

                                for (int cell = 1; cell < cellInfo.ColumnSpan; cell++)
                                {
                                    Size itemSpanWidth = this.tablixItem.TablixBody.TablixColumns[tablixMemberColumnIndex + cell].Width;
                                    cellInfo.Width += itemSpanWidth.PixelValue;
                                    cellInfo.Widths.Add(itemSpanWidth.PixelValue);
                                    groupRow.Add(null);
                                }

                                cellInfo.ItemModel.Width = cellInfo.Width;
                            }
                            else
                            {
                                cellInfo.ColumnSpan = 1;
                            }
                        }
                    }
                }
            }
        }


        private TablixCellInfo GetCellValue(ReportItem cellcontent, GroupInfo rowGroupInfo, GroupInfo columnGroupInfo)
        {
            IReportItemModeler model = this.Model.GetModel(cellcontent, true,this.DataSetName);

            TablixCellInfo info = new TablixCellInfo();

            if (rowGroupInfo != null && columnGroupInfo != null)
            {
                info.IsDetailCell = rowGroupInfo.Type == TablixGroupType.Detail;
            }
            if (model.HasRowNumber)
            {
                this.HasRowNumber = true;
                info.HasRowNumber = true;

                if (columnGroupInfo != null)
                    columnGroupInfo.HasRowNumber = true;
            }

            info.ItemModel = model;
            string rowGroupName = (rowGroupInfo != null) ? rowGroupInfo.Name : this.ReportItem.Name;
            string columnGroupName = (columnGroupInfo != null) ? columnGroupInfo.Name : this.ReportItem.Name;

            var groupInfo = rowGroupInfo == null ? columnGroupInfo : rowGroupInfo;

            if (model.DataSetFields != null && model.ModelType != Internal.ModelType.ChartModel
                && model.ModelType != Internal.ModelType.GaugeModel && model.ModelType != Internal.ModelType.TablixModel)
            {
                foreach (var sumField in model.DataSetFields)
                {
                    if (string.IsNullOrEmpty(sumField.Expression) && string.IsNullOrEmpty(sumField.DataSetName))
                    {
                        sumField.DataSetName = this.DataSetName;
                        sumField.DataTypeName = this.Model.ExpressionEngine.GetTypeName(sumField.FieldName, sumField.DataSetName);
                    }

                    var fieldsCount = (from expField in groupInfo.Fields
                                       where (sumField.FunctionName.Equals(expField.FunctionName) && sumField.Name.Equals(expField.Name) && expField.RowGroupName.Equals(rowGroupName) && expField.ColumnGroupName.Equals(columnGroupName))
                                       select expField).Count();

                    if (fieldsCount == 0)
                    {
                        TablixEngineField engineField = new TablixEngineField();
                        engineField.Name = sumField.Name;
                        engineField.DataSetName = sumField.DataSetName;
                        engineField.FunctionName = sumField.FunctionName;
                        engineField.FieldName = sumField.FieldName;
                        engineField.DataTypeName = sumField.DataTypeName;
                        engineField.Expression = sumField.Expression;
                        engineField.RowGroupName = rowGroupName;
                        engineField.ColumnGroupName = columnGroupName;
                        engineField.IsDataSetField = sumField.IsDataSetField;
                        groupInfo.Fields.Add(engineField);
                    }
                }
            }
            return info;
        }

        private void PopulateTable(bool isMapHeader)
        {
            try
            {
                RowCount = this.columnHeaderCount;
                ColumnCount = this.rowHeaderCount;

                var columnCollection = from groupInfos in this.Engine.GroupCollection
                                       where !groupInfos.IsRow
                                       select groupInfos;
                this.RepeatHeaderIndexes = new Dictionary<int, RepeatHeaderInfo>();
                this.GroupEndPositions = new List<int>();

                if (this.columnHeaderCount > 0 && this.tablixItem.RepeatColumnHeaders == true)
                {
                    for (int i = 0; i < this.columnHeaderCount; i++)
                    {
                        this.RepeatHeaderIndexes.Add(i, new RepeatHeaderInfo() { GroupName = this.tablixItem.Name, KeepWithGroup = KeepWithGroup.After });
                    }
                }

                foreach (var info in columnCollection)
                {
                    if (info.Type == TablixGroupType.Detail && !this.Engine.IsEmptyDataSource)
                    {
                        int colCount = 0;

                        foreach (var group in info.GroupValues)
                        {
                            colCount += group.DataSources.Count;
                        }

                        ColumnCount += info.Columcount * colCount;
                    }
                    else
                    {
                        if (info.ParentGroup != null)
                        {
                            ColumnCount += info.ParentGroup.GroupValues.Count * info.Columcount;
                        }
                        else
                        {
                            ColumnCount += info.Columcount;
                        }
                    }
                }

                var rowCollection = from groupInfos in this.Engine.GroupCollection
                                    where groupInfos.IsRow
                                    select groupInfos;

                foreach (var info in rowCollection)
                {
                    if (info.Type == TablixGroupType.Detail && !this.Engine.IsEmptyDataSource)
                    {
                        int rowCount = 0;

                        foreach (var group in info.GroupValues)
                        {
                            rowCount += group.DataSources.Count;
                        }

                        RowCount += info.Rowcount * rowCount;
                    }
                    else
                    {
                        if (info.ParentGroup != null)
                        {
                            RowCount += info.ParentGroup.GroupValues.Count * info.Rowcount;
                        }
                        else
                        {
                            RowCount += info.Rowcount;
                        }
                    }
                }

                this.Data = new Syncfusion.RDL.Internal.TablixRows();

                for (int i = 0; i < RowCount; i++)
                {
                    this.Data.Add(new Syncfusion.RDL.Internal.TablixRow());

                    for (int j = 0; j < ColumnCount; j++)
                    {
                        this.Data[i].Add(null);
                    }
                }

                this.RowHeights = new List<double>();
                this.ColumnWights = new List<double>();

                for (int i = 0; i < this.RowCount; i++)
                {
                    this.RowHeights.Add(0);
                }

                for (int j = 0; j < this.ColumnCount; j++)
                {
                    this.ColumnWights.Add(0);
                }

                headerCount = 0;
                rowIndex = 0;
                this.dataRowIndex = this.columnHeaderCount;
                this.GroupBreakLocations = new Dictionary<int, bool>();
                //var groupLevel = this.Model.ExpressionEngine.GroupLevels;
                //this.Model.ExpressionEngine.GroupLevels = new Dictionary<string, int>();
                //this.Model.ExpressionEngine.GroupLevels.Add("Level", 0);
                if (this.HasRowNumber)
                {
                    this.Model.ExpressionEngine.RowNumbers = new Dictionary<string, object>();
                    this.Model.ExpressionEngine.RowNumbers.Add(this.tablixItem.Name, 0);
                    this.Model.ExpressionEngine.RowNumbers.Add(this.DataSetName, 0);
                }

                columnGroupKeyValue = new KeysCalculationValues();
                rowGroupKeyValue = new KeysCalculationValues();
                bool hasMoreGroup = false;

                if (this.DrillDownInfos == null)
                {
                    this.DrillDownInfos = new DrillDownModel();
                }
                if (this.Model.MapModel == null)
                {
                    this.Model.MapModel = new DocumentMapModel();
                }
                List<DocumentData> documentDatas = this.Model.MapModel.NodeData;
                if (isMapHeader)
                {
                    if (this.Model.MapModel.NodeData.Last().Node==null)
                    {
                        this.Model.MapModel.NodeData.Last().Node=new DocumentMapModel();
                    }
                    documentDatas = this.Model.MapModel.NodeData.Last().Node.NodeData;
                }

                foreach (var info in this.RowGroupcollection)
                {
                    if (info.Type == TablixGroupType.Group)
                    {
                        if (hasMoreGroup)
                        {
                            rowIndex = 0;
                            this.RepeatHeaderIndexes.Add(dataRowIndex, new RepeatHeaderInfo() { GroupName = info.Name, KeepWithGroup = info.KeepWithGroup, IsNextGroup = true });

                            if(this.HasRowNumber)
                                this.Model.ExpressionEngine.RowNumbers[this.DataSetName] = 0;
                        }
                        else
                        {
                            hasMoreGroup = true;
                        }
                    }

                    List<ToggleInfo> drillDown = this.DrillDownInfos.InnerDrillInfo;
                    List<DocumentData> documentData = documentDatas;
                    UpdateGroup(info, null, new KeysCalculationValues(), new KeysCalculationValues(), documentDatas,null,drillDown);
                    documentDatas = documentData;
                    this.DrillDownInfos.InnerDrillInfo = drillDown;
                }

                this.UpdateCornerCellValues();

                columnGroupKeyValue = rowGroupKeyValue = null;

                if (this.Model.ExpressionEngine.RowNumbers != null)
                {
                    this.Model.ExpressionEngine.RowNumbers.Clear();
                    this.Model.ExpressionEngine.RowNumbers = null;
                }
                //this.Model.ExpressionEngine.GroupLevels.Clear();
            }
            catch (Exception)
            {
               // this.ExceptionDetails.Add(ex.Message);
            }
        }

        void UpdateCornerCellValues()
        {
            GroupInfo groupInfo = this.Engine.GroupCollection.Last();
            if (this.cornerCells != null)
            {
                for (int i = 0; i < this.columnHeaderCount; i++)
                {
                    for (int j = 0; j < this.rowHeaderCount; j++)
                    {
                        if (this.cornerCells[i, j] != null)
                        {
                            GroupValueInfo groupValueInfo = null;

                            TablixCellInfo cell = this.cornerCells[i, j].Clone() as TablixCellInfo;
                            var row = this.Data[i];

                            if (this.cornerCellsGroupInfo.GroupValues.Count > 0)
                            {
                                groupValueInfo = this.cornerCellsGroupInfo.GroupValues.First();
                                this.IntilizeExpressionEngineValue(cell,row, i, j , groupValueInfo, null, this.cornerCellsGroupInfo, null,null,null);
                            }                            

                            if (cell.RowSpan > 1 || cell.ColumnSpan > 1)
                            {
                                CoveredCellRange range = new CoveredCellRange(i, j, i + cell.RowSpan - 1, j + cell.ColumnSpan - 1);
                                cell.CellRange = range;
                                this.CoveredRanges.Add(range);
                            }

                            this.Data[i][j] = cell;
                        }
                    }
                }
            }
        }

        private void GetHierachy(BinaryList recursionKeyList,KeysCalculationValues paraentKey,IEnumerable<GroupValueInfo> groupValueIinfos,int level,GroupValueInfo parentGroup)
        {
            IEnumerable<GroupValueInfo> childItems = groupValueIinfos.Where(gr => gr.RecursiveParentGroupKey.Equals(paraentKey));

            foreach (GroupValueInfo info in childItems)
            {
                recursionKeyList.Add(info.GroupKey);
                info.Level = level;
                this.GetHierachy(recursionKeyList, info.GroupKey, groupValueIinfos, level + 1, info);

                if (parentGroup != null)
                {
                    foreach (var data in info.DataSources)
                    {
                        parentGroup.DataSources.Add(data);
                    }
                }

                foreach (var data in info.DataSources)
                {
                    info.RecursiveDataSources.Add(data);
                }
            }
        }

        private void UpdateGroup(GroupInfo info, GroupInfo rowInfo, KeysCalculationValues groupKey, KeysCalculationValues rowGroupKey,List<DocumentData> documentDatas, List<DocumentData> parentdocumentDatas,List<ToggleInfo> drillDownInfos)
        {
            if (info.IsRow)
            {
                if (info.Groups.Count == 0)
                {
                    this.dataColumnIndex = this.rowHeaderCount;
                    this.columnHierachyColumnIndex = 0;

                    this.startColumnRowIndex = this.dataRowIndex;

                    int startRowIndex = dataRowIndex;
                    Dictionary<string, object> rowNumbers = null;

                    if (this.HasRowNumber)
                    {
                        rowNumbers = new Dictionary<string, object>(this.Model.ExpressionEngine.RowNumbers);
                    }
                    if ((info.KeepTogether || hasRepeatHeader) && info.Type == TablixGroupType.Group)
                    {
                        this.GroupRanges.Add(new CoveredGroupRange() { StartIndex = startRowIndex, KeepTogether = info.KeepTogether }, info.Name);
                    }
                    this.startRowIndex = this.rowIndex;
                    foreach (var group in this.ColumnGroupcollection)
                    {
                        this.rowIndex = this.startRowIndex;
                        if (this.HasRowNumber)
                            this.Model.ExpressionEngine.RowNumbers = new Dictionary<string, object>(rowNumbers);
                        List<DocumentData> documentData = documentDatas;
                        UpdateGroup(group, info, new KeysCalculationValues(), rowGroupKey, documentDatas, parentdocumentDatas,drillDownInfos);
                        documentDatas = documentData;
                    }

                    int endRowIndex = dataRowIndex - 1;

                    if (info.Type == TablixGroupType.None && info.KeepWithGroup != KeepWithGroup.None
                        && info.RepeatOnNewPage == true && !RepeatHeaderIndexes.ContainsKey(endRowIndex))
                    {
                        RepeatHeaderIndexes.Add(endRowIndex, new RepeatHeaderInfo() { GroupName = info.Name, KeepWithGroup = info.KeepWithGroup });
                    }
                    if ((info.KeepTogether || hasRepeatHeader) && info.Type == TablixGroupType.Group)
                    {
                        var coverGroup = this.GroupRanges.Last();
                        coverGroup.Key.EndIndex = endRowIndex;
                    }
                    if (info.HeaderInfo != null)
                    {
                        var groupValues = from groupValue in info.GroupValues 
                                          where groupValue.ParentGroupKey.Equals(groupKey) 
                                          select groupValue;

                        GroupValueInfo groupinfo = groupValues.FirstOrDefault();
                        TablixCellInfo cell = info.HeaderInfo.Clone() as TablixCellInfo;

                        if (cell.HasRowNumber)
                            UpdateRowCount(cell, info, groupinfo);

                        var row = this.Data[startRowIndex];

                        if (groupinfo != null)
                        {
                            this.IntilizeExpressionEngineValue(cell, row, startRowIndex, info.HeaderLevel - 1, groupinfo, null, info, null, documentDatas,parentdocumentDatas);
                        }

                        if (endRowIndex > startRowIndex)
                        {
                            CoveredCellRange range = new CoveredCellRange(startRowIndex, info.HeaderLevel - 1, endRowIndex, info.HeaderLevel - 1);
                            cell.CellRange = range;
                            this.CoveredRanges.Add(range);
                        }
                        else if (info.HeaderInfo.ColumnSpan > 1)
                        {
                            CoveredCellRange range = new CoveredCellRange(startRowIndex, info.HeaderLevel - 1, endRowIndex, (info.HeaderLevel + info.HeaderInfo.ColumnSpan - 2));
                            cell.CellRange = range;
                           this.CoveredRanges.Add(range);
                        }

                        this.ColumnWights[info.HeaderLevel - 1] = this.ColumnWights[info.HeaderLevel - 1] < cell.Width ? cell.Width : this.ColumnWights[info.HeaderLevel - 1];
                        this.Data[startRowIndex][info.HeaderLevel - 1] = cell;
                        this.ClearExpressionEngineValue();
                    }
                }
                else
                {
                    var groupValues = from groupValue in info.GroupValues 
                                      where groupValue.ParentGroupKey.Equals(groupKey) 
                                      select groupValue;

                    var tempvalues = groupValues;

                    bool isRecrsiveGroup = false;

                    if (info.GroupRecursiveParentOrderKey.Count > 1)
                    {
                        isRecrsiveGroup = true;
                        KeysCalculationValues firstVal = info.GroupRecursiveParentOrderKey.First() as KeysCalculationValues;

                        if (firstVal.Keys[0] != null)
                        {
                            firstVal = groupValues.First().GroupKey;
                        }

                        info.GroupRecursiveParentOrderKey.Clear();
                        this.GetHierachy(info.GroupRecursiveParentOrderKey, firstVal, groupValues,0,null);
                    }

                    foreach (KeysCalculationValues key in info.GroupRecursiveParentOrderKey)
                    {
                        if (isRecrsiveGroup)
                        {
                            tempvalues = groupValues.Where(gr => gr.GroupKey.Equals(key)).ToList();
                        }

                        foreach (var valueInfo in tempvalues)
                        {
                            this.Model.ExpressionEngine.GroupLevels["Level"] = valueInfo.Level;
                            if (this.HasRowNumber && !this.tablixItem.Name.Equals(info.Name))
                                this.Model.ExpressionEngine.RowNumbers[info.Name] = 0;

                            int startRowIndex = dataRowIndex;

                            List<ToggleInfo> toggleInfos = null;
                            if (!string.IsNullOrEmpty(valueInfo.ToggleItem) || !string.IsNullOrEmpty(valueInfo.IsHidden))
                            {
                                drillDownInfos.Add(this.GetDrillDownInfo(dataRowIndex + 1, dataRowIndex, valueInfo.ToggleItem, valueInfo.IsHidden, valueInfo.HiddenFields, valueInfo.DataSources, valueInfo.ToggleGroups as List<ToggleGropInfo>, valueInfo.ParentGroupKey, info.HeaderLevel));
                                toggleInfos = drillDownInfos;
                                if (drillDownInfos.Last().Node == null)
                                {
                                    drillDownInfos.Last().Node = new DrillDownModel();
                                }
                                drillDownInfos = drillDownInfos.Last().Node.InnerDrillInfo;
                            }

                            if ((info.PageBreak == BreakLocation.Start || info.PageBreak == BreakLocation.StartAndEnd) &&
                                info.Type == TablixGroupType.Group && !this.GroupBreakLocations.ContainsKey(startRowIndex))
                            {
                                this.GroupBreakLocations.Add(startRowIndex, info.KeepTogether);
                            }
                            if ((info.KeepTogether || hasRepeatHeader) && info.Type == TablixGroupType.Group)
                            {
                                this.GroupRanges.Add(new CoveredGroupRange() { StartIndex = startRowIndex, KeepTogether = info.KeepTogether }, info.Name);
                            }
                            foreach (var groupInfo in info.Groups)
                            {
                                int startPos = dataRowIndex;
                                List<DocumentData> parentdata = null;
                                if (valueInfo.DocumentKey != null)
                                {
                                    if (info.Groups.First().Equals(groupInfo))
                                    {
                                        if (info.Type == TablixGroupType.Group && info.HeaderInfo == null)
                                        {
                                            mapRowIndex = dataRowIndex;
                                        }
                                        DocumentData data = new DocumentData();
                                        data.DocumentLable = this.GetExpressionValue(valueInfo.DataSources, valueInfo.DocumentMapFields, valueInfo.DocumentKey);
                                        documentDatas.Add(data);
                                    }
                                    parentdata = documentDatas;
                                    if (documentDatas.Last().Node == null)
                                    {
                                        documentDatas.Last().Node = new DocumentMapModel();
                                    }
                                    documentDatas = documentDatas.Last().Node.NodeData;
                                }
                                UpdateGroup(groupInfo, null, valueInfo.GroupKey, valueInfo.GroupKey,documentDatas,parentdata,drillDownInfos);
                                if (valueInfo.DocumentKey != null) 
                                {
                                    documentDatas = parentdata;
                                }

                                if (groupInfo.Type != TablixGroupType.Group)
                                {
                                    if (!string.IsNullOrEmpty(groupInfo.ToggleItem) || !string.IsNullOrEmpty(groupInfo.IsHidden))
                                    {
                                        var groupValue = groupInfo.GroupValues.Where(t => t.GroupKey == valueInfo.GroupKey);
                                        drillDownInfos.Add(this.GetDrillDownInfo(startPos + 1, dataRowIndex, groupInfo.ToggleItem, groupInfo.IsHidden, groupInfo.HiddenFields, valueInfo.DataSources, groupInfo.ToggleGroups as List<ToggleGropInfo>, groupValue.First().ParentGroupKey, info.HeaderLevel));
                                    }
                                }

                                //if (groupInfo.Type != TablixGroupType.None)
                                //{
                                //    toggleInfo.StartIndex = startPos;
                                //}
                            }

                            int endRowIndex = dataRowIndex - 1;

                            if (info.Type == TablixGroupType.None && info.KeepWithGroup != KeepWithGroup.None
                                && info.RepeatOnNewPage == true && !RepeatHeaderIndexes.ContainsKey(endRowIndex))
                            {
                                RepeatHeaderIndexes.Add(endRowIndex, new RepeatHeaderInfo() { GroupName = info.Name, KeepWithGroup = info.KeepWithGroup });
                            }
                            if (info.Type == TablixGroupType.Group && info.ParentGroup == null && !GroupEndPositions.Contains(dataRowIndex))
                            {
                                this.GroupEndPositions.Add(dataRowIndex);
                            }
                            if ((info.PageBreak != BreakLocation.Start && info.PageBreak != BreakLocation.None)
                                && info.Type == TablixGroupType.Group && !this.GroupBreakLocations.ContainsKey(dataRowIndex))
                            {
                                if (!(info.PageBreak == BreakLocation.Between && groupValues.Last().Equals(valueInfo)))
                                {
                                    this.GroupBreakLocations.Add(dataRowIndex, info.KeepTogether);
                                }
                            }
                            if ((info.KeepTogether || hasRepeatHeader) && info.Type == TablixGroupType.Group)
                            {
                                var coverGroup = this.GroupRanges.Last();
                                coverGroup.Key.EndIndex = dataRowIndex;
                            }
                            if (info.HeaderInfo != null)
                            {
                                if (this.HasRowNumber && !valueInfo.GroupKey.Equals(rowGroupKeyValue) && info.ParentGroup != null)
                                {
                                    if (!(rowGroupKeyValue.Keys != null && rowGroupKeyValue.Keys.Count > 1 && rowGroupKeyValue.Keys[rowGroupKeyValue.Keys.Count - 2].Equals(valueInfo.GroupKey.Keys.Last())))
                                    {
                                        Dictionary<string, object> rowNumbers = new Dictionary<string, object>(this.Model.ExpressionEngine.RowNumbers);
                                        int value = 0;

                                        foreach (var num in rowNumbers)
                                        {
                                            value = Convert.ToInt32(this.Model.ExpressionEngine.RowNumbers[num.Key]) + valueInfo.DataSources.Count;
                                            this.Model.ExpressionEngine.RowNumbers[num.Key] = value;
                                        }

                                        value = Convert.ToInt32(this.Model.ExpressionEngine.RowNumbers[this.DataSetName]);
                                        this.Model.ExpressionEngine.RowNumbers[this.tablixItem.Name] = value;
                                        rowNumbers = null;
                                    }

                                    rowGroupKeyValue = valueInfo.GroupKey;
                                }

                                TablixCellInfo cell = info.HeaderInfo.Clone() as TablixCellInfo;
                                var row = this.Data[startRowIndex];

                                if (valueInfo.DocumentKey != null && valueInfo.DocumentMapFields!=null && valueInfo.DocumentMapFields.Count>0)
                                {
                                    if (cell.IsDocumentMapRefer == null)
                                    {
                                        cell.IsDocumentMapRefer = new List<DocumentData>();
                                    }
                                    cell.IsMapCell = true;
                                    cell.IsDocumentMapRefer.Add(documentDatas.Last());
                                }
                                else if (info.ParentGroup != null && parentdocumentDatas != null && parentdocumentDatas.Count > 0)
                                {
                                    var valinfo = from valuesinform in info.ParentGroup.GroupValues
                                                  where valuesinform.GroupKey == valueInfo.ParentGroupKey
                                                  select valuesinform;
                                    if (valinfo.Count() > 0 && valinfo.First().DocumentKey != null)
                                    {
                                        if (cell.IsDocumentMapRefer == null)
                                        {
                                            cell.IsDocumentMapRefer = new List<DocumentData>();
                                        }
                                        cell.IsMapCell = true;
                                        cell.IsDocumentMapRefer.Add(parentdocumentDatas.Last());
                                    }
                                }

                                if (cell.HasRowNumber)
                                    UpdateRowCount(cell, info, valueInfo);

                                this.IntilizeExpressionEngineValue(cell, row, startRowIndex, info.HeaderLevel - 1, valueInfo, null, info, null, documentDatas, parentdocumentDatas);                                

                                if (endRowIndex > startRowIndex)
                                {
                                    CoveredCellRange range = new CoveredCellRange(startRowIndex, info.HeaderLevel - 1, endRowIndex, info.HeaderLevel - 1);
                                    cell.CellRange = range;
                                    this.CoveredRanges.Add(range);
                                }
                                else if (info.HeaderInfo.ColumnSpan > 1)
                                {
                                    CoveredCellRange range = new CoveredCellRange(startRowIndex, info.HeaderLevel - 1, endRowIndex, (info.HeaderLevel + info.HeaderInfo.ColumnSpan - 2));
                                    cell.CellRange = range;
                                    this.CoveredRanges.Add(range);
                                }

                                this.ColumnWights[info.HeaderLevel - 1] = this.ColumnWights[info.HeaderLevel - 1] < cell.Width ? cell.Width : this.ColumnWights[info.HeaderLevel - 1];
                                this.Data[startRowIndex][info.HeaderLevel - 1] = cell;
                                this.ClearExpressionEngineValue();
                            }

                            if (!string.IsNullOrEmpty(valueInfo.ToggleItem) || !string.IsNullOrEmpty(valueInfo.IsHidden))
                            {
                                drillDownInfos = toggleInfos;
                                drillDownInfos.Last().EndIndex = dataRowIndex;
                            }
                        }
                    }
                }
            }
            else
            {
                if (info.Groups.Count == 0)
                {
                    int startColumnIndex = this.dataColumnIndex;

                    for (int column = 0; column < info.Columcount; column++)
                    {
                        UpdateColumn(rowInfo, info, rowGroupKey, groupKey, this.startColumnRowIndex, documentDatas, parentdocumentDatas);
                        columnHierachyColumnIndex++;
                    }

                    int endColumnIndex = this.dataColumnIndex - 1;

                    if (info.HeaderInfo != null)
                    {
                        if (this.Data[info.HeaderLevel - 1][startColumnIndex] == null)
                        {
                            var groupValues = from groupValue in info.GroupValues 
                                              where groupValue.ParentGroupKey.Equals(groupKey) 
                                              select groupValue;

                            GroupValueInfo groupValueInfo = groupValues.First();
                            TablixCellInfo cell = info.HeaderInfo.Clone() as TablixCellInfo;
                            var row = this.Data[info.HeaderLevel-1];

                            if (cell.HasRowNumber)
                                UpdateRowCount(cell, info, groupValueInfo);

                            this.IntilizeExpressionEngineValue(cell, row, info.HeaderLevel - 1, startColumnIndex, groupValueInfo, null, info, null, documentDatas, parentdocumentDatas);                           

                            if (endColumnIndex > startColumnIndex)
                            {
                                CoveredCellRange range = new CoveredCellRange(info.HeaderLevel - 1, startColumnIndex, info.HeaderLevel - 1, endColumnIndex);
                                cell.CellRange = range;
                                this.CoveredRanges.Add(range);
                            }
                            else if (info.HeaderInfo.RowSpan > 1)
                            {
                                CoveredCellRange range = new CoveredCellRange(info.HeaderLevel - 1, startColumnIndex, (info.HeaderLevel + info.HeaderInfo.RowSpan - 2), endColumnIndex);
                                cell.CellRange = range;
                                this.CoveredRanges.Add(range);
                            }

                            double cellHeight = cell.Height;
                            cellHeight = cellHeight >= cell.Height && cellHeight >= this.RowHeights[info.HeaderLevel - 1] ? cellHeight : this.RowHeights[info.HeaderLevel - 1];
                            this.RowHeights[info.HeaderLevel - 1] = cellHeight;
                            this.Data[info.HeaderLevel - 1][startColumnIndex] = cell;
                            this.ClearExpressionEngineValue();
                        }
                    }
                }
                else
                {
                    var groupValues = from groupValue in info.GroupValues 
                                      where groupValue.ParentGroupKey.Equals(groupKey) 
                                      select groupValue;

                    int columnHierachyColumnIndexer = this.columnHierachyColumnIndex;

                    bool isRecrsiveGroup = false;

                    if (info.GroupRecursiveParentOrderKey.Count > 1)
                    {
                        isRecrsiveGroup = true;

                        KeysCalculationValues firstVal = info.GroupRecursiveParentOrderKey.First() as KeysCalculationValues;

                        if (firstVal.Keys[0] != null)
                        {
                            firstVal = groupValues.First().GroupKey;
                        }

                        info.GroupRecursiveParentOrderKey.Clear();
                        this.GetHierachy(info.GroupRecursiveParentOrderKey, firstVal, groupValues,0,null);
                    }                  

                    foreach (KeysCalculationValues key in info.GroupRecursiveParentOrderKey)
                    {
                        if (isRecrsiveGroup)
                        {
                            groupValues = groupValues.Where(gr => gr.GroupKey.Equals(key)).ToList();
                        }

                        foreach (var valueInfo in groupValues)
                        {
                            this.columnHierachyColumnIndex = columnHierachyColumnIndexer;
                            int startColumnIndex = this.dataColumnIndex;

                            if (this.HasRowNumber && !this.tablixItem.Name.Equals(info.Name))
                                this.Model.ExpressionEngine.RowNumbers[info.Name] = 0;

                            foreach (var groupInfo in info.Groups)
                            {
                                List<DocumentData> parentdata = null;
                                if (valueInfo.DocumentKey != null)
                                {
                                    if (info.Groups.First().Equals(groupInfo))
                                    {
                                        DocumentData data = new DocumentData();
                                        data.DocumentLable = this.GetExpressionValue(valueInfo.DataSources, valueInfo.DocumentMapFields, valueInfo.DocumentKey);
                                        documentDatas.Add(data);
                                    }
                                    parentdata = documentDatas;
                                    if (documentDatas.Last().Node == null)
                                    {
                                        documentDatas.Last().Node = new DocumentMapModel();
                                    }
                                    documentDatas = documentDatas.Last().Node.NodeData;
                                }
                                UpdateGroup(groupInfo, rowInfo, valueInfo.GroupKey, rowGroupKey,documentDatas,parentdata,drillDownInfos);
                                if (valueInfo.DocumentKey != null)
                                {
                                    documentDatas = parentdata;
                                }
                            }

                            int endColumnIndex = this.dataColumnIndex - 1;

                            if (info.HeaderInfo != null)
                            {
                                if (this.Data[info.HeaderLevel - 1][startColumnIndex] == null)
                                {
                                    if (this.HasRowNumber && !valueInfo.GroupKey.Equals(columnGroupKeyValue) && info.ParentGroup != null)
                                    {
                                        if (!(columnGroupKeyValue.Keys != null && columnGroupKeyValue.Keys.Count > 1 && columnGroupKeyValue.Keys[columnGroupKeyValue.Keys.Count - 2].Equals(valueInfo.GroupKey.Keys.Last())))
                                        {
                                            Dictionary<string, object> rowNumbers = new Dictionary<string, object>(this.Model.ExpressionEngine.RowNumbers);
                                            int value = 0;

                                            foreach (var num in rowNumbers)
                                            {
                                                value = Convert.ToInt32(this.Model.ExpressionEngine.RowNumbers[num.Key]) + valueInfo.DataSources.Count;
                                                this.Model.ExpressionEngine.RowNumbers[num.Key] = value;
                                            }

                                            value = Convert.ToInt32(this.Model.ExpressionEngine.RowNumbers[this.DataSetName]);
                                            this.Model.ExpressionEngine.RowNumbers[this.tablixItem.Name] = value;
                                            rowNumbers = null;
                                        }

                                        columnGroupKeyValue = valueInfo.GroupKey;
                                    }

                                    TablixCellInfo cell = info.HeaderInfo.Clone() as TablixCellInfo;
                                    var row = this.Data[info.HeaderLevel - 1];

                                    if (valueInfo.DocumentKey != null && valueInfo.DocumentMapFields != null && valueInfo.DocumentMapFields.Count > 0)
                                    {
                                        if (cell.IsDocumentMapRefer == null)
                                        {
                                            cell.IsDocumentMapRefer = new List<DocumentData>();
                                        }
                                        cell.IsMapCell = true;
                                        cell.IsDocumentMapRefer.Add(documentDatas.Last());
                                    }
                                    else if (info.ParentGroup != null && parentdocumentDatas != null && parentdocumentDatas.Count > 0)
                                    {
                                        var valinfo = from valuesinform in info.ParentGroup.GroupValues
                                                      where valuesinform.GroupKey == valueInfo.ParentGroupKey
                                                      select valuesinform;
                                        if (valinfo.Count() > 0 && valinfo.First().DocumentKey != null)
                                        {
                                            if (cell.IsDocumentMapRefer == null)
                                            {
                                                cell.IsDocumentMapRefer = new List<DocumentData>();
                                            }
                                            cell.IsMapCell = true;
                                            cell.IsDocumentMapRefer.Add(parentdocumentDatas.Last());
                                        }
                                    }


                                    if (cell.HasRowNumber)
                                        UpdateRowCount(cell, info, valueInfo);

                                    this.IntilizeExpressionEngineValue(cell, row, info.HeaderLevel - 1, startColumnIndex, valueInfo, null, info, null, documentDatas, parentdocumentDatas);

                                    if (endColumnIndex > startColumnIndex)
                                    {
                                        CoveredCellRange range = new CoveredCellRange(info.HeaderLevel - 1, startColumnIndex, info.HeaderLevel - 1, endColumnIndex);
                                        cell.CellRange = range;
                                        this.CoveredRanges.Add(range);
                                    }
                                    else if (info.HeaderInfo.RowSpan > 1)
                                    {
                                        CoveredCellRange range = new CoveredCellRange(info.HeaderLevel - 1, startColumnIndex, (info.HeaderLevel + info.HeaderInfo.RowSpan - 2), endColumnIndex);
                                        cell.CellRange = range;
                                        this.CoveredRanges.Add(range);
                                    }

                                    double cellHeight = cell.Height;
                                    cellHeight = cellHeight >= cell.Height && cellHeight >= this.RowHeights[info.HeaderLevel - 1] ? cellHeight : this.RowHeights[info.HeaderLevel - 1];
                                    this.RowHeights[info.HeaderLevel - 1] = cellHeight;
                                    this.Data[info.HeaderLevel - 1][startColumnIndex] = cell;
                                    this.ClearExpressionEngineValue();
                                }
                            }
                        }
                    }
                }
            }
        }

        ToggleInfo GetDrillDownInfo(int startIndex, int endIndex, string toggleItem, string isHidden, List<DataField> hiddenFields, object dataSources, List<ToggleGropInfo> toggleGroups, KeysCalculationValues parentKey, int headerLevel)
        {
            ToggleInfo togInfo = new ToggleInfo();
            togInfo.StartIndex = startIndex;
            togInfo.ToggleItem = toggleItem;
            togInfo.EndIndex = endIndex;
            togInfo.HeaderLevel = headerLevel;

            if (!string.IsNullOrEmpty(isHidden))
            {
                togInfo.IsHidden = bool.Parse(this.GetExpressionValue(dataSources, hiddenFields, isHidden));
            }

            if (toggleGroups != null)
            {
                if (!isHeader.Contains(toggleItem))
                {
                    if (toggleGroups.Count > 0 && toggleGroups.Last().ColNo == -1 && toggleGroups.Last().RowNo == -1)
                    {
                        if (toggleGroups.Last().DrillDownInfos == null)
                        {
                            toggleGroups.Last().DrillDownInfos = new DrillDownModel();
                        }
                        if (toggleGroups.Last().DrillDownInfos.InnerDrillInfo.Last().ParentGroupKey != parentKey.ToString())
                        {
                            togInfo.IsRow = true;
                        }
                        togInfo.ParentGroupKey = parentKey.ToString();
                        toggleGroups.Last().DrillDownInfos.InnerDrillInfo.Add(togInfo);
                    }
                    else
                    {
                        ToggleGropInfo groupInfo = new ToggleGropInfo();
                        groupInfo.ColNo = -1;
                        groupInfo.RowNo = -1;
                        if (groupInfo.DrillDownInfos == null)
                        {
                            groupInfo.DrillDownInfos = new DrillDownModel();
                        }
                        groupInfo.DrillDownInfos.InnerDrillInfo.Add(togInfo);
                        togInfo.ParentGroupKey = parentKey.ToString();
                        togInfo.IsRow = true;
                        toggleGroups.Add(groupInfo);
                    }
                }
                else
                {
                    if (toggleGroups.Last().DrillDownInfos == null)
                    {
                        toggleGroups.Last().DrillDownInfos = new DrillDownModel();
                    }
                    toggleGroups.Last().DrillDownInfos.InnerDrillInfo.Add(togInfo);
                }
            }

            togInfo.Node = new DrillDownModel();
            return togInfo;
        }

        private void UpdateColumn(GroupInfo rowInfo, GroupInfo columnInfo, KeysCalculationValues rowGroupKey, KeysCalculationValues ColumnGroupKey, int startRowIndex, List<DocumentData> documentDatas, List<DocumentData> parentdocumentDatas)
        {
            var columnGroupValues = from groupValue in columnInfo.GroupValues 
                                    where groupValue.ParentGroupKey.Equals(ColumnGroupKey) 
                                    select groupValue;

            GroupValueInfo columnGroupValue = null;

            if (columnGroupValues.Count() > 0)
            {
                columnGroupValue = columnGroupValues.First();
            }

            if (columnInfo.Type == TablixGroupType.None || (columnInfo.Type == TablixGroupType.Detail && columnGroupValue != null))
            {
                int dataColumnCount = columnInfo.Type == TablixGroupType.Detail ? columnGroupValue.DataSources.Count : 1;

                for (int columnsCount = 0; columnsCount < dataColumnCount; columnsCount++)
                {
                    var rowGroupValues = from groupValue in rowInfo.GroupValues 
                                         where groupValue.ParentGroupKey.Equals(rowGroupKey) 
                                         select groupValue;

                    GroupValueInfo rowGroupValue = null;

                    if (rowGroupValues.Count() > 0)
                    {
                        rowGroupValue = rowGroupValues.First();
                    }

                    if (rowInfo.Type == TablixGroupType.None || (rowInfo.Type == TablixGroupType.Detail && rowGroupValue != null))
                    {
                        int dataRowCount = rowInfo.Type == TablixGroupType.Detail ? rowGroupValue.DataSources.Count : 1;
                        this.dataRowIndex = this.startColumnRowIndex;

                        for (int data = 0; data < dataRowCount; data++)
                        {
                            if (this.HasRowNumber && rowInfo.Type == TablixGroupType.Detail)
                            {
                                this.rowIndex++;
                                this.Model.ExpressionEngine.RowNumbers[this.tablixItem.Name] = this.rowIndex;
                                this.Model.ExpressionEngine.RowNumbers[this.DataSetName] = this.rowIndex;
                                this.Model.ExpressionEngine.RowNumbers[rowInfo.Name] = rowInfo.Rowcount;
                                Dictionary<string, object> rowNumbers = new Dictionary<string, object>(this.Model.ExpressionEngine.RowNumbers);

                                foreach (var num in rowNumbers)
                                {
                                    if (!(num.Key.Equals(this.tablixItem.Name) || num.Key.Equals(this.DataSetName) || num.Key.Equals(rowInfo.Name)))
                                    {
                                        var value = (Convert.ToInt32(num.Value) + 1);
                                        this.Model.ExpressionEngine.RowNumbers[num.Key] = value <= this.rowIndex ? value : num.Value;
                                    }
                                }
                            }

                            for (int i = 0; i < rowInfo.Rowcount; i++)
                            {
                                var row = rowInfo.RowValues[i];

                                if (row[columnHierachyColumnIndex] != null)
                                {
                                    TablixCellInfo cell = row[columnHierachyColumnIndex].Clone() as TablixCellInfo;

                                    if (columnInfo.HasRowNumber)
                                    {
                                        cell.HasRowNumber = true;
                                        UpdateRowCount(cell, rowInfo, rowGroupValue);
                                    }

                                    if (this.Data.Count <= dataRowIndex)
                                    {
                                        this.Data.Add(new Internal.TablixRow());
                                        this.RowHeights.Add(0);
                                        for (int j = 0; j < ColumnCount; j++)
                                        {
                                            this.Data[dataRowIndex].Add(null);
                                        }
                                    }

                                    var rowdata = this.Data[dataRowIndex];

                                    if (rowInfo.Type == TablixGroupType.None && rowGroupValue != null && columnGroupValue != null && rowInfo != null)
                                    {
                                        this.IntilizeExpressionEngineValue(cell, rowdata, dataRowIndex, dataColumnIndex, rowGroupValue, columnGroupValue, rowInfo, null, documentDatas, parentdocumentDatas);
                                    }
                                    else if (rowGroupValue != null && columnGroupValue != null && rowInfo != null)
                                    {
                                        this.IntilizeExpressionEngineValue(cell, rowdata, dataRowIndex, dataColumnIndex, rowGroupValue, columnGroupValue, rowInfo, data, documentDatas, parentdocumentDatas);
                                    }

                                    if (cell.ColumnSpan > 1)
                                    {
                                        CoveredCellRange range = new CoveredCellRange(dataRowIndex, dataColumnIndex, dataRowIndex, dataColumnIndex + cell.ColumnSpan - 1);
                                        cell.CellRange = range;
                                        this.CoveredRanges.Add(range);
                                    }

                                    double cellHeight = cell.Height;
                                    cellHeight = cell.Height > cellHeight ? cell.Height : cellHeight;
                                    cellHeight = cellHeight >= this.RowHeights[dataRowIndex] ? cellHeight : this.RowHeights[dataRowIndex];
                                    this.RowHeights[dataRowIndex] = cellHeight;

                                    if (cell.ColumnSpan == 1)
                                    {
                                        this.ColumnWights[this.dataColumnIndex] = this.ColumnWights[dataColumnIndex] < cell.Width ? cell.Width : this.ColumnWights[dataColumnIndex];
                                    }
                                    else
                                    {
                                        for (int column = 0; column < cell.ColumnSpan; column++)
                                        {
                                            this.ColumnWights[this.dataColumnIndex + column] = this.ColumnWights[dataColumnIndex + column] < cell.Widths[column] ? cell.Widths[column] : this.ColumnWights[dataColumnIndex + column];
                                        }
                                    }
                                    
                                    this.Data[dataRowIndex][dataColumnIndex] = cell;
                                    this.ClearExpressionEngineValue();
                                }

                                dataRowIndex++;
                            }
                            if (rowInfo.PageBreak != BreakLocation.None && rowInfo.Type == TablixGroupType.Detail)
                            {
                                int index = dataRowIndex;
                                bool canAdd = true;

                                if ((rowInfo.PageBreak == BreakLocation.Start || rowInfo.PageBreak == BreakLocation.StartAndEnd
                                    || rowInfo.PageBreak == BreakLocation.End))
                                {
                                    index = rowInfo.PageBreak == BreakLocation.End ? index : index - 1;

                                    if ((rowInfo.PageBreak == BreakLocation.StartAndEnd || rowInfo.PageBreak == BreakLocation.End)
                                        && !GroupBreakLocations.ContainsKey(dataRowIndex))
                                    {
                                        GroupBreakLocations.Add(dataRowIndex,rowInfo.KeepTogether);
                                    }
                                }
                                else
                                {
                                    index = data + 1 == dataRowCount ? index - 1 : index;
                                    canAdd = dataRowCount > 1;
                                }
                                if (canAdd && !GroupBreakLocations.ContainsKey(index))
                                {
                                    GroupBreakLocations.Add(index, rowInfo.KeepTogether);
                                }
                            }
                        }
                    }

                    this.dataColumnIndex++;
                }
            }
        }

        private void UpdateRowCount(TablixCellInfo cell, GroupInfo rowInfo, GroupValueInfo rowGroupValue)
        {
            if (cell.ItemModel.ModelType == ModelType.TextBoxModel)
            {
                var txtbox = cell.ItemModel.ReportItem as TextBox;
                string txt = (from text in txtbox.Paragraphs
                              where text.TextRuns.FirstOrDefault() != null && text.TextRuns.FirstOrDefault().Value.Contains("=RowNumber(")
                              select text.TextRuns.FirstOrDefault().Value.Replace("=RowNumber(", "")).FirstOrDefault();

                if (!string.IsNullOrEmpty(txt))
                {
                    txt = txt.Replace(')', ' ').Trim();
                    txt = txt.Contains("\"") ? txt.Replace("\"", " ").Trim() : txt;

                    if (txt.Equals(rowInfo.Name) && !cell.IsDetailCell)
                    {
                        this.Model.ExpressionEngine.RowNumbers[rowInfo.Name] = rowGroupValue.DataSources.Count;
                    }
                    else if (!txt.ToLower().Equals("nothing") && !ValidateGroupScope(rowInfo, txt)
                        && !txt.Equals(this.DataSetName) && !txt.Equals(this.tablixItem.Name))
                    {
                        //throw new Exception(string.Format("The Value expression for the text box {0} has a scope parameter that is not valid for the aggregate function." +
                        //    "\n The scope parameter must be set to a string constant that is equal to either the name of the containing group," +
                        //    "name of the containing data region or name of the data set.", txtbox.Name));
                    }
                }
            }
        }

        private bool ValidateGroupScope(GroupInfo rowInfo, string txt)
        {
            if (rowInfo.ParentGroup != null)
            {
                return rowInfo.ParentGroup.Name.Equals(txt) ? true : ValidateGroupScope(rowInfo.ParentGroup, txt);
            }

            return false;
        }

        public override void DisposeEvalObjects()
        {
            this.Data = null;
            this.ColumnWights = null;
            this.RowHeights = null;
            this.RepeatHeaderIndexes = null;
            this.GroupBreakLocations = null;
            this.GroupEndPositions = null;
            base.DisposeEvalObjects();
        }

        public override void DisposeReportItemObj()
        {
            this.coveredRanges = null;
            this.ContainerModel = null;
            this.DataSetFields = null;
            this.DataSetName = null;
            this.DataSource = null;
            this.Engine = null;
            this.ExpFilters = null;
            this.FieldValues = null;
            this.GroupLevels = null;
            this.ItemPosition = null;
            base.DisposeReportItemObj();
        }

        void EvaluateValue(TablixCellInfo cell,int row,int column)
        {
            if (this.Model.EnableVirtualEvaluation)
            {
                cell.CurrentKey = new List<int>();
                cell.CurrentKey.Add(row);
                cell.CurrentKey.Add(column);
                if (cell.ItemModel.ModelType == Internal.ModelType.TablixModel)
                {
                    cell.Evaluate();
                    TablixEvaluationItems items = new TablixEvaluationItems();
                    var txModel = cell.ItemModel as TablixModel;
                    items.Rows = txModel.Data;
                    items.RowCount = txModel.RowCount;
                    items.ColumnCount = txModel.ColumnCount;
                    items.ColumnWidths = txModel.ColumnWights;
                    items.RowHeights = txModel.RowHeights;
                    items.RepeatHeaderIndexes = txModel.RepeatHeaderIndexes;
                    items.GroupBreakLocations = txModel.GroupBreakLocations;
                    items.GroupEndPositions = txModel.GroupEndPositions;
                    items.FlowLayoutInfo = txModel.FlowLayoutInfo;
                    items.PageLayoutInfo = txModel.PageInfo;
                    items.PrintPageLayoutInfo = txModel.PrintPageInfo;
                    items.PageSize = txModel.PageSizes;
                    items.FlowPageSize = txModel.FlowPageSizes;
                    items.PrintPageSize = txModel.PrintPageSizes;
                    items.ItemPostion = txModel.ItemPosition;

                    if (cell.Rows[row].TablixValues == null)
                    {
                        cell.Rows[row].TablixValues=new List<Dictionary<string, TablixEvaluationItems>>();
                    }

                    for (int i = cell.Rows[row].TablixValues.Count; i < (column + 1 ) ; i++)
                    {
                        cell.Rows[row].TablixValues.Add(null);
                    }

                    Dictionary<string, TablixEvaluationItems> tablixItem = cell.Rows[row].TablixValues[column];

                    if (tablixItem == null)
                    {
                        tablixItem = new Dictionary<string, TablixEvaluationItems>();
                        cell.Rows[row].TablixValues[column] = tablixItem;
                    }
                        
                    tablixItem.Add(cell.ItemModel.Name,items);

                    foreach (var rowValue in items.Rows)
                    {
                        if (rowValue != null)
                        {
                            foreach (var cellValue in rowValue)
                            {
                                if (cellValue != null)
                                {
                                    if (items.CellRows == null)
                                    {
                                        items.CellRows = new Dictionary<string, Internal.TablixRows>();
                                    }

                                    if (!items.CellRows.ContainsKey(cellValue.ItemModel.Name))
                                    {
                                        items.CellRows.Add(cellValue.ItemModel.Name, cellValue.Rows);
                                        cellValue.Rows = null;
                                    }
                                }
                            }
                        }
                    }

                    cell.DisposeEvalObjects();
                }
                else
                {
                    if (cell != null && cell.ItemModel!=null)
                    {
                        if (cell.ItemModel.DocumentMapLable != null && !string.IsNullOrEmpty(cell.ItemModel.DocumentMapLable))
                        {
                            cell.ItemModel.DocumentMapLable = this.GetExpressionValue(cell.GetDataSource(row,column), cell.ItemModel.DataSetFields, cell.ItemModel.DocumentMapLable);
                        }
                    }
                }
            }
            else
            {
                cell.Evaluate();
            }
        }

        internal void UpdateTablixValue(TablixModel model,TablixEvaluationItems item)
        {
            model.Data = item.Rows;
            model.RowCount = item.RowCount;
            model.ColumnCount = item.ColumnCount;
            model.ColumnWights = item.ColumnWidths;
            model.RowHeights = item.RowHeights;
            model.RepeatHeaderIndexes = item.RepeatHeaderIndexes;
            model.GroupBreakLocations = item.GroupBreakLocations;
            model.GroupEndPositions = item.GroupEndPositions;
            model.FlowLayoutInfo = item.FlowLayoutInfo;
            model.PageInfo = item.PageLayoutInfo;
            model.PrintPageInfo = item.PrintPageLayoutInfo;
            model.PageSizes = item.PageSize;
            model.FlowPageSizes = item.FlowPageSize;
            model.PrintPageSizes = item.PrintPageSize;
            model.ItemPosition = item.ItemPostion;

            foreach (var row in item.Rows)
            {
                foreach (var cell in row)
                {
                    if (cell != null)
                    {
                        cell.Rows = item.CellRows[cell.ItemModel.Name];
                    }
                }
            }
        }

        KeysCalculationValues emptyKey = new KeysCalculationValues();

        protected void IntilizeExpressionEngineValue(TablixCellInfo cell,Syncfusion.RDL.Internal.TablixRow tablixRow, int row,int column, GroupValueInfo valueInfo, GroupValueInfo columnValueInfo, GroupInfo groupInfo, int? rowIndex,List<DocumentData> documentDatas,List<DocumentData> parentDatas)
        {
            this.Model.ExpressionEngine.FieldValues = new Dictionary<string, object>();
            TablixEngineFieldValue value = rowIndex == null ? valueInfo.Values.First() : null ;

            KeysCalculationValues columnKey = columnValueInfo == null ? new KeysCalculationValues() : columnValueInfo.GroupKey;
            KeysCalculationValues rowKey = valueInfo == null ? new KeysCalculationValues() : valueInfo.GroupKey;

            List<IComparable> groupKeys = new List<IComparable>();

            if (rowKey.Keys != null && rowKey.Keys.Count > 0)
            {
                groupKeys.AddRange(rowKey.Keys);
            }

            if (columnKey.Keys != null && columnKey.Keys.Count > 0)
            {
                groupKeys.AddRange(columnKey.Keys);
            }

            KeysCalculationValues fieldGroupKey = emptyKey;

            if (groupKeys.Count > 0)
            {
                fieldGroupKey = new KeysCalculationValues();
                fieldGroupKey.Keys = groupKeys;
            }

            if (this.Model.EnableVirtualEvaluation)
            {
                if (this.HasRowNumber && tablixRow.RowNumberValues == null)
                {
                    tablixRow.RowNumberValues = new List<Dictionary<string, object>>();
                }

                if (cell.Rows.Count < (row + 1))
                {
                    for (int i = cell.Rows.Count; i < (row + 1); i++)
                    {
                        cell.Rows.Add(null);
                    }
                }

                if (cell.Rows[row] == null)
                {
                    cell.Rows[row] = tablixRow;
                }

                if (tablixRow.FieldValues.Count < (column + 1))
                {
                    for (int i = tablixRow.FieldValues.Count; i < (column + 1); i++)
                    {
                        tablixRow.FieldValues.Add(null);
                        tablixRow.DataSource.Add(null);

                        if (tablixRow.RowNumberValues != null)
                            tablixRow.RowNumberValues.Add(null);
                    }
                }

                if (cell.IsDetailCell)
                {
                    if (fieldGroupKey.Keys != null && fieldGroupKey.Keys.Count > 0)
                    {
                        if (valueInfo.GroupFieldKeys.GetKey(fieldGroupKey) >= 0)
                        {
                            tablixRow.DataSource[column] =  valueInfo.DataSources[(int)rowIndex];
                        }
                    }
                    else
                    {
                        tablixRow.DataSource[column] = valueInfo.DataSources[(int)rowIndex];
                    }
                }
                else
                {
                    if (fieldGroupKey.Keys != null && fieldGroupKey.Keys.Count > 0)
                    {
                        if (valueInfo.GroupFieldKeys.GetKey(fieldGroupKey) >= 0)
                        {
                            var fields = from fieldVal in value.Value
                                         where (fieldVal.GroupKey.Equals(fieldGroupKey))
                                         select fieldVal;

                            tablixRow.DataSource[column] = valueInfo.DataSources.ToList();
                            tablixRow.FieldValues[column] = fields.ToList();
                        }
                        else if (fieldGroupKey.Keys.Count > 0)
                        {
                            var fields = from fieldVal in value.Value
                                         where (fieldVal.GroupKey.Equals(fieldGroupKey))
                                         select fieldVal;

                            tablixRow.DataSource[column] = valueInfo.DataSources.ToList();
                            tablixRow.FieldValues[column] = fields.ToList();   
                        }
                    }
                    else
                    {
                        var fields = from fieldVal in value.Value
                                     where (fieldVal.GroupKey.Equals(fieldGroupKey))
                                     select fieldVal;

                        tablixRow.DataSource[column] = valueInfo.DataSources.ToList();
                        tablixRow.FieldValues[column] = fields.ToList();
                    }
                }
                if (cell.HasRowNumber)
                {
                    tablixRow.RowNumberValues[column] = new Dictionary<string, object>(this.Model.ExpressionEngine.RowNumbers);
                }
            }
            else
            {
                if (fieldGroupKey.Keys != null && fieldGroupKey.Keys.Count > 0)
                {
                    if (valueInfo.GroupFieldKeys.GetKey(fieldGroupKey) >= 0 )
                    {
                        if (cell.IsDetailCell)
                        {
                            List<object> dataSources = new List<object>();
                            dataSources.Add(valueInfo.DataSources[(int)rowIndex]);
                            cell.DataSource = dataSources;
                        }
                        else
                        {
                            var fields = from fieldVal in value.Value
                                         where (fieldVal.GroupKey.Equals(fieldGroupKey))
                                         select fieldVal;

                            cell.DataSource = valueInfo.DataSources;
                            cell.FieldValues = fields.ToList();
                        }
                    }
                    else
                    {
                        if (fieldGroupKey.Keys.Count > 0)
                        {
                            if (cell.IsDetailCell)
                            {
                                List<object> dataSources = new List<object>();
                                dataSources.Add(valueInfo.DataSources[(int) rowIndex]);
                                cell.DataSource = dataSources;
                            }
                            else
                            {
                                var fields = from fieldVal in value.Value
                                             where (fieldVal.GroupKey.Equals(fieldGroupKey))
                                             select fieldVal;

                                cell.DataSource = valueInfo.DataSources;
                                cell.FieldValues = fields.ToList();
                            }
                        }
                    }
                }
                else
                {
                    if (cell.IsDetailCell)
                    {
                        List<object> dataSources = new List<object>();
                        dataSources.Add(valueInfo.DataSources[(int)rowIndex]);
                        cell.DataSource = dataSources;
                    }
                    else
                    {
                        var fields = from fieldVal in value.Value
                                     where (fieldVal.GroupKey.Equals(fieldGroupKey))
                                     select fieldVal;

                        cell.DataSource = valueInfo.DataSources;
                        cell.FieldValues = fields.ToList();
                    }
                }
                if (cell.HasRowNumber)
                {
                    cell.RowNumbers = this.Model.ExpressionEngine.RowNumbers;
                }
            }

            if (cell.ItemModel.ModelType == ModelType.TextBoxModel && (cell.ItemModel as TextboxModel).ToggleGroups != null)
            {
                var togGroups = (cell.ItemModel as TextboxModel).ToggleGroups.Where(t => t.ColNo == -1 && t.RowNo == -1);
                if (togGroups.Count() > 0)
                {
                    var togGroup = togGroups.First();
                    togGroup.ColNo = column;
                    togGroup.RowNo = row;
                }
                else
                {
                    if ((cell.ItemModel as TextboxModel).ToggleGroups.Count == 0)
                    {
                        if (isHeader == null)
                        {
                            isHeader = new List<string>();
                        }
                        isHeader.Add(cell.ItemModel.Name);
                    }
                    (cell.ItemModel as TextboxModel).ToggleGroups.Add(new ToggleGropInfo() { ColNo = column, RowNo = row });
                }
            }

            if (cell.ItemModel.ModelType == ModelType.TablixModel || cell.ItemModel.ModelType==ModelType.RectangleModel)
            {
                if (this.ItemPosition.ContainsKey(row))
                {
                    this.ItemPosition[row].Add(column);
                }
                else
                {
                    var templt=new List<int>();
                    templt.Add(column);
                    this.ItemPosition.Add(row, templt);
                }
            }

            if (cell != null && cell.IsMapCell && this.Model.EnableVirtualEvaluation)
            {
                if (cell.IsDocumentMapRefer != null && cell.IsDocumentMapRefer.Count > 0)
                {
                    cell.IsDocumentMapRefer.Last().PosInfos=(new CellPosInfo() { ColNo = column, RowNo = row });
                }
            }

            if (valueInfo.DocumentKey != null && groupInfo.Type == TablixGroupType.Detail)
            {
                var documentData = documentDatas.Where(t => (t.ReferRowInfo != null && t.ReferRowInfo.Count > 0 && t.ReferRowInfo.First().Key == dataRowIndex && t.ReferRowInfo.First().Value == valueInfo.GroupKey));
                if (!documentData.Any())
                {
                    DocumentData data = new DocumentData();
                    data.DocumentLable = this.GetExpressionValue(this.Model.EnableVirtualEvaluation ? cell.GetDataSource(row,column) : cell.DataSource, valueInfo.DocumentMapFields, valueInfo.DocumentKey);
                    data.ReferRowInfo = new Dictionary<int, KeysCalculationValues> { { dataRowIndex, valueInfo.GroupKey } };
                    if (this.Model.EnableVirtualEvaluation)
                    {
                        data.PosInfos=(new CellPosInfo() {ColNo = column, RowNo = row});
                    }
                    if (cell.IsDocumentMapRefer == null)
                    {
                        cell.IsDocumentMapRefer = new List<DocumentData>();
                    }
                    cell.IsDocumentMapRefer.Add(data);
                    documentDatas.Add(data);
                }
            }
            else if (groupInfo.Type == TablixGroupType.None && dataRowIndex == mapRowIndex && parentDatas!=null && parentDatas.Count>0)
            {
                if (this.Model.EnableVirtualEvaluation)
                {
                    parentDatas.Last().PosInfos = (new CellPosInfo() { ColNo = column, RowNo = row });
                }
                if (cell.IsDocumentMapRefer == null)
                {
                    cell.IsDocumentMapRefer = new List<DocumentData>();
                }
                cell.IsDocumentMapRefer.Add(parentDatas.Last());
                mapRowIndex = -1;
            }

            var mapKey = cell.ItemModel.DocumentMapLable;

            this.EvaluateValue(cell,row,column);

            if (cell != null && cell.ItemModel != null)
            {
               if (cell.ItemModel.DocumentMapLable != null)
               {
                   var documentData = documentDatas.Where(t => (t.ReferRowInfo != null && t.ReferRowInfo.Count > 0 && t.ReferRowInfo.First().Key == dataRowIndex && t.ReferRowInfo.First().Value==valueInfo.GroupKey));
                   if (documentData.Any())
                   {
                       DocumentData data = new DocumentData();
                       data.DocumentLable =cell.ItemModel.DocumentMapLable;
                       if (cell.IsDocumentMapRefer == null)
                       {
                           cell.IsDocumentMapRefer = new List<DocumentData>();
                       }
                       if (this.Model.EnableVirtualEvaluation)
                       {
                           data.PosInfos=(new CellPosInfo() { ColNo = column, RowNo = row });
                       }
                       cell.IsDocumentMapRefer.Add(data);
                       if (documentData.First().Node == null)
                       {
                           documentData.First().Node=new DocumentMapModel();
                       }
                       documentData.First().Node.NodeData.Add(data);
                   }
                   else
                   {
                       DocumentData data = new DocumentData();
                       data.DocumentLable = cell.ItemModel.DocumentMapLable;
                       if (cell.IsDocumentMapRefer == null)
                       {
                           cell.IsDocumentMapRefer=new List<DocumentData>();
                       }
                       data.PosInfos=(new CellPosInfo() { ColNo = column, RowNo = row });
                       cell.IsDocumentMapRefer.Add(data);
                       documentDatas.Add(data);                       
                   }
               }
                cell.IsMapCell = false;
            }
            cell.ItemModel.DocumentMapLable = mapKey;
        }

        string GetExpressionValue(object dataSource, List<DataField> dataFields, string expKey)
        {
            ReportingAggEngine engine = new ReportingAggEngine();
            string strValue = expKey;
            engine.DataSource = dataSource;
            engine.Model = this.Model;
            if (dataFields != null)
            {
                engine.Fields = new List<DataField>(dataFields);
            }

            engine.PopulateValue();

            var engineOldValue = this.Model.ExpressionEngine.FieldValues;

            this.Model.ExpressionEngine.FieldValues = new Dictionary<string, object>();

            if (engine.Fields != null)
            {
                foreach (DataField field in engine.Fields)
                {
                    this.Model.ExpressionEngine.FieldValues.Add(field.Name, field.Value.Value.GetResult());
                }
            }

            strValue = this.Model.ExpressionEngine.GetEvalExpressionString(expKey);
            this.Model.ExpressionEngine.FieldValues = engineOldValue;
            engine.DisposeEngine();
            return strValue;
        }


        protected void ClearExpressionEngineValue()
        {
            if (this.Model.ExpressionEngine.FieldValues != null)
            {
                this.Model.ExpressionEngine.FieldValues.Clear();
            }
        }

        private double AddRepeatedRows(List<int> rowIndices,double pageHeight, int rowValue, bool isFirstPageSet, bool rowReachedEnd, bool addRow)
        {
            try
            {
                if (this.RepeatHeaderIndexes.Count > 0)
                {
                    int max = this.RowCount;
                    int groupEnd = (from row in this.GroupEndPositions where row >= rowValue select row).FirstOrDefault();
                    var keys = (from pair in this.RepeatHeaderIndexes
                                where pair.Value.Equals("nextgroup") && (rowIndices.Count == 0 || rowIndices.Count != 0 && pair.Key >= rowIndices.Last())
                                select pair).FirstOrDefault();

                    max = keys.Value != null ? keys.Key : max;
                    keys = (from pair in this.RepeatHeaderIndexes where pair.Value.Equals("nextgroup") select pair).FirstOrDefault();
                    List<string> evalName = new List<string>();
                    evalName.Add(this.tablixItem.Name);

                    for (int index = 0; index < this.RepeatHeaderIndexes.Count; index++)
                    {
                        var keyValue = this.RepeatHeaderIndexes.ElementAt(index);

                        if (keyValue.Value.Equals(this.tablixItem.Name) && !rowIndices.Contains(keyValue.Key + 1))
                        {
                            if (isFirstPageSet || (isFirstPageSet || keyValue.Key > max / 2))
                            {
                                if (isFirstPageSet && !addRow && keyValue.Key < max / 2)
                                {
                                    if (keys.Value == null || (keys.Value != null && keyValue.Key < (keys.Key / 2)))
                                    {
                                        rowIndices.Insert(index, keyValue.Key + 1);
                                    }
                                }
                                else if (addRow && !rowReachedEnd && keyValue.Key > max / 2 &&
                                    keyValue.Key <= max && keyValue.Key >= rowValue)
                                {
                                    rowIndices.Add(keyValue.Key + 1);
                                }

                                if (!addRow)
                                    pageHeight += this.RowHeights[keyValue.Key];
                            }
                        }
                        else if (!evalName.Contains(keyValue.Value.GroupName) && keyValue.Key < max)
                        {
                            if (this.RepeatHeaderIndexes.ContainsKey(keyValue.Key - 1) && (this.RepeatHeaderIndexes[keyValue.Key - 1].Equals(this.tablixItem.Name) ||
                                this.RepeatHeaderIndexes[keyValue.Key - 1].Equals("nextgroup")))
                            {
                                evalName.Add(keyValue.Value.GroupName);
                            }
                            else if (keyValue.Key >= rowValue)
                            {
                                if (!(this.RepeatHeaderIndexes.ContainsKey(keyValue.Key + 1) && this.RepeatHeaderIndexes[keyValue.Key + 1].Equals(keyValue.Value)))
                                {
                                    evalName.Add(keyValue.Value.GroupName);
                                }
                                if (addRow && !rowIndices.Contains(keyValue.Key + 1) && keyValue.Key <= groupEnd)
                                {
                                    rowIndices.Add(keyValue.Key + 1);
                                }

                                pageHeight = (!addRow) ? pageHeight + this.RowHeights[keyValue.Key] : pageHeight;
                                pageHeight = rowReachedEnd ? pageHeight - this.RowHeights[keyValue.Key] : pageHeight;
                            }
                        }
                    }

                    evalName.Clear();
                    evalName = null;
                }
            }
            catch { }

            return pageHeight;
        }

        #endregion

        #region overridden methods

        public override IReportItemModeler GetModel()
        {
            TablixModel itemModel = new TablixModel();
            itemModel.IsTablixChild = this.IsTablixChild;
            itemModel.IsTablixInnerChild = this.IsTablixInnerChild;
            itemModel.ModelType = this.ModelType;
            itemModel.Model = this.Model;
            itemModel.tablixItem = this.tablixItem;
            itemModel.ReportItem = this.ReportItem;
            itemModel.Name = this.Name;
            itemModel.Top = this.Top;
            itemModel.Left = this.Left;
            itemModel.Width = this.Width;
            itemModel.Height = this.Height;
            itemModel.engineGroupCollection = this.engineGroupCollection;
            itemModel.ColumnGroupcollection = this.ColumnGroupcollection;
            itemModel.RowGroupcollection = this.RowGroupcollection;
            itemModel.columnHeaderCount = this.columnHeaderCount;
            itemModel.rowHeaderCount = this.rowHeaderCount;
            itemModel.DataSetFields = this.DataSetFields;
            itemModel.HiddenExpKey = this.HiddenExpKey;

            return itemModel;
        }

        public override void Evaluate()
        {
            try
            {
                bool isMapHeader=false;
                if (this.coveredRanges != null)
                {
                    this.coveredRanges.Clear();
                }
                if (this.GroupRanges == null)
                {
                    this.GroupRanges = new Dictionary<CoveredGroupRange, string>();
                }
                else
                {
                    this.GroupRanges.Clear();
                }

                this.Engine = new TablixEngine();

                if (this.DataSource == null)
                {
                    var viewAdv = (from view in this.Model.ProcessedData.DataSourceObjects
                                   where view.Key == this.DataSetName
                                   select view.Value).SingleOrDefault();
                    this.Engine.DataSource = this.Model.ProcessedData.FilterItemSoruce(viewAdv, this.ExpFilters);
                }
                else
                {
                    this.Engine.DataSource = this.Model.ProcessedData.FilterItemSoruce(this.DataSource as IEnumerable, this.ExpFilters);
                }

                if (this.ItemPosition == null)
                {
                    this.ItemPosition = new Dictionary<int, List<int>>();
                }

                else
                {
                    this.ItemPosition.Clear();
                }

                if (this.DrillDownInfos != null && this.DrillDownInfos.InnerDrillInfo != null)
                {
                    this.DrillDownInfos.InnerDrillInfo.Clear();
                }

                if (this.ToggleGrops != null && this.ToggleGrops.Count > 0)
                {
                    foreach (var groups in this.ToggleGrops)
                    {
                        if (groups != null)
                        {
                            groups.Clear();                            
                        }
                    }
                }

                this.isHeader = new List<string>();

                this.Engine.GroupCollection = this.engineGroupCollection;
                this.Engine.Model = this;
                this.Engine.PopulateTable();

                if (this.Engine.IsEmptyDataSource && !String.IsNullOrEmpty(this.NoRowsMessage))
                {
                    this.RowCount = 1;
                    this.ColumnCount = 1;
                    this.Data = new Internal.TablixRows();
                    this.Data.Add(new Internal.TablixRow());
                    this.Data[0].Add(this.NoRowCellInfo);

                    this.RowHeights = new List<double>();
                    this.ColumnWights = new List<double>();

                    this.RowHeights.Add(this.Height);
                    this.ColumnWights.Add(this.Width);
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.DocumentMap))
                    {
                        this.DocumentMapLable = this.Model.ExpressionEngine.GetEvalExpressionString(this.DocumentMap);
                        this.SetTreeModel();
                        isMapHeader = true;
                    }

                    this.PopulateTable(isMapHeader);

                    this.Model.isContainsPageBreak = this.GroupBreakLocations != null && this.GroupBreakLocations.Count > 0;

                    if (!string.IsNullOrEmpty(this.HiddenExpKey))
                    {
                        try
                        {
                            this.Hidden = bool.Parse(this.Model.ExpressionEngine.GetEvalExpressionString(this.HiddenExpKey));
                        }
                        catch { }
                    }
                    else
                    {
                        this.Hidden = false;
                    }

                    if (!string.IsNullOrEmpty(this.ToggleItem))
                    {
                        this.GetTextBoxModel(this.ToggleItem);
                    }

                    if (isHeader != null)
                    {
                        isHeader.Clear();
                        isHeader = null;
                    }

                    if (DrillSpanRange != null)
                    {
                        DrillSpanRange.Clear();
                    }

                    foreach (var info in this.Engine.GroupCollection)
                    {
                        info.GroupValueOrderKeys.Clear();
                        info.GroupValuesIndexes.Clear();
                        info.GroupSortOrderKey.Clear();
                        info.GroupRecursiveParentOrderKey.Clear();

                        foreach (var values in info.GroupValues)
                        {
                            values.DataSources = null;

                            if (values.GroupKey != null)
                            {
                                if (values.GroupKey.Keys != null)
                                {
                                    values.GroupKey.Keys.Clear();
                                }

                                if (values.GroupKey.Values != null)
                                {
                                    values.GroupKey.Values.Clear();
                                }

                                values.GroupKey.Comparers = null;
                            }

                            values.GroupKey = null;

                            if (values.ParentGroupKey != null)
                            {
                                if (values.ParentGroupKey.Keys != null)
                                {
                                    values.ParentGroupKey.Keys.Clear();
                                }

                                if (values.ParentGroupKey.Values != null)
                                {
                                    values.ParentGroupKey.Values.Clear();
                                }

                                values.ParentGroupKey.Comparers = null;
                            }

                            values.ParentGroupKey = null;

                            if (values.FieldSortOrderKey != null)
                            {
                                values.FieldSortOrderKey.Clear();
                            }

                            if (values.GroupFieldKeys != null)
                            {
                                values.GroupFieldKeys.Clear();
                            }

                            if (values.Values != null)
                            {
                                values.Values.Clear();
                            }

                            values.ToggleItem = null;
                            values.IsHidden = null;

                            values.FieldSortOrderKey = null;
                        }

                        info.GroupValues.Clear();
                    }
                }

                this.Engine.DisposeEngine();

                base.Evaluate();
            }
            catch (Exception e)
            {
                this.Model.ExceptionDetails.Add("Getting following exception while evaluate the expression in " + this.ReportItem.Name + " " + e.Message);
            }
        }

        private void SetTreeModel()
        {
            if (this.Model.MapModel == null)
            {
                this.Model.MapModel = new DocumentMapModel();
            }
            DocumentData node = new DocumentData();
            node.DocumentLable = this.DocumentMapLable;
            node.ModelType=ModelType.TablixModel;
            node.ReportItemName = this.Name;
            node.IsTablixChild = this.IsTablixChild;
            this.DocumentNodeRefer = node;
            this.Model.MapModel.NodeData.Add(node);
        }

        public override void UpdateSize()
        {
            for (int i = 0; i < this.RowCount; i++)
            { 
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    var cellInfo = this.Data[i][j];

                    if (cellInfo != null && cellInfo.ItemModel!=null && ((cellInfo.ItemModel.CanGrow && 
                        cellInfo.ItemModel.ModelType == Internal.ModelType.TextBoxModel) || cellInfo.ItemModel.ModelType == Internal.ModelType.TablixModel || cellInfo.ItemModel.ModelType==ModelType.RectangleModel ))
                    {
                        if (this.Model.EnableVirtualEvaluation)
                        {
                            cellInfo.CurrentKey = new List<int>();
                            cellInfo.CurrentKey.Add(i);
                            cellInfo.CurrentKey.Add(j);

                            if (cellInfo.ItemModel.ModelType != Internal.ModelType.TablixModel)
                            {
                                cellInfo.Evaluate();
                            }
                        }

                        if (cellInfo.ItemModel.ModelType == Internal.ModelType.TextBoxModel)
                        {
                            if (this.Engine != null && this.Engine.IsEmptyDataSource)
                            {
                                cellInfo.Evaluate();
                            }

                            cellInfo.ItemModel.UpdateSize();
                            double cellHeight = (cellInfo.ItemModel as TextboxModel).ActualHeight;
                            cellHeight = cellHeight >= cellInfo.Height && cellHeight >= this.RowHeights[i] ? cellHeight : this.RowHeights[i];
                            this.RowHeights[i] = cellHeight;
                        }
                        else if (cellInfo.ItemModel.ModelType == Internal.ModelType.TablixModel)
                        {
                            var model = (cellInfo.ItemModel as TablixModel);
                            if (this.Model.EnableVirtualEvaluation)
                            {
                                TablixEvaluationItems items = cellInfo.Rows[i].TablixValues[j][model.Name];
                                this.UpdateTablixValue(model, items);
                            }
                            cellInfo.ItemModel.UpdateSize();
                            double rowHeight = 0, colWidth = 0;
                            if (cellInfo.RowSpan == 0)
                            {
                                for (int pos = 0; pos < model.RowCount; pos++)
                                {
                                    rowHeight = rowHeight + model.RowHeights[pos];
                                }
                                rowHeight = rowHeight >= cellInfo.Height && rowHeight >= this.RowHeights[i]
                                                ? rowHeight
                                                : this.RowHeights[i];
                                this.RowHeights[i] = rowHeight;
                            }
                            if (cellInfo.ColumnSpan == 0)
                            {
                                for (int pos = 0; pos < model.ColumnCount; pos++)
                                {
                                    colWidth = colWidth + model.ColumnWights[pos];
                                }
                                colWidth = colWidth >= cellInfo.Width && colWidth >= this.ColumnWights[j]
                                               ? colWidth
                                               : this.ColumnWights[j];
                                this.ColumnWights[j] = colWidth;
                            }
                        }
                        else if (cellInfo.ItemModel.ModelType == ModelType.RectangleModel)
                        {
                            if (this.Model.EnableVirtualEvaluation)
                            {
                                cellInfo.Evaluate();
                            }
                            cellInfo.ItemModel.UpdateSize();
                            double cellHeight = (cellInfo.ItemModel as RectangleModel).Height;
                            cellHeight = cellHeight >= cellInfo.Height && cellHeight >= this.RowHeights[i] ? cellHeight : this.RowHeights[i];
                            this.RowHeights[i] = cellHeight;
                        }
                        if (cellInfo != null && cellInfo.ItemModel != null)
                        {
                            if (cellInfo.ItemModel.ModelType != ModelType.TablixModel && this.Model.EnableVirtualEvaluation)
                            {
                                if (cellInfo.ItemModel.ModelType != Internal.ModelType.TablixModel)
                                {
                                    cellInfo.DisposeEvalObjects();
                                }
                            }
                        }
                    }
                }
            }

            //cellHeight = cellHeight >= cell.Height && cellHeight >= this.RowHeights[info.HeaderLevel - 1] ? cellHeight : this.RowHeights[info.HeaderLevel - 1];
            //this.RowHeights[info.HeaderLevel - 1] = cellHeight;
            //base.UpdateSize();
        }

        public override void UpdateHeight(double firstPageHeight, LayoutReportItemModel locationInfo, double preferredHeight, ReportItemViewMode itemViewMode)
        {
            if (this.ColumnCount == 0 || this.RowCount == 0 || this.Hidden)
            {
                locationInfo.ActualHeight = 0;
                return;
            }

            var rowValue = 1;
            double actualHeight = 0;
            double abortHeight = preferredHeight;
            var hasPageHeight = !double.IsNaN(firstPageHeight);
            int pageCount = 0;
            var pageHeight = 0.0;
            var rowCounter = 0;
            var pageTop = 0.0;

            var pageSizes = new Dictionary<int, TablixPageInfo>();
            List<SizeWH> rowHeights = new List<SizeWH>();

            List<ColumnInfo> pageColumnSizes = itemViewMode == ReportItemViewMode.Print ? this.PrintPageColumnInfo : this.PageColumnInfo;
            List<int> rowIndices = new List<int>();

            if (itemViewMode == ReportItemViewMode.None)
            {
                bool isFirstPageSet = false;
                bool rowReachedEnd = false;
                bool isAddRepeatHeader = true;

                for (int i = 0; i < this.RowCount; i++)
                {
                    int lineIndex = i + 1;
                    double size = this.RowHeights[i];
                    rowReachedEnd = lineIndex == this.RowCount;

                    var itemIndex = this.ItemPosition.Where(t => (t.Key == i));
                    if (itemIndex.Count() > 0)
                    {
                        foreach (var colPair in itemIndex.First().Value)
                        {
                            var j = colPair;
                            var data = this.Data[i][j];
                            if (data != null && data.ItemModel != null)
                            {
                                if (this.Model.IsToggleState && data.ItemModel.PageInfo != null)
                                {
                                    data.ItemModel.PageInfo.BelongsTo = null;
                                }

                                if (this.Model.IsToggleState && data.ItemModel.PrintPageInfo != null)
                                {
                                    data.ItemModel.PrintPageInfo.BelongsTo = null;
                                }

                                if (this.Model.EnableVirtualEvaluation && data.IsDetailCell)
                                {
                                    data.CurrentKey = new List<int>();
                                    data.CurrentKey.Add(i);
                                    data.CurrentKey.Add(j);
                                }
                                if (data.ItemModel.ModelType == ModelType.TablixModel)
                                {
                                    if (data.ItemModel.FlowLayoutInfo == null)
                                    {
                                        data.ItemModel.FlowLayoutInfo = new LayoutReportItemModel();
                                    }
                                    var model = (data.ItemModel as TablixModel);
                                    if (this.Model.EnableVirtualEvaluation)
                                    {
                                        TablixEvaluationItems items = data.Rows[i].TablixValues[j][model.Name];
                                        data.Rows[i].TablixValues[j][model.Name].FlowLayoutInfo =
                                            data.ItemModel.FlowLayoutInfo;
                                        this.UpdateTablixValue(model, items);
                                        data.ItemModel.UpdateHeight(firstPageHeight, data.ItemModel.FlowLayoutInfo,
                                                                    preferredHeight, itemViewMode);
                                        data.Rows[i].TablixValues[j][model.Name].FlowLayoutInfo =
                                            data.ItemModel.FlowLayoutInfo;
                                        data.Rows[i].TablixValues[j][model.Name].FlowPageSize =
                                            new Dictionary<int, TablixPageInfo>(model.FlowPageSizes);
                                    }
                                    else
                                    {
                                        data.ItemModel.UpdateHeight(firstPageHeight, data.ItemModel.FlowLayoutInfo,
                                                                    preferredHeight, itemViewMode);
                                    }
                                }
                                else if (data.ItemModel.ModelType == ModelType.RectangleModel ||
                                         data.ItemModel.ModelType == ModelType.SubReportModel)
                                {
                                    if (data.ItemModel.FlowLayoutInfo == null)
                                    {
                                        data.ItemModel.FlowLayoutInfo = new LayoutReportItemModel();
                                    }
                                    data.ItemModel.UpdateHeight(firstPageHeight, data.ItemModel.FlowLayoutInfo,
                                                                preferredHeight, itemViewMode);
                                }
                            }
                        }
                    }

                    if (isAddRepeatHeader && this.RepeatHeaderIndexes.Count > 0)
                    {
                        pageHeight = AddRepeatedRows(rowIndices, pageHeight, lineIndex, isFirstPageSet, rowReachedEnd, false);
                        isAddRepeatHeader = false;
                    }

                    if (this.GroupBreakLocations.ContainsKey(lineIndex))
                    {
                        pageHeight += size;
                        rowIndices.Add(lineIndex);

                        if (this.RepeatHeaderIndexes.Count > 0)
                        {
                            pageHeight = AddRepeatedRows(rowIndices, pageHeight, lineIndex, isFirstPageSet, rowReachedEnd, true);
                        }

                        pageSizes.Add(pageCount++, new TablixPageInfo() { Height = pageHeight + 1, LineCount = rowIndices.Count, RowIndices = rowIndices.ToArray() });

                        // reset
                        pageHeight = 0;
                        isFirstPageSet = true;
                        isAddRepeatHeader = true;
                        rowIndices = new List<int>();
                    }
                    else
                    {
                        rowIndices.Add(lineIndex);
                    }

                    pageHeight += size;
                    actualHeight += size;
                }
                if (rowIndices.Count > 0)
                {
                    pageSizes.Add(pageCount++, new TablixPageInfo() { Height = pageHeight + 1, LineCount = rowIndices.Count, RowIndices = rowIndices.ToArray() });
                }
            }
            else if (itemViewMode != ReportItemViewMode.Print && (!hasPageHeight || (this.KeepTogether && !(this.GroupBreakLocations.Count > 0))))
            {
                for (int i = 0; i < this.RowCount; i++)
                {
                    if (!this.IsVisibilityCheck(this.DrillDownInfos.InnerDrillInfo, i+1))
                    {
                        rowIndices.Add(i + 1);
                        actualHeight += this.RowHeights[i];

                        var itemIndex = this.ItemPosition.Where(t => (t.Key == i));
                        if (itemIndex.Count() > 0)
                        {
                            foreach (var colIndex in itemIndex.First().Value)
                            {
                                var j = colIndex;
                                var data = this.Data[i][j];
                                if (data != null && data.ItemModel != null)
                                {
                                    if (this.Model.IsToggleState && data.ItemModel.PageInfo != null)
                                    {
                                        data.ItemModel.PageInfo.BelongsTo = null;
                                    }

                                    if (this.Model.IsToggleState && data.ItemModel.PrintPageInfo != null)
                                    {
                                        data.ItemModel.PrintPageInfo.BelongsTo = null;
                                    }

                                    if (this.Model.EnableVirtualEvaluation && data.IsDetailCell)
                                    {
                                        data.CurrentKey = new List<int>();
                                        data.CurrentKey.Add(i);
                                        data.CurrentKey.Add(j);
                                    }
                                    if (data.ItemModel.ModelType == ModelType.TablixModel)
                                    {
                                        if (data.ItemModel.PageInfo == null)
                                        {
                                            data.ItemModel.PageInfo = new LayoutReportItemModel();
                                        }
                                        var model = (data.ItemModel as TablixModel);
                                        if (this.Model.EnableVirtualEvaluation)
                                        {
                                            TablixEvaluationItems items = data.Rows[i].TablixValues[j][model.Name];
                                            data.Rows[i].TablixValues[j][model.Name].PageLayoutInfo =
                                                data.ItemModel.PageInfo;
                                            this.UpdateTablixValue(model, items);
                                            data.ItemModel.UpdateHeight(firstPageHeight, data.ItemModel.PageInfo,
                                                                        preferredHeight, itemViewMode);
                                            data.Rows[i].TablixValues[j][model.Name].PageLayoutInfo =
                                                data.ItemModel.PageInfo;
                                            data.Rows[i].TablixValues[j][model.Name].PageSize =
                                                new Dictionary<int, TablixPageInfo>(model.PageSizes);
                                        }
                                        else
                                        {
                                            data.ItemModel.UpdateHeight(firstPageHeight, data.ItemModel.PageInfo,
                                                                        preferredHeight, itemViewMode);
                                        }
                                    }
                                    else if (data.ItemModel.ModelType == ModelType.RectangleModel ||
                                             data.ItemModel.ModelType == ModelType.SubReportModel)
                                    {
                                        if (data.ItemModel.PageInfo == null)
                                        {
                                            data.ItemModel.PageInfo = new LayoutReportItemModel();
                                        }
                                        data.ItemModel.UpdateHeight(firstPageHeight, data.ItemModel.PageInfo,
                                                                    preferredHeight, itemViewMode);
                                    }
                                }
                            }
                        }
                    }
                }

                if (pageColumnSizes != null)
                {
                    foreach (var column in pageColumnSizes)
                    {
                        pageSizes.Add(pageCount++, new TablixPageInfo() { Height = actualHeight + 1, LineCount = rowIndices.Count, RowIndices = rowIndices.ToArray(), ColumnIndices = column.ColumnIndices, Width = column.Width, ColumnWiths = column.ColumnWiths });
                    }
                }
            }
            else
            {
                var rowReachedEnd = false;
                var isFirstPageSet = false;
                bool isAddRepeatHeader = true;
                bool canSplitPage = true;
                bool isAppendRow = false;
                bool isGroupKeepTogether = false;
                int extendRowPos = 0;
                rowHeights = null;

                if (this.IsTablixChild)
                {
                    pageHeight = this.ParentPageTop;
                }

                if (itemViewMode == ReportItemViewMode.Normal && this.GroupBreakLocations.Count != 0)
                {
                    if (this.GroupBreakLocations.Values.Where(value => value == true).Count() > 0)
                        canSplitPage = false;
                }
                if (itemViewMode == ReportItemViewMode.Normal && this.GroupRanges != null && this.GroupRanges.Count > 0)
                {
                    if (this.GroupRanges.Keys.Where(value => value.KeepTogether == true).Count() > 0)
                        isGroupKeepTogether = true;
                }
                while (!rowReachedEnd)
                {
                    double size = this.RowHeights[rowValue - 1];
                    int lineIndex = rowValue;
                    rowReachedEnd = rowValue == this.RowCount;
                    bool isHidden = false;
                    
                    if (this.DrillDownInfos != null)
                    {
                       isHidden = this.IsVisibilityCheck(this.DrillDownInfos.InnerDrillInfo, rowValue);
                    }

                    if (!isHidden)
                    {
                        var splitIndics = false;
                        if (rowHeights == null)
                        {
                            rowHeights = new List<SizeWH>();
                        }
                        pageTop = pageHeight;

                        if (isAddRepeatHeader && this.RepeatHeaderIndexes.Count > 0)
                        {
                            pageHeight = UpdateRepeatedRows(rowIndices, rowValue, pageHeight, isFirstPageSet ? abortHeight : firstPageHeight, isFirstPageSet, rowReachedEnd, false);
                            isAddRepeatHeader = false;
                        }

                        if (this.GroupBreakLocations.ContainsKey(rowValue))
                        {
                            pageHeight += size;
                            actualHeight += pageHeight;
                            rowIndices.Add(lineIndex);

                            if (this.RepeatHeaderIndexes.Count > 0)
                                pageHeight = UpdateRepeatedRows(rowIndices, rowValue, pageHeight, isFirstPageSet ? abortHeight : firstPageHeight, isFirstPageSet, rowReachedEnd, true);

                            foreach (var column in pageColumnSizes)
                            {
                                pageSizes.Add(pageCount++, new TablixPageInfo() { Height = pageHeight + 1, LineCount = rowCounter, RowIndices = rowIndices.ToArray(), ColumnIndices = column.ColumnIndices, Width = column.Width });
                            }

                            // reset
                            rowCounter = 0;
                            pageHeight = 0.0;
                            rowIndices = new List<int>();
                            isFirstPageSet = true;
                            isAddRepeatHeader = true;
                        }
                        else
                        {
                            if (isGroupKeepTogether &&
                            ((!isFirstPageSet && pageHeight + size > firstPageHeight) || (isFirstPageSet && pageHeight + size >= abortHeight)) && !isAppendRow)
                            {
                                isAppendRow = true;
                                extendRowPos = this.IsRowKeepToGroup(rowValue);
                            }

                            isAppendRow = extendRowPos != 0 && (rowValue > extendRowPos || rowReachedEnd) ? false : isAppendRow;

                            if (!isFirstPageSet && pageHeight + size > firstPageHeight && canSplitPage && !isAppendRow && !rowReachedEnd)
                            {
                                actualHeight += firstPageHeight;
                                if (this.RepeatHeaderIndexes.Count > 0)
                                    pageHeight = UpdateRepeatedRows(rowIndices, rowValue, pageHeight, pageHeight, isFirstPageSet, rowReachedEnd, true);

                                if (size > abortHeight || rowIndices.Count == 0)
                                {
                                    if (size > firstPageHeight || size > abortHeight)
                                    {
                                        if (pageHeight < firstPageHeight)
                                        {
                                            size = firstPageHeight - pageHeight;
                                        }
                                        else
                                        {
                                            size = pageHeight - firstPageHeight;
                                        }
                                        splitIndics = true;
                                        pageTop = this.RowHeights[rowValue - 1] - size;
                                    }
                                    rowIndices.Add(rowValue);
                                    SizeWH sizeWh = new SizeWH();
                                    sizeWh.Index = rowValue;
                                    sizeWh.Size = size;
                                    rowHeights = new List<SizeWH>();
                                    rowHeights.Add(sizeWh);
                                }
                                else
                                {
                                    pageTop = 0.0;
                                }

                                foreach (var column in pageColumnSizes)
                                {
                                    pageSizes.Add(pageCount++, new TablixPageInfo() { Height = pageHeight == 0.0 ? firstPageHeight + 1 : pageHeight + 1, LineCount = rowCounter, RowIndices = rowIndices.ToArray(), ColumnIndices = column.ColumnIndices, Width = column.Width, ColumnWiths = column.ColumnWiths, RowHeights = new List<SizeWH>(rowHeights) });
                                }

                                if ((pageHeight + this.RowHeights[rowValue - 1]) / firstPageHeight < 1.05 && pageHeight == 0.0)
                                {
                                    if (rowValue == this.RowCount - 1)
                                    {
                                        rowReachedEnd = true;
                                    }
                                    else
                                    {
                                        UpdateHeightofInnerItems(lineIndex, itemViewMode, splitIndics, pageTop, firstPageHeight, preferredHeight);
                                        rowValue++;
                                    }
                                    size = this.RowHeights[rowValue - 1];
                                    lineIndex = rowValue;
                                    splitIndics = false;
                                }
                                else
                                {
                                    if (size != this.RowHeights[rowValue - 1])
                                    {
                                        size = this.RowHeights[rowValue - 1] - size;
                                    }
                                }

                                //if (size != this.RowHeights[rowValue - 1])
                                //{
                                //    size = this.RowHeights[rowValue - 1] - size;
                                //}


                                // reset
                                pageHeight = 0.0;
                                rowCounter = 0;
                                rowIndices = new List<int>();
                                isAddRepeatHeader = true;
                                isFirstPageSet = true;
                            }
                            if (((isFirstPageSet && pageHeight + size >= abortHeight && canSplitPage) || rowReachedEnd) && !isAppendRow)
                            {
                                if (rowReachedEnd)
                                {
                                    //height is more than 50% move to next page 
                                    if (((pageHeight + size) / abortHeight > 1.5) || pageHeight + size >= abortHeight)
                                    {
                                        foreach (var column in pageColumnSizes)
                                        {
                                            pageSizes.Add(pageCount++, new TablixPageInfo() { Height = pageHeight + 1, LineCount = rowCounter, RowIndices = rowIndices.ToArray(), ColumnIndices = column.ColumnIndices, Width = column.Width, ColumnWiths = column.ColumnWiths });
                                        }

                                        actualHeight += pageHeight;
                                        pageHeight = 0.0;
                                        rowCounter = 0;
                                        rowIndices = new List<int>();
                                    }
                                    pageHeight += size;
                                    actualHeight += pageHeight;
                                    rowIndices.Add(lineIndex);
                                }
                                else
                                {
                                    actualHeight += abortHeight;
                                }

                                if (this.RepeatHeaderIndexes.Count > 0)
                                    pageHeight = UpdateRepeatedRows(rowIndices, rowValue, pageHeight, pageHeight, isFirstPageSet, rowReachedEnd, true);

                                foreach (var column in pageColumnSizes)
                                {
                                    pageSizes.Add(pageCount++, new TablixPageInfo() { Height = pageHeight + 1, LineCount = rowCounter, RowIndices = rowIndices.ToArray(), ColumnIndices = column.ColumnIndices, Width = column.Width, ColumnWiths = column.ColumnWiths });
                                }

                                pageHeight = 0.0;
                                rowCounter = 1;
                                rowIndices = new List<int>();
                                isAddRepeatHeader = true;
                            }

                            // sbValue--;
                            rowIndices.Add(lineIndex);
                            pageHeight += size;
                        }

                        rowValue += 1;
                        rowCounter += 1;
                        UpdateHeightofInnerItems(lineIndex, itemViewMode, splitIndics, pageTop, firstPageHeight, preferredHeight);
                    }
                    else if (isHidden && rowReachedEnd)
                    {
                        foreach (var column in pageColumnSizes)
                        {
                            pageSizes.Add(pageCount++, new TablixPageInfo()
                                          {
                                              Height = pageHeight + 1,
                                              LineCount = rowCounter,
                                              RowIndices = rowIndices.ToArray(),
                                              ColumnIndices = column.ColumnIndices,
                                              Width = column.Width
                                          });
                        }
                    }
                    else
                    {
                        rowValue++;
                    }
                }
            }

            if (itemViewMode == ReportItemViewMode.Print)
            {
                this.PrintPageColumnCount = pageColumnSizes.Count;
                this.PrintPageSizes = pageSizes;
            }
            else if (itemViewMode == ReportItemViewMode.Normal)
            {
                this.PageSizes = pageSizes;
            }
            else
            {
                this.FlowPageSizes = pageSizes;
            }

            locationInfo.TotalPages = pageSizes.Count;
            locationInfo.ActualHeight = actualHeight;
            if (this.IsTablixChild)
            {
                locationInfo.ActualTop = this.Top;
            }
        }

        Dictionary<int, RepeatHeaderInfo> appendIndex = new Dictionary<int, RepeatHeaderInfo>();

        private double UpdateRepeatedRows(List<int> rowIndices, int rowValue, double pageHeight, double preferedHeight, bool isFirstPageSet, bool rowReachedEnd, bool addRow)
        {
            try
            {
                if (this.RowHeights.Sum() >= preferedHeight)
                {
                    double tempHeight = 0;
                    int maxRowIndex = rowValue;
#if SILVERLIGHT
                    var index = (from pair in this.RepeatHeaderIndexes where pair.Value.GroupName != this.tablixItem.Name select pair).FirstOrDefault();
                    int groupStartIndex = this.RepeatHeaderIndexes.ToList().IndexOf(index);
#else
                    int groupStartIndex = this.RepeatHeaderIndexes.ToList().FindIndex(pair => pair.Value.GroupName != this.tablixItem.Name);
#endif
                    if (appendIndex.Count == 0)
                    {
                        foreach (var pair in this.RepeatHeaderIndexes)
                        {
                            if (pair.Value.GroupName == this.tablixItem.Name)
                            {
                                if ((pair.Key <= this.columnHeaderCount || pair.Key <= groupStartIndex) && pair.Value.KeepWithGroup == KeepWithGroup.After)
                                {
                                    appendIndex.Add(pair.Key,pair.Value);
                                }
                                else
                                {
                                    appendIndex.Add(pair.Key, pair.Value);
                                }
                            }
                        }
                    }
                    
                    int rowindexKey = rowIndices.FirstOrDefault();
                    if (isFirstPageSet && rowindexKey != 0 && !addRow)
                    {
                        rowIndices.Remove(rowindexKey);
                    }
                    if (!addRow)
                    {
                        int m_rowVal= (rowValue-1);
                        for (int i = 0; i < (this.RowCount - m_rowVal); i++)
                        {
                            if (tempHeight + this.RowHeights[i + m_rowVal] > preferedHeight)
                                break;

                            tempHeight += this.RowHeights[i + m_rowVal];
                            maxRowIndex = i + rowValue;
                        }

                        int nextGroupIndex = (from pair in this.RepeatHeaderIndexes where pair.Key > rowValue && pair.Value.IsNextGroup == true select pair.Key).FirstOrDefault();

                        foreach (var pos in appendIndex)
                        {
                            if ((pos.Key <= this.columnHeaderCount || pos.Key <= groupStartIndex) && pos.Value.KeepWithGroup == KeepWithGroup.After)
                            {
                                if (isFirstPageSet && !addRow)
                                {
                                    rowIndices.Add(pos.Key + 1);
                                    pageHeight += this.RowHeights[pos.Key];
                                }
                            }
                            else
                            {
                                pageHeight += this.RowHeights[pos.Key];
                            }
                        }
                    }
                    //add Rows and Heights
                    if (this.GroupNameHeirachy.Count > 0)
                    {
                        string containerName = this.GroupRanges.Where(group => group.Key.EndIndex >= maxRowIndex && group.Key.StartIndex < maxRowIndex).FirstOrDefault().Value;
                        bool reachParent = false;
                        while (!reachParent)
                        {
                            if (!string.IsNullOrEmpty(containerName))
                            {
                                int previndex = 0;
                                
                                if (isFirstPageSet && !addRow)
                                {
                                    var beforeRange = this.GroupRanges.Where(group => group.Value == containerName && group.Key.EndIndex >= rowValue && group.Key.StartIndex < rowValue).FirstOrDefault();
                                    if (beforeRange.Key != null)
                                    {
                                        var beforeGroup = this.RepeatHeaderIndexes.Where(pair => pair.Value.GroupName == containerName && pair.Key >= beforeRange.Key.StartIndex && pair.Key < rowValue && pair.Key + 1 != rowindexKey);

                                        foreach (var group in beforeGroup)
                                        {
                                            if (group.Value != null && (previndex == 0 || (previndex != 0 && previndex + 1 == group.Key)) && group.Value.KeepWithGroup == KeepWithGroup.After)
                                            {
                                                rowIndices.Add(group.Key + 1);
                                                previndex = group.Key;
                                                pageHeight += this.RowHeights[group.Key];
                                            }
                                        }
                                        previndex = 0;
                                    }
                                }

                                var groupRange = this.GroupRanges.Where(group => group.Value == containerName && group.Key.EndIndex > maxRowIndex && group.Key.StartIndex < maxRowIndex).FirstOrDefault();
                                if (groupRange.Key != null)
                                {
                                    var afterGroup = this.RepeatHeaderIndexes.Where(pair => pair.Value.GroupName == containerName && pair.Key > groupRange.Key.StartIndex && pair.Key < groupRange.Key.EndIndex );

                                    foreach (var group in afterGroup)
                                    {
                                        if (group.Value != null && (previndex == 0 || (previndex != 0 && previndex + 1 == group.Key)) && group.Value.KeepWithGroup == KeepWithGroup.Before)
                                        {
                                            previndex = group.Key;

                                            if (addRow && !rowIndices.Contains(group.Key + 1) && maxRowIndex != groupRange.Key.StartIndex + 1)
                                            {
                                                rowIndices.Add(group.Key + 1);
                                            }
                                            else
                                            {
                                                pageHeight += this.RowHeights[group.Key];
                                            }
                                        }
                                    }
                                }
                            }

                            int prev = this.GroupNameHeirachy.IndexOf(containerName);
                            if (prev > 1 && this.GroupNameHeirachy[prev - 1] != this.tablixItem.Name)
                            {
                                containerName = this.GroupNameHeirachy[prev - 1];
                            }
                            else
                            {
                                reachParent = true;
                            }
                        }
                    }
                    if (isFirstPageSet && rowindexKey != 0 && !addRow)
                    {
                        rowIndices.Add(rowindexKey);
                    }
                    if (appendIndex.Count > 0 && addRow)
                    {
                        foreach (var row in appendIndex)
                        {
                            if (row.Value.KeepWithGroup == KeepWithGroup.Before && row.Key + 1 > rowIndices.Last())
                                rowIndices.Add(row.Key + 1);
                        }
                    }
                    
                }
            }
            catch { }

            return pageHeight;
        }

        private int IsRowKeepToGroup(int rowValue)
        {
            if (this.GroupRanges != null && this.GroupRanges.Count > 0)
            {
                string parentGroup = this.GroupNameHeirachy.FirstOrDefault();
                foreach (string name in this.GroupNameHeirachy)
                {
                    if (this.GroupRanges.ContainsValue(name))
                    {
                        parentGroup = name;
                        break;
                    }
                }
                var row = (from range in this.GroupRanges orderby range.Key.EndIndex where (range.Value.Equals(parentGroup) && range.Key.StartIndex < rowValue && range.Key.EndIndex > rowValue && range.Key.KeepTogether == true) select range.Key.EndIndex).FirstOrDefault();
                return row;
            }
            return 0;
        }

        void UpdateHeightofInnerItems(int lineIndex, ReportItemViewMode itemViewMode, bool splitIndics, double pageTop, double firstPageHeight, double preferredHeight)
        {
            var itemIndex = this.ItemPosition.Where(t => (t.Key == (lineIndex - 1)));
            if (itemIndex.Count() > 0)
            {
                foreach (var colPair in itemIndex.First().Value)
                {
                    var j = colPair;
                    var data = this.Data[lineIndex - 1][j];
                    if (data != null && data.ItemModel != null)
                    {
                        if (this.Model.IsToggleState && data.ItemModel.PageInfo != null)
                        {
                            data.ItemModel.PageInfo.BelongsTo = null;
                        }

                        if (this.Model.IsToggleState && data.ItemModel.PrintPageInfo != null)
                        {
                            data.ItemModel.PrintPageInfo.BelongsTo = null;
                        }

                        if (this.Model.EnableVirtualEvaluation)
                        {
                            data.CurrentKey = new List<int>();
                            data.CurrentKey.Add(j);
                            data.CurrentKey.Add(lineIndex - 1);
                        }
                        if (data.ItemModel.ModelType == ModelType.TablixModel || data.ItemModel.ModelType == ModelType.RectangleModel)
                        {
                            if (itemViewMode == ReportItemViewMode.Normal)
                            {
                                if (data.ItemModel.PageInfo == null)
                                {
                                    data.ItemModel.PageInfo = new LayoutReportItemModel();
                                }

                            }
                            else
                            {
                                if (data.ItemModel.PrintPageInfo == null)
                                {
                                    data.ItemModel.PrintPageInfo = new LayoutReportItemModel();
                                }
                            }
                        }

                        if (data.ItemModel.ModelType == ModelType.TablixModel)
                        {
                            var model = (data.ItemModel as TablixModel);
                            if (this.Model.EnableVirtualEvaluation)
                            {
                                TablixEvaluationItems items = data.Rows[lineIndex - 1].TablixValues[j][model.Name];
                                if (itemViewMode == ReportItemViewMode.Normal)
                                {
                                    data.Rows[lineIndex - 1].TablixValues[j][model.Name].PageLayoutInfo = data.ItemModel.PageInfo;
                                }
                                else
                                {
                                    data.Rows[lineIndex - 1].TablixValues[j][model.Name].PrintPageLayoutInfo = data.ItemModel.PrintPageInfo;
                                }
                                this.UpdateTablixValue(model, items);
                                (data.ItemModel as TablixModel).ParentPageTop = splitIndics ? pageTop : 0;
                                if (itemViewMode == ReportItemViewMode.Normal)
                                {
                                    data.ItemModel.UpdateHeight(firstPageHeight, data.ItemModel.PageInfo, preferredHeight, itemViewMode);
                                    data.Rows[lineIndex - 1].TablixValues[j][model.Name].PageLayoutInfo = data.ItemModel.PageInfo;
                                    data.Rows[lineIndex - 1].TablixValues[j][model.Name].PageSize = new Dictionary<int, TablixPageInfo>(model.PageSizes);
                                }
                                else
                                {
                                    data.ItemModel.UpdateHeight(firstPageHeight, data.ItemModel.PrintPageInfo, preferredHeight, itemViewMode);
                                    data.Rows[lineIndex - 1].TablixValues[j][model.Name].PrintPageLayoutInfo = data.ItemModel.PrintPageInfo;
                                    data.Rows[lineIndex - 1].TablixValues[j][model.Name].PrintPageSize = new Dictionary<int, TablixPageInfo>(model.PrintPageSizes);
                                }
                            }
                            else
                            {
                                data.ItemModel.UpdateHeight(firstPageHeight, itemViewMode == ReportItemViewMode.Normal ? data.ItemModel.PageInfo : data.ItemModel.PrintPageInfo, preferredHeight, itemViewMode);
                            }
                        }
                        else if (data.ItemModel.ModelType == ModelType.RectangleModel)
                        {
                            data.ItemModel.UpdateHeight(firstPageHeight, itemViewMode == ReportItemViewMode.Normal ? data.ItemModel.PageInfo : data.ItemModel.PrintPageInfo, preferredHeight, itemViewMode);
                        }
                    }
                }
            }
        }

        public override void UpdateWidth(double firstPageWidth, LayoutReportItemModel locationInfo, double preferredWidth, ReportItemViewMode itemViewMode)
        {
            if (this.ColumnCount == 0 || this.RowCount == 0 || this.Hidden) 
            {
                locationInfo.ActualWidth = 0;
                return;
            }

            if (this.DrillSpanRange != null)
            {
                this.DrillSpanRange.Clear();
                this.DrillSpanRange = null;
            }

            double actualWidth = 0;
            var columnValue = 1;
            double abortWidth = preferredWidth;
            var hasWidthInfo = !double.IsNaN(firstPageWidth);
            var pageWidth = 0.0;
            var columnCounter = 0;
            var pageLeft = 0.0;
            List<ColumnInfo> columnsInfo = new List<ColumnInfo>();
            List<int> columnIndices = new List<int>();

            if (!hasWidthInfo)
            {
                for (int i = 0; i < this.ColumnCount; i++)
                {
                    columnIndices.Add(i + 1);
                    actualWidth += this.ColumnWights[i];
                    var itemIndex = ItemPosition.Where(t => t.Value.Contains(i));
                    foreach (var rowPair in itemIndex)
                    {
                        var j = rowPair.Key;
                        var data = this.Data[j][i];
                        if (data != null)
                        {
                            if (data.ItemModel.ModelType == ModelType.TablixModel ||
                                data.ItemModel.ModelType == ModelType.RectangleModel)
                            {
                                if (this.Model.EnableVirtualEvaluation)
                                {
                                    data.CurrentKey = new List<int>();
                                    data.CurrentKey.Add(j);
                                    data.CurrentKey.Add(i);
                                }
                                if (data.ItemModel.ModelType == ModelType.TablixModel)
                                {
                                    if (data.ItemModel.PageInfo == null)
                                    {
                                        data.ItemModel.PageInfo =
                                            data.ItemModel.FlowLayoutInfo = new LayoutReportItemModel();
                                    }
                                    var model = (data.ItemModel as TablixModel);
                                    if (this.Model.EnableVirtualEvaluation)
                                    {
                                        TablixEvaluationItems items = data.Rows[j].TablixValues[i][model.Name];
                                        data.Rows[j].TablixValues[i][model.Name].PageLayoutInfo = data.Rows[j].TablixValues[i][model.Name].FlowLayoutInfo = data.ItemModel.PageInfo;
                                        this.UpdateTablixValue(model, items);
                                        data.Rows[j].TablixValues[i][model.Name].PageLayoutInfo = data.Rows[j].TablixValues[i][model.Name].FlowLayoutInfo = data.ItemModel.PageInfo;
                                    }
                                    data.ItemModel.UpdateWidth(firstPageWidth, data.ItemModel.PageInfo, preferredWidth,
                                                               itemViewMode);
                                }
                                else if (data.ItemModel.ModelType == ModelType.RectangleModel ||
                                         data.ItemModel.ModelType == ModelType.SubReportModel)
                                {
                                    if (data.ItemModel.PageInfo == null)
                                    {
                                        data.ItemModel.PageInfo =
                                            data.ItemModel.FlowLayoutInfo = new LayoutReportItemModel();
                                    }
                                    data.ItemModel.UpdateWidth(firstPageWidth, data.ItemModel.PageInfo,
                                                               preferredWidth, itemViewMode);
                                }
                            }
                        }
                    }
                }

                columnsInfo.Add(new ColumnInfo() { Width = actualWidth, ColumnCount = this.ColumnCount, ColumnIndices = columnIndices.ToArray() });
            }
            else
            {
                var columnReachedEnd = false;
                var isFirstPageSet = false;
                var isSplitindics = false;
                List<SizeWH> columnWiths = null;

                if (this.IsTablixChild)
                {
                    pageWidth = this.ParentPageLeft;
                }

                while (!columnReachedEnd)
                {
                    isSplitindics = false;
                    int columnIndex = columnValue;

                    double size = this.ColumnWights[columnValue - 1];
                    columnReachedEnd = columnValue == this.ColumnCount;

                    if (columnWiths == null)
                    {
                        columnWiths = new List<SizeWH>();
                    }
                    pageLeft = pageWidth;
                    if (!isFirstPageSet && pageWidth + size > firstPageWidth)
                    {
                        actualWidth += firstPageWidth;
                        //Column Size greater then page with or no column in page split
                        if (size > abortWidth || columnIndices.Count == 0)
                        {
                            if (size > firstPageWidth || size > abortWidth)
                            {
                                if (pageWidth < firstPageWidth)
                                {
                                    size = firstPageWidth - pageWidth;
                                }
                                else
                                {
                                    size = pageWidth - firstPageWidth;
                                }
                                pageLeft = this.ColumnWights[columnValue - 1] - size;
                            }
                            else
                            {
                                isSplitindics = true;
                            }
                            columnIndices.Add(columnValue);
                            SizeWH sizeWh = new SizeWH();
                            sizeWh.Index = columnValue;
                            sizeWh.Size = size;
                            columnWiths.Add(sizeWh);
                        }
                        else
                        {
                            pageLeft = 0.0;
                        }

                        columnsInfo.Add(new ColumnInfo() { Width = pageWidth == 0.0 ? size : pageWidth, ColumnCount = columnCounter, ColumnIndices = columnIndices.ToArray(), ColumnWiths = new List<SizeWH>(columnWiths) });

                        if (size != this.ColumnWights[columnValue - 1])
                        {
                            size = this.ColumnWights[columnValue - 1] - size;
                        }
                        pageWidth = 0.0;
                        columnCounter = 0;
                        columnIndices = new List<int>();
                        columnWiths = new List<SizeWH>();
                        isFirstPageSet = true;
                    }
                    if ((isFirstPageSet && pageWidth + size > abortWidth) || columnReachedEnd)
                    {
                        if (columnReachedEnd)
                        {
                            pageWidth += size;
                            actualWidth += pageWidth;
                            columnIndices.Add(columnIndex);
                        }
                        else
                        {
                            actualWidth += abortWidth;
                        }

                        columnsInfo.Add(new ColumnInfo() { Width = pageWidth == 0.0 ? size : pageWidth, ColumnCount = columnCounter, ColumnIndices = columnIndices.ToArray(), ColumnWiths = new List<SizeWH>(columnWiths) });

                        pageWidth = 0.0;
                        columnCounter = 0;
                        columnIndices = new List<int>();
                        columnWiths = new List<SizeWH>();
                    }

                    // sbValue--;
                    columnValue += 1;
                    pageWidth += size;
                    columnCounter += 1;
                    if (!isSplitindics)
                        columnIndices.Add(columnIndex);

                    var itemIndex = ItemPosition.Where(t => t.Value.Contains(columnIndex - 1));
                    foreach (var rowPair in itemIndex)
                    {
                        var j = rowPair.Key;
                        var data = this.Data[j][columnIndex - 1];
                        if (data != null)
                        {
                            if (this.Model.EnableVirtualEvaluation && data.IsDetailCell)
                            {
                                data.CurrentKey = new List<int>();
                                data.CurrentKey.Add(j);
                                data.CurrentKey.Add(columnIndex - 1);
                            }
                            if (data.ItemModel.ModelType == ModelType.TablixModel)
                            {
                                if (data.ItemModel.PrintPageInfo == null)
                                {
                                    data.ItemModel.PrintPageInfo = new LayoutReportItemModel();
                                }
                                else
                                {
                                    data.ItemModel.PrintPageInfo.BelongsTo = null;
                                }
                                var model = (data.ItemModel as TablixModel);
                                if (this.Model.EnableVirtualEvaluation)
                                {
                                    TablixEvaluationItems items = data.Rows[j].TablixValues[columnIndex - 1][model.Name];
                                    data.Rows[j].TablixValues[columnIndex - 1][model.Name].PrintPageLayoutInfo = data.ItemModel.PrintPageInfo;
                                    this.UpdateTablixValue(model, items);
                                    (data.ItemModel as TablixModel).ParentPageLeft = pageLeft;
                                    data.Rows[j].TablixValues[columnIndex - 1][model.Name].PrintPageLayoutInfo = data.ItemModel.PrintPageInfo;
                                }
                                data.ItemModel.UpdateWidth(firstPageWidth, data.ItemModel.PrintPageInfo, preferredWidth, itemViewMode);
                            }
                            else if (data.ItemModel.ModelType == ModelType.RectangleModel)
                            {
                                if (data.ItemModel.PrintPageInfo == null)
                                {
                                    data.ItemModel.PrintPageInfo = new LayoutReportItemModel();
                                }
                                else
                                {
                                    data.ItemModel.PrintPageInfo.BelongsTo = null;
                                }
                                data.ItemModel.UpdateWidth(firstPageWidth, data.ItemModel.PrintPageInfo, preferredWidth, itemViewMode);
                            }
                        }
                    }
                }
            }

            if (itemViewMode == ReportItemViewMode.Normal)
            {
                this.PageColumnInfo = columnsInfo;
            }
            else if (itemViewMode == ReportItemViewMode.Print)
            {
                this.PrintPageColumnInfo = columnsInfo;
            }

            locationInfo.ActualWidth = actualWidth;
            if (this.IsTablixChild)
            {
                locationInfo.ActualLeft = this.Left;
            }
        }

        public override void UpdatePageNo(int pageNo)
        {
            if (this.Model.MapModel != null && this.Model.MapModel.NodeData.Count > 0)
            {
                if (this.DocumentNodeRefer != null)
                {
                    this.DocumentNodeRefer.PageNo = pageNo + 1;
                    this.DocumentNodeRefer.TopPos = this.Top;
                    this.DocumentNodeRefer.LeftPos = this.Left;
                }
#if WINRT
               var pageinfo = this.PrintPageSizes[this.PrintPageInfo.BelongsTo[pageNo]];                
#else
                var pageinfo = this.PageSizes[this.PageInfo.BelongsTo[pageNo]];
#endif
                if (pageinfo != null)
                {
                    double height = this.Top;
                    if (this.PageSizes[0] != pageinfo)
                    {
                        height = 0;
                    }
                    foreach (int t in pageinfo.RowIndices)
                    {
                        double width = this.Left;
                        foreach (int t1 in pageinfo.ColumnIndices)
                        {
                            var cell = this.Data[t - 1][t1 - 1];
                            if (cell != null && cell.IsDocumentMapRefer != null && cell.IsDocumentMapRefer.Count > 0)
                            {
                                IEnumerable<DocumentData> documentData =
                                    new List<DocumentData>(this.Model.EnableVirtualEvaluation
                                                               ? cell.IsDocumentMapRefer.Where(
                                                                   temp =>
                                                                   (temp.PosInfos != null &&
                                                                    temp.PosInfos.ColNo == t1 - 1 &&
                                                                    temp.PosInfos.RowNo == t - 1))
                                                               : cell.IsDocumentMapRefer);
                                if (documentData.Any())
                                {
                                    foreach (var refer in documentData)
                                    {
                                        refer.PageNo = pageNo + 1;
                                        refer.TopPos = height;
                                        refer.LeftPos = width;
                                        refer.ReferRowInfo = null;
                                        refer.PosInfos = null;
                                        cell.IsDocumentMapRefer.Remove(refer);
                                    }
                                    if (!this.Model.EnableVirtualEvaluation)
                                    {
                                        cell.IsDocumentMapRefer = null;
                                    }
                                }
                            }
                            width += this.ColumnWights[t1 - 1];
                        }
                        height += this.RowHeights[t - 1];
                    }
                }
                this.DocumentNodeRefer = null;
            }
        }

        #endregion

        internal bool IsVisibilityCheck(List<ToggleInfo> toggleInfos, int row)
        {
            foreach (var node in toggleInfos)
            {
                if (node.IsHidden)
                {
                    if (node.StartIndex <= row && node.EndIndex >= row)
                    {
                        if ((node.IsRow && node.StartIndex == row))
                        {
                            if (DrillSpanRange == null)
                            {
                                DrillSpanRange = new List<CoveredCellRange>();
                            }
                            if (node.HeaderLevel - 2 >= 0)
                            {
                                try
                                {
                                    var loc = from data in this.Data[row - 1] where data != null && data.IsDetailCell select this.Data[row - 1].IndexOf(data);
                                    this.DrillSpanRange.Add(new CoveredCellRange(row - 1, node.HeaderLevel - 2, row - 1, loc.First() - 1));
                                }
                                catch { }
                            }
                            return false;
                        }
                        else
                        {
                            return true;                            
                        }
                    }
                }
                if (node.Node != null && node.Node.InnerDrillInfo.Count > 0)
                {
                    if (IsVisibilityCheck(node.Node.InnerDrillInfo, row))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal void SetNoRowMessageInfo()
        {
            TextBox box = new TextBox();
            box.Name = "testing";
            box.Height = new Size();
            box.Height.PixelValue = this.Height;
            box.Width = new Size();
            box.Width.PixelValue = this.Width;
            box.Paragraphs = new Paragraphs();
            Paragraph para = new Paragraph();
            para.TextRuns = new TextRuns();
            TextRun run = new TextRun();
            run.Value = this.NoRowsMessage;
            para.TextRuns.Add(run);
            box.Paragraphs.Add(para);

            if (this.tablixItem.Style != null)
            {
                run.Style = this.tablixItem.Style;
            }

            TablixCellInfo info = new TablixCellInfo();
            info.ItemModel = this.Model.GetModel(box,true, null) as TextboxModel;
            info.ItemModel.Model.EnableVirtualEvaluation = false;
            info.Evaluate();
            this.NoRowCellInfo = info;
        }
    }
}