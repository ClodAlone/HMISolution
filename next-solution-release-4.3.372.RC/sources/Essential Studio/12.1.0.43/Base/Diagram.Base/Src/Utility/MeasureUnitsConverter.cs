#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// The UnitsConverter class helps convert different metrics units. Conversion is 
    /// based on Graphics object DPI settings. Handles cases such as printers often 
    /// having 300 and greater dpi resolution, while the default screen dpi is 96. 
    /// </summary>
    [Documentation.DocumentationExclude()]
    public class MeasureUnitsConverter
    {
        #region Class members
        private static readonly object m_lock = new object();
        public static float s_fPrevDpiX;
        public static float s_fPrevDpiY;
        private static float s_fDpiX;
        private static float s_fDpiY;
        private static double[] s_proportionsX;
        private static double[] s_proportionsY;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes static members of the <see cref="MeasureUnitsConverter"/> class.
        /// </summary>
        static MeasureUnitsConverter()
        {
            Bitmap bmp = new Bitmap(1, 1);
            Graphics gfx = Graphics.FromImage(bmp);
            PointF[] points = new PointF[] { new PointF(1, 1) };

            GraphicsContainer cont = gfx.BeginContainer(
                          new RectangleF(0, 0, 1, 1),
                          new RectangleF(0, 0, 1, 1),
                          GraphicsUnit.Pixel);

            gfx.PageUnit = GraphicsUnit.Inch;
            gfx.TransformPoints(CoordinateSpace.Device, CoordinateSpace.Page, points);
            gfx.EndContainer(cont);

            double dDpiX = points[0].X;

            s_proportionsX = new double[]
                    {
                        // GDI
                        1, // Pixel
                        dDpiX / 72, // Point
                        dDpiX / 300, // Document
                        dDpiX / 75, // Display
                        dDpiX / 16d, // Sixteenth Inches
                        dDpiX / 8d, // Eighth Inches
                        dDpiX / 4d, // Quarter Inches
                        dDpiX / 2d, // Half Inches
                        dDpiX, // Inch
                        dDpiX / (1d / 12d), // Feet
                        dDpiX / (1d / 36d), // Yards
                        dDpiX / (1d / 63360d), // Miles
                        dDpiX / 25.4d, // Millimeter
                        dDpiX / 2.54d, // Centimeters
                        dDpiX / .0254d, // Meters
                        dDpiX / .0000254d // Kilometers
                    };

            double dDpiY = points[0].Y;

            s_proportionsY = new double[]
                    {
                        // GDI
                        1, // Pixel
                        dDpiY / 72d, // Point
                        dDpiY / 300d, // Document
                        dDpiY / 75d, // Display
                        dDpiY / 16d, // Sixteenth Inches
                        dDpiY / 8d, // Eighth Inches
                        dDpiY / 4d, // Quarter Inches
                        dDpiY / 2d, // Half Inches
                        dDpiY, // Inch
                        dDpiY / (1d / 12d), // Feet
                        dDpiY / (1d / 36d), // Yards
                        dDpiY / (1d / 63360d), // Miles
                        dDpiY / 25.4d, // Millimeter
                        dDpiY / 2.54d, // Centimeters
                        dDpiY / .0254d, // Meters
                        dDpiY / .0000254d // Kilometers
                    };
        }
        #endregion

        #region Class paroperties
        /// <summary>
        /// Gets or sets the dot per inch value by X axis.
        /// </summary>
        /// <value>The dpi X.</value>
        public static float DpiX
        {
            get 
            { 
                return s_fDpiX; 
            }
            set
            {
                lock (m_lock)
                {
                    if (s_fDpiX != value)
                    {
                        s_fDpiX = value;

                        UpdateProportionsX(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the dot per inch value by Y axis.
        /// </summary>
        /// <value>The dpi Y.</value>
        public static float DpiY
        {
            get 
            { 
                return s_fDpiY; 
            }
            set
            {
                lock (m_lock)
                {
                    if (s_fDpiY != value)
                    {
                        s_fDpiY = value;

                        UpdateProportionsY(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the measure unit abbreviations.
        /// </summary>
        /// <value>The measure unit abbreviations.</value>
        public static string[] MeasureUnitAbbreviation
        {
            get
            {
                // Pixel, Point, Document, Display, Sixteenth Inches, Eighth Inches, Quarter Inches, 
                // Half Inches, Inch, Foot ,Yards, Miles, Millimeter, Centimeters, Meters, Kilometers
                return new string[] { "px", "pt", "dc", "ds", "sin", "ein", "qin", "hin", "in", "ft", "yd", "mi", "mm", "cm", "m", "km" };
            }
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Converts value, stored in "from" units, to pixels
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="from">Indicates units to convert from</param>
        /// <returns>Value stored in pixels</returns>
        public static float ToPixelX(float value, MeasureUnits from)
        {
            float fValueToReturn = value;

            if (from != MeasureUnits.Pixel)
            {
                lock (m_lock)
                {
                    fValueToReturn = (float)(value * s_proportionsX[(int)from]);
                }
            }

            return fValueToReturn;
        }

        /// <summary>
        /// Converts value, stored in "from" units, to pixels
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="from">Indicates units to convert from</param>
        /// <returns>Value stored in pixels</returns>
        public static float ToPixelY(float value, MeasureUnits from)
        {
            float fValueToReturn = value;

            if (from != MeasureUnits.Pixel)
            {
                lock (m_lock)
                {
                    fValueToReturn = (float)(value * s_proportionsY[(int)from]);
                }
            }

            return fValueToReturn;
        }

        /// <summary>
        /// Converts value, stored in pixels, to value in "to" units
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="to">Indicates units to convert to</param>
        /// <returns>Value stored in "to" units</returns>
        public static float FromPixelX(float value, MeasureUnits to)
        {
            float fValueToReturn = value;

            if (to != MeasureUnits.Pixel)
            {
                lock (m_lock)
                {
                    fValueToReturn = (float)(value / s_proportionsX[(int)to]);
                }
            }

            return fValueToReturn;
        }

        /// <summary>
        /// Converts value, stored in pixels, to value in "to" units
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="to">Indicates units to convert to</param>
        /// <returns>Value stored in "to" units</returns>
        public static float FromPixelY(float value, MeasureUnits to)
        {
            float fValueToReturn = value;

            if (to != MeasureUnits.Pixel)
            {
                lock (m_lock)
                {
                    fValueToReturn = (float)(value / s_proportionsY[(int)to]);
                }
            }

            return fValueToReturn;
        }

        /// <summary>
        /// Converts value, stored in "from" units, to value in "to" units
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="from">Indicates units to convert from</param>
        /// <param name="to">Indicates units to convert to</param>
        /// <returns>Value stored in "to" units</returns>
        public static float ConvertX(float value, MeasureUnits from, MeasureUnits to)
        {
            return (from == to) ? value : FromPixelX(ToPixelX(value, from), to);
        }

        /// <summary>
        /// Converts value, stored in "from" units, to value in "to" units
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="from">Indicates units to convert from</param>
        /// <param name="to">Indicates units to convert to</param>
        /// <returns>Value stored in "to" units</returns>
        public static float ConvertY(float value, MeasureUnits from, MeasureUnits to)
        {
            return (from == to) ? value : FromPixelY(ToPixelY(value, from), to);
        }

        /// <summary>
        /// Converts value, stored in "from" units, to value in "to" units
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="from">Indicates units to convert from</param>
        /// <param name="to">Indicates units to convert to</param>
        /// <returns>Value stored in "to" units</returns>
        public static float Convert(float value, MeasureUnits from, MeasureUnits to)
        {
            return (from == to) ? value : FromPixelX(ToPixelX(value, from), to);
        }

        /// <summary>
        /// Converts value, stored in "from" units, to value in "to" units
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="from">Indicates units to convert from</param>
        /// <param name="to">Indicates units to convert to</param>
        /// <returns>Value stored in "to" units</returns>
        public static SizeF Convert(SizeF value, MeasureUnits from, MeasureUnits to)
        {
            return (from == to) ? value : FromPixels(ToPixels(value, from), to);
        }

        /// <summary>
        /// Converts value, stored in "from" units, to value in "to" units
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="from">Indicates units to convert from</param>
        /// <param name="to">Indicates units to convert to</param>
        /// <returns>Value stored in "to" units</returns>
        public static PointF Convert(PointF value, MeasureUnits from, MeasureUnits to)
        {
            return (from == to) ? value : FromPixels(ToPixels(value, from), to);
        }

        /// <summary>
        /// Converts value, stored in "from" units, to value in "to" units
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="from">Indicates units to convert from</param>
        /// <param name="to">Indicates units to convert to</param>
        /// <returns>Value stored in "to" units</returns>
        public static RectangleF Convert(RectangleF value, MeasureUnits from, MeasureUnits to)
        {
            return (from == to) ? value : FromPixels(ToPixels(value, from), to);
        }

        /// <summary>
        /// Convert rectangle location and size to Pixels from specified 
        /// measure units
        /// </summary>
        /// <param name="rect">source rectangle</param>
        /// <param name="from">source rectangle measure units</param>
        /// <returns>Rectangle with Pixels</returns>
        public static RectangleF ToPixels(RectangleF rect, MeasureUnits from)
        {
            if (from != MeasureUnits.Pixel)
            {
                float x = ToPixelX(rect.X, from);
                float y = ToPixelY(rect.Y, from);
                float w = ToPixelX(rect.Width, from);
                float h = ToPixelY(rect.Height, from);
                rect = new RectangleF(x, y, w, h);
            }

            return rect;
        }

        /// <summary>
        /// Convert point from specified measure units to pixels
        /// </summary>
        /// <param name="point">source point for convert</param>
        /// <param name="from">measure units</param>
        /// <returns>point in pixels coordinates</returns>
        public static PointF ToPixels(PointF point, MeasureUnits from)
        {
            if (from != MeasureUnits.Pixel)
            {
                float x = ToPixelX(point.X, from);
                float y = ToPixelY(point.Y, from);
                point = new PointF(x, y);
            }

            return point;
        }

        /// <summary>
        /// Convert size from specified measure units to pixels
        /// </summary>
        /// <param name="size">source size</param>
        /// <param name="from">measure units</param>
        /// <returns>size in pixels</returns>
        public static SizeF ToPixels(SizeF size, MeasureUnits from)
        {
            if (from != MeasureUnits.Pixel)
            {
                float w = ToPixelX(size.Width, from);
                float h = ToPixelY(size.Height, from);
                size = new SizeF(w, h);
            }

            return size;
        }

        /// <summary>
        /// Convert rectangle in Pixels into rectangle with specified 
        /// measure units
        /// </summary>
        /// <param name="rect">source rectangle in pixels units</param>
        /// <param name="to">convert to units</param>
        /// <returns>output Rectangle in specified units</returns>
        public static RectangleF FromPixels(RectangleF rect, MeasureUnits to)
        {
            if (to != MeasureUnits.Pixel)
            {
                float x = FromPixelX(rect.X, to);
                float y = FromPixelY(rect.Y, to);
                float w = FromPixelX(rect.Width, to);
                float h = FromPixelY(rect.Height, to);
                rect = new RectangleF(x, y, w, h);
            }

            return rect;
        }

        /// <summary>
        /// Convert rectangle from pixels to specified units
        /// </summary>
        /// <param name="point">point in pixels units</param>
        /// <param name="to">convert to units</param>
        /// <returns>output Point in specified units</returns>
        public static PointF FromPixels(PointF point, MeasureUnits to)
        {
            if (to != MeasureUnits.Pixel)
            {
                float x = FromPixelX(point.X, to);
                float y = FromPixelY(point.Y, to);
                point = new PointF(x, y);
            }

            return point;
        }

        /// <summary>
        /// Convert Size in pixels to size in specified measure units
        /// </summary>
        /// <param name="size">source size</param>
        /// <param name="to">convert to units</param>
        /// <returns>output size in specified measure units</returns>
        public static SizeF FromPixels(SizeF size, MeasureUnits to)
        {
            if (to != MeasureUnits.Pixel)
            {
                float w = FromPixelX(size.Width, to);
                float h = FromPixelY(size.Height, to);
                size = new SizeF(w, h);
            }

            return size;
        }

        /// <summary>
        /// Gets the measure unit abbreviation.
        /// </summary>
        /// <param name="units">The measure units.</param>
        /// <returns>Abbreviated string.</returns>
        public static string GetAbbreviation(MeasureUnits units)
        {
            return MeasureUnitAbbreviation[(int)units];
        }

        /// <summary>
        /// Gets the measure unit from abbreviation.
        /// </summary>
        /// <param name="strAbbreviation">The measure unit abbreviation.</param>
        /// <param name="units">The units.</param>
        /// <returns>true, if get measure unit.</returns>
        public static bool GetMeasureUnit(string strAbbreviation, out MeasureUnits units)
        {
            bool bSuccess = false;
            units = MeasureUnits.Pixel;
            int nIndex = System.Array.IndexOf(MeasureUnitAbbreviation, strAbbreviation);

            if (nIndex >= 0)
            {
                units = (MeasureUnits)System.Enum.GetValues(typeof(MeasureUnits)).GetValue(nIndex);
                bSuccess = true;
            }

            return bSuccess;
        }
        #endregion

        #region Class helper methods
        private static void UpdateProportionsX(double dDpiX)
        {
            s_proportionsX = new double[]
                    {
                        // GDI
                        1, // Pixel
                        dDpiX / 72d, // Point
                        dDpiX / 300d, // Document
                        dDpiX / 75d, // Display
                        dDpiX / 16d, // Sixteenth Inches
                        dDpiX / 8d, // Eighth Inches
                        dDpiX / 4d, // Quarter Inches
                        dDpiX / 2d, // Half Inches
                        dDpiX, // Inch
                        dDpiX / (1d / 12d), // Feet
                        dDpiX / (1d / 36d), // Yards
                        dDpiX / (1d / 63360d), // Miles
                        dDpiX / 25.4d, // Millimeter
                        dDpiX / 2.54d, // Centimeters
                        dDpiX / .0254d, // Meters
                        dDpiX / .000254d // Kilometers
                    };
        }
        private static void UpdateProportionsY(double dDpiY)
        {
            s_proportionsY = new double[]
                    {
                        // GDI
                        1, // Pixel
                        dDpiY / 72d, // Point
                        dDpiY / 300d, // Document
                        dDpiY / 75d, // Display
                        dDpiY / 16d, // Sixteenth Inches
                        dDpiY / 8d, // Eighth Inches
                        dDpiY / 4d, // Quarter Inches
                        dDpiY / 2d, // Half Inches
                        dDpiY, // Inch
                        dDpiY / (1d / 12d), // Feet
                        dDpiY / (1d / 36d), // Yards
                        dDpiY / (1d / 63360d), // Miles
                        dDpiY / 25.4d, // Millimeter
                        dDpiY / 2.54d, // Centimeters
                        dDpiY / .0254d, // Meters
                        dDpiY / .000254d // Kilometers
                    };
        }
        #endregion
    }
}
