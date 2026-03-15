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
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Layout;
using Syncfusion.RDL.ItemModel;

namespace Syncfusion.ReportWriter
{
    internal class ExcelReportItemCellModel
    {
        public double Top { get; set; }

        public double Left { get; set; }

        public double Height { get; set; }

        public bool IsHeaderItem { get; set; }

        public double Width { get; set; }

        public bool IsTablixCell { get; set; }

        public bool IsList { get; set; }

        public object DataSource { get; set; }

        internal List<FieldValue> FieldValues { get; set; }

        internal Dictionary<string, object> RowNumbers { get; set; }

        public TablixCellInfo CellInfo { get; set; }

        public IReportItemModeler ReportItemModel { get; set; }
    }

    class ExcelReportItemCellValue
    {
        public int RowIndex { get; set; }

        public int TotalRows { get; set; }

        public int TotalColumns { get; set; }

        public int ColumnIndex { get; set; }

        public int RowSpan { get; set; }

        public int ColumnSpan { get; set; }

        public double? Bottom { get; set; }

        public double? Right { get; set; }

        public ExcelReportItemCellModel ReportItemCellModel { get; set; }
    }

    internal class ExcelLayoutHelper
    {
        internal PageModelFactory PageModelFactoty { get; set; }

        internal bool IsExcelWriter { get; set; }

        public List<ExcelReportItemCellModel> GetReportItemCellModels(UpdateSection updateSection, ExcelReportItemCellModel contentCollect, int pageNo)
        {
            ReportModelContentCollection contentCollection = new ReportModelContentCollection();
            double top = 0;

            if (contentCollect == null)
            {
                if (updateSection == UpdateSection.Body)
                {
                    contentCollection = PageModelFactoty.FlowLayoutDictionary[pageNo].ReportModelCollection;
                }
                else if (updateSection == UpdateSection.Footer)
                {
                    contentCollection = PageModelFactoty.Model.FooterReportItemModels;
                }
                else if (updateSection == UpdateSection.Header)
                {
                    contentCollection = PageModelFactoty.Model.HeaderReportItemModels;
                }
            }
            else
            {
                contentCollection.Add(contentCollect.CellInfo.ItemModel);
            }

            List<ExcelReportItemCellModel> cellModels = new List<ExcelReportItemCellModel>();
            int value = 0;
            foreach (var model in contentCollection)
            {
                value++;
                if (model.ModelType != ModelType.TablixModel)
                {
                    ExcelReportItemCellModel rptModel = new ExcelReportItemCellModel();
                    if (updateSection == UpdateSection.Header)
                    {
                        rptModel.IsHeaderItem = true;
                        if (value == contentCollection.Count)
                        {
                            top += model.FlowLayoutInfo.ActualTop;
                            top += model.FlowLayoutInfo.ActualHeight + this.PageModelFactoty.HeaderBottomGap;
                        }
                    }
                    rptModel.ReportItemModel = model;
                    if (model.FlowLayoutInfo != null)
                    {
                        rptModel.Height = model.FlowLayoutInfo.ActualHeight;
                        rptModel.Width = model.FlowLayoutInfo.ActualWidth;
                        rptModel.Left = model.FlowLayoutInfo.ActualLeft;
                        if (updateSection == UpdateSection.Body)
                        {
                            rptModel.Top = model.FlowLayoutInfo.ActualTop + top;
                        }
                        else
                        {
                            rptModel.Top = model.FlowLayoutInfo.ActualTop;
                        }
                    }
                    else
                    {
                        rptModel.Height = model.Height;
                        rptModel.Width = model.Width;
                        rptModel.Left = model.Left;
                        if (updateSection == UpdateSection.Body)
                        {
                            rptModel.Top = model.Top + top;
                        }
                        else
                        {
                            rptModel.Top = model.Top;
                        }
                    }
                    cellModels.Add(rptModel);
                }
                else
                {
                    this.GetTablixCellModels(cellModels, model, top, 0, pageNo);
                }
            }
            return cellModels;
        }

