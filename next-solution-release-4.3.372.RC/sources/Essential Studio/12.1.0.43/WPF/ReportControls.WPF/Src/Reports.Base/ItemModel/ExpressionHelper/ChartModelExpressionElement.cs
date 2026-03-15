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

namespace Syncfusion.RDL.ItemModel
{

    internal class ChartItemExp
    {
        /// <summary>
        ///set and get the Tool Tip Properties
        /// </summary>
        public string ToolTip
        {
            get;
            set;
        }

        /// <summary>
        /// get and set Border
        /// </summary>
        public BorderExp Border
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

        public string Visibility
        {
            get;
            set;
        }

        public string DataSetName
        {
            get;
            set;
        }

        public List<ChartLegendExp> ChartLegends
        {
            get;
            set;
        }

        public List<ChartAreasExp> ChartAreas
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        public string SubType
        {
            get;
            set;
        }

        public List<ChartTileExp> ChartTiles
        {
            get;
            set;
        }

        public ChartBorderSkinExp ChartBorderSkin
        {
            get;
            set;
        }

        public ChartTileExp ChartNoDataMessage
        {
            get;
            set;
        }

        public ChartMembersExp ChartSeriesHierarchy 
        { 
            get; 
            set; 
        }

        public ChartMembersExp ChartCategoryHierarchy 
        { 
            get; 
            set; 
        }

        public TileStyleExp ChartStyle
        {
            get;
            set;
        }

        public string DocumentMapLable
        {
            get;
            set;
        }
    }

    internal class ChartBorderSkinExp
    {
        public string ChartBorderSkinType { get; set; }
        public ChartStyleExp Style { get; set; }
    }

    internal class ChartAreaAxisExp
    {
        public string Name { get; set; }
        public string Visible { get; set; }
        public BorderStyleExp Style { get; set; }
        public ChartAxiesExp ChartAxisTitle { get; set; }
        public string Margin { get; set; }
        public string Interval { get; set; }
        public string IntervalType { get; set; }
        public string IntervalOffset { get; set; }
        public string IntervalOffsetType { get; set; }
        public string VariableAutoInterval { get; set; }
        public string LabelInterval { get; set; }
        public string LabelIntervalType { get; set; }
        public string LabelIntervalOffset { get; set; }
        public string LabelIntervalOffsetType { get; set; }
        public ChartGridLinesExp ChartMajorGridLines { get; set; }
        public ChartGridLinesExp ChartMinorGridLines { get; set; }
        public ChartTickMarksExp ChartMajorTickMarks { get; set; }
        public ChartTickMarksExp ChartMinorTickMarks { get; set; }
        public string MarksAlwaysAtPlotEdge { get; set; }
        public string Reverse { get; set; }
        public string CrossAt { get; set; }
        public string Location { get; set; }
        public string Interlaced { get; set; }
        public string InterlacedColor { get; set; }
        public List<ChartStripLineExp> ChartStripLines { get; set; }
        public string Arrows { get; set; }
        public bool Scalar { get; set; }
        public string Minimum { get; set; }
        public string Maximum { get; set; }
        public string LogScale { get; set; }
        public string LogBase { get; set; }
        public string HideLabels { get; set; }
        public string Angle { get; set; }
        public string PreventFontShrink { get; set; }
        public string PreventFontGrow { get; set; }
        public string PreventLabelOffset { get; set; }
        public string PreventWordWrap { get; set; }
        public string AllowLabelRotation { get; set; }
        public string IncludeZero { get; set; }
        public string LabelsAutoFitDisabled { get; set; }
        public string MinFontSize { get; set; }
        public string MaxFontSize { get; set; }
        public string OffsetLabels { get; set; }
        public string HideEndLabels { get; set; }
        public ChartAxisScaleBreakExp ChartAxisScaleBreak { get; set; }
        public CustomPropertiesExp CustomProperties { get; set; }
        public string LabelColor { get; set; }
        public FontExp LabelFont { get; set; }
        public string LabelFormat { get; set; }
        public string TextDecoration { get; set; }
    }

