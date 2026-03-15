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
using Syncfusion.RDL.DOM;

namespace Syncfusion.RDL.ItemModel
{
    internal class ChartItemExpVal
    {
        /// <summary>
        ///set and get the Tool Tip Properties
        /// </summary>
        public string ToolTip
        {
            get;
            set;
        }

        public BorderExpval Border
        {
            get;
            set;
        }

        /// <summary>
        /// get and set Color Palette
        /// </summary>
        public string ColorPalette
        {
            get;
            set;
        }

        public List<string> CustomPaletteColors
        {
            get;
            set;
        }

        public bool Visibility
        {
            get;
            set;
        }

        public string DataSetName
        {
            get;
            set;
        }

        public List<ChartLegendExpVal> ChartLegends
        {
            get;
            set;
        }

        public List<ChartAreasExpVal> ChartAreas
        {
            get;
            set;
        }

        public VisualizationType Type
        {
            get;
            set;
        }

        public VisualizationSubType SubType
        {
            get;
            set;
        }

        public List<ChartTileExpVal> ChartTiles
        {
            get;
            set;
        }

        public ChartBorderSkinExpVal ChartBorderSkin
        {
            get;
            set;
        }

        public ChartTileExpVal ChartNoDataMessage
        {
            get;
            set;
        }

        public ChartMembersExpVal ChartSeriesHierarchy
        {
            get;
            set;
        }

        public ChartMembersExpVal ChartCategoryHierarchy
        {
            get;
            set;
        }

        public TileStyleExpVal ChartStyle
        {
            get;
            set;
        }

        internal string DocumentMapLable
        {
            get;
            set;
        }
    }

    internal class ChartBorderSkinExpVal
    {
        public string ChartBorderSkinType { get; set; }
        public ChartStyleExpVal Style { get; set; }
    }

    internal class ChartAreaAxisExpVal
    {
        public string Name { get; set; }
        public BooleanOptions Visible { get; set; }
        public BorderStyleExpVal Style { get; set; }
        public ChartAxiesExpVal ChartAxisTitle { get; set; }
        public string Margin { get; set; }
        public string Interval { get; set; }
        public IntervalType IntervalType { get; set; }
        public float IntervalOffset { get; set; }
        public IntervalType IntervalOffsetType { get; set; }
        public bool VariableAutoInterval { get; set; }
        public float LabelInterval { get; set; }
        public IntervalType LabelIntervalType { get; set; }
        public float LabelIntervalOffset { get; set; }
        public IntervalType LabelIntervalOffsetType { get; set; }
        public ChartGridLinesExpVal ChartMajorGridLines { get; set; }
        public ChartGridLinesExpVal ChartMinorGridLines { get; set; }
        public ChartTickMarksExpVal ChartMajorTickMarks { get; set; }
        public ChartTickMarksExpVal ChartMinorTickMarks { get; set; }
        public bool MarksAlwaysAtPlotEdge { get; set; }
        public bool Reverse { get; set; }
        public string CrossAt { get; set; }
        public string Location { get; set; }
        public bool Interlaced { get; set; }
        public string InterlacedColor { get; set; }
        public List<ChartStripLineExpVal> ChartStripLines { get; set; }
        public Arrows Arrows { get; set; }
        public bool Scalar { get; set; }
        public string Minimum { get; set; }
        public string Maximum { get; set; }
        public bool LogScale { get; set; }
        public float LogBase { get; set; }
        public bool HideLabels { get; set; }
        public float Angle { get; set; }
        public bool PreventFontShrink { get; set; }
        public bool PreventFontGrow { get; set; }
        public bool PreventLabelOffset { get; set; }
        public bool PreventWordWrap { get; set; }
        public AllowLabelRotation AllowLabelRotation { get; set; }
        public bool IncludeZero { get; set; }
        public bool LabelsAutoFitDisabled { get; set; }
        public Size MinFontSize { get; set; }
        public Size MaxFontSize { get; set; }
        public string OffsetLabels { get; set; }
        public bool HideEndLabels { get; set; }
        public ChartAxisScaleBreakExpVal ChartAxisScaleBreak { get; set; }
        public CustomPropertiesExpVal CustomProperties { get; set; }
        public string LabelColor { get; set; }
        public FontExpVal LabelFont { get; set; }
        public string LabelFormat { get; set; }
        public TextDecoration TextDecoration { get; set; }
    }

