// <copyright file="DocumentHeaderTrimmingTemplate.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Provides a way to choose a <see cref="DataTemplate"/> based on the data
    /// object and the data-bound element.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DocumentHeaderTrimmingTemplate : DataTemplateSelector
    {
        #region Constants
        /// <summary>
        /// Represents BINDING_MASK
        /// </summary>
        private const string BINDING_MASK = "(0).(1)";
        #endregion

        #region Implementation
        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate"/> based on custom logic.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>
        /// Returns a <see cref="T:System.Windows.DataTemplate"/> or null. The default value is null.
        /// </returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (null != item && item is string)
            {
                DataTemplate dataTemplate = new DataTemplate();
                dataTemplate.VisualTree = new FrameworkElementFactory(typeof(TextBlock));
                dataTemplate.VisualTree.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);

                Binding binding = new Binding();
                binding.Path = new PropertyPath(BINDING_MASK, MDIWindow.ContentProperty, DocumentContainer.HeaderProperty);
                binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(MDIWindow), 1);

                dataTemplate.VisualTree.SetBinding(TextBlock.TextProperty, binding);
                dataTemplate.Seal();
                return dataTemplate;
            }

            return null;
        }
        #endregion
    }
}
