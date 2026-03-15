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
    internal class WordReportItemCellModel
    {
        public double WordTop { get; set; }

        public double WordLeft { get; set; }

        public double WordHeight { get; set; }

        public double WordWidth { get; set; }

        public bool IsTablixCellWord { get; set; }

        public bool IsTablixCell { get; set; }

        public object DataSource { get; set; }

        internal Dictionary<string, object> RowNumbers { get; set; }

        internal List<FieldValue> FieldValues { get; set; }

        public TablixCellInfo WordCellInfo { get; set; }

        public IReportItemModeler WordReportItemModel { get; set; }
    }

    class WordReportItemCellValue
    {
        public int WordRowIndex { get; set; }

        public int WordTotalRows { get; set; }

        public int WordTotalColumns { get; set; }

        public int WordColumnIndex { get; set; }

        public int WordRowSpan { get; set; }

        public int WordColumnSpan { get; set; }

        public double? WordBottom { get; set; }

        public double? WordRight { get; set; }

        public WordReportItemCellModel WordReportItemCellModel { get; set; }
    }

    internal class WordLayoutHelper
    {
        internal PageModelFactory WordPageModelFactoty { get; set; }

        public List<WordReportItemCellModel> GetReportItemCellModels(UpdateSection updateSection, int pageNo)
        {
            ReportModelContentCollection contentCollection =new ReportModelContentCollection();
            double top = 0.0;
            if (updateSection == UpdateSection.Body)
            {
                contentCollection = WordPageModelFactoty.FlowLayoutDictionary[pageNo].ReportModelCollection;//Model.BodyReportItemModels;
            }
            else if (updateSection == UpdateSection.Footer)
            {
                contentCollection = WordPageModelFactoty.Model.FooterReportItemModels;
            }
            else if (updateSection == UpdateSection.Header)
            {
                contentCollection = WordPageModelFactoty.Model.HeaderReportItemModels;
            }

            List<WordReportItemCellModel> cellModels = new List<WordReportItemCellModel>();

            List<IReportItemModeler> tempReportItems = new List<IReportItemModeler>();

            int value = 0;
            foreach (var model in contentCollection)
            {
                value++;
                if(model.ModelType == ModelType.TablixModel)
                {
                    this.GetCollectTablixChildModel(model,top,cellModels);
                }
                else
                {
                    WordReportItemCellModel rptModel = new WordReportItemCellModel();
                    
                    if (updateSection == UpdateSection.Header)
                    {
                        //rptModel.IsHeaderItem = true;
                        if (value == contentCollection.Count)
                        {
                            top += model.FlowLayoutInfo.ActualTop;
                            top += model.FlowLayoutInfo.ActualHeight + this.WordPageModelFactoty.HeaderBottomGap;
                        }
                    }
                    rptModel.WordReportItemModel = model;
                    if (model.FlowLayoutInfo != null)
                    {
                        rptModel.WordHeight = model.FlowLayoutInfo.ActualHeight;
                        rptModel.WordWidth = model.FlowLayoutInfo.ActualWidth;
                        rptModel.WordLeft = model.FlowLayoutInfo.ActualLeft;
                        if (updateSection == UpdateSection.Body)
                        {
                            rptModel.WordTop = model.FlowLayoutInfo.ActualTop + top;
                        }
                        else
                        {
                            rptModel.WordTop = model.FlowLayoutInfo.ActualTop;
                        }
                    }
                    else
                    {
                        rptModel.WordHeight = model.Height;
                        rptModel.WordWidth = model.Width;
                        rptModel.WordLeft = model.Left;
                        if (updateSection == UpdateSection.Body)
                        {
                            rptModel.WordTop = model.Top + top;
                        }
                        else
                        {
                            rptModel.WordTop = model.Top;
                        }
                    }
                    //WordReportItemCellModel rptModel = new WordReportItemCellModel();
                    //rptModel.WordReportItemModel = model;
                    //rptModel.WordHeight = model.FlowLayoutInfo.ActualHeight;
                    //rptModel.WordWidth = model.FlowLayoutInfo.ActualWidth;
                    //rptModel.WordLeft = model.FlowLayoutInfo.ActualLeft;
                    //rptModel.WordTop = model.FlowLayoutInfo.ActualTop;

                    cellModels.Add(rptModel);
                }
            }

            return cellModels;
        }

       void GetCollectTablixChildModel(IReportItemModeler model, double top , List<WordReportItemCellModel> cellModels)
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

            var flowinfo = tablix.FlowPageSizes[0];
            var rowHeight = 0.0;
            for (int i = 0; i < flowinfo.RowIndices.Count(); i++)
            {
                if (!tablix.IsVisibilityCheck(tablix.DrillDownInfos.InnerDrillInfo, i + 1))
                {
                    for (int j = 0; j < tablix.ColumnCount; j++)
                    {
                        TablixCellInfo cellInfo = tablix.Data[flowinfo.RowIndices[i] - 1][j];
                        WordReportItemCellModel rptModel = new WordReportItemCellModel();

                        if (cellInfo != null && tablix.Model.EnableVirtualEvaluation)
                        {
                            rptModel.DataSource = cellInfo.GetDataSource(flowinfo.RowIndices[i] - 1, j);
                            rptModel.FieldValues = cellInfo.GetFieldValues(flowinfo.RowIndices[i] - 1, j);
                            rptModel.RowNumbers = cellInfo.GetRowNumbers(flowinfo.RowIndices[i] - 1, j);
                        }

                        celltop = tabTop + top;
                        celltop += rowHeight;
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
                            if (cellInfo.ItemModel.FlowLayoutInfo != null)
                            {
                                rptModel.WordHeight = cellInfo.ItemModel.FlowLayoutInfo.ActualHeight;
                                rptModel.WordWidth = cellInfo.ItemModel.FlowLayoutInfo.ActualWidth;
                                rptModel.WordTop = cellInfo.ItemModel.FlowLayoutInfo.ActualTop + celltop;
                                rptModel.WordLeft = cellInfo.ItemModel.FlowLayoutInfo.ActualLeft + cellLeft;
                            }
                            else
                            {
                                rptModel.WordHeight = cellInfo.ItemModel.Height;
                                rptModel.WordWidth = cellInfo.ItemModel.Width;
                                rptModel.WordTop = cellInfo.ItemModel.Top + celltop + top;
                                rptModel.WordLeft = cellInfo.ItemModel.Left + cellLeft;
                            }
                            rptModel.WordReportItemModel = cellInfo.ItemModel;
                            rptModel.WordCellInfo = cellInfo;
                            cellModels.Add(rptModel);
                            List<IReportItemModeler> temp = new List<IReportItemModeler>();
                            List<IReportItemModeler> report = new List<IReportItemModeler>();
                            report = cellInfo.ItemModel.ReportItemModelers.ToList();
                            if (cellInfo.ItemModel.ReportItemModelers != null)
                            {
                                foreach (var item in cellInfo.ItemModel.ReportItemModelers)
                                {
                                    if (item.ModelType == ModelType.RectangleModel && !temp.Contains(item))
                                    {
                                        for (int m = 0; m < item.ReportItemModelers.Count; m++)
                                        {
                                            if (item.ReportItemModelers[m].ModelType == ModelType.ImageModel)
                                            {
                                                int index = m;
                                                int count = item.ReportItemModelers.Count;
                                                List<IReportItemModeler> reportModels = new List<IReportItemModeler>();
                                                reportModels = item.ReportItemModelers.ToList();
                                                reportModels.Insert(0, item);
                                                foreach (var repitem in item.ReportItemModelers)
                                                {
                                                    report.Remove(item);
                                                    report.Remove(repitem);
                                                    temp.Add(repitem);
                                                }

                                                for (int c = 0; c < reportModels.Count; c++)
                                                {
                                                    report.Insert(c, reportModels[c]);
                                                }
                                            }
                                            if (item.ReportItemModelers[m].ModelType == ModelType.TextBoxModel &&
                                                item.ReportItemModelers.Count > 2)
                                            {
                                                WordReportItemCellModel reportModel = new WordReportItemCellModel();
                                                reportModel.WordCellInfo = cellInfo;
                                                reportModel.IsTablixCell = true;
                                                if (cellInfo != null && tablix.Model.EnableVirtualEvaluation)
                                                {
                                                    reportModel.DataSource =
                                                        cellInfo.GetDataSource(flowinfo.RowIndices[i] - 1, j);
                                                    reportModel.FieldValues =
                                                        cellInfo.GetFieldValues(flowinfo.RowIndices[i] - 1, j);
                                                    reportModel.RowNumbers =
                                                        cellInfo.GetRowNumbers(flowinfo.RowIndices[i] - 1,
                                                                               j);
                                                }
                                                reportModel.WordHeight = item.ReportItemModelers[m].Height;
                                                reportModel.WordLeft = item.ReportItemModelers[m].Left + report[0].Left;
                                                reportModel.WordTop = item.ReportItemModelers[m].Top + celltop +
                                                                      report[0].Top;
                                                reportModel.WordWidth = item.ReportItemModelers[m].Width;
                                                reportModel.WordReportItemModel = item.ReportItemModelers[m];

                                            }
                                        }
                                    }
                                }

                                for (int l = 0; l < report.Count; l++)
                                {
                                    WordReportItemCellModel reportModel = new WordReportItemCellModel();
                                    reportModel.WordCellInfo = cellInfo;
                                    reportModel.IsTablixCell = true;
                                    if (cellInfo != null && tablix.Model.EnableVirtualEvaluation)
                                    {
                                        reportModel.DataSource = cellInfo.GetDataSource(flowinfo.RowIndices[i] - 1, j);
                                        reportModel.FieldValues = cellInfo.GetFieldValues(flowinfo.RowIndices[i] - 1, j);
                                        reportModel.RowNumbers = cellInfo.GetRowNumbers(flowinfo.RowIndices[i] - 1, j);
                                    }
                                    if (report[l].FlowLayoutInfo != null)
                                    {
                                        reportModel.WordHeight = report[l].FlowLayoutInfo.ActualHeight;
                                        reportModel.WordWidth = report[l].FlowLayoutInfo.ActualWidth;
                                    }
                                    else
                                    {
                                        reportModel.WordHeight = report[l].Height;
                                        reportModel.WordWidth = report[l].Width;
                                    }
                                    reportModel.WordTop = report[l].Top + celltop;
                                    reportModel.WordLeft = report[l].Left;

                                    reportModel.WordReportItemModel = report[l];
                                    if (!report[l].IsTablixChild)
                                        cellModels.Add(reportModel);

                                }
                            }
                        }
                        else if (cellInfo != null)
                        {
                            rptModel.WordCellInfo = cellInfo;
                            celltop = tabTop;
                            celltop += rowHeight;
                            //for (int row = 0; row < i; row++)
                            //{
                            //    celltop += tablix.RowHeights[row];
                            //}

                            cellLeft = tabLeft;
                            for (int column = 0; column < j; column++)
                            {
                                cellLeft += tablix.ColumnWights[column];
                            }

                            var coverCells = from coveredCell in tablix.CoveredRanges
                                             where coveredCell.Left == j && coveredCell.Top == i
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

                                rptModel.WordHeight = height;
                                rptModel.WordWidth = width;
                            }
                            else
                            {
                                rptModel.WordHeight = tablix.RowHeights[i];
                                rptModel.WordWidth = tablix.ColumnWights[j];
                            }

                            rptModel.WordLeft = cellLeft;
                            rptModel.WordTop = celltop;

                            cellModels.Add(rptModel);
                        }
                    }
                    rowHeight += tablix.RowHeights[i];
                }
            }
       }

        public List<double> UpdateRowHeightValues(List<WordReportItemCellModel> reportItemCellModels, List<WordReportItemCellValue> cellModels, UpdateSection updateSection)
        {
            List<double> rowHeights = new List<double>();
            int rowCount = 1;
            double lastTop = 0;

            reportItemCellModels.Sort(delegate(WordReportItemCellModel first, WordReportItemCellModel second)
            {
                return first.WordTop.CompareTo(second.WordTop);
            });

            foreach (var reportModel in reportItemCellModels)
            {
                WordReportItemCellValue indexer = new WordReportItemCellValue();

                indexer.WordBottom = null;
                indexer.WordReportItemCellModel = reportModel;

                double top = reportModel.WordTop;
                double height = reportModel.WordHeight;

                indexer.WordBottom = top + height;

                var cellValues = (from cellModel in cellModels
                                  where cellModel.WordBottom != null && cellModel.WordBottom <= top
                                  select cellModel).ToList();

                cellValues.Sort(delegate(WordReportItemCellValue first, WordReportItemCellValue second)
                {
                    return ((double)first.WordBottom).CompareTo((double)second.WordBottom);
                });

                foreach (var cellModel in cellValues)
                {
                    if (Math.Round((decimal)cellModel.WordBottom,2) == Math.Round((decimal)lastTop,2))
                    {
                        cellModel.WordRowSpan = (rowCount-1) - cellModel.WordRowIndex;
                    }
                    else
                    {
                        cellModel.WordRowSpan = rowCount- cellModel.WordRowIndex;
                    }

                    if (Math.Round((decimal)cellModel.WordBottom, 2) != Math.Round((decimal)lastTop, 2) && Math.Round((decimal)cellModel.WordBottom, 2) != Math.Round((decimal)top,2))
                    {
                        rowHeights.Add((double)cellModel.WordBottom - lastTop);
                        lastTop = (double)cellModel.WordBottom;
                        rowCount++;
                    }

                    cellModel.WordBottom = null;
                }

                if (Math.Round(top) != Math.Round(lastTop))
                {
                    rowHeights.Add(top - lastTop);
                    rowCount++;
                    indexer.WordRowIndex = rowCount;
                }
                else
                {
                    indexer.WordRowIndex = rowCount;
                }

                lastTop = top;
                cellModels.Add(indexer);
            }


            var topCellModels = (from cellModel in cellModels
                                 where cellModel.WordBottom != null
                                 select cellModel).ToList();

            topCellModels.Sort(delegate(WordReportItemCellValue first, WordReportItemCellValue second)
            {
                return ((double)first.WordBottom).CompareTo(((double)second.WordBottom));
            });


            foreach (var cellIndex in topCellModels)
            {
                if (Math.Round((decimal)cellIndex.WordBottom, 2) != Math.Round((decimal)lastTop,2))
                {
                    cellIndex.WordRowSpan = rowCount - cellIndex.WordRowIndex;
                    rowHeights.Add((double)cellIndex.WordBottom - lastTop);
                    lastTop = (double)cellIndex.WordBottom;
                    rowCount++;
                }
                else
                {
                    cellIndex.WordRowSpan = (rowCount - 1) - cellIndex.WordRowIndex;
                }

                cellIndex.WordBottom = null;
            }
            bool isRectangle = false;
            double rectHeight = 0;
            foreach (var item in reportItemCellModels)
            {
                if (item.WordReportItemModel!=null && item.WordReportItemModel.ContainerModel != null)
                {
                    if (item.WordReportItemModel.ContainerModel.ModelType == ModelType.RectangleModel)
                    {
                        isRectangle = true;
                        rectHeight = item.WordReportItemModel.ContainerModel.Height;

                        if (item.WordReportItemModel.ContainerModel.FlowLayoutInfo != null)
                        {
                            rectHeight = item.WordReportItemModel.ContainerModel.FlowLayoutInfo.ActualHeight;
                        }
                    }
                }
            }

            if (isRectangle)
            {
               double totalHeight = 0;
               foreach(int height in rowHeights)
               {
                   totalHeight += height;
               }
               rowHeights.Add(rectHeight - totalHeight);
            }
            else
            {
                if (updateSection == UpdateSection.Body)
                {
                    rowHeights.Add(this.WordPageModelFactoty.BodyBottomGap);
                }
                else if (updateSection == UpdateSection.Footer)
                {
                    rowHeights.Add(this.WordPageModelFactoty.FooterBottomGap);
                }
                else if (updateSection == UpdateSection.Header)
                {
                    rowHeights.Add(this.WordPageModelFactoty.HeaderBottomGap);
                }
            }
            

            return rowHeights;
        }

        public List<double> UpdateColumnWidthValues(List<WordReportItemCellModel> reportItemCellModels, List<WordReportItemCellValue> cellModels, UpdateSection updateSection)
        {
            List<double> columnWidths = new List<double>();
            int columnCount = 1;

            double lastLeft = 0;

            reportItemCellModels.Sort(delegate(WordReportItemCellModel first, WordReportItemCellModel second)
            {
                return first.WordLeft.CompareTo(second.WordLeft);
            });

            foreach (var reportModel in reportItemCellModels)
            {
                var indexer = (from cellModel in cellModels
                               where cellModel.WordReportItemCellModel == reportModel
                               select cellModel).First();

                indexer.WordBottom = null;
                indexer.WordRight = null;

                double left = reportModel.WordLeft;
                double width = reportModel.WordWidth;

                indexer.WordRight = left + width;

                var cellValues = (from cellModel in cellModels
                                  where cellModel.WordRight != null && cellModel.WordRight <= left
                                  select cellModel).ToList();

                cellValues.Sort(delegate(WordReportItemCellValue first, WordReportItemCellValue second)
                {
                    return ((double)first.WordRight).CompareTo((double)second.WordRight);
                });

                foreach (var cellModel in cellValues)
                {
                    if (Math.Round((decimal)cellModel.WordRight,2) == Math.Round((decimal)lastLeft,2))
                    {
                        cellModel.WordColumnSpan = (columnCount - 1) - cellModel.WordColumnIndex;
                    }
                    else
                    {
                        cellModel.WordColumnSpan = columnCount - cellModel.WordColumnIndex;
                    }

                    if (Math.Round((decimal)cellModel.WordRight, 2) != Math.Round((decimal)lastLeft, 2) && Math.Round((decimal)cellModel.WordRight, 2) != Math.Round((decimal)left, 2))
                    {
                        columnWidths.Add((double)cellModel.WordRight - lastLeft);
                        lastLeft = (double)cellModel.WordRight;
                        columnCount++;
                    }

                    cellModel.WordRight = null;
                }

                if (Math.Round(left) != Math.Round(lastLeft))
                {
                    columnWidths.Add(left - lastLeft);
                    columnCount++;
                    indexer.WordColumnIndex = columnCount;
                }
                else
                {
                    indexer.WordColumnIndex = columnCount;
                }

                lastLeft = left;
            }

            var rightCellModels = (from cellModel in cellModels
                                   where cellModel.WordRight != null
                                   select cellModel).ToList();

            rightCellModels.Sort(delegate(WordReportItemCellValue first, WordReportItemCellValue second)
            {
                return ((double)first.WordRight).CompareTo(((double)second.WordRight));
            });

            foreach (var cellModel in rightCellModels)
            {
                if (Math.Round((decimal)cellModel.WordRight,2) != Math.Round((decimal)lastLeft,2))
                {
                    cellModel.WordColumnSpan = columnCount - cellModel.WordColumnIndex;
                    columnWidths.Add((double)cellModel.WordRight - lastLeft);
                    lastLeft = (double)cellModel.WordRight;
                    columnCount++;
                }
                else
                {
                    cellModel.WordColumnSpan = (columnCount - 1) - cellModel.WordColumnIndex;
                }

                cellModel.WordRight = null;
            }

            return columnWidths;
        }
    }
}