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
    internal class GaugePanelExp
    {
        public BorderExp Border { get; set; }
        public string BackgroundColor { get; set; }
        public FramePropertiesExp GaugeFrame { get; set; }
        public bool AutoLayout { get; set; }
        public PageBreakExp PageBreak { get; set; }
        public BaseImageExp TopImage { get; set; }
        public string AntiAliasing { get; set; }
        public string TextAntiAliasingQuality { get; set; }
        public List<FilterExp> Filters { get; set; }
        public string PageName { get; set; }
        public string ZIndex { get; set; }
        public string ToolTip { get; set; }
        public string Hidden { get; set; }
        public string ShadowIntensity { get; set; }
        public string Direction { get; set; }
        public string NumeralLanguage { get; set; }
        public string NumeralVariant { get; set; }
        public string Calender { get; set; }
        public string Language { get; set; }
        public string DocumentMapLabel { get; set; }
        public string BookMark { get; set; }
        public CustomPropertiesExp CustomProperties { get; set; }
        public List<GaugeLabelExp> GaugeLabels { get; set; }
        public List<GaugePropertiesExp> RadialGauges { get; set; }
        public List<GaugePropertiesExp> LinearGauges { get; set; }
        public List<IndicatorsExp> Indicators { get; set; }
    }

    internal class IndicatorsExp
    {
        public List<ActionInfoExp> ActionInfo { get; set; }
        public string Hidden { get; set; }
        public string GaugeIndicatorStyle { get; set; }
        public string IconSet { get; set; }
        public string ToolTip { get; set; }
        public GaugeInputValueExp IndicatorData { get; set; }
        public string Angle { get; set; }
        public string ZIndex { get; set; }
        public string ScaleFactor { get; set; }
        public string FillColor { get; set; }
        public BaseImageExp StateImage { get; set; }
        public string Top { get; set; }
        public string Left { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public GaugeInputValueExp MaximumValue { get; set; }
        public GaugeInputValueExp MinimumValue { get; set; }
        public string TransformationType { get; set; }
        public List<IndicatorStateExp> IndicatorState { get; set; }
    }

    internal class IndicatorStateExp
    {
        public string FillColor { get; set; }
        public GaugeInputValueExp StartValue { get; set; }
        public GaugeInputValueExp EndValue { get; set; }
        public string IndicatorStyle { get; set; }
        public string ScaleValue { get; set; }
        public string ScaleFactor { get; set; }
        public BaseImageExp StateImage { get; set; }
        public string ResizeMode { get; set; }
        public string HugeColor { get; set; }
        public string Transparency { get; set; }
    }

    internal class GaugeInputValueExp
    {
        public string Multiplier { get; set; }
        public string DataElementName { get; set; }
        public string AddConstant { get; set; }
        public string DataElementOutput { get; set; }
        public string Formula { get; set; }
        public string MaxPercent { get; set; }
        public string MinPercent { get; set; }
        public string Value { get; set; }
    }

    internal class PageBreakExp
    {
        public bool PagebreakDisabled { get; set; }
        public bool ResetPageNumber { get; set; }
    }

    internal class GaugeLabelExp
    {
        public GaugeStyleExp LabelStyle { get; set; }
        public string Angle { get; set; }
        public string ResizeMode { get; set; }
        public string Text { get; set; }
        public bool UseFontPercent { get; set; }
        public ActionInfoExp ActionInfo { get; set; }
        public string Top { get; set; }
        public string Left { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public string ZIndex { get; set; }
        public string ToolTip { get; set; }
        public bool Hidden { get; set; }
        public string TextAlign { get; set; }
        public string TextDecoration { get; set; }
        public string TextColor { get; set; }
        public string TextShadowOffset { get; set; }
        public string VerticalAlign { get; set; }
        public bool ShowEndLabel { get; set; }
        public string FormatString { get; set; }
        public bool AllowUpsideDown { get; set; }
        public bool EndLabel { get; set; }
        public bool RotateLabel { get; set; }
        public string ScalePlacment { get; set; }
        public string ScaleDistance { get; set; }
        public string LabelInterval { get; set; }
        public string LabelIntervalOffset { get; set; }
        public string FontAngle { get; set; }
    }

    internal class ActionInfoExp
    {
        public string Hyperlink { get; set; }
        public string BookmarkLink { get; set; }
        public string ReportName { get; set; }
        public List<ParameterExp> Parameters { get; set; }
    }

    internal class ParameterExp
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Omit { get; set; }
    }
    
    internal class GaugePropertiesExp
    {
        public string GaugeTooltip { get; set; }
        public string GaugeWidth { get; set; }
        public string GaugeHeight { get; set; }
        public string AspectRatio { get; set; }
        public GaugeStyleExp GaugeStyle { get; set; }
        public FramePropertiesExp GaugeFrame { get; set; }
        public bool ClipContent { get; set; }
        public List<GaugeScalePropertiesExp> GaugeScales { get; set; }
        public ActionInfoExp ActionInfo { get; set; }
        public string Top { get; set; }
        public string Left { get; set; }
        public string ZIndex { get; set; }
        public BaseImageExp GaugeTopImage { get; set; }
        public bool Hidden { get; set; }
        public string Xposition { get; set; }
        public string Yposition { get; set; }
        public string Orientation { get; set; }
    }

    internal class FramePropertiesExp
    {
        public string FrameStyle { get; set; }
        public string FrameShape { get; set; }
        public string FrameWidth { get; set; }
        public string FrameGlassEffect { get; set; }
        public GaugeStyleExp BackFrameStyle { get; set; }
        public BaseImageExp FrameImage { get; set; }
        public GaugeStyleExp FrameProperty { get; set; }
    }

    internal class BaseImageExp
    {
        public bool ClipImage { get; set; }
        public string HueColor { get; set; }
        public string Transparency { get; set; }
        public string MIMEType { get; set; }
        public string Source { get; set; }
        public string TransparentColor { get; set; }
        public string Value { get; set; }
    }

    internal class GaugeScalePropertiesExp
    {
        public List<PointerExp> ScalePointer { get; set; }
        public string ToolTip { get; set; }
        public ActionInfoExp ActionInfo { get; set; }
        public List<ScaleRangeExp> ScaleRange { get; set; }
        public bool ReverseDirection { get; set; }
        public GaugeInputValueExp MinimumValue { get; set; }
        public GaugeInputValueExp MaximumValue { get; set; }
        public string ScaleInterval { get; set; }
        public string ScaleIntervaloffset { get; set; }
        public string LabelMultiplier { get; set; }
        public bool LogrithmicScale { get; set; }
        public string LogBase { get; set; }
        public GaugeLabelExp ScaleLabel { get; set; }
        public TickMarksExp MajorTickMark { get; set; }
        public TickMarksExp MinorTickMark { get; set; }
        public GaugeStyleExp ScaleStyle { get; set; }
        public bool TickMarksonTop { get; set; }
        public bool Hidden { get; set; }
        public string ScaleWidth { get; set; }
        public string Position { get; set; }
        public string StartMargin { get; set; }
        public string EndMargin { get; set; }
        public ScalePinExp MaximumPin { get; set; }
        public ScalePinExp MinimumPin { get; set; }
        public string StartAngle { get; set; }
        public string SweepAngle { get; set; }
        public string Radius { get; set; }
        public CustomLabelsExp CustomLabels { get; set; }
    }

    internal class CustomLabelsExp
    {
        public GaugeLabelExp CustomLabel { get; set; }
        public TickMarksExp TickMarkStyle { get; set; }
        public string Value { get; set; }
    }

    internal class ScalePinExp
    {
        public GaugeStyleExp PinStyle { get; set; }
        public string DistanceFromScale { get; set; }
        public bool EnableGradient { get; set; }
        public string FillColor { get; set; }
        public string GradientDensity { get; set; }
        public bool Hidden { get; set; }
        public string Length { get; set; }
        public string Location { get; set; }
        public string PinLocation { get; set; }
        public bool Enable { get; set; }
        public string Placement{ get; set;}
        public string Shape { get; set; }
        public string Width { get; set; }
        public BaseImageExp PinImage { get; set; }
        public GaugeLabelExp PinLabel { get; set; }
    }

    internal class TickMarksExp
    {
        public bool HideTickMark { get; set; }
        public string TickMarkShape { get; set; }
        public string TickMarkPlacement { get; set; }
        public string Width { get; set; }
        public string Length { get; set; }
        public string Interval { get; set; }
        public string IntervalOffset { get; set; }
        public BaseImageExp TickMarkImage { get; set; }
        public GaugeStyleExp TickMarkStyle { get; set; }
        public string DistanceFromScale { get; set; }
        public bool EnableGradient { get; set; }
        public string GradientDensity { get; set; }
        public string FillColor { get; set; }
    }
    
    internal class RadialPointerExp
    {
        public string PointerType { get; set; }
        public string NeedleStyle { get; set; }
        public string MarkerStyle { get; set; }
        public PointerCapExp  CapProperties { get; set; }
    }
    
    internal class PointerCapExp
    {
        public bool Hidden { get; set; }
        public bool OnTop { get; set; }
        public bool Reflection { get; set; }
        public string PointerCapStyle { get; set; }
        public string PointerCapWidth { get; set; }
        public GaugeStyleExp CapStyle { get; set; }
        public BaseImageExp CapImage { get; set; }
        public string OffsetX { get; set; }
        public string OffsetY { get; set; }
    }

    internal class PointerExp
    {
        public GaugeStyleExp PointerStyle { get; set; }
        public PointerImageExp PointerImage { get; set; }
        public GaugeInputValueExp Value { get; set; }
        public string Placement { get; set; }
        public string ScaleDistance { get; set; }
        public string PointerWidth { get; set; }
        public string DataElementName { get; set; }
        public string AddConstant { get; set; }
        public string DataElementOutput { get; set; }
        public string PointerFormula { get; set; }
        public string MaxPercent { get; set; }
        public string MinPercent { get; set; }
        public string Multiplier { get; set; }
        public ActionInfoExp ActionInfo { get; set; }
        public string PointerTooltip { get; set; }
        public bool Hidden { get; set; }
        public bool SnappingEnabled { get; set; }
        public string SnappingInterval { get; set; }
        public string BarStart { get; set; }
        public string MarkerLength { get; set; }
        public string MarkerStyle { get; set; }
        public RadialPointerExp RadialPointer { get; set; }
        public LinearPointerExp LinearPointer { get; set; }
    }
    
    internal class PointerImageExp
    {
        public string PivotX { get; set; }
        public string PivotY { get; set; }
        public BaseImageExp PointerImage { get; set; }
    }

    internal class LinearPointerExp
    {
        public string PointerType { get; set; }
        public GaugeStyleExp ThermometerProperty { get; set; }
        public string BulbSize { get; set; }
        public string BulbOffset { get; set; }
        public string ThermometerStyle { get; set; }
    }

    internal class ScaleRangeExp
    {
        public GaugeInputValueExp StartValue { get; set; }
        public GaugeInputValueExp EndValue { get; set; }
        public string RangePlacement { get; set; }
        public string ScaleDistance { get; set; }
        public string StartWidth { get; set; }
        public string EndWidth { get; set; }
        public GaugeStyleExp RangeStyle { get; set; }
        public ActionInfoExp ActionInfo { get; set; }
        public bool Hidden { get; set; }
        public string InRangeBarColor { get; set; }
        public string InRangeTickmarkColor { get; set; }
        public string InRangeLabelColor { get; set; }
        public string ToolTip { get; set; }
    }

    internal class GaugeStyleExp
    {
        public string BackgroundColor { get; set; }
        public string BackgroundGradientEndcolor { get; set; }
        public string BackgroundGradientType { get; set; }
        public string BackgroundHatchType { get; set; }
        public string BorderWidth { get; set; }
        public string BorderStyle { get; set; }
        public string BorderColor { get; set; }
        public string OffSet { get; set; }
        public string Intensity { get; set; }
        public string FontStyle { get; set; }
        public string FontSize { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public string FontBrush { get; set; }
        public string FontEffect { get; set; }
        public string FontFamily { get; set; }
        public string FontWeight { get; set; }
    }
}
