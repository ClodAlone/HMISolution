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
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.DataVisualization.Models;


namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class CircularStateRanges
    {
        #region Fields

        //Interger Values
        private int endValue = 15;
        private int startValue = 15;
        
        //String values
        private string backgroundColor = null;
        private string borderColor = null;
        private string textColor = null;
        private string text = "";

        //Object values
        private CircularFont font = new CircularFont();       

        #endregion

        #region Properties

        //Integer values
        [JsonProperty("endValue")]
        [DefaultValue(15)]
        public int EndValue
        {
            get { return this.endValue; }
            set { this.endValue = value; }
        }
        [JsonProperty("startValue")]
        [DefaultValue(15)]
        public int StartValue
        {
            get { return this.startValue; }
            set { this.startValue = value; }
        }
        //String values
        [JsonProperty("borderColor")]
        [DefaultValue(null)]
        public String BorderColor
        {
            get { return this.borderColor; }
            set { this.borderColor = value; }
        }
        [JsonProperty("backgroundColor")]
        [DefaultValue(null)]
        public String BackgroundColor
        {
            get { return this.backgroundColor; }
            set { this.backgroundColor = value; }
        }
        [JsonProperty("textColor")]
        [DefaultValue(null)]
        public String TextColor
        {
            get { return this.textColor; }
            set { this.textColor = value; }
        }
        [JsonProperty("text")]
        [DefaultValue("")]
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
        //Object values
        [JsonProperty("font")]
        public CircularFont Font
        {
            get { return this.font; }
            set { this.font = value; }
        }
        #endregion

        #region ShouldSerialize Methods
        public bool ShouldSerializeFont()
        {
            if (Utils.PropertyCompare(Font, new CircularFont()))
                return true;
            else
                return false;
        }
        #endregion

    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
    public class StateRangesBuilder
    {
        CircularIndicators indicator;
        private List<CircularStateRanges> stateRange = new List<CircularStateRanges>();                                           
        CircularStateRanges stateRanges = new CircularStateRanges();
        public StateRangesBuilder(CircularIndicators stateRanges)
        {
            this.indicator=stateRanges;
            this.stateRange = stateRanges.StateRanges;
        }
        //String Values
        public StateRangesBuilder BackgroundColor(String backgroundColor)
        {
            stateRanges.BackgroundColor = backgroundColor;            
            return this;
        }
        public StateRangesBuilder BorderColor(String BorderColor)
        {
            stateRanges.BorderColor = BorderColor;
            return this;
        }
        public StateRangesBuilder TextColor(String textColor)
        {
            stateRanges.TextColor = textColor;
            return this;
        }
        public StateRangesBuilder Text(String text)
        {
            stateRanges.Text = text;
            return this;
        }
        //Integers
        public StateRangesBuilder EndValue(int endValue)
        {
            stateRanges.EndValue = endValue;
            return this;
        }
        public StateRangesBuilder StartValue(int startValue)
        {
            stateRanges.StartValue = startValue;
            return this;
        }
        // Font Values
        public StateRangesBuilder Font(Action<CircularFontBuilder> font)
        {            
            var builder = new CircularFontBuilder(this.stateRanges);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
        public void Add()
        {
            indicator.StateRanges.Add(stateRanges);
            stateRanges = new CircularStateRanges();          
        }
    }
}
