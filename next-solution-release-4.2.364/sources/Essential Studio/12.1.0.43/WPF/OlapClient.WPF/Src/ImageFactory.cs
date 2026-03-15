#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.Chart;
using Syncfusion.Windows.Chart.Olap;

namespace Syncfusion.Windows.Client.Olap
{
    #region ImageFactory Class
    /// <summary>
    /// ImageFactor class to load Color Palette and ChartType Images. 
    /// </summary>
    public class ImageFactory
    {
        /// <summary>
        /// Instatiate ChartType Images 
        /// </summary>
        public static ChartSeriesImages chartimages = new ChartSeriesImages();

        /// <summary>
        /// Static variable to count the images
        /// </summary>
        public static int count = 0;

        /// <summary>
        /// Instantiate ColorPalette Images 
        /// </summary>
        public static ChartPaletteImages images = new ChartPaletteImages();

        /// <summary>
        /// Instantiate GridLayout Images
        /// </summary>
        public static GridLayoutImages gridlayoutimages = new GridLayoutImages();

        /// <summary>
        /// Gets the chart images.
        /// </summary>
        /// <value>The chart images.</value>
        public static ChartSeriesImages ChartImages
        {
            get
            {
                return chartimages;
            }
        }

        /// <summary>
        /// Gets the images.
        /// </summary>
        /// <value>The images.</value>
        public static ChartPaletteImages Images
        {
            get
            {
                return images;
            }
        }

        /// <summary>
        /// Gets the grid layout images.
        /// </summary>
        /// <value>The grid layout images.</value>
        public static GridLayoutImages GridLayoutImages 
        {
            get
            {
                return gridlayoutimages;
            }
        }
    }
    #endregion

