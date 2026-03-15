// <copyright file="VisualToDrawingConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// This class makes relation between <see cref="Visual"/> value and <see cref="Drawing"/> value.
    /// </summary>
    [ValueConversion(typeof(Visual), typeof(Drawing))]
    internal class VisualToDrawingConverter : IValueConverter
    {
        #region Fields
        /// <summary>
        /// Drawing helper instance.
        /// </summary>
        private readonly DrawingHelper m_helper = new DrawingHelper();

        /// <summary>
        /// Top most visual object reference.
        /// </summary>
        private Visual m_topMostVisual = null;

        /// <summary>
        /// Count variable for conversion chain.
        /// </summary>
        private int m_convertionChainCount = 0;
        #endregion

        #region IValueConverter Members

        /// <summary>
        /// Converts <see cref="Visual"/> value to <see cref="Drawing"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Visual"/> value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted <see cref="Drawing"/> value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DrawingGroup drawing = null;
            Visual visual = value as Visual;

            if (visual != null)
            {
                if (m_topMostVisual == null || m_convertionChainCount == 0)
                {
                    m_topMostVisual = visual;
                }

                m_convertionChainCount++;

                drawing = VisualTreeHelper.GetDrawing(value as Visual);
                if (drawing != null)
                {
                    IEnumerable en = LogicalTreeHelper.GetChildren(visual);
                    if (en != null)
                    {
                        IEnumerator enumerator = en.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            Visual child = enumerator.Current as Visual;
                            if (child != null)
                            {
                                Drawing childDrawing = Convert(child, typeof(Drawing), parameter, culture) as Drawing;
                                if (childDrawing != null)
                                {
                                    GeneralTransform gt = child.TransformToVisual(m_topMostVisual);
                                    Drawing transformedDrawing = DrawingHelper.ApplyTransformToDrawing(childDrawing, gt);

                                    drawing.Children.Add(transformedDrawing);
                                }
                            }
                        }
                    }
                }

                m_convertionChainCount--;
            }

            return drawing;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
