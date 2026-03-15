#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Encapsulates the line properties of an object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This style is used to create pens for drawing lines. The
    /// creates a pen from the properties contained in the line style object.
    /// </para>
    /// </remarks>
    [Serializable]
    [TypeConverter(typeof(LineStyleConverter))]
    public class LineStyle
        : PropertyContainer
    {
        #region Class members
        private Color m_clrLine;
        private float m_fLineWidth;
        private LineCap m_capEnd;
        private LineJoin m_lineJoin;
        private float m_fMiterLimit;
        private DashStyle m_styleDash;
        private DashCap m_capDash;
        private float m_fDashOffset;
        private float[] m_dashPattern;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="LineStyle"/> class.
        /// </summary>
        public LineStyle()
            : base()
        {
            m_clrLine = CommonUsedValues.FORE_COLOR;
            m_fLineWidth = CommonUsedValues.LINE_WIDTH;
            m_capEnd = LineCap.Flat;
            m_lineJoin = LineJoin.Bevel;
            m_fMiterLimit = 10f;
            m_styleDash = DashStyle.Solid;
            m_capDash = DashCap.Flat;
            m_fDashOffset = 0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineStyle"/> class.
        /// </summary>
        /// <param name="src">The line style..</param>
        public LineStyle(LineStyle src)
            : base(src)
        {
            m_clrLine = src.m_clrLine;
            m_fLineWidth = src.m_fLineWidth;
            m_capEnd = src.m_capEnd;
            m_lineJoin = src.m_lineJoin;
            m_fMiterLimit = src.m_fMiterLimit;
            m_styleDash = src.m_styleDash;
            m_capDash = src.m_capDash;
            m_fDashOffset = src.m_fDashOffset;

            if (src.m_dashPattern != null)
                m_dashPattern = (float[])src.m_dashPattern.Clone();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineStyle"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected LineStyle(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "color":
                        m_clrLine = (Color)info.GetValue("color", typeof(Color));
                        break;
                    case "lineWidth":
                        m_fLineWidth = info.GetSingle("lineWidth");
                        break;
                    case "capEnd":
                        m_capEnd = (LineCap)info.GetValue("capEnd", typeof(LineCap));
                        break;
                    case "lineJoin":
                        m_lineJoin = (LineJoin)info.GetValue("lineJoin", typeof(LineJoin));
                        break;
                    case "miterLimit":
                        m_fMiterLimit = info.GetSingle("miterLimit");
                        break;
                    case "dashStyle":
                        m_styleDash = (DashStyle)info.GetValue("dashStyle", typeof(DashStyle));
                        break;
                    case "dashCap":
                        m_capDash = (DashCap)info.GetValue("dashCap", typeof(DashCap));
                        break;
                    case "dashOffset":
                        m_fDashOffset = info.GetSingle("dashOffset");
                        break;
                    case "dashpattern":
                        m_dashPattern = (float[])info.GetValue("dashpattern", typeof(float[]));
                        break;
                }
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets Color used to draw lines.
        /// </summary>
        [Browsable(true)]
        [Description("Color used to draw lines.")]
        public Color LineColor
        {
            get 
            { 
                return m_clrLine; 
            }
            set
            {
                if (m_clrLine != value && OnPropertyChanging(DPN.LineColor, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.LineColor);
                    //// assign new value
                    m_clrLine = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.LineColor);
                }
            }
        }

        /// <summary>
        /// Gets or sets Width of the pen in logical units.
        /// </summary>
        [Browsable(true)]
        [Description("Width of the pen in logical units.")]
        [DefaultValue(1f)]
        public float LineWidth
        {
            get 
            { 
                return m_fLineWidth; 
            }
            set
            {
                if (m_fLineWidth != value && OnPropertyChanging(DPN.LineWidth, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.LineWidth);
                    //// assign new value
                    m_fLineWidth = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.LineWidth);
                }
            }
        }

        /// <summary>
        /// Gets or sets type of end cap used to draw lines.
        /// </summary>
        [Browsable(true)]
        [Description("Type of end cap used to draw lines.")]
        [DefaultValue(LineCap.Flat)]
        public LineCap EndCap
        {
            get 
            { 
                return m_capEnd; 
            }
            set
            {
                if (m_capEnd != value && OnPropertyChanging(DPN.EndCap, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.EndCap);
                    //// assign new value
                    m_capEnd = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.EndCap);
                }
            }
        }

        /// <summary>
        /// Gets or sets  how lines are joined at corners.
        /// </summary>
        [Browsable(true)]
        [Description("Determines how lines are joined at corners.")]
        [DefaultValue(System.Drawing.Drawing2D.LineJoin.Bevel)]
        public LineJoin LineJoin
        {
            get 
            { 
                return m_lineJoin; 
            }
            set
            {
                if (m_lineJoin != value && OnPropertyChanging(DPN.LineJoin, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.LineJoin);
                    //// assign new value
                    m_lineJoin = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.LineJoin);
                }
            }
        }

        /// <summary>
        /// Gets or sets Miter limit value.
        /// </summary>
        [Browsable(true)]
        [Description("Miter limit value.")]
        [DefaultValue(10f)]
        public float MiterLimit
        {
            get 
            { 
                return m_fMiterLimit; 
            }
            set
            {
                if (m_fMiterLimit != value && OnPropertyChanging(DPN.MiterLimit, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.MiterLimit);
                    //// assign new value
                    m_fMiterLimit = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.MiterLimit);
                }
            }
        }

        /// <summary>
        /// Gets or sets style to use for dashed lines.
        /// </summary>
        [Browsable(true)]
        [Description("Style to use for dashed lines.")]
        [DefaultValue(System.Drawing.Drawing2D.DashStyle.Solid)]
        public DashStyle DashStyle
        {
            get 
            { 
                return m_styleDash; 
            }
            set
            {
                if (m_styleDash != value && OnPropertyChanging(DPN.DashStyle, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.DashStyle);
                    //// assign new value
                    m_styleDash = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.DashStyle);
                }
            }
        }

        /// <summary>
        /// Gets or sets type of cap to use for dashed lines.
        /// </summary>
        [Browsable(true)]
        [Description("Type of cap to use for dashed lines.")]
        [DefaultValue(System.Drawing.Drawing2D.DashCap.Flat)]
        public DashCap DashCap
        {
            get 
            { 
                return m_capDash; 
            }
            set
            {
                if (m_capDash != value && OnPropertyChanging(DPN.DashCap, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.DashCap);
                    //// assign new value
                    m_capDash = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.DashCap);
                }
            }
        }

        /// <summary>
        /// Gets or sets Offset of dashes in dashed lines in logical units.
        /// </summary>
        [Browsable(true)]
        [Description("Offset of dashes in dashed lines in logical units.")]
        [DefaultValue(0f)]
        public float DashOffset
        {
            get 
            { 
                return m_fDashOffset; 
            }
            set
            {
                if (m_fDashOffset != value && OnPropertyChanging(DPN.DashOffset, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.DashOffset);
                    //// assign new value
                    m_fDashOffset = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.DashOffset);
                }
            }
        }

        /// <summary>
        /// Gets or sets an array of custom dashes and spaces.
        /// </summary>
        [Browsable(true)]
        [Description("An array of custom dashes and spaces.")]
        public float[] DashPattern
        {
            get 
            { 
                return m_dashPattern; 
            }
            set
            {
                if (m_dashPattern != value && OnPropertyChanging(DPN.DashPattern, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.DashPattern);
                    //// assign new value
                    m_dashPattern = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.DashPattern);
                }
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Creates a Pen object using the properties contained by the line style.
        /// </summary>
        /// <returns>System.Drawing.Pen object.</returns>
        public Pen CreatePen()
        {
            Pen pen = new Pen(this.LineColor, MeasureUnitsConverter.ToPixelX(this.LineWidth, this.MeasureUnit));
            pen.EndCap = this.EndCap;
            pen.StartCap = this.EndCap;
            pen.LineJoin = this.LineJoin;
            pen.MiterLimit = MeasureUnitsConverter.ToPixelX(this.MiterLimit, this.MeasureUnit);
            pen.DashCap = this.DashCap;

            if (this.DashStyle == DashStyle.Custom && this.DashPattern != null)
                pen.DashPattern = this.DashPattern;

            pen.DashStyle = this.DashStyle;
            pen.DashOffset = MeasureUnitsConverter.ToPixelX(this.DashOffset, this.MeasureUnit);
            return pen;
        }

        /// <summary>
        /// Creates a Pen object using the properties contained by the line style.
        /// </summary>
        /// <param name="padding">Padding to add to width of the pen</param>
        /// <returns>System.Drawing.Pen object.</returns>
        public Pen CreatePen(float padding)
        {
            Pen pen = new Pen(this.LineColor, this.LineWidth + padding);
            pen.EndCap = this.EndCap;
          //  pen.StartCap = this.EndCap;
            pen.LineJoin = this.LineJoin;
            pen.MiterLimit = this.MiterLimit;

            if (this.DashStyle == DashStyle.Custom && this.DashPattern != null)
                pen.DashPattern = this.DashPattern;

            pen.DashStyle = this.DashStyle;
            pen.DashCap = this.DashCap;
            pen.DashOffset = this.DashOffset;
            return pen;
        }

        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns>Property container name.</returns>
        protected override string GetPropertyContainerName()
        {
            return DPN.LineStyle;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new LineStyle(this);
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("color", m_clrLine);
            info.AddValue("lineWidth", m_fLineWidth);
            info.AddValue("capEnd", m_capEnd);
            info.AddValue("lineJoin", m_lineJoin);
            info.AddValue("miterLimit", m_fMiterLimit);
            info.AddValue("dashStyle", m_styleDash);
            info.AddValue("dashCap", m_capDash);
            info.AddValue("dashOffset", m_fDashOffset);

            if (m_dashPattern != null)
                info.AddValue("dashpattern", m_dashPattern);
        }

        /// <summary>
        /// Called when measure units changing.
        /// </summary>
        /// <param name="from">The old value.</param>
        /// <param name="to">The new value.</param>
        protected override void OnMeasureUnitsChanging(MeasureUnits from, MeasureUnits to)
        {
            base.OnMeasureUnitsChanging(from, to);

            m_fLineWidth = MeasureUnitsConverter.ConvertX(m_fLineWidth, from, to);
            m_fMiterLimit = MeasureUnitsConverter.ConvertX(m_fMiterLimit, from, to);
            m_fDashOffset = MeasureUnitsConverter.ConvertX(m_fDashOffset, from, to);
        }

        #endregion
    }
}