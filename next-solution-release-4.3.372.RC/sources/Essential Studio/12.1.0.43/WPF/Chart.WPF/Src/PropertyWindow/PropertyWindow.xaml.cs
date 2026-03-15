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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using Syncfusion.Windows.Chart;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PropertyWindow : Window
    {
        private Chart PreviewChart;

        /// <summary>
        /// Called when instance created ProeprtyWindow
        /// </summary>
        public PropertyWindow()
        {
            InitializeComponent();
            this.defaultTabs = new TabItemCollection();
            this.propertyTabs = new TabItemCollection();
        }

        void PropertyWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }  


        internal void SetChart( Chart _Chart)
        {
            this.PreviewChart = _Chart;
            this.DataContext = this.PreviewChart;
            //SetModels();
        }

        internal void AddTab(TabItem _Tab)
        {
            TabControl con = _Tab.Parent as TabControl;            
            if (con != null)
            {
                if (!ChartTabControl.Items.Contains(_Tab))
                {
                    con.Items.Remove(_Tab);
                    propertyTabs.Add(_Tab);
                    defaultTabs.Add(_Tab);
                }
            }
            else
            {
                TabItem conItem = _Tab as TabItem;
                if (!ChartTabControl.Items.Contains(_Tab))
                {
                    defaultTabs.Add(_Tab);
                    propertyTabs.Add(_Tab);
                }
            }
            ChartTabControl.ItemsSource = propertyTabs;
        }       
         private TabItemCollection defaultTabs;
        private TabItemCollection propertyTabs;
        internal void AddTab(TabItemCollection _Tab)
        {
           if(defaultTabs.Count>0)
                foreach (TabItem item in defaultTabs)
                {
                    if (!propertyTabs.Contains(item))
                        propertyTabs.Add(item);
                }
           if (_Tab.Count > 0)
                 foreach (TabItem item in _Tab)
                 {
                     if (!propertyTabs.Contains(item))
                         propertyTabs.Add(item);
                 }
           ChartTabControl.ItemsSource = propertyTabs;
        }

        //private void SetModels()
        //{
        //    /* Dynamically generates Model - inner models depending upon the Chart in preview.*/
        //    this.Model._ChartViewModel.ChartAreaViewModel = new ChartAreaViewModel();
        //    this.Model._ChartViewModel.ChartAreaViewModel.Models = new ObservableCollection<ChartAreaModel>();
        //    /* Need to initialize the values to the Model properties*/
        //    /* This is because the value from Source (Model) to the target (Controls) while setting Binding (Even for TwoWay)*/
        //    this.Model._ChartViewModel.Background = PreviewChart.Background == null ? Brushes.Transparent : PreviewChart.Background;
        //    this.Model._ChartViewModel.Foreground = PreviewChart.Foreground;
        //    this.Model._ChartViewModel.Margin = PreviewChart.Margin;
        //    this.Model._ChartViewModel.BorderBrush = PreviewChart.BorderBrush;
        //    this.Model._ChartViewModel.BorderThickness = PreviewChart.BorderThickness;
        //    this.Model._ChartViewModel.VisualStyle = PreviewChart.ChartVisualStyle;
        //    this.Model._ChartViewModel.CornerRadius = PreviewChart.CornerRadius;
        //    this.Model._ChartViewModel.Padding = PreviewChart.Padding;
        //    this.Model._ChartViewModel.VisualStyle = PreviewChart.ChartVisualStyle;
        //    if (PreviewChart.Header != null)
        //    {
        //        if (PreviewChart.Header.GetType() == typeof(string))
        //        {
        //            this.Model._ChartViewModel.Header = (string)PreviewChart.Header;
        //            PreviewChart.SetBinding(Chart.HeaderProperty, new Binding("Header") { Source = this.Model._ChartViewModel, Mode = BindingMode.TwoWay });
        //        }
        //        else if (PreviewChart.Header.GetType() == typeof(TextBlock))
        //        {
        //            this.Model._ChartViewModel.Header = (PreviewChart.Header as TextBlock).Text;
        //            (PreviewChart.Header as TextBlock).SetBinding(TextBlock.TextProperty, new Binding("Header") { Source = this.Model._ChartViewModel, Mode = BindingMode.TwoWay });
        //        }
        //        else if (PreviewChart.Header.GetType() == typeof(CheckBox))
        //        {
        //            this.Model._ChartViewModel.Header = (string)(PreviewChart.Header as CheckBox).Content;
        //            (PreviewChart.Header as CheckBox).SetBinding(CheckBox.ContentProperty, new Binding("Header") { Source = this.Model._ChartViewModel, Mode = BindingMode.TwoWay });
        //        }
        //        else if (PreviewChart.Header.GetType() == typeof(TextBox))
        //        {
        //            this.Model._ChartViewModel.Header = (string)(PreviewChart.Header as TextBox).Text;
        //            (PreviewChart.Header as TextBox).SetBinding(TextBox.TextProperty, new Binding("Header") { Source = this.Model._ChartViewModel, Mode = BindingMode.TwoWay });
        //        }
        //        else if (PreviewChart.Header.GetType() == typeof(Button))
        //        {
        //            this.Model._ChartViewModel.Header = (string)(PreviewChart.Header as Button).Content;
        //            (PreviewChart.Header as Button).SetBinding(Button.ContentProperty, new Binding("Header") { Source = this.Model._ChartViewModel, Mode = BindingMode.TwoWay });
        //        }
        //    }
        //    else 
        //    {
        //        PreviewChart.SetBinding(Chart.HeaderProperty, new Binding("Header") { Source = this.Model._ChartViewModel, Mode = BindingMode.TwoWay });
        //    }
        //    PreviewChart.SetBinding(Chart.BackgroundProperty, new Binding("Background") { Source = this.Model._ChartViewModel, Mode = BindingMode.TwoWay });
        //    PreviewChart.SetBinding(Chart.ForegroundProperty, new Binding("Foreground") { Source = this.Model._ChartViewModel, Mode = BindingMode.TwoWay });
        //    PreviewChart.SetBinding(Chart.BorderBrushProperty, new Binding("BorderBrush") { Source = this.Model._ChartViewModel, Mode = BindingMode.TwoWay });
        //    PreviewChart.SetBinding(Chart.BorderThicknessProperty, new Binding("BorderThickness") { Source = this.Model._ChartViewModel, Mode = BindingMode.TwoWay });
        //    PreviewChart.SetBinding(Chart.CornerRadiusProperty, new Binding("CornerRadius") { Source = this.Model._ChartViewModel, Mode = BindingMode.TwoWay });
        //    PreviewChart.SetBinding(Chart.MarginProperty, new Binding("Margin") { Source = this.Model._ChartViewModel, Mode = BindingMode.TwoWay });
        //    PreviewChart.SetBinding(Chart.PaddingProperty, new Binding("Padding") { Source = this.Model._ChartViewModel, Mode = BindingMode.TwoWay });
        //    PreviewChart.SetBinding(Chart.ChartVisualStyleProperty, new Binding("VisualStyle") { Source = this.Model._ChartViewModel, Mode = BindingMode.TwoWay });

        //    int LegendCounter = 1;
        //    this.Model._ChartViewModel.ChartLegendViewModel = new ChartLegendViewModel();
        //    this.Model._ChartViewModel.ChartLegendViewModel.Models = new ObservableCollection<ChartLegendModel>();
        //    foreach (ChartLegend legend in PreviewChart.Legends)
        //    {
        //        ChartLegendModel legendModel = new ChartLegendModel();
        //        legendModel.DisplayName = "Legend" + LegendCounter++.ToString();
        //        legendModel.Background = legend.Background;
        //        legendModel.BorderBrush = legend.BorderBrush;
        //        legendModel.BorderThickness = legend.BorderThickness;
        //        legendModel.CheckBoxVisibility = legend.CheckBoxVisibility;
        //        legendModel.CornerRadius = legend.CornerRadius;
        //        legendModel.Foreground = legend.Foreground;                             
        //        legendModel.IconVisibility = legend.IconVisibility;
        //        legendModel.Margin = legend.Margin;
        //        legendModel.Padding = legend.Padding;
        //        legendModel.ShowSymbol = legend.ShowSymbol;
        //        if (legend.Header != null)
        //        {
        //            if (legend.Header.GetType() == typeof(string))
        //            {
        //                legendModel.Header = (string)legend.Header;
        //                legend.SetBinding(Chart.HeaderProperty, new Binding("Header") { Source = legendModel, Mode = BindingMode.TwoWay });
        //            }
        //            else if (legend.Header.GetType() == typeof(TextBlock))
        //            {
        //                legendModel.Header = (legend.Header as TextBlock).Text;
        //                (legend.Header as TextBlock).SetBinding(TextBlock.TextProperty, new Binding("Header") { Source = legendModel, Mode = BindingMode.TwoWay });
        //            }
        //            else if (legend.Header.GetType() == typeof(CheckBox))
        //            {
        //                legendModel.Header = (string)(legend.Header as CheckBox).Content;
        //                (legend.Header as CheckBox).SetBinding(CheckBox.ContentProperty, new Binding("Header") { Source = legendModel, Mode = BindingMode.TwoWay });
        //            }
        //            else if (legend.Header.GetType() == typeof(TextBox))
        //            {
        //                legendModel.Header = (string)(legend.Header as TextBox).Text;
        //                (legend.Header as TextBox).SetBinding(TextBox.TextProperty, new Binding("Header") { Source = legendModel, Mode = BindingMode.TwoWay });
        //            }
        //            else if (legend.Header.GetType() == typeof(Button))
        //            {
        //                legendModel.Header = (string)(legend.Header as Button).Content;
        //                (legend.Header as Button).SetBinding(Button.ContentProperty, new Binding("Header") { Source = legendModel, Mode = BindingMode.TwoWay });
        //            }
        //        }
        //        else 
        //        {
        //            legend.SetBinding(Chart.HeaderProperty, new Binding("Header") { Source = legendModel, Mode = BindingMode.TwoWay });
        //        }
        //        legend.SetBinding(ChartLegend.BackgroundProperty, new Binding("Background") { Source = legendModel, Mode = BindingMode.TwoWay });
        //        legend.SetBinding(ChartLegend.BorderBrushProperty, new Binding("BorderBrush") { Source = legendModel, Mode = BindingMode.TwoWay });
        //        legend.SetBinding(ChartLegend.BorderThicknessProperty, new Binding("BorderThickness") { Source = legendModel, Mode = BindingMode.TwoWay });
        //        legend.SetBinding(ChartLegend.CheckBoxVisibilityProperty, new Binding("CheckBoxVisibility") { Source = legendModel, Mode = BindingMode.TwoWay });
        //        legend.SetBinding(ChartLegend.CornerRadiusProperty, new Binding("CornerRadius") { Source = legendModel, Mode = BindingMode.TwoWay });
        //        legend.SetBinding(ChartLegend.ForegroundProperty, new Binding("Foreground") { Source = legendModel, Mode = BindingMode.TwoWay });
        //        legend.SetBinding(ChartLegend.IconVisibilityProperty, new Binding("IconVisibility") { Source = legendModel, Mode = BindingMode.TwoWay });
        //        legend.SetBinding(ChartLegend.MarginProperty, new Binding("Margin") { Source = legendModel, Mode = BindingMode.TwoWay });
        //        legend.SetBinding(ChartLegend.PaddingProperty, new Binding("Padding") { Source = legendModel, Mode = BindingMode.TwoWay });
        //        legend.SetBinding(ChartLegend.ShowSymbolProperty, new Binding("ShowSymbol") { Source = legendModel, Mode = BindingMode.TwoWay });
        //        this.Model._ChartViewModel.ChartLegendViewModel.Models.Add(legendModel);
        //    }
        //    this.Model._ChartViewModel.ChartLegendViewModel.SelectedModel = this.Model._ChartViewModel.ChartLegendViewModel.Models.FirstOrDefault();
        //    int areaIndex = 1;
        //    foreach (ChartArea area in PreviewChart.Areas)
        //    {
        //        /* Creates one ChartAreaModel for each area in Chart.*/

        //        /* Need to initialize the values to the Model properties*/
        //        /* This is because the value from Source (Model) to the target (Controls) while setting Binding (Even for TwoWay)*/
        //        ChartAreaModel areamodel = new ChartAreaModel();
        //        areamodel.DisplayName = "Area" + (areaIndex++.ToString());
        //        areamodel.Background = Brushes.Transparent;
        //        areamodel.Foreground = area.Foreground;
        //        areamodel.BorderBrush = area.BorderBrush;
        //        areamodel.BorderThickness = area.BorderThickness;
        //        areamodel.Margin = area.Margin;
        //        areamodel.Padding = area.Padding;
        //        areamodel.GridBackground = area.GridBackground;
        //        areamodel.AlternateBackground = area.AlternatingGridBackground;
        //        areamodel.AlternatingFillMode = area.AlternatingFillMode;
        //        areamodel.AlternatingFillDirection = area.AlternatingFillDirection;
        //        areamodel.EnableZoomOnScroll = area.EnableZoomOnScroll;
        //        areamodel.EnableContextMenu = area.IsContextMenuEnabled;
        //        areamodel.CornerRadius = area.CornerRadius;
        //        if (area.Header != null)
        //        {
        //            if (area.Header.GetType() == typeof(string))
        //            {
        //                areamodel.Header = (string)area.Header;
        //                area.SetBinding(Chart.HeaderProperty, new Binding("Header") { Source = areamodel, Mode = BindingMode.TwoWay });
        //            }
        //            else if (area.Header.GetType() == typeof(TextBlock))
        //            {
        //                areamodel.Header = (area.Header as TextBlock).Text;
        //                (area.Header as TextBlock).SetBinding(TextBlock.TextProperty, new Binding("Header") { Source = areamodel, Mode = BindingMode.TwoWay });
        //            }
        //            else if (area.Header.GetType() == typeof(CheckBox))
        //            {
        //                areamodel.Header = (string)(area.Header as CheckBox).Content;
        //                (area.Header as CheckBox).SetBinding(CheckBox.ContentProperty, new Binding("Header") { Source = areamodel, Mode = BindingMode.TwoWay });
        //            }
        //            else if (area.Header.GetType() == typeof(TextBox))
        //            {
        //                areamodel.Header = (string)(area.Header as TextBox).Text;
        //                (area.Header as TextBox).SetBinding(TextBox.TextProperty, new Binding("Header") { Source = areamodel, Mode = BindingMode.TwoWay });
        //            }
        //            else if (area.Header.GetType() == typeof(Button))
        //            {
        //                areamodel.Header = (string)(area.Header as Button).Content;
        //                (area.Header as Button).SetBinding(Button.ContentProperty, new Binding("Header") { Source = areamodel, Mode = BindingMode.TwoWay });
        //            }
        //        }
        //        else 
        //        {
        //            area.SetBinding(Chart.HeaderProperty, new Binding("Header") { Source = areamodel, Mode = BindingMode.TwoWay });
        //        }
        //        area.SetBinding(ChartArea.BackgroundProperty, new Binding("Background") { Source = areamodel, Mode = BindingMode.TwoWay });
        //        area.SetBinding(ChartArea.ForegroundProperty, new Binding("Foreground") { Source = areamodel, Mode = BindingMode.TwoWay });
        //        area.SetBinding(ChartArea.BorderBrushProperty, new Binding("BorderBrush") { Source = areamodel, Mode = BindingMode.TwoWay });
        //        area.SetBinding(ChartArea.BorderThicknessProperty, new Binding("BorderThickness") { Source = areamodel, Mode = BindingMode.TwoWay });
        //        area.SetBinding(ChartArea.CornerRadiusProperty, new Binding("CornerRadius") { Source = areamodel, Mode = BindingMode.TwoWay });
        //        area.SetBinding(ChartArea.MarginProperty, new Binding("Margin") { Source = areamodel, Mode = BindingMode.TwoWay });
        //        area.SetBinding(ChartArea.PaddingProperty, new Binding("Padding") { Source = areamodel, Mode = BindingMode.TwoWay });
        //        area.SetBinding(ChartArea.GridBackgroundProperty, new Binding("GridBackground") { Source = areamodel, Mode = BindingMode.TwoWay });
        //        area.SetBinding(ChartArea.AlternatingGridBackgroundProperty, new Binding("AlternateBackground") { Source = areamodel, Mode = BindingMode.TwoWay });
        //        area.SetBinding(ChartArea.AlternatingFillModeProperty, new Binding("AlternatingFillMode") { Source = areamodel, Mode = BindingMode.TwoWay });
        //        area.SetBinding(ChartArea.AlternatingFillDirectionProperty, new Binding("AlternatingFillDirection") { Source = areamodel, Mode = BindingMode.TwoWay });
        //        area.SetBinding(ChartArea.EnableZoomOnScrollProperty, new Binding("EnableZoomOnScroll") { Source = areamodel, Mode = BindingMode.TwoWay });
        //        area.SetBinding(ChartArea.IsContextMenuEnabledProperty, new Binding("EnableContextMenu") { Source = areamodel, Mode = BindingMode.TwoWay });

        //        areamodel.SeriesModel = new ChartSeriesViewModel();
        //        areamodel.SeriesModel.Models = new ObservableCollection<ChartSeriesModel>();

        //        foreach (ChartSeries series in area.Series)
        //        {

        //            /* Creates one ChartSeriesModel for each area in area.*/

        //            /* Need to initialize the values to the Model properties*/
        //            /* This is because the value from Source (Model) to the target (Controls) while setting Binding (Even for TwoWay)*/
        //            ChartSeriesModel seriesmodel = new ChartSeriesModel();
        //            seriesmodel.DisplayName = "Series" + (area.Series.IndexOf(series).ToString());
        //            seriesmodel.InteriorBrush = series.Interior;
        //            seriesmodel.StrokeBrush = series.Stroke;
        //            seriesmodel.StrokeThickness = series.StrokeThickness;
        //            seriesmodel.Type = series.Type;
        //            seriesmodel.IsVisibleOnLegend = series.IsVisibleOnLegend;
        //            seriesmodel.LegendLabel = series.Label;
        //            seriesmodel.LegendIcon = series.LegendIcon;
        //            seriesmodel.IsZoomable = series.IsZoomable;
        //            seriesmodel.IsSorted = series.IsSortData;
        //            seriesmodel.ShowEmptyPoint = series.ShowEmptyPoints;
        //            seriesmodel.EmptyPointInterior = series.EmptyPointInterior;
        //            seriesmodel.EmptyPointStyle = series.EmptyPointStyle;
        //            seriesmodel.EnableAnimation = series.EnableAnimation;
        //            seriesmodel.AnimateOneByOne = series.AnimateOneByOne;
        //            seriesmodel.AnimationOptions = series.AnimateOption;                    
        //            series.SetBinding(ChartSeries.InteriorProperty, new Binding("InteriorBrush") { Source = seriesmodel, Mode = BindingMode.TwoWay });
        //            series.SetBinding(ChartSeries.StrokeProperty, new Binding("StrokeBrush") { Source = seriesmodel, Mode = BindingMode.TwoWay });
        //            series.SetBinding(ChartSeries.StrokeThicknessProperty, new Binding("StrokeThickness") { Source = seriesmodel, Mode = BindingMode.TwoWay });
        //            series.SetBinding(ChartSeries.TypeProperty, new Binding("Type") { Source = seriesmodel, Mode = BindingMode.TwoWay });
        //            series.SetBinding(ChartSeries.IsVisibleOnLegendProperty, new Binding("IsVisibleOnLegend") { Source = seriesmodel, Mode = BindingMode.TwoWay });
        //            series.SetBinding(ChartSeries.LabelProperty, new Binding("LegendLabel") { Source = seriesmodel, Mode = BindingMode.TwoWay });
        //            series.SetBinding(ChartSeries.LegendIconProperty, new Binding("LegendIcon") { Source = seriesmodel, Mode = BindingMode.TwoWay });
        //            series.SetBinding(ChartSeries.IsZoomableProperty, new Binding("IsZoomable") { Source = seriesmodel, Mode = BindingMode.TwoWay });
        //            series.SetBinding(ChartSeries.IsRotatedProperty, new Binding("IsRotated") { Source = seriesmodel, Mode = BindingMode.TwoWay });
        //            series.SetBinding(ChartSeries.IsSortDataProperty, new Binding("IsSorted") { Source = seriesmodel, Mode = BindingMode.TwoWay });                    
        //            series.SetBinding(ChartSeries.ShowEmptyPointsProperty, new Binding("ShowEmptyPoint") { Source = seriesmodel, Mode = BindingMode.TwoWay });
        //            series.SetBinding(ChartSeries.EmptyPointInteriorProperty, new Binding("EmptyPointInterior") { Source = seriesmodel, Mode = BindingMode.TwoWay });
        //            series.SetBinding(ChartSeries.EmptyPointStyleProperty, new Binding("EmptyPointStyle") { Source = seriesmodel, Mode = BindingMode.TwoWay });
        //            series.SetBinding(ChartSeries.AnimateOneByOneProperty, new Binding("AnimateOneByOne") { Source = seriesmodel, Mode = BindingMode.TwoWay });
        //            series.SetBinding(ChartSeries.AnimateOptionProperty, new Binding("AnimationOptions") { Source = seriesmodel, Mode = BindingMode.TwoWay });
        //            series.SetBinding(ChartSeries.EnableAnimationProperty, new Binding("EnableAnimation") { Source = seriesmodel, Mode = BindingMode.TwoWay });
        //            areamodel.SeriesModel.Models.Add(seriesmodel);
        //        }
        //        areamodel.SeriesModel.SelectedModel = areamodel.SeriesModel.Models.FirstOrDefault();

        //        areamodel.AxisModel = new ChartAxisViewModel();
        //        areamodel.AxisModel.Models = new ObservableCollection<ChartAxisModel>();
        //        int axiscount = 1;
        //        foreach (ChartAxis axis in area.Axes)
        //        {

        //            /* Creates one ChartSeriesModel for each area in area.*/

        //            /* Need to initialize the values to the Model properties*/
        //            /* This is because the value from Source (Model) to the target (Controls) while setting Binding (Even for TwoWay)*/
        //            ChartAxisModel axismodel = new ChartAxisModel();
        //            axismodel.DisplayName = "Axis" + ((axiscount++).ToString());
        //            axismodel.DateTimeInterval = axis.DateTimeInterval;
        //            axismodel.DateTimeRange = axis.DateTimeRange;
        //            axismodel.DesiredIntervalCount = axis.DesiredIntervalsCount;
        //            axismodel.EdgeLabelDrawingMode = axis.EdgeLabelsDrawingMode;
        //            axismodel.EnableZooming = axis.EnableZooming;
        //            axismodel.HeaderAlignment = axis.HeaderAlignment;
        //            axismodel.HidePartialLabels = axis.HidePartialLabel;
        //            axismodel.IntersectAction = axis.IntersectAction;
        //            axismodel.Interval = axis.Interval;
        //            axismodel.IsAutoSetRange = axis.IsAutoSetRange;
        //            axismodel.IsLograthimic = axis.IsLogarithmic;
        //            axismodel.LabelBackground = axis.LabelBackground;
        //            axismodel.LabelBorderBrush = axis.LabelBorderBrush;
        //            axismodel.RangeCalulationMode = axis.RangeCalculationMode;
        //            axismodel.RangePadding = axis.RangePadding;
        //            axismodel.SmallTickLinesStroke = axis.SmallTickLineStroke.Brush;
        //            axismodel.SmallTickSize = axis.SmallTickSize;
        //            axismodel.SmallTicksPerInterval = axis.SmallTicksPerInterval;
        //            axismodel.TickLineStroke = axis.TickLineStroke.Brush;
        //            axismodel.TickSize = axis.TickSize;
        //            axismodel.ValueType = axis.ValueType;
        //            axismodel.LabelBorderThickness = axis.LabelBorderThickness;
        //            axismodel.LabelCornerRadius = axis.LabelCornerRadius;
        //            axismodel.LabelDateTimeFormat = axis.LabelDateTimeFormat;
        //            axismodel.LabelForeground = axis.LabelForeground;
        //            axismodel.LabelFormat = axis.LabelFormat;
        //            axismodel.LabelRotateAngle = axis.LabelRotateAngle;
        //            axismodel.LabelsMode = axis.LabelsMode;
        //            axismodel.LineStroke = axis.LineStroke.Brush;
        //            axismodel.LograthimicBase = axis.LogarithmicBase;
        //            axismodel.LograthimicRange = axis.LogarithmicRange;
        //            axismodel.OpposedPosition = axis.OpposedPosition;
        //            axismodel.Orientation = axis.Orientation;
        //            axismodel.Origin = axis.Origin;
        //            axismodel.Range = axis.Range;
        //            //SetHeader(axis.Header, (FrameworkElement)axis, ChartAxis.HeaderProperty);
        //            if (axis.Header != null)
        //            {
        //                if (axis.Header.GetType() == typeof(string))
        //                {
        //                    axismodel.Header = (string)axis.Header;
        //                    axis.SetBinding(Chart.HeaderProperty, new Binding("Header") { Source = axismodel, Mode = BindingMode.TwoWay });
        //                }
        //                else if (axis.Header.GetType() == typeof(TextBlock))
        //                {
        //                    axismodel.Header = (axis.Header as TextBlock).Text;
        //                    (axis.Header as TextBlock).SetBinding(TextBlock.TextProperty, new Binding("Header") { Source = axismodel, Mode = BindingMode.TwoWay });
        //                }
        //                else if (axis.Header.GetType() == typeof(CheckBox))
        //                {
        //                    axismodel.Header = (string)(axis.Header as CheckBox).Content;
        //                    (axis.Header as CheckBox).SetBinding(CheckBox.ContentProperty, new Binding("Header") { Source = axismodel, Mode = BindingMode.TwoWay });
        //                }
        //                else if (axis.Header.GetType() == typeof(TextBox))
        //                {
        //                    axismodel.Header = (string)(axis.Header as TextBox).Text;
        //                    (axis.Header as TextBox).SetBinding(TextBox.TextProperty, new Binding("Header") { Source = axismodel, Mode = BindingMode.TwoWay });
        //                }
        //                else if (axis.Header.GetType() == typeof(Button))
        //                {
        //                    axismodel.Header = (string)(axis.Header as Button).Content;
        //                    (axis.Header as Button).SetBinding(Button.ContentProperty, new Binding("Header") { Source = axismodel, Mode = BindingMode.TwoWay });
        //                }
        //            }
        //            else
        //            {
        //                axis.SetBinding(Chart.HeaderProperty, new Binding("Header") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            }
        //            axis.SetBinding(ChartAxis.DateTimeIntervalProperty, new Binding("DateTimeInterval") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.DateTimeRangeProperty, new Binding("DateTimeRange") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.DesiredIntervalsCountProperty, new Binding("DesiredIntervalCount") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.EdgeLabelsDrawingModeProperty, new Binding("EdgeLabelDrawingMode") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.EnableZoomingProperty, new Binding("EnableZooming") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.HeaderAlignmentProperty, new Binding("HeaderAlignment") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.HidePartialLabelProperty, new Binding("HidePartialLabels") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.IntersectActionProperty, new Binding("IntersectAction") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.IntervalProperty, new Binding("Interval") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.IsAutoSetRangeProperty, new Binding("IsAutoSetRange") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.IsLogarithmicProperty, new Binding("IsLograthimic") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.LabelBackgroundProperty, new Binding("LabelBackground") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.LabelBorderBrushProperty, new Binding("LabelBorderBrush") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.RangeCalculationModeProperty, new Binding("RangeCalulationMode") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.RangePaddingProperty, new Binding("RangePadding") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.SmallTickLineStrokeProperty, new Binding("SmallTickLinesStroke") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.SmallTickSizeProperty, new Binding("SmallTickSize") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.SmallTicksPerIntervalProperty, new Binding("SmallTicksPerInterval") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.TickLineStrokeProperty, new Binding("TickLineStroke") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.TickSizeProperty, new Binding("TickSize") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.ValueTypeProperty, new Binding("ValueType") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.LabelBorderThicknessProperty, new Binding("LabelBorderThickness") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.LabelCornerRadiusProperty, new Binding("LabelCornerRadius") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.LabelDateTimeFormatProperty, new Binding("LabelDateTimeFormat") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.LabelForegroundProperty, new Binding("LabelForeground") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.LabelFormatProperty, new Binding("LabelFormat") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.LabelRotateAngleProperty, new Binding("LabelRotateAngle") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.LabelsModeProperty, new Binding("LabelsMode") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.LogarithmicBaseProperty, new Binding("LograthimicBase") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.LogarithmicRangeProperty, new Binding("LograthimicRange") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.OpposedPositionProperty, new Binding("OpposedPosition") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.OrientationProperty, new Binding("Orientation") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.OriginProperty, new Binding("Origin") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            axis.SetBinding(ChartAxis.RangeProperty, new Binding("Range") { Source = axismodel, Mode = BindingMode.TwoWay });
        //            areamodel.AxisModel.Models.Add(axismodel);
        //        }
        //        areamodel.AxisModel.SelectedModel = areamodel.AxisModel.Models.FirstOrDefault();


        //        if (area.Legend != null)
        //        {
        //            ChartLegendModel legendModel = new ChartLegendModel();
        //            legendModel.Background = area.Legend.Background;
        //            legendModel.BorderBrush = area.Legend.BorderBrush;
        //            legendModel.BorderThickness = area.Legend.BorderThickness;
        //            legendModel.CheckBoxVisibility = area.Legend.CheckBoxVisibility;
        //            legendModel.CornerRadius = area.Legend.CornerRadius;
        //            legendModel.Foreground = area.Legend.Foreground;
        //            legendModel.IconVisibility = area.Legend.IconVisibility;
        //            legendModel.Margin = area.Legend.Margin;
        //            legendModel.Padding = area.Legend.Padding;
        //            legendModel.ShowSymbol = area.Legend.ShowSymbol;
        //            if (area.Legend.Header != null)
        //            {
        //                if (area.Legend.Header.GetType() == typeof(string))
        //                {
        //                    legendModel.Header = (string)area.Legend.Header;
        //                    area.Legend.SetBinding(Chart.HeaderProperty, new Binding("Header") { Source = legendModel, Mode = BindingMode.TwoWay });
        //                }
        //                else if (area.Legend.Header.GetType() == typeof(TextBlock))
        //                {
        //                    legendModel.Header = (area.Legend.Header as TextBlock).Text;
        //                    (area.Legend.Header as TextBlock).SetBinding(TextBlock.TextProperty, new Binding("Header") { Source = legendModel, Mode = BindingMode.TwoWay });
        //                }
        //                else if (area.Legend.Header.GetType() == typeof(CheckBox))
        //                {
        //                    legendModel.Header = (string)(area.Legend.Header as CheckBox).Content;
        //                    (area.Legend.Header as CheckBox).SetBinding(CheckBox.ContentProperty, new Binding("Header") { Source = legendModel, Mode = BindingMode.TwoWay });
        //                }
        //                else if (area.Legend.Header.GetType() == typeof(TextBox))
        //                {
        //                    legendModel.Header = (string)(area.Legend.Header as TextBox).Text;
        //                    (area.Legend.Header as TextBox).SetBinding(TextBox.TextProperty, new Binding("Header") { Source = legendModel, Mode = BindingMode.TwoWay });
        //                }
        //                else if (area.Legend.Header.GetType() == typeof(Button))
        //                {
        //                    legendModel.Header = (string)(area.Legend.Header as Button).Content;
        //                    (area.Legend.Header as Button).SetBinding(Button.ContentProperty, new Binding("Header") { Source = legendModel, Mode = BindingMode.TwoWay });
        //                }
        //            }
        //            else 
        //            {
        //                area.Legend.SetBinding(Chart.HeaderProperty, new Binding("Header") { Source = legendModel, Mode = BindingMode.TwoWay });
        //            }
        //            area.Legend.SetBinding(ChartLegend.BackgroundProperty, new Binding("Background") { Source = legendModel, Mode = BindingMode.TwoWay });
        //            area.Legend.SetBinding(ChartLegend.BorderBrushProperty, new Binding("BorderBrush") { Source = legendModel, Mode = BindingMode.TwoWay });
        //            area.Legend.SetBinding(ChartLegend.BorderThicknessProperty, new Binding("BorderThickness") { Source = legendModel, Mode = BindingMode.TwoWay });
        //            area.Legend.SetBinding(ChartLegend.CheckBoxVisibilityProperty, new Binding("CheckBoxVisibility") { Source = legendModel, Mode = BindingMode.TwoWay });
        //            area.Legend.SetBinding(ChartLegend.CornerRadiusProperty, new Binding("CornerRadius") { Source = legendModel, Mode = BindingMode.TwoWay });
        //            area.Legend.SetBinding(ChartLegend.ForegroundProperty, new Binding("Foreground") { Source = legendModel, Mode = BindingMode.TwoWay });
        //            area.Legend.SetBinding(ChartLegend.IconVisibilityProperty, new Binding("IconVisibility") { Source = legendModel, Mode = BindingMode.TwoWay });
        //            area.Legend.SetBinding(ChartLegend.MarginProperty, new Binding("Margin") { Source = legendModel, Mode = BindingMode.TwoWay });
        //            area.Legend.SetBinding(ChartLegend.PaddingProperty, new Binding("Padding") { Source = legendModel, Mode = BindingMode.TwoWay });
        //            area.Legend.SetBinding(ChartLegend.ShowSymbolProperty, new Binding("ShowSymbol") { Source = legendModel, Mode = BindingMode.TwoWay });
        //            areamodel.LegendModel = legendModel;
        //        }


        //        foreach (ChartAxis axis in area.Axes)
        //        {
        //            ChartAxisModel axismodel = new ChartAxisModel();
        //        }
        //        this.Model._ChartViewModel.ChartAreaViewModel.Models.Add(areamodel);
                
        //    }
        //    this.Model._ChartViewModel.ChartAreaViewModel.SelectedModel = this.Model._ChartViewModel.ChartAreaViewModel.Models.FirstOrDefault();
            
        //}

        //private void SetHeader(object header, FrameworkElement ele, DependencyProperty prop, object source)
        //{
        //    if (header != null)
        //    {
        //        if (header.GetType() == typeof(TextBlock))
        //        {
        //            this.Model._ChartViewModel.Header = (header as TextBlock).Text;
        //            (header as TextBlock).SetBinding(TextBlock.TextProperty, new Binding("Header") { Source = source, Mode = BindingMode.TwoWay });
        //        }
        //        else if (header.GetType() == typeof(Button))
        //        {
        //            this.Model._ChartViewModel.Header = (header as Button).Content.ToString();
        //            (header as Button).SetBinding(Button.ContentProperty, new Binding("Header") { Source = source, Mode = BindingMode.TwoWay });
        //        }
        //        else if (header.GetType() == typeof(TextBox))
        //        {
        //            this.Model._ChartViewModel.Header = (header as TextBox).Text;
        //            (header as TextBox).SetBinding(TextBox.TextProperty, new Binding("Header") { Source = source, Mode = BindingMode.TwoWay });
        //        }
        //        else if (header.GetType() == typeof(CheckBox))
        //        {
        //            this.Model._ChartViewModel.Header = (header as CheckBox).Content.ToString();
        //            (header as CheckBox).SetBinding(CheckBox.ContentProperty, new Binding("Header") { Source = source, Mode = BindingMode.TwoWay });
        //        }
        //        else if (header.GetType() == typeof(string))
        //        {
        //            this.Model._ChartViewModel.Header = header.ToString();
        //            ele.SetBinding(prop, new Binding("Header") { Source = source, Mode = BindingMode.TwoWay });
        //        }
        //    }
        //    else 
        //    {
        //        ele.SetBinding(prop, new Binding("Header") { Source = this.Model._ChartViewModel, Mode = BindingMode.TwoWay });
        //    }
        //}

            
    }

    /// <summary>
    /// Return String value from the given value
    /// </summary>
    public class PropertyDialogHeaderConverter : IValueConverter, IDisposable
    {
        #region IValueConverter Members

        private object TypeOfConent;

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string returnval = string.Empty;
            TypeOfConent = value;
            if (value != null)
            {
                    if (value.GetType() == typeof(string))
                    {
                        returnval = value.ToString();
                    }
                    else if (value.GetType() == typeof(TextBlock))
                    {
                        returnval = (value as TextBlock).Text;                        
                    }
                    else if (value.GetType() == typeof(CheckBox)) { return (value as CheckBox).Content; }                    
                    else if (value.GetType() == typeof(TextBox)) { return (value as TextBox).Text;}
                    else if (value.GetType() == typeof(Button)) { return (value as Button).Content; }                    
                }


            return returnval;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (TypeOfConent == null) 
            {
                return value.ToString();
            }
            else if (TypeOfConent.GetType() == typeof(string)) { return value.ToString(); }
            else if (TypeOfConent.GetType() == typeof(TextBlock)) { (TypeOfConent as TextBlock).Text = value.ToString(); }
            else if (TypeOfConent.GetType() == typeof(CheckBox)) { (TypeOfConent as CheckBox).Content = value.ToString(); }
            else if (TypeOfConent.GetType() == typeof(TextBox)) { (TypeOfConent as TextBox).Text = value.ToString(); }
            else if (TypeOfConent.GetType() == typeof(Button)) { (TypeOfConent as Button).Content = value.ToString(); }
            return TypeOfConent; 
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.TypeOfConent = null;
        }
        #endregion
    }

   
}
