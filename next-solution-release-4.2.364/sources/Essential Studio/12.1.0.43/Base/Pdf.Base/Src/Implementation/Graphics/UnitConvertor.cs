#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

using Syncfusion.Pdf.Native;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Class allowing to convert different unit metrics. Converting is 
    /// based on Graphics object DPI settings that is why for differ
    /// graphics settings must be created new instance. For example:
    /// printers often has 300 and greater dpi resolution, for compare
    /// default display screen dpi is 96.
    /// </summary>
#if AllowUnsafeCode || NETFX_CORE || WP
    [System.Security.SecurityCritical]
    public class PdfUnitConvertor
#else
    internal class PdfUnitConvertor
#endif
    {
        #region Constants
        /// <summary>
        /// Indicates default horizontal resolution.
        /// </summary>
        internal static readonly float HorizontalResolution = 96f;
        /// <summary>
        /// Indicates default vertical resolution.
        /// </summary>
        internal static readonly float VerticalResolution = 96f;
        /// <summary>
        /// Width, in millimeters, of the physical screen.
        /// </summary>
        internal static readonly float HorizontalSize;
        /// <summary>
        /// Height, in millimeters, of the physical screen.
        /// </summary>
        internal static readonly float VerticalSize;
        /// <summary>
        /// Width, in pixels, of the screen.
        /// </summary>
        internal static readonly float PxHorizontalResolution;
        /// <summary>
        /// Height, in pixels, of the screen.
        /// </summary>
        internal static readonly float PxVerticalResolution;
        #endregion

        #region Fields
        /// <summary>
        /// Matrix for conversations between different numeric systems
        /// </summary>
        private double[] m_proportions;
        #endregion

        #region Constructors
        /// <summary>
        /// Static constructor.
        /// </summary>
        static PdfUnitConvertor()
        {

#if !SILVERLIGHT && !NETFX_CORE && !WP
            IntPtr hdc = GdiApi.CreateDC("DISPLAY", null, null, IntPtr.Zero);

            HorizontalResolution = GdiApi.GetDeviceCaps(hdc, 88 /*LOGPIXELSX*/ );
            VerticalResolution = GdiApi.GetDeviceCaps(hdc, 90 /*LOGPIXELSY*/ );
            HorizontalSize = GdiApi.GetDeviceCaps(hdc, 4 /*HORZSIZE*/ );
            VerticalSize = GdiApi.GetDeviceCaps(hdc, 6 /*VERTSIZE*/ );
            PxHorizontalResolution = GdiApi.GetDeviceCaps(hdc, 8 /*HORZRES*/ );
            PxVerticalResolution = GdiApi.GetDeviceCaps(hdc, 10 /*VERTRES*/ );

            GdiApi.DeleteDC(hdc);
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfUnitConvertor"/> class.
        /// </summary>
        public PdfUnitConvertor()
        {
            UpdateProportions(HorizontalResolution); // Default value is 96 DPI.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UnitConvertor"/> class.
        /// </summary>
        /// <param name="dpi">The dpi.</param>
        public PdfUnitConvertor(float dpi)
        {
            UpdateProportions(dpi);
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Initializes a new instance of the <see cref="T:UnitConvertor"/> class.
        /// </summary>
        /// <param name="g">Graphics for measuring</param>
        public PdfUnitConvertor(System.Drawing.Graphics g)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            // NOTE: the commented part of the code left in case the implemented approach fail.
            //PointF[] points = new PointF[] { new PointF( 1, 1 ) };

            //GraphicsContainer cont = g.BeginContainer(
            //  new System.Drawing.Rectangle( 0, 0, 1, 1 ), new System.Drawing.Rectangle( 0, 0, 1, 1 ),
            //  GraphicsUnit.Pixel );

            //g.PageUnit = GraphicsUnit.Inch;
            //g.TransformPoints( CoordinateSpace.Device, CoordinateSpace.Page, points );
            //g.EndContainer( cont );

            //float PixelPerInch = points[ 0 ].X;

            UpdateProportions(g.DpiX);
        }
#endif
        #endregion

        #region Public methods
        /// <summary>
        /// Converts the value, stored in "from" units, to value in "to" units
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="from">Indicates units to convert from</param>
        /// <param name="to">Indicates units to convert to</param>
        /// <returns>Value stored in "to" units</returns>
        public float ConvertUnits(float value, PdfGraphicsUnit from, PdfGraphicsUnit to)
        {
            return ConvertFromPixels(ConvertToPixels(value, from), to);
        }

        /// <summary>
        /// Converts the value, stored in "from" units, to pixels
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="from">Indicates units to convert from</param>
        /// <returns>Value stored in pixels</returns>
        public float ConvertToPixels(float value, PdfGraphicsUnit from)
        {
            int index = (int)from;
            float result = (float)(value * m_proportions[index]);
            return result;
        }

        /// <summary>
        /// Converts the rectangle location and size to Pixels from specified 
        /// measure units
        /// </summary>
        /// <param name="rect">source rectangle</param>
        /// <param name="from">source rectangle measure units</param>
        /// <returns>Rectangle with Pixels</returns>
        public RectangleF ConvertToPixels(RectangleF rect, PdfGraphicsUnit from)
        {
            float x = ConvertToPixels(rect.X, from);
            float y = ConvertToPixels(rect.Y, from);
            float w = ConvertToPixels(rect.Width, from);
            float h = ConvertToPixels(rect.Height, from);

            return new RectangleF(x, y, w, h);
        }

        /// <summary>
        /// Converts point from specified measure units to pixels
        /// </summary>
        /// <param name="point">source point for convert</param>
        /// <param name="from">measure units</param>
        /// <returns>point in pixels coordinates</returns>
        public PointF ConvertToPixels(PointF point, PdfGraphicsUnit from)
        {
            float x = ConvertToPixels(point.X, from);
            float y = ConvertToPixels(point.Y, from);

            return new PointF(x, y);
        }

        /// <summary>
        /// Converts size from specified measure units to pixels
        /// </summary>
        /// <param name="size">source size</param>
        /// <param name="from">measure units</param>
        /// <returns>size in pixels</returns>
        public SizeF ConvertToPixels(SizeF size, PdfGraphicsUnit from)
        {
            float w = ConvertToPixels(size.Width, from);
            float h = ConvertToPixels(size.Height, from);

            return new SizeF(w, h);
        }

        /// <summary>
        /// Converts value, stored in pixels, to value in "to" units
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="to">Indicates units to convert to</param>
        /// <returns>Value stored in "to" units</returns>
        public float ConvertFromPixels(float value, PdfGraphicsUnit to)
        {
            int index = (int)to;
            float result = (float)(value / m_proportions[index]);
            return result;
        }

        /// <summary>
        /// Converts rectangle in Pixels into rectangle with specified 
        /// measure units
        /// </summary>
        /// <param name="rect">source rectangle in pixels units</param>
        /// <param name="to">convert to units</param>
        /// <returns>output Rectangle in specified units</returns>
        public RectangleF ConvertFromPixels(RectangleF rect, PdfGraphicsUnit to)
        {
            float x = ConvertFromPixels(rect.X, to);
            float y = ConvertFromPixels(rect.Y, to);
            float w = ConvertFromPixels(rect.Width, to);
            float h = ConvertFromPixels(rect.Height, to);

            return new RectangleF(x, y, w, h);
        }

        /// <summary>
        /// Converts rectangle from pixels to specified units
        /// </summary>
        /// <param name="point">point in pixels units</param>
        /// <param name="to">convert to units</param>
        /// <returns>output Point in specified units</returns>
        public PointF ConvertFromPixels(PointF point, PdfGraphicsUnit to)
        {
            float x = ConvertFromPixels(point.X, to);
            float y = ConvertFromPixels(point.Y, to);

            return new PointF(x, y);
        }

        /// <summary>
        /// Converts Size in pixels to size in specified measure units
        /// </summary>
        /// <param name="size">source size</param>
        /// <param name="to">convert to units</param>
        /// <returns>output size in specified measure units</returns>
        public SizeF ConvertFromPixels(SizeF size, PdfGraphicsUnit to)
        {
            float w = ConvertFromPixels(size.Width, to);
            float h = ConvertFromPixels(size.Height, to);

            return new SizeF(w, h);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Update proportions matrix according to Graphics settings
        /// </summary>
        /// <param name="pixelPerInch">The pixel per inch value.</param>
        private void UpdateProportions(float pixelPerInch)
        {
            m_proportions = new double[]
			{
				pixelPerInch / 2.54,  // Centimeter
				pixelPerInch / 6.0,      // Pica
				1,                     // Pixel
				pixelPerInch / 72.0,     // Point
				pixelPerInch,          // Inch
				pixelPerInch / 300.0,    // Document
				pixelPerInch / 25.4  // Millimeter
			};
        }
        #endregion
    }
}
