#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.OlapSilverlight.Engine;
using System.Windows.Controls;
using System;
using System.Windows;
using Syncfusion.Windows.Chart;
using Syncfusion.OlapSilverlight.Data;
using System.ComponentModel;
using Syncfusion.OlapSilverlight.Reports;
using System.Linq;
using System.Collections.Generic;

namespace Syncfusion.Silverlight.Chart.Olap
{
    public class OlapArea : ChartArea
    {
        #region Members

        public bool isNonPanelChartType;

        internal static ResourceDictionary resource;

        #endregion

        #region Initilize/Finalize

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapArea"/> class.
        /// </summary>
        public OlapArea()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                resource = new ResourceDictionary { Source = new Uri("/Syncfusion.OlapChart.Silverlight;component/Themes/generic.xaml", UriKind.RelativeOrAbsolute) };
            }

            DefaultStyleKey = typeof(OlapArea);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Parent chart controls
        /// </summary>
        public OlapChart ChartControl { get; set; }

        /// <summary>
        /// Preserving the last updated engine
        /// </summary>
        public PivotEngine CurrentEngine { get; set; }

        /// <summary>
        /// Gets or sets the olap area axis thickness.
        /// </summary>
        /// <value>The olap area axis thickness.</value>
        internal Thickness OlapAreaAxisThickness
        {
            get
            {
                return this.OlapAxisThickness;
            }
            set
            {
                this.OlapAxisThickness = value;
            }
        }

        internal bool UpdatePanel { get; set; }

        #endregion

        #region Event Handlers

        public event EventHandler LabelClick;

        #endregion

        #region Event Delegates

        public delegate void EventHandler(object sender, OlapLabelClickEvenArgs e);

        #endregion

        #region Raise Label Click

        public virtual void RaiseLabelClick(OlapLabelClickEvenArgs e)
        {
            if (this.LabelClick != null)
            {
                this.LabelClick(this, e);
            }
        }

        #endregion

        #region Label Click

        /// <summary>
        /// Called when [label click] occurs.
        /// </summary>
        /// <param name="e">The e.</param>
        internal void OnLabelClick(OlapLabelClickEvenArgs e)
        {
            var dataRefreshEventArgs = new DataRefreshBeginEventArgs(RefreshType.Drilldown, e.CellDescriptor);
            this.ChartControl.RaiseDataRefreshBegin(dataRefreshEventArgs);
            if (!dataRefreshEventArgs.Handled)
            {
                this.ChartControl.IsProcessing = true;

                if (this.ChartControl.OlapDataManager.ItemSource != null)
                {
                    this.UpdateLayout();

                    this.ChartControl.IsProcessing = true;

                    this.Dispatcher.BeginInvoke(delegate
                    {
                        var customCollectionEngine = this.ChartControl.OlapDataManager.ToggleExpandableState(e.CellDescriptor, Syncfusion.OlapSilverlight.Engine.GridLayout.NoSummaries);//TableBuilder.BuildEngineFromIQueryable(IQueryableSource, SortType.Ascending, pivotDataElements.ColumnItems.ToArray(), pivotDataElements.SeriesItems.ToArray(), pivotDataElements.Summaries.ToArray(), pivotDataElements.IsRowSummary, GridLayout.Normal);

                        if (customCollectionEngine != null)
                        {
                            this.DataBind(customCollectionEngine);
                            this.ChartControl.IsProcessing = false;
                        }
                    });
                }
                else
                {
                    if (this.ChartControl.OlapDataManager != null)
                    {
                        this.ChartControl.IsProcessing = true;

                        if ((this.ChartControl.OlapDataManager.CurrentReport.DrillType != DrillType.DrillPosition && this.ChartControl.OlapDataManager.ToggleExpandableState(e.CellDescriptor.CellType, (Member)e.CellDescriptor.Tag))
                            || (this.ChartControl.OlapDataManager.CurrentReport.DrillType == DrillType.DrillPosition && this.ChartControl.OlapDataManager.ToggleExpandableStateOnDrillPosition(e.CellDescriptor, (Member)e.CellDescriptor.Tag)))
                        {
                            this.ChartControl.OlapDataManager.ExecuteCellSet();
                        }
                    }
                }
            }
            this.RaiseLabelClick(e);
        }