        void GetTablixCellModels(List<ExcelReportItemCellModel> cellModels, IReportItemModeler model, double top, double left, int pageNo)
        {
            TablixModel tablix = model as TablixModel;
            double celltop = 0;
            double cellLeft = 0;
            double tabLeft = 0;
            double tabTop = 0;
            if (model.FlowLayoutInfo != null)
            {
                tabLeft = model.FlowLayoutInfo.ActualLeft;
                tabTop = model.FlowLayoutInfo.ActualTop;
            }
            else
            {
                tabLeft = model.Left;
                tabTop = model.Top;
            }

            var flowinfo = tablix.FlowPageSizes[pageNo];

            var rowheight = 0.0;
            for (int i = 0; i < flowinfo.RowIndices.Count(); i++)
            {
                //if (!tablix.IsVisibilityCheck(tablix.DrillDownInfos.InnerDrillInfo, i + 1))
                {
                    for (int j = 0; j < tablix.ColumnCount; j++)
                    {
                        TablixCellInfo cellInfo = tablix.Data[flowinfo.RowIndices[i] - 1][j];
                        ExcelReportItemCellModel rptModel = new ExcelReportItemCellModel();

                        if (cellInfo != null && tablix.Model.EnableVirtualEvaluation)
                        {
                            rptModel.DataSource  = cellInfo.GetDataSource(flowinfo.RowIndices[i] - 1, j);
                            rptModel.FieldValues = cellInfo.GetFieldValues(flowinfo.RowIndices[i] - 1, j);
                            rptModel.RowNumbers = cellInfo.GetRowNumbers(flowinfo.RowIndices[i] - 1, j);
                        }

                        celltop = tabTop;

                        celltop += rowheight;
                        //for (int row = 0; row < i; row++)
                        //{
                        //    celltop += tablix.RowHeights[row];
                        //}

                        cellLeft = tabLeft;
                        for (int column = 0; column < j; column++)
                        {
                            cellLeft += tablix.ColumnWights[column];
                        }
                        rptModel.IsTablixCell = true;
                        if (cellInfo != null && cellInfo.ItemModel.ModelType == ModelType.RectangleModel)
                        {
                            if (cellInfo != null && tablix.Model.EnableVirtualEvaluation)
                            {
                                rptModel.DataSource = cellInfo.ItemModel.DataSource = cellInfo.GetDataSource(flowinfo.RowIndices[i] - 1, j);
                                rptModel.FieldValues = cellInfo.ItemModel.FieldValues = cellInfo.GetFieldValues(flowinfo.RowIndices[i] - 1, j);
                                rptModel.RowNumbers = cellInfo.ItemModel.RowNumbers = cellInfo.GetRowNumbers(flowinfo.RowIndices[i] - 1, j);
                            }

                            if (cellInfo.ItemModel.FlowLayoutInfo != null)
                            {
                                rptModel.Height = cellInfo.ItemModel.FlowLayoutInfo.ActualHeight;
                                rptModel.Width = cellInfo.ItemModel.FlowLayoutInfo.ActualWidth;
                                rptModel.Top = cellInfo.ItemModel.FlowLayoutInfo.ActualTop + celltop + top;
                                rptModel.Left = cellInfo.ItemModel.FlowLayoutInfo.ActualLeft + cellLeft;
                            }
                            else
                            {
                                rptModel.Height = cellInfo.ItemModel.Height;
                                rptModel.Width = cellInfo.ItemModel.Width;
                                rptModel.Top = cellInfo.ItemModel.Top + celltop + top;
                                rptModel.Left = cellInfo.ItemModel.Left + cellLeft;
                            }
                            rptModel.IsList = true;
                            rptModel.ReportItemModel = cellInfo.ItemModel;
                            //rptModel.CellInfo = cellInfo;
                            cellModels.Add(rptModel);

                            if (cellInfo.ItemModel.ReportItemModelers != null)
                            {
                                this.GetRectCellModels(cellModels, cellInfo.ItemModel, top, cellLeft, celltop, cellInfo, pageNo);
                            }
                        }
                        else if (cellInfo != null && cellInfo.ItemModel.ModelType == ModelType.TablixModel)
                        {
                            this.GetTablixCellModels(cellModels, cellInfo.ItemModel, top + celltop, left + cellLeft, pageNo);
                        }
                        else if (cellInfo != null)
                        {
                            rptModel.CellInfo = cellInfo;

                            var coverCells = from coveredCell in tablix.CoveredRanges
                                             where coveredCell.Left == j && coveredCell.Top == (flowinfo.RowIndices[i] - 1)
                                             select coveredCell;

                            if (coverCells.Count() > 0)
                            {
                                int leftCell = coverCells.First().Left;
                                int topCell = coverCells.First().Top;
                                double width = 0;
                                double height = 0;

                                for (int row = topCell; row <= coverCells.First().Bottom; row++)
                                {
                                    height += tablix.RowHeights[row];
                                }

                                for (int column = leftCell; column <= coverCells.First().Right; column++)
                                {
                                    width += tablix.ColumnWights[column];
                                }

                                rptModel.Height = height;
                                rptModel.Width = width;
                            }
                            else
                            {
                                rptModel.Height = tablix.RowHeights[flowinfo.RowIndices[i] - 1];
                                rptModel.Width = tablix.ColumnWights[j];
                            }

                            rptModel.Left = cellLeft + left;
                            rptModel.Top = celltop + top;

                            cellModels.Add(rptModel);
                        }
                    }
                    rowheight += tablix.RowHeights[flowinfo.RowIndices[i] - 1];
                }
            }
        }

