// <copyright file="IDesiredSize.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Provides a desired size for some element.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public interface IDesiredSize
    {
        /// <summary>
        /// Gets or sets the size of the desired.
        /// </summary>
        /// <value>The size of the desired.</value>
        Size DesiredSize
        {
            get;
            set;
        }
    }
}
