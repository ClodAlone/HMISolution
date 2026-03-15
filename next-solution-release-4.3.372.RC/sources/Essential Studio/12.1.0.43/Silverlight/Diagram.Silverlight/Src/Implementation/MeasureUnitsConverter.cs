// <copyright file="MeasureUnitsConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Diagram
{ 
    /// <summary>
    /// Represents the measurement units converter class.
    /// </summary>
    public class MeasureUnitsConverter 
    {
        #region Class members
       
        /// <summary>
        /// Used to get a lock on the object.
        /// </summary>
        private static readonly object m_lock = new object();
      
        /// <summary>
        /// Used to store current x coordinate points.
        /// </summary>
        private static float mDpiX;
      
        /// <summary>
        /// Used to store current y coordinate points.
        /// </summary>
        private static float mDpiY;
      
        /// <summary>
        /// Used to store x converted points.
        /// </summary>
        private static double[] s_proportionsX;
       
        /// <summary>
        /// Used to store y converted points.
        /// </summary>
        private static double[] s_proportionsY;
     
        #endregion

        #region Class initialize/finalize methods

        /// <summary>
        /// Initializes static members of the <see cref="MeasureUnitsConverter"/> class.
        /// </summary>
        static MeasureUnitsConverter()
        {
            System.Windows.Media.Imaging.WriteableBitmap bitmap = new System.Windows.Media.Imaging.WriteableBitmap(1, 1); 
            //bitmap.Pixels.
            //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(1, 1);
            //System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage(bmp);
            //System.Drawing.PointF[] points = new System.Drawing.PointF[] { new System.Drawing.PointF(1, 1) };

            //System.Drawing.Drawing2D.GraphicsContainer cont = gfx.BeginContainer(
            //                                             new System.Drawing.Rectangle(0, 0, 1, 1),
            //                                             new System.Drawing.Rectangle(0, 0, 1, 1),
            //                                             System.Drawing.GraphicsUnit.Pixel);

            //gfx.PageUnit = System.Drawing.GraphicsUnit.Inch;
            //gfx.TransformPoints(System.Drawing.Drawing2D.CoordinateSpace.Device, System.Drawing.Drawing2D.CoordinateSpace.Page, points);
            //gfx.EndContainer(cont);

            double dDpiX = 96.0;//points[0].X;

            s_proportionsX = new double[]
                    {
                        //// GDI
                        1, //// Pixel
                        dDpiX / 72, //// Point
                        dDpiX / 300, //// Document
                        dDpiX / 75, //// Display
                        dDpiX / 16d, //// Sixteenth Inches
                        dDpiX / 8d, //// Eighth Inches
                        dDpiX / 4d, //// Quarter Inches
                        dDpiX / 2d, //// Half Inches
                        dDpiX, //// Inch
                        dDpiX / (1d / 12d), //// Feet
                        dDpiX / (1d / 36d), //// Yards
                        dDpiX / (1d / 63360d), //// Miles
                        dDpiX / 25.4d, //// Millimeter
                        dDpiX / 2.54d, //// Centimeters
                        dDpiX / .0254d, //// Meters
                        dDpiX / .0000254d //// Kilometers
                    };

            double dDpiY = 96.0;//points[0].Y;

                s_proportionsY = new double[]
                    {
                        //// GDI
                        1, // Pixel
                        dDpiY / 72d, //// Point
                        dDpiY / 300d, //// Document
                        dDpiY / 75d, //// Display
                        dDpiY / 16d, //// Sixteenth Inches
                        dDpiY / 8d, //// Eighth Inches
                        dDpiY / 4d, //// Quarter Inches
                        dDpiY / 2d, //// Half Inches
                        dDpiY, //// Inch
                        dDpiY / (1d / 12d), //// Feet
                        dDpiY / (1d / 36d), //// Yards
                        dDpiY / (1d / 63360d), //// Miles
                        dDpiY / 25.4d, //// Millimeter
                        dDpiY / 2.54d, //// Centimeters
                        dDpiY / .0254d, //// Meters
                        dDpiY / .0000254d //// Kilometers
                    };
        }
        #endregion

        #region Class properties

        /// <summary>
        /// Gets or sets the dot per inch value by X axis.
        /// </summary>
        /// <value>The dpi X.</value>
        internal static float DpiX
        {
            get
            {
                return mDpiX;
            }

            set
            {
                lock (m_lock)
                {
                    if (mDpiX != value)
                    {
                        mDpiX = value;

                        UpdateProportionsX(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the dot per inch value by Y axis.
        /// </summary>
        /// <value>The dpi Y.</value>
        internal static float DpiY
        {
            get 
            {
                return mDpiY; 
            }

            set
            {
                lock (m_lock)
                {
                    if (mDpiY != value)
                    {
                        mDpiY = value;

                        UpdateProportionsY(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the measure unit abbreviations.
        /// </summary>
        /// <value>The measure unit abbreviations.</value>
        internal static string[] MeasureUnitAbbreviation
        {
            get
            {
                //// Pixel, Point, Document, Display, Sixteenth Inches, Eighth Inches, Quarter Inches, 
                //// Half Inches, Inch, Foot ,Yards, Miles, Millimeter, Centimeters, Meters, Kilometers
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
        public static double ToPixelX(double value, MeasureUnits from)
        {
            double fValueToReturn = value;

            if (from != MeasureUnits.Pixel)
            {
                lock (m_lock)
                {
                    fValueToReturn = (double)(value * s_proportionsX[(int)from]);
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
        public static double ToPixelY(double value, MeasureUnits from)
        {
            double fValueToReturn = value;

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
        public static double FromPixelX(double value, MeasureUnits to)
        {
            double fValueToReturn = value;

            if (to != MeasureUnits.Pixel)
            {
                lock (m_lock)
                {
                    fValueToReturn = (double)(value / s_proportionsX[(int)to]);
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
        public static double FromPixelY(double value, MeasureUnits to)
        {
            double fValueToReturn = value;

            if (to != MeasureUnits.Pixel)
            {
                lock (m_lock)
                {
                    fValueToReturn = (double)(value / s_proportionsY[(int)to]);
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
        public static double ConvertX(double value, MeasureUnits from, MeasureUnits to)
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
        public static double ConvertY(double value, MeasureUnits from, MeasureUnits to)
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
        public static double Convert(double value, MeasureUnits from, MeasureUnits to)
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
        public static Size Convert(Size value, MeasureUnits from, MeasureUnits to)
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
        public static Point Convert(Point value, MeasureUnits from, MeasureUnits to)
        {
            return (from == to) ? value : FromPixels(ToPixels(value, from), to);
        }

        public static Thickness Convert(Thickness value, MeasureUnits from, MeasureUnits to)
        {
            return (from == to) ? value : FromPixels(ToPixels(value, from), to);
        }

        /*
        /// <summary>
        /// Converts value, stored in "from" units, to value in "to" units
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="from">Indicates units to convert from</param>
        /// <param name="to">Indicates units to convert to</param>
        /// <returns>Value stored in "to" units</returns>
        //public static System.Drawing.Rectangle Convert(System.Drawing.Rectangle value, MeasureUnits from, MeasureUnits to)
        //{
        //    return (from == to) ? value : FromPixels(ToPixels(value, from), to);
        //}

        /// <summary>
        /// Convert rectangle location and size to Pixels from specified 
        /// measure units
        /// </summary>
        /// <param name="rect">source rectangle</param>
        /// <param name="from">source rectangle measure units</param>
        /// <returns>Rectangle with Pixels</returns>
       */

        /// <summary>
        /// Converts the Thickness value to pixels
        /// </summary>
        /// <param name="rect">bounding rectangle.</param>
        /// <param name="from">measure unit to convert from.</param>
        /// <returns>The converted rect.</returns>
        public static Thickness ToPixels(Thickness rect, MeasureUnits from)
        {
            if (from != MeasureUnits.Pixel)
            {
                float x = (float)ToPixelX(rect.Left, from);
                float y = (float)ToPixelY(rect.Top, from);
                float w = (float)ToPixelX(rect.Right, from);
                float h = (float)ToPixelY(rect.Bottom, from);
                rect = new Thickness(x, y, w, h);
            }

            return rect;
        }

        /// <summary>
        /// Converts the Thickness value to pixels
        /// </summary>
        /// <param name="rect">bounding rectangle.</param>
        /// <param name="from">measure unit to convert from.</param>
        /// <returns>The converted rect.</returns>
        public static Rect ToPixels(Rect rect, MeasureUnits from)
        {
            if (from != MeasureUnits.Pixel)
            {
                float x = (float)ToPixelX(rect.Left, from);
                float y = (float)ToPixelY(rect.Top, from);
                float w = (float)ToPixelX(rect.Right, from);
                float h = (float)ToPixelY(rect.Bottom, from);
                rect = new Rect(x, y, w, h);
            }

            return rect;
        }

        /// <summary>
        /// Convert point from specified measure units to pixels
        /// </summary>
        /// <param name="point">source point for convert</param>
        /// <param name="from">measure units</param>
        /// <returns>point in pixels coordinates</returns>
        public static Point ToPixels(Point point, MeasureUnits from)
        {
            if (from != MeasureUnits.Pixel)
            {
                double x = ToPixelX(point.X, from);
                double y = ToPixelY(point.Y, from);
                point = new Point(x, y);
            }

            return point;
        }

        /// <summary>
        /// Convert size from specified measure units to pixels
        /// </summary>
        /// <param name="size">source size</param>
        /// <param name="from">measure units</param>
        /// <returns>size in pixels</returns>
        public static Size ToPixels(Size size, MeasureUnits from)
        {
            if (from != MeasureUnits.Pixel)
            {
                double w = ToPixelX(size.Width, from);
                double h = ToPixelY(size.Height, from);
                size = new Size(w, h);
            }

            return size;
        }

        /// <summary>
        /// To the pixels.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="from">From measurement unit.</param>
        /// <returns>The converted value.</returns>
        public static double ToPixels(double value, MeasureUnits from)
        {
            if (from != MeasureUnits.Pixel)
            {
                double v = ToPixelX(value, from);
                value = v;
            }

            return value;
        }

        /*
        /// <summary>
        /// Convert rectangle in Pixels into rectangle with specified 
        /// measure units
        /// </summary>
        /// <param name="rect">source rectangle in pixels units</param>
        /// <param name="to">convert to units</param>
        /// <returns>output Rectangle in specified units</returns>
       */

        /// <summary>
        /// Converts from pixels to specified unit.
        /// </summary>
        /// <param name="rect">The Rectangle.</param>
        /// <param name="to">The unit to be converted to.</param>
        /// <returns>output Rectangle in specified units</returns>
        public static Thickness FromPixels(Thickness rect, MeasureUnits to)
        {
            if (to != MeasureUnits.Pixel)
            {
                float x = (float)FromPixelX(rect.Left, to);
                float y = (float)FromPixelY(rect.Top, to);
                float w = (float)FromPixelX(rect.Right, to);
                float h = (float)FromPixelY(rect.Bottom, to);
                rect = new Thickness(x, y, w, h);
            }

            return rect;
        }

        /// <summary>
        /// Converts from pixels to specified unit.
        /// </summary>
        /// <param name="rect">The Rectangle.</param>
        /// <param name="to">The unit to be converted to.</param>
        /// <returns>output Rectangle in specified units</returns>
        public static Rect FromPixels(Rect rect, MeasureUnits to)
        {
            if (to != MeasureUnits.Pixel)
            {
                float x = (float)FromPixelX(rect.Left, to);
                float y = (float)FromPixelY(rect.Top, to);
                float w = (float)FromPixelX(rect.Right, to);
                float h = (float)FromPixelY(rect.Bottom, to);
                rect = new Rect(x, y, w, h);
            }

            return rect;
        }

        /// <summary>
        /// Convert rectangle from pixels to specified units
        /// </summary>
        /// <param name="point">point in pixels units</param>
        /// <param name="to">convert to units</param>
        /// <returns>output Point in specified units</returns>
        public static Point FromPixels(Point point, MeasureUnits to)
        {
            if (to != MeasureUnits.Pixel)
            {
                double x = FromPixelX(point.X, to);
                double y = FromPixelY(point.Y, to);
                point = new Point(x, y);
            }

            return point;
        }

        /// <summary>
        /// Converts from pixels to specified unit.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="to">convert to units.</param>
        /// <returns>output value in specified units</returns>
        public static double FromPixels(double value, MeasureUnits to)
        {
            if (to != MeasureUnits.Pixel)
            {
                double v = FromPixelX(value, to);
                value = v;
            }

            return value;
        }

        /// <summary>
        /// Convert Size in pixels to size in specified measure units
        /// </summary>
        /// <param name="size">source size</param>
        /// <param name="to">convert to units</param>
        /// <returns>output size in specified measure units</returns>
        public static Size FromPixels(Size size, MeasureUnits to)
        {
            if (to != MeasureUnits.Pixel)
            {
                double w = FromPixelX(size.Width, to);
                double h = FromPixelY(size.Height, to);
                size = new Size(w, h);
            }

            return size;
        }

        /// <summary>
        /// Get the measure unit abbreviation.
        /// </summary>
        /// <param name="units">The measure units.</param>
        /// <returns>The abbreviation.</returns>
        public static string GetAbbreviation(MeasureUnits units)
        {
            return MeasureUnitAbbreviation[(int)units];
        }

        /*
        /// <summary>
        /// Gets the measure unit from abbreviation.
        /// </summary>
        /// <param name="strAbbreviation">The measure unit abbreviation.</param>
        /// <param name="units">The units.</param>
        /// <returns>true, if converted.</returns>
        //public static bool GetMeasureUnit(string strAbbreviation, out MeasureUnits units)
        //{
        //    bool bSuccess = false;
        //    units = MeasureUnits.Pixel;
        //    int mIndex = System.Array.IndexOf(MeasureUnitAbbreviation, strAbbreviation);

        //    if (mIndex >= 0)
        //    {
        //        units = (MeasureUnits)System.Enum.GetValues(typeof(MeasureUnits)).GetValue(mIndex);
        //        bSuccess = true;
        //    }

        //    return bSuccess;
        //}
        */

        #endregion

        #region Class helper methods
       
        /// <summary>
        /// Update the x proportion.
        /// </summary>
        /// <param name="dDpiX">The x points</param>
        private static void UpdateProportionsX(double dDpiX)
        {
            s_proportionsX = new double[]
                    {
                        //// GDI
                        1, //// Pixel
                        dDpiX / 72d, //// Point
                        dDpiX / 300d, //// Document
                        dDpiX / 75d, //// Display
                        dDpiX / 16d, //// Sixteenth Inches
                        dDpiX / 8d, //// Eighth Inches
                        dDpiX / 4d, //// Quarter Inches
                        dDpiX / 2d, //// Half Inches
                        dDpiX, //// Inch
                        dDpiX / (1d / 12d), //// Feet
                        dDpiX / (1d / 36d), //// Yards
                        dDpiX / (1d / 63360d), //// Miles
                        dDpiX / 25.4d, //// Millimeter
                        dDpiX / 2.54d, //// Centimeters
                        dDpiX / .0254d, //// Meters
                        dDpiX / .000254d //// Kilometers
                    };
        }

        /// <summary>
        /// Update the y proportion.
        /// </summary>
        /// <param name="dDpiY">The y points</param>
        private static void UpdateProportionsY(double dDpiY)
        {
            s_proportionsY = new double[]
                    {
                        //// GDI
                        1, //// Pixel
                        dDpiY / 72d, //// Point
                        dDpiY / 300d, //// Document
                        dDpiY / 75d, //// Display
                        dDpiY / 16d, //// Sixteenth Inches
                        dDpiY / 8d, //// Eighth Inches
                        dDpiY / 4d, //// Quarter Inches
                        dDpiY / 2d, //// Half Inches
                        dDpiY, //// Inch
                        dDpiY / (1d / 12d), //// Feet
                        dDpiY / (1d / 36d), //// Yards
                        dDpiY / (1d / 63360d), //// Miles
                        dDpiY / 25.4d, //// Millimeter
                        dDpiY / 2.54d, //// Centimeters
                        dDpiY / .0254d, //// Meters
                        dDpiY / .000254d //// Kilometers
                    };
        }
        #endregion
    }
}
