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
using Syncfusion.JavaScript.Shared.Serializer;
using System.Drawing;

using Syncfusion.JavaScript.Shared;


namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class ChartModelProperties
    {
    }

    public class LayoutBorder
    {
        #region Fields

        private string m_color = null;
        private double m_Width = -1;
        private double m_Opacity = -1;

        #endregion

        #region Properties
        [JsonProperty("color")]
        [DefaultValue(null)]
        public string Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        [JsonProperty("width")]
        [DefaultValue(-1)]
        public double Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }
        [JsonProperty("opacity")]
        [DefaultValue(-1)]
        public double Opacity
        {
            get { return m_Opacity; }
            set { m_Opacity = value; }

        }
        
        #endregion
    }
      
    public class ChartArea
    {
        #region Fields

       
        private string m_fill = null;
        private object m_Border= null;
        #endregion

        #region Properties





        [JsonProperty("background")]
        [DefaultValue(null)]
        public string Background
        {
            get { return m_fill; }
            set { m_fill = value; }

        }
        [JsonProperty("border")]
        public object Border
        {
            get { return this.m_Border; }
            set { this.m_Border = value; }
        }
        #endregion
        #region ShouldSerialize Methods

        public bool ShouldSerializeBorder()
        {
            if (Utils.PropertyCompare(Border, new LayoutBorder()))
                return true;
            else
                return false;
        }
        #endregion

    }

    public class CommonSeriesOptions
    {
        #region Fields

        private SeriesType m_type = SeriesType.Line;
        private bool m_animation= true;
		private double m_doughnutCoefficient= 0.4;
		private double	m_explodeOffset = 25;
		private	PyramidMode m_pyramidMode = PyramidMode.Linear;
        private ChartLabelPosition m_labelPosition=ChartLabelPosition.Inside;
	    private double m_gapRatio = 0;
		private double m_pieCoefficient = 0.8;
        private double m_doughnutSize = 0.8;
       
        private object m_tooltip = new NewTooltip();
       
        private object m_marker = null;
        private object m_font = null;
        private object m_dataSource = new ChartDataSource();

        //Integer values
        private int m_ExplodeIndex = -1;
        private int m_StartAngle = 0;
        private bool m_Explode = false;
        private bool m_ExplodeAll = false;

        private SeriesDrawMode m_DrawMode = SeriesDrawMode.Both;
        private string m_bearFillColor = null;
        private string m_bullFillColor = null;
        private double m_opacity = -1;
        private string m_fill = null;
        private object m_border = null;
        private double m_width = 1;
        private string m_dashArray = null;
        private ChartLineCap m_LineCap = ChartLineCap.Butt;
        private ChartLineJoin m_LineJoin = ChartLineJoin.Round;

        #endregion
        
        #region Properties


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
        [JsonProperty("width")]
        [DefaultValue(1)]
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


        [JsonProperty("lineCap")]
        [DefaultValue(ChartLineCap.Butt)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ChartLineCap LineCap
        {
            get
            {
                return this.m_LineCap;
            }
            set
            {
                this.m_LineCap = value;
            }
        }

        [JsonProperty("lineJoin")]
        [DefaultValue(ChartLineJoin.Round)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ChartLineJoin LineJoin
        {
            get { return this.m_LineJoin; }
            set { this.m_LineJoin = value; }

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
        [JsonProperty("border")]
        public object Border
        {
            get { return this.m_border; }
            set { this.m_border = value; }
        }

        [JsonProperty("type")]
        [DefaultValue(SeriesType.Line)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SeriesType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        [JsonProperty("bearFillColor")]
        [DefaultValue(null)]
        public string BearFillColor
        {
            get { return this.m_bearFillColor; }
            set { this.m_bearFillColor = value; }
        }

        [JsonProperty("bullFillColor")]
        [DefaultValue(null)]
        public string BullFillColor
        {
            get { return this.m_bullFillColor; }
            set { this.m_bullFillColor = value; }
        }

        [JsonProperty("drawMode")]
        [DefaultValue(SeriesDrawMode.Both)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SeriesDrawMode DrawMode
        {
            get { return this.m_DrawMode; }
            set { this.m_DrawMode = value; }
        }

        [JsonProperty("animation")]
        [DefaultValue(true)]
        public bool Animation
        {
            get { return m_animation; }
            set { m_animation = value; }
        }

        [JsonProperty("doughnutCoefficient")]
        [DefaultValue(0.4)]
        public double DoughnutCoefficient
        {
            get { return m_doughnutCoefficient; }
            set { m_doughnutCoefficient = value; }
        }

        [JsonProperty("doughnutSize")]
        [DefaultValue(0.8)]
        public double DoughnutSize
        {
            get { return m_doughnutSize; }
            set { m_doughnutSize = value; }
        }

        [JsonProperty("pieCoefficient")]
        [DefaultValue(0.8)]
        public double PieCoefficient
        {
            get { return m_pieCoefficient; }
            set { m_pieCoefficient = value; }
        }

        [JsonProperty("ExplodeOffset")]
        [DefaultValue(25)]
        public double ExplodeOffset
        {
            get { return m_explodeOffset; }
            set { m_explodeOffset = value; }
        }

        [JsonProperty("gapRatio")]
        [DefaultValue(0)]
        public double GapRatio
        {
            get { return m_gapRatio; }
            set { m_gapRatio = value; }
        }

        [JsonProperty("labelPosition")]
        [DefaultValue(ChartLabelPosition.Inside)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ChartLabelPosition LabelPosition
        {
            get { return this.m_labelPosition; }
            set { this.m_labelPosition = value; }
        }

        [JsonProperty("pyramidMode")]
        [DefaultValue(PyramidMode.Linear)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PyramidMode PyramidMode
        {
            get { return this.m_pyramidMode; }
            set { this.m_pyramidMode = value; }
        }
        [JsonProperty("explodeIndex")]
        [DefaultValue(-1)]
        public int ExplodeIndex
        {
            get { return m_ExplodeIndex; }
            set { this.m_ExplodeIndex = value; }
        }

        [JsonProperty("startAngle")]
        [DefaultValue(0)]
        public int StartAngle
        {
            get { return m_StartAngle; }
            set { this.m_StartAngle = value; }
        }

        [JsonProperty("explodeAll")]
        [DefaultValue(false)]
        public bool ExplodeAll
        {
            get { return m_ExplodeAll; }
            set { this.m_ExplodeAll = value; }
        }
        [JsonProperty("explode")]
        [DefaultValue(false)]
        public bool Explode
        {
            get { return m_Explode; }
            set { this.m_Explode = value; }
        }

        [JsonProperty("tooltip")]
        public object Tooltip
        {
            get { return this.m_tooltip; }
            set { this.m_tooltip = value; }
        }
        [JsonProperty("dataSource")]
        public object DataSource
        {
            get { return this.m_dataSource; }
            set { this.m_dataSource = value; }
        }

        [JsonProperty("marker")]
        public object Marker
        {
            get { return this.m_marker; }
            set { this.m_marker = value; }
        }

        [JsonProperty("font")]
        public object Font
        {
            get { return this.m_font; }
            set { this.m_font = value; }
        }
 
        #endregion

       #region ShouldSerialize Methods

        public bool ShouldSerializeTooltip()
        {
            if (Utils.PropertyCompare(Tooltip, new NewTooltip()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeDataSource()
        {
            if (Utils.PropertyCompare(DataSource, new ChartDataSource()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeMarker()
        {
            if (Utils.PropertyCompare(Marker, new Marker()))
                return true;
            else
                return false;
        }
         public bool ShouldSerializeFont()
        {
            if (Utils.PropertyCompare(Font, new ChartFont()))
                return true;
            else
                return false;
        }
   
         public bool ShouldSerializeBorder()
         {
             if (Utils.PropertyCompare(Border, new ChartBorder()))
                 return true;
             else
                 return false;
         }

        #endregion

    }

    public class Zooming {
     
       #region Fields

        private bool m_Enable = false;
        private string m_Type="x,y";
        private bool m_EnableMouseWheel = false;

       #endregion

       #region Properties

        [JsonProperty("enable")]
        [DefaultValue(false)]
        public bool Enable
        {
            get { return m_Enable; }
            set { m_Enable  = value; }
        }

        [JsonProperty("enableMouseWheel")]
        [DefaultValue(false)]
        public bool EnableMouseWheel
        {
            get { return m_EnableMouseWheel; }
            set { m_EnableMouseWheel  = value; }
        }

        [JsonProperty("type")]
        [DefaultValue("x,y")]
        public string Type
        {
            get { return m_Type; }
            set { m_Type  = value; }
        }

     #endregion

    }

    public class CrossHair {

         #region Fields

        private object m_Marker = null;
        private CrosshairType m_moveToArea = CrosshairType.Crosshair;
         private bool m_visible = false;
        private object m_Line;
         #endregion

         #region Properties

         [JsonProperty("marker")]
         public object Marker
         {
            get { return this.m_Marker; }
            set { this.m_Marker = value; }
         }
         [JsonProperty("line")]
         public object Line
         {
             get { return this.m_Line; }
             set { this.m_Line = value; }
         }
         [JsonProperty("type")]
         [DefaultValue(CrosshairType.Crosshair)]
         [JsonConverter(typeof(StringEnumConverter))]
         public CrosshairType Type
         {
            get { return this.m_moveToArea; }
            set { this.m_moveToArea = value; }
         }
         [JsonProperty("visible")]
         [DefaultValue(false)]
         public bool Visible
         {
             get { return this.m_visible; }
             set { this.m_visible = value; }
         }
         #endregion

         #region ShouldSerialize Methods

         public bool ShouldSerializeMarker()
         {
             if (Utils.PropertyCompare(Marker, new Marker()))
                 return true;
             else
                 return false;
         }
         public bool ShouldSerializeLine()
         {
             if (Utils.PropertyCompare(Line, new Line()))
                 return true;
             else
                 return false;
         }
          #endregion

    }
     public class Line
     {
         #region Fields

         private string color =null;
         private double width = 1;
          
         #endregion

         #region Properties

         [JsonProperty("width")]
         [DefaultValue(1)]
         public double Width
         {
             get { return width; }
             set { width = value; }

         }
         [JsonProperty("color")]
         [DefaultValue(null)]
         public string Color
         {
             get { return color; }
             set { color = value; }

         }

         #endregion
     }
    public class Margin
    {

        #region Fields

        private double m_left = -1;
        private double m_right= -1;
        private double m_top = -1;
        private double m_bottom = -1;

        #endregion

        #region Properties

        [JsonProperty("left")]
        [DefaultValue(-1)]
        public double Left
        {
            get { return m_left; }
            set { m_left = value; }

        }

        [JsonProperty("right")]
        [DefaultValue(-1)]
        public double Right
        {
            get { return m_right; }
            set { m_right = value; }

        }
        [JsonProperty("top")]
        [DefaultValue(-1)]
        public double Top
        {
            get { return m_top; }
            set { m_top = value; }

        }
        [JsonProperty("bottom")]
        [DefaultValue(-1)]
        public double Bottom
        {
            get { return m_bottom; }
            set { m_bottom = value; }

        }
        #endregion

       

    }

    public class RowDefinitions
    {
        #region Feilds

        private string m_lineColor=null;
        private double m_rowHeight=0;
        private double m_lineWidth=0;
        private string m_unit=null;

        #endregion


        #region Properties

        [JsonProperty("rowHeight")]
        [DefaultValue(0)]
        public double RowHeight
        {
            get { return m_rowHeight; }
            set { m_rowHeight = value; }
        }

        [JsonProperty("lineWidth")]
        [DefaultValue(0)]
        public double LineWidth
        {
            get { return m_lineWidth; }
            set { m_lineWidth = value; }
        }

        [JsonProperty("lineColor")]
        [DefaultValue(null)]
        public string LineColor
        {
            get { return m_lineColor; }
            set { m_lineColor = value; }
        }

        [JsonProperty("unit")]
        [DefaultValue(null)]
        public string Unit
        {
            get { return m_unit; }
            set { m_unit = value; }
        }

        #endregion
    }

    public class ColumnDefinitions
    {
        #region Field
        
        private double m_columnWidth = 0;
        private string m_unit = null;

        #endregion

        #region Properties

        [JsonProperty("columnWidth")]
        [DefaultValue(0)]
        public double ColumnWidth
        {
            get { return m_columnWidth; }
            set { m_columnWidth = value; }
        }

        [JsonProperty("unit")]
        [DefaultValue(null)]
        public string Unit
        {
            get { return m_unit; }
            set { m_unit = value; }
        }

        #endregion
    }
}
