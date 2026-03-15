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
    public class Series 
    {
        #region Fields

        //Object value
        private List<Points> points = new List<Points>();
        private object marker =null;
        
        private object tooltip = null;
      
        private ConnectorLine connectorline = new ConnectorLine();
        private object m_dataSource = new ChartDataSource();
        private object m_Font = null;
        //enum
        private ChartLabelPosition m_LabelPosition = ChartLabelPosition.Inside;
        private SeriesType m_Type = SeriesType.Line;
        private bool m_visibility = true;
        private PyramidMode m_PyramidMode = PyramidMode.Linear;
        private  SeriesDrawMode m_DrawMode = SeriesDrawMode.Both;
        //Integer values
        private int m_ExplodeIndex = -1;
        private int m_StartAngle = 0;
        //Float values
        private float m_ExplodeOffset = 25f;
        private float m_DoughnutCoefficient = 0.4f;
        private float m_PieCoefficient = 0.8f;
        private float m_DoughnutSize = 0.8f;
        private float m_GapRatio = 0f;
        //Boolean values
        private bool m_Explode=false;
        private bool m_ExplodeAll = false;
        private bool m_Animation = true;
        //String values
        private string m_XAxisName = null;
        private string m_YAxisName = null;
        private string m_Name = null;
        private bool m_smartLabelEnabled = false;
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
        [JsonConverter(typeof (StringEnumConverter))]
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
       
        [JsonProperty("explode")]
        [DefaultValue(false)]
        public bool Explode
        {
            get { return m_Explode; }
            set { this.m_Explode = value; }
        }
        [JsonProperty("smartLabelEnabled")]
        [DefaultValue(false)]
        public bool EnableSmartLabels
        {
            get { return m_smartLabelEnabled; }
            set { this.m_smartLabelEnabled = value; }
        }

         [JsonProperty("bearFillColor")]
        [DefaultValue(null)]
        public string BearFillColor
        {
            get{return this.m_bearFillColor;}
            set{this.m_bearFillColor = value;}
        }

        [JsonProperty("bullFillColor")]
        [DefaultValue(null)]
        public string BullFillColor
        {
            get{return this.m_bullFillColor;}
            set{this.m_bullFillColor = value;}
        }

        [JsonProperty("drawMode")]
        [DefaultValue(SeriesDrawMode.Both)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SeriesDrawMode DrawMode
        {
            get { return this.m_DrawMode; }
            set { this.m_DrawMode = value; }
        }

        [JsonProperty("explodeAll")]
        [DefaultValue(false)]
        public bool ExplodeAll
        {
            get { return m_ExplodeAll; }
            set { this.m_ExplodeAll = value; }
        }

        [JsonProperty("animation")]
        [DefaultValue(true)]
        public bool Animation
        {
            get { return m_Animation; }
            set { this.m_Animation = value; }
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

        [JsonProperty("explodeOffset")]
        [DefaultValue(25)]
        public float ExplodeOffset
        {
            get { return m_ExplodeOffset; }
            set { this.m_ExplodeOffset = value; }
        }

        [JsonProperty("doughnutCoefficient")]
        [DefaultValue(0.4)]
        public float DoughnutCoefficient
        {
            get { return m_DoughnutCoefficient; }
            set { this.m_DoughnutCoefficient = value; }
        }

        [JsonProperty("pieCoefficient")]
        [DefaultValue(0.8)]
        public float PieCoefficient
        {
            get { return m_PieCoefficient; }
            set { this.m_PieCoefficient = value; }
        }

        [JsonProperty("doughnutSize")]
        [DefaultValue(0.8)]
        public float DoughnutSize
        {
            get { return m_DoughnutSize; }
            set { this.m_DoughnutSize = value; }
        }

        [JsonProperty("gapRatio")]
        [DefaultValue(0)]
        public float GapRatio
        {
            get { return m_GapRatio; }
            set { this.m_GapRatio = value; }
        }

        [JsonProperty("labelPosition")]
        [DefaultValue(ChartLabelPosition.Inside)]
        [JsonConverter(typeof (StringEnumConverter))]
        public ChartLabelPosition LabelPosition
        {
            get { return this.m_LabelPosition; }
            set { this.m_LabelPosition = value; }
        }
        [JsonProperty("visibility")]
        [DefaultValue(true)]
        public bool Visibility
        {
            get { return this.m_visibility; }
            set { this.m_visibility = value; }
        }
        [JsonProperty("type")]
        [DefaultValue(SeriesType.Line)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SeriesType Type
        {
            get { return this.m_Type; }
            set { this.m_Type = value; }
        }

        [JsonProperty("pyramidMode")]
        [DefaultValue(PyramidMode.Linear)]
        [JsonConverter(typeof (StringEnumConverter))]
        public PyramidMode PyramidMode
        {
            get { return this.m_PyramidMode; }
            set { this.m_PyramidMode = value; }
        }

        [JsonProperty("xAxisName")]
        [DefaultValue(null)]
        public string XAxisName
        {
            get { return m_XAxisName; }
            set { this.m_XAxisName = value; }
        }

        [JsonProperty("yAxisName")]
        [DefaultValue(null)]
        public string YAxisName
        {
            get { return m_YAxisName; }
            set { this.m_YAxisName = value; }
        }

        [JsonProperty("name")]
        [DefaultValue(null)]
        public string Name
        {
            get { return m_Name; }
            set { this.m_Name = value; }
        }

        [JsonProperty("font")]
        public object Font
        {
            get { return this.m_Font; }
            set { this.m_Font = value; }
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
            get { return this.marker; }
            set { this.marker = value; }
        }
        
      
        [JsonProperty("points")]
        
        public List<Points> Points
        {

            get { return this.points; }
            set{this.points = value;}
        }
        
       
        [JsonProperty("tooltip")]
        public object Tooltip
        {
            get { return this.tooltip; }
            set { this.tooltip = value; }
        }
        #endregion

        #region ShouldSerialize Methods

        public bool ShouldSerializeBorder()
        {
            if (Utils.PropertyCompare(Border, new ChartBorder()))
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

        public bool ShouldSerializeTooltip()
        {
            if (Utils.PropertyCompare(Tooltip, new NewTooltip()))
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
        
        public bool ShouldSerializeDataSource()
        {
            if (Utils.PropertyCompare(DataSource, new ChartDataSource()))
                return true;
            else
                return false;
        }
        
        public bool ShouldSerializePoints()
        {
            if (Points.Count != 0)
                return true;
            else
                return false;
        }

        #endregion
    }

    public class ChartSize
    {

        #region Fields

        private int m_Width=0;
        private int m_Height=0;

        #endregion
       
        #region Properties

        [JsonProperty("width")]
        [DefaultValue(0)]
        public int Width
        {
            get { return this.m_Width; }
            set { this.m_Width = value; }
        }

        [JsonProperty("height")]
        [DefaultValue(0)]
        public int Height
        {
            get { return this.m_Height; }
            set { this.m_Height = value; }
        }

        #endregion
    }

    public class NewTooltip
    {
        #region Fields
        private bool m_visible=false;
        private string m_format = null;
        private string m_templateid = null;
        private bool m_Animation = true;
        private string m_Duration = "500ms";
        private object m_font;
        private object m_border;
        private string m_fill = null;
        private int m_rx = 0;
        private int m_ry = 0;
        #endregion

        #region Properties

        [JsonProperty("fill")]
        [DefaultValue(null)]
        public string Fill
        {
            get { return this.m_fill; }
            set { this.m_fill = value; }
        }

        [JsonProperty("rx")]
        [DefaultValue(0)]
        public int RX
        {
            get{return m_rx;}
            set{m_rx = value;}
        }
        
        [JsonProperty("ry")]
        [DefaultValue(0)]
        public int RY
        {
            get{return m_ry;}
            set{m_ry = value;}
        }

        [JsonProperty("visible")]
        [DefaultValue(false)]
        public bool Visible
        {
            get { return this.m_visible; }
            set { this.m_visible = value; }
        }

        [JsonProperty("font")]
        public object Font
        {
            get { return this.m_font; }
            set { this.m_font = value; }
        }
        [JsonProperty("border")]
        public object Border
        {
            get { return this.m_border; }
            set { this.m_border = value; }
        }
        [JsonProperty("template")]
        [DefaultValue(null)]
        public string Template
        {
            get { return this.m_templateid; }
            set { this.m_templateid = value; }
        }
        [JsonProperty("format")]
        [DefaultValue(null)]
        public string Format
        {
            get { return this.m_format; }
            set { this.m_format = value; }
        }
        [JsonProperty("animation")]
        [DefaultValue(true)]
        public bool Animation
        {
            get { return this.m_Animation; }
            set { this.m_Animation = value; }
        }

        [JsonProperty("duration")]
        [DefaultValue("500ms")]
        public string Duration
        {
            get { return this.m_Duration; }
            set { this.m_Duration = value; }
        }
      #endregion
        #region ShouldSerialize Methods

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

    public class DataLabel
    {

        #region Fields

        private TextPosition m_TextPosition = TextPosition.Top;
        private TextAlignment m_horizontalTextAlignment = TextAlignment.Center;
        private TextAlignment m_verticalTextAlignment = TextAlignment.Center;
        private bool m_visible = false;
        private int m_Offset = 0;
        private string m_TemplateID = null;
        private object m_Font = null;
    
        private ChartShape m_Shape=ChartShape.None;
        private ConnectorType m_connectortype = ConnectorType.Line;
         private object m_connectorLine;
        private object m_margin = null;
        private double m_opacity = -1;
        private object m_Border = null;
        private string m_fill = null;

        #endregion

        #region Properties
         [JsonProperty("visible")]
         [DefaultValue(false)]
         public bool Visible
         {
             get { return this.m_visible; }
             set { this.m_visible = value; }
         }

         [JsonProperty("textPosition")]
         [DefaultValue(TextPosition.Top)]
         [JsonConverter(typeof(StringEnumConverter))]
         public TextPosition TextPosition
         {
             get { return this.m_TextPosition; }
             set { this.m_TextPosition = value; }
         }

         [JsonProperty("horizontalTextAlignment")]
         [DefaultValue(TextAlignment.Center)]
         [JsonConverter(typeof(StringEnumConverter))]
         public TextAlignment HorizontalTextAlignment
         {
             get { return this.m_horizontalTextAlignment; }
             set { this.m_horizontalTextAlignment = value; }
         }
         [JsonProperty("verticalTextAlignment")]
         [DefaultValue(TextAlignment.Center)]
         [JsonConverter(typeof(StringEnumConverter))]
         public TextAlignment VerticalTextAlignment
         {
             get { return this.m_verticalTextAlignment; }
             set { this.m_verticalTextAlignment = value; }
         }
      
        [JsonProperty("offset")]
        [DefaultValue(0)]
        public int Offset
        {
            get { return this.m_Offset; }
            set { this.m_Offset = value; }
        }

        [JsonProperty("template")]
        [DefaultValue(null)]
        public string Template
        {
            get { return this.m_TemplateID; }
            set { this.m_TemplateID = value; }
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
        [JsonProperty("border")]
        public object Border
        {
            get { return this.m_Border; }
            set { this.m_Border = value; }
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

        [JsonProperty("margin")]
        public object Margin
        {
            get
            {

                return this.m_margin;
            }
            set
            {
                this.m_margin = value;
            }
        }
        [JsonProperty("font")]
        public object Font
        {
            get { return this.m_Font; }
            set { this.m_Font = value; }
        }

         
        [JsonProperty("connectorLine")]
        public object ConnectorLine
        {
            get { return this.m_connectorLine; }
            set { this.m_connectorLine = value; }
        }
        [JsonProperty("connectorType")]
        [DefaultValue(ConnectorType.Line)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ConnectorType ConnectorType
        {
            get { return this.m_connectortype; }
            set { this.m_connectortype = value; }
        }
        [JsonProperty("shape")]
        [DefaultValue(ChartShape.None)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ChartShape Shape
        {
            get { return this.m_Shape; }
            set { this.m_Shape = value; }
        }

        #endregion


        #region ShouldSerialize
        public bool ShouldSerializeBorder()
        {
            if (Utils.PropertyCompare(Border, new ChartBorder()))
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
        public bool ShouldSerializeConnectorLine()
        {
            if (Utils.PropertyCompare(ConnectorLine, new ConnectorLine()))
                return true;
            else
                return false;

        }
        public bool ShouldSerializeMargin()
        {
            if (Utils.PropertyCompare(Margin, new Margin()))
                return true;
            else
                return false;
        }

        #endregion
    }

    public class Marker
    {
         #region Fields

        private bool m_Visible=false;
        private double m_opacity = -1;
        private object m_Border = null;
        private ChartShape m_Shape=ChartShape.None;
        private object m_size=null;
        private object m_dataLabel=null;
        private string m_fill = null;

         #endregion
        
        #region Properties
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
        [JsonProperty("border")]
        public object Border
        {
            get { return this.m_Border; }
            set { this.m_Border = value; }
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

        [JsonProperty("visible")]
        [DefaultValue(false)]
        public bool Visible
        {
            get { return m_Visible; }
            set { this.m_Visible = value; }
        }

        
        [JsonProperty("dataLabel")]
        public object DataLabel
        {
            get { return this.m_dataLabel; }
            set { this.m_dataLabel = value; }
        }
        [JsonProperty("shape")]
        [DefaultValue(ChartShape.None)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ChartShape Shape
        {
            get { return this.m_Shape; }
            set { this.m_Shape = value; }
        }

        [JsonProperty("size")]
        public object Size
        {
            get { return this.m_size; }
            set { this.m_size = value; }
        }
          #endregion

        #region ShouldSerialize
        public bool ShouldSerializeSize()
        {
            if (Utils.PropertyCompare(Size, new ChartSize()))
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
        public bool ShouldSerializeDataLabel()
        {
            if (Utils.PropertyCompare(DataLabel, new DataLabel()))
                return true;
            else
                return false;

        }
       

        #endregion
        
    }

     
    public class ConnectorLine
    {

        #region Fields

        private string m_Color = null;
        private double m_width = 0.5;
        private double m_height = -1;
        private ConnectorType m_type = ConnectorType.Line;
        #endregion

        #region Properties

        [JsonProperty("color")]
        [DefaultValue(0.5)]
        public string Color
        {
            get { return this.m_Color; }
            set { this.m_Color = value; }
        }

        [JsonProperty("width")]
        [DefaultValue(0.5)]
        public double Width
        {
            get { return this.m_width; }
            set { this.m_width = value; }
        }

        [JsonProperty("height")]
        [DefaultValue(-1)]
        public double Height
        {
            get { return this.m_height; }
            set { this.m_height = value; }
        }
        [JsonProperty("type")]
        [DefaultValue(ConnectorType.Line)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ConnectorType Type
        {
            get { return this.m_type; }
            set { this.m_type = value; }
        }
        #endregion
    }

    public class Points 
    {
        #region Fields

        private object m_X = null;
        private object m_Y = null;
        private object m_High = null;
        private object m_Low = null;
        private object m_Open = null;
        private object m_Close = null;
        private double m_Size=-1;
        private bool m_visible=true;
        private bool m_isEmpty = false;
        private string m_interior = null;
        private string m_Text = null;
        private int m_Offset = 0;
        private string m_TemplateID = null;
        private object m_Font = null;
        private double m_opacity = -1;
        private string m_fill = null;
        private object m_border = null;
        private double m_width = 1;

        private ChartShape m_Shape = ChartShape.None;
        private object marker = null;

        public Points()
        {

        }
        public Points(object x, object y)
        {
            this.m_X = x;
            this.m_Y = y;

        }
        public Points(object x, object high,object low)
        {
            this.m_X = x;
            this.m_High = high;
            this.m_Low = low;

        }
        public Points(object x, object high, object low, object open, object close)
        {
            this.m_X = x;
            this.m_High = high;
            this.m_Low = low;
            this.m_Open = open;
            this.m_Close = close;
        }
      
        #endregion

        #region Properties
		
		 [EJDate]
        [JsonProperty("x")]
        [DefaultValue(null)]
        public object X
        {
            get { return this.m_X; }
            set { this.m_X = value; }
        }

        [JsonProperty("y")]
        [DefaultValue(null)]
        public object Y
        {
            get { return this.m_Y; }
            set { this.m_Y = value; }
        }
        [JsonProperty("high")]
        [DefaultValue(null)]
        public object High
        {
            get { return this.m_High; }
            set { this.m_High = value; }
        }
        [JsonProperty("low")]
        [DefaultValue(null)]
        public object Low
        {
            get { return this.m_Low; }
            set { this.m_Low = value; }
        }
        [JsonProperty("open")]
        [DefaultValue(null)]
        public object Open
        {
            get { return this.m_Open; }
            set { this.m_Open = value; }
        }
        [JsonProperty("close")]
        [DefaultValue(null)]
        public object Close
        {
            get { return this.m_Close; }
            set { this.m_Close = value; }
        }
        [JsonProperty("visible")]
        [DefaultValue(true)]
        public bool Visible
        {
            get { return this.m_visible; }
            set { this.m_visible = value; }
        }
        [JsonProperty("isEmpty")]
        [DefaultValue(false)]
        public bool IsEmpty
        {
            get { return this.m_isEmpty; }
            set { this.m_isEmpty = value; }
        }
        
        [JsonProperty("size")]
        [DefaultValue(-1)]
        public double Size
        {
            get { return this.m_Size; }
            set { this.m_Size = value; }
        }

        [JsonProperty("text")]
        [DefaultValue(null)]
        public string Text
        {
            get { return this.m_Text; }
            set { this.m_Text = value; }
        }
        [JsonProperty("interior")]
        [DefaultValue(null)]
        public string Interior
        {
            get { return this.m_interior; }
            set { this.m_interior = value; }
        }
        [JsonProperty("offset")]
        [DefaultValue(0)]
        public int Offset
        {
            get { return this.m_Offset; }
            set { this.m_Offset = value; }
        }

        [JsonProperty("templateID")]
        [DefaultValue(0)]
        public string TemplateID
        {
            get { return this.m_TemplateID; }
            set { this.m_TemplateID = value; }
        }

        [JsonProperty("font")]
        public object Font
        {
            get { return this.m_Font; }
            set { this.m_Font = value; }
        }

        
        [JsonProperty("marker")]
        public object Marker
        {
            get { return this.marker; }
            set { this.marker = value; }
        }
        
        [JsonProperty("shape")]
        [DefaultValue(ChartShape.None)]
       [JsonConverter(typeof(StringEnumConverter))]
        public ChartShape Shape
        {
            get { return this.m_Shape; }
            set { this.m_Shape = value; }
        }
        [JsonProperty("border")]
        public object Border
        {
            get { return this.m_border; }
            set { this.m_border = value; }
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
        

        #endregion

        public bool ShouldSerializeBorder()
        {
            if (Utils.PropertyCompare(Border, new ChartBorder()))
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
    }
}