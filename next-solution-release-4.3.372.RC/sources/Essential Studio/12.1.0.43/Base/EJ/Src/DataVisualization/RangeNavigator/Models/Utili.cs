#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.Shared.Serializer;


namespace Syncfusion.JavaScript.DataVisualization.Models
{
    class Utili
    {
    }
    public class NavigatorFont
    {
        # region Field
        private string m_size =null ;
        private string m_fontfamily=null  ;
        private RangeNavigatorFontWeight m_fontweight=RangeNavigatorFontWeight.Regular ;
        private RangeNavigatorFontStyle m_fontStyle = RangeNavigatorFontStyle.Normal;
        private string m_color=null ;
        private double m_opacity=-1 ;
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
        [DefaultValue(RangeNavigatorFontStyle.Normal)]
        [JsonConverter(typeof(StringEnumConverter))]
        public RangeNavigatorFontStyle FontStyle
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
        [DefaultValue(null)]
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
        [DefaultValue(RangeNavigatorFontWeight.Regular)]
        [JsonConverter(typeof(StringEnumConverter))]
        public RangeNavigatorFontWeight FontWeight
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
    public class NavigatorSize
    {

        #region Fields

        private string m_Width="0";
        private string m_Height="0";

        #endregion

       

        #region Properties

        [JsonProperty("width")]
        [DefaultValue("0")]
        public string Width
        {
            get { return this.m_Width; }
            set { this.m_Width = value; }
        }

        [JsonProperty("height")]
        [DefaultValue("0")]
        public string Height
        {
            get { return this.m_Height; }
            set { this.m_Height = value; }
        }

        #endregion
    }

}
