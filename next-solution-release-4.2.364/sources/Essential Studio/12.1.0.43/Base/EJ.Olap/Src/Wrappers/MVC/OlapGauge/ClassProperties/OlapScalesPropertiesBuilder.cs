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
using Syncfusion.JavaScript.DataVisualization.Models;
using Syncfusion.JavaScript.DataVisualization;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapScalesPropertiesBuilder
    {
        //public Scales scales;
        OlapGauge olapGauge;
        private List<CircularScales> scales = new List<CircularScales>();
        CircularScales scale = new CircularScales();
        public OlapScalesPropertiesBuilder(OlapGauge scale)
        {
            this.olapGauge = scale;
            this.scales = olapGauge.OlapGaugeModel.Scales;
        }
        public OlapScalesPropertiesBuilder()
        {
        }
        //Boolean values
        public OlapScalesPropertiesBuilder ShowScaleBar()
        {
            scale.ShowScaleBar = true;
            return this;
        }
        public OlapScalesPropertiesBuilder ShowScaleBar(bool showScaleBar)
        {
            scale.ShowScaleBar = showScaleBar;
            return this;
        }
        public OlapScalesPropertiesBuilder LabelAutoAngle()
        {
            scale.LabelAutoAngle = false;
            return this;
        }
        public OlapScalesPropertiesBuilder LabelAutoAngel(bool labelAutoAngle)
        {
            scale.LabelAutoAngle = labelAutoAngle;
            return this;
        }
        public OlapScalesPropertiesBuilder ShowPointers()
        {
            scale.ShowPointers = true;
            return this;
        }
        public OlapScalesPropertiesBuilder ShowPointers(bool showPointers)
        {
            scale.ShowPointers = showPointers;
            return this;
        }
        public OlapScalesPropertiesBuilder ShowRanges()
        {
            scale.ShowRanges = true;
            return this;
        }
        public OlapScalesPropertiesBuilder ShowRanges(bool showRanges)
        {
            scale.ShowRanges = showRanges;
            return this;
        }
        public OlapScalesPropertiesBuilder ShowLabels()
        {
            scale.ShowLabels = true;
            return this;
        }
        public OlapScalesPropertiesBuilder ShowLabels(bool showLabels)
        {
            scale.ShowLabels = showLabels;
            return this;
        }
        public OlapScalesPropertiesBuilder ShowTicks()
        {
            scale.ShowTicks = true;
            return this;
        }
        public OlapScalesPropertiesBuilder ShowTicks(bool showTicks)
        {
            scale.ShowTicks = showTicks;
            return this;
        }
        public OlapScalesPropertiesBuilder ShowIndicators()
        {
            scale.ShowIndicators = false;
            return this;
        }
        public OlapScalesPropertiesBuilder ShowIndicators(bool showIndicators)
        {
            scale.ShowIndicators = showIndicators;
            return this;
        }       
        //EnumValues
        public OlapScalesPropertiesBuilder ScaleDirection(ScaleDirections scaleDirection)
        {
            scale.ScaleDirection = scaleDirection;
            return this;
        }       
        //String Values
        public OlapScalesPropertiesBuilder BackgroundColor(String backgroundColor)
        {
            scale.BackgroundColor = backgroundColor;
            return this;
        }
        public OlapScalesPropertiesBuilder BorderColor(String borderColor)
        {
            scale.BorderColor = borderColor;
            return this;
        }
        public OlapScalesPropertiesBuilder CapBackgroundColor(String capbackgroundColor)
        {
            scale.CapBackgroundColor = capbackgroundColor;
            return this;
        }
        public OlapScalesPropertiesBuilder CapBorderColor(String capborderColor)
        {
            scale.CapBorderColor = capborderColor;
            return this;
        }
        //Integers
        public OlapScalesPropertiesBuilder ScaleBarSize(int scaleBarSize)
        {
            scale.ScaleBarSize = scaleBarSize;
            return this;
        }        
        public OlapScalesPropertiesBuilder SweepAngle(int sweepAngle)
        {
            scale.SweepAngle = sweepAngle;
            return this;
        }
        public OlapScalesPropertiesBuilder ScaleRadius(int scaleRadius)
        {
            scale.ScaleRadius = scaleRadius;
            return this;
        }        
        public OlapScalesPropertiesBuilder StartAngle(int startAngle)
        {
            scale.StartAngle = startAngle;
            return this;
        }
        public OlapScalesPropertiesBuilder MajorIntervalValue(double majorIntervalValue)
        {
            scale.MajorIntervalValue = majorIntervalValue;
            return this;
        }
        public OlapScalesPropertiesBuilder MinorIntervalValue(double minorIntervalValue)
        {
            scale.MinorIntervalValue = minorIntervalValue;
            return this;
        }
        public OlapScalesPropertiesBuilder Maximum(int maximum)
        {
            scale.Maximum = maximum;
            return this;
        }
        public OlapScalesPropertiesBuilder Minimum(int minimum)
        {
            scale.Minimum = minimum;
            return this;
        }
        public OlapScalesPropertiesBuilder PointerCapBorderWidth(int pointerCapBorderWidth)
        {
            scale.PointerCapBorderWidth = pointerCapBorderWidth;
            return this;
        }
        public OlapScalesPropertiesBuilder PointerCapRadius(int pointerCapRadius)
        {
            scale.PointerCapRadius = pointerCapRadius;
            return this;
        }
        //Double Values        
        public OlapScalesPropertiesBuilder ScaleBorderWidth(double scaleBorderWidth)
        {
            scale.ScaleBorderWidth = scaleBorderWidth;
            return this;
        }        
        // Ticks Values
        public OlapScalesPropertiesBuilder Ticks(Action<CircularTicksBuilder> tick)
        {
            var scaleTicks = new List<CircularTicks>();
            scale.Ticks = scaleTicks;
            var builder = new CircularTicksBuilder(this.scale);
            if (tick != null)
                tick.Invoke(builder);
            return this;
        }
        // Ranges Values
        public OlapScalesPropertiesBuilder Ranges(Action<CircularRangesBuilder> ranges)
        {
            var scaleRanges = new List<CircularRanges>();
            scale.Ranges = scaleRanges;
            var builder = new CircularRangesBuilder(this.scale);
            if (ranges != null)
                ranges.Invoke(builder);
            return this;
        }
        // Labels Values
        public OlapScalesPropertiesBuilder Labels(Action<CircularLabelsBuilder> labels)
        {
            var scaleLabels = new List<CircularLabels>();
            scale.Labels = scaleLabels;
            var builder = new CircularLabelsBuilder(this.scale);
            if (labels != null)
                labels.Invoke(builder);
            return this;
        }       
        // Indicators Values
        public OlapScalesPropertiesBuilder Indicators(Action<CircularIndicatorBuilder> indicators)
        {
            var scaleIndicators = new List<CircularIndicators>();
            scale.Indicators = scaleIndicators;
            var builder = new CircularIndicatorBuilder(this.scale);
            if (indicators != null)
                indicators.Invoke(builder);
            return this;
        }
        //Pointer values
        public OlapScalesPropertiesBuilder Pointers(Action<PointersBuilder> pointer)
        {
            var pointers = new List<Pointers>();
            scale.Pointers = pointers;
            var builder = new PointersBuilder(this.scale);
            if (pointer != null)
                pointer.Invoke(builder);
            return this;
        }
        //Subgauge values
        public OlapScalesPropertiesBuilder SubGauge(Action<SubGaugeBuilder> subgauge)
        {
            var subGauge = new List<SubGauge>();
            scale.SubGauge = subGauge;
            var builder = new SubGaugeBuilder(this.scale);
            if (subgauge != null)
                subgauge.Invoke(builder);
            return this;
        }
        //capInteriorGradient values
        public OlapScalesPropertiesBuilder CapInteriorGradient(Action<CapInteriorGradientBuilder> capinteriorgradient)
        {
            var capInteriorGradient = new List<CapInteriorGradient>();
            scale.CapInteriorGradient = capInteriorGradient;
            var builder = new CapInteriorGradientBuilder(this.scale);
            if (capinteriorgradient != null)
                capinteriorgradient.Invoke(builder);
            return this;
        }
        // CustomLabel Values
        public OlapScalesPropertiesBuilder CustomLabel(Action<CircularCustomLabelBuilder> customLabel)
        {
            var customLabels = new List<CircularCustomLabel>();
            scale.CustomLabel = customLabels;
            var builder = new CircularCustomLabelBuilder(this.scale);
            if (customLabel != null)
                customLabel.Invoke(builder);
            return this;
        }
        public void Add()
        {            
            olapGauge.OlapGaugeModel.Scales.Add(scale);                          
            scale = new CircularScales();
        }
    }
}
