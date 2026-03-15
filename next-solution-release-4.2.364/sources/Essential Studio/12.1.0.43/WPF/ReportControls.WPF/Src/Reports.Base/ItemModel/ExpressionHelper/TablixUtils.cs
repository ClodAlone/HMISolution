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
using System.Xml.Linq;
using System.IO;
using System.Xml;
using Syncfusion.RDL.DOM;
using Syncfusion.RDL.ItemModel;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Layout;

namespace Syncfusion.RDL.Internal
{
    internal class TablixRow : List<TablixCellInfo>
    {
        internal List<List<FieldValue>> FieldValues { get; set; }

        internal List<object> DataSource { get; set; }

        internal List<Dictionary<string,TablixEvaluationItems>> TablixValues { get; set; }

        internal List<Dictionary<string, object>> RowNumberValues { get; set; }

        internal bool IsDrillDown { get; set; }

        internal int IsLastIndex { get; set; }

        public TablixRow()
        {
            this.FieldValues = new List<List<FieldValue>>();
            this.DataSource = new List<object>();
        }
    }

    internal class TablixEvaluationItems
    {
        internal TablixRows Rows { get; set; }

        internal int RowCount { get; set; }

        internal int ColumnCount { get; set; }

        internal List<double> RowHeights { get; set; }

        internal List<double> ColumnWidths { get; set; }

        internal Dictionary<string, TablixRows> CellRows { get; set; }

        internal Dictionary<int, bool> GroupBreakLocations { get; set; }

        internal Dictionary<int, RepeatHeaderInfo> RepeatHeaderIndexes { get; set; }

        internal List<int> GroupEndPositions { get; set; }

        internal LayoutReportItemModel FlowLayoutInfo { get; set; }

        internal LayoutReportItemModel PageLayoutInfo { get; set; }

        internal LayoutReportItemModel PrintPageLayoutInfo { get; set; }

        internal Dictionary<int, TablixPageInfo> PageSize { get; set; }

        internal Dictionary<int, TablixPageInfo> FlowPageSize { get; set; }

        internal Dictionary<int, TablixPageInfo> PrintPageSize { get; set; }

        internal Dictionary<int, List<int>> ItemPostion { get; set; }
    }

    internal class TablixRows : List<TablixRow>
    {
    }

    internal class GroupInfoCollection : List<GroupInfo>
    {
    }

    internal class FieldValue
    {
        public string Name { get; set; }

        public SummaryBase Value { get; set; }

        public KeysCalculationValues GroupKey { get; set; }

        public KeysCalculationValues RowKey { get; set; }

        public KeysCalculationValues ColumnKey { get; set; }

        public string RowGroupName { get; set; }

        public string ColumnGroupName { get; set; }
    }

    internal class TablixEngineFieldValue
    {
        public List<FieldValue> Value { get; set; }
    }

    public class TablixCellInfo
    {
        internal IReportItemModeler CachedItemModel { get; set; }

        internal IReportItemModeler ItemModel { get; set; }

        internal BorderExpval Border { get; set; }

        internal int RowIndex { get; set; }

        internal int ColumnIndex { get; set; }

        internal int RowSpan { get; set; }

        internal int ColumnSpan { get; set; }

        internal double Height { get; set; }

        internal double Width { get; set; }

        internal List<double> Widths { get; set; }
    
        internal CoveredCellRange CellRange { get; set; }

        internal List<FieldValue> FieldValues { get; set; }

        internal TablixRows Rows { get; set; }

        internal List<object> DataSource { get; set; }

        internal object Control { get; set; }

        internal List<int> CurrentKey {get;set;}

        internal Dictionary<string, object> RowNumbers { get; set; }

        internal bool IsDetailCell { get; set; }

        internal bool HasRowNumber { get; set; }

        internal List<DocumentData> IsDocumentMapRefer { get; set; }

        internal bool IsMapCell { get; set; }

        internal TablixCellInfo()
        {
        }

        public object Clone()
        {
            if (this.ItemModel.Model.EnableVirtualEvaluation)
            {
                if (this.IsDetailCell && this.DataSource == null)
                {
                   this.DataSource = new List<object>();
                }

                if (this.Rows == null)
                {
                    this.Rows = new TablixRows();
                }
                return this;
            }            
            else
            {
                TablixCellInfo cellInfo = new TablixCellInfo();
                cellInfo.ItemModel = this.ItemModel.GetModel();
                cellInfo.IsDetailCell = this.IsDetailCell;
                cellInfo.Height = this.Height;
                cellInfo.Width = this.Width;
                cellInfo.ColumnSpan = this.ColumnSpan;
                cellInfo.RowSpan = this.RowSpan;
                cellInfo.Widths = this.Widths;
                cellInfo.HasRowNumber = this.HasRowNumber;
                cellInfo.IsDocumentMapRefer = this.IsDocumentMapRefer;
                return cellInfo;
            }
        }

