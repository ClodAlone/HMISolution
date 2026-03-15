// <copyright file="FindWindowExtension.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Markup extension, used to create the binding to the MDI window among the parents.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [MarkupExtensionReturnType(typeof(Binding))]
    public class FindWindowExtension : MarkupExtension
    {
        #region Public methods
        /// <summary>
        /// Gets binding, used to find MDI window among parents.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Binding binding = new Binding();
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(MDIWindow), 1);

            return binding;
        }
        #endregion
    }
}
