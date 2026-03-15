#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a class to clip the contents of element to fit into the size of the containing element.
    /// </summary>
    public class Clip
    {
        #region Attached Properties

        #region ToBounds
        /// <summary>
        /// Gets the bound value.
        /// </summary>
        public static bool GetToBounds(DependencyObject depObj)
        {
            return (bool)depObj.GetValue(ToBoundsProperty);
        }

        /// <summary>
        /// Sets the bound value.
        /// </summary>
        public static void SetToBounds(DependencyObject depObj, bool clipToBounds)
        {
            depObj.SetValue(ToBoundsProperty, clipToBounds);
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ToBounds.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ToBoundsProperty =
            DependencyProperty.RegisterAttached("ToBounds", typeof(bool), typeof(Clip), new PropertyMetadata(false, OnToBoundsPropertyChanged));

        private static void OnToBoundsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fe = d as FrameworkElement;
            if (fe != null)
            {
                ClipToBounds(fe);

                // whenever the element which this property is attached to is loaded
                // or re-sizes, we need to update its clipping geometry
                fe.Loaded += fe_Loaded;
                fe.SizeChanged += fe_SizeChanged;
            }
        }
        #endregion

        #endregion

        #region Methods

        #region Clip To bounds

        private static void ClipToBounds(FrameworkElement fe)
        {
            fe.Clip = GetToBounds(fe) ? new RectangleGeometry { Rect = new Rect(0, 0, fe.ActualWidth, fe.ActualHeight) } : null;
        }

        #endregion

        #endregion

        #region Events

        #region Size Changed

        static void fe_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ClipToBounds(sender as FrameworkElement);
        }

        #endregion

        #region Loaded

        static void fe_Loaded(object sender, RoutedEventArgs e)
        {
            ClipToBounds(sender as FrameworkElement);
        }

        #endregion

        #endregion
    }
}
