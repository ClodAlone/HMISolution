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
    public class StateRanges
    {
        #region Fields
        //String Values
        private String backgroundColor = null;
        private String borderColor = null;
        private String textColor = null;
        private String text = "";
        //Interger Values
        private int endValue = 60;
        private int startValue = 50;
        #endregion

        #region Properties

        //String Values
        [JsonProperty("backgroundColor")]
        [DefaultValue(null)]
        public String StateRangeBackgroundColor
        {
            get { return this.backgroundColor; }
            set { this.backgroundColor = value; }
        }
        [JsonProperty("borderColor")]
        [DefaultValue(null)]
        public String StateRangeBorderColor
        {
            get { return this.borderColor; }
            set { this.borderColor = value; }
        }
        [JsonProperty("textColor")]
        [DefaultValue(null)]
        public String StateRangetextColor
        {
            get { return this.textColor; }
            set { this.textColor = value; }
        }
        [JsonProperty("text")]
        [DefaultValue("")]
        public String StateRangetext
        {
            get { return this.text; }
            set { this.text = value; }
        }
        //Integer values
        [JsonProperty("endValue")]
        [DefaultValue(60)]
        public int StateRangeEndValue
        {
            get { return this.endValue; }
            set { this.endValue = value; }
        }
        [JsonProperty("startValue")]
        [DefaultValue(50)]
        public int StateRangeStartValue
        {
            get { return this.startValue; }
            set { this.startValue = value; }
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
    public class StateRangeBuilder
    {
        private List<StateRanges> stateRanges = new List<StateRanges>();
        StateRanges stateRange = new StateRanges();
        Indicators indicator;
        public StateRangeBuilder(Indicators stateRange)
        {
            this.indicator = stateRange;
            this.stateRanges = stateRange.StateRange;
        }
        //String Values
        public StateRangeBuilder StateRangeBackgroundColor(String backgroundColor)
        {
            this.stateRange.StateRangeBackgroundColor = backgroundColor;
            return this;
        }
        public StateRangeBuilder StateRangeBorderColor(String borderColor)
        {
            this.stateRange.StateRangeBorderColor = borderColor;
            return this;
        }
        public StateRangeBuilder StateRangetextColor(String textColor)
        {
            this.stateRange.StateRangetextColor = textColor;
            return this;
        }
        public StateRangeBuilder StateRangetext(String text)
        {
            this.stateRange.StateRangetext = text;
            return this;
        }
        //Integers
        public StateRangeBuilder StateRangeEndValue(int endValue)
        {
            this.stateRange.StateRangeEndValue = endValue;
            return this;
        }
        public StateRangeBuilder StateRangeStartValue(int startValue)
        {
            this.stateRange.StateRangeStartValue = startValue;
            return this;
        }
        public void Add()
        {
            indicator.StateRange.Add(stateRange);
            stateRange = new StateRanges();
        }
    }
}
