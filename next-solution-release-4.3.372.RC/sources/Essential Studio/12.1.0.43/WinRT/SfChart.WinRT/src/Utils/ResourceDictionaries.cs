#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.UI.Xaml;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Contains Chart resource dictionaries
    /// </summary>
    [ClassReference(IsReviewed = false)]
    internal static class ChartDictionaries
    {
        internal static ResourceDictionary GenericLegendDictionary = new ResourceDictionary()
        {
#if WINDOWS_PHONE8
            Source = new Uri(@"/Syncfusion.SfChart.WP8;component/Themes/Generic.Legend.xaml", UriKind.Relative)
#endif
#if WINDOWS_PHONE7
            Source = new Uri(@"/Syncfusion.SfChart.WP7;component/Themes/Generic.Legend.xaml", UriKind.Relative)
#endif
#if SILVERLIGHT_UNCOMMON
             Source = new Uri(@"/Syncfusion.SfChart.Silverlight;component/Themes/Generic.Legend.xaml", UriKind.Relative)
#endif
#if WPF
            Source = new Uri(@"/Syncfusion.SfChart.WPF;component/Themes/Generic.Legend.xaml", UriKind.Relative)
#endif
#if NETFX_CORE
            Source = new Uri(@"ms-appx:///Syncfusion.SfChart.WinRT/Themes/Generic.Legend.xaml")
#endif
        };

        internal static ResourceDictionary GenericSymbolDictionary = new ResourceDictionary()
        {
#if WINDOWS_PHONE8
            Source = new Uri(@"/Syncfusion.SfChart.WP8;component/Themes/Generic.Symbol.xaml", UriKind.Relative)
#endif
#if WINDOWS_PHONE7
            Source = new Uri(@"/Syncfusion.SfChart.WP7;component/Themes/Generic.Symbol.xaml", UriKind.Relative)
#endif
#if SILVERLIGHT_UNCOMMON
            Source = new Uri(@"/Syncfusion.SfChart.Silverlight;component/Themes/Generic.Symbol.xaml", UriKind.Relative)
#endif
#if WPF
            Source = new Uri(@"/Syncfusion.SfChart.WPF;component/Themes/Generic.Symbol.xaml", UriKind.Relative)
#endif
#if NETFX_CORE
            Source = new Uri(@"ms-appx:///Syncfusion.SfChart.WinRT/Themes/Generic.Symbol.xaml")
#endif

        };

        internal static ResourceDictionary GenericCommonDictionary = new ResourceDictionary()
        {
#if WINDOWS_PHONE8
            Source = new Uri(@"/Syncfusion.SfChart.WP8;component/Themes/Generic.Common.xaml", UriKind.Relative)
#endif
#if WINDOWS_PHONE7
            Source = new Uri(@"/Syncfusion.SfChart.WP7;component/Themes/Generic.Common.xaml", UriKind.Relative)
#endif
#if SILVERLIGHT_UNCOMMON
            Source = new Uri(@"/Syncfusion.SfChart.Silverlight;component/Themes/Generic.Common.xaml", UriKind.Relative)
#endif
#if WPF
            Source = new Uri(@"/Syncfusion.SfChart.WPF;component/Themes/Generic.Common.xaml", UriKind.Relative)
#endif
#if NETFX_CORE
            Source = new Uri(@"ms-appx:///Syncfusion.SfChart.WinRT/Themes/Generic.Common.xaml")
#endif
        };
    }
}