        void GetRectCellModels(List<ExcelReportItemCellModel> cellModels, IReportItemModeler model, double top, double cellLeft, double cellTop, TablixCellInfo cellInfo, int pageNo)
        {
            foreach (var item in model.ReportItemModelers)
            {
                if (model.Model.EnableVirtualEvaluation)
                {
                    item.DataSource = model.DataSource;
                    item.FieldValues = model.FieldValues;
                    item.RowNumbers = model.RowNumbers;
                }

                if (item.ModelType == ModelType.RectangleModel)
                {
                    ExcelReportItemCellModel reportModel = new ExcelReportItemCellModel();
                    if (model.Model.EnableVirtualEvaluation)
                    {
                        reportModel.DataSource = item.DataSource;
                        reportModel.FieldValues = item.FieldValues;
                        reportModel.RowNumbers = item.RowNumbers;
                    }
                    reportModel.IsTablixCell = true;
                    reportModel.Height = item.Height;
                    reportModel.Left = item.Left + model.Left + cellLeft;
                    reportModel.Top = item.Top + cellTop + top;
                    reportModel.Width = item.Width;
                    reportModel.IsList = false;
                    reportModel.ReportItemModel = item;
                    cellModels.Add(reportModel);

                    this.GetRectCellModels(cellModels, item, top, model.Left + cellLeft, model.Top + item.Top + cellTop, cellInfo, pageNo);
                }
                else if (item.ModelType == ModelType.TablixModel)
                {
                    this.GetTablixCellModels(cellModels, item, top + cellTop + model.Top, model.Left + cellLeft, pageNo);
                }
                else
                {
                    ExcelReportItemCellModel reportModel = new ExcelReportItemCellModel();
                    if (model.Model.EnableVirtualEvaluation)
                    {
                        reportModel.DataSource = item.DataSource;
                        reportModel.FieldValues = item.FieldValues;
                        reportModel.RowNumbers = item.RowNumbers;
                    }
                    reportModel.IsTablixCell = true;
                    reportModel.Height = item.Height;
                    reportModel.Left = item.Left + model.Left + cellLeft;
                    reportModel.Top = item.Top + cellTop + top;
                    reportModel.Width = item.Width;
                    reportModel.IsList = false;
                    reportModel.ReportItemModel = item;
                    cellModels.Add(reportModel);
                }
            }
        }

