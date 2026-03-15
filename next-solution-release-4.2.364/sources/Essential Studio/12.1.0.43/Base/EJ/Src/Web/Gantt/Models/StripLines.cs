#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Shared;



namespace Syncfusion.JavaScript.Models
{
    public class StripLines
    {
        private String day = null;
        private String label = null;
        private String lineStyle =null;
        private String lineColor = null;
        private int lineWidth = 0;

        [JsonProperty("day")]
        [DefaultValue(null)]
        public String Day
        {
            get { return this.day; }
            set { this.day = value; }
        }
        [JsonProperty("label")]
        [DefaultValue(null)]
        public String Label
        {
            get { return this.label; }
            set { this.label = value; }
        }
        [JsonProperty("lineStyle")]
        [DefaultValue("dotted")]
        public String LineStyle
        {
            get { return this.lineStyle; }
            set { this.lineStyle = value; }
        }
        [JsonProperty("lineColor")]
        [DefaultValue("Darkblue")]
        public String LineColor
        {
            get { return this.lineColor; }
            set { this.lineColor = value; }
        }
        [JsonProperty("lineWidth")]
        [DefaultValue(2)]
        public int LineWidth
        {
            get { return this.lineWidth; }
            set { this.lineWidth = value; }
        }
    }
}