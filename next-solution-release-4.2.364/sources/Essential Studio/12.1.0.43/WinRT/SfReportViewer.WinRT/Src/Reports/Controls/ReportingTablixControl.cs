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
using System.Threading.Tasks;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.ItemModel;
using Syncfusion.UI.Xaml.Reports.Reports.Controls;
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Controls.Grid;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.RDL.Controls
{
    class ReportingTablixControl : GridControl
    {
        internal TablixModel TablixModel
        {
            get;
            set;
        }

        internal Dictionary<int, TablixPageInfo> PageSizes
        {
            get
            {
               return this.TablixModel.PrintPageSizes;
            }
        }

        internal int PageCount
        {
            get
            {
                return this.TablixModel.PrintPageSizes.Count;
            }
        }

        int currentPage = -1;
        public int CurrentPage
        {
            get
            {
                return this.currentPage;
            }
            set
            {
                this.currentPage = value;
            }
        }

        public ReportingTablixControl(IReportItemModeler model)
        {
            this.Model.Options.AllowSelection = GridSelectionFlags.None;
            this.Model.Options.ListBoxSelectionMode = GridSelectionMode.None;
            this.Model.Options.ShowCurrentCell = false;

            this.TablixModel = model as TablixModel;

            this.Model.RowCount = 0;
            this.Model.ColumnCount = 0;

            Canvas.SetLeft(this, this.TablixModel.Left);
            Canvas.SetTop(this, this.TablixModel.Top);

            this.Name = this.TablixModel.Name;
            this.Model.RowCount = 0;
            this.Model.ColumnCount = 0;

            this.VerticalPixelScroll = false;
            this.HorizontalPixelScroll = false;
        }


        internal void UpdatePage()
        {
            this.SetCellValues();
            if (!this.TablixModel.IsTablixChild)
            {
                Canvas.SetLeft(this, this.currentPage % this.TablixModel.PrintPageColumnCount == 0 ? this.TablixModel.PrintPageInfo.ActualLeft : 0);
                Canvas.SetTop(this, this.currentPage < this.TablixModel.PrintPageColumnCount ? this.TablixModel.PrintPageInfo.ActualTop : 0);
            }
            this.TablixModel = null;
        }

        private void SetCellValues()
        {
            if (this.PageSizes == null)
            {
                this.Visibility = Visibility.Collapsed;
                return;
            }

            if (this.PageSizes[this.currentPage].Height <= 0 || this.PageSizes[this.currentPage].Width <= 0)
            {
                this.Visibility = Visibility.Collapsed;
                return;
            }

            if (this.PageSizes != null && this.CurrentPage >= 0 && this.CurrentPage < this.PageCount)
            {
                var pageInfo = this.PageSizes[this.currentPage];

                if (pageInfo.RowIndices.Count() > 0 && pageInfo.ColumnIndices.Count() > 0)
                {
                    this.Width = this.PageSizes[this.CurrentPage].Width;
                    this.Height = this.PageSizes[this.CurrentPage].Height;

                    this.Model.RowCount = pageInfo.RowIndices.Count() + 1;
                    this.Model.ColumnCount = pageInfo.ColumnIndices.Count() + 1;

                    this.Model.ColumnWidths[0] = 0;
                    this.Model.RowHeights[0] = 0;

                    int rowStart = pageInfo.RowIndices[0];
                    int rowEnd = pageInfo.RowIndices[pageInfo.RowIndices.Length - 1];

                    int columnStart = pageInfo.ColumnIndices[0];
                    int columnEnd = pageInfo.ColumnIndices[pageInfo.ColumnIndices.Length - 1];

                    for (int row = 1; row < this.Model.RowCount; row++)
                    {
                        this.Model.RowHeights[row] = this.TablixModel.RowHeights[row + (rowStart - 1) - 1];
                    }

                    for (int column = 1; column < this.Model.ColumnCount; column++)
                    {
                            this.Model.ColumnWidths[column] = this.TablixModel.ColumnWights[column + (columnStart - 1) - 1];
                    }

                    for (int row = 1; row < this.Model.RowCount; row++)
                    {
                        for (int column = 1; column < this.Model.ColumnCount; column++)
                        {
                            var cellDetail = this.Model[row, column];
                            if (cellDetail.RowIndex > 0 && cellDetail.ColumnIndex > 0)
                            {
                                var cellInfo = TablixModel.Data[pageInfo.RowIndices[row - 1] - 1][column + (columnStart - 1) - 1];

                                cellDetail.CellType = "DataBoundTemplate";
                                cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";

                                if (cellInfo != null && this.TablixModel.Model.EnableVirtualEvaluation)
                                {
                                    cellInfo.CurrentKey = new List<int>();
                                    cellInfo.CurrentKey.Add(pageInfo.RowIndices[row - 1] - 1);
                                    cellInfo.CurrentKey.Add(column + (columnStart - 1) - 1);

                                    if (cellInfo.ItemModel.ModelType != ModelType.TablixModel && cellInfo.ItemModel.ModelType != ModelType.RectangleModel)
                                    {
                                        cellInfo.Evaluate();
                                    }
                                }

                                cellDetail.Borders.All = new WinRT.Controls.Pen(new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Transparent), 1);

                                if (cellInfo != null && cellInfo.Border != null)
                                {
                                    var border = cellInfo.Border;

                                    this.ApplyBorders(border, brushConvertor, cellDetail);
                                }

                                if (cellInfo != null)
                                {
                                    if (cellInfo.ItemModel.ModelType == ModelType.TextBoxModel)
                                    {
                                        if ((cellInfo.ItemModel as TextboxModel).ToggleGroups == null)
                                        {

                                            cellDetail.CellType = "TextBox";
                                            TextboxModel model = cellInfo.ItemModel as TextboxModel;
                                            ParagraphExpval para = model.ParaExpval.First();
                                            TextRunExpval runVal = para.Runs.First();

                                            cellDetail.TextTrimming = TextTrimming.None;
                                            cellDetail.TextWrapping = TextWrapping.Wrap;

                                            if (para.TextAlignment == "Left")
                                            {
                                                cellDetail.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                                            }
                                            else if (para.TextAlignment == "Right")
                                            {
                                                cellDetail.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
                                            }
                                            else if (para.TextAlignment == "Center")
                                            {
                                                cellDetail.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
                                            }

                                            var expVal = model.TextBoxProperties;

                                            if (expVal.Padding != null)
                                            {
                                                cellDetail.BorderMargins.Left = expVal.Padding.Left;
                                                cellDetail.BorderMargins.Right = expVal.Padding.Right;
                                                cellDetail.BorderMargins.Top = expVal.Padding.Top;
                                                cellDetail.BorderMargins.Bottom = expVal.Padding.Bottom;

                                                cellDetail.TextMargins.Bottom = 0;
                                                cellDetail.TextMargins.Top = 0;
                                                cellDetail.TextMargins.Left = 0;
                                                cellDetail.TextMargins.Right = 0;
                                            }

                                            if (model.TextBoxProperties != null)
                                            {
                                                if (model.TextBoxProperties.VerticalAlignment == DOM.VerticalAlign.Bottom)
                                                {
                                                    cellDetail.VerticalAlignment = VerticalAlignment.Bottom;
                                                }
                                                else if (model.TextBoxProperties.VerticalAlignment == DOM.VerticalAlign.Middle)
                                                {
                                                    cellDetail.VerticalAlignment = VerticalAlignment.Center;
                                                }
                                                else if (model.TextBoxProperties.VerticalAlignment == DOM.VerticalAlign.Top)
                                                {
                                                    cellDetail.VerticalAlignment = VerticalAlignment.Top;
                                                }
                                            }

                                            cellDetail.CellValue = runVal.Text;

                                            if (runVal.Style != null)
                                            {
                                                cellDetail.Foreground = brushConvertor.ConvertFromInvariantString(runVal.Style.TextColor);
                                                cellDetail.Font.FontFamily = new Windows.UI.Xaml.Media.FontFamily(runVal.Style.Font.FontFamily);
                                                cellDetail.Font.FontSize = runVal.Style.Font.FontSize;

                                                if (runVal.Style.Font.FontStyle != DOM.FontStyle.Default)
                                                {
                                                    cellDetail.Font.FontStyle = new ReportingFontStyleConverter().ConvertFromInvariantString(runVal.Style.Font.FontStyle.ToString());
                                                }
                                                if (runVal.Style.Font.FontWeight != DOM.FontWeight.Default)
                                                {
                                                    cellDetail.Font.FontWeight = new ReportingFontWeightConverter().ConvertFromInvariantString(runVal.Style.Font.FontWeight.ToString());
                                                }
                                            }

                                            if (expVal.BackGroundColor != null)
                                            {
                                                cellDetail.Background = brushConvertor.ConvertFromInvariantString(expVal.BackGroundColor);
                                            }
                                        }
                                        else
                                        {
                                            cellDetail.CellType = "DataBoundTemplate";
                                            cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";
                                            ReportingTextbox rtbx = new ReportingTextbox(cellInfo.ItemModel);
                                            rtbx.TablixRow = pageInfo.RowIndices[row - 1] - 1;
                                            rtbx.TablixColumn = column + (columnStart - 1) - 1;
                                            cellInfo.Control = rtbx;
                                            cellDetail.Enabled = true;
                                            this.ApplyStyle(cellInfo, cellDetail);
                                        }
                                    }
                                    else if (cellInfo.ItemModel.ModelType == ModelType.ImageModel)
                                    {
                                        cellDetail.CellType = "DataBoundTemplate";
                                        cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";
                                        ReportingImage image = new ReportingImage(cellInfo.ItemModel);
                                        cellInfo.Control = image;
                                    }
                                    else if (cellInfo.ItemModel.ModelType == ModelType.RectangleModel)
                                    {
                                        cellDetail.CellType = "DataBoundTemplate";
                                        cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";
                                        ReportingRectangle rect = new ReportingRectangle(cellInfo.ItemModel);
                                        cellInfo.Control = rect;
                                    }
                                    else if (cellInfo.ItemModel.ModelType == ModelType.TablixModel)
                                    {
                                        cellDetail.CellType = "DataBoundTemplate";
                                        cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";
                                        ReportingTablixControl dataGrid = new ReportingTablixControl(cellInfo.ItemModel);
                                        dataGrid.CurrentPage = dataGrid.GetPage(this.currentPage);
                                        dataGrid.UpdatePage();
                                        cellInfo.Control = dataGrid;
                                    }
                                    else if (cellInfo.ItemModel.ModelType == ModelType.ChartModel)
                                    {
                                        ChartModel model = cellInfo.ItemModel as ChartModel;
                                        if (model.ChartProperties != null && model.ChartProperties.Border != null && model.ChartProperties.Border.Default != null)
                                        {
                                            this.ApplyBorders(model.ChartProperties.Border, brushConvertor, cellDetail);
                                        }
                                        cellDetail.CellType = "DataBoundTemplate";
                                        cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";
                                        ReportingChartControl chart = new ReportingChartControl(cellInfo.ItemModel);
                                        cellInfo.Control = chart;
                                    }
                                    else if (cellInfo.ItemModel.ModelType == ModelType.LineModel)
                                    {
                                        cellDetail.CellType = "DataBoundTemplate";
                                        cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";
                                        ReportingLine line = new ReportingLine(cellInfo.ItemModel);
                                        cellInfo.Control = line;
                                    }
                                    else if (cellInfo.ItemModel.ModelType == ModelType.GaugeModel)
                                    {
                                        cellDetail.CellType = "DataBoundTemplate";
                                        cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";
                                        GaugeModel model = cellInfo.ItemModel as GaugeModel;
                                        if (model.GaugePanelProperties != null && model.GaugePanelProperties.Border != null && model.GaugePanelProperties.Border.Default != null)
                                        {
                                            this.ApplyBorders(model.GaugePanelProperties.Border, brushConvertor, cellDetail);
                                        }

                                        ReportingGauge gauge = new ReportingGauge(cellInfo.ItemModel);
                                        cellInfo.Control = gauge;
                                    }
                                    else if (cellInfo.ItemModel.ModelType == ModelType.MapModel)
                                    {
                                        MapModel model = cellInfo.ItemModel as MapModel;
                                        if (model.MapProperties != null && model.MapProperties.Border != null && model.MapProperties.Border.Default != null)
                                        {
                                            this.ApplyBorders(model.MapProperties.Border, brushConvertor, cellDetail);
                                        }
                                        cellDetail.CellType = "DataBoundTemplate";
                                        cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";
                                        ReportingMap map = new ReportingMap(cellInfo.ItemModel);
                                        cellInfo.Control = map;                                        
                                    }
                                }

                                if (cellInfo != null && cellInfo.Control != null)
                                {
                                    cellDetail.CellValue = cellInfo.Control;
                                }

                                cellDetail.Enabled = false;

                                if (cellInfo != null)
                                {
                                    if (cellInfo.ItemModel.ModelType == ModelType.TextBoxModel && cellDetail.CellType == "DataBoundTemplate")
                                    {
                                        cellDetail.Enabled = true;
                                    }

                                    if (cellInfo.ItemModel.ModelType != ModelType.TablixModel)
                                    {
                                        cellInfo.DisposeEvalObjects();
                                    }
                                    cellInfo.Control = null;
                                }
                            }
                        }
                    }
                    SetCoveredRange();
                }
            }
        }

        ReportingBrushConverter brushConvertor = new ReportingBrushConverter();

        internal int GetPage(int page)
        {
            return this.TablixModel.PrintPageInfo.BelongsTo[page];
        }

        int GetIndexPos(int[] indicis, int element)
        {
            int pos = Array.IndexOf(indicis, element);
            if (pos == -1)
            {
                return IsheaderSequential(indicis);
            }
            return pos;
        }

        int IsheaderSequential(int[] indicis)
        {
            if (this.TablixModel.RepeatHeaderIndexes.ContainsKey(indicis[0] - 1))
            {
                int sqno = indicis[0];
                for (int i = 1; i < indicis.Length - 2; i++)
                {
                    sqno++;
                    if (!this.TablixModel.RepeatHeaderIndexes.ContainsKey(indicis[i] - 1))
                    {
                        if (sqno == indicis[i])
                        {
                            return 0;
                        }
                        return i;
                    }
                }
            }
            return 0;
        }

        int GetBottomIndexPos(int[] indicis, int element)
        {
            int pos = Array.IndexOf(indicis, element);
            if (pos == -1)
            {
                var coveredInfo = indicis.Where(t => t < element);
                if (coveredInfo.Count() > 0)
                {
                    int index = Array.IndexOf(indicis, coveredInfo.Last());
                    return index;
                }
            }
            return pos;
        }

        void SetCoveredRange()
        {
            ReportingBrushConverter brushConverter = new ReportingBrushConverter();
            var pageInfo = this.PageSizes[this.currentPage];
            var startLeft = pageInfo.ColumnIndices.First();
            var endLeft = pageInfo.ColumnIndices.Last();
            var originalTop = pageInfo.RowIndices.First();
            var originalbottom = pageInfo.RowIndices.Last();
            var startTop = pageInfo.RowIndices[this.IsheaderSequential(pageInfo.RowIndices)];
            var endTop = originalbottom;
            var endCol = this.Model.ColumnCount - 1;
            var endRow = this.Model.RowCount - 1;

            var coveredRange = this.TablixModel.CoveredRanges.Where(range => ((range.Left + 1 >= startLeft && range.Left + 1 <= endLeft && pageInfo.ColumnIndices.Contains(range.Left + 1) || (startLeft > range.Left + 1 && endLeft < range.Right + 1)) || ((range.Right + 1 <= endLeft && range.Right + 1 >= startLeft && pageInfo.ColumnIndices.Contains(range.Right + 1)) && range.Left + 1 < startLeft)) && ((range.Top + 1 >= startTop && range.Top + 1 <= endTop && pageInfo.RowIndices.Contains(range.Top + 1)) || ((range.Bottom + 1 <= endTop && range.Bottom + 1 >= startTop && pageInfo.RowIndices.Contains(range.Bottom + 1)) && range.Top + 1 < startTop)) || (startTop > range.Top + 1 && endTop < range.Bottom + 1)).ToList();

            if (this.TablixModel.DrillSpanRange != null && this.TablixModel.DrillSpanRange.Count > 0 && coveredRange.Count() > 0)
            {
                var drillspanrange = this.TablixModel.DrillSpanRange.Where(range => ((range.Left + 1 >= startLeft && range.Left + 1 <= endLeft && pageInfo.ColumnIndices.Contains(range.Left + 1) || (startLeft > range.Left + 1 && endLeft < range.Right + 1)) || ((range.Right + 1 <= endLeft && range.Right + 1 >= startLeft && pageInfo.ColumnIndices.Contains(range.Right + 1)) && range.Left + 1 < startLeft)) && ((range.Top + 1 >= startTop && range.Top + 1 <= endTop && pageInfo.RowIndices.Contains(range.Top + 1)) || ((range.Bottom + 1 <= endTop && range.Bottom + 1 >= startTop && pageInfo.RowIndices.Contains(range.Bottom + 1)) && range.Top + 1 < startTop)) || (startTop > range.Top + 1 && endTop < range.Bottom + 1));
                if (drillspanrange.Count() > 0)
                {
                    coveredRange.AddRange(drillspanrange);
                }
            }

            foreach (CoveredCellRange range in coveredRange)
            {
                int top = (range.Top + 1 < originalTop) ? 1 : (this.GetIndexPos(pageInfo.RowIndices, (range.Top + 1))) + 1;
                int right = ((range.Right + 1) <= endLeft) ? (Array.IndexOf(pageInfo.ColumnIndices, (range.Right + 1))) + 1 : endCol;
                int left = (range.Left + 1 < startLeft) ? 1 : (Array.IndexOf(pageInfo.ColumnIndices, (range.Left + 1))) + 1;
                int bottom = ((range.Bottom + 1) <= originalbottom) ? (this.GetBottomIndexPos(pageInfo.RowIndices, (range.Bottom + 1))) + 1 : endRow;
                if (top == bottom && left == right)
                {
                    this.ApplyBorders(this.TablixModel.Data[range.Top][range.Left].Border, brushConverter, this.Model[top, left]);
                    this.ApplyMergeStyle(this.TablixModel.Data[range.Top][range.Left], this.Model[top, left]);
                }
                else if (top <= bottom && left <= right)
                {
                    bool isLeft = pageInfo.ColumnIndices.Contains(range.Left + 1);
                    bool isTop = pageInfo.RowIndices.Contains(range.Top + 1);
                    bool isColumnCover = !(isLeft && pageInfo.ColumnIndices.Contains(range.Right + 1));
                    bool isRowCover = !(isTop && pageInfo.RowIndices.Contains(range.Bottom + 1));
                    if ((isColumnCover && !isLeft) || (isRowCover && !isTop))
                    {
                        this.ApplyBorders(this.TablixModel.Data[range.Top][range.Left].Border, brushConverter, this.Model[top, left]);
                        this.ApplyMergeStyle(this.TablixModel.Data[range.Top][range.Left], this.Model[top, left]);
                    }
                    this.CoveredCells.Add(new CoveredCellInfo(top, left, bottom, right));
                }
            }
        }

        private void ApplyStyle(TablixCellInfo cellInfo, GridStyleInfo cellDetail)
        {
            TextboxModel model = cellInfo.ItemModel as TextboxModel;
            ParagraphExpval para = model.ParaExpval.First();
            TextRunExpval runVal = para.Runs.First();

            cellDetail.TextTrimming = TextTrimming.None;
            cellDetail.TextWrapping = TextWrapping.Wrap;

            if (para.TextAlignment == "Left")
            {
                cellDetail.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
            }
            else if (para.TextAlignment == "Right")
            {
                cellDetail.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
            }
            else if (para.TextAlignment == "Center")
            {
                cellDetail.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            }

            var expVal = model.TextBoxProperties;

            if (expVal.Padding != null)
            {
                cellDetail.BorderMargins.Left = expVal.Padding.Left;
                cellDetail.BorderMargins.Right = expVal.Padding.Right;
                cellDetail.BorderMargins.Top = expVal.Padding.Top;
                cellDetail.BorderMargins.Bottom = expVal.Padding.Bottom;

                cellDetail.TextMargins.Bottom = 0;
                cellDetail.TextMargins.Top = 0;
                cellDetail.TextMargins.Left = 0;
                cellDetail.TextMargins.Right = 0;
            }

            if (model.TextBoxProperties != null)
            {
                if (model.TextBoxProperties.VerticalAlignment == DOM.VerticalAlign.Bottom)
                {
                    cellDetail.VerticalAlignment = VerticalAlignment.Bottom;
                }
                else if (model.TextBoxProperties.VerticalAlignment == DOM.VerticalAlign.Middle)
                {
                    cellDetail.VerticalAlignment = VerticalAlignment.Center;
                }
                else if (model.TextBoxProperties.VerticalAlignment == DOM.VerticalAlign.Top)
                {
                    cellDetail.VerticalAlignment = VerticalAlignment.Top;
                }
            }

            cellDetail.CellValue = runVal.Text;

            if (runVal.Style != null)
            {
                cellDetail.Foreground = brushConvertor.ConvertFromInvariantString(runVal.Style.TextColor);
                cellDetail.Font.FontFamily = new Windows.UI.Xaml.Media.FontFamily(runVal.Style.Font.FontFamily);
                cellDetail.Font.FontSize = runVal.Style.Font.FontSize;

                if (runVal.Style.Font.FontStyle != DOM.FontStyle.Default)
                {
                    cellDetail.Font.FontStyle = new ReportingFontStyleConverter().ConvertFromInvariantString(runVal.Style.Font.FontStyle.ToString());
                }
                if (runVal.Style.Font.FontWeight != DOM.FontWeight.Default)
                {
                    cellDetail.Font.FontWeight = new ReportingFontWeightConverter().ConvertFromInvariantString(runVal.Style.Font.FontWeight.ToString());
                }
            }

            if (expVal.BackGroundColor != null)
            {
                cellDetail.Background = brushConvertor.ConvertFromInvariantString(expVal.BackGroundColor);
            }
        }

        private void ApplyMergeStyle(TablixCellInfo cellInfo, GridStyleInfo cellDetail)
        {
            if (cellInfo.ItemModel.ModelType == ModelType.TextBoxModel && !(cellDetail.CellValue is ReportingTextbox))
            {
                if (cellInfo != null && this.TablixModel.Model.EnableVirtualEvaluation)
                {
                    cellInfo.Evaluate();
                }
                this.ApplyStyle(cellInfo, cellDetail);
                if (cellInfo != null && this.TablixModel.Model.EnableVirtualEvaluation)
                {
                    cellInfo.DisposeEvalObjects();
                }
            }
        }

        private void ApplyBorders(BorderExpval border, ReportingBrushConverter brushConverter, GridStyleInfo cellDetail)
        {
            if (border.Default != null && border.Default.BorderStyle != DOM.BorderStyles.None && border.Default.BorderStyle != DOM.BorderStyles.Default)
            {
                cellDetail.Borders.All = new WinRT.Controls.Pen(brushConvertor.ConvertFromString(border.Default.BorderBrush), border.Default.Thickness);
            }

            if (border.RightBorder != null && border.RightBorder.BorderStyle != DOM.BorderStyles.None && border.RightBorder.BorderStyle != DOM.BorderStyles.Default)
            {
                cellDetail.Borders.Right = new WinRT.Controls.Pen(brushConvertor.ConvertFromString(border.RightBorder.BorderBrush), border.RightBorder.Thickness);
            }

            if (border.LeftBorder != null && border.LeftBorder.BorderStyle != DOM.BorderStyles.None && border.LeftBorder.BorderStyle != DOM.BorderStyles.Default)
            {
                cellDetail.Borders.Left = new WinRT.Controls.Pen(brushConvertor.ConvertFromString(border.LeftBorder.BorderBrush), border.LeftBorder.Thickness);
            }

            if (border.TopBorder != null && border.TopBorder.BorderStyle != DOM.BorderStyles.None && border.TopBorder.BorderStyle != DOM.BorderStyles.Default)
            {
                cellDetail.Borders.Top = new WinRT.Controls.Pen(brushConvertor.ConvertFromString(border.TopBorder.BorderBrush), border.TopBorder.Thickness);
            }

            if (border.BottomBorder != null && border.BottomBorder.BorderStyle != DOM.BorderStyles.None && border.BottomBorder.BorderStyle != DOM.BorderStyles.Default)
            {
                cellDetail.Borders.Bottom = new WinRT.Controls.Pen(brushConvertor.ConvertFromString(border.BottomBorder.BorderBrush), border.BottomBorder.Thickness);
            }
        }

    }
}