        /// <summary>
        /// Called when [label cell click].
        /// </summary>
        /// <param name="e">The e.</param>
        internal void OnLabelCellClick(OlapLabelClickEvenArgs e)
        {
            if (this.ChartControl.OlapDataManager.CurrentReport.DrillType == DrillType.DrillReplace)
            {
                this.ChartControl.IsProcessing = true;
                if (e.CellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                {
                    Items items = this.ChartControl.OlapDataManager.CurrentReport.SeriesElements;
                    foreach (Item item in items)
                    {
                        if (item.ElementValue is DimensionElement)
                        {
                            if ((item.ElementValue as DimensionElement).Hierarchy.UniqueName == (e.CellDescriptor.Tag as Member).UniqueName.Split('.')[0] + "." + (e.CellDescriptor.Tag as Member).UniqueName.Split('.')[1])
                            {
                                MemberElement memberElement = null;
                                foreach (LevelElement _levelElement in ((item.ElementValue as DimensionElement).Hierarchy.LevelElements as LevelElementCollection))
                                {
                                    if (_levelElement.MemberElements.Count != 0)
                                    {
                                        GetMemberElement(_levelElement.MemberElements, e.CellDescriptor.Tag as Member);
                                        if (PreviousMemberElement.ParentMemberElement != null)
                                        {
                                            PreviousMemberElement.ParentMemberElement.ChildMemberElements.Clear();
                                        }
                                        if (PreviousMemberElement.ParentMemberElement == null && PreviousMemberElement.IsParentLevel == true)
                                        {
                                            _levelElement.MemberElements.Clear();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                this.ChartControl.OlapDataManager.NotifyReportChanged();
            }
        }

        /// <summary>
        /// Gets or sets the previous member element.
        /// </summary>
        /// <value>The previous member element.</value>
        private MemberElement PreviousMemberElement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the previous member element.
        /// </summary>
        /// <param name="memberElementCollection">The member element collection.</param>
        /// <param name="member">The member.</param>
        private void GetMemberElement(MemberElementCollection memberElementCollection, Member member)
        {
            foreach (MemberElement _memberElement in memberElementCollection)
            {
                if (_memberElement.UniqueName != member.UniqueName && _memberElement.ChildMemberElements.Count > 0)
                    GetMemberElement(_memberElement.ChildMemberElements, member);
                else
                    PreviousMemberElement = _memberElement;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Trying to convert the cell value to double, if its not converted then 
        /// returning 0
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static double GetCellValue(string value)
        {
            double cellValue = 0d;
            //// Try converting the cell value to double f
            var isValid = double.TryParse(value.ToLowerInvariant(), out cellValue);

            return isValid ? cellValue : 0d;
        }

        /// <summary>
        /// Handles the SizeChanged event of the chartAreaContainer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void chartAreaContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.BeginInit();
            this.EndInit();
        }

        /// <summary>
        /// Updates the axis range.
        /// </summary>
        private void UpdateAxisRange()
        {
            this.PrimaryAxis.IsAutoSetRange = false;
            this.PrimaryAxis.Range = new DoubleRange(0, this.GetRowCount(this.CurrentEngine));
        }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <returns></returns>
        internal int GetRowCount(PivotEngine engine)
        {
            var rowcount = engine.RowsCount - engine.HeaderSection.Height;
            return rowcount;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updating the OlapArea with the engine data
        /// </summary>
        /// <param name="engine"></param>
        public void DataBind(PivotEngine engine)
        {
            var dataRefreshEventArgs = new DataRefreshBeginEventArgs(RefreshType.DataBind);
            this.ChartControl.RaiseDataRefreshBegin(dataRefreshEventArgs);
            this.Host = Windows.Chart.Host.OLAPChart;
            List<ChartSeries> listOfCollapsedSeries = new List<ChartSeries>();
            listOfCollapsedSeries.AddRange(this.Series.Where(i => i.Visibility == System.Windows.Visibility.Collapsed));
            //// Clear the series
            this.Series.Clear();

            if (this.CurrentEngine != engine)
            {
                //// Configures the chart types.
                this.ConfigureChartTypes();

                //// backing the engine to later use
                this.CurrentEngine = engine;

                //// variables declaration            
                int rowHeader = string.IsNullOrEmpty(engine.RowHeaderSection.Info) ? engine.RowHeaderSection.Right : engine.RowHeaderSection.Width,
                    columnHeader = engine.HeaderSection.Height,
                    columnsCount = engine.TableColumns.Count,
                    rowsCount = engine.RowsCount,
                    counter = 0;
#if DEBUG
                System.Diagnostics.Debug.WriteLine("DateTime Before Series Render" + DateTime.Now.ToString());
#endif
                this.BeginInit();

                //// Local variable for get back label format string from measure cells.
                string labelformatstring = string.Empty;

                //// rowHeader - Ignoring the row headers
                for (int column = rowHeader; column < columnsCount; column++)
                {
                    string legendText = string.Empty;
                    //// Creating a chart points collection
                    var dataPointCollection = new OlapChartPointCollection();
                    //// Creating a new chart series
                    var series = new ChartSeries();
                    //// Resetting the counter
                    counter = 0;

                    series.Segments.Clear();

                    //// Setting the chart type
                    series.Type = this.ChartControl.ChartType;

                    if (this.ChartControl.ChartType == ChartTypes.Pie ||
                        this.ChartControl.ChartType == ChartTypes.Pyramid ||
                        this.ChartControl.ChartType == ChartTypes.Funnel)
                    {
                        //// Iterating rows of the pivot table column
                        for (int row = 0; row < rowsCount; row++)
                        {
                            PivotCellDescriptor cellDescriptor = engine.TableColumns[column].Cells[row];

                            if (cellDescriptor.CellType == PivotCellDescriptorType.Value)
                            {
                                string legendString = string.Empty;
                                int stringcount = cellDescriptor.CellData.Rows.Count;
                                foreach (var item in cellDescriptor.CellData.Rows)
                                {
                                    legendString += item;
                                    if (stringcount > 1)
                                    {
                                        legendString += " - ";
                                        stringcount--;
                                    }
                                    legendText = legendString;
                                }
                                dataPointCollection.Add(new OlapChartPoint
                                {
                                    BindingX = legendText,
                                    BindingY = GetCellValue(cellDescriptor.Value),
                                    Tag = cellDescriptor
                                });

                                //// Gets the label format string from its corresponding descriptor.
                                labelformatstring = cellDescriptor.FormatString;
                            }

                            this.UpdateKPIValues(cellDescriptor, series);
                        }
                    }
                    else
                    {
                        //// Iterating rows of the pivot table column
                        for (int row = 0; row < rowsCount; row++)
                        {
                            PivotCellDescriptor cellDescriptor = engine.TableColumns[column].Cells[row];

                            if (cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader)
                            {
                                if (row != 0)
                                    legendText += " - ";
                                legendText += cellDescriptor.CellValue;
                            }
                            else if (cellDescriptor.CellType == PivotCellDescriptorType.Value)
                            {
                                //// Adding the chart points to the points collection
                                dataPointCollection.Add(new OlapChartPoint
                                {
                                    BindingX = (++counter).ToString(),
                                    BindingY = GetCellValue(cellDescriptor.Value),
                                    Tag = cellDescriptor
                                });

                                //// Gets the label format string from its corresponding descriptor.
                                labelformatstring = cellDescriptor.FormatString;
                            }

                            this.UpdateKPIValues(cellDescriptor, series);
                        }
                    }

                    //// Specifying the legend text
                    series.Label = legendText;

                    //// Associating the series values
                    series.DataSource = dataPointCollection;
                    this.PrimaryAxis.LabelsSource = dataPointCollection;
                    //this.SecondaryAxis.LabelsSource = dataPointCollection;

                    // this.PrimaryAxis.LabelsSource = dataPointCollection;
                    series.BindingPathX = "BindingX";
                    series.BindingPathsY = new System.Collections.Generic.List<string> { "BindingY" };

                    if (this.ChartControl != null)
                    {
                        if (this.ChartControl.OlapChartAdornmentInfo != null)
                        {
                            if (this.ChartControl.OlapChartAdornmentInfo.Visible)
                            {
                                series.AdornmentsInfo = this.ChartControl.OlapChartAdornmentInfo;
                            }
                        }
                    }
                    if (listOfCollapsedSeries.Any(j => j.Label == series.Label))
                    {
                        series.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    //// Adding the series to the ChartArea series collection
                    this.Series.Add(series);
                }

                //// Adding Label Headers
                OlapChartPointCollection labelHeaderChartPointCollection = new OlapChartPointCollection();

                //// Ignoring the Column header portion of engine
                for (int row = columnHeader; row < rowsCount; row++)
                {
                    PivotCellDescriptor cellDescriptor = engine.TableColumns[0].Cells[row];
                    labelHeaderChartPointCollection.Add(new OlapChartPoint { BindingX = cellDescriptor.CellValue });
                }

                //// Sets the label format string to secondary axis of chart.
                if (!string.IsNullOrEmpty(this.ChartControl.LabelFormat))
                {
                    this.SecondaryAxis.LabelFormat = this.ChartControl.LabelFormat;
                }
                else
                {
                    labelformatstring = labelformatstring ?? string.Empty;
                    if (labelformatstring.Equals("Currency"))
                    {
                        labelformatstring = "C";
                    }
                    else if (labelformatstring.Equals("Percent"))
                    {
                        labelformatstring = "P";
                    }
                    else
                    {
                        labelformatstring = "N";
                    }
                    this.SecondaryAxis.LabelFormat = labelformatstring;
                }

                this.UpdateAxisRange();
                this.EndInit();
#if DEBUG
                System.Diagnostics.Debug.WriteLine("DateTime after series update" + DateTime.Now.ToString());
#endif
                this.ChartControl.RaiseDataRefreshCompleted(new DataRefreshCompletedEventArgs());

                //// Applies styles for Legends, PrimaryAxis and SecondaryAxis if the styles are set externally.
                this.ApplyCommonStyles();

                //// This is used to validate whether the panel is already created or not.
                this.UpdatePanel = true;
            }
        }

        private void ApplyCommonStyles()
        {
            if (this.ChartControl != null)
            {
                //// Primary Axis style
                if (this.ChartControl.PrimaryAxisStyle != null)
                {
                    var primaryAxisStyle = this.ChartControl.PrimaryAxisStyle;

                    this.PrimaryAxis.LabelBackground = primaryAxisStyle.Background;
                    this.PrimaryAxis.LabelForeground = primaryAxisStyle.Foreground;
                    this.PrimaryAxis.LabelFontStyle= primaryAxisStyle.FontStyle;
                    this.PrimaryAxis.GridLineStroke = primaryAxisStyle.LineStroke;
                }

                //// Secondary Axis style
                if (this.ChartControl.SecondaryAxisStyle != null)
                {
                    var secondaryAxisStyle = this.ChartControl.SecondaryAxisStyle;

                    this.SecondaryAxis.LabelBackground = secondaryAxisStyle.Background;
                    this.SecondaryAxis.LabelForeground = secondaryAxisStyle.Foreground;
                    this.SecondaryAxis.LabelFontStyle = secondaryAxisStyle.FontStyle;
                    this.SecondaryAxis.GridLineStroke = secondaryAxisStyle.LineStroke;
                }

                //// Legend style
                if (this.ChartControl.LegendStyle != null)
                {
                    var legendLabelStyle = this.ChartControl.LegendStyle;

                    this.Legends.Background = legendLabelStyle.Background;
                    this.Legends.Foreground = legendLabelStyle.Foreground;
                    this.Legends.FontStyle = legendLabelStyle.FontStyle;
                    this.Legends.BorderBrush = legendLabelStyle.LineStroke;
                }
            }
        }

        private void UpdateKPIValues(PivotCellDescriptor cellDescriptor, ChartSeries series)
        {
            if (cellDescriptor.CellType == PivotCellDescriptorType.Value
            && (cellDescriptor.KpiType == KpiTypeEnum.Kpi_Status || cellDescriptor.KpiType == KpiTypeEnum.Kpi_Trend))
            {
                if (series.AdornmentsInfo == null)
                {
                    series.AdornmentsInfo = new ChartAdornmentInfo();
                }

                series.AdornmentsInfo.Visible = true;
                series.AdornmentsInfo.LabelContentPath = string.Empty;
                series.SegmentTemplate = this.GetKPIGraphicsTemplate("KPIImage");
            }
        }

        private DataTemplate GetKPIGraphicsTemplate(string kpiGraphics)
        {
            return resource[kpiGraphics] as DataTemplate;
        }

        /// <summary>
        /// Configures the chart types.
        /// </summary>
        private void ConfigureChartTypes()
        {
            this.PrimaryAxis.Visibility = Visibility.Visible;
            this.SecondaryAxis.Visibility = Visibility.Visible;
            this.PrimaryAxis.SmallTicksStrokeThickness = 1d;
            this.PrimaryAxis.Opacity = 1d;
            this.PrimaryAxis.ShowGridLines = true;
            this.SecondaryAxis.ShowGridLines = true;

            switch(this.ChartControl.ChartType)
            {
                case ChartTypes.Pie: 
                case ChartTypes.Funnel:
                case ChartTypes.Pyramid:
                    //// Primary and secondary axis are not required for Pie, Funnel and Pyramid charts. We are collapsing them.
                    this.PrimaryAxis.Visibility = Visibility.Collapsed;
                    this.SecondaryAxis.Visibility = Visibility.Collapsed;
                    this.PrimaryAxis.ShowGridLines = false;
                    this.SecondaryAxis.ShowGridLines = false;
                    this.isNonPanelChartType = true;
                    break;
                case ChartTypes.Polar:
                case ChartTypes.Radar:
                    //// Since, collapsing the primary or secondary axis. Causes some issue in base chart and as a result the chart is not getting rendered correctly.
                    //// So, we are toggling the opacity of the primary axis. This solves the problem.
                    this.PrimaryAxis.Opacity = 0d;
                    this.isNonPanelChartType = true;
                    break;
                case ChartTypes.Column:
                case ChartTypes.StackingColumn:
                case ChartTypes.StackingColumn100:
                case ChartTypes.Area:
                case ChartTypes.StackingArea:
                case ChartTypes.Spline:
                case ChartTypes.SplineArea:
                case ChartTypes.Scatter:
                case ChartTypes.Line:
                case ChartTypes.StepArea:
                case ChartTypes.StepLine:
                    //// Collapsing the MinorTick lines for the primary axis label to get close to the axis.
                    this.PrimaryAxis.SmallTicksStrokeThickness = 0d;
                    break;
                case ChartTypes.Bar:
                case ChartTypes.StackingBar:
                case ChartTypes.StackingBar100:
                case ChartTypes.RotatedSpline:
                    //// Collapsing the Mino Tick lines to make the primary axis label to get close to the axis.
                    this.PrimaryAxis.SmallTicksStrokeThickness = 0d;
                    break;
            }
        }

        /// <summary>
        /// Invoke to render chart Area.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Grid chartAreaContainer = this.GetTemplateChild("LayoutRoot") as Grid;
                chartAreaContainer.SizeChanged += new SizeChangedEventHandler(chartAreaContainer_SizeChanged);

                if (this.PrimaryAxis != null)
                {
                    this.PrimaryAxis.Items.Clear();

                    ItemsPanelTemplate panelTemplate = resource["AxisPanelItemTemplate"] as ItemsPanelTemplate;
                    this.PrimaryAxis.ItemsPanel = panelTemplate;
                }
            }
            base.OnApplyTemplate();
        }
            
        #endregion
    }

    public class KpiGraphics
    {
        public const string Cylinder = "Cylinder";
        public const string RoadSigns = "Road Signs";
        public const string StandardArrow = "Standard Arrow";
    }
}