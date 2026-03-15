// <copyright file="Chart.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Input;
    using System.IO;
    using System.Windows.Xps;
    using System.Windows.Xps.Packaging;
    using System.IO.Packaging;
    using System.Printing;
    using System.Windows.Markup;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Collections;
    using System.Security.Permissions;
    using Microsoft.Win32;
    using Syncfusion.Licensing;
    using Syncfusion.Windows.Shared;
    using System.ComponentModel;
    using System.Xml;
    using System.Windows.Threading;
    using System.Xml.Serialization;
    using System.Text;
    using System.Collections.Generic;
using System.Collections.ObjectModel;

    
   
    internal static class ChartDictionaries
    {
        internal static ResourceDictionary GenericDictionary = new SharedResourceDictionary()
        {
            Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/generic.xaml", UriKind.RelativeOrAbsolute)
        };
        internal static ResourceDictionary GenericBaseDictionary = new SharedResourceDictionary()
        {
            Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/ChartVisualStyles.xaml", UriKind.RelativeOrAbsolute)
        };
        internal static ResourceDictionary GenericLegendDictionary = new SharedResourceDictionary()
        {
            Source = new Uri("/Syncfusion.Chart.Wpf;component/Legend/ChartLegend.xaml", UriKind.RelativeOrAbsolute)
        };
        internal static ResourceDictionary ChartAreaContextMenuDictionary = new SharedResourceDictionary()
        {
            Source = new Uri("/Syncfusion.Chart.Wpf;component/ChartArea/ChartAreaContextMenu.xaml", UriKind.RelativeOrAbsolute)
        };
        internal static ResourceDictionary LangDictionary = new SharedResourceDictionary()
        {
            Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/LangDictionary.xaml", UriKind.RelativeOrAbsolute)
        };
        internal static ResourceDictionary GenericSeriesGUIDictionary = new SharedResourceDictionary()
        {
            Source = new Uri("/Syncfusion.Chart.Wpf;component/ChartSeries/SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
        };
        internal static ResourceDictionary ChartPaletteBrushesDictionary = new SharedResourceDictionary()
        {
            Source = new Uri("/Syncfusion.Chart.Wpf;component/Core/ChartPaletteBrushes.xaml", UriKind.RelativeOrAbsolute)
        };

        internal static ResourceDictionary GetResourceDictionary(string uriSource)
        {
            if (SharedResourceDictionary._sharedDictionaries.ContainsKey(uriSource.ToLower()))
            {
                return SharedResourceDictionary._sharedDictionaries[uriSource.ToLower()];
            }
            return new SharedResourceDictionary()
            {
                Source = new Uri(uriSource, UriKind.RelativeOrAbsolute)
            };
        }
    }

    /// <summary>
    /// The shared resource dictionary is a specialized resource dictionary
    /// that loads it content only once. If a second instance with the same source
    /// is created, it only merges the resources from the cache.
    /// </summary>
    public class SharedResourceDictionary : ResourceDictionary, IDisposable
    {
        /// <summary>
        /// Internal cache of loaded dictionaries 
        /// </summary>
        public static Dictionary<string, ResourceDictionary> _sharedDictionaries =
            new Dictionary<string, ResourceDictionary>();

        /// <summary>
        /// Called when instance created for SharedResourceDictionary
        /// </summary>
        public SharedResourceDictionary()
        {
        }

        /// <summary>
        /// Local member of the source uri
        /// </summary>
        internal Uri _sourceUri;

        /// <summary>
        /// Gets or sets the uniform resource identifier (URI) to load resources from.
        /// </summary>
        public new Uri Source
        {
            get { return _sourceUri; }
            set
            {
                _sourceUri = value;
                //base.Source = value;
                if (_sharedDictionaries == null)
                    _sharedDictionaries = new Dictionary<string, ResourceDictionary>();
                try
                {
                    if (!_sharedDictionaries.ContainsKey(value.ToString().ToLower()))
                    {

                        // If the dictionary is not yet loaded, load it by setting
                        // the source of the base class
                        base.Source = value;
                        //("Synfusion_SharedResourceDictionary " + value.ToString());
                        // add it to the cache
                        _sharedDictionaries.Add(value.ToString().ToLower(), this);
                    }
                    else
                    {
                        // If the dictionary is already loaded, get it from the cache
                        MergedDictionaries.Add(_sharedDictionaries[value.ToString().ToLower()]);
                        base.Source = value;
                    }
                }
                catch (Exception ex)
                {
                    string Errormessage = "Syncfusion_SharedResourceDictionary " + value + " Error ";
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        throw new Exception(Errormessage, inner);
                    }
                }
            }
        }


        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            _sharedDictionaries.Clear();
            _sharedDictionaries = null;
        }

        #endregion
    }
}