    internal class ChartAxisScaleBreakExpVal
    {
        public string Enabled { get; set; }
        public BreakLineType BreakLineType { get; set; }
        public int CollapsibleSpaceThreshold { get; set; }
        public int MaxNumberOfBreaks { get; set; }
        public float Spacing { get; set; }
        public BooleanOptions IncludeZero { get; set; }
        public BorderStyleExpVal Style { get; set; }
    }

    internal class ChartStripLineExpVal
    {
        public TileStyleExpVal Style { get; set; }
        public string Title { get; set; }
        public TextOrientation TextOrientation { get; set; }
        public ActionInfo ActionInfo { get; set; }
        public string ToolTip { get; set; }
        public float Interval { get; set; }
        public IntervalType IntervalType { get; set; }
        public float IntervalOffset { get; set; }
        public IntervalType IntervalOffsetType { get; set; }
        public float StripWidth { get; set; }
        public IntervalType StripWidthType { get; set; }
    }

    internal class ChartTickMarksExpVal
    {
        public BooleanOptions Enabled { get; set; }
        public ChartTickMarksType Type { get; set; }
        public BorderStyleExpVal Style { get; set; }
        public float Length { get; set; }
        public float Interval { get; set; }
        public IntervalType IntervalType { get; set; }
        public float IntervalOffset { get; set; }
        public IntervalType IntervalOffsetType { get; set; }
    }

    internal class ChartGridLinesExpVal
    {
        public BooleanOptions Enabled { get; set; }
        public BorderStyleExpVal Style { get; set; }
        public float Interval { get; set; }
        public IntervalType IntervalType { get; set; }
        public float IntervalOffset { get; set; }
        public IntervalType IntervalOffsetType { get; set; }
    }

    //Chart Area
    internal class ChartAreasExpVal
    {
        public string Name { get; set; }
        public bool Hidden { get; set; }
        public List<ChartSeriesExpVal> ChartSeries { get; set; }
        public TileStyleExpVal Style { get; set; }
        public ChartThreeDPropertiesExpVal Chart3D { get; set; }
        //public ChartCategoryAxes ChartCategoryAxes { get; set; }
        //public ChartValueAxes ChartValueAxes { get; set; }
        public AlignOrientation AlignOrientation { get; set; }
        public ChartAlignTypeExpVal ChartAlignType { get; set; }
        public string AlignWithChartArea { get; set; }
        public ChartElementPositionExpVal ChartElementPosition { get; set; }
        public ChartElementPositionExpVal ChartInnerPlotPosition { get; set; }
        public bool EquallySizedAxesFont { get; set; }
    }

    public class ChartElementPositionExpVal
    {
        public float Top { get; set; }
        public float Left { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
    }

    public class ChartAlignTypeExpVal
    {
        public bool AxesView { get; set; }
        public bool Cursor { get; set; }
        public string Position { get; set; }
        public string InnerPlotPosition { get; set; }
    }

    //Chart 3d Properities
    internal class ChartThreeDPropertiesExpVal
    {
        public bool Enabled { get; set; }
        public ProjectionMode ProjectionMode { get; set; }
        public int Perspective { get; set; }
        public int Rotation { get; set; }
        public int Inclination { get; set; }
        public int DepthRatio { get; set; }
        public Shading Shading { get; set; }
        public int GapDepth { get; set; }
        public int WallThickness { get; set; }
        public bool Clustered { get; set; }
    }

    internal class ChartSeriesExpVal
    {
        public List<PointValuesExpVal> PointValues { get; set; }
        public string ValueAxisName { get; set; }
        public string CategoryAxisName { get; set; }
        public ChartSmartLabelExpVal ChartSmartLabel { get; set; }
        public List<ChartAreaAxisExpVal> ChartAreaYAxis { get; set; }
        public string LegendName { get; set; }
        public ChartItemInLegendExpVal Legend { get; set; }
        public List<ChartAreaAxisExpVal> ChartAreaXAxis { get; set; }
        public TileStyleExpVal Style { get; set; }
        public List<PointStylesExpVal> DataPointsStyle { get; set; }
        public PointStylesExpVal EmptyPointsStyle { get; set; }
        public CustomPropertiesExpVal CustomProperties { get; set; }
        public bool Hidden { get; set; }
        public string Name { get; set; }
    }

    //Every series point values with legend
    internal class ChartSmartLabelExpVal
    {
        public bool Disabled { get; set; }
        public AllowOutSidePlotArea AllowOutSidePlotArea { get; set; }
        public string CalloutBackColor { get; set; }
        public CalloutLineAnchor CalloutLineAnchor { get; set; }
        public string CalloutLineColor { get; set; }
        public LineStyle CalloutLineStyle { get; set; }
        public Size CalloutLineWidth { get; set; }
        public CalloutStyle CalloutStyle { get; set; }
        public bool ShowOverlapped { get; set; }
        public bool MarkerOverlapping { get; set; }
        public Size MaxMovingDistance { get; set; }
        public Size MinMovingDistance { get; set; }
        public ChartNoMoveDirectionExpVal ChartNoMoveDirection { get; set; }
    }

