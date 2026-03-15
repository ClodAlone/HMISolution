#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using Syncfusion.DocIO.DLS;
#if !(SILVERLIGHT || WP) || SkipSilverlightNamespaces
using System.Drawing;
using System.Drawing.Drawing2D;
#else
#if WINRT
using Syncfusion.DocIO.WinrtHelper;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#else
#if !WP
using System.Drawing;
#endif
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
#endif
#endif



namespace Syncfusion.Layouting
{
    /// <summary>
    /// Known to us measure units.
    /// </summary>
    internal enum PrintUnits
    {
        /// <summary>
        /// Specifies 1/75 inch as the unit of measure.
        /// </summary>
        Display,

        /// <summary>
        /// Specifies the document unit (1/300 inch) as the unit of measure.
        /// </summary>
        Document,

        /// <summary>
        /// Specifies the inch as the unit of measure.
        /// </summary>
        Inch,

        /// <summary>
        /// Specifies the millimeter as the unit of measure.
        /// </summary>
        Millimeter,

        /// <summary>
        /// Specifies the centimeter as the unit of measure.
        /// </summary>
        Centimeter,

        /// <summary>
        /// Specifies a device pixel as the unit of measure.
        /// </summary>
        Pixel,

        /// <summary>
        /// Specifies a printers point (1/72 inch) as the unit of measure.
        /// </summary>
        Point,
        ///<summary>
        ///Specifies the English Metric Units as the unit of measure
        ///</summary>
        EMU
    }
#if (SILVERLIGHT || WP) && !SkipSilverlightNamespaces && !WINRT
    internal class UIDispatcher
    {
        /// <summary>
        /// Executes the action using UIElement.
        /// </summary>
        /// <param name="action">An Action.</param>
        internal static void Execute(Action action)
        {
#if SILVERLIGHT && !SkipSilverlightNamespaces
            action();
        }
#else
            if (HasThreadAccess)
                action();
            else
            {
                System.Threading.ManualResetEvent threadCompleteEvent = new System.Threading.ManualResetEvent(false);
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(
                  delegate
                  {
                      action();
                      threadCompleteEvent.Set();
                  });
                threadCompleteEvent.WaitOne();
                threadCompleteEvent.Close();
            }
        }
        /// <summary>
        /// Gets whether the current thread has UI access.
        /// </summary>
        /// <value>The current thread has UI access.</value>
        internal static bool HasThreadAccess
        {
            get
            {
                bool hasThreadAccess = true;
                try
                {
                    new System.Windows.Controls.TextBlock();
                }
                catch
                {
                    hasThreadAccess = false;
                }
                return hasThreadAccess;
            }
        }
#endif
    }
#endif
    /// <summary>
    /// Class allow to convert differ metrics units. Convert is 
    /// based on Graphics object DPI settings that is why for differ
    /// graphics settings must be created new instance. For example:
    /// printers often has 300 and greater dpi resolution, for compare
    /// default display screen dpi is 96.
    /// </summary>
    internal class UnitsConvertor
    {
        #region Class constants
        /// <summary>
        /// Standart picture DPI.
        /// </summary>
        internal const int STANDART_DPI = 96;
        #endregion

        #region Class members
#if (!SILVERLIGHT && !WP)|| SkipSilverlightNamespaces
        private Graphics _graph;
#endif
        /// <summary>
        /// Matrix for conversations between different numeric systems
        /// </summary>
        private double[] m_Proportions = null;
        [ThreadStatic]
        private static UnitsConvertor m_instance;
        #endregion

        #region ClassProperties
        private double[] Proportions
        {
            get
            {
                if (m_Proportions == null)
                {
                    InitDefProporsions();
                }
                return m_Proportions;
            }
        }
#if (!SILVERLIGHT && !WP) || SkipSilverlightNamespaces
        /// <summary>
        /// Get Empty graphics created on Bitmap
        /// </summary>
        public Graphics EmptyGraphics
        {
            get
            {
                if (_graph == null)
                {
                    Bitmap bmp = new Bitmap(1, 1);
                    _graph = Graphics.FromImage(bmp);
                }
                return _graph;
            }
        }
#endif
        #endregion

        #region Class static properties
        /// <summary>
        /// Gets an instance of units converter.
        /// </summary>
        public static UnitsConvertor Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new UnitsConvertor();
                }

                return m_instance;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Prevent class creation
        /// </summary>
        public UnitsConvertor()
        {
#if SILVERLIGHT || WP
            InitDefProporsions();
#else
            UpdateProportions(EmptyGraphics);
#endif
        }

