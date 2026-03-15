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
    public class CircularIndicators
    {
        #region Fields
       
        //Interger Values
        private int indicatorHeight = 15;
        private int indicatorWidth = 15;
        private int indicatorvalue = 0;

        //object values       
        private IndicatorLocation indicatorLocation = new IndicatorLocation();
        private List<CircularStateRanges> stateRanges = new List<CircularStateRanges>();
               
        //Enumeration Values
        private IndicatorStyles indicatorStyle = IndicatorStyles.Circle;

        #endregion

        #region Properties
        
        //Integer values
        [JsonProperty("indicatorHeight")]
        [DefaultValue(15)]
        public int IndicatorHeight
        {
            get { return this.indicatorHeight; }
            set { this.indicatorHeight = value; }
        }
        [JsonProperty("indicatorWidth")]
        [DefaultValue(15)]
        public int IndicatorWidth
        {
            get { return this.indicatorWidth; }
            set { this.indicatorWidth = value; }
        }
        [JsonProperty("indicatorvalue")]
        [DefaultValue(0)]
        public int Indicatorvalue
        {
            get { return this.indicatorvalue; }
            set { this.indicatorvalue = value; }
        }        
        //Enumeration Values
        [JsonProperty("indicatorStyle")]
        [DefaultValue(IndicatorStyles.Circle)]
        [JsonConverter(typeof(StringEnumConverter))]
        public IndicatorStyles IndicatorStyle
        {
            get { return this.indicatorStyle; }
            set { this.indicatorStyle = value; }
        }
        //Object values
        [JsonProperty("indicatorLocation")]
        public IndicatorLocation IndicatorLocation
        {
            get { return this.indicatorLocation; }
            set { this.indicatorLocation = value; }
        }
        [JsonProperty("stateRanges")]
        public List<CircularStateRanges> StateRanges
        {
            get { return this.stateRanges; }
            set { this.stateRanges = value; }
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeIndicatorLocation()
        {
            if (Utils.PropertyCompare(IndicatorLocation, new IndicatorLocation()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeStateRanges()
        {
            if (StateRanges.Count != 0)
                return true;
            else
                return false;
        }        
        #endregion
    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
    public class CircularIndicatorBuilder
    {
        CircularScales scales;
        private List<CircularIndicators> indicator = new List<CircularIndicators>();
        CircularIndicators indicators = new CircularIndicators();
        public CircularIndicatorBuilder(CircularScales indicators)
        {
            this.scales=indicators;
            this.indicator = indicators.Indicators;
        }
        
        //Integers
        public CircularIndicatorBuilder IndicatorHeight(int indicatorHeight)
        {
            indicators.IndicatorHeight = indicatorHeight;
            //this.indicators.IndicatorHeight = indicatorHeight;
            return this;
        }
        public CircularIndicatorBuilder IndicatorWidth(int indicatorWidth)
        {
            indicators.IndicatorWidth = indicatorWidth;
            return this;
        }
        public CircularIndicatorBuilder IndicatorValue(int indicatorvalue)
        {
            indicators.Indicatorvalue = indicatorvalue;
            return this;
        }        
        //EnumValues
        public CircularIndicatorBuilder IndicatorStyle(IndicatorStyles indicatorStyle)
        {
            indicators.IndicatorStyle = indicatorStyle;
            return this;
        }
        // IndicatorLocation Values
        public CircularIndicatorBuilder IndicatorLocation(Action<IndicatorLocationBuilder> Indicatorlocation)
        {            
            var builder = new IndicatorLocationBuilder(this.indicators);
            if (Indicatorlocation != null)
                Indicatorlocation.Invoke(builder);
            return this;
        }
        // IndicatorFont Values
        public CircularIndicatorBuilder StateRanges(Action<StateRangesBuilder> Stateranges)
        {
            var stateRanges = new List<CircularStateRanges>();
            indicators.StateRanges = stateRanges;
            var builder = new StateRangesBuilder(this.indicators);
            if (Stateranges != null)
                Stateranges.Invoke(builder);
            return this;
        }
        public void Add()
        {
            scales.Indicators.Add(indicators);
            indicators = new CircularIndicators();            
        }
    }
}
