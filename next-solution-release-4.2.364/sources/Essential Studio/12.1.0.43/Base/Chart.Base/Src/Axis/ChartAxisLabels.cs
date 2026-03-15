#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;

using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// The ChartAxisLabel class holds information about label text, label color, label font and other related information.
	/// </summary>
	public class ChartAxisLabel : ICloneable
	{
		#region Constants
		private readonly static Font c_defualtFont = new Font("Verdana", 10);
		#endregion

		#region Class members
		/// <summary>
		/// Store label custom text.
		/// </summary>
		private string m_labelCustomText;
        private string m_labelToolTip;
		/// <summary>
		/// Store label font.
		/// </summary>
		private Font m_labelFont = null;
		/// <summary>
		/// Store label color.
		/// </summary>
		private Color m_labelColor = Color.Black;
		private double m_labelDoubleValue;
		private double m_labelLogBase = 2;
		/// <summary>
		/// Store label format.
		/// </summary>
		private string m_labelFormat;
		/// <summary>
		/// Store label DateTime format.
		/// </summary>
		private string m_labelDateTimeFormat;
		/// <summary>
		/// Store chart value type.
		/// </summary>
		private ChartValueType m_labelValueType;
		/// <summary>
		/// Store label rounding precision.
		/// </summary>
		private int m_labelRoundingPlaces = 2;

		private RectangleF m_bounds = RectangleF.Empty;
		private RectangleF m_clientRect = RectangleF.Empty;
		private float m_angle = 0;
		private bool m_isAuto = false;
		private string m_text = null;
        internal ChartPlacement m_axisLabelPlacement=ChartPlacement.Outside; 
		#endregion

		#region Class properties
		/// <summary>
		/// Gets or sets the custom text that is to be used as the label.
		/// </summary>
		[DefaultValue("")]
		public string CustomText
		{
			get
			{
				return m_labelCustomText;
			}
			set
			{
				if (m_labelCustomText != value)
				{
					m_labelCustomText = value;
					m_text = null;
				}
			}
		}
        /// <summary>
        /// Gets or sets the tooltip for the axis label.
        /// </summary>
        public string ToolTip
        {
            get
            {
                if (m_labelToolTip != null)
                {
                    return m_labelToolTip;
                }
                else
                {
                    return this.Text;
                }
            }
            set
            {
                if (m_labelToolTip != value)
                {
                    m_labelToolTip = value;
                   
                }
            }
        }
  
		/// <summary>
		/// Gets or sets the date format that is to be used for formatting the value into the label text.
		/// See "Date and Time format strings" section in MSDN for more info.
		/// </summary>
		[DefaultValue("")]
		public string DateTimeFormat
		{
			get
			{
				return m_labelDateTimeFormat;
			}
			set
			{
				if (m_labelDateTimeFormat != value)
				{
					m_labelDateTimeFormat = value;
					m_text = null;
				}
			}
		}
		/// <summary>
		/// Gets or sets the format that is to be used for formatting the double values into the label text.
		/// See "Numeric Format Strings" section in MSDN for more on the supported formats.
		/// </summary>
		[DefaultValue("")]
		public string Format
		{
			get
			{
				return m_labelFormat;
			}
			set
			{
				if (m_labelFormat != value)
				{
					m_labelFormat = value;
					m_text = null;
				}
			}
		}
        /// <summary>
        /// Gets or sets a value indicates whether label is located inside or outside of chart area.
        /// </summary>
        [Description("Specifies label is located inside or outside of chart area."), DefaultValue(ChartPlacement.Outside)]       
         public ChartPlacement AxisLabelPlacement
        {
            get
            {
                return  m_axisLabelPlacement;
            }
            set
            {
                if (m_axisLabelPlacement != value)
                     m_axisLabelPlacement = value;
             }
        }
        
		/// <summary>
		/// Gets or sets the log base that is to be used by the label. Default is 2.
		/// </summary>
		[DefaultValue(2)]
		public double LogBase
		{
			get
			{
				return m_labelLogBase;
			}
			set
			{
				if (m_labelLogBase != value)
				{
					m_labelLogBase = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the value that the label represents.
		/// </summary>
		[DefaultValue(0d)]
		public double DoubleValue
		{
			get
			{
				return m_labelDoubleValue;
			}
			set
			{
				if (m_labelDoubleValue != value)
				{
					m_labelDoubleValue = value;
					m_text = null;
				}
			}
		}
		/// <summary>
		/// Number of relevant rounding places that is to be used for the label. Default is 2.
		/// </summary>
		[DefaultValue(2)]
		public int RoundingPlaces
		{
			get
			{
				return m_labelRoundingPlaces;
			}
			set
			{
				if (m_labelRoundingPlaces != value)
				{
					m_labelRoundingPlaces = value;
					m_text = null;
				}
			}
		}
		/// <summary>
		/// Gets the formatted text that is to be displayed as the label.
		/// </summary>
		[Browsable(false)]
		public string Text
		{
			get
			{
				if (m_text == null)
				{
					this.ComputeText();
				}

				return m_text;
			}
		}
		/// <summary>
		/// Gets or sets the font that is to be used for the label text.
		/// </summary>
		[DefaultValue(typeof(Font), "Verdana, 10pt")]
		public Font Font
		{
			get
			{
				return m_labelFont == null ? c_defualtFont : m_labelFont;
			}
			set
			{
				if (m_labelFont != value)
				{
					m_labelFont = value;
					m_text = null;
				}
			}
		}
		/// <summary>
		/// Gets or sets the color that is to be used for the label text.
		/// </summary>
		[DefaultValue(typeof(Color), "Black")]
		public Color Color
		{
			get
			{
				return m_labelColor.IsEmpty ? Color.Black : m_labelColor;
			}
			set
			{
				if (m_labelColor != value)
				{
					m_labelColor = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the value type that is to be associated with the axis label. Default is Double.
		/// </summary>
		[DefaultValue(ChartValueType.Double)]
		public ChartValueType ValueType
		{
			get
			{
				return m_labelValueType;
			}
			set
			{
				if (m_labelValueType != value)
				{
					m_labelValueType = value;
					m_text = null;
				}
			}
		}

		/// <summary>
		/// Get's the rendering bounds of this label.
		/// </summary>
		internal RectangleF Bounds
		{
			get
			{
				return m_bounds;
			}
		}
		/// <summary>
		/// Indicates if label wasn't added by user.
		/// </summary>
		internal bool IsAuto
		{
			get
			{
				return m_isAuto;
			}
			set
			{
				m_isAuto = value;
			}
		}
		#endregion

		#region Class constructors
		/// <summary>
		/// Overloaded Constructor. Each label along a ChartAxis is held in a ChartAxisLabel.
		/// </summary>
		public ChartAxisLabel()
			: this( "" )
		{
		}
		/// <summary>
		/// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
		/// </summary>
		/// <param name="customText">The text that will be displayed as the label for the axis point.</param>
		public ChartAxisLabel(string customText)
			: this( customText, 0 )
		{
		}
        /// <summary>
		/// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
		/// </summary>
		/// <param name="customText">The text that will be displayed as the label for the axis point.</param>
        /// <param name="toolTip">The text that will be displayed as the tooltip for the axis labels.</param>
		public ChartAxisLabel(string customText,string toolTip)
			: this( customText, 0 ,toolTip)
		{
		}
		/// <summary>
		/// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
		/// </summary>
		/// <param name="customText">The text that will be displayed as the label for the axis point.</param>
		/// <param name="value">The value.</param>
		public ChartAxisLabel(string customText, double value)
			: this( customText, Color.Empty, null, value )
		{
		}
        /// <summary>
        /// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
        /// </summary>
        /// <param name="customText">The text that will be displayed as the label for the axis point.</param>
        /// <param name="value">The value.</param>
        /// <param name="toolTip">The text that will be displayed as the tooltip for the axis labels.</param>
        public ChartAxisLabel(string customText, double value,string toolTip)
            : this(customText, Color.Empty, null, value,toolTip)
        {
        }
		/// <summary>
		/// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
		/// </summary>
		/// <param name="customText">The text that will be displayed as the label for the axis point.</param>
		/// <param name="color">The color that is to be used for the label text.</param>
		/// <param name="font">The font that is to be used for the label text.</param>
		public ChartAxisLabel(string customText, Color color, Font font)
			: this( customText, color, font, 0d)
		{
		}
        /// <summary>
        /// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
        /// </summary>
        /// <param name="customText">The text that will be displayed as the label for the axis point.</param>
        /// <param name="color">The color that is to be used for the label text.</param>
        /// <param name="font">The font that is to be used for the label text.</param>
        /// <param name="toolTip">The text that will be displayed as the tooltip for the axis labels.</param>
        public ChartAxisLabel(string customText, Color color, Font font,string toolTip)
            : this(customText, color, font, 0d,toolTip)
        {
        }
		/// <summary>
		/// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
		/// </summary>
		/// <param name="customText">The text that will be displayed as the label for the axis point.</param>
		/// <param name="color">The color that is to be used for the label text.</param>
		/// <param name="font">The font that is to be used for the label text.</param>
		/// <param name="dvalue">The value represented by the label.</param>
		public ChartAxisLabel(string customText, Color color, Font font, double dvalue)
		{
			m_labelCustomText = customText;
			m_labelColor = color;
			m_labelFont = font;
			m_labelDoubleValue = dvalue;
			m_labelDateTimeFormat = "";
			m_labelValueType = ChartValueType.Custom;
		}
        /// <summary>
        /// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
        /// </summary>
        /// <param name="customText">The text that will be displayed as the label for the axis point.</param>
        /// <param name="color">The color that is to be used for the label text.</param>
        /// <param name="font">The font that is to be used for the label text.</param>
        /// <param name="dvalue">The value represented by the label.</param>
         /// <param name="toolTip">The text that will be displayed as the tooltip for the axis labels.</param>
        public ChartAxisLabel(string customText, Color color, Font font, double dvalue,string toolTip)
        {
            m_labelCustomText = customText;
            m_labelToolTip = toolTip;
            m_labelColor = color;
            m_labelFont = font;
            m_labelDoubleValue = dvalue;
            m_labelDateTimeFormat = "";
            m_labelValueType = ChartValueType.Custom;
        }
		/// <summary>
		/// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
		/// </summary>
		/// <param name="dvalue">The value represented by the label.</param>
		/// <param name="format">The format that is to be used for formatting the display label.</param>
		public ChartAxisLabel(double dvalue, string format)
		{
			m_labelCustomText = "";
			m_labelColor = Color.Black;
			m_labelFont = c_defualtFont;
			m_labelDoubleValue = dvalue;
			m_labelFormat = format;
			m_labelValueType = ChartValueType.Double;
		}
        /// <summary>
        /// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
        /// </summary>
        /// <param name="dvalue">The value represented by the label.</param>
        /// <param name="format">The format that is to be used for formatting the display label.</param>
        /// <param name="toolTip">The text that will be displayed as the tooltip for the axis labels.</param>
        public ChartAxisLabel(double dvalue, string format,string toolTip)
        {
            m_labelCustomText = "";
            m_labelToolTip = toolTip;
            m_labelColor = Color.Black;
            m_labelFont = c_defualtFont;
            m_labelDoubleValue = dvalue;
            m_labelFormat = format;
            m_labelValueType = ChartValueType.Double;
        }
		/// <summary>
		/// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
		/// </summary>
		/// <param name="dt">The Date Time value represented by the label.</param>
		/// <param name="dateTimeFormat">The format that is to be used for formatting the display label.</param>
		public ChartAxisLabel(DateTime dt, string dateTimeFormat)
		{
			m_labelCustomText = "";
			m_labelColor = Color.Black;
			m_labelFont = c_defualtFont;
			m_labelDoubleValue = dt.ToOADate();
			m_labelDateTimeFormat = dateTimeFormat;
			m_labelValueType = ChartValueType.DateTime;
		}
        /// <summary>
        /// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
        /// </summary>
        /// <param name="dt">The Date Time value represented by the label.</param>
        /// <param name="toolTip">The text that will be displayed as the tooltip for the axis labels.</param>
        /// <param name="dateTimeFormat">The format that is to be used for formatting the display label.</param>
        public ChartAxisLabel(DateTime dt, string dateTimeFormat,string toolTip)
        {
            m_labelCustomText = "";
            m_labelToolTip = toolTip;
            m_labelColor = Color.Black;
            m_labelFont = c_defualtFont;
            m_labelDoubleValue = dt.ToOADate();
            m_labelDateTimeFormat = dateTimeFormat;
            m_labelValueType = ChartValueType.DateTime;
        }
		/// <summary>
		/// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
		/// </summary>
		/// <param name="customText">The text that will be displayed as the label for the axis point.</param>
		/// <param name="color">The color that is to be used for the label text.</param>
		/// <param name="font">The font that is to be used for the label text.</param>
		/// <param name="dvalue">The value represented by the label.</param>
		/// <param name="format">The format that is to be used for formatting the display label.</param>
		/// <param name="valueType">The value type of the axis label.</param>
		public ChartAxisLabel(string customText, Color color, Font font, double dvalue, string format, ChartValueType valueType)
			: this( customText, color, font, dvalue, format, "", valueType)
		{
		}
        /// <summary>
        /// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
        /// </summary>
        /// <param name="customText">The text that will be displayed as the label for the axis point.</param>
        /// <param name="toolTip">The text that will be displayed as the tooltip for the axis labels.</param>
        /// <param name="color">The color that is to be used for the label text.</param>
        /// <param name="font">The font that is to be used for the label text.</param>
        /// <param name="dvalue">The value represented by the label.</param>
        /// <param name="format">The format that is to be used for formatting the display label.</param>
        /// <param name="valueType">The value type of the axis label.</param>
        public ChartAxisLabel(string customText,string toolTip, Color color, Font font, double dvalue, string format, ChartValueType valueType)
            : this(customText,toolTip, color, font, dvalue, format, "", valueType)
        {
        }
		/// <summary>
		/// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
		/// </summary>
		/// <param name="customText">The text that will be displayed as the label for the axis point.</param>
		/// <param name="color">The color that is to be used for the label text.</param>
		/// <param name="font">The font that is to be used for the label text.</param>
		/// <param name="dvalue">The value represented by the label.</param>
		/// <param name="format">The format that is to be used for formatting the display label.</param>
		/// <param name="dateTimeFormat">The date time format that is to be used for formatting the value.</param>
		/// <param name="valueType">The value type of the axis label.</param>
		public ChartAxisLabel(string customText, Color color, Font font, double dvalue, string format, string dateTimeFormat, ChartValueType valueType)
		{
			m_labelCustomText = customText;
			m_labelColor = color;
			m_labelFont = font;
			m_labelDoubleValue = dvalue;
			m_labelFormat = format;
			m_labelDateTimeFormat = dateTimeFormat;
			m_labelValueType = valueType;
		}
        /// <summary>
        /// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
        /// </summary>
        /// <param name="customText">The text that will be displayed as the label for the axis point.</param>
        /// <param name="toolTip">The text that will be displayed as tooltip for the label.</param>
        /// <param name="color">The color that is to be used for the label text.</param>
        /// <param name="font">The font that is to be used for the label text.</param>
        /// <param name="dvalue">The value represented by the label.</param>
        /// <param name="format">The format that is to be used for formatting the display label.</param>
        /// <param name="dateTimeFormat">The date time format that is to be used for formatting the value.</param>
        /// <param name="valueType">The value type of the axis label.</param>
        public ChartAxisLabel(string customText,string toolTip,Color color, Font font, double dvalue, string format, string dateTimeFormat, ChartValueType valueType)
        {
            m_labelCustomText = customText;
            m_labelToolTip = toolTip;
            m_labelColor = color;
            m_labelFont = font;
            m_labelDoubleValue = dvalue;
            m_labelFormat = format;
            m_labelDateTimeFormat = dateTimeFormat;
            m_labelValueType = valueType;
        }
		/// <summary>
		/// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
		/// </summary>
		/// <param name="customText">The text that will be displayed as the label for the axis point.</param>
		/// <param name="color">The color that is to be used for the label text.</param>
		/// <param name="font">The font that is to be used for the label text.</param>
		/// <param name="dateTime">The DateTime value represented by the label.</param>
		/// <param name="format">The format that is to be used for formatting the display label.</param>
		/// <param name="dateTimeFormat">The date time format that is to be used for formatting the value.</param>
		/// <param name="valueType">The value type of the axis label.</param>
		public ChartAxisLabel(string customText, Color color, Font font, DateTime dateTime, string format, string dateTimeFormat, ChartValueType valueType)
		{
			m_labelCustomText = customText;
			m_labelColor = color;
			m_labelFont = font;
			m_labelDoubleValue = dateTime.ToOADate();
			m_labelFormat = format;
			m_labelDateTimeFormat = dateTimeFormat;
			m_labelValueType = valueType;
		}
        /// <summary>
        /// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
        /// </summary>
        /// <param name="customText">The text that will be displayed as the label for the axis point.</param>
        /// <param name="toolTip">The text that will be displayed as tooltip for the label.</param>
        /// <param name="color">The color that is to be used for the label text.</param>
        /// <param name="font">The font that is to be used for the label text.</param>
        /// <param name="dateTime">The DateTime value represented by the label.</param>
        /// <param name="format">The format that is to be used for formatting the display label.</param>
        /// <param name="dateTimeFormat">The date time format that is to be used for formatting the value.</param>
        /// <param name="valueType">The value type of the axis label.</param>
        public ChartAxisLabel(string customText,string toolTip, Color color, Font font, DateTime dateTime, string format, string dateTimeFormat, ChartValueType valueType)
        {
            m_labelCustomText = customText;
            m_labelToolTip = toolTip;
            m_labelColor = color;
            m_labelFont = font;
            m_labelDoubleValue = dateTime.ToOADate();
            m_labelFormat = format;
            m_labelDateTimeFormat = dateTimeFormat;
            m_labelValueType = valueType;
        }
		/// <summary>
		/// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
		/// </summary>
		/// <param name="dvalue">The value represented by the label.</param>
		/// <param name="format">The format that is to be used for formatting the display label.</param>
		/// <param name="valueType">The value type of the axis label.</param>
		public ChartAxisLabel(double dvalue, string format, ChartValueType valueType)
		{
			m_labelCustomText = "";
			m_labelColor = Color.Black;
			m_labelFont = c_defualtFont;
			m_labelDoubleValue = dvalue;
			m_labelFormat = format;
			m_labelDateTimeFormat = "";
			m_labelValueType = valueType;
		}
        /// <summary>
        /// Constructor. Each label along a ChartAxis is held in the ChartAxisLabel.
        /// </summary>
        /// <param name="dvalue">The value represented by the label.</param>
        /// <param name="format">The format that is to be used for formatting the display label.</param>
        /// <param name="toolTip">The text that will be displayed as tooltip for the label.</param>
        /// <param name="valueType">The value type of the axis label.</param>
        public ChartAxisLabel(double dvalue, string format,string toolTip,ChartValueType valueType)
        {
            m_labelCustomText = "";
            m_labelToolTip = toolTip;
            m_labelColor = Color.Black;
            m_labelFont = c_defualtFont;
            m_labelDoubleValue = dvalue;
            m_labelFormat = format;
            m_labelDateTimeFormat = "";
            m_labelValueType = valueType;
        }
		#endregion

		#region Implementation
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public ChartAxisLabel Clone()
		{
			ChartAxisLabel label = new ChartAxisLabel();

			label.m_labelCustomText = m_labelCustomText;
            label.m_labelToolTip = m_labelToolTip;
			label.m_labelFont = m_labelFont;
			label.m_labelColor = m_labelColor;
			label.m_labelDoubleValue = m_labelDoubleValue;
			label.m_labelLogBase = m_labelLogBase;
			label.m_labelFormat = m_labelFormat;
			label.m_labelDateTimeFormat = m_labelDateTimeFormat;
			label.m_labelValueType = m_labelValueType;
			label.m_labelRoundingPlaces = m_labelRoundingPlaces;

			return label;
		}
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		/// <summary>
		/// Computes the text of label.
		/// </summary>
		private void ComputeText()
		{
			switch (m_labelValueType)
			{
				case ChartValueType.Double:
					m_text = Math.Round(m_labelDoubleValue, m_labelRoundingPlaces).ToString(m_labelFormat);
					break;

				case ChartValueType.DateTime:
					if (string.IsNullOrEmpty(m_labelDateTimeFormat))
					{
						m_text = DateTime.FromOADate(m_labelDoubleValue).ToShortDateString();
					}
					else
					{
						m_text = DateTime.FromOADate(m_labelDoubleValue).ToString(m_labelDateTimeFormat);
					}
					break;

				case ChartValueType.Custom:
					m_text = m_labelCustomText;
					break;

				case ChartValueType.Logarithmic:
					m_text = Math.Pow(m_labelLogBase, m_labelDoubleValue).ToString(m_labelFormat);
					break;
			}
		}

		/// <summary>
		/// Measures label by the specified <see cref="Graphics"/>.
		/// </summary>
		/// <param name="g"><see cref="Graphics"/></param>
        /// <param name="axis">The ChartAxis</param>
		/// <returns><see cref="SizeF"/></returns>
		internal SizeF Measure(Graphics g, ChartAxis axis)
		{
			System.Drawing.Font font = m_labelFont == null ? axis.Font : m_labelFont;
			m_clientRect = new RectangleF(PointF.Empty, g.MeasureString(this.Text, font));

			return m_clientRect.Size;
		}
		/// <summary>
		/// Measures label by the specified <see cref="Graphics"/>.
		/// </summary>
		/// <param name="g"><see cref="Graphics"/></param>
		/// <param name="width">Max width of the text.</param>
        /// <param name="axis">The ChartAxis</param>
		/// <returns><see cref="SizeF"/></returns>
		internal SizeF Measure(Graphics g, float width, ChartAxis axis)
		{
			System.Drawing.Font font = m_labelFont == null ? axis.Font : m_labelFont;
			m_clientRect = new RectangleF(PointF.Empty, g.MeasureString(this.Text, font, (int)width));

			return m_clientRect.Size;
		}
		/// <summary>
		/// Sets the bounds of label.
		/// </summary>
		/// <param name="connectPoint">The connect point.</param>
		/// <returns>Returns bounds of label</returns>
		internal RectangleF Arrange(PointF connectPoint)
		{
			m_angle = 0;
			return m_bounds = new RectangleF(connectPoint, m_clientRect.Size);
		}
		/// <summary>
		/// Sets the bounds of label.
		/// </summary>
		/// <param name="connectPoint">Position of label.</param>
		/// <param name="angle">Rotation of lebel.</param>
		/// <returns>Returns bounds of label</returns>
		internal RectangleF Arrange(PointF connectPoint, float angle)
		{
			m_angle = angle;
			SizeF sz = m_clientRect.Size;

			#region Measure text
			if (m_angle != 0)
			{
				double cos = Math.Abs(Math.Cos(m_angle * ChartMath.ToRadians));
				double sin = Math.Abs(Math.Sin(m_angle * ChartMath.ToRadians));

				double w = cos * sz.Width + sin * sz.Height;
				double h = sin * sz.Width + cos * sz.Height;

				sz = new SizeF((float)w, (float)h);
			}
			#endregion

			return m_bounds = new RectangleF(connectPoint, sz);
		}
		/// <summary>
		/// Sets the bounds of label.
		/// </summary>
		/// <param name="connectPoint">Position of label.</param>
		/// <param name="aligment">The alignment.</param>
		/// <returns>Returns bounds of label</returns>
		internal RectangleF Arrange(PointF connectPoint, ContentAlignment aligment)
		{
			m_angle = 0;
			PointF loc = connectPoint;
			SizeF sz = m_clientRect.Size;

			#region Add alignment
			switch (aligment)
			{
				case ContentAlignment.BottomCenter:
					loc = new PointF(loc.X - sz.Width / 2, loc.Y);
					break;
				case ContentAlignment.BottomLeft:
					loc = new PointF(loc.X - sz.Width, loc.Y);
					break;
				case ContentAlignment.BottomRight:
					loc = new PointF(loc.X, loc.Y);
					break;

				case ContentAlignment.MiddleCenter:
					loc = new PointF(loc.X - sz.Width / 2, loc.Y - sz.Height / 2);
					break;
				case ContentAlignment.MiddleLeft:
					loc = new PointF(loc.X - sz.Width, loc.Y - sz.Height / 2);
					break;
				case ContentAlignment.MiddleRight:
					loc = new PointF(loc.X, loc.Y - sz.Height / 2);
					break;

				case ContentAlignment.TopCenter:
					loc = new PointF(loc.X - sz.Width / 2, loc.Y - sz.Height);
					break;
				case ContentAlignment.TopLeft:
					loc = new PointF(loc.X - sz.Width, loc.Y - sz.Height);
					break;
				case ContentAlignment.TopRight:
					loc = new PointF(loc.X, loc.Y - sz.Height);
					break;
			}
			#endregion

			return m_bounds = new RectangleF(loc, m_clientRect.Size);
		}
		/// <summary>
		/// Sets the bounds of label.
		/// </summary>
		/// <param name="connectPoint">Position of label.</param>
		/// <param name="aligment">The alignment.</param>
		/// <param name="angle">Rotation of lebel.</param>
        /// <param name="m_rotateFromTicks"></param>
		/// <returns>Returns bounds of label</returns>
        internal RectangleF Arrange(PointF connectPoint, ContentAlignment aligment, float angle, bool m_rotateFromTicks)
		{
			PointF loc = connectPoint;
			SizeF sz = m_clientRect.Size;

			m_angle = angle;

			#region Measure text
			if (m_angle != 0)
			{
				double cos = Math.Abs(Math.Cos(m_angle * ChartMath.ToRadians));
				double sin = Math.Abs(Math.Sin(m_angle * ChartMath.ToRadians));

				double w = cos * sz.Width + sin * sz.Height;
				double h = sin * sz.Width + cos * sz.Height;

				sz = new SizeF((float)w, (float)h);
			}
			#endregion

			#region Add alignment
			switch (aligment)
			{
				case ContentAlignment.BottomCenter:
					loc = new PointF(loc.X - sz.Width / 2, loc.Y);
					break;
				case ContentAlignment.BottomLeft:
					loc = new PointF(loc.X - sz.Width, loc.Y);
					break;
				case ContentAlignment.BottomRight:
                    if (!m_rotateFromTicks)
                        loc = new PointF(loc.X, loc.Y);
                    else
					loc = new PointF(loc.X, loc.Y);
					break;

				case ContentAlignment.MiddleCenter:
					loc = new PointF(loc.X - sz.Width / 2, loc.Y - sz.Height / 2);
					break;
				case ContentAlignment.MiddleLeft:
					loc = new PointF(loc.X - sz.Width, loc.Y - sz.Height / 2);
					break;
				case ContentAlignment.MiddleRight:					
                    loc = new PointF(loc.X, loc.Y - sz.Height / 2);
					break;

				case ContentAlignment.TopCenter:
					loc = new PointF(loc.X - sz.Width / 2, loc.Y - sz.Height);
					break;
				case ContentAlignment.TopLeft:
					loc = new PointF(loc.X - sz.Width, loc.Y - sz.Height);
					break;
				case ContentAlignment.TopRight:
					loc = new PointF(loc.X, loc.Y - sz.Height);
					break;
			}
			#endregion

			return m_bounds = new RectangleF(loc, sz);
		}
		/// <summary>
		/// Draws the label to specified <see cref="Graphics"/>.
		/// </summary>
		/// <param name="g"><see cref="Graphics"/></param>
		/// <param name="axis">The axis.</param>
        /// <param name="chartArea"></param>
		internal void Draw(Graphics g, ChartAxis axis,ChartArea chartArea)
		{
			GraphicsContainer cont = DrawingHelper.BeginTransform(g);

			System.Drawing.Font font = m_labelFont == null ? axis.Font : m_labelFont;
			System.Drawing.Color textColor = m_labelColor.IsEmpty ? axis.ForeColor : m_labelColor;
            StringFormat stringFormat = new StringFormat(axis.LabelStringFormat);
            stringFormat.Alignment = ((axis.Orientation == ChartOrientation.Vertical) && (!axis.OpposedPosition)) ? StringAlignment.Far : 0;

			if (m_angle == 0f)
			{
                if (this.AxisLabelPlacement== ChartPlacement.Inside  && axis.AxisLabelPlacement != this.AxisLabelPlacement )
                {
                    if (axis.Orientation == ChartOrientation.Vertical)
                    {
                       if(!axis.OpposedPosition)
                        g.TranslateTransform((m_bounds.X + axis.LineType.Width + (2 * axis.TickSize.Width) + m_clientRect.Width), m_bounds.Y);
                       else
                        g.TranslateTransform((m_bounds.X - axis.LineType.Width - (2 * axis.TickSize.Width) - m_clientRect.Width), m_bounds.Y);
                    }
                    else {
                        if (!axis.OpposedPosition)
                        g.TranslateTransform(m_bounds.X, m_bounds.Y + axis.LineType.Width - (2 * axis.TickSize.Height) - m_clientRect.Height);
                       else
                        g.TranslateTransform(m_bounds.X, m_bounds.Y + axis.LineType.Width + (2 * axis.TickSize.Height) + m_clientRect.Height);
                            
                    }
                }
                else if (this.AxisLabelPlacement == ChartPlacement.Outside && axis.AxisLabelPlacement != this.AxisLabelPlacement)
                {
                    if (axis.Orientation == ChartOrientation.Vertical)
                    {
                        if (!axis.OpposedPosition)
                            g.TranslateTransform((m_bounds.X - axis.LineType.Width - (2 * axis.TickSize.Width) - m_clientRect.Width), m_bounds.Y);
                        else
                            g.TranslateTransform((m_bounds.X + axis.LineType.Width + (2 * axis.TickSize.Width) + m_clientRect.Width), m_bounds.Y);
                    }
                    else
                        if (!axis.OpposedPosition)
                        g.TranslateTransform(m_bounds.X, m_bounds.Y + axis.LineType.Width + (2 * axis.TickSize.Height) + m_clientRect.Height);
                        else
                        g.TranslateTransform(m_bounds.X, m_bounds.Y + axis.LineType.Width - (2 * axis.TickSize.Height) - m_clientRect.Height);

                }
                else
                    g.TranslateTransform(m_bounds.X, m_bounds.Y);
			}
			else
			{
				g.TranslateTransform(m_bounds.X + m_bounds.Width / 2, m_bounds.Y + m_bounds.Height / 2);
				g.RotateTransform(m_angle);
				g.TranslateTransform(-m_clientRect.Width / 2, -m_clientRect.Height / 2);
			}
            chartArea.Chart.ChartRegions.Add(new ChartRegion(new Region(this.m_bounds),ChartRegionType.Axis,this.ToolTip,"AxesLabel Region"));
			using (SolidBrush sb = new SolidBrush(textColor))
			{
                g.DrawString(this.Text, font, sb, m_clientRect, stringFormat);
			}
            stringFormat.Dispose();
			DrawingHelper.EndTransform(g, cont);
		}
		/// <summary>
		/// Generates the 3d geometry of label.
		/// </summary>
		/// <param name="g"><see cref="Graphics3D"/></param>
		/// <param name="axis">The axis.</param>
		/// <param name="z">Depth of label.</param>
        /// <param name="chartArea"></param>
		/// <returns><see cref="Path3D"/></returns>
		internal Path3D Draw3D(Graphics3D g, ChartAxis axis, float z,ChartArea chartArea)
		{
			Matrix transformation = new Matrix();
			GraphicsPath gp = new GraphicsPath();

			System.Drawing.Font font = m_labelFont == null ? axis.Font : m_labelFont;
			System.Drawing.Color textColor = m_labelColor.IsEmpty ? axis.ForeColor : m_labelColor;

			if (m_angle == 0f)
			{
                if (this.AxisLabelPlacement == ChartPlacement.Inside && axis.AxisLabelPlacement != this.AxisLabelPlacement)
                {
                    if (axis.Orientation == ChartOrientation.Vertical)
                    {
                        if (!axis.OpposedPosition)
                            transformation.Translate((m_bounds.X + axis.LineType.Width + (2 * axis.TickSize.Width) + m_clientRect.Width), m_bounds.Y);
                        else
                            transformation.Translate((m_bounds.X - axis.LineType.Width - (2 * axis.TickSize.Width) - m_clientRect.Width), m_bounds.Y);
                    }
                    else
                    {
                        if (!axis.OpposedPosition)
                            transformation.Translate(m_bounds.X, m_bounds.Y + axis.LineType.Width - (2 * axis.TickSize.Height) - m_clientRect.Height);
                        else
                            transformation.Translate(m_bounds.X, m_bounds.Y + axis.LineType.Width + (2 * axis.TickSize.Height) + m_clientRect.Height);

                    }
                }
                else if (this.AxisLabelPlacement == ChartPlacement.Outside && axis.AxisLabelPlacement != this.AxisLabelPlacement)
                {
                    if (axis.Orientation == ChartOrientation.Vertical)
                    {
                        if (!axis.OpposedPosition)
                            transformation.Translate((m_bounds.X - axis.LineType.Width - (2 * axis.TickSize.Width) - m_clientRect.Width), m_bounds.Y);
                        else
                            transformation.Translate((m_bounds.X + axis.LineType.Width + (2 * axis.TickSize.Width) + m_clientRect.Width), m_bounds.Y);
                    }
                    else
                        if (!axis.OpposedPosition)
                            transformation.Translate(m_bounds.X, m_bounds.Y + axis.LineType.Width + (2 * axis.TickSize.Height) + m_clientRect.Height);
                        else
                            transformation.Translate(m_bounds.X, m_bounds.Y + axis.LineType.Width - (2 * axis.TickSize.Height) - m_clientRect.Height);

                }
                else
				transformation.Translate(m_bounds.X, m_bounds.Y);
			}
			else
			{
				transformation.Translate(m_bounds.X + m_bounds.Width / 2, m_bounds.Y + m_bounds.Height / 2);
				transformation.Rotate(m_angle);
				transformation.Translate(-m_clientRect.Width / 2, -m_clientRect.Height / 2);
			}
            chartArea.Chart.ChartRegions.Add(new ChartRegion(new Region(this.m_bounds), ChartRegionType.Axis, this.ToolTip, "3DAxesLabel Region"));
			RenderingHelper.AddTextPath(gp, g.Graphics, this.Text, font, m_clientRect);
			gp.Transform(transformation);

			return Path3D.FromGraphicsPath(gp, z, new SolidBrush(textColor));
		}
		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	class ChartAxisLabelByDoubleValueComparer : IComparer
	{
		#region Members
		private bool m_inversed;
		#endregion

		#region Constructor
		/// <internalonly/>
		public ChartAxisLabelByDoubleValueComparer(bool inversed)
		{
			m_inversed = inversed;
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		int System.Collections.IComparer.Compare(object x, object y)
		{
			ChartAxisLabel l1 = (ChartAxisLabel)x;
			ChartAxisLabel l2 = (ChartAxisLabel)y;

			if (l1.DoubleValue == l2.DoubleValue)
			{
				return 0;
			}
			else if (l1.DoubleValue < l2.DoubleValue)
			{
				return m_inversed ? 1 : -1;
			}
			else
			{
				return m_inversed ? -1 : 1;
			}
		}
		#endregion
	}

	/// <summary>
	/// The ChartAxisGroupingLabel class holds information about Grouping Label text, Grouping Label color, Grouping Label font and other related information.
	/// </summary>
	[Serializable]
	public class ChartAxisGroupingLabel
	{
		#region Class members
		private StringFormat m_labelFormat = (StringFormat)(StringFormat.GenericDefault.Clone());
		private string m_labelText;
		private string m_description;
		private Font m_labelFont;
		private Color m_labelColor;
		private Color m_backColor = Color.Transparent;
		private Pen m_labelBorderPen = Pens.Black;
		private DoubleRange m_labelDoubleRange;
		private ChartAxisGroupingLabelBorderStyle m_labelBorderStyle = ChartAxisGroupingLabelBorderStyle.Rectangle;
		private float m_labelBorderPadding = 2.0f;
		private float m_labelMaxTextWidth = 200;
		private float m_labelMaxTextHeightToWidthRatio = 2.5f;
		private float m_labelRotateAngle = 0;
		private int m_labelRow = 0;
		private float m_labelGridDimension = 0;
		private RectangleF m_rect = RectangleF.Empty;
		private ChartAxisGroupingLabelTextAlignment m_textAlignment = ChartAxisGroupingLabelTextAlignment.Center;
		private ChartAxisGroupingLabelTextFitMode m_labelTextFitMode = ChartAxisGroupingLabelTextFitMode.Shrink;
		private Object m_tag;
		#endregion

		#region Class properties
		/// <summary>
		/// Gets or sets the double range that this Grouping Label with cover. If the range in the axis is DateTime, use DateTime.ToOADate to get the double value.
		/// </summary>
		public DoubleRange Range
		{
			get
			{
				return m_labelDoubleRange;
			}
			set
			{
				if (m_labelDoubleRange != value)
				{
					m_labelDoubleRange = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the custom text that is to be used as the Grouping Label.
		/// </summary>
		public string Text
		{
			get
			{
				return m_labelText;
			}
			set
			{
				if (m_labelText != value)
				{
					m_labelText = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the the region description text for this region.
		/// </summary>
		public string RegionDescription
		{
			get
			{
				return m_description;
			}
			set
			{
				if (m_description != value)
				{
					m_description = value;
				}
			}
		}
		/// <summary>
		/// Lets you specify the grouping label text in multiple lines
		/// </summary>
		public string[] Lines
		{
			get
			{
				int startIndex = 0;
				ArrayList list = new ArrayList();

				while (startIndex > -1)
				{
					int index = m_labelText.IndexOf(Environment.NewLine, startIndex);

					if (index > -1)
					{
						list.Add(m_labelText.Substring(startIndex, index - startIndex));
						startIndex = index + Environment.NewLine.Length;
					}
					else
					{
						list.Add(m_labelText.Substring(startIndex, m_labelText.Length - startIndex));
						startIndex = index;
					}
				}

				return (string[])list.ToArray(typeof(string));
			}
			set
			{
				m_labelText = String.Join(Environment.NewLine, value);
			}
		}
		/// <summary>
		/// Gets or sets <see cref="System.Drawing.StringFormat"/> to render label. Default is GenericDefault.
		/// </summary>
		public StringFormat Format
		{
			get
			{
				return m_labelFormat;
			}
			set
			{
				if (m_labelFormat != value)
				{
					m_labelFormat = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the font that is to be used for the label text.
		/// </summary>
		public Font Font
		{
			get
			{
				return m_labelFont;
			}
			set
			{
				if (m_labelFont != value)
				{
					m_labelFont = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color that is to be used for the Grouping Label text.
		/// </summary>
		public Color Color
		{
			get
			{
				return m_labelColor;
			}

			set
			{
				if (m_labelColor != value)
				{
					m_labelColor = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets back color. Default is Transparent.
		/// </summary>
		public Color BackColor
		{
			get
			{
				return m_backColor;
			}
			set
			{
				if (m_backColor != value)
				{
					m_backColor = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the color that is to be used for the Grouping Label border/brace etc. Default is Black.
		/// </summary>
		public Pen BorderPen
		{
			get
			{
				return m_labelBorderPen;
			}

			set
			{
				if (m_labelBorderPen != value)
				{
					m_labelBorderPen = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the Style of border drawing that is to be used for the Grouping Label. Default is Rectangle
		/// </summary>
		public ChartAxisGroupingLabelBorderStyle BorderStyle
		{
			get
			{
				return m_labelBorderStyle;
			}
			set
			{
				if (m_labelBorderStyle != value)
				{
					m_labelBorderStyle = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the Padding of border drawing that is to be used for the Grouping Label. Default is 2.0f.
		/// </summary>
		[DefaultValue(2f)]
		public float BorderPadding
		{
			get
			{
				return m_labelBorderPadding;
			}

			set
			{
				if (m_labelBorderPadding != value)
				{
					m_labelBorderPadding = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets max text width. Default is 200.
		/// </summary>
		public float MaxTextWidth
		{
			get
			{
				return m_labelMaxTextWidth;
			}

			set
			{
				if (m_labelMaxTextWidth != value)
				{
					m_labelMaxTextWidth = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets max text height to width ratio. Default is 2.5
		/// </summary>
		public float MaxTextHeightToWidthRatio
		{
			get
			{
				return m_labelMaxTextHeightToWidthRatio;
			}

			set
			{
				if (m_labelMaxTextHeightToWidthRatio != value)
				{
					m_labelMaxTextHeightToWidthRatio = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets labels rotate angle. Default is 0.
		/// </summary>
		public float RotateAngle
		{
			get
			{
				return m_labelRotateAngle;
			}
			set
			{
				if (m_labelRotateAngle != value)
				{
					m_labelRotateAngle = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets row in which to render this grouping label. specify 0 for the 1st row and so on. Default is 0.
		/// </summary>
		[DefaultValue(0)]
		public int Row
		{
			get
			{
				return m_labelRow;
			}
			set
			{
				if (m_labelRow != value)
				{
					m_labelRow = value;
				}
			}
		}

		/// <summary>
		///Gets or sets grid dimension.
		/// </summary>
		internal float GridDimension
		{
			get
			{
				return m_labelGridDimension;
			}
			set
			{
				if (m_labelGridDimension != value)
				{
					m_labelGridDimension = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets label fit mode. Default is Shrink.
		/// </summary>
		public ChartAxisGroupingLabelTextFitMode LabelTextFitMode
		{
			get
			{
				return m_labelTextFitMode;
			}
			set
			{
				if (m_labelTextFitMode != value)
				{
					m_labelTextFitMode = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets label alignment. Default is Center.
		/// </summary>
		public ChartAxisGroupingLabelTextAlignment LabelTextAlignment
		{
			get
			{
				return m_textAlignment;
			}

			set
			{
				if (m_textAlignment != value)
				{
					m_textAlignment = value;
				}
			}
		}
		/// <summary>
		///Gets the rectangle in which to draw the label.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public RectangleF Rect
		{
			get
			{
				return m_rect;
			}
		}
		//COTDO: This doesn't even seem to be getting used?
		/// <summary>
		/// Gets or sets label broadness(thickness, wideness).
		/// </summary>
		[ EditorBrowsable(EditorBrowsableState.Never), Obsolete ]
		public float Broadness
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
        /// <summary>
        /// Gets or sets tag
        /// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Object Tag
        {
            get
            {
                return m_tag;
            }
            set
            {
                if (m_tag != value)
                {
                    m_tag = value;
                }
            }
        }
		#endregion

		#region Class constructors
		/// <summary>
		/// Overloaded Constructor. Each Grouping Label along a ChartAxis is held in a ChartAxisGroupingLabel.
		/// </summary>
		/// <param name="range">The range to group labels.</param>
		public ChartAxisGroupingLabel(DoubleRange range)
			: this(range, "", Color.Black, new Pen(Color.Black), new Font("Verdana", 10))
		{
		}
		/// <summary>
		/// Constructor. Each Grouping Label along a ChartAxis is held in the ChartAxisGroupingLabel.
		/// </summary>
		/// <param name="range">The range to group labels.</param>
		/// <param name="text">The text that will be displayed as the Grouping Label for the axis point.</param>
		public ChartAxisGroupingLabel(DoubleRange range, string text)
			: this(range, text, Color.Black, new Pen(Color.Black), new Font("Verdana", 10))
		{
		}
		/// <summary>
		/// Constructor. Each Grouping Label along a ChartAxis is held in the ChartAxisGroupingLabel.
		/// </summary>
		/// <param name="range">The range to group labels.</param>
		/// <param name="text">The text that will be displayed as the Grouping Label for the axis point.</param>
		/// <param name="color">The color that is to be used for the Grouping Label text.</param>
		/// <param name="font">The font that is to be used for the Grouping Label text.</param>
		public ChartAxisGroupingLabel(DoubleRange range, string text, Color color, Font font)
			: this(range, text, color, new Pen(color), font)
		{
		}
		/// <summary>
		/// Constructor. Each Grouping Label along a ChartAxis is held in the ChartAxisGroupingLabel.
		/// </summary>
		/// <param name="range">The range to group labels.</param>
		/// <param name="text">The text that will be displayed as the Grouping Label for the axis point.</param>
		/// <param name="color">The color that is to be used for the Grouping Label text.</param>
		/// <param name="borderColor">The border color to render group.</param>
		/// <param name="font">The font that is to be used for the Grouping Label text.</param>
		public ChartAxisGroupingLabel(DoubleRange range, string text, Color color, Color borderColor, Font font)
			: this( range,text, color, new Pen(borderColor), font )
		{
		}
		/// <summary>
		/// Constructor. Each Grouping Label along a ChartAxis is held in the ChartAxisGroupingLabel.
		/// </summary>
		/// <param name="range">The range to group labels.</param>
		/// <param name="text">The text that will be displayed as the Grouping Label for the axis point.</param>
		/// <param name="color">The color that is to be used for the Grouping Label text.</param>
		/// <param name="borderPen">Border <see cref="System.Drawing.Pen"/> to render group border.</param>
		/// <param name="font">The font that is to be used for the Grouping Label text.</param>
		public ChartAxisGroupingLabel(DoubleRange range, string text, Color color, Pen borderPen, Font font)
		{
			m_labelDoubleRange = range;
			m_labelText = text;
			m_labelColor = color;
			m_labelBorderPen = borderPen;
			m_labelFont = font;

			m_labelFormat.Alignment = StringAlignment.Center;
			m_labelFormat.LineAlignment = StringAlignment.Center;
		}
		#endregion

		#region Class methods
		/// <summary>
		/// Render label and calculate its bound rectangle.
		/// </summary>
		/// <param name="g"><see cref="System.Drawing.Graphics"/> to render label.</param>
		/// <param name="position">Location of the label.</param>
		/// <param name="dimention">Dimension of the label.</param>
		/// <param name="axis">The axis label belong to.</param>
		/// <param name="measureDraw"></param>
		/// <returns>Bound rectangle of the label.</returns>
		private RectangleF Draw(Graphics g, float position, float dimention, ChartAxis axis, bool measureDraw)
		{//p - is marginal point of axis, with size of Grouping labels taken into account.
			float opposedCoef = axis.OpposedPosition ? -1 : 1;

			float start = axis.GetCoordinateFromValue(this.Range.Start);
			float end = axis.GetCoordinateFromValue(this.Range.End);

			Matrix mtr = new Matrix(1, 0, 0, 1, 0, 0);

			SizeF textSize = SizeF.Empty, stringSize = SizeF.Empty, labelSize = SizeF.Empty;
			Font font = string.IsNullOrEmpty( m_labelText ) ? null : this.GetTextFontAndSizes(g, axis, out stringSize, out textSize, out labelSize);

			//stringSize - size of unrotated string
			//textSize - size of rotated string
			//labelSize - size of all label with text and border

			if (!measureDraw)
			{
				if (axis.Orientation == ChartOrientation.Horizontal)
				{
					labelSize.Height = dimention;
				}
				else
				{
					labelSize.Width = dimention;
				}
			}

			switch (BorderStyle)
			{
				#region BRACE
				case ChartAxisGroupingLabelBorderStyle.Brace:
					{
						RectangleF borderRect, textRect;
						PointF borderRectCenter;
						PointF p1, p2, p3, p4, p5, p6;

						if (axis.Orientation == ChartOrientation.Horizontal)
						{
							borderRect = new RectangleF(Math.Min(start, end), position - (1 + opposedCoef) / 2 * labelSize.Height, Math.Abs(end-start), labelSize.Height);
							textRect = new RectangleF(borderRect.Left + borderRect.Width / 2 - stringSize.Width / 2, borderRect.Top + borderRect.Height / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
							borderRectCenter = new PointF(borderRect.X + borderRect.Width / 2, borderRect.Y + borderRect.Height / 2);

							p1 = new PointF(borderRect.Left + borderRect.Width / 2 - textSize.Width / 2, borderRect.Top + borderRect.Height / 2);
							p2 = new PointF(borderRect.Left, borderRect.Top + borderRect.Height / 2);
							p3 = new PointF(borderRect.Left, borderRect.Top + borderRect.Height / 2 - opposedCoef * borderRect.Height / 2);

							p4 = new PointF(borderRect.Left + borderRect.Width / 2 + textSize.Width / 2, borderRect.Top + borderRect.Height / 2);
							p5 = new PointF(borderRect.Right, borderRect.Top + borderRect.Height / 2);
							p6 = new PointF(borderRect.Right, borderRect.Top + borderRect.Height / 2 - opposedCoef * borderRect.Height / 2);
						}
						else
						{
							borderRect = new RectangleF(position - (1 - opposedCoef) / 2 * labelSize.Width, Math.Min(start, end), labelSize.Width, Math.Abs(end - start));
							textRect = new RectangleF(borderRect.Left + borderRect.Width / 2 - stringSize.Width / 2, borderRect.Top + borderRect.Height / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
							borderRectCenter = new PointF(borderRect.X + borderRect.Width / 2, borderRect.Y + borderRect.Height / 2);

							p1 = new PointF(borderRect.Left + borderRect.Width / 2, borderRect.Top + borderRect.Height / 2 - textSize.Height / 2);
							p2 = new PointF(borderRect.Left + borderRect.Width / 2, borderRect.Top);
							p3 = new PointF(borderRect.Left + borderRect.Width / 2 + opposedCoef * borderRect.Width / 2, borderRect.Top);

							p4 = new PointF(borderRect.Left + borderRect.Width / 2, borderRect.Top + borderRect.Height / 2 + textSize.Height / 2);
							p5 = new PointF(borderRect.Left + borderRect.Width / 2, borderRect.Bottom);
							p6 = new PointF(borderRect.Left + borderRect.Width / 2 + opposedCoef * borderRect.Width / 2, borderRect.Bottom);
						}

						m_rect = borderRect;

						if (!measureDraw)
						{
							GraphicsContainer cont = DrawingHelper.BeginTransform(g);
							mtr.RotateAt(this.RotateAngle, borderRectCenter);
							g.Transform = mtr;

							g.DrawString(this.Text, font, new SolidBrush(this.Color), textRect, m_labelFormat);
							DrawingHelper.EndTransform(g, cont);

							//Upper Brace
							g.DrawLine(this.BorderPen, p1, p2);
							g.DrawLine(this.BorderPen, p2, p3);
							g.DrawLine(this.BorderPen, p4, p5);
							g.DrawLine(this.BorderPen, p5, p6);
						}
						break;
					}
				#endregion

				#region  RECTANGLE
				case ChartAxisGroupingLabelBorderStyle.Rectangle:
					{
						RectangleF borderRect, textRect;
						PointF borderRectCenter;

						if (axis.Orientation == ChartOrientation.Horizontal)
						{
							borderRect = new RectangleF(new PointF(Math.Min(start, end), position - (1 + opposedCoef) / 2 * labelSize.Height), new SizeF(Math.Abs(end - start), labelSize.Height));
						}
						else
						{
							borderRect = new RectangleF(new PointF(position - (1 - opposedCoef) / 2 * labelSize.Width, Math.Min(start, end)), new SizeF(labelSize.Width, Math.Abs(end - start)));
						}

						float textSizeWidth = Math.Min(textSize.Width, borderRect.Width);
						float textSizeHeight = Math.Min(textSize.Height, borderRect.Height);
						switch (LabelTextAlignment)
						{
							case ChartAxisGroupingLabelTextAlignment.Left:
								textRect = new RectangleF(borderRect.Left + textSizeWidth / 2 - stringSize.Width / 2, borderRect.Top + borderRect.Height / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.Left + textSizeWidth / 2, borderRect.Y + borderRect.Height / 2);
								break;
							case ChartAxisGroupingLabelTextAlignment.Right:
								textRect = new RectangleF(borderRect.Right - textSizeWidth / 2 - stringSize.Width / 2, borderRect.Top + borderRect.Height / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.Right - textSizeWidth / 2, borderRect.Y + borderRect.Height / 2);
								break;
							case ChartAxisGroupingLabelTextAlignment.Top:
								textRect = new RectangleF(borderRect.Left + borderRect.Width / 2 - stringSize.Width / 2, borderRect.Top + textSizeHeight / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.X + borderRect.Width / 2, borderRect.Y + textSizeHeight / 2);
								break;
							case ChartAxisGroupingLabelTextAlignment.Bottom:
								textRect = new RectangleF(borderRect.Left + borderRect.Width / 2 - stringSize.Width / 2, borderRect.Bottom - textSizeHeight / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.X + borderRect.Width / 2, borderRect.Bottom - textSizeHeight / 2);
								break;
							case ChartAxisGroupingLabelTextAlignment.TopLeft:
								textRect = new RectangleF(borderRect.Left + textSizeWidth / 2 - stringSize.Width / 2, borderRect.Top + textSizeHeight / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.X + textSizeWidth / 2, borderRect.Y + textSizeHeight / 2);
								break;
							case ChartAxisGroupingLabelTextAlignment.TopRight:
								textRect = new RectangleF(borderRect.Right - textSizeWidth / 2 - stringSize.Width / 2, borderRect.Top + textSizeHeight / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.Right - textSizeWidth / 2, borderRect.Y + textSizeHeight / 2);
								break;
							case ChartAxisGroupingLabelTextAlignment.BottomLeft:
								textRect = new RectangleF(borderRect.Left + textSizeWidth / 2 - stringSize.Width / 2, borderRect.Bottom - textSizeHeight / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.Left + textSizeWidth / 2, borderRect.Bottom - textSizeHeight / 2);
								break;
							case ChartAxisGroupingLabelTextAlignment.BottomRight:
								textRect = new RectangleF(borderRect.Right - textSizeWidth / 2 - stringSize.Width / 2, borderRect.Bottom - textSizeHeight / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.Right - textSizeWidth / 2, borderRect.Bottom - textSizeHeight / 2);
								break;
							default:
								textRect = new RectangleF(borderRect.Left + borderRect.Width / 2 - stringSize.Width / 2, borderRect.Top + borderRect.Height / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.X + borderRect.Width / 2, borderRect.Y + borderRect.Height / 2);
								break;
						}

						m_rect = borderRect;
						if (!measureDraw)
						{
							using (SolidBrush fillBrush = new SolidBrush(m_backColor))
							{
								g.FillRectangle(fillBrush, borderRect.X, borderRect.Y, borderRect.Width, borderRect.Height);
							}

							GraphicsContainer cont = DrawingHelper.BeginTransform(g);
							mtr.RotateAt(this.RotateAngle, borderRectCenter);
							g.Transform = mtr;

							g.DrawString(this.Text, font, new SolidBrush(this.Color), textRect, m_labelFormat);
							DrawingHelper.EndTransform(g, cont);

							g.DrawRectangle(this.BorderPen, borderRect.X, borderRect.Y, borderRect.Width, borderRect.Height);
						}
						break;
					}
				#endregion

                #region WithoutTopBorder
                case ChartAxisGroupingLabelBorderStyle.WithoutTopBorder:
                    {
                        RectangleF borderRect, textRect;
                        PointF borderRectCenter;
                        PointF p1, p2, p3, p4, p5, p6;

                        if (axis.Orientation == ChartOrientation.Horizontal)
                        {
                            borderRect = new RectangleF(Math.Min(start, end), position - (1 + opposedCoef) / 2 * labelSize.Height, Math.Abs(end - start), labelSize.Height);
                            textRect = new RectangleF(borderRect.Left + borderRect.Width / 2 - stringSize.Width / 2, borderRect.Top + borderRect.Height / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
                            borderRectCenter = new PointF(borderRect.X + borderRect.Width / 2, borderRect.Y + borderRect.Height / 2);

                            p1 = new PointF(borderRect.Left + borderRect.Width - textSize.Width, borderRect.Top + borderRect.Height);
                            p2 = new PointF(borderRect.Left, borderRect.Top + borderRect.Height);
                            p3 = new PointF(borderRect.Left, borderRect.Top + borderRect.Height - opposedCoef * borderRect.Height);

                            p4 = new PointF(borderRect.Left + borderRect.Width + textSize.Width, borderRect.Top + borderRect.Height);
                            p5 = new PointF(borderRect.Right, borderRect.Top + borderRect.Height);
                            p6 = new PointF(borderRect.Right, borderRect.Top + borderRect.Height - opposedCoef * borderRect.Height);
                        }
                        else
                        {
                            borderRect = new RectangleF(position - (1 - opposedCoef) / 2 * labelSize.Width, Math.Min(start, end), labelSize.Width, Math.Abs(end - start));
                            textRect = new RectangleF(borderRect.Left + borderRect.Width / 2 - stringSize.Width / 2, borderRect.Top + borderRect.Height / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
                            borderRectCenter = new PointF(borderRect.X + borderRect.Width / 2, borderRect.Y + borderRect.Height / 2);

                            p1 = new PointF(borderRect.Left + borderRect.Width - textSize.Width, borderRect.Top + borderRect.Height);
                            p2 = new PointF(borderRect.Left, borderRect.Top + borderRect.Height);
                            p3 = new PointF(borderRect.Left, borderRect.Top + borderRect.Height - opposedCoef * borderRect.Height);

                            p4 = new PointF(borderRect.Left + borderRect.Width + textSize.Width, borderRect.Top + borderRect.Height);
                            p5 = new PointF(borderRect.Right, borderRect.Top + borderRect.Height);
                            p6 = new PointF(borderRect.Right, borderRect.Top + borderRect.Height - opposedCoef * borderRect.Height);
                        }

                        m_rect = borderRect;

                        if (!measureDraw)
                        {
                            GraphicsContainer cont = DrawingHelper.BeginTransform(g);
                            mtr.RotateAt(this.RotateAngle, borderRectCenter);
                            g.Transform = mtr;

                            g.DrawString(this.Text, font, new SolidBrush(this.Color), textRect, m_labelFormat);
                            DrawingHelper.EndTransform(g, cont);

                            //Upper Brace
                            if (axis.Orientation == ChartOrientation.Horizontal)
                            {
                                //g.DrawLine(this.BorderPen, p1, p2);
                                g.DrawLine(this.BorderPen, p5, p2);
                                g.DrawLine(this.BorderPen, p2, p3);
                                g.DrawLine(this.BorderPen, p5, p6);
                            }
                            else
                            {
                                //Upper Brace
                                g.DrawLine(this.BorderPen, p5, p2);
                                g.DrawLine(this.BorderPen, p3, p6);
                                g.DrawLine(this.BorderPen, p2, p3);
                            }
                        }
                        break;
                    }
                #endregion

                #region WithoutTopandBottomBorder
                case ChartAxisGroupingLabelBorderStyle.WithoutTopAndBottomBorder:
                    {
                        RectangleF borderRect, textRect;
                        PointF borderRectCenter;
                        PointF p1, p2, p3, p4, p5, p6;

                        if (axis.Orientation == ChartOrientation.Horizontal)
                        {
                            borderRect = new RectangleF(Math.Min(start, end), position - (1 + opposedCoef) / 2 * labelSize.Height, Math.Abs(end - start), labelSize.Height);
                            textRect = new RectangleF(borderRect.Left + borderRect.Width / 2 - stringSize.Width / 2, borderRect.Top + borderRect.Height / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
                            borderRectCenter = new PointF(borderRect.X + borderRect.Width / 2, borderRect.Y + borderRect.Height / 2);

                            p1 = new PointF(borderRect.Left + borderRect.Width - textSize.Width, borderRect.Top + borderRect.Height);
                            p2 = new PointF(borderRect.Left, borderRect.Top + borderRect.Height);
                            p3 = new PointF(borderRect.Left, borderRect.Top + borderRect.Height - opposedCoef * borderRect.Height);

                            p4 = new PointF(borderRect.Left + borderRect.Width + textSize.Width, borderRect.Top + borderRect.Height);
                            p5 = new PointF(borderRect.Right, borderRect.Top + borderRect.Height);
                            p6 = new PointF(borderRect.Right, borderRect.Top + borderRect.Height  - opposedCoef * borderRect.Height);
                        }
                        else
                        {
                            borderRect = new RectangleF(position - (1 - opposedCoef) / 2 * labelSize.Width, Math.Min(start, end), labelSize.Width, Math.Abs(end - start));
                            textRect = new RectangleF(borderRect.Left + borderRect.Width / 2 - stringSize.Width / 2, borderRect.Top + borderRect.Height / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
                            borderRectCenter = new PointF(borderRect.X + borderRect.Width / 2, borderRect.Y + borderRect.Height / 2);

                            p1 = new PointF(borderRect.Left + borderRect.Width - textSize.Width, borderRect.Top + borderRect.Height);
                            p2 = new PointF(borderRect.Left, borderRect.Top + borderRect.Height);
                            p3 = new PointF(borderRect.Left, borderRect.Top + borderRect.Height - opposedCoef * borderRect.Height);

                            p4 = new PointF(borderRect.Left + borderRect.Width + textSize.Width, borderRect.Top + borderRect.Height);
                            p5 = new PointF(borderRect.Right, borderRect.Top + borderRect.Height);
                            p6 = new PointF(borderRect.Right, borderRect.Top + borderRect.Height - opposedCoef * borderRect.Height);
                        }

                        m_rect = borderRect;

                        if (!measureDraw)
                        {
                            GraphicsContainer cont = DrawingHelper.BeginTransform(g);
                            mtr.RotateAt(this.RotateAngle, borderRectCenter);
                            g.Transform = mtr;

                            g.DrawString(this.Text, font, new SolidBrush(this.Color), textRect, m_labelFormat);
                            DrawingHelper.EndTransform(g, cont);

                            if (axis.Orientation == ChartOrientation.Horizontal)
                            {
                                //Upper Brace
                                g.DrawLine(this.BorderPen, p2, p3);
                                g.DrawLine(this.BorderPen, p5, p6);
                            }
                            else
                            {
                                g.DrawLine(this.BorderPen, p5, p2);
                                g.DrawLine(this.BorderPen, p3, p6);

                            }
                        }
                        break;
                    }
                #endregion
                #region RightBorder
                case ChartAxisGroupingLabelBorderStyle.RightBorder:
                    {
                        RectangleF borderRect, textRect;
                        PointF borderRectCenter;
                        PointF p3, p5, p6;

                        if (axis.Orientation == ChartOrientation.Horizontal)
                        {
                            borderRect = new RectangleF(Math.Min(start, end), position - (1 + opposedCoef) / 2 * labelSize.Height, Math.Abs(end - start), labelSize.Height);
                        }
                        else
                        {
                            borderRect = new RectangleF(position - (1 - opposedCoef) / 2 * labelSize.Width, Math.Min(start, end), labelSize.Width, Math.Abs(end - start));
                        }
                        textRect = new RectangleF(borderRect.Left + borderRect.Width / 2 - stringSize.Width / 2, borderRect.Top + borderRect.Height / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
                        borderRectCenter = new PointF(borderRect.X + borderRect.Width / 2, borderRect.Y + borderRect.Height / 2);

                        p3 = new PointF(borderRect.Left, borderRect.Top + borderRect.Height - opposedCoef * borderRect.Height);

                        p5 = new PointF(borderRect.Right, borderRect.Top + borderRect.Height);
                        p6 = new PointF(borderRect.Right, borderRect.Top + borderRect.Height - opposedCoef * borderRect.Height);

                        m_rect = borderRect;
                        
                        drawline(measureDraw, p5, p6, p3, borderRectCenter, g, font, textRect, axis, ChartAxisGroupingLabelBorderStyle.RightBorder);
                        break;
                    }
                #endregion
                #region LeftBorder
                case ChartAxisGroupingLabelBorderStyle.LeftBorder:
                    {
                        RectangleF borderRect, textRect;
                        PointF borderRectCenter;
                        PointF p2, p3, p5;

                        if (axis.Orientation == ChartOrientation.Horizontal)
                        {
                            borderRect = new RectangleF(Math.Min(start, end), position - (1 + opposedCoef) / 2 * labelSize.Height, Math.Abs(end - start), labelSize.Height);
                        }
                        else
                        {
                            borderRect = new RectangleF(position - (1 - opposedCoef) / 2 * labelSize.Width, Math.Min(start, end), labelSize.Width, Math.Abs(end - start));
                        }
                        textRect = new RectangleF(borderRect.Left + borderRect.Width / 2 - stringSize.Width / 2, borderRect.Top + borderRect.Height / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
                        borderRectCenter = new PointF(borderRect.X + borderRect.Width / 2, borderRect.Y + borderRect.Height / 2);

                        p2 = new PointF(borderRect.Left, borderRect.Top + borderRect.Height);
                        p3 = new PointF(borderRect.Left, borderRect.Top + borderRect.Height - opposedCoef * borderRect.Height);

                        p5 = new PointF(borderRect.Right, borderRect.Top + borderRect.Height);

                        m_rect = borderRect;

                        drawline(measureDraw, p2, p3, p5, borderRectCenter, g, font, textRect, axis, ChartAxisGroupingLabelBorderStyle.LeftBorder);
                        break;
                    }
                #endregion
			}
			return m_rect;
		}

        /// <summary>
		/// Draw the left or right border for the Rendered label.
		/// </summary>
        /// <param name="measureDraw">Used to indicate the Measure</param>
		/// <param name="g"><see cref="System.Drawing.Graphics"/> to render label.</param>
        /// <param name="font">Font style for the label text.</param>
        /// <param name="textRect">The text of label.</param>
		/// <param name="axis">The axis label belong to.</param>
        /// <param name="BorderType">Style of the ChartAxis GroupingLabel Border</param>
        private void drawline(bool measureDraw, PointF point1, PointF point2, PointF point3, PointF borderRectCenter, Graphics g, Font font, RectangleF textRect, ChartAxis axis, ChartAxisGroupingLabelBorderStyle BorderType)
        {
            Matrix mtr = new Matrix(1, 0, 0, 1, 0, 0);
            if (!measureDraw)
            {
                GraphicsContainer cont = DrawingHelper.BeginTransform(g);
                mtr.RotateAt(this.RotateAngle, borderRectCenter);
                g.Transform = mtr;

                g.DrawString(this.Text, font, new SolidBrush(this.Color), textRect, m_labelFormat);
                DrawingHelper.EndTransform(g, cont);

                if (axis.Orientation == ChartOrientation.Horizontal)
                {
                    //Upper Brace
                    g.DrawLine(this.BorderPen, point1, point2);
                }
                else
                {
                    if(BorderType == ChartAxisGroupingLabelBorderStyle.LeftBorder)
                        g.DrawLine(this.BorderPen, point3, point1);
                    else
                        g.DrawLine(this.BorderPen, point3, point2);
                }
            }
        }
		/// <summary>
		/// Render label and calculate its bound rectangle.
		/// </summary>
		/// <param name="g">Graphics3D to render label.</param>
		/// <param name="position">Location of the label.</param>
		/// <param name="dimention">Dimension of the label.</param>
		/// <param name="axis">The axis label belong to.</param>
		/// <param name="measureDraw"></param>
		/// <returns>Bound rectangle of the label.</returns>
		private RectangleF Draw(Graphics3D g, float position, float dimention, ChartAxis axis, bool measureDraw)
		{//p - is marginal point of axis, with size of Grouping labels taken into account.
			float opposedCoef = axis.OpposedPosition ? -1 : 1;

			float start = axis.GetCoordinateFromValue(this.Range.Start);
			float end = axis.GetCoordinateFromValue(this.Range.End);

			Matrix mtr = new Matrix(1, 0, 0, 1, 0, 0);
			double cos = Math.Abs(Math.Cos(this.RotateAngle * Math.PI / 180));
			double sin = Math.Abs(Math.Sin(this.RotateAngle * Math.PI / 180));

			SizeF textSize = SizeF.Empty, stringSize = SizeF.Empty, labelSize = SizeF.Empty;
			Font font = string.IsNullOrEmpty(m_labelText) ? null : this.GetTextFontAndSizes(g.Graphics, axis, out stringSize, out textSize, out labelSize);

			if (!measureDraw)
			{
				if (axis.Orientation == ChartOrientation.Horizontal)
				{
					labelSize.Height = dimention;
				}
				else
				{
					labelSize.Width = dimention;
				}
			}

			switch (BorderStyle)
			{
				//BRACE
				case ChartAxisGroupingLabelBorderStyle.Brace:
					{
						RectangleF borderRect, textRect;
						PointF borderRectCenter;
						PointF p1, p2, p3, p4, p5, p6;
						if (axis.Orientation == ChartOrientation.Horizontal)
						{
							borderRect = new RectangleF(Math.Min(start, end), position - (1 + opposedCoef) / 2 * labelSize.Height, Math.Abs(end - start), labelSize.Height);
							textRect = new RectangleF(borderRect.Left + borderRect.Width / 2 - stringSize.Width / 2, borderRect.Top + borderRect.Height / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
							borderRectCenter = new PointF(borderRect.X + borderRect.Width / 2, borderRect.Y + borderRect.Height / 2);

							p1 = new PointF(borderRect.Left + borderRect.Width / 2 - textSize.Width / 2, borderRect.Top + borderRect.Height / 2);
							p2 = new PointF(borderRect.Left, borderRect.Top + borderRect.Height / 2);
							p3 = new PointF(borderRect.Left, borderRect.Top + borderRect.Height / 2 - opposedCoef * borderRect.Height / 2);

							p4 = new PointF(borderRect.Left + borderRect.Width / 2 + textSize.Width / 2, borderRect.Top + borderRect.Height / 2);
							p5 = new PointF(borderRect.Right, borderRect.Top + borderRect.Height / 2);
							p6 = new PointF(borderRect.Right, borderRect.Top + borderRect.Height / 2 - opposedCoef * borderRect.Height / 2);
						}
						else
						{
							borderRect = new RectangleF(position - (1 - opposedCoef) / 2 * labelSize.Width, Math.Min(start, end), labelSize.Width, Math.Abs(end - start));
							textRect = new RectangleF(borderRect.Left + borderRect.Width / 2 - stringSize.Width / 2, borderRect.Top + borderRect.Height / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
							borderRectCenter = new PointF(borderRect.X + borderRect.Width / 2, borderRect.Y + borderRect.Height / 2);

							p1 = new PointF(borderRect.Left + borderRect.Width / 2, borderRect.Top + borderRect.Height / 2 - textSize.Height / 2);
							p2 = new PointF(borderRect.Left + borderRect.Width / 2, borderRect.Top);
							p3 = new PointF(borderRect.Left + borderRect.Width / 2 + opposedCoef * borderRect.Width / 2, borderRect.Top);

							p4 = new PointF(borderRect.Left + borderRect.Width / 2, borderRect.Top + borderRect.Height / 2 + textSize.Height / 2);
							p5 = new PointF(borderRect.Left + borderRect.Width / 2, borderRect.Bottom);
							p6 = new PointF(borderRect.Left + borderRect.Width / 2 + opposedCoef * borderRect.Width / 2, borderRect.Bottom);
						}

						if (!measureDraw)
						{
							GraphicsPath gpText = new GraphicsPath();

							mtr.RotateAt(this.RotateAngle, borderRectCenter);

							gpText.Transform(mtr);
							gpText.AddString(this.Text, font.FontFamily, (int)font.Style,
								RenderingHelper.GetFontSizeInPixels(font), textRect, StringFormat.GenericDefault);

							//Upper Brace
							GraphicsPath grBorder = new GraphicsPath();

							grBorder.AddLine(p1, p2);
							grBorder.AddLine(p2, p3);
							grBorder.AddLine(p3, p2);
							grBorder.CloseFigure();
							grBorder.AddLine(p4, p5);
							grBorder.AddLine(p5, p6);
							grBorder.AddLine(p6, p5);
							grBorder.CloseFigure();

							g.AddPolygon(new Path3DCollect(new Polygon[]{ 
                                                            Path3D.FromGraphicsPath( gpText, 0, new BrushInfo( this.Color )),
                                                            Path3D.FromGraphicsPath( grBorder, 0, this.BorderPen ) }));
						}
						m_rect = borderRect;
						break;
					}

				//RECTANGLE
				case ChartAxisGroupingLabelBorderStyle.Rectangle:
					{
						RectangleF borderRect, textRect;
						PointF borderRectCenter;

						if (axis.Orientation == ChartOrientation.Horizontal)
						{
							borderRect = new RectangleF(new PointF(Math.Min(start, end), position - (1 + opposedCoef) / 2 * labelSize.Height), new SizeF(Math.Abs(end - start), labelSize.Height));
						}
						else
						{
							borderRect = new RectangleF(new PointF(position - (1 - opposedCoef) / 2 * labelSize.Width, Math.Min(start, end)), new SizeF(labelSize.Width, Math.Abs(end - start)));
						}

						float textSizeWidth = Math.Min(textSize.Width, borderRect.Width);
						float textSizeHeight = Math.Min(textSize.Height, borderRect.Height);

						switch (LabelTextAlignment)
						{
							case ChartAxisGroupingLabelTextAlignment.Left:
								textRect = new RectangleF(borderRect.Left + textSizeWidth / 2 - stringSize.Width / 2, borderRect.Top + borderRect.Height / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.Left + textSizeWidth / 2, borderRect.Y + borderRect.Height / 2);
								break;
							case ChartAxisGroupingLabelTextAlignment.Right:
								textRect = new RectangleF(borderRect.Right - textSizeWidth / 2 - stringSize.Width / 2, borderRect.Top + borderRect.Height / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.Right - textSizeWidth / 2, borderRect.Y + borderRect.Height / 2);
								break;
							case ChartAxisGroupingLabelTextAlignment.Top:
								textRect = new RectangleF(borderRect.Left + borderRect.Width / 2 - stringSize.Width / 2, borderRect.Top + textSizeHeight / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.X + borderRect.Width / 2, borderRect.Y + textSizeHeight / 2);
								break;
							case ChartAxisGroupingLabelTextAlignment.Bottom:
								textRect = new RectangleF(borderRect.Left + borderRect.Width / 2 - stringSize.Width / 2, borderRect.Bottom - textSizeHeight / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.X + borderRect.Width / 2, borderRect.Bottom - textSizeHeight / 2);
								break;
							case ChartAxisGroupingLabelTextAlignment.TopLeft:
								textRect = new RectangleF(borderRect.Left + textSizeWidth / 2 - stringSize.Width / 2, borderRect.Top + textSizeHeight / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.X + textSizeWidth / 2, borderRect.Y + textSizeHeight / 2);
								break;
							case ChartAxisGroupingLabelTextAlignment.TopRight:
								textRect = new RectangleF(borderRect.Right - textSizeWidth / 2 - stringSize.Width / 2, borderRect.Top + textSizeHeight / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.Right - textSizeWidth / 2, borderRect.Y + textSizeHeight / 2);
								break;
							case ChartAxisGroupingLabelTextAlignment.BottomLeft:
								textRect = new RectangleF(borderRect.Left + textSizeWidth / 2 - stringSize.Width / 2, borderRect.Bottom - textSizeHeight / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.Left + textSizeWidth / 2, borderRect.Bottom - textSizeHeight / 2);
								break;
							case ChartAxisGroupingLabelTextAlignment.BottomRight:
								textRect = new RectangleF(borderRect.Right - textSizeWidth / 2 - stringSize.Width / 2, borderRect.Bottom - textSizeHeight / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.Right - textSizeWidth / 2, borderRect.Bottom - textSizeHeight / 2);
								break;
							default:
								textRect = new RectangleF(borderRect.Left + borderRect.Width / 2 - stringSize.Width / 2, borderRect.Top + borderRect.Height / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height);
								borderRectCenter = new PointF(borderRect.X + borderRect.Width / 2, borderRect.Y + borderRect.Height / 2);
								break;
						}
						if (!measureDraw)
						{
							GraphicsPath gpText = new GraphicsPath();

							mtr.RotateAt(this.RotateAngle, borderRectCenter);

							gpText.AddString(this.Text, font.FontFamily, (int)font.Style,
								RenderingHelper.GetFontSizeInPixels(font), textRect, StringFormat.GenericDefault);

							gpText.Transform(mtr);

							GraphicsPath grBorder = new GraphicsPath();
							grBorder.AddRectangle(borderRect);
							g.AddPolygon(new Path3DCollect(new Polygon[]{ 
                                                            Path3D.FromGraphicsPath( gpText, 0, new BrushInfo( this.Color )),
                                                            Path3D.FromGraphicsPath( grBorder, 0, this.BorderPen ) }));
						}
						m_rect = borderRect;
						break;
					}
			}
			return m_rect;
		}
		/// <summary>
		/// Calculate size of the label.
		/// </summary>
		/// <param name="g"><see cref="System.Drawing.Graphics"/> to calculate size.</param>
		/// <param name="axis">Axis label belong to.</param>
		/// <returns>Calculated size.</returns>
		internal SizeF GetSize(Graphics g, ChartAxis axis)
		{
			return Draw(g, 0, 0, axis, true).Size;
		}
		/// <summary>
		/// Calculate bounds of the label.
		/// </summary>
		/// <param name="g"><see cref="System.Drawing.Graphics"/> to calculate bounds.</param>
		/// <param name="position">Location of the label.</param>
		/// <param name="axis">Axis label belong to.</param>
		/// <returns>Boundary rectangle.</returns>
		internal RectangleF Draw(Graphics g, float position, ChartAxis axis)
		{
			return Draw(g, position, this.GridDimension, axis, false);
		}
		/// <summary>
		/// Calculate bounds of the label.
		/// </summary>
		/// <param name="g">Graphics3D to calculate bounds.</param>
		/// <param name="position">Location of the label.</param>
		/// <param name="axis">Axis label belong to.</param>
		/// <returns>Boundary rectangle.</returns>
		internal RectangleF Draw(Graphics3D g, float position, ChartAxis axis)
		{
			return Draw(g, position, this.GridDimension, axis, false);
		}
		/// <summary>
		/// Calculates region for given axis.
		/// </summary>
		/// <param name="axis">The axis to get region.</param>
		/// <returns>Region for given axis.</returns>
		internal ChartRegion GetRegion(ChartAxis axis)
		{
			return new ChartRegion(new Region(this.Rect), 
				(axis.Orientation == ChartOrientation.Horizontal) ? ChartRegionType.HorAxisLabel : ChartRegionType.VerAxisLabel, this.Text, this.RegionDescription);
		}
		/// <summary>
		/// Calculates string size, text size and label size.
		/// </summary>
		/// <param name="g"><see cref="System.Drawing.Graphics"/> to calculate values.</param>
		/// <param name="axis">Axis label belong to.</param>
		/// <param name="stringSize">String size to calculate.</param>
		/// <param name="textSize">Text size to calculate.</param>
		/// <param name="labelSize">Label size to calculate.</param>
		/// <returns><see cref="System.Drawing.Font"/> to calculated label.</returns>
		internal Font GetTextFontAndSizes(Graphics g, ChartAxis axis, out SizeF stringSize, out SizeF textSize, out SizeF labelSize)
		{
			float xs = axis.GetCoordinateFromValue(this.Range.Start);
			float xe = axis.GetCoordinateFromValue(this.Range.End);
			float ys = axis.GetCoordinateFromValue(this.Range.Start);
			float ye = axis.GetCoordinateFromValue(this.Range.End);

			float start = axis.GetCoordinateFromValue(this.Range.Start);
			float end = axis.GetCoordinateFromValue(this.Range.End);

			Matrix mtr = new Matrix(1, 0, 0, 1, 0, 0);

			float cos = (float)Math.Abs(Math.Cos(this.RotateAngle * ChartMath.ToRadians));
			float sin = (float)Math.Abs(Math.Sin(this.RotateAngle * ChartMath.ToRadians));
			float width = (float)Math.Min(Math.Ceiling(Math.Abs(end - start)), this.MaxTextWidth) / cos;

			stringSize = SizeF.Empty;
			textSize = SizeF.Empty;
			labelSize = SizeF.Empty;

			float minScale = 0.01f;

			Font font = this.Font.Clone() as Font;
			int num_cycles = 3;
			int num_glob_cycles = 4;

			if (axis.Orientation == ChartOrientation.Horizontal)
			{
				Font tempFont = font;
				float scale = 1.0f;
				for (int n = 0; n < num_glob_cycles; n++)
				{
					tempFont = new Font(font.FontFamily, font.Size * (float)Math.Max(scale, minScale),
						font.Style, font.Unit, font.GdiCharSet);
					//a little unprecision was made here
					double cellSize = double.MaxValue / 2;
					if (this.LabelTextFitMode == ChartAxisGroupingLabelTextFitMode.Wrap ||
						this.LabelTextFitMode == ChartAxisGroupingLabelTextFitMode.WrapAndShrink)
						cellSize = (Math.Abs(xs - xe) - 2 * BorderPadding) / cos;

					stringSize = g.MeasureString(this.Text, tempFont, (int)Math.Min(Math.Abs(cellSize),
						scale * this.MaxTextWidth), m_labelFormat);

					for (int i = 0; i < num_cycles; i++)
						stringSize = g.MeasureString(this.Text, tempFont,
							(int)Math.Min(Math.Abs(cellSize - stringSize.Height * (sin / cos)), this.MaxTextWidth), m_labelFormat);

					textSize = new SizeF((float)Math.Abs(stringSize.Width * cos + stringSize.Height * sin), (float)Math.Abs(stringSize.Width * sin + stringSize.Height * cos));
					labelSize = new SizeF(Math.Abs(xs - xe), Math.Max(textSize.Height + 2 * BorderPadding, m_labelGridDimension));
					if (this.LabelTextFitMode == ChartAxisGroupingLabelTextFitMode.None ||
						this.LabelTextFitMode == ChartAxisGroupingLabelTextFitMode.Wrap || (n == num_glob_cycles - 1)) break;

					if ((textSize.Width < labelSize.Width) && (stringSize.Height < MaxTextHeightToWidthRatio * stringSize.Width))
						break;
					else
					{
						float tscale1 = 1.0f;
						float tscale2 = 1.0f;
						if (!(textSize.Width < labelSize.Width))
							tscale1 = labelSize.Width / textSize.Width;
						if (!(stringSize.Height < MaxTextHeightToWidthRatio * stringSize.Width))
							tscale2 = MaxTextHeightToWidthRatio / (stringSize.Height / stringSize.Width);

						scale = Math.Min(tscale1, tscale2);
					}
				}
				font = tempFont;
			}
			else
			{
				//vertical axis custom label
				Font tempFont = font;
				float scale = 1.0f;
				for (int n = 0; n < num_glob_cycles; n++)
				{
					//if text doesn't fit into cell, then we decrease font size
					tempFont = new Font(font.FontFamily, font.Size * (float)Math.Max(scale, minScale),
						font.Style, font.Unit, font.GdiCharSet);

					double cellSize = double.MaxValue / 2;
					if (this.LabelTextFitMode == ChartAxisGroupingLabelTextFitMode.Wrap ||
						this.LabelTextFitMode == ChartAxisGroupingLabelTextFitMode.WrapAndShrink)
						cellSize = (Math.Abs(ys - ye) - 2 * BorderPadding) / sin;

					//a little unprecision was made here
					stringSize = g.MeasureString(this.Text, tempFont, (int)this.MaxTextWidth, m_labelFormat);
					if (sin != 0)
					{
						for (int i = 0; i < num_cycles; i++)
							stringSize = g.MeasureString(this.Text, tempFont, (int)Math.Min(Math.Abs(cellSize - stringSize.Height * (cos / sin)), scale * this.MaxTextWidth), m_labelFormat);
					}

					textSize = new SizeF((float)Math.Abs(stringSize.Width * cos + stringSize.Height * sin), (float)Math.Abs(stringSize.Width * sin + stringSize.Height * cos));
					labelSize = new SizeF(Math.Max(textSize.Width + 2 * BorderPadding, m_labelGridDimension), Math.Abs(ys - ye));
					if (this.LabelTextFitMode == ChartAxisGroupingLabelTextFitMode.None ||
						this.LabelTextFitMode == ChartAxisGroupingLabelTextFitMode.Wrap || (n == num_glob_cycles - 1)) break;

					if ((textSize.Height < 1.0 * labelSize.Height) && (stringSize.Height < MaxTextHeightToWidthRatio * stringSize.Width))
						break;
					else
					{
						float tscale1 = 1.0f;
						float tscale2 = 1.0f;
						if (!(textSize.Height < 1.0 * labelSize.Height))
							tscale1 = labelSize.Height / textSize.Height;
						if (!(stringSize.Height < MaxTextHeightToWidthRatio * stringSize.Width))
							tscale2 = MaxTextHeightToWidthRatio / (stringSize.Height / stringSize.Width);

						scale = Math.Min(tscale1, tscale2);
					}
				}
				font = tempFont;
			}

			labelSize = new SizeF(textSize.Width + 2 * this.BorderPadding,
				textSize.Height + 2 * this.BorderPadding);

			return font;
		}
		#endregion
	}

	/// <summary>
	/// Enumeration of the different border styles in which the axis grouping label could be drawn.
	/// </summary>
	public enum ChartAxisGroupingLabelBorderStyle
	{
		/// <summary>
		/// A plain rectangle around the grouping label
		/// </summary>
		Rectangle,
		/// <summary>
		/// A brace indicating the range that this grouping label covers
		/// </summary>
		Brace,
        /// <summary>
        /// A plain rectangle around the grouping label without top border
        /// </summary>
        WithoutTopBorder,
        /// <summary>
        /// A plain rectangle around the grouping label without top and bottom border
        /// </summary>
        WithoutTopAndBottomBorder,
        /// <summary>
        /// A plain rectangle around the grouping label without top and bottom border
        /// </summary>
        WithoutBorder,
        /// <summary>
        /// A plain rectangle around the grouping label without border
        /// </summary>
        LeftBorder,
        /// <summary>
        /// A plain rectangle around the grouping label only with left border
        /// </summary>
        RightBorder
        /// <summary>
        /// A plain rectangle around the grouping label only with right border
        /// </summary>
	}

	/// <summary>
	/// Specifies the options for rendering the text in the ChartAxisGroupingLabel.
	/// </summary>
	public enum ChartAxisGroupingLabelTextFitMode
	{
		/// <summary>
		/// No action will be taken
		/// </summary>
		None,
		/// <summary>
		/// The long text will be wrapped if cannot be fit within a single line.
		/// </summary>
		Wrap,
		/// <summary>
		/// The long text will be shrunk to fit the available space.
		/// </summary>
		Shrink,
		/// CTODO: Find out the exactly how WrapAndShrink works - can't tell the difference between this and Wrap
		/// <summary>
		/// 
		/// </summary>
		WrapAndShrink
	}

	/// <summary>
	/// Specifies the alignment options available for rendering the text within a grouping label.
	/// </summary>
	public enum ChartAxisGroupingLabelTextAlignment
	{
		/// <summary>
		/// Centers both vertically and horizontally
		/// </summary>
		Center,
		/// <summary>
		/// 
		/// </summary>
		Left,
		/// <summary>
		/// 
		/// </summary>
		Right,
		/// <summary>
		/// 
		/// </summary>
		Top,
		/// <summary>
		/// 
		/// </summary>
		Bottom,
		/// <summary>
		/// 
		/// </summary>
		TopLeft,
		/// <summary>
		/// 
		/// </summary>
		TopRight,
		/// <summary>
		/// 
		/// </summary>
		BottomLeft,
		/// <summary>
		/// 
		/// </summary>
		BottomRight
	}
}