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
using System.Xml.Serialization;
using System.ComponentModel;

namespace Syncfusion.RDL.DOM
{
    public class ChartCategoryAxes : List<ChartAxis>
    {

    }

    public class ChartValueAxes : List<ChartAxis>
    {

    }


    public class ChartAxis
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [DefaultValue(BooleanOptions.Auto)]
        public BooleanOptions Visible { get; set; }
        public Style Style { get; set; }
        public ChartAxisTitle ChartAxisTitle { get; set; }
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
        public ChartMajorGridLines ChartMajorGridLines { get; set; }
        public ChartMinorGridLines ChartMinorGridLines { get; set; }
        public ChartMajorTickMarks ChartMajorTickMarks { get; set; }
        public ChartMinorTickMarks ChartMinorTickMarks { get; set; }
        public bool MarksAlwaysAtPlotEdge { get; set; }
        public bool Reverse { get; set; }
        public string CrossAt { get; set; }
        public Location Location { get; set; }
        public bool Interlaced { get; set; }
        public string InterlacedColor { get; set; }
        public ChartStripLines ChartStripLines { get; set; }
        public Arrows Arrows { get; set; }
        public bool Scalar { get; set; }
        public string Minimum { get; set; }
        public string Maximum { get; set; }
        public bool LogScale { get; set; }
        [DefaultValue(0)]
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
        public ChartAxisScaleBreak ChartAxisScaleBreak { get; set; }
        public CustomProperties CustomProperties { get; set; }

        public bool ShouldSerializeCustomProperties()
        {
            return CustomProperties != null && CustomProperties.Count > 0;
        }

        public void ResetCustomProperties()
        {
            this.CustomProperties = new CustomProperties();
        }

        public bool ShouldSerializeAllowLabelRotation()
        {
            return AllowLabelRotation != DOM.AllowLabelRotation.None;
        }

        public void ResetAllowLabelRotation()
        {
            this.AllowLabelRotation = DOM.AllowLabelRotation.None;
        }

        public bool ShouldSerializeLabelIntervalOffsetType()
        {
            return LabelIntervalOffsetType != DOM.IntervalType.Auto;
        }

        public void ResetLabelIntervalOffsetType()
        {
            this.LabelIntervalOffsetType = DOM.IntervalType.Auto;
        }

        public bool ShouldSerializeIntervalOffsetType()
        {
            return IntervalOffsetType != DOM.IntervalType.Auto;
        }

        public void ResetIntervalOffsetType()
        {
            this.IntervalOffsetType = DOM.IntervalType.Auto;
        }

        public bool ShouldSerializeLabelIntervalType()
        {
            return LabelIntervalType != DOM.IntervalType.Auto;
        }

        public void ResetLabelIntervalType()
        {
            this.LabelIntervalType = DOM.IntervalType.Auto;
        }

        public bool ShouldSerializeIntervalType()
        {
            return IntervalType != DOM.IntervalType.Auto;
        }

        public void ResetIntervalType()
        {
            this.IntervalType = DOM.IntervalType.Auto;
        }

        public bool ShouldSerializeArrows()
        {
            return Arrows != DOM.Arrows.None;
        }

        public void ResetArrows()
        {
            this.Arrows = DOM.Arrows.None;
        }

        public bool ShouldSerializeMarksAlwaysAtPlotEdge()
        {
            return MarksAlwaysAtPlotEdge != false;
        }

        public void ResetMarksAlwaysAtPlotEdge()
        {
            this.MarksAlwaysAtPlotEdge = false;
        }

        public bool ShouldSerializePreventFontShrink()
        {
            return PreventFontShrink != false;
        }

        public void ResetPreventFontShrink()
        {
            this.PreventFontShrink = false;
        }

        public bool ShouldSerializePreventFontGrow()
        {
            return PreventFontGrow != false;
        }

        public void ResetPreventFontGrow()
        {
            this.PreventFontGrow = false;
        }

        public bool ShouldSerializePreventLabelOffset()
        {
            return PreventLabelOffset != false;
        }

        public void ResetPreventLabelOffset()
        {
            this.PreventLabelOffset = false;
        }

        public bool ShouldSerializePreventWordWrap()
        {
            return PreventWordWrap != false;
        }

        public void ResetPreventWordWrap()
        {
            this.PreventWordWrap = false;
        }

        public bool ShouldSerializeIncludeZero()
        {
            return IncludeZero != false;
        }

        public void ResetIncludeZero()
        {
            this.IncludeZero = false;
        }

        public bool ShouldSerializeLabelsAutoFitDisabled()
        {
            return LabelsAutoFitDisabled != false;
        }

        public void ResetLabelsAutoFitDisabled()
        {
            this.LabelsAutoFitDisabled = false;
        }

        public bool ShouldSerializeVariableAutoInterval()
        {
            return VariableAutoInterval != false;
        }

        public void ResetVariableAutoInterval()
        {
            this.VariableAutoInterval = false;
        }

        public bool ShouldSerializeHideEndLabels()
        {
            return HideEndLabels != false;
        }

        public void ResetHideEndLabels()
        {
            this.HideEndLabels = false;
        }

        public bool ShouldSerializeReverse()
        {
            return Reverse != false;
        }

        public void ResetReverse()
        {
            this.Reverse = false;
        }

        public bool ShouldSerializeInterlaced()
        {
            return Interlaced != false;
        }

        public void ResetInterlaced()
        {
            this.Interlaced = false;
        }

        public bool ShouldSerializeScalar()
        {
            return Scalar != false;
        }

        public void ResetScalar()
        {
            this.Scalar = false;
        }

        public bool ShouldSerializeLogScale()
        {
            return LogScale != false;
        }

        public void ResetLogScale()
        {
            this.LogScale = false;
        }

        public bool ShouldSerializeHideLabels()
        {
            return HideLabels != false;
        }

        public void ResetHideLabels()
        {
            this.HideLabels = false;
        }

        public bool ShouldSerializeLabelIntervalOffset()
        {
            return LabelIntervalOffset != 0;
        }

        public void ResetLabelIntervalOffset()
        {
            this.LabelIntervalOffset = 0;
        }

        public bool ShouldSerializeIntervalOffset()
        {
            return IntervalOffset != 0;
        }

        public void ResetIntervalOffset()
        {
            this.IntervalOffset = 0;
        }

        public bool ShouldSerializeLabelInterval()
        {
            return LabelInterval != 0;
        }

        public void ResetLabelInterval()
        {
            this.LabelInterval = 0;
        }

        public bool ShouldSerializeAngle()
        {
            return Angle != 0;
        }

        public void ResetAngle()
        {
            this.Angle = 0;
        }
    }
}