    internal class ChartAxisScaleBreakExp
    {
        public string Enabled { get; set; }
        public string BreakLineType { get; set; }
        public string CollapsibleSpaceThreshold { get; set; }
        public string MaxNumberOfBreaks { get; set; }
        public string Spacing { get; set; }
        public string IncludeZero { get; set; }
        public BorderStyleExp Style { get; set; }
    }

    internal class ChartStripLineExp
    {
        public TileStyleExp Style { get; set; }
        public string Title { get; set; }
        public string TextOrientation { get; set; }
        public string ActionInfo { get; set; }
        public string ToolTip { get; set; }
        public string Interval { get; set; }
        public string IntervalType { get; set; }
        public string IntervalOffset { get; set; }
        public string IntervalOffsetType { get; set; }
        public string StripWidth { get; set; }
        public string StripWidthType { get; set; }
    }

    internal class ChartTickMarksExp
    {
        public string Enabled { get; set; }
        public string Type { get; set; }
        public BorderStyleExp Style { get; set; }
        public string Length { get; set; }
        public string Interval { get; set; }
        public string IntervalType { get; set; }
        public string IntervalOffset { get; set; }
        public string IntervalOffsetType { get; set; }
    }

    internal class ChartGridLinesExp
    {
        public string Enabled { get; set; }
        public BorderStyleExp Style { get; set; }
        public string Interval { get; set; }
        public string IntervalType { get; set; }
        public string IntervalOffset { get; set; }
        public string IntervalOffsetType { get; set; }
    }

    //Chart Area
    internal class ChartAreasExp
    {
        public string Name { get; set; }
        public string Hidden { get; set; }
        public List<ChartSeriesExp> ChartSeries { get; set; }
        public TileStyleExp Style { get; set; }
        public ChartThreeDPropertiesExp Chart3D { get; set; }
        //public ChartCategoryAxes ChartCategoryAxes { get; set; }
        //public ChartValueAxes ChartValueAxes { get; set; }
        public string AlignOrientation { get; set; }
        public ChartAlignTypeExp ChartAlignType { get; set; }
        public string AlignWithChartArea { get; set; }
        public ChartElementPositionExp ChartElementPosition { get; set; }
        public ChartElementPositionExp ChartInnerPlotPosition { get; set; }
        public string EquallySizedAxesFont { get; set; }
    }

    public class ChartElementPositionExp
    {
        public float Top { get; set; }
        public float Left { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
    }

    public class ChartAlignTypeExp
    {
        public string AxesView { get; set; }
        public string Cursor { get; set; }
        public string Position { get; set; }
        public string InnerPlotPosition { get; set; }
    }

    //Chart 3d Properities
    internal class ChartThreeDPropertiesExp
    {
        public string Enabled { get; set; }
        public string ProjectionMode { get; set; }
        public string Perspective { get; set; }
        public string Rotation { get; set; }
        public string Inclination { get; set; }
        public string DepthRatio { get; set; }
        public string Shading { get; set; }
        public string GapDepth { get; set; }
        public string WallThickness { get; set; }
        public string Clustered { get; set; }
    }

    internal class ChartSeriesExp
    {
        public List<PointValuesExp> PointValues { get; set; }
        public string ValueAxisName { get; set; }
        public string CategoryAxisName { get; set; }
        public ChartSmartLabelExp ChartSmartLabel { get; set; }
        public List<ChartAreaAxisExp> ChartAreaYAxis { get; set; }
        public string LegendName { get; set; }
        public ChartItemInLegendExp Legend { get; set; }
        public List<ChartAreaAxisExp> ChartAreaXAxis { get; set; }
        public TileStyleExp Style { get; set; }
        public List<PointStylesExp> DataPointsStyle { get; set; }
        public PointStylesExp EmptyPointsStyle { get; set; }
        public CustomPropertiesExp CustomProperties { get; set; }
        public string Hidden { get; set; }
        public string Name { get; set; }
    }

