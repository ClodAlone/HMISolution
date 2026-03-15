#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows.Input;
using System.Windows;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Provides the system implementation for displaying a ContextMenuAdv.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public static class ContextMenuAdvService
    {
        /// <summary>
        /// Gets the value of the ContextMenuAdv property of the specified object.
        /// </summary>
        /// <param name="obj">Object to query concerning the ContextMenuAdv property.</param>
        /// <returns>Value of the ContextMenuAdv property.</returns>
        public static ContextMenuAdv GetContextMenuAdv(DependencyObject obj)
        {
            return (ContextMenuAdv)obj.GetValue(ContextMenuAdvProperty);
        }

        /// <summary>
        /// Sets the value of the ContextMenuAdv property of the specified object.
        /// </summary>
        /// <param name="obj">Object to set the property on.</param>
        /// <param name="value">Value to set.</param>
        public static void SetContextMenuAdv(DependencyObject obj, ContextMenuAdv value)
        {
            obj.SetValue(ContextMenuAdvProperty, value);
        }

        /// <summary>
        /// Identifies the ContextMenuAdv attached property.
        /// </summary>
        public static readonly DependencyProperty ContextMenuAdvProperty = DependencyProperty.RegisterAttached(
            "ContextMenuAdv",
            typeof(ContextMenuAdv),
            typeof(ContextMenuAdvService),
            new PropertyMetadata(null, OnContextMenuChanged));

        /// <summary>
        /// Handles changes to the ContextMenuAdv DependencyProperty.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnContextMenuChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = o as FrameworkElement;
            if (null != element)
            {
                ContextMenuAdv oldContextMenu = e.OldValue as ContextMenuAdv;
                if (null != oldContextMenu)
                {
                    oldContextMenu.Owner = null;
                }
                ContextMenuAdv newContextMenu = e.NewValue as ContextMenuAdv;
                if (null != newContextMenu)
                {
                    newContextMenu.Owner = element;
                }
            }
        }
    }
}
