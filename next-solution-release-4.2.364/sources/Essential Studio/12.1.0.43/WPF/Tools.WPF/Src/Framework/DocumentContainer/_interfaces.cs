// <copyright file="_interfaces.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents interface for layout panel in DocumentContainer.
    /// </summary>
    public interface ILayoutPanel : IDisposable
    {
        /// <summary>
        /// Creates the child document params.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <returns>ChildDocument Params</returns>
        ChildDocumentParams CreateChildDocumentParams(FrameworkElement element, bool isActive);

        /// <summary>
        /// Gets the ordered items.
        /// </summary>
        /// <returns>Ilist Control Items</returns>
        IList<Control> GetOrderedItems();
        
        /// <summary>
        /// Sets the active item.
        /// </summary>
        /// <param name="activeItem">The active item.</param>
        void SetActiveItem(FrameworkElement activeItem);
        
        /// <summary>
        /// Determines whether this instance can switch.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance can switch; otherwise, <c>false</c>.
        /// </returns>
        bool CanSwitch();
        
        /// <summary>
        /// Sets the focus.
        /// </summary>
        void SetFocus();
        
        /// <summary>
        /// Gets the Content.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        /// <returns>UIElement content</returns>
        UIElement GetContent(Control wrapper);
        
        /// <summary>
        /// Resets the visible list.
        /// </summary>
        void ResetVisibleList();
        
        /// <summary>
        /// Forwards the switch immediate.
        /// </summary>
        /// <param name="firstTabulation">if set to <c>true</c> [first tabulation].</param>
        /// <param name="isKeepCircle">if set to <c>true</c> [is keep circle].</param>
        void ForwardSwitchImmediate(bool firstTabulation, bool isKeepCircle);
        
        /// <summary>
        /// Back forward the switch immediate.
        /// </summary>
        void BackforwardSwitchImmediate();
        
        /// <summary>
        /// Sets the active document.
        /// </summary>
        /// <param name="element">The element.</param>
        void SetActiveDocument(UIElement element);
        
        /// <summary>
        /// Sets the active window.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        void SetActiveWindow(Control wrapper);
        
        /// <summary>
        /// Updates the after persist load.
        /// </summary>
        void UpdateAfterPersistLoad();
    }

    /// <summary>
    /// Presents interface for VistaFlip owner.
    /// </summary>
    public interface IVistaFlipOwner : IFlipOwner
    {
        /// <summary>
        /// Gets the size of the render.
        /// </summary>
        /// <value>The size of the render.</value>
        Size RenderSize
        {
            get;
        }
        
        /// <summary>
        /// Gets a value indicating whether this instance is items in full screen.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is items in full screen; otherwise, <c>false</c>.
        /// </value>
        bool IsItemsInFullScreen
        {
            get;
        }
        
        /// <summary>
        /// Gets the first flip item opacity.
        /// </summary>
        /// <value>The first flip item opacity.</value>
        double FirstFlipItemOpacity
        {
            get;
        }
        
        /// <summary>
        /// Gets the opacity factor of vista flip.
        /// </summary>
        /// <value>The opacity factor of vista flip.</value>
        double OpacityFactorOfVistaFlip
        {
            get;
        }
        
        /// <summary>
        /// Gets the factory of view vista flip.
        /// </summary>
        /// <value>The factory of view vista flip.</value>
        double FactoryOfViewVistaFlip
        {
            get;
        }
        
        /// <summary>
        /// Gets the duration of the vista flip animation.
        /// </summary>
        /// <value>The duration of the vista flip animation.</value>
        Duration VistaFlipAnimationDuration
        {
            get;
        }
        
        /// <summary>
        /// Gets a value indicating whether [keep limited vista item stack].
        /// </summary>
        /// <value>
        /// <c>true</c> if [keep limited vista item stack]; otherwise, <c>false</c>.
        /// </value>
        bool KeepLimitedVistaItemsStack
        {
            get;
        }
    }

    /// <summary>
    /// Presents interface for VistaFlipOwner.
    /// </summary>
    public interface IFlipOwner
    {
        #region Properties
        /// <summary>
        /// Gets or sets the flip parent.
        /// </summary>
        /// <value>The flip parent.</value>
        IFlipParent FlipParent
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Starts the flip.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        void StartFlip(KeyEventArgs e);
        
        /// <summary>
        /// Ends the flip.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        void EndFlip(KeyEventArgs e);
        #endregion
    }

    /// <summary>
    /// Presents interface for FlipParent
    /// </summary>
    public interface IFlipParent : IInputElement
    {
        /// <summary>
        /// Gets the flip items.
        /// </summary>
        /// <value>The flip items.</value>
        IList FlipItems
        {
            get;
        }
        
        /// <summary>
        /// Gets a value indicating whether this instance can parent switch.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can parent switch; otherwise, <c>false</c>.
        /// </value>
        bool CanParentSwitch
        {
            get;
        }
        
        /// <summary>
        /// Selects the item.
        /// </summary>
        /// <param name="item">The item Flip input.</param>
        void SelectItem(object item);
    }

    /// <summary>
    /// Presents interface owner of VS2005 Flip.
    /// </summary>
    public interface IVS2005FlipOwner
    {
        /// <summary>
        /// Gets or sets the tool windows list.
        /// </summary>
        /// <value>The tool windows list.</value>
        ObservableFrameworkElements ToolWindowsList
        {
            get;
            set;
        }
        
        /// <summary>
        /// Closes the preview.
        /// </summary>
        void ClosePreview();
        
        /// <summary>
        /// Called when [tool windows item selected].
        /// </summary>
        /// <param name="item">The item Flip owner.</param>
        void OnToolWindowsItemSelected(object item);
    }
}