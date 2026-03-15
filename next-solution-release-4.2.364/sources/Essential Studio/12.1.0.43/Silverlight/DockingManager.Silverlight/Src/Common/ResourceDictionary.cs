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
using System.Windows.Resources;
using System.ComponentModel;
using System.IO;
using System.Windows.Markup;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represent the Resource Dictionary Class.
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
        /// <param name="d">The d.</param>
        /// <returns>The Resource Dictionary.</returns>
        public static ResourceDictionary GetMergedDictionaries(DependencyObject d)
        {
            if (d == null)
            {
                throw new ArgumentNullException("d");
            }

            return (ResourceDictionary)d.GetValue(MergedDictionariesProperty);
        }

        /// <summary>
        /// Sets the merged dictionaries.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="dictionary">The dictionary.</param>
        public static void SetMergedDictionaries(DependencyObject d, ResourceDictionary dictionary)
        {
            if (d == null)
            {
                throw new ArgumentNullException("d");
            }

            d.SetValue(MergedDictionariesProperty, dictionary);
        }

        /// <summary>
        /// Identifies the Dependency Property MergedDictionariesProperty.
        /// </summary>
        public static readonly DependencyProperty MergedDictionariesProperty = DependencyProperty.RegisterAttached(
            "MergedDictionaries",
            typeof(ResourceDictionary),
            typeof(ResourceDictionary),
            new PropertyMetadata(new PropertyChangedCallback(OnMergedDictionariesPropertyChanged)));

        static ResourceDictionary sample = null;

        /// <summary>
        /// Called when [merged dictionaries property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnMergedDictionariesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ResourceDictionary dictionaryToMerge = e.NewValue as ResourceDictionary;

            if (d is System.Windows.ResourceDictionary)
            { 
                dictionaryToMerge.OnMergedDictionariesChanged((d as System.Windows.ResourceDictionary));
            }
        }

        /// <summary>
        /// Gets or sets the resource collection.
        /// </summary>
        /// <value>The resource collection.</value>
        protected static internal Dictionary<string, object> ResourceCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the resource collection.
        /// </summary>
        /// <returns>The Resource Dictionary.</returns>
        protected static internal ResourceDictionary GetResourceCollection()
        {
            return sample;
        }

        /// <summary>
        /// Called when [merged dictionaries changed].
        /// </summary>
        /// <param name="targetDictionary">The target dictionary.</param>
        protected virtual void OnMergedDictionariesChanged(System.Windows.ResourceDictionary targetDictionary)
        {
            sample = this;
            ResourceCollection = new Dictionary<string, object>();
            if (targetDictionary == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(Keys))
            {
                throw new Exception("Keys property is not defined");
            }

            System.Windows.ResourceDictionary dictionaryToMerge = GetResourceDictionary();

            //// NOTE: Silverlight 2 does not provide an enumerator or iteration option
            //// for resource dictionaries

            foreach (string key in Keys.Split(",".ToCharArray()))
            {
                string kv = key.Trim();

                if (!string.IsNullOrEmpty(kv))
                {
                    if (!dictionaryToMerge.Contains(kv))
                    {
                        throw new Exception(string.Format("Key '{0}' does not exist in resource dictionary '{1}'", kv, Source));
                    }

                    if (!targetDictionary.Contains(kv))
                    {
                        targetDictionary.Add(kv, dictionaryToMerge[kv]);
                        ResourceCollection.Add(kv, dictionaryToMerge[kv]);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the resource dictionary.
        /// </summary>
        /// <returns>The Resource Dictionary.</returns>
        protected internal virtual System.Windows.ResourceDictionary GetResourceDictionary()
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
