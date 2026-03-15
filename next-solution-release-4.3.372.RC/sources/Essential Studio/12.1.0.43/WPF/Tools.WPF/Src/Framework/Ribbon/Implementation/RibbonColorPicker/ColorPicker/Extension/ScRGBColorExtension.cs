// <copyright file="ScRGBColorExtension.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    /// <summary>
    /// Class implements converting into MultiBinding.
    /// </summary>
    /// <property name="flag" value="Finished"/>
    internal class xScRGBColorExtension : MarkupExtension
    {
        #region Private fields
        /// <summary>
        /// Converter for MultiBinding.
        /// </summary>
        /// <property name="flag" value="Finished" />
        private static xRGBToStringConverter m_converter;

        /// <summary>
        /// Key for MultiBinding.
        /// </summary>
        /// <property name="flag" value="Finished" />
        private string m_key;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="ScRGBColorExtension"/> class.
        /// </summary>
        /// <property name="flag" value="Finished"/>
        static xScRGBColorExtension()
        {
            m_converter = new xRGBToStringConverter();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScRGBColorExtension"/>
        /// class.
        /// </summary>
        /// <param name="key">The key to be stored.</param>
        /// <property name="flag" value="Finished"/>
        public xScRGBColorExtension(string key)
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
            multiBinding.ConverterParameter = m_key;
            multiBinding.Converter = m_converter;

            Binding binding = new Binding();
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(xColorEdit), 1);
            binding.Path = new PropertyPath(xColorEdit.ColorProperty);

            multiBinding.Bindings.Add(binding);

            binding = new Binding();
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(xColorEdit), 1);
            binding.Path = new PropertyPath(xColorEdit.IsScRGBColorProperty);

            multiBinding.Bindings.Add(binding);

            return multiBinding;
        }
        #endregion
    }
}
