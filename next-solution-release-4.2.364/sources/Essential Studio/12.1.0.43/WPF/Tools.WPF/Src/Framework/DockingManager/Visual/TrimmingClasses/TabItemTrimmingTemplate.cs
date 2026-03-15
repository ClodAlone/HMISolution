// <copyright file="TabItemTrimmingTemplate.cs" company="Syncfusion">
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
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TabItemTrimmingTemplate : DataTemplateSelector
    {
        #region Constants
        /// <summary>
        /// Represents Element name;
        /// </summary>
        private const string ElementName = "Border";

        /// <summary>
        /// Represents Binding Mask.
        /// </summary>
        private const string BindingMask = "(0).(1)";
        #endregion

        #region Implementation
        /// <summary>
        /// Returns a <see cref="DataTemplate"/> based on custom logic that is used in 
        /// <see cref="TabItem"/> template for presenting DockingManager's child.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>return null</returns>
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (null != item && item is string)
            {
                DataTemplate dataTemplate = new DataTemplate
                {
                    VisualTree = new FrameworkElementFactory(typeof(TextBlock))
                };
                dataTemplate.VisualTree.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);

                Binding binding = new Binding
                {
                    ElementName = ElementName, 
                    Path = new PropertyPath(BindingMask, Border.DataContextProperty, DockingManager.HeaderProperty)
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