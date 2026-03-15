#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Text;
using Syncfusion.Pdf.ColorSpace;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// A class defining settings for drawing operations.
    /// </summary>
    public class PdfPen : ICloneable
    {
        #region Fields
        private PdfColor m_color = PdfColor.Empty;
        private float m_dashOffset;
        private float[] m_dashPattern = new float[0];
        private PdfDashStyle m_dashStyle = PdfDashStyle.Solid;
        private PdfLineCap m_lineCap;
        private PdfLineJoin m_lineJoin;
        private float m_width = 1.0f;
        private PdfBrush m_brush;
        private float m_miterLimit = 0.0f;
        private PdfColorSpace m_colorSpace = PdfColorSpace.RGB;
        private PdfExtendedColor m_colorspaces;
        /// <summary>
        /// Indicates if the pen is immutable.
        /// </summary>
        private bool m_bImmutable = false;
        #endregion

        #region Properties
        /// <summary>
        /// Get or set the Colorspace.
        /// </summary>
        internal PdfExtendedColor Colorspaces
        {
            get
            {
                return m_colorspaces;
            }
            set
            {
                m_colorspaces = value;
            }
        }

        /// <summary>
        /// Gets or sets the brush, which specifies the pen behaviour.
        /// </summary>
        /// <remarks>If the brush is set, the color values are ignored,
        /// except for PdfSolidBrush.</remarks>
        public PdfBrush Brush
        {
            get
            {
                PdfBrush brush = (m_brush != null) ? m_brush.Clone() : null;

                if (m_brush != null)
                {
                    ResetStroking(brush);
                }

                return m_brush;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Brush");

                CheckImmutability("Brush");
                SetBrush(value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the pen.
        /// </summary>
        public PdfColor Color
        {
            get
            {
                return m_color;
            }
            set
            {
                CheckImmutability("Color");
                m_color = value;
            }
        }

        /// <summary>
        /// Gets or sets the dash offset of the pen.
        /// </summary>
        public float DashOffset
        {
            get
            {
                return m_dashOffset;
            }
            set
            {
                CheckImmutability("DashOffset");
                m_dashOffset = value;
            }
        }

        /// <summary>
        /// Gets or sets the dash pattern of the pen.
        /// </summary>
        public float[] DashPattern
        {
            get
            {
                return m_dashPattern;
            }
            set
            {
                if (DashStyle == PdfDashStyle.Solid)
                    throw new ArgumentException(
                        "This operation is not allowed. Set Custom dash style to change the pattern.");

                CheckImmutability("DashPattern");
                m_dashPattern = value;
            }
        }

        /// <summary>
        /// Gets or sets the dash style of the pen.
        /// </summary>
        public PdfDashStyle DashStyle
        {
            get
            {
                return m_dashStyle;
            }
            set
            {
                CheckImmutability("DashStyle");

                if (m_dashStyle != value)
                {
                    m_dashStyle = value;

                    switch (m_dashStyle)
                    {
                        case PdfDashStyle.Custom:
                            break;

                        case PdfDashStyle.Dash:
                            m_dashPattern = new float[] { 3, 1 };
                            break;

                        case PdfDashStyle.Dot:
                            m_dashPattern = new float[] { 1, 1 };
                            break;

                        case PdfDashStyle.DashDot:
                            m_dashPattern = new float[] { 3, 1, 1, 1 };
                            break;

                        case PdfDashStyle.DashDotDot:
                            m_dashPattern = new float[] { 3, 1, 1, 1, 1, 1 };
                            break;

                        case PdfDashStyle.Solid:
                        default:
                            m_dashStyle = PdfDashStyle.Solid;
                            m_dashPattern = new float[0];
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the line cap of the pen.
        /// </summary>
        public PdfLineCap LineCap
        {
            get
            {
                return m_lineCap;
            }
            set
            {
                CheckImmutability("LineCap");
                m_lineCap = value;
            }
        }

        /// <summary>
        /// Gets or sets the line join style of the pen.
        /// </summary>
        /// <value>The line join.</value>
        public PdfLineJoin LineJoin
        {
            get
            {
                return m_lineJoin;
            }
            set
            {
                CheckImmutability("LineJoin");
                m_lineJoin = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the pen.
        /// </summary>
        public float Width
        {
            get
            {
                return m_width;
            }
            set
            {
                CheckImmutability("Width");
                m_width = value;
            }
        }

        /// <summary>
        /// Gets or sets the miter limit.
        /// </summary>
        public float MiterLimit
        {
            get
            {
                return m_miterLimit;
            }
            set
            {
                CheckImmutability("MiterLimit");
                m_miterLimit = value;
            }
        }

        internal bool IsImmutable
        {
            get
            {
                return m_bImmutable;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPen"/> class.
        /// </summary>
        /// <remarks>Doesn't change current colour.</remarks>
        private PdfPen()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPen"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public PdfPen(PdfColor color)
        {
            Color = color;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPen"/> class.
        /// </summary>
        /// <param name="color">Color of the pen.</param>
        /// <param name="width">Width of the pen's line.</param>
        public PdfPen(PdfColor color, float width)
            : this(color)
        {
            Width = width;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPen"/> class.
        /// </summary>
        /// <param name="brush">The brush.</param>
        public PdfPen(PdfBrush brush)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            SetBrush(brush);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPen"/> class.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="width">Width of the pen's line.</param>
        public PdfPen(PdfBrush brush, float width)
            : this(brush)
        {
            Width = width;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPen"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="immutable">if set to <c>true</c> the pen is immutable.</param>
        internal PdfPen(PdfColor color, bool immutable)
            : this(color)
        {
            m_bImmutable = immutable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPen"/> class.
        /// </summary>
        /// <param name="color"></param>
        public PdfPen(PdfExtendedColor color)
        {
            PdfColorSpaces cs1 = color.ColorSpace;
            m_colorspaces = color;
            PdfColorSpaces cs = color.ColorSpace;
            m_colorspaces = color;
            if (color is PdfCalRGBColor)
            {
                PdfCalRGBColor cl = color as PdfCalRGBColor;
                m_color = new PdfColor((byte)cl.Red, (byte)cl.Green, (byte)cl.Blue);
            }
            else if (color is PdfCalGrayColor)
            {
                PdfCalGrayColor c1 = color as PdfCalGrayColor;
                m_color = new PdfColor((byte)c1.Gray);
                m_color.Gray = Convert.ToSingle(c1.Gray);
            }
            else if (color is PdfLabColor)
            {
                PdfLabColor cl = color as PdfLabColor;
                m_color = new PdfColor((byte)cl.L, (byte)cl.A, (byte)cl.B);
            }
            else if (color is PdfICCColor)
            {
                PdfICCColor c1 = color as PdfICCColor;
                if (c1.ColorSpaces.AlternateColorSpace is PdfCalGrayColorSpace == true)
                {
                    m_color = new PdfColor((byte)c1.ColorComponents[0]);
                    m_color.Gray = Convert.ToSingle(c1.ColorComponents[0]);
                }
                else if (c1.ColorSpaces.AlternateColorSpace is PdfCalRGBColorSpace == true)
                {
                    m_color = new PdfColor((byte)c1.ColorComponents[0], (byte)c1.ColorComponents[1], (byte)c1.ColorComponents[2]);
                }
                else if (c1.ColorSpaces.AlternateColorSpace is PdfLabColorSpace == true)
                {
                    m_color = new PdfColor((byte)c1.ColorComponents[0], (byte)c1.ColorComponents[1], (byte)c1.ColorComponents[2]);
                }
                else if (c1.ColorSpaces.AlternateColorSpace is PdfDeviceColorSpace == true)
                {
                    PdfDeviceColorSpace temp = c1.ColorSpaces.AlternateColorSpace as PdfDeviceColorSpace;
                    string type = temp.DeviceColorSpaceType.ToString();
                    if (type == "RGB")
                    {
                        m_color = new PdfColor((byte)c1.ColorComponents[0], (byte)c1.ColorComponents[1], (byte)c1.ColorComponents[2]);
                    }
                    else if (type == "GrayScale")
                    {
                        m_color = new PdfColor((byte)c1.ColorComponents[0]);
                        m_color.Gray = Convert.ToSingle(c1.ColorComponents[0]);
                    }
                    else if (type == "CMYK")
                    {
                        m_color = new PdfColor((float)c1.ColorComponents[0], (float)c1.ColorComponents[1], (float)c1.ColorComponents[2], (float)c1.ColorComponents[3]);

                    }
                }
                else
                {
                    m_color = new PdfColor((byte)c1.ColorComponents[0], (byte)c1.ColorComponents[1], (byte)c1.ColorComponents[2]);
                }
            }
            else if (color is PdfSeparationColor)
            {
                PdfSeparationColor c1 = color as PdfSeparationColor;
                m_color.Gray = (float)c1.Tint;
            }
            else if (color is PdfIndexedColor)
            {
                PdfIndexedColor c1 = color as PdfIndexedColor;
                m_color.G = (byte)(c1.SelectColorIndex);
            }
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A new pen with the same properties.</returns>
        public PdfPen Clone()
        {
            PdfPen pen = MemberwiseClone() as PdfPen;
            return pen;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the brush.
        /// </summary>
        /// <param name="brush">The brush.</param>
        private void SetBrush(PdfBrush brush)
        {
            PdfSolidBrush sBrush = brush as PdfSolidBrush;

            if (sBrush != null)
            {
                Color = sBrush.Color;
            }
            else
            {
                m_brush = brush.Clone();

                SetStrokingToBrush(m_brush);
            }
        }

        /// <summary>
        /// Sets the stroking flag to brush.
        /// </summary>
        /// <param name="brush">The brush.</param>
        private void SetStrokingToBrush(PdfBrush brush)
        {
            PdfTilingBrush tBrush = brush as PdfTilingBrush;
            PdfGradientBrush gBrush = brush as PdfGradientBrush;

            if (tBrush != null)
            {
                tBrush.Stroking = true;
            }
            else if (gBrush != null)
            {
                gBrush.Stroking = true;
            }
            else if (!(brush is PdfSolidBrush))
            {
                throw new ArgumentException("Unsupported brush.", "brush");
            }
        }

        /// <summary>
        /// Resets the stroking.
        /// </summary>
        /// <param name="brush">The brush.</param>
        private void ResetStroking(PdfBrush brush)
        {
            PdfTilingBrush tBrush = m_brush as PdfTilingBrush;
            PdfGradientBrush gBrush = m_brush as PdfGradientBrush;

            if (tBrush != null)
            {
                tBrush.Stroking = false;
            }
            else if (gBrush != null)
            {
                gBrush.Stroking = false;
            }
            else
            {
                throw new ArgumentException("Unsupported brush.", "brush");
            }
        }

        /// <summary>
        /// Monitors the changes.
        /// </summary>
        /// <param name="currentPen">The current pen.</param>
        /// <param name="streamWriter">The stream writer.</param>
        /// <param name="getResources">The get resources.</param>
        /// <param name="saveState">if it is save state, set to <c>true</c>.</param>
        /// <param name="currentColorSpace">The current color space.</param>
        /// <param name="matrix">The current transformation matrix.</param>
        /// <returns>True if the pen was different.</returns>
        internal bool MonitorChanges(PdfPen currentPen,
            Syncfusion.Pdf.IO.PdfStreamWriter streamWriter,
            PdfGraphics.GetResources getResources, bool saveState,
            PdfColorSpace currentColorSpace, PdfTransformationMatrix matrix)
        {
            bool diff = false;
            saveState = true;
            if (currentPen == null)
            {
                
                diff = true;
            }

            diff = DashControl(currentPen, saveState, streamWriter);

            if (saveState || (Width != currentPen.Width))
            {
                streamWriter.SetLineWidth(Width);
                diff = true;
            }

            if (saveState || (LineJoin != currentPen.LineJoin))
            {
                streamWriter.SetLineJoin(LineJoin);
                diff = true;
            }

            if (saveState || (LineCap != currentPen.LineCap))
            {
                streamWriter.SetLineCap(LineCap);
                diff = true;
            }

            if (saveState || (MiterLimit != currentPen.MiterLimit))
            {
                float miterLimit = MiterLimit;

                if (miterLimit > 0)
                {
                    streamWriter.SetMiterLimit(miterLimit);
                    diff = true;
                }
            }

            if (saveState || (Color != currentPen.Color)
                || (Brush != currentPen.Brush) || m_colorSpace != currentColorSpace)
            {
                PdfBrush brush = m_brush;

                if (brush != null)
                {
                    PdfBrush b = brush.Clone();
                    SetStrokingToBrush(b);

                    PdfGradientBrush lgb = b as PdfGradientBrush;

                    if (lgb != null)
                    {
                        PdfTransformationMatrix m = lgb.Matrix;

                        if (m != null)
                        {
                            matrix.Multiply(m);
                        }

                        lgb.Matrix = matrix;
                    }

                    PdfBrush oldBrush = (currentPen != null) ? currentPen.Brush : null;

                    diff |= b.MonitorChanges(oldBrush, streamWriter, getResources, saveState, currentColorSpace);
                }
                else
                {
                    if (this.Colorspaces is PdfExtendedColor == false)
                    {
                        streamWriter.SetColorAndSpace(Color, currentColorSpace, true);
                        diff = true;
                    }
                    else
                    {
                        streamWriter.SetColorAndSpace(Color, currentColorSpace, true, true);
                        diff = true;
                    }
                }
            }

            return diff;
        }


        internal bool MonitorChanges(PdfPen currentPen,
            Syncfusion.Pdf.IO.PdfStreamWriter streamWriter,
            PdfGraphics.GetResources getResources, bool saveState,
            PdfColorSpace currentColorSpace, PdfTransformationMatrix matrix, bool iccBased)
        {
            bool diff = false;

            if (currentPen == null)
            {
                saveState = true;
                diff = true;
            }

            diff = DashControl(currentPen, saveState, streamWriter);

            if (saveState || (Width != currentPen.Width))
            {
                streamWriter.SetLineWidth(Width);
                diff = true;
            }

            if (saveState || (LineJoin != currentPen.LineJoin))
            {
                streamWriter.SetLineJoin(LineJoin);
                diff = true;
            }

            if (saveState || (LineCap != currentPen.LineCap))
            {
                streamWriter.SetLineCap(LineCap);
                diff = true;
            }

            if (saveState || (MiterLimit != currentPen.MiterLimit))
            {
                float miterLimit = MiterLimit;

                if (miterLimit > 0)
                {
                    streamWriter.SetMiterLimit(miterLimit);
                    diff = true;
                }
            }

            if (saveState || (Color != currentPen.Color)
                || (Brush != currentPen.Brush) || m_colorSpace != currentColorSpace)
            {
                PdfBrush brush = m_brush;

                if (brush != null)
                {
                    PdfBrush b = brush.Clone();
                    SetStrokingToBrush(b);

                    PdfGradientBrush lgb = b as PdfGradientBrush;

                    if (lgb != null)
                    {
                        PdfTransformationMatrix m = lgb.Matrix;

                        if (m != null)
                        {
                            matrix.Multiply(m);
                        }

                        lgb.Matrix = matrix;
                    }

                    PdfBrush oldBrush = (currentPen != null) ? currentPen.Brush : null;

                    diff |= b.MonitorChanges(oldBrush, streamWriter, getResources, saveState, currentColorSpace);
                }
                else
                {
                    if (this.Colorspaces is PdfExtendedColor == false)
                    {
                        streamWriter.SetColorAndSpace(Color, currentColorSpace, true);
                        diff = true;
                    }
                    else
                    {
                        if (this.Colorspaces is PdfIndexedColor)
                        {
                            streamWriter.SetColorAndSpace(Color, currentColorSpace, true, true, true, true);
                            diff = true;
                        }
                        else
                        {
                            streamWriter.SetColorAndSpace(Color, currentColorSpace, true, true, true);
                            diff = true;
                        }
                    }
                }
            }

            return diff;
        }

        /// <summary>
        /// Gets the pattern.
        /// </summary>
        /// <returns>The initialized pattern.</returns>
        internal float[] GetPattern()
        {
            float[] pattern = DashPattern.Clone() as float[];

            for (int i = 0; i < pattern.Length; ++i)
            {
                pattern[i] *= Width;
            }

            return pattern;
        }

        /// <summary>
        /// Controls the dash style and behaviour of each line.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="saveState">if set to <c>true</c> the state should be changed anyway.</param>
        /// <param name="streamWriter">The stream writer.</param>
        /// <returns>True if the dash is different.</returns>
        private bool DashControl(PdfPen pen, bool saveState, Syncfusion.Pdf.IO.PdfStreamWriter streamWriter)
        {
            if (pen != null)
            {
                saveState |= ((DashOffset != pen.DashOffset | DashPattern != pen.DashPattern |
                    DashStyle != pen.DashStyle | Width != pen.Width));
            }
            else
            {
                saveState = true;
            }

            if (saveState)
            {
                float lineWidth = Width;
                float[] pattern = GetPattern();

                streamWriter.SetLineDashPattern(pattern, DashOffset * lineWidth);
            }

            return saveState;
        }

        /// <summary>
        /// Checks the immutability.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void CheckImmutability(string propertyName)
        {
            if (m_bImmutable)
                throw new ArgumentException("The immutable object can't be changed", propertyName);
        }
        #endregion
    }
}
