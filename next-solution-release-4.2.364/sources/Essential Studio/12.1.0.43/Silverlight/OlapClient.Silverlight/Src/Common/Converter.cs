#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using Syncfusion.OlapSilverlight.Data;
using System.Windows.Media.Imaging;

namespace Syncfusion.Silverlight.Client.Olap
{
    /// <summary>
    /// Converter class to change the Grid layout image.
    /// </summary>
    public class GridLayoutConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string layout = value.ToString();
            
            if (!string.IsNullOrEmpty(layout))
            {
                return new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ToolBarImages/" + layout + ".png", UriKind.RelativeOrAbsolute));
            }
          
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    /// <summary>
    /// Converter class to change the Chart types image.
    /// </summary>
    public class ChartTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string chartType = value.ToString();
            if (!string.IsNullOrEmpty(chartType))
            {
                return new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/"+chartType+".png", UriKind.RelativeOrAbsolute));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    /// <summary>
    /// Converter class to change the Chart palettes image.
    /// </summary>
    public class ChartPaletteConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string palette = value.ToString();
            if (!string.IsNullOrEmpty(palette))
            {
                return new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/" + palette + ".png", UriKind.RelativeOrAbsolute));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
    /// <summary>
    /// Class for Localizing ComboBox Items
    /// </summary>
    public class ImageData
    {
        /// <summary>
        /// Gets or sets the Constructor of ImageData for Localization Use
        /// </summary>
        /// <param name="_styleName"></param>
        /// <param name="_styleImage"></param>
        public ImageData(string _styleName, BitmapImage _styleImage)
        {
            this.styleName = _styleName;
            this.styleImage = _styleImage;
        }
        private string styleName;
        private BitmapImage styleImage;
        /// <summary>
        /// Gets or sets the StyleName for Localization use
        /// </summary>
        public string StyleName
        {
            get { return styleName; }
            set { styleName = value; }
        }
        /// <summary>
        /// Gets or set the StyleImage for Localization
        /// </summary>
        public BitmapImage StyleImage
        {
            get { return styleImage; }
            set { styleImage = value; }
        }
    }
    
}
