#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Implements a colored tiling brush.
    /// </summary>
    public sealed class PdfTilingBrush :
        PdfBrush,
        IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Local variable to store rectanble box.
        /// </summary>
        private RectangleF m_box;

        /// <summary>
        /// Local variable to store graphics.
        /// </summary>
        private PdfGraphics m_graphics;

        /// <summary>
        /// Local variable to store brush Stream.
        /// </summary>
        private PdfStream m_brushStream;

        /// <summary>
        /// Local variable to store resources .
        /// </summary>
        private PdfResources m_resources;

        /// <summary>
        /// Local variable to store Stroking.
        /// </summary>
        private bool m_bStroking;

        /// <summary>
        /// Local variable to store the page.
        /// </summary>
        private PdfPage m_page;

        /// <summary>
        /// Local variable to store the tile start location.
        /// </summary>
        private PointF m_location;
        #endregion

        #region Properties
        /// <summary>
        /// Location representing the start position of the tiles.
        /// </summary>
        internal PointF Location
        {
            get
            {
                return m_location;
            }
            set
            {
                m_location = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfTilingBrush"/> class.
        /// </summary>
        /// <param name="rectangle">The boundaries of the smallest brush cell.</param>
        public PdfTilingBrush(RectangleF rectangle)
        {
            m_brushStream = new PdfStream();
            m_resources = new PdfResources();
            m_brushStream[DictionaryProperties.Resources] = m_resources;

            SetBox(rectangle);
            SetObligatoryFields();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfTilingBrush"/> class.
        /// </summary>
        /// <param name="rectangle">The boundaries of the smallest brush cell.</param>
        /// <param name="page">The Current Page Object.</param>
        public PdfTilingBrush(RectangleF rectangle, PdfPage page)
        {
            m_page = page;
            m_brushStream = new PdfStream();
            m_resources = new PdfResources();
            m_brushStream[DictionaryProperties.Resources] = m_resources;

            SetBox(rectangle);
            SetObligatoryFields();
            Graphics.ColorSpace = page.Document.ColorSpace;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfTilingBrush"/> class.
        /// </summary>
        /// <param name="size">The size of the smallest brush cell.</param>
        public PdfTilingBrush(SizeF size)
            : this(new RectangleF(PointF.Empty, size))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfTilingBrush"/> class.
        /// </summary>
        /// <param name="size">The size of the smallest brush cell.</param>
        /// <param name="page">The Current Page Object.</param>
        public PdfTilingBrush(SizeF size, PdfPage page)
            : this(new RectangleF(PointF.Empty, size), page)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfTilingBrush"/> class.
        /// </summary>
        /// <param name="size">The size of the smallest brush cell.</param>
        /// <param name="page">The Current Page Object.</param>
        /// <param name="location">The Tile start location.</param
        internal PdfTilingBrush(RectangleF rectangle, PdfPage page, PointF location)
        {
            m_page = page;
            m_location = location;
            m_brushStream = new PdfStream();
            m_resources = new PdfResources();
            m_brushStream[DictionaryProperties.Resources] = m_resources;

            SetBox(rectangle);
            SetObligatoryFields();
        }

        /// <summary>
        /// Sets the obligatory fields.
        /// </summary>
        private void SetObligatoryFields()
        {
            m_brushStream[DictionaryProperties.PatternType] = new PdfNumber(1); // Tiling brush.
            m_brushStream[DictionaryProperties.PaintType] = new PdfNumber(1); // Coloured.
            m_brushStream[DictionaryProperties.TilingType] = new PdfNumber(1); // Constant spacing.
            m_brushStream[DictionaryProperties.XStep] = new PdfNumber(m_box.Right - m_box.Left);
            m_brushStream[DictionaryProperties.YStep] = new PdfNumber(m_box.Bottom - m_box.Top);           
           
            if (m_page != null && m_location != null)
            {
                //Transform the tile origin to fit the location
                float tileTransform = (m_page.Size.Height % Rectangle.Size.Height) - (Location.Y);
            
                m_brushStream[DictionaryProperties.Matrix] = new PdfArray(new float[] { 1, 0, 0, 1, m_location.X, tileTransform});
            }
        }

        /// <summary>
        /// Sets the BBox coordinates.
        /// </summary>
        /// <param name="box">The box.</param>
        private void SetBox(RectangleF box)
        {
            m_box = box;
            m_brushStream[DictionaryProperties.BBox] = PdfArray.FromRectangle(m_box);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the boundary box of the smallest brush cell.
        /// </summary>
        public RectangleF Rectangle
        {
            get
            {
                return m_box;
            }
        }

        /// <summary>
        /// Gets the size of the smallest brush cell.
        /// </summary>
        public SizeF Size
        {
            get
            {
                return m_box.Size;
            }
        }

        /// <summary>
        /// Gets Graphics context of the brush.
        /// </summary>
        public PdfGraphics Graphics
        {
            get
            {
                if (m_graphics == null)
                {
                    m_graphics = new PdfGraphics(Size, new PdfGraphics.GetResources(GetResources), m_brushStream);
                    m_graphics.InitializeCoordinates();//TranslateTransform( 0, -m_box.Height );
                }

                return m_graphics;
            }
        }

        /// <summary>
        /// Gets the resources dictionary.
        /// </summary>
        internal PdfResources Resources
        {
            get
            {
                return m_resources;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:PdfTilingBrush"/>
        /// is used for stroking operations.
        /// </summary>
        /// <value><c>true</c> if, the brush is for stroking operations; otherwise, <c>false</c>.</value>
        /// <remarks>This property allows to use tiling brush like a pen to draw lines.</remarks>
        internal bool Stroking
        {
            get
            {
                return m_bStroking;
            }

            set
            {
                m_bStroking = value;
            }
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <returns>PDF resource dictionary.</returns>
        private PdfResources GetResources()
        {
            return Resources;
        }
        #endregion

        #region PdfBrush methods
        /// <summary>
        /// Creates a new copy of a brush.
        /// </summary>
        /// <returns>A new instance of the Brush class.</returns>
        public override PdfBrush Clone()
        {
            PdfTilingBrush brush = new PdfTilingBrush(Rectangle, m_page, Location);

            brush.m_brushStream.Data = m_brushStream.Data;
            brush.m_resources = new PdfResources(m_resources);
            brush.m_brushStream[DictionaryProperties.Resources] = brush.m_resources;

            return brush;
        }

        /// <summary>
        /// Monitors the changes of the brush and modify PDF state respectfully..
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="streamWriter">The stream writer.</param>
        /// <param name="getResources">The get resources delegate.</param>
        /// <param name="saveChanges">if set to <c>true</c> the changes should be saved anyway.</param>
        /// <param name="currentColorSpace">The current color space.</param>
        /// <returns>True if the brush was different.</returns>
        /// <remarks>currentColorSpace parameter doesn't have any impact on the output result.</remarks>
        internal override bool MonitorChanges(PdfBrush brush, PdfStreamWriter streamWriter,
            PdfGraphics.GetResources getResources, bool saveChanges, PdfColorSpace currentColorSpace)
        {
            bool diff = false;

            if (brush != this)
            {
                // Set the /Pattern colour space.
                streamWriter.SetColorSpace("Pattern", m_bStroking);
                PdfName name = getResources().GetName(this);
                streamWriter.SetColourWithPattern(null, name, m_bStroking);
                diff = true;
            }

            return diff;
        }

        /// <summary>
        /// Monitors the changes of the brush and modify PDF state respectfully..
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="streamWriter">The stream writer.</param>
        /// <param name="getResources">The get resources delegate.</param>
        /// <param name="saveChanges">if set to <c>true</c> the changes should be saved anyway.</param>
        /// <param name="currentColorSpace">The current color space.</param>
        /// <param name="check">check</param>
        internal override bool MonitorChanges(PdfBrush brush, PdfStreamWriter streamWriter,
        PdfGraphics.GetResources getResources, bool saveChanges, PdfColorSpace currentColorSpace, bool check)
        {
            bool diff = false;

            if (brush != this)
            {
                // Set the /Pattern colour space.
                streamWriter.SetColorSpace("Pattern", m_bStroking);
                // Set the pattern for non-stroking operations.
                PdfName name = getResources().GetName(this);
                streamWriter.SetColourWithPattern(null, name, m_bStroking);
                diff = true;
            }

            return diff;
        }

        /// <summary>
        /// Monitors the changes of the brush and modify PDF state respectfully..
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
            bool diff = false;

            if (brush != this)
            {
                // Set the /Pattern colour space.
                streamWriter.SetColorSpace("Pattern", m_bStroking);
                // Set the pattern for non-stroking operations.
                PdfName name = getResources().GetName(this);
                streamWriter.SetColourWithPattern(null, name, m_bStroking);
                diff = true;
            }

            return diff;
        }

        /// <summary>
        /// Monitors the changes of the brush and modify PDF state respectfully..
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
            bool diff = false;

            if (brush != this)
            {
                streamWriter.SetColorSpace("Pattern", m_bStroking);
                PdfName name = getResources().GetName(this);
                streamWriter.SetColourWithPattern(null, name, m_bStroking);
                diff = true;
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
            // We shouldn't do anything to reset changes.
            // All changes will be reset automatically by setting a new colour space.
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_brushStream;
            }
        }
        #endregion
    }
}