        public List<double> UpdateRowHeightValues(List<ExcelReportItemCellModel> reportItemCellModels, List<ExcelReportItemCellValue> cellModels, UpdateSection updateSection, int headerRows)
        {
            List<double> rowHeights = new List<double>();
            int rowCount = 1;
            double lastTop = 0;
            reportItemCellModels.Sort(delegate(ExcelReportItemCellModel first, ExcelReportItemCellModel second)
            {
                return first.Top.CompareTo(second.Top);
            });

            foreach (var reportModel in reportItemCellModels)
            {
                ExcelReportItemCellValue indexer = new ExcelReportItemCellValue();

                indexer.Bottom = null;
                indexer.ReportItemCellModel = reportModel;

                double top = reportModel.Top;
                double height = reportModel.Height;

                indexer.Bottom = top + height;

                var cellValues = (from cellModel in cellModels
                                  where cellModel.Bottom != null && cellModel.Bottom <= top
                                  select cellModel).ToList();

                cellValues.Sort(delegate(ExcelReportItemCellValue first, ExcelReportItemCellValue second)
                {
                    return ((double)first.Bottom).CompareTo((double)second.Bottom);
                });
                //bool isSame = true;
                foreach (var cellModel in cellValues)
                {
                    if (Math.Round((decimal)cellModel.Bottom, 2) == Math.Round((decimal)lastTop, 2))
                    {
                        cellModel.RowSpan = (rowCount - 1) - cellModel.RowIndex;
                    }
                    else
                    {
                        cellModel.RowSpan = rowCount - cellModel.RowIndex;
                    }

                    if (Math.Round((decimal)cellModel.Bottom, 2) != Math.Round((decimal)lastTop, 2) && Math.Round((decimal)cellModel.Bottom, 2) != Math.Round((decimal)top, 2))
                    {
                        rowHeights.Add((double)cellModel.Bottom - lastTop);
                        //if (rowHeights.Count == headerRows && rowHeights.Count != 0)
                        //{
                        //    rowHeights.Add(this.PageModelFactoty.HeaderBottomGap);
                        //}
                        lastTop = (double)cellModel.Bottom;
                        rowCount++;
                    }

                    cellModel.Bottom = null;
                }

                if (Math.Round(top) != Math.Round(lastTop))
                {
                    //if (rowHeights.Count == headerRows + 1)
                    //{
                    //    rowHeights.Add(top - lastTop - this.PageModelFactoty.HeaderBottomGap);
                    //}
                    //else
                    {
                        rowHeights.Add(top - lastTop);
                    }
                    //if (rowHeights.Count == headerRows && rowHeights.Count != 0)
                    //{
                    //    rowHeights.Add(this.PageModelFactoty.HeaderBottomGap);
                    //}
                    rowCount++;
                    indexer.RowIndex = rowCount + headerRows;
                }
                else
                {
                    indexer.RowIndex = rowCount + headerRows;
                }

                lastTop = top;
                cellModels.Add(indexer);
            }


            var topCellModels = (from cellModel in cellModels
                                 where cellModel.Bottom != null
                                 select cellModel).ToList();

            topCellModels.Sort(delegate(ExcelReportItemCellValue first, ExcelReportItemCellValue second)
            {
                return ((double)first.Bottom).CompareTo(((double)second.Bottom));
            });


            foreach (var cellIndex in topCellModels)
            {
                if (Math.Round((decimal)cellIndex.Bottom, 2) != Math.Round((decimal)lastTop, 2))
                {
                    cellIndex.RowSpan = rowCount - cellIndex.RowIndex;
                    if (((double)cellIndex.Bottom - lastTop) <= lastTop)
                    {
                        rowHeights.Add((double)cellIndex.Bottom - lastTop);
                    }
                    else if (cellIndex.RowSpan != 0)
                    {
                        rowHeights.Add(cellIndex.RowSpan);
                    }
                    else
                    {
                        rowHeights.Add((double)cellIndex.Bottom - lastTop);
                    }
                    //if (rowHeights.Count == headerRows && rowHeights.Count != 0)
                    //{
                    //    rowHeights.Add(this.PageModelFactoty.HeaderBottomGap);
                    //}
                    lastTop = (double)cellIndex.Bottom;
                    rowCount++;
                }
                else
                {
                    cellIndex.RowSpan = (rowCount - 1) - cellIndex.RowIndex;
                }

                cellIndex.Bottom = null;
            }

            if (updateSection == UpdateSection.Body)
            {
                if (this.PageModelFactoty.BodyBottomGap <= 409.5)
                {
                    rowHeights.Add(this.PageModelFactoty.BodyBottomGap);
                }
                else
                {
                    rowHeights.Add(409.5);
                }
            }
            else if (updateSection == UpdateSection.Footer)
            {
                rowHeights.Add(this.PageModelFactoty.FooterBottomGap);
            }
            else if (updateSection == UpdateSection.Header && headerRows == 0)
            {
                rowHeights.Add(this.PageModelFactoty.HeaderBottomGap);
            }

            return rowHeights;
        }

