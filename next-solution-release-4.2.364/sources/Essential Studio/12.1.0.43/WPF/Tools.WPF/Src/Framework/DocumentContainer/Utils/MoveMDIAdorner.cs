// <copyright file="MoveMDIAdorner.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Present adorner for move MDI window.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class MoveMDIAdorner : TemplatedAdornerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveMDIAdorner"/> class.
        /// </summary>
        /// <param name="shadedElement">The shaded element.</param>
        public MoveMDIAdorner(UIElement shadedElement)
            : base(shadedElement)
        {
        }
    }
}