    //Every series point values with legend
    internal class ChartSmartLabelExp
    {
        public string Disabled { get; set; }
        public string AllowOutSidePlotArea { get; set; }
        public string CalloutBackColor { get; set; }
        public string CalloutLineAnchor { get; set; }
        public string CalloutLineColor { get; set; }
        public string CalloutLineStyle { get; set; }
        public string CalloutLineWidth { get; set; }
        public string CalloutStyle { get; set; }
        public string ShowOverlapped { get; set; }
        public string MarkerOverlapping { get; set; }
        public string MaxMovingDistance { get; set; }
        public string MinMovingDistance { get; set; }
        public ChartNoMoveDirectionExp ChartNoMoveDirection { get; set; }
    }

    internal class ChartNoMoveDirectionExp
    {
        public string Up { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
        public string Down { get; set; }
        public string UpLeft { get; set; }
        public string UpRight { get; set; }
        public string DownLeft { get; set; }
        public string DownRight { get; set; }
    }

    //Point Values 
    internal class PointValuesExp
    {
        public string X { get; set; }
        public string Y { get; set; }
        public string Size { get; set; }
        public string Mean { get; set; }
        public string Median { get; set; }
        public string Low { get; set; }
        public string High { get; set; }
        public string End { get; set; }
        public string Start { get; set; }
        public string LegendHidden { get; set; }
        public string LegendText { get; set; }
        public ChartDataLabelExp ChartDataLabel { get; set; }
    }

    internal class ChartDataLabelExp
    {
        public string BorderColor { get; set; }
        public string TextColor { get; set; }
        public FontExp Font { get; set; }
        public string Format { get; set; }
        public string Label { get; set; }
        public string TextDecoration { get; set; }
        public BorderStyleExp BorderStyle { get; set; }
        public string UseValueAsLabel { get; set; }
        public string Visible { get; set; }
        public string Position { get; set; }
        public string Rotation { get; set; }
        public string ToolTip { get; set; }
        public string ActionInfo { get; set; }
    }

    internal class PointStylesExp
    {
        public TileStyleExp Style { get; set; }
        //public StyleExp DataLabelStyle { get; set; }
        public ChartMarkerExp ChartMarker { get; set; }
    }

    internal class ChartMarkerExp
    {
        public string BorderColor { get; set; }
        public string Borderwidth { get; set; }
        public string Color { get; set; }
        public string Type { get; set; }
        public string MarkerType { get; set; }
        public string Size { get; set; }
        public string ShadowColor { get; set; }
        public string Shadowoffset { get; set; }
        public BackGroundImageExp Image { get; set; }
    }

    internal class CustomPropertyExp
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    //Chart Axies x & y
    internal class ChartAxiesExp
    {
        public string Caption { get; set; }
        public string TextColor { get; set; }
        public FontExp Font { get; set; }
        public string Textalign { get; set; }
        public string Textorientation { get; set; }
        public string TextDecoration { get; set; }
    }

    //Chart tile
    internal class ChartTileExp
    {
        public string Name { get; set; }
        public string Caption { get; set; }
        public string Hidden { get; set; }
        public TileStyleExp Style { get; set; }
        public string Position { get; set; }
        public string DockOutChartArea { get; set; }
        public string DocToChartArea { get; set; }
        public string DockOffset { get; set; }
        public string ToolTip { get; set; }
        public string TextOerientation { get; set; }
        public string ActionInfo { get; set; }
    }

    internal class ChartLegandTileExp
    {
        public string Caption { get; set; }
        public string TitleSeparator { get; set; }
        public string TileAlignment { get; set; }
        public string SeperatorStyle { get; set; }
        public string SeperatorColor { get; set; }
        public FontExp Font { get; set; }
    }

    //Chart Legend
    internal class ChartLegendExp
    {
        public ChartLegandTileExp LegendTile { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Hidden { get; set; }
        public TileStyleExp Style { get; set; }
        public string Layout { get; set; }
        public string DockToChartArea { get; set; }
        public string DockOutsideChartArea { get; set; }
        public ChartElementPositionExp ChartElementPosition { get; set; }
        public string AutoFitTextDisabled { get; set; }
        public string MinFontSize { get; set; }
        public string HeaderSeparator { get; set; }
        public string HeaderSeparatorColor { get; set; }
        public string ColumnSeparator { get; set; } 
        public string ColumnSeparatorColor { get; set; }
        public string ColumnSpacing { get; set; }
        public string InterlacedRows { get; set; }
        public string InterlacedRowsColor { get; set; }
        public string EquallySpacedItems { get; set; }
        public string Reversed { get; set; }
        public string MaxAutoSize { get; set; }
        public string TextWrapThreshold { get; set; }
    }