        public void DisposeEvalObjects()
        {
            if (this.ItemModel.Model.EnableVirtualEvaluation)
            {
                this.Border = null;
                this.ItemModel.DisposeEvalObjects();

                this.ItemModel.DisposeReportItemObj();
                this.ItemModel = null;
                this.ItemModel = this.CachedItemModel;
                this.CachedItemModel = null;
            }
        }
        
        public object GetDataSource(int i,int j)
        {
            int index = i;
            int colIndex = j;

            if (this.Rows.Count != 0)
            {
                if (this.IsDetailCell)
                {
                    List<object> dataSources = new List<object>();
                    dataSources.Add(this.Rows[index].DataSource[colIndex]);
                    return dataSources;
                }
                else
                {
                    return this.Rows[index].DataSource[colIndex];
                }
            }

            return null;
        }

        internal List<FieldValue> GetFieldValues(int i, int j)
        {
            int index = i;
            int colIndex = j;

            if (!this.IsDetailCell && this.Rows.Count != 0)
            {
                return this.Rows[index].FieldValues[colIndex];
            }

            return null;
        }

        internal Dictionary<string, object> GetRowNumbers(int i, int j)
        {
            int index = i;
            int colIndex = j;

            if (this.HasRowNumber && this.Rows.Count != 0)
            {
                return new Dictionary<string, object>(this.Rows[index].RowNumberValues[colIndex]);
            }

            return null;
        }

        public void Evaluate()
        {
            this.CachedItemModel = this.ItemModel;

            if (this.ItemModel.ModelType != ModelType.SubReportModel || this.ItemModel.ModelType != ModelType.GaugeModel || this.ItemModel.ModelType != ModelType.ChartModel || this.ItemModel.ModelType != ModelType.MapModel)
            {
                if (this.ItemModel.Model.EnableVirtualEvaluation)
                {
                    this.ItemModel = this.ItemModel.GetModel();
                    int index = CurrentKey[0];
                    int colIndex = CurrentKey[1];

                    if (this.Rows.Count != 0)
                    {
                        if (this.IsDetailCell)
                        {
                            List<object> dataSources = new List<object>();
                            dataSources.Add(this.Rows[index].DataSource[colIndex]);
                            this.ItemModel.DataSource = dataSources;
                        }
                        else
                        {
                            this.ItemModel.DataSource = this.Rows[index].DataSource[colIndex];
                            this.ItemModel.FieldValues = this.Rows[index].FieldValues[colIndex];
                        }
                        if (this.HasRowNumber)
                        {
                            this.ItemModel.RowNumbers = new Dictionary<string, object>(this.Rows[index].RowNumberValues[colIndex]);
                        }
                    }
                }
                else
                {
                    if (this.IsDetailCell)
                    {
                        this.ItemModel.FieldValues = null;
                        this.ItemModel.DataSource = this.DataSource;
                    }
                    else
                    {
                        this.ItemModel.FieldValues = this.FieldValues;
                        this.ItemModel.DataSource = this.DataSource;
                    }
                    if (this.HasRowNumber)
                    {
                        this.ItemModel.RowNumbers = new Dictionary<string, object>(this.RowNumbers);
                    }
                }
            }

            this.ItemModel.Evaluate();

            BorderExpval borderVal = null;

            if (this.ItemModel is TextboxModel)
            {
                borderVal = (this.ItemModel as TextboxModel).TextBoxProperties.Border;
            }
            else if (this.ItemModel is RectangleModel)
            {
                borderVal = (this.ItemModel as RectangleModel).RectItemExpPro.Border;
            }
            else if (this.ItemModel is ImageModel)
            {
                borderVal = (this.ItemModel as ImageModel).ImageProperties.Border;
            }

            this.Border = borderVal;
        }
    }

    internal class GroupValueInfo
    {
        public KeysCalculationValues GroupKey { get; set; }

        public KeysCalculationValues RecursiveParentGroupKey { get; set; }

        public KeysCalculationValues ParentGroupKey { get; set; }

        public BinaryList GroupFieldKeys { get; set; }

        public BinaryList FieldSortOrderKey { get; set; }

        public int Level { get; set; }

        public List<object> DataSources { get; set; }

        public List<object> RecursiveDataSources { get; set; }

        public List<TablixEngineFieldValue> Values { get; set; }

        public List<DataField> DocumentMapFields { get; set; }

        public string DocumentKey { get; set; }

        public string ToggleItem { get; set; }

        public string IsHidden { get; set; }

        public object ToggleGroups { get; set; }

        public List<DataField> HiddenFields { get; set; }

        public GroupValueInfo()
        {
            this.GroupFieldKeys = new BinaryList();
            this.DataSources = new List<object>();
            this.RecursiveDataSources = new List<object>();
            this.Values = new List<TablixEngineFieldValue>();
            this.FieldSortOrderKey = new BinaryList();
        }
    }

    internal class GroupRow : List<TablixCellInfo>
    {
    }

    internal class SortExpression
    {
        public string Expression { get; set; }

        public SortDirection SortOrder { get; set; }
    }

    internal class RepeatHeaderInfo
    {
        public string GroupName { get; set; }

