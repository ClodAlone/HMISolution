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
using System.Drawing;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Represents the points array object of SVG DOM.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class PointsArray : FloatArray
    {
        #region Member     
        private PointF[] m_points;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the points.
        /// </summary>
        /// <value>The points.</value>
        public PointF[] Points
        {
            get
            {
                return m_points;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PointsArray"/> class.
        /// </summary>
        /// <param name="array">The array.</param>
        public PointsArray(float[] array)
            : base(array)
        {
            m_points = new PointF[array.Length / 2];

            for (int i = 0, c = array.Length / 2; i < c; i++)
            {
                m_points[i] = new PointF(array[2 * i], array[2 * i + 1]);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointsArray"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PointsArray(string value)
            : base(value)
        {
            m_points = new PointF[m_array.Length / 2];

            for (int i = 0, c = m_array.Length / 2; i < c; i++)
            {
                m_points[i] = new PointF(m_array[2 * i], m_array[2 * i + 1]);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointsArray"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        public PointsArray(PointF[] points)
        {
            m_points = points;
            m_array = FromPointFToFloat(points);
        }
        #endregion

        #region Helper methdos
        /// <summary>
        /// Creates the float array by the specified array of <see cref="PointF"/>.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>Returns Float array from Point array.</returns>
        private float[] FromPointFToFloat(PointF[] points)
        {
            float[] array = new float[2 * points.Length];

            for (int i = 0, c = points.Length; i < c; i++)
            {
                array[2 * i] = points[i].X;
                array[2 * i + 1] = points[i].Y;
            }

            return array;
        }
        #endregion
    }
}
