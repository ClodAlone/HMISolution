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
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Shared.Serializer;

using Syncfusion.JavaScript.Shared;
using System.Drawing;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class ChartFont
    {
        # region Field
        private string m_size=null ;
        private string m_fontfamily=null ;
        private ChartFontWeight m_fontweight=ChartFontWeight.Regular ;
        private ChartFontStyle m_fontStyle=ChartFontStyle.Normal;
        private string m_color=null  ;
        private double m_opacity=-1 ;
        #endregion
     
        # region Methods
      
        #endregion
        
        # region Properies
        /// <summary>
        /// gets or sets Color
        /// </summary>
        [JsonProperty("color")]
        [DefaultValue(null)]
        public string Color
        {
            get
            {
                return this.m_color;
            }
            set
            {
                this.m_color = value;
            }
        }
        /// <summary>
        /// gets or sets  FontSize
        /// </summary>
        [JsonProperty("size")]
        [DefaultValue(null)]
        public string FontSize
        {
            get
            {
                return this.m_size;
            }
            set
            {
                this.m_size = value;
            }
        }
        /// <summary>
        /// gets or sets FontStyle
        /// </summary>
        [JsonProperty("fontStyle")]
        [DefaultValue(ChartFontStyle.Normal)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ChartFontStyle FontStyle
        {
            get
            {
                return this.m_fontStyle;
            }
            set
            {
                this.m_fontStyle = value;
            }
        }
        /// <summary>
        /// gets or sets FontFamily
        /// </summary>
         [JsonProperty("fontFamily")]
        [DefaultValue("Segoe UI")]
        public string FontFamily
        {
            get
            {
                return this.m_fontfamily;
            }
            set
            {
                this.m_fontfamily = value;
            }
        }
        /// <summary>
        /// gets or sets Opacity
        /// </summary>
         [JsonProperty("opacity")]
         [DefaultValue(-1)]
        public double Opacity
        {
            get
            {
                return this.m_opacity;
            }
            set
            {
                this.m_opacity = value;
            }
        }
        /// <summary>
        /// gets or sets FontWeight
        /// </summary>
        [JsonProperty("fontWeight")]
        [DefaultValue(ChartFontWeight.Regular)]
        [JsonConverter(typeof(StringEnumConverter))]
         public ChartFontWeight FontWeight
        {
            get
            {
                return this.m_fontweight;
            }
            set
            {
                this.m_fontweight = value;
            }
        }
     #endregion
    }
    public class ChartStyle
    {
        # region Field
        private double m_opacity = -1;
        private double m_borderwidth = -1;
        private string m_bordercolor = null;
        private string m_fill = null;
        private string m_dashArray = null;
        #endregion

        # region Properies
        [JsonProperty("opacity")]
        [DefaultValue(-1)]
        public double Opacity
        {
            get
            {
                return this.m_opacity;
            }
            set
            {
                this.m_opacity = value;
            }
        }
        [JsonProperty("fill")]
        [DefaultValue(null)]
        public string Fill
        {
            get
            {
                return this.m_fill;
            }
            set
            {
                this.m_fill = value;
            }
        }
        [JsonProperty("dashArray")]
        [DefaultValue(null)]
        public string DashArray
        {
            get
            {
                return this.m_dashArray;
            }
            set
            {
                this.m_dashArray = value;
            }
        }
        [JsonProperty("borderColor")]
        [DefaultValue(null)]
        public string BorderColor
        {
            get
            {
                return this.m_bordercolor;
            }
            set
            {
                this.m_bordercolor = value;
            }
        }
        [JsonProperty("borderWidth")]
        [DefaultValue(-1)]
        public double BorderWidth
        {
            get
            {
                return this.m_borderwidth;
            }
            set
            {
                this.m_borderwidth = value;
            }
        }
        #endregion

    }
    public class ChartBorder
    {

        #region Fields

        private string m_color = null;
        private double m_width= -1;

        #endregion

        #region Properties

        [JsonProperty("color")]
        [DefaultValue(null)]
        public string Color
        {
            get { return this.m_color; }
            set { this.m_color = value; }
        }

        [JsonProperty("width")]
        [DefaultValue(-1)]
        public double Width
        {
            get { return this.m_width; }
            set { this.m_width = value; }
        }

        #endregion
    }



}