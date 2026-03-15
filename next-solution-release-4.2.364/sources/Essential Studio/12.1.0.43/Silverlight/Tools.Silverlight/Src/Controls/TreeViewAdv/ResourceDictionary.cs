#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Resources;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the ResourceDictionary Class.
    /// </summary>
    public class ResourceDictionary
    {
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        [TypeConverter(typeof(UriTypeConverter))]
        public Uri Source { get; set; }
        /// <summary>
        /// Gets or sets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public string Keys { get; set; }

        /// <summary>
        /// Gets the merged dictionaries.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static ResourceDictionary GetMergedDictionaries(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj", "Argument missing or Null");
            }

            return (ResourceDictionary)obj.GetValue(MergedDictionariesProperty);
        }

        /// <summary>
        /// Sets the merged dictionaries.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="resource">The resource.</param>
        public static void SetMergedDictionaries(DependencyObject d, ResourceDictionary resource)
        {
            if (d == null)
            {
                throw new ArgumentNullException("Argument value \"d\" is null");
            }

            d.SetValue(MergedDictionariesProperty, resource);
        }

        /// <summary>
        /// Identified the MergedDictionaries Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MergedDictionariesProperty =
            DependencyProperty.RegisterAttached("MergedDictionaries", typeof(ResourceDictionary), typeof(ResourceDictionary), new PropertyMetadata(new PropertyChangedCallback(OnMergedDictionariesPropertyChanged)));

        /// <summary>
        /// Called when [merged dictionaries property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnMergedDictionariesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ResourceDictionary file = e.NewValue as ResourceDictionary;
            if (d is System.Windows.ResourceDictionary)
            {
                file.OnMergedDictionariesChanged((d as System.Windows.ResourceDictionary));
            }
        }

        /// <summary>
        /// Called when [merged dictionaries changed].
        /// </summary>
        /// <param name="targetDictionary">The target dictionary.</param>
        protected virtual void OnMergedDictionariesChanged(System.Windows.ResourceDictionary targetDictionary)
        {
            if (targetDictionary == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(Keys))
            {
                throw new NullReferenceException("Keys property is not defined");
            }
            
            System.Windows.ResourceDictionary dictionaryToMerge = GetResourceDictionary();

            foreach (string key in Keys.Split(",".ToCharArray()))
            {
                string kv = key.Trim();
                if (!string.IsNullOrEmpty(kv))
                {
                    if (!dictionaryToMerge.Contains(kv))
                    {
                        throw new Exception("Invalid key name or given key does not exist in the ResourceDictionary");
                    }

                    if (!targetDictionary.Contains(kv))
                    {
                        targetDictionary.Add(kv, dictionaryToMerge[kv]);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the resource dictionary.
        /// </summary>
        /// <returns></returns>
        protected virtual System.Windows.ResourceDictionary GetResourceDictionary()
        {
            if (Source == null)
            {
                throw new Exception("Source property is not defined");
            }

            StreamResourceInfo resourceInfo = Application.GetResourceStream(Source);
            if (resourceInfo != null && resourceInfo.Stream != null)
            {
                using (StreamReader reader = new StreamReader(resourceInfo.Stream))
                {
                    string xaml = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(xaml))
                    {
                        return XamlReader.Load(xaml) as System.Windows.ResourceDictionary;
                    }
                }
            }

            throw new Exception(string.Format("Resource dictionary '{0}' does not exist", Source));
        }
    }
}