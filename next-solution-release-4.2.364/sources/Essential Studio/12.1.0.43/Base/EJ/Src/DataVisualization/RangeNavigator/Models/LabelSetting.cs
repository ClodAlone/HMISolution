#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript;
using System.ComponentModel;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
   public class LabelSetting
   {
       # region Field

       private object m_higherLevel = null;
       private object m_lowerLevel = null;
       private object m_labelStyles = null;
       #endregion

       # region Property

       [JsonProperty("higherLevel")]
       public object HigherLevel
       {
           get
           {

               return this.m_higherLevel;
           }
           set
           {
               this.m_higherLevel = value;
           }
       }
       [JsonProperty("lowerLevel")]
       public object LowerLevel
       {
           get
           {

               return this.m_lowerLevel;
           }
           set
           {
               this.m_lowerLevel = value;
           }
       }
       [JsonProperty("labelstyles")]
       public object LabelStyles
       {
           get
           {

               return this.m_labelStyles;
           }
           set
           {
               this.m_labelStyles = value;
           }
       }
       #endregion

       # region ShouldSerialize
       public bool ShouldSerializeHigherLevel()
       {
           if (Utils.PropertyCompare(HigherLevel, new HigherLabelSettingLevel()))
               return true;
           else
               return false;
       }
       public bool ShouldSerializeLowerLevel()
       {
           if (Utils.PropertyCompare(LowerLevel, new LowerLabelSettingLevel()))
               return true;
           else
               return false;
       }
       public bool ShouldSerializeLabelStyles()
       {
           if (Utils.PropertyCompare(LabelStyles, new LabelStyles()))
               return true;
           else
               return false;
       }
       #endregion

   }
    
    public class HigherLabelSettingLevel
    {
        # region Field
        private NavigatorIntervalType m_intervalType=NavigatorIntervalType.Years;
        private object m_labelStyles = null;
        private object m_gridLineStyle = null; 
        private object m_border = null;
        private NavigatorPosition m_position=NavigatorPosition.Top;
        private bool m_visible=true;
        private string m_labelPlacement=null;
        #endregion


        # region Property
        [JsonProperty("position")]
        [DefaultValue(NavigatorPosition.Top)]
        [JsonConverter(typeof(StringEnumConverter))]
        public NavigatorPosition Position
        {
            get { return this.m_position; }
            set { this.m_position = value; }
        }

        [JsonProperty("labelPlacement")]
        [DefaultValue(null)]
        public string LabelPlacement
        {
            get { return this.m_labelPlacement; }
            set { this.m_labelPlacement = value; }
        }

        [JsonProperty("intervalType")]
        [DefaultValue(NavigatorIntervalType.Years)]
        [JsonConverter(typeof(StringEnumConverter))]
        public NavigatorIntervalType IntervalType
        {
            get { return this.m_intervalType; }
            set { this.m_intervalType = value; }
        }
        [JsonProperty("visible")]
        [DefaultValue(true)]
        public bool Visible
        {
            get { return this.m_visible; }
            set { this.m_visible = value; }
        }
        [JsonProperty("labelstyles")]
        public object LabelStyles
        {
            get { return this.m_labelStyles; }
            set { this.m_labelStyles = value; }
        }
        [JsonProperty("border")]
        public object Border
        {
            get { return this.m_border; }
            set { this.m_border = value; }
        }
        [JsonProperty("gridLineStyle")]
        public object GridLineStyle
        {
            get { return this.m_gridLineStyle; }
            set { this.m_gridLineStyle = value; }
        }
        #endregion

        #region ShouldSerialize

        public bool ShouldSerializeLabelStyles()
        {
            if (Utils.PropertyCompare(LabelStyles, new LabelStyles()))
               return true;
            else
                return false;
        }
        public bool ShouldSerializeBorder()
        {
            if (Utils.PropertyCompare(Border, new LineStyle()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeGridLineStyle()
        {
            if (Utils.PropertyCompare(GridLineStyle, new LineStyle()))
                return true;
            else
                return false;
        }

        #endregion

    }

    public class LowerLabelSettingLevel
    {
        # region Field
        private NavigatorIntervalType m_intervalType = NavigatorIntervalType.Quarters;
        private object m_labelStyles = null;
        private object m_gridLineStyle = null;
        private object m_border = null;
        private NavigatorPosition m_position = NavigatorPosition.Bottom;
        private bool m_visible = true;
        private string m_labelPlacement = null;
        #endregion


        # region Property
        [JsonProperty("position")]
        [DefaultValue(NavigatorPosition.Bottom)]
        [JsonConverter(typeof(StringEnumConverter))]
        public NavigatorPosition Position
        {
            get { return this.m_position; }
            set { this.m_position = value; }
        }

        [JsonProperty("labelPlacement")]
        [DefaultValue(null)]
        public string LabelPlacement
        {
            get { return this.m_labelPlacement; }
            set { this.m_labelPlacement = value; }
        }

        [JsonProperty("intervalType")]
        [DefaultValue(NavigatorIntervalType.Quarters)]
        [JsonConverter(typeof(StringEnumConverter))]
        public NavigatorIntervalType IntervalType
        {
            get { return this.m_intervalType; }
            set { this.m_intervalType = value; }
        }
        [JsonProperty("visible")]
        [DefaultValue(true)]
        public bool Visible
        {
            get { return this.m_visible; }
            set { this.m_visible = value; }
        }
        [JsonProperty("labelstyles")]
        public object LabelStyles
        {
            get { return this.m_labelStyles; }
            set { this.m_labelStyles = value; }
        }
        [JsonProperty("border")]
        public object Border
        {
            get { return this.m_border; }
            set { this.m_border = value; }
        }
        [JsonProperty("gridLineStyle")]
        public object GridLineStyle
        {
            get { return this.m_gridLineStyle; }
            set { this.m_gridLineStyle = value; }
        }
        #endregion

        #region ShouldSerialize

        public bool ShouldSerializeLabelStyles()
        {
            if (Utils.PropertyCompare(LabelStyles, new LabelStyles()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeBorder()
        {
            if (Utils.PropertyCompare(Border, new LineStyle()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeGridLineStyle()
        {
            if (Utils.PropertyCompare(GridLineStyle, new LineStyle()))
                return true;
            else
                return false;
        }

        #endregion

    }

    public class LabelStyles
    {
        #region Field

        private object m_font = null;
        private HorizontalAlignment m_horizontalAlignment = HorizontalAlignment.Center;
        #endregion

      
        #region Property

        [JsonProperty("font")]
        public object Font
        {
            get { return this.m_font; }
            set { this.m_font = value; }
        }

        [JsonProperty("horizontalAlignment")]
        [DefaultValue(HorizontalAlignment.Center)]
        [JsonConverter(typeof(StringEnumConverter))]
        public HorizontalAlignment HorizontalAlignment
        {
            get { return this.m_horizontalAlignment; }
            set { this.m_horizontalAlignment = value; }
        }
        #endregion

        #region ShouldSerialize
        public bool ShouldSerializeFont()
        {
            if (Utils.PropertyCompare(Font, new NavigatorFont()))
                return true;
            else
                return false;
        }

        #endregion

        public static LabelStyles Major { get; set; }
    }

    public class LineStyle
    {
        # region Field
        private string m_color=null;
        private string m_daharray = null;
        private double m_width=-1;
        #endregion

       # region Property
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
        [JsonProperty("strokedasharray")]
        [DefaultValue(null)]
        public string StrokeDashArray
        {
            get { return this.m_daharray; }
            set { this.m_daharray = value; }
        }
        
        #endregion
    }
    public class NavigatorGridLines
    {
        #region Field

        private double m_width = -1;
        private bool m_visible = false;
        private string m_dasharray = null;
        private string m_color = null;
        private double m_opacity = -1;
        private int m_offset = 0;
        #endregion

        #region Property
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
        /// gets or sets offset for axisLine
        /// </summary>
        [JsonProperty("offset")]
        [DefaultValue(0)]
        public int Offset
        {
            get
            {
                return this.m_offset;
            }
            set
            {
                this.m_offset = value;
            }
        }
        /// <summary>
        /// gets or sets DashArray
        /// </summary>
        [JsonProperty("dashArray")]
        [DefaultValue(null)]
        public string DashArray
        {
            get
            {
                return this.m_dasharray;
            }
            set
            {
                this.m_dasharray = value;
            }
        }
        /// <summary>
        /// gets or sets Width
        /// </summary>
        [JsonProperty("width")]
        [DefaultValue(-1)]
        public double Width
        {
            get
            {
                return this.m_width;
            }
            set
            {
                this.m_width = value;
            }
        }
        /// <summary>
        /// gets or sets Visible
        /// </summary>

        [JsonProperty("visible")]
        [DefaultValue(false)]
        public bool Visible
        {
            get
            {
                return this.m_visible;
            }
            set
            {
                this.m_visible = value;
            }
        }
        /// <summary>
        /// gets or sets Width
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

        #endregion

    }
    public class ValueAxisSettings
    {
        # region Field

        private object m_font = null;
        private object range = null;
        private string m_rangePadding = null;
        private bool visible = false;
        private object majorTickLines = null;
        private object majorGridLines = null;
        private object axisLine = null;
        #endregion

        # region Property
        [JsonProperty("rangePadding")]
        [DefaultValue(null)]
        public string RangePadding
        {
            get { return this.m_rangePadding; }
            set { this.m_rangePadding = value; }
        }
        [JsonProperty("visible")]
        [DefaultValue(false)]
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }
        [JsonProperty("font")]
        public object Font
        {
            get { return this.m_font; }
            set { this.m_font = value; }
        }
        [JsonProperty("range")]
        public object Range
        {
            get { return this.range; }
            set { this.range = value; }
        }
        [JsonProperty("axisLine")]
        public object AxisLine
        {
            get
            {

                return this.axisLine;
            }
            set
            {
                this.axisLine = value;
            }
        }
        [JsonProperty("majorGridLines")]
        public object MajorGridLines
        {
            get
            {

                return this.majorGridLines;
            }
            set
            {
                this.majorGridLines = value;
            }
        }
        [JsonProperty("majorTickLines")]
        public object MajorTickLines
        {
            get
            {

                return this.majorTickLines;
            }
            set
            {
                this.majorTickLines = value;
            }
        }
        #endregion

        #region ShouldSerialize
        public bool ShouldSerializeFont()
        {
            if (Utils.PropertyCompare(Font, new NavigatorFont()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeAxisLine()
        {
            if (Utils.PropertyCompare(AxisLine, new NavigatorGridLines()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeMajorGridLines()
        {
            if (Utils.PropertyCompare(MajorGridLines, new NavigatorGridLines()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeMajorTickLines()
        {
            if (Utils.PropertyCompare(MajorTickLines, new MajorTicks()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeRange()
        {
            if (Utils.PropertyCompare(Range, new Range()))
                return true;
            else
                return false;
        }

        #endregion
    }
}
