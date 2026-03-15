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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Syncfusion.Windows.Reports.Designer.Dialogs;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Reports.Common;
using Syncfusion.Windows.Chart;
using System.Text.RegularExpressions;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Syncfusion.RDL.Internal;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Controls
{   
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    /// <summary>
    /// Interaction logic for Chart control
    /// </summary>
    internal class ChartControl : Control, IReportItemControl, INotifyPropertyChanged
    {
        # region Variables

        private bool isFocusedItem;
        private bool isItemSelected;
        private string columnsCount = "2";
        private string rowsCount = "1";
        private string seriesName;
        private int seriesCount = 0;
        //private bool IsaddedValuePanel = false;
        internal Editors.ChartProperties chartProperties;
        public Editors.ChartSeriesPropertiesCollection chartSeriesPropertiesCollection;
        private Editors.ChartTitleProperties titleProperties;
        private Editors.CategoryAxisTitleProperties categoryAxisTitleProperties;
        private Editors.CategoryAxisTitleProperties secondaryXAxisTitleProperties;
        private Editors.ChartSeriesProperties chartSeriesProperties;
        private Editors.ChartLegendProperties chartLegendProperties;
        private Editors.ValueAxisTitleProperties valueAxisTitleProperties;
        private Editors.ValueAxisTitleProperties secondaryYAxisTitleProperties;
        private Editors.CategoryAxisProperties chartCategoryAxisProperties;
        private Editors.CategoryAxisProperties chartSecondaryCategoryAxisProperties;
        private Editors.ValueAxisProperties chartValueAxisProperties;
        private Editors.ValueAxisProperties chartSecondaryValueAxisProperties;
        private Editors.DefaultChartProperties defaultChartProperties;
        private Editors.LegendTitleProperties legendTitleProperties;
        private List<String> seriesColorCollection;
        private ChartAxis secondaryXAxis;
        private ChartAxis secondaryYAxis;
        private object propertyOldValue = null;
        private string error_title;
        # endregion

        #region ReportItemControl Interface

        public RDL.DOM.ReportItem ReportItem
        {
            get;
            set;
        }

        public DesignPanel Panel
        {
            get;
            set;
        }

        public string ItemName
        {
            get
            {
                return this.Name;
            }
            set
            {
                this.Name = value;
            }
        }

        public double ItemHeight
        {
            get
            {
                return this.ActualHeight;
            }
            set
            {
                if (this.ItemHeight != value)
                {
                    this.chartProperties.IsInternalPropertyChange = true;
                    this.chartProperties.Height = propertyValueConvertor.GetSizeValue(value, this.chartProperties.Height);
                    this.chartProperties.IsInternalPropertyChange = false;
                }
                this.Height = value;
            }
        }

        public double ItemWidth
        {
            get
            {
                return this.ActualWidth;
            }
            set
            {
                if (this.ItemWidth != value)
                {
                    this.chartProperties.IsInternalPropertyChange = true;
                    this.chartProperties.Width = propertyValueConvertor.GetSizeValue(value, this.chartProperties.Width);
                    this.chartProperties.IsInternalPropertyChange = false;
                }
                this.Width = value;
            }
        }

        public double ItemTop
        {
            get
            {
                return Canvas.GetTop(this);
            }
            set
            {
                if (this.ItemTop != value)
                {
                    this.chartProperties.IsInternalPropertyChange = true;
                    this.chartProperties.Top = propertyValueConvertor.GetSizeValue(value, this.chartProperties.Top);
                    this.chartProperties.IsInternalPropertyChange = false;
                }
                Canvas.SetTop(this, value);
            }
        }

        public double ItemLeft
        {
            get
            {
                return Canvas.GetLeft(this);
            }
            set
            {
                if (this.ItemLeft != value)
                {
                    this.chartProperties.IsInternalPropertyChange = true;
                    this.chartProperties.Left = propertyValueConvertor.GetSizeValue(value, this.chartProperties.Left);
                    this.chartProperties.IsInternalPropertyChange = false;
                }
                Canvas.SetLeft(this, value);
            }
        }

        public bool IsTablixItem
        {
            get;
            set;
        }

        public bool IsItemSelected
        {
            get
            {
                return this.isItemSelected;
            }
            set
            {
                if (this.isItemSelected != value)
                {
                    isItemSelected = value;
                    this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.chartProperties, IsSelected = value });

                    if (!isItemSelected)
                    {
                        this.RemoveChartObjectSelectionAdorner();
                    }

                    OnPropertyChanged("IsItemSelected");
                }
            }
        }

        public bool IsFocusedItem
        {
            get
            {
                return isFocusedItem;
            }
            set
            {
                if (isFocusedItem != value)
                {
                    this.isFocusedItem = value;
                    OnPropertyChanged("IsFocusedItem");
                }
            }
        }

        public new Canvas Parent { get; set; }

        public bool IsExploded { get; set; }     
        public bool ChartMarker { get; set; }      
        public DrawingReportItem ItemType
        {
            get
            {
                return DrawingReportItem.Chart;
            }
        }

        public event ReportItemControlSizeHandler ReportItemSizeChanged;

        public void RaiseReportItemSizeChangedEvent()
        {
            if (this.ReportItemSizeChanged != null)
            {
                this.ReportItemSizeChanged(this, new EventArgs());
            }
        }

        public event ReportItemSelectedEvent ReportItemSelected;

        public void RaiseReportItemSelectedEvent(SelectedItemEventArgs selectedObjectArg)
        {
            if (this.ReportItemSelected != null)
            {
                if (selectedObjectArg.SelectedItem is Editors.ChartProperties)
                {
                    this.ReportItemSelected(this, selectedObjectArg);
                }
                else
                {
                    this.ReportItemSelected(null, selectedObjectArg);
                }
            }
        }

        public void RaiseReportItemSelectedEvent(object sender, SelectedItemEventArgs selectedObjectArg)
        {
            if (this.ReportItemSelected != null)
            {
                if (sender is AdornerLayer)
                {
                    this.ReportItemSelected(null, selectedObjectArg);
                }
                else if(sender is ChartControl)
                {
                    this.ReportItemSelected(this, selectedObjectArg);
                }
            }
        }

        public ImageSource GetImageSource()
        {
            System.Windows.Media.Imaging.RenderTargetBitmap rtb = new System.Windows.Media.Imaging.RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Default);            
            DrawingVisual dv = new DrawingVisual();

            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush();
                vb.AutoLayoutContent = true;
                vb.Visual = this;
                ctx.DrawRectangle(vb, null, new Rect(new Point(0,0), new System.Windows.Size(this.ActualWidth, this.ActualHeight)));
            }

            rtb.Render(dv);
            return rtb;
        }

        public RDL.DOM.ReportItem GetReportItem()
        {
            RDL.DOM.Chart chart = new RDL.DOM.Chart();
            chart.Name = this.chartProperties.Name;
            chart.Visibility = new RDL.DOM.Visibility();

            if (this.chartProperties.ToggleItem != null)
            {
                chart.Visibility.ToggleItem = this.chartProperties.ToggleItem;
            }
            if (this.chartProperties.Hidden == "True" || this.chartProperties.Hidden.StartsWith("="))
            {
                chart.Visibility.Hidden = this.chartProperties.Hidden;
            }

            chart.DataSetName = this.DataSetName;
            chart.Height = new RDL.DOM.Size(this.ItemHeight / 96 + DesignPanel.GetMeasuredUnit(this.chartProperties.Height));
            chart.Width = new RDL.DOM.Size(this.ItemWidth / 96 + DesignPanel.GetMeasuredUnit(this.chartProperties.Width));
            
            chart.Left = new RDL.DOM.Size(this.ItemLeft / 96 + DesignPanel.GetMeasuredUnit(this.chartProperties.Left));
            chart.Top = new RDL.DOM.Size(this.ItemTop / 96 + DesignPanel.GetMeasuredUnit(this.chartProperties.Top));

            chart.Style = new RDL.DOM.Style();
            chart.Style.Border = new RDL.DOM.Border();
            chart.ToolTip = this.chartProperties.ToolTip;
            chart.ChartCustomPaletteColors = new RDL.DOM.ChartCustomPaletteColors();
            chart.Style.Border.Width = this.chartProperties.BorderWidth;
            chart.Style.Border.Color = this.chartProperties.BorderColor;

            if (this.ChartControlType != Controls.ChartControlType.Chart)
            {
                chart.DesignerMode = this.ChartControlType.ToString();
            }
            if (!string.IsNullOrEmpty(this.chartProperties.BackgroundImage.ImageValue))
            {
                chart.Style.BackgroundImage = new RDL.DOM.BackgroundImage();
                chart.Style.BackgroundImage.Value = this.chartProperties.BackgroundImage.ImageValue;
                chart.Style.BackgroundImage.Source = this.chartProperties.BackgroundImage.Source;
                if (this.chartProperties.BackgroundImage.MIMEType != null)
                    chart.Style.BackgroundImage.MIMEType = this.chartProperties.BackgroundImage.MIMEType;
            }
            if (!string.IsNullOrEmpty(this.chartProperties.DocumentMapLabel))
            {
                chart.DocumentMapLabel = this.chartProperties.DocumentMapLabel;
            }
            if (this.chartProperties.PageBreak != RDL.DOM.BreakLocation.None)
            {
                chart.PageBreak = new RDL.DOM.PageBreak();
                chart.PageBreak.BreakLocation = this.chartProperties.PageBreak;
            }
            if (!string.IsNullOrEmpty(this.chartProperties.DataElementName))
            {
                chart.DataElementName = this.chartProperties.DataElementName;
            }
            if (this.chartProperties.DataElementOutput != "Auto")
            {
                chart.DataElementOutput = (RDL.DOM.DataElementOutputs)Enum.Parse(typeof(RDL.DOM.DataElementOutputs), this.chartProperties.DataElementOutput);
            }

            chart.Style.BackgroundColor = this.chartProperties.ChartBackground;
            chart.Style.Border.Style = this.chartProperties.BorderStyle;
            this.UpdateChartSeries(chart);
            this.UpdateChartObj(chart);

            return chart;
        }

        public void RestoreReportItem(RDL.DOM.ReportItem reportItem)
        {
            this.SetInternalPropertyChangetoTrue();
            this.ReportItem = reportItem;
            if (this.ReportItem != null)
            {
                this.PopulateReportItem();
                this.ReportItem = null;
            }

            this.SetInternalPropertyChangetoFalse();
        }

        public void UpdateItemSizeProperties()
        {
            this.chartProperties.IsInternalPropertyChange = true;
            this.chartProperties.Width = propertyValueConvertor.GetSizeValue(this.ItemWidth, this.chartProperties.Width);
            this.chartProperties.Top = propertyValueConvertor.GetSizeValue(this.ItemTop, this.chartProperties.Top);
            this.chartProperties.Left = propertyValueConvertor.GetSizeValue(this.ItemLeft, this.chartProperties.Left);
            this.chartProperties.Height = propertyValueConvertor.GetSizeValue(this.ItemHeight, this.chartProperties.Height);
            this.chartProperties.IsInternalPropertyChange = false;
        }

        #endregion

        public string ColumnsCount
        {
            get
            {
                return this.columnsCount;
            }
            set
            {
                if (value != this.columnsCount)
                {
                    this.columnsCount = value;
                    this.OnPropertyChanged("ColumnsCount");
                }
            }
        }

        public string RowsCount
        {
            get
            {
                return this.rowsCount;
            }
            set
            {
                if (value != this.rowsCount)
                {
                    this.rowsCount = value;
                    this.OnPropertyChanged("RowsCount");
                }
            }
        }

        #region Desrialization

        private void PopulateReportItem()
        {
            this.RemoveChartObjectSelectionAdorner();
            RDL.DOM.Chart chartBase = this.ReportItem as RDL.DOM.Chart;
            this.Seriespanel.Children.Clear();
            this.ValuePanel.Children.Clear();
            this.ColumnPanel.Children.Clear();
            if (chartBase != null)
            {
                if (chartBase.ChartAreas != null && chartBase.ChartAreas.Count > 0)
                {
                    if (chartBase.ChartData != null && chartBase.ChartData.ChartSeriesCollection != null && chartBase.ChartData.ChartSeriesCollection.Count > 0)
                    {
                        this.InnerChart.Areas[0].Series.Clear();
                        this.chartSeriesPropertiesCollection.Clear();
                        this.InitializeOldChart();
                        this.SeriesPropertiesChanged();
                    }

                    this.ChartPropertiesChanged();
                    this.CategoryAxisPropertiesChanged();
                    this.SecondaryCategoryAxisPropertiesChanged();
                    this.ValueAxisPropertiesChanged();
                    this.SecondaryValueAxisPropertiesChanged();
                    int i = 0;
                    foreach (RDL.DOM.ChartAxis axis in chartBase.ChartAreas[0].ChartCategoryAxes)
                    {
                        chartBase.ChartAreas[0].ChartCategoryAxes[i].ChartStripLines = null;
                        i++;
                    }

                    int j = 0;

                    foreach (RDL.DOM.ChartAxis axis in chartBase.ChartAreas[0].ChartValueAxes)
                    {
                        chartBase.ChartAreas[0].ChartValueAxes[j].ChartStripLines = null;
                        j++;
                    }

                    this.AddLegendToChart();

                    if (chartBase.ChartLegends != null && chartBase.ChartLegends.Count > 0)
                    {
                        this.legendPropertiesChanged();
                    }
                    else
                    {
                        this.ChartLegand.Visibility = Visibility.Collapsed;
                    }

                    if (chartBase.ChartTitles != null)
                    {
                        if (chartBase.ChartTitles.Count != 0)
                        {
                            this.AddChartTitle();
                            this.ChartTitlePropertiesChanged();
                        }
                        else
                        {
                            this.titleProperties.Visibility = "False";                            
                            this.AddTitle.IsChecked = false;
                        }
                    }

                    if (chartBase.ChartAreas[0].ChartCategoryAxes[0].ChartAxisTitle != null)
                    {
                        this.AddPrimaryAxisTitle();
                        if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                        {
                            this.CategoryAxisTitle.Margin = new Thickness(15, 0, 5, 10);
                        }
                        this.CategoryTitlePropertiesChanged();
                        if (string.IsNullOrEmpty(chartBase.ChartAreas[0].ChartCategoryAxes[0].ChartAxisTitle.Caption))
                        {
                            this.chartCategoryAxisProperties.Visibility = "False";
                            this.ShowCategoryTitle.IsChecked = false;
                        }
                    }
                    else
                    {
                        this.chartCategoryAxisProperties.Visibility = "False";
                        this.ShowCategoryTitle.IsChecked = false;
                    }

                    if (chartBase.ChartAreas[0].ChartCategoryAxes[1].ChartAxisTitle != null)
                    {
                        this.AddSecondaryXAxisTitle();
                        if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                        {
                            this.SecondaryXAxisTitle.Margin = new Thickness(15, 0, 5, 10);
                        }
                        this.SecondaryCategoryTitlePropertiesChanged();
                        if (string.IsNullOrEmpty(chartBase.ChartAreas[0].ChartCategoryAxes[1].ChartAxisTitle.Caption))
                        {
                            this.SecondaryXAxisTitle.Visibility = Visibility.Hidden;
                        }
                    }
                    else
                    {
                        this.SecondaryXAxisTitle.Visibility = Visibility.Hidden;
                    }           

                    if (chartBase.ChartAreas[0].ChartValueAxes[0].ChartAxisTitle != null)
                    {
                        this.AddSecondaryAxisTitle();
                        if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                        {
                            this.ValueAxisTitle.Margin = new Thickness(10, 5, 10, 10);
                        }                       
                        this.ValueTitlePropertiesChanged();
                        if (string.IsNullOrEmpty(chartBase.ChartAreas[0].ChartValueAxes[0].ChartAxisTitle.Caption))
                        {
                            this.chartValueAxisProperties.Visibility = "False";
                            this.ShowValueTitle.IsChecked = false;
                        }
                    }
                    else
                    {
                        this.chartValueAxisProperties.Visibility = "False";
                        this.ShowValueTitle.IsChecked = false;
                    }

                    if (chartBase.ChartAreas[0].ChartValueAxes[1].ChartAxisTitle != null)
                    {
                        this.AddSecondaryYAxisTitle();
                        if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                        {
                            this.SecondaryYAxisTitle.Margin = new Thickness(10, 5, 10, 10);
                        }
                        this.SecondaryValueTitlePropertiesChanged();
                        if (string.IsNullOrEmpty(chartBase.ChartAreas[0].ChartValueAxes[1].ChartAxisTitle.Caption))
                        {
                            this.SecondaryYAxisTitle.Visibility = Visibility.Hidden;
                        }
                    }
                    else
                    {
                        this.SecondaryYAxisTitle.Visibility = Visibility.Hidden;
                    }
                }

                if (chartBase.DataSetName != null)
                {
                    this.DataSetName = chartBase.DataSetName;
                }

                this.ChartPanel();
                this.AddPanelValues();

                if (this.DataSets.Count > 0)
                {
                    this.chartProperties.Dataset = this.DataSetName;
                }
                else
                {
                    this.chartProperties.Dataset = string.Empty;
                }
                if (chartBase.PageBreak != null )
                {
                    this.chartProperties.PageBreak = chartBase.PageBreak.BreakLocation;
                }
                if (chartBase.Visibility != null)
                {
                    if (chartBase.Visibility.ToggleItem != null)
                        this.chartProperties.ToggleItem = chartBase.Visibility.ToggleItem;
                    if (chartBase.Visibility.Hidden != null)
                        this.chartProperties.Hidden = chartBase.Visibility.Hidden;
                }
                if (chartBase.Style != null)
                {
                    if (chartBase.Style.BackgroundImage != null)
                    {
                        this.chartProperties.BackgroundImage.Source = chartBase.Style.BackgroundImage.Source;
                        this.chartProperties.BackgroundImage.ImageValue = chartBase.Style.BackgroundImage.Value;
                        if (chartBase.Style.BackgroundImage.MIMEType != null)
                            this.chartProperties.BackgroundImage.MIMEType = chartBase.Style.BackgroundImage.MIMEType;
                    }
                }
                if (chartBase.DataElementOutput != RDL.DOM.DataElementOutputs.Auto)
                {
                    this.chartProperties.DataElementOutput = chartBase.DataElementOutput.ToString();
                }
                if (chartBase.Height != null)
                {
                    this.chartProperties.Height = chartBase.Height.size;
                }
                if (chartBase.Width != null)
                {
                    this.chartProperties.Width = chartBase.Width.size;
                }
                if (chartBase.Top != null)
                {
                    this.chartProperties.Top = chartBase.Top.size;
                }
                if (chartBase.Left != null)
                {
                    this.chartProperties.Left = chartBase.Left.size;
                }

                this.chartProperties.DataElementName = chartBase.DataElementName;
                this.chartProperties.DocumentMapLabel = chartBase.DocumentMapLabel;
            }
        }

        private void ChartPanel()
        {
            RDL.DOM.Chart ChartBase = this.ReportItem as RDL.DOM.Chart;
            if ((ReportItem as RDL.DOM.Chart).DataSetName != null)
            {
                List<string> categoryItemList = new List<string>();
                List<string> seriesItemList = new List<string>();
                List<string> dataItemList = new List<string>();
                RDL.DOM.ChartMembers chartMembersSeries = (ReportItem as RDL.DOM.Chart).ChartSeriesHierarchy.ChartMembers;
                RDL.DOM.ChartMembers chartMembersCategory = (ReportItem as RDL.DOM.Chart).ChartCategoryHierarchy.ChartMembers;
                RDL.DOM.ChartMembers chartMembersPanel = (ReportItem as RDL.DOM.Chart).ChartCategoryHierarchy.ChartMembers;

                double chartCategoryMembersCount = 0;

                while (chartMembersCategory != null && chartMembersCategory.Count > 0)
                {
                    if (chartMembersCategory[0].Group != null)
                    {
                        chartCategoryMembersCount++;
                    }

                    chartMembersCategory = chartMembersCategory[0].ChartMembers;
                }

                double chartSeriesMembersCount = 0;

                while (chartMembersSeries != null && chartMembersSeries.Count > 0)
                {
                    if (chartMembersSeries[0].Group != null)
                    {
                        chartSeriesMembersCount++;
                    }

                    chartMembersSeries = chartMembersSeries[0].ChartMembers;
                }

                chartMembersSeries = (ReportItem as RDL.DOM.Chart).ChartSeriesHierarchy.ChartMembers;
                chartMembersCategory = (ReportItem as RDL.DOM.Chart).ChartCategoryHierarchy.ChartMembers;


                var chartDataListTemp = (from chartSeries in (ReportItem as RDL.DOM.Chart).ChartData.ChartSeriesCollection
                                         from dataPoint in chartSeries.ChartDataPoints
                                         where dataPoint.ChartDataPointValues.Y != string.Empty
                                         select dataPoint.ChartDataPointValues.Y).FirstOrDefault();

                if (chartDataListTemp != null)
                {
                    var chartDataList = from chartSeries in (ReportItem as RDL.DOM.Chart).ChartData.ChartSeriesCollection
                                        from dataPoint in chartSeries.ChartDataPoints
                                        select "[" + dataPoint.ChartDataPointValues.Y.ToString().Replace("Fields!", string.Empty).Replace(".Value", string.Empty).Replace("=", string.Empty) + "]";

                    dataItemList = chartDataList.ToList();

                    while (chartMembersCategory != null && chartMembersCategory.Count > 0)
                    {
                        if (chartMembersCategory[0].Group != null)
                        {
                            string labelStr = chartMembersCategory[0].Label;
                            //ReportTablixData temp = new ReportTablixData();

                            //if (this.DataSets != null)
                            if (labelStr != null)
                            {
                                var columnName = (from dataSetThis in DataSets
                                                  from DataSetfield in dataSetThis.Fields
                                                  where DataSetfield.Name == new ExpressionEngine().ParsedFieldName(labelStr)
                                                  where dataSetThis.Name == (ReportItem as RDL.DOM.Chart).DataSetName
                                                  select DataSetfield.DataField).FirstOrDefault();

                                categoryItemList.Add("[" + columnName + "]");
                            }
                        }

                        chartMembersCategory = chartMembersCategory[0].ChartMembers;
                    }
                    while (chartMembersSeries != null && chartMembersSeries.Count > 0)
                    {
                        if (chartMembersSeries[0].Group != null)
                        {
                            string labelStr = chartMembersSeries[0].Label;

                            if (labelStr != null)
                            {
                                var columnName = (from dataSetThis in DataSets
                                                  from DataSetfield in dataSetThis.Fields
                                                  where DataSetfield.Name == new ExpressionEngine().ParsedFieldName(labelStr)
                                                  where dataSetThis.Name == (ReportItem as RDL.DOM.Chart).DataSetName
                                                  select DataSetfield.DataField).FirstOrDefault();

                                seriesItemList.Add("[" + columnName + "]");
                            }
                        }

                        chartMembersSeries = chartMembersSeries[0].ChartMembers;
                    }
                }

                this.DataSets = DataSets;
                this.DataSources = DataSources;
                if (categoryItemList == null && dataItemList == null)
                {
                    this.IsInitiallyBinded = false;
                }
                else
                {
                    if (categoryItemList != null)
                    {
                        this.CategoryBindingList = categoryItemList;
                    }

                    if (dataItemList != null)
                    {
                        this.DataBindingList = dataItemList;
                    }
                    if (seriesItemList != null)
                    {
                        this.SeriesBindingList = seriesItemList;
                    }
                }
            }
        }

        private void ValueAxisPropertiesChanged()
        {
            RDL.DOM.Chart ChartBase = this.ReportItem as RDL.DOM.Chart;
            RDL.DOM.ChartAxis secondaryAxis = ChartBase.ChartAreas[0].ChartValueAxes[0] as RDL.DOM.ChartAxis;
            ChartArea.SecondaryAxis.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(SecondaryAxis_PreviewMouseLeftButtonDown);

            if (secondaryAxis.Style != null && secondaryAxis.Style.Border != null)
            {
                this.chartValueAxisProperties.LineColor = secondaryAxis.Style.Border.Color;
                this.chartValueAxisProperties.LineStyle = secondaryAxis.Style.Border.Style;
                if (secondaryAxis.Style.Border.Width != null)
                {
                    this.chartValueAxisProperties.LineWidth = secondaryAxis.Style.Border.Width.size;
                }
                else
                {
                    this.chartValueAxisProperties.LineWidth = "1pt";
                }
            }
            else
            {
                secondaryAxis.Style = new RDL.DOM.Style();
                secondaryAxis.Style.Border = new RDL.DOM.Border();
            }

            if (secondaryAxis.HideLabels == true)
            {
                this.chartValueAxisProperties.HideAxisLabels = "True";
            }
            else
            {
                this.chartValueAxisProperties.FontAngle = secondaryAxis.Angle.ToString();
                if (secondaryAxis.Style != null)
                {
                    this.chartValueAxisProperties.FontFamily = secondaryAxis.Style.FontFamily;
                    this.chartValueAxisProperties.FontSize = secondaryAxis.Style.FontSize.size;
                    this.chartValueAxisProperties.FontColor = secondaryAxis.Style.Color;
                    this.chartValueAxisProperties.FontWeight = secondaryAxis.Style.FontWeight;
                }
            }

            if (secondaryAxis.ChartMajorTickMarks != null)
            {
                this.chartValueAxisProperties.EnableMajorTickMarks = secondaryAxis.ChartMajorTickMarks.Enabled.ToString();
                this.chartValueAxisProperties.EnableMinorTickMarks = secondaryAxis.ChartMinorTickMarks.Enabled.ToString();

                if (secondaryAxis.ChartMajorTickMarks.Enabled == RDL.DOM.BooleanOptions.False)
                {
                    this.chartValueAxisProperties.TickLength = "0";
                }

                else
                {
                    if (secondaryAxis.ChartMajorTickMarks.Style != null)
                    {
                        if (secondaryAxis.ChartMajorTickMarks.Style.Border != null)
                        {
                            this.chartValueAxisProperties.TickStyle = secondaryAxis.ChartMajorTickMarks.Style.Border.Style;
                            this.chartValueAxisProperties.TickColor = secondaryAxis.ChartMajorTickMarks.Style.Border.Color;
                            if (secondaryAxis.ChartMajorTickMarks.Style.Border.Width != null)
                            {
                                this.chartValueAxisProperties.TickLength = secondaryAxis.ChartMajorTickMarks.Style.Border.Width.size;
                            }
                            else
                            {
                                this.chartValueAxisProperties.TickLength = "1pt";
                            }
                        }
                    }
                    else
                    {
                        secondaryAxis.ChartMajorTickMarks.Style = new RDL.DOM.Style();
                        secondaryAxis.ChartMajorTickMarks.Style.Border = new RDL.DOM.Border();
                    }
                }
            }
            else
            {
                secondaryAxis.ChartMajorTickMarks = new RDL.DOM.ChartMajorTickMarks();
                secondaryAxis.ChartMajorTickMarks.Style = new RDL.DOM.Style();
                secondaryAxis.ChartMajorTickMarks.Style.Border = new RDL.DOM.Border();
                secondaryAxis.ChartMajorTickMarks.Enabled = RDL.DOM.BooleanOptions.False;
            }

            if (secondaryAxis.ChartMinorTickMarks != null)
            {
                if (secondaryAxis.ChartMinorTickMarks.Enabled == RDL.DOM.BooleanOptions.True)
                {
                    if (secondaryAxis.ChartMinorTickMarks.Style != null && secondaryAxis.ChartMinorTickMarks.Style.Border != null)
                    {
                        this.chartValueAxisProperties.MinorTickStyle = secondaryAxis.ChartMinorTickMarks.Style.Border.Style;
                        this.chartValueAxisProperties.MinorTickColor = secondaryAxis.ChartMinorTickMarks.Style.Border.Color;
                        if (secondaryAxis.ChartMinorTickMarks.Style.Border.Width == null)
                        {
                            secondaryAxis.ChartMinorTickMarks.Style.Border.Width = "1pt";
                        }
                        else
                        {
                            this.chartValueAxisProperties.TickWidth = secondaryAxis.ChartMinorTickMarks.Style.Border.Width.size;
                        }
                    }

                    else
                    {
                        secondaryAxis.ChartMinorTickMarks.Style = new RDL.DOM.Style();
                        secondaryAxis.ChartMinorTickMarks.Style.Border = new RDL.DOM.Border();
                        //chartAxis2.ChartMinorTickMarks.Style.Border.Style = new RDL.DOM.BorderStyles();
                        secondaryAxis.ChartMinorTickMarks.Enabled = RDL.DOM.BooleanOptions.False;
                    }

                    InnerChart.Areas[0].SecondaryAxis.SmallTickSize = secondaryAxis.ChartMinorTickMarks.Length;
                    InnerChart.Areas[0].SecondaryAxis.SmallTicksPerInterval = 4;
                }
                else
                {
                    InnerChart.Areas[0].SecondaryAxis.SmallTickSize = 0;

                    if (secondaryAxis.ChartMinorTickMarks.Style == null)
                    {
                        secondaryAxis.ChartMinorTickMarks.Style = new RDL.DOM.Style();
                        secondaryAxis.ChartMinorTickMarks.Style.Border = new RDL.DOM.Border();
                        secondaryAxis.ChartMinorTickMarks.Enabled = RDL.DOM.BooleanOptions.False;
                    }
                }
            }
            else
            {
                secondaryAxis.ChartMinorTickMarks = new RDL.DOM.ChartMinorTickMarks();
                secondaryAxis.ChartMinorTickMarks.Style = new RDL.DOM.Style();
                secondaryAxis.ChartMinorTickMarks.Style.Border = new RDL.DOM.Border();
                secondaryAxis.ChartMinorTickMarks.Enabled = RDL.DOM.BooleanOptions.False;
            }

            if (secondaryAxis.ChartMajorGridLines != null)
            {
                if (secondaryAxis.ChartMajorGridLines.Enabled == RDL.DOM.BooleanOptions.True || secondaryAxis.ChartMajorGridLines.Enabled == RDL.DOM.BooleanOptions.Auto)
                {
                    if (secondaryAxis.ChartMajorGridLines.Enabled == RDL.DOM.BooleanOptions.True)
                    {
                        this.defaultChartProperties.ValueAxisMajorGridLines.EnableMajorGridLines = "True";
                    }
                    if (secondaryAxis.ChartMajorGridLines.Enabled == RDL.DOM.BooleanOptions.Auto)
                    {
                        this.defaultChartProperties.ValueAxisMajorGridLines.EnableMajorGridLines = "Auto";
                    }
                    if (secondaryAxis.ChartMajorGridLines.Style != null && secondaryAxis.ChartMajorGridLines.Style.Border != null)
                    {
                        this.defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesColor = secondaryAxis.ChartMajorGridLines.Style.Border.Color;
                        this.defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesStyle = secondaryAxis.ChartMajorGridLines.Style.Border.Style;

                        if (secondaryAxis.ChartMajorGridLines.Style.Border.Width != null)
                        {
                            this.defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesWidth = secondaryAxis.ChartMajorGridLines.Style.Border.Width.size;
                        }
                    }
                }
            }
            else
            {
                this.defaultChartProperties.ValueAxisMajorGridLines.EnableMajorGridLines = "False";
            }

            if (secondaryAxis.ChartMinorGridLines != null)
            {
                if (secondaryAxis.ChartMinorGridLines.Enabled == RDL.DOM.BooleanOptions.True)
                {
                    this.defaultChartProperties.ValueAxisMinorGridLines.EnableMinorGridLines = "True";

                    if (secondaryAxis.ChartMinorGridLines.Style != null)
                    {
                        if (secondaryAxis.ChartMinorGridLines.Style.Border != null)
                        {
                            this.defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesColor = secondaryAxis.ChartMinorGridLines.Style.Border.Color;
                            this.defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesStyle = secondaryAxis.ChartMinorGridLines.Style.Border.Style;

                            if (secondaryAxis.ChartMinorGridLines.Style.Border.Width != null)
                            {
                                this.defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesWidth = secondaryAxis.ChartMinorGridLines.Style.Border.Width.size;
                            }
                        }
                    }
                }
            }
            else
            {
                if (secondaryAxis.ChartMinorGridLines.Enabled == RDL.DOM.BooleanOptions.False)
                {
                    this.defaultChartProperties.ValueAxisMinorGridLines.EnableMinorGridLines = "False";
                }
                else
                {
                    this.defaultChartProperties.ValueAxisMinorGridLines.EnableMinorGridLines = "Auto";
                }
            }

            if (secondaryAxis.Reverse == true)
            {
                this.chartValueAxisProperties.ReverseDirection = "True";
            }
            else
            {
                this.chartValueAxisProperties.ReverseDirection = "False";
            }

            if (secondaryAxis.ChartAxisTitle != null)
            {
                ValueAxisTitle = new TextBox();

                ValueAxisTitle.Text = secondaryAxis.ChartAxisTitle.Caption;

                this.valueAxisTitleProperties.Name = secondaryAxis.ChartAxisTitle.Caption;
                this.valueAxisTitleProperties.TitleAlignment = secondaryAxis.ChartAxisTitle.Position.ToString();

                if (secondaryAxis.ChartAxisTitle.Style != null)
                {
                    this.valueAxisTitleProperties.FontFamily = secondaryAxis.ChartAxisTitle.Style.FontFamily;
                    this.valueAxisTitleProperties.FontSize = secondaryAxis.ChartAxisTitle.Style.FontSize.size;
                    this.valueAxisTitleProperties.FontStyle = secondaryAxis.ChartAxisTitle.Style.FontStyle;
                    this.valueAxisTitleProperties.FontColor = secondaryAxis.ChartAxisTitle.Style.Color;
                }

                ValueAxisTitle.BorderBrush = Brushes.Transparent;
                ValueAxisTitle.Background = Brushes.Transparent;
                this.ChartArea.SecondaryAxis.Header = ValueAxisTitle;

                ChartBase.ChartAreas[0].ChartValueAxes[0] = secondaryAxis;
            }
            if (secondaryAxis.Visible == RDL.DOM.BooleanOptions.False)
            {
                InnerChart.Areas[0].SecondaryAxis.AxisVisibility = System.Windows.Visibility.Collapsed;
                ChartArea.SetShowGridLines(ChartArea.SecondaryAxis, false);
            }

            ChartBase.ChartAreas[0].ChartValueAxes[0] = secondaryAxis;
        }

        private void SecondaryValueAxisPropertiesChanged()
        {
            RDL.DOM.Chart ChartBase = this.ReportItem as RDL.DOM.Chart;
            RDL.DOM.ChartAxis secondaryAxis1 = ChartBase.ChartAreas[0].ChartValueAxes[1] as RDL.DOM.ChartAxis;

            if (secondaryAxis1.Style != null && secondaryAxis1.Style.Border != null)
            {
                this.chartSecondaryValueAxisProperties.LineColor = secondaryAxis1.Style.Border.Color;
                this.chartSecondaryValueAxisProperties.LineStyle = secondaryAxis1.Style.Border.Style;
                if (secondaryAxis1.Style.Border.Width != null)
                {
                    this.chartSecondaryValueAxisProperties.LineWidth = secondaryAxis1.Style.Border.Width.size;
                }
                else
                {
                    this.chartSecondaryValueAxisProperties.LineWidth = "1pt";
                }
            }
            else
            {
                secondaryAxis1.Style = new RDL.DOM.Style();
                secondaryAxis1.Style.Border = new RDL.DOM.Border();
            }

            if (secondaryAxis1.ChartMajorTickMarks != null)
            {
                this.chartSecondaryValueAxisProperties.EnableMajorTickMarks = secondaryAxis1.ChartMajorTickMarks.Enabled.ToString();
                this.chartSecondaryValueAxisProperties.EnableMinorTickMarks = secondaryAxis1.ChartMinorTickMarks.Enabled.ToString();

                if (secondaryAxis1.ChartMajorTickMarks.Enabled == RDL.DOM.BooleanOptions.False)
                {
                    this.chartSecondaryValueAxisProperties.TickLength = "0";
                }
                else
                {
                    if (secondaryAxis1.ChartMajorTickMarks.Style != null && secondaryAxis1.ChartMajorTickMarks.Style.Border != null)
                    {
                        this.chartSecondaryValueAxisProperties.TickStyle = secondaryAxis1.ChartMajorTickMarks.Style.Border.Style;
                        this.chartSecondaryValueAxisProperties.TickColor = secondaryAxis1.ChartMajorTickMarks.Style.Border.Color;
                        if (secondaryAxis1.ChartMajorTickMarks.Style.Border.Width != null)
                        {
                            this.chartSecondaryValueAxisProperties.TickLength = secondaryAxis1.ChartMajorTickMarks.Style.Border.Width.size;
                        }
                        else
                        {
                            this.chartSecondaryValueAxisProperties.TickLength = "1pt";
                        }
                    }

                }
            }
            else
            {
                secondaryAxis1.ChartMajorTickMarks = new RDL.DOM.ChartMajorTickMarks();
                secondaryAxis1.ChartMajorTickMarks.Style = new RDL.DOM.Style();
                secondaryAxis1.ChartMajorTickMarks.Style.Border = new RDL.DOM.Border();
                secondaryAxis1.ChartMajorTickMarks.Enabled = RDL.DOM.BooleanOptions.False;
            }

            if (secondaryAxis1.ChartMinorTickMarks != null)
            {
                if (secondaryAxis1.ChartMinorTickMarks.Enabled == RDL.DOM.BooleanOptions.True)
                {
                    if (secondaryAxis1.ChartMinorTickMarks.Style != null && secondaryAxis1.ChartMinorTickMarks.Style.Border != null)
                    {
                        this.chartSecondaryValueAxisProperties.MinorTickStyle = secondaryAxis1.ChartMinorTickMarks.Style.Border.Style;
                        this.chartSecondaryValueAxisProperties.MinorTickColor = secondaryAxis1.ChartMinorTickMarks.Style.Border.Color;
                        if (secondaryAxis1.ChartMinorTickMarks.Style.Border.Width == null)
                        {
                            this.chartSecondaryValueAxisProperties.TickWidth = "1pt";
                        }
                        else
                        {
                            this.chartSecondaryValueAxisProperties.TickWidth = secondaryAxis1.ChartMinorTickMarks.Style.Border.Width.size;
                        }
                    }

                    this.secondaryYAxis.SmallTickSize = secondaryAxis1.ChartMinorTickMarks.Length;
                    this.secondaryYAxis.SmallTicksPerInterval = 4;
                }
                else
                {
                    this.secondaryXAxis.SmallTickSize = 0;

                    if (secondaryAxis1.ChartMinorTickMarks.Style == null)
                    {
                        secondaryAxis1.ChartMinorTickMarks.Style = new RDL.DOM.Style();
                        secondaryAxis1.ChartMinorTickMarks.Style.Border = new RDL.DOM.Border();
                        secondaryAxis1.ChartMinorTickMarks.Enabled = RDL.DOM.BooleanOptions.False;
                    }
                }
            }
            else
            {
                secondaryAxis1.ChartMinorTickMarks = new RDL.DOM.ChartMinorTickMarks();
                secondaryAxis1.ChartMinorTickMarks.Style = new RDL.DOM.Style();
                secondaryAxis1.ChartMinorTickMarks.Style.Border = new RDL.DOM.Border();
                secondaryAxis1.ChartMinorTickMarks.Enabled = RDL.DOM.BooleanOptions.False;
            }            

            if (secondaryAxis1.Reverse == true)
            {
                this.chartSecondaryValueAxisProperties.ReverseDirection = "True";
            }
            else
            {
                this.chartSecondaryValueAxisProperties.ReverseDirection = "False";
            }

            if (secondaryAxis1.HideLabels == true)
            {
                this.chartSecondaryValueAxisProperties.HideAxisLabels = "True";
            }
            else
            {
                this.chartSecondaryValueAxisProperties.FontAngle = secondaryAxis1.Angle.ToString();
                if (secondaryAxis1.Style != null)
                {
                    this.chartSecondaryValueAxisProperties.FontFamily = secondaryAxis1.Style.FontFamily;
                    this.chartSecondaryValueAxisProperties.FontSize = secondaryAxis1.Style.FontSize.size;
                    this.chartSecondaryValueAxisProperties.FontColor = secondaryAxis1.Style.Color;
                    this.chartSecondaryValueAxisProperties.FontWeight = secondaryAxis1.Style.FontWeight;
                }
            }
            if (secondaryAxis1.ChartAxisTitle != null)
            {
                this.secondaryYAxisTitleProperties.Name = secondaryAxis1.ChartAxisTitle.Caption;
                this.secondaryYAxisTitleProperties.TitleAlignment = secondaryAxis1.ChartAxisTitle.Position.ToString();
                if (secondaryAxis1.ChartAxisTitle.Style != null)
                {
                    this.secondaryYAxisTitleProperties.FontFamily = secondaryAxis1.ChartAxisTitle.Style.FontFamily;
                    this.secondaryYAxisTitleProperties.FontSize = secondaryAxis1.ChartAxisTitle.Style.FontSize.size;
                    this.secondaryYAxisTitleProperties.FontStyle = secondaryAxis1.ChartAxisTitle.Style.FontStyle;
                    this.secondaryYAxisTitleProperties.FontColor = secondaryAxis1.ChartAxisTitle.Style.Color;
                }

                this.SecondaryYAxisTitle.BorderBrush = Brushes.Transparent;
                this.SecondaryYAxisTitle.Background = Brushes.Transparent;
                this.secondaryYAxis.Header = this.SecondaryYAxisTitle;
                ChartBase.ChartAreas[0].ChartValueAxes[1] = secondaryAxis1;
            }

            ChartBase.ChartAreas[0].ChartValueAxes[1] = secondaryAxis1;
        }

        private void CategoryAxisPropertiesChanged()
        {
            RDL.DOM.Chart ChartBase = this.ReportItem as RDL.DOM.Chart;
            RDL.DOM.ChartAxis primaryAxis = ChartBase.ChartAreas[0].ChartCategoryAxes[0] as RDL.DOM.ChartAxis;
            ChartArea.PrimaryAxis.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PrimaryAxis_PreviewMouseLeftButtonDown);

            if (primaryAxis.Style != null && primaryAxis.Style.Border != null)
            {
                this.chartCategoryAxisProperties.LineColor = primaryAxis.Style.Border.Color;
                this.chartCategoryAxisProperties.LineStyle = primaryAxis.Style.Border.Style;
                if (primaryAxis.Style.Border.Width != null)
                {
                    this.chartCategoryAxisProperties.LineWidth = primaryAxis.Style.Border.Width.size;
                }
                else
                {
                    this.chartCategoryAxisProperties.LineWidth = "1pt";
                }
            }
            else
            {
                primaryAxis.Style = new RDL.DOM.Style();
                primaryAxis.Style.Border = new RDL.DOM.Border();
            }

            if (primaryAxis.ChartMajorTickMarks != null)
            {
                this.chartCategoryAxisProperties.EnableMajorTickMarks = primaryAxis.ChartMajorTickMarks.Enabled.ToString();
                this.chartCategoryAxisProperties.EnableMinorTickMarks = primaryAxis.ChartMinorTickMarks.Enabled.ToString();

                if (primaryAxis.ChartMajorTickMarks.Enabled == RDL.DOM.BooleanOptions.False)
                {
                    this.chartCategoryAxisProperties.TickLength = "0";
                }
                else
                {
                    if (primaryAxis.ChartMajorTickMarks.Style != null && primaryAxis.ChartMajorTickMarks.Style.Border != null)
                    {
                        this.chartCategoryAxisProperties.TickStyle = primaryAxis.ChartMajorTickMarks.Style.Border.Style;
                        this.chartCategoryAxisProperties.TickColor = primaryAxis.ChartMajorTickMarks.Style.Border.Color;
                        if (primaryAxis.ChartMajorTickMarks.Style.Border.Width != null)
                        {
                            this.chartCategoryAxisProperties.TickLength = primaryAxis.ChartMajorTickMarks.Style.Border.Width.size;
                        }
                        else
                        {
                            this.chartCategoryAxisProperties.TickLength = "1pt";
                        }
                    }

                }
            }
            else
            {
                primaryAxis.ChartMajorTickMarks = new RDL.DOM.ChartMajorTickMarks();
                primaryAxis.ChartMajorTickMarks.Style = new RDL.DOM.Style();
                primaryAxis.ChartMajorTickMarks.Style.Border = new RDL.DOM.Border();
                primaryAxis.ChartMajorTickMarks.Enabled = RDL.DOM.BooleanOptions.False;
            }

            if (primaryAxis.ChartMinorTickMarks != null)
            {
                if (primaryAxis.ChartMinorTickMarks.Enabled == RDL.DOM.BooleanOptions.True)
                {
                    if (primaryAxis.ChartMinorTickMarks.Style != null && primaryAxis.ChartMinorTickMarks.Style.Border != null)
                    {
                        this.chartCategoryAxisProperties.MinorTickStyle = primaryAxis.ChartMinorTickMarks.Style.Border.Style;
                        this.chartCategoryAxisProperties.MinorTickColor = primaryAxis.ChartMinorTickMarks.Style.Border.Color;
                        if (primaryAxis.ChartMinorTickMarks.Style.Border.Width == null)
                        {
                            this.chartCategoryAxisProperties.TickWidth = "1pt";
                        }
                        else
                        {
                            this.chartCategoryAxisProperties.TickWidth = primaryAxis.ChartMinorTickMarks.Style.Border.Width.size;
                        }                         
                    }

                    InnerChart.Areas[0].PrimaryAxis.SmallTickSize = primaryAxis.ChartMinorTickMarks.Length;
                    InnerChart.Areas[0].PrimaryAxis.SmallTicksPerInterval = 4;
                }
                else
                {
                    InnerChart.Areas[0].PrimaryAxis.SmallTickSize = 0;

                    if (primaryAxis.ChartMinorTickMarks.Style == null)
                    {
                        primaryAxis.ChartMinorTickMarks.Style = new RDL.DOM.Style();
                        primaryAxis.ChartMinorTickMarks.Style.Border = new RDL.DOM.Border();
                        primaryAxis.ChartMinorTickMarks.Enabled = RDL.DOM.BooleanOptions.False;
                    }
                }
            }
            else
            {
                primaryAxis.ChartMinorTickMarks = new RDL.DOM.ChartMinorTickMarks();
                primaryAxis.ChartMinorTickMarks.Style = new RDL.DOM.Style();
                primaryAxis.ChartMinorTickMarks.Style.Border = new RDL.DOM.Border();
                primaryAxis.ChartMinorTickMarks.Enabled = RDL.DOM.BooleanOptions.False;
            }

            InnerChart.Areas[0].PrimaryAxis.ContentPath = "SeriesName";
            InnerChart.Areas[0].PrimaryAxis.PositionPath = "SeriesId";
            InnerChart.Areas[0].PrimaryAxis.LabelsSource = DefaultChartData1();

            if (primaryAxis.ChartMajorGridLines != null)
            {
                if (primaryAxis.ChartMajorGridLines.Enabled == RDL.DOM.BooleanOptions.True)
                {
                    if (primaryAxis.ChartMajorGridLines.Enabled == RDL.DOM.BooleanOptions.True)
                    {
                        this.defaultChartProperties.CategoryAxisMajorGridLines.EnableMajorGridLines = "True";
                    }
                    if (primaryAxis.ChartMajorGridLines.Enabled == RDL.DOM.BooleanOptions.Auto)
                    {
                        this.defaultChartProperties.CategoryAxisMajorGridLines.EnableMajorGridLines = "Auto";
                    }
                    if (primaryAxis.ChartMajorGridLines.Style != null && primaryAxis.ChartMajorGridLines.Style.Border != null)
                    {
                        this.defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesColor = primaryAxis.ChartMajorGridLines.Style.Border.Color;
                        this.defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesStyle = primaryAxis.ChartMajorGridLines.Style.Border.Style;
                        if (primaryAxis.ChartMajorGridLines.Style.Border.Width != null)
                        {
                            this.defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesWidth = primaryAxis.ChartMajorGridLines.Style.Border.Width.size;
                        }
                    }
                }
            }
            else
            {
                if (primaryAxis.ChartMajorGridLines.Enabled == RDL.DOM.BooleanOptions.False)
                {
                    this.defaultChartProperties.CategoryAxisMajorGridLines.EnableMajorGridLines = "False";
                }
                else
                {
                    this.defaultChartProperties.CategoryAxisMajorGridLines.EnableMajorGridLines = "Auto";
                }
            }
            if (primaryAxis.ChartMinorGridLines != null)
            {
                if (primaryAxis.ChartMinorGridLines.Enabled == RDL.DOM.BooleanOptions.True)
                {
                    if (primaryAxis.ChartMinorGridLines.Enabled == RDL.DOM.BooleanOptions.True)
                    {
                        this.defaultChartProperties.CategoryAxisMinorGridLines.EnableMinorGridLines = "True";
                    }
                    if (primaryAxis.ChartMinorGridLines.Enabled == RDL.DOM.BooleanOptions.Auto)
                    {
                        this.defaultChartProperties.CategoryAxisMinorGridLines.EnableMinorGridLines = "Auto";
                    }
                    if (primaryAxis.ChartMinorGridLines.Style != null)
                    {
                        if (primaryAxis.ChartMinorGridLines.Style.Border != null)
                        {
                            this.defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesColor = primaryAxis.ChartMinorGridLines.Style.Border.Color;
                            this.defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesStyle = primaryAxis.ChartMinorGridLines.Style.Border.Style;

                            if (primaryAxis.ChartMinorGridLines.Style.Border.Width != null)
                            {
                                this.defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesWidth = primaryAxis.ChartMinorGridLines.Style.Border.Width.size;
                            }
                        }
                    }
                }
            }
            else
            {
                if (primaryAxis.ChartMinorGridLines.Enabled == RDL.DOM.BooleanOptions.False)
                {
                    this.defaultChartProperties.CategoryAxisMinorGridLines.EnableMinorGridLines = "False";
                }
                else
                {
                    this.defaultChartProperties.CategoryAxisMinorGridLines.EnableMinorGridLines = "Auto";
                }
            }

            if (primaryAxis.Reverse == true)
            {
                this.chartCategoryAxisProperties.ReverseDirection = "True";
            }
            else
            {
                this.chartCategoryAxisProperties.ReverseDirection = "False";
            }

            if (primaryAxis.HideLabels == true)
            {
                this.chartCategoryAxisProperties.HideAxisLabels = "True";
            }
            else
            {
                this.chartCategoryAxisProperties.FontAngle = primaryAxis.Angle.ToString();
                if (primaryAxis.Style != null)
                {
                    this.chartCategoryAxisProperties.FontFamily = primaryAxis.Style.FontFamily;
                    this.chartCategoryAxisProperties.FontSize = primaryAxis.Style.FontSize.size;
                    this.chartCategoryAxisProperties.FontColor = primaryAxis.Style.Color;
                    this.chartCategoryAxisProperties.FontWeight = primaryAxis.Style.FontWeight;
                }
            }
            if (primaryAxis.ChartAxisTitle != null)
            {
                CategoryAxisTitle = new TextBox();
                CategoryAxisTitle.TextChanged += new TextChangedEventHandler(CategoryAxisTitle_TextChanged);

                this.categoryAxisTitleProperties.Name = primaryAxis.ChartAxisTitle.Caption;
                this.categoryAxisTitleProperties.TitleAlignment = primaryAxis.ChartAxisTitle.Position.ToString();
                if (primaryAxis.ChartAxisTitle.Style != null)
                {
                    this.categoryAxisTitleProperties.FontFamily = primaryAxis.ChartAxisTitle.Style.FontFamily;
                    this.categoryAxisTitleProperties.FontSize = primaryAxis.ChartAxisTitle.Style.FontSize.size;
                    this.categoryAxisTitleProperties.FontStyle = primaryAxis.ChartAxisTitle.Style.FontStyle;
                    this.categoryAxisTitleProperties.FontColor = primaryAxis.ChartAxisTitle.Style.Color;
                }

                this.CategoryAxisTitle.BorderBrush = Brushes.Transparent;
                this.CategoryAxisTitle.Background = Brushes.Transparent;
                this.ChartArea.PrimaryAxis.Header = CategoryAxisTitle;
                ChartBase.ChartAreas[0].ChartCategoryAxes[0] = primaryAxis;
            }
            if (primaryAxis.Visible == RDL.DOM.BooleanOptions.False)
            {
                InnerChart.Areas[0].PrimaryAxis.AxisVisibility = System.Windows.Visibility.Collapsed;
                ChartArea.SetShowGridLines(ChartArea.PrimaryAxis, false);
            }

            ChartBase.ChartAreas[0].ChartCategoryAxes[0] = primaryAxis;
        }

        private void SecondaryCategoryAxisPropertiesChanged()
        {
            RDL.DOM.Chart ChartBase = this.ReportItem as RDL.DOM.Chart;
            RDL.DOM.ChartAxis primaryAxis1 = ChartBase.ChartAreas[0].ChartCategoryAxes[1] as RDL.DOM.ChartAxis;

            if (primaryAxis1.Style != null && primaryAxis1.Style.Border != null)
            {
                this.chartSecondaryCategoryAxisProperties.LineColor = primaryAxis1.Style.Border.Color;
                this.chartSecondaryCategoryAxisProperties.LineStyle = primaryAxis1.Style.Border.Style;
                if (primaryAxis1.Style.Border.Width != null)
                {
                    this.chartSecondaryCategoryAxisProperties.LineWidth = primaryAxis1.Style.Border.Width.size;
                }
                else
                {
                    this.chartSecondaryCategoryAxisProperties.LineWidth = "1pt";
                }
            }
            else
            {
                primaryAxis1.Style = new RDL.DOM.Style();
                primaryAxis1.Style.Border = new RDL.DOM.Border();
            }

            if (primaryAxis1.ChartMajorTickMarks != null)
            {
                this.chartSecondaryCategoryAxisProperties.EnableMajorTickMarks = primaryAxis1.ChartMajorTickMarks.Enabled.ToString();
                this.chartSecondaryCategoryAxisProperties.EnableMinorTickMarks = primaryAxis1.ChartMinorTickMarks.Enabled.ToString();

                if (primaryAxis1.ChartMajorTickMarks.Enabled == RDL.DOM.BooleanOptions.False)
                {
                    this.chartSecondaryCategoryAxisProperties.TickLength = "0";
                }
                else
                {
                    if (primaryAxis1.ChartMajorTickMarks.Style != null && primaryAxis1.ChartMajorTickMarks.Style.Border != null)
                    {
                        this.chartSecondaryCategoryAxisProperties.TickStyle = primaryAxis1.ChartMajorTickMarks.Style.Border.Style;
                        this.chartSecondaryCategoryAxisProperties.TickColor = primaryAxis1.ChartMajorTickMarks.Style.Border.Color;
                        if (primaryAxis1.ChartMajorTickMarks.Style.Border.Width != null)
                        {
                            this.chartSecondaryCategoryAxisProperties.TickLength = primaryAxis1.ChartMajorTickMarks.Style.Border.Width.size;
                        }
                        else
                        {
                            this.chartSecondaryCategoryAxisProperties.TickLength = "1pt";
                        }
                    }

                }
            }
            else
            {
                primaryAxis1.ChartMajorTickMarks = new RDL.DOM.ChartMajorTickMarks();
                primaryAxis1.ChartMajorTickMarks.Style = new RDL.DOM.Style();
                primaryAxis1.ChartMajorTickMarks.Style.Border = new RDL.DOM.Border();
                primaryAxis1.ChartMajorTickMarks.Enabled = RDL.DOM.BooleanOptions.False;
            }

            if (primaryAxis1.ChartMinorTickMarks != null)
            {
                if (primaryAxis1.ChartMinorTickMarks.Enabled == RDL.DOM.BooleanOptions.True)
                {
                    if (primaryAxis1.ChartMinorTickMarks.Style != null && primaryAxis1.ChartMinorTickMarks.Style.Border != null)
                    {
                        this.chartSecondaryCategoryAxisProperties.MinorTickStyle = primaryAxis1.ChartMinorTickMarks.Style.Border.Style;
                        this.chartSecondaryCategoryAxisProperties.MinorTickColor = primaryAxis1.ChartMinorTickMarks.Style.Border.Color;
                        if (primaryAxis1.ChartMinorTickMarks.Style.Border.Width == null)
                        {
                            this.chartSecondaryCategoryAxisProperties.TickWidth = "1pt";
                        }
                        else
                        {
                            this.chartSecondaryCategoryAxisProperties.TickWidth = primaryAxis1.ChartMinorTickMarks.Style.Border.Width.size;
                        }
                    }

                    this.secondaryXAxis.SmallTickSize = primaryAxis1.ChartMinorTickMarks.Length;
                    this.secondaryXAxis.SmallTicksPerInterval = 4;
                }
                else
                {
                    this.secondaryXAxis.SmallTickSize = 0;

                    if (primaryAxis1.ChartMinorTickMarks.Style == null)
                    {
                        primaryAxis1.ChartMinorTickMarks.Style = new RDL.DOM.Style();
                        primaryAxis1.ChartMinorTickMarks.Style.Border = new RDL.DOM.Border();
                        primaryAxis1.ChartMinorTickMarks.Enabled = RDL.DOM.BooleanOptions.False;
                    }
                }
            }
            else
            {
                primaryAxis1.ChartMinorTickMarks = new RDL.DOM.ChartMinorTickMarks();
                primaryAxis1.ChartMinorTickMarks.Style = new RDL.DOM.Style();
                primaryAxis1.ChartMinorTickMarks.Style.Border = new RDL.DOM.Border();
                primaryAxis1.ChartMinorTickMarks.Enabled = RDL.DOM.BooleanOptions.False;
            }            

            if (primaryAxis1.Reverse == true)
            {
                this.chartSecondaryCategoryAxisProperties.ReverseDirection = "True";
            }
            else
            {
                this.chartSecondaryCategoryAxisProperties.ReverseDirection = "False";
            }

            if (primaryAxis1.HideLabels == true)
            {
                this.chartSecondaryCategoryAxisProperties.HideAxisLabels = "True";
            }
            else
            {
                this.chartSecondaryCategoryAxisProperties.FontAngle = primaryAxis1.Angle.ToString();
                if (primaryAxis1.Style != null)
                {
                    this.chartSecondaryCategoryAxisProperties.FontFamily = primaryAxis1.Style.FontFamily;
                    this.chartSecondaryCategoryAxisProperties.FontSize = primaryAxis1.Style.FontSize.size;
                    this.chartSecondaryCategoryAxisProperties.FontColor = primaryAxis1.Style.Color;
                    this.chartSecondaryCategoryAxisProperties.FontWeight = primaryAxis1.Style.FontWeight;
                }
            }
            if (primaryAxis1.ChartAxisTitle != null)
            {
                this.SecondaryXAxisTitle = new TextBox();
                this.secondaryXAxisTitleProperties.Name = primaryAxis1.ChartAxisTitle.Caption;
                this.secondaryXAxisTitleProperties.TitleAlignment = primaryAxis1.ChartAxisTitle.Position.ToString();
                if (primaryAxis1.ChartAxisTitle.Style != null)
                {
                    this.secondaryXAxisTitleProperties.FontFamily = primaryAxis1.ChartAxisTitle.Style.FontFamily;
                    this.secondaryXAxisTitleProperties.FontSize = primaryAxis1.ChartAxisTitle.Style.FontSize.size;
                    this.secondaryXAxisTitleProperties.FontStyle = primaryAxis1.ChartAxisTitle.Style.FontStyle;
                    this.secondaryXAxisTitleProperties.FontColor = primaryAxis1.ChartAxisTitle.Style.Color;
                }

                this.SecondaryXAxisTitle.BorderBrush = Brushes.Transparent;
                this.SecondaryXAxisTitle.Background = Brushes.Transparent;
                this.secondaryXAxis.Header = this.SecondaryXAxisTitle;
                ChartBase.ChartAreas[0].ChartCategoryAxes[1] = primaryAxis1;
            }

            ChartBase.ChartAreas[0].ChartCategoryAxes[1] = primaryAxis1;
        }

        void ValueAxisTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            RDL.DOM.Chart ChartBase = this.ReportItem as RDL.DOM.Chart;
            if (ChartBase != null)
            {
                ChartBase.ChartAreas[0].ChartValueAxes[0].ChartAxisTitle.Caption = this.ValueAxisTitle.Text;
            }
        }

        void CategoryAxisTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            RDL.DOM.Chart ChartBase = this.ReportItem as RDL.DOM.Chart;

            if (ChartBase != null)
            {
                ChartBase.ChartAreas[0].ChartCategoryAxes[0].ChartAxisTitle.Caption = this.CategoryAxisTitle.Text;
            }

            TextBox titleTextBox = sender as TextBox;
            this.RemoveSelectionAdorner(titleTextBox);
        }

        private void ChartPropertiesChanged()
        {
            RDL.DOM.Chart ChartBase = this.ReportItem as RDL.DOM.Chart;
            this.chartProperties.Name = ChartBase.Name;

            if (ChartBase.Style != null)
            {
                if (ChartBase.Style.Border != null)
                {
                    if (ChartBase.Style.Border.Width != null)
                    {
                        if (ChartBase.Style.Border.Width.PixelValue != 1)
                            this.chartProperties.BorderWidth = ChartBase.Style.Border.Width.size;
                    }

                    if (ChartBase.Style.Border.Color.ToString() != "Black")
                    {
                        this.chartProperties.BorderColor = ChartBase.Style.Border.Color;
                    }
                    if (ChartBase.Style.Border.Style != null)
                    {
                        this.chartProperties.BorderStyle = ChartBase.Style.Border.Style;
                    }
                }

                if (ChartBase.Style.BackgroundColor.ToString() != "Transparent")
                {
                    this.chartProperties.ChartBackground = ChartBase.Style.BackgroundColor;
                }
            }

            if (ChartBase.ChartAreas[0].Style != null)
            {
                if (string.IsNullOrEmpty(ChartBase.ChartAreas[0].Style.BackgroundGradientEndColor))
                {
                    this.chartProperties.FillStyle = "Solid";
                    if (ChartBase.ChartAreas[0].Style.BackgroundColor != "Transparent")
                        this.chartProperties.PrimaryColor = ChartBase.ChartAreas[0].Style.BackgroundColor;
                }
                else
                {
                    this.chartProperties.FillStyle = "Gradient";
                    if (ChartBase.ChartAreas[0].Style.BackgroundGradientType == RDL.DOM.BackgroundGradientTypes.LeftRight)
                    {
                        this.chartProperties.GradientStyle = "LeftRight";
                    }
                    else if (ChartBase.ChartAreas[0].Style.BackgroundGradientType == RDL.DOM.BackgroundGradientTypes.TopBottom)
                    {
                        this.chartProperties.GradientStyle = "TopBottom";
                    }
                    else
                        this.chartProperties.GradientStyle = "None";
                    this.chartProperties.PrimaryColor = ChartBase.ChartAreas[0].Style.BackgroundColor;
                    this.chartProperties.SecondaryColor = ChartBase.ChartAreas[0].Style.BackgroundGradientEndColor;
                }
            }
        }

        private void SeriesPropertiesChanged()
        {
            RDL.DOM.Chart ChartBase = this.ReportItem as RDL.DOM.Chart;
            int i = 0;

            for (int j = 0; j < chartSeriesPropertiesCollection.Count; j++)
            {
                if (chartProperties.IsInternalPropertyChange == true)
                {
                    chartSeriesPropertiesCollection[j].IsInternalPropertyChange = true;
                }
                else
                {
                    chartSeriesPropertiesCollection[j].IsInternalPropertyChange = false;
                }
            }

            foreach (ChartSeries chartseries in InnerChart.Areas[0].Series)
            {
                RDL.DOM.ChartSeries chartSeries1 = ChartBase.ChartData.ChartSeriesCollection[i] as RDL.DOM.ChartSeries;
                seriesName = chartSeries1.Name;

                seriesCount = i;
                this.chartSeriesPropertiesCollection[i].ChartType = chartseries.Type.ToString();
                this.chartSeriesPropertiesCollection[i].CategoryAxisName = chartSeries1.CategoryAxisName;
                this.chartSeriesPropertiesCollection[i].ValueAxisName = chartSeries1.ValueAxisName;

                if (chartSeries1.ChartDataPoints != null)
                {
                    if (chartSeries1.ChartDataPoints.Count != 0)
                    {
                        if (chartSeries1.ChartDataPoints[0].Style != null)
                        {
                            if (chartSeries1.ChartDataPoints[0].Style.Color != "Black" && chartSeries1.ChartDataPoints[0].Style.Color != null && chartSeries1.ChartDataPoints[0].Style.Color!="System.Windows.Media.LinearGradientBrush" )
                            {
                                this.chartSeriesPropertiesCollection[i].SeriesColor = chartSeries1.ChartDataPoints[0].Style.Color.ToString();
                            }
                            else if (chartseries.Interior.ToString() == "System.Windows.Media.LinearGradientBrush")
                            {
                                this.chartSeriesPropertiesCollection[i].SeriesColor = "";
                            }
                            else
                            {
                                chartSeries1.ChartDataPoints[0].Style.Color = chartseries.Interior.ToString();
                                this.chartSeriesPropertiesCollection[i].SeriesColor = chartseries.Interior.ToString();
                            }

                            if (chartSeries1.ChartDataPoints[0].Style.Border != null && chartSeries1.ChartDataPoints[0].Style.Border.Width != null)
                            {
                                this.chartSeriesPropertiesCollection[i].BorderWidth = chartSeries1.ChartDataPoints[0].Style.Border.Width.size;
                                this.chartSeriesPropertiesCollection[i].BorderColor = chartSeries1.ChartDataPoints[0].Style.Border.Color.ToString();
                            }
                            else
                            {
                                chartSeries1.ChartDataPoints[0].Style.Border = new RDL.DOM.Border();
                                this.chartSeriesPropertiesCollection[i].BorderWidth = "1pt";
                            }
                        }
                        else
                        {
                            chartSeries1.ChartDataPoints[0].Style = new RDL.DOM.Style();
                            chartSeries1.ChartDataPoints[0].Style.Border = new RDL.DOM.Border();
                        }

                        if (chartSeries1.ChartDataPoints[0].ChartMarker != null)
                        {
                            switch (chartSeries1.ChartDataPoints[0].ChartMarker.Type)
                            {
                                case "None":
                                    {
                                        this.chartSeriesPropertiesCollection[i].AdornmentType = "None";
                                        break;
                                    }
                                case "Cross":
                                    {
                                        this.chartSeriesPropertiesCollection[i].AdornmentType = "Cross";
                                        break;
                                    }
                                case "Diamond":
                                    {
                                        this.chartSeriesPropertiesCollection[i].AdornmentType = "Diamond";
                                        break;
                                    }
                                case "Ellipse":
                                    {
                                        this.chartSeriesPropertiesCollection[i].AdornmentType = "Ellipse";
                                        break;
                                    }
                                case "Hexagon":
                                    {
                                        this.chartSeriesPropertiesCollection[i].AdornmentType = "Hexagon";
                                        break;
                                    }
                                case "InvertedTriangle":
                                    {
                                        this.chartSeriesPropertiesCollection[i].AdornmentType = "InvertedTriangle";
                                        break;
                                    }
                                case "Pentagon":
                                    {
                                        this.chartSeriesPropertiesCollection[i].AdornmentType = "Pentagon";
                                        break;
                                    }
                                case "Square":
                                case "Auto":      
                                    {
                                        this.chartSeriesPropertiesCollection[i].AdornmentType = "Square";
                                        break;
                                    }
                                case "Plus":
                                    {
                                        this.chartSeriesPropertiesCollection[i].AdornmentType = "Plus";
                                        break;
                                    }
                                case "Triangle":
                                    {
                                        this.chartSeriesPropertiesCollection[i].AdornmentType = "Triangle";
                                        break;
                                    }
                                default:
                                    {
                                        this.chartSeriesPropertiesCollection[i].AdornmentType = "None";
                                        break;
                                    }
                            }

                            if (this.chartSeriesPropertiesCollection[i].AdornmentType != "None")
                            {
                                if (chartSeries1.ChartDataPoints[0].ChartMarker.Size != null)
                                {
                                    this.chartSeriesPropertiesCollection[i].Size = chartSeries1.ChartDataPoints[0].ChartMarker.Size.size;
                                }
                                if (chartSeries1.ChartDataPoints[0].ChartMarker.Style != null)
                                {
                                    this.chartSeriesPropertiesCollection[i].AdornmentColor = chartSeries1.ChartDataPoints[0].ChartMarker.Style.Color.ToString();
                                }
                            }
                        }
                        else
                        {
                            chartSeries1.ChartDataPoints[0].ChartMarker = new RDL.DOM.ChartMarker();
                            chartSeries1.ChartDataPoints[0].ChartMarker.Type = "None";
                        }
                        chartseries.AdornmentsInfo.SymbolTemplate = null;

                        if (chartSeries1.ChartDataPoints[0].ChartDataLabel != null)
                        {
                            if (chartSeries1.ChartDataPoints[0].ChartDataLabel.Visible == true)
                            {
                                this.chartSeriesPropertiesCollection[i].ShowDataLabels = "True";
                            }
                            else
                            {
                                this.chartSeriesPropertiesCollection[i].ShowDataLabels = "False";
                            }
                            this.chartSeriesPropertiesCollection[i].DataLabelsPosition = chartSeries1.ChartDataPoints[0].ChartDataLabel.Position.ToString();
                        }
                    }
                }

                ChartBase.ChartData.ChartSeriesCollection[i] = chartSeries1;
                i++;
            }

            for (int j = 0; j < chartSeriesPropertiesCollection.Count; j++)
            {
                chartSeriesPropertiesCollection[j].IsInternalPropertyChange = false;
            }                
        }

        internal string GetChartType(RDL.DOM.VisualizationType chartType, RDL.DOM.VisualizationSubType subType)
        {
            switch ((RDL.DOM.VisualizationType)chartType)
            {
                case RDL.DOM.VisualizationType.Shape:
                    {
                        switch (subType)
                        {
                            case RDL.DOM.VisualizationSubType.Plain:
                                return "Pie";
                            case RDL.DOM.VisualizationSubType.Funnel:
                                return "Funnel";
                            case RDL.DOM.VisualizationSubType.Pyramid:
                                return "Pyramid";
                            case RDL.DOM.VisualizationSubType.Doughnut:
                                return "Doughnut";
                            case RDL.DOM.VisualizationSubType.ExplodedDoughnut:     
                                this.IsExploded = true;                               
                                return "Doughnut";                            
                            case RDL.DOM.VisualizationSubType.ExplodedPie:   
                                this.IsExploded = true;  
                                return "Pie";                        
                            default:
                                return "Pie";
                        }
                    }
                case RDL.DOM.VisualizationType.Bar:
                    {
                        switch (subType)
                        {
                            case RDL.DOM.VisualizationSubType.Stacked:
                                return "StackingBar";
                            case RDL.DOM.VisualizationSubType.PercentStacked:
                                return "StackingBar100";
                            default:
                                return "Bar";
                        }
                    }
                case RDL.DOM.VisualizationType.Line:
                    {
                        switch (subType)
                        {
                            case RDL.DOM.VisualizationSubType.Stepped:
                                return "StepLine";
                            case RDL.DOM.VisualizationSubType.Smooth:
                                return "FastLine";
                            default:
                                return "Line";
                        }
                    }
                case RDL.DOM.VisualizationType.Polar:
                    {
                        switch (subType)
                        {
                            case RDL.DOM.VisualizationSubType.Radar:
                                return "Polar";
                            default:
                                return "Radar";
                        }
                    }
                case RDL.DOM.VisualizationType.Range:
                    {
                        switch (subType)
                        {
                            case RDL.DOM.VisualizationSubType.Plain:
                                return "RangeArea";
                            case RDL.DOM.VisualizationSubType.Candlestick:
                                return "Candle";
                            case RDL.DOM.VisualizationSubType.Stock:
                                return "HiLoOpenClose";
                            case RDL.DOM.VisualizationSubType.BoxPlot:
                                return "BoxAndWhisker";
                            default:
                                return "RangeArea";
                        }
                    }
                case RDL.DOM.VisualizationType.Area:
                    {
                        switch (subType)
                        {
                            case RDL.DOM.VisualizationSubType.Stacked:
                                return "StackingArea";
                            case RDL.DOM.VisualizationSubType.Smooth:
                                return "SplineArea";
                            case RDL.DOM.VisualizationSubType.PercentStacked:
                                return "StackingArea100";
                            default:
                                return "Area";
                        }
                    }
                case RDL.DOM.VisualizationType.Scatter:
                    {
                        switch (subType)
                        {
                            case RDL.DOM.VisualizationSubType.Bubble:
                                return "Bubble";
                            default:
                                return "Scatter";
                        }
                    }
                default:
                    {
                        switch (subType)
                        {
                            case RDL.DOM.VisualizationSubType.Stacked:
                                return "StackingColumn";
                            case RDL.DOM.VisualizationSubType.PercentStacked:
                                return "StackingColumn100";
                            default:
                                return "Column";
                        }
                    }
            }
        }

        private void ValueTitlePropertiesChanged()
        {
            RDL.DOM.Chart chartBase = this.ReportItem as RDL.DOM.Chart;
            RDL.DOM.ChartAxis secondaryAxis = chartBase.ChartAreas[0].ChartValueAxes[0] as RDL.DOM.ChartAxis;

            this.valueAxisTitleProperties.Name = secondaryAxis.ChartAxisTitle.Caption;

            if (secondaryAxis.ChartAxisTitle.Style != null)
            {
                this.valueAxisTitleProperties.FontFamily = secondaryAxis.ChartAxisTitle.Style.FontFamily;
                this.valueAxisTitleProperties.FontSize = secondaryAxis.ChartAxisTitle.Style.FontSize.size;
                this.valueAxisTitleProperties.FontStyle = secondaryAxis.ChartAxisTitle.Style.FontStyle;
                this.valueAxisTitleProperties.FontColor = secondaryAxis.ChartAxisTitle.Style.Color;
                this.valueAxisTitleProperties.TitleAlignment = secondaryAxis.ChartAxisTitle.Position.ToString();
            }

            this.ValueAxisTitle.BorderBrush = Brushes.Transparent;
            this.ValueAxisTitle.Background = Brushes.Transparent;
            this.ChartArea.SecondaryAxis.Header = ValueAxisTitle;

            this.ChangeContextMenuVisibility();
        }

        private void SecondaryValueTitlePropertiesChanged()
        {
            RDL.DOM.Chart chartBase = this.ReportItem as RDL.DOM.Chart;
            RDL.DOM.ChartAxis secondaryAxis1 = chartBase.ChartAreas[0].ChartValueAxes[1] as RDL.DOM.ChartAxis;

            this.secondaryYAxisTitleProperties.Name = secondaryAxis1.ChartAxisTitle.Caption;
            this.secondaryYAxisTitleProperties.TitleAlignment = secondaryAxis1.ChartAxisTitle.Position.ToString();

            if (secondaryAxis1.ChartAxisTitle.Style != null)
            {
                this.secondaryYAxisTitleProperties.FontFamily = secondaryAxis1.ChartAxisTitle.Style.FontFamily;
                this.secondaryYAxisTitleProperties.FontSize = secondaryAxis1.ChartAxisTitle.Style.FontSize.size;
                this.secondaryYAxisTitleProperties.FontStyle = secondaryAxis1.ChartAxisTitle.Style.FontStyle;
                this.secondaryYAxisTitleProperties.FontColor = secondaryAxis1.ChartAxisTitle.Style.Color;
            }

            this.SecondaryYAxisTitle.BorderBrush = Brushes.Transparent;
            this.SecondaryYAxisTitle.Background = Brushes.Transparent;
            this.secondaryYAxis.Header = SecondaryYAxisTitle;
        }

        private void CategoryTitlePropertiesChanged()
        {
            RDL.DOM.Chart ChartBase = this.ReportItem as RDL.DOM.Chart;
            RDL.DOM.ChartAxis primaryAxis = ChartBase.ChartAreas[0].ChartCategoryAxes[0] as RDL.DOM.ChartAxis;

            this.categoryAxisTitleProperties.Name = primaryAxis.ChartAxisTitle.Caption;
            this.categoryAxisTitleProperties.TitleAlignment = primaryAxis.ChartAxisTitle.Position.ToString();

            if (primaryAxis.ChartAxisTitle.Style != null)
            {
                this.categoryAxisTitleProperties.FontFamily = primaryAxis.ChartAxisTitle.Style.FontFamily;
                this.categoryAxisTitleProperties.FontSize = primaryAxis.ChartAxisTitle.Style.FontSize.size;
                this.categoryAxisTitleProperties.FontStyle = primaryAxis.ChartAxisTitle.Style.FontStyle;
                this.categoryAxisTitleProperties.FontColor = primaryAxis.ChartAxisTitle.Style.Color;
            }
            CategoryAxisTitle.BorderBrush = Brushes.Transparent;
            CategoryAxisTitle.Background = Brushes.Transparent;
            this.ChartArea.PrimaryAxis.Header = CategoryAxisTitle;

            ChangeContextMenuVisibility();
        }

        private void SecondaryCategoryTitlePropertiesChanged()
        {
            RDL.DOM.Chart ChartBase = this.ReportItem as RDL.DOM.Chart;
            RDL.DOM.ChartAxis primaryAxis1 = ChartBase.ChartAreas[0].ChartCategoryAxes[1] as RDL.DOM.ChartAxis;

            this.secondaryXAxisTitleProperties.Name = primaryAxis1.ChartAxisTitle.Caption;
            this.secondaryXAxisTitleProperties.TitleAlignment = primaryAxis1.ChartAxisTitle.Position.ToString();

            if (primaryAxis1.ChartAxisTitle.Style != null)
            {
                this.secondaryXAxisTitleProperties.FontFamily = primaryAxis1.ChartAxisTitle.Style.FontFamily;
                this.secondaryXAxisTitleProperties.FontSize = primaryAxis1.ChartAxisTitle.Style.FontSize.size;
                this.secondaryXAxisTitleProperties.FontStyle = primaryAxis1.ChartAxisTitle.Style.FontStyle;
                this.secondaryXAxisTitleProperties.FontColor = primaryAxis1.ChartAxisTitle.Style.Color;
            }
            this.SecondaryXAxisTitle.BorderBrush = Brushes.Transparent;
            this.SecondaryXAxisTitle.Background = Brushes.Transparent;
            this.secondaryXAxis.Header = this.SecondaryXAxisTitle;
        }

        private void ChartTitlePropertiesChanged()
        {
            RDL.DOM.Chart ChartBase = this.ReportItem as RDL.DOM.Chart;
            if (this.Width != 0)
            {
                ChartTitle.Width = this.Width - 30;
            }
            this.titleProperties.Name = ChartBase.ChartTitles[0].Caption;
            if (ChartBase.ChartTitles[0].Style != null)
            {
                this.titleProperties.TitleFontColor = ChartBase.ChartTitles[0].Style.Color;
                this.titleProperties.TitleBackFill = ChartBase.ChartTitles[0].Style.BackgroundColor;
                if (ChartBase.ChartTitles[0].Style.FontStyle != null && ChartBase.ChartTitles[0].Style.FontStyle.ToString() != "Default")
                {
                    this.titleProperties.TitleFontStyle = ChartBase.ChartTitles[0].Style.FontStyle;
                }
                else
                {
                    this.titleProperties.TitleFontStyle = "Normal";
                }
                if (ChartBase.ChartTitles[0].Style.FontSize != null && ChartBase.ChartTitles[0].Style.FontSize.ToString() != "16pt")
                {
                    this.titleProperties.TitleFontSize = ChartBase.ChartTitles[0].Style.FontSize.size;
                }
                else
                {
                    this.titleProperties.TitleFontSize = "16pt";
                }
            }

            ChartTitle.BorderBrush = Brushes.Transparent;
            this.InnerChart.Header = ChartTitle;
        }

        private void legendPropertiesChanged()
        {
            RDL.DOM.Chart ChartBase = this.ReportItem as RDL.DOM.Chart;
            this.chartLegendProperties.Name = ChartBase.ChartLegends[0].Name;
            if (ChartBase.ChartLegends[0].Hidden == true)
                this.chartLegendProperties.Hidden = "False";
            else
                this.chartLegendProperties.Hidden = "True";
            switch (ChartBase.ChartLegends[0].Position)
            {
                case RDL.DOM.Positions.TopCenter:
                    {
                        this.chartLegendProperties.Position = "TopCenter";
                        break;
                    }
                case RDL.DOM.Positions.RightCenter:
                    {
                        this.chartLegendProperties.Position = "RightCenter";
                        break;
                    }
                case RDL.DOM.Positions.BottomCenter:
                    {
                        this.chartLegendProperties.Position = "BottomCenter";
                        break;
                    }
                case RDL.DOM.Positions.LeftCenter:
                    {
                        this.chartLegendProperties.Position = "LeftCenter";
                        break;
                    }
                default:
                    {
                        this.chartLegendProperties.Position = "TopCenter";
                        break;
                    }
            }
            switch (ChartBase.ChartLegends[0].Layout)
            {
                case RDL.DOM.Layout.Row:
                    {
                        int i = 0;
                        foreach (ChartSeries chartSeries in InnerChart.Areas[0].Series)
                        {
                            i++;
                        }
                        ChartLegand.ColumnsCount = i;
                        ChartLegand.RowsCount = 1;
                        break;
                    }
                case RDL.DOM.Layout.Column:
                    {
                        int i = 0;
                        foreach (ChartSeries chartSeries in InnerChart.Areas[0].Series)
                        {
                            i++;
                        }
                        ChartLegand.ColumnsCount = 1;
                        ChartLegand.RowsCount = i;
                        break;
                    }
                default:
                    {
                        int i = 0;
                        foreach (ChartSeries chartSeries in InnerChart.Areas[0].Series)
                        {
                            i++;
                        }
                        ChartLegand.ColumnsCount = i;
                        ChartLegand.RowsCount = 1;
                        this.chartLegendProperties.Layout = "Row";
                        break;
                    }
            }

            if (ChartBase.ChartLegends[0].Style != null)
            {
                this.chartLegendProperties.BackFill = ChartBase.ChartLegends[0].Style.BackgroundColor;
                this.chartLegendProperties.FontFamily = ChartBase.ChartLegends[0].Style.FontFamily;
                if (ChartBase.ChartLegends[0].Style.FontSize != null)
                    this.chartLegendProperties.FontSize = ChartBase.ChartLegends[0].Style.FontSize.size;
                this.chartLegendProperties.FontStyle = ChartBase.ChartLegends[0].Style.FontStyle;
                this.chartLegendProperties.FontColor = ChartBase.ChartLegends[0].Style.Color;
                if (ChartBase.ChartLegends[0].Style.Border != null)
                {
                    if (ChartBase.ChartLegends[0].Style.Border.Width != null)
                        this.chartLegendProperties.BorderThickness = ChartBase.ChartLegends[0].Style.Border.Width.size;
                    if (ChartBase.ChartLegends[0].Style.Border.Color != null)
                        this.chartLegendProperties.LegendBorderColor = ChartBase.ChartLegends[0].Style.Border.Color;
                }
            }
            if (ChartBase.ChartLegends[0].ChartLegendTitle != null && !string.IsNullOrEmpty(ChartBase.ChartLegends[0].ChartLegendTitle.Caption))
            {
                LegendTextbox.Visibility = Visibility.Visible;
                LegendTextbox.BorderThickness = new Thickness(0);
                LegendTextbox.TextAlignment = TextAlignment.Center;
                LegendTextbox.Text = ChartBase.ChartLegends[0].ChartLegendTitle.Caption;
                if (ChartBase.ChartTitles.Count>0 && ChartBase.ChartTitles[0].Style != null)
                {
                    this.legendTitleProperties.FontColor = ChartBase.ChartLegends[0].Style.Color;
                    this.legendTitleProperties.BackFill = ChartBase.ChartLegends[0].Style.BackgroundColor;
                    this.legendTitleProperties.FontFamily = ChartBase.ChartLegends[0].Style.FontFamily;
                    if (ChartBase.ChartTitles[0].Style.FontStyle != null)
                    {
                        this.legendTitleProperties.FontStyle = ChartBase.ChartLegends[0].Style.FontStyle;
                    }
                    else
                    {
                        this.legendTitleProperties.FontStyle = "Defaulr";
                    }
                    if (ChartBase.ChartTitles[0].Style.FontSize != null && ChartBase.ChartLegends[0].Style.FontSize.ToString() != "8pt")
                    {
                        this.legendTitleProperties.FontSize = ChartBase.ChartLegends[0].Style.FontSize.size;
                    }
                    else
                    {
                        this.legendTitleProperties.FontSize = "8pt";
                    }
                }

            }
            else
            {
                LegendTextbox.Visibility = Visibility.Collapsed;
            }
        }

        private void InitializeOldChart()
        {
            if (this.DataSets.Count > 0)
            {
                ChartArea.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(ChartArea_PreviewMouseLeftButtonDown);
                ChartArea.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ChartGrid_PreviewMouseLeftButtonDown);
            }

            RDL.DOM.Chart ChartBase = this.ReportItem as RDL.DOM.Chart;
            this.ChartSeries = new ChartSeries();
            int j = 0;

            if (ChartBase != null)
            {
                if (ChartBase.ChartData != null)
                {
                    if ((ChartBase.ChartData.ChartSeriesCollection != null) && (ChartBase.ChartData.ChartSeriesCollection.Count != 0))
                    {
                        foreach (RDL.DOM.ChartSeries chartSeries1 in ChartBase.ChartData.ChartSeriesCollection)
                        {
                            ChartSeries chartSeries = new ChartSeries();
                            chartSeries.Interior = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.seriesColorCollection[j]));
                            chartSeries.DataSource = DefaultChartData1();
                            chartSeries.BindingPathX = "SeriesID";
                            chartSeries.BindingPathsY = new string[] { "SeriesValue1" };
                            if (ChartBase.ChartSeriesHierarchy.ChartMembers[0].ChartMembers != null && ChartBase.ChartSeriesHierarchy.ChartMembers[0].ChartMembers.Count > 0)
                            {
                                chartSeries.Label = ChartBase.ChartSeriesHierarchy.ChartMembers[0].ChartMembers[j].Label;
                            }
                            else if (ChartBase.ChartSeriesHierarchy.ChartMembers != null && ChartBase.ChartSeriesHierarchy.ChartMembers.Count > 0)
                            {
                                chartSeries.Label = ChartBase.ChartSeriesHierarchy.ChartMembers[j].Label;
                            }
                            else
                            {
                                chartSeries.Label = chartSeries1.Name;
                            }
                            if (string.IsNullOrEmpty(chartSeries.Label))
                            {
                                chartSeries.Label = chartSeries1.Name;
                            }
                            chartSeries.Name = chartSeries1.Name;                           
                            chartSeries.MouseLeftButtonDown += new ChartMouseEventHandler(ChartSeries_MouseLeftButtonDown);
                            chartSeries.MouseRightButtonDown += new ChartMouseEventHandler(chartSeries_MouseRightButtonDown);
                            this.chartSeriesProperties = new Editors.ChartSeriesProperties();
                            this.chartSeriesProperties.IsInternalPropertyChange = true;
                            this.chartSeriesProperties.Name = chartSeries.Name;
                            this.chartSeriesProperties.IsInternalPropertyChange = false;
                            this.chartSeriesProperties.PropertyChanged += new PropertyChangedEventHandler(OnChartSeriesPropertyChanged);
                            this.chartSeriesProperties.PropertyChanging += new PropertyChangingEventHandler(chartSeriesProperties_PropertyChanging);
                            this.chartSeriesPropertiesCollection.Add(chartSeriesProperties);
                            string type = this.GetChartType(ChartBase.ChartData.ChartSeriesCollection[j].Type, ChartBase.ChartData.ChartSeriesCollection[j].Subtype);
                            ChartTypes chartType = (ChartTypes)Enum.Parse(typeof(ChartTypes), type, true);
                            chartSeries.Type = chartType;
                            chartSeries1.Name = chartSeries.Name;
                            seriesCount = j;
                            InnerChart.Areas[0].Series.Add(chartSeries);                           
                            j++;
                        }
                    }
                }
            }

            ChartArea.PrimaryAxis.ContentPath = "SeriesName";
            ChartArea.PrimaryAxis.PositionPath = "SeriesId";
            ChartArea.PrimaryAxis.LabelsSource = DefaultChartData1();
        }

        # endregion

        # region Constructors

        internal ChartControl(RDL.DOM.DataSets dataSets, RDL.DOM.DataSources dataSources, Controls.ChartControlType chartControlType)
        {
            this.DataSets = dataSets;
            this.DataSources = dataSources;
            this.ChartControlType = chartControlType;
            this.chartProperties = new Syncfusion.Windows.Reports.Designer.Editors.ChartProperties();
            this.chartSeriesPropertiesCollection = new Editors.ChartSeriesPropertiesCollection();
            this.titleProperties = new Editors.ChartTitleProperties();
            this.categoryAxisTitleProperties = new Editors.CategoryAxisTitleProperties();
            this.secondaryXAxisTitleProperties = new Editors.CategoryAxisTitleProperties();
            this.chartLegendProperties = new Editors.ChartLegendProperties();
            this.valueAxisTitleProperties = new Editors.ValueAxisTitleProperties();
            this.secondaryYAxisTitleProperties = new Editors.ValueAxisTitleProperties();
            this.chartCategoryAxisProperties = new Editors.CategoryAxisProperties();
            this.chartSecondaryCategoryAxisProperties = new Editors.CategoryAxisProperties();
            this.chartValueAxisProperties = new Editors.ValueAxisProperties();
            this.chartSecondaryValueAxisProperties = new Editors.ValueAxisProperties();
            this.defaultChartProperties = new Editors.DefaultChartProperties();
            this.legendTitleProperties = new Editors.LegendTitleProperties();
            this.seriesColorCollection = new List<string>();
            this.WireEvents();
            this.error_title = SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner");
        }



        # endregion

        # region Members

        private bool IsInitiallyBinded { get; set; }
        private List<string> CategoryBindingList { get; set; }
        private List<string> DataBindingList { get; set; }
        private List<string> SeriesBindingList { get; set; }
        internal string DataSetName { get; set; }
        internal RDL.DOM.ReportDefinition Report { get; set; }
        internal RDL.DOM.DataSets DataSets { get; set; }
        internal RDL.DOM.DataSources DataSources { get; set; }
        internal Syncfusion.Windows.Chart.Chart InnerChart { get; set; }
        internal ChartLegend ChartLegand { get; set; }
        internal ChartAreasCollection Areas { get; set; }
        internal ChartSeriesCollection Series { get; set; }
        internal ChartArea ChartArea { get; set; }
        internal ChartSeries ChartSeries { get; set; }
        internal ChartDock ChartDock { get; set; }
        internal ChartControlType ChartControlType { get; set; }
        internal DesignerDashStyleBorder InnerBorder { get; set; }
        private TextBox ChartTitle { get; set; }
        private TextBox ValueAxisTitle { get; set; }
        private TextBox CategoryAxisTitle { get; set; }
        private TextBox SecondaryXAxisTitle { get; set; }
        private TextBox SecondaryYAxisTitle { get; set; }
        private TextBox LegendTextbox { get; set; }
        private Label ValueDataField { get; set; }
        private Label ColumnDataField { get; set; }
        private Label SeriesDataField { get; set; }
        internal ReportingConvertorUtil propertyValueConvertor = new ReportingConvertorUtil();

        //private Popup valuePopup;
        //private Popup seriesPopup;
        //private Popup categoryPopup;

        internal List<string> ValueItems
        {
            get
            {
                return GetPanelChildrens(this.ValuePanel);
            }
        }

        internal List<string> ColumnItems
        {
            get
            {
                return GetPanelChildrens(this.ColumnPanel);
            }
        }

        internal List<string> SereiesItems
        {
            get
            {
                return GetPanelChildrens(this.Seriespanel);
            }
        }

        public string ChartType
        {
            get;
            set;
        }

        public string LegendPostion
        {
            get;
            set;
        }

        public Color ChartAreaColor
        {
            get;
            set;
        }

        public Color CharAreaColorPrimary
        {
            get;
            set;
        }

        public bool IsCheckedGradiant = true;

        private MenuItem AddTitle { get; set; }
        private MenuItem AddLegend { get; set; }
        private MenuItem ShowCategoryTitle { get; set; }
        private MenuItem ShowLegendTitle { get; set; }
        private MenuItem ShowValueTitle { get; set; }
        private MenuItem ShowDatalabels { get; set; }
        private MenuItem InnerDeleteLegend { get; set; }
        private MenuItem DeleteChart { get; set; }
        private MenuItem MenuChartProperties { get; set; }
        private MenuItem SeriesProperties { get; set; }
        private MenuItem LegendProperties { get; set; }
        private MenuItem InnerLegendProperties { get; set; }
        private MenuItem ValueAxisProperties { get; set; }
        private MenuItem CategoryAxisProperties { get; set; }
        private MenuItem ChartTitleProperties { get; set; }
        private MenuItem ConvertToFullChart { get; set; }
        private MenuItem CategoryTitleProperties { get; set; }
        private MenuItem ValueTitleProperties { get; set; }
        private MenuItem ChartHeaderProperties { get; set; }
        private MenuItem DeleteChartTitle { get; set; }
        private System.Windows.Controls.Separator Separator1 { get; set; }
        private System.Windows.Controls.Separator Separator2 { get; set; }
        public StackPanel ValuePanel { get; set; }
        public StackPanel ColumnPanel { get; set; }
        public StackPanel Seriespanel { get; set; }
        private Border ValueInnerBorder { get; set; }
        private Border ColumnInnerBorder { get; set; }
        private Border SeriesInnerBorder { get; set; }
        private ContextMenu ChartContextMenu { get; set; }

        # endregion

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            InnerChart = GetTemplateChild("PART_InnerChart") as Chart.Chart;
            InnerBorder = GetTemplateChild("PART_InnerBorder") as DesignerDashStyleBorder;
            ChartLegand = GetTemplateChild("PART_ChartLegand") as Chart.ChartLegend;
            ValuePanel = GetTemplateChild("PART_ValuePanel") as StackPanel;
            ValuePanel.PreviewDrop += new DragEventHandler(ValuePanel_PreviewDrop);
            ValuePanel.PreviewDragOver += new DragEventHandler(Panel_PreviewDragOver);
            ColumnPanel = GetTemplateChild("PART_ColumnPanel") as StackPanel;
            ColumnPanel.PreviewDrop += new DragEventHandler(ColumnPanel_PreviewDrop);
            ColumnPanel.PreviewDragOver += new DragEventHandler(Panel_PreviewDragOver);
            Seriespanel = GetTemplateChild("PART_SeriesPanel") as StackPanel;
            Seriespanel.PreviewDrop += new DragEventHandler(Seriespanel_PreviewDrop);
            Seriespanel.PreviewDragOver += new DragEventHandler(Panel_PreviewDragOver);
            SeriesDataField = GetTemplateChild("PART_SeriesDataField") as Label;
            ValueDataField = GetTemplateChild("PART_ValueDataField") as Label;
            ValueDataField.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(ValueDataField_MouseLeftButtonUp);
            ValueDataField.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(ValueDataField_MouseRightButtonDown);
            ColumnDataField = GetTemplateChild("PART_ColumnDataField") as Label;
            ColumnDataField.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(columnDataField_MouseLeftButtonUp);
            ColumnDataField.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(ColumnDataField_MouseRightButtonDown);
            SeriesDataField.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(SeriesDataField_MouseLeftButtonUp);
            SeriesDataField.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(SeriesDataField_MouseRightButtonDown);
            InnerChart.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(InnerChart_MouseRightButtonDown);
            ShowCategoryTitle = GetTemplateChild("PART_ShowCategoryTitle") as MenuItem;
            ShowLegendTitle = GetTemplateChild("PART_ShowLegendTitle") as MenuItem;
            ShowValueTitle = GetTemplateChild("PART_ShowValueTitle") as MenuItem;
            AddTitle = GetTemplateChild("PART_AddChartTitle") as MenuItem;
            AddLegend = GetTemplateChild("PART_AddLegend") as MenuItem;
            InnerDeleteLegend = GetTemplateChild("PART_DeleteLegend1") as MenuItem;
            DeleteChart = GetTemplateChild("PART_DeleteProperty") as MenuItem;
            MenuChartProperties = GetTemplateChild("PART_MenuChartProperties") as MenuItem;
            SeriesProperties = GetTemplateChild("PART_SeriesProperties") as MenuItem;
            LegendProperties = GetTemplateChild("PART_LegendProperties") as MenuItem;
            InnerLegendProperties = GetTemplateChild("PART_LegendProperties1") as MenuItem;
            ValueAxisProperties = GetTemplateChild("PART_ValueAxisProperties") as MenuItem;
            CategoryAxisProperties = GetTemplateChild("PART_CategoryAxisProperties") as MenuItem;
            ConvertToFullChart = GetTemplateChild("PART_MenuConvertToFullChart") as MenuItem;
            ChartTitleProperties = GetTemplateChild("PART_ChartTitleProperties") as MenuItem;
            CategoryTitleProperties = GetTemplateChild("PART_CategoryTitleProperties") as MenuItem;
            ValueTitleProperties = GetTemplateChild("PART_ValueTitleProperties") as MenuItem;
            ChartHeaderProperties = GetTemplateChild("PART_TitleProperties") as MenuItem;
            DeleteChartTitle = GetTemplateChild("PART_DeleteTitle") as MenuItem;
            ValueInnerBorder = GetTemplateChild("PART_ValueInnerBorder") as Border;
            ColumnInnerBorder = GetTemplateChild("PART_ColumnInnerBorder") as Border;
            SeriesInnerBorder = GetTemplateChild("PART_SeriesInnerBorder") as Border;
            ChartTitle = GetTemplateChild("ChartHeader") as TextBox;
            ShowDatalabels = GetTemplateChild("PART_ShowDataLabels") as MenuItem;
            ChartContextMenu = GetTemplateChild("PART_ContextMenu") as ContextMenu;

            Syncfusion.Windows.Shared.SkinStorage.SetVisualStyle(this.ChartTitle, "Default");
            Syncfusion.Windows.Shared.SkinStorage.SetVisualStyle(this.InnerChart, "Default");
            AdornerLayer.GetAdornerLayer(this).PreviewMouseDown += new MouseButtonEventHandler(ChartControl_MouseButtonDown);

            if (InnerChart != null)
            {
                AddTitle.Click += new RoutedEventHandler(AddTitle_Click);
                AddLegend.Click += new RoutedEventHandler(ShowChartLegend_Click);
                ShowCategoryTitle.Click += new RoutedEventHandler(ShowCategoryTitle_Click);
                ShowValueTitle.Click += new RoutedEventHandler(ShowValueTitle_Click);
                ShowLegendTitle.Unchecked += new RoutedEventHandler(ShowLegendTitle_Unchecked);
                ShowLegendTitle.Checked += new RoutedEventHandler(ShowLegendTitle_Checked);
                InnerDeleteLegend.Click += new RoutedEventHandler(RemoveChartLegand_Click);
                DeleteChart.Click += new RoutedEventHandler(DeleteChart_Click);
                ConvertToFullChart.Click += new RoutedEventHandler(ConvertToChart_Click);
                MenuChartProperties.Click += new System.Windows.RoutedEventHandler(ChartProperties_Click);
                SeriesProperties.Click += new System.Windows.RoutedEventHandler(SeriesProperties_Click);
                LegendProperties.Click += new System.Windows.RoutedEventHandler(LegendProperties_Click);
                InnerLegendProperties.Click += new RoutedEventHandler(LegendProperties_Click);
                ValueAxisProperties.Click += new System.Windows.RoutedEventHandler(ValueAxisProperties_Click);
                CategoryAxisProperties.Click += new System.Windows.RoutedEventHandler(CategoryAxisProperties_Click);
                ChartTitleProperties.Click += new RoutedEventHandler(ChartTitleProperties_Click);
                CategoryTitleProperties.Click += new RoutedEventHandler(CategoryTitleProperties_Click);
                ValueTitleProperties.Click += new RoutedEventHandler(ValueTitleProperties_Click);
                ChartHeaderProperties.Click += new RoutedEventHandler(ChartTitleProperties_Click);
                DeleteChartTitle.Click += new RoutedEventHandler(DeleteChartTitle_Click);
                ShowDatalabels.Click += new RoutedEventHandler(ShowDataLabels_Click);
                this.MouseLeftButtonDown += new MouseButtonEventHandler(ChartControl_MouseButtonDown);
                this.MouseRightButtonDown += new MouseButtonEventHandler(ChartControl_MouseButtonDown);
                if (this.ReportItem != null)
                {
                    this.SetInternalPropertyChangetoTrue();
                    var chart = this.ReportItem as RDL.DOM.Chart;

                    if(!string.IsNullOrEmpty(chart.DesignerMode))
                    {
                        this.ChartControlType = (Controls.ChartControlType)Enum.Parse(typeof(Controls.ChartControlType), chart.DesignerMode, true);
                    }
                }
                InitializeChart();
                this.SetInternalPropertyChangetoFalse();
            }

            this.ChartTitle.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ChartTitle_PreviewMouseLeftButtonDown);
            this.ChartTitle.PreviewMouseRightButtonDown += new MouseButtonEventHandler(ChartTitle_PreviewMouseRightButtonDown);
            this.ChartTitle.LostFocus += new RoutedEventHandler(ChartTitle_LostFocus);
            this.LegendTextbox.Style = FindResource("TextBoxStyle") as Style;
            this.LegendTextbox.LostFocus += new RoutedEventHandler(LegendTextbox_LostFocus);

            base.OnApplyTemplate();
            this.SetInternalPropertyChangetoTrue();
            this.chartProperties.Name = this.Name;
            this.chartProperties.Size.Height = this.Height.ToString();
            this.chartProperties.Size.Width = this.Width.ToString();
            this.chartProperties.DesignerMode = this.ChartControlType.ToString();

            if (this.ReportItem != null)
            {
                this.PopulateReportItem();
            }
            this.SetInternalPropertyChangetoFalse();
        }

        void ConvertToChart_Click(object sender, RoutedEventArgs e)
        {
            this.chartProperties.IsInternalPropertyChange = true;
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange = change;
            this.Panel.EditingManager.AddAction(action);
            this.SetInternalPropertyChangetoTrue();

            this.ChartControlType = Controls.ChartControlType.Chart;
            ChartTitle.Visibility = System.Windows.Visibility.Visible;
            ChartLegand.Visibility = System.Windows.Visibility.Visible;
            ChartArea.SetShowGridLines(ChartArea.SecondaryAxis, true);
            ChartArea.SecondaryAxis.AxisVisibility = System.Windows.Visibility.Visible;
            ChartArea.PrimaryAxis.AxisVisibility = System.Windows.Visibility.Visible;

            AddTitle.Visibility = System.Windows.Visibility.Visible;
            ShowValueTitle.Visibility = System.Windows.Visibility.Visible;
            LegendProperties.Visibility = System.Windows.Visibility.Visible;
            ShowCategoryTitle.Visibility = System.Windows.Visibility.Visible;
            ValueAxisProperties.Visibility = System.Windows.Visibility.Visible;
            ChartTitleProperties.Visibility = System.Windows.Visibility.Visible;
            ValueTitleProperties.Visibility = System.Windows.Visibility.Visible;
            CategoryAxisProperties.Visibility = System.Windows.Visibility.Visible;
            CategoryTitleProperties.Visibility = System.Windows.Visibility.Visible;

            ConvertToFullChart.Visibility = System.Windows.Visibility.Collapsed;

            this.SetInternalPropertyChangetoFalse();
            change.NewValue = this.GetReportItem();
            this.chartProperties.IsInternalPropertyChange = false;
        }

        void ShowValueTitle_Click(object sender, RoutedEventArgs e)
        {
            if (this.ShowValueTitle.IsChecked)
            {
                this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.valueAxisTitleProperties, IsSelected = true });
                this.chartValueAxisProperties.Visibility = "True";                
            }
            else
            {
                this.chartValueAxisProperties.Visibility = "False";                
            }
        }

        void ShowCategoryTitle_Click(object sender, RoutedEventArgs e)
        {
            if (this.ShowCategoryTitle.IsChecked)
            {
                this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.categoryAxisTitleProperties, IsSelected = true });
                this.chartCategoryAxisProperties.Visibility = "True";                
            }
            else
            {
                this.chartCategoryAxisProperties.Visibility = "False";                
            }
        }

        void AddTitle_Click(object sender, RoutedEventArgs e)
        {
            if (this.AddTitle.IsChecked)
            {
                this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.titleProperties, IsSelected = true });
                this.titleProperties.Visibility = "True";
            }
            else
            {
                this.titleProperties.Visibility = "False";
            }
        }

        void AddPanelValues()
        {
            if (this.ValuePanel != null)
            {
                try
                {
                    foreach (string data in this.DataBindingList)
                    {
                        if (data.IndexOf("[") < 0)
                        {
                            if (data[0] == '=' && data.IndexOf("(") > 0)
                            {
                                string str = data.Substring(1, data.Length - 1);
                                string newStr = "[" + str + "]";
                                str = data.Substring(str.IndexOf("(") + 2, (str.IndexOf(")") - (str.IndexOf("("))) - 1);
                                Button buttonObj = new Button();
                                buttonObj.ContextMenu = GetButtonContextMenu(buttonObj);
                                buttonObj.Margin = new Thickness(2);
                                buttonObj.Content = newStr.Replace("_", "__");
                                buttonObj.PreviewMouseDown += new MouseButtonEventHandler(ValuePanelButtonObj_PreviewMouseDown);
                                UpdateChartSeries(str);
                                this.ValuePanel.Children.Add(buttonObj);
                            }
                            else
                            {
                                string data1 = string.Empty;

                                if (data[0] == '=')
                                {
                                    data1 = data.Substring(1, data.Length - 1);
                                    }
                                    else
                                    {
                                        data1 = data;
                                    }
                                    CreateValueButtonObj(data1, this.DataSetName);
                                }
                            }
                            else
                            {
                                Button buttonObj = new Button();
                                buttonObj.ContextMenu = GetButtonContextMenu(buttonObj);
                                buttonObj.Margin = new Thickness(2);
                                buttonObj.Content = data.ToString().Replace("_", "__");
                                buttonObj.PreviewMouseDown += new MouseButtonEventHandler(ValuePanelButtonObj_PreviewMouseDown);
                                this.ValuePanel.Children.Add(buttonObj);
                            }
                        }
                    
                }
                catch
                {
                }
            }

            if (this.ColumnPanel != null)
            {
                try
                {
                    foreach (string category in this.CategoryBindingList)
                    {
                        Button buttonObj = new Button();
                        buttonObj.ContextMenu = GetButtonContextMenu(buttonObj);
                        buttonObj.Margin = new Thickness(2);
                        buttonObj.Content = category.ToString().Replace("_", "__");
                        this.ColumnPanel.Children.Add(buttonObj);
                    }
                }
                catch
                {
                }
            }

            if (this.Seriespanel != null)
            {
                try
                {
                    foreach (string category in this.SeriesBindingList)
                    {
                        Button buttonObj = new Button();
                        buttonObj.ContextMenu = GetButtonContextMenu(buttonObj);
                        buttonObj.Margin = new Thickness(2);
                        buttonObj.Content = category.ToString().Replace("_", "__");
                        this.Seriespanel.Children.Add(buttonObj);
                    }

                        //this.ShowPanels();
                }
                catch
                {
                    //MessageBox.Show("Unexpected error occurs");
                }
            }
        }

        void WireEvents()
        {
            this.chartProperties.PropertyChanged += new PropertyChangedEventHandler(OnChartPropertyChanged);
            this.chartProperties.PropertyChanging += new PropertyChangingEventHandler(chartProperties_PropertyChanging);
            this.titleProperties.PropertyChanged += new PropertyChangedEventHandler(OnChartTitlePropertyChanged);
            this.titleProperties.PropertyChanging += new PropertyChangingEventHandler(titleProperties_PropertyChanging);
            this.chartLegendProperties.PropertyChanged += new PropertyChangedEventHandler(OnChartLegendPropertyChanged);
            this.chartLegendProperties.PropertyChanging += new PropertyChangingEventHandler(chartLegendProperties_PropertyChanging);
            this.valueAxisTitleProperties.PropertyChanged += new PropertyChangedEventHandler(OnChartValueAxisTitlePropertyChanged);
            this.valueAxisTitleProperties.PropertyChanging += new PropertyChangingEventHandler(valueAxisTitleProperties_PropertyChanging);
            this.secondaryYAxisTitleProperties.PropertyChanged += new PropertyChangedEventHandler(OnChartSecondaryYAxisTitlePropertyChanged);
            this.secondaryYAxisTitleProperties.PropertyChanging += new PropertyChangingEventHandler(secondaryYAxisTitleProperties_PropertyChanging);
            this.categoryAxisTitleProperties.PropertyChanged += new PropertyChangedEventHandler(OnChartCategoryAxisTitlePropertyChanged);
            this.categoryAxisTitleProperties.PropertyChanging += new PropertyChangingEventHandler(categoryAxisTitleProperties_PropertyChanging);
            this.secondaryXAxisTitleProperties.PropertyChanged += new PropertyChangedEventHandler(OnChartSecondaryXAxisTitlePropertyChanged);
            this.secondaryXAxisTitleProperties.PropertyChanging += new PropertyChangingEventHandler(secondaryXAxisTitleProperties_PropertyChanging);
            this.chartCategoryAxisProperties.PropertyChanged += new PropertyChangedEventHandler(OnChartCategoryAxisPropertyChanged);
            this.chartCategoryAxisProperties.PropertyChanging += new PropertyChangingEventHandler(chartCategoryAxisProperties_PropertyChanging);
            this.chartSecondaryCategoryAxisProperties.PropertyChanged += new PropertyChangedEventHandler(OnChartSecondaryCategoryAxisPropertyChanged);
            this.chartSecondaryCategoryAxisProperties.PropertyChanging += new PropertyChangingEventHandler(chartSecondaryCategoryAxisProperties_PropertyChanging);
            this.chartValueAxisProperties.PropertyChanged += new PropertyChangedEventHandler(OnChartValueAxisPropertyChanged);
            this.chartValueAxisProperties.PropertyChanging += new PropertyChangingEventHandler(chartValueAxisProperties_PropertyChanging);
            this.chartSecondaryValueAxisProperties.PropertyChanged += new PropertyChangedEventHandler(OnChartSecondaryValueAxisPropertyChanged);
            this.chartSecondaryValueAxisProperties.PropertyChanging += new PropertyChangingEventHandler(chartSecondaryValueAxisProperties_PropertyChanging);
            this.defaultChartProperties.PropertyChanged += new PropertyChangedEventHandler(OnDefaultChartPropertyChanged);
            this.defaultChartProperties.PropertyChanging += new PropertyChangingEventHandler(defaultChartProperties_PropertyChanging);
            this.defaultChartProperties.CategoryAxisMajorGridLines.PropertyChanged += new PropertyChangedEventHandler(OnCategoryAxisMajorGridLinesPropertyChanged);
            this.defaultChartProperties.CategoryAxisMajorGridLines.PropertyChanging += new PropertyChangingEventHandler(CategoryAxisMajorGridLines_PropertyChanging);
            this.defaultChartProperties.CategoryAxisMinorGridLines.PropertyChanged += new PropertyChangedEventHandler(OnCategoryAxisMinorGridLinesPropertyChanged);
            this.defaultChartProperties.CategoryAxisMinorGridLines.PropertyChanging += new PropertyChangingEventHandler(CategoryAxisMinorGridLines_PropertyChanging);
            this.defaultChartProperties.ValueAxisMajorGridLines.PropertyChanged += new PropertyChangedEventHandler(OnValueAxisMajorGridLinesPropertyChanged);
            this.defaultChartProperties.ValueAxisMajorGridLines.PropertyChanging += new PropertyChangingEventHandler(ValueAxisMajorGridLines_PropertyChanging);
            this.defaultChartProperties.ValueAxisMinorGridLines.PropertyChanged += new PropertyChangedEventHandler(OnValueAxisMinorGridLinesPropertyChanged);
            this.defaultChartProperties.ValueAxisMinorGridLines.PropertyChanging += new PropertyChangingEventHandler(ValueAxisMinorGridLines_PropertyChanging);
            this.legendTitleProperties.PropertyChanged += new PropertyChangedEventHandler(OnLegendTitilePropertyChanged);
            this.legendTitleProperties.PropertyChanging += new PropertyChangingEventHandler(legendTitleProperties_PropertyChanging);
            this.PropertyChanged += new PropertyChangedEventHandler(ChartControl_PropertyChanged);
        }

        void SetInternalPropertyChangetoFalse()
        {
            chartProperties.IsInternalPropertyChange = false;
            titleProperties.IsInternalPropertyChange = false;
            categoryAxisTitleProperties.IsInternalPropertyChange = false;
            secondaryXAxisTitleProperties.IsInternalPropertyChange = false;
            chartLegendProperties.IsInternalPropertyChange = false;
            valueAxisTitleProperties.IsInternalPropertyChange = false;
            secondaryYAxisTitleProperties.IsInternalPropertyChange = false;
            chartCategoryAxisProperties.IsInternalPropertyChange = false;
            chartSecondaryCategoryAxisProperties.IsInternalPropertyChange = false;
            chartValueAxisProperties.IsInternalPropertyChange = false;
            chartSecondaryValueAxisProperties.IsInternalPropertyChange = false;
            defaultChartProperties.IsInternalPropertyChange = false;
            defaultChartProperties.CategoryAxisMajorGridLines.IsInternalPropertyChange = false;
            defaultChartProperties.CategoryAxisMinorGridLines.IsInternalPropertyChange = false;
            defaultChartProperties.ValueAxisMajorGridLines.IsInternalPropertyChange = false;
            defaultChartProperties.ValueAxisMinorGridLines.IsInternalPropertyChange = false;
            legendTitleProperties.IsInternalPropertyChange = false;
            if (this.chartSeriesPropertiesCollection.Count > 0)
            {
                for (int i = 0; i < this.chartSeriesPropertiesCollection.Count; i++)
                {
                    this.chartSeriesPropertiesCollection[seriesCount].IsInternalPropertyChange = false;
                }
            }
        }

        void SetInternalPropertyChangetoTrue()
        {
            chartProperties.IsInternalPropertyChange = true;
            titleProperties.IsInternalPropertyChange = true;
            categoryAxisTitleProperties.IsInternalPropertyChange = true;
            secondaryXAxisTitleProperties.IsInternalPropertyChange = true;
            chartLegendProperties.IsInternalPropertyChange = true;
            valueAxisTitleProperties.IsInternalPropertyChange = true;
            secondaryYAxisTitleProperties.IsInternalPropertyChange = true;
            chartCategoryAxisProperties.IsInternalPropertyChange = true;
            chartSecondaryCategoryAxisProperties.IsInternalPropertyChange = true;
            chartValueAxisProperties.IsInternalPropertyChange = true;
            chartSecondaryValueAxisProperties.IsInternalPropertyChange = true;
            defaultChartProperties.IsInternalPropertyChange = true;
            defaultChartProperties.CategoryAxisMajorGridLines.IsInternalPropertyChange = true;
            defaultChartProperties.CategoryAxisMinorGridLines.IsInternalPropertyChange = true;
            defaultChartProperties.ValueAxisMajorGridLines.IsInternalPropertyChange = true;
            defaultChartProperties.ValueAxisMinorGridLines.IsInternalPropertyChange = true;
            legendTitleProperties.IsInternalPropertyChange = true;
            if (this.chartSeriesPropertiesCollection.Count > 0)
            {
                for (int i = 0; i < this.chartSeriesPropertiesCollection.Count; i++)
                {
                    this.chartSeriesPropertiesCollection[seriesCount].IsInternalPropertyChange = true;
                }
            }
        }

        void SeriesDataField_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if ((this.Seriespanel.Children.Count == 0) && (this.ColumnPanel.Children.Count == 0))
                {
                    if ((this.DataSets != null) && (this.DataSources != null))
                    {
                        if (this.DataSources.Count != 0)
                        {
                            if ((this.DataSources.Count >= 1) && (this.DataSets.Count > 1))
                            {
                                this.SeriesDataField.ContextMenu = null;
                                ContextMenu contextMenu = new ContextMenu();

                                foreach (RDL.DOM.DataSource dataSource in this.DataSources)
                                {
                                    var dataSets = from dataSet in this.DataSets
                                                   where dataSet.Query.DataSourceName == dataSource.Name
                                                   select dataSet;


                                    var dataSets1 = dataSets.ToList();
                                    MenuItem menuItem = new MenuItem();
                                    menuItem.Header = dataSource.Name;


                                    foreach (RDL.DOM.DataSet dataSet in dataSets1)
                                    {
                                        MenuItem seriesmenuItem2 = new MenuItem();
                                        seriesmenuItem2.Header = dataSet.Name;


                                        var fields = from field in dataSet.Fields
                                                     select field;
                                        var field1 = fields.ToList();

                                        foreach (RDL.DOM.Field field in field1)
                                        {
                                            MenuItem seriesmenuItem3 = new MenuItem();
                                            seriesmenuItem3.Header = field.Name;
                                            seriesmenuItem2.Items.Add(seriesmenuItem3);
                                            seriesmenuItem3.Click += new RoutedEventHandler(SeriesmenuItem3_Click);
                                        }
                                        menuItem.Items.Add(seriesmenuItem2);
                                    }
                                    contextMenu.Items.Add(menuItem);
                                }
                                this.SeriesDataField.ContextMenu = contextMenu;
                                this.SeriesDataField.ContextMenu.IsOpen = true;
                            }
                            else
                            {
                                this.SeriesDataField.ContextMenu = null;
                                ContextMenu contextMenu3 = new ContextMenu();
                                if (this.DataSets.Count == 1)
                                {
                                    var fields = from field in DataSets[0].Fields
                                                 select field;
                                    var field1 = fields.ToList();

                                    foreach (RDL.DOM.Field field in field1)
                                    {
                                        MenuItem seriesmenuItem4 = new MenuItem();
                                        seriesmenuItem4.Header = field.Name;
                                        seriesmenuItem4.Click += new RoutedEventHandler(SeriesmenuItem4_Click);
                                        contextMenu3.Items.Add(seriesmenuItem4);
                                    }
                                    this.SeriesDataField.ContextMenu = contextMenu3;
                                    this.SeriesDataField.ContextMenu.IsOpen = true;
                                }
                            }
                            if ((this.DataSources.Count >= 1) && (this.DataSets.Count == 0))
                            {
                                this.SeriesDataField.ContextMenu = null;
                                ContextMenu contextMenu = new ContextMenu();
                                MenuItem seriesmenuItem12 = new MenuItem();
                                seriesmenuItem12.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerAddDataSet");
                                contextMenu.Items.Add(seriesmenuItem12);
                                seriesmenuItem12.Click += new RoutedEventHandler(AddDataSet_Click);
                                this.SeriesDataField.ContextMenu = contextMenu;
                                this.SeriesDataField.ContextMenu.IsOpen = true;
                            }
                        }
                        else
                        {
                            if ((this.DataSources != null) && (this.DataSets != null))
                            {
                                this.SeriesDataField.ContextMenu = null;
                                ContextMenu contextMenu = new ContextMenu();
                                MenuItem seriesmenuItem13 = new MenuItem();
                                seriesmenuItem13.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerAddDataSource");
                                contextMenu.Items.Add(seriesmenuItem13);
                                seriesmenuItem13.Click += new RoutedEventHandler(AddDataSource_Click);
                                this.SeriesDataField.ContextMenu = contextMenu;
                                this.SeriesDataField.ContextMenu.IsOpen = true;
                            }
                        }
                    }
                }

                else
                {
                    if ((this.DataSources != null) && (this.DataSets != null))
                    {
                        if (this.DataSets.Count != 0)
                        {
                            this.SeriesDataField.ContextMenu = null;
                            ContextMenu contextMenu = new ContextMenu();
                            MenuItem seriesmenuItem = new MenuItem();
                            string str = this.DataSetName;
                            var fields = (from dataSetThis in this.DataSets
                                          where dataSetThis.Name == str
                                          select dataSetThis.Fields).SingleOrDefault();

                            foreach (RDL.DOM.Field field in fields)
                            {
                                MenuItem seriesmenuItem1 = new MenuItem();
                                seriesmenuItem1.Header = field.Name;
                                seriesmenuItem1.Click += new RoutedEventHandler(SeriesmenuItem1_Click);
                                contextMenu.Items.Add(seriesmenuItem1);
                            }
                            this.SeriesDataField.ContextMenu = contextMenu;
                            this.SeriesDataField.ContextMenu.IsOpen = true;
                        }
                        else
                        {
                            this.SeriesDataField.ContextMenu = null;
                            ContextMenu context = new ContextMenu();
                            MenuItem seriesmenu = new MenuItem();
                            seriesmenu.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerDataSetNotExist");
                            context.Items.Add(seriesmenu);
                            this.SeriesDataField.ContextMenu = context;
                            this.SeriesDataField.ContextMenu.IsOpen = true;
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }

        void SeriesmenuItem1_Click(object sender, RoutedEventArgs e)
        {
            string header = (sender as MenuItem).Header.ToString();
            string parent = this.DataSetName;
            CreateSeriesButtonObj(header, parent);
        }

        void SeriesmenuItem4_Click(object sender, RoutedEventArgs e)
        {
            string header = (sender as MenuItem).Header.ToString();
            string parent = this.DataSets[0].Name;
            CreateSeriesButtonObj(header, parent);
        }

        void SeriesmenuItem3_Click(object sender, RoutedEventArgs e)
        {
            string header = (sender as MenuItem).Header.ToString();
            string parent = ((sender as MenuItem).Parent as MenuItem).Header.ToString();
            CreateSeriesButtonObj(header, parent);
        }

        void Seriespanel_PreviewDrop(object sender, DragEventArgs e)
        {
            this.SetInternalPropertyChangetoTrue();
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange =change;
            this.Panel.EditingManager.AddAction(action);

            TreeObjectCollection itemCollection = e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;
            if (itemCollection != null && itemCollection.Count > 0)
            {
                TreeViewItemAdv treeViewItem = itemCollection[0] as TreeViewItemAdv;
                AddDataFields(treeViewItem, Seriespanel);

                if (treeViewItem != null)
                {
                    ChartArea.PrimaryAxis.ContentPath = "SeriesName";
                    ChartArea.PrimaryAxis.PositionPath = "SeriesId";
                    ChartArea.PrimaryAxis.LabelsSource = DefaultChartData1();
                    this.secondaryXAxis.ContentPath = this.InnerChart.Areas[0].PrimaryAxis.ContentPath;
                    this.secondaryXAxis.PositionPath = this.InnerChart.Areas[0].PrimaryAxis.PositionPath;
                    this.secondaryXAxis.LabelsSource = this.InnerChart.Areas[0].PrimaryAxis.LabelsSource;
                }
            }

            change.NewValue = this.GetReportItem();
            this.SetInternalPropertyChangetoFalse();

        }

        void SeriesDataField_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SeriesDataField.ContextMenu != null)
            {
                SeriesDataField.ContextMenu = null;
            }
        }

        void DeleteChartTitle_Click(object sender, RoutedEventArgs e)
        {
            this.ChartTitle.Visibility = Visibility.Collapsed;
            this.RemoveSelectionAdorner(ChartTitle);
            this.AddTitle.IsChecked = false;
        }      

        void ShowPrimaryAxisMajorGridLines_Click(object sender, RoutedEventArgs e)
        {
            MenuItem primaryAxisMajorGridLines = sender as MenuItem;
            if (primaryAxisMajorGridLines.IsChecked)
            {
                this.defaultChartProperties.CategoryAxisMajorGridLines.EnableMajorGridLines = "True";
                primaryAxisMajorGridLines.IsChecked = false;
            }
            else
            {
                this.defaultChartProperties.CategoryAxisMajorGridLines.EnableMajorGridLines = "False";
                primaryAxisMajorGridLines.IsChecked = true;
            }
        }

        void ShowPrimaryAxisMinorGridLines_Click(object sender, RoutedEventArgs e)
        {
            MenuItem primaryAxisMinorGridLines = sender as MenuItem;
            if (primaryAxisMinorGridLines.IsChecked)
            {
                this.defaultChartProperties.CategoryAxisMinorGridLines.EnableMinorGridLines = "True";
            }
            else
            {
                this.defaultChartProperties.CategoryAxisMinorGridLines.EnableMinorGridLines = "False";
            }
        }

        void ShowPrimaryAxis_Click(object sender, RoutedEventArgs e)
        {
            MenuItem showAxis = sender as MenuItem;
            if (showAxis.IsChecked)
            {
                this.ChartArea.PrimaryAxis.AxisVisibility = Visibility.Visible;
                showAxis.IsChecked = false;
            }
            else
            {
                this.ChartArea.PrimaryAxis.AxisVisibility = Visibility.Collapsed;
                this.RemoveSelectionAdorner(ChartArea);
                showAxis.IsChecked = true;
            }
        }

        void ShowSecondaryAxisMajorGridLines_Click(object sender, RoutedEventArgs e)
        {
            MenuItem secondaryAxisMajorGridLines = sender as MenuItem;
            if (secondaryAxisMajorGridLines.IsChecked)
            {
                this.defaultChartProperties.ValueAxisMajorGridLines.EnableMajorGridLines = "True";
            }
            else
            {
                this.defaultChartProperties.ValueAxisMajorGridLines.EnableMajorGridLines = "False";
            }
        }

        void ShowSecondaryAxisMinorGridLines_Click(object sender, RoutedEventArgs e)
        {
            MenuItem secondaryAxisMinorGridLines = sender as MenuItem;
            if (secondaryAxisMinorGridLines.IsChecked)
            {
                this.defaultChartProperties.ValueAxisMinorGridLines.EnableMinorGridLines = "True";
            }
            else
            {
                this.defaultChartProperties.ValueAxisMinorGridLines.EnableMinorGridLines = "False";
            }
        }

        void ShowSecondaryAxis_Click(object sender, RoutedEventArgs e)
        {
            MenuItem showAxis = sender as MenuItem;
            if (showAxis.IsChecked)
            {
                this.ChartArea.SecondaryAxis.AxisVisibility = Visibility.Visible;
            }
            else
            {
                this.ChartArea.SecondaryAxis.AxisVisibility = Visibility.Collapsed;
                this.RemoveSelectionAdorner(ChartArea);
            }
        }

        void ShowSecondaryXAxisTitle_Click(object sender, RoutedEventArgs e)
        {
            MenuItem primaryAxisTitle = sender as MenuItem;

            if (primaryAxisTitle.IsChecked)
            {
                this.SecondaryXAxisTitle.Visibility = Visibility.Visible;
            }
            else
            {
                this.SecondaryXAxisTitle.Visibility = Visibility.Collapsed;
                this.RemoveSelectionAdorner(SecondaryXAxisTitle);
            }
        }

        void ShowSecondaryYAxisTitle_Click(object sender, RoutedEventArgs e)
        {
            MenuItem secondaryAxisTitle = sender as MenuItem;
            if (secondaryAxisTitle.IsChecked)
            {
                this.SecondaryYAxisTitle.Visibility = Visibility.Visible;
            }
            else
            {
                this.SecondaryYAxisTitle.Visibility = Visibility.Collapsed;
            }
        }


        # region Panel and Smart tag Events

        void columnDataField_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if ((this.ValuePanel.Children.Count == 0) && (this.ColumnPanel.Children.Count == 0))
                {
                    if ((this.DataSets != null) && (this.DataSources != null))
                    {
                        if (this.DataSources.Count != 0)
                        {
                            if ((this.DataSources.Count >= 1) && (this.DataSets.Count > 1))
                            {
                                this.ColumnDataField.ContextMenu = null;
                                ContextMenu contextMenu = new ContextMenu();

                                foreach (RDL.DOM.DataSource dataSource in this.DataSources)
                                {
                                    var dataSets = from dataSet in this.DataSets
                                                   where dataSet.Query.DataSourceName == dataSource.Name
                                                   select dataSet;


                                    var dataSets1 = dataSets.ToList();
                                    MenuItem menuItem5 = new MenuItem();
                                    menuItem5.Header = dataSource.Name;


                                    foreach (RDL.DOM.DataSet dataSet in dataSets1)
                                    {
                                        MenuItem menuItem6 = new MenuItem();
                                        menuItem6.Header = dataSet.Name;


                                        var fields = from field in dataSet.Fields
                                                     select field;
                                        var field1 = fields.ToList();

                                        foreach (RDL.DOM.Field field in field1)
                                        {
                                            MenuItem menuItem7 = new MenuItem();
                                            menuItem7.Header = field.Name;
                                            menuItem6.Items.Add(menuItem7);
                                            menuItem7.Click += new RoutedEventHandler(MenuItem7_Click);
                                        }
                                        menuItem5.Items.Add(menuItem6);
                                    }
                                    contextMenu.Items.Add(menuItem5);
                                }
                                this.ColumnDataField.ContextMenu = contextMenu;
                                this.ColumnDataField.ContextMenu.IsOpen = true;
                            }
                            else
                            {
                                this.ColumnDataField.ContextMenu = null;
                                ContextMenu contextMenu3 = new ContextMenu();
                                if (this.DataSets.Count == 1)
                                {
                                    var fields = from field in DataSets[0].Fields
                                                 select field;
                                    var field1 = fields.ToList();

                                    foreach (RDL.DOM.Field field in field1)
                                    {
                                        MenuItem menuItem9 = new MenuItem();
                                        menuItem9.Header = field.Name;
                                        menuItem9.Click += new RoutedEventHandler(MenuItem9_Click);
                                        contextMenu3.Items.Add(menuItem9);
                                    }
                                    this.ColumnDataField.ContextMenu = contextMenu3;
                                    this.ColumnDataField.ContextMenu.IsOpen = true;
                                }
                            }

                            if ((this.DataSources.Count >= 1) && (this.DataSets.Count == 0))
                            {
                                this.ColumnDataField.ContextMenu = null;
                                ContextMenu contextMenu = new ContextMenu();
                                MenuItem menuItem10 = new MenuItem();
                                menuItem10.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerAddDataSet");
                                contextMenu.Items.Add(menuItem10);
                                menuItem10.Click += new RoutedEventHandler(AddDataSet_Click);
                                this.ColumnDataField.ContextMenu = contextMenu;
                                this.ColumnDataField.ContextMenu.IsOpen = true;
                            }
                        }
                        else
                        {
                            this.ColumnDataField.ContextMenu = null;
                            ContextMenu contextMenu = new ContextMenu();
                            MenuItem menuItem11 = new MenuItem();
                            menuItem11.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerAddDataSource");
                            menuItem11.Click += new RoutedEventHandler(AddDataSource_Click);
                            contextMenu.Items.Add(menuItem11);
                            this.ColumnDataField.ContextMenu = contextMenu;
                            this.ColumnDataField.ContextMenu.IsOpen = true;
                        }
                    }
                }

                else
                {
                    if ((this.DataSources != null) && (this.DataSets != null))
                    {
                        if (this.DataSets.Count != 0)
                        {
                            this.ColumnDataField.ContextMenu = null;
                            ContextMenu contextMenu = new ContextMenu();
                            string str = this.DataSetName;
                            var fields = (from dataSetThis in this.DataSets
                                          where dataSetThis.Name == str
                                          select dataSetThis.Fields).SingleOrDefault();

                            foreach (RDL.DOM.Field field in fields)
                            {
                                MenuItem menuItem8 = new MenuItem();
                                menuItem8.Header = field.Name;
                                menuItem8.Click += new RoutedEventHandler(MenuItem8_Click);
                                contextMenu.Items.Add(menuItem8);
                            }
                            this.ColumnDataField.ContextMenu = contextMenu;
                            this.ColumnDataField.ContextMenu.IsOpen = true;
                        }
                        else
                        {
                            this.ColumnDataField.ContextMenu = null;
                            ContextMenu context = new ContextMenu();
                            MenuItem menu = new MenuItem();
                            menu.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerDataSetNotExist");
                            context.Items.Add(menu);
                            this.ColumnDataField.ContextMenu = context;
                            this.ColumnDataField.ContextMenu.IsOpen = true;
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }

        void ValueDataField_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if ((this.ValuePanel.Children.Count == 0) && (this.ColumnPanel.Children.Count == 0))
                {
                    if ((this.DataSets != null) && (this.DataSources != null))
                    {
                        if (this.DataSources.Count != 0)
                        {
                            if ((this.DataSources.Count >= 1) && (this.DataSets.Count > 1))
                            {
                                this.ValueDataField.ContextMenu = null;
                                ContextMenu contextMenu = new ContextMenu();

                                foreach (RDL.DOM.DataSource dataSource in this.DataSources)
                                {
                                    var dataSets = from dataSet in this.DataSets
                                                   where dataSet.Query.DataSourceName == dataSource.Name
                                                   select dataSet;


                                    var dataSets1 = dataSets.ToList();
                                    MenuItem menuItem = new MenuItem();
                                    menuItem.Header = dataSource.Name;


                                    foreach (RDL.DOM.DataSet dataSet in dataSets1)
                                    {
                                        MenuItem menuItem2 = new MenuItem();
                                        menuItem2.Header = dataSet.Name;


                                        var fields = from field in dataSet.Fields
                                                     select field;
                                        var field1 = fields.ToList();

                                        foreach (RDL.DOM.Field field in field1)
                                        {
                                            MenuItem menuItem3 = new MenuItem();
                                            menuItem3.Header = field.Name;
                                            menuItem2.Items.Add(menuItem3);
                                            menuItem3.Click += new RoutedEventHandler(MenuItem3_Click);
                                        }
                                        menuItem.Items.Add(menuItem2);
                                    }
                                    contextMenu.Items.Add(menuItem);
                                }
                                this.ValueDataField.ContextMenu = contextMenu;
                                this.ValueDataField.ContextMenu.IsOpen = true;
                            }
                            else
                            {
                                this.ValueDataField.ContextMenu = null;
                                ContextMenu contextMenu3 = new ContextMenu();
                                if (this.DataSets.Count == 1)
                                {
                                    var fields = from field in DataSets[0].Fields
                                                 select field;
                                    var field1 = fields.ToList();

                                    foreach (RDL.DOM.Field field in field1)
                                    {
                                        MenuItem menuItem4 = new MenuItem();
                                        menuItem4.Header = field.Name;
                                        menuItem4.Click += new RoutedEventHandler(MenuItem4_Click);
                                        contextMenu3.Items.Add(menuItem4);
                                    }
                                    this.ValueDataField.ContextMenu = contextMenu3;
                                    this.ValueDataField.ContextMenu.IsOpen = true;
                                }
                            }
                            if ((this.DataSources.Count >= 1) && (this.DataSets.Count == 0))
                            {
                                this.ValueDataField.ContextMenu = null;
                                ContextMenu contextMenu = new ContextMenu();
                                MenuItem menuItem12 = new MenuItem();
                                menuItem12.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerAddDataSet");
                                contextMenu.Items.Add(menuItem12);
                                menuItem12.Click += new RoutedEventHandler(AddDataSet_Click);
                                this.ValueDataField.ContextMenu = contextMenu;
                                this.ValueDataField.ContextMenu.IsOpen = true;
                            }
                        }
                        else
                        {
                            if ((this.DataSources != null) && (this.DataSets != null))
                            {
                                this.ValueDataField.ContextMenu = null;
                                ContextMenu contextMenu = new ContextMenu();
                                MenuItem menuItem13 = new MenuItem();
                                menuItem13.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerAddDataSource");
                                contextMenu.Items.Add(menuItem13);
                                menuItem13.Click += new RoutedEventHandler(AddDataSource_Click);
                                this.ValueDataField.ContextMenu = contextMenu;
                                this.ValueDataField.ContextMenu.IsOpen = true;
                            }
                        }
                    }
                }

                else
                {
                    if ((this.DataSources != null) && (this.DataSets != null))
                    {
                        if (this.DataSets.Count != 0)
                        {
                            this.ValueDataField.ContextMenu = null;
                            ContextMenu contextMenu = new ContextMenu();
                            MenuItem menuItem = new MenuItem();
                            string str = this.DataSetName;
                            var fields = (from dataSetThis in this.DataSets
                                          where dataSetThis.Name == str
                                          select dataSetThis.Fields).SingleOrDefault();

                            if (fields != null)
                            {
                                foreach (RDL.DOM.Field field in fields)
                                {
                                    MenuItem menuItem1 = new MenuItem();
                                    menuItem1.Header = field.Name;
                                    menuItem1.Click += new RoutedEventHandler(MenuItem1_Click);
                                    contextMenu.Items.Add(menuItem1);
                                }
                            }
                            else
                            {
                                MenuItem menuItem1 = new MenuItem();
                                menuItem1.Header = RESX.msgBoxDataSet + " '" + this.DataSetName.ToString() + "' " + RESX.msgBoxDataSetNotExistInReport;
                                contextMenu.Items.Add(menuItem1);
                            }
                            this.ValueDataField.ContextMenu = contextMenu;
                            this.ValueDataField.ContextMenu.IsOpen = true;
                        }
                        else
                        {
                            this.ValueDataField.ContextMenu = null;
                            ContextMenu context = new ContextMenu();
                            MenuItem menu = new MenuItem();
                            menu.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerDataSetNotExist");
                            context.Items.Add(menu);
                            this.ValueDataField.ContextMenu = context;
                            this.ValueDataField.ContextMenu.IsOpen = true;
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }

        private RDL.DOM.Chart UpdateChartObj(RDL.DOM.Chart ChartObj)
        {

            if (this.InnerChart != null)
            {
                if (this.ChartArea != null)
                {
                    ChartObj.ChartAreas = new RDL.DOM.ChartAreas();
                    RDL.DOM.ChartArea chartArea = new RDL.DOM.ChartArea();
                    chartArea.Style = new RDL.DOM.Style();
                    chartArea.Name = "Default";

                    chartArea.ChartCategoryAxes = new RDL.DOM.ChartCategoryAxes();
                    chartArea.ChartValueAxes = new RDL.DOM.ChartValueAxes();

                    RDL.DOM.ChartAxis primaryAxis = new RDL.DOM.ChartAxis();
                    primaryAxis.ChartMajorGridLines = new RDL.DOM.ChartMajorGridLines();
                    primaryAxis.ChartMajorGridLines.Style = new RDL.DOM.Style();
                    primaryAxis.ChartMajorGridLines.Style.Border = new RDL.DOM.Border();
                    primaryAxis.ChartMinorGridLines = new RDL.DOM.ChartMinorGridLines();
                    primaryAxis.ChartMinorGridLines.Style = new RDL.DOM.Style();
                    primaryAxis.ChartMinorGridLines.Style.Border = new RDL.DOM.Border();
                    primaryAxis.ChartMajorTickMarks = new RDL.DOM.ChartMajorTickMarks();
                    primaryAxis.ChartMajorTickMarks.Style = new RDL.DOM.Style();
                    primaryAxis.ChartMajorTickMarks.Style.Border = new RDL.DOM.Border();
                    primaryAxis.ChartMinorTickMarks = new RDL.DOM.ChartMinorTickMarks();
                    primaryAxis.ChartMinorTickMarks.Style = new RDL.DOM.Style();
                    primaryAxis.ChartMinorTickMarks.Style.Border = new RDL.DOM.Border();
                    primaryAxis.Style = new RDL.DOM.Style();
                    primaryAxis.Style.Border = new RDL.DOM.Border();

                    RDL.DOM.ChartAxis secondaryCategoryAxis = new RDL.DOM.ChartAxis();
                    secondaryCategoryAxis.ChartMajorGridLines = new RDL.DOM.ChartMajorGridLines();
                    secondaryCategoryAxis.ChartMinorGridLines = new RDL.DOM.ChartMinorGridLines();
                    secondaryCategoryAxis.ChartMajorTickMarks = new RDL.DOM.ChartMajorTickMarks();
                    secondaryCategoryAxis.ChartMajorTickMarks.Style = new RDL.DOM.Style();
                    secondaryCategoryAxis.ChartMajorTickMarks.Style.Border = new RDL.DOM.Border();
                    secondaryCategoryAxis.ChartMinorTickMarks = new RDL.DOM.ChartMinorTickMarks();
                    secondaryCategoryAxis.ChartMinorTickMarks.Style = new RDL.DOM.Style();
                    secondaryCategoryAxis.ChartMinorTickMarks.Style.Border = new RDL.DOM.Border();
                    secondaryCategoryAxis.Style = new RDL.DOM.Style();
                    secondaryCategoryAxis.Style.Border = new RDL.DOM.Border();

                    RDL.DOM.ChartAxis secondaryAxis = new RDL.DOM.ChartAxis();
                    secondaryAxis.ChartMajorGridLines = new RDL.DOM.ChartMajorGridLines();
                    secondaryAxis.ChartMajorGridLines.Style = new RDL.DOM.Style();
                    secondaryAxis.ChartMajorGridLines.Style.Border = new RDL.DOM.Border();
                    secondaryAxis.ChartMinorGridLines = new RDL.DOM.ChartMinorGridLines();
                    secondaryAxis.ChartMinorGridLines.Style = new RDL.DOM.Style();
                    secondaryAxis.ChartMinorGridLines.Style.Border = new RDL.DOM.Border();
                    secondaryAxis.ChartMajorTickMarks = new RDL.DOM.ChartMajorTickMarks();
                    secondaryAxis.ChartMajorTickMarks.Style = new RDL.DOM.Style();
                    secondaryAxis.ChartMajorTickMarks.Style.Border = new RDL.DOM.Border();
                    secondaryAxis.ChartMinorTickMarks = new RDL.DOM.ChartMinorTickMarks();
                    secondaryAxis.ChartMinorTickMarks.Style = new RDL.DOM.Style();
                    secondaryAxis.ChartMinorTickMarks.Style.Border = new RDL.DOM.Border();
                    secondaryAxis.Style = new RDL.DOM.Style();
                    secondaryAxis.Style.Border = new RDL.DOM.Border();

                    RDL.DOM.ChartAxis secondaryValueAxis = new RDL.DOM.ChartAxis();
                    secondaryValueAxis.ChartMajorGridLines = new RDL.DOM.ChartMajorGridLines();
                    secondaryValueAxis.ChartMajorGridLines.Style = new RDL.DOM.Style();
                    secondaryValueAxis.ChartMajorGridLines.Style.Border = new RDL.DOM.Border();
                    secondaryValueAxis.ChartMinorGridLines = new RDL.DOM.ChartMinorGridLines();
                    secondaryValueAxis.ChartMajorTickMarks = new RDL.DOM.ChartMajorTickMarks();
                    secondaryValueAxis.ChartMajorTickMarks.Style = new RDL.DOM.Style();
                    secondaryValueAxis.ChartMajorTickMarks.Style.Border = new RDL.DOM.Border();
                    secondaryValueAxis.ChartMinorTickMarks = new RDL.DOM.ChartMinorTickMarks();
                    secondaryValueAxis.ChartMinorTickMarks.Style = new RDL.DOM.Style();
                    secondaryValueAxis.ChartMinorTickMarks.Style.Border = new RDL.DOM.Border();
                    secondaryValueAxis.Style = new RDL.DOM.Style();
                    secondaryValueAxis.Style.Border = new RDL.DOM.Border();

                    if (this.chartProperties.FillStyle.ToLower() == "gradient")
                    {
                        chartArea.Style.BackgroundColor = this.chartProperties.PrimaryColor;
                        chartArea.Style.BackgroundGradientEndColor = this.chartProperties.SecondaryColor;
                        if (this.chartProperties.GradientStyle == "LeftRight")
                            chartArea.Style.BackgroundGradientType = RDL.DOM.BackgroundGradientTypes.LeftRight;
                        else if (this.chartProperties.GradientStyle == "TopBottom")
                            chartArea.Style.BackgroundGradientType = RDL.DOM.BackgroundGradientTypes.TopBottom;
                        else
                            chartArea.Style.BackgroundGradientType = RDL.DOM.BackgroundGradientTypes.None;
                    }
                    else
                    {
                        chartArea.Style.BackgroundColor = this.chartProperties.PrimaryColor;
                    }


                    primaryAxis.Name = "Primary";
                    if (this.chartCategoryAxisProperties.ReverseDirection.ToLower() == "false")
                    {
                        primaryAxis.Reverse = false;
                    }
                    else
                    {
                        primaryAxis.Reverse = true;
                    }
                    primaryAxis.Style.Border.Color = this.chartCategoryAxisProperties.LineColor;
                    primaryAxis.Style.Border.Style = this.chartCategoryAxisProperties.LineStyle;
                    primaryAxis.Style.Border.Width = this.chartCategoryAxisProperties.LineWidth;
                    primaryAxis.Style.FontSize = this.chartCategoryAxisProperties.FontSize;
                    primaryAxis.Style.FontFamily = this.chartCategoryAxisProperties.FontFamily;
                    primaryAxis.Style.FontWeight = this.chartCategoryAxisProperties.FontWeight;
                    primaryAxis.Style.Color = this.chartCategoryAxisProperties.FontColor;
                    primaryAxis.Angle = float.Parse(new RDL.DOM.Size(this.chartCategoryAxisProperties.FontAngle).PixelValue.ToString());
                    if (primaryAxis.Angle != 0 || primaryAxis.Style.FontSize.size != "8pt")
                    {
                        primaryAxis.LabelsAutoFitDisabled = true;
                    }
                    primaryAxis.ChartMajorGridLines.Enabled = (RDL.DOM.BooleanOptions)Enum.Parse(typeof(RDL.DOM.BooleanOptions), this.defaultChartProperties.CategoryAxisMajorGridLines.EnableMajorGridLines);
                    primaryAxis.ChartMinorGridLines.Enabled = (RDL.DOM.BooleanOptions)Enum.Parse(typeof(RDL.DOM.BooleanOptions), this.defaultChartProperties.CategoryAxisMinorGridLines.EnableMinorGridLines);

                    if (primaryAxis.ChartMajorGridLines.Enabled == RDL.DOM.BooleanOptions.True)
                    {
                        primaryAxis.ChartMajorGridLines.Style.Border.Color = this.defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesColor;
                        primaryAxis.ChartMajorGridLines.Style.Border.Style = this.defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesStyle;
                        primaryAxis.ChartMajorGridLines.Style.Border.Width = this.defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesWidth;
                    }
                    if (primaryAxis.ChartMinorGridLines.Enabled == RDL.DOM.BooleanOptions.True)
                    {
                        primaryAxis.ChartMinorGridLines.Style.Border.Color = this.defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesColor;
                        primaryAxis.ChartMinorGridLines.Style.Border.Style = this.defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesStyle;
                        primaryAxis.ChartMinorGridLines.Style.Border.Width = this.defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesWidth;
                    }

                    primaryAxis.ChartMajorTickMarks.Enabled = (RDL.DOM.BooleanOptions)Enum.Parse(typeof(RDL.DOM.BooleanOptions), this.chartCategoryAxisProperties.EnableMajorTickMarks, true);
                    primaryAxis.ChartMinorTickMarks.Enabled = (RDL.DOM.BooleanOptions)Enum.Parse(typeof(RDL.DOM.BooleanOptions), this.chartCategoryAxisProperties.EnableMinorTickMarks, true);
                    primaryAxis.ChartMajorTickMarks.Length = float.Parse(new RDL.DOM.Size(this.chartCategoryAxisProperties.TickLength).FloatValue.ToString());
                    primaryAxis.ChartMajorTickMarks.Style.Border.Color = this.chartCategoryAxisProperties.TickColor;
                    primaryAxis.ChartMajorTickMarks.Style.Border.Style = this.chartCategoryAxisProperties.TickStyle;
                    primaryAxis.ChartMajorTickMarks.Style.Border.Width = this.chartCategoryAxisProperties.TickLength;
                    if (this.chartCategoryAxisProperties.HideAxisLabels.ToLower() == "true")
                    {
                        primaryAxis.Style.Color = "White";
                    }
                    if (this.chartCategoryAxisProperties.EnableMajorTickMarks.ToLower() == "false")
                    {
                        primaryAxis.ChartMajorTickMarks.Style.Border.Width = "0pt";
                    }
                    primaryAxis.ChartMinorTickMarks.Length = float.Parse(new RDL.DOM.Size(this.chartCategoryAxisProperties.TickWidth).FloatValue.ToString());
                    primaryAxis.ChartMinorTickMarks.Style.Border.Color = this.chartCategoryAxisProperties.MinorTickColor;
                    primaryAxis.ChartMinorTickMarks.Style.Border.Style = this.chartCategoryAxisProperties.MinorTickStyle;
                    primaryAxis.ChartMinorTickMarks.Style.Border.Width = this.chartCategoryAxisProperties.TickWidth;

                    if (this.CategoryAxisTitle != null && this.CategoryAxisTitle.Visibility == Visibility.Visible)
                    {
                        primaryAxis.ChartAxisTitle = new RDL.DOM.ChartAxisTitle();
                        primaryAxis.ChartAxisTitle.Style = new RDL.DOM.Style();
                        primaryAxis.ChartAxisTitle.Caption = this.categoryAxisTitleProperties.Name;
                        switch (this.categoryAxisTitleProperties.TitleAlignment.ToLower())
                        {
                            case "far":
                                primaryAxis.ChartAxisTitle.Position = RDL.DOM.ChartAxisTitlePosition.Far;
                                break;
                            case "near":
                                primaryAxis.ChartAxisTitle.Position = RDL.DOM.ChartAxisTitlePosition.Near;
                                break;
                            default:
                                primaryAxis.ChartAxisTitle.Position = RDL.DOM.ChartAxisTitlePosition.Center;
                                break;
                        }
                        primaryAxis.ChartAxisTitle.Style.FontSize = this.categoryAxisTitleProperties.FontSize;
                        primaryAxis.ChartAxisTitle.Style.BackgroundColor = Brushes.White.ToString();
                        primaryAxis.ChartAxisTitle.Style.FontWeight = RDL.DOM.FontWeight.Normal.ToString();
                        primaryAxis.ChartAxisTitle.Style.FontStyle = this.categoryAxisTitleProperties.FontStyle;
                        primaryAxis.ChartAxisTitle.Style.FontFamily = this.categoryAxisTitleProperties.FontFamily;
                        primaryAxis.ChartAxisTitle.Style.Color = this.categoryAxisTitleProperties.FontColor;
                    }
                    if (ChartArea.PrimaryAxis != null && ChartArea.PrimaryAxis.AxisVisibility != System.Windows.Visibility.Visible)
                    {
                        primaryAxis.Visible = RDL.DOM.BooleanOptions.False;
                    }

                    secondaryCategoryAxis.Name = "Secondary";
                    if (this.chartSecondaryCategoryAxisProperties.ReverseDirection.ToLower() == "false")
                    {
                        secondaryCategoryAxis.Reverse = false;
                    }
                    else
                    {
                        secondaryCategoryAxis.Reverse = true;
                    }
                    secondaryCategoryAxis.Style.Border.Color = this.chartSecondaryCategoryAxisProperties.LineColor;
                    secondaryCategoryAxis.Style.Border.Style = this.chartSecondaryCategoryAxisProperties.LineStyle;
                    secondaryCategoryAxis.Style.Border.Width = this.chartSecondaryCategoryAxisProperties.LineWidth;
                    secondaryCategoryAxis.Style.FontSize = this.chartSecondaryCategoryAxisProperties.FontSize;
                    secondaryCategoryAxis.Style.FontFamily = this.chartSecondaryCategoryAxisProperties.FontFamily;
                    secondaryCategoryAxis.Style.FontWeight = this.chartSecondaryCategoryAxisProperties.FontWeight;
                    secondaryCategoryAxis.Style.Color = this.chartSecondaryCategoryAxisProperties.FontColor;
                    secondaryCategoryAxis.Angle = float.Parse(new RDL.DOM.Size(this.chartSecondaryCategoryAxisProperties.FontAngle).PixelValue.ToString());
                    if (secondaryCategoryAxis.Angle != 0 || secondaryCategoryAxis.Style.FontSize.size != "8pt")
                    {
                        secondaryCategoryAxis.LabelsAutoFitDisabled = true;
                    }
                    secondaryCategoryAxis.ChartMajorGridLines.Enabled = RDL.DOM.BooleanOptions.False; 
                    secondaryCategoryAxis.ChartMinorGridLines.Enabled = RDL.DOM.BooleanOptions.False; 

                    secondaryCategoryAxis.ChartMajorTickMarks.Enabled = (RDL.DOM.BooleanOptions)Enum.Parse(typeof(RDL.DOM.BooleanOptions), this.chartSecondaryCategoryAxisProperties.EnableMajorTickMarks, true);
                    secondaryCategoryAxis.ChartMinorTickMarks.Enabled = (RDL.DOM.BooleanOptions)Enum.Parse(typeof(RDL.DOM.BooleanOptions), this.chartSecondaryCategoryAxisProperties.EnableMinorTickMarks, true);
                    secondaryCategoryAxis.ChartMajorTickMarks.Length = float.Parse(new RDL.DOM.Size(this.chartSecondaryCategoryAxisProperties.TickLength).FloatValue.ToString());
                    secondaryCategoryAxis.ChartMajorTickMarks.Style.Border.Color = this.chartSecondaryCategoryAxisProperties.TickColor;
                    secondaryCategoryAxis.ChartMajorTickMarks.Style.Border.Style = this.chartSecondaryCategoryAxisProperties.TickStyle;
                    secondaryCategoryAxis.ChartMajorTickMarks.Style.Border.Width = this.chartSecondaryCategoryAxisProperties.TickLength;
                    if (this.chartSecondaryCategoryAxisProperties.HideAxisLabels.ToLower() == "true")
                    {
                        secondaryCategoryAxis.Style.Color = "White";
                    }
                    if (this.chartSecondaryCategoryAxisProperties.EnableMajorTickMarks.ToLower() == "false")
                    {
                        secondaryCategoryAxis.ChartMajorTickMarks.Style.Border.Width = "0pt";
                    }
                    secondaryCategoryAxis.ChartMinorTickMarks.Length = float.Parse(new RDL.DOM.Size(this.chartSecondaryCategoryAxisProperties.TickWidth).FloatValue.ToString());
                    secondaryCategoryAxis.ChartMinorTickMarks.Style.Border.Color = this.chartSecondaryCategoryAxisProperties.MinorTickColor;
                    secondaryCategoryAxis.ChartMinorTickMarks.Style.Border.Style = this.chartSecondaryCategoryAxisProperties.MinorTickStyle;
                    secondaryCategoryAxis.ChartMinorTickMarks.Style.Border.Width = this.chartSecondaryCategoryAxisProperties.TickWidth;

                    if (this.SecondaryXAxisTitle != null && this.SecondaryXAxisTitle.Visibility == Visibility.Visible)
                    {
                        secondaryCategoryAxis.ChartAxisTitle = new RDL.DOM.ChartAxisTitle();
                        secondaryCategoryAxis.ChartAxisTitle.Style = new RDL.DOM.Style();
                        secondaryCategoryAxis.ChartAxisTitle.Caption = this.secondaryXAxisTitleProperties.Name;
                        switch (this.secondaryXAxisTitleProperties.TitleAlignment.ToLower())
                        {
                            case "far":
                                secondaryCategoryAxis.ChartAxisTitle.Position = RDL.DOM.ChartAxisTitlePosition.Far;
                                break;
                            case "near":
                                secondaryCategoryAxis.ChartAxisTitle.Position = RDL.DOM.ChartAxisTitlePosition.Near;
                                break;
                            default:
                                secondaryCategoryAxis.ChartAxisTitle.Position = RDL.DOM.ChartAxisTitlePosition.Center;
                                break;
                        }
                        secondaryCategoryAxis.ChartAxisTitle.Style.FontSize = this.secondaryXAxisTitleProperties.FontSize;
                        secondaryCategoryAxis.ChartAxisTitle.Style.BackgroundColor = Brushes.White.ToString();
                        secondaryCategoryAxis.ChartAxisTitle.Style.FontWeight = RDL.DOM.FontWeight.Normal.ToString();
                        secondaryCategoryAxis.ChartAxisTitle.Style.FontStyle = this.secondaryXAxisTitleProperties.FontStyle;
                        secondaryCategoryAxis.ChartAxisTitle.Style.FontFamily = this.secondaryXAxisTitleProperties.FontFamily;
                        secondaryCategoryAxis.ChartAxisTitle.Style.Color = this.secondaryXAxisTitleProperties.FontColor;
                    }

                    secondaryAxis.Name = "Primary";
                    if (this.chartValueAxisProperties.ReverseDirection.ToLower() == "false")
                    {
                        secondaryAxis.Reverse = false;
                    }
                    else
                    {
                        secondaryAxis.Reverse = true;
                    }
                    secondaryAxis.Style.Border.Color = this.chartValueAxisProperties.LineColor;
                    secondaryAxis.Style.Border.Style = this.chartValueAxisProperties.LineStyle;
                    secondaryAxis.Style.Border.Width = this.chartValueAxisProperties.LineWidth;
                    secondaryAxis.Style.FontSize = this.chartValueAxisProperties.FontSize;
                    secondaryAxis.Style.FontFamily = this.chartValueAxisProperties.FontFamily;
                    secondaryAxis.Style.FontWeight = this.chartValueAxisProperties.FontWeight;
                    secondaryAxis.Style.Color = this.chartValueAxisProperties.FontColor;
                    secondaryAxis.Angle = float.Parse(new RDL.DOM.Size(this.chartValueAxisProperties.FontAngle).PixelValue.ToString());

                    if (secondaryAxis.Angle != 0 || secondaryAxis.Style.FontSize.size != "8pt")
                    {
                        secondaryAxis.LabelsAutoFitDisabled = true;
                    }

                    secondaryAxis.ChartMajorGridLines.Enabled = (RDL.DOM.BooleanOptions)Enum.Parse(typeof(RDL.DOM.BooleanOptions), this.defaultChartProperties.ValueAxisMajorGridLines.EnableMajorGridLines);
                    secondaryAxis.ChartMinorGridLines.Enabled = (RDL.DOM.BooleanOptions)Enum.Parse(typeof(RDL.DOM.BooleanOptions), this.defaultChartProperties.ValueAxisMinorGridLines.EnableMinorGridLines);

                    if (secondaryAxis.ChartMajorGridLines.Enabled == RDL.DOM.BooleanOptions.Auto || secondaryAxis.ChartMajorGridLines.Enabled == RDL.DOM.BooleanOptions.True)
                    {
                        secondaryAxis.ChartMajorGridLines.Style.Border.Color = this.defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesColor;
                        secondaryAxis.ChartMajorGridLines.Style.Border.Style = this.defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesStyle;
                        secondaryAxis.ChartMajorGridLines.Style.Border.Width = this.defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesWidth;
                    }
                    if (secondaryAxis.ChartMinorGridLines.Enabled == RDL.DOM.BooleanOptions.True)
                    {
                        secondaryAxis.ChartMinorGridLines.Style.Border.Color = this.defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesColor;
                        secondaryAxis.ChartMinorGridLines.Style.Border.Style = this.defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesStyle;
                        secondaryAxis.ChartMinorGridLines.Style.Border.Width = this.defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesWidth;
                    }

                    secondaryAxis.ChartMajorTickMarks.Enabled = (RDL.DOM.BooleanOptions)Enum.Parse(typeof(RDL.DOM.BooleanOptions), this.chartValueAxisProperties.EnableMajorTickMarks, true);
                    secondaryAxis.ChartMinorTickMarks.Enabled = (RDL.DOM.BooleanOptions)Enum.Parse(typeof(RDL.DOM.BooleanOptions), this.chartValueAxisProperties.EnableMinorTickMarks, true);
                    secondaryAxis.ChartMajorTickMarks.Length = float.Parse(new RDL.DOM.Size(this.chartValueAxisProperties.TickLength).FloatValue.ToString());
                    secondaryAxis.ChartMajorTickMarks.Style.Border.Color = this.chartValueAxisProperties.TickColor;
                    secondaryAxis.ChartMajorTickMarks.Style.Border.Style = this.chartValueAxisProperties.TickStyle;
                    secondaryAxis.ChartMajorTickMarks.Style.Border.Width = this.chartValueAxisProperties.TickLength;
                    if (this.chartValueAxisProperties.HideAxisLabels.ToLower() == "true")
                    {
                        secondaryAxis.Style.Color = "White";
                    }
                    if (this.chartValueAxisProperties.EnableMajorTickMarks.ToLower() == "false")
                    {
                        secondaryAxis.ChartMajorTickMarks.Style.Border.Width = "0pt";
                    }
                    secondaryAxis.ChartMinorTickMarks.Length = float.Parse(new RDL.DOM.Size(this.chartValueAxisProperties.TickWidth).FloatValue.ToString());
                    secondaryAxis.ChartMinorTickMarks.Style.Border.Color = this.chartValueAxisProperties.MinorTickColor;
                    secondaryAxis.ChartMinorTickMarks.Style.Border.Style = this.chartValueAxisProperties.MinorTickStyle;
                    secondaryAxis.ChartMinorTickMarks.Style.Border.Width = this.chartValueAxisProperties.TickWidth;

                    if (this.ValueAxisTitle != null && this.ValueAxisTitle.Visibility == Visibility.Visible)
                    {
                        secondaryAxis.ChartAxisTitle = new RDL.DOM.ChartAxisTitle();
                        secondaryAxis.ChartAxisTitle.Style = new RDL.DOM.Style();
                        secondaryAxis.ChartAxisTitle.Caption = this.valueAxisTitleProperties.Name;
                        switch (this.valueAxisTitleProperties.TitleAlignment.ToLower())
                        {
                            case "far":
                                secondaryAxis.ChartAxisTitle.Position = RDL.DOM.ChartAxisTitlePosition.Far;
                                break;
                            case "near":
                                secondaryAxis.ChartAxisTitle.Position = RDL.DOM.ChartAxisTitlePosition.Near;
                                break;
                            default:
                                secondaryAxis.ChartAxisTitle.Position = RDL.DOM.ChartAxisTitlePosition.Center;
                                break;
                        }
                        secondaryAxis.ChartAxisTitle.Style.FontSize = this.valueAxisTitleProperties.FontSize;
                        secondaryAxis.ChartAxisTitle.Style.BackgroundColor = Brushes.White.ToString();
                        secondaryAxis.ChartAxisTitle.Style.FontWeight = RDL.DOM.FontWeight.Normal.ToString();
                        secondaryAxis.ChartAxisTitle.Style.FontStyle = this.valueAxisTitleProperties.FontStyle;
                        secondaryAxis.ChartAxisTitle.Style.FontFamily = this.valueAxisTitleProperties.FontFamily;
                        secondaryAxis.ChartAxisTitle.Style.Color = this.valueAxisTitleProperties.FontColor;
                    }
                    if (ChartArea.SecondaryAxis != null && ChartArea.SecondaryAxis.AxisVisibility != System.Windows.Visibility.Visible)
                    {
                        secondaryAxis.Visible = RDL.DOM.BooleanOptions.False;
                    }

                    secondaryValueAxis.Name = "Secondary";
                    if (this.chartSecondaryCategoryAxisProperties.ReverseDirection.ToLower() == "false")
                    {
                        secondaryValueAxis.Reverse = false;
                    }
                    else
                    {
                        secondaryValueAxis.Reverse = true;
                    }
                    secondaryValueAxis.Style.Border.Color = this.chartSecondaryValueAxisProperties.LineColor;
                    secondaryValueAxis.Style.Border.Style = this.chartSecondaryValueAxisProperties.LineStyle;
                    secondaryValueAxis.Style.Border.Width = this.chartSecondaryValueAxisProperties.LineWidth;
                    secondaryValueAxis.Style.FontSize = this.chartSecondaryValueAxisProperties.FontSize;
                    secondaryValueAxis.Style.FontFamily = this.chartSecondaryValueAxisProperties.FontFamily;
                    secondaryValueAxis.Style.FontWeight = this.chartSecondaryValueAxisProperties.FontWeight;
                    secondaryValueAxis.Style.Color = this.chartSecondaryValueAxisProperties.FontColor;
                    secondaryValueAxis.Angle = float.Parse(new RDL.DOM.Size(this.chartSecondaryValueAxisProperties.FontAngle).PixelValue.ToString());
                    if (secondaryValueAxis.Angle != 0 || secondaryValueAxis.Style.FontSize.size != "8pt")
                    {
                        secondaryValueAxis.LabelsAutoFitDisabled = true;
                    }
                    secondaryValueAxis.ChartMajorGridLines.Enabled = RDL.DOM.BooleanOptions.True;
                    secondaryValueAxis.ChartMajorGridLines.Style.Border.Color = "Silver";
                    secondaryValueAxis.ChartMajorGridLines.Style.Border.Style = "Solid";
                    secondaryValueAxis.ChartMajorGridLines.Style.Border.Width = "1pt";                   
                    secondaryValueAxis.ChartMinorGridLines.Enabled = RDL.DOM.BooleanOptions.False;

                    secondaryValueAxis.ChartMajorTickMarks.Enabled = (RDL.DOM.BooleanOptions)Enum.Parse(typeof(RDL.DOM.BooleanOptions), this.chartSecondaryValueAxisProperties.EnableMajorTickMarks, true);
                    secondaryValueAxis.ChartMinorTickMarks.Enabled = (RDL.DOM.BooleanOptions)Enum.Parse(typeof(RDL.DOM.BooleanOptions), this.chartSecondaryValueAxisProperties.EnableMinorTickMarks, true);
                    secondaryValueAxis.ChartMajorTickMarks.Length = float.Parse(new RDL.DOM.Size(this.chartSecondaryValueAxisProperties.TickLength).FloatValue.ToString());
                    secondaryValueAxis.ChartMajorTickMarks.Style.Border.Color = this.chartSecondaryValueAxisProperties.TickColor;
                    secondaryValueAxis.ChartMajorTickMarks.Style.Border.Style = this.chartSecondaryValueAxisProperties.TickStyle;
                    secondaryValueAxis.ChartMajorTickMarks.Style.Border.Width = this.chartSecondaryValueAxisProperties.TickLength;
                    if (this.chartSecondaryValueAxisProperties.HideAxisLabels.ToLower() == "true")
                    {
                        secondaryValueAxis.Style.Color = "White";
                    }
                    if (this.chartSecondaryValueAxisProperties.EnableMajorTickMarks.ToLower() == "false")
                    {
                        secondaryValueAxis.ChartMajorTickMarks.Style.Border.Width = "0pt";
                    }
                    secondaryValueAxis.ChartMinorTickMarks.Length = float.Parse(new RDL.DOM.Size(this.chartSecondaryValueAxisProperties.TickWidth).FloatValue.ToString());
                    secondaryValueAxis.ChartMinorTickMarks.Style.Border.Color = this.chartSecondaryValueAxisProperties.MinorTickColor;
                    secondaryValueAxis.ChartMinorTickMarks.Style.Border.Style = this.chartSecondaryValueAxisProperties.MinorTickStyle;
                    secondaryValueAxis.ChartMinorTickMarks.Style.Border.Width = this.chartSecondaryValueAxisProperties.TickWidth;

                    if (this.SecondaryYAxisTitle != null && this.SecondaryYAxisTitle.Visibility == Visibility.Visible)
                    {
                        secondaryValueAxis.ChartAxisTitle = new RDL.DOM.ChartAxisTitle();
                        secondaryValueAxis.ChartAxisTitle.Style = new RDL.DOM.Style();
                        secondaryValueAxis.ChartAxisTitle.Caption = this.secondaryYAxisTitleProperties.Name;
                        switch (this.secondaryYAxisTitleProperties.TitleAlignment.ToLower())
                        {
                            case "far":
                                secondaryValueAxis.ChartAxisTitle.Position = RDL.DOM.ChartAxisTitlePosition.Far;
                                break;
                            case "near":
                                secondaryValueAxis.ChartAxisTitle.Position = RDL.DOM.ChartAxisTitlePosition.Near;
                                break;
                            default:
                                secondaryValueAxis.ChartAxisTitle.Position = RDL.DOM.ChartAxisTitlePosition.Center;
                                break;
                        }
                        secondaryValueAxis.ChartAxisTitle.Style.FontSize = this.secondaryYAxisTitleProperties.FontSize;
                        secondaryValueAxis.ChartAxisTitle.Style.BackgroundColor = Brushes.White.ToString();
                        secondaryValueAxis.ChartAxisTitle.Style.FontWeight = RDL.DOM.FontWeight.Normal.ToString();
                        secondaryValueAxis.ChartAxisTitle.Style.FontStyle = this.secondaryYAxisTitleProperties.FontStyle;
                        secondaryValueAxis.ChartAxisTitle.Style.FontFamily = this.secondaryYAxisTitleProperties.FontFamily;
                        secondaryValueAxis.ChartAxisTitle.Style.Color = this.secondaryYAxisTitleProperties.FontColor;
                    }

                    chartArea.ChartCategoryAxes.Add(primaryAxis);
                    chartArea.ChartCategoryAxes.Add(secondaryCategoryAxis);
                    chartArea.ChartValueAxes.Add(secondaryAxis);
                    chartArea.ChartValueAxes.Add(secondaryValueAxis);
                    ChartObj.ChartAreas.Add(chartArea);
                }

                if (this.ChartLegand != null && this.ChartLegand.Visibility == Visibility.Visible)
                {
                    ChartObj.ChartLegends = new RDL.DOM.ChartLegends();
                    RDL.DOM.ChartLegend chartLegand = new RDL.DOM.ChartLegend();
                    chartLegand.Style = new RDL.DOM.Style();
                    chartLegand.Style.FontSize = this.chartLegendProperties.FontSize;
                    chartLegand.Style.FontFamily = this.chartLegendProperties.FontFamily;
                    chartLegand.Style.FontStyle = this.chartLegendProperties.FontStyle;
                    chartLegand.Style.Color = this.chartLegendProperties.FontColor;
                    chartLegand.Style.Border = new RDL.DOM.Border();
                    chartLegand.Style.Border.Width = this.chartLegendProperties.BorderThickness;
                    chartLegand.Style.Border.Color = this.chartLegendProperties.LegendBorderColor;
                    if (this.ChartLegand != null)
                    {
                        if (this.chartLegendProperties.Hidden.ToLower() == "true")
                            chartLegand.Hidden = false;
                        else
                            chartLegand.Hidden = true;
                        if (string.IsNullOrEmpty(this.chartLegendProperties.Name))
                        {
                            chartLegand.Name = "ChartLegend";
                        }
                        else
                        {
                            chartLegand.Name = this.chartLegendProperties.Name;
                        }
                        chartLegand.Layout = (RDL.DOM.Layout)Enum.Parse(typeof(RDL.DOM.Layout), this.chartLegendProperties.Layout, true);
                        if (this.chartLegendProperties.Position.StartsWith("="))
                            chartLegand.Position = (RDL.DOM.Positions)Enum.Parse(typeof(RDL.DOM.Positions), this.chartLegendProperties.Position.Remove(0, 1), true);
                        else
                            chartLegand.Position = (RDL.DOM.Positions)Enum.Parse(typeof(RDL.DOM.Positions), this.chartLegendProperties.Position, true);
                        chartLegand.Style.BackgroundColor = this.chartLegendProperties.BackFill;

                        if (this.ShowLegendTitle.IsChecked)
                        {
                            chartLegand.ChartLegendTitle = new RDL.DOM.ChartLegendTitle();
                            chartLegand.ChartLegendTitle.Style = new RDL.DOM.Style();
                            chartLegand.ChartLegendTitle.Caption = this.legendTitleProperties.Caption;
                            chartLegand.ChartLegendTitle.Style.BackgroundColor = this.legendTitleProperties.BackFill;
                            chartLegand.ChartLegendTitle.Style.FontSize = this.legendTitleProperties.FontSize;
                            chartLegand.ChartLegendTitle.Style.FontWeight = RDL.DOM.FontWeight.Normal.ToString();
                            chartLegand.ChartLegendTitle.Style.FontStyle = this.legendTitleProperties.FontStyle;
                            chartLegand.ChartLegendTitle.Style.FontFamily = this.legendTitleProperties.FontFamily;
                            chartLegand.ChartLegendTitle.Style.Color = this.legendTitleProperties.FontColor;
                        }

                    }

                    ChartObj.ChartLegends.Add(chartLegand);
                }

                if (this.ChartTitle != null && this.ChartTitle.Visibility == Visibility.Visible)
                {
                    ChartObj.ChartTitles = new RDL.DOM.ChartTitles();
                    RDL.DOM.ChartTitle chartTitle = new RDL.DOM.ChartTitle();
                    chartTitle.Style = new RDL.DOM.Style();

                    chartTitle.Name = this.chartProperties.Name;
                    chartTitle.Caption = "Chart Title";
                    chartTitle.Hidden = false;
                    chartTitle.Position = RDL.DOM.Positions.TopCenter;
                    chartTitle.Style.BackgroundColor = this.titleProperties.TitleBackFill;
                    chartTitle.Style.FontSize = this.titleProperties.TitleFontSize;
                    chartTitle.Style.FontWeight = RDL.DOM.FontWeight.Normal.ToString();
                    chartTitle.Style.FontStyle = this.titleProperties.TitleFontStyle;
                    chartTitle.Style.FontFamily = this.titleProperties.TitleFontFamily;
                    chartTitle.Style.Color = this.titleProperties.TitleFontColor;
                    ChartObj.ChartTitles.Add(chartTitle);
                }
            }

            return ChartObj;
        }

        private RDL.DOM.Chart UpdateChartSeries(RDL.DOM.Chart ChartObj)
        {
            ChartObj.ChartCategoryHierarchy = new RDL.DOM.ChartCategoryHierarchy();
            ChartObj.ChartCategoryHierarchy.ChartMembers = new RDL.DOM.ChartMembers();
            ChartObj.ChartSeriesHierarchy = new RDL.DOM.ChartSeriesHierarchy();
            ChartObj.ChartSeriesHierarchy.ChartMembers = new RDL.DOM.ChartMembers();
            ChartObj.ChartData = new RDL.DOM.ChartData();
            ChartObj.ChartData.ChartSeriesCollection = new RDL.DOM.ChartSeriesCollection();
            RDL.DOM.ChartMember chartMember = new RDL.DOM.ChartMember();
            chartMember.Label = string.Empty;

            if (this.ColumnPanel != null)
            {
                if (this.ColumnPanel.Children.Count != 0)
                {
                    chartMember.Group = new RDL.DOM.Group();
                    RDL.DOM.GroupExpressions groupExpressions = new RDL.DOM.GroupExpressions();
                    RDL.DOM.GroupExpression groupExpression = new RDL.DOM.GroupExpression();
                    string chartCategoryValue = string.Empty;
                    string str = (this.ColumnPanel.Children[0] as Button).Content.ToString();
                    int i = str.IndexOf("[");
                    int j = str.IndexOf("]");
                    int l = j - i;
                    string subFieldName = string.Empty;

                    if (l > 0)
                    {
                        subFieldName = str.Substring(i + 1, l - 1);
                    }
                    else
                    {
                        subFieldName = str;
                    }

                    subFieldName = subFieldName.Replace("__", "_");
                    chartCategoryValue = "=" + "Fields!" + subFieldName + ".Value";
                    chartMember.Label = chartCategoryValue;
                    chartMember.Group.Name = ChartObj.Name+"_CategoryGroup"; 
                    groupExpression.Value = chartCategoryValue;

                    groupExpressions.Add(groupExpression);
                    chartMember.Group.GroupExpressions = groupExpressions;
                }
                ChartObj.ChartCategoryHierarchy.ChartMembers.Add(chartMember);
            }
            if (this.Seriespanel!=null && this.Seriespanel.Children.Count != 0)
            {
                RDL.DOM.ChartMember sreieschartmember = new RDL.DOM.ChartMember();
                sreieschartmember.Group = new RDL.DOM.Group();
                sreieschartmember.Group.GroupExpressions = new RDL.DOM.GroupExpressions();
                RDL.DOM.GroupExpression groupexpression = new RDL.DOM.GroupExpression();
                string str = (this.Seriespanel.Children[0] as Button).Content.ToString();
                int i = str.IndexOf("[");
                int j = str.IndexOf("]");
                int l = j - i;
                string subFieldName = string.Empty;

                if (l > 0)
                {
                    subFieldName = str.Substring(i + 1, l - 1);
                }
                else
                {
                    subFieldName = str;
                }

                subFieldName = subFieldName.Replace("__", "_");
                sreieschartmember.Label = "=" + "Fields!" + subFieldName + ".Value";
                sreieschartmember.Group.Name = ChartObj.Name+"_SeriesGroup1";
                groupexpression.Value = "=" + "Fields!" + subFieldName + ".Value";
                sreieschartmember.Group.GroupExpressions.Add(groupexpression);
                sreieschartmember.ChartMembers = new RDL.DOM.ChartMembers();
                foreach (ChartSeries chartseries in this.InnerChart.Areas[0].Series)
                {
                    RDL.DOM.ChartMember chartmember1 = new RDL.DOM.ChartMember();
                    chartmember1.Label = chartseries.Label;
                    sreieschartmember.ChartMembers.Add(chartmember1);
                }
                ChartObj.ChartSeriesHierarchy.ChartMembers.Add(sreieschartmember);

            }
            else
            {
                foreach (ChartSeries chartseries in this.InnerChart.Areas[0].Series)
                {
                    RDL.DOM.ChartMember chartmember = new RDL.DOM.ChartMember();
                    chartmember.Label = chartseries.Label;
                    if (this.ChartControlType == Controls.ChartControlType.Sparkline)
                    {
                        chartmember.Group = new RDL.DOM.Group();
                        chartmember.Group.Name = chartseries.Name;
                    }
                    ChartObj.ChartSeriesHierarchy.ChartMembers.Add(chartmember);
                }
            }
            int x = 0;
            foreach (ChartSeries chartSeries in this.InnerChart.Areas[0].Series)
            {

                RDL.DOM.ChartSeries chartseries = new RDL.DOM.ChartSeries();
                chartseries.ChartDataPoints = new RDL.DOM.ChartDataPoints();
                RDL.DOM.ChartDataPoint ChartDataPoint = new RDL.DOM.ChartDataPoint();
                ChartDataPoint.Style = new RDL.DOM.Style();
                ChartDataPoint.Style.Border = new RDL.DOM.Border();
                ChartDataPoint.ChartMarker = new RDL.DOM.ChartMarker();
                ChartDataPoint.ChartMarker.Style = new RDL.DOM.Style();
                ChartDataPoint.ChartDataPointValues = new RDL.DOM.ChartDataPointValues();
                ChartDataPoint.ChartDataLabel = new RDL.DOM.ChartDataLabel();

                chartseries.Name = chartSeries.Name;

                chartseries.ChartDataLabel = new RDL.DOM.ChartDataLabel();
                if (this.chartSeriesPropertiesCollection.Count > 0)
                {
                    chartseries.ValueAxisName = this.chartSeriesPropertiesCollection[x].ValueAxisName;
                    chartseries.CategoryAxisName = this.chartSeriesPropertiesCollection[x].CategoryAxisName;
                }
                else
                {
                    chartseries.ValueAxisName = "Primary";
                    chartseries.CategoryAxisName = "Primary";
                }

                if (this.Seriespanel != null && this.Seriespanel.Children.Count != 0)
                {

                }
                if ((this.ValuePanel != null) && (this.ValuePanel.Children.Count != 0))
                {
                    string chartSeriesValue = string.Empty;
                    foreach (UIElement button in this.ValuePanel.Children)
                    {
                        string str = (button as Button).Content.ToString();
                        str = str.Replace("__", "_");
                        int i = str.IndexOf("[");
                        string fieldName = string.Empty;

                        if (str.Contains('('))
                        {
                            int j = str.IndexOf("(");
                            int k = str.IndexOf(")");
                            int l = j - i;
                            int l1 = k - j;
                            string functionName = str.Substring(i + 1, l - 1);
                            fieldName = str.Substring(j + 1, l1 - 1);

                            if (fieldName == chartSeries.Label)
                            {
                                chartSeriesValue = "=" + functionName + "(" + "Fields!" + fieldName + ".Value" + ")";
                                ChartDataPoint.ChartDataPointValues.Y = chartSeriesValue;
                            }
                        }
                        else
                        {
                            fieldName = str.Substring(i + 1, str.IndexOf("]") - i - 1);

                            if (fieldName == chartSeries.Label)
                            {
                                chartSeriesValue = "Fields!" + fieldName + ".Value";
                                ChartDataPoint.ChartDataPointValues.Y = chartSeriesValue;
                            }
                        }
                    }
                }

                switch (chartSeries.Type)
                {
                    case ChartTypes.Area:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Area;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Plain;
                            break;
                        }
                    case ChartTypes.Bubble:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Scatter;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Bubble;
                            break;
                        }
                    case ChartTypes.Bar:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Bar;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Plain;
                            break;
                        }
                    case ChartTypes.Column:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Column;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Plain;
                            break;
                        }
                    case ChartTypes.Doughnut:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Shape;
                            if (IsExploded)
                            { 
                                chartseries.Subtype = RDL.DOM.VisualizationSubType.ExplodedDoughnut; 
                            }   
                            else   
                            {     
                                chartseries.Subtype = RDL.DOM.VisualizationSubType.Doughnut; 
                            } 
                            break;
                        }
                    case ChartTypes.FastLine:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Line;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Smooth;
                            break;
                        }
                    case ChartTypes.Funnel:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Shape;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Funnel;
                            break;
                        }
                    case ChartTypes.Line:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Line;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Plain;
                            break;
                        }
                    case ChartTypes.Pie:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Shape;
                            if (IsExploded)
                            {     
                                chartseries.Subtype = RDL.DOM.VisualizationSubType.ExplodedPie; 
                            }                        
                            else 
                            { 
                                chartseries.Subtype = RDL.DOM.VisualizationSubType.Plain;
                            } 
                            break;
                        }
                    case ChartTypes.Polar:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Polar;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Radar;
                            break;
                        }
                    case ChartTypes.Pyramid:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Shape;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Pyramid;
                            break;
                        }
                    case ChartTypes.Radar:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Polar;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Plain;
                            break;
                        }
                    case ChartTypes.RangeArea:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Range;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Plain;
                            break;
                        }
                    case ChartTypes.StackingArea:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Area;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Stacked;
                            break;
                        }
                    case ChartTypes.StackingColumn:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Column;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Stacked;
                            break;
                        }
                    case ChartTypes.StackingColumn100:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Column;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.PercentStacked;
                            break;
                        }
                    case ChartTypes.StepLine:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Line;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Stepped;
                            break;
                        }
                    case ChartTypes.StackingBar:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Bar;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Stacked;
                            break;
                        }
                    case ChartTypes.Scatter:
                        {
                            chartseries.Type = RDL.DOM.VisualizationType.Scatter;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Plain;
                            break;
                        }
                    case ChartTypes.Spline:  
                        {                 
                            chartseries.Type = RDL.DOM.VisualizationType.Line;   
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Smooth;   
                            break;                
                        }                 
                    case ChartTypes.SplineArea:         
                        { 
                            chartseries.Type = RDL.DOM.VisualizationType.Area;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.Smooth;
                            break; 
                        } 
                    case ChartTypes.StackingArea100: 
                        {  
                            chartseries.Type = RDL.DOM.VisualizationType.Area;
                            chartseries.Subtype = RDL.DOM.VisualizationSubType.PercentStacked;
                            break;
                        }  
                }

                if (chartSeries.Interior is SolidColorBrush)
                {
                    ChartDataPoint.Style.Color = ((SolidColorBrush)chartSeries.Interior).Color.ToString();
                }

                ChartDataPoint.Style.Border.Width = (chartSeries.StrokeThickness.ToString() + "pt");
                ChartDataPoint.Style.Border.Color = chartSeries.Stroke.ToString();

                if (chartSeries.AdornmentsInfo.Visible == false)
                {
                    ChartDataPoint.ChartMarker.Type = "None";
                }
                else
                {
                    switch (chartSeries.AdornmentsInfo.Symbol)
                    {
                        case Symbol.Cross:
                            {
                                ChartDataPoint.ChartMarker.Type = "Cross";
                                break;
                            }
                        case Symbol.Diamond:
                            {
                                ChartDataPoint.ChartMarker.Type = "Diamond";
                                break;
                            }
                        case Symbol.Ellipse:
                            {
                                ChartDataPoint.ChartMarker.Type = "Ellipse";
                                break;
                            }
                        case Symbol.Hexagon:
                            {
                                ChartDataPoint.ChartMarker.Type = "Hexagon";
                                break;
                            }
                        case Symbol.InvertedTriangle:
                            {
                                ChartDataPoint.ChartMarker.Type = "InvertedTriangle";
                                break;
                            }
                        case Symbol.Pentagon:
                            {
                                ChartDataPoint.ChartMarker.Type = "Pentagon";
                                break;
                            }
                        case Symbol.Plus:
                            {
                                ChartDataPoint.ChartMarker.Type = "Plus";
                                break;
                            }
                        case Symbol.Square:
                            {
                                ChartDataPoint.ChartMarker.Type = "Square";
                                break;
                            }
                        case Symbol.Triangle:
                            {
                                ChartDataPoint.ChartMarker.Type = "Triangle";
                                break;
                            }
                    }
                    ChartDataPoint.ChartMarker.Size = chartSeries.AdornmentsInfo.SymbolHeight.ToString() + "pt";
                    if (chartSeries.AdornmentsInfo.SymbolInterior != null)
                    {
                        ChartDataPoint.ChartMarker.Style.Color = chartSeries.AdornmentsInfo.SymbolInterior.ToString();
                    }
                }
                chartseries.ChartDataPoints.Add(ChartDataPoint);
                ChartObj.ChartData.ChartSeriesCollection.Add(chartseries);

                if (this.chartSeriesPropertiesCollection.Count > 0)
                {
                    if (this.chartSeriesPropertiesCollection[x].ShowDataLabels.ToLower() == "true")
                    {
                        ChartDataPoint.ChartDataLabel.Visible = true;
                        ChartDataPoint.ChartDataLabel.UseValueAsLabel = true;
                        if (this.chartSeriesPropertiesCollection[x].DataLabelsPosition.StartsWith("="))
                        {
                            ChartDataPoint.ChartDataLabel.Position = RDL.DOM.Position.Default;
                        }
                        else
                        {
                            ChartDataPoint.ChartDataLabel.Position = (RDL.DOM.Position)Enum.Parse(typeof(RDL.DOM.Position), this.chartSeriesPropertiesCollection[x].DataLabelsPosition, true);
                        }
                    }
                    else
                    {
                        ChartDataPoint.ChartDataLabel.Visible = false;
                        ChartDataPoint.ChartDataLabel.UseValueAsLabel = false;
                    }
                }
                x++;
            }
            return ChartObj;
        }

        void ColumnDataField_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ColumnDataField.ContextMenu != null)
            {
                ColumnDataField.ContextMenu = null;
            }
        }

        void ValueDataField_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ValueDataField.ContextMenu != null)
            {
                ValueDataField.ContextMenu = null;
            }
        }

        void AddDataSource_Click(object sender, RoutedEventArgs e)
        {
            Panel.AddDataSource();
        }

        void AddDataSet_Click(object sender, RoutedEventArgs e)
        {
            Panel.AddDataSet();
        }

        void MenuItem8_Click(object sender, RoutedEventArgs e)
        {
            string header = (sender as MenuItem).Header.ToString();
            string parent = this.DataSetName;
            CreateColumnButtonObj(header, parent);
        }

        void MenuItem9_Click(object sender, RoutedEventArgs e)
        {
            string header = (sender as MenuItem).Header.ToString();
            string parent = this.DataSets[0].Name;
            CreateColumnButtonObj(header, parent);
        }

        void MenuItem7_Click(object sender, RoutedEventArgs e)
        {
            string header = (sender as MenuItem).Header.ToString();
            string parent = ((sender as MenuItem).Parent as MenuItem).Header.ToString();
            CreateColumnButtonObj(header, parent);
        }

        void MenuItem13_Click(object sender, RoutedEventArgs e)
        {
            this.Panel.AddDataSet();
        }

        void MenuItem12_Click(object sender, RoutedEventArgs e)
        {
            this.Panel.AddDataSource();
        }

        void MenuItem1_Click(object sender, RoutedEventArgs e)
        {
            string header = (sender as MenuItem).Header.ToString();
            string parent = this.DataSetName;
            CreateValueButtonObj(header, parent);
        }

        void MenuItem4_Click(object sender, RoutedEventArgs e)
        {
            string header = (sender as MenuItem).Header.ToString();
            string parent = this.DataSets[0].Name;
            CreateValueButtonObj(header, parent);
        }

        void MenuItem3_Click(object sender, RoutedEventArgs e)
        {
            string header = (sender as MenuItem).Header.ToString();
            string parent = ((sender as MenuItem).Parent as MenuItem).Header.ToString();
            CreateValueButtonObj(header, parent);
        }

        # endregion

        # region Common Events

        void ShowLegendTitle_Unchecked(object sender, RoutedEventArgs e)
        {
            LegendTextbox.Visibility = Visibility.Collapsed;
            this.RemoveSelectionAdorner(LegendTextbox);
        }

        void ShowLegendTitle_Checked(object sender, RoutedEventArgs e)
        {
            LegendTextbox.Visibility = Visibility.Visible;
            LegendTextbox.BorderThickness = new Thickness(0);
            LegendTextbox.TextAlignment = TextAlignment.Center;
            LegendTextbox.Text = this.legendTitleProperties.Caption;
            LegendTextbox.FontFamily = new FontFamily(this.legendTitleProperties.FontFamily);
            LegendTextbox.FontSize = Convert.ToDouble(new RDL.DOM.Size(this.LegendTextbox.FontSize).PixelValue);
            LegendTextbox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.legendTitleProperties.FontColor));
            LegendTextbox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.legendTitleProperties.BackFill));
            switch (this.legendTitleProperties.FontStyle)
            {
                case "italic":
                    LegendTextbox.FontStyle = FontStyles.Italic;
                    break;
                default:
                    LegendTextbox.FontStyle = FontStyles.Normal;
                    break;
            }
            this.RemoveSelectionAdorner(ChartLegand);
            AdornerLayer.GetAdornerLayer(LegendTextbox).Add(new ChartObjectSelectionAdorner(LegendTextbox, Editors.ChartObject.LegendTitle, new Thickness(0), new Size()));
            this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.legendTitleProperties, IsSelected = true });
            LegendTextbox.Focusable = false;
        }

        void RemoveChartLegand_Click(object sender, RoutedEventArgs e)
        {
            this.ChartLegand.Visibility = Visibility.Collapsed;
            this.RemoveSelectionAdorner(ChartLegand);
            ChangeContextMenuVisibility();
        }

        void ShowChartLegend_Click(object sender, RoutedEventArgs e)
        {
            this.ChartLegand.Visibility = Visibility.Visible;
            this.RemoveChartObjectSelectionAdorner();
            AdornerLayer.GetAdornerLayer(ChartLegand).Add(new ChartObjectSelectionAdorner(ChartLegand, Editors.ChartObject.ChartLegend, new Thickness(0), new Size()));
            ChangeContextMenuVisibility();
        }


        void ShowCategoryAxisTitle_Click(object sender, RoutedEventArgs e)
        {

            ChangeContextMenuVisibility();
        }

        void ShowChartTitle_Click(object sender, RoutedEventArgs e)
        {
            AddChartTitle();
            ChangeContextMenuVisibility();
        }

        void DeleteChart_Click(object sender, RoutedEventArgs e)
        {
            DeleteChartRaised();
        }

        private void ChartProperties_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdateChartProperties();

            if (controlProperties.ShowDialog() == true)
            {
                this.chartProperties.IsInternalPropertyChange = true;
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);

                foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
                {
                    if (uiElement is ChartGeneral)
                    {
                        ChartGeneral chartgeneral = (ChartGeneral)uiElement;
                        this.chartProperties.Name = chartgeneral.GeneralName.Text;
                        this.chartProperties.ToolTip = chartgeneral.txt_GeneralToolTip.Text;
                        foreach (ChartSeries chartseries in this.InnerChart.Areas[0].Series)
                        {
                            this.chartProperties.ChartType = chartgeneral.cmb_ChartType.TextValue;
                        }

                    }
                    else if (uiElement is ChartBorder)
                    {
                        ChartBorder chartborder = (ChartBorder)uiElement;
                        this.chartProperties.BorderWidth = chartborder.ChartBorderWidth.TextValue;
                        if (chartborder.ChartBorderColor.Text.ToLower() != "black")
                            this.chartProperties.BorderColor = chartborder.ChartBorderColor.Text;
                    }
                    else if (uiElement is ChartBackground)
                    {
                        ChartBackground chartbackground = (ChartBackground)uiElement;
                        this.chartProperties.ChartBackground = chartbackground.clrpkr_ChartBackground.Text;
                    }
                    else if (uiElement is ChartAreaBackground)
                    {
                        ChartAreaBackground chartareabackground = (ChartAreaBackground)uiElement;
                        this.chartProperties.FillStyle = chartareabackground.cmb_FillStyle.TextValue;
                        this.chartProperties.PrimaryColor = chartareabackground.clrpkr_ColorFill.Text;
                        this.chartProperties.SecondaryColor = chartareabackground.clrpkr_SecondaryColor.Text;
                        this.chartProperties.GradientStyle = chartareabackground.cmb_GradientStyle.TextValue;
                    }
                }
                change.NewValue = this.GetReportItem();
                this.chartProperties.IsInternalPropertyChange = false;
            }
        }

        private ControlProperties UpdateChartProperties()
        {
            ControlProperties controlProperties = new ControlProperties(this, this.Report, ChartChild.Chart);
            this.Panel.UpdateOwnerWindow(controlProperties);
            controlProperties.ChartGeneral.GeneralName.Text = this.chartProperties.Name;
            controlProperties.ChartGeneral.txt_GeneralToolTip.Text = this.chartProperties.ToolTip;
            if (this.chartProperties.ChartType != null)
            {
                controlProperties.ChartGeneral.cmb_ChartType.TextValue = this.chartProperties.ChartType;
            }
            controlProperties.ChartBorder.ChartBorderColor.Text = this.chartProperties.BorderColor;
            controlProperties.ChartBorder.ChartBorderWidth.TextValue = this.chartProperties.BorderWidth;
            controlProperties.ChartBackground.ChartBackgroundFill.Text = this.chartProperties.ChartBackground;
            controlProperties.ChartAreaBackground.cmb_FillStyle.TextValue = this.chartProperties.FillStyle;
            switch (this.chartProperties.GradientStyle.ToLower())
            {
                case "leftright":
                    {
                        controlProperties.ChartAreaBackground.cmb_GradientStyle.TextValue = "LeftRight";
                        break;
                    }
                case "topbottom":
                    {
                        controlProperties.ChartAreaBackground.cmb_GradientStyle.TextValue = "TopBottom";
                        break;
                    }
                default:
                    {
                        controlProperties.ChartAreaBackground.cmb_GradientStyle.TextValue = "LeftRight";
                        break;
                    }

            }
            controlProperties.ChartAreaBackground.ColorFill.Text = this.chartProperties.PrimaryColor;
            controlProperties.ChartAreaBackground.SecondaryColor.Text = this.chartProperties.SecondaryColor;

            return controlProperties;
        }


        private void SeriesProperties_Click(object sender, RoutedEventArgs e)
        {
            this.GetSeriesCount(this.ChartSeries);
            ControlProperties controlProperties = UpdateSeriesProperties();

            if (this.ChartSeries.Type == ChartTypes.Doughnut || this.ChartSeries.Type == ChartTypes.Funnel || this.ChartSeries.Type == ChartTypes.Pie || this.ChartSeries.Type == ChartTypes.Pyramid || this.ChartSeries.Type == ChartTypes.StackingArea || this.ChartSeries.Type == ChartTypes.StackingColumn100 || this.ChartSeries.Type == ChartTypes.StackingBar)
            {
                controlProperties.SeriesGeneral.groupBox_Adorner.Visibility = Visibility.Collapsed;
            }
            else
            {
                controlProperties.SeriesGeneral.groupBox_Adorner.Visibility = Visibility.Visible;
            }
            if (this.ChartSeries.Type == ChartTypes.Doughnut || this.ChartSeries.Type == ChartTypes.Funnel || this.ChartSeries.Type == ChartTypes.Pie || this.ChartSeries.Type == ChartTypes.Pyramid || this.ChartSeries.Type == ChartTypes.Polar || this.ChartSeries.Type == ChartTypes.Radar)
            {
                controlProperties.SeriesAxes.stackPanel_Axes.IsEnabled = false;
            }
            else
            {
                controlProperties.SeriesAxes.stackPanel_Axes.IsEnabled = true;
            }

            controlProperties.ShowDialog();
            this.chartSeriesPropertiesCollection[seriesCount].IsInternalPropertyChange = true;
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange =change;
            this.Panel.EditingManager.AddAction(action);

            foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
            {
                if (uiElement is SeriesGeneral1)
                {

                    this.chartSeriesPropertiesCollection[seriesCount].SeriesColor = controlProperties.SeriesGeneral.clrpkr_SeriesColor.Text;
                    this.chartSeriesPropertiesCollection[seriesCount].BorderColor = controlProperties.SeriesGeneral.clrpkr_SeriesBorderColor.Text;
                    this.chartSeriesPropertiesCollection[seriesCount].BorderWidth = controlProperties.SeriesGeneral.updwn_SeriesBorderWidth.TextValue;
                    ChartTypes chartType = (ChartTypes)Enum.Parse(typeof(ChartTypes), controlProperties.SeriesGeneral.cmb_ChartType.TextValue, true);
                    if (chartType == ChartTypes.Bar || chartType == ChartTypes.StackingBar || chartType == ChartTypes.Polar || chartType == ChartTypes.Funnel || chartType == ChartTypes.Pyramid || chartType == ChartTypes.Radar || chartType == ChartTypes.Pie || chartType == ChartTypes.Doughnut || chartType == ChartTypes.StackingColumn100 || chartType == ChartTypes.StackingBar100)
                    {
                        int i = 0;
                        foreach (ChartSeries chartseries in this.InnerChart.Areas[0].Series)
                        {
                            chartseries.Type = chartType;
                            this.chartSeriesPropertiesCollection[i].ChartType = controlProperties.SeriesGeneral.cmb_ChartType.TextValue;
                            i++;
                        }
                    }
                    else
                    {
                        foreach (ChartSeries chartseries in this.InnerChart.Areas[0].Series)
                        {
                            if (chartseries.Type == ChartTypes.Bar || chartseries.Type == ChartTypes.StackingBar || chartseries.Type == ChartTypes.Polar || chartseries.Type == ChartTypes.Funnel || chartseries.Type == ChartTypes.Pyramid || chartseries.Type == ChartTypes.Radar || chartseries.Type == ChartTypes.Pie || chartseries.Type == ChartTypes.Doughnut || chartseries.Type == ChartTypes.Bubble || chartseries.Type == ChartTypes.RangeArea || chartseries.Type == ChartTypes.StackingColumn100 || chartseries.Type == ChartTypes.StackingBar100)
                            {
                                chartseries.Type = chartType;
                            }
                        }
                        this.chartSeriesPropertiesCollection[seriesCount].ChartType = controlProperties.SeriesGeneral.cmb_ChartType.Text;
                    }

                    if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                    {
                        this.ValueAxisTitle.Margin = new Thickness(10, 5, 10, 10);
                        this.CategoryAxisTitle.Margin = new Thickness(15, 0, 5, 10);
                        SecondaryXAxisTitle.Margin = new Thickness(15, 0, 5, 10);
                        SecondaryYAxisTitle.Margin = new Thickness(10, 5, 10, 10);
                    }
                    else
                    {
                        this.CategoryAxisTitle.Margin = new Thickness(10, 0, 10, 0);
                        this.ValueAxisTitle.Margin = new Thickness(15, 0, 5, 0);
                        SecondaryXAxisTitle.Margin = new Thickness(10, 0, 10, 0);
                        SecondaryYAxisTitle.Margin = new Thickness(15, 0, 5, 0);
                    }

                    this.chartSeriesPropertiesCollection[seriesCount].AdornmentType = controlProperties.SeriesGeneral.cmb_MarkerType.Text;
                    this.chartSeriesPropertiesCollection[seriesCount].Size = controlProperties.SeriesGeneral.updwn_MarkerSize.TextValue;
                    this.chartSeriesPropertiesCollection[seriesCount].AdornmentColor = controlProperties.SeriesGeneral.clrpkr_MarkerColor.Text;

                    if (controlProperties.SeriesGeneral.chk_ShowDataLabels.IsChecked == true)
                    {
                        this.chartSeriesPropertiesCollection[seriesCount].ShowDataLabels = "True";
                    }
                    else
                    {
                        this.chartSeriesPropertiesCollection[seriesCount].ShowDataLabels = "False";
                    }
                    this.chartSeriesPropertiesCollection[seriesCount].DataLabelsPosition = controlProperties.SeriesGeneral.cmb_DatalabelPosition.Text;
                }

                else if (uiElement is SeriesAxes)
                {
                    if (controlProperties.SeriesAxes.rb_PrimaryXAxis.IsChecked == true)
                    {
                        this.chartSeriesPropertiesCollection[seriesCount].CategoryAxisName = "Primary";
                    }
                    else
                    {
                        this.chartSeriesPropertiesCollection[seriesCount].CategoryAxisName = "Secondary";
                    }
                    if (controlProperties.SeriesAxes.rb_PrimaryYAxis.IsChecked == true)
                    {
                        this.chartSeriesPropertiesCollection[seriesCount].ValueAxisName = "Primary";
                    }
                    else
                    {
                        this.chartSeriesPropertiesCollection[seriesCount].ValueAxisName = "Secondary";
                    }
                }
            }
            change.NewValue = this.GetReportItem();
            this.chartSeriesPropertiesCollection[seriesCount].IsInternalPropertyChange = false;
        }

        private ControlProperties UpdateSeriesProperties()
        {
            ControlProperties controlProperties = new ControlProperties(this, this.Report, ChartChild.Series);
            this.Panel.UpdateOwnerWindow(controlProperties);
            try
            {
                controlProperties.SeriesGeneral.updwn_SeriesBorderWidth.TextValue = this.chartSeriesPropertiesCollection[seriesCount].BorderWidth;
                controlProperties.SeriesGeneral.clrpkr_SeriesBorderColor.Text = this.chartSeriesPropertiesCollection[seriesCount].BorderColor;
                controlProperties.SeriesGeneral.cmb_ChartType.TextValue = this.chartSeriesPropertiesCollection[seriesCount].ChartType;
                controlProperties.SeriesGeneral.clrpkr_MarkerColor.Text = this.chartSeriesPropertiesCollection[seriesCount].AdornmentColor;
                controlProperties.SeriesGeneral.cmb_MarkerType.Text = this.chartSeriesPropertiesCollection[seriesCount].AdornmentType;
                controlProperties.SeriesGeneral.updwn_MarkerSize.TextValue = this.chartSeriesPropertiesCollection[seriesCount].Size;
                if (this.chartSeriesPropertiesCollection[seriesCount].ShowDataLabels.ToLower() == "true")
                {
                    controlProperties.SeriesGeneral.chk_ShowDataLabels.IsChecked = true;
                    controlProperties.SeriesGeneral.cmb_DatalabelPosition.IsEnabled = true;
                }
                else
                {
                    controlProperties.SeriesGeneral.chk_ShowDataLabels.IsChecked = false;
                    controlProperties.SeriesGeneral.cmb_DatalabelPosition.IsEnabled = false;
                }
                controlProperties.SeriesGeneral.cmb_DatalabelPosition.Text = this.chartSeriesPropertiesCollection[seriesCount].DataLabelsPosition;
                if (this.chartSeriesPropertiesCollection[seriesCount].CategoryAxisName == "Primary")
                {
                    controlProperties.SeriesAxes.rb_PrimaryXAxis.IsChecked = true;
                }
                else
                {
                    controlProperties.SeriesAxes.rb_SecondaryXAxis.IsChecked = true;
                }
                if (this.chartSeriesPropertiesCollection[seriesCount].ValueAxisName == "Primary")
                {
                    controlProperties.SeriesAxes.rb_PrimaryYAxis.IsChecked = true;
                }
                else
                {
                    controlProperties.SeriesAxes.rb_SecondaryYAxis.IsChecked = true;
                }
                controlProperties.SeriesGeneral.clrpkr_SeriesColor.Text = this.chartSeriesPropertiesCollection[seriesCount].SeriesColor;
            }
            catch (Exception)
            {
                controlProperties.SeriesGeneral.clrpkr_SeriesColor.Text = "AliceBlue";
            }

            return controlProperties;
        }

        private void LegendProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdateLegendProperties();
            controlProperties.ShowDialog();
            this.chartLegendProperties.IsInternalPropertyChange = true;
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange =change;
            this.Panel.EditingManager.AddAction(action);


            foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
            {
                if (uiElement is LegendGeneral)
                {
                    switch (controlProperties.LegendGeneral.LegendPosition.Text.ToLower())
                    {
                        case "rightcenter":
                            {
                                this.chartLegendProperties.Position = "RightCenter";
                                break;
                            }

                        case "leftcenter":
                            {
                                this.chartLegendProperties.Position = "LeftCenter";
                                break;
                            }
                        case "bottomcenter":
                            {
                                this.chartLegendProperties.Position = "BottomCenter";
                                break;
                            }
                        default:
                            {
                                this.chartLegendProperties.Position = "TopCenter";
                                break;
                            }
                    }

                    this.chartLegendProperties.Name = controlProperties.LegendGeneral.LegendName.Text;
                    this.chartLegendProperties.BackFill = controlProperties.LegendGeneral.LegendBackFill.Text;
                    this.chartLegendProperties.Hidden = controlProperties.LegendGeneral.ShowLegend.TextValue;
                    this.chartLegendProperties.Layout = controlProperties.LegendGeneral.LegendLayout.Text;
                    switch (controlProperties.LegendGeneral.LegendLayout.Text.ToLower())
                    {
                        case "row":
                            {
                                int i = 0;
                                foreach (ChartSeries chartSeries in InnerChart.Areas[0].Series)
                                {
                                    i++;
                                }
                                ChartLegand.ColumnsCount = i;
                                ChartLegand.RowsCount = 1;
                                this.columnsCount = i.ToString();
                                this.rowsCount = "1";
                                break;
                            }
                        case "column":
                            {
                                int i = 0;
                                foreach (ChartSeries chartSeries in InnerChart.Areas[0].Series)
                                {
                                    i++;
                                }
                                ChartLegand.ColumnsCount = 1;
                                ChartLegand.RowsCount = i;
                                this.RowsCount = i.ToString();
                                this.ColumnsCount = "1";
                                break;
                            }
                        default:
                            {
                                int i = 0;
                                foreach (ChartSeries chartSeries in InnerChart.Areas[0].Series)
                                {
                                    i++;
                                }
                                ChartLegand.ColumnsCount = i;
                                ChartLegand.RowsCount = 1;
                                this.ColumnsCount = i.ToString();
                                this.RowsCount = "1";
                                break;
                            }
                    }

                }

                if (uiElement is LegendFont)
                {
                    this.chartLegendProperties.FontFamily = controlProperties.LegendFont.cmb_LegendFont.TextValue;
                    this.chartLegendProperties.FontSize = controlProperties.LegendFont.cmb_LegendFontSize.TextValue;
                    this.chartLegendProperties.FontColor = controlProperties.LegendFont.clrpkr_LegendFontColor.Text;
                    this.chartLegendProperties.FontStyle = controlProperties.LegendFont.cmb_FontStyle.TextValue;
                }

                if (uiElement is LegendBorder)
                {
                    this.chartLegendProperties.BorderThickness = controlProperties.LegendBorder.LegendBorderWidth.TextValue;
                    this.chartLegendProperties.LegendBorderColor = controlProperties.LegendBorder.LegendBorderColor.Text;
                }
            }
            change.NewValue = this.GetReportItem();
            this.chartLegendProperties.IsInternalPropertyChange = false;
        }

        private ControlProperties UpdateLegendProperties()
        {
            ControlProperties controlProperties = new ControlProperties(this, this.Report, ChartChild.Legend);
            this.Panel.UpdateOwnerWindow(controlProperties);

            controlProperties.LegendGeneral.LegendPosition.Text = this.chartLegendProperties.Position;

            controlProperties.LegendGeneral.LegendBackFill.Text = this.chartLegendProperties.BackFill;
            controlProperties.LegendGeneral.LegendLayout.Text = this.chartLegendProperties.Layout;
            controlProperties.LegendGeneral.ShowLegend.TextValue = this.chartLegendProperties.Hidden;
            controlProperties.LegendFont.LegendFontColor.Text = this.chartLegendProperties.FontColor;
            controlProperties.LegendFont.cmb_LegendFont.TextValue = this.chartLegendProperties.FontFamily;
            controlProperties.LegendFont.cmb_LegendFontSize.TextValue = this.chartLegendProperties.FontSize;
            controlProperties.LegendFont.cmb_FontStyle.TextValue = this.chartLegendProperties.FontStyle;
            controlProperties.LegendBorder.LegendBorderColor.Text = this.chartLegendProperties.LegendBorderColor;
            controlProperties.LegendBorder.LegendBorderWidth.TextValue = this.chartLegendProperties.BorderThickness;

            switch (this.chartLegendProperties.Layout.ToLower())
            {
                case "row":
                    {
                        int i = 0;
                        foreach (ChartSeries chartSeries in InnerChart.Areas[0].Series)
                        {
                            i++;
                        }
                        ChartLegand.ColumnsCount = i;
                        ChartLegand.RowsCount = 1;
                        this.columnsCount = i.ToString();
                        this.rowsCount = "1";
                        break;
                    }
                case "column":
                    {
                        int i = 0;
                        foreach (ChartSeries chartSeries in InnerChart.Areas[0].Series)
                        {
                            i++;
                        }
                        ChartLegand.ColumnsCount = 1;
                        ChartLegand.RowsCount = i;
                        this.RowsCount = i.ToString();
                        this.ColumnsCount = "1";
                        break;
                    }
                default:
                    {
                        int i = 0;
                        foreach (ChartSeries chartSeries in InnerChart.Areas[0].Series)
                        {
                            i++;
                        }
                        ChartLegand.ColumnsCount = i;
                        ChartLegand.RowsCount = 1;
                        this.ColumnsCount = i.ToString();
                        this.RowsCount = "1";
                        break;
                    }
            }

            return controlProperties;
        }

        private void CategoryAxisProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdateControlAxisProperties();
            controlProperties.ShowDialog();
            this.chartCategoryAxisProperties.IsInternalPropertyChange = true;
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange =change;
            this.Panel.EditingManager.AddAction(action);

            foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
            {
                if (uiElement is AxisCategoryGeneral)
                {
                    this.chartCategoryAxisProperties.ReverseDirection = controlProperties.AxisCategoryGeneral.CategoryReverse.TextValue;
                    this.chartCategoryAxisProperties.LineColor = controlProperties.AxisCategoryGeneral.CategoryLineColor.Text;
                    this.chartCategoryAxisProperties.LineWidth = controlProperties.AxisCategoryGeneral.CategoryLineWidth.TextValue;
                    this.chartCategoryAxisProperties.LineStyle = controlProperties.AxisCategoryGeneral.CategoryLineStyle.TextValue;
                }
                else if (uiElement is AxisCategoryLabel)
                {
                    this.chartCategoryAxisProperties.FontFamily = controlProperties.AxisCategoryLabel.CategoryLabelFontFamily.TextValue;
                    this.chartCategoryAxisProperties.FontSize = controlProperties.AxisCategoryLabel.CategoryLabelFontSize.TextValue;
                    this.chartCategoryAxisProperties.FontColor = controlProperties.AxisCategoryLabel.CategoryLabelFontColor.Text;
                    this.chartCategoryAxisProperties.FontWeight = controlProperties.AxisCategoryLabel.CategoryLabelFontWeight.TextValue;
                    this.chartCategoryAxisProperties.FontAngle = controlProperties.AxisCategoryLabel.CategoryLabelFontAngle.Value.ToString();
                    this.chartCategoryAxisProperties.HideAxisLabels = controlProperties.AxisCategoryLabel.HideCategoryAxisLabels.TextValue;
                }
                else if (uiElement is AxisCategoryTickMarks)
                {
                    this.chartCategoryAxisProperties.EnableMajorTickMarks = controlProperties.AxisCategoryTickMarks.HideCategoryMajorTick.TextValue;
                    this.chartCategoryAxisProperties.EnableMinorTickMarks = controlProperties.AxisCategoryTickMarks.HideCategoryMinorTick.TextValue;
                    this.chartCategoryAxisProperties.TickLength = controlProperties.AxisCategoryTickMarks.CategoryMajorTickLength.TextValue;
                    this.chartCategoryAxisProperties.TickWidth = controlProperties.AxisCategoryTickMarks.CategoryMajorTickWidth.TextValue;
                    this.chartCategoryAxisProperties.TickColor = controlProperties.AxisCategoryTickMarks.CategoryMajorTickColor.Text;
                    this.chartCategoryAxisProperties.TickStyle = controlProperties.AxisCategoryTickMarks.CategoryMajorTickStyle.TextValue;
                }
            }
            change.NewValue = this.GetReportItem();
            this.chartCategoryAxisProperties.IsInternalPropertyChange = false;
        }

        private ControlProperties UpdateControlAxisProperties()
        {
            ControlProperties controlProperties = new ControlProperties(this, this.Report, ChartChild.CategoryAxis);
            this.Panel.UpdateOwnerWindow(controlProperties);

            controlProperties.AxisCategoryGeneral.CategoryReverse.TextValue = this.chartCategoryAxisProperties.ReverseDirection;
            controlProperties.AxisCategoryGeneral.CategoryLineColor.Text = this.chartCategoryAxisProperties.LineColor;
            controlProperties.AxisCategoryGeneral.CategoryLineWidth.TextValue = this.chartCategoryAxisProperties.LineWidth;
            controlProperties.AxisCategoryGeneral.CategoryLineStyle.TextValue = this.chartCategoryAxisProperties.LineStyle;
            controlProperties.AxisCategoryLabel.CategoryLabelFontFamily.TextValue = this.chartCategoryAxisProperties.FontFamily;
            controlProperties.AxisCategoryLabel.CategoryLabelFontSize.TextValue = this.chartCategoryAxisProperties.FontSize;
            controlProperties.AxisCategoryLabel.CategoryLabelFontColor.Text = this.chartCategoryAxisProperties.FontColor;
            controlProperties.AxisCategoryLabel.CategoryLabelFontWeight.TextValue = this.chartCategoryAxisProperties.FontWeight;
            controlProperties.AxisCategoryLabel.CategoryLabelFontAngle.Value = Convert.ToDouble(this.chartCategoryAxisProperties.FontAngle);
            controlProperties.AxisCategoryLabel.HideCategoryAxisLabels.TextValue = this.chartCategoryAxisProperties.HideAxisLabels;
            controlProperties.AxisCategoryTickMarks.HideCategoryMajorTick.TextValue = this.chartCategoryAxisProperties.EnableMajorTickMarks;
            controlProperties.AxisCategoryTickMarks.HideCategoryMinorTick.TextValue = this.chartCategoryAxisProperties.EnableMinorTickMarks;
            controlProperties.AxisCategoryTickMarks.CategoryMajorTickLength.TextValue = this.chartCategoryAxisProperties.TickLength;
            controlProperties.AxisCategoryTickMarks.CategoryMajorTickWidth.TextValue = this.chartCategoryAxisProperties.TickWidth;
            controlProperties.AxisCategoryTickMarks.CategoryMajorTickColor.Text = this.chartCategoryAxisProperties.TickColor;
            controlProperties.AxisCategoryTickMarks.CategoryMajorTickStyle.TextValue = this.chartCategoryAxisProperties.TickStyle;

            return controlProperties;

        }

        private void ValueAxisProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdateValuesAxisProperties();
            controlProperties.ShowDialog();
            this.chartValueAxisProperties.IsInternalPropertyChange = true;
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange =change;
            this.Panel.EditingManager.AddAction(action);

            foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
            {
                if (uiElement is AxisValueGeneral)
                {
                    this.chartValueAxisProperties.ReverseDirection = controlProperties.AxisValueGeneral.ValueReverse.TextValue;
                    this.chartValueAxisProperties.LineColor = controlProperties.AxisValueGeneral.ValueLineColor.Text;
                    this.chartValueAxisProperties.LineWidth = controlProperties.AxisValueGeneral.ValueLineWidth.TextValue;
                    this.chartValueAxisProperties.LineStyle = controlProperties.AxisValueGeneral.ValueLineStyle.TextValue;
                }
                else if (uiElement is AxisValueLabel)
                {
                    this.chartValueAxisProperties.FontFamily = controlProperties.AxisValueLabel.ValueLabelFontFamily.TextValue;
                    this.chartValueAxisProperties.FontSize = controlProperties.AxisValueLabel.ValueLabelFontSize.TextValue;
                    this.chartValueAxisProperties.FontColor = controlProperties.AxisValueLabel.ValueLabelFontColor.Text;
                    this.chartValueAxisProperties.FontWeight = controlProperties.AxisValueLabel.ValueLabelFontWeight.TextValue;
                    this.chartValueAxisProperties.FontAngle = controlProperties.AxisValueLabel.ValueLabelFontAngle.Value.ToString();
                    this.chartValueAxisProperties.HideAxisLabels = controlProperties.AxisValueLabel.HideValueAxisLabels.TextValue;
                }
                else if (uiElement is AxisValueTickMarks)
                {
                    this.chartValueAxisProperties.EnableMajorTickMarks = controlProperties.AxisValueTickMarks.HideValueMajorTick.TextValue;
                    this.chartValueAxisProperties.EnableMinorTickMarks = controlProperties.AxisValueTickMarks.HideValueMinorTick.TextValue;
                    this.chartValueAxisProperties.TickLength = controlProperties.AxisValueTickMarks.ValueMajorTickLength.TextValue;
                    this.chartValueAxisProperties.TickWidth = controlProperties.AxisValueTickMarks.ValueMajorTickWidth.TextValue;
                    this.chartValueAxisProperties.TickColor = controlProperties.AxisValueTickMarks.ValueMajorTickColor.Text;
                    this.chartValueAxisProperties.TickStyle = controlProperties.AxisValueTickMarks.ValueMajorTickStyle.TextValue;
                }
            }
            change.NewValue = this.GetReportItem();
            this.chartValueAxisProperties.IsInternalPropertyChange = false;
        }

        private ControlProperties UpdateValuesAxisProperties()
        {
            ControlProperties controlProperties = new ControlProperties(this, this.Report, ChartChild.ValueAxis);
            this.Panel.UpdateOwnerWindow(controlProperties);

            controlProperties.AxisValueGeneral.ValueReverse.TextValue = this.chartValueAxisProperties.ReverseDirection;
            controlProperties.AxisValueGeneral.ValueLineColor.Text = this.chartValueAxisProperties.LineColor;
            controlProperties.AxisValueGeneral.ValueLineWidth.TextValue = this.chartValueAxisProperties.LineWidth;
            controlProperties.AxisValueGeneral.ValueLineStyle.TextValue = this.chartValueAxisProperties.LineStyle;
            controlProperties.AxisValueLabel.ValueLabelFontFamily.TextValue = this.chartValueAxisProperties.FontFamily;
            controlProperties.AxisValueLabel.ValueLabelFontSize.TextValue = this.chartValueAxisProperties.FontSize;
            controlProperties.AxisValueLabel.ValueLabelFontColor.Text = this.chartValueAxisProperties.FontColor;
            controlProperties.AxisValueLabel.ValueLabelFontWeight.TextValue = this.chartValueAxisProperties.FontWeight;
            controlProperties.AxisValueLabel.ValueLabelFontAngle.Value = Convert.ToDouble(this.chartValueAxisProperties.FontAngle);
            controlProperties.AxisValueLabel.HideValueAxisLabels.TextValue = this.chartValueAxisProperties.HideAxisLabels;
            controlProperties.AxisValueTickMarks.HideValueMajorTick.TextValue = this.chartValueAxisProperties.EnableMajorTickMarks;
            controlProperties.AxisValueTickMarks.HideValueMinorTick.TextValue = this.chartValueAxisProperties.EnableMinorTickMarks;
            controlProperties.AxisValueTickMarks.ValueMajorTickLength.TextValue = this.chartValueAxisProperties.TickLength;
            controlProperties.AxisValueTickMarks.ValueMajorTickWidth.TextValue = this.chartValueAxisProperties.TickWidth;
            controlProperties.AxisValueTickMarks.ValueMajorTickColor.Text = this.chartValueAxisProperties.TickColor;
            controlProperties.AxisValueTickMarks.ValueMajorTickStyle.TextValue = this.chartValueAxisProperties.TickStyle;

            return controlProperties;
        }

        void ChartTitleProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdateChartTitle();
            controlProperties.ShowDialog();
            this.titleProperties.IsInternalPropertyChange = true;
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange =change;
            this.Panel.EditingManager.AddAction(action);

            foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
            {
                if (uiElement is ChartTitle)
                {
                    this.titleProperties.Name = controlProperties.ChartTitle.TitleText.Text;
                    this.titleProperties.TitleFontFamily = controlProperties.ChartTitle.TitleFontFamily.TextValue;
                    this.titleProperties.TitleFontSize = controlProperties.ChartTitle.TitleFontSize.TextValue;
                    this.titleProperties.TitleBackFill = controlProperties.ChartTitle.TitleBackground.Text;
                    this.titleProperties.TitleFontColor = controlProperties.ChartTitle.TitleFontColor.Text;
                    this.titleProperties.TitleFontStyle = controlProperties.ChartTitle.FontStyle.TextValue;
                }
            }
            change.NewValue = this.GetReportItem();
            this.titleProperties.IsInternalPropertyChange = false;
        }

        private ControlProperties UpdateChartTitle()
        {
            ControlProperties controlProperties = new ControlProperties(this, this.Report, ChartChild.ChartTitle);
            this.Panel.UpdateOwnerWindow(controlProperties);
            controlProperties.ChartTitle.FontStyle.TextValue = this.titleProperties.TitleFontStyle;
            controlProperties.ChartTitle.TitleText.Text = this.titleProperties.Name;
            controlProperties.ChartTitle.TitleFontFamily.TextValue = this.titleProperties.TitleFontFamily;
            controlProperties.ChartTitle.TitleFontSize.TextValue = this.titleProperties.TitleFontSize;
            controlProperties.ChartTitle.TitleFontColor.Text = this.titleProperties.TitleFontColor;
            controlProperties.ChartTitle.TitleBackground.Text = this.titleProperties.TitleBackFill;
            return controlProperties;
        }

        void ValueTitleProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdateValueTitle();
            controlProperties.ShowDialog();
            this.valueAxisTitleProperties.IsInternalPropertyChange = true;
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange =change;
            this.Panel.EditingManager.AddAction(action);

            foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
            {
                if (uiElement is ValueAxisTitle)
                {
                    this.valueAxisTitleProperties.Name = controlProperties.ValueAxisTitle.ValueTitleText.Text;
                    this.valueAxisTitleProperties.FontFamily = controlProperties.ValueAxisTitle.ValueTitleFontFamily.TextValue;
                    this.valueAxisTitleProperties.FontSize = controlProperties.ValueAxisTitle.ValueTitleFontSize.TextValue;
                    this.valueAxisTitleProperties.TitleAlignment = controlProperties.ValueAxisTitle.ValueTitleAlignment.TextValue; ;
                    this.valueAxisTitleProperties.FontColor = controlProperties.ValueAxisTitle.ValueTitleFontColor.Text;
                    this.valueAxisTitleProperties.FontStyle = controlProperties.ValueAxisTitle.ValueTitleFontStyle.TextValue;
                }
            }
            change.NewValue = this.GetReportItem();
            this.valueAxisTitleProperties.IsInternalPropertyChange = false;
        }

        private ControlProperties UpdateValueTitle()
        {
            ControlProperties controlProperties = new ControlProperties(this, this.Report, ChartChild.ValueTitle);
            this.Panel.UpdateOwnerWindow(controlProperties);
            controlProperties.ValueAxisTitle.ValueTitleFontFamily.TextValue = this.valueAxisTitleProperties.FontFamily;
            controlProperties.ValueAxisTitle.ValueTitleFontSize.TextValue = this.valueAxisTitleProperties.FontSize;
            controlProperties.ValueAxisTitle.ValueTitleFontColor.Text = this.valueAxisTitleProperties.FontColor;
            controlProperties.ValueAxisTitle.ValueTitleAlignment.TextValue = this.valueAxisTitleProperties.TitleAlignment;
            controlProperties.ValueAxisTitle.ValueTitleFontStyle.TextValue = this.valueAxisTitleProperties.FontStyle;
            controlProperties.ValueAxisTitle.ValueTitleText.Text = this.valueAxisTitleProperties.Name;

            return controlProperties;
        }  

        void CategoryTitleProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdateCatagoryTitle();
            controlProperties.ShowDialog();
            this.categoryAxisTitleProperties.IsInternalPropertyChange = true;
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange =change;
            this.Panel.EditingManager.AddAction(action);

            foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
            {
                if (uiElement is CategoryAxisTitle)
                {
                    this.categoryAxisTitleProperties.Name = controlProperties.CategoryAxisTitle.CategoryTitleText.Text;
                    this.categoryAxisTitleProperties.FontFamily = controlProperties.CategoryAxisTitle.CategoryTitleFontFamily.TextValue;
                    this.categoryAxisTitleProperties.FontSize = controlProperties.CategoryAxisTitle.CategoryTitleFontSize.TextValue;
                    this.categoryAxisTitleProperties.TitleAlignment = controlProperties.CategoryAxisTitle.CategoryTitleAllignment.TextValue; ;
                    this.categoryAxisTitleProperties.FontColor = controlProperties.CategoryAxisTitle.CategoryTitleFontColor.Text;
                    this.categoryAxisTitleProperties.FontStyle = controlProperties.CategoryAxisTitle.CategoryTitleStyle.TextValue;
                }
            }
            change.NewValue = this.GetReportItem();
            this.categoryAxisTitleProperties.IsInternalPropertyChange = false;
        }

        private ControlProperties UpdateCatagoryTitle()
        {
            ControlProperties controlProperties = new ControlProperties(this, this.Report, ChartChild.CategoryTitle);
            this.Panel.UpdateOwnerWindow(controlProperties);
            controlProperties.CategoryAxisTitle.CategoryTitleText.Text = this.categoryAxisTitleProperties.Name;
            controlProperties.CategoryAxisTitle.CategoryTitleStyle.TextValue = this.categoryAxisTitleProperties.FontStyle;
            controlProperties.CategoryAxisTitle.CategoryTitleFontFamily.TextValue = this.categoryAxisTitleProperties.FontFamily;
            controlProperties.CategoryAxisTitle.CategoryTitleFontSize.TextValue = this.categoryAxisTitleProperties.FontSize;
            controlProperties.CategoryAxisTitle.CategoryTitleFontColor.Text = this.categoryAxisTitleProperties.FontColor;
            controlProperties.CategoryAxisTitle.CategoryTitleAllignment.TextValue = this.categoryAxisTitleProperties.TitleAlignment;

            return controlProperties; 
        }

        void SecondaryCategoryTitleProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdateSecondaryCatagoryTitle();
            controlProperties.ShowDialog();
            this.secondaryXAxisTitleProperties.IsInternalPropertyChange = true;
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange =change;
            this.Panel.EditingManager.AddAction(action);

            foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
            {
                if (uiElement is SecondaryCategoryAxisTitle)
                {
                    this.secondaryXAxisTitleProperties.Name = controlProperties.SecondaryCategoryAxisTitle.CategoryTitleText.Text;
                    this.secondaryXAxisTitleProperties.FontFamily = controlProperties.SecondaryCategoryAxisTitle.CategoryTitleFontFamily.TextValue;
                    this.secondaryXAxisTitleProperties.FontSize = controlProperties.SecondaryCategoryAxisTitle.CategoryTitleFontSize.TextValue;
                    this.secondaryXAxisTitleProperties.TitleAlignment = controlProperties.SecondaryCategoryAxisTitle.CategoryTitleAllignment.TextValue; ;
                    this.secondaryXAxisTitleProperties.FontColor = controlProperties.SecondaryCategoryAxisTitle.CategoryTitleFontColor.Text;
                    this.secondaryXAxisTitleProperties.FontStyle = controlProperties.SecondaryCategoryAxisTitle.CategoryTitleStyle.TextValue;
                }
            }
            change.NewValue = this.GetReportItem();
            this.secondaryXAxisTitleProperties.IsInternalPropertyChange = false;
        }

        private ControlProperties UpdateSecondaryCatagoryTitle()
        {
            ControlProperties controlProperties = new ControlProperties(this, this.Report, ChartChild.SecondaryCategoryTitle);
            this.Panel.UpdateOwnerWindow(controlProperties);
            controlProperties.SecondaryCategoryAxisTitle.CategoryTitleText.Text = this.secondaryXAxisTitleProperties.Name;
            controlProperties.SecondaryCategoryAxisTitle.CategoryTitleStyle.TextValue = this.secondaryXAxisTitleProperties.FontStyle;
            controlProperties.SecondaryCategoryAxisTitle.CategoryTitleFontFamily.TextValue = this.secondaryXAxisTitleProperties.FontFamily;
            controlProperties.SecondaryCategoryAxisTitle.CategoryTitleFontSize.TextValue = this.secondaryXAxisTitleProperties.FontSize;
            controlProperties.SecondaryCategoryAxisTitle.CategoryTitleFontColor.Text = this.secondaryXAxisTitleProperties.FontColor;
            controlProperties.SecondaryCategoryAxisTitle.CategoryTitleAllignment.TextValue = this.secondaryXAxisTitleProperties.TitleAlignment;

            return controlProperties;
        }

        void SecondaryValueTitleProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdateSecondaryValueTitle();
            controlProperties.ShowDialog();
            this.secondaryYAxisTitleProperties.IsInternalPropertyChange = true;
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange =change;
            this.Panel.EditingManager.AddAction(action);

            foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
            {
                if (uiElement is SecondaryValueAxisTitle)
                {
                    this.secondaryYAxisTitleProperties.Name = controlProperties.SecondaryValueAxisTitle.ValueTitleText.Text;
                    this.secondaryYAxisTitleProperties.FontFamily = controlProperties.SecondaryValueAxisTitle.ValueTitleFontFamily.TextValue;
                    this.secondaryYAxisTitleProperties.FontSize = controlProperties.SecondaryValueAxisTitle.ValueTitleFontSize.TextValue;
                    this.secondaryYAxisTitleProperties.TitleAlignment = controlProperties.SecondaryValueAxisTitle.ValueTitleAlignment.TextValue; ;
                    this.secondaryYAxisTitleProperties.FontColor = controlProperties.SecondaryValueAxisTitle.ValueTitleFontColor.Text;
                    this.secondaryYAxisTitleProperties.FontStyle = controlProperties.SecondaryValueAxisTitle.ValueTitleFontStyle.TextValue;
                }
            }
            change.NewValue = this.GetReportItem();
            this.secondaryYAxisTitleProperties.IsInternalPropertyChange = false;
        }

        private ControlProperties UpdateSecondaryValueTitle()
        {
            ControlProperties controlProperties = new ControlProperties(this, this.Report, ChartChild.SecondaryValueTitle);
            this.Panel.UpdateOwnerWindow(controlProperties);
            controlProperties.SecondaryValueAxisTitle.ValueTitleFontFamily.TextValue = this.secondaryYAxisTitleProperties.FontFamily;
            controlProperties.SecondaryValueAxisTitle.ValueTitleFontSize.TextValue = this.secondaryYAxisTitleProperties.FontSize;
            controlProperties.SecondaryValueAxisTitle.ValueTitleFontColor.Text = this.secondaryYAxisTitleProperties.FontColor;
            controlProperties.SecondaryValueAxisTitle.ValueTitleAlignment.TextValue = this.secondaryYAxisTitleProperties.TitleAlignment;
            controlProperties.SecondaryValueAxisTitle.ValueTitleFontStyle.TextValue = this.secondaryYAxisTitleProperties.FontStyle;
            controlProperties.SecondaryValueAxisTitle.ValueTitleText.Text = this.secondaryYAxisTitleProperties.Name;

            return controlProperties;
        }  

        #endregion

        # region Common Helper Methods

        private void InitializeChart()
        {
            this.GetSeriesColorCollection();
            AddChartArea();
            AddChartTitle();
            AddPrimaryAxisTitle();
            AddSecondaryAxisTitle();
            AddLegendToChart();
        }

        private void AddChartArea()
        {
            ChartArea = new ChartArea();
            ChartArea.BorderBrush = Brushes.Transparent;
            ChartArea.PrimaryAxis.LabelFontSize = 10;
            Pen pen1 = new Pen(Brushes.Black, 1);
            pen1.DashStyle = DashStyles.Solid;
            ChartArea.PrimaryAxis.LineStroke = pen1;            

            ChartArea.SetShowGridLines(ChartArea.PrimaryAxis, false);

            Pen pen2 = new Pen(Brushes.Transparent, 1);
            pen2.DashStyle = DashStyles.Solid;
            ChartArea.SetGridLineStroke(this.ChartArea.PrimaryAxis, pen2);

            Pen pen3 = new Pen(Brushes.Black, 2);
            pen3.DashStyle = DashStyles.Solid;

            ChartArea.SecondaryAxis.LineStroke = pen1;
            ChartArea.SecondaryAxis.LabelFontSize = 10;

            Pen pen4 = new Pen(Brushes.Silver, 1);
            pen4.DashStyle = DashStyles.Solid;
            ChartArea.SetGridLineStroke(this.ChartArea.SecondaryAxis, pen4);

            Pen pen5 = new Pen(Brushes.Transparent, 1);
            pen4.DashStyle = DashStyles.Solid;
            ChartArea.SetSmallGridLineStroke(this.ChartArea.SecondaryAxis, pen5);

            ChartArea.SecondaryAxis.TickLineStroke = pen3;
            ChartArea.SecondaryAxis.TickSize = 2;
            ChartArea.SecondaryAxis.SmallTickSize = 0;
            ChartArea.SecondaryAxis.SmallTicksPerInterval = 4;

            ChartArea.PrimaryAxis.TickLineStroke = pen3;
            ChartArea.PrimaryAxis.TickSize = 2;
            ChartArea.PrimaryAxis.SmallTickSize = 0;
            ChartArea.PrimaryAxis.SmallTicksPerInterval = 4;

            if (this.ChartControlType != Controls.ChartControlType.Chart)
            {
                ChartTitle.Visibility = System.Windows.Visibility.Collapsed;
                ChartLegand.Visibility = System.Windows.Visibility.Collapsed;
                ChartArea.SetShowGridLines(ChartArea.SecondaryAxis, false);
                ChartArea.SetShowGridLines(ChartArea.PrimaryAxis, false);
                ChartArea.SecondaryAxis.AxisVisibility = System.Windows.Visibility.Collapsed;
                ChartArea.PrimaryAxis.AxisVisibility = System.Windows.Visibility.Collapsed;

                AddTitle.Visibility = System.Windows.Visibility.Collapsed;
                ShowValueTitle.Visibility = System.Windows.Visibility.Collapsed;
                LegendProperties.Visibility = System.Windows.Visibility.Collapsed;
                ShowCategoryTitle.Visibility = System.Windows.Visibility.Collapsed;
                ValueAxisProperties.Visibility = System.Windows.Visibility.Collapsed;
                ChartTitleProperties.Visibility = System.Windows.Visibility.Collapsed;
                ValueTitleProperties.Visibility = System.Windows.Visibility.Collapsed;
                CategoryAxisProperties.Visibility = System.Windows.Visibility.Collapsed;
                CategoryTitleProperties.Visibility = System.Windows.Visibility.Collapsed;

                ConvertToFullChart.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ConvertToFullChart.Visibility = System.Windows.Visibility.Collapsed;
            }

            InnerChart.Areas.Add(ChartArea);

            ChartArea.View3DMode = false;
            ChartArea.SideBySideSeriesPlacement = true;
            ChartArea.Background = Brushes.White;

            this.ChartSeries = new ChartSeries();
            if (this.ChartControlType == Controls.ChartControlType.Chart)
            {
                for (int i = 1; i <= 2; i++)
                {
                    ChartSeries chartSeries = new ChartSeries();
                    chartSeries.Interior = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.seriesColorCollection[i - 1]));
                    chartSeries.DataSource = DefaultChartData1();
                    chartSeries.BindingPathX = "SeriesID";
                    chartSeries.BindingPathsY = new string[] { "SeriesValue1" };
                    chartSeries.Label = "Series" + i.ToString();
                    chartSeries.Name = "Series" + i.ToString();
                    InnerChart.Areas[0].Series.Add(chartSeries);
                }
            }
            else
            {
                ChartSeries chartSeries = new ChartSeries();
                chartSeries.Interior = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.seriesColorCollection[0]));
                chartSeries.DataSource = DefaultChartData1();
                chartSeries.BindingPathX = "SeriesID";
                chartSeries.BindingPathsY = new string[] { "SeriesValue1" };
                chartSeries.Label = "";
                chartSeries.Name = "EmptySeriesName";
                InnerChart.Areas[0].Series.Add(chartSeries);
            }

            ChartArea.PrimaryAxis.ContentPath = "SeriesName";
            ChartArea.PrimaryAxis.PositionPath = "SeriesId";
            ChartArea.PrimaryAxis.LabelsSource = DefaultChartData1();

            this.secondaryXAxis = new ChartAxis();
            this.secondaryXAxis.ContentPath = this.InnerChart.Areas[0].PrimaryAxis.ContentPath;
            this.secondaryXAxis.PositionPath = this.InnerChart.Areas[0].PrimaryAxis.PositionPath;
            this.secondaryXAxis.LabelsSource = this.InnerChart.Areas[0].PrimaryAxis.LabelsSource;
            this.secondaryXAxis.AxisVisibility = Visibility.Collapsed;
            this.secondaryXAxis.Orientation = Orientation.Horizontal;
            this.secondaryXAxis.OpposedPosition = true;
            this.AddSecondaryXAxisTitle();

            this.secondaryYAxis = new ChartAxis();
            this.secondaryYAxis.OpposedPosition = true;
            this.secondaryYAxis.Orientation = Orientation.Vertical;
            this.AddSecondaryYAxisTitle();

            this.secondaryXAxis.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(SecondaryXAxis_PreviewMouseLeftButtonDown);
            this.secondaryYAxis.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(SecondaryYAxis_PreviewMouseLeftButtonDown);

            ChartArea.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ChartArea_PreviewMouseLeftButtonDown);
            ChartArea.PrimaryAxis.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PrimaryAxis_PreviewMouseLeftButtonDown);
            ChartArea.SecondaryAxis.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(SecondaryAxis_PreviewMouseLeftButtonDown);
            ChartArea.PrimaryAxis.PreviewMouseRightButtonDown += new MouseButtonEventHandler(PrimaryAxis_PreviewMouseRightButtonDown);
            ChartArea.SecondaryAxis.PreviewMouseRightButtonDown += new MouseButtonEventHandler(SecondaryAxis_PreviewMouseRightButtonDown);
        }

        private void AddChartTitle()
        {
            ChartTitle.TextWrapping = TextWrapping.Wrap;
            ChartTitle.BorderThickness = new Thickness(0);
            ChartTitle.TextAlignment = TextAlignment.Center;
            ChartTitle.Margin = new Thickness(10);
            ChartTitle.Text = this.titleProperties.Name;
            ChartTitle.FontFamily = new FontFamily(this.titleProperties.TitleFontFamily);
            ChartTitle.FontSize = Convert.ToDouble(new RDL.DOM.Size(this.titleProperties.TitleFontSize).PixelValue);
            ChartTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.titleProperties.TitleFontColor));
            ChartTitle.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.titleProperties.TitleBackFill));

            switch (this.titleProperties.TitleFontStyle)
            {
                case "bold":
                    ChartTitle.FontStyle = FontStyles.Normal;
                    ChartTitle.FontWeight = FontWeights.Bold;
                    break;
                case "italic":
                    ChartTitle.FontStyle = FontStyles.Italic;
                    ChartTitle.FontWeight = FontWeights.Normal;
                    break;
                default:
                    ChartTitle.FontStyle = FontStyles.Normal;
                    ChartTitle.FontWeight = FontWeights.Normal;
                    break;
            }

            ChartTitle.BorderBrush = Brushes.Transparent;
            InnerChart.Header = this.ChartTitle;
        }

        private void AddLegendToChart()
        {
            LegendTextbox = new TextBox();
            LegendTextbox.Text = "Legend Title";
            LegendTextbox.FontSize = 12;
            LegendTextbox.Background = Brushes.Transparent;
            LegendTextbox.BorderBrush = Brushes.Transparent;
            LegendTextbox.Visibility = Visibility.Collapsed;
            ChartLegand.Header = this.LegendTextbox;
            ChartLegand.PreviewMouseDown += new MouseButtonEventHandler(LegendTextbox_PreviewMouseDown);
            InnerChart.Legends.Add(ChartLegand);
        }

        private void AddPrimaryAxisTitle()
        {
            CategoryAxisTitle = new TextBox();
            CategoryAxisTitle.TextAlignment = TextAlignment.Center;
            CategoryAxisTitle.Margin = new Thickness(10, 0, 10, 0);
            CategoryAxisTitle.BorderThickness = new Thickness(0);
            CategoryAxisTitle.Style = FindResource("TextBoxStyle") as Style;
            CategoryAxisTitle.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(CategoryAxisTitle_PreviewMouseLeftButtonDown);
            CategoryAxisTitle.PreviewMouseRightButtonDown += new MouseButtonEventHandler(CategoryAxisTitle_PreviewMouseRightButtonDown);
            CategoryAxisTitle.LostFocus += new RoutedEventHandler(CategoryAxisTitle_LostFocus);
            CategoryAxisTitle.ContextMenu = this.AddAxisTitleContextMenu(Editors.ChartObject.PrimaryAxisTitle);
            CategoryAxisTitle.Text = this.categoryAxisTitleProperties.Name;
            CategoryAxisTitle.FontSize = Convert.ToDouble(new RDL.DOM.Size(this.categoryAxisTitleProperties.FontSize).PixelValue);
            CategoryAxisTitle.FontFamily = new System.Windows.Media.FontFamily(this.categoryAxisTitleProperties.FontFamily);
            CategoryAxisTitle.TextAlignment = TextAlignment.Center;
            CategoryAxisTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.categoryAxisTitleProperties.FontColor));
            switch (this.categoryAxisTitleProperties.FontStyle.ToLower())
            {
                case "bold":
                    CategoryAxisTitle.FontWeight = FontWeights.Bold;
                    break;
                case "italic":
                    CategoryAxisTitle.FontWeight = FontWeights.Normal;
                    CategoryAxisTitle.FontStyle = FontStyles.Italic;
                    break;
                default:
                    CategoryAxisTitle.FontWeight = FontWeights.Normal;
                    CategoryAxisTitle.FontStyle = FontStyles.Normal;
                    break;
            }
            CategoryAxisTitle.BorderBrush = Brushes.Transparent;
            CategoryAxisTitle.Background = Brushes.Transparent;
            ChartArea.PrimaryAxis.Header = CategoryAxisTitle;
            ChartArea.PrimaryAxis.HeaderAlignment = ChartAlignment.Center;
        }

        private void AddSecondaryXAxisTitle()
        {
            SecondaryXAxisTitle = new TextBox();
            SecondaryXAxisTitle.TextAlignment = TextAlignment.Center;
            SecondaryXAxisTitle.Margin = new Thickness(10, 0, 10, 0);
            SecondaryXAxisTitle.BorderThickness = new Thickness(0);
            SecondaryXAxisTitle.Style = FindResource("TextBoxStyle") as Style;
            SecondaryXAxisTitle.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(SecondaryXAxisTitle_PreviewMouseLeftButtonDown);
            SecondaryXAxisTitle.PreviewMouseRightButtonDown += new MouseButtonEventHandler(SecondaryXAxisTitle_PreviewMouseRightButtonDown);
            SecondaryXAxisTitle.LostFocus += new RoutedEventHandler(CategoryAxisTitle_LostFocus);
            SecondaryXAxisTitle.ContextMenu = this.AddAxisTitleContextMenu(Editors.ChartObject.SecondaryXAxisTitle);
            SecondaryXAxisTitle.Text = this.secondaryXAxisTitleProperties.Name;
            SecondaryXAxisTitle.FontSize = Convert.ToDouble(new RDL.DOM.Size(this.secondaryXAxisTitleProperties.FontSize).PixelValue);
            SecondaryXAxisTitle.FontFamily = new System.Windows.Media.FontFamily(this.secondaryXAxisTitleProperties.FontFamily);
            SecondaryXAxisTitle.TextAlignment = TextAlignment.Center;
            SecondaryXAxisTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.secondaryXAxisTitleProperties.FontColor));
            switch (this.secondaryXAxisTitleProperties.FontStyle.ToLower())
            {
                case "bold":
                    SecondaryXAxisTitle.FontWeight = FontWeights.Bold;
                    break;
                case "italic":
                    SecondaryXAxisTitle.FontWeight = FontWeights.Normal;
                    SecondaryXAxisTitle.FontStyle = FontStyles.Italic;
                    break;
                default:
                    SecondaryXAxisTitle.FontWeight = FontWeights.Normal;
                    SecondaryXAxisTitle.FontStyle = FontStyles.Normal;
                    break;
            }
            SecondaryXAxisTitle.BorderBrush = Brushes.Transparent;
            SecondaryXAxisTitle.Background = Brushes.Transparent;
            this.secondaryXAxis.Header = SecondaryXAxisTitle;
            this.secondaryXAxis.HeaderAlignment = ChartAlignment.Center;
        }


        private void AddSecondaryAxisTitle()
        {
            ValueAxisTitle = new TextBox();
            ValueAxisTitle.Margin = new Thickness(15, 0, 5, 0);
            ValueAxisTitle.BorderThickness = new Thickness(0);
            ValueAxisTitle.ContextMenu = this.AddAxisTitleContextMenu(Editors.ChartObject.SecondaryAxisTile);
            ValueAxisTitle.Text = this.valueAxisTitleProperties.Name;
            ValueAxisTitle.FontSize = Convert.ToDouble(new RDL.DOM.Size(this.valueAxisTitleProperties.FontSize).PixelValue);
            ValueAxisTitle.FontFamily = new System.Windows.Media.FontFamily(this.valueAxisTitleProperties.FontFamily);
            ValueAxisTitle.TextAlignment = TextAlignment.Center;
            ValueAxisTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.valueAxisTitleProperties.FontColor));
            switch (this.valueAxisTitleProperties.FontStyle.ToLower())
            {
                case "bold":
                    ValueAxisTitle.FontWeight = FontWeights.Bold;
                    break;
                case "italic":
                    ValueAxisTitle.FontWeight = FontWeights.Normal;
                    ValueAxisTitle.FontStyle = FontStyles.Italic;
                    break;
                default:
                    ValueAxisTitle.FontWeight = FontWeights.Normal;
                    ValueAxisTitle.FontStyle = FontStyles.Normal;
                    break;
            }
            ValueAxisTitle.BorderBrush = Brushes.Transparent;
            ValueAxisTitle.Background = Brushes.Transparent;
            ChartArea.SecondaryAxis.Header = ValueAxisTitle;
            ChartArea.SecondaryAxis.HeaderAlignment = ChartAlignment.Center;
            this.ValueAxisTitle.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ValueAxisTitle_PreviewMouseLeftButtonDown);
            this.ValueAxisTitle.PreviewMouseRightButtonDown += new MouseButtonEventHandler(ValueAxisTitle_PreviewMouseRightButtonDown);
            this.ValueAxisTitle.LostFocus += new RoutedEventHandler(ValueAxisTitle_LostFocus);
        }

        private void AddSecondaryYAxisTitle()
        {
            SecondaryYAxisTitle = new TextBox();
            SecondaryYAxisTitle.Margin = new Thickness(15, 0, 5, 0);
            SecondaryYAxisTitle.BorderThickness = new Thickness(0);
            SecondaryYAxisTitle.ContextMenu = this.AddAxisTitleContextMenu(Editors.ChartObject.SecondaryYAxisTitle);
            SecondaryYAxisTitle.Text = this.secondaryYAxisTitleProperties.Name;
            SecondaryYAxisTitle.FontSize = Convert.ToDouble(new RDL.DOM.Size(this.secondaryYAxisTitleProperties.FontSize).PixelValue);
            SecondaryYAxisTitle.FontFamily = new System.Windows.Media.FontFamily(this.secondaryYAxisTitleProperties.FontFamily);
            SecondaryYAxisTitle.TextAlignment = TextAlignment.Center;
            SecondaryYAxisTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.secondaryYAxisTitleProperties.FontColor));
            switch (this.secondaryYAxisTitleProperties.FontStyle.ToLower())
            {
                case "bold":
                    SecondaryYAxisTitle.FontWeight = FontWeights.Bold;
                    break;
                case "italic":
                    SecondaryYAxisTitle.FontWeight = FontWeights.Normal;
                    SecondaryYAxisTitle.FontStyle = FontStyles.Italic;
                    break;
                default:
                    SecondaryYAxisTitle.FontWeight = FontWeights.Normal;
                    SecondaryYAxisTitle.FontStyle = FontStyles.Normal;
                    break;
            }
            SecondaryYAxisTitle.BorderBrush = Brushes.Transparent;
            SecondaryYAxisTitle.Background = Brushes.Transparent;
            this.secondaryYAxis.Header = SecondaryYAxisTitle;
            this.secondaryYAxis.HeaderAlignment = ChartAlignment.Center;
            this.SecondaryYAxisTitle.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(SecondaryYAxisTitle_PreviewMouseLeftButtonDown);
            this.SecondaryYAxisTitle.PreviewMouseRightButtonDown += new MouseButtonEventHandler(SecondaryYAxisTitle_PreviewMouseRightButtonDown);
            this.SecondaryYAxisTitle.LostFocus += new RoutedEventHandler(SecondaryYAxisTitle_LostFocus);
        }

        private void GetSeriesColorCollection()
        {
            this.seriesColorCollection.Add("#FF6495ED");
            this.seriesColorCollection.Add("Orange");
            this.seriesColorCollection.Add("Crimson");
            this.seriesColorCollection.Add("SteelBlue");
            this.seriesColorCollection.Add("Gray");
            this.seriesColorCollection.Add("DarkSlateBlue");
            this.seriesColorCollection.Add("Khaki");
            this.seriesColorCollection.Add("DarkCyan");
            this.seriesColorCollection.Add("Chocolate");
            this.seriesColorCollection.Add("Blue");
            this.seriesColorCollection.Add("Moccasin");
            this.seriesColorCollection.Add("SlateGray");
            this.seriesColorCollection.Add("DarkSalmon");
            this.seriesColorCollection.Add("Peru");
            this.seriesColorCollection.Add("LightSteelBlue");
        }

        private void UpdateChartSeries(string header)
        {

            if (this.ValuePanel.Children.Count == 0)
            {
                this.InnerChart.Areas[0].Series.Clear();
                this.chartSeriesPropertiesCollection.Clear();
            }
            ChartArea.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(ChartArea_PreviewMouseLeftButtonDown);
            ChartArea.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ChartGrid_PreviewMouseLeftButtonDown);
            ChartSeries chartSeries = new ChartSeries();
            chartSeries.DataSource = DefaultChartData1();
            chartSeries.BindingPathX = "SeriesID";
            chartSeries.BindingPathsY = new string[] { "SeriesValue1" };
            chartSeries.Label = header;
            chartSeries.MouseLeftButtonDown += new ChartMouseEventHandler(ChartSeries_MouseLeftButtonDown);
            chartSeries.MouseRightButtonDown += new ChartMouseEventHandler(chartSeries_MouseRightButtonDown);
            this.chartSeriesProperties = new Editors.ChartSeriesProperties();
            chartSeries.Name = chartSeries.Label;
            this.chartSeriesProperties.IsInternalPropertyChange = true;
            this.chartSeriesProperties.Name = chartSeries.Name;
            if (this.InnerChart.Areas[0].Series.Count > 0)
            {
                chartSeries.Type = this.InnerChart.Areas[0].Series[seriesCount].Type;
            }
            this.chartSeriesProperties.IsInternalPropertyChange = false;
            this.chartSeriesProperties.PropertyChanged += new PropertyChangedEventHandler(OnChartSeriesPropertyChanged);
            this.chartSeriesProperties.PropertyChanging += new PropertyChangingEventHandler(chartSeriesProperties_PropertyChanging);
            this.chartSeriesPropertiesCollection.Add(chartSeriesProperties);
            seriesCount = this.InnerChart.Areas[0].Series.Count;
            this.InnerChart.Areas[0].Series.Add(chartSeries);

            this.chartSeriesPropertiesCollection[seriesCount].IsInternalPropertyChange = true;
           
            if (this.chartSeriesPropertiesCollection.Count > 1)
            {
                int nameCount = 0;
                for (int i = 0; i < this.chartSeriesPropertiesCollection.Count - 1; i++)
                {
                    string name = this.chartSeriesPropertiesCollection[i].Name;
                    if (name.Equals(chartSeries.Label + nameCount.ToString()) || name.Equals(chartSeries.Label))
                    {
                        nameCount = nameCount + 1;
                    }
                }
                if (nameCount > 0)
                {
                    this.chartSeriesPropertiesCollection[seriesCount].Name = chartSeries.Label + nameCount.ToString();
                }
                else
                {
                    this.chartSeriesPropertiesCollection[seriesCount].Name = chartSeries.Label;
                }
            }
            else
            {
                this.chartSeriesPropertiesCollection[seriesCount].Name = chartSeries.Label;
            }

            this.chartSeriesPropertiesCollection[seriesCount].SeriesColor = "";
            this.chartSeriesPropertiesCollection[seriesCount].ChartType = this.InnerChart.Areas[0].Series[this.InnerChart.Areas[0].Series.Count - 1].ChartType.ToString();
            this.chartSeriesPropertiesCollection[seriesCount].IsInternalPropertyChange = false;

        }

        private System.Collections.IList DefaultChartData1()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            List<DefaultChart> SeriesData = new List<DefaultChart>();
            SeriesData.Add(new DefaultChart() { SeriesId = 1, SeriesName = "A", SeriesValue1 = rand.Next(20, 100) });

            if (this.ChartControlType != Controls.ChartControlType.DataBar)
            {
                SeriesData.Add(new DefaultChart() { SeriesId = 2, SeriesName = "B", SeriesValue1 = rand.Next(20, 100) });
                SeriesData.Add(new DefaultChart() { SeriesId = 3, SeriesName = "C", SeriesValue1 = rand.Next(20, 100) });
                SeriesData.Add(new DefaultChart() { SeriesId = 4, SeriesName = "D", SeriesValue1 = rand.Next(20, 100) });
                SeriesData.Add(new DefaultChart() { SeriesId = 5, SeriesName = "E", SeriesValue1 = rand.Next(20, 100) });
                SeriesData.Add(new DefaultChart() { SeriesId = 6, SeriesName = "F", SeriesValue1 = rand.Next(20, 100) });
            }

            return SeriesData;
        }

        private void ChangeContextMenuVisibility()
        {
            if (this.ChartLegand != null && this.ChartControlType == Controls.ChartControlType.Chart)
            {
                if (this.ChartLegand.Visibility == Visibility.Collapsed)
                {
                    this.AddLegend.Visibility = Visibility.Visible;
                    this.LegendProperties.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.AddLegend.Visibility = Visibility.Collapsed;
                    this.LegendProperties.Visibility = Visibility.Visible;
                }
            }
        }

        private DashStyle DashStyleFromString(string calloutLineStyle)
        {
            if (calloutLineStyle == null)
            {
                return DashStyles.Solid;
            }

            switch (calloutLineStyle)
            {
                case "Solid": return DashStyles.Solid;
                case "Dashed": return DashStyles.Dash;
                case "Dotted": return DashStyles.Dot;
                case "DashDot": return DashStyles.DashDot;
                case "DashDotDot": return DashStyles.DashDotDot;
                default: return DashStyles.Solid;
            }
        }

        private ContextMenu AddAxisTitleContextMenu(Editors.ChartObject axisTitleName)
        {
            ContextMenu axisTitleContextMenu = new System.Windows.Controls.ContextMenu();
            MenuItem showAxisTitle = new MenuItem();
            showAxisTitle.Header = "Show Axis Title";
            showAxisTitle.HorizontalAlignment = HorizontalAlignment.Left;
            showAxisTitle.IsCheckable = true;
            showAxisTitle.IsChecked = true;
            axisTitleContextMenu.Items.Add(showAxisTitle);
            axisTitleContextMenu.Items.Add(new Separator());
            MenuItem axisTitleProperties = new MenuItem();
            axisTitleProperties.Header = "Axis Title Properties...";
            axisTitleProperties.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("Images/properties.png", UriKind.Relative))
            };
            axisTitleContextMenu.Items.Add(axisTitleProperties);

            if (axisTitleName == Editors.ChartObject.PrimaryAxisTitle)
            {
                showAxisTitle.Click += new RoutedEventHandler(ShowCategoryTitle_Click);
                axisTitleProperties.Click += new RoutedEventHandler(CategoryTitleProperties_Click);
            }
            else if (axisTitleName == Editors.ChartObject.SecondaryAxisTile)
            {
                showAxisTitle.Click += new RoutedEventHandler(ShowValueTitle_Click);
                axisTitleProperties.Click += new RoutedEventHandler(ValueTitleProperties_Click);
            }
            else if (axisTitleName == Editors.ChartObject.SecondaryXAxisTitle)
            {
                showAxisTitle.Click += new RoutedEventHandler(ShowSecondaryXAxisTitle_Click);
                axisTitleProperties.Click += new RoutedEventHandler(SecondaryCategoryTitleProperties_Click);
            }
            else if (axisTitleName == Editors.ChartObject.SecondaryYAxisTitle)
            {
                showAxisTitle.Checked += new RoutedEventHandler(ShowSecondaryYAxisTitle_Click);
                axisTitleProperties.Click += new RoutedEventHandler(SecondaryValueTitleProperties_Click);
            }  

            return axisTitleContextMenu;
        }

        # endregion

        # region Panel and Smart tag Helper Methods

        private void CreateColumnButtonObj(string header, string parent)
        {
            this.SetInternalPropertyChangetoTrue();
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange =change;
            this.Panel.EditingManager.AddAction(action);

            if (header != null)
            {
                ChartArea.PrimaryAxis.ContentPath = "SeriesName";
                ChartArea.PrimaryAxis.PositionPath = "SeriesId";
                ChartArea.PrimaryAxis.LabelsSource = DefaultChartData1();
                this.secondaryXAxis.ContentPath = this.InnerChart.Areas[0].PrimaryAxis.ContentPath;
                this.secondaryXAxis.PositionPath = this.InnerChart.Areas[0].PrimaryAxis.PositionPath;
                this.secondaryXAxis.LabelsSource = this.InnerChart.Areas[0].PrimaryAxis.LabelsSource;
            }

            this.DataSetName = parent;

            if (parent != null)
            {
                //  ColumnPanel.Children.Clear();
                Button buttonObj = new Button();
                buttonObj.ContextMenu = GetButtonContextMenu(buttonObj);
                buttonObj.Margin = new Thickness(2);
                buttonObj.Content = "[" + header.Replace("_", "__") + "]";
                ColumnPanel.Children.Add(buttonObj);
            }
            else
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxFieldFromCurrentDataSet") + this.DataSetName.ToString() + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxCanBeAdded"), this.error_title, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            change.NewValue = this.GetReportItem();
            this.SetInternalPropertyChangetoFalse();

        }

        void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.SetInternalPropertyChangetoTrue();
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange =change;
            this.Panel.EditingManager.AddAction(action);

            Button buttonObj = ((MenuItem)sender).Tag as Button;
            string orgString = buttonObj.Content.ToString();
            int firstIndex = orgString.IndexOf('[')+1;
            int secondIndex = orgString.IndexOf(']');
            int totalLength = secondIndex - firstIndex;
            string str;

            if ((sender as MenuItem).Header.ToString() == "SeriesProperties...")
            {
                str = orgString.Substring(firstIndex, totalLength);
                foreach (ChartSeries series in this.InnerChart.Areas[0].Series)
                {
                    if (str == series.Label)
                    {
                        this.ChartSeries = series;
                        break;
                    }
                }
                SeriesProperties_Click(this.ChartSeries, e);
                return;
            }

            if (buttonObj != null)
            {
                StackPanel stackPanel = Util.GetParentItem<StackPanel>(buttonObj as DependencyObject) as StackPanel;
                if (stackPanel != null)
                {
                    if (buttonObj.Parent == this.ColumnPanel)
                    {
                        if (this.ColumnPanel.Children.Count > 0)
                        {
         
                            stackPanel.Children.Remove((UIElement)buttonObj);

                            if (this.ValuePanel.Children.Count == 0 && this.ColumnPanel.Children.Count == 0)
                            {
                                this.DataSetName = null;
                            }
                            if (this.ColumnPanel.Children.Count == 0)
                            {
                                ChartArea.PrimaryAxis.LabelsSource = null;
                                ChartArea.PrimaryAxis.ContentPath = "SeriesName";
                                ChartArea.PrimaryAxis.PositionPath = "SeriesId";
                                ChartArea.PrimaryAxis.LabelsSource = DefaultChartData1();
                                this.secondaryXAxis.LabelsSource = null;
                                this.secondaryXAxis.ContentPath = this.InnerChart.Areas[0].PrimaryAxis.ContentPath;
                                this.secondaryXAxis.PositionPath = this.InnerChart.Areas[0].PrimaryAxis.PositionPath;
                                this.secondaryXAxis.LabelsSource = this.InnerChart.Areas[0].PrimaryAxis.LabelsSource;
                            }
                        }
                    }

                    else if (buttonObj.Parent == this.ValuePanel)
                    {
                        if (this.ValuePanel.Children.Count > 0)
                        {
                            ChartTypes chartType = ChartTypes.Column;
                            str = orgString.Substring(firstIndex, totalLength);

                            for (int i = 0; i < this.ValuePanel.Children.Count; i++)
                            {
                                if (str == InnerChart.Areas[0].Series[i].Label)
                                {
                                    chartType = InnerChart.Areas[0].Series[i].Type;
                                    this.InnerChart.Areas[0].Series.Remove(InnerChart.Areas[0].Series[i]);
                                    this.chartSeriesPropertiesCollection.RemoveAt(i);
                                    seriesCount -= 1;                                    
                                    break;
                                }
                            }

                            stackPanel.Children.Remove((UIElement)buttonObj);

                            if (this.ValuePanel.Children.Count == 0 && this.ColumnPanel.Children.Count == 0)
                            {
                                this.DataSetName = null;
                            }

                            if (this.ValuePanel.Children.Count == 0)
                            {
                                this.InnerChart.Areas[0].Series.Clear();
                                this.chartSeriesPropertiesCollection.Clear();
                                this.ChartSeries = new ChartSeries();

                                for (int j = 1; j <= 2; j++)
                                {
                                    ChartSeries chartSeries = new ChartSeries();
                                    chartSeries.Interior = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.seriesColorCollection[j - 1]));
                                    chartSeries.DataSource = DefaultChartData1();
                                    chartSeries.BindingPathX = "SeriesID";
                                    chartSeries.BindingPathsY = new string[] { "SeriesValue1" };
                                    chartSeries.Label = "Series " + j.ToString();
                                    chartSeries.Type = chartType;
                                    this.InnerChart.Areas[0].Series.Add(chartSeries);
                                }

                                ChartArea.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ChartArea_PreviewMouseLeftButtonDown);
                            }


                        }
                    }
                    else if (buttonObj.Parent == this.Seriespanel)
                    {
                        if (this.Seriespanel.Children.Count > 0)
                        {
                            Seriespanel.Children.Remove((UIElement)buttonObj);

                            if (this.ValuePanel.Children.Count == 0 && this.ColumnPanel.Children.Count == 0 && Seriespanel.Children.Count == 0)
                            {
                                this.DataSetName = null;
                            }
                        }
                    }

                }
            }

            change.NewValue = this.GetReportItem();
            this.SetInternalPropertyChangetoFalse();

        }


        private void CreateSeriesButtonObj(string header, string parent)
        {
            this.SetInternalPropertyChangetoTrue();
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange =change;
            this.Panel.EditingManager.AddAction(action);

            if (header != null)
            {
                this.ChartArea.PrimaryAxis.ContentPath = "SeriesName";
                this.ChartArea.PrimaryAxis.PositionPath = "SeriesId";
                this.ChartArea.PrimaryAxis.LabelsSource = DefaultChartData1();
                this.secondaryXAxis.ContentPath = this.InnerChart.Areas[0].PrimaryAxis.ContentPath;
                this.secondaryXAxis.PositionPath = this.InnerChart.Areas[0].PrimaryAxis.PositionPath;
                this.secondaryXAxis.LabelsSource = this.InnerChart.Areas[0].PrimaryAxis.LabelsSource;
            }

            this.DataSetName = parent;

            if (parent != null)
            {
                // Seriespanel.Children.Clear();
                Button buttonObj = new Button();
                buttonObj.ContextMenu = GetButtonContextMenu(buttonObj);
                buttonObj.Margin = new Thickness(2);
                buttonObj.Content = "[" + header.Replace("_", "__") + "]";
                Seriespanel.Children.Add(buttonObj);
            }
            else
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxFieldFromCurrentDataSet") + this.DataSetName.ToString() + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxCanBeAdded"), this.error_title, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            change.NewValue = this.GetReportItem();
            this.SetInternalPropertyChangetoFalse();

        }

        private void CreateValueButtonObj(string header, string parent)
        {
            this.SetInternalPropertyChangetoTrue();
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange =change;
            this.Panel.EditingManager.AddAction(action);

            UpdateChartSeries(header);
            string prefixString = string.Empty;
            if (parent != null && this.DataSets != null)
            {
                string dataSetName = parent;
                string DataType = string.Empty;
                if (dataSetName != null)
                {
                    DataType = (from dataSetThis in this.DataSets
                                from DataSetfield in dataSetThis.Fields
                                where DataSetfield.Name == header
                                where dataSetThis.Name == dataSetName
                                select DataSetfield.TypeName).FirstOrDefault();
                }

                if (DataType!=null&&DataType != string.Empty && DataType.StartsWith("System."))
                {
                    if (DataType.StartsWith("System.Int")
                        || DataType.StartsWith("System.Byte")
                        || DataType.StartsWith("System.Boolean")
                        || DataType.StartsWith("System.Decimal")
                        || DataType.StartsWith("System.Double")
                        || DataType.StartsWith("System.Single")
                        )
                    {
                        prefixString = "Sum";
                    }
                    else
                    {
                        prefixString = "Count";
                    }
                }
                else if (DataType != string.Empty)
                {
                    if (
                            DataType == "bigint" ||
                            DataType == "int" ||
                            DataType == "smallint" ||
                            DataType == "tinyint" ||
                            DataType == "bit" ||
                            DataType == "decimal" ||
                            DataType == "numeric" ||
                            DataType == "money" ||
                            DataType == "smallmoney" ||
                            DataType == "float" ||
                            DataType == "real"
                        )
                    {
                        prefixString = "Sum";
                    }
                    else if (
                                DataType == "datetime" ||
                                DataType == "smalldatetime" ||
                                DataType == "char" ||
                                DataType == "varchar" ||
                                DataType == "varchar(max)" ||
                                DataType == "text" ||
                                DataType == "nchar" ||
                                DataType == "nvarchar" ||
                                DataType == "nvarchar(max)" ||
                                DataType == "ntext" ||
                                DataType == "binary" ||
                                DataType == "varbinary" ||
                                DataType == "varbinary(max)" ||
                                DataType == "image"
                            )
                    {
                        prefixString = "Count";
                    }
                    else
                    {
                        prefixString = "Count";
                    }
                }
            }

            if (prefixString != string.Empty)
            {
                if (this.DataSetName == null)
                {
                    this.DataSetName = parent;
                }

                if (parent == this.DataSetName)
                {
                    Button buttonObj = new Button();
                    buttonObj.ContextMenu = GetButtonContextMenu(buttonObj);
                    buttonObj.Margin = new Thickness(2);
                    buttonObj.Content = "[" + prefixString + "(" + header.Replace("_", "__") + ")" + "]";
                    this.DataSetName = parent;
                    buttonObj.PreviewMouseDown += new MouseButtonEventHandler(ValuePanelButtonObj_PreviewMouseDown);
                    ValuePanel.Children.Add(buttonObj);
                }
                else
                {
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxFieldFromCurrentDataSet") + this.DataSetName.ToString() + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxCanBeAdded"), this.error_title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            change.NewValue = this.GetReportItem();
            this.SetInternalPropertyChangetoFalse();
        }

        void ValuePanelButtonObj_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.RemoveChartObjectSelectionAdorner();
            int i = 0;
            foreach (Button button in this.ValuePanel.Children)
            {
                Button buttonObj = sender as Button;
                if (button.Content == buttonObj.Content)
                {
                    break;
                }
                else
                {
                    i++;
                }
            }
            this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.chartSeriesPropertiesCollection[i], IsSelected = true });
            this.SelectSeries(this.InnerChart.Areas[0].Series[i]);
            i = 0;
            e.Handled = true;
        }

        void ValuePanel_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {
            this.SetInternalPropertyChangetoTrue();           
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange =change;
            this.Panel.EditingManager.AddAction(action);

            try
            {
                TreeObjectCollection itemCollection = e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;
                if (itemCollection.Count > 0)
                {
                    TreeViewItemAdv treeViewItem = itemCollection[0] as TreeViewItemAdv;

                    if (treeViewItem != null)
                    {

                        if (this.ValuePanel.Children.Count == 0)
                        {
                            this.InnerChart.Areas[0].Series.Clear();
                            this.chartSeriesPropertiesCollection.Clear();
                        }
                        ChartArea.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(ChartArea_PreviewMouseLeftButtonDown);
                        ChartArea.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ChartGrid_PreviewMouseLeftButtonDown);
                        ChartSeries chartSeries = new ChartSeries();
                        chartSeries.DataSource = DefaultChartData1();
                        chartSeries.BindingPathX = "SeriesID";
                        chartSeries.BindingPathsY = new string[] { "SeriesValue1" };
                        chartSeries.Label = treeViewItem.Header.ToString();
                        chartSeries.MouseLeftButtonDown += new ChartMouseEventHandler(ChartSeries_MouseLeftButtonDown);
                        chartSeries.MouseRightButtonDown += new ChartMouseEventHandler(chartSeries_MouseRightButtonDown);
                        this.chartSeriesProperties = new Editors.ChartSeriesProperties();
                        chartSeries.Name = chartSeries.Label;
                        this.chartSeriesProperties.IsInternalPropertyChange = true;
                        this.chartSeriesProperties.Name = chartSeries.Name;
                        if (this.InnerChart.Areas[0].Series.Count > 0)
                        {
                            chartSeries.Type = this.InnerChart.Areas[0].Series[seriesCount].Type;
                        }
                        this.chartSeriesProperties.IsInternalPropertyChange = false;
                        this.chartSeriesProperties.PropertyChanged += new PropertyChangedEventHandler(OnChartSeriesPropertyChanged);
                        this.chartSeriesProperties.PropertyChanging += new PropertyChangingEventHandler(chartSeriesProperties_PropertyChanging);
                        this.chartSeriesPropertiesCollection.Add(chartSeriesProperties);
                        seriesCount = this.InnerChart.Areas[0].Series.Count;
                        this.InnerChart.Areas[0].Series.Add(chartSeries);


                        this.chartSeriesPropertiesCollection[seriesCount].IsInternalPropertyChange = true;

                        if (this.chartSeriesPropertiesCollection.Count > 1)
                        {
                            int nameCount = 0;
                            for (int i = 0; i < this.chartSeriesPropertiesCollection.Count - 1; i++)
                            {
                                string name = this.chartSeriesPropertiesCollection[i].Name;
                                if (name.Equals(chartSeries.Label + nameCount.ToString()) || name.Equals(chartSeries.Label))
                                {
                                    nameCount = nameCount + 1;
                                }
                            }                            
                            if (nameCount > 0)
                            {
                                this.chartSeriesPropertiesCollection[seriesCount].Name = chartSeries.Label + nameCount.ToString();
                            }
                            else
                            {
                                this.chartSeriesPropertiesCollection[seriesCount].Name = chartSeries.Label;
                            }
                        }
                        else
                        {
                            this.chartSeriesPropertiesCollection[seriesCount].Name = chartSeries.Label;
                        }
                        this.chartSeriesPropertiesCollection[seriesCount].SeriesColor = "";
                        this.chartSeriesPropertiesCollection[seriesCount].ChartType = this.InnerChart.Areas[0].Series[this.InnerChart.Areas[0].Series.Count - 1].ChartType.ToString();
                        this.chartSeriesPropertiesCollection[seriesCount].IsInternalPropertyChange = false;
                       
                    }

                    string prefixString = "Count";
                    if (treeViewItem.Tag != null)
                    {
                        string dataSetName = treeViewItem.Tag.ToString();
                        string DataType = string.Empty;
                        if (dataSetName != null)
                        {
                            DataType = (from dataSetThis in this.DataSets
                                        from DataSetfield in dataSetThis.Fields
                                        where DataSetfield.Name == treeViewItem.Header.ToString()
                                        where dataSetThis.Name == dataSetName
                                        select DataSetfield.TypeName).FirstOrDefault();
                        }

                        if (DataType != string.Empty && DataType.StartsWith("System."))
                        {
                            if (DataType.StartsWith("System.Int")
                                || DataType.StartsWith("System.Byte")
                                || DataType.StartsWith("System.Boolean")
                                || DataType.StartsWith("System.Decimal")
                                || DataType.StartsWith("System.Double")
                                || DataType.StartsWith("System.Single")
                                )
                            {
                                prefixString = "Sum";
                            }
                            else
                            {
                                prefixString = "Count";
                            }
                        }
                        else if (DataType != string.Empty)
                        {
                            if (
                                    DataType == "bigint" ||
                                    DataType == "int" ||
                                    DataType == "smallint" ||
                                    DataType == "tinyint" ||
                                    DataType == "bit" ||
                                    DataType == "decimal" ||
                                    DataType == "numeric" ||
                                    DataType == "money" ||
                                    DataType == "smallmoney" ||
                                    DataType == "float" ||
                                    DataType == "real"
                                )
                            {
                                prefixString = "Sum";
                            }
                            else if (
                                        DataType == "datetime" ||
                                        DataType == "smalldatetime" ||
                                        DataType == "char" ||
                                        DataType == "varchar" ||
                                        DataType == "varchar(max)" ||
                                        DataType == "text" ||
                                        DataType == "nchar" ||
                                        DataType == "nvarchar" ||
                                        DataType == "nvarchar(max)" ||
                                        DataType == "ntext" ||
                                        DataType == "binary" ||
                                        DataType == "varbinary" ||
                                        DataType == "varbinary(max)" ||
                                        DataType == "image"
                                    )
                            {
                                prefixString = "Count";
                            }
                            else
                            {
                                prefixString = "Count";
                            }
                        }
                    }

                    if (prefixString != string.Empty)
                    {
                        if (this.DataSetName == null)
                        {
                            this.DataSetName = treeViewItem.Tag.ToString();
                        }

                        if (treeViewItem.Tag.ToString() == this.DataSetName)
                        {
                            Button buttonObj = new Button();
                            buttonObj.ContextMenu = GetButtonContextMenu(buttonObj);
                            buttonObj.Margin = new Thickness(2);
                            buttonObj.Content = "[" + prefixString + "(" + treeViewItem.Header.ToString().Replace("_", "__") + ")" + "]";
                            this.DataSetName = treeViewItem.Tag.ToString();
                            buttonObj.PreviewMouseDown += new MouseButtonEventHandler(ValuePanelButtonObj_PreviewMouseDown);
                            ValuePanel.Children.Add(buttonObj);
                        }
                        else
                        {
                            MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxFieldFromCurrentDataSet") + this.DataSetName.ToString() + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxCanBeAdded"), this.error_title, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                }
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }


            change.NewValue = this.GetReportItem();
            this.SetInternalPropertyChangetoFalse();

        }

        void ColumnPanel_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {
            this.SetInternalPropertyChangetoTrue();
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange =change;
            this.Panel.EditingManager.AddAction(action);

            TreeObjectCollection itemCollection = e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;
            if (itemCollection != null && itemCollection.Count > 0)
            {
                TreeViewItemAdv treeViewItem = itemCollection[0] as TreeViewItemAdv;
                AddDataFields(treeViewItem, ColumnPanel);

                if (treeViewItem != null)
                {
                    ChartArea.PrimaryAxis.ContentPath = "SeriesName";
                    ChartArea.PrimaryAxis.PositionPath = "SeriesId";
                    ChartArea.PrimaryAxis.LabelsSource = DefaultChartData1();
                    this.secondaryXAxis.ContentPath = this.InnerChart.Areas[0].PrimaryAxis.ContentPath;
                    this.secondaryXAxis.PositionPath = this.InnerChart.Areas[0].PrimaryAxis.PositionPath;
                    this.secondaryXAxis.LabelsSource = this.InnerChart.Areas[0].PrimaryAxis.LabelsSource;
                }
            }

            change.NewValue = this.GetReportItem();
            this.SetInternalPropertyChangetoFalse();

        }

        private void AddDataFields(TreeViewItemAdv treeViewItem, StackPanel stackPanel)
        {
            if (this.DataSetName == null)
            {
                this.DataSetName = treeViewItem.Tag.ToString();
            }

            if (treeViewItem.Tag.ToString() == this.DataSetName)
            {
                Button buttonObj = new Button();
                buttonObj.ContextMenu = GetButtonContextMenu(buttonObj);
                buttonObj.Margin = new Thickness(2);
                buttonObj.Content = "[" + treeViewItem.Header.ToString().Replace("_", "__") + "]";
                stackPanel.Children.Add(buttonObj);
            }
            else
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxFieldFromCurrentDataSet") + this.DataSetName.ToString() + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxCanBeAdded"), this.error_title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void Panel_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
        {
            TreeObjectCollection itemCollection = e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;

            if (itemCollection != null && itemCollection.Count > 0)
            {
                TreeViewItemAdv treeViewItem = itemCollection[0] as TreeViewItemAdv;
                if (treeViewItem.Tag != null &&treeViewItem.Tag.GetType().Name == "String"&&treeViewItem.Tag.ToString()!="Parameters"&&!treeViewItem.Tag.ToString().StartsWith("#BuiltIn#") && treeViewItem.Tag.ToString() != string.Empty)
                {
                    e.Effects = DragDropEffects.All;
                    e.Handled = true;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                    e.Handled = true;
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        internal void ShowPanels()
        {
            if (ValuePanel != null && ColumnPanel != null)
            {
                ValueInnerBorder.Visibility = System.Windows.Visibility.Visible;
                ValueInnerBorder.Height = 30;
                ValueInnerBorder.Width = this.ActualWidth;
                ValueInnerBorder.Margin = new System.Windows.Thickness(0, -33, 0, 0);

                ColumnInnerBorder.Visibility = System.Windows.Visibility.Visible;
                ColumnInnerBorder.Height = 30;
                ColumnInnerBorder.Width = this.ActualWidth;
                ColumnInnerBorder.Margin = new System.Windows.Thickness(0, 0, 0, -33);
            }
            this.InvalidateArrange();
            this.InvalidateMeasure();
        }

        internal void HidePanels()
        {
            if (this.ValuePanel != null && this.ColumnPanel != null)
            {
                this.ValueInnerBorder.Visibility = Visibility.Collapsed;
                this.ColumnInnerBorder.Visibility = Visibility.Collapsed;
            }
        }

        private List<string> GetPanelChildrens(StackPanel stackPanel)
        {
            List<string> listOfChildren = new List<string>();
            if (stackPanel != null)
            {
                foreach (var item in stackPanel.Children)
                {
                    if (item is Button)
                    {
                        listOfChildren.Add((item as Button).Content.ToString());
                    }
                }
            }
            return listOfChildren;
        }

        ContextMenu GetButtonContextMenu(Button parentButton)
        {
            ContextMenu contentMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            MenuItem seriesproperties = new MenuItem { Header = "SeriesProperties..." };
            menuItem.Tag = parentButton;
            seriesproperties.Tag = parentButton;
            menuItem.Header = RESX.headerDelete;
            menuItem.Click += new RoutedEventHandler(MenuItem_Click);
            seriesproperties.Click += new RoutedEventHandler(MenuItem_Click);
            contentMenu.Items.Add(menuItem);
            contentMenu.Items.Add(seriesproperties);
            return contentMenu;
        }

        # endregion

        #region Custom Event Methods      

        internal void DeleteChartRaised()
        {
            this.Panel.DeleteSelectedReportItems();
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        void ChartControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;

            if (propertyName.Equals("IsFocusedItem"))
            {
                if (this.isFocusedItem)
                {
                    this.ValueInnerBorder.Margin = new Thickness(0, -30, 0, 0);
                    this.ColumnInnerBorder.Margin = new Thickness(0, 0, 0, -30);
                    this.SeriesInnerBorder.Margin = new Thickness(0, 0, -100, 0);
                    this.ValueInnerBorder.Visibility = System.Windows.Visibility.Visible;
                    this.ColumnInnerBorder.Visibility = System.Windows.Visibility.Visible;
                    this.SeriesInnerBorder.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.ValueInnerBorder.Margin = new Thickness(0);
                    this.ColumnInnerBorder.Margin = new Thickness(0);
                    this.SeriesInnerBorder.Margin = new Thickness(0);
                    this.UpdateLayout();
                    this.ValueInnerBorder.Visibility = System.Windows.Visibility.Collapsed;
                    this.ColumnInnerBorder.Visibility = System.Windows.Visibility.Collapsed;
                    this.SeriesInnerBorder.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        void chartProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;
            Editors.ChartProperties properties = sender as Editors.ChartProperties;

            if (e.PropertyName == "FillStyle")
            {
                propertyValue = properties.FillStyle;
            }
            if (e.PropertyName == "GradientStyle")
            {
                propertyValue = properties.GradientStyle;
            }
            if (e.PropertyName == "PrimaryColor")
            {
                propertyValue = properties.PrimaryColor;
            }
            if (e.PropertyName == "SecondaryColor")
            {
                propertyValue = properties.SecondaryColor;
            }

            if (e.PropertyName == "BorderStyle")
            {
                propertyValue = properties.BorderStyle;
            }

            if (e.PropertyName == "BorderColor")
            {
                propertyValue = properties.BorderColor;
            }

            if (e.PropertyName == "BorderWidth")
            {
                propertyValue = properties.BorderWidth;
            }

            if (e.PropertyName == "Name")
            {
                propertyValue = properties.Name;
            }

            if (e.PropertyName == "Hidden")
            {
                propertyValue = properties.Hidden;
            }

            if (e.PropertyName == "ToggleItem")
            {
                propertyValue = properties.ToggleItem;
            }

            if (e.PropertyName == "ChartBackground")
            {
                propertyValue = this.propertyValueConvertor.GetBackGroundColor(properties.ChartBackground);
            }

            if (e.PropertyName == "Source")
            {
                propertyValue = properties.BackgroundImage.Source;
            }

            if (e.PropertyName == "ImageValue")
            {
                propertyValue = properties.BackgroundImage.ImageValue;
            }

            if (e.PropertyName == "MIMEType")
            {
                propertyValue = properties.BackgroundImage.MIMEType;
            }

            if (e.PropertyName == "DocumentMapLabel")
            {
                propertyValue = properties.DocumentMapLabel;
            }

            if (e.PropertyName == "PageBreak")
            {
                propertyValue = properties.PageBreak;
            }

            if (e.PropertyName == "Left")
            {
                propertyValue = properties.Location.Left;
            }

            if (e.PropertyName == "Top")
            {
                propertyValue = properties.Location.Top;
            }

            if (e.PropertyName == "Height")
            {
                propertyValue = properties.Location.Left;
            }

            if (e.PropertyName == "Width")
            {
                propertyValue = properties.Location.Top;
            }

            if (e.PropertyName == "DataElementName")
            {
                propertyValue = properties.DataElementName;
            }

            if (e.PropertyName == "DataElementOutput")
            {
                propertyValue = properties.DataElementOutput;
            }

            if (e.PropertyName == "DataElementStyle")
            {
                propertyValue = properties.DataElementStyle;
            }
            
            if (e.PropertyName == "ChartType")
            {
                foreach (ChartSeries series in this.InnerChart.Areas[0].Series)
                {
                    propertyValue = properties.ChartType;
                    series.AdornmentsInfo = new ChartAdornmentInfo();
                    properties.AdornmentType = "None";
                }
            }
            if (e.PropertyName == "AdornmentType")
            {
                foreach (ChartSeries series in this.InnerChart.Areas[0].Series)
                {
                    if (string.IsNullOrEmpty(properties.AdornmentType) || (properties.AdornmentType != null && (properties.AdornmentType.StartsWith("=") || properties.AdornmentType.ToLower() == "none")))
                    {
                        series.AdornmentsInfo.Visible = false;

                    }
                    else
                    {
                        series.AdornmentsInfo.Visible = true;
                        series.AdornmentsInfo.SymbolHeight = 20;
                        series.AdornmentsInfo.SymbolWidth = 30;
                        series.AdornmentsInfo.SymbolInterior = Brushes.Red;
                        series.AdornmentsInfo.LabelTemplate = FindResource("dataTemplate") as DataTemplate;
                        propertyValue = properties.AdornmentType;
                    }
                }
            }

            this.propertyOldValue = propertyValue;

        }

        //Chart Properties Changed Events
        protected void OnChartPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Editors.ChartProperties properties = sender as Editors.ChartProperties;
            object propertyValue = null;

            if (e.PropertyName == "Top" || e.PropertyName == "Height" || e.PropertyName == "Width" || e.PropertyName == "Left")
            {
                if (e.PropertyName == "Top")
                {
                    propertyValue = properties.Top;
                    if (!this.chartProperties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.chartProperties.Top))
                        this.ItemTop = new RDL.DOM.Size(this.chartProperties.Top).PixelValue;
                }

                if (e.PropertyName == "Height")
                {
                    propertyValue = properties.Height;
                    if (!this.chartProperties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.chartProperties.Height))
                        this.ItemHeight = new RDL.DOM.Size(this.chartProperties.Height).PixelValue;
                }

                if (e.PropertyName == "Width")
                {
                    propertyValue = properties.Width;
                    if (!this.chartProperties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.chartProperties.Width))
                        this.ItemWidth = new RDL.DOM.Size(this.chartProperties.Width).PixelValue;
                }

                if (e.PropertyName == "Left")
                {
                    propertyValue = properties.Left;
                    if (!this.chartProperties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.chartProperties.Left))
                        this.ItemLeft = new RDL.DOM.Size(this.chartProperties.Left).PixelValue;
                }
            }
            else
            {

                if (e.PropertyName == "FillStyle" || e.PropertyName == "GradientStyle" || e.PropertyName == "PrimaryColor" || e.PropertyName == "SecondaryColor")
                {
                    if (e.PropertyName == "FillStyle")
                    {
                        propertyValue = properties.FillStyle;
                    }
                    else if (e.PropertyName == "GradientStyle")
                    {
                        propertyValue = properties.GradientStyle;
                    }
                    else if (e.PropertyName == "PrimaryColor")
                    {
                        propertyValue = properties.PrimaryColor;
                    }
                    else if (e.PropertyName == "SecondaryColor")
                    {
                        propertyValue = properties.SecondaryColor;
                    }

                    switch (properties.FillStyle.ToLower())
                    {
                        case "solid":
                            this.InnerChart.Areas[0].GridBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(properties.PrimaryColor));
                            this.defaultChartProperties.GradientStyle = "None";
                            break;
                        default:
                            Color color1 = (Color)ColorConverter.ConvertFromString(properties.PrimaryColor);
                            Color color2 = (Color)ColorConverter.ConvertFromString(properties.SecondaryColor);
                            switch (properties.GradientStyle.ToLower())
                            {
                                case "leftright":
                                    {
                                        this.InnerChart.Areas[0].GridBackground = new LinearGradientBrush(color1, color2, 0);
                                        break;
                                    }
                                case "topbottom":
                                    {
                                        this.InnerChart.Areas[0].GridBackground = new LinearGradientBrush(color1, color2, 90);
                                        break;
                                    }
                                default:
                                    {
                                        this.InnerChart.Areas[0].GridBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(properties.PrimaryColor));
                                        break;
                                    }
                            }
                            break;
                    }
                }

                if (e.PropertyName == "BorderStyle")
                {
                    propertyValue = properties.BorderStyle;
                    this.InnerBorder.DashStyle = this.propertyValueConvertor.GetBorderStyle(properties.BorderStyle);
                }

                if (e.PropertyName == "BorderColor")
                {
                    propertyValue = properties.BorderColor;
                    this.InnerBorder.BorderBrush = this.propertyValueConvertor.GetBackGroundColor(properties.BorderColor);
                }

                if (e.PropertyName == "BorderWidth")
                {
                    propertyValue = properties.BorderWidth;
                    this.InnerBorder.BorderThickness = this.propertyValueConvertor.GetBorderThickness(properties.BorderWidth);
                }

                if (e.PropertyName == "Name")
                {
                    propertyValue = properties.Name;
                    this.Name = properties.Name;
                }

                if (e.PropertyName == "Hidden")
                {
                    propertyValue = properties.Hidden;
                }

                if (e.PropertyName == "ToggleItem")
                {
                    propertyValue = properties.ToggleItem;
                }

                if (e.PropertyName == "ToolTip")
                {
                    propertyValue = properties.ToolTip;
                    if (properties.ChartBackground.StartsWith("="))
                    {
                        this.InnerChart.ToolTip = "Chart ToolTip";
                    }
                    else
                    {
                        this.InnerChart.ToolTip = properties.ToolTip;
                    }
                }

                if (e.PropertyName == "ChartBackground")
                {
                    propertyValue = properties.ChartBackground;
                    if (string.IsNullOrEmpty(this.chartProperties.BackgroundImage.ImageValue))
                    {
                        this.InnerChart.Background = this.propertyValueConvertor.GetBackGroundColor(properties.ChartBackground);
                    }
                }

                if (e.PropertyName == "ImageValue")
                {
                    this.UpdateBackgroundImage();
                    propertyValue = properties.BackgroundImage.ImageValue;
                }

                if (e.PropertyName == "Source")
                {
                    this.UpdateBackgroundImage();
                    propertyValue = properties.BackgroundImage.Source;
                }

                if (e.PropertyName == "MIMEType")
                {
                    propertyValue = properties.BackgroundImage.MIMEType;
                }

                if (e.PropertyName == "DocumentMapLabel")
                {
                    propertyValue = properties.DocumentMapLabel;
                }

                if (e.PropertyName == "PageBreak")
                {
                    propertyValue = properties.PageBreak;
                }

                if (e.PropertyName == "DataElementName")
                {
                    propertyValue = properties.DataElementName;
                }

                if (e.PropertyName == "DataElementOutput")
                {
                    propertyValue = properties.DataElementOutput;
                }

                if (e.PropertyName == "DataElementStyle")
                {
                    propertyValue = properties.DataElementStyle;
                }

                if (e.PropertyName == "ChartType")
                {
                    foreach (ChartSeries series in this.InnerChart.Areas[0].Series)
                    {
                        propertyValue = properties.ChartType;
                        if (properties.ChartType == "ExplodedDoughnut")
                        {
                            this.IsExploded = true;
                            properties.ChartType = "Doughnut";
                        }
                        else if (properties.ChartType == "ExplodedPie")
                        {
                            this.IsExploded = true;
                            properties.ChartType = "Pie";
                        }
                        //series.AdornmentsInfo = new ChartAdornmentInfo();  
                        //properties.AdornmentType = "None";   
                        if (properties.ChartType == "Line with Markers")
                        {
                            this.ChartMarker = true;
                            properties.ChartType = "Line";
                        }
                        if (properties.ChartType == "Spline with Markers")
                        {
                            this.ChartMarker = true;
                            properties.ChartType = "Spline";
                        }
                        if (this.ChartMarker == true)
                        {
                            series.AdornmentsInfo = new ChartAdornmentInfo();
                            series.AdornmentsInfo.Visible = true;
                            series.AdornmentsInfo.Symbol = Symbol.Square;
                            series.AdornmentsInfo.SymbolHeight = 10;
                            series.AdornmentsInfo.SymbolWidth = 10;
                            series.AdornmentsInfo.SymbolInterior = Brushes.Red;
                        }

                        series.Type = this.propertyValueConvertor.GetChartType(properties.ChartType);


                    }
                    this.defaultChartProperties.ChartType = properties.ChartType;
                    for (int i = 0; i < this.chartSeriesPropertiesCollection.Count; i++)
                    {
                        this.chartSeriesPropertiesCollection[i].SeriesType = this.chartProperties.ChartType;
                    }

                    if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                    {
                        this.ValueAxisTitle.Margin = new Thickness(10, 5, 10, 10);
                        this.CategoryAxisTitle.Margin = new Thickness(15, 0, 5, 10);
                        SecondaryXAxisTitle.Margin = new Thickness(15, 0, 5, 10);
                        SecondaryYAxisTitle.Margin = new Thickness(10, 5, 10, 10);
                    }
                    else
                    {
                        this.CategoryAxisTitle.Margin = new Thickness(10, 0, 10, 0);
                        this.ValueAxisTitle.Margin = new Thickness(15, 0, 5, 0);
                        SecondaryXAxisTitle.Margin = new Thickness(10, 0, 10, 0);
                        SecondaryYAxisTitle.Margin = new Thickness(15, 0, 5, 0);
                    }
                }

                if (this.ChartSeries.Type == ChartTypes.Area || this.ChartSeries.Type == ChartTypes.Bar || this.ChartSeries.Type == ChartTypes.Bubble || this.ChartSeries.Type == ChartTypes.Column || this.ChartSeries.Type == ChartTypes.FastLine || this.ChartSeries.Type == ChartTypes.Line || this.ChartSeries.Type == ChartTypes.Polar || this.ChartSeries.Type == ChartTypes.Radar || this.ChartSeries.Type == ChartTypes.Scatter || this.ChartSeries.Type == ChartTypes.StepLine || this.ChartSeries.Type == ChartTypes.RangeArea)
                {
                    if (e.PropertyName == "AdornmentType")
                    {
                        foreach (ChartSeries series in this.InnerChart.Areas[0].Series)
                        {
                            if (string.IsNullOrEmpty(properties.AdornmentType) || (properties.AdornmentType != null && (properties.AdornmentType.StartsWith("=") || properties.AdornmentType.ToLower() == "none")))
                            {
                                series.AdornmentsInfo.Visible = false;

                            }
                            else
                            {
                                series.AdornmentsInfo.Visible = true;
                                series.AdornmentsInfo.SymbolHeight = 20;
                                series.AdornmentsInfo.SymbolWidth = 30;
                                series.AdornmentsInfo.SymbolInterior = Brushes.Red;
                                series.AdornmentsInfo.LabelTemplate = FindResource("dataTemplate") as DataTemplate;
                                propertyValue = properties.AdornmentType;
                                series.AdornmentsInfo.Symbol = this.propertyValueConvertor.GetAdornmentType(properties.AdornmentType);
                            }
                        }
                        for (int i = 0; i < this.chartSeriesPropertiesCollection.Count; i++)
                        {
                            this.chartSeriesPropertiesCollection[i].AdornmentType = properties.AdornmentType;
                        }
                    }
                }
            }
            if (!this.chartProperties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.chartProperties;

                switch (e.PropertyName)
                {
                    case "Source":
                    case "MIMEType":
                    case "ImageValue":
                        change.PropertyObject = this.chartProperties.BackgroundImage;
                        break;
                }

                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
                if (e.PropertyName == "Top" || e.PropertyName == "Left" || e.PropertyName == "Height" || e.PropertyName == "Width")
                {
                    this.Panel.EditingManager.IsMergeAction = true;
                    this.RaiseReportItemSizeChangedEvent();
                    this.Panel.EditingManager.IsMergeAction = false;
                }
            }

        }

        internal void UpdateBackgroundImage()
        {
            if (this.chartProperties.BackgroundImage.Source == RDL.DOM.Source.Embedded && this.Panel != null
                && !string.IsNullOrEmpty(this.chartProperties.BackgroundImage.ImageValue))
            {
                RDL.DOM.EmbeddedImage embededimage = (from image in this.Panel.EmbeddedImages
                                                  where image.Name.Equals(this.chartProperties.BackgroundImage.ImageValue)
                                                  select image).FirstOrDefault();

                if (embededimage != null && embededimage.MIMEType != "image/emf")
                {
                    Base64ImageConverter converter = new Base64ImageConverter();
                    System.Windows.Media.Imaging.BitmapImage bitmapImage = converter.ConvertToImage(embededimage.ImageData);
                    ImageBrush brush = new ImageBrush(bitmapImage);
                    this.InnerChart.Background = brush;
                }
                else
                {
                    this.InnerChart.Background = this.propertyValueConvertor.GetBackGroundColor(this.chartProperties.ChartBackground);
                }
            }
            else
            {
                this.InnerChart.Background = this.propertyValueConvertor.GetBackGroundColor(this.chartProperties.ChartBackground);
            }
        }

        void chartSeriesProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "Name")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].Name;
            }
            else if (e.PropertyName == "ChartType")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].ChartType;
            }
            else if (e.PropertyName == "BorderWidth")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].BorderWidth;
            }
            else if (e.PropertyName == "BorderColor")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].BorderColor;
            }
            else if (e.PropertyName == "SeriesColor")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].SeriesColor;
            }
            else if (e.PropertyName == "AdornmentColor")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].AdornmentColor;
            }
            else if (e.PropertyName == "AdornmentType")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].AdornmentType;
            }
            else if (e.PropertyName == "Size")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].Size;
            }
            else if (e.PropertyName == "ShowDataLabels")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].ShowDataLabels;
            }
            else if (e.PropertyName == "DataLabelsPosition")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].DataLabelsPosition;
            }
            else if (e.PropertyName == "CategoryAxisName")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].CategoryAxisName;
            }
            else if (e.PropertyName == "ValueAxisName")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].ValueAxisName;
            }
            this.propertyOldValue = propertyValue;
        }


        //Series Properties Changed Events

        protected void OnChartSeriesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "Name")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].Name;
                if (string.IsNullOrEmpty(chartSeriesPropertiesCollection[seriesCount].Name))
                {
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxEmptySeries"), SR.GetString(CultureInfo.CurrentUICulture, "titleControlProperty"), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                if (this.InnerChart.Areas[0].Series.Any(x => x.Name == chartSeriesPropertiesCollection[seriesCount].Name))
                {
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSeriesName") + chartSeriesPropertiesCollection[seriesCount].Name + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxExist"), SR.GetString(CultureInfo.CurrentUICulture, "titleControlProperty"), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    this.InnerChart.Areas[0].Series[seriesCount].Name = chartSeriesPropertiesCollection[seriesCount].Name;
                }
            }                   
            

            if (e.PropertyName == "ChartType")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].ChartType;
                string chartSeriesType = chartSeriesPropertiesCollection[seriesCount].ChartType;
                ChartTypes chartType = this.propertyValueConvertor.GetChartType(chartSeriesPropertiesCollection[seriesCount].ChartType);
                int i = 0;
                foreach (ChartSeries series in this.InnerChart.Areas[0].Series)
                {
                    if (series.Name == seriesName)
                    {
                        if (chartType == ChartTypes.Bar || chartType == ChartTypes.StackingBar || chartType == ChartTypes.Polar || chartType == ChartTypes.Funnel || chartType == ChartTypes.Pyramid || chartType == ChartTypes.Radar || chartType == ChartTypes.Pie || chartType == ChartTypes.Doughnut || chartType == ChartTypes.StackingColumn100 || chartType == ChartTypes.StackingBar100)
                        {
                            int j = 0;
                            foreach (ChartSeries chartSeries in this.InnerChart.Areas[0].Series)
                            {
                                chartSeries.Type = chartType;
                                this.chartSeriesPropertiesCollection[j].ChartType = chartSeriesType;
                                chartSeries.AdornmentsInfo = new ChartAdornmentInfo();
                                chartSeriesPropertiesCollection[j].AdornmentType = "None";
                                j++;
                            }
                            this.chartProperties.ChartType = chartSeriesType;
                        }
                        else if (series.Type == ChartTypes.Bar || series.Type == ChartTypes.StackingBar || series.Type == ChartTypes.Polar || series.Type == ChartTypes.Funnel || series.Type == ChartTypes.Pyramid || series.Type == ChartTypes.Radar || series.Type == ChartTypes.Pie || series.Type == ChartTypes.Doughnut || series.Type == ChartTypes.StackingColumn100 || series.Type == ChartTypes.StackingBar100)
                        {
                            int j = 0;
                            foreach (ChartSeries chartSeries in this.InnerChart.Areas[0].Series)
                            {
                                chartSeries.Type = chartType;
                                this.chartSeriesPropertiesCollection[j].ChartType = chartSeriesType;
                                chartSeries.AdornmentsInfo = new ChartAdornmentInfo();
                                chartSeriesPropertiesCollection[j].AdornmentType = "None";
                                j++;
                            }
                            this.chartProperties.ChartType = chartSeriesType;
                        }
                        else
                        {
                            series.Type = chartType;
                            this.SelectSeries(series);
                            series.AdornmentsInfo = new ChartAdornmentInfo();
                            chartSeriesPropertiesCollection[seriesCount].AdornmentType = "None";
                            this.chartSeriesPropertiesCollection[i].SeriesType = this.chartSeriesPropertiesCollection[i].ChartType;
                        }
                    }
                    
                    i++;
                }

                if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                {
                    this.ValueAxisTitle.Margin = new Thickness(10, 5, 10, 10);
                    this.CategoryAxisTitle.Margin = new Thickness(15, 0, 5, 10);
                    SecondaryXAxisTitle.Margin = new Thickness(15, 0, 5, 10);
                    SecondaryYAxisTitle.Margin = new Thickness(10, 5, 10, 10);
                }
                else
                {
                    this.CategoryAxisTitle.Margin = new Thickness(10, 0, 10, 0);
                    this.ValueAxisTitle.Margin = new Thickness(15, 0, 5, 0);
                    SecondaryXAxisTitle.Margin = new Thickness(10, 0, 10, 0);
                    SecondaryYAxisTitle.Margin = new Thickness(15, 0, 5, 0);
                }

                this.SelectSeries(this.InnerChart.Areas[0].Series[seriesCount]);
            }
            if (e.PropertyName == "BorderWidth")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].BorderWidth;               
                this.InnerChart.Areas[0].Series[seriesCount].StrokeThickness = this.propertyValueConvertor.GetBorderWidth(chartSeriesPropertiesCollection[seriesCount].BorderWidth);                   
            }
            if (e.PropertyName == "BorderColor")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].BorderColor;
                this.InnerChart.Areas[0].Series[seriesCount].Stroke = this.propertyValueConvertor.GetColor(chartSeriesPropertiesCollection[seriesCount].BorderColor);                   
            }
            if (e.PropertyName == "SeriesColor")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].SeriesColor;

                if (string.IsNullOrEmpty(chartSeriesPropertiesCollection[seriesCount].SeriesColor) || (chartSeriesPropertiesCollection[seriesCount].SeriesColor != null && chartSeriesPropertiesCollection[seriesCount].SeriesColor.StartsWith("=")))
                {
                    if (seriesCount >= 30)
                    {
                        this.InnerChart.Areas[0].Series[seriesCount].Interior = new ReportingBrushConverter().ConvertFromInvariantString(this.seriesColorCollection[seriesCount - 30]);
                    }
                    else if (seriesCount >= 15)
                    {
                        this.InnerChart.Areas[0].Series[seriesCount].Interior = new ReportingBrushConverter().ConvertFromInvariantString(this.seriesColorCollection[seriesCount - 15]);
                    }
                    else
                    {
                        this.InnerChart.Areas[0].Series[seriesCount].Interior = new ReportingBrushConverter().ConvertFromInvariantString(this.seriesColorCollection[seriesCount]);
                    }
                }
                else
                {
                    this.InnerChart.Areas[0].Series[seriesCount].Interior = new ReportingBrushConverter().ConvertFromInvariantString(chartSeriesPropertiesCollection[seriesCount].SeriesColor);
                }
            }
            if (this.ChartSeries.Type == ChartTypes.Area || this.ChartSeries.Type == ChartTypes.Bar || this.ChartSeries.Type == ChartTypes.Bubble || this.ChartSeries.Type == ChartTypes.Column || this.ChartSeries.Type == ChartTypes.FastLine || this.ChartSeries.Type == ChartTypes.Line || this.ChartSeries.Type == ChartTypes.Polar || this.ChartSeries.Type == ChartTypes.Radar || this.ChartSeries.Type == ChartTypes.Scatter || this.ChartSeries.Type == ChartTypes.StepLine || this.ChartSeries.Type == ChartTypes.RangeArea)
            {
                if (e.PropertyName == "AdornmentColor") 
                {
                    propertyValue = chartSeriesPropertiesCollection[seriesCount].AdornmentColor;
                    this.InnerChart.Areas[0].Series[seriesCount].AdornmentsInfo.SymbolInterior = this.propertyValueConvertor.GetBackGroundColor(chartSeriesPropertiesCollection[seriesCount].AdornmentColor);
                }
                if (e.PropertyName == "AdornmentType")
                {
                    propertyValue = chartSeriesPropertiesCollection[seriesCount].AdornmentType;

                    if (string.IsNullOrEmpty(chartSeriesPropertiesCollection[seriesCount].AdornmentType) || (chartSeriesPropertiesCollection[seriesCount].AdornmentType != null && (chartSeriesPropertiesCollection[seriesCount].AdornmentType.StartsWith("=") || chartSeriesPropertiesCollection[seriesCount].AdornmentType.ToLower() == "none")))
                    {
                        this.InnerChart.Areas[0].Series[seriesCount].AdornmentsInfo = new ChartAdornmentInfo();
                        this.InnerChart.Areas[0].Series[seriesCount].AdornmentsInfo.Visible = false;
                        this.SelectSeries(this.InnerChart.Areas[0].Series[seriesCount]);
                    }
                    else
                    {
                        this.InnerChart.Areas[0].Series[seriesCount].AdornmentsInfo.Visible = true;
                        this.InnerChart.Areas[0].Series[seriesCount].AdornmentsInfo.Symbol = this.propertyValueConvertor.GetAdornmentType(chartSeriesPropertiesCollection[seriesCount].AdornmentType);
                        this.InnerChart.Areas[0].Series[seriesCount].AdornmentsInfo.SegmentLabelFontSize = 12;
                        this.InnerChart.Areas[0].Series[seriesCount].AdornmentsInfo.SymbolHeight = Convert.ToDouble(new RDL.DOM.Size(chartSeriesPropertiesCollection[seriesCount].Size).FloatValue);
                        this.InnerChart.Areas[0].Series[seriesCount].AdornmentsInfo.SymbolWidth = Convert.ToDouble(new RDL.DOM.Size(chartSeriesPropertiesCollection[seriesCount].Size).FloatValue);
                        this.InnerChart.Areas[0].Series[seriesCount].AdornmentsInfo.SymbolInterior = this.propertyValueConvertor.GetColor(chartSeriesPropertiesCollection[seriesCount].AdornmentColor);
                        this.SelectSeries(this.InnerChart.Areas[0].Series[seriesCount]);
                    }
                }

                if (e.PropertyName == "Size")
                {
                    propertyValue = chartSeriesPropertiesCollection[seriesCount].Size;

                    if (string.IsNullOrEmpty(chartSeriesPropertiesCollection[seriesCount].Size) || (chartSeriesPropertiesCollection[seriesCount].Size != null && chartSeriesPropertiesCollection[seriesCount].Size.StartsWith("=")))
                    {
                        this.InnerChart.Areas[0].Series[seriesCount].AdornmentsInfo.Visible = false;
                    }
                    else
                    {
                        this.InnerChart.Areas[0].Series[seriesCount].AdornmentsInfo.SymbolHeight = Convert.ToDouble(new RDL.DOM.Size(chartSeriesPropertiesCollection[seriesCount].Size).FloatValue);
                        this.InnerChart.Areas[0].Series[seriesCount].AdornmentsInfo.SymbolWidth = Convert.ToDouble(new RDL.DOM.Size(chartSeriesPropertiesCollection[seriesCount].Size).FloatValue);
                    }
                }
            }
            if (e.PropertyName == "ShowDataLabels")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].ShowDataLabels;
                switch (chartSeriesPropertiesCollection[seriesCount].ShowDataLabels.ToLower())
                {
                    case "true":
                        this.InnerChart.Areas[0].Series[seriesCount].ShowDataLabels = true;
                        this.InnerChart.Areas[0].Series[seriesCount].AdornmentsInfo.LabelTemplate = FindResource("datalabelTemplate") as DataTemplate;
                        this.SetDataLabelsPosition(this.InnerChart.Areas[0].Series[seriesCount], this.chartSeriesPropertiesCollection[seriesCount].DataLabelsPosition);
                        this.ShowDatalabels.IsChecked = true;
                        break;
                    default:
                        this.InnerChart.Areas[0].Series[seriesCount].ShowDataLabels = false;
                        this.InnerChart.Areas[0].Series[seriesCount].AdornmentsInfo.LabelTemplate = FindResource("dataTemplate") as DataTemplate;
                        this.ShowDatalabels.IsChecked = false;
                        break;
                }
            }
            if (e.PropertyName == "Position")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].DataLabelsPosition;
                this.SetDataLabelsPosition(this.InnerChart.Areas[0].Series[seriesCount], this.chartSeriesPropertiesCollection[seriesCount].DataLabelsPosition);
            }
            if (e.PropertyName == "CategoryAxisName")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].CategoryAxisName;

                if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                {
                    this.secondaryYAxis.ContentPath = this.InnerChart.Areas[0].PrimaryAxis.ContentPath;
                    this.secondaryYAxis.PositionPath = this.InnerChart.Areas[0].PrimaryAxis.PositionPath;
                    this.secondaryYAxis.LabelsSource = this.InnerChart.Areas[0].PrimaryAxis.LabelsSource;
                    this.secondaryXAxis.ContentPath = null;
                    this.secondaryXAxis.PositionPath = null;
                    this.secondaryXAxis.LabelsSource = null;
                }
                else
                {
                    this.secondaryXAxis.ContentPath = this.InnerChart.Areas[0].PrimaryAxis.ContentPath;
                    this.secondaryXAxis.PositionPath = this.InnerChart.Areas[0].PrimaryAxis.PositionPath;
                    this.secondaryXAxis.LabelsSource = this.InnerChart.Areas[0].PrimaryAxis.LabelsSource;
                    this.secondaryYAxis.ContentPath = null;
                    this.secondaryYAxis.PositionPath = null;
                    this.secondaryYAxis.LabelsSource = null;
                }               

                int i = 0, j = 0;
                foreach (ChartSeries series in this.InnerChart.Areas[0].Series)
                {
                    if (this.chartSeriesPropertiesCollection[i].CategoryAxisName == "Secondary")
                    {
                        if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                        {
                            series.XAxis = this.secondaryYAxis;
                        }
                        else
                        {
                            series.XAxis = this.secondaryXAxis;
                        }
                        j++;
                    }
                    else
                    {
                        series.XAxis = this.InnerChart.Areas[0].PrimaryAxis;
                    }
                    i++;
                }
                if (j == this.InnerChart.Areas[0].Series.Count)
                {
                    if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                    {
                        this.InnerChart.Areas[0].PrimaryAxis.AxisVisibility = Visibility.Collapsed;
                        this.secondaryYAxis.AxisVisibility = Visibility.Visible;
                    }
                    else
                    {
                        this.InnerChart.Areas[0].PrimaryAxis.AxisVisibility = Visibility.Collapsed;
                        this.secondaryXAxis.AxisVisibility = Visibility.Visible;
                    }
                }
                else if (j > 0)
                {
                    if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                    {
                        this.InnerChart.Areas[0].PrimaryAxis.AxisVisibility = Visibility.Visible;
                        this.secondaryYAxis.AxisVisibility = Visibility.Visible;
                    }
                    else
                    {
                        this.secondaryXAxis.AxisVisibility = Visibility.Visible;
                        this.InnerChart.Areas[0].PrimaryAxis.AxisVisibility = Visibility.Visible;
                    }
                }
                else if (j == 0)
                {
                    if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                    {
                        this.InnerChart.Areas[0].PrimaryAxis.AxisVisibility = Visibility.Collapsed;
                        this.secondaryYAxis.AxisVisibility = Visibility.Visible;
                    }
                    else
                    {
                        this.secondaryXAxis.AxisVisibility = Visibility.Collapsed;
                        this.InnerChart.Areas[0].PrimaryAxis.AxisVisibility = Visibility.Visible;
                    }
                }
            }
            if (e.PropertyName == "ValueAxisName")
            {
                propertyValue = chartSeriesPropertiesCollection[seriesCount].ValueAxisName;

                if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                {
                    this.secondaryYAxis.ContentPath = this.InnerChart.Areas[0].PrimaryAxis.ContentPath;
                    this.secondaryYAxis.PositionPath = this.InnerChart.Areas[0].PrimaryAxis.PositionPath;
                    this.secondaryYAxis.LabelsSource = this.InnerChart.Areas[0].PrimaryAxis.LabelsSource;
                    this.secondaryXAxis.ContentPath = null;
                    this.secondaryXAxis.PositionPath = null;
                    this.secondaryXAxis.LabelsSource = null;
                }
                else
                {
                    this.secondaryXAxis.ContentPath = this.InnerChart.Areas[0].PrimaryAxis.ContentPath;
                    this.secondaryXAxis.PositionPath = this.InnerChart.Areas[0].PrimaryAxis.PositionPath;
                    this.secondaryXAxis.LabelsSource = this.InnerChart.Areas[0].PrimaryAxis.LabelsSource;
                    this.secondaryYAxis.ContentPath = null;
                    this.secondaryYAxis.PositionPath = null;
                    this.secondaryYAxis.LabelsSource = null;
                }               

                int i = 0, j = 0;
                foreach (ChartSeries series in this.InnerChart.Areas[0].Series)
                {
                    if (this.chartSeriesPropertiesCollection[i].ValueAxisName == "Secondary")
                    {
                        if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                        {
                            series.YAxis = this.secondaryXAxis;
                        }
                        else
                        {
                            series.YAxis = this.secondaryYAxis;
                        }
                        j++;
                    }
                    else
                    {
                        series.YAxis = this.InnerChart.Areas[0].SecondaryAxis;
                    }
                    i++;
                }
                if (j == this.InnerChart.Areas[0].Series.Count)
                {
                    if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                    {
                        this.InnerChart.Areas[0].SecondaryAxis.AxisVisibility = Visibility.Collapsed;
                        this.secondaryXAxis.AxisVisibility = Visibility.Visible;
                    }
                    else
                    {
                        this.InnerChart.Areas[0].SecondaryAxis.AxisVisibility = Visibility.Collapsed;
                        this.secondaryYAxis.AxisVisibility = Visibility.Visible;
                    }
                }
                else if (j > 0)
                {
                    if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                    {
                        this.InnerChart.Areas[0].SecondaryAxis.AxisVisibility = Visibility.Visible;
                        this.secondaryXAxis.AxisVisibility = Visibility.Visible;
                    }
                    else
                    {
                        this.secondaryYAxis.AxisVisibility = Visibility.Visible;
                        this.InnerChart.Areas[0].SecondaryAxis.AxisVisibility = Visibility.Visible;
                    }
                }
                else if (j == 0)
                {
                    if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                    {
                        this.InnerChart.Areas[0].SecondaryAxis.AxisVisibility = Visibility.Visible;
                        this.secondaryXAxis.AxisVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this.secondaryYAxis.AxisVisibility = Visibility.Collapsed;
                        this.InnerChart.Areas[0].SecondaryAxis.AxisVisibility = Visibility.Visible;
                    }
                }
            }

            if (!chartSeriesPropertiesCollection[seriesCount].IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = chartSeriesPropertiesCollection[seriesCount];
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }
        }
        void titleProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;
            if (e.PropertyName == "Name")
            {
                propertyValue = this.titleProperties.Name;
            }
            if (e.PropertyName == "TitleFontFamily")
            {
                propertyValue = titleProperties.TitleFontFamily;
            }
            if (e.PropertyName == "TitleFontSize")
            {
                propertyValue = titleProperties.TitleFontSize;
            }
            if (e.PropertyName == "TitleFontStyle")
            {
                propertyValue = titleProperties.TitleFontStyle;
            }
            if (e.PropertyName == "TitleFontColor")
            {
                propertyValue = titleProperties.TitleFontColor;
            }
            if (e.PropertyName == "TitleBackFill")
            {
                propertyValue = titleProperties.TitleBackFill;
            }
            if (e.PropertyName == "Visibility")
            {
                propertyValue = titleProperties.Visibility;
            }
            this.propertyOldValue = propertyValue;
        }

        //Chart Title Properties Changed
        protected void OnChartTitlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;
            if (e.PropertyName == "Name")
            {
                propertyValue = titleProperties.Name;
                this.ChartTitle.Text = titleProperties.Name;
            }
            if (e.PropertyName == "TitleFontFamily")
            {
                propertyValue = titleProperties.TitleFontFamily;
                this.ChartTitle.FontFamily = this.propertyValueConvertor.GetFontFamilyName(titleProperties.TitleFontFamily);
            }
            if (e.PropertyName == "TitleFontSize")
            {
                propertyValue = titleProperties.TitleFontSize;
                this.ChartTitle.FontSize = this.propertyValueConvertor.GetFontSize(titleProperties.TitleFontSize);
            }
            if (e.PropertyName == "TitleFontStyle")
            {
                propertyValue = titleProperties.TitleFontStyle;
                this.ChartTitle.FontStyle = this.propertyValueConvertor.GetFontStyle(titleProperties.TitleFontStyle);
            }
            if (e.PropertyName == "TitleFontColor")
            {
                propertyValue = titleProperties.TitleFontColor;
                this.ChartTitle.Foreground = this.propertyValueConvertor.GetColor(titleProperties.TitleFontColor);
            }
            if (e.PropertyName == "TitleBackFill")
            {
                propertyValue = titleProperties.TitleBackFill;
                this.ChartTitle.Background = this.propertyValueConvertor.GetBackGroundColor(titleProperties.TitleBackFill);
            }
            if (e.PropertyName == "Visibility")
            {
                propertyValue = titleProperties.Visibility;
                if (titleProperties.Visibility == "False")
                {
                    this.ChartTitle.Visibility = Visibility.Collapsed;
                    this.RemoveSelectionAdorner(ChartTitle);
                }
                else
                {
                    this.ChartTitle.Visibility = Visibility.Visible;
                    this.RemoveChartObjectSelectionAdorner();
                    this.SetTitleSelectionAdorner(ChartTitle, Editors.ChartObject.ChartTitle);
                }
            }
            if (!this.titleProperties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.titleProperties;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }

        }

        void chartLegendProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;
            if (e.PropertyName == "Name")
            {
                propertyValue = this.chartLegendProperties.Name;
            }
            if (e.PropertyName == "BackFill")
            {
                propertyValue = this.chartLegendProperties.BackFill;
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = this.chartLegendProperties.FontFamily;
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = this.chartLegendProperties.FontSize;
            }
            if (e.PropertyName == "FontStyle")
            {
                propertyValue = this.chartLegendProperties.FontStyle;
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = this.chartLegendProperties.FontColor;
            }
            if (e.PropertyName == "BorderThickness")
            {
                propertyValue = this.chartLegendProperties.BorderThickness;
            }
            if (e.PropertyName == "LegendBorderColor")
            {
                propertyValue = this.chartLegendProperties.LegendBorderColor;
            }
            if (e.PropertyName == "Layout")
            {
                propertyValue = this.chartLegendProperties.Layout;
            }
            if (e.PropertyName == "Position")
            {
                propertyValue = this.chartLegendProperties.Position;
            }
            this.propertyOldValue = propertyValue;
        }
        //Chart Legend Properties Changed
        protected void OnChartLegendPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;
            if (e.PropertyName == "Name")
            {
                propertyValue = this.chartLegendProperties.Name;
                ChartLegand.Name = this.chartLegendProperties.Name;
            }
            if (e.PropertyName == "BackFill")
            {
                propertyValue = this.chartLegendProperties.BackFill;
                ChartLegand.Background = this.propertyValueConvertor.GetBackGroundColor(this.chartLegendProperties.BackFill);
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = this.chartLegendProperties.FontFamily;
                ChartLegand.FontFamily = this.propertyValueConvertor.GetFontFamilyName(this.chartLegendProperties.FontFamily);
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = this.chartLegendProperties.FontSize;
                ChartLegand.FontSize = this.propertyValueConvertor.GetFontSize(this.chartLegendProperties.FontSize);
            }
            if (e.PropertyName == "FontStyle")
            {
                propertyValue = this.chartLegendProperties.FontStyle;
                ChartLegand.FontStyle = this.propertyValueConvertor.GetFontStyle(this.chartLegendProperties.FontStyle);
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = this.chartLegendProperties.FontColor;
                ChartLegand.Foreground = this.propertyValueConvertor.GetColor(this.chartLegendProperties.FontColor);
            }
            if (e.PropertyName == "BorderThickness")
            {
                propertyValue = this.chartLegendProperties.BorderThickness;
                ChartLegand.BorderThickness = this.propertyValueConvertor.GetBorderThickness(this.chartLegendProperties.BorderThickness);
            }
            if (e.PropertyName == "LegendBorderColor")
            {
                propertyValue = this.chartLegendProperties.LegendBorderColor;
                ChartLegand.BorderBrush = this.propertyValueConvertor.GetColor(this.chartLegendProperties.LegendBorderColor);
            }
            if (e.PropertyName == "Layout")
            {
                propertyValue = this.chartLegendProperties.Layout;
                switch (this.chartLegendProperties.Layout.ToLower())
                {
                    case "column":
                        {
                            int i = 0;
                            foreach (ChartSeries chartSeries in InnerChart.Areas[0].Series)
                            {
                                i++;
                            }
                            ChartLegand.ColumnsCount = 1;
                            ChartLegand.RowsCount = i;
                            break;
                        }
                    default:
                        {
                            int i = 0;
                            foreach (ChartSeries chartSeries in InnerChart.Areas[0].Series)
                            {
                                i++;
                            }
                            ChartLegand.ColumnsCount = i;
                            ChartLegand.RowsCount = 1;
                            break;
                        }
                }
            }
            if (e.PropertyName == "Position")
            {
                propertyValue = this.chartLegendProperties.Position;
                switch (this.chartLegendProperties.Position.ToLower())
                {
                    case "bottomcenter":
                        ChartDockPanel.SetDock(ChartLegand, ChartDock.Bottom);
                        break;
                    case "leftcenter":
                        ChartDockPanel.SetDock(ChartLegand, ChartDock.Left);
                        break;
                    case "rightcenter":
                        ChartDockPanel.SetDock(ChartLegand, ChartDock.Right);
                        break;
                    default:
                        ChartDockPanel.SetDock(ChartLegand, ChartDock.Top);
                        break;
                }
            }
            if (!this.chartLegendProperties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.chartLegendProperties;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }

        }

        void legendTitleProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "Caption")
            {
                propertyValue = legendTitleProperties.Caption;
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = legendTitleProperties.FontFamily;
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = legendTitleProperties.FontSize;
            }
            if (e.PropertyName == "FontStyle")
            {
                propertyValue = legendTitleProperties.FontStyle;
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = legendTitleProperties.FontColor;
            }
            if (e.PropertyName == "BackFill")
            {
                propertyValue = legendTitleProperties.BackFill;
            }
            this.propertyOldValue = propertyValue;
        }

        //Legend Title Properties Changed
        protected void OnLegendTitilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "Caption")
            {
                propertyValue = legendTitleProperties.Caption;
                this.LegendTextbox.Text = this.legendTitleProperties.Caption;
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = legendTitleProperties.FontFamily;
                this.LegendTextbox.FontFamily = this.propertyValueConvertor.GetFontFamilyName(this.legendTitleProperties.FontFamily);
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = legendTitleProperties.FontSize;
                this.LegendTextbox.FontSize = this.propertyValueConvertor.GetFontSize(this.legendTitleProperties.FontSize);
            }
            if (e.PropertyName == "FontStyle")
            {
                propertyValue = legendTitleProperties.FontStyle;
                this.LegendTextbox.FontStyle = this.propertyValueConvertor.GetFontStyle(this.legendTitleProperties.FontStyle);
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = legendTitleProperties.FontColor;
                this.LegendTextbox.Foreground = this.propertyValueConvertor.GetColor(this.legendTitleProperties.FontColor);
            }
            if (e.PropertyName == "BackFill")
            {
                propertyValue = legendTitleProperties.BackFill;
                this.LegendTextbox.Background = this.propertyValueConvertor.GetBackGroundColor(this.legendTitleProperties.BackFill);
            }

            if (!this.legendTitleProperties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.legendTitleProperties;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }

        }

        void categoryAxisTitleProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;
            if (e.PropertyName == "Name")
            {
                propertyValue = categoryAxisTitleProperties.Name;
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = categoryAxisTitleProperties.FontFamily;
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = categoryAxisTitleProperties.FontSize;
            }
            if (e.PropertyName == "FontStyle")
            {
                propertyValue = categoryAxisTitleProperties.FontStyle;
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = categoryAxisTitleProperties.FontColor;
            }
            if (e.PropertyName == "TitleAlignment")
            {
                propertyValue = categoryAxisTitleProperties.TitleAlignment;
            }
            this.propertyOldValue = propertyValue;
        }

        //Chart category Axis Title Properties Changed
        protected void OnChartCategoryAxisTitlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;
            if (e.PropertyName == "Name")
            {
                propertyValue = categoryAxisTitleProperties.Name;
                this.CategoryAxisTitle.Text = categoryAxisTitleProperties.Name;
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = categoryAxisTitleProperties.FontFamily;
                this.CategoryAxisTitle.FontFamily = this.propertyValueConvertor.GetFontFamilyName(categoryAxisTitleProperties.FontFamily);
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = categoryAxisTitleProperties.FontSize;
                this.CategoryAxisTitle.FontSize = this.propertyValueConvertor.GetFontSize(categoryAxisTitleProperties.FontSize);
            }
            if (e.PropertyName == "FontStyle")
            {
                propertyValue = categoryAxisTitleProperties.FontStyle;
                this.CategoryAxisTitle.FontStyle = this.propertyValueConvertor.GetFontStyle(categoryAxisTitleProperties.FontStyle);
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = categoryAxisTitleProperties.FontColor;
                this.CategoryAxisTitle.Foreground = this.propertyValueConvertor.GetColor(categoryAxisTitleProperties.FontColor);
            }
            if (e.PropertyName == "TitleAlignment")
            {
                propertyValue = categoryAxisTitleProperties.TitleAlignment;
                this.ChartArea.PrimaryAxis.HeaderAlignment = this.propertyValueConvertor.GetChartAlignment(categoryAxisTitleProperties.TitleAlignment);
                switch (categoryAxisTitleProperties.TitleAlignment.ToLower())
                {
                    case "far":
                        CategoryAxisTitle.TextAlignment = TextAlignment.Right;
                        break;
                    case "near":
                        CategoryAxisTitle.TextAlignment = TextAlignment.Left;
                        break;
                    default:
                        CategoryAxisTitle.TextAlignment = TextAlignment.Center;
                        break;
                }
            }
            if (!this.categoryAxisTitleProperties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.categoryAxisTitleProperties;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }
        }

        void secondaryXAxisTitleProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "Name")
            {
                propertyValue = secondaryXAxisTitleProperties.Name;
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = secondaryXAxisTitleProperties.FontFamily;
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = secondaryXAxisTitleProperties.FontSize;
            }
            if (e.PropertyName == "FontStyle")
            {
                propertyValue = secondaryXAxisTitleProperties.FontStyle;
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = secondaryXAxisTitleProperties.FontColor;
            }
            if (e.PropertyName == "TitleAlignment")
            {
                propertyValue = secondaryXAxisTitleProperties.TitleAlignment;
            }
            this.propertyOldValue = propertyValue;
        }

        //Chart secondary x-Axis Title Properties Changed 
        protected void OnChartSecondaryXAxisTitlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "Name")
            {
                propertyValue = secondaryXAxisTitleProperties.Name;
                this.SecondaryXAxisTitle.Text = secondaryXAxisTitleProperties.Name;
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = secondaryXAxisTitleProperties.FontFamily;
                this.SecondaryXAxisTitle.FontFamily = this.propertyValueConvertor.GetFontFamilyName(secondaryXAxisTitleProperties.FontFamily);
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = secondaryXAxisTitleProperties.FontSize;
                this.SecondaryXAxisTitle.FontSize = this.propertyValueConvertor.GetFontSize(secondaryXAxisTitleProperties.FontSize);
            }
            if (e.PropertyName == "FontStyle")
            {
                propertyValue = secondaryXAxisTitleProperties.FontStyle;
                this.SecondaryXAxisTitle.FontStyle = this.propertyValueConvertor.GetFontStyle(secondaryXAxisTitleProperties.FontStyle);
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = secondaryXAxisTitleProperties.FontColor;
                this.SecondaryXAxisTitle.Foreground = this.propertyValueConvertor.GetColor(secondaryXAxisTitleProperties.FontColor);
            }
            if (e.PropertyName == "TitleAlignment")
            {
                propertyValue = secondaryXAxisTitleProperties.TitleAlignment;
                this.secondaryXAxis.HeaderAlignment = this.propertyValueConvertor.GetChartAlignment(secondaryXAxisTitleProperties.TitleAlignment);
                switch (secondaryXAxisTitleProperties.TitleAlignment.ToLower())
                {
                    case "far":
                        SecondaryXAxisTitle.TextAlignment = TextAlignment.Right;
                        break;
                    case "near":
                        SecondaryXAxisTitle.TextAlignment = TextAlignment.Left;
                        break;
                    default:
                        SecondaryXAxisTitle.TextAlignment = TextAlignment.Center;
                        break;
                }
            }
            if (!this.secondaryXAxisTitleProperties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.secondaryXAxisTitleProperties;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }
        }

        void valueAxisTitleProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "Name")
            {
                propertyValue = this.valueAxisTitleProperties.Name;
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = valueAxisTitleProperties.FontFamily;
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = valueAxisTitleProperties.FontSize;
            }
            if (e.PropertyName == "FontStyle")
            {
                propertyValue = valueAxisTitleProperties.FontStyle;
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = valueAxisTitleProperties.FontColor;
            }
            if (e.PropertyName == "TitleAlignment")
            {
                propertyValue = valueAxisTitleProperties.TitleAlignment;
            }

            this.propertyOldValue = propertyValue;
        }

        //Chart category Axis Title Properties Changed
        protected void OnChartValueAxisTitlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;
            if (e.PropertyName == "Name")
            {
                propertyValue = this.valueAxisTitleProperties.Name;
                this.ValueAxisTitle.Text = this.valueAxisTitleProperties.Name;
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = valueAxisTitleProperties.FontFamily;
                this.ValueAxisTitle.FontFamily = this.propertyValueConvertor.GetFontFamilyName(valueAxisTitleProperties.FontFamily);
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = valueAxisTitleProperties.FontSize;
                this.ValueAxisTitle.FontSize = this.propertyValueConvertor.GetFontSize(valueAxisTitleProperties.FontSize);
            }
            if (e.PropertyName == "FontStyle")
            {
                propertyValue = valueAxisTitleProperties.FontStyle;
                this.ValueAxisTitle.FontStyle = this.propertyValueConvertor.GetFontStyle(valueAxisTitleProperties.FontStyle);
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = valueAxisTitleProperties.FontColor;
                this.ValueAxisTitle.Foreground = this.propertyValueConvertor.GetColor(valueAxisTitleProperties.FontColor);
            }
            if (e.PropertyName == "TitleAlignment")
            {
                propertyValue = valueAxisTitleProperties.TitleAlignment;
                this.ChartArea.SecondaryAxis.HeaderAlignment = this.propertyValueConvertor.GetChartAlignment(valueAxisTitleProperties.TitleAlignment);
                switch (valueAxisTitleProperties.TitleAlignment.ToLower())
                {
                    case "far":
                        ValueAxisTitle.TextAlignment = TextAlignment.Right;
                        break;
                    case "near":
                        ValueAxisTitle.TextAlignment = TextAlignment.Left;
                        break;
                    default:
                        ValueAxisTitle.TextAlignment = TextAlignment.Center;
                        break;
                }
            }

            if (!this.valueAxisTitleProperties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.valueAxisTitleProperties;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }
        }

        void secondaryYAxisTitleProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;
            if (e.PropertyName == "Name")
            {
                propertyValue = secondaryYAxisTitleProperties.Name;
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = secondaryYAxisTitleProperties.FontFamily;
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = secondaryYAxisTitleProperties.FontSize;
            }
            if (e.PropertyName == "FontStyle")
            {
                propertyValue = secondaryYAxisTitleProperties.FontStyle;
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = secondaryYAxisTitleProperties.FontColor;
            }
            if (e.PropertyName == "TitleAlignment")
            {
                propertyValue = secondaryYAxisTitleProperties.TitleAlignment;
            }
            this.propertyOldValue = propertyValue;
        }

        //Chart secondary y-Axis Title Properties Changed
        protected void OnChartSecondaryYAxisTitlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;
            if (e.PropertyName == "Name")
            {
                propertyValue = secondaryYAxisTitleProperties.Name;
                this.SecondaryYAxisTitle.Text = secondaryYAxisTitleProperties.Name;
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = secondaryYAxisTitleProperties.FontFamily;
                this.SecondaryYAxisTitle.FontFamily = this.propertyValueConvertor.GetFontFamilyName(secondaryYAxisTitleProperties.FontFamily);
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = secondaryXAxisTitleProperties.FontSize;
                this.SecondaryYAxisTitle.FontSize = this.propertyValueConvertor.GetFontSize(secondaryYAxisTitleProperties.FontSize);
            }
            if (e.PropertyName == "FontStyle")
            {
                propertyValue = secondaryXAxisTitleProperties.FontStyle;
                this.SecondaryYAxisTitle.FontStyle = this.propertyValueConvertor.GetFontStyle(secondaryYAxisTitleProperties.FontStyle);
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = secondaryXAxisTitleProperties.FontColor;
                this.SecondaryYAxisTitle.Foreground = this.propertyValueConvertor.GetColor(secondaryYAxisTitleProperties.FontColor);
            }
            if (e.PropertyName == "TitleAlignment")
            {
                propertyValue = secondaryXAxisTitleProperties.TitleAlignment;
                this.secondaryYAxis.HeaderAlignment = this.propertyValueConvertor.GetChartAlignment(secondaryYAxisTitleProperties.TitleAlignment);
                switch (secondaryYAxisTitleProperties.TitleAlignment.ToLower())
                {
                    case "far":
                        SecondaryYAxisTitle.TextAlignment = TextAlignment.Right;
                        break;
                    case "near":
                        SecondaryYAxisTitle.TextAlignment = TextAlignment.Left;
                        break;
                    default:
                        SecondaryYAxisTitle.TextAlignment = TextAlignment.Center;
                        break;
                }
            }

            if (!this.secondaryXAxisTitleProperties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.secondaryYAxisTitleProperties;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }
        }

        void chartCategoryAxisProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "ReverseDirection")
            {
                propertyValue = chartCategoryAxisProperties.ReverseDirection;
            }
            else if (e.PropertyName == "LineStyle")
            {
                propertyValue = chartCategoryAxisProperties.LineStyle;
            }
            else if (e.PropertyName == "LineColor")
            {
                propertyValue = chartCategoryAxisProperties.LineColor;
            }
            else if (e.PropertyName == "LineWidth")
            {
                propertyValue = chartCategoryAxisProperties.LineWidth;
            }
            else if (e.PropertyName == "FontFamily")
            {
                propertyValue = chartCategoryAxisProperties.FontFamily;
            }
            else if (e.PropertyName == "FontSize")
            {
                propertyValue = chartCategoryAxisProperties.FontSize;
            }
            else if (e.PropertyName == "FontColor")
            {
                propertyValue = chartCategoryAxisProperties.FontColor;
            }
            else if (e.PropertyName == "FontWeight")
            {
                propertyValue = chartCategoryAxisProperties.FontWeight;
            }
            else if (e.PropertyName == "FontAngle")
            {
                propertyValue = chartCategoryAxisProperties.FontAngle;
            }
            else if (e.PropertyName == "HideAxisLabels")
            {
                propertyValue = chartCategoryAxisProperties.HideAxisLabels;
            }
            else if (e.PropertyName == "TickStyle") 
            {
                propertyValue = chartCategoryAxisProperties.TickStyle;
            }
            else if (e.PropertyName == "TickColor")
            {
                propertyValue = chartCategoryAxisProperties.TickColor;
            }
            else if (e.PropertyName == "TickLength")
            {
                propertyValue = chartCategoryAxisProperties.TickLength;
            }
            else if (e.PropertyName == "TickWidth")
            {
                propertyValue = chartCategoryAxisProperties.TickWidth;
            }
            else if (e.PropertyName == "EnableMajorTickMarks")
            {
                propertyValue = chartCategoryAxisProperties.EnableMajorTickMarks;
            }
            else if (e.PropertyName == "MinorTickStyle") 
            {
                propertyValue = chartCategoryAxisProperties.MinorTickStyle;
            }
            else if (e.PropertyName == "MinorTickColor")
            {
                propertyValue = chartCategoryAxisProperties.MinorTickColor;
            }
            else if (e.PropertyName == "MinorTickLength")
            {
                propertyValue = chartCategoryAxisProperties.TickLength;
            }
            else if (e.PropertyName == "EnableMinorTickMarks")
            {
                propertyValue = chartCategoryAxisProperties.EnableMinorTickMarks;
            }          
            else if (e.PropertyName == "Visibility")
            {
                propertyValue = chartCategoryAxisProperties.Visibility;
            }
            this.propertyOldValue = propertyValue;
        }

        //Chart Category Axis Properties Changed  
        protected void OnChartCategoryAxisPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;
            if (e.PropertyName == "ReverseDirection")
            {
                propertyValue = chartCategoryAxisProperties.ReverseDirection;
                if (chartCategoryAxisProperties.ReverseDirection.ToLower() == "true" && chartValueAxisProperties.ReverseDirection.ToLower() == "true")
                {
                    this.InnerChart.Areas[0].PrimaryAxis.IsInversed = true;
                    this.InnerChart.Areas[0].SecondaryAxis.IsInversed = true;
                    this.InnerChart.Areas[0].PrimaryAxis.OpposedPosition = true;
                    this.InnerChart.Areas[0].SecondaryAxis.OpposedPosition = true;
                }
                else if (chartCategoryAxisProperties.ReverseDirection.ToLower() == "false" && chartValueAxisProperties.ReverseDirection.ToLower() == "false")
                {
                    this.InnerChart.Areas[0].PrimaryAxis.IsInversed = false;
                    this.InnerChart.Areas[0].SecondaryAxis.IsInversed = false;
                    this.InnerChart.Areas[0].PrimaryAxis.OpposedPosition = false;
                    this.InnerChart.Areas[0].SecondaryAxis.OpposedPosition = false;
                }
                else if (chartCategoryAxisProperties.ReverseDirection.ToLower() == "true")
                {
                    InnerChart.Areas[0].PrimaryAxis.IsInversed = true;
                    InnerChart.Areas[0].SecondaryAxis.OpposedPosition = true;
                }
                else
                {
                    InnerChart.Areas[0].PrimaryAxis.IsInversed = false;
                    InnerChart.Areas[0].SecondaryAxis.OpposedPosition = false;
                }
            }
            if (e.PropertyName == "LineStyle" || e.PropertyName == "LineColor" || e.PropertyName == "LineWidth")
            {
                if (e.PropertyName == "LineStyle")
                {
                    propertyValue = chartCategoryAxisProperties.LineColor;
                }
                else if (e.PropertyName == "LineColor")
                {
                    propertyValue = chartCategoryAxisProperties.LineColor;
                }
                else if (e.PropertyName == "LineWidth")
                {
                    propertyValue = chartCategoryAxisProperties.LineWidth;
                }

                Pen pen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartCategoryAxisProperties.LineColor)), Convert.ToDouble(new RDL.DOM.Size(chartCategoryAxisProperties.LineWidth).PixelValue));
                CharAreaColorPrimary = (Color)ColorConverter.ConvertFromString(chartCategoryAxisProperties.LineColor);
                pen.DashStyle = DashStyleFromString(chartCategoryAxisProperties.LineStyle);
                InnerChart.Areas[0].PrimaryAxis.LineStroke = pen;
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = chartCategoryAxisProperties.FontFamily;
                InnerChart.Areas[0].PrimaryAxis.LabelFontFamily = this.propertyValueConvertor.GetFontFamilyName(chartCategoryAxisProperties.FontFamily);
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = chartCategoryAxisProperties.FontSize;
                InnerChart.Areas[0].PrimaryAxis.LabelFontSize = this.propertyValueConvertor.GetFontSize(chartCategoryAxisProperties.FontSize);
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = chartCategoryAxisProperties.FontColor;
                InnerChart.Areas[0].PrimaryAxis.LabelForeground = this.propertyValueConvertor.GetColor(chartCategoryAxisProperties.FontColor);
            }
            if (e.PropertyName == "FontWeight")
            {
                propertyValue = chartCategoryAxisProperties.FontWeight;
                InnerChart.Areas[0].PrimaryAxis.LabelFontWeight = this.propertyValueConvertor.GetFontWeights(chartCategoryAxisProperties.FontWeight);
            }
            if (e.PropertyName == "FontAngle")
            {
                propertyValue = chartCategoryAxisProperties.FontAngle;
                if (string.IsNullOrEmpty(chartCategoryAxisProperties.FontAngle) || (chartCategoryAxisProperties.FontAngle != null && chartCategoryAxisProperties.FontAngle.StartsWith("=")))
                {
                    InnerChart.Areas[0].PrimaryAxis.LabelRotateAngle = 0;
                }
                else
                {
                    InnerChart.Areas[0].PrimaryAxis.LabelRotateAngle = Convert.ToDouble(new RDL.DOM.Size(chartCategoryAxisProperties.FontAngle).PixelValue);
                }
            }
            if (e.PropertyName == "HideAxisLabels")
            {
                propertyValue = chartCategoryAxisProperties.HideAxisLabels;
                if (chartCategoryAxisProperties.HideAxisLabels.ToLower() == "true")
                {
                    InnerChart.Areas[0].PrimaryAxis.LabelTemplate = FindResource("dataTemplate") as DataTemplate;
                }
                else
                {
                    InnerChart.Areas[0].PrimaryAxis.LabelTemplate = null;
                }
            }
            if (e.PropertyName == "TickStyle" || e.PropertyName == "TickColor" || e.PropertyName == "TickLength" || e.PropertyName == "EnableMajorTickMarks")
            {
                if (e.PropertyName == "TickStyle")
                {
                    propertyValue = chartCategoryAxisProperties.TickStyle;
                }
                else if (e.PropertyName == "TickColor")
                {
                    propertyValue = chartCategoryAxisProperties.TickColor;
                }
                else if (e.PropertyName == "TickLength")
                {
                    propertyValue = chartCategoryAxisProperties.TickLength;
                }
                else if (e.PropertyName == "TickWidth")
                {
                    propertyValue = chartCategoryAxisProperties.TickWidth;
                }
                else if (e.PropertyName == "EnableMajorTickMarks")
                {
                    propertyValue = chartCategoryAxisProperties.EnableMajorTickMarks;
                }
                
                if (chartCategoryAxisProperties.EnableMajorTickMarks.ToLower() == "true" || chartCategoryAxisProperties.EnableMajorTickMarks.ToLower() == "auto")
                {
                    Pen pen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartCategoryAxisProperties.TickColor)), Convert.ToDouble(new RDL.DOM.Size(chartCategoryAxisProperties.TickLength).PixelValue));
                    pen.DashStyle = DashStyleFromString(chartCategoryAxisProperties.TickStyle);
                    this.InnerChart.Areas[0].PrimaryAxis.TickSize = Convert.ToDouble(Convert.ToDouble(new RDL.DOM.Size(chartCategoryAxisProperties.TickLength).PixelValue));
                    this.InnerChart.Areas[0].PrimaryAxis.TickLineStroke = pen;
                }
                else
                {
                    this.InnerChart.Areas[0].PrimaryAxis.TickSize = 0;
                }
            }
            if (e.PropertyName == "MinorTickStyle" || e.PropertyName == "MinorTickColor" || e.PropertyName == "TickWidth" || e.PropertyName == "EnableMinorTickMarks")
            {
                if (e.PropertyName == "MinorTickStyle")
                {
                    propertyValue = chartCategoryAxisProperties.MinorTickStyle;
                }
                else if (e.PropertyName == "MinorTickColor")
                {
                    propertyValue = chartCategoryAxisProperties.MinorTickColor;
                }
                else if (e.PropertyName == "MinorTickLength")
                {
                    propertyValue = chartCategoryAxisProperties.TickLength;
                }
                else if (e.PropertyName == "EnableMinorTickMarks")
                {
                    propertyValue = chartCategoryAxisProperties.EnableMinorTickMarks;
                }

                if (chartCategoryAxisProperties.EnableMinorTickMarks.ToLower() == "true")
                {
                    Pen pen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartCategoryAxisProperties.MinorTickColor)), Convert.ToDouble(new RDL.DOM.Size(chartCategoryAxisProperties.TickWidth).PixelValue));
                    pen.DashStyle = DashStyleFromString(chartCategoryAxisProperties.MinorTickStyle);
                    InnerChart.Areas[0].PrimaryAxis.SmallTickSize = Convert.ToDouble(new RDL.DOM.Size(chartCategoryAxisProperties.TickWidth).PixelValue);
                    InnerChart.Areas[0].PrimaryAxis.SmallTickLineStroke = pen;                    
                }
                else
                {
                    InnerChart.Areas[0].PrimaryAxis.SmallTickSize = 0;
                }
            }
           
            if (e.PropertyName == "Visibility")
            {
                propertyValue = chartCategoryAxisProperties.Visibility;
                if (chartCategoryAxisProperties.Visibility == "True")
                {
                    if (string.IsNullOrEmpty(this.categoryAxisTitleProperties.Name))
                    {
                        this.categoryAxisTitleProperties.Name = "Axis Title";                                            
                    }
                    this.CategoryAxisTitle.Visibility = Visibility.Visible;
                    this.RemoveChartObjectSelectionAdorner();
                    MenuItem showAxisTitle = this.CategoryAxisTitle.ContextMenu.Items[0] as MenuItem;
                    showAxisTitle.IsChecked = true;
                    this.ShowCategoryTitle.IsChecked = true;
                    this.SetTitleSelectionAdorner(CategoryAxisTitle, Editors.ChartObject.PrimaryAxisTitle);
                }
                else
                {
                    this.CategoryAxisTitle.Visibility = Visibility.Hidden;
                    this.RemoveSelectionAdorner(CategoryAxisTitle);
                    MenuItem showAxisTitle = this.CategoryAxisTitle.ContextMenu.Items[0] as MenuItem;
                    showAxisTitle.IsChecked = false;
                    this.ShowCategoryTitle.IsChecked = false;
                }
            }

            if (!this.chartCategoryAxisProperties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.chartCategoryAxisProperties;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }
        }


        void chartSecondaryCategoryAxisProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "ReverseDirection")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.ReverseDirection;
            }
            if (e.PropertyName == "LineStyle")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.LineStyle;
            }
            if (e.PropertyName == "LineColor")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.LineColor;
            }
            if (e.PropertyName == "LineWidth")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.LineWidth;
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.FontFamily;
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.FontSize;
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.FontColor;
            }
            if (e.PropertyName == "FontWeight")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.FontWeight;
            }
            if (e.PropertyName == "FontAngle")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.FontAngle;
            }
            if (e.PropertyName == "HideAxisLabels")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.HideAxisLabels;
            }
            if (e.PropertyName == "TickStyle")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.TickStyle;
            }
            if (e.PropertyName == "TickColor")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.TickColor;
            }
            if (e.PropertyName == "TickLength")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.TickLength;
            }
            if (e.PropertyName == "EnableMajorTickMarks")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.EnableMajorTickMarks;
            }
            if (e.PropertyName == "MinorTickStyle")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.MinorTickStyle;
            }
            if (e.PropertyName == "MinorTickColor")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.MinorTickColor;
            }
            if (e.PropertyName == "TickWidth")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.TickWidth;
            }
            if (e.PropertyName == "EnableMinorTickMarks")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.EnableMinorTickMarks;
            }
            
            this.propertyOldValue = propertyValue;
        }

        //Chart Secondary Category Axis Properties Changed  OnChartSecondaryCategoryAxisPropertyChanged
        protected void OnChartSecondaryCategoryAxisPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;
            if (e.PropertyName == "ReverseDirection")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.ReverseDirection;
               
                if (chartSecondaryCategoryAxisProperties.ReverseDirection.ToLower() == "true" && chartSecondaryValueAxisProperties.ReverseDirection.ToLower() == "true")
                {
                    this.secondaryXAxis.IsInversed = true;
                    this.secondaryYAxis.IsInversed = true;
                    this.secondaryXAxis.OpposedPosition = false;
                    this.secondaryYAxis.OpposedPosition = false;
                }
                else if (chartSecondaryCategoryAxisProperties.ReverseDirection.ToLower() == "false" && chartSecondaryValueAxisProperties.ReverseDirection.ToLower() == "false")
                {
                    this.secondaryXAxis.IsInversed = false;
                    this.secondaryYAxis.IsInversed = false;
                    this.secondaryXAxis.OpposedPosition = true;
                    this.secondaryYAxis.OpposedPosition = true;
                }
                else if (chartSecondaryCategoryAxisProperties.ReverseDirection.ToLower() == "true")
                {
                    this.secondaryXAxis.IsInversed = true;
                    this.secondaryYAxis.OpposedPosition = false;
                }
                else
                {
                    this.secondaryXAxis.IsInversed = false;
                    this.secondaryYAxis.OpposedPosition = true;
                }
            }
            if (e.PropertyName == "LineStyle" || e.PropertyName == "LineColor" || e.PropertyName == "LineWidth")
            {
                if (e.PropertyName == "LineStyle")
                {
                    propertyValue = chartSecondaryCategoryAxisProperties.LineStyle;
                }
                else if (e.PropertyName == "LineColor")
                {
                    propertyValue = chartSecondaryCategoryAxisProperties.LineColor;
                }
                else if (e.PropertyName == "LineWidth")
                {
                    propertyValue = chartSecondaryCategoryAxisProperties.LineWidth;
                }

                Pen pen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartSecondaryCategoryAxisProperties.LineColor)), Convert.ToDouble(new RDL.DOM.Size(chartSecondaryCategoryAxisProperties.LineWidth).PixelValue));
                CharAreaColorPrimary = (Color)ColorConverter.ConvertFromString(chartSecondaryCategoryAxisProperties.LineColor);
                pen.DashStyle = DashStyleFromString(chartSecondaryCategoryAxisProperties.LineStyle);
                this.secondaryXAxis.LineStroke = pen;
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.FontFamily;
                this.secondaryXAxis.LabelFontFamily = this.propertyValueConvertor.GetFontFamilyName(chartSecondaryCategoryAxisProperties.FontFamily);
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.FontSize;
                this.secondaryXAxis.LabelFontSize = this.propertyValueConvertor.GetFontSize(chartSecondaryCategoryAxisProperties.FontSize);
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.FontColor;
                this.secondaryXAxis.LabelForeground = this.propertyValueConvertor.GetColor(chartSecondaryCategoryAxisProperties.FontColor);
            }
            if (e.PropertyName == "FontWeight")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.FontWeight;
                this.secondaryXAxis.LabelFontWeight = this.propertyValueConvertor.GetFontWeights(chartSecondaryCategoryAxisProperties.FontWeight);
            }
            if (e.PropertyName == "FontAngle")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.FontAngle;
                if (string.IsNullOrEmpty(chartSecondaryCategoryAxisProperties.FontAngle) || (chartSecondaryCategoryAxisProperties.FontAngle != null && chartSecondaryCategoryAxisProperties.FontAngle.StartsWith("=")))
                {
                    this.secondaryXAxis.LabelRotateAngle = 0;
                }
                else
                {
                    this.secondaryXAxis.LabelRotateAngle = Convert.ToDouble(new RDL.DOM.Size(chartSecondaryCategoryAxisProperties.FontAngle).PixelValue);
                }
            }
            if (e.PropertyName == "HideAxisLabels")
            {
                propertyValue = chartSecondaryCategoryAxisProperties.HideAxisLabels;
                if (chartSecondaryCategoryAxisProperties.HideAxisLabels.ToLower() == "true")
                {
                    this.secondaryXAxis.LabelTemplate = FindResource("dataTemplate") as DataTemplate;
                }
                else
                {
                    this.secondaryXAxis.LabelTemplate = null;
                }
            }
            if (e.PropertyName == "TickStyle" || e.PropertyName == "TickColor" || e.PropertyName == "TickLength" || e.PropertyName == "EnableMajorTickMarks")
            {
                if (e.PropertyName == "TickStyle")
                {
                    propertyValue = chartSecondaryCategoryAxisProperties.TickStyle;
                }
                else if (e.PropertyName == "TickColor")
                {
                    propertyValue = chartSecondaryCategoryAxisProperties.TickColor;
                }
                else if (e.PropertyName == "TickLength")
                {
                    propertyValue = chartSecondaryCategoryAxisProperties.TickLength;
                }
                else if (e.PropertyName == "EnableMajorTickMarks")
                {
                    propertyValue = chartSecondaryCategoryAxisProperties.EnableMajorTickMarks;
                }
                if (chartSecondaryCategoryAxisProperties.EnableMajorTickMarks.ToLower() == "true" || chartSecondaryCategoryAxisProperties.EnableMajorTickMarks.ToLower() == "auto")
                {
                    Pen pen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartSecondaryCategoryAxisProperties.TickColor)), Convert.ToDouble(new RDL.DOM.Size(chartSecondaryCategoryAxisProperties.TickLength).PixelValue));
                    pen.DashStyle = DashStyleFromString(chartSecondaryCategoryAxisProperties.TickStyle);
                    this.secondaryXAxis.TickSize = Convert.ToDouble(Convert.ToDouble(new RDL.DOM.Size(chartSecondaryCategoryAxisProperties.TickLength).PixelValue));
                    this.secondaryXAxis.TickLineStroke = pen;
                }
                else
                {
                    this.secondaryXAxis.TickSize = 0;
                }
            }
            if (e.PropertyName == "MinorTickStyle" || e.PropertyName == "MinorTickColor" || e.PropertyName == "TickWidth" || e.PropertyName == "EnableMinorTickMarks")
            {
                if (e.PropertyName == "MinorTickStyle")
                {
                    propertyValue = chartSecondaryCategoryAxisProperties.MinorTickStyle;
                }
                else if (e.PropertyName == "MinorTickColor")
                {
                    propertyValue = chartSecondaryCategoryAxisProperties.MinorTickColor;
                }
                else if (e.PropertyName == "TickWidth")
                {
                    propertyValue = chartSecondaryCategoryAxisProperties.TickWidth;
                }
                else if (e.PropertyName == "EnableMinorTickMarks")
                {
                    propertyValue = chartSecondaryCategoryAxisProperties.EnableMinorTickMarks;
                }
                if (chartSecondaryCategoryAxisProperties.EnableMinorTickMarks.ToLower() == "true")
                {
                    Pen pen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartSecondaryCategoryAxisProperties.MinorTickColor)), Convert.ToDouble(new RDL.DOM.Size(chartSecondaryCategoryAxisProperties.TickWidth).PixelValue));
                    pen.DashStyle = DashStyleFromString(chartSecondaryCategoryAxisProperties.MinorTickStyle);
                    this.secondaryXAxis.SmallTickSize = Convert.ToDouble(new RDL.DOM.Size(chartSecondaryCategoryAxisProperties.TickWidth).PixelValue);
                    this.secondaryXAxis.SmallTicksPerInterval = 4;
                    this.secondaryXAxis.SmallTickLineStroke = pen;
                }
                else
                {
                    this.secondaryXAxis.SmallTickSize = 0;
                }
            }            

            if (!this.chartSecondaryCategoryAxisProperties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.chartSecondaryCategoryAxisProperties;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }
        }

        void chartValueAxisProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "ReverseDirection")
            {
                propertyValue = chartValueAxisProperties.ReverseDirection;
            }
            else if (e.PropertyName == "LineStyle")
            {
                propertyValue = chartValueAxisProperties.LineStyle;
            }
            else if (e.PropertyName == "LineColor")
            {
                propertyValue = chartValueAxisProperties.LineColor;
            }
            else if (e.PropertyName == "LineWidth")
            {
                propertyValue = chartValueAxisProperties.LineWidth;
            }
            else if (e.PropertyName == "FontFamily")
            {
                propertyValue = chartValueAxisProperties.FontFamily;
            }
            else if (e.PropertyName == "FontSize")
            {
                propertyValue = chartValueAxisProperties.FontSize;
            }
            else if (e.PropertyName == "FontColor")
            {
                propertyValue = chartValueAxisProperties.FontColor;
            }
            else if (e.PropertyName == "FontWeight")
            {
                propertyValue = chartValueAxisProperties.FontWeight;
            }
            else if (e.PropertyName == "FontAngle")
            {
                propertyValue = chartValueAxisProperties.FontAngle;
            }
            else if (e.PropertyName == "HideAxisLabels")
            {
                propertyValue = chartValueAxisProperties.HideAxisLabels;
            }
            else if (e.PropertyName == "TickStyle")
            {
                propertyValue = chartValueAxisProperties.TickStyle;
            }
            else if(e.PropertyName == "TickColor")
            {
                propertyValue=chartValueAxisProperties.TickColor;
            }
            else if (e.PropertyName == "TickLength")
            {
                propertyValue = chartValueAxisProperties.TickLength;
            }
            else if (e.PropertyName == "TickWidth")
            {
                propertyValue = chartValueAxisProperties.TickWidth;
            }
            else if (e.PropertyName == "EnableMajorTickMarks")
            {
                propertyValue = chartValueAxisProperties.EnableMajorTickMarks;
            }
            else if (e.PropertyName == "MinorTickStyle") 
            {
                propertyValue = chartValueAxisProperties.MinorTickStyle;
            }
            else if (e.PropertyName == "MinorTickColor")
            {
                propertyValue = chartValueAxisProperties.MinorTickColor;
            }
            else if (e.PropertyName == "EnableMinorTickMarks")
            {
                propertyValue = chartValueAxisProperties.EnableMinorTickMarks;
            }           
            else if (e.PropertyName == "Visibility")
            {
                propertyValue = chartValueAxisProperties.Visibility;
            }

            this.propertyOldValue = propertyValue;
        }

        //Chart Value Axis Properties Changed OnChartValueAxisPropertyChanged 
        protected void OnChartValueAxisPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "ReverseDirection")
            {
                propertyValue = chartValueAxisProperties.ReverseDirection;

                if (chartCategoryAxisProperties.ReverseDirection.ToLower() == "true" && chartValueAxisProperties.ReverseDirection.ToLower() == "true")
                {
                    this.InnerChart.Areas[0].PrimaryAxis.IsInversed = true;
                    this.InnerChart.Areas[0].SecondaryAxis.IsInversed = true;
                    this.InnerChart.Areas[0].PrimaryAxis.OpposedPosition = true;
                    this.InnerChart.Areas[0].SecondaryAxis.OpposedPosition = true;
                }
                else if (chartCategoryAxisProperties.ReverseDirection.ToLower() == "false" && chartValueAxisProperties.ReverseDirection.ToLower() == "false")
                {
                    this.InnerChart.Areas[0].PrimaryAxis.IsInversed = false;
                    this.InnerChart.Areas[0].SecondaryAxis.IsInversed = false;
                    this.InnerChart.Areas[0].PrimaryAxis.OpposedPosition = false;
                    this.InnerChart.Areas[0].SecondaryAxis.OpposedPosition = false;
                }
                else if (chartValueAxisProperties.ReverseDirection.ToLower() == "true")
                {
                    InnerChart.Areas[0].SecondaryAxis.IsInversed = true;
                    InnerChart.Areas[0].PrimaryAxis.OpposedPosition = true;
                }
                else
                {
                    InnerChart.Areas[0].SecondaryAxis.IsInversed = false;
                    InnerChart.Areas[0].PrimaryAxis.OpposedPosition = false;
                }
            }
            if (e.PropertyName == "LineStyle" || e.PropertyName == "LineColor" || e.PropertyName == "LineWidth")
            {
                if (e.PropertyName == "LineStyle")
                {
                    propertyValue = chartValueAxisProperties.LineStyle;
                }
                else if (e.PropertyName == "LineColor")
                {
                    propertyValue = chartValueAxisProperties.LineColor;
                }
                else if (e.PropertyName == "LineWidth")
                {
                    propertyValue = chartValueAxisProperties.LineWidth;
                }

                Pen pen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartValueAxisProperties.LineColor)), Convert.ToDouble(new RDL.DOM.Size(chartValueAxisProperties.LineWidth).PixelValue));
                CharAreaColorPrimary = (Color)ColorConverter.ConvertFromString(chartValueAxisProperties.LineColor);
                pen.DashStyle = DashStyleFromString(chartValueAxisProperties.LineStyle);
                this.ChartArea.SecondaryAxis.LineStroke = pen;

            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = chartValueAxisProperties.FontFamily;
                InnerChart.Areas[0].SecondaryAxis.LabelFontFamily = this.propertyValueConvertor.GetFontFamilyName(chartValueAxisProperties.FontFamily);
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = chartValueAxisProperties.FontSize;
                InnerChart.Areas[0].SecondaryAxis.LabelFontSize = this.propertyValueConvertor.GetFontSize(chartValueAxisProperties.FontSize);
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = chartValueAxisProperties.FontColor;
                InnerChart.Areas[0].SecondaryAxis.LabelForeground = this.propertyValueConvertor.GetColor(chartValueAxisProperties.FontColor);
            }
            if (e.PropertyName == "FontWeight")
            {
                propertyValue = chartValueAxisProperties.FontWeight;
                InnerChart.Areas[0].SecondaryAxis.LabelFontWeight = this.propertyValueConvertor.GetFontWeights(chartValueAxisProperties.FontWeight);
            }
            if (e.PropertyName == "FontAngle")
            {
                propertyValue = chartValueAxisProperties.FontAngle;
                if (string.IsNullOrEmpty(chartValueAxisProperties.FontAngle) || (chartValueAxisProperties.FontAngle != null && chartValueAxisProperties.FontAngle.StartsWith("=")))
                {
                    InnerChart.Areas[0].SecondaryAxis.LabelRotateAngle = 0;
                }
                else
                {
                    InnerChart.Areas[0].SecondaryAxis.LabelRotateAngle = Convert.ToDouble(new RDL.DOM.Size(chartValueAxisProperties.FontAngle).PixelValue);
                }
            }
            if (e.PropertyName == "HideAxisLabels")
            {
                propertyValue = chartValueAxisProperties.HideAxisLabels;
                if (chartValueAxisProperties.HideAxisLabels.ToLower() == "true")
                {
                    InnerChart.Areas[0].SecondaryAxis.LabelTemplate = FindResource("dataTemplate") as DataTemplate;
                }
                else
                {
                    InnerChart.Areas[0].SecondaryAxis.LabelTemplate = null;
                }
            }            
            if (e.PropertyName == "TickStyle" || e.PropertyName == "TickColor" || e.PropertyName == "TickLength" || e.PropertyName == "EnableMajorTickMarks")
            {
                if (e.PropertyName == "TickStyle")
                {
                    propertyValue = chartValueAxisProperties.TickStyle;
                }
                else if (e.PropertyName == "TickColor")
                {
                    propertyValue = chartValueAxisProperties.TickColor;
                }
                else if (e.PropertyName == "TickLength")
                {
                    propertyValue = chartValueAxisProperties.TickLength;
                }
                else if (e.PropertyName == "EnableMajorTickMarks")
                {
                    propertyValue = chartValueAxisProperties.EnableMajorTickMarks;
                }

                if (chartValueAxisProperties.EnableMajorTickMarks.ToLower() == "true" || chartValueAxisProperties.EnableMajorTickMarks.ToLower() == "auto")
                {
                    Pen pen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartValueAxisProperties.TickColor)), Convert.ToDouble(new RDL.DOM.Size(chartValueAxisProperties.TickLength).PixelValue));
                    pen.DashStyle = DashStyleFromString(chartValueAxisProperties.TickStyle);
                    this.InnerChart.Areas[0].SecondaryAxis.TickSize = Convert.ToDouble(Convert.ToDouble(new RDL.DOM.Size(chartValueAxisProperties.TickLength).PixelValue));
                    this.InnerChart.Areas[0].SecondaryAxis.TickLineStroke = pen;
                }
                else
                {
                    this.InnerChart.Areas[0].SecondaryAxis.TickSize = 0;
                }
            }
            if (e.PropertyName == "MinorTickStyle" || e.PropertyName == "MinorTickColor" || e.PropertyName == "TickWidth" || e.PropertyName == "EnableMinorTickMarks")
            {
                if (e.PropertyName == "MinorTickStyle")
                {
                    propertyValue = chartValueAxisProperties.MinorTickStyle;
                }
                else if (e.PropertyName == "MinorTickColor")
                {
                    propertyValue = chartValueAxisProperties.MinorTickColor;
                }
                else if (e.PropertyName == "TickWidth")
                {
                    propertyValue = chartValueAxisProperties.TickWidth;
                }
                else if (e.PropertyName == "EnableMinorTickMarks")
                {
                    propertyValue = chartValueAxisProperties.EnableMinorTickMarks;
                }
                if (chartValueAxisProperties.EnableMinorTickMarks.ToLower() == "true")
                {
                    Pen pen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartValueAxisProperties.MinorTickColor)), Convert.ToDouble(new RDL.DOM.Size(chartValueAxisProperties.TickWidth).PixelValue));
                    pen.DashStyle = DashStyleFromString(chartValueAxisProperties.MinorTickStyle);
                    InnerChart.Areas[0].SecondaryAxis.SmallTickSize = Convert.ToDouble(new RDL.DOM.Size(chartValueAxisProperties.TickWidth).PixelValue);
                    InnerChart.Areas[0].SecondaryAxis.SmallTickLineStroke = pen;
                }
                else
                {
                    InnerChart.Areas[0].SecondaryAxis.SmallTickSize = 0;
                }
            }           

            if (e.PropertyName == "Visibility")
            {
                propertyValue = chartValueAxisProperties.Visibility;
                if (chartValueAxisProperties.Visibility == "True")
                {
                    if(string.IsNullOrEmpty(this.valueAxisTitleProperties.Name))
                    {
                        this.valueAxisTitleProperties.Name = "Axis Title";                       
                    }
                    this.ValueAxisTitle.Visibility = Visibility.Visible;
                    this.RemoveChartObjectSelectionAdorner();
                    MenuItem showAxisTitle = this.ValueAxisTitle.ContextMenu.Items[0] as MenuItem;
                    showAxisTitle.IsChecked = true;
                    this.ShowValueTitle.IsChecked = true;
                    this.SetTitleSelectionAdorner(ValueAxisTitle, Editors.ChartObject.SecondaryAxisTile);
                }
                else
                {
                    this.ValueAxisTitle.Visibility = Visibility.Hidden;
                    this.RemoveSelectionAdorner(ValueAxisTitle);
                    MenuItem showAxisTitle = this.ValueAxisTitle.ContextMenu.Items[0] as MenuItem;
                    showAxisTitle.IsChecked = false;
                    this.ShowValueTitle.IsChecked = false;
                }
            }

            if (!this.chartValueAxisProperties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.chartValueAxisProperties;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }
        }

        void chartSecondaryValueAxisProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "ReverseDirection")
            {
                propertyValue = chartSecondaryValueAxisProperties.ReverseDirection;
                if (chartSecondaryValueAxisProperties.ReverseDirection.ToLower() == "true")
                {
                    this.secondaryYAxis.IsInversed = true;
                    this.secondaryXAxis.OpposedPosition = true;
                }
                else
                {
                    this.secondaryYAxis.IsInversed = false;
                    this.secondaryXAxis.OpposedPosition = false;
                }
            }
            if (e.PropertyName == "LineStyle" || e.PropertyName == "LineColor" || e.PropertyName == "LineWidth")
            {
                if (e.PropertyName == "LineStyle")
                {
                    propertyValue = chartSecondaryValueAxisProperties.LineStyle;
                }
                else if (e.PropertyName == "LineColor")
                {
                    propertyValue = chartSecondaryValueAxisProperties.LineColor;
                }
                else if (e.PropertyName == "LineWidth")
                {
                    propertyValue = chartSecondaryValueAxisProperties.LineWidth;
                }
                Pen pen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartSecondaryValueAxisProperties.LineColor)), Convert.ToDouble(new RDL.DOM.Size(chartSecondaryValueAxisProperties.LineWidth).PixelValue));
                CharAreaColorPrimary = (Color)ColorConverter.ConvertFromString(chartSecondaryValueAxisProperties.LineColor);
                pen.DashStyle = DashStyleFromString(chartSecondaryValueAxisProperties.LineStyle);
                this.secondaryYAxis.LineStroke = pen;
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = chartSecondaryValueAxisProperties.FontFamily;
                this.secondaryYAxis.LabelFontFamily = this.propertyValueConvertor.GetFontFamilyName(chartSecondaryValueAxisProperties.FontFamily);
            }
            if (e.PropertyName == "FontSize")
            {
                propertyValue = chartSecondaryValueAxisProperties.FontSize;
                this.secondaryYAxis.LabelFontSize = this.propertyValueConvertor.GetFontSize(chartSecondaryValueAxisProperties.FontSize);
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = chartSecondaryValueAxisProperties.FontSize;
                this.secondaryYAxis.LabelForeground = this.propertyValueConvertor.GetColor(chartSecondaryValueAxisProperties.FontColor);
            }
            if (e.PropertyName == "FontWeight")
            {
                propertyValue = chartSecondaryValueAxisProperties.FontWeight;
                this.secondaryYAxis.LabelFontWeight = this.propertyValueConvertor.GetFontWeights(chartSecondaryValueAxisProperties.FontWeight);                
            }
            if (e.PropertyName == "FontAngle")
            {
                propertyValue = chartSecondaryValueAxisProperties.FontAngle;
                if (string.IsNullOrEmpty(chartSecondaryValueAxisProperties.FontAngle) || (chartSecondaryValueAxisProperties.FontAngle != null && chartSecondaryValueAxisProperties.FontAngle.StartsWith("=")))
                {
                    this.secondaryYAxis.LabelRotateAngle = 0;
                }
                else
                {
                    this.secondaryYAxis.LabelRotateAngle = Convert.ToDouble(new RDL.DOM.Size(chartSecondaryValueAxisProperties.FontAngle).PixelValue);
                }
            }
            if (e.PropertyName == "HideAxisLabels")
            {
                propertyValue = chartSecondaryValueAxisProperties.HideAxisLabels;
                if (chartSecondaryValueAxisProperties.HideAxisLabels.ToLower() == "true")
                {
                    this.secondaryYAxis.LabelTemplate = FindResource("dataTemplate") as DataTemplate;
                }
                else
                {
                    this.secondaryYAxis.LabelTemplate = null;
                }
            }            
            if (e.PropertyName == "TickStyle" || e.PropertyName == "TickColor" || e.PropertyName == "TickLength" || e.PropertyName == "EnableMajorTickMarks")
            {
                if (e.PropertyName == "TickStyle")
                {
                    propertyValue = chartSecondaryValueAxisProperties.TickStyle;
                }
                else if (e.PropertyName == "TickColor")
                {
                    propertyValue = chartSecondaryValueAxisProperties.TickColor;
                }
                else if (e.PropertyName == "TickLength")
                {
                    propertyValue = chartSecondaryValueAxisProperties.TickLength;
                }
                if (e.PropertyName == "EnableMajorTickMarks")
                {
                    propertyValue = chartSecondaryValueAxisProperties.EnableMajorTickMarks;
                }
                if (chartSecondaryValueAxisProperties.EnableMajorTickMarks.ToLower() == "true" || chartSecondaryValueAxisProperties.EnableMajorTickMarks.ToLower() == "auto")
                {
                    Pen pen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartSecondaryValueAxisProperties.TickColor)), Convert.ToDouble(new RDL.DOM.Size(chartSecondaryValueAxisProperties.TickLength).PixelValue));
                    pen.DashStyle = DashStyleFromString(chartSecondaryValueAxisProperties.TickStyle);
                    this.secondaryYAxis.TickSize = Convert.ToDouble(Convert.ToDouble(new RDL.DOM.Size(chartSecondaryValueAxisProperties.TickLength).PixelValue));
                    this.secondaryYAxis.TickLineStroke = pen;
                }
                else
                {
                    this.secondaryYAxis.TickSize = 0;
                }
            }
            if (e.PropertyName == "MinorTickStyle" || e.PropertyName == "MinorTickColor" || e.PropertyName == "TickWidth" || e.PropertyName == "EnableMinorTickMarks")
            {
                if (e.PropertyName == "MinorTickStyle")
                {
                    propertyValue = chartSecondaryValueAxisProperties.MinorTickStyle; ;
                }
                else if (e.PropertyName == "MinorTickColor")
                {
                    propertyValue = chartSecondaryValueAxisProperties.MinorTickColor;
                }
                else if (e.PropertyName == "TickWidth")
                {
                    propertyValue = chartSecondaryValueAxisProperties.TickWidth;
                }
                else if (e.PropertyName == "EnableMinorTickMarks")
                {
                    propertyValue = chartSecondaryValueAxisProperties.EnableMinorTickMarks;
                }
                if (chartSecondaryValueAxisProperties.EnableMinorTickMarks.ToLower() == "true")
                {
                    Pen pen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartSecondaryValueAxisProperties.MinorTickColor)), Convert.ToDouble(new RDL.DOM.Size(chartSecondaryValueAxisProperties.TickWidth).PixelValue));
                    pen.DashStyle = DashStyleFromString(chartSecondaryValueAxisProperties.MinorTickStyle);
                    this.secondaryYAxis.SmallTickSize = Convert.ToDouble(new RDL.DOM.Size(chartSecondaryValueAxisProperties.TickWidth).PixelValue);
                    this.secondaryYAxis.SmallTickLineStroke = pen;
                }
                else
                {
                    this.secondaryYAxis.SmallTickSize = 0;
                }
            }
           
        }

        //Chart Value Axis Properties Changed OnChartSecondaryValueAxisPropertyChanged
        protected void OnChartSecondaryValueAxisPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "LineStyle" || e.PropertyName == "LineColor" || e.PropertyName == "LineWidth")
            {
                if (e.PropertyName == "LineStyle")
                {
                    propertyValue = chartSecondaryValueAxisProperties.LineStyle;
                }
                else if (e.PropertyName == "LineColor")
                {
                    propertyValue = chartSecondaryValueAxisProperties.LineColor;
                }
                else if (e.PropertyName == "LineWidth")
                {
                    propertyValue = chartSecondaryValueAxisProperties.LineWidth;
                }

                Pen pen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartSecondaryValueAxisProperties.LineColor)), Convert.ToDouble(new RDL.DOM.Size(chartSecondaryValueAxisProperties.LineWidth).PixelValue));
                CharAreaColorPrimary = (Color)ColorConverter.ConvertFromString(chartSecondaryValueAxisProperties.LineColor);
                pen.DashStyle = DashStyleFromString(chartSecondaryValueAxisProperties.LineStyle);
                this.secondaryYAxis.LineStroke = pen;
            }
            if (e.PropertyName == "ReverseDirection")
            {
                if (chartSecondaryValueAxisProperties.ReverseDirection.ToLower() == "true" && chartSecondaryCategoryAxisProperties.ReverseDirection.ToLower() == "true")
                {
                    this.secondaryXAxis.IsInversed = true;
                    this.secondaryYAxis.IsInversed = true;
                    this.secondaryXAxis.OpposedPosition = false;
                    this.secondaryYAxis.OpposedPosition = false;
                }
                else if (chartSecondaryValueAxisProperties.ReverseDirection.ToLower() == "false" && chartSecondaryCategoryAxisProperties.ReverseDirection.ToLower() == "false")
                {
                    this.secondaryXAxis.IsInversed = false;
                    this.secondaryYAxis.IsInversed = false;
                    this.secondaryXAxis.OpposedPosition = true;
                    this.secondaryYAxis.OpposedPosition = true;
                }
                else if (chartSecondaryValueAxisProperties.ReverseDirection.ToLower() == "true")
                {
                    this.secondaryYAxis.IsInversed = true;
                    this.secondaryXAxis.OpposedPosition = false;
                }
                else
                {
                    this.secondaryYAxis.IsInversed = false;
                    this.secondaryXAxis.OpposedPosition = true;
                }
            }
            if (e.PropertyName == "FontFamily")
            {
                propertyValue = chartSecondaryValueAxisProperties.FontFamily;
                this.secondaryYAxis.LabelFontFamily = this.propertyValueConvertor.GetFontFamilyName(chartSecondaryValueAxisProperties.FontFamily);
            }
            if (e.PropertyName == "FontSize")
            {
                this.secondaryYAxis.LabelFontSize = this.propertyValueConvertor.GetFontSize(chartSecondaryValueAxisProperties.FontSize);
            }
            if (e.PropertyName == "FontColor")
            {
                propertyValue = chartSecondaryValueAxisProperties.FontSize;
                this.secondaryYAxis.LabelForeground = this.propertyValueConvertor.GetColor(chartSecondaryValueAxisProperties.FontColor);
            }
            if (e.PropertyName == "FontWeight")
            {
                propertyValue = chartSecondaryValueAxisProperties.FontWeight;
                this.secondaryYAxis.LabelFontWeight = this.propertyValueConvertor.GetFontWeights(chartSecondaryValueAxisProperties.FontWeight);
            }
            if (e.PropertyName == "FontAngle")
            {
                propertyValue = chartSecondaryValueAxisProperties.FontAngle;
                if (string.IsNullOrEmpty(chartSecondaryValueAxisProperties.FontAngle) || (chartSecondaryValueAxisProperties.FontAngle != null && chartSecondaryValueAxisProperties.FontAngle.StartsWith("=")))
                {
                    this.secondaryYAxis.LabelRotateAngle = 0;
                }
                else
                {
                    this.secondaryYAxis.LabelRotateAngle = Convert.ToDouble(new RDL.DOM.Size(chartSecondaryValueAxisProperties.FontAngle).PixelValue);
                }
            }
            if (e.PropertyName == "HideAxisLabels")
            {
                propertyValue = chartSecondaryValueAxisProperties.HideAxisLabels;
                if (chartSecondaryValueAxisProperties.HideAxisLabels.ToLower() == "true")
                {
                    this.secondaryYAxis.LabelTemplate = FindResource("dataTemplate") as DataTemplate;
                }
                else
                {
                    this.secondaryYAxis.LabelTemplate = null;
                }
            }     
            
            if (e.PropertyName == "TickStyle" || e.PropertyName == "TickColor" || e.PropertyName == "TickLength" || e.PropertyName == "EnableMajorTickMarks")
            {
                if (e.PropertyName == "TickStyle")
                {
                    propertyValue = chartSecondaryValueAxisProperties.TickStyle;
                }
                else if (e.PropertyName == "TickColor")
                {
                    propertyValue = chartSecondaryValueAxisProperties.TickColor;
                }
                else if (e.PropertyName == "TickLength")
                {
                    propertyValue = chartSecondaryValueAxisProperties.TickLength;
                }
                if (e.PropertyName == "EnableMajorTickMarks")
                {
                    propertyValue = chartSecondaryValueAxisProperties.EnableMajorTickMarks;
                }

                if (chartSecondaryValueAxisProperties.EnableMajorTickMarks.ToLower() == "true" || chartSecondaryValueAxisProperties.EnableMajorTickMarks.ToLower() == "auto")
                {
                    Pen pen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartSecondaryValueAxisProperties.TickColor)), Convert.ToDouble(new RDL.DOM.Size(chartSecondaryValueAxisProperties.TickLength).PixelValue));
                    pen.DashStyle = DashStyleFromString(chartSecondaryValueAxisProperties.TickStyle);
                    this.secondaryYAxis.TickSize = Convert.ToDouble(Convert.ToDouble(new RDL.DOM.Size(chartSecondaryValueAxisProperties.TickLength).PixelValue));
                    this.secondaryYAxis.TickLineStroke = pen;
                }
                else
                {
                    this.secondaryYAxis.TickSize = 0;
                }
            }
            if (e.PropertyName == "MinorTickStyle" || e.PropertyName == "MinorTickColor" || e.PropertyName == "TickWidth" || e.PropertyName == "EnableMinorTickMarks")
            {
                if (e.PropertyName == "MinorTickStyle")
                {
                    propertyValue = chartSecondaryValueAxisProperties.MinorTickStyle; ;
                }
                else if (e.PropertyName == "MinorTickColor")
                {
                    propertyValue = chartSecondaryValueAxisProperties.MinorTickColor;
                }
                else if (e.PropertyName == "TickWidth")
                {
                    propertyValue = chartSecondaryValueAxisProperties.TickWidth;
                }
                else if (e.PropertyName == "EnableMinorTickMarks")
                {
                    propertyValue = chartSecondaryValueAxisProperties.EnableMinorTickMarks;
                }

                if (chartSecondaryValueAxisProperties.EnableMinorTickMarks.ToLower() == "true")
                {
                    Pen pen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartSecondaryValueAxisProperties.MinorTickColor)), Convert.ToDouble(new RDL.DOM.Size(chartSecondaryValueAxisProperties.TickWidth).PixelValue));
                    pen.DashStyle = DashStyleFromString(chartSecondaryValueAxisProperties.MinorTickStyle);
                    this.secondaryYAxis.SmallTickSize = Convert.ToDouble(new RDL.DOM.Size(chartSecondaryValueAxisProperties.TickWidth).PixelValue);
                    this.secondaryYAxis.SmallTicksPerInterval = 4;
                    this.secondaryYAxis.SmallTickLineStroke = pen;
                }
                else
                {
                    this.secondaryYAxis.SmallTickSize = 0;
                }
            }          

            if (!this.chartSecondaryValueAxisProperties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.chartSecondaryValueAxisProperties;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }
        }


        void defaultChartProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "FillStyle")
            {
                propertyValue = defaultChartProperties.FillStyle;
            }
            else if (e.PropertyName == "GradientStyle")
            {
                propertyValue = defaultChartProperties.GradientStyle;
            }
            else if (e.PropertyName == "PrimaryColor")
            {
                propertyValue = defaultChartProperties.PrimaryColor;
            }
            else if (e.PropertyName == "SecondaryColor")
            {
                propertyValue = defaultChartProperties.SecondaryColor;
            }

            this.propertyOldValue = propertyValue;
        }

        //Default Chart Property Changed
        protected void OnDefaultChartPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;
            if (e.PropertyName == "FillStyle" || e.PropertyName == "GradientStyle" || e.PropertyName == "PrimaryColor" || e.PropertyName == "SecondaryColor")
            {
                if (e.PropertyName == "FillStyle")
                {
                    propertyValue = defaultChartProperties.FillStyle;
                }
                else if (e.PropertyName == "GradientStyle")
                {
                    propertyValue = defaultChartProperties.GradientStyle;
                }
                else if (e.PropertyName == "PrimaryColor")
                {
                    propertyValue = defaultChartProperties.PrimaryColor;
                }
                else if (e.PropertyName == "SecondaryColor")
                {
                    propertyValue = defaultChartProperties.SecondaryColor;
                }

                switch (defaultChartProperties.FillStyle.ToLower())
                {
                    case "solid":
                        this.InnerChart.Areas[0].GridBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(defaultChartProperties.PrimaryColor));
                        this.defaultChartProperties.GradientStyle = "None";
                        break;
                    default:
                        Color color1 = (Color)ColorConverter.ConvertFromString(defaultChartProperties.PrimaryColor);
                        Color color2 = (Color)ColorConverter.ConvertFromString(defaultChartProperties.SecondaryColor);
                        switch (defaultChartProperties.GradientStyle.ToLower())
                        {
                            case "leftright":
                                {
                                    this.InnerChart.Areas[0].GridBackground = new LinearGradientBrush(color1, color2, 0);
                                    break;
                                }
                            case "topbottom":
                                {
                                    this.InnerChart.Areas[0].GridBackground = new LinearGradientBrush(color1, color2, 90);
                                    break;
                                }
                            default:
                                {
                                    this.InnerChart.Areas[0].GridBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(defaultChartProperties.PrimaryColor));
                                    break;
                                }
                        }
                        break;
                }
            }
            if (!this.defaultChartProperties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.defaultChartProperties;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }
        }

        void CategoryAxisMajorGridLines_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "EnableMajorGridLines")
            {
                propertyValue = defaultChartProperties.CategoryAxisMajorGridLines.EnableMajorGridLines;
            }

            else if (e.PropertyName == "MajorGridLinesStyle")
            {
                propertyValue = defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesStyle;
            }
            else if (e.PropertyName == "MajorGridLinesWidth")
            {
                propertyValue = defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesWidth;
            }
            else if (e.PropertyName == "MajorGridLinesColor")
            {
                propertyValue = defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesColor;
            }

            this.propertyOldValue = propertyValue;
        }

        protected void OnCategoryAxisMajorGridLinesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "EnableMajorGridLines" || e.PropertyName == "MajorGridLinesStyle" || e.PropertyName == "MajorGridLinesWidth" || e.PropertyName == "MajorGridLinesColor")
            {
                if (e.PropertyName == "EnableMajorGridLines")
                {
                    propertyValue = defaultChartProperties.CategoryAxisMajorGridLines.EnableMajorGridLines;
                }

                else if(e.PropertyName == "MajorGridLinesStyle")
                {
                    propertyValue = defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesStyle;
                }
                else if(e.PropertyName == "MajorGridLinesWidth")
                {
                    propertyValue = defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesWidth;
                }
                else if (e.PropertyName == "MajorGridLinesColor")
                {
                    propertyValue = defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesColor;
                }

                if (defaultChartProperties.CategoryAxisMajorGridLines.EnableMajorGridLines.ToLower() == "false" && defaultChartProperties.CategoryAxisMinorGridLines.EnableMinorGridLines.ToLower() == "false")
                {
                    ChartArea.SetShowGridLines(ChartArea.PrimaryAxis, false);
                }
                else
                {
                    switch (defaultChartProperties.CategoryAxisMajorGridLines.EnableMajorGridLines.ToLower())
                    {
                        case "false":
                            Pen pen2 = new Pen(Brushes.Transparent, 1);
                            pen2.DashStyle = DashStyles.Solid;
                            ChartArea.SetGridLineStroke(this.ChartArea.PrimaryAxis, pen2);
                            break;
                        default:
                            ChartArea.SetShowGridLines(ChartArea.PrimaryAxis, true);
                            Pen pen1 = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesColor)), Convert.ToDouble(new RDL.DOM.Size(defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesWidth).PixelValue));
                            pen1.DashStyle = DashStyleFromString(defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesStyle);
                            ChartArea.SetGridLineStroke(this.ChartArea.PrimaryAxis, pen1);
                            break;
                    }
                    switch (defaultChartProperties.CategoryAxisMinorGridLines.EnableMinorGridLines.ToLower())
                    {
                        case "true":
                            ChartArea.SetShowGridLines(ChartArea.PrimaryAxis, true);
                            Pen pen1 = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesColor)), Convert.ToDouble(new RDL.DOM.Size(defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesWidth).PixelValue));
                            pen1.DashStyle = DashStyleFromString(defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesStyle);
                            ChartArea.SetSmallGridLineStroke(this.ChartArea.PrimaryAxis, pen1);
                            ChartArea.PrimaryAxis.SmallTickLineStroke = pen1;
                            break;
                        default:
                            Pen pen2 = new Pen(Brushes.Transparent, 1);
                            pen2.DashStyle = DashStyles.Solid;
                            ChartArea.SetSmallGridLineStroke(this.ChartArea.PrimaryAxis, pen2);
                            break;
                    }
                }
            }

            if (!this.defaultChartProperties.CategoryAxisMajorGridLines.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.defaultChartProperties.CategoryAxisMajorGridLines;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }
        }

        void CategoryAxisMinorGridLines_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "EnableMinorGridLines")
            {
                propertyValue = defaultChartProperties.CategoryAxisMinorGridLines.EnableMinorGridLines;
            }
            else if (e.PropertyName == "MinorGridLinesStyle")
            {
                propertyValue = defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesStyle;
            }
            else if (e.PropertyName == "MinorGridLinesWidth")
            {
                propertyValue = defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesWidth;
            }
            else if (e.PropertyName == "MinorGridLinesColor")
            {
                propertyValue = defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesColor;
            }

            this.propertyOldValue = propertyValue;
        }

        protected void OnCategoryAxisMinorGridLinesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "EnableMinorGridLines" || e.PropertyName == "MinorGridLinesStyle" || e.PropertyName == "MinorGridLinesWidth" || e.PropertyName == "MinorGridLinesColor")
            {
                if (e.PropertyName == "EnableMinorGridLines")
                {
                    propertyValue = defaultChartProperties.CategoryAxisMinorGridLines.EnableMinorGridLines;
                }
                else if (e.PropertyName == "MinorGridLinesStyle")
                {
                    propertyValue = defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesStyle;
                }
                else if (e.PropertyName == "MinorGridLinesWidth")
                {
                    propertyValue = defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesWidth;
                }
                else if (e.PropertyName == "MinorGridLinesColor")
                {
                    propertyValue = defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesColor;
                }

                if (defaultChartProperties.CategoryAxisMajorGridLines.EnableMajorGridLines.ToLower() == "false" && defaultChartProperties.CategoryAxisMinorGridLines.EnableMinorGridLines.ToLower() == "false")
                {
                    ChartArea.SetShowGridLines(ChartArea.PrimaryAxis, false);
                }
                else
                {
                    switch (defaultChartProperties.CategoryAxisMinorGridLines.EnableMinorGridLines.ToLower())
                    {
                        case "true":
                            ChartArea.SetShowGridLines(ChartArea.PrimaryAxis, true);
                            Pen pen1 = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesColor)), Convert.ToDouble(new RDL.DOM.Size(defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesWidth).PixelValue));
                            pen1.DashStyle = DashStyleFromString(defaultChartProperties.CategoryAxisMinorGridLines.MinorGridLinesStyle);
                            ChartArea.SetSmallGridLineStroke(this.ChartArea.PrimaryAxis, pen1);
                            ChartArea.PrimaryAxis.SmallTickLineStroke = pen1;
                            break;
                        default:
                            Pen pen2 = new Pen(Brushes.Transparent, 1);
                            pen2.DashStyle = DashStyles.Solid;
                            ChartArea.SetSmallGridLineStroke(this.ChartArea.PrimaryAxis, pen2);
                            break;
                    }
                    switch (defaultChartProperties.CategoryAxisMajorGridLines.EnableMajorGridLines.ToLower())
                    {
                        case "false":
                            Pen pen2 = new Pen(Brushes.Transparent, 1);
                            pen2.DashStyle = DashStyles.Solid;
                            ChartArea.SetGridLineStroke(this.ChartArea.PrimaryAxis, pen2);
                            break;
                        default:
                            ChartArea.SetShowGridLines(ChartArea.PrimaryAxis, true);
                            Pen pen1 = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesColor)), Convert.ToDouble(new RDL.DOM.Size(defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesWidth).PixelValue));
                            pen1.DashStyle = DashStyleFromString(defaultChartProperties.CategoryAxisMajorGridLines.MajorGridLinesStyle);
                            ChartArea.SetGridLineStroke(this.ChartArea.PrimaryAxis, pen1);
                            break;
                    }
                }
            }

            if (!this.defaultChartProperties.CategoryAxisMinorGridLines.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.defaultChartProperties.CategoryAxisMinorGridLines;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }

        }

        void ValueAxisMajorGridLines_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "EnableMajorGridLines")
            {
                propertyValue = defaultChartProperties.ValueAxisMajorGridLines.EnableMajorGridLines;
            }
            else if (e.PropertyName == "MajorGridLinesStyle")
            {
                propertyValue = defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesStyle;
            }
            else if (e.PropertyName == "MajorGridLinesWidth")
            {
                propertyValue = defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesWidth;
            }
            else if (e.PropertyName == "MajorGridLinesColor")
            {
                propertyValue = defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesColor;
            }

            this.propertyOldValue = propertyValue;
        }

        protected void OnValueAxisMajorGridLinesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;
            if (e.PropertyName == "EnableMajorGridLines" || e.PropertyName == "MajorGridLinesStyle" || e.PropertyName == "MajorGridLinesWidth" || e.PropertyName == "MajorGridLinesColor")
            {
                if (e.PropertyName == "EnableMajorGridLines")
                {
                    propertyValue =defaultChartProperties.ValueAxisMajorGridLines.EnableMajorGridLines;
                }
                else if (e.PropertyName == "MajorGridLinesStyle")
                {
                    propertyValue = defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesStyle;
                }
                else if (e.PropertyName == "MajorGridLinesWidth")
                {
                    propertyValue = defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesWidth;
                }
                else if (e.PropertyName == "MajorGridLinesColor")
                {
                     propertyValue = defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesColor;
                }

                if (defaultChartProperties.ValueAxisMajorGridLines.EnableMajorGridLines.ToLower() == "false" && defaultChartProperties.ValueAxisMinorGridLines.EnableMinorGridLines.ToLower() == "false")
                {
                    ChartArea.SetShowGridLines(ChartArea.SecondaryAxis, false);
                }
                else
                {
                    switch (defaultChartProperties.ValueAxisMajorGridLines.EnableMajorGridLines.ToLower())
                    {
                        case "false":
                            Pen pen2 = new Pen(Brushes.Transparent, 1);
                            pen2.DashStyle = DashStyles.Solid;
                            ChartArea.SetGridLineStroke(this.ChartArea.SecondaryAxis, pen2);
                            break;
                        default:
                            ChartArea.SetShowGridLines(ChartArea.SecondaryAxis, true);
                            Pen pen1 = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesColor)), Convert.ToDouble(new RDL.DOM.Size(defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesWidth).PixelValue));
                            pen1.DashStyle = DashStyleFromString(defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesStyle);
                            ChartArea.SetGridLineStroke(this.ChartArea.SecondaryAxis, pen1);
                            break;
                    }
                    switch (defaultChartProperties.ValueAxisMinorGridLines.EnableMinorGridLines.ToLower())
                    {
                        case "true":
                            ChartArea.SetShowGridLines(ChartArea.SecondaryAxis, true);
                            Pen pen1 = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesColor)), Convert.ToDouble(new RDL.DOM.Size(defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesWidth).PixelValue));
                            pen1.DashStyle = DashStyleFromString(defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesStyle);
                            ChartArea.SetSmallGridLineStroke(this.ChartArea.SecondaryAxis, pen1);
                            ChartArea.SecondaryAxis.SmallTickLineStroke = pen1;
                            break;
                        default:
                            Pen pen2 = new Pen(Brushes.Transparent, 1);
                            pen2.DashStyle = DashStyles.Solid;
                            ChartArea.SetSmallGridLineStroke(this.ChartArea.SecondaryAxis, pen2);
                            break;
                    }           
                }
                
            }

            if (!this.defaultChartProperties.ValueAxisMajorGridLines.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.defaultChartProperties.ValueAxisMajorGridLines;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }

        }

        void ValueAxisMinorGridLines_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "EnableMinorGridLines")
            {
                propertyValue = defaultChartProperties.ValueAxisMinorGridLines.EnableMinorGridLines;
            }
            else if (e.PropertyName == "MinorGridLinesStyle")
            {
                propertyValue = defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesStyle;
            }
            else if (e.PropertyName == "MinorGridLinesWidth")
            {
                propertyValue = defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesWidth;
            }
            else if (e.PropertyName == "MinorGridLinesColor")
            {
                propertyValue = defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesColor;
            }

            this.propertyOldValue = propertyValue;

        }

        protected void OnValueAxisMinorGridLinesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;

            if (e.PropertyName == "EnableMinorGridLines" || e.PropertyName == "MinorGridLinesStyle" || e.PropertyName == "MinorGridLinesWidth" || e.PropertyName == "MinorGridLinesColor")
            {

                if (e.PropertyName == "EnableMinorGridLines")
                {
                    propertyValue = defaultChartProperties.ValueAxisMinorGridLines.EnableMinorGridLines;
                }
                else if (e.PropertyName == "MinorGridLinesStyle")
                {
                    propertyValue = defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesStyle;
                }
                else if (e.PropertyName == "MinorGridLinesWidth")
                {
                    propertyValue = defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesWidth;
                }
                else if (e.PropertyName == "MinorGridLinesColor")
                {
                    propertyValue = defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesColor;
                }

                if (defaultChartProperties.ValueAxisMajorGridLines.EnableMajorGridLines.ToLower() == "false" && defaultChartProperties.ValueAxisMinorGridLines.EnableMinorGridLines.ToLower() == "false")
                {
                    ChartArea.SetShowGridLines(ChartArea.SecondaryAxis, false);
                }
                else
                {
                    switch (defaultChartProperties.ValueAxisMinorGridLines.EnableMinorGridLines.ToLower())
                    {
                        case "true":
                            ChartArea.SetShowGridLines(ChartArea.SecondaryAxis, true);
                            Pen pen1 = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesColor)), Convert.ToDouble(new RDL.DOM.Size(defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesWidth).PixelValue));
                            pen1.DashStyle = DashStyleFromString(defaultChartProperties.ValueAxisMinorGridLines.MinorGridLinesStyle);
                            ChartArea.SetSmallGridLineStroke(this.ChartArea.SecondaryAxis, pen1);
                            ChartArea.SecondaryAxis.SmallTickLineStroke = pen1;
                            break;
                        default:
                            Pen pen2 = new Pen(Brushes.Transparent, 1);
                            pen2.DashStyle = DashStyles.Solid;
                            ChartArea.SetSmallGridLineStroke(this.ChartArea.SecondaryAxis, pen2);
                            break;
                    }
                    switch (defaultChartProperties.ValueAxisMajorGridLines.EnableMajorGridLines.ToLower())
                    {
                        case "false":
                            Pen pen2 = new Pen(Brushes.Transparent, 1);
                            pen2.DashStyle = DashStyles.Solid;
                            ChartArea.SetGridLineStroke(this.ChartArea.SecondaryAxis, pen2);
                            break;
                        default:
                            ChartArea.SetShowGridLines(ChartArea.SecondaryAxis, true);
                            Pen pen1 = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesColor)), Convert.ToDouble(new RDL.DOM.Size(defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesWidth).PixelValue));
                            pen1.DashStyle = DashStyleFromString(defaultChartProperties.ValueAxisMajorGridLines.MajorGridLinesStyle);
                            ChartArea.SetGridLineStroke(this.ChartArea.SecondaryAxis, pen1);
                            break;
                    }
                }               
            }

            if (!this.defaultChartProperties.ValueAxisMinorGridLines.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.defaultChartProperties.ValueAxisMinorGridLines;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }

        }

        private void SetDataLabelsPosition(ChartSeries series, string position)
        {
            if (series.Type == ChartTypes.Pie || series.Type == ChartTypes.Doughnut || series.Type == ChartTypes.Pyramid || series.Type == ChartTypes.Funnel)
            {
                if (position.ToLower() == "outside")
                {
                    series.AdornmentsInfo.SegmentIsOut = true;
                    if (series.Type == ChartTypes.Pyramid || series.Type == ChartTypes.Funnel)
                    {
                        series.AdornmentsInfo.OffsetX = 150;
                    }
                    else
                    {
                        series.AdornmentsInfo.OffsetX = 100;
                        series.AdornmentsInfo.OffsetY = 100;
                    }
                }
                else
                {
                    series.AdornmentsInfo.SegmentIsOut = false;
                    series.AdornmentsInfo.SymbolTemplate = null;
                    series.AdornmentsInfo.OffsetX = 0;
                }
            }
            else
            {
                series.AdornmentsInfo.OffsetX = 0;
                series.AdornmentsInfo.SegmentShowLine = false;
                switch (position.ToLower())
                {
                    case "default":
                        series.AdornmentsInfo.VerticalAlignment = VerticalAlignment.Top;
                        series.AdornmentsInfo.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case "top":
                        series.AdornmentsInfo.VerticalAlignment = VerticalAlignment.Top;
                        series.AdornmentsInfo.HorizontalAlignment = HorizontalAlignment.Stretch;
                        break;
                    case "bottom":
                        series.AdornmentsInfo.VerticalAlignment = VerticalAlignment.Bottom;
                        series.AdornmentsInfo.HorizontalAlignment = HorizontalAlignment.Stretch;
                        break;
                    case "left":
                        series.AdornmentsInfo.HorizontalAlignment = HorizontalAlignment.Left;
                        series.AdornmentsInfo.VerticalAlignment = VerticalAlignment.Stretch;
                        break;
                    case "right":
                        series.AdornmentsInfo.HorizontalAlignment = HorizontalAlignment.Right;
                        series.AdornmentsInfo.VerticalAlignment = VerticalAlignment.Stretch;
                        break;
                    case "center":
                        series.AdornmentsInfo.HorizontalAlignment = HorizontalAlignment.Center;
                        series.AdornmentsInfo.VerticalAlignment = VerticalAlignment.Center;
                        break;
                    case "topleft":
                        series.AdornmentsInfo.HorizontalAlignment = HorizontalAlignment.Left;
                        series.AdornmentsInfo.VerticalAlignment = VerticalAlignment.Top;
                        break;
                    case "topright":
                        series.AdornmentsInfo.HorizontalAlignment = HorizontalAlignment.Right;
                        series.AdornmentsInfo.VerticalAlignment = VerticalAlignment.Top;
                        break;
                    case "bottomleft":
                        series.AdornmentsInfo.HorizontalAlignment = HorizontalAlignment.Left;
                        series.AdornmentsInfo.VerticalAlignment = VerticalAlignment.Bottom;
                        break;
                    case "bottomright":
                        series.AdornmentsInfo.HorizontalAlignment = HorizontalAlignment.Right;
                        series.AdornmentsInfo.VerticalAlignment = VerticalAlignment.Bottom;
                        break;
                    default:
                        series.AdornmentsInfo.VerticalAlignment = VerticalAlignment.Top;
                        series.AdornmentsInfo.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                }
            }
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
    where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void RemoveChartObjectSelectionAdorner()
        {
            foreach (ChartSeries series in this.InnerChart.Areas[0].Series)
            {
                if (series.AdornmentsInfo.LabelTemplate != null && series.AdornmentsInfo.SymbolTemplate != null)
                {
                    if (this.chartSeriesPropertiesCollection[seriesCount].ShowDataLabels.ToLower() == "false")
                    {
                        series.AdornmentsInfo.LabelTemplate = FindResource("dataTemplate") as DataTemplate;
                    }
                    else
                    {
                        series.AdornmentsInfo.LabelTemplate = FindResource("datalabelTemplate") as DataTemplate;
                    }
                    series.AdornmentsInfo.SymbolTemplate = null;
                }
            }
            this.RemoveSelectionAdorner(ChartLegand);
            this.RemoveSelectionAdorner(ValueAxisTitle);
            this.RemoveSelectionAdorner(CategoryAxisTitle);
            this.RemoveSelectionAdorner(ChartTitle);
            this.RemoveSelectionAdorner(LegendTextbox);
            this.RemoveSelectionAdorner(ChartArea);
            this.RemoveSelectionAdorner(SecondaryXAxisTitle);
            this.RemoveSelectionAdorner(SecondaryYAxisTitle);
            UIElement primaryAxisPanel = null, secondaryAxisPanel = null, secondaryXAxisPanel = null, secondaryYAxisPanel = null;
            if (CategoryAxisTitle != null)
            {
                primaryAxisPanel = this.GetAxisPanel(CategoryAxisTitle);
            }
            if (primaryAxisPanel != null)
            {
                this.RemoveSelectionAdorner(primaryAxisPanel);
            }
            if (ValueAxisTitle != null)
            {
                secondaryAxisPanel = this.GetAxisPanel(ValueAxisTitle);
            }
            if (secondaryAxisPanel != null)
            {
                this.RemoveSelectionAdorner(secondaryAxisPanel);
            }
            if (SecondaryXAxisTitle != null)
            {
                secondaryXAxisPanel = this.GetAxisPanel(SecondaryXAxisTitle);
            }
            if (secondaryXAxisPanel != null)
            {
                this.RemoveSelectionAdorner(secondaryXAxisPanel);
            }
            if (SecondaryYAxisTitle != null)
            {
                secondaryYAxisPanel = this.GetAxisPanel(SecondaryYAxisTitle);
            }
            if (secondaryYAxisPanel != null)
            {
                this.RemoveSelectionAdorner(secondaryYAxisPanel);
            }
        }


        private UIElement GetAxisPanel(TextBox axisTitle)
        {
            UIElement axisTitleParent, axisTitleGrid, axisPanel;
            axisTitleParent = axisTitleGrid = axisPanel = null;
            if (axisTitle != null)
            {
                axisTitleParent = VisualTreeHelper.GetParent(axisTitle) as UIElement;
            }
            if (axisTitleParent != null)
            {
                axisTitleGrid = VisualTreeHelper.GetParent(axisTitleParent) as UIElement;
            }
            if (axisTitleGrid != null)
            {
                axisPanel = VisualTreeHelper.GetParent(axisTitleGrid) as UIElement;
            }
            return axisPanel;
        }

        private void chartSeries_MouseRightButtonDown(object sender, ChartMouseEventArgs e)
        {
            ChartSeries series = sender as ChartSeries;
            this.RemoveChartObjectSelectionAdorner();
            this.GetSeriesCount(series);
            this.SelectSeries(series);
            this.ShowDatalabels.Visibility = Visibility.Visible;
            this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.chartSeriesPropertiesCollection[seriesCount], IsSelected = true });

            if (this.chartSeriesPropertiesCollection[seriesCount].ShowDataLabels.ToLower() == "true")
            {
                this.ShowDatalabels.IsChecked = true;
            }
            else
            {
                this.ShowDatalabels.IsChecked = false;
            }
            this.ChartArea.Focus();
            this.ChartTitle.Focusable = true;
            this.CategoryAxisTitle.Focusable = true;
            this.ValueAxisTitle.Focusable = true;
            this.SecondaryXAxisTitle.Focusable = true;
            this.SecondaryYAxisTitle.Focusable = true;
            this.LegendTextbox.Focusable = true;
            e.MouseEventArgs.Handled = true;
        }

        private void ShowDataLabels_Click(object sender, RoutedEventArgs e)
        {
            if (this.ShowDatalabels.IsChecked)
            {
                this.chartSeriesPropertiesCollection[seriesCount].ShowDataLabels = "True";
            }

            else
            {
                this.chartSeriesPropertiesCollection[seriesCount].ShowDataLabels = "False";
            }
        }

        private void InnerChart_MouseRightButtonDown(object sender, EventArgs e)
        {
            this.ShowDatalabels.Visibility = Visibility.Collapsed;
        }

        private void ChartSeries_MouseLeftButtonDown(object sender, ChartMouseEventArgs e)
        {
            this.RemoveChartObjectSelectionAdorner();
            ChartSeries series = sender as ChartSeries;
            this.GetSeriesCount(series);
            this.SelectSeries(series);
            this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.chartSeriesPropertiesCollection[seriesCount], IsSelected = true });                                  
            this.ChartArea.Focus();
            this.ChartTitle.Focusable = true;
            this.CategoryAxisTitle.Focusable = true;
            this.ValueAxisTitle.Focusable = true;
            this.SecondaryXAxisTitle.Focusable = true;
            this.SecondaryYAxisTitle.Focusable = true;
            this.LegendTextbox.Focusable = true;
            e.MouseEventArgs.Handled = true;
        }

        private void SelectSeries(ChartSeries series)
        {
            series.AdornmentsInfo.Visible = true;
            series.AdornmentsInfo.SymbolTemplate = FindResource("seriesselectionTemplate") as DataTemplate;
            if (this.chartSeriesPropertiesCollection[seriesCount].ShowDataLabels.ToLower() == "false")
            {
                series.AdornmentsInfo.LabelTemplate = FindResource("dataTemplate") as DataTemplate;
                series.AdornmentsInfo.HorizontalAlignment = HorizontalAlignment.Center;
                series.AdornmentsInfo.VerticalAlignment = VerticalAlignment.Center;               
            }
            else
            {
                series.AdornmentsInfo.LabelTemplate = FindResource("datalabelTemplate") as DataTemplate;                
                this.SetDataLabelsPosition(series, this.chartSeriesPropertiesCollection[seriesCount].DataLabelsPosition);
            }

        }

        private void GetSeriesCount(ChartSeries series)
        {
            seriesCount = 0;
            seriesName = series.Name;

            foreach (ChartSeries chartseries in this.InnerChart.Areas[0].Series)
            {
                if (series == chartseries)
                {
                    //seriesCount++;
                    break;
                }

                if (series != chartseries)
                {
                    if (this.chartSeriesPropertiesCollection[seriesCount].ShowDataLabels.ToLower() == "false")
                    {
                        series.AdornmentsInfo.LabelTemplate = FindResource("dataTemplate") as DataTemplate;
                    }
                    else
                    {
                        series.AdornmentsInfo.LabelTemplate = FindResource("datalabelTemplate") as DataTemplate;
                    }
                    chartseries.AdornmentsInfo.SymbolTemplate = null;
                }
            }
        }

        private void SetTitleSelectionAdorner(TextBox titleTextBox, Editors.ChartObject selectedChartObject)
        {
            UIElement axisTitleParent = VisualTreeHelper.GetParent(titleTextBox) as UIElement;
            if (axisTitleParent != null)
            {
                Grid axisTitleGrid = VisualTreeHelper.GetParent(axisTitleParent) as Grid;

                if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                {
                    if (titleTextBox == this.CategoryAxisTitle || titleTextBox == this.ValueAxisTitle)
                    {
                        if (selectedChartObject == Editors.ChartObject.PrimaryAxisTitle)
                        {
                            selectedChartObject = Editors.ChartObject.SecondaryAxisTile;
                        }
                        else if (selectedChartObject == Editors.ChartObject.SecondaryAxisTile)
                        {
                            selectedChartObject = Editors.ChartObject.PrimaryAxisTitle;
                        }
                    }
                }
                
                if (selectedChartObject == Editors.ChartObject.ChartTitle)
                {
                    ChartTitle.Width = this.ChartArea.RenderSize.Width - 15;
                    AdornerLayer.GetAdornerLayer(titleTextBox).Add(new ChartObjectSelectionAdorner(titleTextBox, Editors.ChartObject.ChartTitle, ChartArea.AxesThickness, new Size()));
                }              
                else if (selectedChartObject == Editors.ChartObject.LegendTitle)
                {
                    AdornerLayer.GetAdornerLayer(titleTextBox).Add(new ChartObjectSelectionAdorner(titleTextBox, Editors.ChartObject.LegendTitle, ChartArea.AxesThickness, new Size()));
                }
                else if (selectedChartObject == Editors.ChartObject.SecondaryAxisTile)
                {
                    axisTitleGrid.Width = this.ChartArea.RenderSize.Height - this.ChartArea.AxesThickness.Bottom - this.ChartArea.AxesThickness.Top - 15;
                    AdornerLayer.GetAdornerLayer(titleTextBox).Add(new ChartObjectSelectionAdorner(titleTextBox, Editors.ChartObject.SecondaryAxisTile, ChartArea.AxesThickness, new Size()));
                }
                else
                {
                    axisTitleGrid.Width = this.ChartArea.RenderSize.Width - this.ChartArea.AxesThickness.Left - this.ChartArea.AxesThickness.Right - 15;
                    AdornerLayer.GetAdornerLayer(titleTextBox).Add(new ChartObjectSelectionAdorner(titleTextBox, Editors.ChartObject.PrimaryAxisTitle, ChartArea.AxesThickness, new Size()));
                }
            }
        }

        private void SetTitleTextBoxFocus(TextBox titleTextBox, MouseButtonEventArgs e, Editors.ChartObject selectedChartObject)
        {
            this.RemoveChartObjectSelectionAdorner();
            if (titleTextBox.Focusable)
            {
                if (!titleTextBox.IsFocused)
                {
                    this.SetTitleSelectionAdorner(titleTextBox, selectedChartObject);
                    titleTextBox.Focusable = false;
                    this.ChartArea.Focus();
                    e.Handled = true;
                }
                else
                {
                    titleTextBox.CaptureMouse();
                    titleTextBox.CaretIndex = titleTextBox.GetCharacterIndexFromPoint(Mouse.GetPosition(titleTextBox), true);
                }
            }
            else
            {
                titleTextBox.Focusable = true;
                titleTextBox.CaretIndex = titleTextBox.Text.Length;
                titleTextBox.Focus();
                this.RemoveSelectionAdorner(titleTextBox);
                e.Handled = true;
            }
        }

        private void ChartControl_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.RemoveChartObjectSelectionAdorner();
            this.ChartTitle.Focusable = true;
            this.CategoryAxisTitle.Focusable = true;
            this.ValueAxisTitle.Focusable = true;
            this.SecondaryXAxisTitle.Focusable = true;
            this.SecondaryYAxisTitle.Focusable = true;
            this.ChartArea.IsContextMenuEnabled = false;
            this.RaiseReportItemSelectedEvent(null, new SelectedItemEventArgs() { SelectedItem = this.chartProperties, IsSelected = false });
        }

        private void ChartTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.LegendTextbox.Focusable = true;
            this.CategoryAxisTitle.Focusable = true;
            this.ValueAxisTitle.Focusable = true;
            this.SecondaryXAxisTitle.Focusable = true;
            this.SecondaryYAxisTitle.Focusable = true;
            this.SetTitleTextBoxFocus(ChartTitle, e, Editors.ChartObject.ChartTitle);
            this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.titleProperties, IsSelected = true });
            e.Handled = true;
        }

        private void ChartTitle_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.RemoveChartObjectSelectionAdorner();
            this.SetTitleSelectionAdorner(ChartTitle, Editors.ChartObject.ChartTitle);
            this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.titleProperties, IsSelected = true });
            ChartTitle.Focusable = false;
            e.Handled = true;
        }

        private void RemoveSelectionAdorner(UIElement chartObject)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(chartObject);
            if (layer != null)
            {
                Adorner[] adorners = layer.GetAdorners(chartObject);
                if (adorners != null)
                {
                    foreach (Adorner adorner in adorners)
                    {
                        if (adorner is ChartObjectSelectionAdorner)
                        {
                            layer.Remove(adorner);
                        }
                    }
                }
            }
        }

        private void ChartTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            this.titleProperties.Name = ChartTitle.Text;
        }

        private void LegendTextbox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.RemoveChartObjectSelectionAdorner();
            ChartLegend ChartLegend = sender as ChartLegend;
            if (e.OriginalSource.GetType().Name == "TextBoxView")
            {
                this.SetTitleTextBoxFocus(LegendTextbox, e, Editors.ChartObject.LegendTitle);
                this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.legendTitleProperties, IsSelected = true });
                e.Handled = true;
            }
            else
            {
                this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.chartLegendProperties, IsSelected = true });
                AdornerLayer.GetAdornerLayer(ChartLegand).Add(new ChartObjectSelectionAdorner(ChartLegend, Editors.ChartObject.ChartLegend, new Thickness(0), new Size()));
                ChartLegand.Focus();
                this.LegendTextbox.Focusable = true;
                e.Handled = true;
            }
            this.ChartTitle.Focusable = true;
            this.CategoryAxisTitle.Focusable = true;
            this.ValueAxisTitle.Focusable = true;
            this.SecondaryXAxisTitle.Focusable = true;
            this.SecondaryYAxisTitle.Focusable = true;
        }

        private void LegendTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.legendTitleProperties.Caption = LegendTextbox.Text;
        }

        private void ChartArea_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            ChartTypes chartType = this.propertyValueConvertor.GetChartType(this.chartProperties.ChartType);

            if (this.ChartControlType == Controls.ChartControlType.Chart && (e.OriginalSource.GetType().Name == "ChartWatermarkElement" || e.OriginalSource.GetType().Name == "Rectangle" || e.OriginalSource.GetType().Name == "Line" || e.OriginalSource.GetType().Name == "PolyLine" || e.OriginalSource.GetType().Name == "Path" || e.OriginalSource.GetType().Name == "Ellipse")
                || (e.OriginalSource.GetType().Name == "ChartDockPanel" && (chartType == ChartTypes.Pyramid || chartType == ChartTypes.Funnel || chartType == ChartTypes.Pie || chartType == ChartTypes.Doughnut))
                || (e.OriginalSource.GetType().Name == "ChartPanel" && (chartType == ChartTypes.Polar || chartType == ChartTypes.Radar)))
            {
                this.RemoveChartObjectSelectionAdorner();
                this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.defaultChartProperties, IsSelected = true });
                if (chartType == ChartTypes.Doughnut || chartType == ChartTypes.Polar || chartType == ChartTypes.Pie || chartType == ChartTypes.Pyramid || chartType == ChartTypes.Radar || chartType == ChartTypes.Funnel)
                {
                    AdornerLayer.GetAdornerLayer(ChartArea).Add(new ChartObjectSelectionAdorner(ChartArea, Editors.ChartObject.ChartPlotArea, new Thickness(), new Size()));
                }
                else
                {
                    AdornerLayer.GetAdornerLayer(ChartArea).Add(new ChartObjectSelectionAdorner(ChartArea, Editors.ChartObject.ChartPlotArea, ChartArea.AxesThickness, new Size()));
                }
                this.ChartArea.Focus();
                this.ChartTitle.Focusable = true;
                this.CategoryAxisTitle.Focusable = true;
                this.ValueAxisTitle.Focusable = true;
                this.SecondaryXAxisTitle.Focusable = true;
                this.SecondaryYAxisTitle.Focusable = true;
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
            this.LegendTextbox.Focusable = true;
        }

        private void ChartGrid_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            ChartTypes chartType = this.propertyValueConvertor.GetChartType(this.chartProperties.ChartType);

            if (this.ChartControlType == Controls.ChartControlType.Chart && (e.OriginalSource.GetType().Name == "ChartWatermarkElement")
                || (e.OriginalSource.GetType().Name == "ChartDockPanel" && (chartType == ChartTypes.Pyramid || chartType == ChartTypes.Funnel || chartType == ChartTypes.Pie || chartType == ChartTypes.Doughnut))
                || (e.OriginalSource.GetType().Name == "ChartPanel" && (chartType == ChartTypes.Polar || chartType == ChartTypes.Radar)))
            {
                this.RemoveChartObjectSelectionAdorner();
                this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.defaultChartProperties, IsSelected = true });
                if (chartType == ChartTypes.Doughnut || chartType == ChartTypes.Polar || chartType == ChartTypes.Pie || chartType == ChartTypes.Pyramid || chartType == ChartTypes.Radar || chartType == ChartTypes.Funnel)
                {
                    AdornerLayer.GetAdornerLayer(ChartArea).Add(new ChartObjectSelectionAdorner(ChartArea, Editors.ChartObject.ChartPlotArea, new Thickness(), new Size()));
                }
                else
                {
                    AdornerLayer.GetAdornerLayer(ChartArea).Add(new ChartObjectSelectionAdorner(ChartArea, Editors.ChartObject.ChartPlotArea, ChartArea.AxesThickness, new Size()));
                }
                this.ChartArea.Focus();
                e.Handled = true;
            }
        }

        private void SetAxisSelectionAdorner(TextBox axisTitle, Editors.ChartObject selectedItem)
        {
            UIElement axisTitleParent = null, axisTitleGrid = null, axisPanel = null;
            double axisHeight = 0;
            if (axisTitle != null)
            {
                axisTitleParent = VisualTreeHelper.GetParent(axisTitle) as UIElement;
            }
            if (axisTitleParent != null)
            {
                axisTitleGrid = VisualTreeHelper.GetParent(axisTitleParent) as UIElement;
            }
            if (axisTitleGrid != null)
            {
                axisPanel = VisualTreeHelper.GetParent(axisTitleGrid) as UIElement;
                axisHeight = axisTitleGrid.RenderSize.Height;
            }
            if (axisPanel != null)
            {
                if (selectedItem == Editors.ChartObject.PrimaryAxis)
                {
                    this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.chartCategoryAxisProperties, IsSelected = true });
                    if (this.chartValueAxisProperties.ReverseDirection.ToLower() == "true")
                    {
                        if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                        {
                            AdornerLayer.GetAdornerLayer(axisPanel).Add(new ChartObjectSelectionAdorner(axisPanel, Editors.ChartObject.ReversePrimaryAxis, new Thickness(0), new Size(5, axisHeight)));
                        }
                        else
                        {
                            AdornerLayer.GetAdornerLayer(axisPanel).Add(new ChartObjectSelectionAdorner(axisPanel, Editors.ChartObject.ReverseSecondaryAxis, new Thickness(0), new Size(0, axisHeight)));
                        }
                    }
                    else
                    {
                        if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                        {
                            AdornerLayer.GetAdornerLayer(axisPanel).Add(new ChartObjectSelectionAdorner(axisPanel, Editors.ChartObject.SecondaryAxis, new Thickness(0, 0, 0, -5), new Size(5, axisHeight)));
                        }
                        else
                        {
                            AdornerLayer.GetAdornerLayer(axisPanel).Add(new ChartObjectSelectionAdorner(axisPanel, Editors.ChartObject.PrimaryAxis, new Thickness(0), new Size(0, axisHeight)));
                        }
                    }
                }
                else if (selectedItem == Editors.ChartObject.SecondaryAxis)
                {
                    this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.chartValueAxisProperties, IsSelected = true });
                    if (this.chartCategoryAxisProperties.ReverseDirection.ToLower() == "true")
                    {
                        if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                        {
                            AdornerLayer.GetAdornerLayer(axisPanel).Add(new ChartObjectSelectionAdorner(axisPanel, Editors.ChartObject.ReverseSecondaryAxis, new Thickness(0, 5, 0, 0), new Size(5, axisHeight)));
                        }
                        else
                        {
                            AdornerLayer.GetAdornerLayer(axisPanel).Add(new ChartObjectSelectionAdorner(axisPanel, Editors.ChartObject.ReversePrimaryAxis, new Thickness(0), new Size(0, axisHeight)));
                        }
                    }
                    else
                    {
                        if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                        {
                            AdornerLayer.GetAdornerLayer(axisPanel).Add(new ChartObjectSelectionAdorner(axisPanel, Editors.ChartObject.PrimaryAxis, new Thickness(0), new Size(5, axisHeight)));
                        }
                        else
                        {
                            AdornerLayer.GetAdornerLayer(axisPanel).Add(new ChartObjectSelectionAdorner(axisPanel, Editors.ChartObject.SecondaryAxis, new Thickness(0), new Size(0, axisHeight)));
                        }
                    }
                }
                else if (selectedItem == Editors.ChartObject.ReverseSecondaryAxis)
                {
                    this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.chartSecondaryCategoryAxisProperties, IsSelected = true });
                    if (this.secondaryXAxis.OpposedPosition == false)
                    {
                        if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                        {
                            AdornerLayer.GetAdornerLayer(axisPanel).Add(new ChartObjectSelectionAdorner(axisPanel, Editors.ChartObject.ReverseSecondaryAxis, new Thickness(0, 33, 0, 0), new Size(5, axisHeight)));
                        }
                        else
                        {
                            AdornerLayer.GetAdornerLayer(axisPanel).Add(new ChartObjectSelectionAdorner(axisPanel, Editors.ChartObject.ReverseSecondaryAxis, new Thickness(0,23,0,0), new Size(0, axisHeight)));
                        }
                    }
                    else
                    {
                        AdornerLayer.GetAdornerLayer(axisPanel).Add(new ChartObjectSelectionAdorner(axisPanel, Editors.ChartObject.ReverseSecondaryAxis, new Thickness(0), new Size(0, axisHeight)));
                    }
                }
                else if (selectedItem == Editors.ChartObject.ReversePrimaryAxis)
                {
                    this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.chartSecondaryValueAxisProperties, IsSelected = true });
                    if (this.secondaryYAxis.OpposedPosition == false)
                    {
                        if (this.chartProperties.ChartType.ToLower() == "bar" || this.chartProperties.ChartType.ToLower() == "stackingbar")
                        {
                            AdornerLayer.GetAdornerLayer(axisPanel).Add(new ChartObjectSelectionAdorner(axisPanel, Editors.ChartObject.ReversePrimaryAxis, new Thickness(33, 0, 0, 0), new Size(5, axisHeight)));
                        }
                        else
                        {
                            AdornerLayer.GetAdornerLayer(axisPanel).Add(new ChartObjectSelectionAdorner(axisPanel, Editors.ChartObject.ReversePrimaryAxis, new Thickness(23, 0, 0, 0), new Size(0, axisHeight)));
                        }
                    }
                    else
                    {
                        AdornerLayer.GetAdornerLayer(axisPanel).Add(new ChartObjectSelectionAdorner(axisPanel, Editors.ChartObject.ReversePrimaryAxis, new Thickness(0), new Size(0, axisHeight)));
                    }
                }
            }
            this.ChartArea.Focus();
            this.ChartTitle.Focusable = true;
            this.CategoryAxisTitle.Focusable = true;
            this.ValueAxisTitle.Focusable = true;
            this.LegendTextbox.Focusable = true;
            this.SecondaryXAxisTitle.Focusable = true;
            this.SecondaryYAxisTitle.Focusable = true;
        }

        private void PrimaryAxis_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            this.RemoveChartObjectSelectionAdorner();
            if (e.OriginalSource.GetType().Name == "TextBoxView")
            {
                e.Handled = false;
            }
            else
            {
                this.SetAxisSelectionAdorner(CategoryAxisTitle, Editors.ChartObject.PrimaryAxis);
                e.Handled = true;
            }

        }

        private void PrimaryAxis_PreviewMouseRightButtonDown(object sender, RoutedEventArgs e)
        {
            this.RemoveChartObjectSelectionAdorner();
            if (e.OriginalSource.GetType().Name == "TextBoxView")
            {
                e.Handled = false;
            }
            else
            {
                this.SetAxisSelectionAdorner(CategoryAxisTitle, Editors.ChartObject.PrimaryAxis);
                e.Handled = true;
            }
        }

        private void SecondaryAxis_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            this.RemoveChartObjectSelectionAdorner();
            if (e.OriginalSource.GetType().Name == "TextBoxView")
            {
                e.Handled = false;
            }
            else
            {
                this.SetAxisSelectionAdorner(ValueAxisTitle, Editors.ChartObject.SecondaryAxis);
                e.Handled = true;
            }

        }

        private void SecondaryAxis_PreviewMouseRightButtonDown(object sender, RoutedEventArgs e)
        {
            this.RemoveChartObjectSelectionAdorner();
            if (e.OriginalSource.GetType().Name == "TextBoxView")
            {
                e.Handled = false;
            }
            else
            {
                this.SetAxisSelectionAdorner(ValueAxisTitle, Editors.ChartObject.SecondaryAxis);
                e.Handled = true;
            }
        }

        private void SecondaryXAxis_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            this.RemoveChartObjectSelectionAdorner();
            if (e.OriginalSource.GetType().Name == "TextBoxView")
            {
                e.Handled = false;
            }
            else
            {
                this.SetAxisSelectionAdorner(SecondaryXAxisTitle, Editors.ChartObject.ReverseSecondaryAxis);
                e.Handled = true;
            }

        }

        private void SecondaryYAxis_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            this.RemoveChartObjectSelectionAdorner();
            if (e.OriginalSource.GetType().Name == "TextBoxView")
            {
                e.Handled = false;
            }
            else
            {
                this.SetAxisSelectionAdorner(SecondaryYAxisTitle, Editors.ChartObject.ReversePrimaryAxis);
                e.Handled = true;
            }

        }

        private void CategoryAxisTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SetTitleTextBoxFocus(CategoryAxisTitle, e, Editors.ChartObject.PrimaryAxisTitle);
            this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.categoryAxisTitleProperties, IsSelected = true });
            this.ChartTitle.Focusable = true;
            this.ValueAxisTitle.Focusable = true;
            this.SecondaryXAxisTitle.Focusable = true;
            this.SecondaryYAxisTitle.Focusable = true;
            e.Handled = true;
        }

        private void CategoryAxisTitle_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.RemoveChartObjectSelectionAdorner();
            this.SetTitleSelectionAdorner(CategoryAxisTitle, Editors.ChartObject.PrimaryAxisTitle);
            this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.categoryAxisTitleProperties, IsSelected = true });
            CategoryAxisTitle.Focusable = false;
            e.Handled = true;
        }

        private void CategoryAxisTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            this.categoryAxisTitleProperties.Name = CategoryAxisTitle.Text;
        }

        private void SecondaryXAxisTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SetTitleTextBoxFocus(SecondaryXAxisTitle, e, Editors.ChartObject.PrimaryAxisTitle);
            this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.secondaryXAxisTitleProperties, IsSelected = true });
            this.ChartTitle.Focusable = true;
            this.ValueAxisTitle.Focusable = true;
            this.SecondaryYAxisTitle.Focusable = true;
            e.Handled = true;
        }

        private void SecondaryXAxisTitle_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.RemoveChartObjectSelectionAdorner();
            this.SetTitleSelectionAdorner(SecondaryYAxisTitle, Editors.ChartObject.PrimaryAxisTitle);
            this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.secondaryXAxisTitleProperties, IsSelected = true });
            SecondaryXAxisTitle.Focusable = false;
            e.Handled = true;
        }

        private void SecondaryXAxisTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            this.secondaryXAxisTitleProperties.Name = SecondaryXAxisTitle.Text;
        }

        private void ValueAxisTitle_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.SetTitleTextBoxFocus(ValueAxisTitle, e, Editors.ChartObject.SecondaryAxisTile);
            this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.valueAxisTitleProperties, IsSelected = true });
            this.ChartTitle.Focusable = true;
            this.CategoryAxisTitle.Focusable = true;
            this.SecondaryXAxisTitle.Focusable = true;
            this.SecondaryYAxisTitle.Focusable = true;
            e.Handled = true;
        }

        private void ValueAxisTitle_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.RemoveChartObjectSelectionAdorner();
            this.SetTitleSelectionAdorner(ValueAxisTitle, Editors.ChartObject.SecondaryAxisTile);
            this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.valueAxisTitleProperties, IsSelected = true });
            ValueAxisTitle.Focusable = false;
            e.Handled = true;
        }

        private void ValueAxisTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            this.valueAxisTitleProperties.Name = ValueAxisTitle.Text;
        }

        private void SecondaryYAxisTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SetTitleTextBoxFocus(SecondaryYAxisTitle, e, Editors.ChartObject.SecondaryAxisTile);
            this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.secondaryYAxisTitleProperties, IsSelected = true });
            this.ChartTitle.Focusable = true;
            this.ValueAxisTitle.Focusable = true;
            this.SecondaryXAxisTitle.Focusable = true;
            e.Handled = true;
        }

        private void SecondaryYAxisTitle_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.RemoveChartObjectSelectionAdorner();
            this.SetTitleSelectionAdorner(SecondaryYAxisTitle, Editors.ChartObject.SecondaryAxisTile);
            this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.secondaryYAxisTitleProperties, IsSelected = true });
            SecondaryYAxisTitle.Focusable = false;
            e.Handled = true;
        }

        private void SecondaryYAxisTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            this.secondaryYAxisTitleProperties.Name = SecondaryXAxisTitle.Text;
        }

    }

    #region Adorner

    internal class ChartObjectSelectionAdorner : System.Windows.Documents.Adorner
    {
        Editors.ChartObject selectedItem;
        double marginLeft, marginBottom, marginRight, marginTop;
        Size size;
        public ChartObjectSelectionAdorner(UIElement adornedElement, Editors.ChartObject selectedChartObject, Thickness Margin, Size renderSize)
            : base(adornedElement)
        {
            this.selectedItem = selectedChartObject;
            this.marginLeft = Margin.Left;
            this.marginBottom = Margin.Bottom;
            this.marginTop = Margin.Top;
            this.marginRight = Margin.Right;
            this.size = renderSize;

            if (selectedChartObject == Editors.ChartObject.SecondaryAxisTile)
            {
                this.Margin = new Thickness(2, 0, 0, 0);
            }
            else if (selectedChartObject == Editors.ChartObject.LegendTitle)
            {
                this.Margin = new Thickness(-10, -10, -10, -5);
            }
            else if (selectedChartObject == Editors.ChartObject.ChartPlotArea)
            {
                if (this.marginLeft != 0)
                {
                    this.marginLeft += 10;
                }
                if (this.marginTop != 0)
                {
                    this.marginTop += 15;
                }
                this.Margin = new Thickness(this.marginLeft, this.marginTop, this.marginRight, this.marginBottom);
            }
            else if (selectedChartObject == Editors.ChartObject.PrimaryAxis)
            {
                this.Margin = new Thickness(-15 + this.size.Width, -3, 0, 0);
            }
            else if (selectedChartObject == Editors.ChartObject.ReverseSecondaryAxis)
            {
                this.Margin = new Thickness(-15 + this.size.Width, this.size.Height + 3 - this.marginTop, 0, this.marginBottom);
            }            
            else if (selectedChartObject == Editors.ChartObject.SecondaryAxis)
            {
                this.Margin = new Thickness(this.size.Height + this.marginBottom, -10 + this.marginTop, 0, 10);
            }
            else if (selectedChartObject == Editors.ChartObject.ReversePrimaryAxis)
            {
                this.Margin = new Thickness(-5 + this.marginLeft, -10, this.size.Height - 5, 0);
            }
            else
            {
                this.Margin = new Thickness(-5, -5, 10, -5);
            }
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            Rect adornedElementRect;
            if (this.selectedItem == Editors.ChartObject.ChartPlotArea)
            {
                if (this.marginLeft != 0 && this.marginBottom != 0 && this.marginRight != 0 && this.marginTop != 0)
                {
                    adornedElementRect = new Rect(new Size(this.AdornedElement.RenderSize.Width - this.marginLeft - this.marginRight - 10, this.AdornedElement.RenderSize.Height - this.marginBottom - this.marginTop - 20));
                }
                else if (this.marginLeft != 0 && this.marginBottom != 0 && this.marginRight == 0 && this.marginTop != 0)
                {
                    adornedElementRect = new Rect(new Size(this.AdornedElement.RenderSize.Width - this.marginLeft - 10, this.AdornedElement.RenderSize.Height - this.marginBottom - this.marginTop - 20));
                }
                else if (this.marginLeft != 0 && this.marginBottom != 0 && this.marginRight != 0 && this.marginTop == 0)
                {
                    adornedElementRect = new Rect(new Size(this.AdornedElement.RenderSize.Width - this.marginLeft - this.marginRight - 10, this.AdornedElement.RenderSize.Height - this.marginBottom - 20));
                }
                else if (this.marginLeft == 0 && this.marginBottom != 0 && this.marginRight != 0 && this.marginTop != 0)
                {
                    adornedElementRect = new Rect(new Size(this.AdornedElement.RenderSize.Width - this.marginRight - 10, this.AdornedElement.RenderSize.Height - this.marginTop - this.marginBottom - 20));
                }
                else if (this.marginLeft != 0 && this.marginBottom == 0 && this.marginRight != 0 && this.marginTop != 0)
                {
                    adornedElementRect = new Rect(new Size(this.AdornedElement.RenderSize.Width - this.marginLeft - this.marginRight - 10, this.AdornedElement.RenderSize.Height - this.marginTop - 20));
                }
                else if (this.marginLeft != 0 && this.marginBottom != 0)
                {
                    adornedElementRect = new Rect(new Size(this.AdornedElement.RenderSize.Width - this.marginLeft - 10, this.AdornedElement.RenderSize.Height - this.marginBottom - 20));
                }
                else if (this.marginTop != 0 && this.marginLeft != 0)
                {
                    adornedElementRect = new Rect(new Size(this.AdornedElement.RenderSize.Width - this.marginLeft - 10, this.AdornedElement.RenderSize.Height - this.marginTop - 20));
                }
                else if (this.marginTop != 0 && this.marginRight != 0)
                {
                    adornedElementRect = new Rect(new Size(this.AdornedElement.RenderSize.Width - this.marginRight - 10, this.AdornedElement.RenderSize.Height - this.marginTop - 20));
                }
                else
                {
                    adornedElementRect = new Rect(new Size(this.AdornedElement.RenderSize.Width - this.marginRight - 10, this.AdornedElement.RenderSize.Height - this.marginBottom - 20));
                }

            }
            else if (this.selectedItem == Editors.ChartObject.LegendTitle)
            {
                adornedElementRect = new Rect(new Size(this.AdornedElement.RenderSize.Width + 20, this.AdornedElement.RenderSize.Height + 32 - this.AdornedElement.RenderSize.Height));
            }
            else if (this.selectedItem == Editors.ChartObject.PrimaryAxis || this.selectedItem == Editors.ChartObject.ReverseSecondaryAxis)
            {
                adornedElementRect = new Rect(new Size(this.AdornedElement.RenderSize.Width + 25, this.AdornedElement.RenderSize.Height - this.size.Height + this.size.Width));
            }
            else if (this.selectedItem == Editors.ChartObject.SecondaryAxis || this.selectedItem == Editors.ChartObject.ReversePrimaryAxis)
            {
                adornedElementRect = new Rect(new Size(this.AdornedElement.RenderSize.Width - this.size.Height + 3 + this.size.Width, this.AdornedElement.RenderSize.Height + 20));
            }
            else if (this.selectedItem == Editors.ChartObject.SecondaryAxisTile)
            {
                adornedElementRect = new Rect(new Size(this.AdornedElement.RenderSize.Width - 5, this.AdornedElement.RenderSize.Height + 3));
            }
            else
            {
                adornedElementRect = new Rect(new Size(this.AdornedElement.RenderSize.Width + 10, this.AdornedElement.RenderSize.Height + 10));
            }

            Pen drawingPen = new Pen(new SolidColorBrush(Colors.Black), 1.5);
            drawingPen.DashStyle = DashStyles.Dot;
            Pen circledrawingPen = new Pen(new SolidColorBrush(Colors.Black), 1.5);
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Transparent);
            renderBrush.Opacity = 1;
            double renderRadius = 5;

            drawingContext.DrawLine(drawingPen, adornedElementRect.BottomLeft, adornedElementRect.BottomRight);
            drawingContext.DrawLine(drawingPen, adornedElementRect.TopRight, adornedElementRect.BottomRight);
            drawingContext.DrawLine(drawingPen, adornedElementRect.TopLeft, adornedElementRect.TopRight);
            drawingContext.DrawLine(drawingPen, adornedElementRect.TopLeft, adornedElementRect.BottomLeft);
            drawingContext.DrawEllipse(renderBrush, circledrawingPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, circledrawingPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, circledrawingPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, circledrawingPen, adornedElementRect.BottomRight, renderRadius, renderRadius);

        }
    }

    #endregion

    internal enum ChartChild
    {
        Chart,
        ChartArea,
        Series,
        Legend,
        ValueAxis,
        CategoryAxis,
        ChartTitle,
        CategoryTitle,
        ValueTitle,
        SecondaryCategoryTitle,
        SecondaryValueTitle
    }

    internal enum ChartControlType
    {
        Chart,
        DataBar,
        Sparkline
    }
}


