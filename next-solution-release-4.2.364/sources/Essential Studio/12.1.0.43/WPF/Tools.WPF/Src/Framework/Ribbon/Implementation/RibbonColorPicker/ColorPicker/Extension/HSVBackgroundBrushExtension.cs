// <copyright file="HSVBackgroundBrushExtension.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    /// <summary>
    /// HSVBackgroundBrushExtension class.
    /// </summary>
    /// <property name="flag" value="Finished"/>
    [MarkupExtensionReturnType(typeof(Brush))]
    internal class xHSVBackgroundBrushExtension : MarkupExtension
    {
        #region Private fields
        /// <summary>
        /// Converter for MultiBinding.
        /// </summary>
        /// <property name="flag" value="Finished" />
        private static ColorToHSVBackgroundConverter m_converter;

        /// <summary>
        /// Key for MultiBinding.
        /// </summary>
        /// <property name="flag" value="Finished" />
        private string m_key;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="HSVBackgroundBrushExtension"/> class.
        /// class.
        /// </summary>
        /// <property name="flag" value="Finished"/>
        static xHSVBackgroundBrushExtension()
        {
            m_converter = new ColorToHSVBackgroundConverter();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HSVBackgroundBrushExtension"/>
        /// class.
        /// </summary>
        /// <param name="key">The key to be stored.</param>
        /// <property name="flag" value="Finished"/>
        public xHSVBackgroundBrushExtension(string key)
        {
            m_key = key;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// When implemented in a derived class, returns an object that
        /// is set as the value of the target property for this markup
        /// extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide
        /// services for the markup
        /// extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension
        /// is applied.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            MultiBinding multiBinding = new MultiBinding();
            multiBinding.Converter = m_converter;
            multiBinding.ConverterParameter = m_key;

            Binding binding = new Binding();
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(xColorEdit), 1);
            binding.Path = new PropertyPath(xColorEdit.HSVProperty);

            multiBinding.Bindings.Add(binding);

            binding = new Binding();
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(xColorEdit), 1);
            binding.Path = new PropertyPath(xColorEdit.HProperty);

            multiBinding.Bindings.Add(binding);

            binding = new Binding();
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(xColorEdit), 1);
            binding.Path = new PropertyPath(xColorEdit.SProperty);

            multiBinding.Bindings.Add(binding);

            binding = new Binding();
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(xColorEdit), 1);
            binding.Path = new PropertyPath(xColorEdit.VProperty);

            multiBinding.Bindings.Add(binding);

            return multiBinding;
        }
        #endregion
    }
}

