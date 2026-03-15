// <copyright file="ColorToHSVBackgroundConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using System.Globalization;

namespace Syncfusion.Windows.Shared
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// Class which converts color to HSV background.
    /// </summary>
    public class ColorToHSVBackgroundConverter : IMultiValueConverter
    {
        #region Private fields
        /// <summary>
        /// HSV color.
        /// </summary>
        private HSV m_hsv;

        /// <summary>
        /// Hue parameter.
        /// </summary>
        private float m_h;

        /// <summary>
        /// Saturation parameter.
        /// </summary>
        private float m_s;

        /// <summary>
        /// Brightness parameter.
        /// </summary>
        private float m_v;

        #endregion

        #region IMultiValueConverter Members

        /// <summary>
        /// Converts color brush into solid brush.
        /// </summary>
        /// <param name="values">Color brush value</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Binding parameters</param>
        /// <param name="culture">Current UICulture</param>
        /// <returns>
        /// Returns HSV value.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is HSV)
            {
                m_hsv = (HSV)values[0];
            }

            if (values[1] is float)
            {
                m_h = (float)values[1];
            }

            if (values[2] is float)
            {
                m_s = (float)values[2];
            }

            if (values[3] is float)
            {
                m_v = (float)values[3];
            }

            if (parameter.ToString() == "Background")
            {
                return GenerateHSVBrush();
            }
            else if (parameter.ToString() == "VerticalSlider")
            {
                return GenerateSliderBrushHSV();
            }
            else if (parameter.ToString() == "HorizontalSlider")
            {
                return GenerateHorizontalSliderBrush();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts solid color brush into color brush.
        /// </summary>
        /// <returns>
        /// Color brush
        /// </returns>
        /// <param name="value">Solid color value</param>
        /// <param name="targetTypes">Target Type</param>
        /// <param name="parameter">Binding parameter</param>
        /// <param name="culture">Current UICulture</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region Help Methods
        /// <summary>
        /// Generates the brush for HSV model.
        /// </summary>
        /// <returns>
        /// Generated brush.
        /// </returns>
        private Brush GenerateHSVBrush()
        {
            switch (m_hsv)
            {
                case HSV.H:
                    return GenerateHBrush();
                case HSV.S:
                    return GenerateSBrush();
                case HSV.V:
                    return GenerateVBrush();
                default:
                    return GenerateHBrush();
            }
        }

        /// <summary>
        /// Generates the Hue brush for HSV model.
        /// </summary>
        /// <returns>
        /// Generated brush.
        /// </returns>
        private Brush GenerateHBrush()
        {
            Color defColor = HsvColor.ConvertHsvToRgb(m_h, 1, 1);

            DrawingBrush drawingBrush = new DrawingBrush();
            DrawingGroup drawingGroup = new DrawingGroup();

            RectangleGeometry myRectGeometry = new RectangleGeometry();
            myRectGeometry.Rect = new Rect(0, 0, 1, 1);

            GeometryDrawing geometreDrawing1 = new GeometryDrawing();
            LinearGradientBrush firstBrush = new LinearGradientBrush();
            firstBrush.StartPoint = new Point(1, 0.5);
            firstBrush.EndPoint = new Point(0, 0.5);
            firstBrush.GradientStops.Add(new GradientStop(defColor, 0));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 255, 255), 1));

            geometreDrawing1.Brush = firstBrush;
            geometreDrawing1.Geometry = myRectGeometry;
   
            GeometryDrawing geometreDrawing2 = new GeometryDrawing();
            LinearGradientBrush secondBrush = new LinearGradientBrush();
            secondBrush.StartPoint = new Point(0.5, 1);
            secondBrush.EndPoint = new Point(0.5, 0);
            secondBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 0, 0, 0), 0));
            secondBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1));

            geometreDrawing2.Brush = secondBrush;
            geometreDrawing2.Geometry = myRectGeometry;
            
            drawingGroup.Children.Add(geometreDrawing1);
            drawingGroup.Children.Add(geometreDrawing2);

            drawingBrush.Drawing = drawingGroup;

            return drawingBrush;
        }

        /// <summary>
        /// Generates the Saturation brush for HSV model.
        /// </summary>
        /// <returns>
        /// Generated brush.
        /// </returns>
        private Brush GenerateSBrush()
        {
            DrawingBrush drawingBrush = new DrawingBrush();
            DrawingGroup drawingGroup = new DrawingGroup();

            RectangleGeometry myRectGeometry = new RectangleGeometry();
            myRectGeometry.Rect = new Rect(0, 0, 1, 1);
            
            GeometryDrawing geometreDrawing1 = new GeometryDrawing();

            LinearGradientBrush firstBrush = new LinearGradientBrush();
            firstBrush.StartPoint = new Point(0, 0.5);
            firstBrush.EndPoint = new Point(1, 0.5);
            firstBrush.GradientStops.Add(new GradientStop(Color.FromScRgb((float)m_s, 1, 0, 0), 0.000));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromScRgb((float)m_s, 1, 1, 0), 0.166));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromScRgb((float)m_s, 0, 1, 0), 0.333));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromScRgb((float)m_s, 0, 1, 1), 0.500));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromScRgb((float)m_s, 0, 0, 1), 0.666));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromScRgb((float)m_s, 1, 0, 1), 0.833));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromScRgb((float)m_s, 1, 0, 0), 1.000));

            geometreDrawing1.Brush = firstBrush;
            geometreDrawing1.Geometry = myRectGeometry;

            GeometryDrawing geometreDrawing2 = new GeometryDrawing();

            LinearGradientBrush secondBrush = new LinearGradientBrush();
            secondBrush.StartPoint = new Point(0.5, 0);
            secondBrush.EndPoint = new Point(0.5, 1);
            secondBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(0, 0, 0, 0), 0));
            secondBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(1, 0, 0, 0), 1));

            geometreDrawing2.Brush = secondBrush;
            geometreDrawing2.Geometry = myRectGeometry;

            drawingGroup.Children.Add(geometreDrawing1);
            drawingGroup.Children.Add(geometreDrawing2);

            drawingBrush.Drawing = drawingGroup;

            return drawingBrush;
        }

        /// <summary>
        /// Generates the Value brush for HSV model.
        /// </summary>
        /// <returns>
        /// Generated brush.
        /// </returns>
        private Brush GenerateVBrush()
        {
            DrawingBrush drawingBrush = new DrawingBrush();
            DrawingGroup drawingGroup = new DrawingGroup();

            RectangleGeometry myRectGeometry = new RectangleGeometry();
            myRectGeometry.Rect = new Rect(0, 0, 1, 1);

            GeometryDrawing geometreDrawing1 = new GeometryDrawing();

            LinearGradientBrush firstBrush = new LinearGradientBrush();
            firstBrush.StartPoint = new Point(0, 0.5);
            firstBrush.EndPoint = new Point(1, 0.5);
            firstBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 000, 000), 0.000));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 255, 000), 0.166));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 000, 255, 000), 0.333));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 000, 255, 255), 0.500));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 000, 000, 255), 0.666));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 000, 255), 0.833));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 000, 000), 1.000));

            geometreDrawing1.Brush = firstBrush;
            geometreDrawing1.Geometry = myRectGeometry;

            GeometryDrawing geometreDrawing2 = new GeometryDrawing();

            LinearGradientBrush secondBrush = new LinearGradientBrush();
            secondBrush.StartPoint = new Point(0.5, 0);
            secondBrush.EndPoint = new Point(0.5, 1);
            float opacity = 1 - (float)m_v;
            secondBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(opacity, 0, 0, 0), 0));
            secondBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(opacity, 0, 0, 0), 1));

            geometreDrawing2.Brush = secondBrush;
            geometreDrawing2.Geometry = myRectGeometry;

            GeometryDrawing geometreDrawing3 = new GeometryDrawing();

            LinearGradientBrush thirdBrush = new LinearGradientBrush();
            thirdBrush.StartPoint = new Point(0.5, 0);
            thirdBrush.EndPoint = new Point(0.5, 1);
            thirdBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(0, 1, 1, 1), 0));
            thirdBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(1, 1, 1, 1), 1));

            geometreDrawing3.Brush = thirdBrush;
            geometreDrawing3.Geometry = myRectGeometry;
           
            drawingGroup.Children.Add(geometreDrawing1);
            drawingGroup.Children.Add(geometreDrawing3);
            drawingGroup.Children.Add(geometreDrawing2);

            drawingBrush.Drawing = drawingGroup;

            return drawingBrush;
        }

        /// <summary>
        /// Generates the slider brush for HSV model.
        /// </summary>
        /// <returns>
        /// Return brush value for corresponding HSV value.
        /// </returns>
        private Brush GenerateSliderBrushHSV()
        {
            switch (m_hsv)
            {
                case HSV.H:
                    return GenerateHSliderBrush();
                case HSV.S:
                    return GenerateSSliderBrush();
                case HSV.V:
                    return GenerateVSliderBrush();
                default:
                    return GenerateHSliderBrush();
            }
        }

        /// <summary>
        /// Generates horizontal Hue slider brush for HSV model.
        /// </summary>
        /// <returns>Generated brush.</returns>
        private Brush GenerateHorizontalSliderBrush()
        {
            return GetHSliderBrush(0, 0, 1, 0);
        }

        /// <summary>
        /// Generates vertical Hue slider brush for HSV model.
        /// </summary>
        /// <returns>Generated brush.</returns>
        private Brush GenerateHSliderBrush()
        {
            return GetHSliderBrush(0.5, 1, 0.5, 0);
        }

        /// <summary>
        /// Generates the Hue slider brush for HSV model.
        /// </summary>
        /// <param name="brushStartX">X coordinate of the LinearGradientBrush StartPoint</param>
        /// <param name="brushStartY">Y coordinate of the LinearGradientBrush StartPoint</param>
        /// <param name="brushEndX">X coordinate of the LinearGradientBrush EndPoint</param>
        /// <param name="brushEndY">Y coordinate of the LinearGradientBrush EndPoint</param>
        /// <returns>Generated brush.</returns>
        private DrawingBrush GetHSliderBrush(double brushStartX, double brushStartY, double brushEndX, double brushEndY)
        {
            DrawingBrush drawingBrush = new DrawingBrush();
            DrawingGroup drawingGroup = new DrawingGroup();

            RectangleGeometry myRectGeometry = new RectangleGeometry();
            myRectGeometry.Rect = new Rect(0, 0, 100, 100);

            GeometryDrawing geometreDrawing1 = new GeometryDrawing();

            LinearGradientBrush firstBrush = new LinearGradientBrush();
            firstBrush.StartPoint = new Point(brushStartX, brushStartY);
            firstBrush.EndPoint = new Point(brushEndX, brushEndY);
            firstBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(1, 1, 0, 0), 0.000));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(1, 1, 1, 0), 0.166));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(1, 0, 1, 0), 0.333));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(1, 0, 1, 1), 0.500));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(1, 0, 0, 1), 0.666));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(1, 1, 0, 1), 0.833));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(1, 1, 0, 0), 1.000));

            geometreDrawing1.Brush = firstBrush;
            geometreDrawing1.Geometry = myRectGeometry;

            drawingGroup.Children.Add(geometreDrawing1);

            drawingBrush.Drawing = drawingGroup;

            return drawingBrush;
        }

        /// <summary>
        /// Generates the Saturation slider brush for HSV model.
        /// </summary>
        /// <returns>
        /// Generated brush.
        /// </returns>
        private Brush GenerateSSliderBrush()
        {
            LinearGradientBrush firstBrush = new LinearGradientBrush();
            firstBrush.StartPoint = new Point(0.5, 1);
            firstBrush.EndPoint = new Point(0.5, 0);
            firstBrush.GradientStops.Add(new GradientStop(HsvColor.ConvertHsvToRgb(m_h, 0, m_v), 0));
            firstBrush.GradientStops.Add(new GradientStop(HsvColor.ConvertHsvToRgb(m_h, 1, m_v), 1));

            return firstBrush;
        }

        /// <summary>
        /// Generates the Value slider brush for HSV model.
        /// </summary>
        /// <returns>
        /// Generated brush.
        /// </returns>
        private Brush GenerateVSliderBrush()
        {
            LinearGradientBrush firstBrush = new LinearGradientBrush();
            firstBrush.StartPoint = new Point(0.5, 0);
            firstBrush.EndPoint = new Point(0.5, 1);
            firstBrush.GradientStops.Add(new GradientStop(HsvColor.ConvertHsvToRgb(m_h, m_s, 0), 1));
            firstBrush.GradientStops.Add(new GradientStop(HsvColor.ConvertHsvToRgb(m_h, m_s, 1), 0));

            return firstBrush;
        }
        #endregion
    }
}
