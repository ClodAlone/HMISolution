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
    

    public class   Axis
    {
        # region Field

        private object majorGridLines = null;
        private object minorGridLines = null;
        private object axisLine = null;
        private object majorTickLines = null;
        private object minorTickLines = null;
        private object m_title =null;
        
        private ChartRangePadding m_rangepadding = ChartRangePadding.Normal;
        private Orientation m_orientation;
        private bool m_hidepartiallabels = false;
        private bool m_opposedposition = false;
        private bool m_visible = true;
        private object m_font= null;  
        private int m_desiredIntervals=6;
        private double m_zoomposition = 0;
        private double m_zoomfactor = 1;
        private int m_rowIndex=0;
        private int m_plotOffset = 0;
        private int m_columnIndex=0;
        private int m_rowSpan = 0;
        private int m_columnSpan = 0;
        private int m_logBase = 10;
        private int m_categoryInterval = 0;
        private AxisValueType m_chartaxisvaluetype = AxisValueType.Double;
        private LabelIntersectAction m_labelIntersectAction = LabelIntersectAction.None;
        private string m_labelFormat;
        private int m_labelRotation=0;
        private string m_axisname = null;
        private ChartIntervalType m_intervaltype = ChartIntervalType.Milliseconds;
        private object m_range=null;
        private int m_minorTicksPerInterval = 4;
        private object m_crossHairLabel = null;
        private List<Stripline> m_stripline = new List<Stripline>();
        private int m_roundingPlaces = -1;
        private bool m_inversed = false;
        private string labelPlacement;
        #endregion
        #region Properties
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
        [JsonProperty("minorGridLines")]
        public object MinorGridLines
        {
            get
            {

                return this.minorGridLines;
            }
            set
            {
                this.minorGridLines = value;
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
        [JsonProperty("minorTickLines")]
        public object MinorTickLines
        {
            get
            {
                 
                return this.minorTickLines;
            }
            set
            {
                this.minorTickLines = value;
            }
        }
        
        /// <summary>
        /// gets or sets RangePadding
        /// </summary>
         [JsonProperty("rangePadding")]
         [DefaultValue(ChartRangePadding.Normal)]
         [JsonConverter(typeof(StringEnumConverter))]
        public ChartRangePadding RangePadding
        {
            get
            {
                return this.m_rangepadding;
            }
            set
            {
                this.m_rangepadding = value;
            }
        }
         /// <summary>
         /// gets or sets Orientation
         /// </summary>
         [JsonProperty("orientation")]
         [DefaultValue(null)]
         [JsonConverter(typeof(StringEnumConverter))]
         public Orientation Orientation
         {
             get
             {
                 return this.m_orientation;
             }
             set
             {
                 this.m_orientation = value;
             }
         }
         /// <summary>
         /// gets or sets OpposedPosition
         /// </summary>
         [JsonProperty("opposedPosition")]
         [DefaultValue(false)]
         public bool OpposedPosition
         {
             get
             {
                 return this.m_opposedposition;
             }
             set
             {
                 this.m_opposedposition = value;
             }
         }
         /// <summary>
         /// gets or sets HidePartialLabels
         /// </summary>
          [JsonProperty("hidePartialLabels")]
         [DefaultValue(false)]
         public bool HidePartialLabels
         {
             get
             {
                 return this.m_hidepartiallabels;
             }
             set
             {
                 this.m_hidepartiallabels = value;
             }
         }
        
          /// <summary>
          /// gets or sets Visible
          /// </summary>
          [DefaultValue(true)]
          [JsonProperty("visible")]
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
          /// gets or sets DesiredIntervals
          /// </summary>
          [DefaultValue(6)]
          [JsonProperty("desiredIntervals")]
          public int DesiredIntervals
          {
              get
              {
                  return this.m_desiredIntervals;
              }
              set
              {
                  this.m_desiredIntervals = value;
              }
          }
          /// <summary>
          /// gets or sets ZoomPosition
          /// </summary>
          [DefaultValue(0)]
          [JsonProperty("zoomPosition")]
          public double ZoomPosition
          {
              get
              {
                  return this.m_zoomposition;
              }
              set
              {
                  this.m_zoomposition = value;
              }

          }
          /// <summary>
          /// gets or sets ZoomFactor
          /// </summary>
          [DefaultValue(1)]
          [JsonProperty("zoomFactor")]
          public double ZoomFactor
          {
              get
              {
                  return this.m_zoomfactor;
              }
              set
              {
                  this.m_zoomfactor = value;
              }
          }
          /// <summary>
          /// gets or sets font
          /// </summary>
           [JsonProperty("font")]
          public object Font
          {
              get
              {
                  return this.m_font;
              }
              set
              {
                  this.m_font = value;
              }
          }
           /// <summary>
           /// gets or sets ValueType
           /// </summary>
           [DefaultValue(AxisValueType.Double)]
           [JsonProperty("valueType")]      
           [JsonConverter(typeof(StringEnumConverter))]
           public AxisValueType ValueType
           {
               get
               {
                   return this.m_chartaxisvaluetype;
               }
               set
               {
                   this.m_chartaxisvaluetype = value;
               }
           }
           /// <summary>
           /// gets or sets LabelFormat
           /// </summary>
            [JsonProperty("labelFormat")]
            [DefaultValue(null)]
           public string LabelFormat
           {
               get
               {
                   return this.m_labelFormat;
               }
               set
               {
                   this.m_labelFormat = value;
               }
           }

            [JsonProperty("labelRotation")]
            [DefaultValue(0)]
            public int LabelRotation
            {
                get
                {
                    return this.m_labelRotation;
                }
                set
                {
                    this.m_labelRotation = value;
                }
            }
             
            [JsonProperty("minorTicksPerInterval")]
            [DefaultValue(4)]
            public int MinorTicksPerInterval
            {
                get
                {
                    return this.m_minorTicksPerInterval;
                }
                set
                {
                    this.m_minorTicksPerInterval = value;
                }
            }
            [JsonProperty("rowIndex")]
            [DefaultValue(0)]
            public int RowIndex
            {
                get
                {
                    return this.m_rowIndex;
                }
                set
                {
                    this.m_rowIndex = value;
                }
            }
            [JsonProperty("columnIndex")]
            [DefaultValue(0)]
            public int ColumnIndex
            {
                get
                {
                    return this.m_columnIndex;
                }
                set
                {
                    this.m_columnIndex = value;
                }
            }
            /// <summary>
            /// gets or sets row span
            /// </summary>
            [JsonProperty("rowSpan")]
            [DefaultValue(0)]
            public int RowSpan
            {
                get
                {
                    return this.m_rowSpan;
                }
                set
                {
                    this.m_rowSpan = value;
                }
            }
            /// <summary>
            /// gets or sets column span
            /// </summary>
            [JsonProperty("columnSpan")]
            [DefaultValue(0)]
            public int ColumnSpan
            {
                get
                {
                    return this.m_columnSpan;
                }
                set
                {
                    this.m_columnSpan = value;
                }
            }
            /// <summary>
            /// gets or sets log base
            /// </summary>
            [JsonProperty("logBase")]
            [DefaultValue(10)]
            public int LogBase
            {
                get
                {
                    return this.m_logBase;
                }
                set
                {
                    this.m_logBase = value;
                }
            }
            /// <summary>
            /// gets or sets plot offset
            /// </summary>
            [JsonProperty("plotOffset")]
            [DefaultValue(0)]
            public int PlotOffset
            {
                get
                {
                    return this.m_plotOffset;
                }
                set
                {
                    this.m_plotOffset = value;
                }
            }
                /// <summary>
            /// gets or sets log categoryInterval
            /// </summary>
            [JsonProperty("categoryInterval")]
            [DefaultValue(0)]
            public int CategoryInterval
            {
                get
                {
                    return this.m_categoryInterval;
                }
                set
                {
                    this.m_categoryInterval = value;
                }
            }
            /// <summary>
            /// gets or sets labelIntersectAction
            /// </summary>
            [DefaultValue(LabelIntersectAction.None)]
            [JsonProperty("labelIntersectAction")]
            [JsonConverter(typeof(StringEnumConverter))]
            public LabelIntersectAction LabelIntersectAction
            {
                get
                {
                    return this.m_labelIntersectAction;
                }
                set
                {
                    this.m_labelIntersectAction = value;
                }
            }
            /// <summary>
            /// gets or sets is inversed
            /// </summary>
            [JsonProperty("isInversed")]
            [DefaultValue(false)]
            public bool IsInversed
            {
                get
                {
                    return this.m_inversed;
                }
                set
                {
                    this.m_inversed = value;
                }
            }
            /// <summary>
            /// gets or sets labelPlacement
            /// </summary>
            [JsonProperty("labelPlacement")]
            [DefaultValue(null)]
            public string LabelPlacement
            {
                get
                {
                    return this.labelPlacement;
                }
                set
                {
                    this.labelPlacement = value;
                }
            }
            /// <summary>
            /// gets or sets AxisName
            /// </summary>
            [JsonProperty("axisName")]
            [DefaultValue(null)]
            public string AxisName
            {
                get
                {
                    return this.m_axisname;
                }
                set
                {
                    this.m_axisname = value;
                }
            }
            /// <summary>
            /// gets or sets RoundingPlace
            /// </summary>
            [JsonProperty("roundingPlaces")]
            [DefaultValue(-1)]
            public int RoundingPlaces
            {
                get
                {
                    return this.m_roundingPlaces;
                }
                set
                {
                    this.m_roundingPlaces = value;
                }
            }
            /// <summary>
            /// gets or sets Orientation
            /// </summary>
            [DefaultValue(ChartIntervalType.Milliseconds)]
            [JsonProperty("intervalType")]
            [JsonConverter(typeof(StringEnumConverter))]
            public ChartIntervalType IntervalType
            {
                get
                {
                    return this.m_intervaltype;
                }
                set
                {
                    this.m_intervaltype = value;
                }
            }
        /// <summary>
        /// gets or sets Tilte
        /// </summary>
         [JsonProperty("title")]
            public object Title
        {
            get
            {
                return this.m_title;
            }
            set
            {
                m_title = value;
            }
        }

         [JsonProperty("stripline")]
         
         public List<Stripline> Stripline
         {

             get { return this.m_stripline; }
             set { this.m_stripline = value; }
         }

         [JsonProperty("crosshairLabel")]
         public object CrosshairLabel
         {
             get
             {
                 return this.m_crossHairLabel;
             }
             set
             {
                 m_crossHairLabel = value;
             }
         }
        
         [JsonProperty("range")]
         public object Range
         {
             get
             {
                 return this.m_range;
             }
             set
             {
                 m_range = value;
             }
         }
        #endregion
        
        #region ShouldSerialize Methods
        
         public bool ShouldSerializeAxisLine()
         {
             if (Utils.PropertyCompare(AxisLine, new MajorGridLines()))
                 return true;
             else
                 return false;
         }
        public bool ShouldSerializeMajorGridLines()
        {
            if (Utils.PropertyCompare(MajorGridLines, new MajorGridLines()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeMinorGridLines()
        {
            if (Utils.PropertyCompare(MinorGridLines, new MinorGridLines()))
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
        public bool ShouldSerializeCrosshairLabel()
        {
            if (Utils.PropertyCompare(CrosshairLabel, new CrosshairLabel()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeMinorTickLines()
        {
            if (Utils.PropertyCompare(MinorTickLines, new MinorTicks()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeTitle()
        {
            if (Utils.PropertyCompare(Title, new Title()))
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
        public bool ShouldSerializeRange()
        {
            if (Utils.PropertyCompare(Range, new Range()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeStripline()
        {
            if (Stripline.Count != 0)
                return true;
            else
                return false;
        }
        #endregion
    }
    /// <summary>
    /// create a Range class
    /// </summary>
    public class Range
    {
        private double m_interval = 0.0;
        private object m_min = null;
        private object m_max = null;
        /// <summary>
        /// create a Range class
        /// </summary>
        
        /// <summary>
        /// gets or sets Min
        /// </summary>
		[EJDate]
        [JsonProperty("min")]
        [DefaultValue(null)]
        public object Min
        {
            get { return this.m_min; }
            set { this.m_min=value; }
        }
        /// <summary>
        /// gets or sets Max
        /// </summary>
		 [EJDate]
        [JsonProperty("max")]
        [DefaultValue(null)]
        public object Max
        {
            get { return this.m_max; }
            set { this.m_max = value; }
        }
        /// <summary>
        /// gets or sets Interval
        /// </summary>
        [DefaultValue(0.0)]
        [JsonProperty("interval")]
        public double Interval
        {
            get
            {
                return this.m_interval;
            }
            set
            {
                this.m_interval = value;
            }
        }

    }
    /// <summary>
    /// create a Title class
    /// </summary>
    public class Title
    {
        private object m_Font = null;
        private string m_text = null;
        private TextAlignment m_textAlignment = TextAlignment.Center;
        /// <summary>
        /// gets or sets Text
        /// </summary>
        [JsonProperty("text")]
         [DefaultValue(null)]
        public string Text
        {
            get
            {
                return this.m_text;
            }
            set
            {
                this.m_text = value;
            }
        }
        /// <summary>
        /// gets or sets Text
        /// </summary>
        /// TextAlignment
        [JsonProperty("textAlignment")]
        [DefaultValue(TextAlignment.Center)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TextAlignment TextAlignment
        {
            get
            {
                return this.m_textAlignment;
            }
            set
            {
                this.m_textAlignment = value;
            }
        }
        /// <summary>
        /// gets or sets Font
        /// </summary>
         [JsonProperty("font")]
        
        public object Font
        {
            get
            { 
               
                return m_Font;
            }
            set
            {
                this.m_Font = value;
            }
        }

         #region ShouldSerialize
         public bool ShouldSerializeFont()
        {
            if (Utils.PropertyCompare(Font, new ChartFont()))
                return true;
            else
                return false;
        }
        #endregion
    }

    public class CrosshairLabel
    {

    #region Fields
        private string m_fillColor = null;
        private int m_rX = 0;
        private int m_rY = 0;
        private object m_font = null;
        private object m_border = null;
        private bool m_Visible = false;
    #endregion

    #region Properties
        [JsonProperty("border")]
        public object Border
        {
            get { return this.m_border; }
            set { this.m_border = value; }
        }
       [JsonProperty("font")]
       public object Font
        {
            get { return this.m_font; }
            set { this.m_font = value; }
        }
       [JsonProperty("visible")]
       [DefaultValue(false)]
       public bool Visible
       {
           get { return m_Visible; }
           set { this.m_Visible = value; }
       }
       /// <summary>
       /// Gets or sets the bottom value.
       /// </summary>
       /// 
       [JsonProperty("fill")]
       [DefaultValue(null)]
       public string Fill
       {
           get
           {
               return m_fillColor;
           }
           set
           {
               m_fillColor = value;
           }
       }
      
       /// <summary>
       /// Gets or sets the bottom value.
       /// </summary>
       [JsonProperty("rx")]
       [DefaultValue(0)]
       public int RX
       {
           get
           {
               return m_rX;
           }
           set
           {
               m_rX = value;
           }
       }
       /// <summary>
       /// Gets or sets the bottom value.
       /// </summary>
       [JsonProperty("ry")]
       [DefaultValue(0)]
       public int RY
       {
           get
           {
               return m_rY;
           }
           set
           {
               m_rY = value;
           }
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

    public class Stripline
    {
        #region Feilds

        private bool m_visible = true;
        private bool m_startFromAxis = false;
        private string m_text = "stripline";
        private StriplineTextAlignment m_textAlignment = StriplineTextAlignment.MiddleCenter;
        private object m_font = null;
        private string m_color = null;
        private string m_borderColor = null;
        private ChartZOrder m_zOrder = ChartZOrder.Over;
        private double m_borderWidth = 1;
        private double m_start = 0;
        private double m_end = 0;

        #endregion

        #region Properties


        [JsonProperty("visible")]
        [DefaultValue(true)]
        public bool Visible
        {
            get
            {
                return m_visible;
            }
            set
            {
                m_visible = value;
            }
        }

        [JsonProperty("startFromAxis")]
        [DefaultValue(false)]
        public bool StartFromAxis
        {
            get
            {
                return m_startFromAxis;
            }
            set
            {
                m_startFromAxis = value;
            }
        }

        [JsonProperty("color")]
        [DefaultValue(null)]
        public string StriplineColor
        {
            get
            {
                return m_color;
            }
            set
            {
                m_color = value;
            }
        }

        [JsonProperty("borderColor")]
        [DefaultValue(null)]
        public string BorderColor
        {
            get
            {
                return m_borderColor;
            }
            set
            {
                m_borderColor = value;
            }
        }

        [JsonProperty("textAlignment")]
        [DefaultValue(StriplineTextAlignment.MiddleCenter)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StriplineTextAlignment TextAlignment
        {
            get
            {
                return this.m_textAlignment;
            }
            set
            {
                this.m_textAlignment = value;
            }
        }

        [JsonProperty("zOrder")]
        [DefaultValue(ChartZOrder.Over)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ChartZOrder ZOrder
        {
            get
            {
                return this.m_zOrder;
            }
            set
            {
                this.m_zOrder = value;
            }
        }

        [JsonProperty("text")]
        [DefaultValue("stripline")]
        public string Text
        {
            get
            {
                return this.m_text;
            }
            set
            {
                this.m_text = value;
            }
        }

        [JsonProperty("borderWidth")]
        [DefaultValue(1)]
        public double BorderWidth
        {
            get
            {
                return this.m_borderWidth;
            }
            set
            {
                this.m_borderWidth = value;
            }
        }

        [JsonProperty("start")]
        [DefaultValue(0)]
        public double Start
        {
            get
            {
                return this.m_start;
            }
            set
            {
                this.m_start = value;
            }
        }


        [JsonProperty("end")]
        [DefaultValue(0)]
        public double End
        {
            get
            {
                return this.m_end;
            }
            set
            {
                this.m_end = value;
            }
        }

        [JsonProperty("font")]
        public object Font
        {
            get
            {
                return this.m_font;
            }
            set
            {
                this.m_font = value;
            }
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

        #endregion

    }
    
}