        public bool IsNextGroup { get; set; }

        public KeepWithGroup KeepWithGroup { get; set; }
    }

    internal class GroupInfo
    {
        public string Name { get; set; }

        public bool IsRow { get; set; }

        public TablixGroupType Type { get; set; }

        public List<TablixEngineField> Fields { get; set; }

        public BinaryList GroupValueOrderKeys { get; set; }

        public List<GroupValueInfo> GroupValuesIndexes { get; set; }

        public string RecursiveParent { get; set; }

        public List<string> GroupExpressions { get; set; }

        public List<DataField> GroupFields { get; set; }

        public List<DataField> DocumentMapFields { get; set; }

        public string DocumentKey { get; set; }

        public BinaryList GroupRecursiveParentOrderKey { get; set; }

        public BinaryList GroupSortOrderKey { get; set; }

        public List<SortExpression> SortExpressions { get; set; }

        public List<DataField> SortFields { get; set; }

        public List<DataField> RecursiveParentFields { get; set; }

        public List<DataField> HiddenFields { get; set; }

        public List<GroupValueInfo> GroupValues { get; set; }

        public int Rowcount { get; set; }

        public int Columcount { get; set; }

        public List<GroupRow> RowValues { get; set; }

        public Group Group { get; set; }

        public GroupInfo ParentGroup { get; set; }

        public GroupInfoCollection Groups { get; set; }

        public TablixCellInfo HeaderInfo { get; set; }

        public int HeaderLevel { get; set; }

        public BreakLocation PageBreak { get; set; }

        public KeepWithGroup KeepWithGroup { get; set; }

        public bool KeepTogether { get; set; }

        public bool RepeatOnNewPage { get; set; }

        public bool HasRowNumber { get; set; }

        public string IsHidden { get; set; }

        public string ToggleItem { get; set; }

        public object ToggleGroups { get; set; }

        public GroupInfo()
        {
            this.Groups = new GroupInfoCollection();
            this.Fields = new List<TablixEngineField>();
            this.RowValues = new List<GroupRow>();
            this.GroupValuesIndexes = new List<GroupValueInfo>();
            this.GroupValueOrderKeys = new BinaryList();
            this.GroupSortOrderKey = new BinaryList();
            this.GroupRecursiveParentOrderKey = new BinaryList();
            this.GroupValues = new List<GroupValueInfo>();
            this.Rowcount = 0;
            this.Columcount = 0;
            this.HeaderLevel = 0;
        }
    }

    internal enum TablixGroupType
    {
        None,
        Detail,
        Group
    }

    internal class TablixEngineField : DataField
    {
        public string RowGroupName { get; set; }

        public string ColumnGroupName { get; set; }
    }


    internal class TablixCellRun
    {
        public string Text
        {
            get;
            set;
        }

        public string Format
        {
            get;
            set;
        }

        public string Language
        {
            get;
            set;
        }
    }

    internal class TablixPageInfo
    {
        internal int LineCount { get; set; }

        internal int[] RowIndices { get; set; }

        internal int[] ColumnIndices { get; set; }
        
        internal double Height { get; set; }

        internal double Width { get; set; }

        internal List<SizeWH> ColumnWiths { get; set; }

        internal List<SizeWH> RowHeights { get; set; }
    }

    internal class ColumnInfo
    {
        internal int ColumnCount { get; set; }

        internal int[] ColumnIndices { get; set; }

        internal double Width { get; set; }

        internal List<SizeWH> ColumnWiths { get; set; }
    }

    internal class CoveredCellRange
    {
        public int Top { get; set; }

        public int Left { get; set; }

        public int Bottom { get; set; }

        public int Right { get; set; }

        public CoveredCellRange()
        {
        }

        public CoveredCellRange(int top, int left, int bottom, int right)
        {
            this.Top = top;
            this.Bottom = bottom;
            this.Left = left;
            this.Right = right;
        }
    }

    internal class CoveredGroupRange
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public bool KeepTogether { get; set; }

        public CoveredGroupRange()
        {
        }
    }

    internal class SizeWH
    {
        internal int Index { get; set; }
        internal double Size { get; set; }
    }

    internal class DrillDownModel
    {
        public List<ToggleInfo> InnerDrillInfo { get; set; }

        public DrillDownModel()
        {
            this.InnerDrillInfo=new List<ToggleInfo>();
        }
    }

    internal class ToggleGropInfo
    {
        public int RowNo { get; set; }
        public int ColNo { get; set; }
        public DrillDownModel DrillDownInfos { get; set; }

        public ToggleGropInfo()
        {
            this.RowNo = -1;
            this.ColNo = -1;
        }
    }

    internal class ToggleInfo
    {
        public string ToggleItem { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public bool IsHidden { get; set; }
        public DrillDownModel Node { get; set; }
        public string ParentGroupKey { get; set; }
        public bool IsRow { get; set; }
        public int HeaderLevel { get; set; }

        public ToggleInfo()
        {
            this.StartIndex = -1;
            this.EndIndex = -1;
        }
    }
}