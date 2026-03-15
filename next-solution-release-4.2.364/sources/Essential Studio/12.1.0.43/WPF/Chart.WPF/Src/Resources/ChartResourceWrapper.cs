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
using System.Globalization;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartResourceWrapper
    /// </summary>
    public class ChartResourceWrapper
    {
        /// <summary>
        /// Called when instance created for ChartResourceWrapper
        /// </summary>
        public ChartResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;

            legendWindowTitle = SR.GetString(ci, LegendWindowTitleValue);
            legendWindowIcon = SR.GetString(ci, LegendWindowIconValue);
            legendWindowCheckBox = SR.GetString(ci, LegendWindowCheckBoxValue);
            legendWindowOK = SR.GetString(ci, LegendWindowOKValue);
            legendWindowCancel = SR.GetString(ci, LegendWindowCancelValue);

            contextMenuZooming = SR.GetString(ci, ContextMenuZoomingValue);
            contextMenuZoomAll = SR.GetString(ci, ContextMenuZoomAllValue);
            contextMenuSeries = SR.GetString(ci, ContextMenuSeriesValue);
            contextMenuPalettes = SR.GetString(ci, ContextMenuPalettesValue);
            printDialogPrint = SR.GetString(ci, PrintDialogPrintValue);
            printDialogPrintMode = SR.GetString(ci, PrintDialogPrintModeValue);
            style = SR.GetString(ci, StyleValue);

            analog = SR.GetString(ci, AnalogValue);
            area= SR.GetString(ci, AreaValue);
            bar= SR.GetString(ci, BarValue);
            boxAndWhisker= SR.GetString(ci, BoxAndWhiskerValue);
            bubble= SR.GetString(ci, BubbleValue);
            candle= SR.GetString(ci, CandleValue);
            colorful= SR.GetString(ci, ColorfulValue);
            column= SR.GetString(ci, ColumnValue);
            custom= SR.GetString(ci, CustomValue);
            m_default= SR.GetString(ci, DefaultValue);
            defaultAlpha= SR.GetString(ci, DefaultAlphaValue);
            defaultDark = SR.GetString(ci, DefaultDarkValue);
            doughnut= SR.GetString(ci, DoughnutValue);
            earthTone= SR.GetString(ci, EarthToneValue);
            fastLine = SR.GetString(ci, FastLineValue);
            funnel= SR.GetString(ci, FunnelValue);
            gantt= SR.GetString(ci, GanttValue);
            gradient= SR.GetString(ci, GradientValue);
            grayscale= SR.GetString(ci, GrayscaleValue);
            hiLo= SR.GetString(ci, HiLoValue);
            hiLoArea= SR.GetString(ci, HiLoAreaValue);
            hiLoOpenClose= SR.GetString(ci, HiLoOpenCloseValue);
            histogram = SR.GetString(ci, HistogramValue);
            kagi= SR.GetString(ci, KagiValue);
            line= SR.GetString(ci, LineValue);
            nature= SR.GetString(ci, NatureValue);
            office2007Black= SR.GetString(ci, Office2007BlackValue);
            office2007Blue= SR.GetString(ci, Office2007BlueValue);
            office2007Silver= SR.GetString(ci, Office2007SilverValue);
            MixedGray= SR.GetString(ci, MixedGrayValue);
            BlueScale= SR.GetString(ci, BlueScaleValue);
            MaroonRed= SR.GetString(ci, MaroonRedValue);
            GreenScale= SR.GetString(ci, GreenScaleValue);
            MixedViolet= SR.GetString(ci, MixedVioletValue);
            CoolBlueScale= SR.GetString(ci, CoolBlueScaleValue);
            ChocolateOrange= SR.GetString(ci, ChocolateOrangeValue);
            MixedFantasy= SR.GetString(ci, MixedFantasyValue);
            metroTheme = SR.GetString(ci, MetroThemeValue);
            pastel= SR.GetString(ci, PastelValue);
            pie= SR.GetString(ci, PieValue);
            pointAndFigure= SR.GetString(ci, PointAndFigureValue);
            polar= SR.GetString(ci, PolarValue);
            m_printDialogAdvanced= SR.GetString(ci, PrintDialogAdvancedValue);
            m_printDialogBWMode = SR.GetString(ci, PrintDialogBWModeValue);
            printDialogCancel= SR.GetString(ci, PrintDialogCancelValue);
            printDialogColorMode = SR.GetString(ci, PrintDialogColorModeValue);
            printDialogPrint = SR.GetString(ci, PrintDialogPrintValue);
            printDialogPrintStretch= SR.GetString(ci, PrintDialogPrintStretchValue);
            pyramid= SR.GetString(ci, PyramidValue);
            radar= SR.GetString(ci, RadarValue);
            rangeArea= SR.GetString(ci, RangeAreaValue);
            rangeColumn= SR.GetString(ci, RangeColumnValue);
            renko= SR.GetString(ci, RenkoValue);
            rotatedSpline= SR.GetString(ci, RotatedSplineValue);
            scatter= SR.GetString(ci, ScatterValue);
            spline = SR.GetString(ci, SplineValue);
            splineArea= SR.GetString(ci, SplineAreaValue);
            stackingArea= SR.GetString(ci, StackingAreaValue);
            stackingArea100 = SR.GetString(ci, StackingArea100Value);
            stackingBar= SR.GetString(ci, StackingBarValue);
            stackingBar100= SR.GetString(ci, StackingBar100Value);
            stackingLine = SR.GetString(ci, StackingLineValue);
            stackingLine100 = SR.GetString(ci, StackingLine100Value);
            stackingSpline = SR.GetString(ci, StackingSplineValue);
            stackingSpline100 = SR.GetString(ci, StackingSpline100Value);
            stackingSplineArea = SR.GetString(ci, StackingSplineAreaValue);
            stackingSplineArea100 = SR.GetString(ci, StackingSplineArea100Value);
            stackingColumn= SR.GetString(ci, StackingColumnValue);
            stackingColumn100= SR.GetString(ci, StackingColumn100Value);
            stepArea= SR.GetString(ci, StepAreaValue);
            stepLine= SR.GetString(ci, StepLineValue);
            surface3D = SR.GetString(ci, Surface3DValue);
            threeLineBreak= SR.GetString(ci, ThreeLineBreakValue);
            tornado= SR.GetString(ci, TornadoValue);
            triad = SR.GetString(ci, TriadValue);
            warmCold = SR.GetString(ci, WarmColdValue);

            fastColumn = SR.GetString(ci, FastColumnValue);
            fastScatter = SR.GetString(ci, FastScatterValue);
            fastStackingColumn = SR.GetString(ci, FastStackingColumnValue);
            fastHiLoOpenClose = SR.GetString(ci, FastHiLoOpenCloseValue);
            fastBar = SR.GetString(ci, FastBarValue);
            zoomIn = SR.GetString(ci, ZoomInValue);
            zoomOut = SR.GetString(ci, ZoomOutValue);
            resetZoom = SR.GetString(ci, ResetZoomValue);
            panning = SR.GetString(ci, PanningValue);
            Close = SR.GetString(ci, CloseValue);
            ChartPropertiesDialogTitle = SR.GetString(ci, ChartPropertiesDialogTitleValue);
            toolbarPrint = SR.GetString(ci, ToolbarPrintValue);
            ToolbarSwitchPrint = SR.GetString(ci, ToolbarSwitchPrintValue);
            Save = SR.GetString(ci, SaveValue);
            Copy = SR.GetString(ci, CopyValue);
            ToolbarLegend = SR.GetString(ci, ToolbarLegendValue);
            EnableZooming = SR.GetString(ci, EnableZoomingValue);
            ColorPalette = SR.GetString(ci, ColorPaletteValue);
            ChangeType = SR.GetString(ci, ChangeTypeValue);
            Properties = SR.GetString(ci, PropertiesValue);
            Header = SR.GetString(ci, HeaderValue);
            Appearance = SR.GetString(ci, AppearenceValue);
            Background= SR.GetString(ci, BackgroundValue);
            BorderBrush = SR.GetString(ci, BorderBrushValue);
            BorderThickness = SR.GetString(ci, BorderThicknessValue);
            CornerRadius = SR.GetString(ci, CornerRadiusValue);
            Foreground = SR.GetString(ci, ForegroundValue);
            Margin = SR.GetString(ci, MarginValue);
            Padding = SR.GetString(ci, PaddingValue);
            VisualStyle = SR.GetString(ci, VisualStyleValue);
            ChartVisualStyle = SR.GetString(ci, ChartVisualStyleValue);
            ChartArea = SR.GetString(ci, ChartAreaValue);
            SelectChartArea = SR.GetString(ci, SelectChartAreaValue);
            GridRegionProperties = SR.GetString(ci, GridRegionPropertiesValue);
            GridBackground = SR.GetString(ci, GridBackgroundValue);
            AlternativeGridBackground= SR.GetString(ci, AlternatingGridBackgroundValue);
            AlternatingBackgoundDirection = SR.GetString(ci, alternatingGridBackgroundModeValue);
            AlternatingBackgoundMode = SR.GetString(ci, AlternatingGridBackgroundDirectionValue);
            InteractiveFeatures = SR.GetString(ci, InteractiveFeaturesValue);
            EnableZoomOnScroll = SR.GetString(ci, EnableZoomOnScrollValue);
            EnableContextMenu = SR.GetString(ci, EnableContextMenuValue);
            ChartAreaLegend = SR.GetString(ci, ChartAreaLegendValue);
            CheckBoxVisibility = SR.GetString(ci, CheckBoxVisibilityValue);
            IconVisibility = SR.GetString(ci, IconVisibilityValue);
            ShowSymbol = SR.GetString(ci, ShowSymbolValue);   
            ChartSeries = SR.GetString(ci, ChartSeriesValue);
            SelectChartSeries = SR.GetString(ci, SelectChartSeriesValue);
            Interior = SR.GetString(ci, InteriorValue);
            Stroke = SR.GetString(ci, StrokeValue);
            StrokeThickness = SR.GetString(ci, StrokeThicknessValue);
            Type = SR.GetString(ci, TypeValue);
            Legend = SR.GetString(ci, LegendValue);
            IsVisibleOnLegend = SR.GetString(ci, IsVisibleOnLegendValue);
            LegendLabel = SR.GetString(ci, LegendLabelValue);
            LegendIcon = SR.GetString(ci, LegendIconValue);
            Data = SR.GetString(ci, DataValue);
            IsZoomable = SR.GetString(ci, IsZoomableValue);
            IsRotated = SR.GetString(ci, IsRotatedValue);
            IsSorted = SR.GetString(ci, IsSortedValue);
            ShowSeriesEmptyPoints = SR.GetString(ci, ShowSeriesEmptyPointsValue);
            EmptyPointsInterior = SR.GetString(ci, EmptyPointsInteriorValue);
            EmptyPointsStyle = SR.GetString(ci, EmptyPointsStyleValue);
            AnimationOption = SR.GetString(ci, AnimationOptionValue);
            AnimateSeriesonebyone = SR.GetString(ci, AnimateSeriesonebyoneValue);
            EnableAnimation = SR.GetString(ci, EnableAnimationValue);
            ChartAxis = SR.GetString(ci, ChartAxisValue);
            SelectChartAxis = SR.GetString(ci, SelectChartAxisValue);
            ValueType = SR.GetString(ci, ValueTypeValue);
            OpposedPosition = SR.GetString(ci, OpposedPositionValue);
            Orientation = SR.GetString(ci, OrientationValue);
            EnableZooming = SR.GetString(ci, EnableZoomingValue);
            HeaderAlignment = SR.GetString(ci, HeaderAlignmentValue);
            HeaderSettings = SR.GetString(ci, HeaderSettingsValue);
            Origin = SR.GetString(ci, OriginValue);
            AutoSetRange = SR.GetString(ci, AutoSetRangeValue);
            Range = SR.GetString(ci, RangeValue);
            RangeCalculationMode = SR.GetString(ci, RangeCalculationModeValue);
            RangePadding = SR.GetString(ci, RangePaddingValue);
            ChartType = SR.GetString(ci, ChartTypeValue);
            ChartTypeHeader = SR.GetString(ci, ChartTypeHeaderValue);
            Interval = SR.GetString(ci, IntervalValue);
            DesiredIntervalCount = SR.GetString(ci, DesiredIntervalCountValue);
            LabelBackground = SR.GetString(ci, LabelBackgroundValue);
            LabelForeground = SR.GetString(ci, LabelForegroundValue);
            LabelBorderBrush = SR.GetString(ci, LabelBorderBrushValue);
            LabelBorderThickness = SR.GetString(ci, LabelBorderThicknessValue);
            LabelCornerRadius = SR.GetString(ci, LabelCornerRadiusValue);
            HidePartialLabels = SR.GetString(ci, HidePartialLabelsValue);
            IntersectAction = SR.GetString(ci, IntersectActionValue);
            LabelFormat = SR.GetString(ci, LabelFormatValue);
            LabelRotateAngle = SR.GetString(ci, LabelRotateAngleValue);
            LabelsMode = SR.GetString(ci, LabelsModeValue);
            EdgeLabelDrawingMode = SR.GetString(ci, EdgeLabelDrawingModeValue);
            LineStroke = SR.GetString(ci, LineStrokeValue);
            SmallTickLinesStroke = SR.GetString(ci, SmallTickLinesStrokeValue);
            SmallTickSize = SR.GetString(ci, SmallTickSizeValue);
            SmallTicksPerInterval = SR.GetString(ci, SmallTicksPerIntervalValue);
            TickLineStroke = SR.GetString(ci, TickLineStrokeValue);
            TickSize = SR.GetString(ci, TickSizeValue);
            IsLograthimic = SR.GetString(ci, IsLograthimicValue);
            LograthimicRange = SR.GetString(ci, LograthimicRangeValue);
            LograthimicBase = SR.GetString(ci, LograthimicBaseValue);
            DateTimeRange = SR.GetString(ci, DateTimeRangeValue);
            DateTimeInterval = SR.GetString(ci, DateTimeIntervalValue);
            LabelDateTimeFormat = SR.GetString(ci, LabelDateTimeFormatValue);
            ChartLegend = SR.GetString(ci, ChartLegendValue);
            SelectChartLegend = SR.GetString(ci, SelectChartLegendValue);
            Settings = SR.GetString(ci, SettingsValue);
            AxisType = SR.GetString(ci, AxisTypeValue);
            AxisPosition = SR.GetString(ci, AxisPositionValue);
            AxisHeader = SR.GetString(ci, AxisHeaderValue);
            RangeAndInterval = SR.GetString(ci, RangeAndIntervalValue);
            LabelSettings = SR.GetString(ci, LabelSettingsValue);
            ChartProperties = SR.GetString(ci, ChartPropertiesValue);
            LegendPosition = SR.GetString(ci, LegendPositionValue);
            Chart = SR.GetString(ci, ChartValue);
        }

        const string ChartValue = "Chart";
        string chart;
        /// <summary>
        /// Get or Set Chart property
        /// </summary>
        public string Chart
        {
            get { return chart; }
            set { chart = value; }
        }

        const string PastelValue = "Pastel";
        string pastel;
        /// <summary>
        /// Get and Set Pastel property
        /// </summary>
        public string Pastel
        {
            get { return pastel; }
            set { pastel = value; }
        }

        const string ToolbarPrintValue = "ToolbarPrint";
        string toolbarPrint;
        /// <summary>
        /// Gets or Sets the ToolbarPrint
        /// </summary>
        public string ToolbarPrint
        {
            get { return toolbarPrint; }
            set { toolbarPrint = value; }
        }

        const string ToolbarSwitchPrintValue = "ToolbarSwitchPrint";
        string toolbarSwitchPrint;
        /// <summary>
        /// Gets or Sets ToolbarSwitchPrintproperty
        /// </summary>
        public string ToolbarSwitchPrint
        {
            get { return toolbarSwitchPrint; }
            set { toolbarSwitchPrint = value; }
        }

        const string SaveValue = "Save";
        string save;
        /// <summary>
        /// Get or Set Save property
        /// </summary>
        public string Save
        {
            get { return save; }
            set { save = value; }
        }

        const string CopyValue = "Copy";
        string copy;
        /// <summary>
        /// Get or Set Copy property 
        /// </summary>
        public string Copy
        {
            get { return copy; }
            set { copy = value; }
        }

        const string ToolbarLegendValue = "ToolbarLegend";
        string toolbarLegend;
        /// <summary>
        /// gets or sets the ToolbarLegend
        /// </summary>
        public string ToolbarLegend
        {
            get { return toolbarLegend; }
            set { toolbarLegend = value; }
        }

        const string EnableZoomingValue = "EnableZooming";
        string enableZooming;
        /// <summary>
        /// Get or Set enableZooming property
        /// </summary>
        public string EnableZooming
        {
            get { return enableZooming; }
            set { enableZooming = value; }
        }

        const string HeaderValue = "Header";
        string header;
        /// <summary>
        /// Get or Set header property
        /// </summary>
        public string Header
        {
            get { return header; }
            set { header = value; }
        }

        const string ColorPaletteValue = "ColorPalette";
        string colorPalette;
        /// <summary>
        /// Get or Set Colorpalette property
        /// </summary>
        public string ColorPalette
        {
            get { return colorPalette; }
            set { colorPalette = value; }
        }

        const string ChangeTypeValue = "ChangeType";
        string changeType;
        /// <summary>
        /// Get or Set ChangeType
        /// </summary>
        public string ChangeType
        {
            get { return changeType; }
            set { changeType = value; }
        }
        const string HeaderSettingsValue = "HeaderSettings";
        string headerSettings;
        /// <summary>
        /// Get or Set Headersettings
        /// </summary>
        public string HeaderSettings
        {
            get { return headerSettings; }
            set { headerSettings = value; }
        }
        const string PropertiesValue = "Properties";
        string properties;
        /// <summary>
        /// Get or Set Properties property
        /// </summary>
        public string Properties
        {
            get { return properties; }
            set { properties = value; }
        }

        const string ZoomInValue = "ZoomIn";
        string zoomIn;
        /// <summary>
        /// Gets or Sets the ZoomInProperty
        /// </summary>
        public string ZoomIn
        {
            get { return zoomIn; }
            set { zoomIn = value; }
        }

        const string ZoomOutValue = "ZoomOut";
        string zoomOut;
        /// <summary>
        /// Gets or Sets the ZoomOut property
        /// </summary>
        public string ZoomOut
        {
            get { return zoomOut; }
            set { zoomOut = value; }
        }

        const string ResetZoomValue = "ResetZoom";
        string resetZoom;
        /// <summary>
        /// Get or Set ResetZoom Property
        /// </summary>
        public string ResetZoom
        {
            get { return resetZoom; }
            set { resetZoom = value; }
        }

        const string PanningValue = "Panning";
        string panning;
        /// <summary>
        /// Get or Set Panning Property
        /// </summary>
        public string Panning
        {
            get { return panning; }
            set { panning = value; }
        }

        const string CloseValue = "Close";
        string close;
        /// <summary>
        /// Get or Set Close property
        /// </summary>
        public string Close
        {
            get { return close; }
            set { close = value; }
        }

        const string FastColumnValue = "FastColumn";
        string fastColumn;
        /// <summary>
        /// Get or Set FastColumn property
        /// </summary>
        public string FastColumn
        {
            get { return fastColumn; }
            set { fastColumn = value; }
        }

        const string FastScatterValue = "FastScatter";
        string fastScatter;
        /// <summary>
        /// Get or Set FastScatter property
        /// </summary>
        public string FastScatter
        {
            get { return fastScatter; }
            set { fastScatter = value; }
        }

        const string FastStackingColumnValue = "FastStackingColumn";
        string fastStackingColumn;
        /// <summary>
        /// Get or Set FastStackingColumn
        /// </summary>
        public string FastStackingColumn
        {
            get { return fastStackingColumn; }
            set { fastStackingColumn = value; }
        }

        const string FastHiLoOpenCloseValue = "FastHiLoOpenClose";
        string fastHiLoOpenClose;
        /// <summary>
        /// Get or Set FastHiLoOpenclose property
        /// </summary>
        public string FastHiLoOpenClose
        {
            get { return fastHiLoOpenClose; }
            set { fastHiLoOpenClose = value; }
        }

        const string FastBarValue = "FastBar";
        string fastBar;
        /// <summary>
        /// Get or Set FastBar Property
        /// </summary>
        public string FastBar
        {
            get { return fastBar; }
            set { fastBar = value; }
        }

        const string PrintDialogCancelValue = "printDialogCancel";
        string printDialogCancel;
        /// <summary>
        /// Get and Set PrintDialogCancel property
        /// </summary>
        public string PrintDialogCancel
        {
            get { return printDialogCancel; }
            set { printDialogCancel = value; }
        }

        const string LegendWindowTitleValue = "LegendWindowTitle";
        string legendWindowTitle;
        /// <summary>
        /// Get or Set LegendWindowTitle
        /// </summary>
        public string LegendWindowTitle
        {
            get { return legendWindowTitle; }
            set { legendWindowTitle = value; }
        }

        const string LegendWindowIconValue = "LegendWindowIcon";
        string legendWindowIcon;
        /// <summary>
        /// Get or Set LegendWindowIcon property
        /// </summary>
        public string LegendWindowIcon
        {
            get { return legendWindowIcon; }
            set { legendWindowIcon = value; }
        }


        const string LegendWindowCheckBoxValue = "LegendWindowCheckBox";
        string legendWindowCheckBox;
        /// <summary>
        /// Get or Set LegendWindowCheckBox property
        /// </summary>
        public string LegendWindowCheckBox
        {
            get { return legendWindowCheckBox; }
            set { legendWindowCheckBox = value; }
        }

        const string LegendWindowOKValue = "LegendWindowOK";
        string legendWindowOK;
        /// <summary>
        /// Get or Set LegendWindowOK property
        /// </summary>
        public string LegendWindowOK
        {
            get { return legendWindowOK; }
            set { legendWindowOK = value; }
        }

        const string LegendWindowCancelValue = "LegendWindowCancel";
        string legendWindowCancel;
        /// <summary>
        /// Get or Set LegendWindowCancel property
        /// </summary>
        public string LegendWindowCancel
        {
            get { return legendWindowCancel; }
            set { legendWindowCancel = value; }
        }

        const string ContextMenuZoomingValue = "contextMenuZooming";
        string contextMenuZooming;
        /// <summary>
        /// Get or Set ContextMenuZooming property 
        /// </summary>
        public string ContextMenuZooming
        {
            get { return contextMenuZooming; }
            set { contextMenuZooming = value; }
        }

        const string ContextMenuZoomAllValue = "contextMenuZoomAll";
        string contextMenuZoomAll;
        /// <summary>
        /// Get or set ContextMenuZoomAll property 
        /// </summary>
        public string ContextMenuZoomAll
        {
            get { return contextMenuZoomAll; }
            set { contextMenuZoomAll = value; }
        }

        const string ContextMenuSeriesValue = "contextMenuSeries";
        string contextMenuSeries;
        /// <summary>
        /// Get or Set ContextMenuSeries property 
        /// </summary>
        public string ContextMenuSeries
        {
            get { return contextMenuSeries; }
            set { contextMenuSeries = value; }
        }

        const string ContextMenuPalettesValue = "contextMenuPalettes";
        string contextMenuPalettes;
        /// <summary>
        /// Get or Set ContextMenupalettes
        /// </summary>
        public string ContextMenuPalettes
        {
            get { return contextMenuPalettes; }
            set { contextMenuPalettes = value; }
        }

        const string PrintDialogPrintValue = "printDialogPrint";
        string printDialogPrint;
        /// <summary>
        /// Get or Set PrintDialogPrint Property
        /// </summary>
        public string PrintDialogPrint
        {
            get { return printDialogPrint; }
            set { printDialogPrint = value; }
        }

        const string StyleValue = "style";
        string style;
        /// <summary>
        /// Gets or Sets the Style property
        /// </summary>
        public string Style
        {
            get { return style; }
            set { style = value; }
        }

        const string AnalogValue = "Analog";
        string analog;
        /// <summary>
        /// Get or Set analog property
        /// </summary>
        public string Analog
        {
            get { return analog; }
            set { analog = value; }
        }

        const string AreaValue = "Area";
        string area;
        /// <summary>
        /// Get or Set Area property
        /// </summary>
        public string Area
        {
            get { return area; }
            set { area = value; }
        }

        const string BarValue = "Bar";
        string bar;
        /// <summary>
        /// Get or Set bar Property
        /// </summary>
        public string Bar
        {
            get { return bar; }
            set { bar = value; }
        }

        const string BoxAndWhiskerValue = "BoxAndWhisker";
        string boxAndWhisker;
        /// <summary>
        /// Get or Set BoxAndWhisker property
        /// </summary>
        public string BoxAndWhisker
        {
            get { return boxAndWhisker; }
            set { boxAndWhisker = value; }
        }

        const string BubbleValue = "Bubble";
        string bubble;
        /// <summary>
        /// Get or Set Bubble property 
        /// </summary>
        public string Bubble
        {
            get { return bubble; }
            set { bubble = value; }
        }


        const string CandleValue = "Candle";
        string candle;
        /// <summary>
        /// Get or Set Candle property
        /// </summary>
        public string Candle
        {
            get { return candle; }
            set { candle = value; }
        }

        const string ColorfulValue = "Colorful";
        string colorful;
        /// <summary>
        /// Get or Set Colorful property 
        /// </summary>
        public string Colorful
        {
            get { return colorful; }
            set { colorful = value; }
        }

        const string ColumnValue = "Column";
        string column;
        /// <summary>
        /// Get or Set Column property 
        /// </summary>
        public string Column
        {
            get { return column; }
            set { column = value; }
        }

        const string CustomValue = "Custom";
        string custom;
        /// <summary>
        /// Get or Set Custom property 
        /// </summary>
        public string Custom
        {
            get { return custom; }
            set { custom = value; }
        }

        const string DefaultValue = "Default";
        string m_default;
        /// <summary>
        /// Get or Set default property
        /// </summary>
        public string Default
        {
            get { return m_default; }
            set { m_default = value; }
        }

        const string DefaultAlphaValue = "DefaultAlpha";
        string defaultAlpha;
        /// <summary>
        /// Get or Set DefaultAlpha property 
        /// </summary>
        public string DefaultAlpha
        {
            get { return defaultAlpha; }
            set { defaultAlpha = value; }
        }

        const string DefaultDarkValue = "DefaultDark";
        string defaultDark;
        /// <summary>
        /// Get or set DefaultDark
        /// </summary>
        public string DefaultDark
        {
            get { return defaultDark; }
            set { defaultDark = value; }
        }

        const string DoughnutValue = "Doughnut";
        string doughnut;
        /// <summary>
        /// Get or Set Doughnut property 
        /// </summary>
        public string Doughnut
        {
            get { return doughnut; }
            set { doughnut = value; }
        }

        const string EarthToneValue = "EarthTone";
        string earthTone;
        /// <summary>
        /// Get or Set EarthTone property 
        /// </summary>
        public string EarthTone
        {
            get { return earthTone; }
            set { earthTone = value; }
        }

        const string FastLineValue = "FastLine";
        string fastLine;
        /// <summary>
        /// Get or Set FastLine property
        /// </summary>
        public string FastLine
        {
            get { return fastLine; }
            set { fastLine = value; }
        }

        const string FunnelValue = "Funnel";
        string funnel;
        /// <summary>
        /// Get or Set Funnel property
        /// </summary>
        public string Funnel
        {
            get { return funnel; }
            set { funnel = value; }
        }

        const string GanttValue = "Gantt";
        string gantt;
        /// <summary>
        /// Get or Set Gantt property
        /// </summary>
        public string Gantt
        {
            get { return gantt; }
            set { gantt = value; }
        }

        const string GradientValue = "Gradient";
        string gradient;
        /// <summary>
        /// Get or Set Gradient property
        /// </summary>
        public string Gradient
        {
            get { return gradient; }
            set { gradient = value; }
        }

        const string GrayscaleValue = "Grayscale";
        string grayscale;
        /// <summary>
        /// Get or Set Grayscale property
        /// </summary>
        public string Grayscale
        {
            get { return grayscale; }
            set { grayscale = value; }
        }

        const string HiLoValue = "HiLo";
        string hiLo;
        /// <summary>
        /// Get or Set HiLo property
        /// </summary>
        public string HiLo
        {
            get { return hiLo; }
            set { hiLo = value; }
        }

        const string HiLoAreaValue = "HiLoArea";
        string hiLoArea;
        /// <summary>
        /// Get or Set HiLoArea property
        /// </summary>
        public string HiLoArea
        {
            get { return hiLoArea; }
            set { hiLoArea = value; }
        }

        const string HiLoOpenCloseValue = "HiLoOpenClose";
        string hiLoOpenClose;
        /// <summary>
        /// Get or Set HiLoOpenClose property
        /// </summary>
        public string HiLoOpenClose
        {
            get { return hiLoOpenClose; }
            set { hiLoOpenClose = value; }
        }

        const string HistogramValue = "Histogram";
        string histogram;
        /// <summary>
        /// Get or Set Histogram property
        /// </summary>
        public string Histogram
        {
            get { return histogram; }
            set { histogram = value; }
        }

        const string KagiValue = "Kagi";
        string kagi;
        /// <summary>
        /// Get or Set Kagi property
        /// </summary>
        public string Kagi
        {
            get { return kagi; }
            set { kagi = value; }
        }

        const string LineValue = "Line";
        string line;
        /// <summary>
        /// Get or Set line property
        /// </summary>
        public string Line
        {
            get { return line; }
            set { line = value; }
        }

        const string NatureValue = "Nature";
        string nature;
        /// <summary>
        /// Get or Set Nature Property
        /// </summary>
        public string Nature
        {
            get { return nature; }
            set { nature = value; }
        }

        const string Office2007BlackValue = "Office2007Black";
        string office2007Black;
        /// <summary>
        /// Get or Set Office2007Black Property
        /// </summary>
        public string Office2007Black
        {
            get { return office2007Black; }
            set { office2007Black = value; }
        }

        const string Office2007BlueValue = "Office2007Blue";
        string office2007Blue;
        /// <summary>
        /// Get or Set Office2007Blue Property
        /// </summary>
        public string Office2007Blue
        {
            get { return office2007Blue; }
            set { office2007Blue = value; }
        }

        const string Office2007SilverValue = "Office2007Silver";
        string office2007Silver;
        /// <summary>
        /// Get or Set Office2007SilverProperty
        /// </summary>
        public string Office2007Silver
        {
            get { return office2007Silver; }
            set { office2007Silver = value; }
        }

        const string MixedGrayValue = "MixedGray";
        string mixedgray;
        /// <summary>
        /// Get or Set MixedGray
        /// </summary>
        public string MixedGray
        {
            get { return mixedgray; }
            set { mixedgray = value; }
        }

        const string BlueScaleValue = "BlueScale";
        string bluescale;
        /// <summary>
        /// Get or Set BlueScale property
        /// </summary>
        public string BlueScale
        {
            get { return bluescale; }
            set { bluescale = value; }
        }

        const string MaroonRedValue = "MaroonRed";
        string maroonred;
        /// <summary>
        /// Get or Set MaroonRedProperty
        /// </summary>
        public string MaroonRed
        {
            get { return maroonred; }
            set { maroonred = value; }
        }


        const string GreenScaleValue = "GreenScale";
        string greenscale;
        /// <summary>
        /// Get or Set GreenScale property
        /// </summary>
        public string GreenScale
        {
            get { return greenscale; }
            set { greenscale = value; }
        }

        const string MixedVioletValue = "MixedViolet";
        string mixedviolet;
        /// <summary>
        /// Get or Set MixedViolet
        /// </summary>
        public string MixedViolet
        {
            get { return mixedviolet; }
            set { mixedviolet = value; }
        }

        const string CoolBlueScaleValue = "CoolBlueScale";
        string coolbluescale;
        /// <summary>
        /// Get or Set CoolBlueScale property 
        /// </summary>
        public string CoolBlueScale
        {
            get { return coolbluescale; }
            set { coolbluescale = value; }
        }

        const string MetroThemeValue = "Metro";
        string metroTheme;
        /// <summary>
        /// Get or Set MetroProperty
        /// </summary>
        public string Metro
        {
            get { return metroTheme; }
            set { metroTheme = value; }
        }


        const string ChocolateOrangeValue = "ChocolateOrange";
        string chocolateorange;
        /// <summary>
        /// Get or Set ChocolateOrange property 
        /// </summary>
        public string ChocolateOrange
        {
            get { return chocolateorange; }
            set { chocolateorange = value; }
        }

        const string MixedFantasyValue = "MixedFantasy";
        string mixedfantasy;
        /// <summary>
        /// Get or Set MixedFantasy property
        /// </summary>
        public string MixedFantasy
        {
            get { return mixedfantasy; }
            set { mixedfantasy = value; }
        }

        const string PieValue = "Pie";
        string pie;
        /// <summary>
        /// Get and Set Pie property
        /// </summary>
        public string Pie
        {
            get { return pie; }
            set { pie = value; }
        }

        const string PointAndFigureValue = "PointAndFigure";
        string pointAndFigure;
        /// <summary>
        /// Get and Set PointAndFigure property
        /// </summary>
        public string PointAndFigure
        {
            get { return pointAndFigure; }
            set { pointAndFigure = value; }
        }

        const string PolarValue = "Polar";
        string polar;
        /// <summary>
        /// Get and Set Polar Property
        /// </summary>
        public string Polar
        {
            get { return polar; }
            set { polar = value; }
        }

        const string PrintDialogAdvancedValue = "printDialogAdvanced";
        string m_printDialogAdvanced;
        /// <summary>
        /// Get or Set PrintDialogAdvanced property
        /// </summary>
        public string PrintDialogAdvanced
        {
            get { return m_printDialogAdvanced; }
            set { m_printDialogAdvanced = value; }
        }

        const string PrintDialogBWModeValue = "printDialogBWMode";
        string m_printDialogBWMode;
        /// <summary>
        /// Get and Set PrintDialogBWMode property
        /// </summary>
        public string PrintDialogBWMode
        {
            get { return m_printDialogBWMode; }
            set { m_printDialogBWMode = value; }
        }

        const string PrintDialogColorModeValue = "printDialogColorMode";
        string printDialogColorMode;
        /// <summary>
        /// Get or Set PrintDialogColorMode property
        /// </summary>
        public string PrintDialogColorMode
        {
            get { return printDialogColorMode; }
            set { printDialogColorMode = value; }
        }


        const string PrintDialogPrintStretchValue = "printDialogPrintStretch";
        string printDialogPrintStretch;
        /// <summary>
        /// Get or Set PrintDialogPrintStretch property
        /// </summary>
        public string PrintDialogPrintStretch
        {
            get { return printDialogPrintStretch; }
            set { printDialogPrintStretch = value; }
        }

        const string PrintDialogPrintModeValue = "printDialogPrintMode";
        string printDialogPrintMode;
        /// <summary>
        /// Get or Set PrintDialogPrintMode property
        /// </summary>
        public string PrintDialogPrintMode
        {
            get { return printDialogPrintMode; }
            set { printDialogPrintMode = value; }
        }

        const string PyramidValue = "Pyramid";
        string pyramid;
        /// <summary>
        /// Get or Set Pyramid property
        /// </summary>
        public string Pyramid
        {
            get { return pyramid; }
            set { pyramid = value; }
        }

        const string RadarValue = "Radar";
        string radar;
        /// <summary>
        /// Get or Set Radar Property
        /// </summary>
        public string Radar
        {
            get { return radar; }
            set { radar = value; }
        }

        const string RangeAreaValue = "RangeArea";
        string rangeArea;
        /// <summary>
        /// Get or Set RangeArea property
        /// </summary>
        public string RangeArea
        {
            get { return rangeArea; }
            set { rangeArea = value; }
        }

        const string RangeColumnValue = "RangeColumn";
        string rangeColumn;
        /// <summary>
        /// Get or Set RangeColumn property
        /// </summary>
        public string RangeColumn
        {
            get { return rangeColumn; }
            set { rangeColumn = value; }
        }


        const string RenkoValue = "Renko";
        string renko;
        /// <summary>
        /// Get or Set renko property
        /// </summary>
        public string Renko
        {
            get { return renko; }
            set { renko = value; }
        }

        const string RotatedSplineValue = "RotatedSpline";
        string rotatedSpline;
        /// <summary>
        ///Get or Set RotatedSpline Property
        /// </summary>
        public string RotatedSpline
        {
            get { return rotatedSpline; }
            set { rotatedSpline = value; }
        }

        const string ScatterValue = "Scatter";
        string scatter;
        /// <summary>
        /// Get or Ser Scatter Property
        /// </summary>
        public string Scatter
        {
            get { return scatter; }
            set { scatter = value; }
        }

        const string SplineValue = "Spline";
        string spline;
        /// <summary>
        /// Gets or Sets the spline proeprty
        /// </summary>
        public string Spline
        {
            get { return spline; }
            set { spline = value; }
        }

        const string SplineAreaValue = "SplineArea";
        string splineArea;
        /// <summary>
        /// Gets or Sets the SplineArea property
        /// </summary>
        public string SplineArea
        {
            get { return splineArea; }
            set { splineArea = value; }
        }


        const string StackingAreaValue = "StackingArea";
        string stackingArea;
        /// <summary>
        /// Gets or Sets the StackingArea property
        /// </summary>
        public string StackingArea
        {
            get { return stackingArea; }
            set { stackingArea = value; }
        }
        
        const string StackingArea100Value = "StackingArea100";
        string stackingArea100;
        /// <summary>
        /// Gets or Sets the StackingArea100 property
        /// </summary>
        public string StackingArea100
        {
            get { return stackingArea100; }
            set { stackingArea100 = value; }
        }

        const string StackingLineValue = "StackingLine";
        string stackingLine;
        /// <summary>
        /// Gets or Sets the StackingLine property
        /// </summary>
        public string StackingLine
        {
            get { return stackingLine; }
            set { stackingLine = value; }
        }

        const string StackingLine100Value = "StackingLine100";
        string stackingLine100;
        /// <summary>
        /// Gets or Sets the StackingLine100 property
        /// </summary>
        public string StackingLine100
        {
            get { return stackingLine100; }
            set { stackingLine100 = value; }
        }

        const string StackingSplineValue = "StackingSpline";
        string stackingSpline;
        /// <summary>
        /// Gets or Sets the Stackingspline property
        /// </summary>
        public string StackingSpline
        {
            get { return stackingSpline; }
            set { stackingSpline = value; }
        }

        const string StackingSpline100Value = "StackingSpline100";
        string stackingSpline100;
        /// <summary>
        /// Gets or Sets the StackingSpline100 property
        /// </summary>
        public string StackingSpline100
        {
            get { return stackingSpline100; }
            set { stackingSpline100 = value; }
        }

        const string StackingSplineAreaValue = "StackingSplineArea";
        string stackingSplineArea;
        /// <summary>
        /// Gets or sets the StackingSplineArea
        /// </summary>
        public string StackingSplineArea
        {
            get { return stackingSplineArea; }
            set { stackingSplineArea = value; }
        }

        const string StackingSplineArea100Value = "StackingSplineArea100";
        string stackingSplineArea100;
        /// <summary>
        /// Gets or Sets the StackingSplineArea100 property
        /// </summary>
        public string StackingSplineArea100
        {
            get { return stackingSplineArea100; }
            set { stackingSplineArea100 = value; }
        }

        const string StackingBarValue = "StackingBar";
        string stackingBar;
        /// <summary>
        ///Gets or Sets StackingBar property
        /// </summary>
        public string StackingBar
        {
            get { return stackingBar; }
            set { stackingBar = value; }
        }

        const string StackingBar100Value = "StackingBar100";
        string stackingBar100;
        /// <summary>
        /// Gets or Sets the StackingBar100
        /// </summary>
        public string StackingBar100
        {
            get { return stackingBar100; }
            set { stackingBar100 = value; }
        }

        const string StackingColumnValue = "StackingColumn";
        string stackingColumn;
        /// <summary>
        /// Gets or Sets the StackingColumn
        /// </summary>
        public string StackingColumn
        {
            get { return stackingColumn; }
            set { stackingColumn = value; }
        }

        const string StackingColumn100Value = "StackingColumn100";
        string stackingColumn100;
        /// <summary>
        /// Gets or Sets the StackingColumn100 property
        /// </summary>
        public string StackingColumn100
        {
            get { return stackingColumn100; }
            set { stackingColumn100 = value; }
        }

        const string StepAreaValue = "StepArea";
        string stepArea;
        /// <summary>
        /// Gets or Sets stepArea property
        /// </summary>
        public string StepArea
        {
            get { return stepArea; }
            set { stepArea = value; }
        }

        const string StepLineValue = "StepLine";
        string stepLine;
        /// <summary>
        /// Gets or Sets the Stepline property
        /// </summary>
        public string StepLine
        {
            get { return stepLine; }
            set { stepLine = value; }
        }

        const string Surface3DValue = "Surface3D";
        string surface3D;
        /// <summary>
        /// Gets or Sets the Surface3D
        /// </summary>
        public string Surface3D
        {
            get { return surface3D; }
            set { surface3D = value; }
        }

        const string ThreeLineBreakValue = "ThreeLineBreak";
        string threeLineBreak;
        /// <summary>
        /// Gets or Sets the ThreeLineBreak property
        /// </summary>
        public string ThreeLineBreak
        {
            get { return threeLineBreak; }
            set { threeLineBreak = value; }
        }

        const string TornadoValue = "Tornado";
        string tornado;
        /// <summary>
        /// Gets or Sets the Tornado property
        /// </summary>
        public string Tornado
        {
            get { return tornado; }
            set { tornado = value; }
        }

        const string TriadValue = "Triad";
        string triad;
        /// <summary>
        /// Gets or Sets the Triad Property
        /// </summary>
        public string Triad
        {
            get { return triad; }
            set { triad = value; }
        }

        const string WarmColdValue = "WarmCold";
        string warmCold;
        /// <summary>
        /// Gets or Sets the Warmcold
        /// </summary>
        public string WarmCold
        {
            get { return warmCold; }
            set { warmCold = value; }
        }

        //const string ChangeTypeValue = "ChangeType";
        //string changeType;
        //public string ChangeType
        //{
        //    get { return changeType; }
        //    set { changeType = value; }
        //}

        const string SmallTickSizeValue = "SmallTickSize";
        string smallTickSize;
        /// <summary>
        /// Get and Set SmallTickSize property
        /// </summary>
        public string SmallTickSize
        {
            get { return smallTickSize; }
            set { smallTickSize = value; }
        }
        const string AppearenceValue = "Appearance";
        string appearence;
        /// <summary>
        /// Get or Set Appearance property
        /// </summary>
        public string Appearance
        {
            get { return appearence; }
            set { appearence = value; }
        }
        const string BackgroundValue = "Background";
        string background;
        /// <summary>
        /// Get or Set BackGround property
        /// </summary>
        public string Background
        {
            get { return background; }
            set { background = value; }
        }
        const string BorderBrushValue = "BorderBrush";
        string borderBrush;
        /// <summary>
        /// Get or Set BorderBrush property 
        /// </summary>
        public string BorderBrush
        {
            get { return borderBrush; }
            set { borderBrush = value; }
        }
        const string BorderThicknessValue = "BorderThickness";
        string borderThickness;
        /// <summary>
        /// Get or Set BorderThickness property 
        /// </summary>
        public string BorderThickness
        {
            get { return borderThickness; }
            set { borderThickness = value; }
        }
        const string CornerRadiusValue = "CornerRadius";
        string cornerRadius;
        /// <summary>
        /// Get or Set CornerRadius Property 
        /// </summary>
        public string CornerRadius
        {
            get { return cornerRadius; }
            set { cornerRadius = value; }
        }
        const string ForegroundValue = "Foreground";
        string foreground;
        /// <summary>
        /// Get or Set Foreground property
        /// </summary>
        public string Foreground
        {
            get { return foreground; }
            set { foreground = value; }
        }
        const string MarginValue = "Margin";
        string margin;
        /// <summary>
        /// Get or Set Margin property
        /// </summary>
        public string Margin
        {
            get { return margin; }
            set { margin = value; }
        }
        const string PaddingValue = "Padding";
        string padding;
        /// <summary>
        /// Get or Set Padding property
        /// </summary>
        public string Padding
        {
            get { return padding; }
            set { padding = value; }
        }
        const string VisualStyleValue = "VisualStyle";
        string visualStyle;
        /// <summary>
        /// Gets or Sets VisualStyleProperty
        /// </summary>
        public string VisualStyle
        {
            get { return visualStyle; }
            set { visualStyle = value; }
        }
        const string ChartVisualStyleValue = "ChartVisualStyle";
        string chartVisualStyle;
        /// <summary>
        /// Get or Set ChartVisualStyle property 
        /// </summary>
        public string ChartVisualStyle
        {
            get { return chartVisualStyle; }
            set { chartVisualStyle = value; }
        }
        const string SettingsValue = "Settings";
        string settings;
        /// <summary>
        /// Get or Set Settings property
        /// </summary>
        public string Settings
        {
            get { return settings; }
            set { settings = value; }
        }
        const string ChartAreaValue = "ChartArea";
        string chartArea;
        /// <summary>
        /// Get or Set ChartArea property
        /// </summary>
        public string ChartArea
        {
            get { return chartArea; }
            set { chartArea = value; }
        }
        const string SelectChartAreaValue = "SelectChartArea";
        string selectChartArea;
        /// <summary>
        /// Get or Set selectionChartArea
        /// </summary>
        public string SelectChartArea
        {
            get { return selectChartArea; }
            set { selectChartArea = value; }
        }
        const string GridRegionPropertiesValue = "GridRegionProperties";
        string gridRegionProperties;
        /// <summary>
        /// Get or Set GridRegionProperties
        /// </summary>
        public string GridRegionProperties
        {
            get { return gridRegionProperties; }
            set { gridRegionProperties = value; }
        }
        const string GridBackgroundValue = "GridBackground";
        string gridBackground;
        /// <summary>
        /// Get or Set GridBackGround property
        /// </summary>
        public string GridBackground
        {
            get { return gridBackground; }
            set { gridBackground = value; }
        }
        const string AlternatingGridBackgroundValue = "AlternatingGridBackground";
        string alternatingGridBackground;
        /// <summary>
        /// Get or Set AlternativeGridBackground
        /// </summary>
        public string AlternativeGridBackground
        {
            get { return alternatingGridBackground; }
            set { alternatingGridBackground = value; }
        }
        const string alternatingGridBackgroundModeValue = "AlternatingGridBackgrounMode";
        string alternatingGridBackgroundMode;
        /// <summary>
        /// Get or Set AlternatingBackgoundMode
        /// </summary>
        public string AlternatingBackgoundMode
        {
            get { return alternatingGridBackgroundMode; }
            set { alternatingGridBackgroundMode = value; }
        }
        const string AlternatingGridBackgroundDirectionValue = "AlternatingGridBackgrounDirection";
        string alternatingGridBackgroundDirection;
        /// <summary>
        /// Get or set AlternatingBackgoundDirection
        /// </summary>
        public string AlternatingBackgoundDirection
        {
            get { return alternatingGridBackgroundDirection; }
            set { alternatingGridBackgroundDirection = value; }
        }
        const string InteractiveFeaturesValue = "InteractiveFeatures";        
        string interactiveFeatures;
        /// <summary>
        /// Get or Set InteractiveFeatures
        /// </summary>
        public string InteractiveFeatures
        {
            get { return interactiveFeatures; }
            set { interactiveFeatures = value; }
        }
        const string EnableZoomOnScrollValue = "EnableZoomOnScroll";
        string enableZoomOnScroll;
        /// <summary>
        /// Get or Set EnableZoomOnScroll
        /// </summary>
        public string EnableZoomOnScroll
        {
            get { return enableZoomOnScroll; }
            set { enableZoomOnScroll = value; }
        }
        const string EnableContextMenuValue = "EnableContextMenu";
        string enableContextMenu;
        /// <summary>
        /// Get or Set EnableContextMenu property
        /// </summary>
        public string EnableContextMenu
        {
            get { return enableContextMenu; }
            set { enableContextMenu = value; }
        }
        const string ChartAreaLegendValue = "ChartAreaLegend";
        string chartAreaLegend;
        /// <summary>
        /// Get or Set ChartAreaLegend property 
        /// </summary>
        public string ChartAreaLegend
        {
            get { return chartAreaLegend; }
            set { chartAreaLegend = value; }
        }
        const string CheckBoxVisibilityValue = "CheckBoxVisibility";
        string checkBoxVisibility;
        /// <summary>
        /// Get or Set CheckBoxVisibility property 
        /// </summary>
        public string CheckBoxVisibility
        {
            get { return checkBoxVisibility; }
            set { checkBoxVisibility = value; }
        }
        const string IconVisibilityValue = "IconVisibility";
        string iconVisibility;
        /// <summary>
        /// Get or Set IconVisibility
        /// </summary>
        public string IconVisibility
        {
            get { return iconVisibility; }
            set { iconVisibility = value; }
        }
        const string ShowSymbolValue = "ShowSymbol";
        string showSymbol;
        /// <summary>
        /// Get or Set ShowSymbolProperty
        /// </summary>
        public string ShowSymbol
        {
            get { return showSymbol; }
            set { showSymbol = value; }
        }
        const string ChartSeriesValue = "ChartSeries";
        string chartSeries;
        /// <summary>
        /// Get or Set ChartSeries property
        /// </summary>
        public string ChartSeries
        {
            get { return chartSeries; }
            set { chartSeries = value; }
        }
        const string SelectChartSeriesValue = "SelectChartSeries";
        string selectChartSeries;
        /// <summary>
        /// Get or Set SelectChartSeries property
        /// </summary>
        public string SelectChartSeries
        {
            get { return selectChartSeries; }
            set { selectChartSeries = value; }
        }
        const string InteriorValue = "Interior";
        string interior;
        /// <summary>
        /// Get or Set Interior property
        /// </summary>
        public string Interior
        {
            get { return interior; }
            set { interior = value; }
        }
        const string StrokeValue = "Stroke";
        string stroke;
        /// <summary>
        /// gets or Sets the Stroke property
        /// </summary>
        public string Stroke
        {
            get { return stroke; }
            set { stroke = value; }
        }
        const string StrokeThicknessValue = "StrokeThickness";
        string strokeThickness;
        /// <summary>
        /// Gets or Sets the StrokeThickness proeprty
        /// </summary>
        public string StrokeThickness
        {
            get { return strokeThickness; }
            set { strokeThickness = value; }
        }
        const string TypeValue = "Type";
        string type;
        /// <summary>
        /// Gets or Sets the Type property
        /// </summary>
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        const string LegendValue = "Legend";
        string legend;
        /// <summary>
        /// Gets or Sets the Legend property
        /// </summary>
        public string Legend
        {
            get { return legend; }
            set { legend = value; }
        }
        const string IsVisibleOnLegendValue = "IsVisibleOnLegend";
        string isVisibleOnLegend;
        /// <summary>
        /// Get or Set IsVisibleOnLegend property 
        /// </summary>
        public string IsVisibleOnLegend
        {
            get { return isVisibleOnLegend; }
            set { isVisibleOnLegend = value; }
        }
        const string LegendLabelValue = "LegendLabel";
        string legendLabel;
        /// <summary>
        /// Get or Set LegendLabel property
        /// </summary>
        public string LegendLabel
        {
            get { return legendLabel; }
            set { legendLabel = value; }
        }
        const string LegendIconValue = "LegendIcon";
        string legendIcon;
        /// <summary>
        /// Gets or Sets LegendIcon property
        /// </summary>
        public string LegendIcon
        {
            get { return legendIcon; }
            set { legendIcon = value; }
        }
        const string DataValue = "Data";
        string data;
        /// <summary>
        /// get or Set Data property 
        /// </summary>
        public string Data
        {
            get { return data; }
            set { data = value; }
        }
        const string IsZoomableValue = "IsZoomable";
        string isZoomable;
        /// <summary>
        /// Get or Set IsZoomable property
        /// </summary>
        public string IsZoomable
        {
            get { return isZoomable; }
            set { isZoomable = value; }
        }
        const string IsRotatedValue = "IsRotated";
        string isRotated;
        /// <summary>
        /// Get or Set IsRotated Property
        /// </summary>
        public string IsRotated
        {
            get { return isRotated; }
            set { isRotated = value; }
        }
        const string IsSortedValue = "IsSorted";
        string isSorted;
        /// <summary>
        /// Get or Set IsSorted property
        /// </summary>
        public string IsSorted
        {
            get { return isSorted; }
            set { isSorted = value; }
        }
        const string ShowSeriesEmptyPointsValue = "ShowSeriesEmptyPoints";
        string showSeriesEmptyPoints;
        /// <summary>
        /// Get or Set ShowSeriesEmptyPoints
        /// </summary>
        public string ShowSeriesEmptyPoints
        {
            get { return showSeriesEmptyPoints; }
            set { showSeriesEmptyPoints = value; }
        }
        const string EmptyPointsInteriorValue = "EmptyPointsInterior";
        string emptyPointsInterior;
        /// <summary>
        /// Get or Set EmptyPointsInterior property 
        /// </summary>
        public string EmptyPointsInterior
        {
            get { return emptyPointsInterior; }
            set { emptyPointsInterior = value; }
        }

        const string EmptyPointsStyleValue = "EmptyPointsStyle";
        string emptyPointsStyle;
        /// <summary>
        /// Get or Set EmptyPointsStyle property 
        /// </summary>
        public string EmptyPointsStyle 
        {
            get { return emptyPointsStyle; }
            set { emptyPointsStyle = value; }
        }
        const string AnimationOptionValue = "AnimationOption";
        string animationOption;
        /// <summary>
        /// Get or Set AnimationOption property
        /// </summary>
        public string AnimationOption
        {
            get { return animationOption; }
            set { animationOption = value; }
        }

        const string AnimateSeriesonebyoneValue = "AnimateSeriesonebyone";
        string animateSeriesonebyone;
        /// <summary>
        /// Get or Set AnimateSeriesonebyone property
        /// </summary>
        public string AnimateSeriesonebyone
        {
            get { return animateSeriesonebyone; }
            set { animateSeriesonebyone = value; }
        }



        const string ChartAxisValue = "ChartAxis";
        string chartAxis;
        /// <summary>
        /// Get or Set ChartAxis property 
        /// </summary>
        public string ChartAxis
        {
            get { return chartAxis; }
            set { chartAxis = value; }
        }
        const string SelectChartAxisValue = "SelectChartAxis";
        string selectChartAxis;
        /// <summary>
        /// Get or Set SelectionChartAxis property
        /// </summary>
        public string SelectChartAxis
        {
            get { return selectChartAxis; }
            set { selectChartAxis = value; }
        }

        const string AxisTypeValue = "AxisType";
        string axisType;
        /// <summary>
        /// Get or Set AxisType.
        /// </summary>
        public string AxisType
        {
            get { return axisType; }
            set { axisType = value; }
        }

        const string AxisPositionValue = "AxisPosition";
        string axisPosition;
        /// <summary>
        /// Get or Set AxisPosition property
        /// </summary>
        public string AxisPosition
        {
            get { return axisPosition; }
            set { axisPosition = value; }
        }
        const string AxisHeaderValue = "AxisHeader";
        string axisHeader;
        /// <summary>
        /// Get or Set AxisHeader property
        /// </summary>
        public string AxisHeader
        {
            get { return axisHeader; }
            set { axisHeader = value; }
        }
        const string RangeAndIntervalValue = "RangeAndInterval";
        string rangeAndInterval;
        /// <summary>
        /// Get or Set RangeAndInterval property
        /// </summary>
        public string RangeAndInterval
        {
            get { return rangeAndInterval; }
            set { rangeAndInterval = value; }
        }
        const string LabelSettingsValue = "LabelSettings";
        string labelSettings;
        /// <summary>
        /// Gets or sets LabelSettings property
        /// </summary>
        public string LabelSettings
        {
            get { return labelSettings; }
            set { labelSettings = value; }
        }

        const string EnableAnimationValue = "EnableAnimation";
        string enableAnimation;
        /// <summary>
        /// Get or Set EnableAnimation property 
        /// </summary>
        public string EnableAnimation
        {
            get { return enableAnimation; }
            set { enableAnimation = value; }
        }

        const string ValueTypeValue = "ValueType";
        string valueType;
        /// <summary>
        /// Gets or Sets the ValueTypeproperty
        /// </summary>
        public string ValueType
        {
            get { return valueType; }
            set { valueType = value; }
        }
        const string OpposedPositionValue = "OpposedPosition";
        string opposedPosition;
        /// <summary>
        /// Get or Set OpposedPosition property
        /// </summary>
        public string OpposedPosition
        {
            get { return opposedPosition; }
            set { opposedPosition = value; }
        }


        const string ChartTypeValue = "ChartType";
        string chartType;
        /// <summary>
        /// get or set ChartType property 
        /// </summary>
        public string ChartType
        {
            get { return chartType; }
            set { chartType = value; }
        }
        const string ChartTypeHeaderValue = "ChartTypeHeader";
        string chartTypeHeader;
        /// <summary>
        /// Get or Set ChartTypeHeader property
        /// </summary>
        public string ChartTypeHeader
        {
            get { return chartTypeHeader; }
            set { chartTypeHeader = value; }
        }
        const string OrientationValue = "Orientation";
        string orientation;
        /// <summary>
        /// Get or Set Orientation property
        /// </summary>
        public string Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }
        //const string EnableZoomingValue = "EnableZooming";
        //string enableZooming;
        //public string EnableZooming
        //{
        //    get { return enableZooming; }
        //    set { enableZooming = value; }
        //}
        const string HeaderAlignmentValue = "HeaderAlignment";
        string headerAlignment;
        /// <summary>
        /// Get or Set HeaderAlignment Property
        /// </summary>
        public string HeaderAlignment
        {
            get { return headerAlignment; }
            set { headerAlignment = value; }
        }
        const string OriginValue = "Origin";
        string origin;
        /// <summary>
        /// Get or Set Origin property
        /// </summary>
        public string Origin
        {
            get { return origin; }
            set { origin = value; }
        }
        const string AutoSetRangeValue = "AutoSetRange";
        string autoSetRange;
        /// <summary>
        /// Get or Set AutoSet Range
        /// </summary>
        public string AutoSetRange
        {
            get { return autoSetRange; }
            set { autoSetRange = value; }
        }
        const string RangeValue = "Range";
        string range;
        /// <summary>
        /// Get or Set Range property
        /// </summary>
        public string Range
        {
            get { return range; }
            set { range = value; }
        }
        const string RangeCalculationModeValue = "RangeCalculationMode";
        string rangeCalculationMode;
        /// <summary>
        /// Get or Set RangeCalculationMode property
        /// </summary>
        public string RangeCalculationMode
        {
            get { return rangeCalculationMode; }
            set { rangeCalculationMode = value; }
        }
        const string RangePaddingValue = "RangePadding";
        string rangePadding;
        /// <summary>
        /// Get or Set RangePaddding property
        /// </summary>
        public string RangePadding
        {
            get { return rangePadding; }
            set { rangePadding = value; }
        }
        const string IntervalValue = "Interval";
        string interval;
        /// <summary>
        /// Get or Set Interval property
        /// </summary>
        public string Interval
        {
            get { return interval; }
            set { interval = value; }
        }
        const string DesiredIntervalCountValue = "DesiredIntervalCount";
        string desiredIntervalCount;
        /// <summary>
        /// Get or Set DesiredIntervalCount property 
        /// </summary>
        public string DesiredIntervalCount
        {
            get { return desiredIntervalCount; }
            set { desiredIntervalCount = value; }
        }

        const string LabelBackgroundValue = "LabelBackground";
        string labelBackground;
        /// <summary>
        /// Get or Set LabelBackground property
        /// </summary>
        public string LabelBackground
        {
            get { return labelBackground; }
            set { labelBackground = value; }
        }

        const string LabelForegroundValue = "LabelForeground";
        string labelForeground;
        /// <summary>
        /// Gets or sets the LabelForeground property
        /// </summary>
        public string LabelForeground
        {
            get { return labelForeground; }
            set { labelForeground = value; }
        }
        const string LabelBorderBrushValue = "LabelBorderBrush";
        string labelBorderBrush;
        /// <summary>
        /// Get or Set LabelBorderBrush property
        /// </summary>
        public string LabelBorderBrush
        {
            get { return labelBorderBrush; }
            set { labelBorderBrush = value; }
        }
        const string LabelBorderThicknessValue = "LabelBorderThickness";
        string labelBorderThickness;
        /// <summary>
        /// Get or Set labelBorderThickness
        /// </summary>
        public string LabelBorderThickness
        {
            get { return labelBorderThickness; }
            set { labelBorderThickness = value; }
        }
        const string LabelCornerRadiusValue = "LabelCornerRadius";
        string labelCornerRadius;
        /// <summary>
        /// Gets or Sets LabelcornerRadius Property
        /// </summary>
        public string LabelCornerRadius
        {
            get { return labelCornerRadius; }
            set { labelCornerRadius = value; }
        }
        const string HidePartialLabelsValue = "HidePartialLabels";
        string hidePartialLabels;
        /// <summary>
        /// Get or Set HidePartialLabels property
        /// </summary>
        public string HidePartialLabels
        {
            get { return hidePartialLabels; }
            set { hidePartialLabels = value; }
        }
        const string IntersectActionValue = "IntersectAction";
        string intersectAction;
        /// <summary>
        /// Get or Set IntersectAction property
        /// </summary>
        public string IntersectAction
        {
            get { return intersectAction; }
            set { intersectAction = value; }
        }
        const string LabelFormatValue = "LabelFormat";
        string labelFormat;
        /// <summary>
        /// Gets or Sets the LabelFormat Property
        /// </summary>
        public string LabelFormat
        {
            get { return labelFormat; }
            set { labelFormat = value; }
        }
        const string LabelRotateAngleValue = "LabelRotateAngle";
        string labelRotateAngle;
        /// <summary>
        /// Gets or Sets the LabelRotateAngle Property
        /// </summary>
        public string LabelRotateAngle
        {
            get { return labelRotateAngle; }
            set { labelRotateAngle = value; }
        }
        const string LabelsModeValue = "LabelsMode";
        string labelsMode;
        /// <summary>
        /// Gets or Sets the LabelsMode property
        /// </summary>
        public string LabelsMode
        {
            get { return labelsMode; }
            set { labelsMode = value; }
        }
        const string EdgeLabelDrawingModeValue = "EdgeLabelDrawingMode";
        string edgeLabelDrawingMode;
        /// <summary>
        /// Get or Set EdgeLabelDrawingMode property 
        /// </summary>
        public string EdgeLabelDrawingMode
        {
            get { return edgeLabelDrawingMode; }
            set { edgeLabelDrawingMode = value; }
        }
        const string LineStrokeValue = "LineStroke";
        string lineStroke;
        /// <summary>
        /// Get or Set the linestroke property
        /// </summary>
        public string LineStroke
        {
            get { return lineStroke; }
            set { lineStroke = value; }
        }
        const string SmallTickLinesStrokeValue = "SmallTickLinesStroke";
        string smallTickLinesStroke;
        /// <summary>
        /// Get or Set SmallTickLinesStroke
        /// </summary>
        public string SmallTickLinesStroke
        {
            get { return smallTickLinesStroke; }
            set { smallTickLinesStroke = value; }
        }

        const string SmallTicksPerIntervalValue = "SmallTicksPerInterval";
        string smallTicksPerInterval;
        /// <summary>
        /// Get and Set SmallTicksPerInterval Property
        /// </summary>
        public string SmallTicksPerInterval
        {
            get { return smallTicksPerInterval; }
            set { smallTicksPerInterval = value; }
        }
        const string TickLineStrokeValue = "TickLineStroke";
        string tickLineStroke;
        /// <summary>
        /// Gets or Sets the TickLineStroke property
        /// </summary>
        public string TickLineStroke
        {
            get { return tickLineStroke; }
            set { tickLineStroke = value; }
        }
        const string TickSizeValue = "TickSize";
        string tickSize;
        /// <summary>
        /// Gets or Sets the TickSize property
        /// </summary>
        public string TickSize
        {
            get { return tickSize; }
            set { tickSize = value; }
        }
        const string IsLograthimicValue = "IsLograthimic";
        string isLograthimic;
        /// <summary>
        /// Get or Set IsLograthimic property
        /// </summary>
        public string IsLograthimic
        {
            get { return isLograthimic; }
            set { isLograthimic = value; }
        }
        const string LograthimicRangeValue = "LograthimicRange";
        string lograthimicRange;
        /// <summary>
        /// Get or Set LograthimicRange property
        /// </summary>
        public string LograthimicRange
        {
            get { return lograthimicRange; }
            set { lograthimicRange = value; }
        }
        const string LograthimicBaseValue = "LograthimicBase";
        string lograthimicBase;
        /// <summary>
        /// Get or Set LograthimicBase property
        /// </summary>
        public string LograthimicBase
        {
            get { return lograthimicBase; }
            set { lograthimicBase = value; }
        }
        const string DateTimeRangeValue = "DateTimeRange";
        string dateTimeRange;
        /// <summary>
        /// Get or Set DateTimeRange property 
        /// </summary>
        public string DateTimeRange
        {
            get { return dateTimeRange; }
            set { dateTimeRange = value; }
        }
        const string DateTimeIntervalValue = "DateTimeInterval";
        string dateTimeInterval;
        /// <summary>
        /// Get or Set DateTimeInterval property 
        /// </summary>
        public string DateTimeInterval
        {
            get { return dateTimeInterval; }
            set { dateTimeInterval = value; }
        }
        const string LabelDateTimeFormatValue = "LabelDateTimeFormat";
        string labelDateTimeFormat;
        /// <summary>
        /// Gets or Sets the LabelDatetimeFormat property
        /// </summary>
        public string LabelDateTimeFormat
        {
            get { return labelDateTimeFormat; }
            set { labelDateTimeFormat = value; }
        }

        const string ChartLegendValue = "ChartLegend";
        string chartLegend;
        /// <summary>
        /// Get or Set ChartLegend property
        /// </summary>
        public string ChartLegend
        {
            get { return chartLegend; }
            set { chartLegend = value; }
        }

        const string ChartPropertiesValue = "ChartProperties";
        string chartProperties;
        /// <summary>
        /// Get or Set ChartProperties
        /// </summary>
        public string ChartProperties
        {
            get { return chartProperties; }
            set { chartProperties = value; }
        }

        const string ChartPropertiesDialogTitleValue = "ChartPropertiesDialogTitle";
        string chartPropertiesDialogTitle;
        /// <summary>
        /// Get or Set ChartPropertiesDialogTitle property
        /// </summary>
        public string ChartPropertiesDialogTitle
        {
            get { return chartPropertiesDialogTitle; }
            set { chartPropertiesDialogTitle = value; }
        }

        const string SelectChartLegendValue = "SelectChartLegend";
        string selectChartLegend;
        /// <summary>
        /// Get or Set SelectChartLegend property
        /// </summary>
        public string SelectChartLegend
        {
            get { return selectChartLegend; }
            set { selectChartLegend = value; }
        }

        const string LegendPositionValue = "LegendPosition";
        string legendPosition;
        /// <summary>
        /// Get or Set LegendPosition property
        /// </summary>
        public string LegendPosition
        {
            get { return legendPosition; }
            set { legendPosition = value; }
        }


    }
}


