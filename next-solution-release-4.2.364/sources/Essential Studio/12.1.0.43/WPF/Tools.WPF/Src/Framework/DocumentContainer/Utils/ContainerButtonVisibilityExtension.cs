// <copyright file="ContainerButtonVisibilityExtension.cs" company="Syncfusion">
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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents extension for button visibility.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [MarkupExtensionReturnType(typeof(MultiBinding))]
    public class ContainerButtonVisibilityExtension : MarkupExtension
    {
        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Binding bindingEnabled = new Binding();
            Binding bindingBehavior = new Binding();
            MultiBinding bindingAll = new MultiBinding();

            bindingEnabled.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
            bindingEnabled.Path = new PropertyPath(FrameworkElement.IsEnabledProperty);

            bindingBehavior.Path = new PropertyPath(DocumentContainer.DisabledButtonsBehaviorProperty);
            bindingBehavior.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DocumentContainer), 1);

            bindingAll.Converter = new DisabledButtonsBehaviorToVisibilityConverter();

            bindingAll.Bindings.Add(bindingEnabled);
            bindingAll.Bindings.Add(bindingBehavior);
            return bindingAll;
        }
    }
}
