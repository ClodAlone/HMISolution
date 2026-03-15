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
    public class CircularCustomLabel
    {
        
        #region fields
        //Integer values
        private int textAngle = 0;

        //String values
        private string labelValue=null;
        private string labelColor = null;

        //Object values
        private CircularLocation location = new CircularLocation();
        private CircularFont font = new CircularFont();

        #endregion

        #region properties

        //Integer values
        [JsonProperty("textAngle")]
        [DefaultValue(0)]
        public int TextAngle
        {
            get { return this.textAngle; }
            set { this.textAngle = value; }
        }
        //String values
        [JsonProperty("labelValue")]
        [DefaultValue(null)]
        public String LabelValue
        {
            get { return this.labelValue; }
            set { this.labelValue = value; }
        }
        [JsonProperty("labelColor")]
        [DefaultValue(null)]
        public String LabelColor
        {
            get { return this.labelColor; }
            set { this.labelColor = value; }
        }
        //Object values
        [JsonProperty("location")]
        //[DefaultValue(null)]
        public CircularLocation Location
        {
            get { return this.location; }
            set { this.location = value; }
        }
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
        public bool ShouldSerializeLocation()
        {
            if (Utils.PropertyCompare(Location, new CircularLocation()))
                return true;
            else
                return false;
        }
        #endregion

    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
    public class CircularCustomLabelBuilder
    {
        CircularScales scales;
        private List<CircularCustomLabel> customLabels = new List<CircularCustomLabel>();
        CircularCustomLabel customLabel = new CircularCustomLabel();
        public CircularCustomLabelBuilder(CircularScales customLabel)
        {
            this.scales = customLabel;
            this.customLabels = customLabel.CustomLabel;
        }
        //String Values
        public CircularCustomLabelBuilder LabelColor(String labelColor)
        {
            customLabel.LabelColor = labelColor;
            return this;
        }
        public CircularCustomLabelBuilder LabelValue(String labelValue)
        {
            customLabel.LabelValue = labelValue;
            return this;
        }
        //Integers       
        public CircularCustomLabelBuilder TextAngle(int textAngle)
        {
            customLabel.TextAngle = textAngle;
            return this;
        }        
        // Font Values
        public CircularCustomLabelBuilder Font(Action<CircularFontBuilder> font)
        {            
            var builder = new CircularFontBuilder(this.customLabel);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
        // IndicatorLocation Values
        public CircularCustomLabelBuilder Location(Action<CircularLocationBuilder> location)
        {            
            var builder = new CircularLocationBuilder(this.customLabel);
            if (location != null)
                location.Invoke(builder);
            return this;
        }
        public void Add()
        {
            scales.CustomLabel.Add(customLabel);
            customLabel = new CircularCustomLabel();
        }
    }
}
