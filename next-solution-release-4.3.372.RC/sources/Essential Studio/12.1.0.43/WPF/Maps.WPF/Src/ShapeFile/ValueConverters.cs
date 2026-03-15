#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    using Syncfusion.Maps.IO;
    using System.Windows.Media;

    /// <summary>
    ///  This method converts the Normal string and format the string for
    /// ShapeFileTooltip
    /// </summary>
    public class ShapeFileTooltipConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value from normal string to formatted string.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var attributes = value as Dictionary<string, object>;
            var sb = new StringBuilder();
            int i = 0;
            foreach (var kvp in attributes)
            {
                var result = String.Format("{0} - {1}", kvp.Key, kvp.Value != null ? kvp.Value : String.Empty);
                sb.Append(result);

                i++;
                if (i != attributes.Count)
                {
                    sb.Append(" / ");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts a FormattedString to normal string.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    /// <summary>
    ///  This helps to convert LatitudeLongitudeDegree to text value
    /// </summary>
    public class LatitudeLongitudeDegreeToTextConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a LatitudeLongitudeDegree to text value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string LatLanText = "";
            var latLonPt = (Point)value;
            double lat = latLonPt.Y;
            int latsec = (int)Math.Round(lat * 3600);
            int latdeg = latsec / 3600;
            latsec = Math.Abs(latsec % 3600);
            int latmin = latsec / 60;
            latsec %= 60;
            String latlony;

            char degree_asciivalue = (char)176;
            if (latdeg < 0)
            {
                latdeg *= -1;
                latlony = latdeg.ToString() + degree_asciivalue + " " + latmin.ToString() + "' " + latsec.ToString() + "\" S";
            }
            else
                latlony = latdeg.ToString() + degree_asciivalue + " " + latmin.ToString() + "' " + latsec.ToString() + "\" N";

            //latlon.Y= (Point)laty;

            double lon = latLonPt.X;
            int lonsec = (int)Math.Round(lon * 3600);
            int londeg = lonsec / 3600;
            lonsec = Math.Abs(lonsec % 3600);
            int lonmin = lonsec / 60;
            lonsec %= 60;
            String latlonx;
            if (londeg < 0)
            {
                londeg *= -1;
                latlonx = londeg.ToString() + degree_asciivalue + " " + lonmin.ToString() + "' " + lonsec.ToString() + "\" W";
            }
            else
                latlonx = londeg.ToString() + degree_asciivalue + " " + lonmin.ToString() + "' " + lonsec.ToString() + "\" E";

            LatLanText = "Latitude:" + latlony + " / " + "Longitude:" + latlonx;
            return LatLanText;


        }

        /// <summary>
        /// Converts a text to LatitudeLongitudeDegree.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    /// <summary>
    ///  This helps to convert LatitudeLongitudePoint to text value
    /// </summary>
    public class LatitudeLongitudeToTextConverter : IValueConverter
    {
        #region IValueConverter Members
        /// <summary>
        /// Converts a LatitudeLongitudePoint to text.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string LatLanText = "";
            var p = (Point)value;
            if (p != null)
            {
                LatLanText = String.Format("Latitude:{0} / Longitude:{1}", p.Y, p.X);
            }

            return LatLanText;
        }
        /// <summary>
        /// Converts a text to LatitudeLongitudePoint.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    ///  This is used to convert Zoom Value to Zoom Factor
    /// </summary>
     public class ZoomLevelConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a  Zoom Value to Zoom Factor.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double zoomFactor = (double)value;
            double tempZoom = 0;
            if (parameter is MapControl)
            {
                tempZoom = (parameter as MapControl).tempZoomFactor;
            }
            return zoomFactor / tempZoom;
        }

        /// <summary>
        /// Converts a  Zoom Factor to Zoom Value
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double zoomLevel = (double)value;
            double tempZoom = (parameter as MapControl).tempZoomFactor;
            return zoomLevel * tempZoom;
        }

        #endregion
    }
     /// <summary>
     ///  This is helps to convert boolean value to BorderThickness
     /// </summary>
     public class BooleanToBorderThickness : IValueConverter
     {
         #region IValueConverter Members

         /// <summary>
         /// Converts a boolean value to BorderThickness.
         /// </summary>
         /// <param name="value">The value produced by the binding source.</param>
         /// <param name="targetType">The type of the binding target property.</param>
         /// <param name="parameter">The converter parameter to use.</param>
         /// <param name="culture">The culture to use in the converter.</param>
         /// <returns>
         /// A converted value. If the method returns null, the valid null value is used.
         /// </returns>
         public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
         {
             bool res = (bool)value;
             if (res)
             {
                 return new Thickness(0.333333);
             }
             else
             {
                 return new Thickness(0);
             }
         }

         /// <summary>
         /// Converts a  BorderThickness to boolean value.
         /// </summary>
         /// <param name="value">The value that is produced by the binding target.</param>
         /// <param name="targetType">The type to convert to.</param>
         /// <param name="parameter">The converter parameter to use.</param>
         /// <param name="culture">The culture to use in the converter.</param>
         /// <returns>
         /// A converted value. If the method returns null, the valid null value is used.
         /// </returns>
         public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
         {
             throw new NotImplementedException();   
         }

         #endregion
     }
     /// <summary>
     ///  This helps to Convert Boolean value to Background
     /// </summary>
     public class BooleanToBackground : IValueConverter
     {
         #region IValueConverter Members

         /// <summary>
         /// Converts a Boolean value to Background.
         /// </summary>
         /// <param name="value">The value produced by the binding source.</param>
         /// <param name="targetType">The type of the binding target property.</param>
         /// <param name="parameter">The converter parameter to use.</param>
         /// <param name="culture">The culture to use in the converter.</param>
         /// <returns>
         /// A converted value. If the method returns null, the valid null value is used.
         /// </returns>
         public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
         {
             bool res = (bool)value;
             if (res)
             {
                 return new SolidColorBrush(new Color() { A = 255, B = 241, G = 241, R = 241 });
             }
             else
             {
                 return new SolidColorBrush(Colors.Transparent);
             }
         }

         /// <summary>
         /// Converts a Background to Boolean value .
         /// </summary>
         /// <param name="value">The value that is produced by the binding target.</param>
         /// <param name="targetType">The type to convert to.</param>
         /// <param name="parameter">The converter parameter to use.</param>
         /// <param name="culture">The culture to use in the converter.</param>
         /// <returns>
         /// A converted value. If the method returns null, the valid null value is used.
         /// </returns>
         public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
         {
             throw new NotImplementedException();
         }

         #endregion
     }
  
}