    internal class ChartNoMoveDirectionExpVal
    {
        public bool Up { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
        public bool Down { get; set; }
        public bool UpLeft { get; set; }
        public bool UpRight { get; set; }
        public bool DownLeft { get; set; }
        public bool DownRight { get; set; }
    }

    //Point Values 
    internal class PointValuesExpVal
    {
        public string X { get; set; }
        public string Y { get; set; }
        public string Size { get; set; }
        public string Low { get; set; }
        public string High { get; set; }
        public string End { get; set; }
        public string Start { get; set; }
        public string LegendHidden { get; set; }
        public string LegendText { get; set; }
        public ChartDataLabelExpVal ChartDataLabel { get; set; }
    }

    internal class ChartDataLabelExpVal
    {
        public string BorderColor { get; set; }
        public string TextColor { get; set; }
        public FontExpVal Font { get; set; }
        public string Format { get; set; }
        public string Label { get; set; }
        public TextDecoration TextDecoration { get; set; }
        public BorderStyleExpVal BorderStyle { get; set; }
        public bool UseValueAsLabel { get; set; }
        public bool Visible { get; set; }
        public Position Position { get; set; }
        public int Rotation { get; set; }
        public string ToolTip { get; set; }
        public ActionInfo ActionInfo { get; set; }
    }

    internal class PointStylesExpVal
    {
        public TileStyleExpVal Style { get; set; }
        //public StyleExp DataLabelStyle { get; set; }
        public ChartMarkerExpVal ChartMarker { get; set; }
    }

    internal class ChartMarkerExpVal
    {
        public string BorderColor { get; set; }
        public Size Borderwidth { get; set; }
        public string Color { get; set; }
        public string Type { get; set; }
        public string MarkerType { get; set; }
        public Size Size { get; set; }
        public string ShadowColor { get; set; }
        public string Shadowoffset { get; set; }
        public BackGroundImageExpVal Image { get; set; }
    }

    internal class CustomPropertyExpVal
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    //Chart Axies x & y
    internal class ChartAxiesExpVal
    {
        public string Caption { get; set; }
        public string TextColor { get; set; }
        public FontExpVal Font { get; set; }
        public TextAlign Textalign { get; set; }
        public TextOrientation Textorientation { get; set; }
        public TextDecoration TextDecoration { get; set; }
    }

    internal class FontExpVal
    {
        public Size FontSize { get; set; }
        public string FontFamily { get; set; }
        public FontWeight FontWeight { get; set; }
        public FontStyle FontStyle { get; set; }
    }

    //Chart tile
    internal class ChartTileExpVal
    {
        public string Name { get; set; }
        public string Caption { get; set; }
        public bool Hidden { get; set; }
        public TileStyleExpVal Style { get; set; }
        public Position Position { get; set; }
        public string DocToChartArea { get; set; }
        public bool DockOutChartArea { get; set; }
        public int DockOffset { get; set; }
        public string ToolTip { get; set; }
        public TextOrientation TextOrientation { get; set; }
        public ActionInfo ActionInfo { get; set; }
    }

    internal class ChartLegandTileExpVal
    {
        public string Caption { get; set; }
        public string TitleSeparator { get; set; }
        public string TileAlignment { get; set; }
        public string SeperatorStyle { get; set; }
        public string SeperatorColor { get; set; }
        public FontExpVal Font { get; set; }
    }

    //Chart Legend
    internal class ChartLegendExpVal
    {
        public ChartLegandTileExpVal LegendTile { get; set; }
        public string Name { get; set; }
        public Positions Position { get; set; }
        public bool Hidden { get; set; }
        public TileStyleExpVal Style { get; set; }
        public string Layout { get; set; }
        public string DockToChartArea { get; set; }
        public bool DockOutsideChartArea { get; set; }
        public ChartElementPositionExpVal ChartElementPosition { get; set; }
        public bool AutoFitTextDisabled { get; set; }
        public string MinFontSize { get; set; }
        public string HeaderSeparator { get; set; }
        public string HeaderSeparatorColor { get; set; }
        public string ColumnSeparator { get; set; }
        public string ColumnSeparatorColor { get; set; }
        public int ColumnSpacing { get; set; }
        public bool InterlacedRows { get; set; }
        public string InterlacedRowsColor { get; set; }
        public bool EquallySpacedItems { get; set; }
        public string Reversed { get; set; }
        public int MaxAutoSize { get; set; }
        public int TextWrapThreshold { get; set; }
    }

