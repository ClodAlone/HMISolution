#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for resourceManager
    /// </summary>
    public static class ResourceManager
    {
        #region Implementation
        /// <summary>
        /// Gets the series template.
        /// </summary>
        /// <param name="type">The type specify the datatype.</param>
        /// <param name="types">The charttype specify the chart series type.</param>
        /// <returns>Type : ResourceDirectory</returns>
        public static DataTemplate GetSeriesTemplate(Type type, ChartTypes types)
        {
#if SyncfusionFramework3_5
            ResourceDictionary resources = new SharedResourceDictionary();
            Stream str = type.Assembly.GetManifestResourceStream("Syncfusion.Windows.Chart.Themes.SeriesGUI.xaml");
            if (str != null)
            {
                StreamReader reader = new StreamReader(str);
                string xaml = reader.ReadToEnd();
                reader.Close();
                resources = (ResourceDictionary)XamlReader.Load(xaml);
            }
            else
            {
                resources = new SharedResourceDictionary()
                {
                    Source = new Uri("/Syncfusion.Chart.Silverlight;component/Themes/SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
                };
            }
#else
            ResourceDictionary resources = new SharedResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Chart.Silverlight;component/Themes/SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
            };
#endif
            return resources[types.ToString()] as DataTemplate;
        }

        /// <summary>
        /// Return DataTemplate value from the given values 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static DataTemplate GetSeriesTemplateWithEffects(Type type, ChartTypes types)
        {
#if SyncfusionFramework3_5
            ResourceDictionary resources = new SharedResourceDictionary();
            Stream str = type.Assembly.GetManifestResourceStream("Syncfusion.Windows.Chart.Themes.SeriesGUI.xaml");
            if (str != null)
            {
                StreamReader reader = new StreamReader(str);
                string xaml = reader.ReadToEnd();
                reader.Close();
                resources = (SharedResourceDictionary)XamlReader.Load(xaml);
            }
            else
            {
                resources = new SharedResourceDictionary()
                {
                    Source = new Uri("/Syncfusion.Chart.Silverlight;component/Themes/SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
                };
            }
#else
            ResourceDictionary resources = new SharedResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Chart.Silverlight;component/Themes/SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
            };
#endif
            return resources[types.ToString() + "_Effects"] as DataTemplate;
        }

        /// <summary>
        /// Gets the Adornments Template.
        /// </summary>
        /// <param name="type">Type specified is a database</param>
        /// <param name="template">XAML Key's name</param>
        /// <returns>Type : DataTemplate</returns>
        public static DataTemplate GetAdornmentsTemplate(Type type, string template)
        {
#if SyncfusionFramework3_5
            ResourceDictionary resources = new SharedResourceDictionary();
            Stream str = type.Assembly.GetManifestResourceStream("Syncfusion.Windows.Chart.Themes.SeriesGUI.xaml");
            if (str != null)
            {
                StreamReader reader = new StreamReader(str);
                string xaml = reader.ReadToEnd();
                reader.Close();
                resources = (SharedResourceDictionary)XamlReader.Load(xaml);
            }
            else
            {
                resources = new SharedResourceDictionary()
                {
                    Source = new Uri("/Syncfusion.Chart.Silverlight;component/Themes/SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
                };
            }
#else
             ResourceDictionary resources = new SharedResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Chart.Silverlight;component/Themes/SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
            };
#endif
            return resources[template] as DataTemplate;
        }

        /// <summary>
        /// Return brush value from the given values
        /// </summary>
        /// <param name="type"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static Brush GetSeriesLayer(Type type, string template)
        {
#if SyncfusionFramework3_5
            ResourceDictionary resources = new SharedResourceDictionary();
            Stream str = type.Assembly.GetManifestResourceStream("Syncfusion.Windows.Chart.Themes.SeriesGUI.xaml");
            if (str != null)
            {
                StreamReader reader = new StreamReader(str);
                string xaml = reader.ReadToEnd();
                reader.Close();
                resources = (SharedResourceDictionary)XamlReader.Load(xaml);
            }
            else
            {
                resources = new SharedResourceDictionary()
                {
                    Source = new Uri("/Syncfusion.Chart.Silverlight;component/Themes/SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
                };
            }
#else
             ResourceDictionary resources = new SharedResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Chart.Silverlight;component/Themes/SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
            };
#endif
            return resources[template] as Brush;
        }
        #endregion
    }
}