    public class ChartItemInLegendExp
    {
        public string LegendText { get; set; }
        public string ToolTip { get; set; }
        public string ActionInfo { get; set; }
        public string Hidden { get; set; }
    }

    internal class CustomPropertiesExp : List<CustomPropertyExp>
    {

    }

    internal class BackGroundImageExp
    {
        public string Source { get; set; }
        public string Value { get; set; }
        public string MimeType { get; set; }
        public string BackgroundRepeat { get; set; }
        public string TransparentColor { get; set; }
        public string Position { get; set; }
    }

    internal class TileStyleExp
    {
        public FillStyleExp FillStyle { get; set; }
        public BackGroundImageExp BackgroundImage { get; set; }
        public string BackgroundPattern { get; set; }
        public BorderStyleExp Border { get; set; }
        public string Color { get; set; }
        public string TextOrientation { get; set; }
        public FontExp Font { get; set; }
        public string TextEffect { get; set; }
        public string TextDecoration { get; set; }
        public string ShadowColor { get; set; }
        public string ShadowOffset { get; set; }
    }

    internal class BorderStyleExp
    {
        public string BorderStyle { get; set; }
        public string BorderWidth { get; set; }
        public string BorderColor { get; set; }
    }

    internal class FillStyleExp
    {
        public string BackgroundColor { get; set; }
        public string BackgroundGradientEndcolor { get; set; }
        public string BackgroundGradientType { get; set; }
        public string BackgroundHatchType { get; set; }
    }

    internal class BorderSideExp
    {
        public BorderStyleExp Left { get; set; }
        public BorderStyleExp Top { get; set; }
        public BorderStyleExp Bottem { get; set; }
        public BorderStyleExp Right { get; set; }
    }

    internal class BorderSkinExp
    {
        public FillStyleExp FillStyle { get; set; }
        public BackGroundImageExp BackgroundImage { get; set; }
        public string BackgroundPattern { get; set; }
        public BorderStyleExp Border { get; set; }
        public string PageColor { get; set; }
    }

    internal class ChartStyleExp
    {
        public BackGroundImageExp BackgroundImage { get; set; }
        public string BackgroundPattern { get; set; }
        public BorderStyleExp Border { get; set; }
        public BorderSideExp BorderSyle { get; set; }
        public FillStyleExp FillStyle { get; set; }
    }

    internal class ChartMembersExp : List<ChartMemberExp>
    {

    }

    internal class ChartMemberExp
    {
        public GroupExp Group { get; set; }
        public SortExpressionsExp SortExpressions { get; set; }
        public ChartMembersExp ChartMembers { get; set; }
        public string Label { get; set; }
        public CustomPropertiesExp CustomProperties { get; set; }
        public string DataElementName { get; set; }
        public string DataElementOutput { get; set; }
    }

    internal class SortExpressionsExp : List<SortExpressionExp>
    {
    }

    internal class SortExpressionExp
    {
        public string Value { get; set; }
        public string Direction { get; set; }
    }

    internal class GroupExp
    {
        public string Name { get; set; }
        public string DocumentMapLabel { get; set; }
        public List<string> GroupExpressions { get; set; }
        public string DomainScope { get; set; }
        public string BreakLocation { get; set; }
        public FiltersExp Filters { get; set; }
        public string Parent { get; set; }
        public string DataElementName { get; set; }
    }

    internal class FiltersExp:List<FilterExp>
    {

    }

    internal class FilterExp
    {
        public string FilterExpression { get; set; }
        public string Operator { get; set; }
        public FilterValuesExp FilterValues { get; set; }
    }

    internal class FilterValuesExp : List<FilterValueExp>
    {
    }

    internal class FilterValueExp
    {
        public string Value { get; set; }
        public string DataType { get; set; }
    }
 
}