    public class ChartItemInLegendExpVal
    {
        public string LegendText { get; set; }
        public string ToolTip { get; set; }
        public ActionInfo ActionInfo { get; set; }
        public bool Hidden { get; set; }
    }

    internal class CustomPropertiesExpVal : List<CustomPropertyExpVal>
    {

    }

    internal class BackGroundImageExpVal
    {
        public Source Source { get; set; }
        public string Value { get; set; }
        public string MimeType { get; set; }
        public string BackgroundRepeat { get; set; }
        public string TransparentColor { get; set; }
        public Position Position { get; set; }
    }

    internal class TileStyleExpVal
    {
        public FillStyleExpVal FillStyle { get; set; }
        public BackGroundImageExpVal BackgroundImage { get; set; }
        public BackgroundHatchTypes BackgroundPattern { get; set; }
        public BorderStyleExpVal Border { get; set; }
        public string Color { get; set; }
        public TextOrientation TextOrientation { get; set; }
        public FontExpVal Font { get; set; }
        public TextEffects TextEffect { get; set; }
        public TextDecoration TextDecoration { get; set; }
        public string ShadowColor { get; set; }
        public string ShadowOffset { get; set; }
    }

    internal class BorderStyleExpVal
    {
        public BorderStyles BorderStyle { get; set; }
        public DOM.Size BorderWidth { get; set; }
        public string BorderColor { get; set; }
    }

    internal class FillStyleExpVal
    {
        public string BackgroundColor { get; set; }
        public string BackgroundGradientEndcolor { get; set; }
        public BackgroundGradientTypes BackgroundGradientType { get; set; }
        public BackgroundHatchTypes BackgroundHatchType { get; set; }
    }

    internal class BorderSideExpVal
    {
        public BorderStyleExpVal Left { get; set; }
        public BorderStyleExpVal Top { get; set; }
        public BorderStyleExpVal Bottem { get; set; }
        public BorderStyleExpVal Right { get; set; }
    }

    internal class BorderSkinExpVal
    {
        public FillStyleExpVal FillStyle { get; set; }
        public BackGroundImageExpVal BackgroundImage { get; set; }
        public string BackgroundPattern { get; set; }
        public BorderStyleExpVal Border { get; set; }
        public string PageColor { get; set; }
    }

    internal class ChartStyleExpVal
    {
        public BackGroundImageExpVal BackgroundImage { get; set; }
        public BackgroundHatchTypes BackgroundPattern { get; set; }
        public BorderStyleExpVal Border { get; set; }
        public BorderSideExpVal BorderSyle { get; set; }
        public FillStyleExpVal FillStyle { get; set; }
    }

    internal class ChartMembersExpVal : List<ChartMemberExpVal>
    {

    }

    internal class ChartMemberExpVal
    {
        public GroupExpVal Group { get; set; }
        public SortExpressionsExpVal SortExpressions { get; set; }
        public ChartMembersExpVal ChartMembers { get; set; }
        public string Label { get; set; }
        public CustomPropertiesExpVal CustomProperties { get; set; }
        public string DataElementName { get; set; }
        public DataElementOutputs DataElementOutput { get; set; }
    }

    internal class SortExpressionsExpVal : List<SortExpressionExpVal>
    {
    }

    internal class SortExpressionExpVal
    {
        public string Value { get; set; }
        public SortDirection Direction { get; set; }
    }

    internal class GroupExpVal
    {
        public string Name { get; set; }
        public string DocumentMapLabel { get; set; }
        public List<string> GroupExpressions { get; set; }
        public string DomainScope { get; set; }
        public RDL.DOM.BreakLocation BreakLocation { get; set; }
        public FiltersExpVal Filters { get; set; }
        public string Parent { get; set; }
        public string DataElementName { get; set; }
    }

    internal class FiltersExpVal : List<FilterExpVal>
    {

    }

    internal class FilterExpVal
    {
        public string FilterExpression { get; set; }
        public FilterOperators Operator { get; set; }
        public FilterValuesExpVal FilterValues { get; set; }
    }

    internal class FilterValuesExpVal : List<FilterValueExpVal>
    {
    }

    internal class FilterValueExpVal
    {
        public string Value { get; set; }
        public RDL.DOM.DataTypes DataType { get; set; }
    }

}