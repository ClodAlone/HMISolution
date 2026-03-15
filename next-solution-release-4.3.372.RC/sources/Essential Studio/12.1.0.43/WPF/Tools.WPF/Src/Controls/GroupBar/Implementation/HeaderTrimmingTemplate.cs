// <copyright file="HeaderTrimmingTemplate.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region file using
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the template with the text trimming for the string
    /// header.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class HeaderTrimmingTemplate : DataTemplateSelector
    {
        #region Initialization
        /// <summary>
        /// Replaces the simple string-content with the data template with trimming support.
        /// </summary>
        /// <param name="item">The <see cref="string"/> object.</param>
        /// <param name="container">Do not used.</param>
        /// <returns>The <see cref="System.Windows.DataTemplate"/> object with trimming support.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (null != item && item is string)
            {
                DataTemplate dataTemplate = new DataTemplate();
                dataTemplate.VisualTree = new FrameworkElementFactory(typeof(TextBlock));
                dataTemplate.VisualTree.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);

                Binding binding = new Binding();
                binding.Path = new PropertyPath(GroupBar.SelectedHeaderProperty);
                binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(GroupBar), 1);
                dataTemplate.VisualTree.SetBinding(TextBlock.TextProperty, binding);

                dataTemplate.Seal();

                return dataTemplate;
            }

            return null;
        }
        #endregion
    }
}
