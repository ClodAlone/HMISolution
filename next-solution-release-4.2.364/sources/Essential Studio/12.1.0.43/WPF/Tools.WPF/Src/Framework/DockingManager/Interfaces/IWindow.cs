// <copyright file="IWindow.cs" company="Syncfusion">
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
using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represent the Interface for the Iwindow
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public interface IWindow
    {
        /// <summary>
        /// Gets or sets the placement rectangle.
        /// </summary>
        /// <value>The placement rectangle.</value>
        Rect PlacementRectangle
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>The data context.</value>
        FrameworkElement InternalDataContext
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the docking manager.
        /// </summary>
        /// <value>The docking manager.</value>
        DockingManager DockingManager
        {
            get;
            set;
        }
       
        /// <summary>
        /// Gets or sets the primary element.
        /// </summary>
        /// <value>The primary element.</value>
        FrameworkElement PrimaryElement
        {
            get;
            set;
        }
       
        /// <summary>
        /// Gets or sets the float child.
        /// </summary>
        /// <value>The float child.</value>
        FrameworkElement FloatChild
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        /// <value>The child.</value>
        UIElement Child
        {
            get;
            set;
        }
       
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        UIElement Header
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether [hit test disabled].
        /// </summary>
        /// <value><c>true</c> if [hit test disabled]; otherwise, <c>false</c>.</value>
        bool HitTestDisabled
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is dragging.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is dragging; otherwise, <c>false</c>.
        /// </value>
        bool IsDragging
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is multi hosts container.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is multi hosts container; otherwise, <c>false</c>.
        /// </value>
        bool IsMultiHostsContainer
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        bool IsOpen
        {
            get;
            set;
        }
       
        /// <summary>
        /// Gets or sets a value indicating whether [allows transparency].
        /// </summary>
        /// <value><c>true</c> if [allows transparency]; otherwise, <c>false</c>.</value>
        bool AllowsTransparency
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the opacity.
        /// </summary>
        /// <value>The opacity.</value>
        double Opacity
        {
            get;
            set;
        }
       
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        double Width
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        double Height
        {
            get;
            set;
        }
       
        /// <summary>
        /// Completes the dragging.
        /// </summary>
        void CompleteDragging();
       
        /// <summary>
        /// Updates the is multi host property.
        /// </summary>
        void UpdateIsMultiHostProperty();
        
        /// <summary>
        /// Gets the visible hosts count.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>return DockedElementContainer.</returns>
        int GetVisibleHostsCount(DockedElementsContainer container);
       
        /// <summary>
        /// Sets the new primary element.
        /// </summary>
        /// <param name="element">The element.</param>
        void SetNewPrimaryElement(FrameworkElement element);
        
        /// <summary>
        /// Updates the data context.
        /// </summary>
        void UpdateDataContext();
        
        /// <summary>
        /// Sets the window on top.
        /// </summary>
        void SetWindowOnTop();
    }
}
