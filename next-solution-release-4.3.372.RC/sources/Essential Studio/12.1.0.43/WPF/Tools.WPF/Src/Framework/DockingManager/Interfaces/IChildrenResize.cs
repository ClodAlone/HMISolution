// <copyright file="IChildrenResize.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Provides an opportunity to resize some elements.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public interface IChildrenResize : IDesiredSize
    {
        /// <summary>
        /// Sets new width value of the element.
        /// </summary>
        /// <param name="width">new width value</param>
        void SetWidth(double width);

        /// <summary>
        /// Sets new height value of the element.
        /// </summary>
        /// <param name="height">new height value</param>
        void SetHeight(double height);
    }
}
