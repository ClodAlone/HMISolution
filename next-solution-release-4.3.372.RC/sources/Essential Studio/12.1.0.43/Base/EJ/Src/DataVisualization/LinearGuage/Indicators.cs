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


namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class Indicators
    {
        #region Fields
        //String Values
        private String pointerGradient = null;
        private String backgroundColor = null;
        private String borderColor = null;
        //Interger Values
        private int indicatorHeight = 30;
        private int indicatorWidth = 30;
        private int value = 0;
        //object values
        private Font font = new Font();
        private LinearLocation location = new LinearLocation();
        private TextLocation textLocation = new TextLocation();
        private List<StateRanges> stateRanges = new List<StateRanges>();
        //Double Values
        private double borderWidth = 1.5;
        private double opacity = 1;
        //Enumeration Values
        private IndicatorStyles indicatorStyle = IndicatorStyles.Rectangle;
        #endregion

        #region Properties

        //String Values
        [JsonProperty("pointerGradient")]
        [DefaultValue(null)]
        public String IndicatorPointerGradient
        {
            get { return this.pointerGradient; }
            set { this.pointerGradient = value; }
        }
        [JsonProperty("backgroundColor")]
        [DefaultValue(null)]
        public String IndicatorBackgroundColor
        {
            get { return this.backgroundColor; }
            set { this.backgroundColor = value; }
        }
        [JsonProperty("borderColor")]
        [DefaultValue(null)]
        public String IndicatorBorderColor
        {
            get { return this.borderColor; }
            set { this.borderColor = value; }
        }
        //Integer values
        [JsonProperty("indicatorHeight")]
        [DefaultValue(30)]
        public int IndicatorHeight
        {
            get { return this.indicatorHeight; }
            set { this.indicatorHeight = value; }
        }
        [JsonProperty("indicatorWidth")]
        [DefaultValue(30)]
        public int IndicatorWidth
        {
            get { return this.indicatorWidth; }
            set { this.indicatorWidth = value; }
        }
        [JsonProperty("value")]
        [DefaultValue(0)]
        public int Indicatorvalue
        {
            get { return this.value; }
            set { this.value = value; }
        }
        //Double Values
        [JsonProperty("borderWidth")]
        [DefaultValue(1.5)]
        public double IndicatorBorderWidth
        {
            get { return this.borderWidth; }
            set { this.borderWidth = value; }
        }
        [JsonProperty("opacity")]
        [DefaultValue(1)]
        public double IndicatorOpacity
        {
            get { return this.opacity; }
            set { this.opacity = value; }
        }
        //Enumeration Values
        [JsonProperty("indicatorStyle")]
        [DefaultValue(IndicatorStyles.Rectangle)]
        [JsonConverter(typeof(StringEnumConverter))]
        public IndicatorStyles IndicatorStyle
        {
            get { return this.indicatorStyle; }
            set { this.indicatorStyle = value; }
        }
        //Object values
        [JsonProperty("location")]
        public LinearLocation Location
        {
            get { return this.location; }
            set { this.location = value; }
        }
        [JsonProperty("font")]
        public Font Font
        {
            get { return this.font; }
            set { this.font = value; }
        }
        [JsonProperty("textLocation")]
        public TextLocation TextLocation
        {
            get { return this.textLocation; }
            set { this.textLocation = value; }
        }
        [JsonProperty("stateRanges")]
        public List<StateRanges> StateRange
        {
            get { return this.stateRanges; }
            set { this.stateRanges = value; }
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeLocation()
        {
            if (Utils.PropertyCompare(Location, new LinearLocation()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeTextLocation()
        {
            if (Utils.PropertyCompare(TextLocation, new TextLocation()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeFont()
        {
            if (Utils.PropertyCompare(Font, new Font()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeStateRanges()
        {
            if (StateRange.Count!=0)
                return true;
            else
                return false;
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
    public class IndicatorBuilder
    {
        private List<Indicators> indicators = new List<Indicators>();
        Indicators indicator = new Indicators();
        Scales scales;
        public IndicatorBuilder(Scales indicator)
        {
            this.scales = indicator;
            this.indicators = indicator.Indicators;
        }
        //String Values
        public IndicatorBuilder IndicatorPointerGradient(String pointerGradient)
        {
            indicator.IndicatorPointerGradient = pointerGradient;
            return this;
        }
        public IndicatorBuilder IndicatorBackgroundColor(String backgroundColor)
        {
            indicator.IndicatorBackgroundColor = backgroundColor;
            return this;
        }
        public IndicatorBuilder IndicatorBorderColor(String borderColor)
        {
            indicator.IndicatorBorderColor = borderColor;
            return this;
        }
        //Integers
        public IndicatorBuilder IndicatorHeight(int indicatorHeight)
        {
            indicator.IndicatorHeight = indicatorHeight;
            return this;
        }
        public IndicatorBuilder IndicatorWidth(int indicatorWidth)
        {
            indicator.IndicatorWidth = indicatorWidth;
            return this;
        }
        public IndicatorBuilder IndicatorValue(int value)
        {
            indicator.Indicatorvalue = value;
            return this;
        }
        //Double Values
        public IndicatorBuilder IndicatorBorderWidth(double borderWidth)
        {
            indicator.IndicatorBorderWidth = borderWidth;
            return this;
        }
        public IndicatorBuilder IndicatorOpacity(double opacity)
        {
            indicator.IndicatorOpacity = opacity;
            return this;
        }
        //EnumValues
        public IndicatorBuilder IndicatorStyle(IndicatorStyles indicatorStyle)
        {
            indicator.IndicatorStyle = indicatorStyle;
            return this;
        }
        // IndicatorLocation Values
        public IndicatorBuilder IndicatorLocation(Action<LinearLocationBuilder> Indicatorlocation)
        {
            var indicatorLocations = new LinearLocation();
            indicator.Location = indicatorLocations;
            var builder = new LinearLocationBuilder(this.indicator);
            if (Indicatorlocation != null)
                Indicatorlocation.Invoke(builder);
            return this;
        }
        // IndicatorFont Values
        public IndicatorBuilder IndicatorFont(Action<FontBuilder> Indicatorfont)
        {
            var indicatorfonts = new Font();
            indicator.Font = indicatorfonts;
            var builder = new FontBuilder(this.indicator);
            if (Indicatorfont != null)
                Indicatorfont.Invoke(builder);
            return this;
        }
        // IndicatorTextLocation Values
        public IndicatorBuilder IndicatorTextLocation(Action<TextLocationBuilder> indicatorTextLocation)
        {
            var indicatorTextLocations = new TextLocation();
            indicator.TextLocation = indicatorTextLocations;
            var builder = new TextLocationBuilder(this.indicator);
            if (indicatorTextLocation != null)
                indicatorTextLocation.Invoke(builder);
            return this;
        }
        // IndicatorStateRange Values
        public IndicatorBuilder IndicatorStateRange(Action<StateRangeBuilder> indicatorStateRange)
        {
            var indicatorStateRanges = new List<StateRanges>();
            indicator.StateRange = indicatorStateRanges;
            var builder = new StateRangeBuilder(this.indicator);
            if (indicatorStateRange != null)
                indicatorStateRange.Invoke(builder);
            return this;
        }
        public void Add()
        {
            scales.Indicators.Add(indicator);
            indicator = new Indicators();
        }
    }
}
