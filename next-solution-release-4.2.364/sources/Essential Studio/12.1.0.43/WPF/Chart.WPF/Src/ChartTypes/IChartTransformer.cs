// <copyright file="IChartTransformer.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents IChartTransformer
    /// </summary>
    /// <exclude/>
    public interface IChartTransformer
    {
        /// <summary>
        /// Gets the viewport.
        /// </summary>
        /// <value>The viewport.</value>
        /// <exclude/>
        Rect Viewport { get; }
        
        /// <summary>
        /// Transforms chart cordinates to real coordinates.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <returns>Visible point</returns>
        Point TransformToVisible(double x, double y);

        /// <summary>
        /// Transforms chart cordinates to real coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        Point3D TransformToVisible(double x, double y, double z);
    }

    /// <summary>
    /// Represents ChartTransform
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public static class ChartTransform
    {
        #region Internal types
        /// <summary>
        /// Represents ChartSimpleTransformer
        /// </summary>
        private class ChartSimpleTransformer : IChartTransformer
        {
            #region Members
            /// <summary>
            /// Initializes m_viewport
            /// </summary>
            private Rect m_viewport = Rect.Empty;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the viewport.
            /// </summary>
            /// <value>The viewport.</value>
            public Rect Viewport
            {
                get
                {
                    return m_viewport;
                }
            }
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="ChartSimpleTransformer"/> class.
            /// </summary>
            /// <param name="viewport">The viewport.</param>
            public ChartSimpleTransformer(Rect viewport)
            {
                m_viewport = viewport;
            }
            #endregion

            #region Public methods
            /// <summary>
            /// Transforms chart cordinates to real coordinates.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            /// <returns>visible point</returns>
            public Point TransformToVisible(double x, double y)
            {
                return new Point(x, y);
            }
            #endregion


            public Point3D TransformToVisible(double x, double y, double z)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Represents ChartCartesianTransformer
        /// </summary>
        private class ChartCartesianTransformer : IChartTransformer
        {
            #region Members
            /// <summary>
            /// Initializes m_viewport
            /// </summary>
            private Rect m_viewport = Rect.Empty;

            /// <summary>
            /// Initializes m_xAxis
            /// </summary>
            private ChartAxis m_xAxis;

            /// <summary>
            /// Initializes m_yAxis
            /// </summary>
            private ChartAxis m_yAxis;

            private ChartAxis m_zAxis;

            /// <summary>
            /// Initializes m_isRotated
            /// </summary>
            private bool m_isRotated;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the viewport.
            /// </summary>
            /// <value>The viewport.</value>
            public Rect Viewport
            {
                get
                {
                    return m_viewport;
                }
            }
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="ChartCartesianTransformer"/> class.
            /// </summary>
            /// <param name="viewport">The viewport.</param>
            /// <param name="xAxis">The x axis.</param>
            /// <param name="yAxis">The y axis.</param>
            public ChartCartesianTransformer(Rect viewport, ChartAxis xAxis, ChartAxis yAxis)
            {
                m_viewport = viewport;
                m_xAxis = xAxis;
                m_yAxis = yAxis;
            }

            public ChartCartesianTransformer(Rect viewport, ChartAxis xAxis, ChartAxis yAxis, ChartAxis zAxis)
            {
                m_viewport = viewport;
                m_xAxis = xAxis;
                m_yAxis = yAxis;
                m_zAxis = zAxis;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ChartCartesianTransformer"/> class.
            /// </summary>
            /// <param name="viewport">The viewport.</param>
            /// <param name="series">The series.</param>
            public ChartCartesianTransformer(Rect viewport, ChartSeries series)
            {
                m_viewport = viewport;
                m_isRotated = series.ChartType.IsRotated ^ series.IsRotated;
                m_xAxis = series.XAxis;
                m_yAxis = series.YAxis;
                m_zAxis = series.ZAxis;
            }
            #endregion

            #region Public methods
            /// <summary>
            /// Transforms chart cordinates to real coordinates.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            /// <returns>The visible point</returns>
            public Point TransformToVisible(double x, double y)
            {
                if (m_xAxis != null && m_yAxis != null && double.IsNaN(m_viewport.Width) == false && double.IsNaN(m_viewport.Height) == false)
                {
                    x = x > 0 && m_xAxis.IsLogarithmic ? Math.Log(x, m_xAxis.LogarithmicBase) : x;
                    y = y > 0 && m_yAxis.IsLogarithmic ? Math.Log(y, m_yAxis.LogarithmicBase) : y;

                    return m_isRotated ?
                      new Point(m_viewport.Width * m_yAxis.ValueToCoefficient(y), m_viewport.Height * (1 - m_xAxis.ValueToCoefficient(x))) :
                      new Point(m_viewport.Width * m_xAxis.ValueToCoefficient(x), m_viewport.Height * (1 - m_yAxis.ValueToCoefficient(y)));
                }

                return new Point(0, 0);
            }

            public Point3D TransformToVisible(double x, double y, double z)
            {
                if (m_xAxis != null && m_yAxis != null && m_zAxis != null && double.IsNaN(m_viewport.Width) == false && double.IsNaN(m_viewport.Height) == false)
                {
                    x = x > 0 && m_xAxis.IsLogarithmic ? Math.Log(x, m_xAxis.LogarithmicBase) : x;
                    y = y > 0 && m_yAxis.IsLogarithmic ? Math.Log(y, m_yAxis.LogarithmicBase) : y;
                    z = z > 0 && m_zAxis.IsLogarithmic ? Math.Log(z, m_zAxis.LogarithmicBase) : z;
                    return m_isRotated ?
                      new Point3D(m_viewport.Width * m_yAxis.ValueToCoefficient(y), m_viewport.Height * (1 - m_xAxis.ValueToCoefficient(x)),m_zAxis.ValueToCoefficient(z)) :
                      new Point3D(m_viewport.Width * m_xAxis.ValueToCoefficient(x), m_viewport.Height * (1 - m_yAxis.ValueToCoefficient(y)), m_zAxis.ValueToCoefficient(z));
                }

                return new Point3D(0, 0,0);
            }
            #endregion
        }

        /// <summary>
        /// Represents ChartPolarTransformer
        /// </summary>
        private class ChartPolarTransformer : IChartTransformer
        {
            #region Members
            /// <summary>
            /// Initializes m_viewport
            /// </summary>
            private Rect m_viewport = Rect.Empty;

            /// <summary>
            /// Initializes m_xAxis
            /// </summary>
            private ChartAxis m_xAxis;

            /// <summary>
            /// Initializes m_yAxis
            /// </summary>
            private ChartAxis m_yAxis;

            /// <summary>
            /// Initializes m_center
            /// </summary>
            private Point m_center = new Point();

            /// <summary>
            /// Initializes m_radius
            /// </summary>
            private double m_radius;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the viewport.
            /// </summary>
            /// <value>The viewport.</value>
            public Rect Viewport
            {
                get
                {
                    return m_viewport;
                }
            }
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="ChartPolarTransformer"/> class.
            /// </summary>
            /// <param name="viewport">The viewport.</param>
            /// <param name="xAxis">The x axis.</param>
            /// <param name="yAxis">The y axis.</param>
            public ChartPolarTransformer(Rect viewport, ChartAxis xAxis, ChartAxis yAxis)
            {
                m_viewport = viewport;
                m_xAxis = xAxis;
                m_yAxis = yAxis;
                m_center = ChartLayoutUtils.GetCenter(m_viewport);
                m_radius = 0.5 * Math.Min(m_viewport.Width, m_viewport.Height);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ChartPolarTransformer"/> class.
            /// </summary>
            /// <param name="viewport">The viewport.</param>
            /// <param name="series">The series.</param>
            public ChartPolarTransformer(Rect viewport, ChartSeries series)
            {
                m_viewport = viewport;
                m_xAxis = series.XAxis;
                m_yAxis = series.YAxis;
                m_center = ChartLayoutUtils.GetCenter(m_viewport);
                m_radius = 0.5 * Math.Min(m_viewport.Width, m_viewport.Height);
            }
            #endregion

            #region Public methods
            /// <summary>
            /// Transforms chart cordinates to real coordinates.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            /// <returns>The visible point</returns>
            public Point TransformToVisible(double x, double y)
            {
                double radius = m_radius * m_yAxis.ValueToCoefficient(y);
                return m_center + radius * ChartTransform.ValueToVector(m_xAxis, x);
            }
            #endregion


            public Point3D TransformToVisible(double x, double y, double z)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates the Cartesian transformer.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The Chart Transformer</returns>
        public static IChartTransformer CreateSimple(Rect viewport)
        {
            return new ChartSimpleTransformer(viewport);
        }

        /// <summary>
        /// Creates the Cartesian transformer.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="series">The series.</param>
        /// <returns>The Cartesian Transformer</returns>
        public static IChartTransformer CreateCartesian(Rect viewport, ChartSeries series)
        {
            return new ChartCartesianTransformer(viewport, series);
        }

        /// <summary>
        /// Creates the Cartesian transformer.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="xAxis">The x axis.</param>
        /// <param name="yAxis">The y axis.</param> 
        /// <returns>The Cartesian Transformer</returns>
        public static IChartTransformer CreateCartesian(Rect viewport, ChartAxis xAxis, ChartAxis yAxis)
        {
            return new ChartCartesianTransformer(viewport, xAxis, yAxis);
        }

        /// <summary>
        /// Creates the polar.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="series">The series.</param>
        /// <returns>The Polar Transformer</returns>
        public static IChartTransformer CreatePolar(Rect viewport, ChartSeries series)
        {
            return new ChartPolarTransformer(viewport, series);
        }

        /// <summary>
        /// Creates the transformer.
        /// </summary>
        /// <param name="axesType">Type of the axes.</param>
        /// <param name="viewport">The viewport.</param>
        /// <param name="series">The series.</param>
        /// <returns>The Transformer</returns>
        public static IChartTransformer CreateTransformer(ChartAxesType axesType, Rect viewport, ChartSeries series)
        {
            IChartTransformer result = null;

            switch (axesType)
            {
                case ChartAxesType.None:
                    result = new ChartSimpleTransformer(viewport);
                    break;
                case ChartAxesType.CartesianAxes:
                    result = new ChartCartesianTransformer(viewport, series);
                    break;
                case ChartAxesType.PolarAxes:
                    result = new ChartPolarTransformer(viewport, series);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Coefficients to vector.
        /// </summary>
        /// <param name="coefficient">The coefficient.</param>
        /// <returns>The vector value</returns>
        public static Vector CoefficientToVector(double coefficient)
        {
            double angle = Math.PI * (1.5 - 2 * coefficient);

            return new Vector(Math.Cos(angle), Math.Sin(angle));
        }

        /// <summary>
        /// Values to angle.
        /// </summary>
        /// <param name="axis">The axis value.</param>
        /// <param name="value">The double value.</param>
        /// <returns>Returns the angle</returns>
        public static double ValueToAngle(ChartAxis axis, double value)
        {
            double angle = 1.5 * Math.PI - ChartMath.DoublePI * axis.ValueToCoefficient(value) % ChartMath.DoublePI;

            if (angle < 0)
            {
                angle += ChartMath.DoublePI;
            }

            return angle;
        }

        /// <summary>
        /// Values to vector.
        /// </summary>
        /// <param name="axis">The axis value.</param>
        /// <param name="value">The value.</param>
        /// <returns>The vector value</returns>
        public static Vector ValueToVector(ChartAxis axis, double value)
        {
            return CoefficientToVector(axis.ValueToCoefficient(value));
        }
        #endregion
    }
}
