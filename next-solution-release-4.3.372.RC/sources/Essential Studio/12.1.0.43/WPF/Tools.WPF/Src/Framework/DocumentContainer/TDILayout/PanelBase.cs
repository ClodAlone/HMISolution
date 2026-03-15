// <copyright file="PanelBase.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents abstract class for tab panels.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class PanelBase : FrameworkElement
    {
        /// <summary>
        /// Removes the element.
        /// </summary>
        /// <param name="element">The element.</param>
        public abstract void RemoveElement(UIElement element);
        
        /// <summary>
        /// Adds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        public abstract void AddElement(UIElement element);
        
        /// <summary>
        /// Determines whether [has other element] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="next">if set to <c>true</c> [next].</param>
        /// <returns>
        /// <c>true</c> if [has other element] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool HasElement(UIElement element, bool next)
        {
            return false;
        }
        
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <param name="first">if set to <c>true</c> [first].</param>
        /// <returns>UIElement GetElement</returns>
        public virtual UIElement GetElement(bool first)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Gets the opposite element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>UIElement element</returns>
        public virtual UIElement GetOppositeElement(UIElement element)
        {
            throw new NotImplementedException();
        }
    }
}