        public List<double> UpdateColumnWidthValues(List<ExcelReportItemCellModel> reportItemCellModels, List<ExcelReportItemCellValue> cellModels, UpdateSection updateSection)
        {
            List<double> columnWidths = new List<double>();
            int columnCount = 1;

            double lastLeft = 0;

            reportItemCellModels.Sort(delegate(ExcelReportItemCellModel first, ExcelReportItemCellModel second)
            {
                return first.Left.CompareTo(second.Left);
            });

            foreach (var reportModel in reportItemCellModels)
            {
                var indexer = (from cellModel in cellModels
                               where cellModel.ReportItemCellModel == reportModel
                               select cellModel).First();

                indexer.Bottom = null;
                indexer.Right = null;

                double left = reportModel.Left;
                double width = reportModel.Width;

                indexer.Right = left + width;

                var cellValues = (from cellModel in cellModels
                                  where cellModel.Right != null && cellModel.Right <= left
                                  select cellModel).ToList();

                cellValues.Sort(delegate(ExcelReportItemCellValue first, ExcelReportItemCellValue second)
                {
                    return ((double)first.Right).CompareTo((double)second.Right);
                });

                foreach (var cellModel in cellValues)
                {
                    if (Math.Round((decimal)cellModel.Right, 2) == Math.Round((decimal)lastLeft, 2))
                    {
                        cellModel.ColumnSpan = (columnCount - 1) - cellModel.ColumnIndex;
                    }
                    else
                    {
                        cellModel.ColumnSpan = columnCount - cellModel.ColumnIndex;
                    }

                    if (Math.Round((decimal)cellModel.Right, 2) != Math.Round((decimal)lastLeft, 2) && Math.Round((decimal)cellModel.Right, 2) != Math.Round((decimal)left, 2))
                    {
                        columnWidths.Add((double)cellModel.Right - lastLeft);
                        lastLeft = (double)cellModel.Right;
                        columnCount++;
                    }

                    cellModel.Right = null;
                }

                if (Math.Round(left) != Math.Round(lastLeft))
                {
                    columnWidths.Add(left - lastLeft);
                    columnCount++;
                    indexer.ColumnIndex = columnCount;
                }
                else
                {
                    indexer.ColumnIndex = columnCount;
                }

                lastLeft = left;
            }

            var rightCellModels = (from cellModel in cellModels
                                   where cellModel.Right != null
                                   select cellModel).ToList();

            rightCellModels.Sort(delegate(ExcelReportItemCellValue first, ExcelReportItemCellValue second)
            {
                return ((double)first.Right).CompareTo(((double)second.Right));
            });

            foreach (var cellModel in rightCellModels)
            {
                if (Math.Round((decimal)cellModel.Right, 2) != Math.Round((decimal)lastLeft, 2))
                {
                    cellModel.ColumnSpan = columnCount - cellModel.ColumnIndex;
                    columnWidths.Add((double)cellModel.Right - lastLeft);
                    lastLeft = (double)cellModel.Right;
                    columnCount++;
                }
                else
                {
                    cellModel.ColumnSpan = (columnCount - 1) - cellModel.ColumnIndex;
                }

                cellModel.Right = null;
            }

            return columnWidths;
        }
    }
}