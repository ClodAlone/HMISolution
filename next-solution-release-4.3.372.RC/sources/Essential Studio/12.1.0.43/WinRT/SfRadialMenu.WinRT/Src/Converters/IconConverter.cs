// <copyright file="IconConverter.cs" company="Syncfusion">
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
#if !(WINDOWS_PHONE_7 || Silverlight4)
using System.Threading.Tasks;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Data;
using System.Windows.Controls;
using Syncfusion.WP.Controls.Navigation;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using System.Windows.Data;
using System.Windows.Controls;

using System.Globalization;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Data;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
namespace Syncfusion.Windows.Controls.Navigation
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// Represents the converter that returns the icon property value of the
    /// RadialMenuItem
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem"/>
    /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem.Icon"/>
    [ClassReference(IsReviewed = false)]
    public class IconConverter : IValueConverter
    {
        private readonly double imgSize = 16;
        /// <summary>
        /// Converts value to object type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
#if !WINRT
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
#else
        public object Convert(object value, Type targetType, object parameter, string language)
#endif

        {
            ItemsControl menuItem = value as ItemsControl;
            if (menuItem != null)
            {
                if (menuItem is SfRadialMenuItem)
                {
#if WPFSILVERLIGHT
                    Grid backbutton=new Grid(); 
#if WPF
                    Path arrow = new Path(){Data = Geometry.Parse("M8.5704565,0 L14.74649,0 L8.5095301,6.2419558 L22.002934,6.2419558 L22.002934,10.902534 L8.5095301,10.902534 L14.74649,17.138 L8.5704565,17.138 L0,8.5690022 z"),Fill = new SolidColorBrush(Colors.Black),Stretch = Stretch.Fill,Margin=new Thickness(8.83,11.333,9.167,11.529)};
#else
                    PathFigure pathFigure = new PathFigure();
                    pathFigure.StartPoint = new Point(8.5704565, 0);
                    pathFigure.IsClosed = true; 
                    LineSegment lineSegment1 = new LineSegment();
                    lineSegment1.Point = new Point(14.74649, 0);
                    LineSegment lineSegment2 = new LineSegment();
                    lineSegment2.Point = new Point(8.5095301, 6.2419558);
                    LineSegment lineSegment3= new LineSegment();
                    lineSegment3.Point = new Point(22.002934, 6.2419558);
                    LineSegment lineSegment4= new LineSegment();
                    lineSegment4.Point = new Point(22.002934, 10.902534);
                    LineSegment lineSegment5 = new LineSegment();
                    lineSegment5.Point = new Point(8.5095301, 10.902534);
                    LineSegment lineSegment6 = new LineSegment();
                    lineSegment6.Point = new Point(14.74649, 17.138);
                    LineSegment lineSegment7 = new LineSegment();
                    lineSegment7.Point = new Point(8.5704565, 17.138);
					LineSegment lineSegment8 = new LineSegment();
                    lineSegment8.Point = new Point(0,8.5690022);
                    PathSegmentCollection pathsegments= new PathSegmentCollection();
                    pathsegments.Add(lineSegment1);
					pathsegments.Add(lineSegment2);
					pathsegments.Add(lineSegment3);
					pathsegments.Add(lineSegment4);
					pathsegments.Add(lineSegment5);
					pathsegments.Add(lineSegment6);
					pathsegments.Add(lineSegment7);
                    pathsegments.Add(lineSegment8);
                    pathFigure.Segments = pathsegments;
                    PathFigureCollection pathFigureCollection = new PathFigureCollection();
                    pathFigureCollection.Add(pathFigure);
                    PathGeometry pathGeometry = new PathGeometry();
                    pathGeometry.Figures = pathFigureCollection;                    
                    Path arrow = new Path() {Fill = new SolidColorBrush(Colors.Black), Stretch = Stretch.Fill, Margin = new Thickness(8.83, 11.333, 9.167, 11.529) };
                    arrow.Data = pathGeometry;
#endif
                    backbutton.Children.Add(arrow);
                    return backbutton;
#else
                    return "";
#endif
                }
                else
                {
                    SfRadialMenu menu = menuItem as SfRadialMenu;
                    if (menu != null)
                    {
                        if (menu.Icon is string)
                        {                            
                            Image img = new Image();
                            img.Source = new BitmapImage(new Uri(menu.Icon.ToString(), UriKind.RelativeOrAbsolute));
                            img.Width = imgSize;
                            img.Height = imgSize;
                            return img;
                        }
                        else
                            return menu.Icon;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Throws an exception
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
#if !WINRT
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
#else
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
        {
            throw new NotImplementedException();
        }
    }

    public class MenuItemIconConverter : IValueConverter
    {
        private readonly double imgSize = 16;

#if !WINRT
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
#else
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
        {
            if (value is SfRadialMenuItem)
            {
                var menuItem = value as SfRadialMenuItem;
                Image img = new Image();
                img.Source = menuItem.Icon;
                img.Width = imgSize;
                img.Height = imgSize;
                return img;
            }
            else
                return null;
        }

#if !WINRT
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
#else
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
        {
            throw new NotImplementedException();
        }
    }
}