    #region Helper Class
    /// <summary>
    /// Creating GridLayout Images
    /// </summary>
    public class GridLayoutImages : ObservableCollection<ImageData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridLayoutImages"/> class.
        /// </summary>
        public GridLayoutImages()
        {
            Syncfusion.Windows.Client.Olap.Resources.ClientResourceWrapper clintrswrp = new Syncfusion.Windows.Client.Olap.Resources.ClientResourceWrapper();
            this.Add(new ImageData("NormalLayout",clintrswrp.GridToolbarGridStyleNormal));
            this.Add(new ImageData("ExcelLikeLayout",clintrswrp.GridToolbarGridStyleExcelLike));
            this.Add(new ImageData("NormalTopSummary",clintrswrp.GridToolbarGridStyleNormalTopSummary));
            this.Add(new ImageData("NoSummaryLayout",clintrswrp.GridToolbarGridStyleNoSummaries));
        }
    }

    /// <summary>
    /// Creating ColorPalette images
    /// </summary>
    public class ChartPaletteImages : ObservableCollection<ImageData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Images"/> class.
        /// </summary>
        public ChartPaletteImages()
        {
            Syncfusion.Windows.Client.Olap.Resources.ClientResourceWrapper clintrswrp = new Syncfusion.Windows.Client.Olap.Resources.ClientResourceWrapper();
            this.Add(new ImageData(@"images\Default.png",clintrswrp.ChartToolbarColorPalette_Default));
            this.Add(new ImageData(@"images\Default-Alpha.png",clintrswrp.ChartToolbarColorPalette_DefaultAlpha));
            this.Add(new ImageData(@"images\Analog.png", clintrswrp.ChartToolbarColorPalette_Analog));
            this.Add(new ImageData(@"images\Colorful.png",clintrswrp.ChartToolbarColorPalette_Colorful));
            this.Add(new ImageData(@"images\EarthTone.png",clintrswrp.ChartToolbarColorPalette_EarthTone));
            this.Add(new ImageData(@"images\GaryScale.png",clintrswrp.ChartToolbarColorPalette_GrayScale));
            this.Add(new ImageData(@"images\Nature.png",clintrswrp.ChartToolbarColorPalette_Nature));
            this.Add(new ImageData(@"images\Pastel.png",clintrswrp.ChartToolbarColorPalette_Pastel));
            this.Add(new ImageData(@"images\Triad.png",clintrswrp.ChartToolbarColorPalette_Triad));
            this.Add(new ImageData(@"images\WarmCold.png",clintrswrp.ChartToolbarColorPalette_WarmCold));
            this.Add(new ImageData(@"images\Custom.png",clintrswrp.ChartToolbarColorPalette_Custom));
        }
    }

    /// <summary>
    /// Creating ChartyType images
    /// </summary>
    public class ChartSeriesImages : ObservableCollection<ImageData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartImages"/> class.
        /// </summary>
        public ChartSeriesImages()
        {
            Syncfusion.Windows.Client.Olap.Resources.ClientResourceWrapper clireswrap = new Syncfusion.Windows.Client.Olap.Resources.ClientResourceWrapper();
            this.Add(new ImageData(@"images\Column.png",clireswrap.ChartToolbarChartTypes_Column));
            this.Add(new ImageData(@"images\StackingColumn.png",clireswrap.ChartToolbarChartTypes_StackingColumn));
            this.Add(new ImageData(@"images\StackingColumn.png",clireswrap.ChartToolbarChartTypes_StackingColumn100));
            this.Add(new ImageData(@"images\Bar.png",clireswrap.ChartToolbarChartTypes_Bar));
            this.Add(new ImageData(@"images\StackingBar.png",clireswrap.ChartToolbarChartTypes_StackingBar));
            this.Add(new ImageData(@"images\Area.png",clireswrap.ChartToolbarChartTypes_Area));
            this.Add(new ImageData(@"images\StackingArea.png",clireswrap.ChartToolbarChartTypes_StackingArea));
            this.Add(new ImageData(@"images\SplineArea.png",clireswrap.ChartToolbarChartTypes_SplineArea));
            this.Add(new ImageData(@"images\StepArea.png",clireswrap.ChartToolbarChartTypes_StepArea));
            this.Add(new ImageData(@"images\Line.png",clireswrap.ChartToolbarChartTypes_Line));
            this.Add(new ImageData(@"images\Spline.png",clireswrap.ChartToolbarChartTypes_Spline));
            this.Add(new ImageData(@"images\RotatedSpline.png",clireswrap.ChartToolbarChartTypes_RotatedSpline));
            this.Add(new ImageData(@"images\StepLine.png",clireswrap.ChartToolbarChartTypes_StepLine));
            this.Add(new ImageData(@"images\Scatter.png",clireswrap.ChartToolbarChartTypes_Scatter));
            this.Add(new ImageData(@"images\Pie.png", clireswrap.ChartToolbarChartTypes_Pie));
            this.Add(new ImageData(@"images\Radar.png", clireswrap.ChartToolbarChartTypes_Radar));
            this.Add(new ImageData(@"images\Funnel.png", clireswrap.ChartToolbarChartTypes_Funnel));
        }
    }
    #endregion

    #region ImageData Class Definition
    /// <summary>
    /// Creating Images and Corresponding Text to load the ColorPalettes and ChartTypes.
    /// </summary>
    public class ImageData : INotifyPropertyChanged
    {
        private string m_imageName = string.Empty;

        public ImageData(string imageName, string text)
        {
            ImageFactory.count++;
            this.ImageName = imageName;
            this.Text = text.ToString(System.Globalization.CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the name of the image.
        /// </summary>
        /// <value>The name of the image.</value>
        public string ImageName
        {
            get
            {
                return this.m_imageName;
            }

            set
            {
                this.m_imageName = value;
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text of ColorPalette/ChartType.</value>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="property">The property.</param>
        protected void NotifyPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
    #endregion

    #region ImageConverter Class Definition
    /// <summary>
    /// Converts the images to resources
    /// </summary>
    public class ImageConverter : System.Windows.Data.IValueConverter
    {
        #region Private Constant Variables
        private const string c_Key = @"/Syncfusion.OlapClient.WPF;component/Themes/Generic.xaml";
        #endregion

        #region Public Methods
        /// <summary>
        /// Converts a string to resource.
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
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri(c_Key, UriKind.RelativeOrAbsolute);
            if (value.ToString() == "NormalLayout")
            {
                return resourceDictionary["NormalLayout"] as DrawingImage;
            }
            else if (value.ToString() == "ExcelLikeLayout")
            {
                return resourceDictionary["ExcelLikeLayout"] as DrawingImage;
            }
            else if (value.ToString() == "NormalTopSummary")
            {
                return resourceDictionary["NormalTopSummary"] as DrawingImage;
            }
            else if (value.ToString() == "NoSummaryLayout")
            {
                return resourceDictionary["NoSummaryLayout"] as DrawingImage;
            }
            else
            {
                return value;
            }
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
    #endregion

    #region VisibleToBoolean Converter
    /// <summary>
    /// Converter class for converting Visibility type to Boolean? type.
    /// </summary>
    public class VisibleToBoolConverter : System.Windows.Data.IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
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
            var enumValue = (Visibility)Enum.Parse(typeof(Visibility), value.ToString());
            return enumValue == Visibility.Visible;
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;            
        }

        #endregion
    }

    #endregion
}