#if !SILVERLIGHT && !WP
        /// <summary>
        /// Create Units convert class based on specified Graphics units
        /// </summary>
        /// <param name="g">Graphics for measuring</param>
        public UnitsConvertor(Graphics g)
        {
            UpdateProportions(g);
        }
#endif
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Converts value, stored in "from" units, to value in "to" units
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="from">Indicates units to convert from</param>
        /// <param name="to">Indicates units to convert to</param>
        /// <returns>Value stored in "to" units</returns>
        public double ConvertUnits(double value, PrintUnits from, PrintUnits to)
        {
            if (from == to)
                return value;

            return ConvertFromPixels(ConvertToPixels(value, from), to);
        }

        /// <summary>
        /// Converts value, stored in "from" units, to pixels
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="from">Indicates units to convert from</param>
        /// <returns>Value stored in pixels</returns>
        public float ConvertToPixels(float value, PrintUnits from)
        {
            if (from == PrintUnits.Pixel)
                return value;

            return (float)(value * m_Proportions[(int)from]);
        }

        /// <summary>
        /// Converts value, stored in "from" units, to pixels
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="from">Indicates units to convert from</param>
        /// <returns>Value stored in pixels</returns>
        public double ConvertToPixels(double value, PrintUnits from)
        {
            if (from == PrintUnits.Pixel)
                return value;

            return value * m_Proportions[(int)from];
        }

        /// <summary>
        /// Convert rectangle location and size to Pixels from specified 
        /// measure units
        /// </summary>
        /// <param name="rect">source rectangle</param>
        /// <param name="from">source rectangle measure units</param>
        /// <returns>Rectangle with Pixels</returns>
        public RectangleF ConvertToPixels(RectangleF rect, PrintUnits from)
        {
            float x = (float)ConvertToPixels(rect.X, from);
            float y = (float)ConvertToPixels(rect.Y, from);
            float w = (float)ConvertToPixels(rect.Width, from);
            float h = (float)ConvertToPixels(rect.Height, from);

            return new RectangleF(x, y, w, h);
        }

        /// <summary>
        /// Convert point from specified measure units to pixels
        /// </summary>
        /// <param name="point">source point for convert</param>
        /// <param name="from">measure units</param>
        /// <returns>point in pixels coordinates</returns>
        public PointF ConvertToPixels(PointF point, PrintUnits from)
        {
            float x = (float)ConvertToPixels(point.X, from);
            float y = (float)ConvertToPixels(point.Y, from);

            return new PointF(x, y);
        }

        /// <summary>
        /// Convert size from specified measure units to pixels
        /// </summary>
        /// <param name="size">source size</param>
        /// <param name="from">measure units</param>
        /// <returns>size in pixels</returns>
        public SizeF ConvertToPixels(SizeF size, PrintUnits from)
        {
            float w = (float)ConvertToPixels(size.Width, from);
            float h = (float)ConvertToPixels(size.Height, from);

            return new SizeF(w, h);
        }

        /// <summary>
        /// Converts value, stored in pixels, to value in "to" units
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="to">Indicates units to convert to</param>
        /// <returns>Value stored in "to" units</returns>
        public float ConvertFromPixels(float value, PrintUnits to)
        {
            if (to == PrintUnits.Pixel)
                return value;

            return (float)(value / m_Proportions[(int)to]);
        }

        /// <summary>
        /// Converts value, stored in pixels, to value in "to" units
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="to">Indicates units to convert to</param>
        /// <returns>Value stored in "to" units</returns>
        public double ConvertFromPixels(double value, PrintUnits to)
        {
            if (to == PrintUnits.Pixel)
                return value;

            return value / m_Proportions[(int)to];
        }

        /// <summary>
        /// Convert rectangle in Pixels into rectangle with specified 
        /// measure units
        /// </summary>
        /// <param name="rect">source rectangle in pixels units</param>
        /// <param name="to">convert to units</param>
        /// <returns>output Rectangle in specified units</returns>
        public RectangleF ConvertFromPixels(RectangleF rect, PrintUnits to)
        {
            float x = (float)ConvertFromPixels(rect.X, to);
            float y = (float)ConvertFromPixels(rect.Y, to);
            float w = (float)ConvertFromPixels(rect.Width, to);
            float h = (float)ConvertFromPixels(rect.Height, to);

            return new RectangleF(x, y, w, h);
        }

        /// <summary>
        /// Convert rectangle from pixels to specified units
        /// </summary>
        /// <param name="point">point in pixels units</param>
        /// <param name="to">convert to units</param>
        /// <returns>output Point in specified units</returns>
        public PointF ConvertFromPixels(PointF point, PrintUnits to)
        {
            float x = (float)ConvertFromPixels(point.X, to);
            float y = (float)ConvertFromPixels(point.Y, to);

            return new PointF(x, y);
        }

        /// <summary>
        /// Convert Size in pixels to size in specified measure units
        /// </summary>
        /// <param name="size">source size</param>
        /// <param name="to">convert to units</param>
        /// <returns>output size in specified measure units</returns>
        public SizeF ConvertFromPixels(Size size, PrintUnits to)
        {
            float w = (float)ConvertFromPixels(size.Width, to);
            float h = (float)ConvertFromPixels(size.Height, to);

            return new SizeF(w, h);
        }

        /// <summary>
        /// Convert Size in pixels to size in specified measure units
        /// </summary>
        /// <param name="size">source size</param>
        /// <param name="to">convert to units</param>
        /// <returns>output size in specified measure units</returns>
        public SizeF ConvertFromPixels(SizeF size, PrintUnits to)
        {
            float w = (float)ConvertFromPixels(size.Width, to);
            float h = (float)ConvertFromPixels(size.Height, to);

            return new SizeF(w, h);
        }

        /// <summary>
        /// Converts to pixels.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="from">From.</param>
        /// <param name="dpi">The dpi.</param>
        /// <returns></returns>
        public float ConvertToPixels(float value, PrintUnits from, float dpi)
        {
            if (from == PrintUnits.Pixel)
                return value;

            double[] proportions = GetProporsion(dpi);
            return (float)(value * proportions[(int)from]);
        }

        /// <summary>
        /// Converts to pixels.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="from">From.</param>
        /// <param name="dpi">The dpi.</param>
        /// <returns></returns>
        public double ConvertToPixels(double value, PrintUnits from, float dpi)
        {
            if (from == PrintUnits.Pixel)
                return value;

            double[] proportions = GetProporsion(dpi);
            return value * proportions[(int)from];
        }

        /// <summary>
        /// Converts from pixels.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="to">To.</param>
        /// <param name="dpi">The dpi.</param>
        /// <returns></returns>
        public float ConvertFromPixels(float value, PrintUnits to, float dpi)
        {
            if (to == PrintUnits.Pixel)
                return value;

            double[] proportions = GetProporsion(dpi);
            return (float)(value / proportions[(int)to]);
        }

        /// <summary>
        /// Converts from pixels.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="to">To.</param>
        /// <param name="dpi">The dpi.</param>
        /// <returns></returns>
        public double ConvertFromPixels(double value, PrintUnits to, float dpi)
        {
            if (to == PrintUnits.Pixel)
                return value;

            double[] proportions = GetProporsion(dpi);
            return value / proportions[(int)to];
        }

        /// <summary>
        /// Convert Size in pixels to size in specified measure units
        /// </summary>
        /// <param name="size">source size</param>
        /// <param name="to">convert to units</param>
        /// <param name="dpi">The dpi.</param>
        /// <returns>output size in specified measure units</returns>
        public SizeF ConvertFromPixels(SizeF size, PrintUnits to, float dpi)
        {
            float w = (float)ConvertFromPixels(size.Width, to, dpi);
            float h = (float)ConvertFromPixels(size.Height, to, dpi);

            return new SizeF(w, h);
        }

        /// <summary>
        /// Gets the proporsion.
        /// </summary>
        /// <param name="dpi">The dpi.</param>
        /// <returns></returns>
        private double[] GetProporsion(float dpi)
        {
            return new double[]
      {
        dpi / 75,     // Display
        dpi / 300,    // Document
        dpi,          // Inch
        dpi / 25.4f,  // Millimeter
        dpi / 2.54f,  // Centimeter
        1,            // Pixel
        dpi / 72,     // Point
      };
        }

