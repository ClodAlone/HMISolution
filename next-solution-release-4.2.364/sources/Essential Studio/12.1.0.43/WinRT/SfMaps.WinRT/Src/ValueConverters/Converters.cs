#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.UI.Xaml.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
#if WINRT
    using System.Threading.Tasks;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
    using Windows.Foundation;
    using Windows.UI.Xaml.Controls;
#else
    using System.Windows.Data;
    using System.Windows;
    using System.Windows.Media;
#endif
    using System.Collections.ObjectModel;
    using System.Reflection;
  

    public class MarginConverter : IValueConverter
    {
#if WINRT
        public object Convert(object value, Type targetType, object parameter, string language)
        {
#else
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#endif
            var val = (Thickness)value;
            return new Thickness(val.Left, val.Top - 20, 0, 0);
        }
    

#if WINRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
#else
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#endif
            throw new NotImplementedException();
        }
    }
    class SizeConverter : IValueConverter
    {
#if WINRT
        public object Convert(object value, Type targetType, object parameter, string language)
        {
#else
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#endif
            double ratio = (double)value;
            string[] param = parameter.ToString().Split(',');
            double val = double.Parse(param[0]);
            double min = double.Parse(param[1]);
            var result = (ratio * val) + min;
            return result;
        }

#if WINRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
#else
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#endif
            throw new NotImplementedException();
        }
    }

    class ColorConverter : IValueConverter
    {
#if WINRT
        public object Convert(object value, Type targetType, object parameter, string language)
        {
#else
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#endif
            byte a, r, g, b;
            string[] paramertes = parameter.ToString().Split(',');
            var max = double.Parse(paramertes[0]);
            var val = double.Parse(paramertes[1]);
            var color = (value as SolidColorBrush).Color.ToString();
            if (value != null)
            {
                if (color.Length == 7)
                {
                    a = (byte)((max / 256) * val);
                    r = byte.Parse(color.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                    g = byte.Parse(color.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                    b = byte.Parse(color.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    a = (byte)((int.Parse(color.Substring(1, 2), System.Globalization.NumberStyles.HexNumber) / max) * val);
                    r = byte.Parse(color.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                    g = byte.Parse(color.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                    b = byte.Parse(color.Substring(7, 2), System.Globalization.NumberStyles.HexNumber);
                }
                return new SolidColorBrush(Color.FromArgb(a, r, g, b));
            }
            else
            {
                return value;
            }

        }

#if WINRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
#else
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#endif
            throw new NotImplementedException();
        }
    }
   
    class LayerContentConverter : IValueConverter
    {

#if WINRT
        public object Convert(object value, Type targetType, object parameter, string language)
        {
#else
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#endif
            var index = (int)value;
            var layers = (ObservableCollection<MapLayer>)parameter;
            if (layers.Count > index)
            {
                return layers[index];
            }
            else
            {
                return null;
            }
        }

#if WINRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
#else
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#endif
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Represents the Boolean to visibility converter in map.
    /// </summary>
    /// <remarks>
    /// Convert the given Boolean converter parameter as visibility
    /// </remarks>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convert function,which converts the given Boolean parameter as visibility.
        /// </summary>
        /// <param name="value">Value which is bonded</param>
        /// <param name="targetType">Target type of the binding</param>
        /// <param name="parameter">Converter Parameter</param>
        /// <param name="language">
        /// Language
        /// </param>
        /// <returns>
        /// Type :<see cref="object"/>
        /// </returns>
#if WINRT
        public object Convert(object value, Type targetType, object parameter, string language)
        {
#else
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
#endif
            bool result = (bool)value;
            if (result)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Convert back function,which converts the given Boolean parameter as visibility.
        /// </summary>
        /// <param name="value">Value which is binded</param>
        /// <param name="targetType">Target type of the binding</param>
        /// <param name="parameter">Converter Parameter</param>
        /// <param name="language">
        /// Language
        /// </param>
        /// <returns>
        /// Type :<see cref="object"/>
        /// </returns>
#if WINRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
#else
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
#endif
            throw new NotImplementedException();
        }
    }

    class BubbleConverter : IValueConverter
    {
#if WINRT
        public object Convert(object value, Type targetType, object parameter, string language)
        {
#else
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#endif
            var param = (List<object>)parameter;
             var ratio = double.Parse(param[0].ToString());
             var min = double.Parse(param[1].ToString());
             double val = 0;
             double parValue;
             if (param[2]!=null && Double.TryParse(param[2].ToString(), out parValue))
             {
                 val = double.Parse(param[2].ToString());
             }     
             var result = (ratio * val) + min;
            return result;
        }

#if WINRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
#else
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#endif
            throw new NotImplementedException();
        }
    }

    class LatitudeLongitudeDegreeToTextConverter : IValueConverter
    {
#if WINRT
        public object Convert(object value, Type targetType, object parameter, string language)
        {
#else
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#endif
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

#if WINRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
#else
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#endif
            throw new NotImplementedException();
        }
    }

    class LatitudeLongitudeToTextConverter : IValueConverter
    {
#if WINRT
        public object Convert(object value, Type targetType, object parameter, string language)
        {
#else
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#endif
            string LatLanText = "";
            var p = (Point)value;
            if (p != null)
            {
                LatLanText = String.Format("Latitude:{0} / Longitude:{1}", p.Y, p.X);
            }
            return LatLanText;
        }

#if WINRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
#else
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#endif
            throw new NotImplementedException();
        }
    }
    public class MapViewConverter:IValueConverter
    {
#if WINRT
        public object Convert(object value, Type targetType, object parameter, string language)
        {
#else
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#endif
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        #if WINRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
#else
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#endif
            throw new NotImplementedException();
        }
    }
#if WINRT
    internal class ScrollModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value == true)
            {
                return ScrollMode.Enabled; 
            }
            else
            {
                return ScrollMode.Disabled; 
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

#endif
    #if WINRT
    internal class ZoomModeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
        

            if ((bool)value == true)
            {
                return ZoomMode.Enabled;
            }
            else
            {
                return ZoomMode.Disabled;
            }
        }
    
        
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {

            throw new NotImplementedException();
        }
    }
    #endif
}
