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
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class ScalesPropertiesBuilder
    {
        //public scales scales;
        LinearGauge linearGauge;
        private List<Scales> scale = new List<Scales>();
        Scales scales = new Scales();
        public ScalesPropertiesBuilder(LinearGauge scales)
        {
            this.linearGauge = scales;
            this.scale = scales.LinearGaugeModel.Scales;
        }
        public ScalesPropertiesBuilder()
        {
        }
        //Boolean values
        public ScalesPropertiesBuilder ShowRanges()
        {
            scales.ShowRanges = false;
            return this;
        }
        public ScalesPropertiesBuilder ShowRanges(bool showRanges)
        {
            scales.ShowRanges = showRanges;
            return this;
        }
        public ScalesPropertiesBuilder ShowIndicators()
        {
            scales.ShowIndicators = false;
            return this;
        }
        public ScalesPropertiesBuilder ShowIndicators(bool showIndicators)
        {
            scales.ShowIndicators = showIndicators;
            return this;
        }
        public ScalesPropertiesBuilder ShowCustomLabel()
        {
            scales.ShowCustomLabel = false;
            return this;
        }
        public ScalesPropertiesBuilder ShowCustomLabel(bool showCustomLabel)
        {
            scales.ShowCustomLabel = showCustomLabel;
            return this;
        }
        public ScalesPropertiesBuilder ShowLabels()
        {
            scales.ShowLabels = true;
            return this;
        }
        public ScalesPropertiesBuilder ShowLabels(bool showLabels)
        {
            scales.ShowLabels = showLabels;
            return this;
        }
        public ScalesPropertiesBuilder ShowTicks()
        {
            scales.ShowTicks = true;
            return this;
        }
        public ScalesPropertiesBuilder ShowTicks(bool showTicks)
        {
            scales.ShowTicks = showTicks;
            return this;
        }
        public ScalesPropertiesBuilder ShowBarPointers()
        {
            scales.ShowBarPointers = true;
            return this;
        }
        public ScalesPropertiesBuilder ShowBarPointers(bool showBarPointers)
        {
            scales.ShowBarPointers = showBarPointers;
            return this;
        }
        public ScalesPropertiesBuilder ShowMarkerPointers()
        {
            scales.ShowMarkerPointers = true;
            return this;
        }
        public ScalesPropertiesBuilder ShowMarkerPointers(bool showMarkerPointers)
        {
            scales.ShowMarkerPointers = showMarkerPointers;
            return this;
        }
        //EnumValues
        public ScalesPropertiesBuilder ScaleDirection(ScaleDirections scaleDirection)
        {
            scales.ScaleDirection = scaleDirection;
            return this;
        }
        public ScalesPropertiesBuilder ScaleStyle(ScaleStyles scaleStyle)
        {
            scales.ScaleStyle = scaleStyle;
            return this;
        }
        //String Values
        public ScalesPropertiesBuilder BackgroundColor(String backgroundColor)
        {
            scales.BackgroundColor = backgroundColor;
            return this;
        }
        public ScalesPropertiesBuilder BorderColor(String borderColor)
        {
            scales.ScaleBorderColor = borderColor;
            return this;
        }
        //Integers
        public ScalesPropertiesBuilder Minimum(int minimum)
        {
            scales.Minimum = minimum;
            return this;
        }
        public ScalesPropertiesBuilder Maximum(int maximum)
        {
            scales.Maximum = maximum;
            return this;
        }
        public ScalesPropertiesBuilder MajorIntervalValue(int majorIntervalValue)
        {
            scales.MajorIntervalValue = majorIntervalValue;
            return this;
        }
        public ScalesPropertiesBuilder MinorIntervalValue(int minorIntervalValue)
        {
            scales.MinorIntervalValue = minorIntervalValue;
            return this;
        }
        public ScalesPropertiesBuilder ScaleBarSize(int scaleBarSize)
        {
            scales.ScaleBarSize = scaleBarSize;
            return this;
        }
        public ScalesPropertiesBuilder ShadowOffset(int shadowOffset)
        {
            scales.ShadowOffset = shadowOffset;
            return this;
        }
        public ScalesPropertiesBuilder ScaleBarLength(int scaleBarLength)
        {
            scales.ScaleBarLength = scaleBarLength;
            return this;
        }
        //Double Values
        public ScalesPropertiesBuilder BorderWidth(double borderWidth)
        {
            scales.BorderWidth = borderWidth;
            return this;
        }
        public ScalesPropertiesBuilder Opacity(double opacity)
        {
            scales.ScaleOpacity = opacity;
            return this;
        }
        // Location Values
        public ScalesPropertiesBuilder ScaleLocation(Action<LinearLocationBuilder> location)
        {
            
            var builder = new LinearLocationBuilder(this.scales);
            if (location != null)
                location.Invoke(builder);
            return this;
        }
        // Ticks Values
        public ScalesPropertiesBuilder Ticks(Action<LinearTicksBuilder> ticks)
        {
            var scaleTicks = new List<LinearTicks>();
            scales.Ticks = scaleTicks;
            var builder = new LinearTicksBuilder(this.scales);
            if (ticks != null)
                ticks.Invoke(builder);
            return this;
        }
        // Ranges Values
        public ScalesPropertiesBuilder Ranges(Action<RangesBuilder> ranges)
        {
            var scaleRanges = new List<Ranges>();
            scales.Ranges = scaleRanges;
            var builder = new RangesBuilder(this.scales);
            if (ranges != null)
                ranges.Invoke(builder);
            return this;
        }
        // Labels Values
        public ScalesPropertiesBuilder Labels(Action<LabelsBuilder> labels)
        {
            var scaleLabels = new List<Labels>();
            scales.Labels = scaleLabels;
            var builder = new LabelsBuilder(this.scales);
            if (labels != null)
                labels.Invoke(builder);
            return this;
        }
        // Marker Pointers Values
        public ScalesPropertiesBuilder MarkerPointers(Action<MarkerPointerBuilder> markerPointers)
        {
            var scaleMarkerPointers = new List<MarkerPointers>();
            scales.MarkerPointers = scaleMarkerPointers;
            var builder = new MarkerPointerBuilder(this.scales);
            if (markerPointers != null)
                markerPointers.Invoke(builder);
            return this;
        }
        // Bar Pointers Values
        public ScalesPropertiesBuilder BarPointers(Action<BarPointerBuilder> barPointers)
        {
            var scaleBarPointers = new List<BarPointers>();
            scales.BarPointers = scaleBarPointers;
            var builder = new BarPointerBuilder(this.scales);
            if (barPointers != null)
                barPointers.Invoke(builder);
            return this;
        }
        // Indicators Values
        public ScalesPropertiesBuilder Indicators(Action<IndicatorBuilder> indicators)
        {
            var scaleIndicators = new List<Indicators>();
            scales.Indicators = scaleIndicators;
            var builder = new IndicatorBuilder(this.scales);
            if (indicators != null)
                indicators.Invoke(builder);
            return this;
        }
        // CustomLabel Values
        public ScalesPropertiesBuilder CustomLabel(Action<CustomLabelBuilder> customLabel)
        {
            var customLabels = new List<CustomLabel>();
            scales.CustomLabel = customLabels;
            var builder = new CustomLabelBuilder(this.scales);
            if (customLabel != null)
                customLabel.Invoke(builder);
            return this;
        }
        public void Add()
        {
            linearGauge.LinearGaugeModel.Scales.Add(scales);
            scales = new Scales();
        }
    }
}
