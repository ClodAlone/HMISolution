#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

using Syncfusion.Pdf.ColorSpace;
using Syncfusion.Pdf.IO;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents a brush that fills any object with a solid colour.
    /// </summary>
    public sealed class PdfSolidBrush : PdfBrush
    {
        #region Fields
        /// <summary>
        /// The colour of the brush.
        /// </summary>
        private PdfColor m_color;

        /// <summary>
        /// The color space of the brush.
        /// </summary>
        private PdfColorSpace m_colorSpace = PdfColorSpace.RGB;

        /// <summary>
        /// Indicates if the brush is immutable.
        /// </summary>
        private bool m_bImmutable = false;

        /// <summary>
        /// Localvariable to store the Colorspace.
        /// </summary>
        private PdfExtendedColor m_colorspaces;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSolidBrush"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public PdfSolidBrush(PdfColor color)
        {
            m_color = color;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfSolidBrush"/> class.
        /// </summary>
        /// <param name="color">color</param>
        public PdfSolidBrush(PdfExtendedColor color)
        {
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

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfSolidBrush"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="immutable">if set to <c>true</c> the brush is immutable.</param>
        internal PdfSolidBrush(PdfColor color, bool immutable)
            : this(color)
        {
            m_bImmutable = immutable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSolidBrush"/> class.
        /// </summary>
        private PdfSolidBrush()
            : this(new PdfColor(0, 0, 0))
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the color of the brush.
        /// </summary>
        public PdfColor Color
        {
            get
            {
                return m_color;
            }

            set
            {
                if (m_bImmutable)
                {
                    throw new ArgumentException("Can't change immutable object.", "Color");
                }

                m_color = value;
            }
        }

        /// <summary>
        /// Gets or sets the Colorspace.
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
        #endregion

        #region Public methods
        /// <summary>
        /// Monitors the changes of the brush and modify PDF state respectively.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="streamWriter">The stream writer.</param>
        /// <param name="getResources">The get resources delegate.</param>
        /// <param name="saveChanges">if set to <c>true</c> the changes should be saved anyway.</param>
        /// <param name="currentColorSpace">The current color space.</param>
        /// <returns>True if the brush was different.</returns>
        internal override bool MonitorChanges(PdfBrush brush, PdfStreamWriter streamWriter,
            PdfGraphics.GetResources getResources, bool saveChanges, PdfColorSpace currentColorSpace)
        {
            if (streamWriter == null)
            {
                throw new ArgumentNullException("streamWriter");
            }

            if (getResources == null)
            {
                throw new ArgumentNullException("getResources");
            }

            bool diff = false;

            if (brush == null)
            {
                diff = true;
                streamWriter.SetColorAndSpace(m_color, currentColorSpace, false);
            }
            else if (brush != this)
            {
                PdfSolidBrush sBrush = brush as PdfSolidBrush;

                if (sBrush != null)
                {
                    if (sBrush.Color != Color || sBrush.m_colorSpace != currentColorSpace)
                    {
                        diff = true;
                        streamWriter.SetColorAndSpace(m_color, currentColorSpace, false);
                    }
                }
                else
                {
                    brush.ResetChanges(streamWriter);
                    streamWriter.SetColorAndSpace(m_color, currentColorSpace, false);
                    diff = true;
                }
            }

            return diff;
        }

        /// <summary>
        /// Monitors the changes of the brush and modify PDF state respectively.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="streamWriter">The stream writer.</param>
        /// <param name="getResources">The get resources delegate.</param>
        /// <param name="saveChanges">if set to <c>true</c> the changes should be saved anyway.</param>
        /// <param name="currentColorSpace">The current color space.</param>
        /// <param name="check">check</param>
        /// <returns>True if the brush was different.</returns>
        internal override bool MonitorChanges(PdfBrush brush, PdfStreamWriter streamWriter,
            PdfGraphics.GetResources getResources, bool saveChanges, PdfColorSpace currentColorSpace, bool check)
        {
            if (streamWriter == null)
            {
                throw new ArgumentNullException("streamWriter");
            }

            if (getResources == null)
            {
                throw new ArgumentNullException("getResources");
            }

            bool diff = false;

            if (brush == null)
            {
                diff = true;
                streamWriter.SetColorAndSpace(m_color, currentColorSpace, false, false);
            }
            else if (brush != this)
            {
                PdfSolidBrush sBrush = brush as PdfSolidBrush;

                if (sBrush != null)
                {
                    if (sBrush.Color != Color || sBrush.m_colorSpace != currentColorSpace)
                    {
                        diff = true;
                        streamWriter.SetColorAndSpace(m_color, currentColorSpace, false, false);
                    }
                }
                else
                {
                    brush.ResetChanges(streamWriter);
                    streamWriter.SetColorAndSpace(m_color, currentColorSpace, false);
                    diff = true;
                }
            }

            return diff;
        }

        /// <summary>
        /// Monitors the changes of the brush and modify PDF state respectively.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="streamWriter">The stream writer.</param>
        /// <param name="getResources">The get resources delegate.</param>
        /// <param name="saveChanges">if set to <c>true</c> the changes should be saved anyway.</param>
        /// <param name="currentColorSpace">The current color space.</param>
        /// <param name="check">check</param>
        /// <param name="iccbased">Indicates the IccBased Color Space.</param>
        /// <returns>True if the brush was different.</returns>
        internal override bool MonitorChanges(PdfBrush brush, PdfStreamWriter streamWriter,
           PdfGraphics.GetResources getResources, bool saveChanges, PdfColorSpace currentColorSpace, bool check, bool iccbased)
        {
            if (streamWriter == null)
            {
                throw new ArgumentNullException("streamWriter");
            }

            if (getResources == null)
            {
                throw new ArgumentNullException("getResources");
            }

            bool diff = false;

            if (brush == null)
            {
                diff = true;
                streamWriter.SetColorAndSpace(m_color, currentColorSpace, false, false, false);
            }
            else if (brush != this)
            {
                PdfSolidBrush sBrush = brush as PdfSolidBrush;

                if (sBrush != null)
                {
                    if (sBrush.Color != Color || sBrush.m_colorSpace != currentColorSpace)
                    {
                        diff = true;
                        streamWriter.SetColorAndSpace(m_color, currentColorSpace, false, false, false);
                    }
                }
                else
                {
                    brush.ResetChanges(streamWriter);
                    streamWriter.SetColorAndSpace(m_color, currentColorSpace, false);
                    diff = true;
                }
            }

            return diff;
        }

        /// <summary>
        /// Monitors the changes of the brush and modify PDF state respectively.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="streamWriter">The stream writer.</param>
        /// <param name="getResources">The get resources delegate.</param>
        /// <param name="saveChanges">if set to <c>true</c> the changes should be saved anyway.</param>
        /// <param name="currentColorSpace">The current color space.</param>
        /// <param name="check">check</param>
        /// <param name="iccbased">Indicates the IccBased Color Space.</param>
        /// <param name="indexed">Indicates the indexed Color Space.</param>
        /// <returns>True if the brush was different.</returns>
        internal override bool MonitorChanges(PdfBrush brush, PdfStreamWriter streamWriter,
           PdfGraphics.GetResources getResources, bool saveChanges, PdfColorSpace currentColorSpace, bool check, bool iccbased, bool indexed)
        {
            if (streamWriter == null)
            {
                throw new ArgumentNullException("streamWriter");
            }

            if (getResources == null)
            {
                throw new ArgumentNullException("getResources");
            }

            bool diff = false;

            if (brush == null)
            {
                diff = true;
                streamWriter.SetColorAndSpace(m_color, currentColorSpace, false, false, false, false);
            }
            else if (brush != this)
            {
                PdfSolidBrush sBrush = brush as PdfSolidBrush;

                if (sBrush != null)
                {
                    if (sBrush.Color != Color || sBrush.m_colorSpace != currentColorSpace)
                    {
                        diff = true;
                        streamWriter.SetColorAndSpace(m_color, currentColorSpace, false, false, false, false);
                    }
                }
                else
                {
                    brush.ResetChanges(streamWriter);
                    streamWriter.SetColorAndSpace(m_color, currentColorSpace, false);
                    diff = true;
                }
            }

            return diff;
        }

        /// <summary>
        /// Resets the changes, which were made by the brush.
        /// In other words resets the state to the initial one.
        /// </summary>
        /// <param name="streamWriter">The stream writer.</param>
        internal override void ResetChanges(PdfStreamWriter streamWriter)
        {
            streamWriter.SetColorAndSpace(new PdfColor(0, 0, 0), PdfColorSpace.RGB, false);
        }
        #endregion

        #region IClonable
        /// <summary>
        /// Creates a new copy of a brush.
        /// </summary>
        /// <returns>A new instance of the Brush class.</returns>
        public override PdfBrush Clone()
        {
            PdfSolidBrush brush = MemberwiseClone() as PdfSolidBrush;

            return brush;
        }
        #endregion
    }
}
