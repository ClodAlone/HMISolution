#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
using Syncfusion.JavaScript.Shared.Serializer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class ShapeSetting
    {
        #region Fields

        private string mouseHoverColor = "gray";
        private double mouseHoverWidth = 1;
        private string shapeSelectionColor = "gray";
        private string shapeFill = "#E5E5E5";
        private double shapeStrokeThickness = 0.2;
        private double shapeSelectionStrokeWidth = 1;
        private string shapeStroke = "#C1C1C1";
	    private string shapeSelectionStroke = "#C1C1C1";
	    private string mouseHoverStroke = "#C1C1C1";
        private string shapeColorValuepath = null;
        private string shapeValuepath = null;
        private ColorMapping colorMappings = null;
        private bool autoFill = false;
        private bool enableGradient = false;
        private ColorPalette shapeColorPalette = ColorPalette.Palette1;
        private List<string> customPalette = new List<string>();

        #endregion

        #region Properties

        [JsonProperty("mouseHoverColor")]
        [DefaultValue("gray")]
        public string MouseHoverColor
        {
            get { return this.mouseHoverColor; }
            set { this.mouseHoverColor = value; }
        }

        [JsonProperty("shapeSelectionColor")]
        [DefaultValue("gray")]
        public string ShapeSelectionColor
        {
            get { return this.shapeSelectionColor; }
            set { this.shapeSelectionColor = value; }
        }

        [JsonProperty("shapeColorValuepath")]
        [DefaultValue(null)]
        public string ShapeColorValuepath
        {
            get { return this.shapeColorValuepath; }
            set { this.shapeColorValuepath = value; }
        }


        [JsonProperty("shapeValuepath")]
        [DefaultValue(null)]
        public string ShapeValuepath
        {
            get { return this.shapeValuepath; }
            set { this.shapeValuepath = value; }
        }

        [JsonProperty("shapeFill")]
        [DefaultValue("#E5E5E5")]
        public string ShapeFill
        {
            get { return this.shapeFill; }
            set { this.shapeFill = value; }
        }

        [JsonProperty("shapeStroke")]
        [DefaultValue("#C1C1C1")]
        public string ShapeStroke
        {
            get { return this.shapeStroke; }
            set { this.shapeStroke = value; }
        }

        [JsonProperty("shapeSelectionStroke")]
        [DefaultValue("#C1C1C1")]
        public string ShapeSelectionStroke
        {
            get { return this.shapeSelectionStroke; }
            set { this.shapeSelectionStroke = value; }
        }

        [JsonProperty("mouseHoverStroke")]
        [DefaultValue("#C1C1C1")]
        public string MouseHoverStroke
        {
            get { return this.mouseHoverStroke; }
            set { this.mouseHoverStroke = value; }
        }

        [JsonProperty("mouseHoverWidth")]
        [DefaultValue(1)]
        public double MouseHoverWidth
        {
            get { return this.mouseHoverWidth; }
            set { this.mouseHoverWidth = value; }
        }

        [JsonProperty("shapeStrokeThickness")]
        [DefaultValue(0.2)]
        public double ShapeStrokeThickness
        {
            get { return this.shapeStrokeThickness; }
            set { this.shapeStrokeThickness = value; }
        }

        [JsonProperty("shapeSelectionStrokeWidth")]
        [DefaultValue(1)]
        public double ShapeSelectionStrokeWidth
        {
            get { return this.shapeSelectionStrokeWidth; }
            set { this.shapeSelectionStrokeWidth = value; }
        }

        [JsonProperty("colorMappings")]
        public ColorMapping ColorMappings
        {
            get { return this.colorMappings; }
            set { this.colorMappings = value; }
        }

        [JsonProperty("autoFill")]
        [DefaultValue(false)]
        public bool AutoFill
        {
            get { return this.autoFill; }
            set { this.autoFill = value; }
        }

        [JsonProperty("enableGradient")]
        [DefaultValue(false)]
        public bool EnableGradient
        {
            get { return this.enableGradient; }
            set { this.enableGradient = value; }
        }

        [JsonProperty("shapeColorPalette")]
        [DefaultValue(ColorPalette.Palette1)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ColorPalette ShapeColorPalette
        {
            get { return this.shapeColorPalette; }
            set { this.shapeColorPalette = value; }
        }

        [JsonProperty("customPalette")]
        public List<string> CustomPalette
        {
            get { return this.customPalette; }
            set { this.customPalette = value; }
        }

        #endregion

    }

    public class ColorMapping : INotifyPropertyChanged
    {
        public List<object> colorMappings;

        [JsonProperty("equalColorMapping")]
        public List<object> EqualColorMap { get; set; }

        [JsonProperty("rangeColorMapping")]
        public List<object> RangeColorMap { get; set; }


        public ColorMapping()
        {
            this.colorMappings = new List<object>();
            this.EqualColorMap = new List<object>();
            this.RangeColorMap = new List<object>();
            this.PropertyChanged += new PropertyChangedEventHandler(ColorMapping_PropertyChanged);
        }

        void ColorMapping_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Type type = colorMappings.First().GetType();
            if (type == typeof (EqualColorMapping))
            {
                if (this.EqualColorMap.Count == 0)
                {
                    this.EqualColorMap = colorMappings;                    
                }
            }
            else if (type == typeof (RangeColorMapping))
            {
                if (this.RangeColorMap.Count == 0)
                {
                    this.RangeColorMap = colorMappings;                                   
                }
            }
        }

        [JsonIgnore]
        public List<object> ColorMappings
        {
            get
            {
                if (this.colorMappings != null && this.colorMappings.Count > 0)
                {
                    NotifyPropertyChanged("changed");
                }
                return this.colorMappings;
            }
            set
            {
                this.colorMappings = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