#if (SILVERLIGHT || WP) && !SkipSilverlightNamespaces
        public static SizeF MeasureString(string text, double fontSize, string fontName)
        {
            SizeF size = new SizeF();
            Action action = new Action(
                delegate
                {
                    TextBlock block = new TextBlock();
                    block.Text = text;
                    block.FontSize = (fontSize * 96) / 72;
                    block.FontFamily = new FontFamily(fontName);
#if WINRT
                    block.Measure(new Windows.Foundation.Size(double.MaxValue, double.MaxValue));
#endif
                    size = new SizeF((float)block.ActualWidth, (float)block.ActualHeight);
                });
            UIDispatcher.Execute(action);
            return size;
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        private void InitDefProporsions()
        {
            double DEF_INCH_PIXEL = 96;
            m_Proportions = new double[]
              {
                DEF_INCH_PIXEL / 75,     // Display
                DEF_INCH_PIXEL / 300,    // Document
                DEF_INCH_PIXEL,          // Inch
                DEF_INCH_PIXEL / 25.4f,  // Millimeter
                DEF_INCH_PIXEL / 2.54f,  // Centimeter
                1,                       // Pixel
                DEF_INCH_PIXEL / 72,     // Point
                DEF_INCH_PIXEL / 914400,          // EMU
              };
        }

#if !SILVERLIGHT && !WP
        /// <summary>
        /// Update proportions matrix according to Graphics settings
        /// </summary>
        /// <param name="g">reference to graphics</param>
        private void UpdateProportions(Graphics g)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            System.Drawing.Point[] points = new System.Drawing.Point[] { new System.Drawing.Point(1, 1) };

            GraphicsContainer cont = g.BeginContainer(
              new System.Drawing.RectangleF(0, 0, 1, 1), new System.Drawing.RectangleF(0, 0, 1, 1),
              GraphicsUnit.Pixel);

            g.PageUnit = GraphicsUnit.Inch;
            g.TransformPoints(CoordinateSpace.Device, CoordinateSpace.Page, points);
            g.EndContainer(cont);

            double DEF_INCH_PIXEL = points[0].X;

            m_Proportions = new double[]
    {
      DEF_INCH_PIXEL / 75,     // Display
      DEF_INCH_PIXEL / 300,    // Document
      DEF_INCH_PIXEL,          // Inch
      DEF_INCH_PIXEL / 25.4f,  // Millimeter
      DEF_INCH_PIXEL / 2.54f,  // Centimeter
      1,                       // Pixel
      DEF_INCH_PIXEL / 72,     // Point
      DEF_INCH_PIXEL / 914400,          // EMU
    };
        }
#endif
        #endregion
    }
}
