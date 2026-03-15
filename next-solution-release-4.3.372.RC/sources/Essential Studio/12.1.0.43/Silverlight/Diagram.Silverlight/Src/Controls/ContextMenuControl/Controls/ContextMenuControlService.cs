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
using Syncfusion.Windows.Diagram;
using System.Windows.Resources;
using System.IO;
using System.Windows.Markup;

namespace Syncfusion.Windows.Diagram
{
    public static class ContextMenuControlService
    {
        /// <summary>
        /// Gets the value of the ContextMenuControl property of the specified object.
        /// </summary>
        /// <param name="obj">Object to query concerning the ContextMenuControl property.</param>
        /// <returns>Value of the ContextMenuControl property.</returns>
        public static ContextMenuControl GetContextMenuControl(DependencyObject obj)
        {
            return (ContextMenuControl)obj.GetValue(ContextMenuControlProperty);
        }

        /// <summary>
        /// Sets the value of the ContextMenuControl property of the specified object.
        /// </summary>
        /// <param name="obj">Object to set the property on.</param>
        /// <param name="value">Value to set.</param>
        public static void SetContextMenuControl(DependencyObject obj, ContextMenuControl value)
        {
            obj.SetValue(ContextMenuControlProperty, value);
        }

        /// <summary>
        /// Identifies the ContextMenuControl attached property.
        /// </summary>
        public static readonly DependencyProperty ContextMenuControlProperty = DependencyProperty.RegisterAttached(
            "ContextMenuControl",
            typeof(ContextMenuControl),
            typeof(ContextMenuControlService),
            new PropertyMetadata(null, OnContextMenuChanged));

        /// <summary>
        /// Handles changes to the ContextMenuControl DependencyProperty.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnContextMenuChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = o as FrameworkElement;
            if (null != element)
            {
                ContextMenuControl oldContextMenu = e.OldValue as ContextMenuControl;
                if (null != oldContextMenu)
                {
                    oldContextMenu.Owner = null;
                }
                ContextMenuControl newContextMenu = e.NewValue as ContextMenuControl;
                if (null != newContextMenu)
                {
                    newContextMenu.Owner = element;
                }
            }
        }
    }
}
