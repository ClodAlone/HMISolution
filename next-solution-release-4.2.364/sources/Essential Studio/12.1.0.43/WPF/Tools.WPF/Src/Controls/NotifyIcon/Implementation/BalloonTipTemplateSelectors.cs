// <copyright file="BalloonTipTemplateSelectors.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class representing the Balloon tip header template selector
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class BalloonTipHeaderTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate"/> based on custom logic.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>
        /// Returns a <see cref="T:System.Windows.DataTemplate"/> or null. The default value is null.
        /// </returns>
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            DataTemplate temp = null;
            BalloonTip ballon = VisualUtils.FindAncestor((Visual)container, typeof(BalloonTip)) as BalloonTip;
            if (ballon != null)
            {
                if (ballon.BalloonTipTitle != null)
                {
                    ResourceDictionary dictionary = new ResourceDictionary();
                    dictionary.Source = new Uri("pack://application:,,,/Syncfusion.Tools.WPF;component/Controls/NotifyIcon/Themes/generic.xaml", UriKind.RelativeOrAbsolute);
                    temp = (DataTemplate)dictionary["BalloonTipHeaderContentTemplate"];
                }
                else
                {
                    temp = ballon.HeaderTemplate;
                }
            }

            return temp;
        }
    }

    /// <summary>
    /// Selects the Balloon tip's content template.
    /// </summary>
    public class BalloonTipContentTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate"/> based on custom logic.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>
        /// Returns a <see cref="T:System.Windows.DataTemplate"/> or null. The default value is null.
        /// </returns>
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            DataTemplate temp = null;
            BalloonTip ballon = (container as FrameworkElement).TemplatedParent as BalloonTip;
            if (ballon != null)
            {
                if (ballon.BalloonTipText != null)
                {
                    ResourceDictionary dictionary = new ResourceDictionary();
                    dictionary.Source = new Uri("pack://application:,,,/Syncfusion.Tools.WPF;component/Controls/NotifyIcon/Themes/generic.xaml", UriKind.RelativeOrAbsolute);
                    temp = (DataTemplate)dictionary["BalloonTipContentTemplate"];
                }
                else
                {
                    temp = ballon.ContentTemplate;
                }
            }

            return temp;
        }
    }
}
