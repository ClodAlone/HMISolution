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
    public class CustomLabel
    {
        #region Fields
        //String Values
        private String labelColor = null;
        private String labelValue = "";
        //Interger Values
        private int textAngle = 0;
        //Double Values
        private double opacity = 0;
        //object values
        private Font font = new Font();
        private LinearLocation location = new LinearLocation();
        #endregion

        #region Properties

        //String Values
        [JsonProperty("labelColor")]
        [DefaultValue(null)]
        public String LabelColor
        {
            get { return this.labelColor; }
            set { this.labelColor = value; }
        }
        [JsonProperty("labelValue")]
        [DefaultValue("")]
        public String LabelValue
        {
            get { return this.labelValue; }
            set { this.labelValue = value; }
        }
        //Integer values
        [JsonProperty("textAngle")]
        [DefaultValue(0)]
        public int TextAngle
        {
            get { return this.textAngle; }
            set { this.textAngle = value; }
        }
        // Double Values
        [JsonProperty("opacity")]
        [DefaultValue(0)]
        public double CustomLabelopacity
        {
            get { return this.opacity; }
            set { this.opacity = value; }
        }
        //object values
        [JsonProperty("font")]
        public Font Font
        {
            get { return this.font; }
            set { this.font = value; }
        }

        [JsonProperty("location")]
        public LinearLocation Location
        {
            get { return this.location; }
            set { this.location = value; }
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
        public bool ShouldSerializeFont()
        {
            if (Utils.PropertyCompare(Font, new Font()))
                return true;
            else
                return false;
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript.DataVisualization
{

    public class CustomLabelBuilder
    {
        private List<CustomLabel> customLabels = new List<CustomLabel>();
        CustomLabel customLabel = new CustomLabel();
        Scales scales;
        public CustomLabelBuilder(Scales customLabel)
        {
            this.scales = customLabel;
            this.customLabels = customLabel.CustomLabel;
        }
        //String Values
        public CustomLabelBuilder LabelColor(String labelColor)
        {
            customLabel.LabelColor = labelColor;
            return this;
        }
        public CustomLabelBuilder LabelValue(String labelValue)
        {
            customLabel.LabelValue = labelValue;
            return this;
        }
        //Integers
        public CustomLabelBuilder TextAngle(int textAngle)
        {
            customLabel.TextAngle = textAngle;
            return this;
        }
        // Double Values
        public CustomLabelBuilder CustomLabelopacity(double opacity)
        {
            customLabel.CustomLabelopacity = opacity;
            return this;
        }
        // Location
        public CustomLabelBuilder Location(Action<LinearLocationBuilder> location)
        {
            var locations = new LinearLocation();
            customLabel.Location = locations;
            var builder = new LinearLocationBuilder(this.customLabel);
            if (location != null)
                location.Invoke(builder);
            return this;
        }
        // Font
        public CustomLabelBuilder Font(Action<FontBuilder> font)
        {
            var fonts = new Font();
            customLabel.Font = fonts;
            var builder = new FontBuilder(this.customLabel);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
        public void Add()
        {
            scales.CustomLabel.Add(customLabel);
            customLabel = new CustomLabel();
        }
    }
}
