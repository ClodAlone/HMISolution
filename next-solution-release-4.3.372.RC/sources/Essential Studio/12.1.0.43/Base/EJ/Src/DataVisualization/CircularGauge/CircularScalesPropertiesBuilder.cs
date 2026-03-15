#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.DataVisualization.Models;


namespace Syncfusion.JavaScript.DataVisualization
{
    public class CircularScalesPropertiesBuilder
    {
        //public Scales scales;
        CircularGauge cirGauge;
        private List<CircularScales> scales = new List<CircularScales>();
        CircularScales scale = new CircularScales();
        public CircularScalesPropertiesBuilder(CircularGauge scale)
        {
            this.cirGauge = scale;
            this.scales = cirGauge.CircularGaugeModel.Scales;
        }
        public CircularScalesPropertiesBuilder()
        {
        }
        //Boolean values
        public CircularScalesPropertiesBuilder ShowScaleBar()
        {
            scale.ShowScaleBar = true;
            return this;
        }
        public CircularScalesPropertiesBuilder ShowScaleBar(bool showScaleBar)
        {
            scale.ShowScaleBar = showScaleBar;
            return this;
        }
        public CircularScalesPropertiesBuilder LabelAutoAngle()
        {
            scale.LabelAutoAngle = false;
            return this;
        }
        public CircularScalesPropertiesBuilder LabelAutoAngel(bool labelAutoAngle)
        {
            scale.LabelAutoAngle = labelAutoAngle;
            return this;
        }
        public CircularScalesPropertiesBuilder ShowPointers()
        {
            scale.ShowPointers = true;
            return this;
        }
        public CircularScalesPropertiesBuilder ShowPointers(bool showPointers)
        {
            scale.ShowPointers = showPointers;
            return this;
        }
        public CircularScalesPropertiesBuilder ShowRanges()
        {
            scale.ShowRanges = true;
            return this;
        }
        public CircularScalesPropertiesBuilder ShowRanges(bool showRanges)
        {
            scale.ShowRanges = showRanges;
            return this;
        }
        public CircularScalesPropertiesBuilder ShowLabels()
        {
            scale.ShowLabels = true;
            return this;
        }
        public CircularScalesPropertiesBuilder ShowLabels(bool showLabels)
        {
            scale.ShowLabels = showLabels;
            return this;
        }
        public CircularScalesPropertiesBuilder ShowTicks()
        {
            scale.ShowTicks = true;
            return this;
        }
        public CircularScalesPropertiesBuilder ShowTicks(bool showTicks)
        {
            scale.ShowTicks = showTicks;
            return this;
        }
        public CircularScalesPropertiesBuilder ShowIndicators()
        {
            scale.ShowIndicators = false;
            return this;
        }
        public CircularScalesPropertiesBuilder ShowIndicators(bool showIndicators)
        {
            scale.ShowIndicators = showIndicators;
            return this;
        }       
        //EnumValues
        public CircularScalesPropertiesBuilder ScaleDirection(ScaleDirections scaleDirection)
        {
            scale.ScaleDirection = scaleDirection;
            return this;
        }       
        //String Values
        public CircularScalesPropertiesBuilder BackgroundColor(String backgroundColor)
        {
            scale.BackgroundColor = backgroundColor;
            return this;
        }
        public CircularScalesPropertiesBuilder BorderColor(String borderColor)
        {
            scale.BorderColor = borderColor;
            return this;
        }
        public CircularScalesPropertiesBuilder CapBackgroundColor(String capbackgroundColor)
        {
            scale.CapBackgroundColor = capbackgroundColor;
            return this;
        }
        public CircularScalesPropertiesBuilder CapBorderColor(String capborderColor)
        {
            scale.CapBorderColor = capborderColor;
            return this;
        }
        //Integers
        public CircularScalesPropertiesBuilder ScaleBarSize(int scaleBarSize)
        {
            scale.ScaleBarSize = scaleBarSize;
            return this;
        }        
        public CircularScalesPropertiesBuilder SweepAngle(int sweepAngle)
        {
            scale.SweepAngle = sweepAngle;
            return this;
        }
        public CircularScalesPropertiesBuilder ScaleRadius(int scaleRadius)
        {
            scale.ScaleRadius = scaleRadius;
            return this;
        }        
        public CircularScalesPropertiesBuilder StartAngle(int startAngle)
        {
            scale.StartAngle = startAngle;
            return this;
        }
        public CircularScalesPropertiesBuilder MajorIntervalValue(double majorIntervalValue)
        {
            scale.MajorIntervalValue = majorIntervalValue;
            return this;
        }
        public CircularScalesPropertiesBuilder MinorIntervalValue(double minorIntervalValue)
        {
            scale.MinorIntervalValue = minorIntervalValue;
            return this;
        }
        public CircularScalesPropertiesBuilder Maximum(int maximum)
        {
            scale.Maximum = maximum;
            return this;
        }
        public CircularScalesPropertiesBuilder Minimum(int minimum)
        {
            scale.Minimum = minimum;
            return this;
        }
        public CircularScalesPropertiesBuilder PointerCapBorderWidth(int pointerCapBorderWidth)
        {
            scale.PointerCapBorderWidth = pointerCapBorderWidth;
            return this;
        }
        public CircularScalesPropertiesBuilder PointerCapRadius(int pointerCapRadius)
        {
            scale.PointerCapRadius = pointerCapRadius;
            return this;
        }
        //Double Values        
        public CircularScalesPropertiesBuilder ScaleBorderWidth(double scaleBorderWidth)
        {
            scale.ScaleBorderWidth = scaleBorderWidth;
            return this;
        }        
        // Ticks Values
        public CircularScalesPropertiesBuilder Ticks(Action<CircularTicksBuilder> tick)
        {
            var scaleTicks = new List<CircularTicks>();
            scale.Ticks = scaleTicks;
            var builder = new CircularTicksBuilder(this.scale);
            if (tick != null)
                tick.Invoke(builder);
            return this;
        }
        // Ranges Values
        public CircularScalesPropertiesBuilder Ranges(Action<CircularRangesBuilder> ranges)
        {
            var scaleRanges = new List<CircularRanges>();
            scale.Ranges = scaleRanges;
            var builder = new CircularRangesBuilder(this.scale);
            if (ranges != null)
                ranges.Invoke(builder);
            return this;
        }
        // Labels Values
        public CircularScalesPropertiesBuilder Labels(Action<CircularLabelsBuilder> labels)
        {
            var scaleLabels = new List<CircularLabels>();
            scale.Labels = scaleLabels;
            var builder = new CircularLabelsBuilder(this.scale);
            if (labels != null)
                labels.Invoke(builder);
            return this;
        }       
        // Indicators Values
        public CircularScalesPropertiesBuilder Indicators(Action<CircularIndicatorBuilder> indicators)
        {
            var scaleIndicators = new List<CircularIndicators>();
            scale.Indicators = scaleIndicators;
            var builder = new CircularIndicatorBuilder(this.scale);
            if (indicators != null)
                indicators.Invoke(builder);
            return this;
        }
        //Pointer values
        public CircularScalesPropertiesBuilder Pointers(Action<PointersBuilder> pointer)
        {
            var pointers = new List<Pointers>();
            scale.Pointers = pointers;
            var builder = new PointersBuilder(this.scale);
            if (pointer != null)
                pointer.Invoke(builder);
            return this;
        }
        //Subgauge values
        public CircularScalesPropertiesBuilder SubGauge(Action<SubGaugeBuilder> subgauge)
        {
            var subGauge = new List<SubGauge>();
            scale.SubGauge = subGauge;
            var builder = new SubGaugeBuilder(this.scale);
            if (subgauge != null)
                subgauge.Invoke(builder);
            return this;
        }
        //capInteriorGradient values
        public CircularScalesPropertiesBuilder CapInteriorGradient(Action<CapInteriorGradientBuilder> capinteriorgradient)
        {
            var capInteriorGradient = new List<CapInteriorGradient>();
            scale.CapInteriorGradient = capInteriorGradient;
            var builder = new CapInteriorGradientBuilder(this.scale);
            if (capinteriorgradient != null)
                capinteriorgradient.Invoke(builder);
            return this;
        }
        // CustomLabel Values
        public CircularScalesPropertiesBuilder CustomLabel(Action<CircularCustomLabelBuilder> customLabel)
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
            cirGauge.CircularGaugeModel.Scales.Add(scale);                          
            scale = new CircularScales();
        }
    }
}
