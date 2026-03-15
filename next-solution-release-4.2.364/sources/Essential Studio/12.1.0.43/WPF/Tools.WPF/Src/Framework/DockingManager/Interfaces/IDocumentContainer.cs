// <copyright file="IDocumentContainer.cs" company="Syncfusion">
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
    /// Base interface for container instance in DockingManager.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public interface IDocumentContainer : IFlipOwner
    {
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        DocumentCollection Items
        {
            get;
        }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        Style Style
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the MDI layout.
        /// </summary>
        /// <param name="layout">The layout.</param>
        void SetMDILayout(MDILayout layout);
        
        /// <summary>
        /// Clears the active document.
        /// </summary>
        /// <param name="newActiveDocument">The new active document.</param>
        void ValidateActiveDocument(UIElement newActiveDocument);
        
        /// <summary>
        /// Updates the layout.
        /// </summary>
        /// <param name="statePersist">if set to <c>true</c> [state persist].</param>
        void UpdateLayout(bool statePersist);
    }
}