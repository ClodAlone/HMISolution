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

    internal class GaugePanelExpVal
    {
        public BorderExpval Border { get; set; }
        public string BackgroundColor { get; set; }
        public FramePropertiesExpVal GaugeFrame { get; set; }
        public bool AutoLayout { get; set; }
        public PageBreakExpVal PageBreak { get; set; }
        public BaseImageExpVal TopImage { get; set; }
        public AntiAliasing AntiAliasing { get; set; }
        public TextAntiAliasingQuality TextAntiAliasingQuality { get; set; }
        public FilterExpVal Filters { get; set; }
        public string PageName { get; set; }
        public int ZIndex { get; set; }
        public string ToolTip { get; set; }
        public bool Hidden { get; set; }
        public double ShadowIntensity { get; set; }
        public Direction Direction { get; set; }
        public string NumeralLanguage { get; set; }
        public string NumeralVariant { get; set; }
        public Calendar Calender { get; set; }
        public string Language { get; set; }
        public string DocumentMapLabel { get; set; }
        public string BookMark { get; set; }
        public CustomPropertiesExpVal CustomProperties { get; set; }
        public List<GaugeLabelExpVal> GaugeLabels { get; set; }
        public List<GaugePropertiesExpVal> RadialGauges { get; set; }
        public List<GaugePropertiesExpVal> LinearGauges { get; set; }
        public List<IndicatorsExpVal> Indicators { get; set; }
    }

    internal class IndicatorsExpVal
    {
        public List<ActionInfoExpVal> ActionInfo { get; set; }
        public bool Hidden { get; set; }
        public GaugeStateIndicatorStyles GaugeIndicatorStyle { get; set; }
        public string ToolTip { get; set; }
        public StateIndicatorIconsSet IconSet { get; set; }
        public GaugeInputValueExpVal IndicatorData { get; set; }
        public double Angle { get; set; }
        public int ZIndex { get; set; }
        public double ScaleFactor { get; set; }
        public string FillColor { get; set; }
        public BaseImageExpVal StateImage { get; set; }
        public double Top { get; set; }
        public double Left { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public GaugeInputValueExpVal MaximumValue { get; set; }
        public GaugeInputValueExpVal MinimumValue { get; set; }
        public TransformationType TransformationType { get; set; }
        public List<IndicatorStateExpVal> IndicatorState { get; set; }
    }

    internal class IndicatorStateExpVal
    {
        public ResizeMode ResizeMode { get; set; }
        public string FillColor { get; set; }
        public GaugeInputValueExpVal StartValue { get; set; }
        public GaugeInputValueExpVal EndValue { get; set; }
        public GaugeStateIndicatorStyles IndicatorStyle { get; set; }
        public int ScaleValue { get; set; }
        public double ScaleFactor { get; set; }
        public BaseImageExpVal StateImage { get; set; }
        public string HugeColor { get; set; }
        public double Transparency { get; set; }
    }

    internal class GaugeInputValueExpVal
    {
        public double Multiplier { get; set; }
        public string DataElementName { get; set; }
        public string AddConstant { get; set; }
        public DataElementOutputs DataElementOutput { get; set; }
        public Formula Formula { get; set; }
        public double MaxPercent { get; set; }
        public double MinPercent { get; set; }
        public double Value { get; set; }
    }

    internal class PageBreakExpVal
    {
        public bool PagebreakDisabled { get; set; }
        public bool ResetPageNumber { get; set; }
    }
    
    internal class GaugeLabelExpVal
    {
        public GaugeStyleExpVal LabelStyle { get; set; }
        public double Angle { get; set; }
        public string ResizeMode { get; set; }
        public string Text { get; set; }
        public bool UseFontPercent { get; set; }
        public ActionInfoExpVal ActionInfo { get; set; }
        public double Top { get; set; }
        public double Left { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public int ZIndex { get; set; }
        public string ToolTip { get; set; }
        public bool Hidden { get; set; }
        public TextAlign TextAlign { get; set; }
        public TextDecoration TextDecoration { get; set; }
        public string TextColor { get; set; }
        public double TextShadowOffset { get; set; }
        public VerticalAlign VerticalAlign { get; set; }
        public bool ShowEndLabel { get; set; }
        public string FormatString { get; set; }
        public bool AllowUpsideDown { get; set; }
        public bool EndLabel { get; set; }
        public bool RotateLabel { get; set; }
        public Placement ScalePlacment { get; set; }
        public float ScaleDistance { get; set; }
        public double LabelInterval { get; set; }
        public double LabelIntervalOffset { get; set; }
        public double FontAngle { get; set; }
    }

    internal class ActionInfoExpVal
    {
        public string Hyperlink { get; set; }
        public string ReportName { get; set; }
        public List<ParameterExpVal> Parameters { get; set; }
        public string BookmarkLink { get; set; }
    }
        
    internal class ParameterExpVal
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Omit { get; set; }
    }

    internal class GaugePropertiesExpVal
    {
        public string GaugeTooltip { get; set; }
        public double GaugeWidth { get; set; }
        public double GaugeHeight { get; set; }
        public int AspectRatio { get; set; }
        public GaugeStyleExpVal GaugeStyle { get; set; }
        public FramePropertiesExpVal GaugeFrame { get; set; }
        public bool ClipContent { get; set; }
        public List<GaugeScalePropertiesExpVal> GaugeScales { get; set; }
        public ActionInfoExpVal ActionInfo { get; set; }
        public double Top { get; set; }
        public double Left { get; set; }
        public int ZIndex { get; set; }
        public BaseImageExpVal GaugeTopImage { get; set; }
        public bool Hidden { get; set; }
        public double Xposition { get; set; }
        public double Yposition { get; set; }
        public Orientation Orientation { get; set; }
    }

    internal class FramePropertiesExpVal
    {
        public FrameStyle FrameStyle { get; set; }
        public FrameShape FrameShape { get; set; }
        public double FrameWidth { get; set; }
        public GlassEffect FrameGlassEffect { get; set; }
        public GaugeStyleExpVal BackFrameStyle { get; set; }
        public BaseImageExpVal FrameImage { get; set; }
        public GaugeStyleExpVal FrameProperty { get; set; }
    }

    internal class BaseImageExpVal
    {
        public bool ClipImage { get; set; }
        public string HueColor { get; set; }
        public string Transparency { get; set; }
        public string MIMEType { get; set; }
        public Source Source { get; set; }
        public string TransparentColor { get; set; }
        public string Value { get; set; }
    }

    internal class GaugeScalePropertiesExpVal
    {
        public List<PointerExpVal> ScalePointer { get; set; }
        public string ToolTip { get; set; }
        public ActionInfoExpVal ActionInfo { get; set; }
        public List<ScaleRangeExpVal> ScaleRange { get; set; }
        public bool ReverseDirection { get; set; }
        public GaugeInputValueExpVal MinimumValue { get; set; }
        public GaugeInputValueExpVal MaximumValue { get; set; }
        public double ScaleInterval { get; set; }
        public double ScaleIntervaloffset { get; set; }
        public double LabelMultiplier { get; set; }
        public bool LogrithmicScale { get; set; }
        public double LogBase { get; set; }
        public GaugeLabelExpVal ScaleLabel { get; set; }
        public TickMarksExpVal MajorTickMark { get; set; }
        public TickMarksExpVal MinorTickMark { get; set; }
        public GaugeStyleExpVal ScaleStyle { get; set; }
        public bool TickMarksonTop { get; set; }
        public bool Hidden { get; set; }
        public double ScaleWidth { get; set; }
        public Position Position { get; set; }
        public double StartMargin { get; set; }
        public double EndMargin { get; set; }
        public ScalePinExpVal MaximumPin { get; set; }
        public ScalePinExpVal MinimumPin { get; set; }
        public double StartAngle { get; set; }
        public double SweepAngle { get; set; }
        public double Radius { get; set; }
        public CustomLabelsExpVal CustomLabels { get; set; }
    }

    internal class CustomLabelsExpVal
    {
        public GaugeLabelExpVal CustomLabel { get; set; }
        public TickMarksExpVal TickMarkStyle { get; set; }
        public string Value { get; set; }
    }

    internal class ScalePinExpVal
    {
        public GaugeStyleExpVal PinStyle { get; set; }
        public double DistanceFromScale { get; set; }
        public bool EnableGradient { get; set; }
        public string FillColor { get; set; }
        public double GradientDensity { get; set; }
        public bool Hidden { get; set; }
        public double Length { get; set; }
        public string Location { get; set; }
        public string PinLocation { get; set; }
        public bool Enable { get; set; }
        public Placement Placement { get; set; }
        public string Shape { get; set; }
        public double Width { get; set; }
        public BaseImageExpVal PinImage { get; set; }
        public GaugeLabelExpVal PinLabel { get; set; }
    }
    
    internal class TickMarksExpVal
    {
        public bool HideTickMark { get; set; }
        public string TickMarkShape { get; set; }
        public Placement TickMarkPlacement { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double Interval { get; set; }
        public double IntervalOffset { get; set; }
        public BaseImageExpVal TickMarkImage { get; set; }
        public GaugeStyleExpVal TickMarkStyle { get; set; }
        public double DistanceFromScale { get; set; }
        public bool EnableGradient { get; set; }
        public double GradientDensity { get; set; }
        public string FillColor { get; set; }
    }

    internal class RadialPointerExpVal
    {
        public RadialPointerType PointerType { get; set; }
        public NeedleStyleGauge NeedleStyle { get; set; }
        public MarkerStyle MarkerStyle { get; set; }
        public PointerCapExpVal CapProperties { get; set; }
    }

    internal class PointerCapExpVal
    {
        public bool Hidden { get; set; }
        public bool OnTop { get; set; }
        public bool Reflection { get; set; }
        public CapStyle PointerCapStyle { get; set; }
        public double PointerCapWidth { get; set; }
        public GaugeStyleExpVal CapStyle { get; set; }
        public BaseImageExpVal CapImage { get; set; }
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
    }

    internal class PointerExpVal
    {
        public GaugeStyleExpVal PointerStyle { get; set; }
        public PointerImageExpVal PointerImage { get; set; }
        public GaugeInputValueExpVal Value { get; set; }
        public Placement Placement { get; set; }
        public double ScaleDistance { get; set; }
        public double PointerWidth { get; set; }
        public string DataElementName { get; set; }
        public string AddConstant { get; set; }
        public string DataElementOutput { get; set; }
        public Formula PointerFormula { get; set; }
        public string MaxPercent { get; set; }
        public string MinPercent { get; set; }
        public string Multiplier { get; set; }
        public ActionInfoExpVal ActionInfo { get; set; }
        public string PointerTooltip { get; set; }
        public bool Hidden { get; set; }
        public bool SnappingEnabled { get; set; }
        public double SnappingInterval { get; set; }
        public BarStart BarStart { get; set; }
        public double MarkerLength { get; set; }
        public MarkerStyle MarkerStyle { get; set; }
        public RadialPointerExpVal RadialPointer { get; set; }
        public LinearPointerExpVal LinearPointer { get; set; }
    }

    internal class PointerImageExpVal
    {
        public double PivotX { get; set; }
        public double PivotY { get; set; }
        public BaseImageExpVal PointerImage { get; set; }
    }

    internal class LinearPointerExpVal
    {
        public LinearPointerType PointerType { get; set; }
        public GaugeStyleExpVal ThermometerProperty { get; set; }
        public double BulbSize { get; set; }
        public double BulbOffset { get; set; }
        public ThermometerStyle ThermometerStyle { get; set; }
    }

    internal class ScaleRangeExpVal
    {
        public GaugeInputValueExpVal StartValue { get; set; }
        public GaugeInputValueExpVal EndValue { get; set; }
        public Placement RangePlacement { get; set; }
        public double ScaleDistance { get; set; }
        public double StartWidth { get; set; }
        public double EndWidth { get; set; }
        public GaugeStyleExpVal RangeStyle { get; set; }
        public ActionInfoExpVal ActionInfo { get; set; }
        public bool Hidden { get; set; }
        public string InRangeBarColor { get; set; }
        public string InRangeTickmarkColor { get; set; }
        public string InRangeLabelColor { get; set; }
        public string ToolTip { get; set; }
    }

    internal class GaugeStyleExpVal
    {
        public string BackgroundColor { get; set; }
        public string BackgroundGradientEndcolor { get; set; }
        public BackgroundGradientType BackgroundGradientType { get; set; }
        public BackgroundHatchTypes BackgroundHatchType { get; set; }
        public double BorderWidth { get; set; }
        public BorderStyles BorderStyle { get; set; }
        public string BorderColor { get; set; }
        public double OffSet { get; set; }
        public double Intensity { get; set; }
        public FontStyle FontStyle { get; set; }
        public double FontSize { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public string FontBrush { get; set; }
        public string FontEffect { get; set; }
        public string FontFamily { get; set; }
        public FontWeight FontWeight { get; set; }
    } 
}
