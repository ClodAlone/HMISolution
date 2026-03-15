// <copyright file="FloatTrimmingTemplate.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
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
    public class FloatTrimmingTemplate : DataTemplateSelector
    {
        #region Constatnts
        /// <summary>
        /// Represents binding mask.
        /// </summary>
        private const string BindingMask = "(0).(1).(2)";
        #endregion

        #region Implemenation
        /// <summary>
        /// Returns a <see cref="DataTemplate"/> based on custom logic that is used in 
        /// <see cref="FloatWindowBorder"/> header template for presenting DockingManager's child.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>return null.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (null != item && (item is string))
            {
                DataTemplate dataTemplate = new DataTemplate
                {
                    VisualTree = new FrameworkElementFactory(typeof(TextBlock))
                };
                dataTemplate.VisualTree.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);

                Binding binding = new Binding
                {
                    Path = new PropertyPath(BindingMask, DockingManager.InternalDataContextProperty, DockingManager.InternalDataContextProperty, DockingManager.HeaderProperty),
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(IWindow), 1)
                };
                dataTemplate.VisualTree.SetBinding(TextBlock.TextProperty, binding);

                dataTemplate.Seal();

                return dataTemplate;
            }

            return null;
        }
        #endregion
    }
}