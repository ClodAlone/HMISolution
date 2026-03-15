//-------------------------------------------------------------------------------------------------
// <copyright file="PagingGrid.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------


namespace Syncfusion.RDL.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using Syncfusion.Linq;
    using Syncfusion.Windows.Controls.Grid;
    using System.Collections;
    using System.Windows.Media;
    using Syncfusion.Windows.Controls.Cells;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Syncfusion.RDL.ItemModel;
    using Syncfusion.RDL.Data;
    using Syncfusion.RDL.Internal;
    using Syncfusion.Windows;
    using Syncfusion.RDL.Layout;

    internal class ReportingTablixControl
        : GridControl
    {
        int currentPage = -1;

        internal TablixModel TablixModel
        {
            get;
            set;
        }

        internal bool IsPrintMode
        {
            get;
            set;
        }

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

        internal Dictionary<int, TablixPageInfo> PageSizes
        {
            get
            {
                if (this.IsPrintMode)
                {
                    return this.TablixModel.PrintPageSizes;
                }

                return this.TablixModel.PageSizes;
            }
        }

        internal new int PageCount
        {
            get
            {
                if (this.IsPrintMode)
                {
                    return this.TablixModel.PrintPageSizes.Count;
                }
                else
                {
                    return this.TablixModel.PageSizes.Count;
                }
            }
        }

        private int RowStart { get; set; }

        private int ColumnStart { get; set; }

        private TablixPageInfo PageInfo { get; set; }

        public ReportingTablixControl(IReportItemModeler pageContent)
        {
#if SILVERLIGHT
            this.Model.CellModels.Add("DataBoundTemplate", new DataTemplateCellModel());
#else
            this.Model.CellModels.Add("CustomDataTemplate", new DataTemplateCellModel());
#endif
            this.Model.Options.ShowCurrentCell = false;
            this.Model.Options.ListBoxSelectionMode = GridSelectionMode.None;
            this.Model.Options.AllowSelection = GridSelectionFlags.None;
            this.TablixModel = pageContent as TablixModel; 
            this.VerticalPixelScroll = false;
            this.HorizontalPixelScroll = false;
            this.IsEnabled = true;
#if !SILVERLIGHT
            this.Model.RowCount = 0;
            this.Model.ColumnCount = 0;
#endif         
            this.Visibility = pageContent.Hidden ? Visibility.Collapsed : Visibility.Visible;

#if !SILVERLIGHT
            this.PreviewMouseWheel += ReportingTablixControl_PreviewMouseWheel;
#else
            this.MouseWheel += ReportingTablixControl_PreviewMouseWheel;
#endif
        }

        void ReportingTablixControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.TablixModel != null && this.TablixModel.Model != null)
            {
                this.TablixModel.Model.RaiseMouseScrolling(sender, e);
            }
        }

        void UpdateCellValues()
        {
            for (int row = 1; row < this.Model.RowCount; row++)
            {
                for (int column = 1; column < this.Model.ColumnCount; column++)
                {
                    var cellInfo = TablixModel.Data[PageInfo.RowIndices[row - 1] - 1][column + (ColumnStart - 1) - 1];
                    this.RenderCellModels(this.Model[row, column], row, column, cellInfo);
                }
            }
        }

        bool GetActionInfo(IReportItemModeler model)
        {
            var txtmodel = model as TextboxModel;
            bool isAction = false;
            if (txtmodel.TextBoxProperties != null)
            {
                if (txtmodel.TextBoxProperties.TextboxActionInfo != null)
                {
                    isAction = true;
                }
            }
            if (txtmodel.IsDrillAction)
            {
                isAction = txtmodel.IsDrillAction;
            }
            return isAction;
        }

        void RenderCellModels(GridStyleInfo cellDetail, int row, int column, TablixCellInfo cellInfo)
        {
            ReportingBrushConverter brushConvertor = new ReportingBrushConverter();

            if (cellInfo != null && cellInfo.ItemModel != null && this.TablixModel.Model.EnableVirtualEvaluation)
            {
                cellInfo.CurrentKey = new List<int>();
                cellInfo.CurrentKey.Add(PageInfo.RowIndices[row - 1] - 1);
                cellInfo.CurrentKey.Add(column + (ColumnStart - 1) - 1);

                if (cellInfo.ItemModel.ModelType != ModelType.TablixModel)
                {
                    cellInfo.Evaluate();
                }
            }

            cellDetail.Borders.All = new Pen(new SolidColorBrush(Colors.Transparent), 1);

            if (cellInfo != null && cellInfo.Border != null)
            {
                var border = cellInfo.Border;
                this.ApplyBorders(border, brushConvertor, cellDetail);
            }

            if (cellInfo != null && cellInfo.ItemModel != null)
            {
                if (cellInfo.ItemModel.ModelType == ModelType.TextBoxModel)
                {
                    bool isAction = this.GetActionInfo(cellInfo.ItemModel);
                    bool isToggle = (cellInfo.ItemModel as TextboxModel).ToggleGroups == null;
                    if (isToggle && !isAction || IsPrintMode)
                    {
                        cellDetail.CellType = "TextBox";
                        TextboxModel model = cellInfo.ItemModel as TextboxModel;
                        if (model.ParaExpval != null && model.ParaExpval.Count > 0)
                        {
                            ParagraphExpval para = model.ParaExpval.First();
                            TextRunExpval runVal = para.Runs.First();

                            cellDetail.TextTrimming = TextTrimming.None;
                            cellDetail.TextWrapping = TextWrapping.Wrap;
                            cellDetail.Padding = new CellMarginsInfo(0, para.SpaceBefore, 0, para.SpaceAfter);

                            if (para.TextAlignment == "Left")
                            {
                                cellDetail.HorizontalAlignment = HorizontalAlignment.Left;
                            }
                            else if (para.TextAlignment == "Right")
                            {
                                cellDetail.HorizontalAlignment = HorizontalAlignment.Right;
                            }
                            else if (para.TextAlignment == "Center")
                            {
                                cellDetail.HorizontalAlignment = HorizontalAlignment.Center;
                            }

                            if (model.TextBoxProperties.VerticalAlignment != DOM.VerticalAlign.Default)
                            {
                                if (model.TextBoxProperties.VerticalAlignment == DOM.VerticalAlign.Bottom)
                                {
                                    cellDetail.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                                }
                                else if (model.TextBoxProperties.VerticalAlignment == DOM.VerticalAlign.Middle)
                                {
                                    cellDetail.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                }
                                else if (model.TextBoxProperties.VerticalAlignment == DOM.VerticalAlign.Top)
                                {
                                    cellDetail.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                                }
                            }

                            var expVal = model.TextBoxProperties;

                            if (expVal.Padding != null)
                            {
                                cellDetail.BorderMargins.Left = expVal.Padding.Left / 2;
                                cellDetail.BorderMargins.Right = expVal.Padding.Right / 2;
                                //cellDetail.BorderMargins.Top = expVal.Padding.Top;
                                //cellDetail.BorderMargins.Bottom = expVal.Padding.Bottom;
                            }

                            cellDetail.Text = runVal.Text;

                            if (runVal.Style != null)
                            {
                                cellDetail.Foreground = brushConvertor.ConvertFromInvariantString(runVal.Style.TextColor);
                                cellDetail.Font.FontFamily = new FontFamily(runVal.Style.Font.FontFamily);
                                cellDetail.Font.FontSize = runVal.Style.Font.FontSize;

                                if (runVal.Style.Font.FontStyle != DOM.FontStyle.Default)
                                {
                                    cellDetail.Font.FontStyle = new ReportingFontStyleConverter().ConvertFromInvariantString(runVal.Style.Font.FontStyle.ToString());
                                }
                                if (runVal.Style.Font.FontWeight != DOM.FontWeight.Default)
                                {
                                    cellDetail.Font.FontWeight = new ReportingFontWeightConverter().ConvertFromInvariantString(runVal.Style.Font.FontWeight.ToString());
                                }
#if !SILVERLIGHT && !WINRT
                                if (runVal.Style.TextDecoration != null && runVal.Style.TextDecoration.ToLower() != "default"
                                    && runVal.Style.TextDecoration.ToLower() != "none")
                                {
                                    TextDecorationCollection txtdecr = new TextDecorationCollection();
                                    string decoration = runVal.Style.TextDecoration.ToLower();

                                    if (decoration == "underline")
                                    {
                                        txtdecr.Add(TextDecorations.Underline);
                                    }
                                    else if (decoration == "baseline")
                                    {
                                        txtdecr.Add(TextDecorations.Baseline);
                                    }
                                    else if (decoration == "overline")
                                    {
                                        txtdecr.Add(TextDecorations.OverLine);
                                    }
                                    else if (decoration == "strikethrough" || decoration == "linethrough")
                                    {
                                        txtdecr.Add(TextDecorations.Strikethrough);
                                    }

                                    cellDetail.Font.TextDecorations = txtdecr;
                                }
#endif
                            }
                            if (expVal.BackGroundColor != null)
                            {
                                cellDetail.Background = brushConvertor.ConvertFromInvariantString(expVal.BackGroundColor);
                            }
                        }
                        cellInfo.Control = null;
                    }
                    else
                    {
#if SILVERLIGHT
                        cellDetail.CellType = "DataBoundTemplate";
                        cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";
#else
                        cellDetail.CellType = "CustomDataTemplate";
#endif
                        ReportingPageControl rtb = new ReportingPageControl(cellInfo.ItemModel, !isToggle, IsPrintMode);
                        
                        if (isAction)
                        {
                           rtb.ControlEvents();                            
                        }

                        if (!isToggle)
                        {
                            rtb.TablixRow = PageInfo.RowIndices[row - 1] - 1;
                            rtb.TablixCol = column + (ColumnStart - 1) - 1;                            
                        }

                        (rtb.ConetentControl as RichTextBox).BorderThickness = new Thickness(0);
                        (rtb.ConetentControl as RichTextBox).Padding = new Thickness(0);
                        (rtb.ConetentControl as RichTextBox).Margin = new Thickness(0);
                        (rtb.ConetentControl as RichTextBox).Height = rtb.Height;
                        
                        rtb.CurrentPage = 0;
                        cellInfo.Control = rtb;
                        this.ApplyStyle(cellInfo, cellDetail);
                    }
                }
                else if (cellInfo.ItemModel.ModelType == ModelType.ImageModel)
                {
#if SILVERLIGHT
                    cellDetail.CellType = "DataBoundTemplate";
                    cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";
#else
                    cellDetail.CellType = "CustomDataTemplate";
#endif
                    ReportingImage image = new ReportingImage(cellInfo.ItemModel);
                    cellInfo.Control = image;
                }
                else if (cellInfo.ItemModel.ModelType == ModelType.RectangleModel)
                {
#if SILVERLIGHT
                    cellDetail.CellType = "DataBoundTemplate";
                    cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";
#else
                    cellDetail.CellType = "CustomDataTemplate";
#endif
                    ReportingRectangle dataRect = new ReportingRectangle(cellInfo.ItemModel, this.IsPrintMode, this.CurrentPage);
                    dataRect.IsPrintMode = this.IsPrintMode;
                    if (!this.TablixModel.Model.EnableVirtualEvaluation)
                    {
                        try{
                            dataRect.CurrentPage = this.TablixModel.RepeatHeaderIndexes.ContainsKey(PageInfo.RowIndices[row - 1] - 1) ? 0 : dataRect.GetPage(this.CurrentPage);
                        }
                        catch
                        {
                            dataRect.CurrentPage = 0;
                        }
                    }
                    dataRect.InnerReportItems();
                    cellInfo.Control = dataRect;
#if SILVERLIGHT
                    try
                    {
                      (dataRect.RectangleBodyContent.Children[0] as Border).BorderThickness = new Thickness(0);
                    }
                    catch {}

#else
                    try
                    {
                      (dataRect.RectangleBodyContent.Children[0] as DashStyleBorder).BorderThickness = new Thickness(0);
                    }
                    catch {}
#endif
                }
                else if (cellInfo.ItemModel.ModelType == ModelType.TablixModel)
                {
#if SILVERLIGHT
                    cellDetail.CellType = "DataBoundTemplate";
                    cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";
#else
                    cellDetail.CellType = "CustomDataTemplate";
#endif
                    var model = (cellInfo.ItemModel as TablixModel);

                    if (this.TablixModel.Model.EnableVirtualEvaluation)
                    {
                        TablixEvaluationItems items = cellInfo.Rows[PageInfo.RowIndices[row - 1] - 1].TablixValues[column + (ColumnStart - 1) - 1][model.Name];
                        model.UpdateTablixValue(model, items);
                    }

                    ReportingTablixControl dataGrid = new ReportingTablixControl(cellInfo.ItemModel);
                    dataGrid.IsPrintMode = this.IsPrintMode;
                    dataGrid.CurrentPage = dataGrid.GetPage(this.CurrentPage);
                    dataGrid.UpdatePage();
                    cellInfo.Control = dataGrid;
                }
                else if (cellInfo.ItemModel.ModelType == ModelType.ChartModel)
                {
#if SILVERLIGHT
                    cellDetail.CellType = "DataBoundTemplate";
                    cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";
#else
                    cellDetail.CellType = "CustomDataTemplate";
#endif
                    ChartModel model = cellInfo.ItemModel as ChartModel;
                    if (model.ChartProperties != null && model.ChartProperties.Border != null && model.ChartProperties.Border.Default != null)
                    {
                        this.ApplyBorders(model.ChartProperties.Border, brushConvertor, cellDetail);
                    }
                  
                    ReportingChartControl chart = new ReportingChartControl(cellInfo.ItemModel);
                    cellInfo.Control = chart;
                }
                else if (cellInfo.ItemModel.ModelType == ModelType.LineModel)
                {
#if SILVERLIGHT
                    cellDetail.CellType = "DataBoundTemplate";
                    cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";
#else
                    cellDetail.CellType = "CustomDataTemplate";
#endif
                    ReportingLine line = new ReportingLine(cellInfo.ItemModel);
                    cellInfo.Control = line;
                }
                else if (cellInfo.ItemModel.ModelType == ModelType.GaugeModel)
                {
#if SILVERLIGHT
                    cellDetail.CellType = "DataBoundTemplate";
                    cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";
#else
                    cellDetail.CellType = "CustomDataTemplate";
#endif
                    GaugeModel model = cellInfo.ItemModel as GaugeModel;

                    if (model.GaugePanelProperties != null && model.GaugePanelProperties.Border != null && model.GaugePanelProperties.Border.Default != null)
                    {
                        this.ApplyBorders(model.GaugePanelProperties.Border, brushConvertor, cellDetail);
                    }
                    ReportingGauge gauge = new ReportingGauge(cellInfo.ItemModel);
                    cellInfo.Control = gauge;

                }
#if !SyncfusionFramework3_5 && !SILVERLIGHT && !WINRT
                else if (cellInfo.ItemModel.ModelType == ModelType.MapModel)
                {
#if SILVERLIGHT
                    cellDetail.CellType = "DataBoundTemplate";
                    cellDetail.CellItemTemplateKey = "ReportViewerCellTeamplate";
#else
                    cellDetail.CellType = "CustomDataTemplate";
#endif
                    MapModel model = cellInfo.ItemModel as MapModel;
                    if (model.MapProperties != null && model.MapProperties.Border != null && model.MapProperties.Border.Default != null)
                    {
                        this.ApplyBorders(model.MapProperties.Border, brushConvertor, cellDetail);
                    }
                    ReportingMap mapControl = new ReportingMap(cellInfo.ItemModel);
                    cellInfo.Control = mapControl;
                }
#endif
            }

            if (cellInfo != null && cellInfo.Control != null)
            {
                cellDetail.CellValue = cellInfo.Control;             
            }

            cellDetail.Enabled = false;

            if (cellInfo != null && cellInfo.ItemModel != null)
            {
                if (cellInfo.ItemModel.ModelType == ModelType.TextBoxModel && (cellDetail.CellType == "CustomDataTemplate" || cellDetail.CellType == "DataBoundTemplate"))
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

        internal void UpdatePage()
        {
            this.SetCellValues();
            if (this.IsPrintMode)
            {
                if (this.TablixModel.IsTablixChild && this.TablixModel.Model.EnableVirtualEvaluation)
                {
                    this.Margin = new Thickness(this.TablixModel.Left, this.currentPage == 0 ? this.TablixModel.Top : 0, 0, 0);
                }
                else
                {
                    if (this.TablixModel.ContainerModel != null && this.TablixModel.ContainerModel.ModelType == ModelType.RectangleModel)
                    {
                        this.Margin = new Thickness(this.currentPage % this.TablixModel.PrintPageColumnCount == 0 ? this.TablixModel.PrintPageInfo.ActualLeft : 0, this.currentPage < this.TablixModel.PrintPageColumnCount ? this.TablixModel.PrintPageInfo.ActualTop : 1.2, 0, 0);                                                
                    }
                    else
                    {
                        this.Margin = new Thickness(this.currentPage % this.TablixModel.PrintPageColumnCount == 0 ? this.TablixModel.PrintPageInfo.ActualLeft : 0, this.currentPage < this.TablixModel.PrintPageColumnCount ? this.TablixModel.PrintPageInfo.ActualTop : 0, 0, 0);                        
                    }
                }
            }
            else
            {
                if (this.TablixModel.IsTablixChild && this.TablixModel.Model.EnableVirtualEvaluation)
                {
                    this.Margin = new Thickness(this.TablixModel.Left, this.currentPage == 0 ? this.TablixModel.Top : 0, 0, 0);
                }
                else
                {
                    if (this.TablixModel.ContainerModel != null &&
                        this.TablixModel.ContainerModel.ModelType == ModelType.RectangleModel)
                    {
                        this.Margin = new Thickness(this.TablixModel.PageInfo.ActualLeft, this.currentPage == 0 ? this.TablixModel.PageInfo.ActualTop : 1.2, 0, 0);
                    }
                    else
                    {
                        this.Margin = new Thickness(this.TablixModel.PageInfo.ActualLeft, this.currentPage == 0 ? this.TablixModel.PageInfo.ActualTop : 0, 0, 0);                        
                    }
                }
            }
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
            if (this.TablixModel.RepeatHeaderIndexes != null && this.TablixModel.RepeatHeaderIndexes.ContainsKey(indicis[0] - 1))
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

            var coveredRange = this.TablixModel.CoveredRanges.Where(range => ((range.Left + 1 >= startLeft && range.Left + 1 <= endLeft && pageInfo.ColumnIndices.Contains(range.Left + 1) || (startLeft > range.Left + 1 && endLeft < range.Right + 1)) || ((range.Right + 1 <= endLeft && range.Right + 1 >= startLeft && pageInfo.ColumnIndices.Contains(range.Right + 1)) && range.Left + 1 < startLeft)) && ((range.Top + 1 >= startTop && range.Top + 1 <= endTop && pageInfo.RowIndices.Contains(range.Top + 1)) || ((range.Bottom + 1 <= endTop && range.Bottom + 1 >= startTop && (pageInfo.RowIndices.Contains(range.Bottom + 1) || (endTop >= range.Bottom + 1))) && range.Top + 1 < startTop)) || (startTop > range.Top + 1 && endTop < range.Bottom + 1) || (pageInfo.RowIndices.Contains(range.Top + 1) && pageInfo.ColumnIndices.Contains(range.Left + 1)) || (pageInfo.RowIndices.Contains(range.Bottom + 1) && pageInfo.ColumnIndices.Contains(range.Right + 1))).ToList();

            if (this.TablixModel.DrillSpanRange != null && this.TablixModel.DrillSpanRange.Count > 0 && coveredRange.Count()>0)
            {
                var drillspanrange = this.TablixModel.DrillSpanRange.Where(range => ((range.Left + 1 >= startLeft && range.Left + 1 <= endLeft && pageInfo.ColumnIndices.Contains(range.Left + 1) || (startLeft > range.Left + 1 && endLeft < range.Right + 1)) || ((range.Right + 1 <= endLeft && range.Right + 1 >= startLeft && pageInfo.ColumnIndices.Contains(range.Right + 1)) && range.Left + 1 < startLeft)) && ((range.Top + 1 >= startTop && range.Top + 1 <= endTop && pageInfo.RowIndices.Contains(range.Top + 1)) || ((range.Bottom + 1 <= endTop && range.Bottom + 1 >= startTop && (pageInfo.RowIndices.Contains(range.Bottom + 1) || (endTop >= range.Bottom + 1))) && range.Top + 1 < startTop)) || (startTop > range.Top + 1 && endTop < range.Bottom + 1));
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
                    var cellInfo = this.TablixModel.Data[range.Top][range.Left];
                    if (this.TablixModel.Model.EnableVirtualEvaluation)
                    {
                        cellInfo.CurrentKey = new List<int>();
                        cellInfo.CurrentKey.Add(range.Top);
                        cellInfo.CurrentKey.Add(range.Left);
                    }
                    this.ApplyMergeStyle(cellInfo, this.Model[top, left]);
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
                        var cellInfo = this.TablixModel.Data[range.Top][range.Left];
                        if (this.TablixModel.Model.EnableVirtualEvaluation)
                        {
                            cellInfo.CurrentKey = new List<int>();
                            cellInfo.CurrentKey.Add(range.Top);
                            cellInfo.CurrentKey.Add(range.Left);
                        }
                        this.ApplyMergeStyle(cellInfo, this.Model[top, left]);
                    }

                    if (this.CoveredCells.Where(cell => cell.Top == top && cell.Left == left && cell.Bottom == bottom && cell.Right == right).Count() == 0)
                        this.CoveredCells.Add(new CoveredCellInfo(top, left, bottom, right));
                }
            }
        }

        void SetCellValues()
        {
            if (this.PageSizes == null)
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }

            if (this.PageSizes[this.currentPage].Height <= 0 || this.PageSizes[this.currentPage].Width <= 0)
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }

            if (this.PageSizes != null && this.CurrentPage >= 0 && this.CurrentPage < this.PageCount)
            {
                this.PageInfo = this.PageSizes[this.currentPage];

                if (PageInfo.RowIndices.Count() > 0 && PageInfo.ColumnIndices.Count() > 0)
                {
                    this.Width = this.PageSizes[this.CurrentPage].Width;
                    this.Height = this.PageSizes[this.CurrentPage].Height;

                    this.Model.RowCount = PageInfo.RowIndices.Count() + 1;
                    this.Model.ColumnCount = PageInfo.ColumnIndices.Count() + 1;

                    this.Model.ColumnWidths[0] = 0;
                    this.Model.RowHeights[0] = 0;

                    //this.RowStart = PageInfo.RowIndices[0];
                    this.ColumnStart = PageInfo.ColumnIndices[0];

                    for (int row = 1; row < this.Model.RowCount; row++)
                    {
                        this.Model.RowHeights[row] = Math.Round(this.TablixModel.RowHeights[PageInfo.RowIndices[row - 1] - 1]);
                    }

                    for (int column = 1; column < this.Model.ColumnCount; column++)
                    {
                        this.Model.ColumnWidths[column] = this.TablixModel.ColumnWights[column + (this.ColumnStart - 1) - 1];
                        if (PageInfo.ColumnWiths != null && PageInfo.ColumnWiths.Count > 0)
                        {
                            var colWidth = from value in PageInfo.ColumnWiths where value.Index == (column + (this.ColumnStart) - 1) select value.Size;
                            if (colWidth.Count() > 0)
                            {
                                this.Model.ColumnWidths[column] = colWidth.First();
                            }
                        }
                    }
                    this.UpdateCellValues();
                    SetCoveredRange();
                }
                else
                {
                    this.Visibility = System.Windows.Visibility.Collapsed;
                    return;
                }
            }

            // Workaround to solve refresing issue.
            if (this.ScrollColumns != null)
            {
                this.ScrollColumns.GetVisibleLines();
            }
        }

        void ApplyBorders(BorderExpval border, ReportingBrushConverter brushConvertor, GridStyleInfo cellDetail)
        {
            if (border != null)
            {
                if (border.Default != null && border.Default.BorderStyle != DOM.BorderStyles.None && border.Default.BorderStyle != DOM.BorderStyles.Default)
                {
                    cellDetail.Borders.All = new Pen(brushConvertor.ConvertFromString(border.Default.BorderBrush), border.Default.Thickness);
                }

                if (border.RightBorder != null && border.RightBorder.BorderStyle != DOM.BorderStyles.None && border.RightBorder.BorderStyle != DOM.BorderStyles.Default)
                {
                    cellDetail.Borders.Right = new Pen(brushConvertor.ConvertFromString(border.RightBorder.BorderBrush), border.RightBorder.Thickness);
                }

                if (border.LeftBorder != null && border.LeftBorder.BorderStyle != DOM.BorderStyles.None && border.LeftBorder.BorderStyle != DOM.BorderStyles.Default)
                {
                    cellDetail.Borders.Left = new Pen(brushConvertor.ConvertFromString(border.LeftBorder.BorderBrush), border.LeftBorder.Thickness);
                }

                if (border.TopBorder != null && border.TopBorder.BorderStyle != DOM.BorderStyles.None && border.TopBorder.BorderStyle != DOM.BorderStyles.Default)
                {
                    cellDetail.Borders.Top = new Pen(brushConvertor.ConvertFromString(border.TopBorder.BorderBrush), border.TopBorder.Thickness);
                }

                if (border.BottomBorder != null && border.BottomBorder.BorderStyle != DOM.BorderStyles.None && border.BottomBorder.BorderStyle != DOM.BorderStyles.Default)
                {
                    cellDetail.Borders.Bottom = new Pen(brushConvertor.ConvertFromString(border.BottomBorder.BorderBrush), border.BottomBorder.Thickness);
                }
            }

            if (this.TablixModel.ReportItem.Style != null && this.TablixModel.ReportItem.Style.Border != null)
            {
                try
                {
                    if (this.TablixModel.ReportItem.Style.Border.Style != null && this.TablixModel.ReportItem.Style.Border.Style != "None")
                    {
                        if (cellDetail.RowIndex == 1)
                        {
                            cellDetail.Borders.Top = new Pen(brushConvertor.ConvertFromString(this.TablixModel.ReportItem.Style.Border.Color), this.TablixModel.ReportItem.Style.Border.Width == null ? 1 : this.TablixModel.ReportItem.Style.Border.Width.PixelValue);
                        }

                        else if (cellDetail.RowIndex == this.Model.RowCount - 1)
                        {
                            cellDetail.Borders.Bottom = new Pen(brushConvertor.ConvertFromString(this.TablixModel.ReportItem.Style.Border.Color), this.TablixModel.ReportItem.Style.Border.Width == null ? 1 : this.TablixModel.ReportItem.Style.Border.Width.PixelValue);
                        }

                        if (cellDetail.ColumnIndex == 1)
                        {
                            cellDetail.Borders.Left = new Pen(brushConvertor.ConvertFromString(this.TablixModel.ReportItem.Style.Border.Color), this.TablixModel.ReportItem.Style.Border.Width == null ? 1 : this.TablixModel.ReportItem.Style.Border.Width.PixelValue);
                        }

                        else if (cellDetail.ColumnIndex == this.Model.ColumnCount - 1)
                        {
                            cellDetail.Borders.Right = new Pen(brushConvertor.ConvertFromString(this.TablixModel.ReportItem.Style.Border.Color), this.TablixModel.ReportItem.Style.Border.Width == null ? 1 : this.TablixModel.ReportItem.Style.Border.Width.PixelValue);
                        }

                    }
                }
                catch
                {

                }
            }
        }

        void ApplyMergeStyle(TablixCellInfo cellInfo, GridStyleInfo cellDetail)
        {
            if (cellInfo.ItemModel.ModelType == ModelType.TextBoxModel && !(cellDetail.CellValue is ReportingPageControl))
            {
                if (cellInfo != null && this.TablixModel.Model.EnableVirtualEvaluation)
                {
                    cellInfo.Evaluate();
                }
                this.ApplyStyle(cellInfo , cellDetail);
                if (cellInfo != null && this.TablixModel.Model.EnableVirtualEvaluation)
                {
                    cellInfo.DisposeEvalObjects();
                }
            }
        }

        void ApplyStyle(TablixCellInfo cellInfo, GridStyleInfo cellDetail)
        {
            TextboxModel model = cellInfo.ItemModel as TextboxModel;
            ParagraphExpval para = model.ParaExpval.First();
            TextRunExpval runVal = para.Runs.First();

            cellDetail.TextTrimming = TextTrimming.None;
            cellDetail.TextWrapping = TextWrapping.Wrap;

            if (para.TextAlignment == "Left")
            {
                cellDetail.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else if (para.TextAlignment == "Right")
            {
                cellDetail.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else if (para.TextAlignment == "Center")
            {
                cellDetail.HorizontalAlignment = HorizontalAlignment.Center;
            }

            if (model.TextBoxProperties.VerticalAlignment != DOM.VerticalAlign.Default)
            {
                if (model.TextBoxProperties.VerticalAlignment == DOM.VerticalAlign.Bottom)
                {
                    cellDetail.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                }
                else if (model.TextBoxProperties.VerticalAlignment == DOM.VerticalAlign.Middle)
                {
                    cellDetail.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                }
                else if (model.TextBoxProperties.VerticalAlignment == DOM.VerticalAlign.Top)
                {
                    cellDetail.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                }
            }

            var expVal = model.TextBoxProperties;

            if (expVal.Padding != null)
            {
                cellDetail.BorderMargins.Left = expVal.Padding.Left / 2;
                cellDetail.BorderMargins.Right = expVal.Padding.Right / 2;
                //cellDetail.BorderMargins.Top = expVal.Padding.Top;
                //cellDetail.BorderMargins.Bottom = expVal.Padding.Bottom;
            }

            cellDetail.CellValue = runVal.Text;

            if (runVal.Style != null)
            {
                cellDetail.Foreground = new ReportingBrushConverter().ConvertFromInvariantString(runVal.Style.TextColor);
                cellDetail.Font.FontFamily = new FontFamily(runVal.Style.Font.FontFamily);
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
                cellDetail.Background = new ReportingBrushConverter().ConvertFromInvariantString(expVal.BackGroundColor);
            }
        }

        internal int GetPage(int page)
        {
            if (this.IsPrintMode)
            {
                return this.TablixModel.PrintPageInfo.BelongsTo[page];
            }

            return this.TablixModel.PageInfo.BelongsTo[page];
        }
    }

#if SILVERLIGHT

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class DataTemplateCellModel : GridCellModel<DataTemplateCellRenderer>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class DataTemplateCellRenderer : GridVirtualizingCellRenderer<ContentControl>
    {
        public DataTemplateCellRenderer()
        {
            IsFocusable = false;
            AllowRecycle = true;
        }

        /// <summary>
        /// Called to initialize the content of the cell
        /// using the information from the cell style (value, text,
        /// behavior etc.). You must override this method in your
        /// derived class.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The cell style info.</param>
        [CLSCompliant(false)]
        public override void OnInitializeContent(ContentControl uiElement, GridRenderStyleInfo style)
        {
            if (uiElement.Content == null)
            {
                uiElement.Content = style.CellValue;
            }
            else
            {

            }
        }

        //public override void CreateRendererElement(ContentControl uiElement, GridRenderStyleInfo style)
        //{
        //    uiElement.Content = style.CellValue;
        //    base.CreateRendererElement(uiElement, style);
        //}
    }

#else

    /// <summary>
    /// 
    /// </summary>
    internal class DataTemplateCellModel : GridCellModel<DataTemplateCellRenderer>
    {
    }

    /// <summary>
    ///
    /// </summary>
    internal class DataTemplateCellRenderer : GridVirtualizingCellRenderer<ContentControl>
    {
        public DataTemplateCellRenderer()
        {
            IsFocusable = true;
            AllowRecycle = true;
        }

        /// <summary>
        /// Called to initialize the content of the cell
        /// using the information from the cell style (value, text,
        /// behavior etc.). You must override this method in your
        /// derived class.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(ContentControl uiElement, GridRenderStyleInfo style)
        {
            //base.OnInitializeContent(uiElement, style);
            //bool found = false;

            //if (style.CellItemTemplateKey != null)
            //{
            //    System.Windows.DataTemplate dt = (System.Windows.DataTemplate)style.GridControl.TryFindResource(style.CellItemTemplateKey);
            //    found = dt != null;
            //    if (found)
            //        uiElement.ContentTemplate = dt;
            //}

            //if (!found)
            //    uiElement.ContentTemplate = style.CellItemTemplate;

            uiElement.Content = style.CellValue;
        }

        public override void CreateRendererElement(ContentControl uiElement, GridRenderStyleInfo style)
        {
            //bool found = false;

            //if (style.CellItemTemplateKey != null)
            //{
            //    System.Windows.DataTemplate dt = (System.Windows.DataTemplate)style.GridControl.TryFindResource(style.CellItemTemplateKey);
            //    found = dt != null;
            //    if (found)
            //        uiElement.ContentTemplate = dt;
            //}

            //if (!found)
            //    uiElement.ContentTemplate = style.CellItemTemplate;

            uiElement.Content = style.CellValue;
            base.CreateRendererElement(uiElement, style);
        }
        protected override string GetControlTextFromEditorCore(ContentControl uiElement)
        {
            // TODO: Could I use a Converter?
            return uiElement.Content.ToString();
        }
    }
#endif
}